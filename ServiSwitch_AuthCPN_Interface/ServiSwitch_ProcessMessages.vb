'
' VERSION HOST2 1 PARAMETRO
'
Imports System.Threading
Imports System.Messaging
Imports System.Runtime.InteropServices
Imports System.Drawing
Imports System.Globalization
Imports System.Threading.Tasks
Imports System.Xml

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

    Public Thread_MRRQ As Thread

    Dim RunningHide As Boolean = False
    'Dim AnalizeTimeOut As Thread
    Dim lockOBJ0 As Object = New Object
    Dim lockOBJ1 As Object = New Object
    Dim lockOBJ2 As Object = New Object
    Dim HTOriginalRequest As New Hashtable
    Dim HTFromSocket As New Hashtable
    Dim ED As New ServiSwitch_EncodeDecodeClass

    Public Sub Init_Task_Process_Request(ByVal TaskNumbers As Int32)

        Thread_MRRQ = New Thread(AddressOf Main_Receiver_Requests)
        Thread_MRRQ.Name = "Thread_MRRQ"
        Thread_MRRQ.Priority = ThreadPriority.Normal
        Thread_MRRQ.Start()

    End Sub


    Private Sub Main_Receiver_Requests()

        Show_Message_Console("Init Main_Receiver_Requests Thread:" & Thread_MRRQ.Name, COLOR_BLACK, COLOR_GRAY, 0, TRACE_LOW, 1)        '********************************************************************************
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
        '****************************************************************************************
        Dim TS_MessageQueue As New MessageQueue(g_RequestQueue)
        TS_MessageQueue.MessageReadPropertyFilter = filter
        TS_MessageQueue.Formatter = New XmlMessageFormatter(New Type() {GetType(SharedStructureMessage)})
        '********************************************************************************
        Do While True
            Try
                Show_Message_Console(MyName & " Waiting with " & Thread.CurrentThread.Name, COLOR_BLACK, COLOR_WHITE, 0, TRACE_LOW, 1)
                Dim myMessage As Message = TS_MessageQueue.Receive
                Dim Struct_Queue_Message As SharedStructureMessage = CType(myMessage.Body, SharedStructureMessage)
                '********************************************************************************
                Dim DiffQueueTime = DateDiff(DateInterval.Second, myMessage.ArrivedTime, DateTime.Now)
                If DiffQueueTime > 10 Then
                    Console.Write("*")
                    DiffQueueTime = Nothing
                    Continue Do
                End If
                '********************************************************************************

                Struct_Queue_Message.SSM_Instance_Times += "T9_" & GetDateTime() & Concatenator

                ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf ___PROCESS_EVALUATE_INTERNAL_REQUIREMENTS___), Struct_Queue_Message)

                '********************************************************************************
            Catch ta As ThreadAbortException
                If Exiting = True Then
                    GoTo Exiting_Thread
                End If
                Show_Message_Console(MyName & " Exception in listen request 2: " & ta.Message, COLOR_OWNER1, COLOR_RED, 0, TRACE_LOW, 1)
                GoTo Exiting_Thread
            Catch ex As Exception
                If Exiting = True Then
                    GoTo Exiting_Thread
                End If
                Show_Message_Console(MyName & " Exception in listen request 1: " & ex.Message, COLOR_OWNER1, COLOR_RED, 0, TRACE_LOW, 1)
                GoTo Exiting_Thread
            End Try
        Loop
        '********************************************************************************
Exiting_Thread:
        Show_Message_Console("Ending Main_Receiver_Requests Thread:" & Thread_MRRQ.Name, COLOR_BLACK, COLOR_GRAY, 0, TRACE_LOW, 1)
        'Thread.Sleep(1000)
        Thread_MRRQ = New Thread(AddressOf Main_Receiver_Requests)
        Thread_MRRQ.Name = "Thread_MRRQ"
        Thread_MRRQ.Priority = ThreadPriority.Normal
        Thread_MRRQ.Start()

    End Sub


    Private Sub ___PROCESS_EVALUATE_INTERNAL_REQUIREMENTS___(ByVal Struct_Queue_Message As SharedStructureMessage)
        Dim MessageType As String = String.Empty
        Dim FormatType As String = String.Empty
        Dim WayType As Byte = 0


        Console.WriteLine(" PROCESANDO REQUERIMIENTO CON TAREA:" & Thread.CurrentThread.Name)

        Select Case Struct_Queue_Message.SSM_Transaction_Indicator
            '*************************************************
            Case TranType_COMMAND
                Evaluate_Message_Type_Received(Struct_Queue_Message.SSM_Common_Data.CRF_Buffer_Data, FormatType, MessageType, WayType)
                Try
                    Select Case FormatType
                        Case ISO8583 '---------------------------------------------------------
                        Case XML     '---------------------------------------------------------
                        Case COMMAND
                            'Evaluate_Command_Received(Struct_Queue_Message.SSM_Common_Data.CRF_Buffer_Data, EndTask)
                            Show_Message_Console(MyName & " Process Command: Disabled" & Struct_Queue_Message.SSM_Common_Data.CRF_Buffer_Data, COLOR_BLACK, COLOR_YELLOW, 0, TRACE_LOW, 1)
                        Case UNKNOW  '---------------------------------------------------------
                            Show_Message_Console(MyName & " Unknow message:" & Struct_Queue_Message.SSM_Common_Data.CRF_Buffer_Data, COLOR_BLACK, COLOR_YELLOW, 0, TRACE_LOW, 1)
                    End Select
                Catch ex As Exception
                    Show_Message_Console(MyName & " Option Message exception ", COLOR_OWNER1, COLOR_RED, 1, TRACE_LOW, 1)
                End Try
                '*************************************************
            Case TranType_ROUTER_OUT_SCP
                Process_Build_Request_Issuer(Struct_Queue_Message)
                '*************************************************
        End Select

    End Sub


    Private Sub Evaluate_Command_Received(ByVal CommandLine As String, ByRef EndTask As Boolean)
        Dim Parms() As String = CommandLine.Split("|")
        Dim PassIdTask As Int16 = Parms(2).Substring(5)
        Dim thNAME As String = Thread.CurrentThread.Name
        Dim ThreadTask As Int16 = thNAME.Substring(thNAME.IndexOf("_") + 1)

        Select Case Parms(0)
            Case "CMD"
                Select Case Parms(1)
                    Case "0061", "0062"
                        If PassIdTask = ThreadTask Then
                            EndTask = True
                        Else
                            EndTask = False
                        End If
                    Case "0063"
                        'ProcessStartTask(CommandLine)
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

    Public Sub Reply_Exception_To_Source(ByVal Struct_Request_Message As SharedStructureMessage, ByVal ErrorCode As String)
        Struct_Request_Message.SSM_Instance_Times += "T11_" & GetDateTime() & Concatenator
        Struct_Request_Message.SSM_Common_Data.CRF_Response_Code = ErrorCode
        Struct_Request_Message.SSM_Common_Data.CRF_Authorization_Code = "000000"
        Struct_Request_Message.SSM_Transaction_Indicator = TranType_REPLY

        Put_Message_To_Router(Struct_Request_Message, Struct_Request_Message.SSM_Rout_Queue_Reply_Name)

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


    Public Sub Process_Build_Request_Issuer(ByVal SQM As SharedStructureMessage)
        Dim ErrorCode As Byte
        Dim Token_Data As String = String.Empty
        Dim TKNid(20) As String
        Dim TKNdata(20) As String

        Dim ED As New ServiSwitch_EncodeDecodeClass
        Try
            If SQM.SSM_Common_Data.CRF_Token_Data.Length > 0 Then
                Dim x, y As Integer
                Token_Data = SQM.SSM_Common_Data.CRF_Token_Data
                Do While Token_Data.Length > 0
                    TKNid(x) = Token_Data.Substring(0, 2)
                    Token_Data = Token_Data.Remove(0, 2)
                    y = Token_Data.Substring(0, 3)
                    Token_Data = Token_Data.Remove(0, 3)
                    TKNdata(x) = Token_Data.Substring(0, y)
                    Token_Data = Token_Data.Remove(0, y)
                    Dim TMP As String = TKNdata(x)
                    TMP = TMP.Replace(" ", Nothing)
                    If TMP.Length = 0 Then
                        TKNdata(x) = TKNdata(x).Replace(" ", "0")
                    End If
                    x += 1
                Loop
                Array.Resize(TKNid, x)
                Array.Resize(TKNdata, x)
            End If
            ErrorCode = SUCCESSFUL
        Catch ex As Exception
            ErrorCode = PROCESS_ERROR
            SaveLogMain(GetDateTime() & " Excepcion recuperando Tokens de entrada")
            Show_Message_Console(GetDateTime() & " Excepcion recuperando Tokens de entrada", COLOR_BLACK, COLOR_RED, 0, TRACE_LOW, 0)
        End Try
        If ErrorCode <> SUCCESSFUL Then
            Show_Message_Console(MyName & " Transaction was not processed by exception", COLOR_BLACK, COLOR_YELLOW, 0, TRACE_LOW, 0)
            SaveLogMain(GetDateTime() & " Salgo por Error")
            Reply_Exception_To_Source(SQM, condError_CANT_PROCESS)
            Exit Sub
        End If

        Dim RequestXML As String
        Try
            If (SQM.SSM_Common_Data.CRF_Transaction_Code = 181) Or (SQM.SSM_Common_Data.CRF_Transaction_Code = 182) Then
                RequestXML = ED.Generate_XML_Broadcast(SQM.SSM_Common_Data.CRF_Response_Code,
                                                 SQM.SSM_Common_Data.CRF_Terminal_ID,
                                                 SQM.SSM_Common_Data.CRF_Adquirer_Sequence,
                                                 SQM.SSM_Common_Data.CRF_Transaction_Code,
                                                 SQM.SSM_Common_Data.CRF_Adquirer_Date_Time,
                                                 SQM.SSM_Common_Data.CRF_Issuer_Institution_Number,
                                                 SQM.SSM_Common_Data.CRF_Transaction_Amount,
                                                 SQM.SSM_Common_Data.CRF_Primary_Account,
                                                 SQM.SSM_Common_Data.CRF_Secondary_Account,
                                                 SQM.SSM_Common_Data.CRF_Channel_Id,
                                                 SQM.SSM_Common_Data.CRF_Token_Data,
                                                 SQM.SSM_Common_Data.CRF_Service_Indicator,
                                                 SQM.SSM_Common_Data.CRF_Reversal_Indicator)
            Else
                RequestXML = ED.Generate_XML_Request(SQM.SSM_Common_Data.CRF_Terminal_ID,
                                                 SQM.SSM_Common_Data.CRF_Adquirer_Sequence,
                                                 SQM.SSM_Common_Data.CRF_Transaction_Code,
                                                 SQM.SSM_Common_Data.CRF_Adquirer_Date_Time,
                                                 SQM.SSM_Common_Data.CRF_Adquirer_Institution_Number,
                                                 SQM.SSM_Common_Data.CRF_Channel_Id,
                                                 ED.Get_Token_Info(NAMES_ID, TKNid, TKNdata),
                                                 SQM.SSM_Common_Data.CRF_Transaction_Amount,
                                                 SQM.SSM_Common_Data.CRF_Primary_Account,
                                                 SQM.SSM_Common_Data.CRF_Secondary_Account,
                                                 ED.Get_Token_Info(TARGET_ACCT_TYPE_ID, TKNid, TKNdata),
                                                 SQM.SSM_Common_Data.CRF_Branch_ID,
                                                 SQM.SSM_Common_Data.CRF_Operator_ID,
                                                 ED.Get_Token_Info(ACCT_TYPE_ID, TKNid, TKNdata),
                                                 SQM.SSM_Common_Data.CRF_PhoneNumber,
                                                 SQM.SSM_Common_Data.CRF_Reversal_Indicator.ToString,
                                                 SQM.SSM_Common_Data.CRF_Names,
                                                 SQM.SSM_Common_Data.CRF_Reference,
                                                 ED.Get_Token_Info(TARGET_DOCUMENT_ID, TKNid, TKNdata),
                                                 ED.Get_Token_Info(DOCUMENT_ID, TKNid, TKNdata))
            End If
        Catch ex As Exception
            SaveLogMain(GetDateTime() & " Excepcion Armando Buffer XML de requerimiento:" & ex.Message)
            Show_Message_Console(GetDateTime() & " ERROR-0:" & ex.Message, COLOR_BLACK, COLOR_RED, 0, TRACE_LOW, 0)
            Reply_Exception_To_Source(SQM, condError_CANT_PROCESS)
            Exit Sub
        End Try

        ' *********************************
        ' EXECUTE WEB SERVICE AUTHORIZATION
        ' *********************************

        Show_Message_Console(MyName & " TranCode:" & SQM.SSM_Common_Data.CRF_Transaction_Code & " SeqNbr:" & SQM.SSM_Common_Data.CRF_Adquirer_Sequence, COLOR_BLACK, COLOR_GRAY, 0, TRACE_LOW, 0)
        Dim StringDateTime = SQM.SSM_Common_Data.CRF_Adquirer_Date_Time.ToString("yyyy-MM-dd HH:mm:ss")
        Dim keyHT As String = CInt(SQM.SSM_Common_Data.CRF_Terminal_ID) & CInt(SQM.SSM_Common_Data.CRF_Adquirer_Sequence) & CInt(SQM.SSM_Common_Data.CRF_Transaction_Code) & CLng(SQM.SSM_Common_Data.CRF_Secondary_Account)
        Show_Message_Console("KEY SEND:" & keyHT, COLOR_BLACK, COLOR_YELLOW, 0, TRACE_LOW, 0)

        Console.WriteLine("REQUERIMIENTO ------------------------" & Chr(13))
        Console.WriteLine(RequestXML)

        RegistryFinantialRequest(SQM, keyHT)

        '2021-03-01
        SaveLogMain(RequestXML)
        '2021-03-01

        Dim ReplyXML As String = ""
        Dim BanredSequence As Int32 = SQM.SSM_Common_Data.CRF_Adquirer_Sequence
        Dim dt1 As DateTime = Now
        Dim WSClient As New ServiSwitch_WSClient

        Try
            SQM.SSM_Instance_Times += "T10_" & GetDateTime() & Concatenator
            ReplyXML = WSClient.Call_WSClient(My.Settings.WSDL, RequestXML)
            SQM.SSM_Instance_Times += "T11_" & GetDateTime() & Concatenator
        Catch ex As Exception
            Dim TSP1 As New TimeSpan
            TSP1 = Now.Subtract(dt1)
            Dim ExMessage As String = "Call_WSClient  Exception:" & ex.Message & "TIEMPO TOTAL ---> Seg:" & TSP1.Seconds & " Mls:" & TSP1.Milliseconds & " Seq:" & BanredSequence & " LenReply:" & ReplyXML
            Show_Message_Console(ExMessage, COLOR_BLACK, COLOR_YELLOW, 0, TRACE_LOW, 0)
            SaveLogMain(ExMessage)
            '------------------------------------
            If RetrieveOriginalRequest(SQM, keyHT) = SUCCESSFUL Then
                Reply_Exception_To_Source(SQM, condError_CANT_PROCESS)
            Else
                Show_Message_Console("No reply because previously responsed....", COLOR_BLACK, COLOR_WHITE, 0, TRACE_LOW, 0)
            End If
            '------------------------------------
            Exit Sub
            '------------------------------------
        End Try

        Dim nSQM As New SharedStructureMessage
        Dim dt2 As DateTime = Now

        '----------------------------------------------
        SaveLogMain(ReplyXML)
        '----------------------------------------------
        Dim TSP As New TimeSpan
        TSP = dt2.Subtract(dt1)
        If TSP.TotalMilliseconds > Mod_Timeout Then
            Console.WriteLine("TIEMPO TOTAL ---> Seg:" & TSP.Seconds & " Mls:" & TSP.Milliseconds & " Seq:" & BanredSequence & " LenReply:" & ReplyXML.Length & " Timeout")
            SaveLogMain("TIEMPO TOTAL ---> Seg:" & TSP.Seconds & " Mls:" & TSP.Milliseconds & " Seq:" & BanredSequence & " LenReply:" & ReplyXML.Length & " Timeout")
        Else
            Console.WriteLine("TIEMPO TOTAL ---> Seg:" & TSP.Seconds & " Mls:" & TSP.Milliseconds & " Seq:" & BanredSequence & " LenReply:" & ReplyXML.Length & " Tiempo OK")
            SaveLogMain("TIEMPO TOTAL ---> Seg:" & TSP.Seconds & " Mls:" & TSP.Milliseconds & " Seq:" & BanredSequence & " LenReply:" & ReplyXML.Length & " Tiempo OK")
        End If

        If Load_Response_From_XML_Reply(nSQM, ReplyXML) = SUCCESSFUL Then
            keyHT = CInt(nSQM.SSM_Common_Data.CRF_Terminal_ID) & CInt(nSQM.SSM_Common_Data.CRF_Adquirer_Sequence) & CInt(nSQM.SSM_Common_Data.CRF_Transaction_Code) & CLng(nSQM.SSM_Common_Data.CRF_Secondary_Account)
            Show_Message_Console("KEY RECEIVE:" & keyHT, COLOR_BLACK, COLOR_YELLOW, 0, TRACE_LOW, 0)
            If RetrieveOriginalRequest(SQM, keyHT) = SUCCESSFUL Then
                Dim RND As New Random
                SQM.SSM_Transaction_Indicator = TranType_REPLY
                SQM.SSM_Common_Data.CRF_Authorization_Code = RND.Next(100000, 999999)
                SQM.SSM_Common_Data.CRF_Response_Code = nSQM.SSM_Common_Data.CRF_Response_Code
                SQM.SSM_Common_Data.CRF_Names = nSQM.SSM_Common_Data.CRF_Names
                SQM.SSM_Common_Data.CRF_Secondary_Account = nSQM.SSM_Common_Data.CRF_Secondary_Account
                Console.WriteLine("PRIMARY ACCT:" & nSQM.SSM_Common_Data.CRF_Primary_Account)
                '**********************************************************************************************
                Put_Message_To_Router(SQM, SQM.SSM_Rout_Queue_Reply_Name)
                '**********************************************************************************************
            Else
                Show_Message_Console("Receiving a invalidad message Key:" & keyHT, COLOR_BLACK, COLOR_YELLOW, 0, TRACE_LOW, 1)
                SaveLogMain("Receiving a invalidad message Key:" & keyHT)
                Reply_Exception_To_Source(SQM, condError_ROUTER_UNAVAILABLE)
                Exit Sub
            End If
        Else
            SaveLogMain("Error cargando el XML de respuesta desde el WS")
            Console.WriteLine("Error cargando el XML de respuesta desde el WS")
            Reply_Exception_To_Source(SQM, condError_UNDEFINED)
        End If

Exiting:
        ED = Nothing
        SQM = Nothing
        nSQM = Nothing
    End Sub


    Private Function Load_Response_From_XML_Reply(ByRef Struct_Queue_Message As SharedStructureMessage, ByVal XMLReply As String) As Byte
        Dim ErrorCode As Byte
        Dim USER_DATA As String = String.Empty
        Try
            Dim XmlArrayList As New List(Of String)
            Dim Message_XML As New XmlDocument
            Try
                Message_XML.LoadXml(XMLReply)
                Dim xml_node_LOAD As XmlNodeList
                xml_node_LOAD = Message_XML.GetElementsByTagName("date_time")
                Struct_Queue_Message.SSM_Common_Data.CRF_Adquirer_Date_Time = xml_node_LOAD.ItemOf(0).InnerText

                xml_node_LOAD = Message_XML.GetElementsByTagName("sequence_number")
                Struct_Queue_Message.SSM_Common_Data.CRF_Adquirer_Sequence = xml_node_LOAD.ItemOf(0).InnerText

                xml_node_LOAD = Message_XML.GetElementsByTagName("transaction_code")
                Struct_Queue_Message.SSM_Common_Data.CRF_Transaction_Code = xml_node_LOAD.ItemOf(0).InnerText

                xml_node_LOAD = Message_XML.GetElementsByTagName("terminal_id")
                Struct_Queue_Message.SSM_Common_Data.CRF_Terminal_ID = xml_node_LOAD.ItemOf(0).InnerText

                xml_node_LOAD = Message_XML.GetElementsByTagName("response_code")
                Struct_Queue_Message.SSM_Common_Data.CRF_Response_Code = xml_node_LOAD.ItemOf(0).InnerText
                Console.WriteLine("RESPONSE CODE:" & Struct_Queue_Message.SSM_Common_Data.CRF_Response_Code)

                xml_node_LOAD = Message_XML.GetElementsByTagName("source_reference")
                Struct_Queue_Message.SSM_Common_Data.CRF_Reference = xml_node_LOAD.ItemOf(0).InnerText

                xml_node_LOAD = Message_XML.GetElementsByTagName("target_account_name")
                Struct_Queue_Message.SSM_Common_Data.CRF_Names = xml_node_LOAD.ItemOf(0).InnerText

                xml_node_LOAD = Message_XML.GetElementsByTagName("target_account_number")
                Struct_Queue_Message.SSM_Common_Data.CRF_Secondary_Account = xml_node_LOAD.ItemOf(0).InnerText

                xml_node_LOAD = Message_XML.GetElementsByTagName("target_identification")
                Dim DocNbr As String = xml_node_LOAD.ItemOf(0).InnerText
                ED.Build_User_Token(USER_DATA, TARGET_DOCUMENT_ID, DocNbr.Trim)
                Struct_Queue_Message.SSM_Common_Data.CRF_Token_Data += USER_DATA
                ErrorCode = SUCCESSFUL
                'Dim xmlElem = XElement.Parse(XMLReply)
                'Console.WriteLine(xmlElem)
                'xmlElem = Nothing
            Catch ex As Exception
                Struct_Queue_Message.SSM_Common_Data.CRF_Response_Code = condError_CANT_PROCESS
                SaveLogMain(GetDateTime() & " ERROR-2 Load_Response_From_XML_Reply:" & ex.Message)
                Show_Message_Console(GetDateTime() & " Error cargando XML de respuesta 1", COLOR_BLACK, COLOR_RED, 0, TRACE_LOW, 0)
                ErrorCode = PROCESS_ERROR
            End Try
        Catch ex As Exception
            Struct_Queue_Message.SSM_Common_Data.CRF_Response_Code = condError_CANT_PROCESS
            SaveLogMain(GetDateTime() & " ERROR-3 Load_Response_From_XML_Reply:" & ex.Message)
            If (XMLReply.Length = 0) Or IsNothing(XMLReply) Then
                SaveLogMain(GetDateTime() & " BUFFER_LENGTH:0")
            Else
                SaveLogMain(GetDateTime() & " BUFFER_LENGTH:" & XMLReply.Length & " DATA=" & XMLReply)
            End If
            Show_Message_Console(GetDateTime() & " Error cargando XML de respuesta 2", COLOR_BLACK, COLOR_RED, 0, TRACE_LOW, 0)
            ErrorCode = 1
        End Try
        Return ErrorCode
    End Function

    Private Function RegistryFinantialRequest(ByVal MainQueueStruct As SharedStructureMessage, ByVal KeyHT As String) As Byte
        Dim ErrorCode As Byte = PROCESS_ERROR

        SyncLock lockOBJ0
            Try
                HTOriginalRequest.Add(KeyHT, MainQueueStruct)
                Wait_Pending_For_Timeout(KeyHT)
                ErrorCode = SUCCESSFUL
            Catch ex As Exception
                SaveLogMain(" ******** NO SE REGISTRO TRX :" & KeyHT & Chr(13) & ex.Message)
            End Try
        End SyncLock

        Return ErrorCode
    End Function

    Private Function RetrieveOriginalRequest(ByRef MainQueueStruct As SharedStructureMessage, ByVal KeyHT As String) As Byte
        Dim ErrorCode As Byte = 0
        SyncLock lockOBJ1
            Try
                If HTOriginalRequest.ContainsKey(KeyHT) Then
                    Console.WriteLine("Contiene KEY")
                    For Each DicEnt In HTOriginalRequest
                        If DicEnt.key = KeyHT Then
                            MainQueueStruct = DicEnt.Value
                            HTOriginalRequest.Remove(KeyHT)
                            Console.WriteLine("Elimina KEY")
                            Exit For
                        End If
                    Next
                    ErrorCode = 0
                Else
                    Console.WriteLine("NO Contiene KEY")
                    ErrorCode = 1
                End If
            Catch ex As Exception
                Show_Message_Console(" Can't retrieve original request:" & KeyHT & " " & ex.Message, COLOR_BLACK, COLOR_RED, 0, TRACE_LOW, 1)
                ErrorCode = 1
            End Try
        End SyncLock
        Return ErrorCode
    End Function

    Public Async Sub Wait_Pending_For_Timeout(ByVal KeyHT As String)
        Dim ErrorCode As Byte
        Await Task.Run(Async Function()
                           ErrorCode = Await Waiting_TimeOut_Request(KeyHT)
                       End Function)
    End Sub

    Private Async Function Waiting_TimeOut_Request(ByVal KeyHT As String) As Task(Of Byte)
        Console.WriteLine(GetDateTime() & " To Sleep..." & KeyHT & " " & System.Threading.Thread.CurrentThread.ManagedThreadId)
        Console.WriteLine(GetDateTime() & " timeout=" & Mod_Timeout)
        Await Task.Delay(CLng(Mod_Timeout))
        Console.WriteLine(GetDateTime() & " WakeUP..." & KeyHT & " " & System.Threading.Thread.CurrentThread.ManagedThreadId)

        If HTOriginalRequest.Count > 0 Then
            If HTOriginalRequest.ContainsKey(KeyHT) Then
                Show_Message_Console(MyName & " Time out of response Key:" & KeyHT, COLOR_BLACK, COLOR_YELLOW, 0, TRACE_LOW, 1)
                Dim MainQueueStruct As SharedStructureMessage = HTOriginalRequest.Item(KeyHT)
                HTOriginalRequest.Remove(KeyHT)
                Reply_Exception_To_Source(MainQueueStruct, condError_TIMEOUT)
                Put_Notify_To_Router(Router_NOTIFY & "1|0|" & MainQueueStruct.SSM_Common_Data.CRF_Issuer_Institution_Number)

                Console.WriteLine("CONTROL # 1 ---------------------------------")
                If MainQueueStruct.SSM_Common_Data.CRF_Transaction_Code = 539 Then
                    MainQueueStruct.SSM_Common_Data.CRF_Reversal_Indicator = 1
                    Console.WriteLine("CONTROL # 2 ---------------------------------")
                    ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf Process_Build_Send_Conditional_Issuer), MainQueueStruct)
                End If

            End If
        End If

        Return SUCCESSFUL
    End Function


    Public Sub Process_Build_Send_Conditional_Issuer(ByVal SQM As SharedStructureMessage)

        Show_Message_Console("INICIO Process_Build_Send_Conditional_Issuer", COLOR_WHITE, COLOR_RED, 0, TRACE_LOW, 0)

        Dim ErrorCode As Byte
        Dim Token_Data As String = String.Empty
        Dim TKNid(20) As String
        Dim TKNdata(20) As String
        Dim BanredSequence As Int32 = SQM.SSM_Common_Data.CRF_Adquirer_Sequence

        Dim ED As New ServiSwitch_EncodeDecodeClass
        Try
            If SQM.SSM_Common_Data.CRF_Token_Data.Length > 0 Then
                Dim x, y As Integer
                Token_Data = SQM.SSM_Common_Data.CRF_Token_Data
                Do While Token_Data.Length > 0
                    TKNid(x) = Token_Data.Substring(0, 2)
                    Token_Data = Token_Data.Remove(0, 2)
                    y = Token_Data.Substring(0, 3)
                    Token_Data = Token_Data.Remove(0, 3)
                    TKNdata(x) = Token_Data.Substring(0, y)
                    Token_Data = Token_Data.Remove(0, y)
                    Dim TMP As String = TKNdata(x)
                    TMP = TMP.Replace(" ", Nothing)
                    If TMP.Length = 0 Then
                        TKNdata(x) = TKNdata(x).Replace(" ", "0")
                    End If
                    x += 1
                Loop
                Array.Resize(TKNid, x)
                Array.Resize(TKNdata, x)
            End If
            ErrorCode = SUCCESSFUL
        Catch ex As Exception
            ErrorCode = PROCESS_ERROR
            SaveLogMain(GetDateTime() & " Excepcion recuperando Tokens de entrada")
            Show_Message_Console(GetDateTime() & " Excepcion recuperando Tokens de entrada", COLOR_BLACK, COLOR_RED, 0, TRACE_LOW, 0)
        End Try
        If ErrorCode <> SUCCESSFUL Then
            Show_Message_Console(MyName & " Transaction was not processed by exception", COLOR_BLACK, COLOR_YELLOW, 0, TRACE_LOW, 0)
            SaveLogMain(GetDateTime() & " Salgo por Error")
            Reply_Exception_To_Source(SQM, condError_CANT_PROCESS)
            Exit Sub
        End If

        Dim RequestXML As String
        Try
            RequestXML = ED.Generate_XML_Request(SQM.SSM_Common_Data.CRF_Terminal_ID,
                                                 SQM.SSM_Common_Data.CRF_Adquirer_Sequence,
                                                 SQM.SSM_Common_Data.CRF_Transaction_Code,
                                                 SQM.SSM_Common_Data.CRF_Adquirer_Date_Time,
                                                 SQM.SSM_Common_Data.CRF_Adquirer_Institution_Number,
                                                 SQM.SSM_Common_Data.CRF_Channel_Id,
                                                 ED.Get_Token_Info(NAMES_ID, TKNid, TKNdata),
                                                 SQM.SSM_Common_Data.CRF_Transaction_Amount,
                                                 SQM.SSM_Common_Data.CRF_Primary_Account,
                                                 SQM.SSM_Common_Data.CRF_Secondary_Account,
                                                 ED.Get_Token_Info(TARGET_ACCT_TYPE_ID, TKNid, TKNdata),
                                                 SQM.SSM_Common_Data.CRF_Branch_ID,
                                                 SQM.SSM_Common_Data.CRF_Operator_ID,
                                                 ED.Get_Token_Info(ACCT_TYPE_ID, TKNid, TKNdata),
                                                 SQM.SSM_Common_Data.CRF_PhoneNumber,
                                                 SQM.SSM_Common_Data.CRF_Reversal_Indicator.ToString,
                                                 SQM.SSM_Common_Data.CRF_Names,
                                                 SQM.SSM_Common_Data.CRF_Reference,
                                                 ED.Get_Token_Info(TARGET_DOCUMENT_ID, TKNid, TKNdata),
                                                 ED.Get_Token_Info(DOCUMENT_ID, TKNid, TKNdata))
        Catch ex As Exception
            SaveLogMain("Excepcion Armando Buffer XML de requerimiento:" & ex.Message)
            Console.WriteLine("Excepcion Armando Buffer XML de requerimiento:" & ex.Message)
            Exit Sub
        End Try

        ' *********************************
        ' EXECUTE WEB SERVICE AUTHORIZATION
        ' *********************************

        Show_Message_Console(MyName & " TranCode:" & SQM.SSM_Common_Data.CRF_Transaction_Code & " SeqNbr:" & SQM.SSM_Common_Data.CRF_Adquirer_Sequence, COLOR_WHITE, COLOR_BLUE, 0, TRACE_LOW, 0)
        Dim StringDateTime = SQM.SSM_Common_Data.CRF_Adquirer_Date_Time.ToString("yyyy-MM-dd HH:mm:ss")
        Dim keyHT As String = CInt(SQM.SSM_Common_Data.CRF_Terminal_ID) & CInt(SQM.SSM_Common_Data.CRF_Adquirer_Sequence) & CInt(SQM.SSM_Common_Data.CRF_Transaction_Code) & CLng(SQM.SSM_Common_Data.CRF_Secondary_Account)
        SQM.SSM_Common_Data.CRF_Currency_Code += 1

        Show_Message_Console("SQM.SSM_Common_Data.CRF_Currency_Code:" & SQM.SSM_Common_Data.CRF_Currency_Code, COLOR_YELLOW, COLOR_BLACK, 0, TRACE_LOW, 0)
        If SQM.SSM_Common_Data.CRF_Currency_Code > 3 Then
            Show_Message_Console(" Ending Reverse Conditional Process ....", COLOR_YELLOW, COLOR_BLACK, 0, TRACE_LOW, 0)
            Exit Sub
        End If

        RegistryFinantialRequest(SQM, keyHT)

        Dim ReplyXML As String = ""
        Dim dt1 As DateTime = Now
        Dim WSClient As New ServiSwitch_WSClient
        Try
            ReplyXML = WSClient.Call_WSClient(My.Settings.WSDL, RequestXML)
        Catch ex As Exception
            Show_Message_Console(MyName & " Transaction was not processed by WS exception:" & ex.Message, COLOR_BLACK, COLOR_YELLOW, 0, TRACE_LOW, 1)
            SaveLogMain(GetDateTime() & " ERROR-1 WSClient.Call_WSClient:" & ex.Message)
            Exit Sub
        End Try

        Dim nSQM As New SharedStructureMessage
        Dim dt2 As DateTime = Now
        SaveLogMain(ReplyXML)

        Dim TSP As New TimeSpan
        TSP = dt2.Subtract(dt1)
        If TSP.TotalMilliseconds > Mod_Timeout Then
            Console.WriteLine("TIEMPO TOTAL ---> Seg:" & TSP.Seconds & " Mls:" & TSP.Milliseconds & " Seq:" & BanredSequence & " LenReply:" & ReplyXML.Length & " Timeout")
            SaveLogMain("TIEMPO TOTAL ---> Seg:" & TSP.Seconds & " Mls:" & TSP.Milliseconds & " Seq:" & BanredSequence & " LenReply:" & ReplyXML.Length & " Timeout")
        Else
            Console.WriteLine("TIEMPO TOTAL ---> Seg:" & TSP.Seconds & " Mls:" & TSP.Milliseconds & " Seq:" & BanredSequence & " LenReply:" & ReplyXML.Length & " Tiempo OK")
            SaveLogMain("TIEMPO TOTAL ---> Seg:" & TSP.Seconds & " Mls:" & TSP.Milliseconds & " Seq:" & BanredSequence & " LenReply:" & ReplyXML.Length & " Tiempo OK")
        End If

        If Load_Response_From_XML_Reply(nSQM, ReplyXML) = SUCCESSFUL Then
            keyHT = CInt(nSQM.SSM_Common_Data.CRF_Terminal_ID) & CInt(nSQM.SSM_Common_Data.CRF_Adquirer_Sequence) & CInt(nSQM.SSM_Common_Data.CRF_Transaction_Code) & CLng(nSQM.SSM_Common_Data.CRF_Secondary_Account)
            Show_Message_Console("KEY RECEIVE:" & keyHT, COLOR_BLACK, COLOR_YELLOW, 0, TRACE_LOW, 0)
        Else
            SaveLogMain("Error cargando el XML de respuesta desde el WS")
            Console.WriteLine("Error cargando el XML de respuesta desde el WS")
            Reply_Exception_To_Source(SQM, condError_UNDEFINED)
        End If

Exiting:
        ED = Nothing
        SQM = Nothing
        nSQM = Nothing
    End Sub

End Class
