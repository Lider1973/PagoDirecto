Imports System.Runtime.InteropServices
Public Class Constanting_Definition

    Public Const req_ID_Terminal As Byte = 0
    Public Const req_ID_Sequence As Byte = 1
    Public Const req_ID_Transaction As Byte = 2
    Public Const req_ID_DateTime As Byte = 3
    Public Const req_ID_Bank_id As Byte = 4
    Public Const req_ID_Channel As Byte = 5
    Public Const req_ID_LocalAcctName As Byte = 6
    Public Const req_ID_TargetAcctName As Byte = 7
    Public Const req_ID_TranAmount As Byte = 8
    Public Const req_ID_LocalAcctNumber As Byte = 9
    Public Const req_ID_TargetAcctNumber As Byte = 10
    Public Const req_ID_TargetAcctType As Byte = 11
    Public Const req_ID_ServiceCode As Byte = 12
    Public Const req_ID_ReverseReason As Byte = 13
    Public Const req_ID_BranchCode As Byte = 14
    Public Const req_ID_OpratorCode As Byte = 15
    Public Const req_ID_SourceAcctType As Byte = 16
    Public Const req_ID_ContactNbr As Byte = 17
    Public Const req_ID_Reference As Byte = 18
    Public Const req_ID_SourceIdentificaction As Byte = 19
    Public Const req_ID_TargetIdentificaction As Byte = 20

    Public Const rep_ID_Terminal As Byte = 0
    Public Const rep_ID_Response As Byte = 1
    Public Const rep_ID_Transaction As Byte = 2
    Public Const rep_ID_DateTime As Byte = 3
    Public Const rep_ID_Sequence As Byte = 4
    Public Const rep_ID_Switch_Sequence As Byte = 5
    Public Const rep_ID_TargetAccountType As Byte = 6
    Public Const rep_ID_TargetAccountNumber As Byte = 7
    Public Const rep_ID_TargetAccountName As Byte = 8
    'Public Const rep_ID_LocalAcctNumber As Byte = 9
    Public Const rep_ID_TargetContact As Byte = 9
    Public Const rep_ID_SourceReference As Byte = 10
    Public Const rep_ID_TargetIdentification As Byte = 11
    Public Const rep_ID_ccMinimunAmt As Byte = 12
    Public Const rep_ID_ccTotalAmt As Byte = 13
    Public Const rep_ID_ccLimitDate As Byte = 14
    Public Const rep_ID_ccContact As Byte = 15

    Public Const from_WEBSERVICE As Byte = 2
    Public Const from_INTERFACES As Byte = 1
    Public Const ISO8583_format As Byte = 101
    Public Const HOST2_format As Byte = 102
    Public Const OWNER_format As Byte = 103

    Public Const ERROR_Xml_string As Integer = 1001
    Public Const ERROR_Buss_Asign As Integer = 1002
    Public Const ERROR_Sending As Integer = 1003
    Public Const ERROR_On_Reply As Integer = 1004
    Public Const ERROR_Timeut As Integer = 1005
    Public Const ERROR_Local_Disconnect As Integer = 1006
    Public Const ERROR_Transaction_Undefined As Integer = 1007
    Public Const ERROR_Not_Route As Integer = 1008
    Public Const ERROR_Inactive_Proces As Integer = 1009
    Public Const ERROR_Undefined_Process As Integer = 1010
    Public Const ERROR_Unknow As Integer = 9999


End Class

Public Class SharedStructureMessage
    Public SSM_Adq_Source_Name As String
    Public SSM_Adq_Queue_Request_Name As String
    Public SSM_Adq_Queue_Reply_Name As String
    '**************************************************
    Public SSM_Auth_Source_Name As String
    Public SSM_Auth_Queue_Request_Name As String
    Public SSM_Auth_Queue_Reply_Name As String
    '**************************************************
    Public SSM_Rout_Source_Name As String
    Public SSM_Rout_Queue_Request_Name As String
    Public SSM_Rout_Queue_Reply_Name As String
    '**************************************************
    Public SSM_Transaction_Indicator As Char
    Public SSM_Queue_Message_ID As String
    Public SSM_Instance_Times As String
    Public SSM_Communication_ID As Int64
    Public SSM_Auth_Module_ID As Int16
    Public SSM_Message_Format As Byte
    Public SSM_Common_Data As New CommonRequestFields
End Class


Public Class CommonRequestFields
    ' --- ACOUNTS & REFERENCES ----
    Public CRF_Reference As String
    Public CRF_Account_Number As Int64
    Public CRF_Primary_Account As Int64
    Public CRF_Secondary_Account As Int64

    ' ----  AMOUNTS ----
    Public CRF_Transaction_Amount As Decimal
    Public CRF_Debit_Amount As Decimal
    Public CRF_Credit_Amount As Decimal
    Public CRF_Reversal_Credit_Amount As Decimal
    Public CRF_Reversal_Debit_Amount As Decimal
    Public CRF_Commision_Amount As Decimal
    Public CRF_Supercargo_Amount As Decimal
    Public CRF_Taxe_IVA As Decimal
    Public CRF_Taxe_ISA As Decimal
    Public CRF_Taxe_ISD As Decimal
    Public CRF_Taxe_RTE As Decimal
    Public CRF_Other_Taxe As Decimal
    Public CRF_Other_Amount As Decimal

    ' ---- BALANCES ----
    Public CRF_Ledger_Balance As Decimal
    Public CRF_Available_Balance As Decimal

    ' ---- DATES & TIMES ----
    Public CRF_Business_Date As Date
    Public CRF_Switch_Date_Time As DateTime
    Public CRF_Adquirer_Date_Time As DateTime
    Public CRF_Device_Date_Time As DateTime
    Public CRF_Issuer_Date_Time As DateTime
    Public CRF_Limit_Date As Date

    ' SEQUENCES
    Public CRF_Switch_Sequence As Int32
    Public CRF_Adquirer_Sequence As Int32
    Public CRF_Device_Sequence As Int32

    ' LOCATIONS
    Public CRF_Adquirer_Region As String
    Public CRF_Adquirer_County As String
    Public CRF_Adquirer_city As String

    ' ---- IDS ----
    Public CRF_Issuer_Institution_ID As Int64
    Public CRF_Adquirer_Institution_ID As Int64
    Public CRF_Terminal_ID As String
    Public CRF_Operator_ID As String
    Public CRF_Supervisor_ID As String
    Public CRF_Branch_ID As String
    Public CRF_Terminal_Location As String
    Public CRF_Channel_Id As Int16

    ' ---- INDCATORS ----
    Public CRF_Reversal_Indicator As Byte
    Public CRF_Service_Indicator As String

    ' ---- CODES & NUMBERS ----
    Public CRF_Transaction_Code As Int32
    Public CRF_Message_Code As Int32
    Public CRF_Authorization_Code As Int64
    Public CRF_Response_Code As String               'Se consideran Codigos de respuesta Alpha
    Public CRF_Currency_Code As Int16
    Public CRF_Issuer_Institution_Number As Int64
    Public CRF_Adquirer_Institution_Number As Int64

    ' ---- COMPLEMENTS ----
    Public CRF_Names As String
    Public CRF_PhoneNumber As String
    Public CRF_Card_Info As String
    Public CRF_Original_Data As String
    Public CRF_Buffer_Data As String
    Public CRF_Security_Data As String
    Public CRF_Token_Data As String
    Public CRF_User_Data As String
    Public CRF_Temporal As String
    Public CRF_Bytes_Data As Byte()
End Class

Public Class Messages

    Public Sub Initialize_Internal_Buss(ByRef BUSS As SharedStructureMessage)

        BUSS.SSM_Transaction_Indicator = String.Empty

        BUSS.SSM_Adq_Source_Name = String.Empty
        BUSS.SSM_Adq_Queue_Request_Name = String.Empty
        BUSS.SSM_Adq_Queue_Reply_Name = String.Empty
        '**************************************************
        BUSS.SSM_Auth_Source_Name = String.Empty
        BUSS.SSM_Auth_Queue_Request_Name = String.Empty
        BUSS.SSM_Auth_Queue_Reply_Name = String.Empty
        '**************************************************
        BUSS.SSM_Rout_Source_Name = String.Empty
        BUSS.SSM_Rout_Queue_Request_Name = String.Empty
        BUSS.SSM_Rout_Queue_Reply_Name = String.Empty
        '**************************************************
        BUSS.SSM_Queue_Message_ID = String.Empty
        'BUSS.SSM_Instance_Times = String.Empty
        BUSS.SSM_Instance_Times += "T1_" & GetDateTime() & "|"
        BUSS.SSM_Communication_ID = 0

        BUSS.SSM_Common_Data.CRF_Reference = String.Empty
        BUSS.SSM_Common_Data.CRF_Account_Number = 0
        BUSS.SSM_Common_Data.CRF_Primary_Account = 0
        BUSS.SSM_Common_Data.CRF_Secondary_Account = 0

        BUSS.SSM_Common_Data.CRF_Adquirer_Region = String.Empty
        BUSS.SSM_Common_Data.CRF_Adquirer_County = String.Empty
        BUSS.SSM_Common_Data.CRF_Adquirer_city = String.Empty

        ' ----  AMOUNTS ----
        BUSS.SSM_Common_Data.CRF_Transaction_Amount = 0
        BUSS.SSM_Common_Data.CRF_Debit_Amount = 0
        BUSS.SSM_Common_Data.CRF_Credit_Amount = 0
        BUSS.SSM_Common_Data.CRF_Reversal_Credit_Amount = 0
        BUSS.SSM_Common_Data.CRF_Reversal_Debit_Amount = 0
        BUSS.SSM_Common_Data.CRF_Commision_Amount = 0
        BUSS.SSM_Common_Data.CRF_Supercargo_Amount = 0
        BUSS.SSM_Common_Data.CRF_Taxe_IVA = 0
        BUSS.SSM_Common_Data.CRF_Taxe_ISA = 0
        BUSS.SSM_Common_Data.CRF_Taxe_ISD = 0
        BUSS.SSM_Common_Data.CRF_Taxe_RTE = 0
        BUSS.SSM_Common_Data.CRF_Other_Amount = 0

        ' ---- BALANCES ----
        BUSS.SSM_Common_Data.CRF_Ledger_Balance = 0
        BUSS.SSM_Common_Data.CRF_Available_Balance = 0

        ' ---- DATES & TIMES ----
        BUSS.SSM_Common_Data.CRF_Business_Date = DateTime.Now
        BUSS.SSM_Common_Data.CRF_Switch_Date_Time = DateTime.Now
        BUSS.SSM_Common_Data.CRF_Adquirer_Date_Time = DateTime.Now
        BUSS.SSM_Common_Data.CRF_Device_Date_Time = DateTime.Now

        ' SEQUENCES
        BUSS.SSM_Common_Data.CRF_Switch_Sequence = 0
        BUSS.SSM_Common_Data.CRF_Adquirer_Sequence = 0

        ' ---- IDS ----
        BUSS.SSM_Common_Data.CRF_Issuer_Institution_ID = 0
        BUSS.SSM_Common_Data.CRF_Adquirer_Institution_ID = 0
        BUSS.SSM_Common_Data.CRF_Terminal_ID = String.Empty
        BUSS.SSM_Common_Data.CRF_Operator_ID = String.Empty
        BUSS.SSM_Common_Data.CRF_Supervisor_ID = String.Empty
        BUSS.SSM_Common_Data.CRF_Branch_ID = String.Empty
        BUSS.SSM_Common_Data.CRF_Terminal_Location = String.Empty
        BUSS.SSM_Common_Data.CRF_Channel_Id = 0

        ' ---- INDCATORS ----
        BUSS.SSM_Common_Data.CRF_Reversal_Indicator = 0
        BUSS.SSM_Common_Data.CRF_Service_Indicator = String.Empty

        ' ---- CODES & NUMBERS ----
        BUSS.SSM_Common_Data.CRF_Transaction_Code = 0
        BUSS.SSM_Common_Data.CRF_Authorization_Code = 0
        BUSS.SSM_Common_Data.CRF_Response_Code = String.Empty
        BUSS.SSM_Common_Data.CRF_Currency_Code = 0
        BUSS.SSM_Common_Data.CRF_Issuer_Institution_Number = 0
        BUSS.SSM_Common_Data.CRF_Adquirer_Institution_Number = 0

        ' ---- COMPLEMENTS ----
        BUSS.SSM_Common_Data.CRF_Card_Info = String.Empty
        BUSS.SSM_Common_Data.CRF_Original_Data = String.Empty
        BUSS.SSM_Common_Data.CRF_Buffer_Data = String.Empty
        BUSS.SSM_Common_Data.CRF_Security_Data = String.Empty
        BUSS.SSM_Common_Data.CRF_Token_Data = String.Empty
        BUSS.SSM_Common_Data.CRF_User_Data = String.Empty
        BUSS.SSM_Common_Data.CRF_Bytes_Data = Nothing

    End Sub

End Class

Public Structure InfoCommands
    Public ICM_CommandBuffer As String
End Structure



