Imports System.Data.SqlClient

Module SWTM_DataBase
    Public Const Lmanager_DISPLAY As String = "DPS"
    Public Const Lmanager_REFRESH As String = "RFH"
    Public Const Lmanager_STATUS As String = "STA"

    Public Const Router_NOTIFY As String = "NTF|"
    Public Const Router_ORDER As String = "ORD|"

    Public Const commander_DISPLAY As String = "DPS|"

    Public Const COLOR_BLACK As Byte = 0
    Public Const COLOR_WHITE As Byte = 15

    Public Const ERROR_UNKNOW_USER As Byte = 101
    Public Const FUNCTION_ERROR As Byte = 2
    Public Const SUCCESSFUL As Byte = 0
    Public Const PROCESS_ERROR As Byte = 1
    Public Const PrivateQueue As String = ".\Private$\"
    Public Const ReleasePath As String = "C:\ServiceSwitch\ServiceSwitch_Release\"
    Public Const ExecutePath As String = "C:\ServiceSwitch\ServiceSwitch_Applications\"



    Public CommandQueue As String
    Public RouterCommandQueue As String
    Public ReplyQueue As String
    Public Const PathStart As String = "C:\SystemATM\Ejecutables"
    'Public Const PathStart As String = "C:\ServerAPPL\Ejecutables"

    Public myConnection As New SqlConnection
    Public myCommand As New SqlCommand
    Public myReader As SqlDataReader
    Dim MyStringConnection(2) As String


    Public Sub InitDatabaseResources()
        Dim Security As New EncodeDecodeFunctions
        Security.Load_Config_Keys()
        SetGetStringConnection(0) = Security.SetgetStringConnection(0)
        SetGetStringConnection(1) = Security.SetgetStringConnection(1)
        SetGetStringConnection(2) = Security.SetgetStringConnection(2)
        Security = Nothing
    End Sub

    Public Property SetGetStringConnection(ByVal idx As Byte) As String
        Get
            Return MyStringConnection(idx)
        End Get
        Set(ByVal value As String)
            MyStringConnection(idx) = value
        End Set
    End Property

    Public Function Create_SQL_Connection(ByVal idx As Byte) As Byte
        Dim ErrorCode As Byte = 1
        Dim StringConnection As String = String.Empty
        'Dim SqlConnection As New SqlClient.SqlConnection
        Dim ED As New EncodeDecodeFunctions
        myConnection.ConnectionString = SetGetStringConnection(idx)
        ErrorCode = 0
        Try
            myConnection.Open()
            myCommand.Connection = myConnection
            Return 0
        Catch ex As Exception
            Return 1
        End Try

    End Function

    Public Function GetProcessList(ByVal ParamsTable As List(Of String)) As Byte
        Dim ErrorCode As Byte = 1

        myCommand.CommandText = "select sabd_module_name from sabd_main_definition"
        myReader = myCommand.ExecuteReader(CommandBehavior.Default)
        Try
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
            ErrorCode = 0
        Catch ex As SqlException
            ErrorCode = 1
        End Try

        Return ErrorCode
    End Function


    Public Function GetInfoNode(ByVal ModuleName As String, ByVal ParamsTable As List(Of String)) As Byte
        Dim ErrorCode As Byte = 1

        myCommand.CommandText = "select * from sabd_main_definition where sabd_module_name ='" & ModuleName & "'"
        myReader = myCommand.ExecuteReader(CommandBehavior.SingleRow)

        Try
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
            ErrorCode = 0
        Catch ex As SqlException
            ErrorCode = 1
        End Try

        Return ErrorCode
    End Function

    Public Function GetTaskNumber(ByVal ModuleName As String) As Int16
        Dim TaskNumber As Int16 = 0

        myCommand.CommandText = "select sabd_task from sabd_main_definition where sabd_module_name ='" & ModuleName & "'"
        myReader = myCommand.ExecuteReader(CommandBehavior.SingleRow)

        Try
            If myReader.HasRows Then
                While myReader.Read()
                    For count As Integer = 0 To (myReader.FieldCount - 1)
                        TaskNumber = myReader.GetValue(0)
                    Next
                End While
            Else
                TaskNumber = 0
            End If
            myReader.Close()
        Catch ex As SqlException
            TaskNumber = 0
        End Try

        Return TaskNumber
    End Function


    Public Function ExistSubsystemName(ByVal SubSystemName As String) As Boolean
        Dim Exist As Boolean = False
        Dim TempValue As String = String.Empty

        myCommand.CommandText = "select sabd_module_name from sabd_main_definition where sabd_module_name ='" & SubSystemName & "'"
        myReader = myCommand.ExecuteReader(CommandBehavior.SingleRow)

        Try
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
            myReader.Close()
        Catch ex As SqlException
            Exist = False
        End Try

        Return Exist
    End Function


    Public Function GetClientName(ByVal PortId As Int16, ByVal SOCKET_MODE As Byte) As String
        Dim ClientName As String = "XXXXXXXXXX"

        myCommand.CommandText = "select sabd_module_name  from sabd_main_definition where sabd_port_number = " & PortId & " and sabd_connection_mode = " & SOCKET_MODE
        myReader = myCommand.ExecuteReader(CommandBehavior.SingleRow)

        Try
            If myReader.HasRows Then
                While myReader.Read()
                    ClientName = myReader.GetString(0).TrimEnd
                End While
            End If
            myReader.Close()
        Catch ex As SqlException
            Return ClientName
        End Try

        Return ClientName
    End Function

    Public Function GetSubSystemId(ByVal SubSystemName As String) As Int16
        Dim SystemId As Int16 = 0

        myCommand.CommandText = "select sabd_module_id  from sabd_main_definition where sabd_module_name ='" & SubSystemName & "'"
        myReader = myCommand.ExecuteReader(CommandBehavior.SingleRow)

        Try
            If myReader.HasRows Then
                While myReader.Read()
                    SystemId = myReader.GetValue(0)
                End While
            End If
            myReader.Close()
        Catch ex As SqlException
            Return SystemId
        End Try

        Return SystemId
    End Function

    Public Function GetAddrPortMode_ByNAME(ByVal SubSystemName As String, ByRef IpAddress As String, ByRef PortNumber As Int32, ByRef SocketMode As Byte) As Int16
        Dim ErrorCode As Int16 = 1

        myCommand.CommandText = "select sabd_ip_address, sabd_port_number, sabd_connection_mode from sabd_main_definition where sabd_module_name ='" & SubSystemName & "'"
        myReader = myCommand.ExecuteReader(CommandBehavior.SingleRow)

        Try
            If myReader.HasRows Then
                While myReader.Read()
                    IpAddress = myReader.GetValue(0)
                    PortNumber = myReader.GetValue(1)
                    SocketMode = myReader.GetValue(2)
                End While
                ErrorCode = 0
            End If
            myReader.Close()
        Catch ex As SqlException
            Return ErrorCode
        End Try

        Return ErrorCode
    End Function


    Public Sub GetModulesDefined(ByRef ModuleList As List(Of String))
        myCommand.CommandText = "select sabd_module_name, sabd_type  from sabd_main_definition where sabd_status=1"
        myReader = myCommand.ExecuteReader(CommandBehavior.SingleResult)

        Try
            If myReader.HasRows Then
                While myReader.Read()
                    ModuleList.Add(myReader.GetString(0) & "," & myReader.GetInt32(1))
                End While
            End If
            myReader.Close()
        Catch ex As SqlException
            Return
        End Try
    End Sub

    Public Function GetModuleType(ByVal ModuleName As String) As Byte
        Dim ModuleType As Byte = 99
        myCommand.CommandText = "select sabd_type  from sabd_main_definition where sabd_module_name='" & ModuleName & "'"
        myReader = myCommand.ExecuteReader(CommandBehavior.SingleResult)

        Try
            If myReader.HasRows Then
                While myReader.Read()
                    ModuleType = myReader.GetInt32(0)
                End While
            End If
            myReader.Close()
        Catch ex As SqlException
            Return ModuleType
        End Try

        Return ModuleType
    End Function



    Public Function GetInfoMainData(ByVal ModuleName As String, ByRef ListData As List(Of String)) As Byte
        Dim ErrorCode As Byte = 1

        myCommand.CommandText = " select * from sabd_main_definition where sabd_module_name = '" & ModuleName & "'"
        myReader = myCommand.ExecuteReader(CommandBehavior.Default)
        While myReader.Read()
            ListData.Add(myReader.GetValue(1))
            ListData.Add(myReader.GetValue(2))
            ListData.Add(myReader.GetValue(3))
            ListData.Add(myReader.GetValue(4))
            ListData.Add(myReader.GetValue(5))
            ListData.Add(myReader.GetValue(6))
            ListData.Add(myReader.GetValue(7))
            ListData.Add(myReader.GetValue(8))
            ListData.Add(myReader.GetValue(9))
            ListData.Add(myReader.GetValue(10))
            ListData.Add(myReader.GetValue(11))
            ListData.Add(myReader.GetValue(12))
            ListData.Add(myReader.GetValue(13))
            ListData.Add(myReader.GetValue(14))
            ListData.Add(myReader.GetValue(15))
            ListData.Add(myReader.GetValue(16))
            ListData.Add(myReader.GetValue(17))
            ListData.Add(myReader.GetValue(18))
            ListData.Add(myReader.GetValue(19)) 'EXE
            ListData.Add(myReader.GetValue(20)) 'SOURCE
            ListData.Add(myReader.GetValue(21)) 'TARGET
        End While
        ErrorCode = 0
        myReader.Close()

        Return ErrorCode

    End Function

    Public Function GetExplicitData(ByVal QuerySQL As String, ByRef ListData As List(Of String)) As Byte
        Dim ErrorCode As Byte = 1

        Try
            myCommand.CommandText = QuerySQL
            myReader = myCommand.ExecuteReader(CommandBehavior.Default)
            If myReader.HasRows Then
                While myReader.Read()
                    ListData.Add(myReader.GetValue(0))
                End While
            Else
                ListData.Add("          ")
            End If
            ErrorCode = 0
        Catch ex As Exception
            ErrorCode = 1
        End Try
        myReader.Close()

        Return ErrorCode

    End Function

    Public Function GetExplicitData_Product(ByVal QuerySQL As String, ByRef ListData As List(Of String)) As Byte
        Dim ErrorCode As Byte = 1

        Dim L_Connection As New SqlConnection
        Dim L_Reader As SqlDataReader
        Dim L_Command As New SqlCommand
        Try
            L_Connection.ConnectionString = SetGetStringConnection(2)
            L_Connection.Open()
        Catch ex As Exception
            MsgBox("No se puede conectar a la Base:" & L_Connection.ConnectionString)
        End Try
        L_Command.Connection = L_Connection
        L_Command.CommandText = QuerySQL
        Try
            L_Reader = L_Command.ExecuteReader(CommandBehavior.Default)
            If L_Reader.HasRows Then
                While L_Reader.Read()
                    ListData.Add(L_Reader.GetString(0))
                End While
            Else
                ListData.Add("          ")
            End If
            L_Reader.Close()
            L_Command.Dispose()
            L_Connection.Close()
            ErrorCode = 0
        Catch ex As Exception
            ErrorCode = 1
        End Try

        Return ErrorCode
    End Function


    Public Function GetExplicitData(ByVal QuerySQL As String, ByRef StringData As String) As Byte
        Dim ErrorCode As Byte = 1

        Try
            myCommand.CommandText = QuerySQL
            myReader = myCommand.ExecuteReader(CommandBehavior.Default)
            If myReader.HasRows Then
                While myReader.Read()
                    StringData = myReader.GetValue(0)
                End While
            Else
                StringData = "          "
            End If
            ErrorCode = 0
        Catch ex As Exception
            ErrorCode = 1
        End Try
        myReader.Close()

        Return ErrorCode

    End Function


    Public Function GetFieldName(ByVal BitNumber As Byte) As String
        Dim FieldName As String = String.Empty

        Try
            myCommand.CommandText = "select sabd_field_name from sabd_iso_definition where sabd_field_id=" & BitNumber
            myReader = myCommand.ExecuteReader(CommandBehavior.Default)
            If myReader.HasRows Then
                While myReader.Read()
                    FieldName = myReader.GetValue(0)
                End While
            Else
                FieldName = "No descriptor"
            End If
        Catch ex As Exception
            FieldName = "No descriptor"
        End Try
        myReader.Close()

        Return FieldName

    End Function


    Public Function GetInfoTranData(ByVal TransactionCode As String, ByRef ListData As List(Of String)) As Byte
        Dim ErrorCode As Byte = 1

        myCommand.CommandText = " select * from sabd_transaction_definition where sabd_transaction_Name = '" & TransactionCode & "'"
        myReader = myCommand.ExecuteReader(CommandBehavior.Default)
        While myReader.Read()
            ListData.Add(myReader.GetValue(0))
            ListData.Add(myReader.GetValue(1))
            ListData.Add(myReader.GetValue(2))
            ListData.Add(myReader.GetValue(3))
            ListData.Add(myReader.GetValue(4))
        End While
        ErrorCode = 0
        myReader.Close()

        Return ErrorCode

    End Function


    Public Function GetInfoFieldData(ByVal FieldId As Byte, ByRef ListData As List(Of String)) As Byte
        Dim ErrorCode As Byte = 1

        myCommand.CommandText = " select * from sabd_iso_definition where sabd_field_id =" & FieldId
        myReader = myCommand.ExecuteReader(CommandBehavior.Default)
        While myReader.Read()
            ListData.Add(myReader.GetValue(0))
            ListData.Add(myReader.GetValue(1))
            ListData.Add(myReader.GetValue(2))
            ListData.Add(myReader.GetValue(3))
            ListData.Add(myReader.GetValue(4))
            ListData.Add(myReader.GetValue(5))
        End While
        ErrorCode = 0
        myReader.Close()

        Return ErrorCode

    End Function

    Public Function GetInfoServiceData(ByVal FieldData As String, ByRef ListData As List(Of String)) As Byte
        Dim ErrorCode As Byte = 1

        myCommand.CommandText = " select * from sabd_service_definition where sabd_service_code ='" & FieldData & "'"
        myReader = myCommand.ExecuteReader(CommandBehavior.Default)
        While myReader.Read()
            ListData.Add(myReader.GetValue(0))
            ListData.Add(myReader.GetValue(1))
        End While
        ErrorCode = 0
        myReader.Close()

        Return ErrorCode

    End Function

    Public Function GetInfoRoutingData(ByVal FieldData As String, ByRef ListData As List(Of String)) As Byte
        Dim ErrorCode As Byte = 1

        myCommand.CommandText = " select * from sabd_routing_definition where sabd_routing_code ='" & FieldData & "'"
        myReader = myCommand.ExecuteReader(CommandBehavior.Default)
        While myReader.Read()
            ListData.Add(myReader.GetValue(0))
            ListData.Add(myReader.GetValue(1))
            ListData.Add(myReader.GetValue(2))
        End While
        ErrorCode = 0
        myReader.Close()

        Return ErrorCode

    End Function

    Public Function GetInfoInstitutionData(ByVal institutionCode As Int32, ByRef ListData As List(Of String)) As Byte

        Dim ErrorCode As Byte = 1

        myCommand.CommandText = " select * from sabd_institution_definition where sabd_codigo_autorizador =" & institutionCode
        myReader = myCommand.ExecuteReader(CommandBehavior.Default)
        While myReader.Read()
            ListData.Add(myReader.GetValue(1))
            ListData.Add(myReader.GetValue(2))
            ListData.Add(myReader.GetValue(3))
            ListData.Add(myReader.GetValue(4))
            ListData.Add(myReader.GetValue(5))
            ListData.Add(myReader.GetValue(6))
            ListData.Add(myReader.GetValue(7))
            ListData.Add(myReader.GetValue(8))
        End While
        ErrorCode = 0
        myReader.Close()

        Return ErrorCode

    End Function


    Public Function Get_Info_Key(ByRef KeySearch As String, ByRef Aux_001 As String, ByRef Aux_002 As String, ByVal mode As Byte) As Byte
        Dim ErrorCode As Byte = 1
        Dim ErrorMessage As String = String.Empty
        Dim StringSQL As String = String.Empty
        Dim Dato1 As String = Nothing
        Dim Dato2 As String = Nothing

        Select Case mode
            Case 0
                StringSQL = "select sabd_module_id, sabd_type from sabd_main_definition where sabd_module_name='" & KeySearch & "'"
            Case 1
                StringSQL = "select sabd_module_name, sabd_type from sabd_main_definition where sabd_module_id=" & KeySearch
                'Case 2
                'StringSQL = "select sabd_transaction_code, sabd_transaction_name from sabd_transaction_definition where sabd_transaction_code='" & KeySearch & "' and sabd_transaction_type=" & SetGetComplement
            Case 3
                StringSQL = "select sabd_field_id, sabd_field_name from sabd_iso_definition where sabd_field_id=" & KeySearch
            Case 4
                StringSQL = "select sabd_codigo_autorizador, sabd_nombre from sabd_institution_definition where sabd_codigo_autorizador=" & KeySearch
            Case 5
                StringSQL = "select sabd_routing_code, sabd_process_ID from sabd_routing_definition where sabd_routing_code=" & KeySearch
            Case 6
                StringSQL = "select sabd_branch_name, sabd_branch_institution from sabd_branch_definition where sabd_branch_code=" & KeySearch
            Case 7
                StringSQL = "select sabd_nombre, sabd_codigo_adquirente from sabd_institution_definition where sabd_codigo_autorizador=" & KeySearch
            Case 8
                'StringSQL = "select sabd_county_code, (CONVERT(varchar(10),sabd_city_code) + ',' + sabd_city_name), sabd_region as SpecialQuery from sabd_location_definition where sabd_location_code=" & KeySearch
                StringSQL = "select sabd_county_code, (CONVERT(varchar(10),sabd_city_code) + ',' + sabd_city_name + ',' + CONVERT(varchar(10),sabd_region)) as SpecialQuery from sabd_location_definition where sabd_location_code=" & KeySearch
            Case 9
                StringSQL = "select sabd_county_name, (CONVERT(varchar(1),sabd_region) + ',' + sabd_city_name) as SpecialQuery from sabd_location_definition where sabd_location_code='" & KeySearch & "'"
            Case 10
                StringSQL = "select sabd_terminal_name, (CONVERT(varchar(1),sabd_terminal_status) + ',' + sabd_terminal_branch + ',' + sabd_terminal_concenter + ',' + CONVERT(varchar(5),sabd_terminal_port)) + ',' + sabd_user_logged + ',' + sabd_terminal_supervisor as SpecialQuery from sabd_terminal_definition where sabd_terminal_code='" & KeySearch & "'"
            Case 11
                StringSQL = "select sabd_supervisor_name, (CONVERT(varchar(1),sabd_supervisor_status) + ',' + sabd_supervisor_branch + ',' + sabd_supervisor_concenter + ',' + CONVERT(varchar(5),sabd_supervisor_port)) + ',' + sabd_user_logged as SpecialQuery from sabd_supervisor_definition where sabd_supervisor_code='" & KeySearch & "'"
        End Select

        myCommand.CommandText = StringSQL
        myReader = myCommand.ExecuteReader(CommandBehavior.Default)
        While myReader.Read()
            Dato1 = myReader.GetValue(0).ToString
            Dato2 = myReader.GetValue(1).ToString
        End While
        ErrorCode = 0
        myReader.Close()

        If IsNothing(Dato1) Then
            Return 1
        End If

        Aux_001 = Dato1
        Aux_002 = Dato2

        Return ErrorCode

    End Function

    Public Function Get_Array_Info(ByVal StringSQL As String, ByRef ListData As List(Of String), ByVal mode As Byte) As Byte
        Dim ErrorCode As Byte = 1
        Dim StringLine As String = String.Empty

        myCommand.CommandText = StringSQL
        myReader = myCommand.ExecuteReader(CommandBehavior.Default)
        If myReader.HasRows Then
            Select Case mode
                Case 0
                    While myReader.Read()
                        StringLine = myReader.GetValue(0).ToString
                        ListData.Add(StringLine)
                    End While
                Case 1
                    While myReader.Read()
                        StringLine = myReader.GetValue(0).ToString & "|" & myReader.GetValue(1).ToString
                        ListData.Add(StringLine)
                    End While
            End Select
            ErrorCode = 0
        End If
        myReader.Close()

        Return ErrorCode

    End Function

    Public Function UpdateInfoMainData(ByVal ModuleName As String, ByRef ListData As List(Of String)) As Byte
        Dim ErrorCode As Byte = 1
        Dim ErrorMessage As String = String.Empty
        Dim StringSQL As String

        StringSQL = " update sabd_main_definition set sabd_module_id=" & ListData(0) & _
            ", sabd_type=" & ListData(1) & _
            ", sabd_detail_name='" & ListData(2) & _
            "', sabd_task=" & ListData(3) & _
            ", sabd_instance=" & ListData(4) & _
            ", sabd_timeout=" & ListData(5) & _
            ", sabd_ip_address='" & ListData(6) & _
            "', sabd_port_number=" & ListData(7) & _
            ", sabd_connection_mode=" & ListData(8) & _
            ", sabd_queue_requirement='" & ListData(9) & _
            "', sabd_queue_replies='" & ListData(10) & _
            "', sabd_queue_comms='" & ListData(11) & _
            "', sabd_queue_saf='" & ListData(12) & _
            "', sabd_queue_command='" & ListData(13) & _
            "', sabd_assign_router='" & ListData(14) & _
            "', sabd_msg_format=" & ListData(15) & _
            ", sabd_queue_ack='" & ListData(16) & "' " & _
            ", sabd_status=" & ListData(17) & _
            ", sabd_executable=" & ListData(18) & _
            ", sabd_source_path='" & ListData(19) & _
            "', sabd_target_path='" & ListData(20) & _
            "' where sabd_module_name='" & ModuleName & "'"

        ErrorCode = ExecuteSingleOrder(StringSQL, ErrorMessage)

        Return ErrorCode
    End Function


    Public Function UpdateInfoTranData(ByVal TransactionCode As String, ByRef ListData As List(Of String)) As Byte
        Dim ErrorCode As Byte = 1
        Dim ErrorMessage As String = String.Empty
        Dim StringSQL As String

        StringSQL = " update sabd_transaction_definition set sabd_transaction_code='" & ListData(0) & _
            "', sabd_transaction_name='" & ListData(1) & _
            "', sabd_transaction_type=" & ListData(2) & _
            ", sabd_message_code='" & ListData(3) & _
            "', sabd_transaction_bitmap='" & ListData(4) & "'  where sabd_transaction_Name='" & TransactionCode & "'"

        ErrorCode = ExecuteSingleOrder(StringSQL, ErrorMessage)

        Return ErrorCode
    End Function

    Public Function UpdateISOData(ByVal IsoField As String, ByRef ListData As List(Of String)) As Byte
        Dim ErrorCode As Byte = 1
        Dim ErrorMessage As String = String.Empty
        Dim StringSQL As String

        StringSQL = " update sabd_iso_definition set sabd_field_id=" & ListData(0) & _
            ", sabd_field_status=" & ListData(1) & _
            ", sabd_data_type=" & ListData(2) & _
            ", sabd_field_length=" & ListData(3) & _
            ", sabd_field_type=" & ListData(4) & _
            ", sabd_field_name='" & ListData(5) & "'  where sabd_field_id=" & IsoField

        ErrorCode = ExecuteSingleOrder(StringSQL, ErrorMessage)

        Return ErrorCode
    End Function

    Public Function UpdateInstitutionData(ByVal ListData As List(Of String)) As Byte
        Dim ErrorCode As Byte = 1
        Dim ErrorMessage As String = String.Empty
        Dim StringSQL As String

        StringSQL = " update sabd_institution_definition set sabd_codigo_autorizador=" & ListData(0) & _
            ", sabd_codigo_adquirente=" & ListData(1) & _
            ", sabd_codigo_aba='" & ListData(2) & _
            "', sabd_nombre='" & ListData(3) & _
            "', sabd_ciudad='" & ListData(4) & _
            "', sabd_direccion='" & ListData(5) & _
            "', sabd_telefono='" & ListData(6) & _
            "', sabd_ruc='" & ListData(7) & _
            "', sabd_categoria=" & ListData(8) & " where sabd_codigo_autorizador=" & ListData(0)

        ErrorCode = ExecuteSingleOrder(StringSQL, ErrorMessage)

        Return ErrorCode
    End Function

    Public Function UpdateServiceData(ByVal ServiceId As String, ByRef ListData As List(Of String)) As Byte
        Dim ErrorCode As Byte = 1
        Dim ErrorMessage As String = String.Empty
        Dim StringSQL As String

        StringSQL = "update sabd_service_definition set sabd_service_code='" & ListData(0) & _
            "', sabd_service_name='" & ListData(1) & "'  where sabd_service_code='" & ServiceId & "'"

        ErrorCode = ExecuteSingleOrder(StringSQL, ErrorMessage)

        Return ErrorCode
    End Function

    Public Function UpdateRoutingData(ByVal RoutingId As String, ByRef ListData As List(Of String)) As Byte
        Dim ErrorCode As Byte = 1
        Dim ErrorMessage As String = String.Empty
        Dim StringSQL As String

        StringSQL = " update sabd_routing_definition set sabd_routing_code='" & ListData(0) & _
            "', sabd_process_ID=" & ListData(1) & ", sabd_routing_name='" & ListData(2) & "'  where sabd_routing_code='" & RoutingId & "'"

        ErrorCode = ExecuteSingleOrder(StringSQL, ErrorMessage)

        Return ErrorCode
    End Function

    Public Function UpdateLocationData(ByVal LocationId As String, ByRef ListData As List(Of String)) As Byte
        Dim ErrorCode As Byte = 1
        Dim ErrorMessage As String = String.Empty
        Dim StringSQL As String

        StringSQL = " update sabd_location_definition set sabd_county_code=" & ListData(0) & _
                                                       ", sabd_county_name='" & ListData(1) & _
                                                       "', sabd_city_code=" & ListData(2) & _
                                                       ", sabd_city_name='" & ListData(3) & _
                                                       "', sabd_region=" & ListData(4) & _
                                                       "  where sabd_location_code='" & LocationId & "'"

        ErrorCode = ExecuteSingleOrder(StringSQL, ErrorMessage)

        Return ErrorCode
    End Function

    Public Function UpdateBranchData(ByVal BranchId As String, ByRef ListData As List(Of String)) As Byte
        Dim ErrorCode As Byte = 1
        Dim ErrorMessage As String = String.Empty
        Dim StringSQL As String

        StringSQL = " update sabd_branch_definition set sabd_branch_name='" & ListData(0) & _
            "', sabd_branch_institution=" & ListData(1) & ", sabd_branch_location='" & ListData(2) & "' where sabd_branch_code='" & BranchId & "'"

        ErrorCode = ExecuteSingleOrder(StringSQL, ErrorMessage)

        Return ErrorCode
    End Function

    Public Function UpdateTerminalData(ByVal TerminalId As String, ByRef ListData As List(Of String)) As Byte
        Dim ErrorCode As Byte = 1
        Dim ErrorMessage As String = String.Empty
        Dim StringSQL As String

        StringSQL = " update sabd_terminal_definition set sabd_terminal_name='" & ListData(0) & _
            "', sabd_terminal_branch='" & ListData(1) & "', sabd_terminal_concenter='" & ListData(2) & _
            "', sabd_terminal_port=" & ListData(3) & ", sabd_terminal_status=" & ListData(4) & _
            ", sabd_user_logged = ' ', sabd_terminal_supervisor='" & ListData(5) & "'  where sabd_terminal_code='" & TerminalId & "'"

        ErrorCode = ExecuteSingleOrder(StringSQL, ErrorMessage)

        Return ErrorCode
    End Function

    Public Function UpdateSupervisorData(ByVal SupervisorId As String, ByRef ListData As List(Of String)) As Byte
        Dim ErrorCode As Byte = 1
        Dim ErrorMessage As String = String.Empty
        Dim StringSQL As String

        StringSQL = " update sabd_Supervisor_definition set sabd_Supervisor_name='" & ListData(0) & _
            "', sabd_Supervisor_branch='" & ListData(1) & "', sabd_Supervisor_concenter='" & ListData(2) & _
            "', sabd_Supervisor_port=" & ListData(3) & ", sabd_Supervisor_status=" & ListData(4) & ", sabd_user_logged = ' '  where sabd_Supervisor_code='" & SupervisorId & "'"

        ErrorCode = ExecuteSingleOrder(StringSQL, ErrorMessage)

        Return ErrorCode
    End Function

    Public Function AddInfoMainData(ByVal ListData As List(Of String)) As Byte
        Dim ErrorCode As Byte = 1
        Dim ErrorMessage As String = String.Empty
        Dim StringSQL As String

        StringSQL = "insert into sabd_main_definition values('" & ListData(0) & _
            "', " & ListData(1) & _
            ", " & ListData(2) & _
            ", '" & ListData(3) & _
            "', " & ListData(4) & _
            ", " & ListData(5) & _
            ", " & ListData(6) & _
            ", '" & ListData(7) & _
            "', " & ListData(8) & _
            ", " & ListData(9) & _
            ", '" & ListData(10) & _
            "', '" & ListData(11) & _
            "', '" & ListData(12) & _
            "', '" & ListData(13) & _
            "', '" & ListData(14) & _
            "', '" & ListData(15) & _
            "', " & ListData(16) & _
            ", '" & ListData(17) & "'" & _
            ", " & ListData(18) & _
            ", " & ListData(19) & _
            ",'" & ListData(20) & _
            "', '" & ListData(21) & "')"

        ErrorCode = ExecuteSingleOrder(StringSQL, ErrorMessage)

        Return ErrorCode
    End Function


    Public Function AddInfoTranData(ByVal ListData As List(Of String)) As Byte
        Dim ErrorCode As Byte = 1
        Dim ErrorMessage As String = String.Empty
        Dim StringSQL As String

        StringSQL = "insert into sabd_transaction_definition values('" & ListData(0) & _
            "', '" & ListData(1) & _
            "', " & ListData(2) & _
            ", '" & ListData(3) & _
            "', '" & ListData(4) & "')"

        ErrorCode = ExecuteSingleOrder(StringSQL, ErrorMessage)
        If ErrorCode <> 0 Then
            MessageBox.Show(ErrorMessage, "Add info", MessageBoxButtons.OK)
        End If

        Return ErrorCode
    End Function

    Public Function AddInfoServiceData(ByVal ListData As List(Of String)) As Byte
        Dim ErrorCode As Byte = 1
        Dim ErrorMessage As String = String.Empty
        Dim StringSQL As String

        StringSQL = "insert into sabd_service_definition values('" & ListData(0) & _
            "', '" & ListData(1) & "')"

        ErrorCode = ExecuteSingleOrder(StringSQL, ErrorMessage)

        Return ErrorCode
    End Function

    Public Function AddRoutingData(ByVal ListData As List(Of String)) As Byte
        Dim ErrorCode As Byte = 1
        Dim ErrorMessage As String = String.Empty
        Dim StringSQL As String

        StringSQL = "insert into sabd_routing_definition values('" & ListData(0) & _
            "', " & ListData(1) & ", '" & ListData(2) & "')"

        ErrorCode = ExecuteSingleOrder(StringSQL, ErrorMessage)

        Return ErrorCode
    End Function


    Public Function AddInfoFieldData(ByVal ListData As List(Of String)) As Byte
        Dim ErrorCode As Byte = 1
        Dim ErrorMessage As String = String.Empty
        Dim StringSQL As String

        StringSQL = "insert into sabd_iso_definition values(" & ListData(0) & _
            ", " & ListData(1) & _
            ", " & ListData(2) & _
            ", " & ListData(3) & _
            ", " & ListData(4) & _
            ", '" & ListData(5) & "')"

        ErrorCode = ExecuteSingleOrder(StringSQL, ErrorMessage)
        If ErrorCode <> 0 Then
            MessageBox.Show(ErrorMessage, "Add info", MessageBoxButtons.OK)
        End If

        Return ErrorCode
    End Function

    Public Function AddInstitutionData(ByVal ListData As List(Of String)) As Byte
        Dim ErrorCode As Byte = 1
        Dim ErrorMessage As String = String.Empty
        Dim StringSQL As String

        StringSQL = "insert into sabd_institution_definition values(" & ListData(0) & _
            ", " & ListData(1) & _
            ", '" & ListData(2) & _
            "', '" & ListData(3) & _
            "', '" & ListData(4) & _
            "', '" & ListData(5) & _
            "', '" & ListData(7) & _
            "', '" & ListData(6) &
            "', " & ListData(8) & ")"

        ErrorCode = ExecuteSingleOrder(StringSQL, ErrorMessage)

        Return ErrorCode
    End Function

    Public Function AddInfoLocationData(ByVal ListData As List(Of String)) As Byte
        Dim ErrorCode As Byte = 1
        Dim ErrorMessage As String = String.Empty
        Dim StringSQL As String

        StringSQL = "insert into sabd_location_definition values('" & ListData(0) & _
             "', " & ListData(1) & _
            ", '" & ListData(2) & _
            "', " & ListData(3) & _
            ", '" & ListData(4) & _
            "'," & ListData(5) & ")"

        ErrorCode = ExecuteSingleOrder(StringSQL, ErrorMessage)

        Return ErrorCode
    End Function

    Public Function AddInfoBranchData(ByVal ListData As List(Of String)) As Byte
        Dim ErrorCode As Byte = 1
        Dim ErrorMessage As String = String.Empty
        Dim StringSQL As String

        StringSQL = "insert into sabd_branch_definition values('" & ListData(0) & _
             "', '" & ListData(1) & _
            "', " & ListData(2) & _
            ", '" & ListData(3) & "')"

        ErrorCode = ExecuteSingleOrder(StringSQL, ErrorMessage)

        Return ErrorCode
    End Function

    Public Function AddInfoTerminalData(ByVal ListData As List(Of String)) As Byte
        Dim ErrorCode As Byte = 1
        Dim ErrorMessage As String = String.Empty
        Dim StringSQL As String

        StringSQL = "insert into sabd_terminal_definition values('" & ListData(0) & _
             "', '" & ListData(1) & _
            "','" & ListData(2) & _
            "', '" & ListData(3) & _
            "', " & ListData(4) & _
            ", " & ListData(5) & _
            ", '" & ListData(6) & "')"

        ErrorCode = ExecuteSingleOrder(StringSQL, ErrorMessage)

        Return ErrorCode
    End Function

    Public Function AddInfoSupervisorData(ByVal ListData As List(Of String)) As Byte
        Dim ErrorCode As Byte = 1
        Dim ErrorMessage As String = String.Empty
        Dim StringSQL As String

        StringSQL = "insert into sabd_supervisor_definition values('" & ListData(0) & _
             "', '" & ListData(1) & _
            "','" & ListData(2) & _
            "', '" & ListData(3) & _
            "', " & ListData(4) & _
            ", " & ListData(5) & ", ' ' )"

        ErrorCode = ExecuteSingleOrder(StringSQL, ErrorMessage)

        Return ErrorCode
    End Function

    Public Function GetArrayData(ByVal TableName As String, ByVal FieldName As String, ByRef NamesData As List(Of String)) As Byte
        Dim ErrorCode As Byte = 1
        Dim idx As Byte
        Dim record As Int16

        myCommand.CommandText = " select count(*) from " & TableName
        myReader = myCommand.ExecuteReader(CommandBehavior.SingleRow)
        While myReader.Read()
            record = myReader.GetValue(0)
        End While
        myReader.Close()

        If record = 0 Then
            Return 1
        Else
            ErrorCode = 0
        End If

        myCommand.CommandText = " select " & FieldName & " from " & TableName
        myReader = myCommand.ExecuteReader(CommandBehavior.Default)
        idx = 0
        While myReader.Read()
            NamesData.Add(myReader.GetValue(0))
            idx += 1
        End While
        myReader.Close()

        Return ErrorCode
    End Function

    Public Function GetArrayData_Products(ByVal TableName As String, ByVal FieldName As String, ByRef NamesData As List(Of String)) As Byte
        Dim ErrorCode As Byte = 1
        Dim idx As Byte
        Dim record As Int16

        Dim L_Connection As New SqlConnection
        Dim L_Reader As SqlDataReader
        Dim L_Command As New SqlCommand
        Try
            L_Connection.ConnectionString = SetGetStringConnection(2)
            L_Connection.Open()
        Catch ex As Exception
            MsgBox("No se puede conectar a la Base:" & L_Connection.ConnectionString)
        End Try
        L_Command.Connection = L_Connection
        L_Command.CommandText = " select count(*) from " & TableName
        L_Reader = L_Command.ExecuteReader(CommandBehavior.SingleRow)
        While L_Reader.Read()
            record = L_Reader.GetValue(0)
        End While
        L_Reader.Close()

        If record = 0 Then
            Return 1
        Else
            ErrorCode = 0
        End If

        L_Command.CommandText = " select " & FieldName & " from " & TableName
        L_Reader = L_Command.ExecuteReader(CommandBehavior.Default)
        idx = 0
        While L_Reader.Read()
            NamesData.Add(L_Reader.GetValue(0).ToString.PadLeft(6, "0"))
            idx += 1
        End While
        L_Reader.Close()

        Return ErrorCode
    End Function


    Public Function GetArrayData(ByVal TableName As String, ByVal FieldName As String, ByRef NamesData As List(Of String), ByRef Complement As String) As Byte
        Dim ErrorCode As Byte = 1
        Dim idx As Byte
        Dim record As Int16

        myCommand.CommandText = " select count(*) from " & TableName
        myReader = myCommand.ExecuteReader(CommandBehavior.SingleRow)
        While myReader.Read()
            record = myReader.GetValue(0)
        End While
        myReader.Close()

        If record = 0 Then
            Return 1
        Else
            ErrorCode = 0
        End If

        myCommand.CommandText = " select " & FieldName & " from " & TableName & Complement
        myReader = myCommand.ExecuteReader(CommandBehavior.Default)
        idx = 0
        While myReader.Read()
            NamesData.Add(myReader.GetValue(0))
            idx += 1
        End While
        myReader.Close()

        Return ErrorCode
    End Function


    Public Function ExecuteSingleOrder(ByRef LineCommand As String, ByRef ErrorMessage As String) As Byte
        Dim ErrorCode As String = 1
        Dim RowsEffected As Integer

        myCommand.CommandText = LineCommand

        Try
            RowsEffected = myCommand.ExecuteNonQuery()
            If RowsEffected > 0 Then
                MessageBox.Show("Proceso exitoso," & RowsEffected & " registros afectados", "Show Info", MessageBoxButtons.OK)
                ErrorCode = 0
            End If
        Catch ex As SqlException
            ErrorMessage = " Error: " & ex.ErrorCode.ToString & ex.Message & " -> " & ex.StackTrace
            ErrorCode = 1
        Catch ex As Exception
            ErrorMessage = " Error: " & ex.Message & " -> " & ex.StackTrace
            ErrorCode = 1
        End Try

        If ErrorCode <> 0 Then
            MessageBox.Show(ErrorMessage, "Show Info", MessageBoxButtons.OK)
        End If
        Return ErrorCode

    End Function


    '    Public Sub PutTextFileQueue(ByVal TextMessage As String)

    '        Try
    '            TextQueue.Enqueue(TextMessage)
    '            TextEvent.Set()
    '        Catch ex As Exception
    '            MsgBox("Error en rutina de mensajes de texto")
    '        End Try

    '    End Sub

    '    Private Sub DequeueTextMessage(ByVal Id As Byte)
    '        Dim TextMessage As String

    'Looping:
    '        TextEvent.WaitOne()
    '        Do While TextQueue.Count > 0
    '            TextMessage = TextQueue.Dequeue
    '            SaveTextMessage(TextMessage)
    '        Loop
    '        GoTo Looping

    '    End Sub


    'Private Sub SaveTextMessage(ByVal TextMessage As String)
    '    Dim PathDirectory As String = Application.StartupPath & "\LOGS"
    '    Dim PathFile As String = PathDirectory & "\log" & Now.Year & Format(Now.Month, "00") & Format(Now.Day, "00") & ".txt"

    '    Try
    '        If Not Directory.Exists(PathDirectory) Then
    '            Directory.CreateDirectory(PathDirectory)
    '        End If

    '        Dim path As String = PathFile
    '        Dim fi As FileInfo = New FileInfo(path)

    '        Dim objWriter As New System.IO.StreamWriter(path, True)
    '        objWriter.WriteLine(TextMessage)
    '        objWriter.Flush()
    '        objWriter.Close()
    '    Catch ex As Exception
    '        MsgBox("Error al grabar archivo " & ex.Message)
    '    End Try

    'End Sub

    Public Function GetNextIDValue(ByVal ModuleGroup As Byte, ByRef ModuleId As Int16) As Byte
        Dim ErrorCode As Byte = 1

        Try
            myCommand.CommandText = "select MAX(sabd_module_id) from sabd_main_definition where sabd_type=" & ModuleGroup
            myReader = myCommand.ExecuteReader(CommandBehavior.SingleRow)
            If myReader.HasRows Then
                While myReader.Read()
                    If IsDBNull(myReader.GetValue(0)) Then
                        ModuleId += 1
                    Else
                        ModuleId = myReader.GetValue(0)
                        ModuleId += 1
                    End If
                End While
            End If
            ErrorCode = 0
        Catch ex As Exception
            ErrorCode = 1
        End Try
        myReader.Close()

        Return ErrorCode

    End Function

    Public Function GetNextCityValue(ByVal CountyID As Int16, ByRef CityId As Int16) As Byte
        Dim ErrorCode As Byte = 1

        Try
            myCommand.CommandText = "select MAX(sabd_city_code) from sabd_location_definition where sabd_county_code=" & CountyID
            myReader = myCommand.ExecuteReader(CommandBehavior.SingleRow)
            If myReader.HasRows Then
                While myReader.Read()
                    If IsDBNull(myReader.GetValue(0)) Then
                        CityId += 1
                    Else
                        CityId = myReader.GetValue(0)
                        CityId += 1
                    End If
                End While
            End If
            ErrorCode = 0
        Catch ex As Exception
            ErrorCode = 1
        End Try
        myReader.Close()

        Return ErrorCode

    End Function

    Public Function Get_User_ArrayList(ByRef Cbb_Users As ComboBox) As Byte
        Dim ErrorCode As Byte = 1
        myCommand.CommandText = "select dm_UserName from SABD_User_Definition"
        myReader = myCommand.ExecuteReader(CommandBehavior.Default)
        If myReader.HasRows Then
            While myReader.Read()
                Cbb_Users.Items.Add(myReader.GetValue(0))
            End While
            ErrorCode = 0
        End If
        myReader.Close()
        Cbb_Users.SelectedIndex = 0

        Return ErrorCode
    End Function

    Public Function Get_User_Record(ByVal PassUserName As String, ByRef LocalUserRecord As List(Of String)) As Byte
        Dim ErrorCode As Byte = 1
        myCommand.CommandText = "select * from SABD_User_Definition where dm_UserName='" & PassUserName & "'"
        myReader = myCommand.ExecuteReader(CommandBehavior.Default)
        If myReader.HasRows Then
            While myReader.Read()
                'LocalUserRecord.Add(myReader.GetValue(User_Name))
                'LocalUserRecord.Add(myReader.GetValue(User_Id))
                'LocalUserRecord.Add(myReader.GetValue(User_Chargue))
                'LocalUserRecord.Add(myReader.GetValue(User_SchedId))
                'LocalUserRecord.Add(myReader.GetValue(User_Password).ToString.TrimEnd)
                'LocalUserRecord.Add(myReader.GetValue(User_FirstName))
                'LocalUserRecord.Add(myReader.GetValue(User_LastName))
                'LocalUserRecord.Add(myReader.GetValue(User_State))
                'LocalUserRecord.Add(myReader.GetValue(User_Privileged))
                'LocalUserRecord.Add(myReader.GetValue(User_Area))
                'LocalUserRecord.Add(myReader.GetValue(User_Document))
                'LocalUserRecord.Add(myReader.GetValue(User_Mobile))
                'LocalUserRecord.Add(myReader.GetValue(User_Conventional))
                'LocalUserRecord.Add(myReader.GetValue(User_Email))
                'LocalUserRecord.Add(myReader.GetValue(User_Logged))
                'LocalUserRecord.Add(myReader.GetValue(User_LastLogged))
                'LocalUserRecord.Add(myReader.GetValue(User_PassChange))
                'LocalUserRecord.Add(myReader.GetValue(User_Machine))
                'LocalUserRecord.Add(myReader.GetValue(User_DateAdd))
                'LocalUserRecord.Add(myReader.GetValue(User_AddBy))
                'LocalUserRecord.Add(myReader.GetValue(User_DatePass))
                'LocalUserRecord.Add(myReader.GetValue(User_PassBy))
                'LocalUserRecord.Add(myReader.GetValue(User_DateUpd))
                'LocalUserRecord.Add(myReader.GetValue(User_UpdBy))
                'LocalUserRecord.Add(myReader.GetValue(User_Group))
            End While
            ErrorCode = 0
        End If
        myReader.Close()

        Return ErrorCode
    End Function


    Public Function Get_Reference_ArrayList(ByVal TableName As String, ByRef LocalReferencesRecord As List(Of String), ByVal ComplementQuery As String) As Byte
        Dim ErrorCode As Byte = 1
        myCommand.CommandText = "select * from " & TableName & ComplementQuery
        myReader = myCommand.ExecuteReader(CommandBehavior.Default)
        If myReader.HasRows Then
            While myReader.Read()
                LocalReferencesRecord.Add(myReader.GetValue(0) & "|" & myReader.GetValue(1))
            End While
            ErrorCode = 0
        End If
        myReader.Close()

        Return ErrorCode
    End Function

    Public Sub Function_Notify_Error(ByVal ErrorCode As Int16, ByRef ShortDescriptor As String, ByRef LongDescriptor As String)
        ShortDescriptor = "Error no definido"
        LongDescriptor = "El error utilizado no ha sido definido en el sistema"
        Dim Level As Int16 = 2

        myCommand.CommandText = " select dm_ErrorShortDesc, dm_ErrorLongDesc, dm_ErrorLevel from dm_Error where dm_ErrorCode =" & ErrorCode
        myReader = myCommand.ExecuteReader(CommandBehavior.SingleRow)
        If myReader.HasRows Then
            While myReader.Read()
                ShortDescriptor = myReader.GetValue(0)
                LongDescriptor = myReader.GetValue(1)
                Level = myReader.GetValue(2)
            End While
            myReader.Close()
        End If
    End Sub

    Public Function Refresh_Fields_PassChangue(ByVal UserName As String, ByVal PassWord As String, ByVal UserChange As String, ByVal ChangePass As Char) As Byte
        Dim SqlCommand As String = String.Empty

        SqlCommand = "update SABD_User_Definition set dm_UserPassChange ='" & ChangePass & "', dm_UserPassword='" & PassWord & "', dm_UserPassBy='" & UserChange & "', dm_UserDatePass='" & Now & "' where dm_UserName='" & UserName & "'"

        'Return ExecuteSingleOrder(SqlCommand, UPD_RECORD)
        Return ExecuteSingleOrder(SqlCommand)

    End Function

    Public Function ExecuteSingleOrder(ByRef LineCommand As String) As Byte
        Dim ErrorCode As Byte = 1

        myCommand.CommandText = LineCommand
        Try
            myCommand.ExecuteNonQuery()
            ErrorCode = SUCCESSFUL
            'Function_Notify_Error(SUCCESSFUL_PROCESS)
        Catch ex As SqlException
            'SetGetTextComplement = ex.Message
            'Function_Notify_Error(ERROR_GENERIC)
            ErrorCode = 1
        Catch ex As Exception
            'SetGetTextComplement = ex.Message
            'Function_Notify_Error(ERROR_GENERIC)
            ErrorCode = 1
        End Try
        Return ErrorCode

    End Function

    Public Function Function_Validate_User(ByVal User As String, ByRef Pass As String, ByRef FName As String, ByRef LName As String, ByRef State As Int16, ByRef PassChangue As Char, ByRef UserType As Byte) As Byte
        Dim ErrorCode As Byte = 0
        Dim Logged As Int16
        Dim LoggedComplement As String = String.Empty

        Try
            myCommand.CommandText = " select dm_UserPassword, dm_UserState, dm_UserFirstName, dm_UserLastName, dm_UserLogged, dm_UserLastLogged, dm_UserMachine, dm_UserPassChange, dm_UserGroup from SABD_User_Definition where dm_UserName ='" & User & "'"
            myReader = myCommand.ExecuteReader(CommandBehavior.SingleRow)
            If myReader.HasRows Then
                While myReader.Read()
                    Pass = myReader.GetValue(0).trim
                    State = myReader.GetValue(1)
                    FName = myReader.GetValue(2)
                    LName = myReader.GetValue(3)
                    Logged = myReader.GetValue(4)
                    LoggedComplement = ", Hora (" & myReader.GetValue(5) & ") desde " & myReader.GetValue(6)
                    PassChangue = myReader.GetValue(7)
                    UserType = myReader.GetValue(8)
                End While
            Else
                myReader.Close()
                Return (ERROR_UNKNOW_USER)
            End If
        Catch ex As Exception
            MsgBox("Excepcion no controlada:" & ex.Message)
        End Try

        myReader.Close()

        Return ErrorCode
    End Function

    Public Function AskingMe(ByVal FunctionOrder As String, ByVal Reference As String) As Byte
        Dim ErrorCode As Byte
        Dim ASK As DialogResult

        ASK = MessageBox.Show(" Esta seguro de " & FunctionOrder.ToLower & " el registro (" & Reference & ") seleccionado ?", FunctionOrder, MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If ASK = Windows.Forms.DialogResult.Yes Then
            ErrorCode = 0
        Else
            ErrorCode = 1
        End If

        Return ErrorCode
    End Function

    Public Function Get_Process_UP(ByRef ParamsTable As List(Of String)) As Byte
        Dim ErrorCode As Byte = 1
        Dim Status As Byte = 1
        Dim myReader As SqlDataReader
        Dim SqlQuery As String = "select  sabd_module_name from sabd_main_definition where sabd_status=@p1 order by sabd_module_name"

        Try
            Dim myCommand As New SqlCommand
            myCommand.CommandText = SqlQuery
            myCommand.Parameters.Add("@p1", System.Data.SqlDbType.VarChar)
            myCommand.Parameters("@p1").Value = Status
            myCommand.Connection = myConnection
            myReader = myCommand.ExecuteReader(CommandBehavior.Default)
            If myReader.HasRows Then
                While myReader.Read()
                    ParamsTable.Add(myReader.GetValue(0).ToString.Trim)
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

        GC.Collect()

        Return ErrorCode
    End Function

    Public Function DB_GetArrayData(ByVal TableName As String, ByVal FieldName As String, ByRef NamesData As List(Of String), ByRef Complement As String) As Byte
        Dim ErrorCode As Byte = 1

        Dim myCommand As New SqlCommand
        Dim myReader As SqlDataReader
        myCommand.Connection = myConnection

        myCommand.CommandText = " select " & FieldName & " from " & TableName & Complement
        myReader = myCommand.ExecuteReader(CommandBehavior.Default)
        While myReader.Read()
            NamesData.Add(myReader.GetValue(0))
        End While

        myReader.Close()
        myReader = Nothing
        myCommand.Dispose()
        myCommand = Nothing

        Return ErrorCode
    End Function


    Public Function DB_Get_Adq_Aut(ByVal ProcName As String, ByVal Mode As Byte) As Int16
        Dim FI As Int16 = 0

        Dim myCommand As New SqlCommand
        Dim myReader As SqlDataReader
        myCommand.Connection = myConnection

        Dim sMode As String = ""
        Select Case Mode
            Case 0
                sMode = "sabd_codigo_autorizador"
            Case 1
                sMode = "sabd_codigo_adquirente"
        End Select

        myCommand.CommandText = " select " & sMode & " from SABD_INSTITUTION_DEFINITION where sabd_nombre='" & ProcName & "'"
        myReader = myCommand.ExecuteReader(CommandBehavior.SingleResult)
        While myReader.Read()
            FI = myReader.GetValue(0)
        End While

        myReader.Close()
        myReader = Nothing
        myCommand.Dispose()
        myCommand = Nothing

        Return FI
    End Function

End Module
