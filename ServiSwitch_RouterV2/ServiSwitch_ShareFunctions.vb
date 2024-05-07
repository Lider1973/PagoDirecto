Imports System.Messaging
Imports System.Threading
Imports System.IO
Imports System.Xml
Imports System.Runtime.InteropServices
Imports System.Collections.Concurrent
Module ServiSwitch_ShareFunctions
    Public Const STATE_ON As Int16 = 1
    Public Const STATE_OFF As Int16 = 0

    Public Const MODE_MANUAL As Int16 = 2
    Public Const MODE_AUTO As Int16 = 3

    Private Declare Auto Function SetProcessWorkingSetSize Lib "kernel32.dll" (ByVal procHandle As IntPtr, ByVal min As Int32, ByVal max As Int32) As Boolean
    Public Const QUEUE_TIMEOUT As Int64 = -2147467259
    Public Const condError_UNDEFINED As String = "30"
    Public Const condError_NOT_AUTH As String = "98"
    Public Const EXCEPTION_code As String = "999"
    Public Const ERROR_Undefined_Transaction As String = "1007"
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
    Public Const ISO_MAX_FIELDS As Byte = 127
    Public Const FUNCTION_ERROR As Byte = 2
    Public Const SUCCESSFUL As Byte = 0
    Public Const PROCESS_ERROR As Byte = 1
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

    Dim TransactionNames() As String = {"Code", "Name", "Type", "Message", "Bitmap"}

    Dim RoutingNames() As String = {"Code", "Target", "Name"}

    Dim InstitutionNames() As String = {"Code", "Name"}

    'Public QueueRequester_Name As String
    'Public QueueReplier_Name As String

    Public Const error_RECORD_NOT_FOUND As Int16 = 7501
    Public Const error_NOT_PROCESS As Int16 = 8510
    Public Const error_NOT_AVAILABLE As Int16 = 7803
    Public Const error_INCORRECT_DATA As Int16 = 8038
    Public Const FI_CPN_ADQ As Int16 = 188
    Public Const FI_CPN_AUT As Int16 = 728
    Public Const ABA_CPN_ADQ As Int32 = 140325

    Dim process_PTR As IntPtr
    Dim transaction_PTR As IntPtr
    Dim routing_PTR As IntPtr
    'Dim institution_PTR As IntPtr
    'Dim DIC_Institution As New Dictionary(Of Int16, Institution_Record)
    Dim DIC_Institution As New ConcurrentDictionary(Of Int16, Institution_Record)
    Public EnableTime As Int16

    '************************************************
    Dim TO_COMMANDER As New InfoCommands
    '************************************************
    Public User_Detail As Byte = 81
    Public Send2Manager As Boolean = False
    Public Exiting As Boolean = False
    Public DeclineSwitch As Boolean = False
    '************************************************

    Public Function Validate_Type_Transaction_Code(ByVal TransactionCode As Int32) As Byte
        Dim ErrorCode As Byte = PROCESS_ERROR
        Try
            Dim xmldoc As XDocument = XDocument.Parse(Marshal.PtrToStringAuto(transaction_PTR))
            Dim ql As XElement = (From ls In xmldoc.Root.Elements("body_record")
                                  Where ls.Element("Code").Value = TransactionCode.ToString("000000")
                                  Select ls.Element("Name")).FirstOrDefault
            If Not IsNothing(ql) Then
                ErrorCode = SUCCESSFUL
            End If
        Catch ex As Exception
            Show_Message_Console("Cant Validate Transaction Code", COLOR_BLACK, COLOR_RED, 0, TRACE_LOW, 0)
        End Try
        Return ErrorCode
    End Function


    Public Function GetPathQueueName(ByVal IssuerID As Int32, ByVal Mode As Byte, ByRef QueueName As String, ByRef SystemName As String) As Byte
        Dim ErrorCode As Byte = PROCESS_ERROR
        Dim x As Int16 = 0
        Dim QueueNames(5) As String
        SystemName = SystemName.ToUpper
        Try
            Dim xelement As XElement = xelement.Parse(Marshal.PtrToStringAuto(process_PTR))
            Dim name As Object = From nm In xelement.Elements("body_record")
                                 Where nm.Element("Id") = IssuerID.ToString
                                 Select nm
            For Each xEle As XElement In name
                SystemName = xEle.Elements.ElementAt(0).Value
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

    Public Function GetPathQueueName(ByVal SystemName As String, ByVal Mode As Byte, ByRef QueueName As String) As Byte
        Dim ErrorCode As Byte = PROCESS_ERROR
        Dim QueueNames(5) As String
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

    Public Function GetDataByID(ByVal SystemName As String, ByVal Mode As Byte) As String
        Dim GenericData As String = ""
        Try
            Dim xelement As XElement = xelement.Parse(Marshal.PtrToStringAuto(process_PTR))
            Dim name As Object = From nm In xelement.Elements("body_record") _
                  Where nm.Element("Name") = SystemName
                  Select nm
            For Each xEle As XElement In name
                Select Case Mode
                    Case 0
                        GenericData = xEle.Elements.ElementAt(4).Value
                End Select
            Next xEle
        Catch ex As Exception
            Show_Message_Console(MyName & " generic for " & SystemName & " not available ", COLOR_BLACK, COLOR_YELLOW, 0, TRACE_LOW, 1)
        End Try
        GC.Collect()
        Return GenericData
    End Function

    Public Sub Put_Message_To_Auth(ByVal Struct_Shared_Message As SharedStructureMessage, ByVal SystemQueueName As String)
        Dim lockOBJ_IN As Object = New Object
        Dim ReplyMessageException As String = String.Empty

        If GetModuleName(Struct_Shared_Message.SSM_Auth_Module_ID, Struct_Shared_Message.SSM_Auth_Source_Name) <> 0 Then
            Show_Message_Console(MyName & " Issuer not available ", COLOR_BLACK, COLOR_YELLOW, 0, TRACE_LOW, 1)
            ReplyMessageException = Build_Reply_Exception(EXCEPTION_code, "No se pudo obtener modulo autorizador para esta transaccion")
            Struct_Shared_Message.SSM_Common_Data.CRF_Buffer_Data = ReplyMessageException
            Reply_Exception_To_Source(Struct_Shared_Message, Constanting_Definition.NOT_PROCESS_ACTIVE)
            Exit Sub
        End If

        If Not Verify_Process_Up(Struct_Shared_Message.SSM_Auth_Source_Name) Then
            Show_Message_Console(MyName & " Issuer not available ", COLOR_BLACK, COLOR_YELLOW, 0, TRACE_LOW, 1)
            ReplyMessageException = Build_Reply_Exception(EXCEPTION_code, "EL modulo autorizador para esta transaccion no esta disponible:" & Struct_Shared_Message.SSM_Auth_Source_Name)
            Struct_Shared_Message.SSM_Common_Data.CRF_Buffer_Data = ReplyMessageException
            Reply_Exception_To_Source(Struct_Shared_Message, Constanting_Definition.NOT_PROCESS_ACTIVE)
            Exit Sub
        End If

        SyncLock lockOBJ_IN
            Try
                Dim MessageToSend As Message = New Message
                Dim QueueSendData As New MessageQueue(SystemQueueName)

                If Struct_Shared_Message.SSM_Communication_ID = Constanting_Definition.from_INTERFACES Then
                    Struct_Shared_Message.SSM_Instance_Times += "T8_" & GetDateTime() & Concatenator
                Else
                    Struct_Shared_Message.SSM_Instance_Times += "T4_" & GetDateTime() & Concatenator
                End If

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


    Public Sub Put_Message_To_Adq(ByVal Struct_Shared_Message As SharedStructureMessage, ByVal SystemQueueName As String)
        Dim lockOBJ_IN As Object = New Object
        '
        ' On WebService Adquirence Can't get the Id Process
        '
        If Struct_Shared_Message.SSM_Communication_ID = Constanting_Definition.from_WEBSERVICE Then
            Struct_Shared_Message.SSM_Instance_Times += "T14_" & GetDateTime() & Concatenator
            Reply_Message_To_WSadq(Struct_Shared_Message, SystemQueueName)
            Exit Sub
        End If

        SyncLock lockOBJ_IN
            Try
                Struct_Shared_Message.SSM_Instance_Times += "T14_" & GetDateTime() & Concatenator
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
                'Console.WriteLine("Id reply:" & msgReply.CorrelationId)
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

        If Not Verify_Process_Up(Commander_Module_Name) Then
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
            Show_Message_Console(MyName & "ERROR PutMessage Manager " & ex.Message, COLOR_OWNER1, COLOR_RED, 0, TRACE_LOW, 1)
        End Try

    End Sub

    Public Sub Reply_Exception_To_Source(ByVal Struct_Request_Message As SharedStructureMessage, ByVal ErrorCode As Byte)

        Struct_Request_Message.SSM_Instance_Times += "T10_" & GetDateTime() & Concatenator
        Struct_Request_Message.SSM_Common_Data.CRF_Authorization_Code = "000000"
        Struct_Request_Message.SSM_Transaction_Indicator = TranType_REPLY
        Struct_Request_Message.SSM_Common_Data.CRF_Message_Code = 210

        Select Case ErrorCode
            Case Constanting_Definition.NOT_TRANSACTION_DEFINED
                Select Case Struct_Request_Message.SSM_Message_Format
                    Case Constanting_Definition.HOST2_format
                        Struct_Request_Message.SSM_Common_Data.CRF_Response_Code = "8003"
                    Case Constanting_Definition.ISO8583_format, Constanting_Definition.OWNER_format
                        Struct_Request_Message.SSM_Common_Data.CRF_Response_Code = "05"
                        Struct_Request_Message.SSM_Common_Data.CRF_Buffer_Data = Struct_Request_Message.SSM_Common_Data.CRF_Temporal
                        Struct_Request_Message.SSM_Common_Data.CRF_Temporal = Nothing
                End Select
            Case Constanting_Definition.NOT_ROUTE_DEFINED
                Select Case Struct_Request_Message.SSM_Message_Format
                    Case Constanting_Definition.HOST2_format
                        Struct_Request_Message.SSM_Common_Data.CRF_Response_Code = "7803"
                    Case Constanting_Definition.ISO8583_format, Constanting_Definition.OWNER_format
                        Struct_Request_Message.SSM_Common_Data.CRF_Response_Code = "05"
                        Struct_Request_Message.SSM_Common_Data.CRF_Buffer_Data = Struct_Request_Message.SSM_Common_Data.CRF_Temporal
                        Struct_Request_Message.SSM_Common_Data.CRF_Temporal = Nothing
                End Select
            Case Constanting_Definition.NOT_PROCESS_ACTIVE
                Select Case Struct_Request_Message.SSM_Message_Format
                    Case Constanting_Definition.HOST2_format
                        Struct_Request_Message.SSM_Common_Data.CRF_Response_Code = "7806"
                    Case Constanting_Definition.ISO8583_format, Constanting_Definition.OWNER_format
                        Struct_Request_Message.SSM_Common_Data.CRF_Response_Code = "91"
                        Struct_Request_Message.SSM_Common_Data.CRF_Buffer_Data = Struct_Request_Message.SSM_Common_Data.CRF_Temporal
                        Struct_Request_Message.SSM_Common_Data.CRF_Temporal = Nothing
                End Select
            Case Constanting_Definition.NOT_PROCESS_DEFINED
                Select Case Struct_Request_Message.SSM_Message_Format
                    Case Constanting_Definition.HOST2_format
                        Struct_Request_Message.SSM_Common_Data.CRF_Response_Code = "7802"
                    Case Constanting_Definition.ISO8583_format, Constanting_Definition.OWNER_format
                        Struct_Request_Message.SSM_Common_Data.CRF_Response_Code = "89"
                        Struct_Request_Message.SSM_Common_Data.CRF_Buffer_Data = Struct_Request_Message.SSM_Common_Data.CRF_Temporal
                        Struct_Request_Message.SSM_Common_Data.CRF_Temporal = Nothing
                End Select
            Case Constanting_Definition.DATE_TIME_INCONSISTENCE
                'Console.WriteLine("Constanting_Definition.DATE_TIME_INCONSISTENCE")
                Select Case Struct_Request_Message.SSM_Message_Format
                    Case Constanting_Definition.HOST2_format
                        'Console.WriteLine("formato1:" & Struct_Request_Message.SSM_Message_Format)
                        Struct_Request_Message.SSM_Common_Data.CRF_Response_Code = "8364"
                    Case Constanting_Definition.ISO8583_format, Constanting_Definition.OWNER_format
                        'Console.WriteLine("formato2:" & Struct_Request_Message.SSM_Message_Format)
                        Struct_Request_Message.SSM_Common_Data.CRF_Response_Code = "2526"
                        Struct_Request_Message.SSM_Common_Data.CRF_Buffer_Data = Struct_Request_Message.SSM_Common_Data.CRF_Temporal
                        Struct_Request_Message.SSM_Common_Data.CRF_Temporal = Nothing
                End Select
            Case Constanting_Definition.INSTITUTION_IS_DOWN
                'Console.WriteLine("Constanting_Definition.INSTITUTION_IS_DOWN")
                Select Case Struct_Request_Message.SSM_Message_Format
                    Case Constanting_Definition.HOST2_format
                        'Console.WriteLine("formato1:" & Struct_Request_Message.SSM_Message_Format)
                        Struct_Request_Message.SSM_Common_Data.CRF_Response_Code = "7300"
                    Case Constanting_Definition.ISO8583_format, Constanting_Definition.OWNER_format
                        'Console.WriteLine("formato2:" & Struct_Request_Message.SSM_Message_Format)
                        Struct_Request_Message.SSM_Common_Data.CRF_Response_Code = "7300"
                        Struct_Request_Message.SSM_Common_Data.CRF_Buffer_Data = Struct_Request_Message.SSM_Common_Data.CRF_Temporal
                        Struct_Request_Message.SSM_Common_Data.CRF_Temporal = Nothing
                End Select
        End Select

        ' ****
        ' *******
        Put_Message_To_Adq(Struct_Request_Message, Struct_Request_Message.SSM_Adq_Queue_Reply_Name)
        ' *******
        ' ****

    End Sub

    Public Function Verify_Process_Up(ByVal ProcessName As String) As Boolean
        Dim PathProcessName As String = String.Empty
        Dim FoundIT As Boolean = False

        Try
            Dim ServiProcess As List(Of Process) = (From p As Process In Process.GetProcesses Where p.ProcessName.ToUpper Like "ServiSwitch*".ToUpper).ToList
            'Console.WriteLine("Procesos:" & ServiProcess.Count)
            For Each p As Process In ServiProcess
                'Console.WriteLine("Proceso:" & p.ProcessName)
                PathProcessName = p.MainModule.FileName()
                'Console.WriteLine(ProcessName & " " & PathProcessName)
                If PathProcessName.Contains("\" & ProcessName & "\") Then
                    FoundIT = True
                    Exit For
                End If
            Next
        Catch ex As Exception
            FoundIT = False
            Console.WriteLine("Exception verifing processUP:" & ProcessName & " " & ex.Message)
        End Try
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

    Public Function Get_Info_Token_Data(ByVal TOKENDATA As String, ByVal tokenId As String) As String
        Dim AnalizeToken As String = "XXXXXXXXXX"
        If IsNothing(TOKENDATA) Then
            Return AnalizeToken
        End If

        If TOKENDATA.IndexOf("&") < 0 Then
            Return AnalizeToken
        End If

        Try
            Dim TknNbr As Int16 = TOKENDATA.Substring(2, 5)
            TOKENDATA = TOKENDATA.Remove(0, 12)
            Dim IdTknArray(TknNbr - 1) As String
            Dim DtTknArray(TknNbr - 1) As String
            Dim LenDin As Int16

            For x As Int16 = 0 To TknNbr - 2
                IdTknArray(x) = TOKENDATA.Substring(0, 4)
                TOKENDATA = TOKENDATA.Remove(0, 4)
                LenDin = TOKENDATA.Substring(0, 5)
                TOKENDATA = TOKENDATA.Remove(0, 6)
                DtTknArray(x) = TOKENDATA.Substring(0, LenDin)
                TOKENDATA = TOKENDATA.Remove(0, LenDin)
            Next
            Dim index As Integer = Array.FindIndex(IdTknArray, Function(s) s = tokenId)
            Return DtTknArray(index)
        Catch ex As Exception
            Return AnalizeToken
        End Try

        Return AnalizeToken

    End Function

    Public Function Get_Issuer_ID(ByVal RouteArmed As String) As String
        Dim IssuerID As String = "9999"
        Try
            Dim xmldoc As XDocument = XDocument.Parse(Marshal.PtrToStringAuto(routing_PTR))
            Dim ql As XElement = (From ls In xmldoc.Root.Elements("body_record") _
                    Where ls.Element("Code").Value = RouteArmed _
                    Select ls.Element("Target")).FirstOrDefault
            If Not IsNothing(ql) Then
                IssuerID = ql.Value
            End If
        Catch ex As Exception
            Show_Message_Console("Cant Validate Route Code", COLOR_BLACK, COLOR_RED, 0, TRACE_LOW, 0)
        End Try
        Return IssuerID

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
                'If ColorB = COLOR_BLACK Then
                '    ColorB = COLOR_WHITE
                'End If
                'If ColorF <> COLOR_RED Then
                '    ColorF = COLOR_BLACK
                'End If
                'If (ColorB = COLOR_RED) And (ColorF = COLOR_BLACK) Then
                '    ColorF = COLOR_WHITE
                'End If
                Put_Message_To_Manager(commander_DISPLAY & ColorB & "#" & ColorF & "#" & TextData, Commander_Command_Name)
            End If

            TextData = Nothing
            Console.BackgroundColor = ConsoleColor.Black
            Console.ForegroundColor = ConsoleColor.White

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


    Public Function Build_Reply_Exception(ByVal code As String, ByVal detail As String) As String
        Dim XmlStringReply As String
        Dim memory_stream As New MemoryStream
        Dim xml_text_writer As New XmlTextWriter(memory_stream, System.Text.Encoding.UTF8)

        ' Use indentation to make the result look nice.
        xml_text_writer.Formatting = Formatting.Indented
        xml_text_writer.Indentation = 5

        ' Write the XML declaration.
        '*******************************************************
        xml_text_writer.WriteStartDocument()
        xml_text_writer.WriteStartElement("reply_exception")
        xml_text_writer.WriteStartElement("body_fields")
        '*******************************************************
        MakeXMLReply(xml_text_writer, "trancode", EXCEPTION_code)
        MakeXMLReply(xml_text_writer, "code", code)
        MakeXMLReply(xml_text_writer, "detail", detail)
        '*******************************************************
        xml_text_writer.WriteEndElement()
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
        Load_Transaction_Definition(ErrorCode)
        If ErrorCode = PROCESS_ERROR Then
            Return ErrorCode
        End If
        '******************************************************
        Load_Routing_Definition(ErrorCode)
        If ErrorCode = PROCESS_ERROR Then
            Return ErrorCode
        End If
        '******************************************************
        Load_Institution_Definition(ErrorCode)
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

        Return SUCCESSFUL
    End Function

    Private Sub Load_Process_Definition(ByVal ErrorCode As Byte)
        Dim ParamsTable As New List(Of String)
        ParamsTable.Clear()
        If Get_Table_Definition(ParamsTable, "sabd_main_definition", "*") = SUCCESSFUL Then
            Dim ParamsArray() As String = ParamsTable.ToArray
            SetGetProcess_PTR = Marshal.StringToHGlobalAuto(Build_XML_String(ParamsArray, ProcessNames))
            Show_Message_Console(" sabd_main_definition: Loaded", COLOR_BLACK, COLOR_GREEN, 0, TRACE_LOW, 0)
        Else
            Show_Message_Console(" No se pudo cargar tabla de configuracion sabd_main_definition", COLOR_BLACK, COLOR_RED, 0, TRACE_LOW, 1)
            ErrorCode = PROCESS_ERROR
        End If
        ParamsTable = Nothing
        GC.Collect()
    End Sub

    Private Sub Load_Transaction_Definition(ByVal ErrorCode As Byte)
        Dim ParamsTable As New List(Of String)
        ParamsTable.Clear()
        If Get_Table_Definition(ParamsTable, "sabd_transaction_definition", "*") = SUCCESSFUL Then
            Dim ParamsArray() As String = ParamsTable.ToArray
            SetGetTransaction_PTR = Marshal.StringToHGlobalAuto(Build_XML_String(ParamsArray, TransactionNames))
            Show_Message_Console(" sabd_transaction_definition: Loaded", COLOR_BLACK, COLOR_GREEN, 0, TRACE_LOW, 0)
        Else
            Show_Message_Console(" No se pudo cargar tabla de configuracion sabd_routing_definition", COLOR_BLACK, COLOR_RED, 0, TRACE_LOW, 1)
            ErrorCode = PROCESS_ERROR
        End If
        ParamsTable = Nothing
        GC.Collect()
    End Sub

    Private Sub Load_Routing_Definition(ByVal ErrorCode As Byte)
        Dim ParamsTable As New List(Of String)
        ParamsTable.Clear()
        If Get_Table_Definition(ParamsTable, "sabd_routing_definition", "*") = SUCCESSFUL Then
            Dim ParamsArray() As String = ParamsTable.ToArray
            SetGetRouting_PTR = Marshal.StringToHGlobalAuto(Build_XML_String(ParamsArray, RoutingNames))
            Show_Message_Console(" sabd_routing_definition: Loaded", COLOR_BLACK, COLOR_GREEN, 0, TRACE_LOW, 0)
        Else
            Show_Message_Console(" No se pudo cargar tabla de configuracion sabd_routing_definition", COLOR_BLACK, COLOR_RED, 0, TRACE_LOW, 1)
            ErrorCode = PROCESS_ERROR
        End If
        ParamsTable = Nothing
        GC.Collect()
    End Sub

    Private Sub Load_Institution_Definition(ByVal ErrorCode As Byte)
        Dim ParamsTable As New List(Of String)
        ParamsTable.Clear()
        If Get_Table_Definition(ParamsTable, "sabd_institution_definition", " sabd_codigo_autorizador, sabd_nombre ") = SUCCESSFUL Then
            Dim ParamsArray() As String = ParamsTable.ToArray
            For x As Int16 = 0 To ParamsArray.Count - 1
                Dim aP() As String = ParamsArray(x).Split(New String() {"#"}, StringSplitOptions.RemoveEmptyEntries)
                Dim IR As New Institution_Record
                IR.SR_FI_Auth = aP(0)
                IR.SR_FI_Name = aP(1)
                IR.SR_FI_Enabled = True
                IR.SR_Date_Time = Now
                DIC_Institution.TryAdd(IR.SR_FI_Auth, IR)
                Console.WriteLine(IR.SR_FI_Auth & " - " & IR.SR_FI_Name)
                '------------------------------------------------------
                Dim SR As New STATE_RECORD
                SR.SABD_INSTITUTION_CODE = IR.SR_FI_Auth
                SR.SABD_CURRENT_STATUS = STATE_ON
                SR.SABD_CHANGUE_MODE = MODE_MANUAL
                SR.SABD_EVENT_SYS_DATE_TIME = Now
                DB_Fill_States_Institution(SR)
                '------------------------------------------------------
            Next
            Show_Message_Console(" sabd_institution_definition: Loaded", COLOR_BLACK, COLOR_GREEN, 0, TRACE_LOW, 0)
        Else
            Show_Message_Console(" No se pudo cargar tabla de configuracion sabd_institution_definition", COLOR_BLACK, COLOR_RED, 0, TRACE_LOW, 1)
            ErrorCode = PROCESS_ERROR
        End If
        ParamsTable = Nothing
        GC.Collect()
    End Sub


    Public Function Set_Get_Status_FI(ByVal Mode As Byte, ByVal sValue As Byte, ByVal FI As Int16) As Byte
        Dim ErrorCode As Byte = PROCESS_ERROR
        Dim nIR As New Institution_Record
        Dim oIR As New Institution_Record

        'Console.WriteLine("-------------------------------------------------------------------------------------")
        Select Case Mode
            Case 1
                If DIC_Institution.TryGetValue(FI, oIR) Then
                    Select Case sValue
                        Case 0
                            nIR = oIR
                            nIR.SR_FI_Enabled = False
                            nIR.SR_Date_Time = Now
                            If DIC_Institution.TryUpdate(FI, nIR, oIR) = True Then
                                Show_Message_Console(nIR.SR_FI_Name.TrimEnd & " ha sido marcada como DOWN automatica", COLOR_RED, COLOR_WHITE, 0, TRACE_LOW, 1)
                                '------------------------------------------------------
                                Dim SR As New STATE_RECORD
                                SR.SABD_INSTITUTION_CODE = nIR.SR_FI_Auth
                                SR.SABD_CURRENT_STATUS = STATE_OFF
                                SR.SABD_CHANGUE_MODE = MODE_AUTO
                                SR.SABD_EVENT_SYS_DATE_TIME = Now
                                DB_Changue_States_Institution(SR)
                                '------------------------------------------------------
                            Else
                                Show_Message_Console(nIR.SR_FI_Name.TrimEnd & ": No se encontró registro en memoria", COLOR_RED, COLOR_WHITE, 0, TRACE_LOW, 1)
                            End If
                        Case 1
                            nIR = oIR
                            nIR.SR_FI_Enabled = True
                            nIR.SR_Date_Time = Now
                            If DIC_Institution.TryUpdate(FI, nIR, oIR) = True Then
                                Show_Message_Console(nIR.SR_FI_Name.TrimEnd & " ha sido marcada como UP automatica", COLOR_BLUE, COLOR_WHITE, 0, TRACE_LOW, 1)
                                '------------------------------------------------------
                                Dim SR As New STATE_RECORD
                                SR.SABD_INSTITUTION_CODE = nIR.SR_FI_Auth
                                SR.SABD_CURRENT_STATUS = STATE_ON
                                SR.SABD_CHANGUE_MODE = MODE_AUTO
                                SR.SABD_EVENT_SYS_DATE_TIME = Now
                                DB_Changue_States_Institution(SR)
                                '------------------------------------------------------
                            Else
                                Show_Message_Console(nIR.SR_FI_Name.TrimEnd & ": No se encontró registro en memoria", COLOR_RED, COLOR_WHITE, 0, TRACE_LOW, 1)
                            End If

                    End Select
                Else
                    Console.WriteLine(GetDateTime() & " Institucion No encontrada en lista de Status")
                End If
                ErrorCode = SUCCESSFUL
            Case 0
                If DIC_Institution.TryGetValue(FI, oIR) Then
                    Select Case sValue
                        Case 0
                            nIR = oIR
                            nIR.SR_FI_Enabled = False
                            nIR.SR_Date_Time = Now
                            If DIC_Institution.TryUpdate(FI, nIR, oIR) = True Then
                                Show_Message_Console(nIR.SR_FI_Name.TrimEnd & " ha sido marcada como DOWN manual", COLOR_RED, COLOR_WHITE, 0, TRACE_LOW, 1)
                                '------------------------------------------------------
                                Dim SR As New STATE_RECORD
                                SR.SABD_INSTITUTION_CODE = nIR.SR_FI_Auth
                                SR.SABD_CURRENT_STATUS = STATE_OFF
                                SR.SABD_CHANGUE_MODE = MODE_MANUAL
                                SR.SABD_EVENT_SYS_DATE_TIME = Now
                                DB_Changue_States_Institution(SR)
                                '------------------------------------------------------
                            Else
                                Show_Message_Console(nIR.SR_FI_Name.TrimEnd & ": No se encontró registro en memoria", COLOR_RED, COLOR_WHITE, 0, TRACE_LOW, 1)
                            End If
                        Case 1
                            nIR = oIR
                            nIR.SR_FI_Enabled = True
                            nIR.SR_Date_Time = Now
                            If DIC_Institution.TryUpdate(FI, nIR, oIR) = True Then
                                Show_Message_Console(nIR.SR_FI_Name.TrimEnd & " ha sido marcada como UP manual", COLOR_BLUE, COLOR_WHITE, 0, TRACE_LOW, 1)
                                '------------------------------------------------------
                                Dim SR As New STATE_RECORD
                                SR.SABD_INSTITUTION_CODE = nIR.SR_FI_Auth
                                SR.SABD_CURRENT_STATUS = STATE_ON
                                SR.SABD_CHANGUE_MODE = MODE_MANUAL
                                SR.SABD_EVENT_SYS_DATE_TIME = Now
                                DB_Changue_States_Institution(SR)
                                '------------------------------------------------------
                            Else
                                Show_Message_Console(nIR.SR_FI_Name.TrimEnd & ": No se encontró registro en memoria", COLOR_RED, COLOR_WHITE, 0, TRACE_LOW, 1)
                            End If
                    End Select
                Else
                    Console.WriteLine(GetDateTime() & " Institucion No encontrada en lista de Status")
                End If
                ErrorCode = SUCCESSFUL
            Case 2
                Dim IR As New Institution_Record
                If DIC_Institution.TryGetValue(FI, IR) Then
                    If IR.SR_FI_Enabled Then
                        ErrorCode = SUCCESSFUL
                    Else
                        Show_Message_Console(IR.SR_FI_Name.TrimEnd & " is DOWN ", COLOR_BLACK, COLOR_YELLOW, 0, TRACE_LOW, 0)
                        ErrorCode = PROCESS_ERROR
                    End If
                End If
        End Select
        'Console.WriteLine("-------------------------------------------------------------------------------------")

        Return ErrorCode
    End Function


    Public Sub Start_Process_Look_Iteract_OBJDC()

        ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf Process_Look_Iteract_OBJDC))

    End Sub

    Private Sub Process_Look_Iteract_OBJDC()
        Show_Message_Console(MyName & " Starting Process_Look_Iteract_OBJDC", COLOR_BLACK, COLOR_WHITE, 0, TRACE_LOW, 1)

        Do While True
            Dim enumerator As IEnumerator(Of KeyValuePair(Of Int16, Institution_Record)) = DIC_Institution.GetEnumerator()
            While enumerator.MoveNext()
                Dim pair As KeyValuePair(Of Int16, Institution_Record) = enumerator.Current
                Dim TSP As TimeSpan = Now.Subtract(pair.Value.SR_Date_Time)
                If pair.Value.SR_FI_Enabled = False Then
                    If TSP.TotalMinutes >= EnableTime Then
                        Dim nIR As New Institution_Record
                        nIR = pair.Value
                        nIR.SR_FI_Enabled = True
                        nIR.SR_Date_Time = Now

                        '-----------------------------------------------------------
                        Set_Get_Status_FI(1, 1, pair.Key)
                        '-----------------------------------------------------------

                        'If DIC_Institution.TryUpdate(pair.Key, nIR, pair.Value) = True Then
                        '    Show_Message_Console(nIR.SR_FI_Name.TrimEnd & " ha sido marcada como UP automatica", COLOR_BLUE, COLOR_WHITE, 0, TRACE_LOW, 1)
                        'Else
                        '    Show_Message_Console(nIR.SR_FI_Name.TrimEnd & ": No se encontró registro en memoria", COLOR_RED, COLOR_WHITE, 0, TRACE_LOW, 1)
                        'End If
                    End If
                End If
            End While
            '-----------------------------------
            Thread.Sleep(5000)
            '-----------------------------------
            ClearMemory()
        Loop
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

    Public Property SetGetProcess_PTR() As IntPtr
        Get
            Return process_PTR
        End Get
        Set(ByVal value As IntPtr)
            process_PTR = value
        End Set
    End Property

    Public Property SetGetTransaction_PTR() As IntPtr
        Get
            Return transaction_PTR
        End Get
        Set(ByVal value As IntPtr)
            transaction_PTR = value
        End Set
    End Property

    Public Property SetGetRouting_PTR() As IntPtr
        Get
            Return routing_PTR
        End Get
        Set(ByVal value As IntPtr)
            routing_PTR = value
        End Set
    End Property

    'Public Property SetGetInstitution_PTR() As IntPtr
    '    Get
    '        Return institution_PTR
    '    End Get
    '    Set(ByVal value As IntPtr)
    '        institution_PTR = value
    '    End Set
    'End Property

    'Public Sub Put_Message_SAF_Message(ByVal SAF As SAF)
    '    Dim lockOBJ_IN As Object = New Object
    '    '
    '    ' On WebService Adquirence Can't get the Id Process
    '    '  
    '    SyncLock lockOBJ_IN
    '        Try
    '            Dim MessageToSend As Message = New Message
    '            Dim filter = New MessagePropertyFilter()
    '            With filter
    '                .AdministrationQueue = False
    '                .ArrivedTime = True
    '                .CorrelationId = True
    '                .Priority = False
    '                .ResponseQueue = False
    '                .SentTime = True
    '                .Body = True
    '                .Label = True
    '                .Id = True
    '            End With
    '            MessageToSend.Label = SAF.SSM.SSM_Queue_Message_ID & "|" & Now.ToString("yyyy-MM-dd HH:mm:ss.fff")
    '            MessageToSend.CorrelationId = SAF.SSM.SSM_Queue_Message_ID
    '            Dim QueueSendData As New MessageQueue(SAF_Router_Queue_Name)
    '            QueueSendData.MessageReadPropertyFilter = filter
    '            MessageToSend.Body = SAF
    '            QueueSendData.Send(MessageToSend)
    '            QueueSendData.Dispose()
    '            MessageToSend.Dispose()
    '            SAF = Nothing
    '        Catch ex As Exception
    '            Show_Message_Console(MyName & " Exception Putting Message on " & " " & " ->" & ex.Message, COLOR_OWNER1, COLOR_RED, 0, TRACE_LOW, 1)
    '        End Try
    '    End SyncLock
    'End Sub

    'Public Sub Put_Message_To_Logger(ByVal Struct_Shared_Message As SharedStructureMessage, ByVal SystemQueueName As String)
    Public Sub Put_Message_To_Logger(ByVal Struct_Shared_Message As SharedStructureMessage)
        Dim lockOBJ_IN As Object = New Object
        Dim SystemQueueName As String = PrivateQueue & Get_Logger_Queue()

        If Not Verify_Process_Up(Get_Logger_Name) Then
            Show_Message_Console(MyName & " Logger not available ", COLOR_BLACK, COLOR_YELLOW, 0, TRACE_LOW, 1)
            If Verify_Queue_Resource(SystemQueueName) = PROCESS_ERROR Then
                Exit Sub
            End If
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

    'Private Sub Process_Auto_Enable_Out_FI()
    '    Console.WriteLine(GetDateTime() & " Init Process_Auto_Enable_Out_FI")
    '    Do While True
    '        If HT_Fi_Down.Count > 0 Then
    '            For Each DicEnt In HT_Fi_Down
    '                Dim DTT As DateTime = DicEnt.value
    '                Dim TSP As New TimeSpan
    '                TSP = Now.Subtract(DTT)
    '                Console.WriteLine(GetDateTime() & " Process_Check_Exceeded_Out FI=" & DicEnt.key & " DTT=" & DTT.ToString("yyyy-MM-dd HH:mm:ss.fff") & " Secs=" & TSP.TotalSeconds)
    '                If TSP.Minutes >= My.Settings.AutoEnable Then
    '                    Put_Notify_To_Router(Router_NOTIFY & "1|1|" & DicEnt.key)
    '                    HT_Fi_Down.Remove(DicEnt.key)
    '                    Console.WriteLine(GetDateTime() & " Process_Check_Exceeded_Out Notified....")
    '                    Continue Do
    '                End If
    '            Next
    '            Console.Write(".")
    '            Thread.Sleep(5000)
    '            ClearMemory()
    '        Else
    '            Console.Write(",")
    '            Thread.Sleep(5000)
    '            ClearMemory()
    '        End If
    '    Loop

    'End Sub


End Module


Public Structure SAF

    Dim SAF_Key As String
    Dim SAF_DateTime As DateTime
    Dim SSM As SharedStructureMessage

End Structure

Public Structure Institution_Record
    Dim SR_FI_Auth As Int16
    Dim SR_FI_Name As String
    Dim SR_FI_Enabled As Boolean
    Dim SR_Date_Time As DateTime
End Structure
