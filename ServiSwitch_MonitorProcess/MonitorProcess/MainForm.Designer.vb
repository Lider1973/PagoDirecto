<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class MainForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MainForm))
        Me.ListBox1 = New System.Windows.Forms.ListBox()
        Me.ListView1 = New System.Windows.Forms.ListView()
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.InicioProcesoToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.HideBlinkToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem2 = New System.Windows.Forms.ToolStripMenuItem()
        Me.StopProcessToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ShutdownProcessToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.TracesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.MinimoToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.MediaToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AltaToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.OFFLineToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ONLineToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ImageList1 = New System.Windows.Forms.ImageList(Me.components)
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.SistemaStartupToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CommanderOpt = New System.Windows.Forms.ToolStripMenuItem()
        Me.ConnectItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ShutDownToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.StartVerifyToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ClearListToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.HideAllToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ConfiguraciónToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.InstitucionesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.Item_Process = New System.Windows.Forms.ToolStripMenuItem()
        Me.ISO8583V87ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ISO8583V87ToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.TransaccionesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.RuteoToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ServicioToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ReloadConfigurationToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.EstadosToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.DenegarToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.DeclinarToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SwitchearToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.PorAutorizadorToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SalirToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SalirManagerToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.CountQueueToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ContextMenuStrip1.SuspendLayout()
        Me.MenuStrip1.SuspendLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'ListBox1
        '
        Me.ListBox1.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.ListBox1.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
        Me.ListBox1.Font = New System.Drawing.Font("Courier New", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ListBox1.FormattingEnabled = True
        Me.ListBox1.ItemHeight = 16
        Me.ListBox1.Location = New System.Drawing.Point(352, 145)
        Me.ListBox1.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.ListBox1.Name = "ListBox1"
        Me.ListBox1.Size = New System.Drawing.Size(1138, 800)
        Me.ListBox1.TabIndex = 0
        '
        'ListView1
        '
        Me.ListView1.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.ListView1.ContextMenuStrip = Me.ContextMenuStrip1
        Me.ListView1.Font = New System.Drawing.Font("Microsoft Sans Serif", 7.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ListView1.HideSelection = False
        Me.ListView1.LargeImageList = Me.ImageList1
        Me.ListView1.Location = New System.Drawing.Point(4, 146)
        Me.ListView1.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.ListView1.Name = "ListView1"
        Me.ListView1.Size = New System.Drawing.Size(334, 799)
        Me.ListView1.TabIndex = 1
        Me.ListView1.UseCompatibleStateImageBehavior = False
        '
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.ImageScalingSize = New System.Drawing.Size(20, 20)
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.InicioProcesoToolStripMenuItem, Me.HideBlinkToolStripMenuItem, Me.ToolStripMenuItem2, Me.StopProcessToolStripMenuItem, Me.ShutdownProcessToolStripMenuItem, Me.TracesToolStripMenuItem, Me.OFFLineToolStripMenuItem, Me.ONLineToolStripMenuItem})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        Me.ContextMenuStrip1.Size = New System.Drawing.Size(199, 196)
        '
        'InicioProcesoToolStripMenuItem
        '
        Me.InicioProcesoToolStripMenuItem.Name = "InicioProcesoToolStripMenuItem"
        Me.InicioProcesoToolStripMenuItem.Size = New System.Drawing.Size(198, 24)
        Me.InicioProcesoToolStripMenuItem.Text = "Start process"
        '
        'HideBlinkToolStripMenuItem
        '
        Me.HideBlinkToolStripMenuItem.Name = "HideBlinkToolStripMenuItem"
        Me.HideBlinkToolStripMenuItem.Size = New System.Drawing.Size(198, 24)
        Me.HideBlinkToolStripMenuItem.Text = "Show process"
        '
        'ToolStripMenuItem2
        '
        Me.ToolStripMenuItem2.Name = "ToolStripMenuItem2"
        Me.ToolStripMenuItem2.Size = New System.Drawing.Size(198, 24)
        Me.ToolStripMenuItem2.Text = "Hide process"
        '
        'StopProcessToolStripMenuItem
        '
        Me.StopProcessToolStripMenuItem.Name = "StopProcessToolStripMenuItem"
        Me.StopProcessToolStripMenuItem.Size = New System.Drawing.Size(198, 24)
        Me.StopProcessToolStripMenuItem.Text = "Interrupt process"
        '
        'ShutdownProcessToolStripMenuItem
        '
        Me.ShutdownProcessToolStripMenuItem.Name = "ShutdownProcessToolStripMenuItem"
        Me.ShutdownProcessToolStripMenuItem.Size = New System.Drawing.Size(198, 24)
        Me.ShutdownProcessToolStripMenuItem.Text = "Shutdown process"
        '
        'TracesToolStripMenuItem
        '
        Me.TracesToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MinimoToolStripMenuItem, Me.MediaToolStripMenuItem, Me.AltaToolStripMenuItem})
        Me.TracesToolStripMenuItem.Name = "TracesToolStripMenuItem"
        Me.TracesToolStripMenuItem.Size = New System.Drawing.Size(198, 24)
        Me.TracesToolStripMenuItem.Text = "Traces"
        '
        'MinimoToolStripMenuItem
        '
        Me.MinimoToolStripMenuItem.Name = "MinimoToolStripMenuItem"
        Me.MinimoToolStripMenuItem.Size = New System.Drawing.Size(143, 26)
        Me.MinimoToolStripMenuItem.Text = "Minimo"
        '
        'MediaToolStripMenuItem
        '
        Me.MediaToolStripMenuItem.Name = "MediaToolStripMenuItem"
        Me.MediaToolStripMenuItem.Size = New System.Drawing.Size(143, 26)
        Me.MediaToolStripMenuItem.Text = "Media"
        '
        'AltaToolStripMenuItem
        '
        Me.AltaToolStripMenuItem.Name = "AltaToolStripMenuItem"
        Me.AltaToolStripMenuItem.Size = New System.Drawing.Size(143, 26)
        Me.AltaToolStripMenuItem.Text = "Alta"
        '
        'OFFLineToolStripMenuItem
        '
        Me.OFFLineToolStripMenuItem.Name = "OFFLineToolStripMenuItem"
        Me.OFFLineToolStripMenuItem.Size = New System.Drawing.Size(198, 24)
        Me.OFFLineToolStripMenuItem.Text = "OFF line"
        '
        'ONLineToolStripMenuItem
        '
        Me.ONLineToolStripMenuItem.Name = "ONLineToolStripMenuItem"
        Me.ONLineToolStripMenuItem.Size = New System.Drawing.Size(198, 24)
        Me.ONLineToolStripMenuItem.Text = "ON line"
        '
        'ImageList1
        '
        Me.ImageList1.ImageStream = CType(resources.GetObject("ImageList1.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.ImageList1.TransparentColor = System.Drawing.Color.Transparent
        Me.ImageList1.Images.SetKeyName(0, "process-icon.png")
        Me.ImageList1.Images.SetKeyName(1, "process_info.png")
        Me.ImageList1.Images.SetKeyName(2, "process-accept-icon.png")
        Me.ImageList1.Images.SetKeyName(3, "process-remove-icon.png")
        Me.ImageList1.Images.SetKeyName(4, "Process-Warning-icon.png")
        Me.ImageList1.Images.SetKeyName(5, "Process-Accept.png")
        Me.ImageList1.Images.SetKeyName(6, "iconON.png")
        Me.ImageList1.Images.SetKeyName(7, "iconOFF.png")
        '
        'MenuStrip1
        '
        Me.MenuStrip1.BackColor = System.Drawing.Color.Gray
        Me.MenuStrip1.ImageScalingSize = New System.Drawing.Size(20, 20)
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripMenuItem1, Me.CommanderOpt, Me.ConfiguraciónToolStripMenuItem, Me.EstadosToolStripMenuItem, Me.SalirToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(1502, 28)
        Me.MenuStrip1.TabIndex = 6
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'ToolStripMenuItem1
        '
        Me.ToolStripMenuItem1.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.SistemaStartupToolStripMenuItem})
        Me.ToolStripMenuItem1.ForeColor = System.Drawing.Color.White
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        Me.ToolStripMenuItem1.Size = New System.Drawing.Size(81, 24)
        Me.ToolStripMenuItem1.Text = "&Procesos"
        '
        'SistemaStartupToolStripMenuItem
        '
        Me.SistemaStartupToolStripMenuItem.Name = "SistemaStartupToolStripMenuItem"
        Me.SistemaStartupToolStripMenuItem.Size = New System.Drawing.Size(139, 26)
        Me.SistemaStartupToolStripMenuItem.Text = "&Reload"
        '
        'CommanderOpt
        '
        Me.CommanderOpt.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ConnectItem, Me.ShutDownToolStripMenuItem, Me.StartVerifyToolStripMenuItem, Me.ClearListToolStripMenuItem, Me.HideAllToolStripMenuItem, Me.CountQueueToolStripMenuItem})
        Me.CommanderOpt.ForeColor = System.Drawing.Color.White
        Me.CommanderOpt.Name = "CommanderOpt"
        Me.CommanderOpt.Size = New System.Drawing.Size(105, 24)
        Me.CommanderOpt.Text = "&Commander"
        '
        'ConnectItem
        '
        Me.ConnectItem.Name = "ConnectItem"
        Me.ConnectItem.Size = New System.Drawing.Size(224, 26)
        Me.ConnectItem.Text = "&StartUp"
        '
        'ShutDownToolStripMenuItem
        '
        Me.ShutDownToolStripMenuItem.Name = "ShutDownToolStripMenuItem"
        Me.ShutDownToolStripMenuItem.Size = New System.Drawing.Size(224, 26)
        Me.ShutDownToolStripMenuItem.Text = "S&hutDown"
        '
        'StartVerifyToolStripMenuItem
        '
        Me.StartVerifyToolStripMenuItem.Name = "StartVerifyToolStripMenuItem"
        Me.StartVerifyToolStripMenuItem.Size = New System.Drawing.Size(224, 26)
        Me.StartVerifyToolStripMenuItem.Text = "&Resume"
        '
        'ClearListToolStripMenuItem
        '
        Me.ClearListToolStripMenuItem.Name = "ClearListToolStripMenuItem"
        Me.ClearListToolStripMenuItem.Size = New System.Drawing.Size(224, 26)
        Me.ClearListToolStripMenuItem.Text = "Clear list"
        '
        'HideAllToolStripMenuItem
        '
        Me.HideAllToolStripMenuItem.Name = "HideAllToolStripMenuItem"
        Me.HideAllToolStripMenuItem.Size = New System.Drawing.Size(224, 26)
        Me.HideAllToolStripMenuItem.Text = "Hide all"
        '
        'ConfiguraciónToolStripMenuItem
        '
        Me.ConfiguraciónToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.InstitucionesToolStripMenuItem, Me.Item_Process, Me.ISO8583V87ToolStripMenuItem, Me.TransaccionesToolStripMenuItem, Me.RuteoToolStripMenuItem, Me.ServicioToolStripMenuItem, Me.ReloadConfigurationToolStripMenuItem})
        Me.ConfiguraciónToolStripMenuItem.ForeColor = System.Drawing.Color.White
        Me.ConfiguraciónToolStripMenuItem.Name = "ConfiguraciónToolStripMenuItem"
        Me.ConfiguraciónToolStripMenuItem.Size = New System.Drawing.Size(116, 24)
        Me.ConfiguraciónToolStripMenuItem.Text = "&Configuración"
        '
        'InstitucionesToolStripMenuItem
        '
        Me.InstitucionesToolStripMenuItem.Name = "InstitucionesToolStripMenuItem"
        Me.InstitucionesToolStripMenuItem.Size = New System.Drawing.Size(234, 26)
        Me.InstitucionesToolStripMenuItem.Text = "&Instituciones"
        '
        'Item_Process
        '
        Me.Item_Process.Name = "Item_Process"
        Me.Item_Process.Size = New System.Drawing.Size(234, 26)
        Me.Item_Process.Text = "&Procesos"
        '
        'ISO8583V87ToolStripMenuItem
        '
        Me.ISO8583V87ToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ISO8583V87ToolStripMenuItem1})
        Me.ISO8583V87ToolStripMenuItem.Name = "ISO8583V87ToolStripMenuItem"
        Me.ISO8583V87ToolStripMenuItem.Size = New System.Drawing.Size(234, 26)
        Me.ISO8583V87ToolStripMenuItem.Text = "Formato de mensajes"
        '
        'ISO8583V87ToolStripMenuItem1
        '
        Me.ISO8583V87ToolStripMenuItem1.Name = "ISO8583V87ToolStripMenuItem1"
        Me.ISO8583V87ToolStripMenuItem1.Size = New System.Drawing.Size(174, 26)
        Me.ISO8583V87ToolStripMenuItem1.Text = "ISO8583 v87"
        '
        'TransaccionesToolStripMenuItem
        '
        Me.TransaccionesToolStripMenuItem.Name = "TransaccionesToolStripMenuItem"
        Me.TransaccionesToolStripMenuItem.Size = New System.Drawing.Size(234, 26)
        Me.TransaccionesToolStripMenuItem.Text = "&Transacciones"
        '
        'RuteoToolStripMenuItem
        '
        Me.RuteoToolStripMenuItem.Name = "RuteoToolStripMenuItem"
        Me.RuteoToolStripMenuItem.Size = New System.Drawing.Size(234, 26)
        Me.RuteoToolStripMenuItem.Text = "&Ruteo"
        '
        'ServicioToolStripMenuItem
        '
        Me.ServicioToolStripMenuItem.Name = "ServicioToolStripMenuItem"
        Me.ServicioToolStripMenuItem.Size = New System.Drawing.Size(234, 26)
        Me.ServicioToolStripMenuItem.Text = "&Servicio"
        '
        'ReloadConfigurationToolStripMenuItem
        '
        Me.ReloadConfigurationToolStripMenuItem.Name = "ReloadConfigurationToolStripMenuItem"
        Me.ReloadConfigurationToolStripMenuItem.Size = New System.Drawing.Size(234, 26)
        Me.ReloadConfigurationToolStripMenuItem.Text = "Reload Configuration"
        '
        'EstadosToolStripMenuItem
        '
        Me.EstadosToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.DenegarToolStripMenuItem, Me.PorAutorizadorToolStripMenuItem})
        Me.EstadosToolStripMenuItem.ForeColor = System.Drawing.Color.White
        Me.EstadosToolStripMenuItem.Name = "EstadosToolStripMenuItem"
        Me.EstadosToolStripMenuItem.Size = New System.Drawing.Size(74, 24)
        Me.EstadosToolStripMenuItem.Text = "Estados"
        '
        'DenegarToolStripMenuItem
        '
        Me.DenegarToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.DeclinarToolStripMenuItem, Me.SwitchearToolStripMenuItem})
        Me.DenegarToolStripMenuItem.Name = "DenegarToolStripMenuItem"
        Me.DenegarToolStripMenuItem.Size = New System.Drawing.Size(196, 26)
        Me.DenegarToolStripMenuItem.Text = "Transacciones"
        '
        'DeclinarToolStripMenuItem
        '
        Me.DeclinarToolStripMenuItem.Name = "DeclinarToolStripMenuItem"
        Me.DeclinarToolStripMenuItem.Size = New System.Drawing.Size(156, 26)
        Me.DeclinarToolStripMenuItem.Text = "Declinar"
        '
        'SwitchearToolStripMenuItem
        '
        Me.SwitchearToolStripMenuItem.Name = "SwitchearToolStripMenuItem"
        Me.SwitchearToolStripMenuItem.Size = New System.Drawing.Size(156, 26)
        Me.SwitchearToolStripMenuItem.Text = "Switchear"
        '
        'PorAutorizadorToolStripMenuItem
        '
        Me.PorAutorizadorToolStripMenuItem.Name = "PorAutorizadorToolStripMenuItem"
        Me.PorAutorizadorToolStripMenuItem.Size = New System.Drawing.Size(196, 26)
        Me.PorAutorizadorToolStripMenuItem.Text = "Por Autorizador"
        '
        'SalirToolStripMenuItem
        '
        Me.SalirToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.SalirManagerToolStripMenuItem})
        Me.SalirToolStripMenuItem.ForeColor = System.Drawing.Color.White
        Me.SalirToolStripMenuItem.Name = "SalirToolStripMenuItem"
        Me.SalirToolStripMenuItem.Size = New System.Drawing.Size(52, 24)
        Me.SalirToolStripMenuItem.Text = "Salir"
        '
        'SalirManagerToolStripMenuItem
        '
        Me.SalirManagerToolStripMenuItem.Name = "SalirManagerToolStripMenuItem"
        Me.SalirManagerToolStripMenuItem.Size = New System.Drawing.Size(184, 26)
        Me.SalirManagerToolStripMenuItem.Text = "Salir Manager"
        '
        'PictureBox1
        '
        Me.PictureBox1.Image = CType(resources.GetObject("PictureBox1.Image"), System.Drawing.Image)
        Me.PictureBox1.Location = New System.Drawing.Point(0, 28)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(1503, 114)
        Me.PictureBox1.TabIndex = 8
        Me.PictureBox1.TabStop = False
        '
        'CountQueueToolStripMenuItem
        '
        Me.CountQueueToolStripMenuItem.Name = "CountQueueToolStripMenuItem"
        Me.CountQueueToolStripMenuItem.Size = New System.Drawing.Size(224, 26)
        Me.CountQueueToolStripMenuItem.Text = "Count Queue"
        '
        'MainForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.Gainsboro
        Me.ClientSize = New System.Drawing.Size(1502, 956)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Controls.Add(Me.ListView1)
        Me.Controls.Add(Me.ListBox1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.KeyPreview = True
        Me.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.Name = "MainForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Switch&Service"
        Me.ContextMenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents ListBox1 As System.Windows.Forms.ListBox
    Friend WithEvents ListView1 As System.Windows.Forms.ListView
    Friend WithEvents ImageList1 As System.Windows.Forms.ImageList
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents ToolStripMenuItem1 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SistemaStartupToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents CommanderOpt As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ConnectItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ConfiguraciónToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents InstitucionesToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents Item_Process As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ISO8583V87ToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ISO8583V87ToolStripMenuItem1 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents TransaccionesToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents RuteoToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ServicioToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ReloadConfigurationToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SalirToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SalirManagerToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ShutDownToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents StartVerifyToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ContextMenuStrip1 As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents InicioProcesoToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents HideBlinkToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem2 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents StopProcessToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ShutdownProcessToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents TracesToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MinimoToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MediaToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents AltaToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ClearListToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
    Friend WithEvents OFFLineToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ONLineToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents HideAllToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents EstadosToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents DenegarToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents DeclinarToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents SwitchearToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents PorAutorizadorToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents CountQueueToolStripMenuItem As ToolStripMenuItem
End Class
