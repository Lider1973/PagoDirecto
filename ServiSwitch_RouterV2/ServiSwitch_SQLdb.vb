Imports System.Data.SqlClient

Module ServiSwitch_SQLdb

    Dim Security As New EncodeDecodeFunctions
    Dim MyStringConnection As String


    Public Function Init_Security_Resources() As Byte
        Dim ErrorCode As Byte = FUNCTION_ERROR
        Try
            Security.Load_Config_Keys()
            SetGetStringConnection = Security.SetgetStringConnection()
            ErrorCode = SUCCESSFUL
        Catch ex As Exception
            Show_Message_Console(MyName & " Exception trying to load security resources ", COLOR_OWNER1, COLOR_RED, 0, TRACE_LOW, 1)
        End Try
        Return ErrorCode
    End Function

    Public Property SetGetStringConnection() As String
        Get
            Return MyStringConnection
        End Get
        Set(ByVal value As String)
            MyStringConnection = value
        End Set
    End Property

    Public Function Create_SQL_Connection(ByRef SQLconn As SqlConnection) As Byte
        Dim ErrorCode As Byte = 1

        SQLconn = New SqlConnection
        SQLconn.ConnectionString = MyStringConnection
        'SQLconn.ConnectionString = "Data Source=BRGPRUEBAS\sqlexpress,1973;Initial Catalog=DESARROLLO;User ID=Desarrollo;Password=Desa2015"
        ErrorCode = 0
        Try
            SQLconn.Open()
            'Console.Write(" dbo:" & Now.ToLongTimeString & "." & Now.Millisecond)
            Return 0
        Catch ex As Exception
            Return 1
        End Try

    End Function

    Public Function Close_SQL_Connection(ByVal SQLconn As SqlConnection) As Byte
        Dim ErrorCode As Byte = 1

        Try
            SQLconn.Close()
            'Console.Write(" dbc:" & Now.ToLongTimeString & "." & Now.Millisecond)
            Return 0
        Catch ex As Exception
            Return 1
        End Try

    End Function

    Public Function GetInfoNode(ByVal ModuleName As String, ByVal ParamsTable As List(Of String)) As Byte
        Dim ErrorCode As Byte = FUNCTION_ERROR

        Dim myConnection As New SqlConnection
        If Create_SQL_Connection(myConnection) <> SUCCESSFUL Then
            Return PROCESS_ERROR
        End If
        Dim myReader As SqlDataReader

        Try
            Dim myCommand As New SqlCommand("select * from sabd_main_definition where sabd_module_name ='" & ModuleName & "'", myConnection)
            myReader = myCommand.ExecuteReader(CommandBehavior.SingleRow)
            If myReader.HasRows Then
                While myReader.Read()
                    For count As Integer = 0 To (myReader.FieldCount - 1)
                        ParamsTable.Add(myReader.GetValue(count))
                    Next
                End While
            Else
                ErrorCode = 1
            End If
            If Not IsNothing(myReader) Then
                myReader.Close()
            End If
            ErrorCode = SUCCESSFUL
        Catch ex As SqlException
            ErrorCode = FUNCTION_ERROR
        End Try

        Close_SQL_Connection(myConnection)

        GC.Collect()

        Return ErrorCode
    End Function

    Public Function Get_Table_Definition(ByRef ParamsTable As List(Of String), ByVal TableName As String, ByVal Fields As String) As Byte
        Dim ErrorCode As Byte = 1
        Dim TempRec As String = String.Empty

        Dim myConnection As New SqlConnection
        If Create_SQL_Connection(myConnection) <> SUCCESSFUL Then
            Return PROCESS_ERROR
        End If
        Dim myReader As SqlDataReader

        Try
            Dim myCommand As New SqlCommand("select " & Fields & " from " & TableName, myConnection)
            myReader = myCommand.ExecuteReader(CommandBehavior.Default)
            If myReader.HasRows Then
                While myReader.Read()
                    TempRec = Nothing
                    For count As Integer = 0 To (myReader.FieldCount - 1)
                        TempRec += myReader.GetValue(count) & "#"
                    Next
                    ParamsTable.Add(TempRec)
                End While
            Else
                ErrorCode = 1
            End If

            If Not IsNothing(myReader) Then
                myReader.Close()
            End If

            ErrorCode = 0
        Catch ex As SqlException
            ErrorCode = 1
        End Try

        Close_SQL_Connection(myConnection)
        GC.Collect()

        Return ErrorCode
    End Function

    Public Function GetTerminalDefinition(ByVal ModuleName As String, ByRef ParamsTable As List(Of String)) As Byte
        Dim ErrorCode As Byte = 1
        Dim TempRec As String = String.Empty

        Dim myConnection As New SqlConnection
        If Create_SQL_Connection(myConnection) <> SUCCESSFUL Then
            Return PROCESS_ERROR
        End If
        Dim myReader As SqlDataReader

        Try
            Dim myCommand As New SqlCommand("select sabd_terminal_code, sabd_terminal_port from sabd_terminal_definition where sabd_terminal_concenter='" & ModuleName & "'", myConnection)
            myReader = myCommand.ExecuteReader(CommandBehavior.Default)
            If myReader.HasRows Then
                While myReader.Read()
                    TempRec = Nothing
                    For count As Integer = 0 To (myReader.FieldCount - 1)
                        TempRec += myReader.GetValue(count) & "#"
                        'ParamsTable.Add(myReader.GetValue(count))
                    Next
                    ParamsTable.Add(TempRec)
                End While
            Else
                ErrorCode = 1
            End If

            If Not IsNothing(myReader) Then
                myReader.Close()
            End If

            ErrorCode = 0
        Catch ex As SqlException
            ErrorCode = 1
        End Try

        Close_SQL_Connection(myConnection)
        GC.Collect()

        Return ErrorCode
    End Function

    Public Function GetSupervisorDefinition(ByVal ModuleName As String, ByRef ParamsTable As List(Of String)) As Byte
        Dim ErrorCode As Byte = 1
        Dim TempRec As String = String.Empty
        Dim myConnection As New SqlConnection
        If Create_SQL_Connection(myConnection) <> SUCCESSFUL Then
            Return PROCESS_ERROR
        End If
        Dim myReader As SqlDataReader

        Try
            Dim myCommand As New SqlCommand("select sabd_supervisor_code, sabd_supervisor_port from sabd_supervisor_definition where sabd_supervisor_concenter='" & ModuleName & "'", myConnection)
            myReader = myCommand.ExecuteReader(CommandBehavior.Default)
            If myReader.HasRows Then
                While myReader.Read()
                    TempRec = Nothing
                    For count As Integer = 0 To (myReader.FieldCount - 1)
                        TempRec += myReader.GetValue(count) & "#"
                        'ParamsTable.Add(myReader.GetValue(count))
                    Next
                    ParamsTable.Add(TempRec)
                End While
            Else
                ErrorCode = 1
            End If

            If Not IsNothing(myReader) Then
                myReader.Close()
            End If

            ErrorCode = 0
        Catch ex As SqlException
            ErrorCode = 1
        End Try

        Close_SQL_Connection(myConnection)
        GC.Collect()

        Return ErrorCode
    End Function

    Public Function GetInstitutionDefinition(ByRef ParamsTable As List(Of String)) As Byte
        Dim ErrorCode As Byte = 1
        Dim TempRec As String = String.Empty
        Dim myConnection As New SqlConnection
        If Create_SQL_Connection(myConnection) <> SUCCESSFUL Then
            Return PROCESS_ERROR
        End If
        Dim myReader As SqlDataReader

        Try
            Dim myCommand As New SqlCommand("select sabd_codigo_autorizador, sabd_nombre from sabd_institution_definition", myConnection)
            myReader = myCommand.ExecuteReader(CommandBehavior.Default)
            If myReader.HasRows Then
                While myReader.Read()
                    TempRec = Nothing
                    For count As Integer = 0 To (myReader.FieldCount - 1)
                        TempRec += myReader.GetValue(count) & "#"
                    Next
                    ParamsTable.Add(TempRec)
                End While
            Else
                ErrorCode = 1
            End If

            If Not IsNothing(myReader) Then
                myReader.Close()
            End If

            ErrorCode = 0
        Catch ex As SqlException
            ErrorCode = 1
        End Try

        Close_SQL_Connection(myConnection)
        GC.Collect()

        Return ErrorCode
    End Function

    Public Function ExistSubsystemName(ByVal SubSystemName As String) As Boolean
        Dim Exist As Boolean = False
        Dim TempValue As String = String.Empty

        Dim myConnection As New SqlConnection
        If Create_SQL_Connection(myConnection) <> SUCCESSFUL Then
            Return PROCESS_ERROR
        End If
        Dim myReader As SqlDataReader
        Try
            Dim myCommand As New SqlCommand("select sabd_module_name from sabd_main_definition where sabd_module_name ='" & SubSystemName & "'", myConnection)
            myReader = myCommand.ExecuteReader(CommandBehavior.SingleRow)
            If myReader.HasRows Then
                While myReader.Read()
                    TempValue = myReader.GetValue(0)
                End While
            Else
                Exist = False
            End If
            If SubSystemName = TempValue Then
                Exist = True
            Else
                Exist = False
            End If

            If Not IsNothing(myReader) Then
                myReader.Close()
            End If

        Catch ex As SqlException
            Exist = False
        End Try

        Close_SQL_Connection(myConnection)
        GC.Collect()

        Return Exist
    End Function


    Public Function GetClientName(ByVal PortId As Int16, ByVal SOCKET_MODE As Byte) As String
        Dim ClientName As String = "XXXXXXXXXX"

        Dim myConnection As New SqlConnection
        If Create_SQL_Connection(myConnection) <> SUCCESSFUL Then
            Return PROCESS_ERROR
        End If
        Dim myReader As SqlDataReader
        Try
            Dim myCommand As New SqlCommand("select sabd_module_name  from sabd_main_definition where sabd_port_number = " & PortId & " and sabd_connection_mode = " & SOCKET_MODE, myConnection)
            myReader = myCommand.ExecuteReader(CommandBehavior.SingleRow)
            If myReader.HasRows Then
                While myReader.Read()
                    ClientName = myReader.GetValue(0)
                End While
            End If
        Catch ex As SqlException
            Return ClientName
        End Try

        Close_SQL_Connection(myConnection)
        If Not IsNothing(myReader) Then
            myReader.Close()
        End If
        GC.Collect()

        Return ClientName
    End Function

    Public Function GetSubSystemId(ByVal SubSystemName As String) As Int16
        Dim SystemId As Int16 = 0
        Dim myConnection As New SqlConnection
        If Create_SQL_Connection(myConnection) <> SUCCESSFUL Then
            Return PROCESS_ERROR
        End If
        Dim myReader As SqlDataReader

        Try
            Dim myCommand As New SqlCommand("select sabd_module_id  from sabd_main_definition where sabd_module_name ='" & SubSystemName & "'", myConnection)
            myReader = myCommand.ExecuteReader(CommandBehavior.SingleRow)
            If myReader.HasRows Then
                While myReader.Read()
                    SystemId = myReader.GetValue(0)
                End While
            End If
        Catch ex As SqlException
            Return SystemId
        End Try

        Close_SQL_Connection(myConnection)
        If Not IsNothing(myReader) Then
            myReader.Close()
        End If
        GC.Collect()

        Return SystemId
    End Function

    Public Function GetInterfaceType(ByVal SubSystemName As String) As Byte
        Dim SystemId As Byte = 0
        Dim myConnection As New SqlConnection
        If Create_SQL_Connection(myConnection) <> SUCCESSFUL Then
            Return PROCESS_ERROR
        End If
        Dim myReader As SqlDataReader
        Try
            Dim myCommand As New SqlCommand("select sabd_msg_format from sabd_main_definition where sabd_module_name ='" & SubSystemName & "'", myConnection)
            myReader = myCommand.ExecuteReader(CommandBehavior.SingleRow)
            If myReader.HasRows Then
                While myReader.Read()
                    SystemId = myReader.GetValue(0)
                End While
            End If
        Catch ex As SqlException
            Return SystemId
        End Try

        Close_SQL_Connection(myConnection)
        If Not IsNothing(myReader) Then
            myReader.Close()
        End If
        GC.Collect()

        Return SystemId
    End Function


    Public Function GetTaskNumber(ByVal SubSystemName As String) As Int16
        Dim TaskNumbers As Int16 = 0

        Dim myConnection As New SqlConnection
        If Create_SQL_Connection(myConnection) <> SUCCESSFUL Then
            Return PROCESS_ERROR
        End If
        Dim myReader As SqlDataReader
        Try
            Dim myCommand As New SqlCommand("select sabd_task  from sabd_main_definition where sabd_module_name ='" & SubSystemName & "'", myConnection)
            myReader = myCommand.ExecuteReader(CommandBehavior.SingleRow)
            If myReader.HasRows Then
                While myReader.Read()
                    TaskNumbers = myReader.GetValue(0)
                End While
            End If
        Catch ex As SqlException
            Return TaskNumbers
        End Try

        Close_SQL_Connection(myConnection)
        If Not IsNothing(myReader) Then
            myReader.Close()
        End If
        GC.Collect()

        Return TaskNumbers
    End Function


    Public Function GetQueueRouter(ByVal SubSystemName As String) As String
        Dim TempVal As String = String.Empty

        Dim myConnection As New SqlConnection
        If Create_SQL_Connection(myConnection) <> SUCCESSFUL Then
            Return PROCESS_ERROR
        End If
        Try
            Dim myReader As SqlDataReader
            Dim myCommand As New SqlCommand("select sabd_assign_router from sabd_main_definition where sabd_module_name ='" & SubSystemName & "'", myConnection)
            myReader = myCommand.ExecuteReader(CommandBehavior.SingleRow)
            If myReader.HasRows Then
                While myReader.Read()
                    TempVal = myReader.GetString(0).Trim
                End While
            End If
            myReader.Close()
        Catch ex As SqlException
            Return ""
        End Try

        Dim myCommand2 As New SqlCommand("select sabd_queue_requirement from sabd_main_definition where sabd_module_name ='" & TempVal & "'", myConnection)

        Try
            Dim myReader As SqlDataReader
            myReader = myCommand2.ExecuteReader(CommandBehavior.SingleRow)
            If myReader.HasRows Then
                While myReader.Read()
                    TempVal = myReader.GetString(0)
                End While
            End If

            myReader.Close()

        Catch ex As SqlException
            Return ""
        End Try

        Close_SQL_Connection(myConnection)
        GC.Collect()

        Return TempVal
    End Function

    'Public Function Get_Init_Information(ByVal ModuleName As String) As String
    '    Dim InitInformation As String = String.Empty
    '    '**********************************************************************************************************************
    '    InitInformation = GetQueueName(MyName, 4) & "|" & GetQueueName(ModuleName, 4) & "|" & MyName
    '    '**********************************************************************************************************************
    '    Return InitInformation
    'End Function
    'Public Function GetQueueName(ByVal SubSystemName As String, ByVal QueueType As Byte) As String
    '    Dim ErrorCode As Int16 = 1
    '    Dim QueueName As String = String.Empty
    '    Dim QueuePathName As String = "XXXXXXXXXX"

    '    Select Case QueueType
    '        Case 0
    '            QueueName = "sabd_queue_requirement"
    '        Case 1
    '            QueueName = "sabd_queue_replies"
    '        Case 2
    '            QueueName = "sabd_queue_comms"
    '        Case 3
    '            QueueName = "sabd_queue_saf"
    '        Case 4
    '            QueueName = "sabd_queue_command"
    '        Case 5
    '            QueueName = "sabd_queue_ack"
    '    End Select
    '    Dim myConnection As New SqlConnection
    '    If Create_SQL_Connection(myConnection) <> SUCCESSFUL Then
    '        Return PROCESS_ERROR
    '    End If
    '    Dim myReader As SqlDataReader
    '    Try
    '        Dim myCommand As New SqlCommand("select " & QueueName & " from sabd_main_definition where sabd_module_name ='" & SubSystemName & "'", myConnection)
    '        myReader = myCommand.ExecuteReader(CommandBehavior.SingleRow)
    '        If myReader.HasRows Then
    '            While myReader.Read()
    '                QueuePathName = myReader.GetValue(0)
    '            End While
    '            ErrorCode = 0
    '        End If
    '    Catch ex As SqlException
    '        Return QueuePathName
    '    End Try

    '    Close_SQL_Connection(myConnection)
    '    If Not IsNothing(myReader) Then
    '        myReader.Close()
    '    End If
    '    GC.Collect()

    '    Return QueuePathName
    'End Function

    Public Function GetAddrPortMode_ByNAME(SubSystemName As String, ByRef IpAddress As String, ByRef PortNumber As Int32, ByRef SocketMode As Byte) As Int16
        Dim ErrorCode As Int16 = 1
        Dim myConnection As New SqlConnection
        If Create_SQL_Connection(myConnection) <> SUCCESSFUL Then
            Return PROCESS_ERROR
        End If
        Dim myReader As SqlDataReader

        Try
            Dim myCommand As New SqlCommand("select sabd_ip_address, sabd_port_number, sabd_connection_mode from sabd_main_definition where sabd_module_name ='" & SubSystemName & "'", myConnection)
            myReader = myCommand.ExecuteReader(CommandBehavior.SingleRow)
            If myReader.HasRows Then
                While myReader.Read()
                    IpAddress = myReader.GetValue(0)
                    PortNumber = myReader.GetValue(1)
                    SocketMode = myReader.GetValue(2)
                End While
                ErrorCode = 0
            End If
        Catch ex As SqlException
            Return ErrorCode
        End Try

        Close_SQL_Connection(myConnection)
        If Not IsNothing(myReader) Then
            myReader.Close()
        End If
        GC.Collect()

        Return ErrorCode
    End Function

    Public Function GetEnabledProcess(ByVal SubSystemName As String) As Byte
        Dim ErrorCode As Byte = UNKNOW_ERROR
        Dim myConnection As New SqlConnection
        If Create_SQL_Connection(myConnection) <> SUCCESSFUL Then
            Return PROCESS_ERROR
        End If
        Try
            Dim myReader As SqlDataReader
            Dim myCommand As New SqlCommand("select sabd_status  from sabd_main_definition where sabd_module_name ='" & SubSystemName & "'", myConnection)
            myReader = myCommand.ExecuteReader(CommandBehavior.SingleRow)
            If myReader.HasRows Then
                While myReader.Read()
                    ErrorCode = myReader.GetValue(0)
                End While
            End If

            If Not IsNothing(myReader) Then
                myReader.Close()
            End If

        Catch ex As SqlException
            Return ErrorCode
        End Try

        Close_SQL_Connection(myConnection)
        GC.Collect()

        Return ErrorCode
    End Function


    Public Function GetPathNames(ByVal ModuleName As String, ByRef ListData As List(Of String)) As Byte
        Dim ErrorCode As Byte = 1

        Dim myConnection As New SqlConnection
        If Create_SQL_Connection(myConnection) <> SUCCESSFUL Then
            Return PROCESS_ERROR
        End If
        Try
            Dim myReader As SqlDataReader
            Dim myCommand As New SqlCommand(" select sabd_source_path, sabd_target_path from sabd_main_definition where sabd_module_name ='" & ModuleName & "'", myConnection)
            myReader = myCommand.ExecuteReader(CommandBehavior.Default)
            While myReader.Read()
                ListData.Add(myReader.GetString(0))
                ListData.Add(myReader.GetString(1))
            End While

            myReader.Close()

            ErrorCode = 0
        Catch ex As Exception
        End Try

        Close_SQL_Connection(myConnection)
        GC.Collect()

        Return ErrorCode
    End Function

    Public Function DB_Get_Transaction_Data(ByVal SRM As SharedStructureMessage) As Int32
        Dim ErrorCode As Int16 = error_RECORD_NOT_FOUND
        Dim Readed As Boolean = False
        Dim l_SwitchSequence As Int32 = SRM.SSM_Common_Data.CRF_Switch_Sequence

        Dim SqlQuery As String = "SELECT SABD_MESSAGE_SEQUENCE FROM SABD_TRANSACTION_LOG_FILE "
        SqlQuery += "where CONVERT(DATE,SABD_TRAN_DATIME_TIME)=@p1 and SABD_TERMINAL_NUMBER=@p2 and SABD_SOURCE_SEQUENCE=@p3 "
        SqlQuery += "and SABD_SOURCE_ACCOUNT=@p5 and SABD_TRAN_CODE=@p6 "
        SqlQuery += "and SABD_TARGET_ACCOUNT=@p7 and SABD_TRANSACTION_AMOUNT=@p8 and SABD_REVERSAL_IND=@p9 "

        Dim myConnection As New SqlConnection
        If Create_SQL_Connection(myConnection) <> SUCCESSFUL Then
            Return error_NOT_AVAILABLE
        End If
        Dim myReader As SqlDataReader
        Try
            Dim myCommand As New SqlCommand
            myCommand.CommandText = SqlQuery
            myCommand.Connection = myConnection

            myCommand.Parameters.Add("@p1", System.Data.SqlDbType.DateTime).Value = Now.ToString("yyyy-MM-dd")
            myCommand.Parameters.Add("@p2", System.Data.SqlDbType.Int).Value = SRM.SSM_Common_Data.CRF_Terminal_ID
            myCommand.Parameters.Add("@p3", System.Data.SqlDbType.Int).Value = SRM.SSM_Common_Data.CRF_Adquirer_Sequence
            'myCommand.Parameters.Add("@p4", System.Data.SqlDbType.Int).Value = SRM.SSM_Common_Data.CRF_Issuer_Institution_Number
            myCommand.Parameters.Add("@p5", System.Data.SqlDbType.BigInt).Value = SRM.SSM_Common_Data.CRF_Primary_Account
            myCommand.Parameters.Add("@p6", System.Data.SqlDbType.Int).Value = SRM.SSM_Common_Data.CRF_Transaction_Code
            myCommand.Parameters.Add("@p7", System.Data.SqlDbType.BigInt).Value = SRM.SSM_Common_Data.CRF_Secondary_Account
            myCommand.Parameters.Add("@p8", System.Data.SqlDbType.Decimal).Value = SRM.SSM_Common_Data.CRF_Transaction_Amount
            myCommand.Parameters.Add("@p9", System.Data.SqlDbType.Int).Value = 0

            myReader = myCommand.ExecuteReader(CommandBehavior.SingleRow)
            If myReader.HasRows Then
                While myReader.Read()
                    l_SwitchSequence = myReader.GetValue(0)
                    Readed = True
                End While
            Else
                Console.WriteLine("No se encontró registro  sec:" & SRM.SSM_Common_Data.CRF_Adquirer_Sequence)
                Readed = False
            End If
            myReader.Close()
            myCommand.Dispose()
            myConnection.Close()
            ReleaseObject(myReader)
            ReleaseObject(myCommand)
            ReleaseObject(myConnection)
        Catch ex As Exception
            Console.WriteLine("Metodo:" & System.Reflection.MethodBase.GetCurrentMethod.Name.ToString & Chr(13) & "Error:" & ex.Message)
            l_SwitchSequence = 0
        End Try
        '******************************************************

        GC.Collect()
        Return l_SwitchSequence
    End Function

    Public Function DB_Fill_States_Institution(ByVal SR As STATE_RECORD) As Byte
        Dim ErrorCode As Int16 = PROCESS_ERROR
        Dim Readed As Boolean = False

        Dim SqlQuery As String = "SELECT * FROM SABD_CURRENT_STATUS_FI WHERE SABD_INSTITUTION_CODE=@p1"

        Dim myConnection As New SqlConnection
        If Create_SQL_Connection(myConnection) <> SUCCESSFUL Then
            Return ErrorCode
        End If
        Dim myReader As SqlDataReader

        Try
            Dim myCommand As New SqlCommand
            myCommand.CommandText = SqlQuery
            myCommand.Connection = myConnection
            myCommand.Parameters.Add("@p1", System.Data.SqlDbType.Int).Value = SR.SABD_INSTITUTION_CODE

            myReader = myCommand.ExecuteReader(CommandBehavior.SingleRow)
            If myReader.HasRows Then
                While myReader.Read()
                    Dim tSR As New STATE_RECORD
                    tSR.SABD_EVENT_SYS_DATE_TIME = myReader.GetValue(0)
                    tSR.SABD_INSTITUTION_CODE = myReader.GetValue(1)
                    tSR.SABD_CURRENT_STATUS = myReader.GetValue(2)
                    tSR.SABD_CHANGUE_MODE = myReader.GetValue(3)
                    Readed = True
                End While
            Else
                Console.WriteLine("No se encontró registro en SABD_CURRENT_STATUS_FI:" & SR.SABD_INSTITUTION_CODE)
                Readed = False
            End If
            myReader.Close()
            myCommand.Dispose()


            If Readed = False Then
                SqlQuery = "INSERT INTO SABD_CURRENT_STATUS_FI VALUES(@p1,@p2,@p3,@p4)"
                myCommand = New SqlCommand
                myCommand.CommandText = SqlQuery
                myCommand.Connection = myConnection
                myCommand.Parameters.Add("@p1", System.Data.SqlDbType.DateTime).Value = Now
                myCommand.Parameters.Add("@p2", System.Data.SqlDbType.Int).Value = SR.SABD_INSTITUTION_CODE
                myCommand.Parameters.Add("@p3", System.Data.SqlDbType.Int).Value = SR.SABD_CURRENT_STATUS
                myCommand.Parameters.Add("@p4", System.Data.SqlDbType.Int).Value = SR.SABD_CHANGUE_MODE
            Else
                SqlQuery = "UPDATE SABD_CURRENT_STATUS_FI SET SABD_EVENT_SYS_DATE_TIME=@p1, SABD_CURRENT_STATUS=@p2, SABD_CHANGUE_MODE=@p3 WHERE SABD_INSTITUTION_CODE=@p4 "
                myCommand = New SqlCommand
                myCommand.CommandText = SqlQuery
                myCommand.Connection = myConnection
                myCommand.Parameters.Add("@p1", System.Data.SqlDbType.DateTime).Value = Now
                myCommand.Parameters.Add("@p2", System.Data.SqlDbType.Int).Value = SR.SABD_CURRENT_STATUS
                myCommand.Parameters.Add("@p3", System.Data.SqlDbType.Int).Value = SR.SABD_CHANGUE_MODE
                myCommand.Parameters.Add("@p4", System.Data.SqlDbType.Int).Value = SR.SABD_INSTITUTION_CODE
            End If

            Dim CNT As Int16 = 0
            '*********************************************
            CNT = myCommand.ExecuteNonQuery()
            '*********************************************
            If CNT = 0 Then
                Console.WriteLine("Registro NO afectado SABD_CURRENT_STATUS_FI:" & SR.SABD_INSTITUTION_CODE)
            End If

            myConnection.Close()
            ReleaseObject(myCommand)
            ReleaseObject(myConnection)
        Catch ex As Exception
            Console.WriteLine("Metodo:" & System.Reflection.MethodBase.GetCurrentMethod.Name.ToString & Chr(13) & "Error:" & ex.Message)
            ErrorCode = FUNCTION_ERROR
        End Try
        '******************************************************

        GC.Collect()

        Return ErrorCode
    End Function

    Public Function DB_Changue_States_Institution(ByVal SR As STATE_RECORD) As Byte
        Dim ErrorCode As Int16 = PROCESS_ERROR
        Dim Readed As Boolean = False

        Dim myConnection As New SqlConnection
        If Create_SQL_Connection(myConnection) <> SUCCESSFUL Then
            Return ErrorCode
        End If

        Try
            Dim SqlQuery As String
            Dim myCommand As New SqlCommand
            SqlQuery = "UPDATE SABD_CURRENT_STATUS_FI SET SABD_EVENT_SYS_DATE_TIME=@p1, SABD_CURRENT_STATUS=@p2, SABD_CHANGUE_MODE=@p3 WHERE SABD_INSTITUTION_CODE=@p4 "
            myCommand = New SqlCommand
            myCommand.CommandText = SqlQuery
            myCommand.Connection = myConnection
            myCommand.Parameters.Add("@p1", System.Data.SqlDbType.DateTime).Value = SR.SABD_EVENT_SYS_DATE_TIME
            myCommand.Parameters.Add("@p2", System.Data.SqlDbType.Int).Value = SR.SABD_CURRENT_STATUS
            myCommand.Parameters.Add("@p3", System.Data.SqlDbType.Int).Value = SR.SABD_CHANGUE_MODE
            myCommand.Parameters.Add("@p4", System.Data.SqlDbType.Int).Value = SR.SABD_INSTITUTION_CODE

            Dim CNT As Int16 = 0
            '*********************************************
            CNT = myCommand.ExecuteNonQuery()
            '*********************************************
            If CNT = 0 Then
                Console.WriteLine("Registro NO afectado SABD_CURRENT_STATUS_FI:" & SR.SABD_INSTITUTION_CODE)
            End If

            myConnection.Close()
            ReleaseObject(myCommand)
            ReleaseObject(myConnection)
        Catch ex As Exception
            Console.WriteLine("Metodo:" & System.Reflection.MethodBase.GetCurrentMethod.Name.ToString & Chr(13) & "Error:" & ex.Message)
            ErrorCode = FUNCTION_ERROR
        End Try
        '******************************************************

        GC.Collect()

        Return ErrorCode
    End Function



End Module

Public Structure STATE_RECORD
    Dim SABD_EVENT_SYS_DATE_TIME As DateTime
    Dim SABD_INSTITUTION_CODE As Int16
    Dim SABD_CURRENT_STATUS As Int16
    Dim SABD_CHANGUE_MODE As Int16
End Structure
