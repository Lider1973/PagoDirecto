Imports System.Threading
Imports System.Messaging

Public Class ServiSwitch_ProcessMessage

    Private Const SERVICE_BCM_ATM As String = "0020041002"
    Private Const SERVICE_CPN_PDR As String = "0020030728"
    Private Const SERVICE_BRD_PDR As String = "0020031003"

    'Dim to_ROUTER As New SharedStructureMessage

    Private Const by_TASK As Byte = 0
    Const QUEUE_TIMEOUT As Int64 = -2147467259
    Dim Seq_Switch As Int64 = 0

    Public Thread_MRRQ As Thread
    Public Thread_MRRP As Thread

    Public Sub Start_Main_Process_Routers()

        '***************************************************************
        Thread_MRRQ = New Thread(AddressOf Main_Receiver_Requests)
        Thread_MRRQ.Name = "Thread_MRRQ"
        Thread_MRRQ.Priority = ThreadPriority.Normal
        Thread_MRRQ.Start()


        Thread_MRRP = New Thread(AddressOf Main_Receiver_Replies)
        Thread_MRRP.Priority = ThreadPriority.Normal
        Thread_MRRP.Name = "Thread_MRRP"
        Thread_MRRP.Start()
        '***************************************************************


    End Sub

    Private Sub Main_Receiver_Requests()
        Show_Message_Console("Init Main_Receiver_Requests Thread:" & Thread_MRRQ.Name, COLOR_BLACK, COLOR_GRAY, 0, TRACE_LOW, 1)
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
                Dim TS_MessageQueue As New MessageQueue(Router_Request_Queue_Name)
                TS_MessageQueue.MessageReadPropertyFilter = filter
                TS_MessageQueue.Formatter = New XmlMessageFormatter(New Type() {GetType(SharedStructureMessage)})
                'Dim msgRequest As System.Messaging.Message
                Dim myMessage As Message = TS_MessageQueue.Receive
                Dim Struct_Request_Message As SharedStructureMessage = CType(myMessage.Body, SharedStructureMessage)
                '********************************************************************************
                Dim DiffQueueTime = DateDiff(DateInterval.Second, myMessage.ArrivedTime, DateTime.Now)
                If DiffQueueTime > 5 Then
                    Show_Message_Console("_", COLOR_BLACK, COLOR_WHITE, 0, TRACE_LOW, 0)
                    Continue Do
                End If
                '********************************************************************************
                If Struct_Request_Message.SSM_Communication_ID = Constanting_Definition.from_INTERFACES Then
                    Struct_Request_Message.SSM_Instance_Times += "T7_" & GetDateTime() & Concatenator
                Else
                    Struct_Request_Message.SSM_Instance_Times += "T3_" & GetDateTime() & Concatenator
                End If

                Struct_Request_Message.SSM_Queue_Message_ID = myMessage.Id

                'Console.WriteLine("Id request:" & myMessage.Id)

                '********************************************************************************
                '___Auth___Process___Routing___(Struct_Request_Message)

                ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf ___Auth___Process___Routing___), Struct_Request_Message)

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
        Thread_MRRQ = New Thread(AddressOf Main_Receiver_Requests)
        Thread_MRRQ.Name = "Thread_MRRQ"
        Thread_MRRQ.Priority = ThreadPriority.Normal
        Thread_MRRQ.Start()
    End Sub


    Private Sub Main_Receiver_Replies()
        Show_Message_Console("Init Main_Receiver_Replies Thread:" & Thread_MRRP.Name, COLOR_BLACK, COLOR_GRAY, 0, TRACE_LOW, 1)
        '****************************************************************************************
        Dim TS_MessageQueue As New MessageQueue(Router_Reply_Queue_Name)
        TS_MessageQueue.Formatter = New XmlMessageFormatter(New Type() {GetType(SharedStructureMessage)})
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
        TS_MessageQueue.MessageReadPropertyFilter = filter
        '****************************************************************************************
        Dim CurrentThread As Thread = Thread.CurrentThread
        CurrentThread.Priority = ThreadPriority.BelowNormal
        '****************************************************************************************
        Do While True
            Try
                Dim myMessage As Message = TS_MessageQueue.Receive
                Dim Struct_Request_Message As SharedStructureMessage = CType(myMessage.Body, SharedStructureMessage)
                '********************************************************************************
                '********************************************************************************
                Dim DiffQueueTime = DateDiff(DateInterval.Second, myMessage.ArrivedTime, DateTime.Now)
                If DiffQueueTime > 5 Then
                    'Show_Message_Console("Request too old :" & myMessage.ArrivedTime.ToString & " Id:" & myMessage.Id.ToString, COLOR_BLACK, COLOR_WHITE, 0, TRACE_LOW, 1)
                    Show_Message_Console("_", COLOR_BLACK, COLOR_WHITE, 0, TRACE_LOW, 0)
                    Continue Do
                End If
                '********************************************************************************
                '********************************************************************************
                If Struct_Request_Message.SSM_Communication_ID = Constanting_Definition.from_INTERFACES Then
                    Struct_Request_Message.SSM_Instance_Times += "T13_" & GetDateTime() & Concatenator
                Else
                    Struct_Request_Message.SSM_Instance_Times += "T13_" & GetDateTime() & Concatenator
                End If
                '********************************************************************************
                '___Auth___Process___Replaying___(Struct_Request_Message)

                ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf ___Auth___Process___Replaying___), Struct_Request_Message)

                '********************************************************************************
            Catch Sn As System.NullReferenceException
                If Exiting = True Then
                    GoTo Exiting_Thread
                End If
                Show_Message_Console(MyName & " Excepion in Main_Receiver_replies1: " & Sn.Message & " - " & Sn.StackTrace, COLOR_OWNER1, COLOR_RED, 0, TRACE_LOW, 1)
            Catch ex As Exception
                If Exiting = True Then
                    GoTo Exiting_Thread
                End If
                Show_Message_Console(MyName & " Excepion in Main_Receiver_replies2: " & ex.Message & " - " & ex.StackTrace, COLOR_OWNER1, COLOR_RED, 0, TRACE_LOW, 1)
                GoTo Exiting_Thread
            End Try
        Loop
Exiting_Thread:
        Thread_MRRP = New Thread(AddressOf Main_Receiver_Replies)
        Thread_MRRP.Name = "Thread_MRRP"
        Thread_MRRP.Priority = ThreadPriority.Normal
        Thread_MRRP.Start()
    End Sub


    Private Sub ___Auth___Process___Routing___(ByVal Struct_Request_Message As SharedStructureMessage)

        Dim CurrentProcess As Process = Process.GetCurrentProcess
        CurrentProcess.PriorityClass = ProcessPriorityClass.BelowNormal

        Dim ReplyMessageException As String = String.Empty
        '-----------------------
        '   Switch Sequence
        '-----------------------

        Seq_Switch += 1
        If Seq_Switch > 8000 Then
            Seq_Switch = 1
        End If

        '------------------------------------------------
        'Apply for Clock Inconsistences
        '------------------------------------------------

        Dim TSP As New TimeSpan
        TSP = DateTime.Now.Subtract(Struct_Request_Message.SSM_Common_Data.CRF_Adquirer_Date_Time)
        If (TSP.TotalSeconds > My.Settings.DT_inc) And (Struct_Request_Message.SSM_Message_Format = Constanting_Definition.OWNER_format) Then
            If (Struct_Request_Message.SSM_Common_Data.CRF_Transaction_Code <> 165) And (Struct_Request_Message.SSM_Common_Data.CRF_Reversal_Indicator = 0) Then
                Console.WriteLine("Adquirer time:" & Struct_Request_Message.SSM_Common_Data.CRF_Adquirer_Date_Time.ToString("yyyy-MM-dd HH:mm:ss.fff"))
                Console.WriteLine("Local time   :" & Now.ToString("yyyy-MM-dd HH:mm:ss.fff"))
                Reply_Exception_To_Source(Struct_Request_Message, Constanting_Definition.DATE_TIME_INCONSISTENCE)
                Show_Message_Console(MyName & " Date time Inconsistence ", COLOR_BLACK, COLOR_YELLOW, 0, TRACE_LOW, 1)
                Exit Sub
            End If
        End If


        If (Struct_Request_Message.SSM_Common_Data.CRF_Reversal_Indicator > 0) AndAlso (Struct_Request_Message.SSM_Common_Data.CRF_Adquirer_Institution_Number = FI_CPN_ADQ) Then
            Struct_Request_Message.SSM_Common_Data.CRF_Switch_Sequence = Seq_Switch
            Struct_Request_Message.SSM_Common_Data.CRF_Switch_Sequence = DB_Get_Transaction_Data(Struct_Request_Message)
        Else
            Struct_Request_Message.SSM_Common_Data.CRF_Switch_Sequence = Seq_Switch
        End If

        '-----------------------
        '   Validating Service OUT
        '-----------------------
        If DeclineSwitch = True Then
            Reply_Exception_To_Source(Struct_Request_Message, Constanting_Definition.NOT_PROCESS_ACTIVE)
            Show_Message_Console(MyName & " Service is DOWN ", COLOR_BLACK, COLOR_YELLOW, 0, TRACE_LOW, 0)
            Exit Sub
        End If

        '-----------------------
        '   Validating Authorizer Available
        '-----------------------
        If Set_Get_Status_FI(2, 0, Struct_Request_Message.SSM_Common_Data.CRF_Issuer_Institution_Number) <> SUCCESSFUL Then
            Reply_Exception_To_Source(Struct_Request_Message, Constanting_Definition.INSTITUTION_IS_DOWN)
            Exit Sub
        End If

        '-----------------------
        '   Validating TranCode 
        '-----------------------

        If Validate_Type_Transaction_Code(Struct_Request_Message.SSM_Common_Data.CRF_Transaction_Code) <> SUCCESSFUL Then
            Show_Message_Console(MyName & " Transaction Undefined ", COLOR_BLACK, COLOR_YELLOW, 0, TRACE_LOW, 1)
            ReplyMessageException = Build_Reply_Exception(EXCEPTION_code, "El codigo de la transaccion no es valido")
            Struct_Request_Message.SSM_Common_Data.CRF_Buffer_Data = ReplyMessageException
            Reply_Exception_To_Source(Struct_Request_Message, Constanting_Definition.NOT_TRANSACTION_DEFINED)
            Exit Sub
        End If

        '-----------------------
        '   Obaining RoutingCode
        '-----------------------
        Dim Key_ServiceCode As String = String.Empty
        Select Case Struct_Request_Message.SSM_Common_Data.CRF_Transaction_Code
            Case 1, 2, 3, 4, 5, 6, 7
                Struct_Request_Message.SSM_Common_Data.CRF_Switch_Sequence = Seq_Switch
                If Process_Obtain_Routing_Issuer(Struct_Request_Message, Key_ServiceCode) <> SUCCESSFUL Then
                    Show_Message_Console(MyName & " Cant retrieve the Token data " & ID_TOKEN_SERVICE, COLOR_BLACK, COLOR_YELLOW, 0, TRACE_LOW, 1)
                    ReplyMessageException = Build_Reply_Exception(EXCEPTION_code, "Los datos de ruteo par esta transaccion no son validos")
                    Struct_Request_Message.SSM_Common_Data.CRF_Buffer_Data = ReplyMessageException
                    Reply_Exception_To_Source(Struct_Request_Message, Constanting_Definition.NOT_PROCESS_DEFINED)
                    Exit Sub
                End If
            Case Else
                Key_ServiceCode = Struct_Request_Message.SSM_Common_Data.CRF_Service_Indicator
        End Select

        '------------------------------
        '   Arming Routing Codification 
        '------------------------------

        Dim Key_IssuerID As String = Struct_Request_Message.SSM_Common_Data.CRF_Issuer_Institution_ID.ToString("0000")
        Dim Key_TransactionCode As String = Struct_Request_Message.SSM_Common_Data.CRF_Transaction_Code.ToString("000000")
        Dim ArmedRoute As String = Key_ServiceCode & Key_TransactionCode & Key_IssuerID
        Console.WriteLine(Key_IssuerID & " " & Key_TransactionCode & " " & ArmedRoute)

        Dim IssuerID As String = Get_Issuer_ID(ArmedRoute)
        If IssuerID = INVALID_NUMBER Then
            Show_Message_Console(MyName & " Cant retrieve the Issuer ID " & ID_TOKEN_SERVICE, COLOR_BLACK, COLOR_YELLOW, 0, TRACE_MEDIUM, 1)
            ReplyMessageException = Build_Reply_Exception(EXCEPTION_code, "No se pudo obtener el codigo autorizador para esta transaccion")
            Struct_Request_Message.SSM_Common_Data.CRF_Buffer_Data = ReplyMessageException
            Reply_Exception_To_Source(Struct_Request_Message, Constanting_Definition.NOT_PROCESS_DEFINED)
            Key_ServiceCode = Nothing
            Key_IssuerID = Nothing
            Key_TransactionCode = Nothing
            ArmedRoute = Nothing
            IssuerID = Nothing
            Exit Sub
        End If

        '------------------------------
        '   Obtaining Queue Route
        '------------------------------
        Dim IssuerQueueToRoute As String = String.Empty
        Dim SystemOutName As String = String.Empty
        Dim ReplyRouterQueue As String = String.Empty
        If GetPathQueueName(CInt(IssuerID), REQUEST_QUEUE, IssuerQueueToRoute, SystemOutName) = SUCCESSFUL Then
            If GetPathQueueName(MyName, 1, ReplyRouterQueue) = SUCCESSFUL Then
                Struct_Request_Message.SSM_Rout_Source_Name = MyName
                Struct_Request_Message.SSM_Rout_Queue_Request_Name = Router_Request_Queue_Name
                Struct_Request_Message.SSM_Rout_Queue_Reply_Name = Router_Reply_Queue_Name
                Struct_Request_Message.SSM_Auth_Module_ID = IssuerID
                Struct_Request_Message.SSM_Transaction_Indicator = TranType_ROUTER_OUT_SCP
                'Build_Wait_Task_Message(Struct_Request_Message)
                Put_Message_To_Auth(Struct_Request_Message, IssuerQueueToRoute)
            End If
        End If

        Console.WriteLine("****************************************************************************")
        Show_Message_Console(MyName & " Routing from " & Struct_Request_Message.SSM_Adq_Source_Name & " to " & SystemOutName, COLOR_BLACK, COLOR_GRAY, 0, TRACE_LOW, 0)
        Show_Message_Console(MyName & " Sequence :" & Struct_Request_Message.SSM_Common_Data.CRF_Adquirer_Sequence, COLOR_BLACK, COLOR_GRAY, 0, TRACE_LOW, 0)

        '------------------------------
        '   Free Resources
        '------------------------------
        Struct_Request_Message = Nothing
        Key_ServiceCode = Nothing
        Key_IssuerID = Nothing
        Key_TransactionCode = Nothing
        ArmedRoute = Nothing
        IssuerID = Nothing
        SystemOutName = Nothing
        ReplyRouterQueue = Nothing
        GC.Collect()
        ClearMemory()

    End Sub

    Private Sub ___Auth___Process___Replaying___(ByVal Struct_Request_Message As SharedStructureMessage)

        Dim CurrentProcess As Process = Process.GetCurrentProcess
        CurrentProcess.PriorityClass = ProcessPriorityClass.BelowNormal


        Show_Message_Console(MyName & " Routing from " & Struct_Request_Message.SSM_Auth_Source_Name & " to " & Struct_Request_Message.SSM_Adq_Source_Name, COLOR_BLACK, COLOR_GRAY, 0, TRACE_LOW, 0)
        Show_Message_Console(MyName & " AuthCode :" & Struct_Request_Message.SSM_Common_Data.CRF_Authorization_Code, COLOR_BLACK, COLOR_GRAY, 0, TRACE_LOW, 0)
        Show_Message_Console(MyName & " RespCode :" & Struct_Request_Message.SSM_Common_Data.CRF_Response_Code, COLOR_BLACK, COLOR_GRAY, 0, TRACE_LOW, 0)
        Show_Message_Console(MyName & " Sequence :" & Struct_Request_Message.SSM_Common_Data.CRF_Adquirer_Sequence, COLOR_BLACK, COLOR_GRAY, 0, TRACE_LOW, 0)
        Console.WriteLine("****************************************************************************")

        '---------------------------------------
        '   Reply Message to Requester Interface
        '---------------------------------------
        Put_Message_To_Adq(Struct_Request_Message, Struct_Request_Message.SSM_Adq_Queue_Reply_Name)


        '---------------------------------------
        '   Send Message to Logger
        '---------------------------------------
        If ((Struct_Request_Message.SSM_Common_Data.CRF_Transaction_Code = 439) Or (Struct_Request_Message.SSM_Common_Data.CRF_Transaction_Code = 539) Or (Struct_Request_Message.SSM_Common_Data.CRF_Transaction_Code = 239)) And (Struct_Request_Message.SSM_Common_Data.CRF_Reversal_Indicator <> 4) Then
            Put_Message_To_Logger(Struct_Request_Message)
        End If

        '------------------------------
        '   Free Resources
        '------------------------------
        Struct_Request_Message = Nothing
        GC.Collect()
        ClearMemory()

    End Sub

    'Private Sub Build_Wait_Task_Message(ByVal SRM As SharedStructureMessage)

    '    If (SRM.SSM_Auth_Module_ID = 1019) And
    '        (SRM.SSM_Common_Data.CRF_Reversal_Indicator = 0) And
    '        ((SRM.SSM_Common_Data.CRF_Transaction_Code = 439) Or (SRM.SSM_Common_Data.CRF_Transaction_Code = 539)) And
    '        (SRM.SSM_Common_Data.CRF_Issuer_Institution_ID = 728) Then
    '        If GetPathQueueName(MyName, 3, SAF_Router_Queue_Name) = SUCCESSFUL Then
    '            Dim SAF As New SAF
    '            SAF.SSM = SRM
    '            SAF.SAF_DateTime = Now
    '            SAF.SAF_Key = SRM.SSM_Common_Data.CRF_Transaction_Code & SRM.SSM_Common_Data.CRF_Adquirer_Sequence & SRM.SSM_Common_Data.CRF_Issuer_Institution_ID
    '            SAF.SSM.SSM_Common_Data.CRF_User_Data = SAF.SSM.SSM_Queue_Message_ID
    '            SAF.SSM.SSM_Common_Data.CRF_Reversal_Indicator = 4
    '            Put_Message_SAF_Message(SAF)
    '            SAF.SSM.SSM_Common_Data.CRF_Reversal_Indicator = 0
    '        End If
    '    End If
    '    SRM = Nothing
    'End Sub


    'Private Sub Receive_Wait_Task_Message(ByVal SRM As SharedStructureMessage)
    '    '***************************************************************************************
    '    Dim arrTypes(1) As System.Type
    '    arrTypes(0) = GetType(SAF)
    '    arrTypes(1) = GetType(Object)
    '    Dim to_SAF As New SAF
    '    '***************************************************************************************
    '    Dim Receive_msg As New Message
    '    Dim msgR As New MessageQueue(SAF_Router_Queue_Name)
    '    '***************************************************************************************
    '    Try
    '        msgR.Formatter = New XmlMessageFormatter(arrTypes)
    '        Receive_msg = msgR.ReceiveByCorrelationId(SRM.SSM_Common_Data.CRF_User_Data, New TimeSpan(0, 0, 10))
    '        to_SAF = CType(Receive_msg.Body, SAF)
    '        If Not IsNothing(to_SAF) Then
    '            SaveLogMain("RESPUESTA DE TRANSACCION EXITOSA:" & to_SAF.SSM.SSM_Common_Data.CRF_Transaction_Code & "-" & to_SAF.SSM.SSM_Common_Data.CRF_Adquirer_Sequence & "-" & to_SAF.SSM.SSM_Common_Data.CRF_Adquirer_Date_Time.ToString("yyyyMMddHHmmssfff"))
    '        End If
    '    Catch qe As MessageQueueException
    '        Console.WriteLine("ERROR:" & qe.Message)
    '    Catch ex As Exception
    '        Console.WriteLine("ERROR:" & ex.Message)
    '    End Try
    '    '***************************************************************************************
    'End Sub

    'Private Sub Receive_From_Task(ByVal ID As String, ByVal TSP As TimeSpan)
    '    Dim arrTypes(1) As System.Type
    '    arrTypes(0) = GetType(SAF)
    '    arrTypes(1) = GetType(Object)
    '    Dim to_SAF As New SAF
    '    Dim Receive_msg As New Message
    '    Dim msgR As New MessageQueue(SAF_Router_Queue_Name)
    '    '***************************************************************************************
    '    Try
    '        msgR.Formatter = New XmlMessageFormatter(arrTypes)
    '        Receive_msg = msgR.ReceiveByCorrelationId(ID, New TimeSpan(0, 0, 10))
    '        to_SAF = CType(Receive_msg.Body, SAF)
    '        If Not IsNothing(to_SAF) Then
    '            SaveLogMain("ELIMINADO POR REINTENTOS:" & to_SAF.SSM.SSM_Common_Data.CRF_Transaction_Code & "-" & to_SAF.SSM.SSM_Common_Data.CRF_Adquirer_Sequence & "-" & to_SAF.SSM.SSM_Common_Data.CRF_Adquirer_Date_Time.ToString("yyyyMMddHHmmssfff") & " Horas:" & TSP.TotalHours & " Minutos:" & TSP.TotalSeconds & " Milisegundos:" & TSP.TotalMilliseconds)
    '        End If
    '    Catch qe As MessageQueueException
    '        Show_Message_Console(MyName & " " & qe.Message, COLOR_BLACK, COLOR_RED, 0, TRACE_LOW, 0)
    '    Catch ex As Exception
    '        Show_Message_Console(MyName & " " & ex.Message, COLOR_BLACK, COLOR_RED, 0, TRACE_LOW, 0)
    '    End Try
    '    '***************************************************************************************
    'End Sub

    'Private Sub Receive_Object_Structure(ByVal ID As String, ByRef SSM As SharedStructureMessage)
    '    Dim arrTypes(1) As System.Type
    '    arrTypes(0) = GetType(SAF)
    '    arrTypes(1) = GetType(Object)
    '    Dim to_SAF As New SAF
    '    Dim Receive_msg As New Message
    '    Dim msgR As New MessageQueue(SAF_Router_Queue_Name)
    '    '***************************************************************************************
    '    Try
    '        msgR.Formatter = New XmlMessageFormatter(arrTypes)
    '        Receive_msg = msgR.PeekByCorrelationId(ID, New TimeSpan(0, 0, 10))
    '        to_SAF = CType(Receive_msg.Body, SAF)
    '        If Not IsNothing(to_SAF) Then
    '            SSM = to_SAF.SSM
    '        End If
    '    Catch qe As MessageQueueException
    '        Show_Message_Console(MyName & " " & qe.Message, COLOR_BLACK, COLOR_RED, 0, TRACE_LOW, 0)
    '    Catch ex As Exception
    '        Show_Message_Console(MyName & " " & ex.Message, COLOR_BLACK, COLOR_RED, 0, TRACE_LOW, 0)
    '    End Try
    '    '***************************************************************************************
    'End Sub


    'Private Sub Process_Load_SAF_DIC()
    '    If GetPathQueueName(MyName, 3, SAF_Router_Queue_Name) = SUCCESSFUL Then
    '        Dim _messageQueue = New MessageQueue(SAF_Router_Queue_Name, QueueAccessMode.Peek)
    '        Dim x = _messageQueue.GetMessageEnumerator2()
    '        Dim iCount As Integer = 0
    '        While x.MoveNext()
    '            Dim XX As New SAF_DIC
    '            XX.SD_Counter = 1
    '            Dim Parms() As String = (x.Current.Label.ToString.Split("|"))
    '            XX.SD_DateTime_Enqueued = Parms(1)
    '            DIC_SAF.Add(Parms(0), XX)
    '        End While

    '        Thread_SAF = New Thread(AddressOf Process_Execute_REVIEW)
    '        Thread_SAF.Name = "THsaf"
    '        Thread_SAF.Start()
    '    End If
    'End Sub

    'Private Sub Process_Execute_REVIEW()
    '    Do While True
    '    Loop
    'End Sub


    'Private Sub Process_Peek_First_Message()
    '    Dim IssuerQueueToRoute As String = String.Empty
    '    Dim SystemOutName As String = String.Empty
    '    Thread.Sleep(20000)
    '    If GetPathQueueName(MyName, 3, SAF_Router_Queue_Name) = SUCCESSFUL Then
    '        Do While True
    '            Dim _messageQueue = New MessageQueue(SAF_Router_Queue_Name, QueueAccessMode.Peek)
    '            Dim x = _messageQueue.GetMessageEnumerator2()
    '            While x.MoveNext()
    '                Dim Parms() As String = (x.Current.Label.ToString.Split("|"))
    '                Dim IDmsg As String = Parms(0)
    '                Dim DTN As DateTime = CType(Parms(1), DateTime)
    '                Dim TSP As TimeSpan = Now.Subtract(DTN)
    '                If TSP.TotalMinutes >= 5 Then
    '                    Console.WriteLine("-------------- LO ELIMINO DE LA COLA ------------------")
    '                    SaveLogMain(GetDateTime() & " " & IDmsg & " ELIMINADO DE LA COLA")
    '                    Receive_From_Task(IDmsg, TSP)
    '                ElseIf TSP.TotalMinutes >= 1 Then
    '                    Console.WriteLine("-------------- LO ENVIO DE NUEVO ------------------")
    '                    SaveLogMain(GetDateTime() & " " & IDmsg & " ENVIO DE NUEVO")
    '                    Dim SSM As New SharedStructureMessage
    '                    Receive_Object_Structure(IDmsg, SSM)
    '                    If GetPathQueueName(CInt(SSM.SSM_Auth_Module_ID), REQUEST_QUEUE, IssuerQueueToRoute, SystemOutName) = SUCCESSFUL Then
    '                        Put_Message_To_Auth(SSM, IssuerQueueToRoute)
    '                    End If
    '                End If
    '            End While
    '            Thread.Sleep(60000)
    '        Loop
    '    End If
    'End Sub

    Private Function Process_Obtain_Routing_Issuer(ByVal Struct_Request_Message As SharedStructureMessage, ByRef Key_ServiceCode As String) As String
        Dim ErrorCode As Byte = PROCESS_ERROR

        Key_ServiceCode = Get_Info_Token_Data(Struct_Request_Message.SSM_Common_Data.CRF_Token_Data, ID_TOKEN_SERVICE)
        If Key_ServiceCode <> INVALID_STRING Then
            ErrorCode = SUCCESSFUL
        End If

        Return ErrorCode
    End Function

End Class

