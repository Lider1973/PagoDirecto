Imports System
Imports System.Net
Imports System.Net.Sockets
Imports System.Threading
Imports System.Messaging
Imports System.IO
Imports System.Reflection
Imports System.Configuration


Module ServiSwitch_ModuleSocketComms

    Dim SID As New ServiSwitch_ReceiverPackageTCP

    Public hostadd As IPHostEntry
    Public EPhost As IPEndPoint

    Public Const DISPLAY_ORDER As Int16 = 0
    Public Const COMMAND_ORDER As Int16 = 1
    Public Const SOCKET_UNKNOW As Int16 = 0
    Public Const SOCKET_LISTEN As Int16 = 1
    Public Const SOCKET_CONNECTED As Int16 = 2
    Public Const CLIENT_MODE As Byte = 1
    Public SharedM_Socket_Mode As Byte
    Public Package As Int32

    Const MAX_SIZE As Integer = 4096
    Const STATUS_CONNECTED As String = "CONNECTED"
    Const STATUS_WAITING As String = "WAITING"
    Const STATUS_DISCONNECTED As String = "DISCONNECTED"

    Dim RECEIVER As New ServiSwitch_ReceiverPackageTCP

    Dim TcpIpMessageQueue As String = String.Empty
    Dim MessageQueueName As String = String.Empty
    Const EXTERNAL_DISCONNECT As Int32 = 10054
    Const USER_DISCONNECT As Int32 = 10004
    Const REQUEST As Int16 = 1001
    Const REPLY As Int16 = 1002

    'Dim ModComms As Byte
    '**************************************
    Dim client As TcpClient
    Dim stream As NetworkStream
    '**************************************
    Dim LocalAddr As IPAddress
    Dim ServerConsole As TcpListener
    '**************************************
    Dim LocalOwname As String

    Dim Status_SERVER As String = "DISCONNECTED"
    Dim Status_CLIENT As String = "DISCONNECTED"
    Dim Thread_Connection As Thread

    Public Sub Init_Thread_Comms_Socket(ByVal MyName As String)
        SaveLogMain("Staring Init_Thread_Comms_Socket Client")

        RECEIVER.Init_Receive_Package_Process()

        Select Case Mod_Comms
            Case MODULE_SERVER
                SharedM_Socket_Mode = Mod_Comms
                ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf Init_Comms_Server_Task))
            Case MODULE_CLIENT
                SharedM_Socket_Mode = Mod_Comms
                ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf Init_Comms_Client_Task))
            Case Else
        End Select

    End Sub

    Private Sub Init_Comms_Client_Task()
        Dim ErrorMessage As String = Nothing
        Dim ErrorCode As Int16 = 0
        Dim LongName As String = String.Empty

        If LongName = "XXXXXXXXXX" Then
            Exit Sub
        End If

Try_Again:
        If Not Client_Establish_Connection(Mod_AddrNumber, Mod_PortNumber) Then
            Thread.Sleep(10000)
            GoTo Try_Again
        End If

        If IsNothing(Thread_Connection) Then
            Thread_Connection = New Thread(AddressOf Get_Status_Client_Mode)
            Thread_Connection.Name = "ConnectionThread"
            Thread_Connection.Start()
        End If

        Client_Start_Thread_Receive_Data(Mod_AddrNumber, Mod_PortNumber)

    End Sub

    Public Sub Client_Start_Thread_Receive_Data(ByVal Addr As String, ByVal Port As Int32)
        Dim CommsData As String = Addr & " " & Port

        ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf Receive_Message_Socket_Client), CommsData)

    End Sub

    Public Function Client_Establish_Connection(ByVal addr As String, ByVal port As Int32) As Boolean
        Dim Result As Boolean = False

        Try
            Show_Message_Console(MyName & " Trying connection with " & addr & ":" & port, COLOR_BLACK, COLOR_GRAY, 0, TRACE_LOW, 1)
            client = New TcpClient
            client.Connect(addr, port)
            stream = client.GetStream()
            Status_CLIENT = STATUS_CONNECTED
            Notify_Process_Status(MyName & "," & listview_CONTROL & "," & CONNECTED_status)
            Show_Message_Console(MyName & " Connection successful with " & addr & ":" & port, COLOR_GRAY, COLOR_BLUE, 0, TRACE_LOW, 1)
            Return True
        Catch ex1 As SocketException
            Show_Message_Console(MyName & " " & ex1.Message, COLOR_BLACK, COLOR_YELLOW, 0, TRACE_LOW, 1)
            Notify_Process_Status(MyName & "," & listview_CONTROL & "," & UNCONNECTED_status)
            Status_CLIENT = STATUS_DISCONNECTED
            Return False
        Catch ex2 As Exception
            Show_Message_Console(MyName & " " & ex2.Message, COLOR_BLACK, COLOR_YELLOW, 0, TRACE_LOW, 1)
            Notify_Process_Status(MyName & "," & listview_CONTROL & "," & UNCONNECTED_status)
            Status_CLIENT = STATUS_DISCONNECTED
            Return False
        End Try

    End Function

    Public Function Send_TCPIP_Message(ByVal MessageToSend As String) As Byte
        Dim buffer_msg As Byte()
        Dim ErrorCode As Byte
        Dim Length As Integer

        If IsNothing(stream) Then
            Return 1
        End If

        Try
            buffer_msg = System.Text.Encoding.ASCII.GetBytes("  " & MessageToSend)
            Length = MessageToSend.Length + 2
            buffer_msg(0) = Length >> 8
            buffer_msg(1) = Length And 255
            stream.Write(buffer_msg, 0, buffer_msg.Length)
            ErrorCode = 0
            'Show_Message_Console("Msg OUT:" & MessageToSend, COLOR_BLACK, COLOR_DARK_GRAY, 0, TRACE_LOW, 0)
            SaveLogMain(MessageToSend)
        Catch SckExpt As SocketException
            ErrorCode = 1
            SaveLogMain("Exception1:" & SckExpt.Message)
        Catch ex As Exception
            SaveLogMain("Exception2:" & ex.Message)
            ErrorCode = 1
        End Try

        Return ErrorCode

    End Function

    Private Sub Receive_Message_Socket_Client(ByVal CommsData As String)
        Dim ArrayMessage(MAX_SIZE) As Byte
        Dim BufferMessage As String = String.Empty
        Dim PortId As Int16 = 0
        Dim ErrorMessage As String = Nothing
        Dim IpLocalHost As String = Nothing
        Dim SocketLength As Int32
        Dim Times As String = Nothing
        Dim ErrorCode As Int32 = 0
        Dim TypeException As String = Nothing
        Dim Id_Transaction As Int64
        Dim ShortBytes(4096) As [Byte]
        Dim LongBytes(40000) As [Byte]

        Show_Message_Console(MyName & " starting main task client socket", COLOR_BLACK, COLOR_GREEN, 0, TRACE_LOW, 1)
        Dim Parms() As String = CommsData.Split(" ")
        IpLocalHost = Parms(0)
        PortId = Parms(1)
        Package = 0

        'Dim LongName As String
        'LongName = GetClientName(PortId, CLIENT_MODE)
        Dim HowTimes As Int16
        Dim CountPosition As Int32 = 0

New_Cycle_Read:
        Try
            Dim Bytes_Defined(4096) As [Byte]
            Try
                CountPosition = 0
                HowTimes = 0
                stream = client.GetStream()
                Do
                    SocketLength = stream.Read(ShortBytes, 0, ShortBytes.Length)
                    Array.ConstrainedCopy(ShortBytes, 0, LongBytes, CountPosition, SocketLength)
                    CountPosition += SocketLength
                    HowTimes += 1
                    'Show_Message_Console("PackNbr:" & CStr(HowTimes) & "=" & SocketLength & " ", COLOR_BLACK, COLOR_GRAY, 0, TRACE_MEDIUM, 0)
                Loop While stream.DataAvailable
                Times = "T1_" & GetDateTime() & Concatenator
                SocketLength = CountPosition
                SaveLogMain("PackNbr:" & CStr(HowTimes) & "=" & SocketLength)
            Catch ex1 As Exception
                If ex1.InnerException Is Nothing Then
                    'Liberar Memoria
                    'stream.Dispose()
                    ErrorCode = EXTERNAL_DISCONNECT
                    TypeException = "Receiving Method: " & ex1.Message
                    Status_CLIENT = STATUS_DISCONNECTED
                    Notify_Process_Status(MyName & "," & listview_CONTROL & "," & UNCONNECTED_status)
                    SaveLogMain("ERROR 3:" & ex1.Message)
                Else
                    'Liberar Memoria
                    'stream.Dispose()
                    ErrorCode = DirectCast(ex1.InnerException, Net.Sockets.SocketException).ErrorCode
                    TypeException = "Receiving Method: " & ex1.Message
                    Status_CLIENT = STATUS_DISCONNECTED
                    Notify_Process_Status(MyName & "," & listview_CONTROL & "," & UNCONNECTED_status)
                    SaveLogMain("ERROR 4:" & ex1.Message)
                End If
                SocketLength = 0
            End Try
            ErrorCode = 0
            '**************************************************************************
            If (SocketLength = 0) And (ErrorCode = EXTERNAL_DISCONNECT Or ErrorCode = 0) Then
                SaveLogMain("Connection canceled by remote node")
                Show_Message_Console(MyName & " Connection canceled by remote node", COLOR_RED, COLOR_WHITE, 0, TRACE_LOW, 1)
                Status_CLIENT = STATUS_DISCONNECTED
                ErrorCode = EXTERNAL_DISCONNECT
                Notify_Process_Status(MyName & "," & listview_CONTROL & "," & UNCONNECTED_status)
                SaveLogMain("ERROR 5: External Disconnect.....")
                Close_Connections_Client()
                GoTo Main_Exiting
            ElseIf (SocketLength = 0) And (ErrorCode = USER_DISCONNECT) Then
                SaveLogMain("Connection canceled by local operator")
                Show_Message_Console(MyName & " Connection canceled by local operator", COLOR_RED, COLOR_WHITE, 0, TRACE_LOW, 1)
                Status_CLIENT = STATUS_DISCONNECTED
                Notify_Process_Status(MyName & "," & listview_CONTROL & "," & UNCONNECTED_status)
                Close_Connections_Client()
                GoTo Main_Exiting
            End If
            '**************************************************************************
            'Array.Resize(Bytes_Defined, SocketLength)
            Dim FinalBytes(CountPosition - 1) As [Byte]
            Array.Copy(LongBytes, FinalBytes, CountPosition)
            Id_Transaction += 1
            If Id_Transaction > 99999 Then
                Id_Transaction = 1
            End If
            'Show_Message_Console("TCP " & MyName & " longitud recibida :" & SocketLength & " IdMessage:" & Id_Transaction, COLOR_BLACK, COLOR_GRAY, 0, TRACE_MEDIUM, 0)
            PutMessageTCPIP(FinalBytes, Id_Transaction, SocketLength, Times, g_TcpQueue)

            BufferMessage = System.Text.Encoding.ASCII.GetString(FinalBytes)
            SaveLogMain(BufferMessage)

        Catch sk_ex As SocketException
            '**************************************************************************
            'Liberar Memoria
            stream.Dispose()
            ErrorCode = sk_ex.NativeErrorCode
            Status_CLIENT = STATUS_DISCONNECTED
            SaveLogMain("ERROR 6:" & sk_ex.Message)
            GoTo Main_Exiting
            '**************************************************************************
        End Try
        GoTo New_Cycle_Read

Main_Exiting:
        Thread.Sleep(5000)
        Init_Thread_Comms_Socket(LocalOwname)

    End Sub

    Public Function GetSocketStatus(ByVal IdConn As Byte) As Byte
        Dim ErrorCode As Byte

        Try
            If client.Connected Then
                ErrorCode = 0
            End If
        Catch ex As Exception
            ErrorCode = 1
        End Try

        Return ErrorCode
    End Function


    'Public Function CloseClientComms(ByVal IdConn As Byte) As Boolean
    Public Function Close_Connections_Client() As Boolean

        Try
            ServerConsole.Stop()
        Catch ex As Exception
            ServerConsole = Nothing
        End Try

        Try
            client.Close()
            client = Nothing
        Catch ex As Exception
            'DisplayMessage(" Close port:" & ex.Message, 1, 0)
        End Try

        Try
            stream.Dispose()
            stream = Nothing
        Catch ex As Exception
            'DisplayMessage(" Close port:" & ex.Message, 1, 0)
        End Try


        Return True

    End Function


    Public Function Close_Connection_Server() As Int16
        Dim ErrorCode As Int16 = 0

        Try
            LocalAddr = Nothing
            ServerConsole.Stop()
            ServerConsole = Nothing
            client.Close()
            client = Nothing
            ErrorCode = 0
            Show_Message_Console(MyName & " Server Socket has been closed", COLOR_RED, COLOR_WHITE, 0, TRACE_LOW, 1)
        Catch ex As Exception
            ErrorCode = 1
        End Try

        Return ErrorCode

    End Function

    'Private Sub Init_Comms_Server_Task(ByVal CommsData As String)
    Private Sub Init_Comms_Server_Task()
        'Dim PortId As Int16 = 0
        Dim Id_Transaction As Int32
        Dim ErrorMessage As String = Nothing
        'Dim IpLocalHost As String = Nothing
        Dim IpRemote As String = Nothing
        Dim Bytes_Received(0) As [Byte]
        Dim SocketLength As Int32
        Dim BytesLength As Int32
        Dim Flags As String = Nothing
        Dim ErrorCode As Int32 = 0
        Dim InstanceTime As String = Nothing
        Dim TypeException As String = Nothing
        Dim ShortBytes(4096) As [Byte]
        Dim LongBytes(40000) As [Byte]
        Dim Times As String = String.Empty
        'Dim Parms() As String = CommsData.Split(New String() {" "}, StringSplitOptions.RemoveEmptyEntries)

        Package = 0

        If IsNothing(Thread_Connection) Then
            Thread_Connection = New Thread(AddressOf Get_Status_Server_Mode)
            Thread_Connection.Name = "PackageThread"
            Thread_Connection.Start()
        End If

        'IpLocalHost = Parms(0)
        'PortId = Parms(1)

        Notify_Process_Status(MyName & "," & listview_CONTROL & "," & UNCONNECTED_status)
        If (Status_SERVER = STATUS_CONNECTED) Or (Status_SERVER = STATUS_WAITING) Then
            Show_Message_Console(MyName & " Alrready have a client process active", COLOR_BLACK, COLOR_YELLOW, 0, TRACE_LOW, 1)
            Exit Sub
        ElseIf Status_SERVER = STATUS_DISCONNECTED Then
            Show_Message_Console(MyName & " Init server connection with " & Mod_AddrNumber & ":" & Mod_PortNumber.ToString, COLOR_BLACK, COLOR_GREEN, 0, TRACE_LOW, 1)
        End If

New_Cycle_Connect:
        Try
            LocalAddr = IPAddress.Parse(Mod_AddrNumber)
            ServerConsole = New TcpListener(LocalAddr, Mod_PortNumber)
            ServerConsole.Start()
            Status_SERVER = STATUS_WAITING
            '********************************************************************
            client = ServerConsole.AcceptTcpClient()
            Status_SERVER = STATUS_CONNECTED
            '********************************************************************
            Dim pi As PropertyInfo = client.GetStream.GetType.GetProperty("Socket", BindingFlags.NonPublic Or BindingFlags.Instance)
            If Not pi Is Nothing Then
                IpRemote = pi.GetValue(client.GetStream, Nothing).RemoteEndPoint.ToString.Split(":")(0)
            End If
            Notify_Process_Status(MyName & "," & listview_CONTROL & "," & CONNECTED_status)
            Show_Message_Console(MyName & " Connection has been stablished with " & IpRemote, COLOR_GRAY, COLOR_BLUE, 0, TRACE_LOW, 1)
        Catch ex1 As InvalidOperationException
            TypeException = "InvalidOperationException:" & ex1.Message
            Status_SERVER = STATUS_DISCONNECTED
            Notify_Process_Status(MyName & "," & listview_CONTROL & "," & UNCONNECTED_status)
            Show_Message_Console(MyName & " " & ex1.Message, COLOR_RED, COLOR_WHITE, 0, TRACE_LOW, 1)
            GoTo Main_Exiting
        Catch ex2 As ApplicationException
            TypeException = "ApplicationException:" & ex2.Message
            Status_SERVER = STATUS_DISCONNECTED
            Notify_Process_Status(MyName & "," & listview_CONTROL & "," & UNCONNECTED_status)
            Show_Message_Console(MyName & " " & ex2.Message, COLOR_RED, COLOR_WHITE, 0, TRACE_LOW, 1)
            GoTo Main_Exiting
        Catch ex3 As ArgumentNullException
            TypeException = "ArgumentNullException:" & ex3.Message
            Status_SERVER = STATUS_DISCONNECTED
            Notify_Process_Status(MyName & "," & listview_CONTROL & "," & UNCONNECTED_status)
            Show_Message_Console(MyName & " " & ex3.Message, COLOR_RED, COLOR_WHITE, 0, TRACE_LOW, 1)
            GoTo Main_Exiting
        Catch ex4 As SocketException
            TypeException = "SocketException:" & ex4.Message
            Status_SERVER = STATUS_DISCONNECTED
            Notify_Process_Status(MyName & "," & listview_CONTROL & "," & UNCONNECTED_status)
            Show_Message_Console(MyName & " " & ex4.Message, COLOR_RED, COLOR_WHITE, 0, TRACE_LOW, 1)
            GoTo Main_Exiting
        Catch ex5 As Messaging.MessageQueueException
            TypeException = "Messaging.MessageQueueException" & ex5.Message
            Status_SERVER = STATUS_DISCONNECTED
            Notify_Process_Status(MyName & "," & listview_CONTROL & "," & UNCONNECTED_status)
            Show_Message_Console(MyName & " " & ex5.Message, COLOR_RED, COLOR_WHITE, 0, TRACE_LOW, 1)
            GoTo Main_Exiting
        Catch ex As Exception
            Status_SERVER = STATUS_DISCONNECTED
            ErrorCode = USER_DISCONNECT
            GoTo Main_Exiting
        End Try

        Dim HowTimes As Int16
        Dim CountPosition As Int32 = 0

New_Cycle_Read:
        '******************************************************************************
        Try
            Try
                CountPosition = 0
                HowTimes = 0
                stream = client.GetStream()
                Do
                    SocketLength = stream.Read(ShortBytes, 0, ShortBytes.Length)
                    Array.ConstrainedCopy(ShortBytes, 0, LongBytes, CountPosition, SocketLength)
                    CountPosition += SocketLength
                    HowTimes += 1
                Loop While stream.DataAvailable
                SocketLength = CountPosition
                Times = "T1_" & GetDateTime() & Concatenator
                'Show_Message_Console(MyName & " Receive on Socket:" & SocketLength, COLOR_BLACK, COLOR_YELLOW, 0, TRACE_LOW, 1)
            Catch ex0 As SocketException
                If ex0.InnerException Is Nothing Then
                    ErrorCode = EXTERNAL_DISCONNECT
                    TypeException = "Receiving Method: " & ex0.Message
                    Status_SERVER = STATUS_DISCONNECTED
                    Notify_Process_Status(MyName & "," & listview_CONTROL & "," & UNCONNECTED_status)
                    Show_Message_Console(MyName & " " & ex0.Message, COLOR_RED, COLOR_WHITE, 0, TRACE_LOW, 1)
                Else
                    ErrorCode = DirectCast(ex0.InnerException, Net.Sockets.SocketException).ErrorCode
                    TypeException = "Receiving Method: " & ex0.Message
                    Status_SERVER = STATUS_DISCONNECTED
                    Notify_Process_Status(MyName & "," & listview_CONTROL & "," & UNCONNECTED_status)
                    Show_Message_Console(MyName & " " & ex0.Message, COLOR_RED, COLOR_WHITE, 0, TRACE_LOW, 1)
                End If
                SocketLength = 0
            Catch ex1 As Exception
                If ex1.InnerException Is Nothing Then
                    ErrorCode = EXTERNAL_DISCONNECT
                    TypeException = "Receiving Method: " & ex1.Message
                    Status_SERVER = STATUS_DISCONNECTED
                    Notify_Process_Status(MyName & "," & listview_CONTROL & "," & UNCONNECTED_status)
                Else
                    ErrorCode = DirectCast(ex1.InnerException, Net.Sockets.SocketException).ErrorCode
                    TypeException = "Receiving Method: " & ex1.Message
                    Status_SERVER = STATUS_DISCONNECTED
                    Notify_Process_Status(MyName & "," & listview_CONTROL & "," & UNCONNECTED_status)
                End If
                Show_Message_Console(MyName & " " & ex1.Message, COLOR_RED, COLOR_WHITE, 0, TRACE_LOW, 1)
                SocketLength = 0
            End Try
            '**************************************************************************
            If (SocketLength = 0) And (ErrorCode = EXTERNAL_DISCONNECT Or ErrorCode = 0) Then
                Show_Message_Console(MyName & " Connection has been canceled by remote node", COLOR_RED, COLOR_WHITE, 0, TRACE_LOW, 1)
                Status_SERVER = STATUS_DISCONNECTED
                Notify_Process_Status(MyName & "," & listview_CONTROL & "," & UNCONNECTED_status)
                Close_Connection_Server()
                ErrorCode = EXTERNAL_DISCONNECT
                GoTo Main_Exiting
            ElseIf (SocketLength = 0) And (ErrorCode = USER_DISCONNECT) Then
                Show_Message_Console(MyName & " Connection canceled by local operator", COLOR_RED, COLOR_WHITE, 0, TRACE_LOW, 1)
                Status_SERVER = STATUS_DISCONNECTED
                Close_Connection_Server()
                Notify_Process_Status(MyName & "," & listview_CONTROL & "," & UNCONNECTED_status)
                GoTo Main_Exiting
            End If
            '*********************************************************************************************************************************
            Dim BufferMessage As String = ""
            Dim FinalBytes(CountPosition - 1) As [Byte]
            Array.Copy(LongBytes, FinalBytes, CountPosition)
            If FinalBytes.Length >= 2 Then
                BytesLength = (FinalBytes(0) * 256) + FinalBytes(1)
            Else
                Show_Message_Console(" Invalid data Received:" & BufferMessage, COLOR_BLACK, COLOR_YELLOW, 0, TRACE_LOW, 0)
                GoTo New_Cycle_Read
            End If
            BufferMessage = System.Text.Encoding.ASCII.GetString(FinalBytes)
            SaveLogMain(BufferMessage)
            '*********************************************************************************************************************************
            'Show_Message_Console("TCP " & MyName & " longitud recibida :" & SocketLength & " IdMessage:" & Id_Transaction, COLOR_BLACK, COLOR_GRAY, 0, TRACE_MEDIUM, 0)
            PutMessageTCPIP(FinalBytes, Id_Transaction, SocketLength, Times, g_TcpQueue)
        Catch sk_ex As SocketException
            '**************************************************************************
            ErrorCode = sk_ex.NativeErrorCode
            Show_Message_Console(MyName & " Connection has been canceled ", COLOR_RED, COLOR_WHITE, 0, TRACE_LOW, 1)
            Status_SERVER = STATUS_DISCONNECTED
            Notify_Process_Status(MyName & "," & listview_CONTROL & "," & UNCONNECTED_status)
            Close_Connection_Server()
            GoTo Main_Exiting
            '**************************************************************************
        End Try
        GoTo New_Cycle_Read

Main_Exiting:
        Thread.Sleep(5000)
        Init_Thread_Comms_Socket(LocalOwname)

    End Sub


    Private Sub Get_Status_Server_Mode()
        Dim LastStatus As Byte

Looping:
        Try
            If (Status_SERVER = STATUS_DISCONNECTED) Or (Status_SERVER = STATUS_WAITING) Then
                If LastStatus <> UNCONNECTED_status Then
                    Notify_Process_Status(MyName & "," & listview_CONTROL & "," & UNCONNECTED_status)
                    LastStatus = UNCONNECTED_status
                End If
            ElseIf (Status_SERVER = STATUS_CONNECTED) Then
                If LastStatus <> CONNECTED_status Then
                    Notify_Process_Status(MyName & "," & listview_CONTROL & "," & CONNECTED_status)
                    LastStatus = CONNECTED_status
                End If
            End If
            Thread.Sleep(5000)
            GoTo Looping
        Catch ex As Exception
            Show_Message_Console(MyName & " Can't get the server status ", COLOR_RED, COLOR_WHITE, 0, TRACE_LOW, 1)
        End Try

    End Sub


    Private Sub Get_Status_Client_Mode()
Looping:
        Try
            If (Status_CLIENT = STATUS_DISCONNECTED) Or (Status_CLIENT = STATUS_WAITING) Then
                Notify_Process_Status(MyName & "," & listview_CONTROL & "," & UNCONNECTED_status)
            ElseIf (Status_CLIENT = STATUS_CONNECTED) And (Exiting = False) Then
                Notify_Process_Status(MyName & "," & listview_CONTROL & "," & CONNECTED_status)
            End If
            Thread.Sleep(5000)
            GoTo Looping
        Catch ex As Exception
            Show_Message_Console(MyName & " Can't get the server status ", COLOR_RED, COLOR_WHITE, 0, TRACE_LOW, 1)
        End Try

    End Sub


    Public Sub PutMessageTCPIP(ByVal ByteData() As Byte, ByVal IdTransaction As Int64, ByVal Total_Length As Int32, ByVal Times As String, ByVal QueueName As String)

        Try
            Dim MessageToSend As Message = New Message
            Dim QueueSendData As New MessageQueue(QueueName)
            Dim QUEUE_MESSAGE_TCPIP As New InfoFromSocket
            QUEUE_MESSAGE_TCPIP.SMB_Times += Times & "T2_" & GetDateTime() & Concatenator
            QUEUE_MESSAGE_TCPIP.SMB_TotLength = Total_Length
            QUEUE_MESSAGE_TCPIP.SMB_BytesMessage = ByteData
            QUEUE_MESSAGE_TCPIP.SMB_Package_Nbr = IdTransaction
            MessageToSend.Body = QUEUE_MESSAGE_TCPIP
            QueueSendData.Send(MessageToSend)
            QUEUE_MESSAGE_TCPIP = Nothing
        Catch ex As Exception
            Show_Message_Console(MyName & " Can't send the tcpip message (queue) :" & ex.Message & "-" & QueueName, COLOR_RED, COLOR_WHITE, 0, TRACE_LOW, 1)
        End Try

    End Sub

    Public Function Get_Socket_Status() As Byte
        Dim Status As Byte

        If Mod_Comms = MODULE_SERVER Then
            If Status_SERVER = STATUS_CONNECTED Then
                Status = CONNECTED_status
            Else
                Status = UNCONNECTED_status
            End If
        ElseIf Mod_Comms = MODULE_CLIENT Then
            If Status_CLIENT = STATUS_CONNECTED Then
                Status = CONNECTED_status
            Else
                Status = UNCONNECTED_status
            End If
        End If
        Return Status
    End Function

End Module
