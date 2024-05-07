Imports System.Messaging

Public Class SWTM_Auxiliar
    Private MMD As New Main_Manager_Definition

    Public Const PrivateQueue As String = ".\Private$\"

    Public Function Fill_Structure_Data(ByVal ParametersDB As List(Of String)) As Byte
        Dim ErrorCode As Byte = 1

        Try
            MMD.Main_Subsystem_Name = ParametersDB(0)
            MMD.Main_Subsystem_Id = ParametersDB(1)
            MMD.Main_Subsystem_Type = ParametersDB(2)
            MMD.Main_Subsystem_Institution = ParametersDB(3)
            MMD.Main_Subsystem_Task = ParametersDB(4)
            MMD.Main_Subsystem_Instance = ParametersDB(5)
            MMD.Main_Subsystem_Timeout = ParametersDB(6)
            MMD.Main_Subsystem_Address = ParametersDB(7)
            MMD.Main_Subsystem_Port = ParametersDB(8)
            MMD.Main_Subsystem_SocketMode = ParametersDB(9)
            MMD.Main_Subsystem_Queue_request_messages = ParametersDB(10)

            If Verify_Queue_Resource(PrivateQueue & MMD.Main_Subsystem_Queue_request_messages) <> 0 Then
                Return ErrorCode
            End If
            MMD.Main_Subsystem_Queue_reply_messages = ParametersDB(11)
            If Verify_Queue_Resource(PrivateQueue & MMD.Main_Subsystem_Queue_reply_messages) <> 0 Then
                Return ErrorCode
            End If
            MMD.Main_Subsystem_Queue_tcpip_messages = ParametersDB(12)
            If Verify_Queue_Resource(PrivateQueue & MMD.Main_Subsystem_Queue_tcpip_messages) <> 0 Then
                Return ErrorCode
            End If
            MMD.Main_Subsystem_Queue_saf_messages = ParametersDB(13)
            If Verify_Queue_Resource(PrivateQueue & MMD.Main_Subsystem_Queue_saf_messages) <> 0 Then
                Return ErrorCode
            End If
            MMD.Main_Subsystem_Queue_cmd_messages = ParametersDB(14)
            If Verify_Queue_Resource(PrivateQueue & MMD.Main_Subsystem_Queue_cmd_messages) <> 0 Then
                Return ErrorCode
            End If
            MMD.Main_Subsystem_RouterApply = ParametersDB(15)
            MMD.Main_Subsystem_MessageFormat = ParametersDB(16)

            MMD.Main_Subsystem_Queue_ack_messages = ParametersDB(17)
            If Verify_Queue_Resource(PrivateQueue & MMD.Main_Subsystem_Queue_ack_messages) <> 0 Then
                Return ErrorCode
            End If
            ErrorCode = 0
        Catch ex As Exception
            ErrorCode = 1
        End Try

        Return ErrorCode

    End Function

    Public Function Verify_Queue_Resource(ByVal QueueName As String) As Byte
        Dim ErrorCode As Byte = FUNCTION_ERROR
        Try
            Dim DFXqueue As MessageQueue = New MessageQueue(QueueName)
            If Not MessageQueue.Exists(QueueName) Then
                MessageQueue.Create(QueueName)
                Try
                    DFXqueue.SetPermissions("everyone", MessageQueueAccessRights.FullControl, AccessControlEntryType.Allow)
                Catch ex As Exception
                    DFXqueue.SetPermissions("todos", MessageQueueAccessRights.FullControl, AccessControlEntryType.Allow)
                End Try
                DFXqueue.SetPermissions("IIS_IUSRS", MessageQueueAccessRights.FullControl, AccessControlEntryType.Allow)
                DFXqueue.SetPermissions("IUSR", MessageQueueAccessRights.FullControl, AccessControlEntryType.Allow)
                ErrorCode = SUCCESSFUL
            Else
                ErrorCode = SUCCESSFUL
            End If
            'End If
        Catch ex As Exception
            MessageBox.Show("Exception on managing Queue:" & QueueName & "->" & ex.Message, "VerifyQueue", MessageBoxButtons.OK, MessageBoxIcon.Error)
            ErrorCode = 1
        End Try
        Return ErrorCode

    End Function

    Public Function Killing_ServiSwitch() As Byte
        Dim ErrorCode As Byte = 1
        Dim PathProcessName As String = String.Empty
        Dim pMy As Process = Process.GetCurrentProcess
        Dim IDp As Int32 = pMy.Id
        pMy.Dispose()

        Try
            Dim ServiProcess As List(Of Process) = (From p As Process In Process.GetProcesses Where p.ProcessName.ToUpper Like "ServiSwitch*".ToUpper).ToList
            For Each p As Process In ServiProcess
                If Not p.MainModule.FileName.Contains("vshost") Then
                    If (p.Id <> IDp) Then
                        p.Kill()
                    End If
                End If
            Next
        Catch ex As Exception
            ErrorCode = False
        End Try

        Return ErrorCode
    End Function

End Class
