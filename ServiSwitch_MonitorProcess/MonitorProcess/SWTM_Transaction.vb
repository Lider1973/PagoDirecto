
Public Class SWTM_Transaction

    Dim txt3GotFocus As Boolean = False
    Const HEXVAL As Byte = 15
    Dim HexArray(HEXVAL), BinArray(HEXVAL) As String
    Dim HexBitMap1, HexBitMap2 As Char()
    Dim ComplementString As String

    Private Sub SABD_Transaction_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        Me.Button1.Enabled = False
        Me.txt_BitMap.Enabled = False
        Me.TextBox2.Enabled = False
        Me.TextBox5.Enabled = False

        Fill_Data_OnFields()
        Enable_Disable_Controls(False)


    End Sub

    Private Sub Fill_Data_OnFields()
        Me.txt_Name.SendToBack()
        Me.cbb_Name.BringToFront()
        Me.cbb_Name.Focus()
        Me.cbb_Name.TabIndex = 0
        Dim NamesData As New List(Of String)

        Me.cbb_Name.Items.Clear()
        GetArrayData("sabd_transaction_definition", "sabd_transaction_Name", NamesData)

        If NamesData.Count = 0 Then
            Init_Control_Form()
            Exit Sub
        End If

        For x = 0 To NamesData.Count - 1
            Me.cbb_Name.Items.Add(NamesData(x))
        Next

        'Me.cbb_Name.SelectedIndex = 0
        'Me.Cbb_Type.SelectedIndex = 0

        If NamesData.Count = 0 Then
            MsgBox("No existen datos en la tabla principal")
            Exit Sub
        End If

        Me.cbb_Name.SelectedIndex = 0
        Me.cbb_Name.TabIndex = 0
        Me.cbb_Name.Focus()
        'Show_Retrieved_Data(NamesData(0))

    End Sub

    Private Sub Init_Control_Form()

        Me.txt_Name.Text = ""
        Me.TextBox2.Text = ""
        Me.TextBox5.Text = ""
        Me.txt_BitMap.Text = ""
        Me.Cbb_Mode.SelectedIndex = 0
        Me.Cbb_Type.SelectedIndex = 2

    End Sub

    Private Sub Enable_Disable_Controls(ByVal Enabled As Boolean)

        Me.txt_Name.Enabled = Enabled
        Me.txt_Name.Enabled = Enabled
        Me.Cbb_Mode.Enabled = Enabled
        Me.Cbb_Type.Enabled = Enabled
        Me.CheckedListBox1.Enabled = Enabled
        Me.CheckedListBox2.Enabled = Enabled

    End Sub

    Private Sub Show_Retrieved_Data(ByVal TransactionName As String)
        Dim ListData As New List(Of String)

        If GetInfoTranData(TransactionName, ListData) <> 0 Then
            Exit Sub
        End If

        Me.txt_Codigo.Text = ListData(0)
        Dim x As Byte

        Me.Cbb_Mode.SelectedIndex = CInt(ListData(2))

        For x = 0 To Me.Cbb_Type.Items.Count - 1
            If Me.Cbb_Type.Items(x).ToString = ListData(3).Trim Then
                Me.Cbb_Type.SelectedIndex = x
                Exit For
            End If
        Next

        Me.txt_BitMap.Text = ListData(4)
        Me.TextBox2.Text = ListData(4).Substring(0, 16)
        Me.TextBox5.Text = ListData(4).Substring(16)

        Dim bitmap1 As String = TextBox2.Text.Replace(" ", "")
        Dim bitmap2 As String = TextBox5.Text.Replace(" ", "")
        Dim Temp As String = String.Empty
        Dim y As Byte
        GenerateBitMapsChar(bitmap1, bitmap2)

        bitmap1 = bitmap1 & bitmap2
        Dim HexBitMap1() As Char = bitmap1.ToCharArray
        For x = 0 To 127
            If HexBitMap1(x) = "1" Then
                Temp = Temp & CStr(x + 1) & ", "
                If x <= 63 Then
                    Me.CheckedListBox1.SetItemChecked(x, True)
                End If
                If x > 63 Then
                    y = x - 64
                    Me.CheckedListBox2.SetItemChecked(y, True)
                End If
            End If
        Next

    End Sub


    Private Sub Button3_Click(sender As System.Object, e As System.EventArgs) Handles Button3.Click

        If Me.Button3.Text = "NUEVO" Then
            Init_Control_Form()
            Me.Button3.Text = "ACEPTAR"
            Me.Button2.Enabled = False
            Me.Button4.Enabled = False
            Me.cbb_Name.Hide()
            Me.Button1.Enabled = True
            InitCheckBoxes()
            Me.txt_BitMap.Text = "00000000000000000000000000000000"
            Me.TextBox2.Text = "0000000000000000"
            Me.TextBox5.Text = "0000000000000000"
            Me.Cbb_Type.SelectedIndex = 0
            Me.txt_Codigo.Text = ""
            Enable_Disable_Controls(True)
            Me.txt_Name.TabIndex = 0
            Me.txt_Name.Focus()
        ElseIf Me.Button3.Text = "ACEPTAR" Then
            If Process_Add_Record() = 0 Then
                Me.Button3.Text = "NUEVO"
                Me.Button2.Enabled = True
                Me.Button4.Enabled = True
                Me.cbb_Name.Show()
                Me.Button1.Enabled = False
                InitCheckBoxes()
                Enable_Disable_Controls(False)
                Fill_Data_OnFields()
                MessageBox.Show("Proceso exitoso !!")
            End If
        End If

    End Sub

    Private Function Process_Add_Record() As Byte

        Dim ListData As New List(Of String)
        ListData.Add(Me.txt_Codigo.Text)
        ListData.Add(Me.txt_Name.Text)
        ListData.Add(Me.Cbb_Mode.SelectedIndex)
        ListData.Add(Me.Cbb_Type.SelectedItem)
        ListData.Add(Me.txt_BitMap.Text)

        Return AddInfoTranData(ListData)

    End Function

    Private Sub CheckedListBox1_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles CheckedListBox1.SelectedIndexChanged
        Dim indexChecked As Integer
        Dim ListData As New List(Of String)
        Dim SetMaps As String = String.Empty
        Dim SetHexa As String = String.Empty
        For Each indexChecked In CheckedListBox1.CheckedIndices
            ListData.Add(indexChecked)
            'SetMaps = SetMaps & CStr(indexChecked + 1) & ", "
        Next

        Dim x, z As Byte
        For x = 0 To 63
            z = 0
            For Each SH In ListData
                If x = CInt(SH) Then
                    z = 1
                End If
            Next
            If z = 1 Then
                SetMaps += "1"
            Else
                SetMaps += "0"
            End If
        Next

        Dim Temp As String
        For x = 0 To 15
            Temp = SetMaps.Substring(0, 4)
            SetMaps = SetMaps.Remove(0, 4)
            SetHexa = SetHexa & Convert.ToString(Convert.ToInt32(Temp, 2), 16)
        Next

        Me.TextBox2.Text = SetHexa.ToUpper
        Me.txt_BitMap.Text = Me.TextBox2.Text & Me.TextBox5.Text

        If Me.CheckedListBox1.GetItemCheckState(CheckedListBox1.SelectedIndex) = CheckState.Checked Then
            Me.Label7.Text = GetFieldName(CheckedListBox1.SelectedIndex + 1)
            Me.Label8.Text = ""
        ElseIf Me.CheckedListBox1.GetItemCheckState(CheckedListBox1.SelectedIndex) = CheckState.Unchecked Then
            Me.Label7.Text = ""
        End If

    End Sub


    Private Sub CheckedListBox2_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles CheckedListBox2.SelectedIndexChanged
        Dim indexChecked As Integer
        Dim ListData As New List(Of String)
        Dim SetMaps As String = String.Empty
        Dim SetHexa As String = String.Empty
        For Each indexChecked In CheckedListBox2.CheckedIndices
            ListData.Add(indexChecked)
        Next

        Dim x, z As Byte
        For x = 0 To 63
            z = 0
            For Each SH In ListData
                If x = CInt(SH) Then
                    z = 1
                End If
            Next
            If z = 1 Then
                SetMaps += "1"
            Else
                SetMaps += "0"
            End If
        Next

        Dim Temp As String
        For x = 0 To 15
            Temp = SetMaps.Substring(0, 4)
            SetMaps = SetMaps.Remove(0, 4)
            SetHexa = SetHexa & Convert.ToString(Convert.ToInt32(Temp, 2), 16)
        Next
        Me.TextBox5.Text = SetHexa.ToUpper
        Me.txt_BitMap.Text = Me.TextBox2.Text & Me.TextBox5.Text

        If Me.CheckedListBox2.GetItemCheckState(CheckedListBox2.SelectedIndex) = CheckState.Checked Then
            Me.Label8.Text = GetFieldName(CheckedListBox2.SelectedIndex + 65)
            Me.Label7.Text = ""
        ElseIf Me.CheckedListBox2.GetItemCheckState(CheckedListBox2.SelectedIndex) = CheckState.Unchecked Then
            Me.Label8.Text = ""
        End If

    End Sub


    Private Sub TextBox3_LostFocus(sender As Object, e As System.EventArgs) Handles txt_Name.LostFocus

        Me.txt_Name.Text = Me.txt_Name.Text.ToUpper

    End Sub

    Private Sub InitCheckBoxes()
        Dim x As Byte

        For x = 0 To Me.CheckedListBox1.Items.Count - 1
            Me.CheckedListBox1.SetItemChecked(x, False)
            Me.CheckedListBox2.SetItemChecked(x, False)
        Next

    End Sub

    Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles Button1.Click

        Fill_Data_OnFields()
        Enable_Disable_Controls(False)
        Me.cbb_Name.Show()
        Me.Button2.Enabled = True
        Me.Button3.Enabled = True
        Me.Button4.Enabled = True
        Me.Button3.Text = "NUEVO"
        Me.Button2.Text = "MODIFICAR"
        Me.Button1.Enabled = False
        Me.Label7.Text = ""
        Me.Label8.Text = ""

    End Sub

    Private Sub Button2_Click(sender As System.Object, e As System.EventArgs) Handles Button2.Click

        If Me.Button2.Text = "MODIFICAR" Then
            Me.Button2.Text = "ACEPTAR"
            Me.Button3.Enabled = False
            Me.Button4.Enabled = False
            Me.cbb_Name.Hide()
            Me.txt_Name.Text = Me.cbb_Name.SelectedItem
            Me.Button1.Enabled = True
            Enable_Disable_Controls(True)
            Me.txt_Name.Focus()
            Me.txt_Name.TabIndex = 0
        ElseIf Me.Button2.Text = "ACEPTAR" Then
            If AskingMe(" modificar ", Me.txt_Name.Text) <> 0 Then
                Exit Sub
            End If
            Me.Button2.Text = "MODIFICAR"
            Me.Button3.Enabled = True
            Me.Button4.Enabled = True
            Me.cbb_Name.Show()
            Me.Button1.Enabled = False
            Enable_Disable_Controls(False)
            Process_Update_Record()
            Fill_Data_OnFields()
        End If

    End Sub

    Private Sub Process_Update_Record()

        Dim ListData As New List(Of String)

        ListData.Add(Me.txt_Codigo.Text)
        ListData.Add(Me.txt_Name.Text)
        ListData.Add(Me.Cbb_Mode.SelectedIndex)
        ListData.Add(Me.Cbb_Type.SelectedItem.ToString)
        ListData.Add(Me.txt_BitMap.Text)

        If UpdateInfoTranData(Me.cbb_Name.SelectedItem.ToString, ListData) = 0 Then
            MessageBox.Show("Proceso exitoso !!")
        End If

    End Sub


    Private Sub ComboBox3_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles cbb_Name.SelectedIndexChanged

        InitCheckBoxes()
        Show_Retrieved_Data(Me.cbb_Name.SelectedItem.ToString)

    End Sub

    Private Sub Button4_Click(sender As System.Object, e As System.EventArgs) Handles Button4.Click

        If AskingMe(" Eliminar ", Me.cbb_Name.SelectedItem.ToString) = 1 Then
            Exit Sub
        End If

        Dim ErrorCode As Byte = 1
        Dim ErrorMessage As String = String.Empty
        Dim StringSQL As String = "delete from sabd_transaction_definition where sabd_transaction_code = '" & Me.txt_Codigo.Text & "' and sabd_transaction_type=" & Me.Cbb_Mode.SelectedIndex

        ErrorCode = ExecuteSingleOrder(StringSQL, ErrorMessage)
        If ErrorCode <> 0 Then
            MessageBox.Show(ErrorMessage, "Delete record", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Else
            Fill_Data_OnFields()
            MessageBox.Show("Proceso exitoso !!")
        End If

    End Sub

    Private Sub txt_Codigo_KeyPress(sender As Object, e As System.Windows.Forms.KeyPressEventArgs) Handles txt_Codigo.KeyPress

        If (Not e.KeyChar = ChrW(Keys.Back) And ("0123456789").IndexOf(e.KeyChar) = -1) Then
            e.Handled = True
        End If

    End Sub

    Private Sub Cbb_Mode_LostFocus(sender As Object, e As System.EventArgs) Handles Cbb_Mode.LostFocus

        Dim ModuleName As String = Me.txt_Name.Text
        Dim IdModule As String = ""
        Dim ModuleType As String = ""

        SetGetComplement = Me.Cbb_Mode.SelectedIndex
        If Get_Info_Key(Me.txt_Codigo.Text, IdModule, ModuleType, 2) = 0 Then
            MessageBox.Show("Ya existe una definicion con este valor y Modo (" & Me.txt_Codigo.Text & ")-" & Me.Cbb_Mode.SelectedItem.ToString, "Create record", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Me.txt_Name.Focus()
            Me.txt_Name.Text = ""
        End If

    End Sub

    Public Sub GenerateBitMapsChar(ByRef BitMap_1 As String, ByRef BitMap_2 As String)
        Dim BitMapsChar1, BitMapsChar2, TempValue As String
        Dim ErrorCode, x As Byte

        BitMapsChar1 = ""
        HexBitMap1 = BitMap_1.ToCharArray
        For x = 0 To HEXVAL
            TempValue = Convert.ToString(Convert.ToInt32(HexBitMap1(x), 16), 2)
            TempValue = TempValue.PadLeft(4, "0")
            If ErrorCode = 0 Then
                BitMapsChar1 = BitMapsChar1 & TempValue
            End If
        Next

        BitMapsChar2 = ""
        HexBitMap2 = BitMap_2.ToCharArray
        For x = 0 To HEXVAL
            TempValue = Convert.ToString(Convert.ToInt32(HexBitMap2(x), 16), 2)
            TempValue = TempValue.PadLeft(4, "0")
            If ErrorCode = 0 Then
                BitMapsChar2 = BitMapsChar2 & TempValue
            End If
        Next

        BitMap_1 = BitMapsChar1
        BitMap_2 = BitMapsChar2

    End Sub

    Public Property SetGetComplement() As String
        Get
            Return ComplementString
        End Get
        Set(ByVal Value As String)
            ComplementString = Value
        End Set

    End Property


End Class

