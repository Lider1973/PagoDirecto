Imports System.Threading
Imports System.Messaging
Imports System.Runtime.InteropServices
Imports System.Drawing
Imports System.Globalization
Imports System.Threading.Tasks

Public Class ServiSwitch_ProcessMessages

    Dim ReqType() As String = {"0800", "0200", "0420"}
    Dim RepType() As String = {"0810", "0210", "0430"}
    Private Const ISO8583 As Char = "I"
    Private Const HOST2 As Char = "H"
    Private Const UNKNOW As Char = "U"
    Private Const XML As Char = "X"
    Private Const COMMAND As Char = "C"
    Private Const WAITING As Byte = 0
    Private Const ALLOWED_NOTIFY As Int16 = 2000
    Private Const TIMEOUT_NOTIFY As Int16 = 10000
    Private Const WAY_REQUEST As Byte = 0
    Private Const WAY_REPLY As Byte = 1

    Private Last_IN_Message As DateTime

    Public Sub Init_Task_Process_Request(ByVal TaskNumbers As Int32)

        ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf RunStartUpThreads), TaskNumbers)
    End Sub

    Private Sub RunStartUpThreads(ByVal TaskNumbers As Int32)

        'ReDim Thread_MRRQ(TaskNumbers)
        'ReDim Thread_MRRP(TaskNumbers)
        'ReDim Thread_MRLP(TaskNumbers)

        'For x = 0 To TaskNumbers
        '***************************************************************
        Thread_MRRQ = New Thread(AddressOf Main_Receiver_Socket_Info)
        Thread_MRRQ.Name = "RQtask_1"
        Thread_MRRQ.Start()

        Thread_MRRP = New Thread(AddressOf Main_Receiver_Requests)
        Thread_MRRP.Name = "RPtask_1"
        Thread_MRRP.Start()

        Thread_MRLP = New Thread(AddressOf Main_Receiver_Replies)
        Thread_MRLP.Name = "RPtask_1"
        Thread_MRLP.Start()
        '***************************************************************
        'Next

        ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf Evaluate_Status_Control_Message))

    End Sub

    Public Sub ProcessStartTask(ByVal CommandLine As String)
        Dim Parms() As String = CommandLine.Split("|")
        Dim IdTh As Byte = Parms(2).Substring(Parms(2).IndexOf("_") + 1)

        Try
            Select Case Parms(4)
                Case "request"
                    Thread_MRRQ = New Thread(AddressOf Main_Receiver_Socket_Info)
                    Thread_MRRQ.Name = "RQtask_1"
                    Thread_MRRQ.Start()
                Case "reply"
                    Thread_MRRP = New Thread(AddressOf Main_Receiver_Requests)
                    Thread_MRRP.Name = "RPtask_1"
                    Thread_MRRP.Start()
            End Select
        Catch ex As Exception
            Show_Message_Console(MyName & " Exception starting Thread:" & ex.Message, COLOR_OWNER1, COLOR_RED, 0, TRACE_LOW, 1)
        End Try

    End Sub

    Private Sub Main_Receiver_Socket_Info()
        Dim BytesMessage() As Byte
        Dim TotalLength As Int32
        Dim PackageNbr As Int32
        Dim EndTask As Boolean = False
        Dim Id As Int16 = Thread.CurrentThread.Name.ToString.Substring(Thread.CurrentThread.Name.ToString.IndexOf("_") + 1)
        Dim Legend As String = MyName & " Init Process SOCKET,  " & Thread.CurrentThread.Name & ":" & System.Threading.Thread.CurrentThread.ManagedThreadId & ":" & Thread.CurrentThread.Name
        Show_Message_Console(Legend, COLOR_BLACK, COLOR_GRAY, 0, TRACE_LOW, 1)
        '******************************************************************************** 
        Dim CurrentThread As Thread = Thread.CurrentThread
        CurrentThread.Priority = ThreadPriority.BelowNormal
        '********************************************************************************
        Dim filter = New MessagePropertyFilter()
        With filter
            .AdministrationQueue = False
            .ArrivedTime = True
            .CorrelationId = False
            .Priority = False
            .ResponseQueue = False
            .SentTime = True
            .Body = True
            .Label = True
            .Id = True
        End With
        '********************************************************************************
        Dim TS_MessageQueue As New MessageQueue(g_AckQueue)
        TS_MessageQueue.Formatter = New XmlMessageFormatter(New Type() {GetType(InfoFromSocket)})
        TS_MessageQueue.MessageReadPropertyFilter = filter
        '********************************************************************************
        Do While True
            Try
                Dim myMessage As Message = TS_MessageQueue.Receive
                Dim Struct_Queue_Message As InfoFromSocket = CType(myMessage.Body, InfoFromSocket)
                BytesMessage = Struct_Queue_Message.SMB_BytesMessage
                TotalLength = Struct_Queue_Message.SMB_TotLength
                PackageNbr = Struct_Queue_Message.SMB_Package_Nbr
                Struct_Queue_Message.SMB_Times += "T5_" & GetDateTime() & Concatenator
                '********************************************************************************
                Dim DiffQueueTime = DateDiff(DateInterval.Second, myMessage.ArrivedTime, DateTime.Now)
                If DiffQueueTime > 10 Then
                    Console.Write("*")
                    DiffQueueTime = Nothing
                    Continue Do
                End If
                '********************************************************************************
                '********************************************************************************

                '____PROCESS_EVALUATE_REQUIREMENT_FROM_SOCKET___(Struct_Queue_Message, Id, PackageNbr)
                ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf ____PROCESS_EVALUATE_REQUIREMENT_FROM_SOCKET___), Struct_Queue_Message)

                '********************************************************************************
                myMessage = Nothing
                Struct_Queue_Message = Nothing
                ClearMemory 
                '********************************************************************************
            Catch ta As ThreadAbortException
                If Exiting = True Then
                    GoTo Exiting_Thread
                End If
                Console.WriteLine(MyName & " Thread - caught ThreadAbortException - resetting.")
                Console.WriteLine(MyName & " Exception message: {0}", ta.Message)
                Exit Sub
            Catch ex As Exception
                If Exiting = True Then
                    GoTo Exiting_Thread
                End If
                Show_Message_Console(MyName & " Exception in listen request: " & ex.Message & " - " & ex.StackTrace, COLOR_OWNER1, COLOR_RED, 0, TRACE_LOW, 1)
                GoTo Exiting_Thread
            End Try
        Loop
        '********************************************************************************
Exiting_Thread:
        Show_Message_Console(MyName & " Ending request " & Id, COLOR_BLACK, COLOR_GRAY, 0, TRACE_LOW, 1)
        Thread.Sleep(5000)
        If Exiting = False Then
            Thread_MRRQ = New Thread(AddressOf Main_Receiver_Socket_Info)
            Thread_MRRQ.Name = "RQtask_1"
            Thread_MRRQ.Start()
        End If

    End Sub

    Private Sub Main_Receiver_Requests()
        'Dim EndTask As Boolean
        Dim Id As Int16 = Thread.CurrentThread.Name.ToString.Substring(Thread.CurrentThread.Name.ToString.IndexOf("_") + 1)
        Dim Legend As String = MyName & " Init process REQUEST,  " & Thread.CurrentThread.Name & ":" & System.Threading.Thread.CurrentThread.ManagedThreadId & ":" & Id
        Show_Message_Console(Legend, COLOR_BLACK, COLOR_GRAY, 0, TRACE_LOW, 1)
        '******************************************************************************** 
        Dim CurrentThread As Thread = Thread.CurrentThread
        CurrentThread.Priority = ThreadPriority.BelowNormal
        '********************************************************************************
        Dim filter = New MessagePropertyFilter()
        With filter
            .AdministrationQueue = False
            .ArrivedTime = True
            .CorrelationId = False
            .Priority = False
            .ResponseQueue = False
            .SentTime = True
            .Body = True
            .Label = True
            .Id = True
        End With
        '********************************************************************************
        Dim TS_MessageQueue As New MessageQueue(g_RequestQueue)
        TS_MessageQueue.Formatter = New XmlMessageFormatter(New Type() {GetType(SharedStructureMessage)})
        TS_MessageQueue.MessageReadPropertyFilter = filter
        '********************************************************************************
        Do While True
            Try
                Dim myMessage As Message = TS_MessageQueue.Receive
                Dim Struct_Queue_Message As SharedStructureMessage = CType(myMessage.Body, SharedStructureMessage)
                '********************************************************************************
                Dim DiffQueueTime = DateDiff(DateInterval.Second, myMessage.ArrivedTime, DateTime.Now)
                If DiffQueueTime > 10 Then
                    Console.Write("*")
                    DiffQueueTime = Nothing
                    Continue Do
                End If

                Struct_Queue_Message.SSM_Instance_Times += "T5_" & GetDateTime() & Concatenator
                '********************************************************************************

                ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf ___PROCESS_EVALUATE_INTERNAL_REQUIREMENTS___), Struct_Queue_Message)

                '********************************************************************************
                myMessage = Nothing
                Struct_Queue_Message = Nothing
                ClearMemory()
                '********************************************************************************
            Catch ta As ThreadAbortException
                If Exiting = True Then
                    GoTo Exiting_Thread
                End If
                Show_Message_Console(MyName & " ThreadException: " & ta.Message, COLOR_OWNER1, COLOR_RED, 0, TRACE_LOW, 1)
                GoTo Exiting_Thread
            Catch ex As Exception
                If Exiting = True Then
                    GoTo Exiting_Thread
                End If
                Show_Message_Console(MyName & " Exception in listen replies: " & ex.Message, COLOR_OWNER1, COLOR_RED, 0, TRACE_LOW, 1)
                GoTo Exiting_Thread
            End Try
        Loop
        '********************************************************************************
Exiting_Thread:
        Show_Message_Console("Replies ending task:" & Id, COLOR_BLACK, COLOR_GRAY, 0, TRACE_LOW, 1)
        'Thread.Sleep(1000)
        If Exiting = False Then
            Thread_MRRP = New Thread(AddressOf Main_Receiver_Requests)
            Thread_MRRP.Name = "RPtask_1"
            Thread_MRRP.Start()
        End If

    End Sub


    Private Sub Main_Receiver_Replies()
        Dim Id As Int16 = Thread.CurrentThread.Name.ToString.Substring(Thread.CurrentThread.Name.ToString.IndexOf("_") + 1)
        Dim Legend As String = MyName & " Init process REPLIES,  " & Thread.CurrentThread.Name & ":" & System.Threading.Thread.CurrentThread.ManagedThreadId & ":" & Id
        Show_Message_Console(Legend, COLOR_BLACK, COLOR_GRAY, 0, TRACE_LOW, 1)
        '********************************************************************************
        Dim CurrentThread As Thread = Thread.CurrentThread
        CurrentThread.Priority = ThreadPriority.BelowNormal
        '********************************************************************************
        Dim filter = New MessagePropertyFilter()
        With filter
            .AdministrationQueue = False
            .ArrivedTime = True
            .CorrelationId = False
            .Priority = False
            .ResponseQueue = False
            .SentTime = True
            .Body = True
            .Label = True
            .Id = True
        End With
        '********************************************************************************
        Dim TS_MessageQueue As New MessageQueue(g_ReplyQueue)
        TS_MessageQueue.Formatter = New XmlMessageFormatter(New Type() {GetType(SharedStructureMessage)})
        TS_MessageQueue.MessageReadPropertyFilter = filter
        '********************************************************************************
        Do While True
            Try
                Dim myMessage As Message = TS_MessageQueue.Receive
                Dim Struct_Queue_Message As SharedStructureMessage = CType(myMessage.Body, SharedStructureMessage)
                '********************************************************************************
                '********************************************************************************
                Dim DiffQueueTime = DateDiff(DateInterval.Second, myMessage.ArrivedTime, DateTime.Now)
                If DiffQueueTime > 10 Then
                    Console.Write("*")
                    DiffQueueTime = Nothing
                    Continue Do
                End If

                Struct_Queue_Message.SSM_Instance_Times += "T15_" & GetDateTime() & Concatenator
                '********************************************************************************

                '___PROCESS_EVALUATE_INTERNAL_REQUIREMENTS___(Struct_Queue_Message, Id, False)
                ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf ___PROCESS_EVALUATE_INTERNAL_REQUIREMENTS___), Struct_Queue_Message)

                myMessage = Nothing
                Struct_Queue_Message = Nothing
                ClearMemory()
                '********************************************************************************
            Catch ta As ThreadAbortException
                If Exiting = True Then
                    GoTo Exiting_Thread
                End If
                Show_Message_Console(MyName & " ThreadException: " & ta.Message, COLOR_OWNER1, COLOR_RED, 0, TRACE_LOW, 1)
                GoTo Exiting_Thread
            Catch ex As Exception
                If Exiting = True Then
                    GoTo Exiting_Thread
                End If
                Show_Message_Console(MyName & " Exception in listen replies: " & ex.Message, COLOR_OWNER1, COLOR_RED, 0, TRACE_LOW, 1)
                GoTo Exiting_Thread
            End Try
        Loop
        '********************************************************************************
Exiting_Thread:
        Show_Message_Console("Replies ending task:" & Id, COLOR_BLACK, COLOR_GRAY, 0, TRACE_LOW, 1)
        'Thread.Sleep(1000)
        If Exiting = False Then
            Thread_MRLP = New Thread(AddressOf Main_Receiver_Requests)
            Thread_MRLP.Name = "RPLtask_1"
            Thread_MRLP.Start()
        End If

    End Sub

    'Private Sub ____PROCESS_EVALUATE_REQUIREMENT_FROM_SOCKET___(ByVal Struct_Queue_Message As InfoFromSocket, ByVal IdTask As Int16, ByVal Package As Int64)
    Private Sub ____PROCESS_EVALUATE_REQUIREMENT_FROM_SOCKET___(ByVal Struct_Queue_Message As InfoFromSocket)
        Dim MessageType As String = String.Empty
        Dim TransactionCode As Int16
        Dim FiMessage As Int16
        Dim IdTask As Int16 = 0
        Dim Package As Int64 = 0

        Dim BufferMessage As String = System.Text.Encoding.ASCII.GetString(Struct_Queue_Message.SMB_BytesMessage)
        'SaveLogMain(BufferMessage)
        'Show_Message_Console("Msg IN:" & BufferMessage, COLOR_BLACK, COLOR_DARK_GRAY, 0, TRACE_LOW, 0)

        MessageType = BufferMessage.Substring(0, 2)
        TransactionCode = BufferMessage.Substring(21, 4)
        FiMessage = BufferMessage.Substring(2, 4)

        Select Case MessageType
            Case "TR", "TC", "XR", "XC", "AR"
                Select Case FiMessage
                    Case FI_BANRED
                        Select Case TransactionCode
                            Case CONTROL_MESSAGE, ECHO_TEST
                                Process_Local_Control_Message(BufferMessage, IdTask, GetDateTime)
                        End Select
                    Case FI_CPN_ADQ
                        Select Case TransactionCode
                            Case SAVING_DIRECT_INQUIRY, SAVING_DIRECT_TRANSFER, CHECKING_DIRECT_TRANSFER,
                                 CHECKING_DIRECT_TRANSFER, CHECKING_DIRECT_REVERSE,
                                 CREDIT_DIRECT_TRANSFER, CREDIT_DIRECT_REVERSE
                                Process_Pending_Adquirence_Transaction(BufferMessage, IdTask, Struct_Queue_Message.SMB_Times)
                            Case Else
                                Show_Message_Console("Transaccion no disponible..." & BufferMessage.Substring(0, 40), COLOR_BLACK, COLOR_RED, 0, TRACE_LOW, 0)
                        End Select
                    Case FI_CPN_AUT
                        Select Case TransactionCode
                            Case CONTROL_MESSAGE
                                Process_Local_Control_Message(BufferMessage, IdTask, GetDateTime)
                            Case SAVING_DIRECT_INQUIRY, SAVING_DIRECT_TRANSFER, SAVING_DIRECT_REVERSE
                                Process_Local_Auth_Transaction(BufferMessage, IdTask, Struct_Queue_Message.SMB_Times)
                            Case CHECKING_DIRECT_TRANSFER, CREDIT_DIRECT_TRANSFER
                                Process_Local_Deny_Transaction(BufferMessage)
                            Case Else
                                Show_Message_Console("Transaccion no disponible..." & BufferMessage.Substring(0, 40), COLOR_BLACK, COLOR_RED, 0, TRACE_LOW, 0)
                        End Select
                    Case Else
                        Show_Message_Console("Unknow Adquirence FI:" & FiMessage, COLOR_BLACK, COLOR_RED, 0, TRACE_LOW, 0)
                End Select
            Case "CM"
                Evaluate_Command_Received(BufferMessage)
            Case UNKNOW  '---------------------------------------------------------
                Show_Message_Console("Unknowed message..." & BufferMessage.Substring(0, 40), COLOR_BLACK, COLOR_YELLOW, 0, TRACE_LOW, 0)
        End Select

        '----------------------------
        ClearMemory()
        '----------------------------

    End Sub


    Private Sub ___PROCESS_EVALUATE_INTERNAL_REQUIREMENTS___(ByVal Struct_Queue_Message As SharedStructureMessage)
        Dim MessageType As String = String.Empty
        Dim FormatType As String = String.Empty
        Dim WayType As Byte = 0
        Dim ReplyMessage As String = Struct_Queue_Message.SSM_Common_Data.CRF_Buffer_Data

        Select Case Struct_Queue_Message.SSM_Transaction_Indicator
            '*************************************************
            Case TranType_COMMAND
                Evaluate_Message_Type_Received(Struct_Queue_Message.SSM_Common_Data.CRF_Buffer_Data, FormatType, MessageType, WayType)
                Try
                    Select Case FormatType
                        Case ISO8583 '---------------------------------------------------------
                        Case XML     '---------------------------------------------------------
                        Case COMMAND
                            Evaluate_Command_Received(Struct_Queue_Message.SSM_Common_Data.CRF_Buffer_Data)
                        Case UNKNOW  '---------------------------------------------------------
                            Show_Message_Console(MyName & " Unknow message:" & Struct_Queue_Message.SSM_Common_Data.CRF_Buffer_Data, COLOR_BLACK, COLOR_YELLOW, 0, TRACE_LOW, 1)
                    End Select
                Catch ex As Exception
                    Show_Message_Console(MyName & " Option Message exception ", COLOR_OWNER1, COLOR_RED, 1, TRACE_LOW, 1)
                End Try
                '*************************************************
            Case TranType_REPLY
                Process_Build_Reply_Data(Struct_Queue_Message)
                '*************************************************
            Case TranType_ROUTER_OUT_SCP
                Process_Build_Request_Issuer(Struct_Queue_Message)
                '*************************************************
        End Select

        '----------------------------
        ClearMemory()
        '----------------------------

    End Sub

    Private Sub Process_Local_Auth_Transaction(ByVal RequestBuffer As String, ByVal IdMessage As Int64, ByVal Times As String)
        '****************************************************************
        '                 Inicio Decodificacion de los mensajes
        '****************************************************************
        Dim RequestFromBANRED As New ServiSwitch_ED_RequestFromBanred
        Select Case RequestBuffer.Substring(21, 5)
            Case "0163+"
                RequestFromBANRED.Process_Request_To_LocalAuth_Inquiry(RequestBuffer, IdMessage, Times)
                'Case "0239+", "0439+", "0539+"
            Case "0539+"
                RequestFromBANRED.Process_Request_To_LocalAuth_Transfer(RequestBuffer, IdMessage, Times)
                'Case "0239-", "0439-", "0539-"
            Case "0539-"
                RequestFromBANRED.Process_Request_To_LocalAuth_Reversal_C(RequestBuffer, IdMessage, Times)
            Case "0524-"
                RequestFromBANRED.Process_Request_To_LocalAuth_Reversal(RequestBuffer, IdMessage, Times)
        End Select
        RequestFromBANRED = Nothing
    End Sub

    Private Sub Process_Local_Deny_Transaction(ByVal RequestBuffer As String)
        '****************************************************************
        '                 Inicio Decodificacion de los mensajes
        '****************************************************************
        Dim RequestFromBANRED As New ServiSwitch_ED_RequestFromBanred
        RequestFromBANRED.Process_Reply_Transfer_NotEnabled(RequestBuffer, condError_INVALID_APPL)
        RequestFromBANRED = Nothing
    End Sub

    Private Sub Process_Pending_Adquirence_Transaction(ByVal RequestBuffer As String, ByVal IdMessage As Int64, ByVal Times As String)
        '****************************************************************
        '                 Inicio Decodificacion de los mensajes
        '****************************************************************
        Dim ReplyFromBANRED As New ServiSwitch_ED_ReplyFromBanred
        Select Case RequestBuffer.Substring(21, 5)
            Case "0163+"
                ReplyFromBANRED.Process_Reply_Inquiry_Transaction(RequestBuffer, IdMessage, Times)
            Case "0439+", "0539+", "0239+"
                ReplyFromBANRED.Process_Reply_Transfer_Transaction(RequestBuffer, IdMessage, Times)
            Case "0439-", "0539-", "0239-"
                ReplyFromBANRED.Process_Reply_Reversal_Transaction(RequestBuffer, IdMessage, Times)
        End Select
        ReplyFromBANRED = Nothing

    End Sub

    Public Sub Process_Build_Request_Issuer(ByVal Struct_Queue_Message As SharedStructureMessage)
        Dim RequestMessage As String = String.Empty
        Dim TypeMesasge As String = String.Empty
        Dim ED As New ServiSwitch_ED_ReplyToBanred
        Dim TypeMsg As String = String.Empty
        Dim oTypeMsg As String = String.Empty

        Dim RequestBANRED As New ServiSwitch_ED_RequestToBanred
        Select Case Struct_Queue_Message.SSM_Common_Data.CRF_Transaction_Code
            Case 163
                RequestMessage = RequestBANRED.Encode_Transfer_Inquiry(Struct_Queue_Message)
                TypeMsg = "TC"
                oTypeMsg = "TR"
            Case 539, 439, 239
                If Struct_Queue_Message.SSM_Common_Data.CRF_Reversal_Indicator = 0 Then
                    RequestMessage = RequestBANRED.Encode_Transfer_Request(Struct_Queue_Message)
                    TypeMsg = "TC"
                    oTypeMsg = "TR"
                ElseIf Struct_Queue_Message.SSM_Common_Data.CRF_Reversal_Indicator = 1 Then
                    RequestMessage = RequestBANRED.Encode_Reversal_Request(Struct_Queue_Message)
                    TypeMsg = "XC"
                    oTypeMsg = "XR"
                End If
        End Select
        RequestBANRED = Nothing

        If RequestMessage = "ERROR" Then
            SaveLogMain("Cant send transaction -> Process_Build_Request_Issuer -> RequestMessage invalid")
            Reply_Exception_To_Source(Struct_Queue_Message, condError_UNDEFINED)
            Exit Sub
        End If

        If Get_Socket_Status() = CONNECTED_status Then
            If Send_TCPIP_Message(RequestMessage) = SUCCESSFUL Then
                Dim KeyHT As String = TypeMsg & RequestMessage.Substring(2, 23)
                Struct_Queue_Message.SSM_Common_Data.CRF_Limit_Date = Now
                Struct_Queue_Message.SSM_Instance_Times += "T6_" & GetDateTime() & Concatenator
                RegistryFinantialRequest(Struct_Queue_Message, KeyHT)
                'Console.WriteLine("RegistryFinantialRequest OK")
                Wait_Pending_For_Timeout(KeyHT)
                'Console.WriteLine("Wait_Pending_For_Timeout OK")
            Else
                SaveLogMain("Cant send transaction -> Process_Build_Request_Issuer -> Send_TCPIP_Message Error")
            End If
        Else
            SaveLogMain("Cant send transaction -> Process_Build_Request_Issuer -> Get_Socket_Status Down")
            Reply_Exception_To_Source(Struct_Queue_Message, condError_NOT_CONNECTED)
            Dim MSG1 As String = "Msg:" & oTypeMsg
            MSG1 += " trx:" & Struct_Queue_Message.SSM_Common_Data.CRF_Transaction_Code
            MSG1 += " adq:" & Struct_Queue_Message.SSM_Common_Data.CRF_Adquirer_Institution_Number
            MSG1 += " aut:" & Struct_Queue_Message.SSM_Common_Data.CRF_Issuer_Institution_Number
            MSG1 += " seq:" & Struct_Queue_Message.SSM_Common_Data.CRF_Adquirer_Sequence
            MSG1 += " amt:" & Struct_Queue_Message.SSM_Common_Data.CRF_Transaction_Amount
            MSG1 += " cod:8367"
            Show_Message_Console(MSG1, COLOR_BLACK, COLOR_WHITE, 0, TRACE_LOW, 0)
            Exit Sub
        End If

        Dim MSG As String = "Msg:" & oTypeMsg
        MSG += " trx:" & Struct_Queue_Message.SSM_Common_Data.CRF_Transaction_Code
        MSG += " adq:" & Struct_Queue_Message.SSM_Common_Data.CRF_Adquirer_Institution_Number
        MSG += " aut:" & Struct_Queue_Message.SSM_Common_Data.CRF_Issuer_Institution_Number
        MSG += " seq:" & Struct_Queue_Message.SSM_Common_Data.CRF_Adquirer_Sequence
        MSG += " amt:" & Struct_Queue_Message.SSM_Common_Data.CRF_Transaction_Amount
        Show_Message_Console(MSG, COLOR_BLACK, COLOR_WHITE, 0, TRACE_LOW, 0)


    End Sub

    Public Sub Process_Build_Reply_Data(ByVal Struct_Queue_Message As SharedStructureMessage)
        Dim ReplyMessage As String = String.Empty
        Dim ReplyToBanred As New ServiSwitch_ED_ReplyToBanred
        Dim oMsgType As String = ""

        Select Case Struct_Queue_Message.SSM_Common_Data.CRF_Transaction_Code
            Case 163
                ReplyMessage = ReplyToBanred.Encode_Transfer_Inquiry_Reply(Struct_Queue_Message)
                oMsgType = "TC"
                'Thread.Sleep(New TimeSpan(0, 0, lTOUTinq))
            Case 539
                If Struct_Queue_Message.SSM_Common_Data.CRF_Reversal_Indicator = 0 Then
                    ReplyMessage = ReplyToBanred.Encode_Transfer_Reply(Struct_Queue_Message)
                    oMsgType = "TC"
                    'Thread.Sleep(New TimeSpan(0, 0, lTOUTtrf))
                Else
                    ReplyMessage = ReplyToBanred.Encode_Reverse_Reply(Struct_Queue_Message)
                    oMsgType = "XC"
                    'Thread.Sleep(New TimeSpan(0, 0, lTOUTrev))
                End If
            Case 524
                ReplyMessage = ReplyToBanred.Encode_Reverse_Reply(Struct_Queue_Message)
                oMsgType = "XC"
                'Thread.Sleep(New TimeSpan(0, 0, lTOUTrev))
        End Select
        ReplyToBanred = Nothing

        If Get_Socket_Status() = CONNECTED_status Then
            If Send_TCPIP_Message(ReplyMessage) <> SUCCESSFUL Then
                Console.WriteLine("Error enviando mensaje a BANRED")
            End If
        Else
            Show_Message_Console(MyName & " Connection not stablished to reply message", COLOR_BLACK, COLOR_YELLOW, 0, TRACE_LOW, 0)
        End If

        Struct_Queue_Message.SSM_Instance_Times += "T16_" & GetDateTime() & Concatenator

        Dim MSG As String = "Msg:" & oMsgType
        MSG += " trx:" & Struct_Queue_Message.SSM_Common_Data.CRF_Transaction_Code
        MSG += " adq:" & Struct_Queue_Message.SSM_Common_Data.CRF_Adquirer_Institution_Number
        MSG += " aut:" & Struct_Queue_Message.SSM_Common_Data.CRF_Issuer_Institution_Number
        MSG += " seq:" & Struct_Queue_Message.SSM_Common_Data.CRF_Adquirer_Sequence
        MSG += " amt:" & Struct_Queue_Message.SSM_Common_Data.CRF_Transaction_Amount
        MSG += " cod:" & Struct_Queue_Message.SSM_Common_Data.CRF_Response_Code
        Show_Message_Console(MSG, COLOR_BLACK, COLOR_DARK_GRAY, 0, TRACE_LOW, 0)

        SaveLogMain(Struct_Queue_Message.SSM_Instance_Times)

    End Sub

    Private Sub Process_Local_Control_Message(ByVal RequestBuffer As String, ByVal IdMessage As Int64, ByVal Times As String)

        RequestBuffer = RequestBuffer.Remove(1, 1)
        RequestBuffer = RequestBuffer.Insert(1, "C")

        RequestBuffer = RequestBuffer.Remove(26, 4)
        RequestBuffer = RequestBuffer.Insert(26, "0000")

        If Send_TCPIP_Message(RequestBuffer) = SUCCESSFUL Then
            Last_IN_Message = Now
        End If

    End Sub

    Private Sub Evaluate_Message_Type_Received(ByVal BufferReceived As String, ByRef FormatType As Char, ByRef MessageType As String, ByVal WayType As Byte)
        '************************************************************************
        FormatType = UNKNOW
        '************************************************************************
        If BufferReceived.StartsWith("TR") Then
            FormatType = HOST2
            MessageType = BufferReceived.Substring(21, 4)
            WayType = WAY_REQUEST
        ElseIf BufferReceived.StartsWith("XR") Then
            FormatType = HOST2
            MessageType = BufferReceived.Substring(21, 4)
            WayType = WAY_REQUEST
        ElseIf BufferReceived.StartsWith("AR") Then
            FormatType = HOST2
            MessageType = BufferReceived.Substring(21, 4)
            WayType = WAY_REQUEST
        ElseIf BufferReceived.StartsWith("TC") Then
            FormatType = HOST2
            MessageType = BufferReceived.Substring(21, 4)
            WayType = WAY_REPLY
        ElseIf BufferReceived.StartsWith("CMD") Then
            FormatType = COMMAND
        End If
        '************************************************************************
    End Sub


    Private Sub Evaluate_Command_Received(ByVal CommandLine As String)
        Dim Parms() As String = CommandLine.Split("|")
        Dim PassIdTask As Int16 = Parms(2).Substring(5)
        Dim thNAME As String = Thread.CurrentThread.Name
        Dim ThreadTask As Int16 = thNAME.Substring(thNAME.IndexOf("_") + 1)
        Select Case Parms(0)
            Case "CMD"
                Select Case Parms(1)
                    Case "0061", "0062"
                        'If PassIdTask = ThreadTask Then
                        '    EndTask = True
                        'Else
                        '    EndTask = False
                        'End If
                    Case "0063"
                        ProcessStartTask(CommandLine)
                End Select
            Case "CMD0061"
                Try
                    Thread.CurrentThread.Abort()
                Catch ex As Exception
                    Show_Message_Console(MyName & " Ending task :" & thNAME & " " & ex.Message, COLOR_OWNER1, COLOR_RED, 0, TRACE_LOW, 1)
                End Try
        End Select
    End Sub

    Private Sub ShowWindowNormal()
        ShowWindow(GetConsoleWindow(), SW_SHOWNORMAL)
    End Sub

    Private Sub Evaluate_Status_Control_Message()
        Show_Message_Console("Iniciando Thread de analisis de mensajes de control", COLOR_BLACK, COLOR_GREEN, 0, TRACE_LOW, 1)
        Last_IN_Message = Now
        Dim Just_In_Time As DateTime
        Do While True
            Just_In_Time = Now
            Dim TSP As New TimeSpan
            TSP = Just_In_Time.Subtract(Last_IN_Message)
            If (TSP.TotalSeconds > 60) And (Get_Socket_Status() = CONNECTED_status) Then
                Show_Message_Console("Tiempo en mensajes de Control:" & Math.Round(TSP.TotalSeconds, 2), COLOR_BLACK, COLOR_GREEN, 0, TRACE_LOW, 0)
                'If TSP.TotalSeconds > 300 Then
                '    Close_Connections_Client()
                '    Last_IN_Message = Now
                'End If
            End If
            Thread.Sleep(10000)
        Loop
    End Sub

    Private Sub Evaluate_Reverse_Build(ByVal MainQueueStruct As SharedStructureMessage)
        Select Case MainQueueStruct.SSM_Common_Data.CRF_Transaction_Code
            Case 239, 439, 539
                If MainQueueStruct.SSM_Common_Data.CRF_Reversal_Indicator = 0 Then
                    MainQueueStruct.SSM_Common_Data.CRF_Reversal_Indicator = 1
                    Dim RequestBANRED As New ServiSwitch_ED_RequestToBanred
                    Dim RequestMessage As String = RequestBANRED.Encode_Reversal_Request(MainQueueStruct)
                    If Get_Socket_Status() = CONNECTED_status Then
                        If Send_TCPIP_Message(RequestMessage) = SUCCESSFUL Then
                            Dim KeyHT As String = "XC" & RequestMessage.Substring(2, 23)
                            RegistryFinantialRequest(MainQueueStruct, KeyHT)
                            Wait_Pending_For_Timeout(KeyHT)
                            'SaveLogMain(RequestMessage)
                            Show_Message_Console(MyName & "***** Conditional reverse has been sended *****", COLOR_BLACK, COLOR_YELLOW, 0, TRACE_LOW, 0)
                            SaveLogMain("***** Conditional reverse has been sended *****")
                        End If
                    End If
                End If
        End Select
    End Sub

End Class

