Imports System.Text.RegularExpressions
Imports System.Xml
Imports System.ComponentModel
Imports System.IO

Public Class SWTM_MainFile
    Dim g_RouterName As String = "          "
    Const Router_TYPE As Byte = 1
    Dim OPT_COMBO As Byte
    Const OPT_ADD As Byte = 0
    Const OPT_UPD As Byte = 1
    Const OPT_DEL As Byte = 2
    Const OPT_NULL As Byte = 99
    Const ERROR_NULL As Byte = 99
    Const SUCCESSFUL As Byte = 0
    Dim OptPath As Byte

    Private Sub TextBox3_KeyPress(sender As Object, e As System.Windows.Forms.KeyPressEventArgs) Handles txtID.KeyPress
        If Asc(e.KeyChar) <> 8 Then
            If Asc(e.KeyChar) < 48 Or Asc(e.KeyChar) > 57 Then
                e.Handled = True
            End If
        End If
    End Sub

    Private Sub TextBox10_KeyPress(sender As Object, e As System.Windows.Forms.KeyPressEventArgs)
        If Asc(e.KeyChar) <> 8 Then
            If Asc(e.KeyChar) < 48 Or Asc(e.KeyChar) > 57 Then
                e.Handled = True
            End If
        End If
    End Sub

    Private Sub TextBox1_KeyPress(sender As Object, e As System.Windows.Forms.KeyPressEventArgs) Handles TextBox1.KeyPress
        If Asc(e.KeyChar) <> 8 Then
            If Asc(e.KeyChar) < 48 Or Asc(e.KeyChar) > 57 Then
                e.Handled = True
            End If
        End If
    End Sub

    Private Sub TextBox2_KeyPress(sender As Object, e As System.Windows.Forms.KeyPressEventArgs) Handles TextBox2.KeyPress
        If Asc(e.KeyChar) <> 8 Then
            If Asc(e.KeyChar) < 48 Or Asc(e.KeyChar) > 57 Then
                e.Handled = True
            End If
        End If
    End Sub

    Private Sub TextBox5_KeyPress(sender As Object, e As System.Windows.Forms.KeyPressEventArgs) Handles TextBox5.KeyPress
        If Asc(e.KeyChar) <> 8 Then
            If Asc(e.KeyChar) < 48 Or Asc(e.KeyChar) > 57 Then
                e.Handled = True
            End If
        End If
    End Sub

    Private Sub TextBox11_KeyPress(sender As Object, e As System.Windows.Forms.KeyPressEventArgs) Handles txt_IP.KeyPress

        If (Not e.KeyChar = ChrW(Keys.Back) And ("0123456789.").IndexOf(e.KeyChar) = -1) Then
            e.Handled = True
        End If

    End Sub

    Private Sub TextBox12_KeyPress(sender As Object, e As System.Windows.Forms.KeyPressEventArgs) Handles txt_Socket.KeyPress
        If Asc(e.KeyChar) <> 8 Then
            If Asc(e.KeyChar) < 48 Or Asc(e.KeyChar) > 57 Then
                e.Handled = True
            End If
        End If
    End Sub

    Private Sub TextBox3_LostFocus(sender As Object, e As System.EventArgs) Handles txtID.LostFocus

        If txtID.TextLength = 0 Then
            Me.ErrorProvider1.SetError(Me.txtID, " Valor no valido")
            Me.txtID.Text = "0000"
            Exit Sub
        Else
            Me.ErrorProvider1.SetError(Me.txtID, "")
        End If

        If ValidateCombo_2() <> 0 Then
            Me.txtID.Focus()
            Me.txtID.Select()
            Exit Sub
        End If

        If Not Regex.Match(txtID.Text, "^[0-9]*$").Success Then
            txtID.Text = "valor invalido"
        End If

        Me.TextBox6.Text = "1000_" & Me.txtID.Text & "_RQQ"
        Me.TextBox7.Text = "2000_" & Me.txtID.Text & "_RPQ"
        Me.TextBox8.Text = "3000_" & Me.txtID.Text & "_TCP"
        Me.TextBox18.Text = "4000_" & Me.txtID.Text & "_SAF"
        Me.TextBox29.Text = "5000_" & Me.txtID.Text & "_CMD"
        Me.TextBox3.Text = "6000_" & Me.txtID.Text & "_ACK"

    End Sub

    Private Sub TextBox4_LostFocus(sender As Object, e As System.EventArgs) Handles txtNAME.LostFocus

        If txtNAME.TextLength = 0 Then
            Me.ErrorProvider1.SetError(Me.txtNAME, " Valor no valido")
            Exit Sub
        Else
            Me.ErrorProvider1.SetError(Me.txtNAME, "")
            Me.txtNAME.Text = Me.txtNAME.Text.ToString.ToUpper
        End If

        If OptPath = 0 Then
            Exit Sub
        Else
            Build_Path_Appl()
        End If

    End Sub


    Private Function ValidateCombo_2() As Byte

        Dim ModuleId As Int16 = Me.txtID.Text
        Me.txtID.Text = Format(ModuleId, "0000")
        Dim ErrorCode As Byte

        Select Case cbb_ModID.SelectedIndex
            Case 0
                If (ModuleId >= 1000) And (ModuleId <= 1999) Then
                    ErrorCode = 0
                    Exit Select
                Else
                    MsgBox("Valor debe estar entre 1000 y 1999")
                    LoadCombo_2()
                    ErrorCode = 1
                End If
            Case 1
                If (ModuleId >= 2000) And (ModuleId <= 2999) Then
                    ErrorCode = 0
                    Exit Select
                Else
                    MsgBox("Valor debe estar entre 2000 y 2999")
                    ErrorCode = 1
                    LoadCombo_2()
                End If
            Case 2
                If (ModuleId >= 3000) And (ModuleId <= 3999) Then
                    ErrorCode = 0
                    Exit Select
                Else
                    MsgBox("Valor debe estar entre 3000 y 3999")
                    ErrorCode = 1
                    LoadCombo_2()
                End If
            Case 3
                If (ModuleId >= 4000) And (ModuleId <= 4999) Then
                    ErrorCode = 0
                    Exit Select
                Else
                    MsgBox("Valor debe estar entre 4000 y 4999")
                    ErrorCode = 1
                    LoadCombo_2()
                End If
            Case 4
                If (ModuleId >= 5000) And (ModuleId <= 5999) Then
                    ErrorCode = 0
                    Exit Select
                Else
                    MsgBox("Valor debe estar entre 5000 y 5999")
                    ErrorCode = 1
                    LoadCombo_2()
                End If
            Case 5
                If (ModuleId >= 6000) And (ModuleId <= 6999) Then
                    ErrorCode = 0
                    Exit Select
                Else
                    MsgBox("Valor debe estar entre 6000 y 6999")
                    ErrorCode = 1
                    LoadCombo_2()
                End If
            Case 6
                If (ModuleId >= 7000) And (ModuleId <= 7999) Then
                    ErrorCode = 0
                    Exit Select
                Else
                    MsgBox("Valor debe estar entre 7000 y 7999")
                    ErrorCode = 1
                    LoadCombo_2()
                End If
            Case 7
                If (ModuleId >= 8000) And (ModuleId <= 8999) Then
                    ErrorCode = 0
                    Exit Select
                Else
                    MsgBox("Valor debe estar entre 8000 y 8999")
                    ErrorCode = 1
                    LoadCombo_2()
                End If
        End Select
        Return ErrorCode

    End Function

    Private Sub LoadCombo_2()
        Dim ModuleId As Int16 = Me.txtID.Text
        Me.txtID.Text = Format(ModuleId, "0000")

        If (ModuleId >= 1000) And (ModuleId <= 1999) Then
            Me.cbb_ModID.SelectedIndex = 0
        ElseIf (ModuleId >= 2000) And (ModuleId <= 2999) Then
            Me.cbb_ModID.SelectedIndex = 1
        ElseIf (ModuleId >= 3000) And (ModuleId <= 3999) Then
            Me.cbb_ModID.SelectedIndex = 2
        ElseIf (ModuleId >= 4000) And (ModuleId <= 4999) Then
            Me.cbb_ModID.SelectedIndex = 3
        ElseIf (ModuleId >= 5000) And (ModuleId <= 5999) Then
            Me.cbb_ModID.SelectedIndex = 4
        ElseIf (ModuleId >= 6000) And (ModuleId <= 6999) Then
            Me.cbb_ModID.SelectedIndex = 5
        ElseIf (ModuleId >= 7000) And (ModuleId <= 7999) Then
            Me.cbb_ModID.SelectedIndex = 6
        End If

    End Sub

    Private Sub Init_Control_Form()

        Me.txtNAME.Text = ""
        Me.txtID.Text = ""
        Me.TextBox1.Text = "0"
        Me.Label15.Text = ""
        Me.TextBox2.Text = "0"
        Me.TextBox5.Text = "0"
        Me.TextBox6.Text = ""
        Me.TextBox7.Text = ""
        Me.TextBox8.Text = ""
        Me.txt_IP.Text = "127.0.0.1"
        Me.txt_Socket.Text = "0000"
        Me.TextBox18.Text = ""
        Me.TextBox29.Text = ""
        Me.TextBox3.Text = ""
        Me.cbb_SocketMode.SelectedIndex = 0
        Me.cbb_ModID.SelectedIndex = 0
        Me.txtNAME.Focus()

    End Sub

    Private Sub TextBox9_KeyPress(sender As Object, e As System.Windows.Forms.KeyPressEventArgs)
        If Asc(e.KeyChar) <> 8 Then
            If Asc(e.KeyChar) < 48 Or Asc(e.KeyChar) > 57 Then
                e.Handled = True
            End If
        End If
    End Sub

    Private Sub TextBox15_KeyPress(sender As Object, e As System.Windows.Forms.KeyPressEventArgs)
        If (Not e.KeyChar = ChrW(Keys.Back) And ("0123456789ABCDEF").IndexOf(e.KeyChar) = -1) Then
            e.Handled = True
        End If
    End Sub

    Private Sub TextBox19_KeyPress(sender As Object, e As System.Windows.Forms.KeyPressEventArgs)

        If Asc(e.KeyChar) <> 8 Then
            If Asc(e.KeyChar) < 48 Or Asc(e.KeyChar) > 57 Then
                e.Handled = True
            End If
        End If

    End Sub

    Private Sub TextBox13_KeyPress(sender As Object, e As System.Windows.Forms.KeyPressEventArgs)

        If Asc(e.KeyChar) <> 8 Then
            If Asc(e.KeyChar) < 48 Or Asc(e.KeyChar) > 57 Then
                e.Handled = True
            End If
        End If

    End Sub

    Private Sub TextBox14_KeyPress(sender As Object, e As System.Windows.Forms.KeyPressEventArgs)

        If Asc(e.KeyChar) <> 8 Then
            If Asc(e.KeyChar) < 48 Or Asc(e.KeyChar) > 57 Then
                e.Handled = True
            End If
        End If

    End Sub


    Private Sub TextBox1_LostFocus(sender As Object, e As System.EventArgs) Handles TextBox1.LostFocus

        If Not Regex.Match(TextBox1.Text, "^[0-9]*$").Success Then
            TextBox1.Text = "valor invalido"
            TextBox1.Focus()
            Return
        End If

    End Sub

    Private Sub TextBox5_LostFocus(sender As Object, e As System.EventArgs) Handles TextBox5.LostFocus

        If Not Regex.Match(TextBox5.Text, "^[0-9]*$").Success Then
            TextBox5.Text = "valor invalido"
            TextBox5.Focus()
            Return
        End If

    End Sub

    Private Sub TextBox2_LostFocus(sender As Object, e As System.EventArgs) Handles TextBox2.LostFocus

        If Not Regex.Match(TextBox2.Text, "^[0-9]*$").Success Then
            TextBox2.Text = "valor invalido"
            TextBox2.Focus()
            Return
        End If

    End Sub

    Private Sub Enable_Disable_Controls(ByVal Enabled As Boolean)

        'Me.TextBox4.Enabled = Enabled
        Me.txtID.Enabled = Enabled
        Me.TextBox1.Enabled = Enabled
        Me.TextBox2.Enabled = Enabled
        Me.TextBox5.Enabled = Enabled
        Me.TextBox6.Enabled = Enabled
        Me.TextBox7.Enabled = Enabled
        Me.TextBox8.Enabled = Enabled
        Me.txt_IP.Enabled = Enabled
        Me.txt_Socket.Enabled = Enabled
        Me.TextBox18.Enabled = Enabled
        Me.TextBox29.Enabled = Enabled
        Me.TextBox3.Enabled = Enabled
        Me.cbb_SocketMode.Enabled = Enabled
        Me.cbb_ModID.Enabled = Enabled
        Me.cbb_Router.Enabled = Enabled
        Me.cbb_Format.Enabled = Enabled
        Me.cbb_Status.Enabled = Enabled
        Me.cbb_ExecName.Enabled = Enabled

    End Sub

    Private Sub ServiSwitch_main_file_KeyPress(sender As Object, e As System.Windows.Forms.KeyPressEventArgs) Handles Me.KeyPress

        If e.KeyChar = ChrW(27) Then
            Me.cbb_Institution.Hide()
            Me.ErrorProvider1.SetError(txtID, "")
            Me.ErrorProvider1.SetError(txtNAME, "")
            'Me.Button1.Enabled = False
            Me.btn_ADD.Enabled = True
            Me.btn_UPD.Enabled = True
            Me.btn_DEL.Enabled = True
            Me.btn_ADD.Text = "NUEVO"
            Me.btn_UPD.Text = "MODIFICAR"
            Me.btn_DEL.Text = "ELIMINAR"
            Me.btn_EXIT.Text = "SALIR"
            Fill_Data_OnFields()
            Enable_Disable_Controls(False)
            Me.txtNAME.TabIndex = 1
            Me.cbb_Module.TabIndex = 0
            Me.cbb_Module.Show()
            Me.cbb_Module.Focus()
            Me.Label15.Show()
            OPT_COMBO = OPT_NULL
            OptPath = 0
        End If

    End Sub

    Private Sub SABD_Main_File_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        OPT_COMBO = OPT_NULL
        Me.cbb_Module.TabIndex = 0
        Me.txtNAME.TabIndex = 1
        Me.cbb_Institution.Hide()
        Fill_Data_OnFields()
        Enable_Disable_Controls(False)
        Me.cbb_Module.Focus()

    End Sub

    Private Sub Fill_Data_OnFields()
        Dim NamesData As New List(Of String)

        Me.cbb_Module.Items.Clear()
        GetArrayData("sabd_main_definition", "sabd_module_name", NamesData)

        If NamesData.Count = 0 Then
            Init_Control_Form()
            Exit Sub
        End If

        For x = 0 To NamesData.Count - 1
            Me.cbb_Module.Items.Add(NamesData(x))
        Next
        Me.cbb_Module.SelectedIndex = 0

        If NamesData.Count = 0 Then
            MsgBox("No existen datos en la tabla principal")
            Exit Sub
        End If

        Show_Retrieved_Data(NamesData(0))

    End Sub

    Private Sub Show_Retrieved_Data(ByVal ModuleName As String)
        Dim ListData As New List(Of String)

        If GetInfoMainData(ModuleName, ListData) <> 0 Then
            Exit Sub
        End If

        Me.txtID.Text = ListData(0)
        Me.cbb_ModID.SelectedIndex = CInt(ListData(1))
        Me.Label15.Text = ListData(2)
        Me.TextBox1.Text = ListData(3)
        Me.TextBox2.Text = ListData(4)
        Me.TextBox5.Text = ListData(5)
        Me.txt_IP.Text = ListData(6).Trim
        Me.txt_Socket.Text = ListData(7)
        Me.cbb_SocketMode.SelectedIndex = CInt(ListData(8))
        Me.TextBox6.Text = ListData(9)
        Me.TextBox7.Text = ListData(10)
        Me.TextBox8.Text = ListData(11)
        Me.TextBox18.Text = ListData(12)
        Me.TextBox29.Text = ListData(13)
        AssignComboOpt(ListData(14))
        Me.cbb_Format.SelectedIndex = CInt(ListData(15))
        Me.TextBox3.Text = ListData(16)
        Me.cbb_Status.SelectedIndex = CInt(ListData(17))
        Me.cbb_ExecName.SelectedIndex = CInt(ListData(18))
        Me.lbl_SourcePath.Text = ListData(19)
        Me.lbl_TargetPath.Text = ListData(20)

    End Sub


    Private Sub Button2_Click(sender As System.Object, e As System.EventArgs) Handles btn_UPD.Click
        OPT_COMBO = OPT_UPD
        If Me.btn_UPD.Text = "MODIFICAR" Then
            OptPath = 1
            Me.btn_UPD.Text = "ACEPTAR"
            Me.btn_EXIT.Text = "CANCELAR"
            Me.cbb_Module.TabIndex = 1
            Me.txtNAME.TabIndex = 0
            Me.cbb_Institution.Show()
            Me.Label15.Hide()
            Me.btn_ADD.Enabled = False
            Me.btn_DEL.Enabled = False
            Me.cbb_Module.Hide()
            Me.txtNAME.Text = Me.cbb_Module.SelectedItem
            Me.txtNAME.Enabled = False
            'Me.Button1.Enabled = True
            Enable_Disable_Controls(True)
            Load_Institution_List()
        ElseIf Me.btn_UPD.Text = "ACEPTAR" Then
            If AskingMe(" modificar ", Me.txtNAME.Text) <> 0 Then
                Exit Sub
            End If
            OptPath = 0
            Me.cbb_Module.TabIndex = 0
            Me.txtNAME.TabIndex = 1
            Me.cbb_Institution.Hide()
            Me.Label15.Show()
            Me.btn_UPD.Text = "MODIFICAR"
            Me.btn_EXIT.Text = "SALIR"
            Me.btn_ADD.Enabled = True
            Me.btn_DEL.Enabled = True
            Me.cbb_Module.Show()
            Me.txtNAME.Text = ""
            Me.txtNAME.Enabled = True
            Enable_Disable_Controls(False)
            Process_Update_Record()
        End If

    End Sub

    Private Sub ShowComBoOptions()
        Dim ArrayData As New List(Of String)
        Dim QueySQL As String

        QueySQL = " select sabd_module_name from sabd_main_definition  where sabd_type = " & Router_TYPE
        GetExplicitData(QueySQL, ArrayData)

    End Sub


    Private Sub Process_Update_Record()

        Dim ListData As New List(Of String)

        ListData.Add(Me.txtID.Text)
        ListData.Add(Me.cbb_ModID.SelectedIndex)
        Me.Label15.Text = Me.cbb_Institution.SelectedItem.ToString
        ListData.Add(Me.Label15.Text)
        ListData.Add(Me.TextBox1.Text)
        ListData.Add(Me.TextBox2.Text)
        ListData.Add(Me.TextBox5.Text)
        If Me.cbb_Format.SelectedIndex = 3 Then
            ListData.Add("127.0.0.1")
            ListData.Add("0000")
            ListData.Add(0)
        Else
            ListData.Add(Me.txt_IP.Text.Trim)
            ListData.Add(Me.txt_Socket.Text)
            ListData.Add(Me.cbb_SocketMode.SelectedIndex)
        End If
        ListData.Add(Me.TextBox6.Text)
        ListData.Add(Me.TextBox7.Text)
        ListData.Add(Me.TextBox8.Text)
        ListData.Add(Me.TextBox18.Text)
        ListData.Add(Me.TextBox29.Text)
        ListData.Add(Me.cbb_Router.SelectedItem.ToString)
        ListData.Add(Me.cbb_Format.SelectedIndex)
        ListData.Add(Me.TextBox3.Text)
        ListData.Add(Me.cbb_Status.SelectedIndex)
        ListData.Add(Me.cbb_ExecName.SelectedIndex)
        ListData.Add(Me.lbl_SourcePath.Text)
        ListData.Add(Me.lbl_TargetPath.Text)

        UpdateInfoMainData(Me.cbb_Module.SelectedItem.ToString, ListData)

    End Sub

    Private Function Process_Add_Record() As Byte
        Dim ModuleName As String = Nothing
        Dim IdModule As String = Me.txtID.Text
        Dim ModuleType As Int16
        '******************************************************
        '******************************************************
        If Me.txtNAME.TextLength = 0 Then
            MessageBox.Show("Valor no valido en nombre de modulo", "Create record", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return 1
        End If

        If Get_Info_Key(Me.txtNAME.Text, IdModule, ModuleType, 0) = 0 Then
            MessageBox.Show("Ya existe un modulo con esa definicion: " & Me.txtNAME.Text.ToString, "Create record", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return 1
        End If
        '******************************************************
        '******************************************************
        If Me.txtID.TextLength = 0 Then
            MessageBox.Show("Valor no valido en nombre de modulo", "Create record", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return 1
        End If

        If Get_Info_Key(IdModule, "", "", 1) = 0 Then
            MessageBox.Show("Ya existe un module Id con este valor:" & IdModule, "Create record", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return 1
        End If
        '******************************************************
        '******************************************************
        ModuleName = Me.txtNAME.Text.Trim
        ModuleName = ModuleName.ToUpper
        Dim ListData As New List(Of String)
        ListData.Add(ModuleName)
        ListData.Add(Me.txtID.Text)
        ListData.Add(Me.cbb_ModID.SelectedIndex)
        ListData.Add(Me.cbb_Institution.SelectedItem.ToString)
        ListData.Add(Me.TextBox1.Text)
        ListData.Add(Me.TextBox2.Text)
        ListData.Add(Me.TextBox5.Text)

        If Me.cbb_Format.SelectedIndex = 3 Then
            ListData.Add("127.0.0.1")
            ListData.Add("0000")
            ListData.Add(0)
        Else
            ListData.Add(Me.txt_IP.Text.Trim)
            ListData.Add(Me.txt_Socket.Text)
            ListData.Add(Me.cbb_SocketMode.SelectedIndex)
        End If

        ListData.Add(Me.TextBox6.Text)
        ListData.Add(Me.TextBox7.Text)
        ListData.Add(Me.TextBox8.Text)
        ListData.Add(Me.TextBox18.Text)
        ListData.Add(Me.TextBox29.Text)
        ListData.Add(g_RouterName)
        ListData.Add(Me.cbb_Format.SelectedIndex)
        ListData.Add(Me.TextBox3.Text)
        ListData.Add(Me.cbb_Status.SelectedIndex)
        ListData.Add(Me.cbb_ExecName.SelectedIndex)
        ListData.Add(Me.lbl_SourcePath.Text)
        ListData.Add(Me.lbl_TargetPath.Text)

        AddInfoMainData(ListData)

        'Process_Create_Path(Me.lbl_SourcePath.Text)
        'Process_Create_Path(Me.lbl_TargetPath.Text)

        Return 0

    End Function

    'Private Sub Process_Create_Path(ByVal PathName As String)
    '    Dim Idx As Byte
    '    Dim PathDirectory As String
    '    Try
    '        Idx = PathName.LastIndexOf("\")
    '        PathDirectory = PathName.Substring(0, Idx)
    '        If Not Directory.Exists(PathDirectory) Then
    '            Directory.CreateDirectory(PathDirectory)
    '        End If
    '    Catch ex As Exception
    '        MsgBox("Error al crear ruta ")
    '    End Try
    'End Sub

    Private Sub Button3_Click(sender As System.Object, e As System.EventArgs) Handles btn_ADD.Click
        OPT_COMBO = OPT_ADD
        Me.cbb_ModID.Refresh()
        If Me.btn_ADD.Text = "NUEVO" Then
            OptPath = 1
            Me.cbb_Institution.Show()
            Me.Label15.Hide()
            Init_Control_Form()
            Me.btn_ADD.Text = "ACEPTAR"
            Me.btn_EXIT.Text = "CANCELAR"
            Me.btn_UPD.Enabled = False
            Me.btn_DEL.Enabled = False
            Me.cbb_Module.Hide()
            Load_Institution_List()
            Enable_Disable_Controls(True)
            Me.cbb_Module.TabIndex = 1
            Me.txtNAME.Enabled = True
            Me.txtNAME.TabIndex = 0
            Me.txtNAME.Select()
        ElseIf Me.btn_ADD.Text = "ACEPTAR" Then
            If ValidateCombo_2() = 0 Then
                If Process_Add_Record() = 0 Then
                    OptPath = 0
                    Me.cbb_Module.TabIndex = 1
                    Me.txtNAME.TabIndex = 0
                    Me.cbb_Institution.Hide()
                    Me.Label15.Show()
                    Me.btn_ADD.Text = "NUEVO"
                    Me.btn_EXIT.Text = "SALIR"
                    Me.btn_UPD.Enabled = True
                    Me.btn_DEL.Enabled = True
                    Me.cbb_Module.Show()
                    'Me.Button1.Enabled = False
                    Enable_Disable_Controls(False)
                    Fill_Data_OnFields()
                    MessageBox.Show("Proceso exitoso !!")
                End If
            Else
                Me.txtID.Focus()
                Me.txtID.Select()
            End If
        End If




    End Sub

    Private Sub Load_Institution_List()
        Dim idx As Int16
        Dim NamesData As New List(Of String)

        Me.cbb_Institution.Items.Clear()
        GetArrayData("sabd_institution_definition", "sabd_nombre", NamesData)

        If NamesData.Count = 0 Then
            Me.cbb_Institution.Text = "No data"
            Exit Sub
        End If

        For x = 0 To NamesData.Count - 1
            Me.cbb_Institution.Items.Add(NamesData(x))
        Next
        If NamesData.Contains(Me.Label15.Text.ToString) Then
            idx = NamesData.IndexOf(Me.Label15.Text.ToString)
            Me.cbb_Institution.SelectedIndex = idx
        Else
            Me.cbb_Institution.SelectedIndex = 0
        End If
    End Sub

    Private Sub ProcessCANCEL()

        OPT_COMBO = OPT_NULL
        Me.cbb_Module.TabIndex = 0
        Me.txtNAME.TabIndex = 1
        Process_Init()
        Me.cbb_Institution.Hide()
        Me.Label15.Show()

    End Sub

    Private Sub Process_Init()

        Fill_Data_OnFields()
        Enable_Disable_Controls(False)
        Me.cbb_Module.Show()
        Me.btn_UPD.Enabled = True
        Me.btn_ADD.Enabled = True
        Me.btn_DEL.Enabled = True
        Me.btn_ADD.Text = "NUEVO"
        Me.btn_UPD.Text = "MODIFICAR"
        'Me.Button1.Enabled = False
        Me.ErrorProvider1.SetError(txtID, "")
        Me.ErrorProvider1.SetError(txtNAME, "")

    End Sub

    Private Sub ComboBox3_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles cbb_Module.SelectedIndexChanged

        Show_Retrieved_Data(Me.cbb_Module.SelectedItem)
        Enable_Disable_Controls(False)

        If OptPath = 0 Then
            Exit Sub
        Else
            Build_Path_Appl()
        End If

    End Sub

    Private Sub TextBox4_KeyPress(sender As Object, e As System.Windows.Forms.KeyPressEventArgs) Handles txtNAME.KeyPress

        Dim KeyAscii As Short = Asc(e.KeyChar)
        Select Case KeyAscii
            Case System.Windows.Forms.Keys.Back  '<--- this is for  backspace
            Case 13
                e.Handled = True
                SendKeys.Send("{TAB}")   '<---- use to tab to next textbox or control
                KeyAscii = 0
            Case Is <= 32
                ' KeyAscii = 0
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


    Private Sub Button4_Click(sender As System.Object, e As System.EventArgs) Handles btn_DEL.Click
        OPT_COMBO = OPT_DEL
        If AskingMe(" Eliminar", Me.cbb_Module.SelectedItem) = 1 Then
            Exit Sub
        End If

        Dim ErrorCode As Byte = 1
        Dim ErrorMessage As String = String.Empty
        Dim StringSQL As String = "delete from sabd_main_definition where sabd_module_name = '" & Me.cbb_Module.SelectedItem & "'"

        ErrorCode = ExecuteSingleOrder(StringSQL, ErrorMessage)
        If ErrorCode <> 0 Then
            MessageBox.Show(ErrorMessage, "Delete record", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Else
            Fill_Data_OnFields()
        End If

    End Sub

    Private Sub cbb_ModID_LostFocus(sender As Object, e As System.EventArgs) Handles cbb_ModID.LostFocus
        Dim ErrorCode As Byte = ERROR_NULL
        Dim NewModuleId As Int16

        ErrorCode = GetNextIDValue(Me.cbb_ModID.SelectedIndex, NewModuleId)
        If ErrorCode = SUCCESSFUL Then
            Me.txtID.Text = NewModuleId.ToString("0000")
        End If

        Me.TextBox6.Text = "1000_" & Me.txtID.Text & "_RQQ"
        Me.TextBox7.Text = "2000_" & Me.txtID.Text & "_RPQ"
        Me.TextBox8.Text = "3000_" & Me.txtID.Text & "_TCP"
        Me.TextBox18.Text = "4000_" & Me.txtID.Text & "_SAF"
        Me.TextBox29.Text = "5000_" & Me.txtID.Text & "_CMD"
        Me.TextBox3.Text = "6000_" & Me.txtID.Text & "_ACK"

    End Sub

    Private Sub ComboBox2_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles cbb_ModID.SelectedIndexChanged
        Dim ModuleName As String
        Dim NewModuleId As Int16
        Dim ErrorCode As Byte = ERROR_NULL
        If Me.cbb_Module.Items.Count = 0 Then
            ModuleName = "          "
        Else
            ModuleName = Me.cbb_Module.SelectedItem.ToString
        End If

        Select Case Me.cbb_ModID.SelectedIndex
            Case 0, 6, 7
                Me.Label14.Show()
                ShowComboOpt()
            Case Else
                Me.Label14.Hide()
                Me.cbb_Router.Hide()
                g_RouterName = "          "
        End Select

        Select Case OPT_COMBO
            Case OPT_ADD
                ErrorCode = GetNextIDValue(Me.cbb_ModID.SelectedIndex, NewModuleId)
                If ErrorCode = SUCCESSFUL Then
                    Me.txtID.Text = NewModuleId.ToString("0000")
                    'OPT_COMBO = OPT_NULL
                End If
        End Select

    End Sub


    Private Sub ShowComboOpt()
        Dim QuerySQL As String
        Dim ArrayData As New List(Of String)

        Me.cbb_Router.Items.Clear()
        Me.cbb_Router.Show()
        QuerySQL = " select sabd_module_name from sabd_main_definition  where sabd_type = " & Router_TYPE
        GetExplicitData(QuerySQL, ArrayData)
        Dim x As Byte
        For x = 0 To ArrayData.Count - 1
            Me.cbb_Router.Items.Add(ArrayData(x))
        Next
        Me.cbb_Router.SelectedIndex = 0

    End Sub

    Private Sub AssignComboOpt(ByVal DetailedValue As String)
        Dim x As Byte

        If String.IsNullOrWhiteSpace(DetailedValue) Then
            Exit Sub
        End If

        For x = 0 To Me.cbb_Router.Items.Count - 1
            If Me.cbb_Router.Items(x).ToString = DetailedValue Then
                Me.cbb_Router.SelectedIndex = x
            End If
        Next

    End Sub

    Private Sub ComboBox4_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles cbb_Router.SelectedIndexChanged

        Try
            g_RouterName = Me.cbb_Router.SelectedItem.ToString
        Catch ex As Exception
            g_RouterName = "          "
        End Try

    End Sub


    Private Sub ComboBox6_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles cbb_Format.SelectedIndexChanged

        'If Me.cbb_Format.SelectedIndex = 3 Then
        '    Me.cbb_SocketMode.SelectedIndex = -1
        '    Me.txt_IP.Text = ""
        '    Me.txt_Socket.Text = ""
        '    Me.gbb_Comm.Enabled = False
        '    'Else
        '    '    Me.cbb_SocketMode.SelectedIndex = 0
        '    '    Me.txt_IP.Text = "127.0.0.1"
        '    '    Me.txt_Socket.Text = "0000"
        '    '    Me.gbb_Comm.Enabled = True
        'End If

        If OptPath = 0 Then
            Exit Sub
        Else
            Build_Path_Appl()
        End If

    End Sub

    Private Sub btn_EXIT_Click(sender As System.Object, e As System.EventArgs) Handles btn_EXIT.Click

        'If Me.btn_EXIT.Text = "CANCELAR" Then
        '    ProcessCANCEL()
        '    Me.btn_EXIT.Text = "SALIR"
        'ElseIf Me.btn_EXIT.Text = "SALIR" Then
        '    Dim Resp As MsgBoxResult
        '    Resp = MessageBox.Show("Esta usted seguro que desea salir ?", "Salir", MessageBoxButtons.OKCancel)
        '    If Resp = MsgBoxResult.Ok Then
        '        Me.Close()
        '    End If
        'End If

        Me.Close()

    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles cbb_ExecName.SelectedIndexChanged

        If OptPath = 0 Then
            Exit Sub
        Else
            Build_Path_Appl()
        End If

    End Sub

    Private Sub Build_Path_Appl()
        Dim PathAppl As String

        If Me.cbb_Format.SelectedItem.ToString = "Componente" Then
            PathAppl = ReleasePath & "Componente\" & Me.cbb_ExecName.SelectedItem.ToString
            Me.lbl_SourcePath.Text = PathAppl
            'PathAppl = ExecutePath & Me.cbb_Module.SelectedItem.ToString.Trim & "\" & Me.cbb_ExecName.SelectedItem.ToString
            PathAppl = ExecutePath & Me.txtNAME.Text.Trim & "\" & Me.cbb_ExecName.SelectedItem.ToString
            Me.lbl_TargetPath.Text = PathAppl
        Else
            PathAppl = ReleasePath & Me.cbb_Format.SelectedItem.ToString & "\" & Me.cbb_ExecName.SelectedItem.ToString
            Me.lbl_SourcePath.Text = PathAppl
            'PathAppl = ExecutePath & Me.cbb_Format.SelectedItem.ToString & "\" & Me.cbb_Module.SelectedItem.ToString.Trim & "\" & Me.cbb_ExecName.SelectedItem.ToString
            PathAppl = ExecutePath & Me.cbb_Format.SelectedItem.ToString & "\" & Me.txtNAME.Text.Trim & "\" & Me.cbb_ExecName.SelectedItem.ToString
            Me.lbl_TargetPath.Text = PathAppl
        End If

    End Sub

    Private Sub cbb_Institution_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles cbb_Institution.SelectedIndexChanged

        If OptPath = 0 Then
            Exit Sub
        Else
            Build_Path_Appl()
        End If

    End Sub
End Class

Public Class RegxValidateTextBox
    Public Shared Sub Main()
        Application.Run(New SWTM_MainFile)
    End Sub
End Class

