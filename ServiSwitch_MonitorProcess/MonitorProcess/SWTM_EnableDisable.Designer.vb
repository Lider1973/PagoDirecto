<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class SWTM_EnableDisable
    Inherits System.Windows.Forms.Form

    'Form reemplaza a Dispose para limpiar la lista de componentes.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Requerido por el Diseñador de Windows Forms
    Private components As System.ComponentModel.IContainer

    'NOTA: el Diseñador de Windows Forms necesita el siguiente procedimiento
    'Se puede modificar usando el Diseñador de Windows Forms.  
    'No lo modifique con el editor de código.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.cbb_Institution = New System.Windows.Forms.ComboBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.cbb_Order = New System.Windows.Forms.ComboBox()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.btn_Exit = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'cbb_Institution
        '
        Me.cbb_Institution.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbb_Institution.FormattingEnabled = True
        Me.cbb_Institution.Location = New System.Drawing.Point(94, 34)
        Me.cbb_Institution.Name = "cbb_Institution"
        Me.cbb_Institution.Size = New System.Drawing.Size(273, 24)
        Me.cbb_Institution.TabIndex = 123
        '
        'Label2
        '
        Me.Label2.ForeColor = System.Drawing.Color.Gray
        Me.Label2.Location = New System.Drawing.Point(9, 35)
        Me.Label2.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(83, 23)
        Me.Label2.TabIndex = 124
        Me.Label2.Text = "Institución"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label1
        '
        Me.Label1.ForeColor = System.Drawing.Color.Gray
        Me.Label1.Location = New System.Drawing.Point(391, 34)
        Me.Label1.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(83, 23)
        Me.Label1.TabIndex = 125
        Me.Label1.Text = "Comando"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'cbb_Order
        '
        Me.cbb_Order.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbb_Order.FormattingEnabled = True
        Me.cbb_Order.Items.AddRange(New Object() {"DESABILITAR", "HABILITAR"})
        Me.cbb_Order.Location = New System.Drawing.Point(481, 33)
        Me.cbb_Order.Name = "cbb_Order"
        Me.cbb_Order.Size = New System.Drawing.Size(273, 24)
        Me.cbb_Order.TabIndex = 126
        '
        'Button2
        '
        Me.Button2.Cursor = System.Windows.Forms.Cursors.Hand
        Me.Button2.ForeColor = System.Drawing.Color.SteelBlue
        Me.Button2.Location = New System.Drawing.Point(94, 92)
        Me.Button2.Margin = New System.Windows.Forms.Padding(4)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(273, 53)
        Me.Button2.TabIndex = 137
        Me.Button2.Text = "ENVIAR COMANDO"
        Me.Button2.UseVisualStyleBackColor = True
        '
        'btn_Exit
        '
        Me.btn_Exit.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btn_Exit.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btn_Exit.ForeColor = System.Drawing.Color.Maroon
        Me.btn_Exit.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btn_Exit.Location = New System.Drawing.Point(481, 99)
        Me.btn_Exit.Margin = New System.Windows.Forms.Padding(4)
        Me.btn_Exit.Name = "btn_Exit"
        Me.btn_Exit.Size = New System.Drawing.Size(273, 46)
        Me.btn_Exit.TabIndex = 136
        Me.btn_Exit.Text = "    SALIR"
        Me.btn_Exit.UseVisualStyleBackColor = True
        '
        'SWTM_EnableDisable
        '
        Me.AcceptButton = Me.Button2
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.btn_Exit
        Me.ClientSize = New System.Drawing.Size(800, 192)
        Me.Controls.Add(Me.Button2)
        Me.Controls.Add(Me.btn_Exit)
        Me.Controls.Add(Me.cbb_Order)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.cbb_Institution)
        Me.Controls.Add(Me.Label2)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "SWTM_EnableDisable"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Habilitar / Desabilitar"
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents cbb_Institution As ComboBox
    Friend WithEvents Label2 As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents cbb_Order As ComboBox
    Friend WithEvents Button2 As Button
    Friend WithEvents btn_Exit As Button
End Class
