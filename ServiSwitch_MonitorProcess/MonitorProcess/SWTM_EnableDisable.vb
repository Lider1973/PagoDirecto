Public Class SWTM_EnableDisable
    Private Sub SWTM_EnableDisable_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Load_Institution(Me.cbb_Institution)

        Me.cbb_Institution.SelectedIndex = 0
        Me.cbb_Order.SelectedIndex = 0

    End Sub

    Public Sub Load_Institution(ByRef CBB As ComboBox)
        Dim NamesData As New List(Of String)
        CBB.Items.Clear()
        DB_GetArrayData("sabd_institution_definition", "sabd_nombre", NamesData, " where sabd_codigo_adquirente between 100 and 1000")
        If NamesData.Count = 0 Then
            Exit Sub
        End If
        For x = 0 To NamesData.Count - 1
            CBB.Items.Add(NamesData(x).Trim)
        Next
        If NamesData.Count = 0 Then
            MsgBox("No existen datos en la tabla principal")
            Exit Sub
        End If
        CBB.SelectedIndex = 0
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click


        Dim ASK As DialogResult = MessageBox.Show(" Esta seguro de " & Me.cbb_Order.SelectedItem.ToString.TrimEnd & Chr(13) & " a " & Me.cbb_Institution.SelectedItem.ToString.TrimEnd & " del servicio ?", " Declinear", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If ASK = Windows.Forms.DialogResult.No Then
            Exit Sub
        End If

        Dim FI As Int16 = DB_Get_Adq_Aut(Me.cbb_Institution.SelectedItem.ToString.TrimEnd, 0)
        Dim CommandLine As String = Router_NOTIFY & "0|" & Me.cbb_Order.SelectedIndex & "|" & FI

        Dim BEHAVIOR As New SWTM_Behavior
        BEHAVIOR.Put_Notify_To_Router(CommandLine)
        BEHAVIOR = Nothing

    End Sub
End Class