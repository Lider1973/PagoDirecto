
Public Class Main_Manager_Definition

    Public Main_Subsystem_Name As String
    Public Main_Subsystem_Id As Int32
    Public Main_Subsystem_Type As Int32
    Public Main_Subsystem_Institution As String
    Public Main_Subsystem_Task As Int32
    Public Main_Subsystem_Instance As Int32
    Public Main_Subsystem_Timeout As Int32
    Public Main_Subsystem_Address As String
    Public Main_Subsystem_Port As String
    Public Main_Subsystem_SocketMode As String
    Public Main_Subsystem_Queue_request_messages As String
    Public Main_Subsystem_Queue_reply_messages As String
    Public Main_Subsystem_Queue_tcpip_messages As String
    Public Main_Subsystem_Queue_saf_messages As String
    Public Main_Subsystem_Queue_cmd_messages As String
    Public Main_Subsystem_RouterApply As String
    Public Main_Subsystem_MessageFormat As String
    Public Main_Subsystem_Queue_ack_messages As String

End Class

Public Structure InfoCommands
    Public ICM_CommandBuffer As String
End Structure

Public Class InfoFromSocket
    Public SMB_Times As String
    Public SMB_Package_Nbr As Int64
    Public SMB_TotLength As Int32
    Public SMB_ByteLength As Int32
    Public SMB_BytesMessage() As Byte
End Class


