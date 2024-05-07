Imports System.Messaging
Imports System.Threading
Imports System.IO
Imports System.Xml
Imports System.Runtime.InteropServices

Module ServiSwitch_ShareFunctions

    Public Const AUTHPD_ID As String = "A1"

    Dim SeqCorrelationID As Int64
    Const Sumator As Int64 = 1000000

    Private Declare Auto Function SetProcessWorkingSetSize Lib "kernel32.dll" (ByVal procHandle As IntPtr, ByVal min As Int32, ByVal max As Int32) As Boolean
    Public Const SE_Duplicated_Record As Int16 = 2627
    Public Const SE_Duplicated_Key As Int16 = 2601

    Public Const error_RECORD_NOT_FOUND As Int16 = 7501
    Public Const error_NOT_PROCESS As Int16 = 8510
    Public Const error_NOT_AVAILABLE As Int16 = 7803
    Public Const error_INCORRECT_DATA As Int16 = 8038
    Public Const BROADCAST_SUCCESS_tran As Int16 = 181
    Public Const BROADCAST_DENIED_tran As Int16 = 182

    Public Const LoggerType As Byte = 4
    Public Const TranType_REQUIREMENT As Char = "R"
    Public Const ID_TOKEN_SERVICE As String = "! X1"
    Public Const INVALID_STRING As String = "XXXXXXXXXX"
    Public Const INVALID_NUMBER As String = "9999"
    Public Const Concatenator As Char = "|"
    Public Const PrivateQueue As String = ".\Private$\"
    Public Const DUMMY_Auth As String = "9999"
    Public Const TIMEOUT_HOST2 As int16 = 8364
    '************************************************
    Public Const TranType_ROUTER_IN_SCP As Char = "R"
    Public Const TranType_REPLY As Char = "P"
    Public Const TranType_COMMAND As Char = "C"
    Public Const TranType_ROUTER_OUT_SCP As Char = "O"
    '************************************************
    Public Const FUNCTION_ERROR As Byte = 2
    Public Const SUCCESSFUL As Byte = 0
    Public Const PROCESS_ERROR As Byte = 1
    Public Const DUPLICATED As Byte = 3
    Public Const WAIT As Byte = 4
    Public Const NOT_SEND As Byte = 5
    Public Const DELETE_RECORD As Byte = 6
    Public Const NOTIFY_RECORD As Byte = 7
    Public Const UNKNOW_ERROR As Byte = 99
    Public Const REQUEST_QUEUE As Byte = 0
    Public Const REPLY_QUEUE As Byte = 1
    Public Const ACK_QUEUE As Byte = 5
    Public Const Commander_TYPE As Byte = 5
    '************************************************
    Public Const COLOR_BLUE As Byte = 9
    Public Const COLOR_YELLOW As Byte = 14
    Public Const COLOR_RED As Byte = 12
    Public Const COLOR_BLACK As Byte = 0
    Public Const COLOR_DARK_GRAY As Byte = 8
    Public Const COLOR_GRAY As Byte = 7
    Public Const COLOR_GREEN As Byte = 10
    Public Const COLOR_WHITE As Byte = 15
    Public Const COLOR_OWNER1 As Byte = 17
    '************************************************
    Public Const TRACE_LOW As Byte = 81
    Public Const TRACE_MEDIUM As Byte = 82
    Public Const TRACE_HIGH As Byte = 83

    Public Const SW_SHOWMINNOACTIVE As Int32 = 7
    Public Const SW_SHOWNORMAL As Int32 = 1
    Public Const SW_HIDE As Int32 = 0

    Public Const ROUTER As Byte = 1
    Public Const listview_CONTROL As Byte = 0
    Public Const treeview_CONTROL As Byte = 1

    Public Const UNKNOW_status As Byte = 4
    Public Const STARTED_status As Byte = 5

    '************************************************
    Public Commander_Module_Name As String
    Public Commander_Queue_Name As String
    Public Router_Request_Queue_Name As String
    Public Router_Reply_Queue_Name As String
    Public Router_Command_Queue_Name As String
    Public Logger_Request_Queue_Name As String
    Public MyName As String
    Public Commander_Command_Name As String
    Public SAF_Router_Queue_Name As String

    Public g_RequestQueue As String = String.Empty
    Public g_ReplyQueue As String = String.Empty
    Public g_TcpQueue As String = String.Empty
    Public g_SafQueue As String = String.Empty
    Public g_CmdQueue As String = String.Empty
    Public g_AckQueue As String = String.Empty
    Public g_RouterReply As String = String.Empty
    Public g_RouterReplyQueue As String = String.Empty
    Public g_RouterRequestQueue As String = String.Empty
    Public g_CommanderQueue As String = String.Empty
    Public Mod_RouterName As String
    Public Const FI_CPN_AUT As Int16 = 728

    '************************************************
    Public Const commander_DISPLAY As String = "DPS|"
    Public Const commander_STATUS As String = "STA|"
    Public Const commander_REQUEST As String = "REQ|"

    Public Const GIVE_TRAN_DEFINITIONS As String = "0005|"
    Public Const GIVE_PROC_DEFINITIONS As String = "0006|"
    Public Const GIVE_ROUT_DEFINITIONS As String = "0008|"
    Public Const GIVE_INST_DEFINITIONS As String = "0010|"

    Public Const rep_GIVE_MAIN_INFO As String = "0000"
    Public Const rep_GIVE_COMD_DATA As String = "0001"
    Public Const rep_GIVE_ISO_DEFINITIONS As String = "0003"
    Public Const rep_GET_ALL_CONFIG As String = "0004"
    Public Const rep_GET_TRAN_CONFIG As String = "0005"
    Public Const rep_GIVE_PROC_DEFINITIONS As String = "0006"
    Public Const rep_GIVE_ROUT_DEFINITIONS As String = "0008"
    Public Const rep_GIVE_INST_DEFINITIONS As String = "0010"

    Dim ProcessNames() As String = {"Name", "Id", "Type", "Institution", "Task", "Instance", "Timeout", "Address", "Port", _
                                    "SocketMode", "Queue_Request_messages", "Queue_Reply_messages", "Queue_Tcp_messages", _
                                    "Queue_Saf_messages", "Queue_Cmd_messages", "Router", "Format", "Queue_Ack_messages"}


    Dim process_PTR As IntPtr

    Public Const inquiry_TRANSFER As Int16 = 165
    Public Const checking_TRANSFER As Int16 = 439
    Public Const saving_TRANSFER As Int16 = 539
    Public Const Credits_CARDS As Int16 = 239
    Dim DIC_MAINp As New Dictionary(Of String, Main_Record)


    '************************************************
    Dim TO_COMMANDER As New InfoCommands
    '************************************************
    Public User_Detail As Byte = 81
    Public Send2Manager As Boolean = False
    Public Exiting As Boolean = False
    '************************************************

    Public Function GetPathQueueNameBySystemName(ByVal SystemName As String, ByVal Mode As Byte, ByRef QueueName As String, ByRef IssuerID As Int32) As Byte
        Dim ErrorCode As Byte = PROCESS_ERROR
        Dim x As Int16 = 0
        Dim QueueNames(5) As String
        SystemName = SystemName.ToUpper
        Try
            Dim xelement As XElement = xelement.Parse(Marshal.PtrToStringAuto(process_PTR))
            Dim name As Object = From nm In xelement.Elements("body_record") _
                  Where nm.Element("Name") = SystemName
                  Select nm
            For Each xEle As XElement In name
                IssuerID = CInt(xEle.Elements.ElementAt(1).Value)
                QueueNames(0) = xEle.Elements.ElementAt(10).Value
                QueueNames(1) = xEle.Elements.ElementAt(11).Value
                QueueNames(2) = xEle.Elements.ElementAt(12).Value
                QueueNames(3) = xEle.Elements.ElementAt(13).Value
                QueueNames(4) = xEle.Elements.ElementAt(14).Value
                QueueNames(5) = xEle.Elements.ElementAt(17).Value
            Next xEle
            QueueName = PrivateQueue & QueueNames(Mode)
            ErrorCode = SUCCESSFUL
        Catch ex As Exception
            ErrorCode = UNKNOW_ERROR
        End Try
        QueueNames = Nothing
        GC.Collect()
        Return ErrorCode
    End Function

    Public Function GetModuleName(ByVal IssuerID As Int32, ByRef SystemName As String) As Byte
        Dim ErrorCode As Byte = PROCESS_ERROR
        Try
            Dim xmldoc As XDocument = XDocument.Parse(Marshal.PtrToStringAuto(process_PTR))
            Dim ql As XElement = (From ls In xmldoc.Root.Elements("body_record") _
                    Where ls.Element("Id").Value = IssuerID.ToString _
                    Select ls.Element("Name")).FirstOrDefault
            If Not IsNothing(ql) Then
                SystemName = ql.Value
                ErrorCode = SUCCESSFUL
            End If
        Catch ex As Exception
            ErrorCode = UNKNOW_ERROR
        End Try
        GC.Collect()
        Return ErrorCode
    End Function

    Public Function Get_Logger_Queue() As String
        Dim QueueName As String = INVALID_STRING
        Try
            Dim xmldoc As XDocument = XDocument.Parse(Marshal.PtrToStringAuto(process_PTR))
            Dim ql As XElement = (From ls In xmldoc.Root.Elements("body_record") _
                    Where ls.Element("Type").Value = LoggerType _
                    Select ls.Element("Queue_Request_messages")).FirstOrDefault
            If Not IsNothing(ql) Then
                QueueName = ql.Value
            End If
        Catch ex As Exception
            Show_Message_Console(MyName & " request queue for logger not available ", COLOR_BLACK, COLOR_YELLOW, 0, TRACE_LOW, 1)
        End Try
        GC.Collect()
        Return QueueName
    End Function

    Public Function Get_Logger_Name() As String
        Dim LoggerName As String = INVALID_STRING
        Try
            Dim xmldoc As XDocument = XDocument.Parse(Marshal.PtrToStringAuto(process_PTR))
            Dim ql As XElement = (From ls In xmldoc.Root.Elements("body_record") _
                    Where ls.Element("Type").Value = LoggerType _
                    Select ls.Element("Name")).FirstOrDefault
            If Not IsNothing(ql) Then
                LoggerName = ql.Value
            End If
        Catch ex As Exception
            Show_Message_Console(MyName & " name for logger not available ", COLOR_BLACK, COLOR_YELLOW, 0, TRACE_LOW, 1)
        End Try
        GC.Collect()
        Return LoggerName
    End Function

    'Public Function GetDataByID(ByVal SystemName As String, ByVal Mode As Byte) As String
    '    Dim GenericData As String = ""
    '    Try
    '        Dim xelement As XElement = xelement.Parse(Marshal.PtrToStringAuto(process_PTR))
    '        Dim name As Object = From nm In xelement.Elements("body_record") _
    '              Where nm.Element("Name") = SystemName
    '              Select nm
    '        For Each xEle As XElement In name
    '            Select Case Mode
    '                Case 0
    '                    GenericData = xEle.Elements.ElementAt(4).Value
    '            End Select
    '        Next xEle
    '    Catch ex As Exception
    '        Show_Message_Console(MyName & " generic for " & SystemName & " not available ", COLOR_BLACK, COLOR_YELLOW, 0, TRACE_LOW, 1)
    '    End Try
    '    GC.Collect()
    '    Return GenericData
    'End Function


    Public Sub Put_Message_To_Adq(ByVal Struct_Shared_Message As SharedStructureMessage, ByVal SystemQueueName As String)
        Dim lockOBJ_IN As Object = New Object
        '
        ' On WebService Adquirence Can't get the Id Process
        '
        If Struct_Shared_Message.SSM_Communication_ID = Constanting_Definition.from_WEBSERVICE Then
            Reply_Message_To_WSadq(Struct_Shared_Message, SystemQueueName)
            Exit Sub
        End If

        SyncLock lockOBJ_IN
            Try
                Dim MessageToSend As Message = New Message
                Dim QueueSendData As New MessageQueue(SystemQueueName)
                MessageToSend.Body = Struct_Shared_Message
                QueueSendData.Send(MessageToSend)
                QueueSendData.Dispose()
                MessageToSend.Dispose()
                Struct_Shared_Message = Nothing
            Catch ex As Exception
                Show_Message_Console(MyName & " Exception Putting Message on " & SystemQueueName & " ->" & ex.Message, COLOR_OWNER1, COLOR_RED, 0, TRACE_LOW, 1)
            End Try
        End SyncLock

    End Sub

    Private Sub Reply_Message_To_WSadq(ByVal Struct_Shared_Message As SharedStructureMessage, ByVal SystemQueueName As String)
        Dim lockOBJ_IN As Object = New Object

        SyncLock lockOBJ_IN
            Try
                Dim arrTypes(1) As System.Type
                arrTypes(0) = GetType(SharedStructureMessage)
                arrTypes(1) = GetType(Object)

                Dim msgReply As New System.Messaging.Message
                Dim msgQ As New MessageQueue(SystemQueueName)
                msgQ.Formatter = New XmlMessageFormatter(arrTypes)

                With msgReply
                    .Body = Struct_Shared_Message
                    .TimeToBeReceived = New TimeSpan(0, 0, 20) ' 5 seconds
                    .CorrelationId = Struct_Shared_Message.SSM_Queue_Message_ID
                    .Label = " From Routers"
                End With

                Try
                    msgQ.Send(msgReply, MessageQueueTransactionType.Single)
                Catch ex As Exception
                    Show_Message_Console(" Error enviando mensaje a la cola " & SystemQueueName, COLOR_BLACK, COLOR_RED, 0, TRACE_LOW, 1)
                End Try

                Struct_Shared_Message = Nothing
                msgReply.Dispose()
                msgQ.Dispose()
            Catch ex As Exception
                Show_Message_Console(MyName & " Exception Putting Message on " & SystemQueueName & " ->" & ex.Message, COLOR_OWNER1, COLOR_RED, 0, TRACE_LOW, 1)
            End Try
        End SyncLock
    End Sub


    Public Sub Put_Message_To_Manager(ByVal Data As String, ByVal LocalThreadQueueName As String)

        'Console.Write("1")
        If Not Verify_Process_Up(Commander_Module_Name) Then
            'Console.Write("2")
            'Console.Write(Commander_Module_Name)
            'Show_Message_Console("Process commander not active,", COLOR_BLACK, COLOR_YELLOW, 3, TRACE_MEDIUM, 0)
            'Console.Write("3")
            Exit Sub
        End If
        'Console.Write("4")
        Try
            Dim MessageToSend As Message = New Message
            Dim QueueSendData As New MessageQueue(LocalThreadQueueName)
            TO_COMMANDER.ICM_CommandBuffer = Data
            MessageToSend.Body = TO_COMMANDER
            MessageToSend.TimeToBeReceived = New TimeSpan(0, 0, 5)
            QueueSendData.Send(MessageToSend)
            QueueSendData.Dispose()
            MessageToSend.Dispose()
            TO_COMMANDER.ICM_CommandBuffer = Nothing
            'Console.Write("5")
        Catch ex As Exception
            Show_Message_Console(MyName & "ERROR PutMessage Manager " & ex.Message, COLOR_OWNER1, COLOR_RED, 0, TRACE_LOW, 1)
            'Console.Write("6")
        End Try
        'Console.Write("7")

    End Sub


    Public Function Verify_Process_Up(ByVal ProcessName As String) As Boolean
        Dim PathProcessName As String = String.Empty
        Dim FoundIT As Boolean = False

        'Console.WriteLine("Nombre:" & ProcessName)

        Try
            Dim ServiProcess As List(Of Process) = (From p As Process In Process.GetProcesses Where p.ProcessName.ToUpper Like "ServiSwitch*".ToUpper).ToList
            'Console.WriteLine("Procesos:" & ServiProcess.Count)
            For Each p As Process In ServiProcess
                'Console.WriteLine("Proceso:" & p.ProcessName)
                PathProcessName = p.MainModule.FileName()
                'Console.WriteLine(ProcessName & " " & PathProcessName)
                If PathProcessName.Contains("\" & ProcessName.Trim & "\") Then
                    FoundIT = True
                    Exit For
                End If
            Next
        Catch ex As Exception
            FoundIT = False
            Console.WriteLine("Exception verifing processUP:" & ProcessName & " " & ex.Message)
        End Try
        'Console.WriteLine("Encontrado:" & FoundIT)
        Return FoundIT

    End Function

    Public Function Verify_Queue_Resource(ByVal QueueName As String) As Byte
        Dim ErrorCode As Byte = FUNCTION_ERROR
        Try
            Dim DFXqueue As MessageQueue = New MessageQueue(QueueName)
            If Not MessageQueue.Exists(QueueName) Then
                MessageQueue.Create(QueueName)
                DFXqueue.SetPermissions("everyone", MessageQueueAccessRights.FullControl, AccessControlEntryType.Allow)
                DFXqueue.SetPermissions("IIS_IUSRS", MessageQueueAccessRights.FullControl, AccessControlEntryType.Allow)
                DFXqueue.SetPermissions("IUSR", MessageQueueAccessRights.FullControl, AccessControlEntryType.Allow)
                Show_Message_Console(MyName & " Queue has been created:" & QueueName, COLOR_BLACK, COLOR_GREEN, 0, TRACE_LOW, 0)
                ErrorCode = SUCCESSFUL
            Else
                Show_Message_Console(MyName & " Queue has been verified:" & QueueName, COLOR_BLACK, COLOR_GREEN, 0, TRACE_LOW, 0)
                ErrorCode = SUCCESSFUL
            End If
        Catch ex As Exception
            Show_Message_Console(MyName & " Exception on managing Queue:" & QueueName & "->" & ex.Message, COLOR_OWNER1, COLOR_RED, 0, TRACE_LOW, 0)
            ErrorCode = 1
        End Try
        Return ErrorCode

    End Function

    Public Function Get_Token_Data(ByVal TokenId As String, ByVal PassTokenField As String) As String
        Dim TokenData As String = ""
        Dim TknId As New List(Of String)
        Dim TknData As New List(Of String)
        Try
            If PassTokenField.Length > 0 Then
                Dim x, y As Integer
                Do While PassTokenField.Length > 0
                    TknId.Add(PassTokenField.Substring(0, 2))
                    PassTokenField = PassTokenField.Remove(0, 2)
                    y = PassTokenField.Substring(0, 3)
                    PassTokenField = PassTokenField.Remove(0, 3)
                    TknData.Add(PassTokenField.Substring(0, y))
                    PassTokenField = PassTokenField.Remove(0, y)
                    x += 1
                Loop
            End If
        Catch ex As Exception
            Console.WriteLine("Error # 001:" & ex.Message)
        End Try

        If TknId.Contains(TokenId) Then
            Dim Idx As Byte = TknId.IndexOf(TokenId)
            TokenData = TknData.Item(Idx)
        End If

        Return TokenData
    End Function


    Public Sub Build_User_Token(ByRef USER_DATA As String, ByVal Token_Id As String, ByRef InputData As String)
        If IsNothing(InputData) Or (InputData.Length = 0) Then
            InputData = " "
        End If
        Try
            USER_DATA += Token_Id & InputData.Length.ToString("000") & InputData
        Catch ex As Exception
            USER_DATA += Token_Id & "000"
        End Try
    End Sub




    'Public Function GetPathQueueName(ByVal SystemType As Int16, ByVal Mode As Byte, ByRef ReferenceName As String) As Byte
    '    Dim ErrorCode As Byte = PROCESS_ERROR
    '    Dim QueueNames(6) As String
    '    Try
    '        Dim xelement As XElement = xelement.Parse(Marshal.PtrToStringAuto(process_PTR))
    '        Dim name As Object = From nm In xelement.Elements("body_record") _
    '              Where nm.Element("Type") = SystemType.ToString
    '              Select nm
    '        For Each xEle As XElement In name
    '            QueueNames(0) = xEle.Elements.ElementAt(10).Value
    '            QueueNames(1) = xEle.Elements.ElementAt(11).Value
    '            QueueNames(2) = xEle.Elements.ElementAt(12).Value
    '            QueueNames(3) = xEle.Elements.ElementAt(13).Value
    '            QueueNames(4) = xEle.Elements.ElementAt(14).Value
    '            QueueNames(5) = xEle.Elements.ElementAt(17).Value
    '            QueueNames(6) = xEle.Elements.ElementAt(0).Value
    '        Next xEle
    '        If Mode = 6 Then
    '            ReferenceName = QueueNames(Mode)
    '        Else
    '            ReferenceName = PrivateQueue & QueueNames(Mode)
    '        End If
    '        ErrorCode = SUCCESSFUL
    '    Catch ex As Exception
    '        ErrorCode = UNKNOW_ERROR
    '    End Try
    '    QueueNames = Nothing
    '    GC.Collect()
    '    Return ErrorCode
    'End Function

    Public Sub Show_Message_Console(ByVal TextData As String, ByVal ColorB As Byte, ByVal ColorF As Byte, ByVal Mode As Byte, ByVal Detail As Byte, ByVal BypassToManager As Byte)

        'If Detail > User_Detail Then
        '    Exit Sub
        'End If

        Dim lockOBJ_IN As Object = New Object
        SyncLock lockOBJ_IN
            Console.ForegroundColor = ColorF
            If ColorB = 17 Then
                Console.BackgroundColor = ConsoleColor.Black
            Else
                Console.BackgroundColor = ColorB
            End If

            Select Case Mode
                Case 0
                    TextData = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") & " " & TextData
                    Console.WriteLine(TextData)
                Case 1
                    TextData = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") & " " & TextData
                    Console.Write(TextData)
                Case 2
                    Console.WriteLine(TextData)
                Case 3
                    Console.Write(TextData)
            End Select

            If Send2Manager And (BypassToManager = 1) Then
                If ColorB = COLOR_BLACK Then
                    ColorB = COLOR_WHITE
                End If
                If ColorF <> COLOR_RED Then
                    ColorF = COLOR_BLACK
                End If
                If (ColorB = COLOR_RED) And (ColorF = COLOR_BLACK) Then
                    ColorF = COLOR_WHITE
                End If
                Put_Message_To_Manager(commander_DISPLAY & ColorB & "#" & ColorF & "#" & TextData, Commander_Command_Name)
            End If

            TextData = Nothing

        End SyncLock
    End Sub

    Public Sub Notify_Process_Status(ByVal Command_Notify As String)

        Put_Message_To_Manager(commander_STATUS & Command_Notify, Commander_Queue_Name)

    End Sub

    Friend Sub ClearMemory()
        Try
            GC.Collect()
            GC.WaitForPendingFinalizers()
            If Environment.OSVersion.Platform = PlatformID.Win32NT Then
                SetProcessWorkingSetSize(System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1)
            End If
        Catch ex As Exception
            Show_Message_Console(MyName & " Exception:" & ex.Message, COLOR_BLACK, COLOR_RED, 0, TRACE_LOW, 1)
        End Try
    End Sub



    Public Function Load_Setting_Database() As Byte
        Dim ErrorCode As Byte = FUNCTION_ERROR

        '******************************************************
        If Init_Security_Resources() <> SUCCESSFUL Then
            Return ErrorCode
        End If
        '******************************************************
        Load_Process_Definition(ErrorCode)
        If ErrorCode = PROCESS_ERROR Then
            Return ErrorCode
        End If
        '******************************************************
        ' LOADING COMMANDER QUEUE NAME
        Dim Mode As Byte
        Mode = 6
        If GetPathQueueName(Commander_TYPE, Mode, Commander_Module_Name) <> SUCCESSFUL Then
            Return ErrorCode
        End If

        '******************************************************
        ' LOADING COMMANDER QUEUE NAME
        Mode = 1
        If GetPathQueueName(Commander_TYPE, Mode, Commander_Queue_Name) <> SUCCESSFUL Then
            Return ErrorCode
        End If

        '******************************************************
        ' LOADING COMMANDER COMMAND QUEUE NAME
        Mode = 4
        If GetPathQueueName(Commander_TYPE, Mode, Commander_Command_Name) <> SUCCESSFUL Then
            Return ErrorCode
        End If

        '******************************************************
        ' LOADING ROUTER MODULE NAME
        Mode = 5
        Mod_RouterName = GetDataByID(MyName, Mode)
        Console.WriteLine("Router:" & Mod_RouterName)

        '******************************************************
        ' LOADING ROUTER QUEUE REQUEST NAME
        Mode = 0
        If GetPathQueueName(Mod_RouterName, Mode, Router_Request_Queue_Name) <> SUCCESSFUL Then
            Return ErrorCode
        End If

        '******************************************************
        ' LOADING ROUTER QUEUE REPLY NAME
        Mode = 1
        If GetPathQueueName(Mod_RouterName, Mode, Router_Reply_Queue_Name) <> SUCCESSFUL Then
            Return ErrorCode
        End If

        '******************************************************
        ' LOADING LOCAL MODULE REQUEST QUEUE NAME
        Mode = 0
        If GetPathQueueName(MyName, Mode, g_RequestQueue) <> SUCCESSFUL Then
            Return ErrorCode
        End If

        '******************************************************
        ' LOADING LOCAL MODULE REQUEST QUEUE NAME
        Mode = 1
        If GetPathQueueName(MyName, Mode, g_ReplyQueue) <> SUCCESSFUL Then
            Return ErrorCode
        End If

        '******************************************************
        ' LOADING LOCAL MODULE COMMAND QUEUE NAME
        Mode = 4
        If GetPathQueueName(MyName, Mode, g_CmdQueue ) <> SUCCESSFUL Then
            Return ErrorCode
        End If


        '******************************************************
        ' LOADING LOCAL MODULE TCPIP QUEUE NAME
        Mode = 2
        If GetPathQueueName(MyName, Mode, g_TcpQueue) <> SUCCESSFUL Then
            Return ErrorCode
        End If

        '******************************************************
        ' LOADING LOCAL MODULE TCPIP QUEUE NAME
        Mode = 3
        If GetPathQueueName(MyName, Mode, g_SafQueue) <> SUCCESSFUL Then
            Return ErrorCode
        End If


        Return SUCCESSFUL
    End Function

    'Private Sub Load_Process_Definition(ByVal ErrorCode As Byte)
    '    Dim ParamsTable As New List(Of String)
    '    ParamsTable.Clear()
    '    If Get_Table_Definition(ParamsTable, "sabd_main_definition", "*") = SUCCESSFUL Then
    '        Dim ParamsArray() As String = ParamsTable.ToArray
    '        SetGetProcess_PTR = Marshal.StringToHGlobalAuto(Build_XML_String(ParamsArray, ProcessNames))
    '        Show_Message_Console(" sabd_main_definition: Loaded", COLOR_BLACK, COLOR_GREEN, 0, TRACE_LOW, 0)
    '    Else
    '        Show_Message_Console(" No se pudo cargar tabla de configuracion sabd_main_definition", COLOR_BLACK, COLOR_RED, 0, TRACE_LOW, 1)
    '        ErrorCode = PROCESS_ERROR
    '    End If
    '    ParamsTable = Nothing
    '    GC.Collect()
    'End Sub

    Private Sub Load_Process_Definition(ByVal ErrorCode As Byte)
        DIC_MAINp.Clear()
        Dim ParamsTable As New List(Of String)
        If Get_Table_Definition(ParamsTable, "sabd_main_definition", "*", "#") = SUCCESSFUL Then
            Dim ParmsArray() As String = ParamsTable.ToArray
            For x As Integer = 0 To ParmsArray.Count - 1
                Dim LocParm() As String = ParmsArray(x).Split(New String() {"#"}, StringSplitOptions.RemoveEmptyEntries)
                Dim RecMain As New Main_Record
                RecMain.sabd_module_name = LocParm(0)
                RecMain.sabd_module_id = CInt(LocParm(1))
                RecMain.sabd_type = CInt(LocParm(2))
                RecMain.sabd_detail_name = LocParm(3)
                RecMain.sabd_task = CInt(LocParm(4))
                RecMain.sabd_instance = CInt(LocParm(5))
                RecMain.sabd_timeout = CInt(LocParm(6))
                RecMain.sabd_ip_address = LocParm(7)
                RecMain.sabd_port_number = CInt(LocParm(8))
                RecMain.sabd_connection_mode = CInt(LocParm(9))
                RecMain.sabd_queue_requirement = LocParm(10)
                RecMain.sabd_queue_replies = LocParm(11)
                RecMain.sabd_queue_comms = LocParm(12)
                RecMain.sabd_queue_saf = LocParm(13)
                RecMain.sabd_queue_command = LocParm(14)
                RecMain.sabd_assign_router = LocParm(15)
                RecMain.sabd_msg_format = CInt(LocParm(16))
                RecMain.sabd_queue_ack = LocParm(17)
                RecMain.sabd_status = CInt(LocParm(18))
                RecMain.sabd_executable = LocParm(19)
                RecMain.sabd_source_path = LocParm(20)
                RecMain.sabd_security_profile = LocParm(21)
                DIC_MAINp.Add(LocParm(0).Trim, RecMain)
            Next
            Show_Message_Console("sabd_main_definition: Loaded", COLOR_BLACK, COLOR_GREEN, 0, TRACE_LOW, 0)
            ErrorCode = SUCCESSFUL
        Else
            Show_Message_Console(" No se pudo cargar tabla de configuracion sabd_main_definition", COLOR_BLACK, COLOR_RED, 0, TRACE_LOW, 1)
            ErrorCode = PROCESS_ERROR
        End If
        ParamsTable = Nothing
        GC.Collect()
    End Sub


    Public Function Build_XML_String(ByVal ParmsArray() As String, ByVal ParmsNames() As String) As String
        Dim XmlStringReply As String
        Dim memory_stream As New MemoryStream
        Dim xml_text_writer As New XmlTextWriter(memory_stream, System.Text.Encoding.UTF8)

        ' Use indentation to make the result look nice.
        xml_text_writer.Formatting = Formatting.Indented
        xml_text_writer.Indentation = 5

        ' Write the XML declaration.
        '*******************************************************
        xml_text_writer.WriteStartDocument()
        xml_text_writer.WriteStartElement("main_document")
        For x As Int16 = 0 To (ParmsArray.Count - 1)
            '*******************************************************
            Dim ParmsData() As String = ParmsArray(x).Split(New String() {"#"}, StringSplitOptions.RemoveEmptyEntries)
            xml_text_writer.WriteStartElement("body_record")
            For y = 0 To (ParmsNames.Count - 1)
                MakeXMLReply(xml_text_writer, ParmsNames(y), ParmsData(y).Trim)
            Next
            xml_text_writer.WriteEndElement()
        Next
        xml_text_writer.WriteEndElement()
        xml_text_writer.WriteEndDocument()
        '*******************************************************
        xml_text_writer.Flush()

        ' Use a StreamReader to display the result.
        Dim stream_reader As New StreamReader(memory_stream)

        memory_stream.Seek(0, SeekOrigin.Begin)
        'Dim TextoXML As String = stream_reader.ReadToEnd()
        XmlStringReply = stream_reader.ReadToEnd()
        ' Close the XmlTextWriter.
        xml_text_writer.Close()

        Return XmlStringReply

    End Function

    Public Sub MakeXMLReply(ByVal xml_text_writer As XmlTextWriter, ByVal FieldName As String, ByVal FieldValue As String)
        xml_text_writer.WriteStartElement(FieldName)
        xml_text_writer.WriteString(FieldValue)
        xml_text_writer.WriteEndElement()
    End Sub

    Public Property SetGetProcess_PTR() As IntPtr
        Get
            Return process_PTR
        End Get
        Set(ByVal value As IntPtr)
            process_PTR = value
        End Set
    End Property

    Public Function Put_Message_To_Router(ByVal to_ROUTER As SharedStructureMessage, ByVal RouterQueueName As String) As Byte
        Dim ErrorCode As Byte = PROCESS_ERROR
        Dim QueueName As String = String.Empty

        If Not Verify_Process_Up(to_ROUTER.SSM_Rout_Source_Name) Then
            Show_Message_Console(MyName & " Process Router is not active(" & to_ROUTER.SSM_Rout_Source_Name.Trim & ")", COLOR_BLACK, COLOR_YELLOW, 0, TRACE_LOW, 1)
            Return PROCESS_ERROR
        End If

        Try
            Dim MessageToSend As Message = New Message
            Dim QueueSendData As New MessageQueue(RouterQueueName)
            MessageToSend.Body = to_ROUTER
            QueueSendData.Send(MessageToSend)
            QueueSendData.Dispose()
            MessageToSend.Dispose()
            to_ROUTER = Nothing
            ErrorCode = SUCCESSFUL
        Catch ex As Exception
            Show_Message_Console(MyName & " ERROR PutMessage Router " & ex.Message, COLOR_OWNER1, COLOR_RED, 0, TRACE_LOW, 1)
            Return PROCESS_ERROR
        End Try

        Return ErrorCode
    End Function


    Public Function Put_Message_To_SAF(ByVal SSM As SharedStructureMessage) As Byte
        Dim ErrorCode As Byte = PROCESS_ERROR
        Dim QueueName As String = String.Empty

        Try
            Dim MessageToSend As Message = New Message
            Dim QueueSendData As New MessageQueue(g_SafQueue)
            MessageToSend.Body = SSM
            QueueSendData.Send(MessageToSend)
            QueueSendData.Dispose()
            MessageToSend.Dispose()
            SSM = Nothing
            ErrorCode = SUCCESSFUL
        Catch ex As Exception
            Show_Message_Console(MyName & " ERROR PutMessage SAF " & ex.Message, COLOR_OWNER1, COLOR_RED, 0, TRACE_LOW, 1)
            Return PROCESS_ERROR
        End Try

        Return ErrorCode
    End Function


    Public Function Get_Mode_Data(ByVal SystemName As String, ByVal Field As String) As String
        Dim Reference As String = String.Empty
        SystemName = SystemName.ToUpper
        Try
            Dim xmldoc As XDocument = XDocument.Parse(Marshal.PtrToStringAuto(process_PTR))
            Dim ql As XElement = (From ls In xmldoc.Root.Elements("body_record")
                                  Where ls.Element("Name").Value = SystemName
                                  Select ls.Element(Field)).FirstOrDefault
            If Not IsNothing(ql) Then
                Reference = ql.Value
            End If
        Catch ex As Exception
            Reference = " "
        End Try
        GC.Collect()
        Return Reference
    End Function

    Public Function GetCorrelationID() As String
        Dim CorrelationID As String = String.Empty
        Dim NumericBuffer As String = String.Empty
        Dim ComplementInt As Int64
        Dim TempVal As Byte
        NumericBuffer = (Now.Year * Now.Millisecond).ToString & (Now.Month * Now.Millisecond).ToString & (Now.Day * Now.Millisecond).ToString & (Now.Hour * Now.Millisecond).ToString & (Now.Minute * Now.Millisecond) & (Now.Second * Now.Millisecond).ToString & Now.Millisecond.ToString("00")
        NumericBuffer = NumericBuffer & (Now.Year * Now.Millisecond).ToString & (Now.Month * Now.Millisecond).ToString & (Now.Day * Now.Millisecond).ToString & (Now.Hour * Now.Millisecond).ToString & (Now.Minute * Now.Millisecond) & (Now.Second * Now.Millisecond).ToString & Now.Millisecond.ToString("00")
        NumericBuffer = NumericBuffer & (Now.Year * Now.Millisecond).ToString & (Now.Month * Now.Millisecond).ToString & (Now.Day * Now.Millisecond).ToString & (Now.Hour * Now.Millisecond).ToString & (Now.Minute * Now.Millisecond) & (Now.Second * Now.Millisecond).ToString & Now.Millisecond.ToString("00")
        Do While NumericBuffer.Length >= 2
            TempVal = NumericBuffer.Substring(0, 2)
            CorrelationID = CorrelationID & Convert.ToString(TempVal, 16)
            NumericBuffer = NumericBuffer.Remove(0, 2)
        Loop
        CorrelationID = CorrelationID.Insert(8, "-")
        CorrelationID = CorrelationID.Insert(13, "-")
        CorrelationID = CorrelationID.Insert(18, "-")
        CorrelationID = CorrelationID.Insert(23, "-")
        CorrelationID = CorrelationID.Insert(36, "\")
        CorrelationID = CorrelationID.Remove(37)
        SeqCorrelationID = Now.Millisecond * Now.Second
        ComplementInt = Sumator + SeqCorrelationID
        CorrelationID = CorrelationID & ComplementInt.ToString("0000000")
        Return CorrelationID
    End Function


    Public Function GetDataByID(ByVal SystemName As String, ByVal Mode As Byte) As String
        Dim GenericData As String = ""

        Try
            Dim MR As New Main_Record
            If DIC_MAINp.TryGetValue(SystemName.Trim, MR) Then
                Select Case Mode
                    Case 0
                        GenericData = MR.sabd_task
                    Case 1
                        GenericData = MR.sabd_port_number
                    Case 2
                        GenericData = MR.sabd_ip_address
                    Case 3
                        GenericData = MR.sabd_type & "|" & PrivateQueue & MR.sabd_queue_command
                    Case 4
                        GenericData = MR.sabd_connection_mode
                    Case 5
                        GenericData = MR.sabd_assign_router
                    Case 6
                        GenericData = MR.sabd_timeout
                End Select
            End If
        Catch ex As Exception
            Show_Message_Console(MyName & " generic for " & SystemName & " not available ", COLOR_BLACK, COLOR_YELLOW, 0, TRACE_LOW, 1)
        End Try
        GC.Collect()
        Return GenericData
    End Function

    Public Function GetPathQueueName(ByVal SystemType As Int16, ByVal Mode As Byte, ByRef ReferenceName As String) As Byte
        Dim ErrorCode As Byte = PROCESS_ERROR
        Dim QueueNames(6) As String
        ReferenceName = ""
        Try
            For Each iVal As Main_Record In DIC_MAINp.Values
                If iVal.sabd_type = SystemType Then
                    Select Case Mode
                        Case 0
                            ReferenceName = PrivateQueue & iVal.sabd_queue_requirement.Trim
                        Case 1
                            ReferenceName = PrivateQueue & iVal.sabd_queue_replies.Trim
                        Case 2
                            ReferenceName = PrivateQueue & iVal.sabd_queue_comms.Trim
                        Case 3
                            ReferenceName = PrivateQueue & iVal.sabd_queue_saf.Trim
                        Case 4
                            ReferenceName = PrivateQueue & iVal.sabd_queue_command.Trim
                        Case 5
                            ReferenceName = PrivateQueue & iVal.sabd_queue_ack.Trim
                        Case 6
                            ReferenceName = iVal.sabd_module_name.Trim
                    End Select
                    ErrorCode = SUCCESSFUL
                    Exit For
                End If
            Next
        Catch ex As Exception
            ErrorCode = UNKNOW_ERROR
        End Try
        QueueNames = Nothing
        GC.Collect()
        Return ErrorCode
    End Function

    Public Function GetPathQueueName(ByVal SystemName As String, ByVal Mode As Byte, ByRef QueueName As String) As Byte
        Dim ErrorCode As Byte = PROCESS_ERROR
        Dim QueueNames(5) As String
        SystemName = SystemName.ToUpper
        Try
            Dim MR As New Main_Record
            If DIC_MAINp.TryGetValue(SystemName.Trim, MR) Then
                Select Case Mode
                    Case 0
                        QueueName = PrivateQueue & MR.sabd_queue_requirement.Trim
                    Case 1
                        QueueName = PrivateQueue & MR.sabd_queue_replies.Trim
                    Case 2
                        QueueName = PrivateQueue & MR.sabd_queue_comms.Trim
                    Case 3
                        QueueName = PrivateQueue & MR.sabd_queue_saf.Trim
                    Case 4
                        QueueName = PrivateQueue & MR.sabd_queue_command.Trim
                    Case 5
                        QueueName = PrivateQueue & MR.sabd_queue_ack.Trim
                    Case 6
                        QueueName = PrivateQueue & MR.sabd_module_name.Trim
                End Select
                ErrorCode = SUCCESSFUL
            End If
        Catch ex As Exception
            ErrorCode = UNKNOW_ERROR
        End Try
        QueueNames = Nothing
        GC.Collect()
        Return ErrorCode
    End Function

    Public Function GetPathQueueName(ByVal IssuerID As Int32, ByVal Mode As Byte, ByRef QueueName As String, ByRef SystemName As String) As Byte
        Dim ErrorCode As Byte = PROCESS_ERROR
        Dim x As Int16 = 0
        Try
            For Each MR As Main_Record In DIC_MAINp.Values
                If MR.sabd_module_id = IssuerID Then
                    SystemName = MR.sabd_module_name.Trim
                    Select Case Mode
                        Case 0
                            QueueName = PrivateQueue & MR.sabd_queue_requirement.Trim
                        Case 1
                            QueueName = PrivateQueue & MR.sabd_queue_replies.Trim
                        Case 2
                            QueueName = PrivateQueue & MR.sabd_queue_comms.Trim
                        Case 3
                            QueueName = PrivateQueue & MR.sabd_queue_saf.Trim
                        Case 4
                            QueueName = PrivateQueue & MR.sabd_queue_command.Trim
                        Case 5
                            QueueName = PrivateQueue & MR.sabd_queue_ack.Trim
                    End Select
                    ErrorCode = SUCCESSFUL
                    Exit For
                End If
            Next
        Catch ex As Exception
            ErrorCode = UNKNOW_ERROR
        End Try
        Console.WriteLine("Dato:" & QueueName)
        GC.Collect()
        Return ErrorCode
    End Function

    Public Sub ReleaseObject(ByVal obj As Object)
        Try
            System.Runtime.InteropServices.Marshal.ReleaseComObject(obj)
            obj = Nothing
        Catch ex As Exception
            obj = Nothing
        Finally
            GC.Collect()
        End Try
    End Sub

    Public Function Convert_Structure_To_String(obj As Object, ByVal Separator As Char) As String
        'Return String.Join(Separator, obj.GetType().GetFields().Select(Function(field) field.Name(obj) & ":" & field.GetValue(obj)))
        Return String.Join(Separator, obj.GetType().GetFields().Select(Function(field) field.GetValue(obj)))
    End Function

    Public Function Convert_Structure_To_String(obj As Object) As String
        Return String.Join(Nothing, obj.GetType().GetFields().Select(Function(field) field.GetValue(obj)))
    End Function


End Module

