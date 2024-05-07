Imports System.Data.SqlClient

Module ServiSwitch_SQLdb

    Dim Security As New EncodeDecodeFunctions

    Dim MyStringConnection As String


    Public Function Init_Security_Resources() As Byte
        Dim ErrorCode As Byte = FUNCTION_ERROR
        Try
            '****  2017-04-28 LLNO
            Security.Load_Config_Keys()
            SetGetStringConnection = Security.SetgetStringConnection()
            '****  2017-04-28 LLNO
            'SetGetStringConnection = "Server=DESKTOP-23NB7P9\SQLEXPRESS;Database=SERVISWITCH;User Id=sa;Password=Lider1973;"
            '****  2017-04-28 LLNO
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
            ErrorCode = 0
            If Not IsNothing(myReader) Then
                myReader.Close()
            End If
        Catch ex As SqlException
            ErrorCode = 1
        End Try

        Close_SQL_Connection(myConnection)
        GC.Collect()

        Return ErrorCode
    End Function

End Module
