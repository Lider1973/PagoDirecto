Imports System.Xml
Imports System.Messaging
Imports System.Threading
Imports System.IO
Imports System.Runtime.InteropServices

Module ServiSwitch_ModuleLoadingData

    Public Const SERVICE_CPN_PDR As String = "0020030728"
    Public Const FI_CPN_ADQ As Int16 = 188
    Public Const FI_CPN_AUT As Int16 = 728
    Public Const ABA_CPN_ADQ As Int32 = 140325

    Public Const NAMES_ID As String = "X2"
    Public Const MESSAGE_ID As String = "X3"
    Public Const DOCUMENT_ID As String = "X4"
    Public Const LABEL_ID As String = "X5"
    Public Const SERVICE_ID As String = "X6"
    Public Const ACCT_TYPE_ID As String = "X7"
    Public Const AUTHPD_ID As String = "A1"
    Public Const TIII_ID As String = "A2"
    Public Const TARGET_ACCT_TYPE_ID As String = "A3"
    Public Const TARGET_DOCUMENT_ID As String = "A4"

    Public Const condError_UNDEFINED As String = "8003"
    Public Const condError_NOT_CONNECTED As String = "8367"
    Public Const condError_TIMEOUT As String = "8364"
    Public Const condError_DISCONECT As String = "8365"
    Public Const condError_ROUTER_UNAVAILABLE As String = "7803"
    Public Const condError_CANT_PROCESS As String = "8510"

    Public Const PrivateQueue As String = ".\Private$\"
    Public Const FUNCTION_ERROR As Byte = 2
    Public Const SUCCESSFUL As Byte = 0
    Public Const PROCESS_ERROR As Byte = 1
    Public Const UNKNOW_ERROR As Byte = 99
    Public Const MODULE_SERVER As Byte = 0
    Public Const MODULE_CLIENT As Byte = 1

    '   Identification System Modules
    '****************************************
    Public Const MODULES As Byte = 4
    Public Const INTERFACES As Byte = 0
    Public Const ROUTER As Byte = 1
    Public Const COMMANDER As Byte = 2
    Public Const SCANNER As Byte = 3
    Public Const LOGGERS As Byte = 4
    Public Const COMMAND As Byte = 5

    Public Const commander_DISPLAY As String = "DPS|"
    Public Const commander_STATUS As String = "STA|"
    Public Const commander_REQUEST As String = "REQ|"
    Public Const Router_NOTIFY As String = "NTF|"
    Public Const Concatenator As Char = "|"

    Public Const GIVE_MAIN_INFO As String = "0000|"
    Public Const GIVE_COMD_DATA As String = "0001|"
    Public Const GIVE_ROUTER_DATA As String = "0007|"
    Public Const GIVE_ALL_DATA As String = "0002|"
    Public Const GIVE_ISO_DEFINITIONS As String = "0003|"
    Public Const GET_ALL_DEFINITIONS As String = "0004|"
    Public Const GIVE_TRAN_DEFINITIONS As String = "0005|"

    Public Const rep_GIVE_MAIN_INFO As String = "0000"
    Public Const rep_GIVE_COMD_DATA As String = "0001"
    Public Const rep_GIVE_ROUTER_DATA As String = "0007"
    Public Const rep_GIVE_ISO_DEFINITIONS As String = "0003"
    Public Const rep_GET_ALL_CONFIG As String = "0004"
    Public Const rep_GET_TRAN_CONFIG As String = "0005"

    Public Const TranType_ROUTER_IN_SCP As Char = "R"
    Public Const TranType_REPLY As Char = "P"
    Public Const TranType_COMMAND As Char = "C"
    Public Const TranType_ROUTER_OUT_SCP As Char = "O"

    Public Const UNKNOW_status As Byte = 4
    Public Const STARTED_status As Byte = 5

    Public Const Task_Wait As Byte = 1
    Public Const Task_Running As Byte = 2
    Public Const Task_Stop As Byte = 3
    Public Const Task_Abort As Byte = 4
    Public Const Task_Start As Byte = 4

    Public Const Tree_Status As Byte = 0
    Public Const Tree_Count As Byte = 1
    Public Const Tree_Date As Byte = 2

    Public Const listview_CONTROL As Byte = 0
    Public Const treeview_CONTROL As Byte = 1

    '             Color Codes
    '****************************************
    Public Const COLOR_BLUE As Byte = 9
    Public Const COLOR_YELLOW As Byte = 14
    Public Const COLOR_RED As Byte = 12
    Public Const COLOR_BLACK As Byte = 0
    Public Const COLOR_GRAY As Byte = 7
    Public Const COLOR_DARK_GRAY As Byte = 8
    Public Const COLOR_GREEN As Byte = 10
    Public Const COLOR_WHITE As Byte = 15
    Public Const COLOR_OWNER1 As Byte = 17

    Public Const INQUIRY_DIRECT_TRANSFER As String = "0163"
    Public Const SAVING_DIRECT_TRANSFER As String = "0539"
    Public Const CHECKING_DIRECT_TRANSFER As String = "0439"
    Public Const CREDIT_DIRECT_TRANSFER As String = "0239"
    Public Const CONTROL_MESSAGE As String = "2001"

    Public Const SW_SHOWMINNOACTIVE As Int32 = 7
    Public Const SW_SHOWNORMAL As Int32 = 1
    Public Const SW_HIDE As Int32 = 0

    Public Const TRACE_LOW As Byte = 81
    Public Const TRACE_MEDIUM As Byte = 82
    Public Const TRACE_HIGH As Byte = 83

    Public ProcessStatus As Byte
    Public TRAN_TIMEOUT As Int64

    Public ConfigPath As String
    Public ApplType As String

    'Public CommandModuleQueue As String
    'Public CommandCommanderQueue As String
    'Public RouterRequesterQueue As String
    Public Exiting As Boolean = False
    Dim xml_node_LOAD As XmlNodeList

    Public MODULE_DRIVER_DB As Byte = 2

    'Dim MMD As New Main_Manager_Definition
    'Dim LTD As New Loaded_Transaction_Data

    Dim TO_COMMANDER As New InfoCommands
    Public Status_Interface As Byte
    Public Commander_Name As String

    Public Const REQUEST_TASK As Byte = 0
    Public Const REPLY_TASK As Byte = 1
    Public User_Detail As Byte = 81

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
    Public g_RouterCommandQueue As String = String.Empty
    Public ReplyCommanderQueue As String

    Public Mod_TaskNumbers As Int32
    Public Mod_RouterName As String
    Public Mod_Timeout As Int16
    Public Mod_PortNumber As Int32
    Public Mod_AddrNumber As String
    Public Mod_Comms As String

    Dim QUEUE_MESSAGE_BYTES As New InfoFromSocket
    Dim QUEUE_MESSAGE_BUFFER As New SharedStructureMessage

    Dim ProcessNames() As String = {"Name", "Id", "Type", "Institution", "Task", "Instance", "Timeout", "Address", "Port", _
                                    "SocketMode", "Queue_Request_messages", "Queue_Reply_messages", "Queue_Tcp_messages", _
                                    "Queue_Saf_messages", "Queue_Cmd_messages", "Router", "Format", "Queue_Ack_messages"}

    Dim TransactionNames() As String = {"Code", "Name", "Type", "Message", "Bitmap"}

    Dim FormatNames() As String = {"id", "status", "datatype", "length", "fieldtype", "name"}

    Public Const Commander_TYPE As Byte = 5

    Dim process_PTR As IntPtr
    Dim transaction_PTR As IntPtr
    Dim format_PTR As IntPtr
    Dim SeqCorrelationID As Int64
    Const Sumator As Int64 = 1000000
    Dim Xname(0) As String
    Dim Xaba(0) As Int32
    Dim Xadq(0) As Int16
    Dim Xauth(0) As Int16
    Public ServiceURL As string


    Public Function GetPathQueueName(ByVal SystemName As String, ByVal Mode As Byte, ByRef ReferenceName As String) As Byte
        Dim ErrorCode As Byte = PROCESS_ERROR
        Dim QueueNames(6) As String
        SystemName = SystemName.ToUpper
        Try
            Dim xelement As XElement = xelement.Parse(Marshal.PtrToStringAuto(process_PTR))
            Dim name As Object = From nm In xelement.Elements("body_record") _
                  Where nm.Element("Name") = SystemName
                  Select nm
            For Each xEle As XElement In name
                QueueNames(0) = xEle.Elements.ElementAt(10).Value
                QueueNames(1) = xEle.Elements.ElementAt(11).Value
                QueueNames(2) = xEle.Elements.ElementAt(12).Value
                QueueNames(3) = xEle.Elements.ElementAt(13).Value
                QueueNames(4) = xEle.Elements.ElementAt(14).Value
                QueueNames(5) = xEle.Elements.ElementAt(17).Value
                QueueNames(6) = xEle.Elements.ElementAt(15).Value
            Next xEle
            If Mode = 6 Then
                ReferenceName = QueueNames(Mode)
            Else
                ReferenceName = PrivateQueue & QueueNames(Mode)
            End If
            ErrorCode = SUCCESSFUL
        Catch ex As Exception
            ErrorCode = UNKNOW_ERROR
        End Try
        QueueNames = Nothing
        GC.Collect()
        Return ErrorCode
    End Function

    Public Function GetPathQueueName(ByVal SystemType As Int16, ByVal Mode As Byte, ByRef ReferenceName As String) As Byte
        Dim ErrorCode As Byte = PROCESS_ERROR
        Dim QueueNames(6) As String

        Try
            Dim xelement As XElement = xelement.Parse(Marshal.PtrToStringAuto(process_PTR))
            Dim name As Object = From nm In xelement.Elements("body_record") _
                  Where nm.Element("Type") = SystemType.ToString
                  Select nm
            For Each xEle As XElement In name
                QueueNames(0) = xEle.Elements.ElementAt(10).Value
                QueueNames(1) = xEle.Elements.ElementAt(11).Value
                QueueNames(2) = xEle.Elements.ElementAt(12).Value
                QueueNames(3) = xEle.Elements.ElementAt(13).Value
                QueueNames(4) = xEle.Elements.ElementAt(14).Value
                QueueNames(5) = xEle.Elements.ElementAt(17).Value
                QueueNames(6) = xEle.Elements.ElementAt(0).Value
            Next xEle
            If Mode = 6 Then
                ReferenceName = QueueNames(Mode)
            Else
                ReferenceName = PrivateQueue & QueueNames(Mode)
            End If
            ErrorCode = SUCCESSFUL
        Catch ex As Exception
            ErrorCode = UNKNOW_ERROR
        End Try
        QueueNames = Nothing
        GC.Collect()
        Return ErrorCode
    End Function

    Public Function Verify_Queue_Resource(ByVal QueueName As String) As Byte
        Dim ErrorCode As Byte = FUNCTION_ERROR
        Try
            If Not MessageQueue.Exists(QueueName) Then
                MessageQueue.Create(QueueName)
                Show_Message_Console(MyName & " Queue:" & QueueName & " Created", COLOR_BLACK, COLOR_GRAY, 0, TRACE_LOW, 1)
            Else
                Show_Message_Console(MyName & " Queue:" & QueueName & " Finded", COLOR_BLACK, COLOR_GRAY, 0, TRACE_LOW, 1)
            End If
            ErrorCode = SUCCESSFUL
        Catch ex As Exception
            Show_Message_Console(MyName & " Exception on managing Queue:" & QueueName & "->" & ex.Message, COLOR_OWNER1, COLOR_RED, 0, TRACE_LOW, 1)
            ErrorCode = 1
        End Try
        Return ErrorCode

    End Function

    Public Function RemoveQueueFromLocal(ByVal QueueName As String) As Byte
        Dim ErrorCode As Byte

        Try
            If MessageQueue.Exists(QueueName) Then
                MessageQueue.Delete(QueueName)
                Show_Message_Console(MyName & " Queue:" & QueueName & " Deleted", COLOR_BLACK, COLOR_GRAY, 0, TRACE_LOW, 1)
                SaveLogMain(MyName & " Queue:" & QueueName & " Deleted")
            End If
        Catch ex As Exception
            Show_Message_Console(MyName & " Cant delete Queue:" & QueueName & " On system", COLOR_OWNER1, COLOR_RED, 0, TRACE_LOW, 1)
            ErrorCode = 1
        End Try

        Return ErrorCode
    End Function

    Public Function GetBitOnOff(ByVal Hexa As Char) As String
        Dim Binary As String

        Binary = Convert.ToString(Convert.ToInt32(Hexa, 16), 2)
        If Len(Binary) < 4 Then
            Binary = Binary.PadLeft(4, "0")
        End If
        Return Binary

    End Function


    Public Function GetDateTime() As String

        Return System.DateTime.Now.Year & "-" & Format(System.DateTime.Now.Month, "00") & "-" & Format(System.DateTime.Now.Day, "00") & " " & Format(System.DateTime.Now.Hour, "00") & ":" & Format(System.DateTime.Now.Minute, "00") & ":" & Format(System.DateTime.Now.Second, "00") & "." & Format(System.DateTime.Now.Millisecond, "000")

    End Function


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
                    TextData = GetDateTime() & " " & TextData
                    Console.WriteLine(TextData)
                Case 1
                    TextData = GetDateTime() & " " & TextData
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
                Put_Message_To_Manager(commander_DISPLAY & ColorB & "#" & ColorF & "#" & TextData, g_CommanderQueue, 0)
            End If

            Console.ForegroundColor = ConsoleColor.Gray
            Console.BackgroundColor = ConsoleColor.Black

            TextData = Nothing

        End SyncLock
    End Sub

    Public Sub Notify_Process_Status(ByVal Command_Notify As String)

        Put_Message_To_Manager(commander_STATUS & Command_Notify, g_CommanderQueue, 1)

    End Sub

    Public Sub Notify_Socket_Status(ByVal Command_Notify As String)

        Put_Message_To_Manager(commander_STATUS & Command_Notify, ReplyCommanderQueue, 2)

    End Sub


    Private Function Verify_Process_Up(ByVal ProcessName As String, ByVal From As Byte) As Boolean
        'Console.WriteLine("Called from:" & From)

        If IsNothing(ProcessName) Then
            Console.WriteLine("No se puede enviar al Router, ProcessName es invalido: Len" & ProcessName.Length)
        End If

        Dim PathProcessName As String
        Dim FoundIT As Boolean = False
        ProcessName = ProcessName.Trim
        Try
            Dim ServiProcess As List(Of Process) = (From p As Process In Process.GetProcesses Where p.ProcessName.ToUpper Like "ServiSwitch*".ToUpper).ToList
            For Each p As Process In ServiProcess
                PathProcessName = p.MainModule.FileName()
                If PathProcessName.Contains("\" & ProcessName & "\") Then
                    FoundIT = True
                    Exit For
                End If
            Next
        Catch ex As Exception
            FoundIT = False
        End Try
        Return FoundIT

    End Function

    Public Function Put_Message_To_Router(ByVal to_ROUTER As SharedStructureMessage, ByVal RouterQueueName As String) As Byte
        Dim ErrorCode As Byte = PROCESS_ERROR
        Dim QueueName As String = String.Empty

        If Not Verify_Process_Up(to_ROUTER.SSM_Rout_Source_Name, 0) Then
            Show_Message_Console(MyName & " Process Router is not active(" & to_ROUTER.SSM_Rout_Source_Name.Trim & ")", COLOR_BLACK, COLOR_YELLOW, 0, TRACE_LOW, 1)
            Return PROCESS_ERROR
        End If

        Try
            to_ROUTER.SSM_Instance_Times += "T12_" & GetDateTime() & Concatenator
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

    Public Sub Put_Message_To_Manager(ByVal Data As String, ByVal LocalThreadQueueName As String, ByVal From As Byte)

        If Not Verify_Process_Up(Commander_Name, 1) Then
            Exit Sub
        End If

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
        Catch ex As Exception
            Show_Message_Console(MyName & " ERROR PutMessage Manager " & ex.Message, COLOR_OWNER1, COLOR_RED, 0, TRACE_LOW, 0)
        End Try

    End Sub

    Public Sub Put_Message_Socket_Request(ByVal Total_length As Int16, ByVal BufferData() As Byte, ByVal PackageNbr As Int32)

        Try
            Dim MessageToSend As Message = New Message
            Dim QueueSendData As New MessageQueue(g_RequestQueue)
            QUEUE_MESSAGE_BYTES.SMB_BytesMessage = BufferData
            QUEUE_MESSAGE_BYTES.SMB_TotLength = Total_length
            QUEUE_MESSAGE_BYTES.SMB_Package_Nbr = PackageNbr

            MessageToSend.Body = QUEUE_MESSAGE_BYTES
            QueueSendData.Send(MessageToSend)
        Catch ex As Exception
            Show_Message_Console(MyName & " Excepcion " & ex.Message, COLOR_OWNER1, COLOR_RED, 0, TRACE_LOW, 1)
        End Try

    End Sub

    Public Sub Put_Message_Queue_Reply(ByVal IdMessage As Int64, ByVal BufferData As String)

        Try
            Dim MessageToSend As Message = New Message
            Dim QueueSendData As New MessageQueue(g_ReplyQueue)
            QUEUE_MESSAGE_BUFFER.SSM_Transaction_Indicator = TranType_COMMAND
            QUEUE_MESSAGE_BUFFER.SSM_Communication_ID = IdMessage
            QUEUE_MESSAGE_BUFFER.SSM_Queue_Message_ID = 0
            QUEUE_MESSAGE_BUFFER.SSM_Instance_Times = DateTime.Now.ToString
            QUEUE_MESSAGE_BUFFER.SSM_Common_Data.CRF_Buffer_Data = BufferData
            MessageToSend.Body = QUEUE_MESSAGE_BUFFER
            QueueSendData.Send(MessageToSend)
        Catch ex As Exception
            Show_Message_Console(MyName & " Excepcion " & ex.Message, COLOR_OWNER1, COLOR_RED, 0, TRACE_LOW, 1)
        End Try
    End Sub

    Public Sub Put_Notify_To_Router(ByVal Data As String)

        Try
            Dim MessageToSend As Message = New Message
            Dim QueueSendData As New MessageQueue(g_RouterCommandQueue)
            TO_COMMANDER.ICM_CommandBuffer = Data
            MessageToSend.Body = TO_COMMANDER
            QueueSendData.Send(MessageToSend)
            QueueSendData.Dispose()
            MessageToSend.Dispose()
            TO_COMMANDER.ICM_CommandBuffer = Nothing
        Catch ex As Exception
            Show_Message_Console(MyName & " ERROR PutMessage Manager " & ex.Message, COLOR_OWNER1, COLOR_RED, 0, TRACE_LOW, 0)
        End Try

    End Sub


    Public Function Load_Institution_ID() As Byte
        Dim ErrorCode As Int16 = 0
        Dim XmlConfigFile As New XmlDocument
        Dim xml_node_LOAD As XmlNodeList
        Dim x As Int16 = 0

        Try
            XmlConfigFile.Load(System.AppDomain.CurrentDomain.BaseDirectory & "institutionID.xml")
            xml_node_LOAD = XmlConfigFile.GetElementsByTagName("name")
            ReDim Xname(xml_node_LOAD.Count - 1)
            For x = 0 To (xml_node_LOAD.Count - 1)
                Xname(x) = xml_node_LOAD.ItemOf(x).InnerText
            Next

            xml_node_LOAD = XmlConfigFile.GetElementsByTagName("aba")
            ReDim Xaba(xml_node_LOAD.Count - 1)
            For x = 0 To (xml_node_LOAD.Count - 1)
                Xaba(x) = xml_node_LOAD.ItemOf(x).InnerText
            Next

            xml_node_LOAD = XmlConfigFile.GetElementsByTagName("adq")
            ReDim Xadq(xml_node_LOAD.Count - 1)
            For x = 0 To (xml_node_LOAD.Count - 1)
                Xadq(x) = xml_node_LOAD.ItemOf(x).InnerText
            Next

            xml_node_LOAD = XmlConfigFile.GetElementsByTagName("auth")
            ReDim Xauth(xml_node_LOAD.Count - 1)
            For x = 0 To (xml_node_LOAD.Count - 1)
                Xauth(x) = xml_node_LOAD.ItemOf(x).InnerText
            Next
            ErrorCode = 0
        Catch ex As Exception
            ErrorCode = 1
            Return ErrorCode
        End Try

        Return ErrorCode
    End Function

    Public Function GetInfoInstitution(ByVal Account As Long, ByVal Input As Int16, ByRef Output As Int32, ByRef Name As String) As Byte
        Dim ErrorCode As Byte = FUNCTION_ERROR
        Dim Idx As Int16 = 0

        If Xauth.Contains(Input) Then
            Idx = Array.IndexOf(Xauth, Input)
            Output = Xaba(Idx)
            Name = Xname(Idx)
            ErrorCode = SUCCESSFUL
        Else
            Output = 0
            Name = ""
        End If

        Return ErrorCode
    End Function


    Public Function Load_Setting_Database() As Byte
        Dim ErrorCode As Byte = FUNCTION_ERROR
        Dim Mode As Byte
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
        ' LOADING COMMANDER MODULE NAME
        Mode = 6
        If GetPathQueueName(Commander_TYPE, Mode, Commander_Name) <> SUCCESSFUL Then
            Return PROCESS_ERROR
        End If

        '******************************************************
        ' LOADING COMMANDER QUEUE NAME
        Mode = 1
        If GetPathQueueName(Commander_TYPE, Mode, g_CommanderQueue) = SUCCESSFUL Then
            If Verify_Queue_Resource(g_CommanderQueue) <> SUCCESSFUL Then
                ErrorCode = PROCESS_ERROR
            End If
        Else
            ErrorCode = PROCESS_ERROR
        End If
        If ErrorCode = PROCESS_ERROR Then
            Return ErrorCode
        End If
        '******************************************************
        ' LOADING REQUEST QUEUE NAME
        GetPathQueueName(MyName, 0, g_RequestQueue)
        If Verify_Queue_Resource(g_RequestQueue) <> SUCCESSFUL Then
            Return ErrorCode
        End If

        '******************************************************
        ' LOADING COMMANDER REPLY QUEUE NAME
        Mode = 1
        If GetPathQueueName(Commander_TYPE, Mode, ReplyCommanderQueue) = SUCCESSFUL Then
            If Verify_Queue_Resource(ReplyCommanderQueue) <> SUCCESSFUL Then
                ErrorCode = PROCESS_ERROR
            End If
        Else
            ErrorCode = PROCESS_ERROR
        End If
        If ErrorCode = PROCESS_ERROR Then
            Return ErrorCode
        End If


        '******************************************************
        ' LOADING REPLY QUEUE NAME
        GetPathQueueName(MyName, 1, g_ReplyQueue)
        If Verify_Queue_Resource(g_ReplyQueue) <> SUCCESSFUL Then
            Return ErrorCode
        End If

        '******************************************************
        ' LOADING TCPIP QUEUE NAME
        GetPathQueueName(MyName, 2, g_TcpQueue)
        If Verify_Queue_Resource(g_TcpQueue) <> SUCCESSFUL Then
            Return ErrorCode
        End If

        '******************************************************
        ' LOADING SAF QUEUE NAME
        GetPathQueueName(MyName, 3, g_SafQueue)
        If Verify_Queue_Resource(g_SafQueue) <> SUCCESSFUL Then
            Return ErrorCode
        End If

        '******************************************************
        ' LOADING COMMAND QUEUE NAME
        GetPathQueueName(MyName, 4, g_CmdQueue)
        If Verify_Queue_Resource(g_CmdQueue) <> SUCCESSFUL Then
            Return ErrorCode
        End If

        '******************************************************
        ' LOADING ACKNOWLEDGE QUEUE NAME
        GetPathQueueName(MyName, 5, g_AckQueue)
        If Verify_Queue_Resource(g_AckQueue) <> SUCCESSFUL Then
            Return ErrorCode
        End If

        '******************************************************
        ' LOADING COMMAND ROUTER QUEUE NAME
        Dim RouterName As String = ""
        If GetPathQueueName(MyName, 6, RouterName) <> SUCCESSFUL Then
            Return PROCESS_ERROR
        End If

        If GetPathQueueName(RouterName, 4, g_RouterCommandQueue) <> SUCCESSFUL Then
            Return PROCESS_ERROR
        End If


        '******************************************************
        ' LOADING TASK NUMBERS DEFINED
        Mod_TaskNumbers = CInt(Get_Mode_Data(MyName, "Task"))

        Mod_PortNumber = CInt(Get_Mode_Data(MyName, "Port"))

        Mod_AddrNumber = Get_Mode_Data(MyName, "Address")

        Mod_Comms = CInt(Get_Mode_Data(MyName, "SocketMode"))

        Mod_RouterName = Get_Mode_Data(MyName, "Router")

        '******************************************************
        ' LOADING ISO DEFINITION FIELDS
        Load_FormatMessage_Definition(ErrorCode)
        If ErrorCode = PROCESS_ERROR Then
            Return ErrorCode
        End If

        '******************************************************
        ' LOADING TASK NUMBERS DEFINED
        Mod_TaskNumbers = CInt(Get_Mode_Data(MyName, "Task"))
        Mod_RouterName = Get_Mode_Data(MyName, "Router")
        Mod_Timeout = Get_Mode_Data(MyName, "Timeout")
        Mod_Timeout = Mod_Timeout * 1000

        '******************************************************
        ' LOADING REQUEST ROUTER NAME
        Mode = 0
        If GetPathQueueName(Mod_RouterName, Mode, g_RouterRequestQueue) <> SUCCESSFUL Then
            Return PROCESS_ERROR
        End If

        Mode = 1
        If GetPathQueueName(Mod_RouterName, Mode, g_RouterReplyQueue) <> SUCCESSFUL Then
            Return PROCESS_ERROR
        End If

        Return SUCCESSFUL
    End Function

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

    Private Sub Load_Process_Definition(ByVal ErrorCode As Byte)
        Dim ParamsTable As New List(Of String)
        ParamsTable.Clear()
        If Get_Table_Definition(ParamsTable, "sabd_main_definition", "*") = SUCCESSFUL Then
            Dim ParamsArray() As String = ParamsTable.ToArray
            SetGetProcess_PTR = Marshal.StringToHGlobalAuto(Build_XML_String(ParamsArray, ProcessNames))
            Show_Message_Console("sabd_main_definition: Loaded", COLOR_BLACK, COLOR_GREEN, 0, TRACE_LOW, 0)
        Else
            Show_Message_Console(" No se pudo cargar tabla de configuracion sabd_main_definition", COLOR_BLACK, COLOR_RED, 0, TRACE_LOW, 0)
            ErrorCode = PROCESS_ERROR
        End If
        ParamsTable = Nothing
        GC.Collect()

    End Sub

    Private Sub Load_FormatMessage_Definition(ByVal ErrorCode As Byte)
        Dim ParamsTable As New List(Of String)
        ParamsTable.Clear()
        If Get_Table_Definition(ParamsTable, "sabd_iso_definition", "*") = SUCCESSFUL Then
            Dim ParamsArray() As String = ParamsTable.ToArray
            SetGetFormat_PTR = Marshal.StringToHGlobalAuto(Build_XML_String(ParamsArray, FormatNames))
            Show_Message_Console("sabd_iso_definition: Loaded", COLOR_BLACK, COLOR_GREEN, 0, TRACE_LOW, 0)
        Else
            Show_Message_Console(" No se pudo cargar tabla de configuracion sabd_iso_definition", COLOR_BLACK, COLOR_RED, 0, TRACE_LOW, 0)
            ErrorCode = PROCESS_ERROR
        End If
        ParamsTable = Nothing
        GC.Collect()
    End Sub

    Public Function Get_Mode_Data(ByVal SystemName As String, ByVal Field As String) As String
        Dim Reference As String = String.Empty
        SystemName = SystemName.ToUpper
        Try
            Dim xmldoc As XDocument = XDocument.Parse(Marshal.PtrToStringAuto(process_PTR))
            Dim ql As XElement = (From ls In xmldoc.Root.Elements("body_record") _
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

    Public Property SetGetProcess_PTR() As IntPtr
        Get
            Return process_PTR
        End Get
        Set(ByVal value As IntPtr)
            process_PTR = value
        End Set
    End Property

    Public Property SetGetFormat_PTR() As IntPtr
        Get
            Return format_PTR
        End Get
        Set(ByVal value As IntPtr)
            format_PTR = value
        End Set
    End Property

End Module