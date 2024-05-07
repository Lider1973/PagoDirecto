Imports System.Threading
Imports System.Messaging
Imports System.Threading.Tasks

Public Class ServiSwitch_ProcessMessage

    Private Const SERVICE_BCM_ATM As String = "0020041002"
    Private Const SERVICE_CPN_PDR As String = "0020030728"
    Private Const SERVICE_BRD_PDR As String = "0020031003"

    'Dim to_ROUTER As New SharedStructureMessage

    Private Const by_TASK As Byte = 0
    Const QUEUE_TIMEOUT As Int64 = -2147467259
    Dim Seq_Switch As Int64 = 0

    Public Thread_MRRQ(0) As Thread
    Public Thread_MRRP(0) As Thread
    Public Thread_NOTIFY As Thread
    Private Tasking As Int16
    Dim DIC_TLR As New Dictionary(Of Int64, Transaction_Log_Record)

    Public Sub Start_Main_Process_Routers()

        Tasking = Convert.ToInt16(GetDataByID(MyName, by_TASK))
        ReDim Thread_MRRQ(Tasking)
        'ReDim Thread_MRRP(Tasking)

        For x = 0 To Tasking
            '***************************************************************
            Thread_MRRQ(x) = New Thread(AddressOf Main_Receiver_Requests)
            Thread_MRRQ(x).Name = "RQtask_" & x
            Thread_MRRQ(x).Start()

            'Thread_MRRP(x) = New Thread(AddressOf Main_Receiver_Replies)
            'Thread_MRRP(x).Name = "RPtask_" & x
            'Thread_MRRP(x).Start()
        Next

        'Thread_NOTIFY = New Thread(AddressOf Process_Analyze_Pending_Notify)
        'Thread_NOTIFY.Name = "TNotify"
        'Thread_NOTIFY.Start()

        'ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf Process_Analyze_Broadcast_Message))

    End Sub



    Private Sub Main_Receiver_Requests()
        Dim TaskID As Int16 = Thread.CurrentThread.Name.Substring(Thread.CurrentThread.Name.IndexOf("_") + 1)
        Dim Legend As String = MyName & " Starting Main_Receiver_Requests #" & TaskID
        Show_Message_Console(Legend, COLOR_BLACK, COLOR_GRAY, 0, TRACE_LOW, 1)
        Legend = Nothing
        '****************************************************************************************
        '****************************************************************************************
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
        '****************************************************************************************
        Dim CurrentThread As Thread = Thread.CurrentThread
        CurrentThread.Priority = ThreadPriority.BelowNormal
        '****************************************************************************************
        Do While True
            Try
                Dim TS_MessageQueue As New MessageQueue(g_RequestQueue)
                TS_MessageQueue.MessageReadPropertyFilter = filter
                TS_MessageQueue.Formatter = New XmlMessageFormatter(New Type() {GetType(SharedStructureMessage)})
                'Dim msgRequest As System.Messaging.Message
                Dim myMessage As Message = TS_MessageQueue.Receive
                Dim Struct_Request_Message As SharedStructureMessage = CType(myMessage.Body, SharedStructureMessage)
                '********************************************************************************
                Struct_Request_Message.SSM_Instance_Times += "T4_" & GetDateTime() & Concatenator

                ___Auth___Process___Routing___(Struct_Request_Message)

                '********************************************************************************
                '********************************************************************************
            Catch ex2 As Messaging.MessageQueueException
                If ex2.ErrorCode = QUEUE_TIMEOUT Then
                    'Console.Write("~")
                    Continue Do
                Else
                    Show_Message_Console(" REQUEST Main_Receiver_Package Messaging.MessageQueueException: " & ex2.Message, COLOR_BLACK, COLOR_RED, 0, TRACE_LOW, 1)
                End If
                Continue Do
            Catch Sn As System.NullReferenceException
                Show_Message_Console(MyName & " Excepion in Main_Receiver_request1: " & Sn.Message & " - " & Sn.StackTrace, COLOR_OWNER1, COLOR_RED, 0, TRACE_LOW, 1)
                Exit Do
            Catch ex As Exception
                Show_Message_Console(MyName & " Excepion in Main_Receiver_request2: " & ex.Message & " - " & ex.StackTrace, COLOR_OWNER1, COLOR_RED, 0, TRACE_LOW, 1)
                Exit Do
            End Try
        Loop
Exiting_Thread:
        Thread.Sleep(1000)
        Show_Message_Console(MyName & " Request Ending " & System.Threading.Thread.CurrentThread.ManagedThreadId, COLOR_BLACK, COLOR_RED, 0, TRACE_LOW, 1)
        Thread_MRRQ(TaskID) = New Thread(AddressOf Main_Receiver_Requests)
        Thread_MRRQ(TaskID).Name = "RQtask_" & TaskID
        Thread_MRRQ(TaskID).Start()
    End Sub


    Private Sub Main_Receiver_Replies()
        Dim TaskID As Int16 = Thread.CurrentThread.Name.Substring(Thread.CurrentThread.Name.IndexOf("_") + 1)
        Dim Legend As String = MyName & " Starting Main_Receiver_Replies #" & TaskID
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

                ___Auth___Process___Reply___(Struct_Queue_Message)

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
        Show_Message_Console(MyName & " Replies Ending " & System.Threading.Thread.CurrentThread.ManagedThreadId, COLOR_BLACK, COLOR_RED, 0, TRACE_LOW, 1)
        Thread_MRRP(TaskID) = New Thread(AddressOf Main_Receiver_Replies)
        Thread_MRRP(TaskID).Name = "RPtask_" & TaskID
        Thread_MRRP(TaskID).Start()
    End Sub


    Private Sub ___Auth___Process___Routing___(ByVal Struct_Request_Message As SharedStructureMessage)
        Dim ReplyMessageException As String = String.Empty

        Struct_Request_Message.SSM_Instance_Times += GetDateTime() & "|"

        Select Case Struct_Request_Message.SSM_Common_Data.CRF_Transaction_Code
            Case inquiry_TRANSFER
                Dim ErrorCode As Int16 = Get_Unique_Transaction_Log(Struct_Request_Message)
                Struct_Request_Message.SSM_Common_Data.CRF_Response_Code = ErrorCode.ToString("0000")
                Put_Message_To_Router(Struct_Request_Message, Struct_Request_Message.SSM_Rout_Queue_Reply_Name)
                Process_Registry_Transaction(Struct_Request_Message)
            Case checking_TRANSFER, saving_TRANSFER, Credits_CARDS
                If Process_Registry_Transaction(Struct_Request_Message) = SUCCESSFUL Then
                    Build_User_Token(Struct_Request_Message.SSM_Common_Data.CRF_Token_Data, "S1", Struct_Request_Message.SSM_Common_Data.CRF_Transaction_Code.ToString("0000"))
                    Build_User_Token(Struct_Request_Message.SSM_Common_Data.CRF_Token_Data, "S2", Struct_Request_Message.SSM_Common_Data.CRF_Switch_Sequence.ToString)
                    If Struct_Request_Message.SSM_Common_Data.CRF_Issuer_Institution_ID = 728 Then
                        Build_User_Token(Struct_Request_Message.SSM_Common_Data.CRF_Token_Data, "A1", Struct_Request_Message.SSM_Common_Data.CRF_Issuer_Institution_ID)
                    End If
                    'Put_Message_To_SAF(Struct_Request_Message)
                End If
        End Select

        Dim MSG As String = " Trx:" & Struct_Request_Message.SSM_Common_Data.CRF_Transaction_Code & " - Resp:"
        MSG += Struct_Request_Message.SSM_Common_Data.CRF_Response_Code.PadLeft(4, "0") & " - Adq:"
        MSG += Struct_Request_Message.SSM_Common_Data.CRF_Adquirer_Institution_ID.ToString("000000") & " - Swt:"
        MSG += Struct_Request_Message.SSM_Common_Data.CRF_Switch_Sequence.ToString("000000") & " - Acct1:"
        MSG += Struct_Request_Message.SSM_Common_Data.CRF_Primary_Account.ToString("0000000000") & " - Acct2:"
        MSG += Struct_Request_Message.SSM_Common_Data.CRF_Secondary_Account.ToString("0000000000")

        Show_Message_Console(MSG, COLOR_BLACK, COLOR_WHITE, 0, TRACE_LOW, 0)

        Console.WriteLine(Struct_Request_Message.SSM_Instance_Times)

        Struct_Request_Message = Nothing
        GC.Collect()
        ClearMemory()
        'Console.WriteLine("TERMINO DRIVER.....")
    End Sub


    Private Sub ___Auth___Process___Reply___(ByVal SRM As SharedStructureMessage)

        Console.WriteLine("___Auth___Process___Reply___ process......")
        Select Case SRM.SSM_Common_Data.CRF_Transaction_Code
            Case BROADCAST_SUCCESS_tran, BROADCAST_DENIED_tran
                If DB_Update_Transaction_Broadcast_Log(SRM) = SUCCESSFUL Then
                    Console.WriteLine("Transaction has been notified.....")
                End If
            Case Else
                Show_Message_Console("No se puede procesar, mensaje desconocido:" & SRM.SSM_Common_Data.CRF_Transaction_Code, COLOR_BLACK, COLOR_YELLOW, 0, TRACE_LOW, 0)
        End Select

    End Sub



    Private Sub Process_Analyze_Pending_Notify()
        '****************************************************************************************
        Show_Message_Console(MyName & " Starting Process_Analyze_Pending_Notify....", COLOR_BLACK, COLOR_GRAY, 0, TRACE_LOW, 1)
        '****************************************************************************************
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
            .TransactionId = True
        End With
        '****************************************************************************************
        Dim CurrentThread As Thread = Thread.CurrentThread
        CurrentThread.Priority = ThreadPriority.BelowNormal
        '****************************************************************************************
        Dim TS_MessageQueue As New MessageQueue(g_SafQueue)
        TS_MessageQueue.MessageReadPropertyFilter = filter
        TS_MessageQueue.Formatter = New XmlMessageFormatter(New Type() {GetType(SharedStructureMessage)})
        '****************************************************************************************
        Do While True
            Try
                Dim myMessages() As Message = TS_MessageQueue.GetAllMessages
                '********************************************************************************
                'Dim Struct_Request_Message As SharedStructureMessage = CType(myMessage.Body, SharedStructureMessage)
                '********************************************************************************
                If myMessages.Length > 0 Then
                    For Each CurrentMessage As Message In myMessages
                        Console.WriteLine("Arrived         :" & CurrentMessage.ArrivedTime)
                        Console.WriteLine("ID              :" & CurrentMessage.Id)
                        Dim TMSP As New TimeSpan
                        TMSP = Now.Subtract(CurrentMessage.ArrivedTime)
                        Console.WriteLine("Seconds in queue:" & TMSP.TotalSeconds)
                        If TMSP.TotalSeconds > 60 Then
                            Dim SSM As SharedStructureMessage = CType(CurrentMessage.Body, SharedStructureMessage)
                            Dim ErrorCode As Byte = DB_Get_Status_Log_Record(SSM)
                            Select Case ErrorCode
                                Case DELETE_RECORD
                                    Dim myMessage As Message = TS_MessageQueue.ReceiveById(CurrentMessage.Id)
                                    Console.WriteLine("message has been received from SAF queue:" & myMessage.Id)
                                Case NOT_SEND
                                    Console.WriteLine("Dont send, will wait task time")
                                Case NOTIFY_RECORD
                                    Build_Message_To_Authorizer(SSM)
                            End Select
                        End If
                        Thread.Sleep(1000)
                    Next
                Else
                    Console.WriteLine("No records on queue")
                End If
                '********************************************************************************
                ReleaseObject(myMessages)
                '********************************************************************************
                Thread.Sleep(10000)
                '********************************************************************************
            Catch ex2 As Messaging.MessageQueueException
                If ex2.ErrorCode = QUEUE_TIMEOUT Then
                    Continue Do
                Else
                    Show_Message_Console(" REQUEST Main_Receiver_Package Messaging.MessageQueueException: " & ex2.Message, COLOR_BLACK, COLOR_RED, 0, TRACE_LOW, 1)
                End If
                Continue Do
            Catch Sn As System.NullReferenceException
                If Exiting = True Then
                    Exit Do
                End If
                Show_Message_Console(MyName & " Excepion in Main_Receiver_request1: " & Sn.Message & " - " & Sn.StackTrace, COLOR_OWNER1, COLOR_RED, 0, TRACE_LOW, 1)
            Catch ex As Exception
                If Exiting = True Then
                    Exit Do
                End If
                Show_Message_Console(MyName & " Excepion in Main_Receiver_request2: " & ex.Message & " - " & ex.StackTrace, COLOR_OWNER1, COLOR_RED, 0, TRACE_LOW, 1)
                Exit Do
            End Try
        Loop
Exiting_Thread:
        Thread.Sleep(1000)
        Show_Message_Console(MyName & " Request Ending " & System.Threading.Thread.CurrentThread.ManagedThreadId, COLOR_BLACK, COLOR_GRAY, 0, TRACE_LOW, 1)
        Thread_NOTIFY = New Thread(AddressOf Process_Analyze_Pending_Notify)
        Thread_NOTIFY.Name = "TNotify"
        Thread_NOTIFY.Start()
    End Sub


    Private Sub Build_Message_To_Authorizer(ByVal SSM As SharedStructureMessage)

        Try
            SSM.SSM_Adq_Queue_Reply_Name = g_ReplyQueue
            SSM.SSM_Rout_Queue_Request_Name = g_RouterRequestQueue
            SSM.SSM_Rout_Source_Name = Mod_RouterName
            SSM.SSM_Adq_Source_Name = MyName
            SSM.SSM_Queue_Message_ID = GetCorrelationID()
            SSM.SSM_Instance_Times = GetDateTime() & "|"
            SSM.SSM_Transaction_Indicator = TranType_ROUTER_IN_SCP
            SSM.SSM_Communication_ID = Constanting_Definition.from_INTERFACES
            SSM.SSM_Rout_Source_Name = Mod_RouterName
            SSM.SSM_Message_Format = Constanting_Definition.HOST2_format
            '******************************************************************************
            SSM.SSM_Common_Data.CRF_Issuer_Institution_ID = FI_CPN_AUT
            SSM.SSM_Common_Data.CRF_Service_Indicator = SERVICE_CPN_PDR
            SSM.SSM_Common_Data.CRF_Issuer_Institution_Number = Get_Token_Data("A1", SSM.SSM_Common_Data.CRF_Token_Data)
            '*****************************************************************************
            If CInt(SSM.SSM_Common_Data.CRF_Response_Code) = SUCCESSFUL Then
                SSM.SSM_Common_Data.CRF_Transaction_Code = BROADCAST_SUCCESS_tran
            Else
                SSM.SSM_Common_Data.CRF_Transaction_Code = BROADCAST_DENIED_tran
            End If
            '*****************************************************************************
            '*************************  Putting message
            If Put_Message_To_Router(SSM, Router_Request_Queue_Name) <> SUCCESSFUL Then
                Console.WriteLine(GetDateTime() & " No fue posible enviar la notificación al autorizador F2")
            End If
            '*************************  Putting message
        Catch ex As Exception
            Console.WriteLine(GetDateTime() & " No fue posible enviar la notificación al autorizador F1")
        End Try
    End Sub

End Class

