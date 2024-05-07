Public Class SWTM_Routing

    Dim G_Routing As String
    Dim OriginKeyRouting As String

    Private Sub SABD_routing_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        Load_Primary_Info()

    End Sub

    Private Sub Load_Primary_Info()

        Load_Combo_Data(Me.ComboBox2, "sabd_service_definition", "sabd_service_code")
        Load_Combo_Data(Me.ComboBox3, "sabd_transaction_definition", "sabd_transaction_code")
        Load_Combo_Data(Me.ComboBox4, "sabd_institution_definition", "sabd_codigo_autorizador")
        Load_Combo_Data(Me.ComboBox5, "sabd_main_definition", "sabd_module_name")
        Fill_Data_OnFields()
        Disable_Enable_Combos(False)

    End Sub


    Private Sub Fill_Data_OnFields()
        Dim NamesData As New List(Of String)

        Me.ComboBox1.Items.Clear()
        GetArrayData("sabd_routing_definition", "sabd_routing_code", NamesData)

        If NamesData.Count = 0 Then
            Show_Combo_Values(-1)
            Exit Sub
        End If

        For x = 0 To NamesData.Count - 1
            Me.ComboBox1.Items.Add(NamesData(x))
        Next
        Me.ComboBox1.SelectedIndex = 0

        'Show_Retrieved_Data(NamesData(0))

    End Sub

    Private Sub Disable_Enable_Combos(ByVal Mode As Boolean)

        Me.ComboBox2.Enabled = Mode
        Me.ComboBox3.Enabled = Mode
        Me.ComboBox4.Enabled = Mode
        Me.ComboBox5.Enabled = Mode
        Me.TextBox1.Enabled = Mode

    End Sub

    Private Sub Show_Combo_Values(ByVal Value As Int16)

        Try
            Me.ComboBox1.SelectedIndex = Value
        Catch ex As Exception

        End Try

        Me.ComboBox2.SelectedIndex = Value
        Me.ComboBox3.SelectedIndex = Value
        Me.ComboBox4.SelectedIndex = Value
        Me.ComboBox5.SelectedIndex = Value
        Me.TextBox1.Text = ""

    End Sub

    Private Sub Load_Combo_Data(ByVal ComboName As ComboBox, ByVal TableName As String, ByVal FieldName As String)

        Dim NamesData As New List(Of String)

        ComboName.Items.Clear()
        GetArrayData(TableName, FieldName, NamesData)

        If NamesData.Count = 0 Then
            Exit Sub
        End If

        For x = 0 To NamesData.Count - 1
            ComboName.Items.Add(NamesData(x).PadLeft(4, "0"))
        Next
        ComboName.SelectedIndex = 0

        If NamesData.Count = 0 Then
            MsgBox("No existen datos en la tabla principal")
            Exit Sub
        End If
    End Sub


    Private Sub Load_Combo_Data_Products(ByVal ComboName As ComboBox, ByVal TableName As String, ByVal FieldName As String)

        Dim NamesData As New List(Of String)

        ComboName.Items.Clear()
        GetArrayData_Products(TableName, FieldName, NamesData)

        If NamesData.Count = 0 Then
            Exit Sub
        End If

        For x = 0 To NamesData.Count - 1
            ComboName.Items.Add(NamesData(x).PadLeft(4, "0"))
        Next
        ComboName.SelectedIndex = 0

        If NamesData.Count = 0 Then
            MsgBox("No existen datos en la tabla principal")
            Exit Sub
        End If
    End Sub

    Private Sub ComboBox2_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles ComboBox2.SelectedIndexChanged
        Dim ArrayData As New List(Of String)

        Try
            If GetExplicitData("select sabd_service_name from sabd_service_definition where sabd_service_code ='" & Me.ComboBox2.SelectedItem.ToString & "'", ArrayData) = 0 Then
                Me.Label2.Text = ArrayData(0)
            End If
            Build_Routing_Code()
        Catch ex As Exception
            Me.Label2.Text = ""
        End Try

    End Sub

    Private Sub ComboBox3_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles ComboBox3.SelectedIndexChanged
        Dim ArrayData As New List(Of String)

        Try
            If GetExplicitData("select sabd_transaction_name from sabd_transaction_definition where sabd_transaction_code ='" & Me.ComboBox3.SelectedItem.ToString & "'", ArrayData) = 0 Then
                Me.Label3.Text = ArrayData(0)
            End If
            Build_Routing_Code()
        Catch ex As Exception
            Me.Label3.Text = ""
        End Try

    End Sub

    Private Sub ComboBox4_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles ComboBox4.SelectedIndexChanged
        Dim ArrayData As New List(Of String)

        Try
            If GetExplicitData("select sabd_nombre from sabd_institution_definition where sabd_codigo_autorizador ='" & Me.ComboBox4.SelectedItem.ToString & "'", ArrayData) = 0 Then
                Me.Label4.Text = ArrayData(0)
            End If
            Build_Routing_Code()
        Catch ex As Exception
            Me.Label4.Text = ""
        End Try

    End Sub

    Private Sub ComboBox5_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles ComboBox5.SelectedIndexChanged
        Dim ArrayData As New List(Of String)

        Try
            If GetExplicitData("select sabd_module_id from sabd_main_definition where sabd_module_name ='" & Me.ComboBox5.SelectedItem.ToString & "'", ArrayData) = 0 Then
                Me.lbl_ModuleID.Text = ArrayData(0)
            End If
        Catch ex As Exception
            Me.lbl_ModuleID.Text = ""
        End Try

    End Sub

    Private Sub Build_Routing_Code()

        Try
            G_Routing = Me.ComboBox2.SelectedItem.ToString & Me.ComboBox3.SelectedItem.ToString & Me.ComboBox4.SelectedItem.ToString
            Me.Label6.Text = G_Routing
        Catch ex As Exception

        End Try

    End Sub

    Private Sub Button3_Click(sender As System.Object, e As System.EventArgs) Handles Button3.Click

        If Me.Button3.Text = "NUEVO" Then
            Me.Button3.Text = "ACEPTAR"
            Me.Button2.Enabled = False
            Me.Button4.Enabled = False
            Me.Button1.Enabled = True
            Disable_Enable_Combos(True)
            Show_Combo_Values(0)
            Me.Label6.Show()
            Me.ComboBox1.Hide()
        ElseIf Me.Button3.Text = "ACEPTAR" Then
            If Get_Info_Key(Me.Label6.Text, "", "", 5) = 1 Then
                If Process_Add_Record() = 0 Then
                    Me.Button3.Text = "NUEVO"
                    Me.Button2.Enabled = True
                    Me.Button4.Enabled = True
                    Me.Button1.Enabled = False
                    Disable_Enable_Combos(False)
                    Fill_Data_OnFields()
                    Me.Label6.Hide()
                    Me.ComboBox1.Show()
                    Me.ComboBox1.Focus()
                    MessageBox.Show("Proceso exitoso !!")
                End If
            Else
                MessageBox.Show("Ya existe una definicion con este valor (" & Me.Label6.Text & ")", "Create record", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        End If
    End Sub

    Private Function Process_Add_Record() As Byte
        If Me.TextBox1.TextLength = 0 Then
            MsgBox("Campo requerido debe ser ingresado", MsgBoxStyle.Information, "Routing")
            Me.TextBox1.Focus()
            Return 1
        End If

        Dim ListData As New List(Of String)
        ListData.Add(Label6.Text)
        ListData.Add(lbl_ModuleID.Text)
        ListData.Add(TextBox1.Text)

        Return AddRoutingData(ListData)

    End Function


    Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles Button1.Click

        Me.Button2.Enabled = True
        Me.Button3.Enabled = True
        Me.Button4.Enabled = True
        Me.Button3.Text = "NUEVO"
        Me.Button2.Text = "MODIFICAR"
        Me.Button1.Enabled = False
        Disable_Enable_Combos(False)
        Fill_Data_OnFields()
        Me.ComboBox1.Show()
        Me.ComboBox1.Focus()

    End Sub

    Private Sub Button2_Click(sender As System.Object, e As System.EventArgs) Handles Button2.Click

        If Me.Button2.Text = "MODIFICAR" Then
            Me.Button2.Text = "ACEPTAR"
            Me.Button3.Enabled = False
            Me.Button4.Enabled = False
            Me.Button1.Enabled = True
            Me.ComboBox1.Hide()
            Disable_Enable_Combos(True)
        ElseIf Me.Button2.Text = "ACEPTAR" Then
            If AskingMe(" modificar ", Me.Label6.Text) <> 0 Then
                Exit Sub
            End If
            Me.Button2.Text = "MODIFICAR"
            Me.Button3.Enabled = True
            Me.Button4.Enabled = True
            Me.Button1.Enabled = False
            Me.ComboBox1.Show()
            Disable_Enable_Combos(False)
            Process_Update_Record()
            Me.ComboBox1.Focus()
        End If

    End Sub

    Private Sub Process_Update_Record()
        Dim ListData As New List(Of String)

        ListData.Add(Me.Label6.Text)
        ListData.Add(Me.lbl_ModuleID.Text)
        ListData.Add(Me.TextBox1.Text)
        If UpdateRoutingData(OriginKeyRouting, ListData) = 0 Then
            Load_Primary_Info()
        End If

    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles ComboBox1.SelectedIndexChanged

        Show_Retrieved_Data(Me.ComboBox1.SelectedItem.ToString)

    End Sub

    Private Sub Show_Retrieved_Data(ByVal ModuleName As String)
        Dim ListData As New List(Of String)
        OriginKeyRouting = ModuleName

        If GetInfoRoutingData(ModuleName, ListData) <> 0 Then
            Exit Sub
        End If

        If ListData.Count = 0 Then
            Exit Sub
        End If

        Dim AuxTmp As Int32
        AuxTmp = ListData(0).ToString.Substring(0, 10)
        For x = 0 To Me.ComboBox2.Items.Count - 1
            If CInt(Me.ComboBox2.Items(x).ToString.Substring(0, 10)) = AuxTmp Then
                Me.ComboBox2.SelectedIndex = x
            End If
        Next

        AuxTmp = ListData(0).ToString.Substring(10, 6)
        For x = 0 To Me.ComboBox3.Items.Count - 1
            If CInt(Me.ComboBox3.Items(x).ToString) = AuxTmp Then
                Me.ComboBox3.SelectedIndex = x
            End If
        Next

        AuxTmp = ListData(0).ToString.Substring(16, 4)
        For x = 0 To Me.ComboBox4.Items.Count - 1
            If CInt(Me.ComboBox4.Items(x).ToString) = AuxTmp Then
                Me.ComboBox4.SelectedIndex = x
            End If
        Next

        AuxTmp = CInt(ListData(1))
        Me.TextBox1.Text = ListData(2)
        ListData.Clear()
        GetArrayData("sabd_main_definition", "sabd_module_id", ListData)
        For x = 0 To Me.ComboBox5.Items.Count - 1
            If CInt(ListData(x)) = AuxTmp Then
                Me.ComboBox5.SelectedIndex = x
            End If
        Next

    End Sub

    Private Sub Button4_Click(sender As System.Object, e As System.EventArgs) Handles Button4.Click
        If AskingMe(" Eliminar", Me.ComboBox1.SelectedItem) = 1 Then
            Exit Sub
        End If

        Dim ErrorCode As Byte = 1
        Dim ErrorMessage As String = String.Empty
        Dim StringSQL As String = "delete from sabd_routing_definition where sabd_routing_code ='" & Me.ComboBox1.SelectedItem & "'"

        ErrorCode = ExecuteSingleOrder(StringSQL, ErrorMessage)
        If ErrorCode <> 0 Then
            MessageBox.Show(ErrorMessage, "Delete record", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Else
            Fill_Data_OnFields()
        End If

    End Sub

    Private Sub TextBox1_LostFocus(sender As Object, e As System.EventArgs) Handles TextBox1.LostFocus

        Me.TextBox1.Text = Me.TextBox1.Text.ToString.ToUpper

    End Sub


End Class