Public Class SWTM_Institution
    Dim txtVal1 As String
    Dim txtVal2 As String
    Dim txtVal3 As String
    Dim txtVal4 As String
    Dim txtVal5 As String
    Dim txtVal6 As String
    Dim txtVal7 As String
    Dim txtVal8 As String

    Private Sub SABD_Institution_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        Me.ComboBox1.TabIndex = 0
        Me.txtb1.TabIndex = 1
        Me.ComboBox1.Focus()
        Me.ComboBox1.Select()

        Me.Button1.Enabled = False
        Me.Button2.Enabled = True
        Me.Button3.Enabled = True
        Me.Button4.Enabled = True
        Init_Control_Form()
        Fill_Data_OnFields()
        Show_Control_Mode(False)

    End Sub

    Private Sub Fill_Data_OnFields()
        Dim NamesData As New List(Of String)

        Me.ComboBox1.Items.Clear()
        GetArrayData("sabd_institution_definition", "sabd_codigo_autorizador", NamesData)

        If NamesData.Count = 0 Then
            Init_Control_Form()
            Exit Sub
        End If

        For x = 0 To NamesData.Count - 1
            Me.ComboBox1.Items.Add(NamesData(x).PadLeft(4, "0"))
        Next
        Me.ComboBox1.SelectedIndex = 0

        If NamesData.Count = 0 Then
            MsgBox("No existen datos en la tabla principal")
            Exit Sub
        End If

        Show_Retrieved_Data(NamesData(0))

    End Sub

    Private Sub Show_Retrieved_Data(ByVal InstitutionCode As Int32)
        Dim ListData As New List(Of String)

        If GetInfoInstitutionData(InstitutionCode, ListData) <> 0 Then
            Exit Sub
        End If

        Me.txtb2.Text = ListData(0)
        Me.txtb3.Text = ListData(1)
        Me.txtb4.Text = ListData(2)
        Me.txtb5.Text = ListData(3)
        Me.txtb6.Text = ListData(4)
        Me.txtb7.Text = ListData(5)
        Me.txtb8.Text = ListData(6)
        Me.ComboBox2.SelectedIndex = CInt(ListData(7))

        'Dim x As Byte

        'For x = 0 To Me.ComboBox1.Items.Count - 1
        '    If Me.ComboBox1.Items(x).ToString = ListData(1).Trim Then
        '        Me.ComboBox1.SelectedIndex = x
        '        Exit For
        '    End If
        'Next

        'For x = 0 To Me.ComboBox2.Items.Count - 1
        '    If Me.ComboBox2.Items(x).ToString = ListData(2).Trim Then
        '        Me.ComboBox2.SelectedIndex = x
        '        Exit For
        '    End If
        'Next

        'Me.TextBox6.Text = ListData(3)
        'Me.TextBox2.Text = ListData(3).Substring(0, 16)
        'Me.TextBox5.Text = ListData(3).Substring(16)

        'Dim bitmap1 As String = TextBox2.Text.Replace(" ", "")
        'Dim bitmap2 As String = TextBox5.Text.Replace(" ", "")
        'Dim Temp As String = String.Empty
        'Dim y As Byte
        'GenerateBitMapsChar(bitmap1, bitmap2)

        'bitmap1 = bitmap1 & bitmap2
        'Dim HexBitMap1() As Char = bitmap1.ToCharArray
        'For x = 0 To 127
        '    If HexBitMap1(x) = "1" Then
        '        Temp = Temp & CStr(x + 1) & ", "
        '        If x <= 63 Then
        '            Me.CheckedListBox1.SetItemChecked(x, True)
        '        End If
        '        If x > 63 Then
        '            y = x - 64
        '            Me.CheckedListBox2.SetItemChecked(y, True)
        '        End If
        '    End If
        'Next

    End Sub


    Private Sub Init_Control_Form()

        Me.txtb1.Text = ""
        Me.txtb2.Text = ""
        Me.txtb3.Text = ""
        Me.txtb4.Text = ""
        Me.txtb5.Text = ""
        Me.txtb6.Text = ""
        Me.txtb7.Text = ""
        Me.txtb8.Text = ""
        Me.ComboBox2.SelectedIndex = 0

    End Sub

    Private Sub Show_Control_Mode(ByVal Mode As Boolean)

        Me.txtb1.Enabled = Mode
        Me.txtb2.Enabled = Mode
        Me.txtb3.Enabled = Mode
        Me.txtb4.Enabled = Mode
        Me.txtb5.Enabled = Mode
        Me.txtb6.Enabled = Mode
        Me.txtb7.Enabled = Mode
        Me.txtb8.Enabled = Mode
        Me.ComboBox1.Visible = Not Mode
        Me.ComboBox2.Enabled = Mode
        Me.txtb1.Visible = Mode

    End Sub

    Private Sub Button3_Click(sender As System.Object, e As System.EventArgs) Handles Button3.Click

        If Me.Button3.Text = "NUEVO" Then
            Init_Control_Form()
            Show_Control_Mode(True)
            Me.Button3.Text = "ACEPTAR"
            Me.Button2.Enabled = False
            Me.Button4.Enabled = False
            Me.ComboBox1.Hide()
            Me.Button1.Enabled = True
            Me.txtb1.Text = "0000"
            Me.txtb2.Text = "0000"
            Me.txtb3.Text = "0000000000"
            Me.txtb7.Text = "0000000000000"
            Me.txtb8.Text = "000000000000000"
        ElseIf Me.Button3.Text = "ACEPTAR" Then
            If Process_Add_Record() = 0 Then
                Me.Button3.Text = "NUEVO"
                Me.Button2.Enabled = True
                Me.Button4.Enabled = True
                Me.ComboBox1.Show()
                Me.Button1.Enabled = False
                Show_Control_Mode(False)
                Fill_Data_OnFields()
                MessageBox.Show("Proceso exitoso !!")
            End If
        End If

    End Sub

    Private Function Process_Add_Record() As Byte
        Dim ListData As New List(Of String)
        '*************************************************************
        If Me.txtb4.TextLength = 0 Then
            Me.ErrorProvider1.SetError(Me.txtb4, "Valor no es valido")
            Me.txtb4.Focus()
            Return 1
        Else
            Me.ErrorProvider1.SetError(Me.txtb4, "")
        End If
        '*************************************************************
        If Me.txtb5.TextLength = 0 Then
            Me.ErrorProvider1.SetError(Me.txtb5, "Valor no es valido")
            Me.txtb5.Focus()
            Return 1
        Else
            Me.ErrorProvider1.SetError(Me.txtb5, "")
        End If
        '*************************************************************
        If Me.txtb6.TextLength = 0 Then
            Me.ErrorProvider1.SetError(Me.txtb6, "Valor no es valido")
            Me.txtb6.Focus()
            Return 1
        Else
            Me.ErrorProvider1.SetError(Me.txtb6, "")
        End If
        '*************************************************************
        ListData.Add(txtb1.Text)
        ListData.Add(txtb2.Text)
        ListData.Add(txtb3.Text)
        ListData.Add(txtb4.Text)
        ListData.Add(txtb5.Text)
        ListData.Add(txtb6.Text)
        ListData.Add(txtb7.Text)
        ListData.Add(txtb8.Text)
        ListData.Add(Me.ComboBox2.SelectedIndex)
        Return AddInstitutionData(ListData)

    End Function

    Private Sub txtb1_KeyPress(sender As Object, e As System.Windows.Forms.KeyPressEventArgs) Handles txtb1.KeyPress

        If (Not e.KeyChar = ChrW(Keys.Back) And ("0123456789").IndexOf(e.KeyChar) = -1) Then
            e.Handled = True
        End If

    End Sub

    Private Sub txtb2_KeyPress(sender As Object, e As System.Windows.Forms.KeyPressEventArgs) Handles txtb2.KeyPress

        If (Not e.KeyChar = ChrW(Keys.Back) And ("0123456789").IndexOf(e.KeyChar) = -1) Then
            e.Handled = True
        End If

    End Sub

    Private Sub txtb3_KeyPress(sender As Object, e As System.Windows.Forms.KeyPressEventArgs) Handles txtb3.KeyPress

        If (Not e.KeyChar = ChrW(Keys.Back) And ("0123456789").IndexOf(e.KeyChar) = -1) Then
            e.Handled = True
        End If

    End Sub

    Private Sub txtb4_KeyPress(sender As Object, e As System.Windows.Forms.KeyPressEventArgs) Handles txtb4.KeyPress

        Dim KeyAscii As Short = Asc(e.KeyChar)
        Select Case KeyAscii
            Case System.Windows.Forms.Keys.Back  '<--- this is for  backspace
            Case 13
                e.Handled = True
                SendKeys.Send("{TAB}")   '<---- use to tab to next textbox or control
                KeyAscii = 0
            Case Is <= 32
                ' KeyAscii = 0
            Case 45, 46
                Exit Sub
            Case 48 To 57     '<--- this is for numbers 
                Exit Sub
            Case 65 To 90     '<--- this is for Uppercase Alpha 
                Exit Sub
            Case 97 To 122     '<--- this is for Lowercase Alpha 
                Exit Sub
            Case Else
                e.Handled = True
        End Select

    End Sub

    Private Sub txtb5_KeyPress(sender As Object, e As System.Windows.Forms.KeyPressEventArgs) Handles txtb5.KeyPress

        Dim KeyAscii As Short = Asc(e.KeyChar)
        Select Case KeyAscii
            Case System.Windows.Forms.Keys.Back  '<--- this is for  backspace
            Case 13
                e.Handled = True
                SendKeys.Send("{TAB}")   '<---- use to tab to next textbox or control
                KeyAscii = 0
            Case Is <= 32
                ' KeyAscii = 0
            Case 45, 46
                Exit Sub
            Case 48 To 57     '<--- this is for numbers 
                Exit Sub
            Case 65 To 90     '<--- this is for Uppercase Alpha 
                Exit Sub
            Case 97 To 122     '<--- this is for Lowercase Alpha 
                Exit Sub
            Case Else
                e.Handled = True
        End Select

    End Sub

    Private Sub txtb6_KeyPress(sender As Object, e As System.Windows.Forms.KeyPressEventArgs) Handles txtb6.KeyPress

        Dim KeyAscii As Short = Asc(e.KeyChar)
        Select Case KeyAscii
            Case System.Windows.Forms.Keys.Back  '<--- this is for  backspace
            Case 13
                e.Handled = True
                SendKeys.Send("{TAB}")   '<---- use to tab to next textbox or control
                KeyAscii = 0
            Case Is <= 32
                ' KeyAscii = 0
            Case 45, 46
                Exit Sub
            Case 48 To 57     '<--- this is for numbers 
                Exit Sub
            Case 65 To 90     '<--- this is for Uppercase Alpha 
                Exit Sub
            Case 97 To 122     '<--- this is for Lowercase Alpha 
                Exit Sub
            Case Else
                e.Handled = True
        End Select

    End Sub

    Private Sub txtb7_KeyPress(sender As Object, e As System.Windows.Forms.KeyPressEventArgs) Handles txtb7.KeyPress

        If (Not e.KeyChar = ChrW(Keys.Back) And ("0123456789").IndexOf(e.KeyChar) = -1) Then
            e.Handled = True
        End If

    End Sub

    Private Sub txtb8_KeyPress(sender As Object, e As System.Windows.Forms.KeyPressEventArgs) Handles txtb8.KeyPress

        If (Not e.KeyChar = ChrW(Keys.Back) And ("0123456789").IndexOf(e.KeyChar) = -1) Then
            e.Handled = True
        End If

    End Sub

    Private Sub txtb1_LostFocus(sender As Object, e As System.EventArgs) Handles txtb1.LostFocus
        Dim valor As String = Me.txtb1.Text

        If valor.Length > 0 Then
            valor = valor.PadLeft(4, "0")
            Me.txtb1.Text = valor
        Else
            Me.txtb1.Text = txtVal1
        End If

        If Get_Info_Key(Me.txtb1.Text, "", "", 4) = 0 Then
            MessageBox.Show("Ya existe una definicion con este valor (" & Me.txtb1.Text.ToString & ")", "Create record", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Me.txtb1.Focus()
            Me.txtb1.Text = ""
        End If

    End Sub

    Private Sub txtb2_LostFocus(sender As Object, e As System.EventArgs) Handles txtb2.LostFocus
        Dim valor As String = Me.txtb2.Text

        If valor.Length > 0 Then
            valor = valor.PadLeft(4, "0")
            Me.txtb2.Text = valor
        ElseIf valor.Length = 0 Then
            Me.txtb2.Text = txtVal2
        End If

    End Sub

    Private Sub txtb3_LostFocus(sender As Object, e As System.EventArgs) Handles txtb3.LostFocus
        Dim valor As String = Me.txtb3.Text

        If valor.Length > 0 Then
            valor = valor.PadLeft(10, "0")
            Me.txtb3.Text = valor
        Else
            Me.txtb3.Text = txtVal3
        End If

    End Sub

    Private Sub txtb7_LostFocus(sender As Object, e As System.EventArgs) Handles txtb7.LostFocus
        Dim valor As String = Me.txtb7.Text

        If valor.Length > 0 Then
            valor = valor.PadLeft(13, "0")
            Me.txtb7.Text = valor
        Else
            Me.txtb7.Text = txtVal7
        End If

    End Sub

    Private Sub txtb8_LostFocus(sender As Object, e As System.EventArgs) Handles txtb8.LostFocus
        Dim valor As String = Me.txtb8.Text

        If valor.Length > 0 Then
            valor = valor.PadLeft(15, "0")
            Me.txtb8.Text = valor
        Else
            Me.txtb8.Text = txtVal8
        End If

    End Sub


    Private Sub txtb1_GotFocus(sender As Object, e As System.EventArgs) Handles txtb1.GotFocus
        txtVal1 = txtb1.Text
        txtb1.Text = ""

    End Sub

    Private Sub txtb2_GotFocus(sender As Object, e As System.EventArgs) Handles txtb2.GotFocus
        txtVal2 = txtb2.Text
        txtb2.Text = ""

    End Sub

    Private Sub txtb3_GotFocus(sender As Object, e As System.EventArgs) Handles txtb3.GotFocus
        txtVal3 = txtb3.Text
        txtb3.Text = ""

    End Sub

    Private Sub txtb4_GotFocus(sender As Object, e As System.EventArgs) Handles txtb4.GotFocus
        txtVal4 = txtb4.Text
        txtb4.Text = ""

    End Sub

    Private Sub txtb5_GotFocus(sender As Object, e As System.EventArgs) Handles txtb5.GotFocus
        txtVal5 = txtb5.Text
        txtb5.Text = ""

    End Sub

    Private Sub txtb6_GotFocus(sender As Object, e As System.EventArgs) Handles txtb6.GotFocus
        txtVal6 = txtb6.Text
        txtb6.Text = ""

    End Sub

    Private Sub txtb7_GotFocus(sender As Object, e As System.EventArgs) Handles txtb7.GotFocus
        txtVal7 = txtb7.Text
        txtb7.Text = ""

    End Sub

    Private Sub txtb8_GotFocus(sender As Object, e As System.EventArgs) Handles txtb8.GotFocus
        txtVal8 = txtb8.Text
        txtb8.Text = ""

    End Sub

    Private Sub txtb4_LostFocus(sender As Object, e As System.EventArgs) Handles txtb4.LostFocus

        If txtb4.Text.Length > 0 Then
            txtb4.Text = txtb4.Text.ToString.ToUpper
        Else
            txtb4.Text = txtVal4
        End If

    End Sub

    Private Sub txtb5_LostFocus(sender As Object, e As System.EventArgs) Handles txtb5.LostFocus

        If txtb5.Text.Length > 0 Then
            txtb5.Text = txtb5.Text.ToString.ToUpper
        Else
            txtb5.Text = txtVal5
        End If

    End Sub

    Private Sub txtb6_LostFocus(sender As Object, e As System.EventArgs) Handles txtb6.LostFocus

        If txtb6.Text.Length > 0 Then
            txtb6.Text = txtb6.Text.ToString.ToUpper
        Else
            txtb6.Text = txtVal6
        End If

    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles ComboBox1.SelectedIndexChanged

        Show_Retrieved_Data(Me.ComboBox1.SelectedItem.ToString)

    End Sub

    Private Sub Button5_Click(sender As System.Object, e As System.EventArgs) Handles Button5.Click

        Me.Close()

    End Sub

    Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles Button1.Click

        Me.Button2.Enabled = True
        Me.Button3.Enabled = True
        Me.Button4.Enabled = True
        Me.Button3.Text = "NUEVO"
        Me.Button2.Text = "MODIFICAR"
        Me.Button1.Enabled = False
        Init_Control_Form()
        Fill_Data_OnFields()
        Show_Control_Mode(False)
        Me.ComboBox1.Focus()

    End Sub

    Private Sub Button2_Click(sender As System.Object, e As System.EventArgs) Handles Button2.Click

        If Me.Button2.Text = "MODIFICAR" Then
            Me.Button2.Text = "ACEPTAR"
            Me.Button3.Enabled = False
            Me.Button4.Enabled = False
            Me.ComboBox1.Hide()
            Me.txtb1.Text = Me.ComboBox1.SelectedItem
            Me.Button1.Enabled = True
            Show_Control_Mode(True)
        ElseIf Me.Button2.Text = "ACEPTAR" Then
            If AskingMe(" modificar ", Me.txtb1.Text) <> 0 Then
                Exit Sub
            End If
            Me.Button2.Text = "MODIFICAR"
            Me.Button3.Enabled = True
            Me.Button4.Enabled = True
            Me.ComboBox1.Show()
            Me.Button1.Enabled = False
            Show_Control_Mode(False)
            Process_Update_Record()
        End If

    End Sub

    Private Sub Process_Update_Record()
        Dim ListData As New List(Of String)

        ListData.Add(txtb1.Text)
        ListData.Add(txtb2.Text)
        ListData.Add(txtb3.Text)
        ListData.Add(txtb4.Text)
        ListData.Add(txtb5.Text)
        ListData.Add(txtb6.Text)
        ListData.Add(txtb7.Text)
        ListData.Add(txtb8.Text)
        ListData.Add(Me.ComboBox2.SelectedIndex)

        If UpdateInstitutionData(ListData) <> 0 Then
            MsgBox("No se pudo actualizar el registro", MsgBoxStyle.Information, "Update")
        End If

    End Sub


    Private Sub Button4_Click(sender As System.Object, e As System.EventArgs) Handles Button4.Click

        If AskingMe(" Eliminar", Me.ComboBox1.SelectedItem) = 1 Then
            Exit Sub
        End If

        Dim ErrorCode As Byte = 1
        Dim ErrorMessage As String = String.Empty
        Dim StringSQL As String = "delete from sabd_institution_definition where sabd_codigo_autorizador = " & Me.ComboBox1.SelectedItem

        ErrorCode = ExecuteSingleOrder(StringSQL, ErrorMessage)
        If ErrorCode <> 0 Then
            MessageBox.Show(ErrorMessage, "Delete record", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Else
            Fill_Data_OnFields()
        End If

    End Sub

End Class