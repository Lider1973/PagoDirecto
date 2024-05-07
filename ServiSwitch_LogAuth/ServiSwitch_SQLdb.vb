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
        Dim ErrorCode As Byte = 1

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
            myReader.Close()
            myConnection.Close()
            myCommand.Dispose()
            ErrorCode = 0
        Catch ex As SqlException
            ErrorCode = 1
        End Try
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
                    Console.WriteLine(TempRec)
                    ParamsTable.Add(TempRec)
                End While
            Else
                ErrorCode = 1
            End If
            myReader.Close()
            myConnection.Close()
            myCommand.Dispose()
            ErrorCode = 0
        Catch ex As SqlException
            ErrorCode = 1
        End Try

        GC.Collect()
        Return ErrorCode
    End Function

    Public Function Get_Table_Definition(ByRef ParamsTable As List(Of String), ByVal TableName As String, ByVal Fields As String, ByVal Separator As Char) As Byte
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
                        TempRec += myReader.GetValue(count) & Separator
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

        If myConnection.State = ConnectionState.Open Then
            myConnection.Close()
        End If
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
            ErrorCode = 0
        Catch ex As SqlException
            ErrorCode = 1
        End Try

        Close_SQL_Connection(myConnection)
        If Not IsNothing(myReader) Then
            myReader.Close()
        End If
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
            ErrorCode = 0
        Catch ex As SqlException
            ErrorCode = 1
        End Try

        Close_SQL_Connection(myConnection)
        If Not IsNothing(myReader) Then
            myReader.Close()
        End If
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
            ErrorCode = 0
        Catch ex As SqlException
            ErrorCode = 1
        End Try

        Close_SQL_Connection(myConnection)
        If Not IsNothing(myReader) Then
            myReader.Close()
        End If
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
        Catch ex As SqlException
            Exist = False
        End Try

        Close_SQL_Connection(myConnection)
        If Not IsNothing(myReader) Then
            myReader.Close()
        End If
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
        Dim myReader As SqlDataReader
        Try
            Dim myCommand As New SqlCommand("select sabd_assign_router from sabd_main_definition where sabd_module_name ='" & SubSystemName & "'", myConnection)
            myReader = myCommand.ExecuteReader(CommandBehavior.SingleRow)
            If myReader.HasRows Then
                While myReader.Read()
                    TempVal = myReader.GetString(0).Trim
                End While
            End If
            myReader.Close()
        Catch ex As SqlException
            myReader.Close()
            Return ""
        End Try

        Dim myCommand2 As New SqlCommand("select sabd_queue_requirement from sabd_main_definition where sabd_module_name ='" & TempVal & "'", myConnection)
        myReader = myCommand2.ExecuteReader(CommandBehavior.SingleRow)
        Try
            If myReader.HasRows Then
                While myReader.Read()
                    TempVal = myReader.GetString(0)
                End While
            End If
        Catch ex As SqlException
            Return ""
        End Try

        Close_SQL_Connection(myConnection)
        If Not IsNothing(myReader) Then
            myReader.Close()
        End If
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
        Dim myReader As SqlDataReader
        Try
            Dim myCommand As New SqlCommand("select sabd_status  from sabd_main_definition where sabd_module_name ='" & SubSystemName & "'", myConnection)
            myReader = myCommand.ExecuteReader(CommandBehavior.SingleRow)
            If myReader.HasRows Then
                While myReader.Read()
                    ErrorCode = myReader.GetValue(0)
                End While
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


    Public Function GetPathNames(ByVal ModuleName As String, ByRef ListData As List(Of String)) As Byte
        Dim ErrorCode As Byte = 1

        Dim myConnection As New SqlConnection
        If Create_SQL_Connection(myConnection) <> SUCCESSFUL Then
            Return PROCESS_ERROR
        End If
        Dim myReader As SqlDataReader
        Try
            Dim myCommand As New SqlCommand(" select sabd_source_path, sabd_target_path from sabd_main_definition where sabd_module_name ='" & ModuleName & "'", myConnection)
            myReader = myCommand.ExecuteReader(CommandBehavior.Default)
            While myReader.Read()
                ListData.Add(myReader.GetString(0))
                ListData.Add(myReader.GetString(1))
            End While
            ErrorCode = 0
            myReader.Close()
            myCommand.Connection.Close()
            myConnection.Close()
        Catch ex As Exception
            Console.WriteLine("Metodo:" & System.Reflection.MethodBase.GetCurrentMethod.Name.ToString & Chr(13) & "Error:" & ex.Message)
        End Try

        GC.Collect()
        Return ErrorCode
    End Function


    Public Function Process_Registry_Transaction(ByVal SRM As SharedStructureMessage) As Byte
        '******************************************************************************
        Dim BigError As String = ""
        Dim ErrorCode As Byte = PROCESS_ERROR
        Try
            ' Estableciento propiedades
            Dim myConnection As New SqlConnection
            If Create_SQL_Connection(myConnection) <> SUCCESSFUL Then
                Return PROCESS_ERROR
            End If

            Dim myCommand As New SqlCommand
            myCommand.Connection = myConnection
            myCommand.CommandText = "INSERT INTO SABD_TRANSACTION_LOG_FILE VALUES (@SABD_SWITCH_DATE_TIME,@SABD_TRAN_DATIME_TIME,@SABD_TERMINAL_NUMBER,@SABD_SOURCE_SEQUENCE,@SABD_SOURCE_FI_CODE,@SABD_MESSAGE_SEQUENCE"
            myCommand.CommandText += ",@SABD_TRAN_CODE,@SABD_ABA_NUMBER,@SABD_SOURCE_ACCOUNT,@SABD_TARGET_ACCOUNT,@SABD_RESPONSE_CODE,@SABD_TRANSACTION_AMOUNT,@SABD_REVERSAL_IND"
            myCommand.CommandText += ",@SABD_TARGET_FI_CODE,@SABD_CHANNEL_ID,@SABD_TOKEN_INFO,@SABD_NOTIFY_CODE,@SABD_NOTIFY_TRIES,@SABD_NOTIFY_DATE_TIME)"

            ' Asignando los valores a los atributos

            myCommand.Parameters.Add("@SABD_SWITCH_DATE_TIME", System.Data.SqlDbType.DateTime).Value = Now
            myCommand.Parameters.Add("@SABD_TRAN_DATIME_TIME", System.Data.SqlDbType.DateTime).Value = SRM.SSM_Common_Data.CRF_Adquirer_Date_Time
            myCommand.Parameters.Add("@SABD_TERMINAL_NUMBER", System.Data.SqlDbType.Int).Value = SRM.SSM_Common_Data.CRF_Terminal_ID
            myCommand.Parameters.Add("@SABD_SOURCE_SEQUENCE", System.Data.SqlDbType.Int).Value = SRM.SSM_Common_Data.CRF_Adquirer_Sequence
            myCommand.Parameters.Add("@SABD_SOURCE_FI_CODE", System.Data.SqlDbType.Int).Value = SRM.SSM_Common_Data.CRF_Adquirer_Institution_Number
            myCommand.Parameters.Add("@SABD_MESSAGE_SEQUENCE", System.Data.SqlDbType.Int).Value = SRM.SSM_Common_Data.CRF_Switch_Sequence
            myCommand.Parameters.Add("@SABD_TRAN_CODE", System.Data.SqlDbType.Int).Value = SRM.SSM_Common_Data.CRF_Transaction_Code
            myCommand.Parameters.Add("@SABD_ABA_NUMBER", System.Data.SqlDbType.Int).Value = SRM.SSM_Common_Data.CRF_Adquirer_Institution_ID
            myCommand.Parameters.Add("@SABD_SOURCE_ACCOUNT", System.Data.SqlDbType.BigInt).Value = SRM.SSM_Common_Data.CRF_Primary_Account
            myCommand.Parameters.Add("@SABD_TARGET_ACCOUNT", System.Data.SqlDbType.BigInt).Value = SRM.SSM_Common_Data.CRF_Secondary_Account
            myCommand.Parameters.Add("@SABD_RESPONSE_CODE", System.Data.SqlDbType.Int).Value = SRM.SSM_Common_Data.CRF_Response_Code
            myCommand.Parameters.Add("@SABD_TRANSACTION_AMOUNT", System.Data.SqlDbType.Decimal).Value = SRM.SSM_Common_Data.CRF_Transaction_Amount
            myCommand.Parameters.Add("@SABD_REVERSAL_IND", System.Data.SqlDbType.Int).Value = SRM.SSM_Common_Data.CRF_Reversal_Indicator
            '***********************************************************************************************************************************************
            If SRM.SSM_Common_Data.CRF_Adquirer_Institution_Number = 188 Then
                myCommand.Parameters.Add("@SABD_TARGET_FI_CODE", System.Data.SqlDbType.Int).Value = Get_Token_Data("A1", SRM.SSM_Common_Data.CRF_Token_Data)
            Else
                myCommand.Parameters.Add("@SABD_TARGET_FI_CODE", System.Data.SqlDbType.Int).Value = SRM.SSM_Common_Data.CRF_Issuer_Institution_ID
            End If
            '***********************************************************************************************************************************************
            myCommand.Parameters.Add("@SABD_CHANNEL_ID", System.Data.SqlDbType.Int).Value = SRM.SSM_Common_Data.CRF_Channel_Id
            myCommand.Parameters.Add("@SABD_TOKEN_INFO", System.Data.SqlDbType.VarChar).Value = SRM.SSM_Common_Data.CRF_Token_Data

            '2020-09-20 Implementación Broadcast
            myCommand.Parameters.Add("@SABD_NOTIFY_CODE", System.Data.SqlDbType.Int).Value = 9999
            myCommand.Parameters.Add("@SABD_NOTIFY_TRIES", System.Data.SqlDbType.Int).Value = 0
            myCommand.Parameters.Add("@SABD_NOTIFY_DATE_TIME", System.Data.SqlDbType.DateTime).Value = Now
            '2020-09-20 Implementación Broadcast

            For x As Int16 = 0 To myCommand.Parameters.Count - 1
                BigError += "Name:" & myCommand.Parameters.Item(x).ParameterName & " Value:" & myCommand.Parameters.Item(x).Value & Chr(13)
            Next
            '*********************************************
            myCommand.ExecuteNonQuery()
            '*********************************************
            myCommand.Parameters.Clear()
            myCommand.Connection.Close()
            myConnection.Close()
            myCommand.Dispose()
            '*********************************************
            ErrorCode = SUCCESSFUL
            '*********************************************
        Catch ex As SqlException
            SaveLogMain(BigError)
            Console.WriteLine("Metodo:" & System.Reflection.MethodBase.GetCurrentMethod.Name.ToString & Chr(13) & "Error:" & ex.Message)
            If (ex.Number = SE_Duplicated_Record) Or (ex.Number = SE_Duplicated_Key) Then
                ErrorCode = DUPLICATED
            End If
        Catch ex As System.Exception
            SaveLogMain(BigError)
            Console.WriteLine("Metodo:" & System.Reflection.MethodBase.GetCurrentMethod.Name.ToString & Chr(13) & "Error:" & ex.Message)
        End Try
        GC.Collect()
        Return ErrorCode
    End Function


    Public Function Get_Unique_Transaction_Log(ByVal SRM As SharedStructureMessage) As Int16
        Dim ErrorCode As Int16 = error_RECORD_NOT_FOUND
        Dim Readed As Boolean = False
        Dim TypeAppl As Int16 = Get_Token_Data("A3", SRM.SSM_Common_Data.CRF_Token_Data)
        Select Case TypeAppl
            Case 2
                TypeAppl = 239
            Case 4
                TypeAppl = 439
            Case 5
                TypeAppl = 539
            Case Else
                TypeAppl = 0
        End Select

        Dim SqlQuery As String = "SELECT * FROM SABD_TRANSACTION_LOG_FILE "
        SqlQuery += "where SABD_TRAN_DATIME_TIME=@p1 and SABD_TERMINAL_NUMBER=@p2 and SABD_SOURCE_SEQUENCE=@p3 "
        SqlQuery += "and SABD_SOURCE_FI_CODE=@p4 and SABD_SOURCE_ACCOUNT=@p5 and SABD_TRAN_CODE=@p6 "
        SqlQuery += "and SABD_TARGET_ACCOUNT=@p7 and SABD_TRANSACTION_AMOUNT=@p8 and SABD_REVERSAL_IND=@p9"

        Dim myConnection As New SqlConnection
        If Create_SQL_Connection(myConnection) <> SUCCESSFUL Then
            Return error_NOT_AVAILABLE
        End If
        Dim myReader As SqlDataReader
        Try
            Dim myCommand As New SqlCommand
            myCommand.CommandText = SqlQuery
            myCommand.Connection = myConnection

            myCommand.Parameters.Add("@p1", System.Data.SqlDbType.DateTime).Value = SRM.SSM_Common_Data.CRF_Adquirer_Date_Time
            myCommand.Parameters.Add("@p2", System.Data.SqlDbType.Int).Value = SRM.SSM_Common_Data.CRF_Terminal_ID
            myCommand.Parameters.Add("@p3", System.Data.SqlDbType.Int).Value = SRM.SSM_Common_Data.CRF_Adquirer_Sequence
            myCommand.Parameters.Add("@p4", System.Data.SqlDbType.Int).Value = SRM.SSM_Common_Data.CRF_Adquirer_Institution_Number
            myCommand.Parameters.Add("@p5", System.Data.SqlDbType.BigInt).Value = SRM.SSM_Common_Data.CRF_Primary_Account
            myCommand.Parameters.Add("@p6", System.Data.SqlDbType.Int).Value = TypeAppl
            myCommand.Parameters.Add("@p7", System.Data.SqlDbType.BigInt).Value = SRM.SSM_Common_Data.CRF_Secondary_Account
            myCommand.Parameters.Add("@p8", System.Data.SqlDbType.Decimal).Value = SRM.SSM_Common_Data.CRF_Transaction_Amount
            myCommand.Parameters.Add("@p9", System.Data.SqlDbType.Int).Value = 0
            'myCommand.Parameters.Add("@p10", System.Data.SqlDbType.Int).Value = 0

            Dim BigError As String = ""
            For x As Int16 = 0 To myCommand.Parameters.Count - 1
                BigError += "Name:" & myCommand.Parameters.Item(x).ParameterName & " Value:" & myCommand.Parameters.Item(x).Value & Chr(13)
            Next

            SaveLogMain(BigError)

            myReader = myCommand.ExecuteReader(CommandBehavior.Default)
            If myReader.HasRows Then
                While myReader.Read()
                    ErrorCode = myReader.GetValue(10)
                    Readed = True
                End While
            End If
            myReader.Close()
            myCommand.Connection.Close()
            myConnection.Close()
        Catch ex As Exception
            Console.WriteLine("Metodo:" & System.Reflection.MethodBase.GetCurrentMethod.Name.ToString & Chr(13) & "Error:" & ex.Message)
            ErrorCode = error_NOT_PROCESS
        End Try

        If Readed = True Then
            Select Case ErrorCode
                Case SUCCESSFUL
                    ErrorCode = SUCCESSFUL
                Case TIMEOUT_HOST2
                    ErrorCode = TIMEOUT_HOST2
                Case Else
                    ErrorCode = error_NOT_PROCESS
            End Select
        End If

        GC.Collect()
        Return ErrorCode
    End Function


    Public Function DB_Update_Transaction_Broadcast_Log(ByVal SRM As SharedStructureMessage) As Int16
        Dim ErrorCode As Int16 = error_RECORD_NOT_FOUND
        Dim Readed As Boolean = False
        Dim l_Status As Int16 = 0
        Dim l_Tries As Int16 = 0
        Dim l_DateTime As DateTime
        Dim OrgSwtSeq As Int64

        Dim TypeAppl As Int16 = Get_Token_Data("S1", SRM.SSM_Common_Data.CRF_Token_Data)
        SRM.SSM_Common_Data.CRF_Issuer_Institution_Number = Get_Token_Data("A1", SRM.SSM_Common_Data.CRF_Token_Data)
        OrgSwtSeq = Get_Token_Data("S2", SRM.SSM_Common_Data.CRF_Token_Data)

        Dim SqlQuery As String = "SELECT SABD_NOTIFY_CODE,SABD_NOTIFY_TRIES,SABD_NOTIFY_DATE_TIME  FROM SABD_TRANSACTION_LOG_FILE "
        SqlQuery += "where SABD_TRAN_DATIME_TIME=@p1 and SABD_TERMINAL_NUMBER=@p2 and SABD_SOURCE_SEQUENCE=@p3 "
        SqlQuery += "and SABD_TARGET_FI_CODE=@p4 and SABD_SOURCE_ACCOUNT=@p5 and SABD_TRAN_CODE=@p6 "
        SqlQuery += "and SABD_TARGET_ACCOUNT=@p7 and SABD_TRANSACTION_AMOUNT=@p8 and SABD_REVERSAL_IND=@p9 "
        SqlQuery += "and SABD_MESSAGE_SEQUENCE=@p10"

        Dim myConnection As New SqlConnection
        If Create_SQL_Connection(myConnection) <> SUCCESSFUL Then
            Return error_NOT_AVAILABLE
        End If
        Dim myReader As SqlDataReader
        Try
            Dim myCommand As New SqlCommand
            myCommand.CommandText = SqlQuery
            myCommand.Connection = myConnection

            myCommand.Parameters.Add("@p1", System.Data.SqlDbType.DateTime).Value = SRM.SSM_Common_Data.CRF_Adquirer_Date_Time
            myCommand.Parameters.Add("@p2", System.Data.SqlDbType.Int).Value = SRM.SSM_Common_Data.CRF_Terminal_ID
            myCommand.Parameters.Add("@p3", System.Data.SqlDbType.Int).Value = SRM.SSM_Common_Data.CRF_Adquirer_Sequence
            myCommand.Parameters.Add("@p4", System.Data.SqlDbType.Int).Value = SRM.SSM_Common_Data.CRF_Issuer_Institution_Number
            myCommand.Parameters.Add("@p5", System.Data.SqlDbType.BigInt).Value = SRM.SSM_Common_Data.CRF_Primary_Account
            myCommand.Parameters.Add("@p6", System.Data.SqlDbType.Int).Value = TypeAppl
            myCommand.Parameters.Add("@p7", System.Data.SqlDbType.BigInt).Value = SRM.SSM_Common_Data.CRF_Secondary_Account
            myCommand.Parameters.Add("@p8", System.Data.SqlDbType.Decimal).Value = SRM.SSM_Common_Data.CRF_Transaction_Amount
            myCommand.Parameters.Add("@p9", System.Data.SqlDbType.Int).Value = SRM.SSM_Common_Data.CRF_Reversal_Indicator
            myCommand.Parameters.Add("@p10", System.Data.SqlDbType.Int).Value = OrgSwtSeq

            For x As Int16 = 0 To (myCommand.Parameters.Count - 1)
                Console.WriteLine(myCommand.Parameters(x).ParameterName & ":" & myCommand.Parameters(x).Value)
            Next

            myReader = myCommand.ExecuteReader(CommandBehavior.SingleRow)
            If myReader.HasRows Then
                While myReader.Read()
                    l_Status = myReader.GetValue(0)
                    l_Tries = myReader.GetValue(1)
                    l_DateTime = myReader.GetValue(2)
                    Readed = True
                End While
                Console.WriteLine("Registro encontrado..........")
            Else
                Console.WriteLine("No se encontró registro  sec:" & SRM.SSM_Common_Data.CRF_Adquirer_Sequence)
                Readed = False
            End If
            myReader.Close()
            myCommand.Dispose()
        Catch ex As Exception
            Console.WriteLine("Metodo:" & System.Reflection.MethodBase.GetCurrentMethod.Name.ToString & Chr(13) & "Error:" & ex.Message)
            ErrorCode = error_NOT_PROCESS
        End Try

        If Readed = False Then
            myConnection.Close()
            Return PROCESS_ERROR
        End If

        Try
            '****************************************************
            'Select Case SRM.SSM_Common_Data.CRF_Response_Code
            '    Case "8364"
            '        l_Status = "W"
            '    Case "8367"
            '        l_Status = "T"
            '    Case "9001"
            '        l_Status = "M"
            '    Case "9002"
            '        l_Status = "N"
            'End Select
            '****************************************************
            l_Status = SRM.SSM_Common_Data.CRF_Response_Code
            l_Tries += 1
            '****************************************************
            SqlQuery = "UPDATE SABD_TRANSACTION_LOG_FILE set SABD_NOTIFY_CODE=@p10,SABD_NOTIFY_TRIES=@p11,SABD_NOTIFY_DATE_TIME=@p12 "
            SqlQuery += "where SABD_TRAN_DATIME_TIME=@p1 and SABD_TERMINAL_NUMBER=@p2 and SABD_SOURCE_SEQUENCE=@p3 "
            SqlQuery += "and SABD_TARGET_FI_CODE=@p4 and SABD_SOURCE_ACCOUNT=@p5 and SABD_TRAN_CODE=@p6 "
            SqlQuery += "and SABD_TARGET_ACCOUNT=@p7 and SABD_TRANSACTION_AMOUNT=@p8 and SABD_REVERSAL_IND=@p9 "
            SqlQuery += "and SABD_MESSAGE_SEQUENCE=@p13"

            '****************************************************
            Dim myCommand As New SqlCommand
            myCommand.CommandText = SqlQuery
            myCommand.Connection = myConnection
            '****************************************************
            myCommand.Parameters.Add("@p1", System.Data.SqlDbType.DateTime).Value = SRM.SSM_Common_Data.CRF_Adquirer_Date_Time
            myCommand.Parameters.Add("@p2", System.Data.SqlDbType.Int).Value = SRM.SSM_Common_Data.CRF_Terminal_ID
            myCommand.Parameters.Add("@p3", System.Data.SqlDbType.Int).Value = SRM.SSM_Common_Data.CRF_Adquirer_Sequence
            myCommand.Parameters.Add("@p4", System.Data.SqlDbType.Int).Value = SRM.SSM_Common_Data.CRF_Issuer_Institution_Number
            myCommand.Parameters.Add("@p5", System.Data.SqlDbType.BigInt).Value = SRM.SSM_Common_Data.CRF_Primary_Account
            myCommand.Parameters.Add("@p6", System.Data.SqlDbType.Int).Value = TypeAppl
            myCommand.Parameters.Add("@p7", System.Data.SqlDbType.BigInt).Value = SRM.SSM_Common_Data.CRF_Secondary_Account
            myCommand.Parameters.Add("@p8", System.Data.SqlDbType.Decimal).Value = SRM.SSM_Common_Data.CRF_Transaction_Amount
            myCommand.Parameters.Add("@p9", System.Data.SqlDbType.Int).Value = SRM.SSM_Common_Data.CRF_Reversal_Indicator
            myCommand.Parameters.Add("@p10", System.Data.SqlDbType.Int).Value = l_Status
            myCommand.Parameters.Add("@p11", System.Data.SqlDbType.Int).Value = l_Tries
            myCommand.Parameters.Add("@p12", System.Data.SqlDbType.DateTime).Value = Now
            myCommand.Parameters.Add("@p13", System.Data.SqlDbType.Int).Value = OrgSwtSeq
            '*********************************************
            Dim Affected As Int16 = myCommand.ExecuteNonQuery()
            If Affected = 0 Then
                Console.WriteLine("No se actualizó registro  sec:" & SRM.SSM_Common_Data.CRF_Adquirer_Sequence)
            End If
            '*********************************************
            myCommand.Parameters.Clear()
            myCommand.Connection.Close()
            myConnection.Close()
            myCommand.Dispose()
            '*********************************************
            'Select Case l_Status
            '    Case "W"
            '        ErrorCode = PROCESS_ERROR
            '    Case "M", "N"
            '        ErrorCode = SUCCESSFUL
            'End Select
            ErrorCode = SUCCESSFUL
            '*********************************************
        Catch ex As Exception
            Console.WriteLine("Metodo:" & System.Reflection.MethodBase.GetCurrentMethod.Name.ToString & Chr(13) & "Error:" & ex.Message)
        End Try

        GC.Collect()
        Return ErrorCode
    End Function

    Public Function DB_Get_Status_Log_Record(ByVal SRM As SharedStructureMessage) As Int16
        Dim ErrorCode As Int16 = error_RECORD_NOT_FOUND
        Dim Readed As Boolean = False
        Dim l_Status As Int16
        Dim l_Tries As Int16 = 0
        Dim l_DateTime As DateTime

        Dim TypeAppl As Int16 = Get_Token_Data("S1", SRM.SSM_Common_Data.CRF_Token_Data)
        SRM.SSM_Common_Data.CRF_Issuer_Institution_Number = Get_Token_Data("A1", SRM.SSM_Common_Data.CRF_Token_Data)

        Dim SqlQuery As String = "SELECT SABD_NOTIFY_CODE,SABD_NOTIFY_TRIES,SABD_NOTIFY_DATE_TIME  FROM SABD_TRANSACTION_LOG_FILE "
        SqlQuery += "where SABD_TRAN_DATIME_TIME=@p1 and SABD_TERMINAL_NUMBER=@p2 and SABD_SOURCE_SEQUENCE=@p3 "
        SqlQuery += "and SABD_TARGET_FI_CODE=@p4 and SABD_SOURCE_ACCOUNT=@p5 and SABD_TRAN_CODE=@p6 "
        SqlQuery += "and SABD_TARGET_ACCOUNT=@p7 and SABD_TRANSACTION_AMOUNT=@p8 and SABD_REVERSAL_IND=@p9 "
        SqlQuery += "and SABD_MESSAGE_SEQUENCE=@p10"

        Dim myConnection As New SqlConnection
        If Create_SQL_Connection(myConnection) <> SUCCESSFUL Then
            Return error_NOT_AVAILABLE
        End If
        Dim myReader As SqlDataReader
        Try
            Dim myCommand As New SqlCommand
            myCommand.CommandText = SqlQuery
            myCommand.Connection = myConnection

            myCommand.Parameters.Add("@p1", System.Data.SqlDbType.DateTime).Value = SRM.SSM_Common_Data.CRF_Adquirer_Date_Time
            myCommand.Parameters.Add("@p2", System.Data.SqlDbType.Int).Value = SRM.SSM_Common_Data.CRF_Terminal_ID
            myCommand.Parameters.Add("@p3", System.Data.SqlDbType.Int).Value = SRM.SSM_Common_Data.CRF_Adquirer_Sequence
            myCommand.Parameters.Add("@p4", System.Data.SqlDbType.Int).Value = SRM.SSM_Common_Data.CRF_Issuer_Institution_Number
            myCommand.Parameters.Add("@p5", System.Data.SqlDbType.BigInt).Value = SRM.SSM_Common_Data.CRF_Primary_Account
            myCommand.Parameters.Add("@p6", System.Data.SqlDbType.Int).Value = TypeAppl
            myCommand.Parameters.Add("@p7", System.Data.SqlDbType.BigInt).Value = SRM.SSM_Common_Data.CRF_Secondary_Account
            myCommand.Parameters.Add("@p8", System.Data.SqlDbType.Decimal).Value = SRM.SSM_Common_Data.CRF_Transaction_Amount
            myCommand.Parameters.Add("@p9", System.Data.SqlDbType.Int).Value = SRM.SSM_Common_Data.CRF_Reversal_Indicator
            myCommand.Parameters.Add("@p10", System.Data.SqlDbType.Int).Value = SRM.SSM_Common_Data.CRF_Switch_Sequence

            myReader = myCommand.ExecuteReader(CommandBehavior.SingleRow)
            If myReader.HasRows Then
                While myReader.Read()
                    l_Status = myReader.GetValue(0)
                    l_Tries = myReader.GetValue(1)
                    l_DateTime = myReader.GetValue(2)
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
            ErrorCode = error_NOT_PROCESS
        End Try
        '******************************************************
        Select Case l_Status
            Case 9001, 9002
                ErrorCode = DELETE_RECORD
            Case 8364
                If l_Tries >= 3 Then
                    ErrorCode = DELETE_RECORD
                    SaveLogMain(Convert_Structure_To_String(SRM.SSM_Common_Data, ","))
                Else
                    Dim TMSP As TimeSpan
                    TMSP = Now.Subtract(l_DateTime)
                    Console.WriteLine("Inside table time Secs:" & TMSP.TotalSeconds)
                    If TMSP.TotalSeconds < 60 Then
                        ErrorCode = NOT_SEND
                    Else
                        ErrorCode = NOTIFY_RECORD
                    End If
                End If
            Case 8367
                If l_Tries >= 5 Then
                    ErrorCode = DELETE_RECORD
                    SaveLogMain(Convert_Structure_To_String(SRM.SSM_Common_Data, ","))
                Else
                    Dim TMSP As TimeSpan
                    TMSP = Now.Subtract(l_DateTime)
                    Console.WriteLine("Inside table time Secs:" & TMSP.TotalSeconds)
                    If TMSP.TotalSeconds < 60 Then
                        ErrorCode = NOT_SEND
                    Else
                        ErrorCode = NOTIFY_RECORD
                    End If
                End If
            Case 9999
                ErrorCode = NOTIFY_RECORD
            Case Else
                ErrorCode = NOTIFY_RECORD
        End Select
        '******************************************************
        GC.Collect()
        Return ErrorCode
    End Function

    Public Function Get_Dictionary_Data(ByVal DIC_TLR As Dictionary(Of Int64, Transaction_Log_Record)) As Byte
        Dim ErrorCode As Int16 = PROCESS_ERROR

        Dim SqlQuery As String = "select * from [SERVISWITCH].[dbo].[SABD_TRANSACTION_LOG_FILE] "
        SqlQuery += "where DATEDIFF(SECOND,SABD_NOTIFY_DATE_TIME, GETDATE())>60 "
        SqlQuery += "and SABD_NOTIFY_STATUS IN('W')"

        Dim myConnection As New SqlConnection
        If Create_SQL_Connection(myConnection) <> SUCCESSFUL Then
            Return ErrorCode
        End If
        Dim myReader As SqlDataReader
        Try
            Dim myCommand As New SqlCommand
            myCommand.CommandText = SqlQuery
            myCommand.Connection = myConnection

            myReader = myCommand.ExecuteReader(CommandBehavior.Default)
            If myReader.HasRows Then
                Dim CNT As Int64
                While myReader.Read()
                    Dim TLR As New Transaction_Log_Record
                    TLR.SABD_SWITCH_DATE_TIME = myReader.GetValue(0)
                    TLR.SABD_TRAN_DATIME_TIME = myReader.GetValue(1)
                    TLR.SABD_TERMINAL_NUMBER = myReader.GetValue(2)
                    TLR.SABD_SOURCE_SEQUENCE = myReader.GetValue(3)
                    TLR.SABD_SOURCE_FI_CODE = myReader.GetValue(4)
                    TLR.SABD_MESSAGE_SEQUENCE = myReader.GetValue(5)
                    TLR.SABD_TRAN_CODE = myReader.GetValue(6)
                    TLR.SABD_ABA_NUMBER = myReader.GetValue(7)
                    TLR.SABD_SOURCE_ACCOUNT = myReader.GetValue(8)
                    TLR.SABD_TARGET_ACCOUNT = myReader.GetValue(9)
                    TLR.SABD_RESPONSE_CODE = myReader.GetValue(10)
                    TLR.SABD_TRANSACTION_AMOUNT = myReader.GetValue(11)
                    TLR.SABD_REVERSAL_IND = myReader.GetValue(12)
                    TLR.SABD_TARGET_FI_CODE = myReader.GetValue(13)
                    TLR.SABD_CHANNEL_ID = myReader.GetValue(14)
                    TLR.SABD_TOKEN_INFO = myReader.GetValue(15)
                    TLR.SABD_NOTIFY_STATUS = myReader.GetValue(16)
                    TLR.SABD_NOTIFY_TRIES = myReader.GetValue(17)
                    TLR.SABD_NOTIFY_DATE_TIME = myReader.GetValue(18)
                    CNT += 1
                    DIC_TLR.Add(CNT, TLR)
                End While
            End If
            myReader.Close()
            ReleaseObject(myReader)
            myCommand.Dispose()
            ReleaseObject(myCommand)
            myConnection.Close()
            ReleaseObject(myConnection)
            ErrorCode = SUCCESSFUL
        Catch ex As Exception
            Console.WriteLine("Metodo:" & System.Reflection.MethodBase.GetCurrentMethod.Name.ToString & Chr(13) & "Error:" & ex.Message)
            ErrorCode = error_NOT_PROCESS
        End Try

        GC.Collect()
        Return ErrorCode
    End Function


End Module

Public Structure Transaction_Log_Record
    Dim SABD_SWITCH_DATE_TIME As DateTime
    Dim SABD_TRAN_DATIME_TIME As DateTime
    Dim SABD_TERMINAL_NUMBER As Int32
    Dim SABD_SOURCE_SEQUENCE As Int32
    Dim SABD_SOURCE_FI_CODE As Int16
    Dim SABD_TARGET_FI_CODE As Int16
    Dim SABD_MESSAGE_SEQUENCE As Int32
    Dim SABD_TRAN_CODE As Int16
    Dim SABD_ABA_NUMBER As Int32
    Dim SABD_SOURCE_ACCOUNT As Int64
    Dim SABD_TARGET_ACCOUNT As Int64
    Dim SABD_RESPONSE_CODE As Int16
    Dim SABD_TRANSACTION_AMOUNT As Decimal
    Dim SABD_REVERSAL_IND As Int16
    Dim SABD_CHANNEL_ID As Int16
    Dim SABD_TOKEN_INFO As String
    Dim SABD_NOTIFY_STATUS As Char
    Dim SABD_NOTIFY_TRIES As Int16
    Dim SABD_NOTIFY_DATE_TIME As DateTime
End Structure


Public Structure Main_Record
    Public sabd_module_name As String
    Public sabd_executable As String
    Public sabd_status As Byte
    Public sabd_queue_ack As String
    Public sabd_msg_format As Byte
    Public sabd_assign_router As String
    Public sabd_queue_command As String
    Public sabd_queue_saf As String
    Public sabd_queue_comms As String
    Public sabd_queue_replies As String
    Public sabd_queue_requirement As String
    Public sabd_connection_mode As Byte
    Public sabd_port_number As Short
    Public sabd_ip_address As String
    Public sabd_timeout As Byte
    Public sabd_instance As Byte
    Public sabd_task As Byte
    Public sabd_detail_name As String
    Public sabd_type As Integer
    Public sabd_module_id As Integer
    Public sabd_source_path As String
    Public sabd_security_profile As String
End Structure
