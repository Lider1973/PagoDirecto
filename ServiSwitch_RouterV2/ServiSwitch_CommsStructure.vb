Public Class Constanting_Definition

    Public Const req_ID_Terminal As Byte = 0
    Public Const req_ID_Sequence As Byte = 1
    Public Const req_ID_Transaction As Byte = 2
    Public Const req_ID_DateTime As Byte = 3
    Public Const req_ID_Bank_id As Byte = 4
    Public Const req_ID_Channel As Byte = 5
    Public Const req_ID_LocalAcctName As Byte = 6
    Public Const req_ID_TranAmount As Byte = 7
    Public Const req_ID_LocalAcctNumber As Byte = 8
    Public Const req_ID_TargetAcctNumber As Byte = 9
    Public Const req_ID_TargetAcctType As Byte = 10

    Public Const rep_ID_Terminal As Byte = 0
    Public Const rep_ID_Response As Byte = 1
    Public Const rep_ID_Transaction As Byte = 2
    Public Const rep_ID_DateTime As Byte = 3
    Public Const rep_ID_Sequence As Byte = 4
    Public Const rep_ID_TargetAccountType As Byte = 5
    Public Const rep_ID_TargetAccountNumber As Byte = 6
    Public Const rep_ID_TargetAccountName As Byte = 7
    Public Const rep_ID_LocalAcctNumber As Byte = 8
    Public Const rep_ID_ccMinimunAmt As Byte = 9
    Public Const rep_ID_ccTotalAmt As Byte = 10
    Public Const rep_ID_ccLimitDate As Byte = 11
    Public Const rep_ID_ccContact As Byte = 12

    Public Const from_WEBSERVICE As Byte = 2
    Public Const from_INTERFACES As Byte = 1
    Public Const ISO8583_format As Byte = 101
    Public Const HOST2_format As Byte = 102
    Public Const OWNER_format As Byte = 103

    Public Const NOT_TRANSACTION_DEFINED As Byte = 201
    Public Const NOT_ROUTE_DEFINED As Byte = 202
    Public Const NOT_PROCESS_ACTIVE As Byte = 203
    Public Const NOT_PROCESS_DEFINED As Byte = 204
    Public Const DATE_TIME_INCONSISTENCE As Byte = 205
    Public Const INSTITUTION_IS_DOWN As Byte = 206


End Class


Public Class Loaded_Transaction_Data
    Public load_Tran_Code(0) As Int32
    Public Load_Tran_Type(0) As Int16
    Public Load_Tran_Message(0) As String
    Public Load_Tran_BitMap(0) As String
End Class

Public Class Loaded_Routing_Data
    Public load_Routing_Code(0) As String
    Public Load_Routing_Target(0) As Int16
    Public Load_Routing_Name(0) As String
End Class

Public Class Loaded_Institution_Data
    Public load_Authorizer_Code(0) As Int16
    Public Load_Institution_Name(0) As String
End Class

Public Class Main_Manager_Definition
    Public Main_Subsystem_Name(0) As String
    Public Main_Subsystem_Id(0) As Int32
    Public Main_Subsystem_Type(0) As Int32
    Public Main_Subsystem_Institution(0) As String
    Public Main_Subsystem_Task(0) As Int32
    Public Main_Subsystem_Instance(0) As Int32
    Public Main_Subsystem_Timeout(0) As Int32
    Public Main_Subsystem_Address(0) As String
    Public Main_Subsystem_Port(0) As String
    Public Main_Subsystem_SocketMode(0) As String
    Public Main_Subsystem_Queue_Request_messages(0) As String
    Public Main_Subsystem_Queue_Reply_messages(0) As String
    Public Main_Subsystem_Queue_Tcp_messages(0) As String
    Public Main_Subsystem_Queue_Saf_messages(0) As String
    Public Main_Subsystem_Queue_Cmd_messages(0) As String
    Public Main_Subsystem_Router(0) As String
    Public Main_Subsystem_Format(0) As Int16
    Public Main_Subsystem_Queue_Ack_messages(0) As String
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
    Public CRF_Account_Number As String
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


Public Structure InfoCommands
    Public ICM_CommandBuffer As String
End Structure



