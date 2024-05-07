Imports System.Threading
Imports System.Messaging
Imports System.Threading.Tasks

Public Class ServiSwitch_ProcessCommand

    Dim Processing As New ServiSwitch_ProcessMessages
    Public Thread_CMND As Thread

    Public Sub Start_Task_Process_Command()
        Thread_CMND = New Thread(AddressOf Main_Receiver_Commands)
        Thread_CMND.Name = "Command"
        Thread_CMND.Start()

        ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf Process_Send_Status_Commander))

    End Sub

    Private Sub Process_Send_Status_Commander()

        Do While True
            Dim CommandLine As String = "STA|" & MyName & "|"
            'Dim ImgIdx As Byte = 2
            Dim ImgIdx As Byte = Get_Socket_Status()
            CommandLine += GetDateTime() & "|" & ImgIdx & "|" & ValMem
            Put_Message_To_Manager(CommandLine, g_CommanderQueue2)
            ClearMemory()
            Thread.Sleep(5000)
        Loop
    End Sub

    Public Sub Send_Status_Commander(ByVal ImgIdx As Byte)
        Dim CommandLine As String = "STA|" & MyName & "|" & GetDateTime() & "|" & ImgIdx & "|" & ValMem
        Put_Message_To_Manager(CommandLine, g_CommanderQueue2)
    End Sub


    Private Sub Main_Receiver_Commands()
        Dim CommandData As String
        Dim Legend As String = MyName & " Init Process Command,  Main TaskId:" & System.Threading.Thread.CurrentThread.ManagedThreadId
        Show_Message_Console(Legend, COLOR_BLACK, COLOR_GRAY, 0, TRACE_LOW, 1)
        Legend = Nothing
        '****************************************************************************************
        '****************************************************************************************
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
        '****************************************************************************************
        '****************************************************************************************
        Do While True
            Try
                Dim TS_MessageQueue As New MessageQueue(g_CmdQueue)
                TS_MessageQueue.MessageReadPropertyFilter = filter
                TS_MessageQueue.Formatter = New XmlMessageFormatter(New Type() {GetType(InfoCommands)})
                Dim myMessage As Message = TS_MessageQueue.Receive
                Dim Struct_Queue_Message As InfoCommands = CType(myMessage.Body, InfoCommands)
                CommandData = Struct_Queue_Message.ICM_CommandBuffer
                '********************************************************************************
                '********************************************************************************
                Dim DiffQueueTime = DateDiff(DateInterval.Second, myMessage.ArrivedTime, DateTime.Now)
                If DiffQueueTime > 5 Then
                    Console.Write("*")
                    Continue Do
                End If
                '********************************************************************************
                '********************************************************************************

                _____PROCESS_COMMAND_MESSAGE____(CommandData)

                '********************************************************************************
                '********************************************************************************
            Catch Sn As System.NullReferenceException
                If Exiting = True Then
                    GoTo Exiting_Thread
                End If
                Show_Message_Console(MyName & " Excepion in Main_Receiver_Commands1: " & Sn.Message & " - " & Sn.StackTrace, COLOR_OWNER1, COLOR_RED, 0, TRACE_LOW, 1)
            Catch ex As Exception
                If Exiting = True Then
                    GoTo Exiting_Thread
                End If
                Show_Message_Console(MyName & " Excepion in Main_Receiver_Commands2: " & ex.Message & " - " & ex.StackTrace, COLOR_OWNER1, COLOR_RED, 0, TRACE_LOW, 1)
                GoTo Exiting_Thread
            End Try
        Loop
Exiting_Thread:
        Show_Message_Console(MyName & " Commands Ending " & System.Threading.Thread.CurrentThread.ManagedThreadId, COLOR_BLACK, COLOR_GRAY, 0, TRACE_LOW, 1)
        Thread.Sleep(1000)
        If Exiting = False Then
            ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf Main_Receiver_Commands))
        End If

    End Sub

    Public Async Sub _____PROCESS_COMMAND_MESSAGE____(ByVal CommandMessage As String)
        'Show_Message_Console("From Commander:" & CommandMessage, COLOR_BLACK, COLOR_GRAY, 0, TRACE_LOW, 0)
        'Console.Write(".")
        Dim ErrorCode As Byte
        Await Task.Run(Async Function()
                           ErrorCode = Await Evaluate_and_Authorize_Command(CommandMessage)
                       End Function)
    End Sub

    Private Async Function Evaluate_and_Authorize_Command(ByVal CommandLine As String) As Task(Of Byte)
        Await Task.Delay(0)
        'Console.WriteLine("Step2")
        Dim Parms() As String = CommandLine.Split("|")

        Select Case Parms(0)
            Case "REP"
                Process_Command_Reply(CommandLine)
            Case "CMD"
                Process_Command_Request(CommandLine)
            Case "STA"
                Evaluate_Status_Received(CommandLine)
        End Select
        Return 0
    End Function

    Private Sub Evaluate_Status_Received(ByVal CommandLine As String)

        Dim ImgIdx As Byte = Get_Socket_Status()

        CommandLine += GetDateTime() & "|" & ImgIdx & "|" & ValMem
        Put_Message_To_Manager(CommandLine, g_CommanderQueue)
        'Console.Write(CommandLine)
        'Show_Message_Console(MyName & " " & CommandLine, COLOR_BLACK, COLOR_DARK_GRAY, 0, TRACE_LOW, 0)

    End Sub


    'Private Sub _____PROCESS_COMMAND_MESSAGE____(ByVal CommandLine As String)
    '    Dim Parms() As String = CommandLine.Split(New String() {"|"}, StringSplitOptions.RemoveEmptyEntries)

    '    Select Case Parms(0)
    '        Case "REP"
    '            Process_Command_Reply(CommandLine)
    '        Case "CMD"
    '            Process_Command_Request(CommandLine)
    '    End Select
    'End Sub


    Private Sub Process_Command_Request(ByVal CommandLine As String)
        Dim Parms() As String = CommandLine.Split("|")

        Dim Legend As String = String.Empty

        If IsNumeric(Parms(2)) Then
            Select Case CInt(Parms(2))
                Case 0
                    Legend = " (Show_Process)"
                Case 1
                    Legend = " (Blink)"
                Case 2
                    Legend = " (Hide_Window)"
                Case 3
                    Legend = " (Window_Normal)"
                Case 4
                    Legend = " (End_Task)"
                Case 5
                    Legend = " (Abort_Task)"
                Case 6
                    Legend = " (Giveme process info)"
                Case 7
                    Legend = " (Shutdown_Process)"
                Case 8
                    Legend = " (Reload configuration)"
                Case 81
                    Legend = " (View low detail)"
                Case 82
                    Legend = " (View medium detail)"
                Case 83
                    Legend = " (View high detail)"
            End Select
        Else
            Select Case CInt(Parms(1))
                Case 61
                    Legend = " (End Task)"
                Case 62
                    Legend = " (Reset Task)"
                Case 63
                    Legend = " (Start Task)"
            End Select

        End If

        Show_Message_Console(MyName & " receive command " & Legend, COLOR_BLACK, COLOR_GRAY, 0, TRACE_LOW, 1)
        SaveLogMain("Receive and Executing command" & Legend)

        If IsNumeric(Parms(2)) Then
            Select Case CInt(Parms(2))
                Case 0
                    ShowWindow(GetConsoleWindow(), SW_SHOWMINNOACTIVE)
                    ShowWindow(GetConsoleWindow(), SW_SHOWNORMAL)
                Case 1
                    FlashWindowNormal(Process.GetCurrentProcess().MainWindowHandle)
                Case 2
                    ShowWindow(GetConsoleWindow(), SW_HIDE)
                Case 3
                    ShowWindow(GetConsoleWindow(), SW_SHOWNORMAL)
                Case 4
                    ProcessEndTask(CommandLine)
                Case 6
                    'Get_Current_Data_Process(CommandLine)
                Case 7
                    ProcessShutdownModule()
                Case 8
                    'Load_Event_Multilevel_Data()
                Case 81, 82, 83
                    ProcessViewDetail(CommandLine)
            End Select
        Else
            Select Case CInt(Parms(1))
                Case 61
                    ProcessEndTask(CommandLine)
                Case 62
                    ProcessEndTask(CommandLine)
                    Thread.Sleep(1000)
                    Processing.ProcessStartTask(CommandLine)
                Case 63
                    Processing.ProcessStartTask(CommandLine)
            End Select
        End If

    End Sub

    Private Sub Process_Command_Reply(ByVal CommandLine As String)
        Dim Parms() As String = CommandLine.Split(New String() {"|"}, StringSplitOptions.RemoveEmptyEntries)
        Select Case Parms(1)
            Case rep_GIVE_MAIN_INFO
                'If Fill_Main_Info(0, Parms) = 0 Then
                'Notify_Load_Data()
                'End If
            Case rep_GIVE_COMD_DATA
                'If Fill_Main_Info(1, Parms) = 0 Then
                'Notify_Load_Data()
                'End If
            Case rep_GIVE_ROUTER_DATA
                'If Fill_Main_Info(2, Parms) = 0 Then
                'Notify_Load_Data()
                'End If
            Case rep_GET_TRAN_CONFIG
                'Fill_Transaction_Definition(CommandLine)
                'Notify_Load_Data()
            Case rep_GET_ALL_CONFIG
                'ProcessRefreshConfiguration(CommandLine)
        End Select
    End Sub

    'Private Sub ProcessRefreshConfiguration(ByVal CommandLine As String)
    '    Dim LocParms() As String
    '    Dim Parms() As String = CommandLine.Split("|")
    '    Parms = Parms(2).Split(Chr(13))
    '    LocParms = Parms(0).Split("#")
    '    If RefFill_Main_Info(0, LocParms) = 0 Then
    '        Show_Message_Console(MyName & " Information of " & LocParms(0) & " updated", COLOR_BLACK, COLOR_GRAY, 0, TRACE_LOW, 1)
    '    End If
    '    LocParms = Parms(1).Split("#")
    '    If RefFill_Main_Info(1, LocParms) = 0 Then
    '        Show_Message_Console(MyName & " Information of " & LocParms(0) & " updated", COLOR_BLACK, COLOR_GRAY, 0, TRACE_LOW, 1)
    '    End If
    '    LocParms = Parms(2).Split("#")
    '    If RefFill_Main_Info(2, LocParms) = 0 Then
    '        Show_Message_Console(MyName & " Information of " & LocParms(0) & " updated", COLOR_BLACK, COLOR_GRAY, 0, TRACE_LOW, 1)
    '    End If
    '    If ReFill_Transaction_Definition(Parms(4)) = 0 Then
    '        Show_Message_Console(MyName & " Information of Transaction table updated", COLOR_BLACK, COLOR_GRAY, 0, TRACE_LOW, 1)
    '    End If
    '    If GetPathQueueName(GetRouterName(0), 0, RouterRequesterQueue) = 0 Then
    '        Show_Message_Console(MyName & " Process update Sucessful ->" & RouterRequesterQueue, COLOR_BLACK, COLOR_GRAY, 0, TRACE_LOW, 1)
    '    End If
    'End Sub

    Private Sub ProcessEndTask(ByVal CommandLine As String)
        Dim x As Int16
        Dim Task As Int16
        'Dim Length As Int16
        Dim Parms() As String = CommandLine.Split("|")
        'Dim Command_In_String As String = "CMD" & Task_Stop.ToString("0000") & "|" & Parms(1)
        Dim Command_In_Bytes As Byte()
        'Length = CommandLine.Length + 2
        Command_In_Bytes = System.Text.Encoding.ASCII.GetBytes(CommandLine)
        'Command_In_Bytes(0) = Length >> 8
        'Command_In_Bytes(1) = Length And 255
        'Task = GetTaskNumber(MyName)
        Select Case Parms(4)
            Case "request"
                For x = 0 To Task
                    Put_Message_Socket_Request(Command_In_Bytes.Length, Command_In_Bytes, 0)
                Next
            Case "reply"
                For x = 0 To Task
                    Put_Message_Queue_Reply(Task, CommandLine)
                Next
        End Select
    End Sub


    Private Sub ProcessViewDetail(ByVal CommandLine As String)
        Dim Parms() As String = CommandLine.Split("|")
        Dim LevelDetail As String = String.Empty

        Try
            If CInt(Parms(2)) = User_Detail Then
                Exit Sub
            End If
            Select Case CInt(Parms(2))
                Case 81
                    LevelDetail = " Low "
                Case 82
                    LevelDetail = " Medium "
                Case 83
                    LevelDetail = " High "
            End Select
            User_Detail = CInt(Parms(2))
            Show_Message_Console(MyName & " Level detail changed to " & LevelDetail, COLOR_BLACK, COLOR_GRAY, 0, TRACE_LOW, 1)
        Catch ex As Exception
            Show_Message_Console(MyName & " Exception viewing detail:" & ex.Message, COLOR_OWNER1, COLOR_RED, 0, TRACE_LOW, 1)
        End Try

    End Sub


    Private Sub ProcessShutdownModule()

        Try
            Dim Mem As Process = Process.GetCurrentProcess()
            Mem.Kill()
        Catch ex As Exception
            Console.WriteLine(GetDateTime() & " Exception en proceso de Shutdown")
        End Try

    End Sub

End Class
