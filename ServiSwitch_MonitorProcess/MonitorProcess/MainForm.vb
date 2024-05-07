Imports System.Threading
Imports System.Runtime.InteropServices
Imports System.Messaging

Public Class MainForm

    Private Declare Auto Function SetProcessWorkingSetSize Lib "kernel32.dll" (ByVal procHandle As IntPtr, ByVal min As Int32, ByVal max As Int32) As Boolean

    Public Const SHOW_PROCESS As String = "0000"
    Public Const HIDE_PROCESS As String = "0002"
    Public Const SHUTDOWN_PROCESS As String = "0007"
    Public Const OFF_LINE As String = "0084"
    Public Const ON_LINE As String = "0085"

    Dim AUXILIAR As New SWTM_Auxiliar
    Dim BEHAVIOR As New SWTM_Behavior

    Private Sub MainForm_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown

        If (e.Control And e.KeyCode = Keys.B) Then
            Me.ListBox1.Items.Clear()
        End If

    End Sub

    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        CheckForIllegalCrossThreadCalls = False

        Me.Size = New Size(1143, 768)

        ' Init Database Resources
        InitDatabaseResources()

        Init_Task_Save_Log()

        If Create_SQL_Connection(0) <> 0 Then
            DisplayMessage("15#12#Sistema no pudo conectarse a la base de datos")
            Exit Sub
        Else
            DisplayMessage("15#0#Conexion a la base de datos, exitosa !!")
        End If

        ' Load List view Info
        LoadListViewData()

        Dim ParamsTable As New List(Of String)
        If GetInfoNode(My.Settings.SystemName, ParamsTable) = 0 Then
            CommandQueue = ParamsTable(14)
            ReplyQueue = ParamsTable(11)
            If AUXILIAR.Fill_Structure_Data(ParamsTable) <> 0 Then
                DisplayMessage("15#12#Carga de configuracion MANAGER No pudo ser procesada !!")
                Exit Sub
            End If
        Else
            DisplayMessage("Carga de configuracion MANAGER, no pudo ser procesada !!")
            Exit Sub
        End If

        BEHAVIOR.Init_Process_Status_Modules()

        ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf Main_Receiver_Commands))

        'ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf Process_Show_Mem_Resources))

        ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf Process_Analize_Times))

    End Sub

    Private Sub DisplayMessage(ByVal BufferMessage As String)

        If Me.ListBox1.Items.Count > 200 Then
            Me.ListBox1.Items.Clear()
        End If

        If BufferMessage = "*" Then
            Me.ListBox1.Items.Insert(0, " ")
        Else
            Me.ListBox1.Items.Insert(0, " " & BufferMessage)
        End If

    End Sub

    Private Sub LoadListViewData()
        Dim Task As Byte = 0
        Dim ModuleList As New List(Of String)

        Me.ListView1.Clear()
        GetModulesDefined(ModuleList)

        For x = 0 To (ModuleList.Count - 1)
            Dim Parms() As String = ModuleList(x).Split(",")
            Select Case Parms(1)
                Case "0"
                    Me.ListView1.Items.Add(Parms(0), 0)
                Case "1", "2", "5", "6"
                    Me.ListView1.Items.Add(Parms(0), 4)
                Case Else
                    Me.ListView1.Items.Add(Parms(0), 0)
            End Select
        Next

    End Sub

    Private Sub ListBox1_DrawItem(sender As Object, e As DrawItemEventArgs) Handles ListBox1.DrawItem
        Dim OwnColor1 As Color = Color.FromArgb(133, 133, 133)
        Dim OwnColor2 As Color = Color.FromArgb(220, 224, 215)
        Dim FORECOLOR As New SolidBrush(Color.Gray)
        Dim BACKCOLOR As New SolidBrush(Color.Black)
        Dim BACKCOLOR2 As New SolidBrush(OwnColor1)
        Dim BACKCOLOR3 As New SolidBrush(OwnColor2)
        Dim TextData As String

        SaveLogMain("ListBox1_DrawItem:" & ListBox1.Items(e.Index))

        If e.Index = -1 Then
            Exit Sub
        End If
        e.DrawBackground()

        Try
            Dim Parms() As String = ListBox1.Items(e.Index).Split("#")
            TextData = Parms(2)
            Select Case CInt(Parms(0))
                Case 0
                    BACKCOLOR = BACKCOLOR2
                Case 9
                    BACKCOLOR = Brushes.Navy
                Case 14
                    BACKCOLOR = Brushes.DarkRed
                Case 12
                    BACKCOLOR = Brushes.Red
                Case 7
                    BACKCOLOR = Brushes.Gray
                Case 10
                    BACKCOLOR = Brushes.Green
                Case 15
                    BACKCOLOR = Brushes.White
                Case 16
                    BACKCOLOR = Brushes.Yellow
                Case 17
                    BACKCOLOR = BACKCOLOR3
            End Select

            Select Case CInt(Parms(1))
                Case 0
                    FORECOLOR = Brushes.Black
                Case 9
                    FORECOLOR = Brushes.Navy
                Case 14
                    FORECOLOR = Brushes.DarkRed
                Case 12
                    FORECOLOR = Brushes.Red
                Case 7
                    FORECOLOR = Brushes.White
                Case 10
                    FORECOLOR = Brushes.GreenYellow
                Case 15
                    FORECOLOR = Brushes.White
                Case 16
                    FORECOLOR = Brushes.Yellow
            End Select

            ' Create rectangle.
            Dim rect As New Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height)
            ' Fill rectangle to screen.
            e.Graphics.FillRectangle(BACKCOLOR, rect)
            e.Graphics.DrawString(TextData, e.Font, FORECOLOR, New RectangleF(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height))
            e.DrawFocusRectangle()
        Catch ex As Exception
            SaveLogMain("Metodo:" & System.Reflection.MethodBase.GetCurrentMethod.Name.ToString & " Error:" & ex.Message)
        End Try
    End Sub

    Private Sub SistemaStartupToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SistemaStartupToolStripMenuItem.Click

        ' Load List view Info
        LoadListViewData()

    End Sub


    Private Sub ConnectItem_Click(sender As Object, e As EventArgs) Handles ConnectItem.Click
        Dim ProgramName As String = Nothing

        BEHAVIOR.Abort_Threads()

        Thread.Sleep(1000)
        AUXILIAR.Killing_ServiSwitch()

        Thread.Sleep(1000)
        BEHAVIOR.Init_Process_Status_Modules()

        Dim ModuleList As New List(Of String)
        Dim ModuleFields As New List(Of String)
        GetModulesDefined(ModuleList)

        For x As Integer = 0 To ModuleList.Count - 1
            Dim Parms() As String = ModuleList(x).Split(",")
            ModuleFields.Clear()
            If GetInfoMainData(Parms(0).Trim, ModuleFields) = 0 Then
                ProgramName = ModuleFields(19).Substring(ModuleFields(19).LastIndexOf("\") + 1)
                BEHAVIOR.Process_Start_Module(Parms(0).Trim, ProgramName)
            End If
        Next

    End Sub


    Private Sub Main_Receiver_Commands(ByVal id As Int16)
        Dim CommandData As String

        Dim filter = New MessagePropertyFilter()
        With filter
            .AdministrationQueue = False
            .ArrivedTime = True
            .CorrelationId = False
            .Priority = False
            .ResponseQueue = False
            .SentTime = True
            .Body = True
            .Label = True
            .Id = True
        End With
        '********************************************************************************
        '********************************************************************************
        Do While True
            Try
                Dim TS_MessageQueue As New MessageQueue(".\Private$\" & CommandQueue)
                TS_MessageQueue.MessageReadPropertyFilter = filter
                TS_MessageQueue.Formatter = New XmlMessageFormatter(New Type() {GetType(InfoCommands)})
                Dim myMessage As Message = TS_MessageQueue.Receive
                Dim Struct_Queue_Message As InfoCommands = CType(myMessage.Body, InfoCommands)
                CommandData = Struct_Queue_Message.ICM_CommandBuffer
                '********************************************************************************
                Dim DateTimeEND As DateTime = Now
                Dim DiffQueueTime = DateDiff(DateInterval.Second, myMessage.ArrivedTime, DateTimeEND)
                If DiffQueueTime > 5 Then
                    'Show_Message_Console("@", COLOR_BLACK, COLOR_WHITE, 3, TRACE_LOW)
                    Continue Do
                End If
                '********************************************************************************
                'SaveLogMain("Main_Receiver_Commands -->" & CommandData & " Len:" & CommandData.Length)

                _____PROCESS_COMMAND_MESSAGE____(CommandData)

                ClearMemory()
                '********************************************************************************
                '********************************************************************************
            Catch Sn As System.NullReferenceException
                SaveLogMain("Metodo:" & System.Reflection.MethodBase.GetCurrentMethod.Name.ToString & " Error:" & Sn.Message)
                GoTo Exiting_Thread
            Catch ex As Exception
                SaveLogMain("Metodo:" & System.Reflection.MethodBase.GetCurrentMethod.Name.ToString & " Error:" & ex.Message)
                GoTo Exiting_Thread
            End Try
        Loop
Exiting_Thread:
        ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf Main_Receiver_Commands), id)

    End Sub

    Private Sub _____PROCESS_COMMAND_MESSAGE____(ByVal CommandData As String)

        Dim Parms() As String = CommandData.Split("|")

        Select Case Parms(0)
            Case Lmanager_DISPLAY
                DisplayMessage(Parms(1))
            Case Lmanager_REFRESH, Lmanager_STATUS
                Update_Listview_Control(Parms(1), CInt(Parms(2)))
        End Select

    End Sub

    Private Sub Update_Listview_Control(ByVal InterfaceName As String, ByVal ImageIndex As Byte)

        Try
            Dim item1 As ListViewItem = ListView1.FindItemWithText(InterfaceName)
            'Dim ModuleType As Byte = GetModuleType(InterfaceName)
            Dim idx As Byte = item1.Index
            item1.ImageIndex = ImageIndex
            Dim CL As Color
            Select Case ImageIndex
                Case 0, 1
                    CL = Color.White
                Case 2, 5
                    CL = Color.DarkGreen
                Case 3
                    CL = Color.FromArgb(237, 88, 33)
            End Select

            With ListView1
                .BeginUpdate()
                .Items(idx) = item1
                .Items(idx).ForeColor = Color.Black
                .EndUpdate()
                .Refresh()
            End With
        Catch ex As Exception
            SaveLogMain("Metodo:" & System.Reflection.MethodBase.GetCurrentMethod.Name.ToString & " Error:" & ex.Message)
        End Try

    End Sub

    Private Sub StartVerifyToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles StartVerifyToolStripMenuItem.Click

        BEHAVIOR.Abort_Threads()
        BEHAVIOR.Init_Process_Status_Modules()

    End Sub

    Private Sub SalirManagerToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SalirManagerToolStripMenuItem.Click

        Dim Resp As MsgBoxResult
        Resp = MessageBox.Show("Esta usted seguro que desea salir ?", "Salir", MessageBoxButtons.OKCancel)

        If Resp = MsgBoxResult.Cancel Then
            Exit Sub
        End If

        Try
            Dim Mem As Process = Process.GetCurrentProcess()
            Mem.Kill()
        Catch ex As Exception
            Me.Close()
        End Try

    End Sub

    Private Sub Item_Process_Click(sender As Object, e As EventArgs) Handles Item_Process.Click

        Dim ShowForm As New SWTM_MainFile
        ShowForm.ShowDialog()

    End Sub

    Private Sub ShutDownToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ShutDownToolStripMenuItem.Click
        Dim Resp As MsgBoxResult
        Resp = MessageBox.Show("Esta usted seguro que desea abortar todos los procesos ?", "Shutdown", MessageBoxButtons.OKCancel)

        If Resp = MsgBoxResult.Cancel Then
            Exit Sub
        End If

        BEHAVIOR.Abort_Threads()

        AUXILIAR.Killing_ServiSwitch()
        LoadListViewData()

    End Sub

    Private Sub HideBlinkToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles HideBlinkToolStripMenuItem.Click
        Try
            If IsNothing(Me.ListView1.SelectedItems(0).Text) Then
                Exit Sub
            End If
        Catch ex As Exception
            Exit Sub
        End Try
        Dim CommandMessage As String
        CommandMessage = "CMD|" & Me.ListView1.SelectedItems(0).Text & "|" & SHOW_PROCESS
        BEHAVIOR.Put_Message_To_Module(CommandMessage, BEHAVIOR.Get_Queue_To_Send(Me.ListView1.SelectedItems(0).Text.ToString.Trim), 0)
    End Sub

    Private Sub ToolStripMenuItem2_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem2.Click
        Try
            If IsNothing(Me.ListView1.SelectedItems(0).Text) Then
                Exit Sub
            End If
        Catch ex As Exception
            SaveLogMain("Metodo:" & System.Reflection.MethodBase.GetCurrentMethod.Name.ToString & Chr(13) & "Error:" & ex.Message)
            Exit Sub
        End Try
        Dim CommandMessage As String
        CommandMessage = "CMD|" & Me.ListView1.SelectedItems(0).Text & "|" & HIDE_PROCESS
        BEHAVIOR.Put_Message_To_Module(CommandMessage, BEHAVIOR.Get_Queue_To_Send(Me.ListView1.SelectedItems(0).Text.ToString.Trim), 0)
    End Sub

    Private Sub InicioProcesoToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles InicioProcesoToolStripMenuItem.Click
        Dim PProcessName As String
        Dim PathProcessName As String
        Try
            If IsNothing(Me.ListView1.SelectedItems(0).Text) Then
                Exit Sub
            End If
        Catch ex As Exception
            Exit Sub
        End Try
        PProcessName = Me.ListView1.SelectedItems(0).Text.ToString.Trim

        Try
            Dim ServiProcess As List(Of Process) = (From p As Process In Process.GetProcesses Where p.ProcessName.ToUpper Like "ServiSwitch*".ToUpper).ToList
            For Each p As Process In ServiProcess
                PathProcessName = p.MainModule.FileName()
                'Dim VsHost As Boolean = PathProcessName.Contains("vshost")
                'If (PathProcessName.Contains("\" & PProcessName & "\")) And (VsHost = False) Then
                If (PathProcessName.Contains("\" & PProcessName & "\")) Then
                    Dim Legend As String = "Ya existe un proceso en memoria con esa caracteristica" & Chr(13)
                    Legend += "PROCESSID  :" & p.Id & Chr(13)
                    Legend += "PATHNAME" & Chr(13) & p.MainModule.FileName() & Chr(13)
                    Dim LastAck As DateTime = BEHAVIOR.Get_Last_Reply(PProcessName)
                    Legend += "LAST REPLY :" & LastAck.ToShortDateString & " " & LastAck.ToLongTimeString
                    MessageBox.Show(Legend, "Start Process", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Exit Sub
                End If
            Next
        Catch ex As Exception
            MessageBox.Show("Excepcion:" & ex.Message, "Start Process", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        End Try

        Dim ProgramName As String = BEHAVIOR.Get_Path_Name(PProcessName)
        BEHAVIOR.Process_Start_Module(PProcessName, ProgramName)

    End Sub

    Private Sub StopProcessToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles StopProcessToolStripMenuItem.Click
        Dim PProcessName As String
        Try
            If IsNothing(Me.ListView1.SelectedItems(0).Text) Then
                Exit Sub
            End If
        Catch ex As Exception
            SaveLogMain("Metodo:" & System.Reflection.MethodBase.GetCurrentMethod.Name.ToString & " Error:" & ex.Message)
            Exit Sub
        End Try

        PProcessName = Me.ListView1.SelectedItems(0).Text.ToString.Trim
        Dim Resp As MsgBoxResult
        Resp = MessageBox.Show("Esta usted seguro que desea abortar el proceso " & PProcessName & " ?", "Abort", MessageBoxButtons.OKCancel)

        If Resp = MsgBoxResult.Cancel Then
            Exit Sub
        End If

        If BEHAVIOR.Killing_ServiSwitch(PProcessName) <> SUCCESSFUL Then
            MessageBox.Show("No se pudo abortar el proceso", "Abort", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Else
            Update_Listview_Control(PProcessName, 0)
        End If

    End Sub

    Private Sub ShutdownProcessToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ShutdownProcessToolStripMenuItem.Click

        Dim ASK As DialogResult = MessageBox.Show(" Esta seguro de ejecutar SHUTDOWN al proceso " & Me.ListView1.SelectedItems(0).Text, " Shutdown", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If ASK = Windows.Forms.DialogResult.No Then
            Exit Sub
        End If

        Try
            If IsNothing(Me.ListView1.SelectedItems(0).Text) Then
                Exit Sub
            End If
        Catch ex As Exception
            Exit Sub
        End Try

        Dim CommandMessage As String
        CommandMessage = "CMD|" & Me.ListView1.SelectedItems(0).Text & "|" & SHUTDOWN_PROCESS
        BEHAVIOR.Put_Message_To_Module(CommandMessage, BEHAVIOR.Get_Queue_To_Send(Me.ListView1.SelectedItems(0).Text.ToString.Trim), 0)
        Update_Listview_Control(Me.ListView1.SelectedItems(0).Text.ToString.Trim, 0)

    End Sub

    'Private Sub Process_Show_Mem_Resources()
    '    Dim ProcessMem() As Int32
    '    Dim ProcessName() As String
    '    Dim ModuleList As New List(Of String)
    '    Dim ValMem As Int32
    '    Dim LastReply As DateTime
    '    Get_Process_UP(ModuleList)
    '    ProcessName = ModuleList.ToArray()
    '    ReDim ProcessMem(ProcessName.GetUpperBound(0))
    '    Fill_Process_Label(ProcessName)
    '    '******************************************************************
    '    '******************************************************************
    '    Do While True
    '        For x As Int16 = 0 To ProcessName.GetUpperBound(0)
    '            ValMem = BEHAVIOR.Get_MemResource(ProcessName(x))
    '            Update_Status_Bar(x, ValMem)
    '            LastReply = BEHAVIOR.Get_Last_Reply(ProcessName(x))
    '            Update_Time_Label(x, LastReply)
    '        Next
    '        Thread.Sleep(5000)
    '    Loop
    '    '******************************************************************
    '    '******************************************************************
    'End Sub

    'Private Sub Update_Status_Bar(ByVal Index As Byte, ByVal ValMem As Int32)
    '    Dim NewVal1, NewVal2 As Decimal
    '    Select Case Index
    '        Case 0
    '            Me.lbl_back1.Text = ValMem.ToString & " KB"
    '            NewVal1 = (ValMem * 100) / 20000
    '            NewVal2 = (NewVal1 * 100) / 250
    '            Me.lbl_prog1.Size = New Size(NewVal2, 13)
    '        Case 1
    '            Me.lbl_back2.Text = ValMem.ToString & " KB"
    '            NewVal1 = (ValMem * 100) / 20000
    '            NewVal2 = (NewVal1 * 100) / 250
    '            Me.lbl_prog2.Size = New Size(NewVal2, 13)
    '        Case 2
    '            Me.lbl_back3.Text = ValMem.ToString & " KB"
    '            NewVal1 = (ValMem * 100) / 20000
    '            NewVal2 = (NewVal1 * 100) / 250
    '            Me.lbl_prog3.Size = New Size(NewVal2, 13)
    '        Case 3
    '            Me.lbl_back4.Text = ValMem.ToString & " KB"
    '            NewVal1 = (ValMem * 100) / 20000
    '            NewVal2 = (NewVal1 * 100) / 250
    '            Me.lbl_prog4.Size = New Size(NewVal2, 13)
    '        Case 4
    '            Me.lbl_back5.Text = ValMem.ToString & " KB"
    '            NewVal1 = (ValMem * 100) / 20000
    '            NewVal2 = (NewVal1 * 100) / 250
    '            Me.lbl_prog5.Size = New Size(NewVal2, 13)
    '        Case 5
    '            Me.lbl_back6.Text = ValMem.ToString & " KB"
    '            NewVal1 = (ValMem * 100) / 20000
    '            NewVal2 = (NewVal1 * 100) / 250
    '            Me.lbl_prog6.Size = New Size(NewVal2, 13)
    '    End Select
    'End Sub

    'Private Sub Fill_Process_Label(ByVal ProcessName() As String)

    '    For x As Int16 = 0 To ProcessName.GetUpperBound(0)
    '        Select Case x
    '            Case 0
    '                Me.lbl_Pname1.Text = ProcessName(x)
    '            Case 1
    '                Me.lbl_Pname2.Text = ProcessName(x)
    '            Case 2
    '                Me.lbl_Pname3.Text = ProcessName(x)
    '            Case 3
    '                Me.lbl_Pname4.Text = ProcessName(x)
    '            Case 4
    '                Me.lbl_Pname5.Text = ProcessName(x)
    '            Case 5
    '                Me.lbl_Pname6.Text = ProcessName(x)
    '        End Select
    '    Next

    'End Sub

    'Private Sub Update_Time_Label(ByVal Index As Byte, ByVal DT As DateTime)
    '    Select Case Index
    '        Case 0
    '            Me.lbl_time_1.Text = DT.ToString("yyyy-MM-dd HH:mm:ss.fff")
    '        Case 1
    '            Me.lbl_time_2.Text = DT.ToString("yyyy-MM-dd HH:mm:ss.fff")
    '        Case 2
    '            Me.lbl_time_3.Text = DT.ToString("yyyy-MM-dd HH:mm:ss.fff")
    '        Case 3
    '            Me.lbl_time_4.Text = DT.ToString("yyyy-MM-dd HH:mm:ss.fff")
    '        Case 4
    '            Me.lbl_time_5.Text = DT.ToString("yyyy-MM-dd HH:mm:ss.fff")
    '        Case 5
    '            Me.lbl_time_6.Text = DT.ToString("yyyy-MM-dd HH:mm:ss.fff")
    '    End Select
    'End Sub

    Friend Sub ClearMemory()
        Try
            GC.Collect()
            GC.WaitForPendingFinalizers()
            If Environment.OSVersion.Platform = PlatformID.Win32NT Then
                SetProcessWorkingSetSize(System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1)
            End If
        Catch ex As Exception

        End Try
    End Sub


    Private Sub ISO8583V87ToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles ISO8583V87ToolStripMenuItem1.Click

        Dim ShowForm As New SWTM_ISO8583V87
        ShowForm.ShowDialog()

    End Sub

    Private Sub ClearListToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ClearListToolStripMenuItem.Click

        Me.ListBox1.Items.Clear()

    End Sub

    Private Sub InstitucionesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles InstitucionesToolStripMenuItem.Click

        Dim ShowForm As New SWTM_Institution
        ShowForm.ShowDialog()

    End Sub

    Private Sub TransaccionesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles TransaccionesToolStripMenuItem.Click

        Dim ShowForm As New SWTM_Transaction
        ShowForm.ShowDialog()

    End Sub

    Private Sub RuteoToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RuteoToolStripMenuItem.Click

        Dim ShowForm As New SWTM_Routing
        ShowForm.ShowDialog()

    End Sub

    Private Sub ServicioToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ServicioToolStripMenuItem.Click

        Dim ShowForm As New SWTM_Services
        ShowForm.ShowDialog()

    End Sub

    Private Sub OFFLineToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OFFLineToolStripMenuItem.Click
        Try
            If IsNothing(Me.ListView1.SelectedItems(0).Text) Then
                Exit Sub
            End If
        Catch ex As Exception
            Exit Sub
        End Try
        Dim CommandMessage As String
        CommandMessage = "CMD|" & Me.ListView1.SelectedItems(0).Text & "|" & OFF_LINE
        BEHAVIOR.Put_Message_To_Module(CommandMessage, BEHAVIOR.Get_Queue_To_Send(Me.ListView1.SelectedItems(0).Text.ToString.Trim), 0)
    End Sub

    Private Sub ONLineToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ONLineToolStripMenuItem.Click
        Try
            If IsNothing(Me.ListView1.SelectedItems(0).Text) Then
                Exit Sub
            End If
        Catch ex As Exception
            Exit Sub
        End Try
        Dim CommandMessage As String
        CommandMessage = "CMD|" & Me.ListView1.SelectedItems(0).Text & "|" & ON_LINE
        BEHAVIOR.Put_Message_To_Module(CommandMessage, BEHAVIOR.Get_Queue_To_Send(Me.ListView1.SelectedItems(0).Text.ToString.Trim), 0)
    End Sub

    Private Sub HideAllToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles HideAllToolStripMenuItem.Click

        Dim ModuleList As New List(Of String)
        GetModulesDefined(ModuleList)

        For x As Int16 = 0 To (ModuleList.Count - 1)
            Dim CommandMessage As String
            Dim ProcessName As String = ModuleList(x).Substring(0, ModuleList(x).IndexOf(","))
            CommandMessage = "CMD|" & ProcessName & "|" & HIDE_PROCESS
            BEHAVIOR.Put_Message_To_Module(CommandMessage, BEHAVIOR.Get_Queue_To_Send(ProcessName), 0)
        Next

    End Sub

    Private Sub DeclinarToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DeclinarToolStripMenuItem.Click

        Dim ASK As DialogResult = MessageBox.Show(" Esta seguro de RECHAZAR todas las transacciones ?", " Declinear", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If ASK = Windows.Forms.DialogResult.No Then
            Exit Sub
        End If

        Dim MessageToSend As String
        MessageToSend = Router_ORDER & "1|0|0"
        BEHAVIOR.Put_Notify_To_Router(MessageToSend)

    End Sub

    Private Sub SwitchearToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SwitchearToolStripMenuItem.Click
        Dim ASK As DialogResult = MessageBox.Show(" Esta seguro de HABILITAR el switcheo de transacciones ?", " Switchear", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If ASK = Windows.Forms.DialogResult.No Then
            Exit Sub
        End If

        Dim MessageToSend As String
        MessageToSend = Router_ORDER & "0|0|0"
        BEHAVIOR.Put_Notify_To_Router(MessageToSend)
    End Sub

    Private Sub PorAutorizadorToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PorAutorizadorToolStripMenuItem.Click

        Dim ShowForm As New SWTM_EnableDisable
        ShowForm.ShowDialog()

    End Sub

    Private Sub Process_Analize_Times()
        'SaveLogMain("Ingresando a Process_Analize_Times " & Now.Millisecond)
        Thread.Sleep(10000)
        'SaveLogMain("sleep a Process_Analize_Times " & Now.Millisecond)

        Dim x As Int16 = 0
        Do While True
            Dim dt1 As DateTime
            'SaveLogMain("Get_Value_Process " & aNamesP(x))
            dt1 = Get_Value_Process(aNamesP(x))
            Dim TSP As New TimeSpan
            TSP = Now.Subtract(dt1)
            If TSP.TotalSeconds >= 10 Then
                Update_Listview_Control(aNamesP(x), 0)
            End If
            Thread.Sleep(1000)
            If x = aNamesP.Count - 1 Then
                x = 0
            Else
                x += 1
            End If
        Loop

    End Sub

    Private Sub CountQueueToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CountQueueToolStripMenuItem.Click

        Dim ModuleList As New List(Of String)
        Dim ModuleFields As New List(Of String)
        GetModulesDefined(ModuleList)
        Dim Counter As Int32 = 0
        For x As Integer = 0 To ModuleList.Count - 1
            Dim Parms() As String = ModuleList(x).Split(",")
            ModuleFields.Clear()
            If GetInfoMainData(Parms(0).Trim, ModuleFields) = 0 Then
                '--------------------------------------------------------------------------------------
                Dim lQUEUE As New MessageQueue(PrivateQueue & ModuleFields(9))
                Counter = lQUEUE.GetAllMessages.Count
                DisplayMessage("15#0#" & GetDateTime() & Chr(11) & Parms(0).PadRight(13, " ") & Chr(11) & ModuleFields(9) & Chr(11) & " MSMQ --> " & Counter)
                '--------------------------------------------------------------------------------------
                lQUEUE = New MessageQueue(PrivateQueue & ModuleFields(10))
                Counter = lQUEUE.GetAllMessages.Count
                DisplayMessage("15#0#" & GetDateTime() & Chr(11) & Parms(0).PadRight(13, " ") & Chr(11) & ModuleFields(10) & Chr(11) & " MSMQ --> " & Counter)
                '--------------------------------------------------------------------------------------
                lQUEUE = New MessageQueue(PrivateQueue & ModuleFields(11))
                Counter = lQUEUE.GetAllMessages.Count
                DisplayMessage("15#0#" & GetDateTime() & Chr(11) & Parms(0).PadRight(13, " ") & Chr(11) & ModuleFields(11) & Chr(11) & " MSMQ --> " & Counter)
                '--------------------------------------------------------------------------------------
                lQUEUE = New MessageQueue(PrivateQueue & ModuleFields(12))
                Counter = lQUEUE.GetAllMessages.Count
                DisplayMessage("15#0#" & GetDateTime() & Chr(11) & Parms(0).PadRight(13, " ") & Chr(11) & ModuleFields(12) & Chr(11) & " MSMQ --> " & Counter)
                '--------------------------------------------------------------------------------------
                lQUEUE = New MessageQueue(PrivateQueue & ModuleFields(13))
                Counter = lQUEUE.GetAllMessages.Count
                DisplayMessage("15#0#" & GetDateTime() & Chr(11) & Parms(0).PadRight(13, " ") & Chr(11) & ModuleFields(13) & Chr(11) & " MSMQ --> " & Counter)
                '--------------------------------------------------------------------------------------
                lQUEUE = New MessageQueue(PrivateQueue & ModuleFields(16))
                Counter = lQUEUE.GetAllMessages.Count
                DisplayMessage("15#0#" & GetDateTime() & Chr(11) & Parms(0).PadRight(13, " ") & Chr(11) & ModuleFields(16) & Chr(11) & " MSMQ --> " & Counter)
                '--------------------------------------------------------------------------------------
            End If
        Next
    End Sub

    Private Sub ListBox1_DoubleClick(sender As Object, e As EventArgs) Handles ListBox1.DoubleClick

        Dim SelectedLine As String = Me.ListBox1.SelectedItem.ToString
        Dim aSEL() As String = SelectedLine.Split(Chr(11))
        If aSEL.Length = 4 Then
            If aSEL(3).Contains("MSMQ -->") Then
                Dim Resp As MsgBoxResult
                Resp = MessageBox.Show("Esta usted seguro que quiere purgar " & aSEL(1).TrimEnd & " - " & aSEL(2).TrimEnd & " ?", "Purge", MessageBoxButtons.YesNo)
                If Resp = MsgBoxResult.No Then
                    Exit Sub
                Else
                    Dim lQUEUE As New MessageQueue(PrivateQueue & aSEL(2))
                    lQUEUE.Purge()
                    MsgBox("Queue ha sido truncada......")
                End If
            End If
        End If

    End Sub
End Class

