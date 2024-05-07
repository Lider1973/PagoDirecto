Public Class SWTM_Services

    Dim G_Services As String
    Dim Initialization As Boolean = False
    Dim TempKey As String

    Private Sub SABD_services_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        Me.Button1.Enabled = False
        Me.Button2.Enabled = True
        Me.Button3.Enabled = True
        Me.Button4.Enabled = True
        Fill_Data_OnFields2()
        Fill_Data_OnFields1()
        Show_Control_Mode(False)
        'Me.ComboBox1.Focus()

    End Sub

    Private Sub Fill_Data_OnFields2()
        Dim NamesData As New List(Of String)

        Me.Cmb4.Items.Clear()
        GetArrayData("sabd_institution_definition", "sabd_codigo_autorizador", NamesData)

        If NamesData.Count = 0 Then
            'Init_Control_Form()
            Exit Sub
        End If

        For x = 0 To NamesData.Count - 1
            Me.Cmb4.Items.Add(NamesData(x).PadLeft(4, "0"))
        Next

        If NamesData.Count = 0 Then
            MsgBox("No existen datos en la tabla principal")
            Exit Sub
        End If

        Initialization = True

    End Sub


    Private Sub Fill_Data_OnFields1()
        Dim NamesData As New List(Of String)

        Me.Cmb1.Items.Clear()
        GetArrayData("sabd_service_definition", "sabd_service_code", NamesData)

        If NamesData.Count = 0 Then
            Cmb1.SelectedIndex = -1
            Cmb2.SelectedIndex = -1
            Cmb_TypeService.SelectedIndex = -1
            Me.txt5.Text = ""
            Exit Sub
        End If

        For x = 0 To NamesData.Count - 1
            Me.Cmb1.Items.Add(NamesData(x).PadLeft(4, "0"))
        Next
        Me.Cmb1.SelectedIndex = 0
        Me.Label6.Hide()

        If NamesData.Count = 0 Then
            MsgBox("No existen datos en la tabla principal")
            Exit Sub
        End If

        Show_Retrieved_Data(NamesData(0))

    End Sub

    Private Sub Show_Retrieved_Data(ByVal ModuleName As String)
        Dim ListData As New List(Of String)

        If GetInfoServiceData(ModuleName, ListData) <> 0 Then
            Exit Sub
        End If

        Dim AuxTmp As Int16

        AuxTmp = ListData(0).ToString.Substring(0, 3)
        For x = 0 To Me.Cmb2.Items.Count - 1
            If CInt(Me.Cmb2.Items(x).ToString.Substring(0, 3)) = AuxTmp Then
                Me.Cmb2.SelectedIndex = x
            End If
        Next

        AuxTmp = ListData(0).ToString.Substring(3, 3)
        For x = 0 To Me.Cmb_TypeService.Items.Count - 1
            If CInt(Me.Cmb_TypeService.Items(x).ToString.Substring(0, 3)) = AuxTmp Then
                Me.Cmb_TypeService.SelectedIndex = x
            End If
        Next

        AuxTmp = ListData(0).ToString.Substring(6, 4)
        For x = 0 To Me.Cmb4.Items.Count - 1
            If CInt(Me.Cmb4.Items(x).ToString) = AuxTmp Then
                Me.Cmb4.SelectedIndex = x
            End If
        Next
        Me.txt5.Text = ListData(1)

    End Sub


    Private Sub Show_Control_Mode(ByVal Mode As Boolean)

        Me.Cmb2.Enabled = Mode
        Me.Cmb_TypeService.Enabled = Mode
        Me.Cmb4.Enabled = Mode
        Me.txt5.Enabled = Mode

        If Mode Then
            Me.Cmb2.SelectedIndex = 0
            Me.Cmb_TypeService.SelectedIndex = 0
            Me.Cmb4.SelectedIndex = 0
            Me.txt5.Text = ""
            'Else
            '    Me.Cmb2.SelectedIndex = -1
            '    Me.Cmb3.SelectedIndex = -1
            '    Me.Cmb4.SelectedIndex = -1
        End If

    End Sub

    Private Sub Enable_Disable_Control(ByVal Mode As Boolean)

        Me.Cmb2.Enabled = Mode
        Me.Cmb_TypeService.Enabled = Mode
        Me.Cmb4.Enabled = Mode
        Me.Button3.Enabled = Not Mode
        Me.Button4.Enabled = Not Mode
        Me.Button1.Enabled = Mode
        Me.txt5.Enabled = Mode

        If Mode Then
            Me.Cmb1.Hide()
            Me.Label6.Show()
            Me.Label6.Text = Cmb1.SelectedItem.ToString
        Else
            Me.Cmb1.Show()
            Me.Label6.Hide()
            Me.Cmb1.Focus()
        End If

    End Sub


    Private Sub Button3_Click(sender As System.Object, e As System.EventArgs) Handles Button3.Click

        If Me.Button3.Text = "NUEVO" Then
            Me.Cmb1.Hide()
            Me.Label6.Show()
            Me.Button3.Text = "ACEPTAR"
            Me.Button2.Enabled = False
            Me.Button4.Enabled = False
            Me.Button1.Enabled = True
            'Load_Institution_List()
            Show_Control_Mode(True)
        ElseIf Me.Button3.Text = "ACEPTAR" Then
            If Process_Add_Record() = 0 Then
                Me.Cmb1.Show()
                Me.Label6.Hide()
                Me.Button3.Text = "NUEVO"
                Me.Button2.Enabled = True
                Me.Button4.Enabled = True
                Me.Button1.Enabled = False
                Show_Control_Mode(False)
                Fill_Data_OnFields1()
                MessageBox.Show("Proceso exitoso !!")
            End If
        End If

    End Sub

    Private Function Process_Add_Record() As Byte

        Dim ListData As New List(Of String)
        ListData.Add(Label6.Text)
        ListData.Add(Me.txt5.Text)

        Return AddInfoServiceData(ListData)

    End Function

    Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles Button1.Click

        Me.Cmb1.Show()
        Me.Label6.Hide()
        Me.Button1.Enabled = False
        Me.Button2.Enabled = True
        Me.Button3.Enabled = True
        Me.Button4.Enabled = True
        Me.Button3.Text = "NUEVO"
        Me.Button2.Text = "MODIFICAR"
        Show_Control_Mode(False)
        Me.Cmb1.Focus()

    End Sub

    Private Sub Cmb4_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles Cmb4.SelectedIndexChanged
        Dim ModuleName As String = String.Empty

        If Get_Info_Key(Me.Cmb4.SelectedItem.ToString, "", ModuleName, 4) = 0 Then
            Me.Label7.Text = ModuleName
        End If

        Build_Service_Code()

    End Sub

    Private Sub Cmb2_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles Cmb2.SelectedIndexChanged

        Build_Service_Code()

    End Sub

    Private Sub Cmb3_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles Cmb_TypeService.SelectedIndexChanged

        Build_Service_Code()

    End Sub

    Private Sub Build_Service_Code()

        Try
            If Initialization Then
                G_Services = Me.Cmb2.SelectedItem.ToString.Substring(0, 3) & Me.Cmb_TypeService.SelectedItem.ToString.Substring(0, 3) & Me.Cmb4.SelectedItem.ToString
                Me.Label6.Text = G_Services
            End If
        Catch ex As Exception

        End Try

    End Sub

    Private Sub TextBox1_LostFocus(sender As Object, e As System.EventArgs) Handles txt5.LostFocus

        Me.txt5.Text = Me.txt5.Text.ToString.ToUpper

    End Sub


    Private Sub Cmb1_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles Cmb1.SelectedIndexChanged

        Show_Retrieved_Data(Me.Cmb1.SelectedItem.ToString)

    End Sub

    Private Sub Button2_Click(sender As System.Object, e As System.EventArgs) Handles Button2.Click

        If Me.Button2.Text = "MODIFICAR" Then
            Me.Button2.Text = "ACEPTAR"
            TempKey = Me.Cmb1.SelectedItem.ToString
            Enable_Disable_Control(True)
        ElseIf Me.Button2.Text = "ACEPTAR" Then
            If AskingMe(" modificar ", Me.Cmb1.SelectedItem.ToString) <> 0 Then
                Exit Sub
            End If
            Me.Button2.Text = "MODIFICAR"
            Enable_Disable_Control(False)
            Process_Update_Record()
        End If

    End Sub

    Private Sub Process_Update_Record()
        Dim ListData As New List(Of String)

        ListData.Add(Me.Label6.Text)
        ListData.Add(Me.txt5.Text)

        If UpdateServiceData(TempKey, ListData) <> 0 Then
            MsgBox("No se pudo actualizar el registro", MsgBoxStyle.Information, "Update")
        Else
            Fill_Data_OnFields2()
            Fill_Data_OnFields1()
        End If

    End Sub

    Private Sub Button4_Click(sender As System.Object, e As System.EventArgs) Handles Button4.Click

        If AskingMe(" Eliminar", Me.Cmb1.SelectedItem) = 1 Then
            Exit Sub
        End If

        Dim ErrorCode As Byte = 1
        Dim ErrorMessage As String = String.Empty
        Dim StringSQL As String = "delete from sabd_service_definition where sabd_service_code = " & Me.Cmb1.SelectedItem

        ErrorCode = ExecuteSingleOrder(StringSQL, ErrorMessage)
        If ErrorCode <> 0 Then
            MessageBox.Show(ErrorMessage, "Delete record", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Else
            Fill_Data_OnFields1()
        End If

    End Sub
End Class