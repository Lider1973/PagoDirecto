Imports System.Messaging
Imports System.Threading
Imports System.IO

Public Class ServiSwitch_ReceiverPackageTCP

    Const CLIENT_MODE As Byte = 1
    Const ROUTER_NAME As String = "ROUTERS"
    Const LOGGER_NAME As String = "LOGGER"

    Dim ThreadTemporal As Thread
    Dim MessageQueueName As String
    Dim IncomingTcpipQueueName As String
    Dim OutgoingTcpipQueueName As String

    Dim contador As Int16
    Dim PackageThread As Thread

    Public Sub Init_Receive_Package_Process()
        '********************************************
        '********************************************
        If Not IsNothing(PackageThread) Then
            Exit Sub
        End If

        PackageThread = New Thread(AddressOf Main_Receiver_Package)
        PackageThread.Name = "PackageThread"
        PackageThread.Priority = ThreadPriority.Highest
        PackageThread.Start()
        '********************************************
        '********************************************
    End Sub

    Public Sub Main_Receiver_Package()
        Dim length As Integer = 0, rc As Integer = 0
        Dim Position As Integer = 0
        Dim lengthTemp As Integer = 0
        Dim InitMessage As String = "Begin thread "
        Dim BufferMessage As String = Nothing
        Dim Total_Length As Int16
        Dim LengthReceived As Integer = 0
        Dim BytesMessage() As Byte
        Dim BufferPackage As Byte() = New Byte(20480) {}
        Dim NewBufferPackage As Byte() = New Byte(20480) {}
        Dim LengthByte(4) As Byte

        Show_Message_Console(MyName & " Begin main package task....", COLOR_BLACK, COLOR_GREEN, 0, TRACE_LOW, 1)
        Console.WriteLine("INIT PACKAGE ROUTINE WITH:" & g_TcpQueue)

        While True
            If Package > 999999 Then
                Package = 1
            End If
            Try
                Dim TS_MessageQueue As New MessageQueue(g_TcpQueue)
                TS_MessageQueue.Formatter = New XmlMessageFormatter(New Type() {GetType(InfoFromSocket)})
                Dim myMessage As Message = TS_MessageQueue.Receive
                Dim Struct_Queue_Message As InfoFromSocket = CType(myMessage.Body, InfoFromSocket)
                Total_Length = Struct_Queue_Message.SMB_TotLength
                BytesMessage = Struct_Queue_Message.SMB_BytesMessage
                Struct_Queue_Message.SMB_Times += "T3_" & GetDateTime() & Concatenator
                Array.Copy(BytesMessage, 0, BufferPackage, Position, Total_Length)
                LengthReceived = Total_Length + Position
                While True
                    length = (BufferPackage(0) * 256) + BufferPackage(1)

                    If length = LengthReceived Then
                        lengthTemp = length - 2
                        BytesMessage = New Byte(lengthTemp - 1) {}
                        Array.Copy(BufferPackage, 2, BytesMessage, 0, lengthTemp)
                        BufferMessage = System.Text.Encoding.ASCII.GetString(BytesMessage)
                        LengthReceived = 0
                        Position = 0
                        BufferPackage = New Byte(20480) {}
                        NewBufferPackage = New Byte(20480) {}
                        Package += 1
                        Put_Message_Socket_Request(BytesMessage, Package, Total_Length, length, Struct_Queue_Message.SMB_Times, g_AckQueue)
                        Exit While
                    End If
                    If length < LengthReceived Then
                        lengthTemp = length - 2
                        BytesMessage = New Byte(lengthTemp - 1) {}
                        Array.Copy(BufferPackage, 2, BytesMessage, 0, lengthTemp)
                        Console.WriteLine(" Twin package  Tot:" & length & " Rec:" & LengthReceived)
                        SaveLogMain("Twin package  Tot:" & length & " Rec:" & LengthReceived)
                        Package += 1
                        Put_Message_Socket_Request(BytesMessage, Package, lengthTemp, length, Struct_Queue_Message.SMB_Times, g_AckQueue)
                        LengthReceived = LengthReceived - length
                        Array.Copy(BufferPackage, length, NewBufferPackage, 0, LengthReceived)
                        BufferPackage = New Byte(20480) {}
                        NewBufferPackage.CopyTo(BufferPackage, 0)
                        NewBufferPackage = New Byte(20480) {}
                        Continue While
                    End If
                    If length > LengthReceived Then
                        Position = LengthReceived
                        Exit While
                    End If
                End While
            Catch ex As MessageQueueException
                SaveLogMain("ERROR 1:|" & g_TcpQueue & "|" & ex.Message)
                Console.WriteLine("--------------------- " & g_TcpQueue)
                Exit While
            Catch ex As Exception
                SaveLogMain("ERROR 2:" & ex.Message)
                Console.WriteLine("--------------------- " & g_TcpQueue)
                Continue While
            Finally
            End Try
        End While
    End Sub

    Private Sub Put_Message_Socket_Request(ByVal ByteData() As Byte, ByVal IdTransaction As Int64, ByVal Total_Length As Int32, ByVal ByteLen As Int32, ByVal Times As String, ByVal QueueName As String)
        Try
            Dim MessageToSend As Message = New Message
            Dim QueueSendData As New MessageQueue(QueueName)
            Dim QUEUE_MESSAGE_BYTES As New InfoFromSocket
            QUEUE_MESSAGE_BYTES.SMB_BytesMessage = ByteData
            QUEUE_MESSAGE_BYTES.SMB_TotLength = Total_Length
            QUEUE_MESSAGE_BYTES.SMB_ByteLength = ByteLen
            QUEUE_MESSAGE_BYTES.SMB_Package_Nbr = IdTransaction
            QUEUE_MESSAGE_BYTES.SMB_Times += Times & "T4_" & GetDateTime() & Concatenator
            MessageToSend.Body = QUEUE_MESSAGE_BYTES
            QueueSendData.Send(MessageToSend)
            QUEUE_MESSAGE_BYTES = Nothing
        Catch ex As Exception
            Console.WriteLine("Put_Message_Socket_Request excepcion " & ex.Message)
        End Try
    End Sub

End Class
