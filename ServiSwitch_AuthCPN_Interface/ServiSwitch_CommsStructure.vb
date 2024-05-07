Imports System.Runtime.InteropServices

Public Class Constanting_Definition

    Public Const from_WEBSERVICE As Byte = 2
    Public Const from_INTERFACES As Byte = 1

End Class


Public Class InfoFromSocket
    Public SMB_Times As String
    Public SMB_Package_Nbr As Int64
    Public SMB_TotLength As Int32
    Public SMB_ByteLength As Int32
    Public SMB_BytesMessage() As Byte
End Class

Public Class Loaded_Transaction_Data
    Public load_Tran_Code(0) As Int32
    Public Load_Tran_Type(0) As Int16
    Public Load_Tran_Message(0) As String
    Public Load_Tran_BitMap(0) As String
End Class

Public Class Main_Manager_Definition
    Public Main_Subsystem_Name(2) As String
    Public Main_Subsystem_Id(2) As Int32
    Public Main_Subsystem_Type(2) As Int32
    Public Main_Subsystem_Institution(2) As String
    Public Main_Subsystem_Task(2) As Int32
    Public Main_Subsystem_Instance(2) As Int32
    Public Main_Subsystem_Timeout(2) As Int32
    Public Main_Subsystem_Address(2) As String
    Public Main_Subsystem_Port(2) As String
    Public Main_Subsystem_SocketMode(2) As String
    Public Main_Subsystem_Queue_Request_messages(2) As String
    Public Main_Subsystem_Queue_Reply_messages(2) As String
    Public Main_Subsystem_Queue_Tcp_messages(2) As String
    Public Main_Subsystem_Queue_Saf_messages(2) As String
    Public Main_Subsystem_Queue_Cmd_messages(2) As String
    Public Main_Subsystem_Router(2) As String
    Public Main_Subsystem_Format(2) As Int16
    Public Main_Subsystem_Queue_Ack_messages(2) As String
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
        BUSS.SSM_Instance_Times = String.Empty
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

Public Class FixedStrucure

    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Unicode)> _
    Public Structure RequestInquiry
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=2)> Public MessageType As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=4)> Public SourceFiNbr As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=5)> Public SourceTrmlNbr As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=6)> Public SourceSeqNbr As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=4)> Public MessageSeqNbr As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=5)> Public TransactionCode As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=6)> Public SourceDate As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=6)> Public SourceTime As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=10)> Public SourceAbaNbr As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=4)> Public SourceBranchNbr As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=6)> Public SourceBussDate As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=1)> Public TerminalType As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=31)> Public SourceName As String
        '<MarshalAs(UnmanagedType.ByValTStr, SizeConst:=32)> Public PassthruField As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=1)> Public ForcedIndicator As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=1)> Public ReversalIndicator As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=18)> Public AccountNbr As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=17)> Public TransactionAmount As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=1)> Public TransactionSign As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=4)> Public AuthFiNbr As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=6)> Public HostBussDate As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=1)> Public StandinAuthType As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=1)> Public StandinAuthMethod As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=4)> Public StandinResultCode As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=1)> Public PinVerifyFlag As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=1)> Public TrackIIValidFlag As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=10)> Public TargetPhone As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=1)> Public SourceApplType As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=2)> Public FlagRevPD As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=3)> Public Filler As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=40)> Public TrackIIData As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=2)> Public CardApplCode As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=4)> Public OtherFi As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=2)> Public OtherAppl As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=18)> Public OtherAcct As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=1)> Public InternalInd As String
        '<MarshalAs(UnmanagedType.ByValTStr, SizeConst:=25)> Public InternalInd As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=107)> Public TrackIIIData As String

    End Structure

    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Unicode)> _
    Public Structure ReplyInquiry
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=2)> Public MessageType As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=4)> Public SourceFiNbr As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=5)> Public SourceTrmlNbr As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=6)> Public SourceSeqNbr As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=4)> Public MessageSeqNbr As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=5)> Public TransactionCode As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=4)> Public ResultCode As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=1)> Public AcctInfoFlag As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=1)> Public HostDataInfoFlag As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=18)> Public AccountNumber As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=17)> Public AvailableBalance As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=1)> Public SignAvailable As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=17)> Public CurrentBalance As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=1)> Public SignCurrent As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=2)> Public ApplCode As String
        '<MarshalAs(UnmanagedType.ByValTStr, SizeConst:=1)> Public TerminalType As String
        '<MarshalAs(UnmanagedType.ByValTStr, SizeConst:=31)> Public SourceName As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=32)> Public PassthruField As String
        '<MarshalAs(UnmanagedType.ByValTStr, SizeConst:=2)> Public TrfAcctType As String
        '<MarshalAs(UnmanagedType.ByValTStr, SizeConst:=18)> Public TrfAcctNumber As String
        '<MarshalAs(UnmanagedType.ByValTStr, SizeConst:=40)> Public TrfAcctName As String
        '<MarshalAs(UnmanagedType.ByValTStr, SizeConst:=8)> Public TrfMinPayment As String
        '<MarshalAs(UnmanagedType.ByValTStr, SizeConst:=8)> Public TrfTotPayment As String
        '<MarshalAs(UnmanagedType.ByValTStr, SizeConst:=8)> Public TrfLimitDate As String
        '<MarshalAs(UnmanagedType.ByValTStr, SizeConst:=10)> Public TrfTargetPhone As String
        '<MarshalAs(UnmanagedType.ByValTStr, SizeConst:=13)> Public TrfFiller As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=107)> Public TrackIIIData As String
    End Structure

    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Unicode)> _
    Public Structure RequestTransfer
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=2)> Public MessageType As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=4)> Public SourceFiNbr As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=5)> Public SourceTrmlNbr As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=6)> Public SourceSeqNbr As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=4)> Public MessageSeqNbr As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=5)> Public TransactionCode As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=6)> Public SourceDate As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=6)> Public SourceTime As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=10)> Public SourceAbaNbr As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=4)> Public SourceBranchNbr As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=6)> Public SourceBussDate As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=1)> Public TerminalType As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=31)> Public SourceName As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=1)> Public ForcedIndicator As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=1)> Public ReversalIndicator As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=18)> Public AccountNbr As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=17)> Public TransactionAmount As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=1)> Public TransactionSign As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=4)> Public AuthFiNbr As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=6)> Public HostBussDate As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=1)> Public StandinAuthType As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=1)> Public StandinAuthMethod As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=4)> Public StandinResultCode As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=1)> Public PinVerifyFlag As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=1)> Public TrackIIValidFlag As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=10)> Public TargetPhone As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=1)> Public SourceApplType As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=2)> Public FlagRevPD As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=3)> Public Filler As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=40)> Public TrackIIData As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=2)> Public CardApplCode As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=4)> Public OtherFi As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=2)> Public OtherAppl As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=18)> Public OtherAcct As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=1)> Public InternalInd As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=35)> Public TargetName As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=10)> Public TargetObservation As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=10)> Public SourcePhone As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=3)> Public CountryCode As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=10)> Public TargetCity As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=39)> Public TargetAddress As String
    End Structure

    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Unicode)> _
    Public Structure ReplyTransfer
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=2)> Public MessageType As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=4)> Public SourceFiNbr As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=5)> Public SourceTrmlNbr As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=6)> Public SourceSeqNbr As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=4)> Public MessageSeqNbr As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=5)> Public TransactionCode As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=4)> Public ResultCode As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=1)> Public AcctInfoFlag As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=1)> Public HostDataInfoFlag As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=18)> Public AccountNumber1 As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=17)> Public AvailableBalance1 As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=1)> Public SignAvailable1 As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=17)> Public CurrentBalance1 As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=1)> Public SignCurrent1 As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=2)> Public ApplCode As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=1)> Public TerminalType As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=31)> Public SourceName As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=18)> Public AccountNumber2 As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=17)> Public AvailableBalance2 As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=1)> Public SignAvailable2 As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=17)> Public CurrentBalance2 As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=1)> Public SignCurrent2 As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=2)> Public ApplCode2 As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=107)> Public TrackIIIData As String
    End Structure

    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Unicode)> _
    Public Structure RequestReversal
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=2)> Public MessageType As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=4)> Public SourceFiNbr As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=5)> Public SourceTrmlNbr As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=6)> Public SourceSeqNbr As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=4)> Public MessageSeqNbr As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=5)> Public TransactionCode As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=6)> Public SourceDate As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=6)> Public SourceTime As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=10)> Public SourceAbaNbr As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=4)> Public SourceBranchNbr As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=6)> Public SourceBussDate As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=1)> Public TerminalType As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=31)> Public SourceName As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=1)> Public ForcedIndicator As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=1)> Public ReversalIndicator As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=18)> Public AccountNbr As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=17)> Public TransactionAmount As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=1)> Public TransactionSign As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=4)> Public AuthFiNbr As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=6)> Public HostBussDate As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=1)> Public StandinAuthType As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=1)> Public StandinAuthMethod As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=4)> Public StandinResultCode As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=1)> Public PinVerifyFlag As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=1)> Public TrackIIValidFlag As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=10)> Public TargetPhone As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=1)> Public SourceApplType As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=2)> Public FlagRevPD As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=3)> Public Filler As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=40)> Public TrackIIData As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=2)> Public CardApplCode As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=17)> Public ReversalAmount As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=1)> Public ReversalSign As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=107)> Public TrackIIIData As String
    End Structure

    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Unicode)> _
    Public Structure ReplyReversal
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=2)> Public MessageType As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=4)> Public SourceFiNbr As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=5)> Public SourceTrmlNbr As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=6)> Public SourceSeqNbr As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=4)> Public MessageSeqNbr As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=5)> Public TransactionCode As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=4)> Public ResultCode As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=1)> Public AcctInfoFlag As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=1)> Public HostDataInfoFlag As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=18)> Public AccountNumber1 As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=17)> Public AvailableBalance1 As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=1)> Public SignAvailable1 As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=17)> Public CurrentBalance1 As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=1)> Public SignCurrent1 As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=2)> Public ApplCode As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=1)> Public TerminalType As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=31)> Public SourceName As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=18)> Public AccountNumber2 As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=17)> Public AvailableBalance2 As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=1)> Public SignAvailable2 As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=17)> Public CurrentBalance2 As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=1)> Public SignCurrent2 As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=2)> Public ApplCode2 As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=107)> Public TrackIIIData As String
    End Structure

End Class
