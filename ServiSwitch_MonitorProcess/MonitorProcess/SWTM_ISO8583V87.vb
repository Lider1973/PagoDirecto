Imports System.Text.RegularExpressions

Public Class SWTM_ISO8583V87

    Private Const REC_SHOW As Byte = 0
    Private Const REC_EDIT As Byte = 1

    Dim Exiting As Boolean = False


    Private Sub SABD_iso_standard_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        Fill_Data_OnFields()
        ShowControlMode(REC_SHOW)
        'Enable_Disable_Controls(False)


    End Sub

    Private Sub Fill_Data_OnFields()
        Dim NamesData As New List(Of String)

        Me.cbb_IdField.Items.Clear()

        GetArrayData("sabd_iso_definition", "sabd_field_id", NamesData)

        If NamesData.Count = 0 Then
            Init_Control_Form()
            Enable_Disable_Controls(False)
            Exit Sub
        End If

        For x = 0 To NamesData.Count - 1
            Me.cbb_IdField.Items.Add(NamesData(x))
        Next
        Me.cbb_IdField.SelectedIndex = 0

        If NamesData.Count = 0 Then
            MsgBox("No existen datos en la tabla principal")
            Exit Sub
        End If

        Show_Retrieved_Data(NamesData(0))

    End Sub

    Private Sub Show_Retrieved_Data(ByVal FieldId As Byte)
        Dim ListData As New List(Of String)

        Try
            If GetInfoFieldData(FieldId, ListData) <> 0 Then
                Exit Sub
            End If
            Me.cbb_Status.SelectedIndex = CInt(ListData(1))
            Me.cbb_TypeData.SelectedIndex = CInt(ListData(2))
            Me.Txt_Length.Text = ListData(3)
            Me.cbb_TypeField.SelectedIndex = CInt(ListData(4))
            Me.Txt_Description.Text = ListData(5)
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try

    End Sub

    Private Sub ComboBox4_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles cbb_IdField.SelectedIndexChanged

        Show_Retrieved_Data(Me.cbb_IdField.SelectedItem.ToString)

    End Sub

    Private Sub Button3_Click(sender As System.Object, e As System.EventArgs) Handles Button3.Click

        If Me.Button3.Text = "NUEVO" Then
            Init_Control_Form()
            Me.Button3.Text = "ACEPTAR"
            ShowControlMode(REC_EDIT)
        ElseIf Me.Button3.Text = "ACEPTAR" Then
            If Process_Add_Record() = 0 Then
                Me.Button3.Text = "NUEVO"
                ShowControlMode(REC_SHOW)
                Enable_Disable_Controls(False)
                Fill_Data_OnFields()
                MessageBox.Show("Proceso exitoso !!")
            End If
        End If

    End Sub

    Private Function Process_Add_Record() As Byte

        Dim ModuleName As String = Me.TextBox2.Text
        Dim IdModule As String = ""
        Dim ModuleType As String = ""

        If Get_Info_Key(ModuleName, IdModule, ModuleType, 3) = 0 Then
            MessageBox.Show("Ya existe una definicion con este valor (" & ModuleName & ")", "Create record", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Enable_Disable_Controls(True)
            Me.cbb_IdField.Hide()
            Me.TextBox2.Focus()
            Return 1
        End If

        If Me.Txt_Description.Text.Length = 0 Then
            MessageBox.Show("Valor descripción es incorrecto:" & ModuleName, "Create record", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Enable_Disable_Controls(True)
            Me.Txt_Description.Focus()
            Return 1
        End If

        Dim ListData As New List(Of String)
        ListData.Add(ModuleName)
        ListData.Add(Me.cbb_Status.SelectedIndex)
        ListData.Add(Me.cbb_TypeData.SelectedIndex)
        ListData.Add(Me.Txt_Length.Text)
        ListData.Add(Me.cbb_TypeField.SelectedIndex)
        ListData.Add(Me.Txt_Description.Text)

        Return AddInfoFieldData(ListData)

    End Function

    Private Sub ShowControlMode(ByVal Mode As Byte)

        Select Case Mode
            Case REC_SHOW
                Me.cbb_IdField.Show()
                Me.cbb_Status.Enabled = False
                Me.cbb_TypeData.Enabled = False
                Me.cbb_TypeField.Enabled = False
                Me.Txt_Description.Enabled = False
                Me.TextBox2.Enabled = False
                Me.Txt_Length.Enabled = False
                Me.Button2.Enabled = True
                Me.Button4.Enabled = True
                Me.Button1.Enabled = False
            Case REC_EDIT
                Me.cbb_IdField.Hide()
                Me.cbb_Status.Enabled = True
                Me.cbb_TypeData.Enabled = True
                Me.cbb_TypeField.Enabled = True
                Me.Txt_Description.Enabled = True
                Me.TextBox2.Enabled = True
                Me.Txt_Length.Enabled = True
                Me.Txt_Description.Text = ""
                Me.TextBox2.Text = ""
                Me.Txt_Length.Text = "0"
                Me.cbb_Status.SelectedIndex = 0
                Me.cbb_TypeData.SelectedIndex = 0
                Me.cbb_TypeField.SelectedIndex = 0
                Me.TextBox2.Focus()
                Me.Button2.Enabled = False
                Me.Button4.Enabled = False
                Me.Button1.Enabled = True
        End Select

    End Sub


    Private Sub Init_Control_Form()

        Me.Txt_Description.Text = ""
        Me.TextBox2.Text = ""
        Me.Txt_Length.Text = "0"
        Me.cbb_Status.SelectedIndex = 0
        Me.cbb_TypeData.SelectedIndex = 0
        Me.cbb_TypeField.SelectedIndex = 0

    End Sub

    Private Sub Enable_Disable_Controls(ByVal Enabled As Boolean)

        Me.cbb_Status.Enabled = Enabled
        Me.cbb_TypeData.Enabled = Enabled
        Me.cbb_TypeField.Enabled = Enabled
        Me.Txt_Description.Enabled = Enabled
        Me.TextBox2.Enabled = Enabled
        Me.Txt_Length.Enabled = Enabled

    End Sub

    Private Sub TextBox2_KeyPress(sender As Object, e As System.Windows.Forms.KeyPressEventArgs) Handles TextBox2.KeyPress

        If (Not e.KeyChar = ChrW(Keys.Back) And ("0123456789.").IndexOf(e.KeyChar) = -1) Then
            e.Handled = True
        End If

    End Sub


    Private Sub TextBox2_LostFocus(sender As Object, e As System.EventArgs) Handles TextBox2.LostFocus

        If Exiting = True Then
            Exit Sub
        End If


        If Not Regex.Match(TextBox2.Text, "^[0-9]*$").Success Then
            MessageBox.Show("Valor invalido", "BitMap", MessageBoxButtons.OK)
            TextBox2.Focus()
            Return
        End If

        Dim Valor As Int16
        Try
            Valor = CInt(Me.TextBox2.Text)
        Catch ex As Exception
            MessageBox.Show("Valor invalido", "BitMap", MessageBoxButtons.OK)
            Me.TextBox2.Text = "000"
            Me.TextBox2.Focus()
            Exit Sub
        End Try

        If Valor > 128 Then
            MessageBox.Show("Valor debe estar entre 1 y 128", "BitMap", MessageBoxButtons.OK)
            Me.TextBox2.Focus()
            Valor = "000"
            Exit Sub
        End If

        Me.TextBox2.Text = Valor.ToString("000")

    End Sub

    Private Sub Txt_Length_KeyPress(sender As Object, e As System.Windows.Forms.KeyPressEventArgs) Handles Txt_Length.KeyPress

        If (Not e.KeyChar = ChrW(Keys.Back) And ("0123456789").IndexOf(e.KeyChar) = -1) Then
            e.Handled = True
        End If

    End Sub

    Private Sub TextBox3_LostFocus(sender As Object, e As System.EventArgs) Handles Txt_Length.LostFocus

        If Not Regex.Match(Txt_Length.Text, "^[0-9]*$").Success Then
            MessageBox.Show("Valor invalido", "BitMap", MessageBoxButtons.OK)
            Txt_Length.Focus()
            Return
        End If

        Dim Valor As Int16
        Try
            Valor = CInt(Me.Txt_Length.Text)
        Catch ex As Exception
            Valor = "000"
        End Try

        Me.Txt_Length.Text = Valor.ToString("000")

    End Sub

    Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles Button1.Click

        Fill_Data_OnFields()
        Enable_Disable_Controls(False)
        Me.cbb_IdField.Show()
        Me.Button2.Enabled = True
        Me.Button3.Enabled = True
        Me.Button4.Enabled = True
        Me.Button3.Text = "NUEVO"
        Me.Button2.Text = "MODIFICAR"
        Me.Button1.Enabled = False

    End Sub

    Private Sub Button5_Click(sender As System.Object, e As System.EventArgs) Handles Button5.Click

        Exiting = True
        Me.Close()

    End Sub

    'Private Sub ComboBox3_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles ComboBox3.SelectedIndexChanged

    '    Select Case ComboBox3.SelectedIndex
    '        Case 1
    '            Me.Txt_Length.Text = "99"
    '        Case 2
    '            Me.Txt_Length.Text = "999"
    '    End Select

    'End Sub

    Private Sub Button2_Click(sender As System.Object, e As System.EventArgs) Handles Button2.Click

        If Me.Button2.Text = "MODIFICAR" Then
            Me.Button2.Text = "ACEPTAR"
            Me.Button3.Enabled = False
            Me.Button4.Enabled = False
            'Me.ComboBox4.Hide()
            Me.Button1.Enabled = True
            Enable_Disable_Controls(True)
        ElseIf Me.Button2.Text = "ACEPTAR" Then
            If AskingMe(" modificar ", Me.Txt_Description.Text.ToString) <> 0 Then
                Exit Sub
            End If
            Me.Button2.Text = "MODIFICAR"
            Me.Button3.Enabled = True
            Me.Button4.Enabled = True
            'Me.ComboBox4.Show()
            Me.Button1.Enabled = False
            Enable_Disable_Controls(False)
            Process_Update_Record()
        End If

    End Sub

    Private Sub Process_Update_Record()

        Dim ListData As New List(Of String)

        ListData.Add(Me.cbb_IdField.SelectedItem.ToString)
        ListData.Add(Me.cbb_Status.SelectedIndex)
        ListData.Add(Me.cbb_TypeData.SelectedIndex)
        ListData.Add(Me.Txt_Length.Text)
        ListData.Add(Me.cbb_TypeField.SelectedIndex)
        ListData.Add(Me.Txt_Description.Text)

        UpdateISOData(Me.cbb_IdField.SelectedItem.ToString, ListData)

    End Sub


    Private Sub Button4_Click(sender As System.Object, e As System.EventArgs) Handles Button4.Click

        If AskingMe(" Eliminar", Me.cbb_IdField.SelectedItem) = 1 Then
            Exit Sub
        End If

        Dim ErrorCode As Byte = 1
        Dim ErrorMessage As String = String.Empty
        Dim StringSQL As String = "delete from sabd_iso_definition where sabd_field_id = " & Me.cbb_IdField.SelectedItem

        ErrorCode = ExecuteSingleOrder(StringSQL, ErrorMessage)
        If ErrorCode <> 0 Then
            MessageBox.Show(ErrorMessage, "Delete record", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Else
            Fill_Data_OnFields()
        End If

    End Sub
End Class

Public Class Verification_ID
    Public Shared Sub Main()
        Application.Run(New SWTM_ISO8583V87)
    End Sub
End Class