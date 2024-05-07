Imports System.Messaging
Imports System.Runtime.InteropServices
Imports System.Globalization
Imports System.IO


' NOTE: You can use the "Rename" command on the context menu to change the class name "Service1" in code, svc and config file together.
' NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.vb at the Solution Explorer and start debugging.
<ServiceBehavior(InstanceContextMode:=InstanceContextMode.Single, ConcurrencyMode:=ConcurrencyMode.Multiple)>
Public Class Service1
    Implements Service_AsynchroniousMain

    'Dim SeqCorrelationID As Int64
    'Const Sumator As Int64 = 1000000
    Const TranType_ROUTER_IN_SCP As Char = "R"
    Const TranType_REPLY As Char = "P"
    Const TranType_COMMAND As Char = "C"
    Const TranType_ROUTER_OUT_SCP As Char = "O"
    Const PathQ As String = ".\Private$\"
    Const OWNER_format As Byte = 103
    Const Service_PDR_BANRED As String = "0020031003"
    Const module_ID_BANRED As Int16 = 1003

    Public Const FI_CPN_ADQ As Int16 = 188
    Public Const FI_CPN_AUT As Int16 = 728
    Public Const ABA_CPN_ADQ As Int32 = 140325


    Public Sub New()

        PathLog = "C:\ServiceSwitch\ServiSwitch_WebService\LOGS"
        If Not Directory.Exists(PathLog) Then
            Directory.CreateDirectory(PathLog)
        End If

    End Sub

    Public Function Invoque_Service_Asynchronious(ByVal Xml_Request As String) As String Implements Service_AsynchroniousMain.Invoque_Service_Asynchronious

        Return Process_Request_Message(Xml_Request)

        'Return String.Format("Process Completed Succesfully at " & Now.ToLongTimeString, Xml_Request)
    End Function


    Public Function GetDataUsingDataContract(ByVal composite As CompositeType) As CompositeType Implements Service_AsynchroniousMain.GetDataUsingDataContract
        If composite Is Nothing Then
            Throw New ArgumentNullException("composite")
        End If
        If composite.BoolValue Then
            composite.StringValue &= "Suffix"
        End If
        Return composite
    End Function

    Public Function Get_Status_Service(ModuleID As Short) As String Implements Service_AsynchroniousMain.Get_Status_Service

        Return String.Format("CODE 0000: " & Format(System.DateTime.Now, "yyyy-MM-dd hh:mm:ss.fff"))

    End Function

    Private Function Process_Request_Message(ByVal XmlRequestMessage As String) As String
        Dim XmlReplyMessage As String = String.Empty
        Dim INIT As New Messages
        Dim BUSS As New SharedStructureMessage

        Continous_Save_Log(XmlRequestMessage)

        ' -----------------------------------------------------------------------------
        ' Initialize Internal BUSS
        ' -----------------------------------------------------------------------------
        INIT.Initialize_Internal_Buss(BUSS)

        ' -----------------------------------------------------------------------------
        ' Fill Array from XML msg
        ' -----------------------------------------------------------------------------
        Dim XmlArrayList As List(Of String) = GetArrayXMLFields(XmlRequestMessage)
        If XmlArrayList.Count = 0 Then
            Initialize_XmlArrayList(XmlArrayList)
            XmlArrayList(Constanting_Definition.rep_ID_Response) = Constanting_Definition.ERROR_Xml_string
            XmlReplyMessage = Generate_XML_Reply(XmlArrayList)
            Continous_Save_Log(XmlReplyMessage)
            Return XmlReplyMessage
        End If
        ' ----------------------------------------------------------------------------
        If Fill_Buss_Data(XmlArrayList, BUSS) = SUCCESSFUL Then
            Send_And_Wait(BUSS, XmlReplyMessage)
        End If
        ' ----------------------------------------------------------------------------

        BUSS.SSM_Instance_Times += "T16_" & GetDateTime() & "|"
        Continous_Save_Log(XmlReplyMessage & ControlChars.CrLf & BUSS.SSM_Instance_Times)

        ' ----------------------------------------------------------------------------
        Return XmlReplyMessage
        ' ----------------------------------------------------------------------------


    End Function

    Private Sub Send_And_Wait(ByRef BUSSRQ As SharedStructureMessage, ByRef XmlReplyMessage As String)
        Dim WaitCorrelationID As String = String.Empty
        ' 
        ' Filling the Header
        '
        BUSSRQ.SSM_Adq_Queue_Reply_Name = PathQ & My.Settings.ReplyQueue
        BUSSRQ.SSM_Rout_Queue_Request_Name = PathQ & My.Settings.RequestRouter
        BUSSRQ.SSM_Adq_Source_Name = My.Settings.MyName
        'BUSSRQ.SSM_Queue_Message_ID = GetCorrelationID() 2020-03-25  
        BUSSRQ.SSM_Queue_Message_ID = Now.Millisecond
        WaitCorrelationID = BUSSRQ.SSM_Queue_Message_ID
        BUSSRQ.SSM_Instance_Times += "T2_" & GetDateTime() & "|"
        BUSSRQ.SSM_Transaction_Indicator = TranType_ROUTER_IN_SCP
        BUSSRQ.SSM_Communication_ID = Constanting_Definition.from_WEBSERVICE
        BUSSRQ.SSM_Message_Format = Constanting_Definition.OWNER_format

        '***************************************************************************************
        Dim arrTypes(1) As System.Type
        arrTypes(0) = GetType(SharedStructureMessage)
        arrTypes(1) = GetType(Object)

        Dim msg As New Message
        Dim MsgQ As New MessageQueue(BUSSRQ.SSM_Rout_Queue_Request_Name)
        With msg
            .Body = BUSSRQ
            .TimeToBeReceived = New TimeSpan(0, 0, 20)
            '.CorrelationId = BUSSRQ.SSM_Queue_Message_ID 2020-03-25
            .Label = BUSSRQ.SSM_Queue_Message_ID
        End With
        Dim XmlArrayList As New List(Of String)
        '**********************************************************************************************
        Try
            MsgQ.Send(msg)
            WaitCorrelationID = msg.Id   '2020-03-25
            'Continous_Save_Log("Send ID message:" & msg.Id & " Sequence:" & BUSSRQ.SSM_Common_Data.CRF_Adquirer_Sequence)
        Catch ex As Exception
            Initialize_XmlArrayList(XmlArrayList)
            XmlArrayList(Constanting_Definition.rep_ID_Terminal) = Constanting_Definition.ERROR_Sending
            XmlArrayList(Constanting_Definition.rep_ID_Response) = Constanting_Definition.ERROR_On_Reply
            XmlArrayList(Constanting_Definition.rep_ID_Transaction) = BUSSRQ.SSM_Common_Data.CRF_Transaction_Code
            XmlArrayList(Constanting_Definition.rep_ID_DateTime) = BUSSRQ.SSM_Common_Data.CRF_Adquirer_Date_Time
            XmlArrayList(Constanting_Definition.rep_ID_Sequence) = BUSSRQ.SSM_Common_Data.CRF_Adquirer_Sequence
            XmlArrayList(Constanting_Definition.rep_ID_Switch_Sequence) = BUSSRQ.SSM_Common_Data.CRF_Switch_Sequence
            XmlReplyMessage = Generate_XML_Reply(XmlArrayList)
            Exit Sub
        End Try
        '***************************************************************************************
        '
        ' Receive Transaction by Original CorrelationID
        Dim BUSSRP As New SharedStructureMessage
        '***************************************************************************************
        Dim arrTypes2(1) As System.Type
        arrTypes2(0) = GetType(SharedStructureMessage)
        arrTypes2(1) = GetType(Object)
        Dim Receive_msg As New Message
        Dim msgR As New MessageQueue(BUSSRQ.SSM_Adq_Queue_Reply_Name)
        '***************************************************************************************
        Try
            msgR.Formatter = New XmlMessageFormatter(arrTypes)
            'Continous_Save_Log("Wait ID message:" & msg.Id & " Sequence:" & BUSSRQ.SSM_Common_Data.CRF_Adquirer_Sequence)
            Receive_msg = msgR.ReceiveByCorrelationId(WaitCorrelationID, New TimeSpan(0, 0, 40))
            BUSSRP = CType(Receive_msg.Body, SharedStructureMessage)
            If IsNothing(BUSSRP) Then
                Initialize_XmlArrayList(XmlArrayList)
                XmlArrayList(Constanting_Definition.rep_ID_Terminal) = BUSSRQ.SSM_Common_Data.CRF_Terminal_ID
                XmlArrayList(Constanting_Definition.rep_ID_Response) = Constanting_Definition.ERROR_On_Reply
                XmlArrayList(Constanting_Definition.rep_ID_Transaction) = BUSSRQ.SSM_Common_Data.CRF_Transaction_Code
                XmlArrayList(Constanting_Definition.rep_ID_DateTime) = BUSSRQ.SSM_Common_Data.CRF_Adquirer_Date_Time
                XmlArrayList(Constanting_Definition.rep_ID_Sequence) = BUSSRQ.SSM_Common_Data.CRF_Adquirer_Sequence
                XmlArrayList(Constanting_Definition.rep_ID_Switch_Sequence) = BUSSRQ.SSM_Common_Data.CRF_Switch_Sequence
                XmlReplyMessage = Generate_XML_Reply(XmlArrayList)
                Exit Sub
            Else
                BUSSRQ.SSM_Instance_Times = BUSSRP.SSM_Instance_Times
                BUSSRQ.SSM_Instance_Times += "T15_" & GetDateTime() & "|"
            End If
        Catch qe As MessageQueueException
            If qe.ErrorCode = TIMEOUT Then
                Initialize_XmlArrayList(XmlArrayList)
                XmlArrayList(Constanting_Definition.rep_ID_Terminal) = BUSSRQ.SSM_Common_Data.CRF_Terminal_ID
                XmlArrayList(Constanting_Definition.rep_ID_Response) = Constanting_Definition.ERROR_Timeut
                XmlArrayList(Constanting_Definition.rep_ID_Transaction) = BUSSRQ.SSM_Common_Data.CRF_Transaction_Code
                XmlArrayList(Constanting_Definition.rep_ID_DateTime) = BUSSRQ.SSM_Common_Data.CRF_Adquirer_Date_Time
                XmlArrayList(Constanting_Definition.rep_ID_Sequence) = BUSSRQ.SSM_Common_Data.CRF_Adquirer_Sequence
                XmlArrayList(Constanting_Definition.rep_ID_Switch_Sequence) = BUSSRQ.SSM_Common_Data.CRF_Switch_Sequence
                XmlReplyMessage = Generate_XML_Reply(XmlArrayList)
                Exit Sub
            End If
        Catch ex As Exception
            Initialize_XmlArrayList(XmlArrayList)
            XmlArrayList(Constanting_Definition.rep_ID_Terminal) = BUSSRQ.SSM_Common_Data.CRF_Terminal_ID
            XmlArrayList(Constanting_Definition.rep_ID_Response) = Constanting_Definition.ERROR_Unknow
            XmlArrayList(Constanting_Definition.rep_ID_Transaction) = BUSSRQ.SSM_Common_Data.CRF_Transaction_Code
            XmlArrayList(Constanting_Definition.rep_ID_DateTime) = BUSSRQ.SSM_Common_Data.CRF_Adquirer_Date_Time
            XmlArrayList(Constanting_Definition.rep_ID_Sequence) = BUSSRQ.SSM_Common_Data.CRF_Adquirer_Sequence
            XmlArrayList(Constanting_Definition.rep_ID_Switch_Sequence) = BUSSRQ.SSM_Common_Data.CRF_Switch_Sequence
            Exit Sub
        End Try
        '***************************************************************************************
        Select Case BUSSRP.SSM_Common_Data.CRF_Response_Code
            Case "89"
                BUSSRP.SSM_Common_Data.CRF_Response_Code = Constanting_Definition.ERROR_Unknow
            Case "91"
                BUSSRP.SSM_Common_Data.CRF_Response_Code = Constanting_Definition.ERROR_Local_Disconnect
                'Case "8003"
                '    BUSSRP.SSM_Common_Data.CRF_Response_Code = Constanting_Definition.ERROR_Transaction_Undefined
                'Case "7803"
                '    BUSSRP.SSM_Common_Data.CRF_Response_Code = Constanting_Definition.ERROR_Not_Route
                'Case "7806"
                BUSSRP.SSM_Common_Data.CRF_Response_Code = Constanting_Definition.ERROR_Inactive_Proces
            Case "7802"
                BUSSRP.SSM_Common_Data.CRF_Response_Code = Constanting_Definition.ERROR_Undefined_Process
        End Select

        '***************************************************************************************
        Initialize_XmlArrayList(XmlArrayList)
        '***************************************************************************************
        Try
            Retrieve_Buss_Data(XmlArrayList, BUSSRP)
            XmlReplyMessage = Generate_XML_Reply(XmlArrayList)
        Catch ex As Exception
            XmlArrayList(Constanting_Definition.rep_ID_Response) = Constanting_Definition.ERROR_Unknow
            XmlReplyMessage = Generate_XML_Reply(XmlArrayList)
        End Try
        '***************************************************************************************
    End Sub

    Private Function Retrieve_Buss_Data(ByRef XmlArrayList As List(Of String), ByVal BUSSRP As SharedStructureMessage) As Byte
        Dim ErrorCode As Byte = UNKNOW
        Dim RequestMessage As String = String.Empty
        Dim Token_Data As String = String.Empty
        Dim TKNid(20) As String
        Dim TKNdata(20) As String

        Try
            If BUSSRP.SSM_Common_Data.CRF_Token_Data.Length > 0 Then
                Dim x, y As Integer
                Token_Data = BUSSRP.SSM_Common_Data.CRF_Token_Data
                Do While Token_Data.Length > 0
                    TKNid(x) = Token_Data.Substring(0, 2)
                    Token_Data = Token_Data.Remove(0, 2)
                    y = Token_Data.Substring(0, 3)
                    Token_Data = Token_Data.Remove(0, 3)
                    TKNdata(x) = Token_Data.Substring(0, y)
                    Token_Data = Token_Data.Remove(0, y)
                    x += 1
                Loop
                Array.Resize(TKNid, x)
                Array.Resize(TKNdata, x)
            End If
        Catch ex As Exception
            Console.WriteLine("Error # 001:" & ex.Message)
        End Try

        Select Case BUSSRP.SSM_Common_Data.CRF_Transaction_Code
            Case 163
                If CInt(BUSSRP.SSM_Common_Data.CRF_Response_Code) = 0 Then
                    Try
                        XmlArrayList(Constanting_Definition.rep_ID_TargetAccountType) = Get_Token_Info(ACCT_TYPE_TARGET_ID, TKNid, TKNdata)
                        XmlArrayList(Constanting_Definition.rep_ID_TargetAccountNumber) = BUSSRP.SSM_Common_Data.CRF_Secondary_Account
                        XmlArrayList(Constanting_Definition.rep_ID_TargetAccountName) = BUSSRP.SSM_Common_Data.CRF_Names
                        XmlArrayList(Constanting_Definition.rep_ID_TargetContact) = BUSSRP.SSM_Common_Data.CRF_PhoneNumber
                        XmlArrayList(Constanting_Definition.rep_ID_SourceReference) = BUSSRP.SSM_Common_Data.CRF_Reference
                        XmlArrayList(Constanting_Definition.rep_ID_TargetIdentification) = Get_Token_Info(TARGET_DOCUMENT_ID, TKNid, TKNdata)
                        XmlArrayList(Constanting_Definition.rep_ID_ccTotalAmt) = "00000000"
                        XmlArrayList(Constanting_Definition.rep_ID_ccMinimunAmt) = "00000000"
                        XmlArrayList(Constanting_Definition.rep_ID_ccLimitDate) = "00000000"
                        XmlArrayList(Constanting_Definition.rep_ID_ccContact) = "0000000000"
                    Catch ex As Exception
                        XmlArrayList(Constanting_Definition.rep_ID_TargetAccountType) = "00"
                        XmlArrayList(Constanting_Definition.rep_ID_TargetAccountNumber) = "000000000000000000"
                        XmlArrayList(Constanting_Definition.rep_ID_TargetAccountName) = " "
                        XmlArrayList(Constanting_Definition.rep_ID_TargetContact) = "0000000000"
                        XmlArrayList(Constanting_Definition.rep_ID_SourceReference) = " "
                        XmlArrayList(Constanting_Definition.rep_ID_TargetIdentification) = "0000000000"
                        XmlArrayList(Constanting_Definition.rep_ID_ccMinimunAmt) = "00000000"
                        XmlArrayList(Constanting_Definition.rep_ID_ccTotalAmt) = "00000000"
                        XmlArrayList(Constanting_Definition.rep_ID_ccLimitDate) = "00000000"
                        XmlArrayList(Constanting_Definition.rep_ID_ccContact) = "0000000000"
                    End Try
                Else
                    XmlArrayList(Constanting_Definition.rep_ID_TargetAccountType) = "00"
                    XmlArrayList(Constanting_Definition.rep_ID_TargetAccountNumber) = "000000000000000000"
                    XmlArrayList(Constanting_Definition.rep_ID_TargetAccountName) = " "
                    XmlArrayList(Constanting_Definition.rep_ID_TargetContact) = "0000000000"
                    XmlArrayList(Constanting_Definition.rep_ID_SourceReference) = " "
                    XmlArrayList(Constanting_Definition.rep_ID_TargetIdentification) = "0000000000"
                    XmlArrayList(Constanting_Definition.rep_ID_ccMinimunAmt) = "00000000"
                    XmlArrayList(Constanting_Definition.rep_ID_ccTotalAmt) = "00000000"
                    XmlArrayList(Constanting_Definition.rep_ID_ccLimitDate) = "00000000"
                    XmlArrayList(Constanting_Definition.rep_ID_ccContact) = "0000000000"
                End If
            Case 439, 539
                If CInt(BUSSRP.SSM_Common_Data.CRF_Response_Code) = 0 Then
                    Try
                        XmlArrayList(Constanting_Definition.rep_ID_TargetAccountType) = Get_Token_Info(ACCT_TYPE_TARGET_ID, TKNid, TKNdata)
                        XmlArrayList(Constanting_Definition.rep_ID_TargetAccountNumber) = BUSSRP.SSM_Common_Data.CRF_Secondary_Account
                        XmlArrayList(Constanting_Definition.rep_ID_TargetAccountName) = BUSSRP.SSM_Common_Data.CRF_Names
                        XmlArrayList(Constanting_Definition.rep_ID_TargetContact) = BUSSRP.SSM_Common_Data.CRF_PhoneNumber
                        XmlArrayList(Constanting_Definition.rep_ID_SourceReference) = BUSSRP.SSM_Common_Data.CRF_Reference
                        XmlArrayList(Constanting_Definition.rep_ID_TargetIdentification) = Get_Token_Info(TARGET_DOCUMENT_ID, TKNid, TKNdata)
                        XmlArrayList(Constanting_Definition.rep_ID_ccTotalAmt) = "00000000"
                        XmlArrayList(Constanting_Definition.rep_ID_ccMinimunAmt) = "00000000"
                        XmlArrayList(Constanting_Definition.rep_ID_ccLimitDate) = "00000000"
                        XmlArrayList(Constanting_Definition.rep_ID_ccContact) = "0000000000"
                    Catch ex As Exception
                        XmlArrayList(Constanting_Definition.rep_ID_TargetAccountType) = "00"
                        XmlArrayList(Constanting_Definition.rep_ID_TargetAccountNumber) = "000000000000000000"
                        XmlArrayList(Constanting_Definition.rep_ID_TargetAccountName) = " "
                        XmlArrayList(Constanting_Definition.rep_ID_TargetContact) = "0000000000"
                        XmlArrayList(Constanting_Definition.rep_ID_SourceReference) = " "
                        XmlArrayList(Constanting_Definition.rep_ID_TargetIdentification) = "0000000000"
                        XmlArrayList(Constanting_Definition.rep_ID_ccMinimunAmt) = "00000000"
                        XmlArrayList(Constanting_Definition.rep_ID_ccTotalAmt) = "00000000"
                        XmlArrayList(Constanting_Definition.rep_ID_ccLimitDate) = "00000000"
                        XmlArrayList(Constanting_Definition.rep_ID_ccContact) = "0000000000"
                    End Try
                Else
                    XmlArrayList(Constanting_Definition.rep_ID_TargetAccountType) = "00"
                    XmlArrayList(Constanting_Definition.rep_ID_TargetAccountNumber) = "000000000000000000"
                    XmlArrayList(Constanting_Definition.rep_ID_TargetAccountName) = " "
                    XmlArrayList(Constanting_Definition.rep_ID_TargetContact) = "0000000000"
                    XmlArrayList(Constanting_Definition.rep_ID_SourceReference) = " "
                    XmlArrayList(Constanting_Definition.rep_ID_TargetIdentification) = "0000000000"
                    XmlArrayList(Constanting_Definition.rep_ID_ccMinimunAmt) = "00000000"
                    XmlArrayList(Constanting_Definition.rep_ID_ccTotalAmt) = "00000000"
                    XmlArrayList(Constanting_Definition.rep_ID_ccLimitDate) = "00000000"
                    XmlArrayList(Constanting_Definition.rep_ID_ccContact) = "0000000000"
                End If
        End Select

        XmlArrayList(Constanting_Definition.rep_ID_Terminal) = BUSSRP.SSM_Common_Data.CRF_Terminal_ID
        XmlArrayList(Constanting_Definition.rep_ID_Response) = BUSSRP.SSM_Common_Data.CRF_Response_Code
        XmlArrayList(Constanting_Definition.rep_ID_Transaction) = BUSSRP.SSM_Common_Data.CRF_Transaction_Code
        XmlArrayList(Constanting_Definition.rep_ID_DateTime) = BUSSRP.SSM_Common_Data.CRF_Adquirer_Date_Time.ToString("yyyy-MM-dd HH:mm:ss")
        XmlArrayList(Constanting_Definition.rep_ID_Sequence) = BUSSRP.SSM_Common_Data.CRF_Adquirer_Sequence
        XmlArrayList(Constanting_Definition.rep_ID_Switch_Sequence) = BUSSRP.SSM_Common_Data.CRF_Switch_Sequence

        Return ErrorCode
    End Function

    Private Function Fill_Buss_Data(ByVal XmlArrayList As List(Of String), ByRef BUSS As SharedStructureMessage) As Integer
        Dim USER_DATA As String = String.Empty
        Dim ErrorCode As Int16 = Constanting_Definition.ERROR_Unknow

        Try
            BUSS.SSM_Common_Data.CRF_Terminal_ID = XmlArrayList(Constanting_Definition.req_ID_Terminal)
            BUSS.SSM_Common_Data.CRF_Adquirer_Sequence = XmlArrayList(Constanting_Definition.req_ID_Sequence)
            BUSS.SSM_Common_Data.CRF_Transaction_Code = XmlArrayList(Constanting_Definition.req_ID_Transaction)
            BUSS.SSM_Common_Data.CRF_Adquirer_Date_Time = XmlArrayList(Constanting_Definition.req_ID_DateTime)
            BUSS.SSM_Common_Data.CRF_Channel_Id = XmlArrayList(Constanting_Definition.req_ID_Channel)
            'BUSS.SSM_Common_Data.CRF_Transaction_Amount = XmlArrayList(Constanting_Definition.req_ID_TranAmount)
            Dim CultureLN As New CultureInfo("en-US")
            BUSS.SSM_Common_Data.CRF_Transaction_Amount = Decimal.Parse(XmlArrayList(Constanting_Definition.req_ID_TranAmount), CultureLN)
            BUSS.SSM_Common_Data.CRF_Primary_Account = XmlArrayList(Constanting_Definition.req_ID_LocalAcctNumber)
            BUSS.SSM_Common_Data.CRF_Secondary_Account = XmlArrayList(Constanting_Definition.req_ID_TargetAcctNumber)
            BUSS.SSM_Common_Data.CRF_Names = XmlArrayList(Constanting_Definition.req_ID_LocalAcctName)
            BUSS.SSM_Common_Data.CRF_Reversal_Indicator = XmlArrayList(Constanting_Definition.req_ID_ReverseReason)
            BUSS.SSM_Common_Data.CRF_Branch_ID = XmlArrayList(Constanting_Definition.req_ID_BranchCode)
            BUSS.SSM_Common_Data.CRF_Operator_ID = XmlArrayList(Constanting_Definition.req_ID_OpratorCode)
            BUSS.SSM_Common_Data.CRF_PhoneNumber = XmlArrayList(Constanting_Definition.req_ID_ContactNbr)
            BUSS.SSM_Common_Data.CRF_Reference = XmlArrayList(Constanting_Definition.req_ID_Reference)

            Select Case BUSS.SSM_Common_Data.CRF_Transaction_Code
                Case 163, 439, 539, 239, 165
                    BUSS.SSM_Common_Data.CRF_Service_Indicator = Service_PDR_BANRED
                    BUSS.SSM_Common_Data.CRF_Issuer_Institution_ID = module_ID_BANRED
            End Select

            BUSS.SSM_Common_Data.CRF_Adquirer_Institution_Number = FI_CPN_ADQ
            BUSS.SSM_Common_Data.CRF_Adquirer_Institution_ID = ABA_CPN_ADQ

            '2022-10-31 
            BUSS.SSM_Common_Data.CRF_Issuer_Institution_Number = XmlArrayList(Constanting_Definition.req_ID_Bank_id)
            '2022-10-31 


            '
            '*************************  User Data 
            '
            Build_User_Token(USER_DATA, ACCT_TYPE_TARGET_ID, XmlArrayList(Constanting_Definition.req_ID_TargetAcctType))
            Build_User_Token(USER_DATA, ACCT_TYPE_ID, XmlArrayList(Constanting_Definition.req_ID_SourceAcctType))
            'Build_User_Token(USER_DATA, ACCT_TYPE_TARGET_ID, XmlArrayList(Constanting_Definition.req_ID_SourceAcctType))
            Build_User_Token(USER_DATA, SERVICES, XmlArrayList(Constanting_Definition.req_ID_ServiceCode))
            Build_User_Token(USER_DATA, AUTHPD_ID, XmlArrayList(Constanting_Definition.req_ID_Bank_id))
            'Build_User_Token(USER_DATA, ACCT_TYPE_TARGET_ID, XmlArrayList(Constanting_Definition.req_ID_SourceAcctType))
            Build_User_Token(USER_DATA, NAMES_ID, XmlArrayList(Constanting_Definition.req_ID_LocalAcctName))
            Build_User_Token(USER_DATA, DOCUMENT_ID, XmlArrayList(Constanting_Definition.req_ID_SourceIdentificaction))
            Build_User_Token(USER_DATA, TARGET_DOCUMENT_ID, XmlArrayList(Constanting_Definition.req_ID_TargetIdentificaction))

            BUSS.SSM_Common_Data.CRF_Token_Data = USER_DATA

            ErrorCode = SUCCESSFUL
        Catch ex As Exception
            BUSS = Nothing
            ErrorCode = Constanting_Definition.ERROR_Buss_Asign
        End Try

        Return ErrorCode
    End Function


    Private Function GetCorrelationID() As String
        Dim RND As New Random
        Dim Value As Int64 = RND.Next(999999)
        Value = 1000000 + Value
        Dim CorrelationID As String = "12384943-2451-a34f-414b-2b2731493115\" & Value.ToString

        'Dim CorrelationID As String = String.Empty
        'Dim NumericBuffer As String = String.Empty
        'Dim ComplementInt As Int64
        'Dim TempVal As Byte

        'NumericBuffer = (Now.Year * Now.Millisecond).ToString & (Now.Month * Now.Millisecond).ToString & (Now.Day * Now.Millisecond).ToString & (Now.Hour * Now.Millisecond).ToString & (Now.Minute * Now.Millisecond) & (Now.Second * Now.Millisecond).ToString & Now.Millisecond.ToString("00")
        'NumericBuffer = NumericBuffer & (Now.Year * Now.Millisecond).ToString & (Now.Month * Now.Millisecond).ToString & (Now.Day * Now.Millisecond).ToString & (Now.Hour * Now.Millisecond).ToString & (Now.Minute * Now.Millisecond) & (Now.Second * Now.Millisecond).ToString & Now.Millisecond.ToString("00")
        'NumericBuffer = NumericBuffer & (Now.Year * Now.Millisecond).ToString & (Now.Month * Now.Millisecond).ToString & (Now.Day * Now.Millisecond).ToString & (Now.Hour * Now.Millisecond).ToString & (Now.Minute * Now.Millisecond) & (Now.Second * Now.Millisecond).ToString & Now.Millisecond.ToString("00")

        'Do While NumericBuffer.Length >= 2
        '    TempVal = NumericBuffer.Substring(0, 2)
        '    CorrelationID = CorrelationID & Convert.ToString(TempVal, 16)
        '    NumericBuffer = NumericBuffer.Remove(0, 2)
        'Loop

        'CorrelationID = CorrelationID.Insert(8, "-")
        'CorrelationID = CorrelationID.Insert(13, "-")
        'CorrelationID = CorrelationID.Insert(18, "-")
        'CorrelationID = CorrelationID.Insert(23, "-")
        'CorrelationID = CorrelationID.Insert(36, "\")
        'CorrelationID = CorrelationID.Remove(37)
        'SeqCorrelationID = Now.Millisecond * Now.Second
        'ComplementInt = Sumator + SeqCorrelationID
        'CorrelationID = CorrelationID & ComplementInt.ToString("0000000")

        Return CorrelationID
    End Function

    Private Function Get_Token_Info(ByVal Search_Token As String, ByVal TokenData As String) As String
        Dim TKNid(20) As String
        Dim TKNdata(20) As String

        Try
            If TokenData.Length > 0 Then
                Dim x, y As Integer
                Do While TokenData.Length > 0
                    TKNid(x) = TokenData.Substring(0, 2)
                    TokenData = TokenData.Remove(0, 2)
                    y = TokenData.Substring(0, 3)
                    TokenData = TokenData.Remove(0, 3)
                    TKNdata(x) = TokenData.Substring(0, y)
                    TokenData = TokenData.Remove(0, y)
                    x += 1
                Loop
                Array.Resize(TKNid, x)
                Array.Resize(TKNdata, x)
            Else
                Return Space(107)
            End If
        Catch ex As Exception

        End Try

        Dim TKN_reply As String = Nothing
        If TKNid.Contains(Search_Token) Then
            TKN_reply = TKNdata(Array.IndexOf(TKNid, Search_Token))
        End If

        Return TKN_reply
    End Function

    Public Function Get_Token_Info(ByVal Search_Token As String, ByVal TKN_id() As String, ByVal TKN_val() As String) As String
        Dim TKN_reply As String = Nothing

        If TKN_id.Contains(Search_Token) Then
            TKN_reply = TKN_val(Array.IndexOf(TKN_id, Search_Token))
        End If

        Return TKN_reply
    End Function


End Class

