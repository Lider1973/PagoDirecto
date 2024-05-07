Imports System.Threading
Imports System.Messaging
Imports System.Text

Public Class SWTM_Behavior

    Dim ProcessName(0) As String
    Dim ProcessDate(0) As DateTime
    Dim ExecutableName(0) As String
    Dim CommandQueueName(0) As String
    Dim MemResource(0) As Long
    Dim Counters(0) As Byte

    Dim MainSequence As Long
    'Dim HASH_pending As New Hashtable

    Public Thread_EVAL As Thread
    Public Thread_ECHO As Thread
    Public Thread_MAIN As Thread

    Dim TO_COMMANDER As New InfoCommands

    Public Sub Init_Process_Status_Modules()
        ModuleList.Clear()
        DIC_CTRL_time.Clear()

        GetModulesDefined(ModuleList)
        ReDim ProcessName(ModuleList.Count - 1)
        ReDim ProcessDate(ModuleList.Count - 1)
        ReDim ExecutableName(ModuleList.Count - 1)
        ReDim CommandQueueName(ModuleList.Count - 1)
        ReDim MemResource(ModuleList.Count - 1)
        ReDim MemResource(ModuleList.Count - 1)
        ReDim Counters(ModuleList.Count - 1)

        For x = 0 To UBound(ProcessName)
            ProcessDate(x) = DateTime.Now
            MemResource(x) = 0
            Counters(x) = 0
        Next
        SaveLogMain("ReDim  aNamesP(ProcessName.Count - 1)" & ProcessName.Count)
        ReDim aNamesP(ProcessName.Count - 1)
        '*****************************************************************
        For x As Int16 = 0 To UBound(ProcessName)
            Dim Parms() As String = ModuleList(x).Split(",")
            ProcessName(x) = Parms(0).Trim
            DIC_CTRL_time.Add(ProcessName(x), Now)
            aNamesP(x) = ProcessName(x)
        Next
        '*****************************************************************
        Dim ModuleFields As New List(Of String)
        For x As Int16 = 0 To UBound(ProcessName)
            Dim Parms() As String = ModuleList(x).Split(",")
            ModuleFields.Clear()
            If GetInfoMainData(Parms(0).Trim, ModuleFields) = 0 Then
                ExecutableName(x) = ModuleFields(19).Substring(ModuleFields(19).LastIndexOf("\") + 1)
                CommandQueueName(x) = ModuleFields(13)
                If ModuleFields(1) = "1" Then
                    RouterCommandQueue = ModuleFields(13)
                End If
            End If
        Next

        'Start_Thread_Eval()
        'Start_Thread_Echo()
        Start_Thread_Main()

    End Sub

    'Private Sub Start_Thread_Eval()
    '    '*****************************************************************
    '    Try
    '        If Not IsNothing(Thread_EVAL) Then
    '            Thread_EVAL = Nothing
    '        End If
    '    Catch ex As Exception
    '    End Try
    '    Thread_EVAL = New Thread(AddressOf Evaluate_Status_Response)
    '    Thread_EVAL.Name = "EVAL"
    '    Thread_EVAL.Start()
    'End Sub

    'Private Sub Start_Thread_Echo()
    '    '*****************************************************************
    '    Try
    '        If Not IsNothing(Thread_ECHO) Then
    '            Thread_ECHO = Nothing
    '        End If
    '    Catch ex As Exception

    '    End Try

    '    Thread_ECHO = New Thread(AddressOf Process_Send_ECHO)
    '    Thread_ECHO.Name = "ECHO"
    '    Thread_ECHO.Start()
    '    'ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf Process_Send_ECHO))
    'End Sub

    Private Sub Start_Thread_Main()
        '*****************************************************************
        Try
            If Not IsNothing(Thread_MAIN) Then
                Thread_MAIN = Nothing
                'Exit Sub
            End If
        Catch ex As Exception

        End Try

        Thread_MAIN = New Thread(AddressOf Main_Receiver_Commands)
        Thread_MAIN.Name = "MAIN"
        Thread_MAIN.Start()
        'ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf Main_Receiver_Commands))
    End Sub


    Public Sub Abort_Threads()

        Try
            Thread_EVAL.Abort()
            Thread_EVAL = Nothing
        Catch ex As Exception
            'MsgBox(ex.Message, MsgBoxStyle.Information, "Eval Abort")
        End Try

        Try
            Thread_ECHO.Abort()
            Thread_ECHO = Nothing
        Catch ex As Exception
            'MsgBox(ex.Message, MsgBoxStyle.Information, "Echo Abort")
        End Try

        Try
            Thread_MAIN.Abort()
            Thread_MAIN = Nothing
        Catch ex As Exception
            'MsgBox(ex.Message, MsgBoxStyle.Information, "Main Abort")
        End Try

    End Sub

    'Private Sub Evaluate_Status_Response()
    '    Dim TimeWait As Long = My.Settings.TimeWait * 1000
    '    Dim Date1 As DateTime
    '    Dim Date2 As DateTime
    '    Dim TotalSeconds As Int32        
    '    Try
    '        Do While True
    '            For x As Int16 = 0 To UBound(ProcessName)
    '                Dim TMSP As TimeSpan
    '                Date1 = ProcessDate(x)
    '                Date2 = DateTime.Now
    '                TMSP = System.DateTime.op_Subtraction(Date2, Date1)
    '                TotalSeconds = (TMSP.Seconds * 1000) + TMSP.Milliseconds
    '                If TotalSeconds > TimeWait Then
    '                    Dim TextData As String
    '                    Counters(x) += 1
    '                    Dim Counter As Byte = Get_Counters(ProcessName(x))
    '                    Select Case Counter
    '                        Case 1
    '                            Process_Send_Update_Status(ProcessName(x), 4)
    '                            Dim TMSPl As TimeSpan = Date2.Subtract(Date1)
    '                            'TextData = ProcessName(x) & " -> " & Date1.ToString("yyyy-MM-dd HH:mm:ss.fff") & " - " & Date2.ToString("yyyy-MM-dd HH:mm:ss.fff") & " Ms:" & TMSP.Seconds & "." & TMSP.Milliseconds
    '                            'Put_Message_To_Show(commander_DISPLAY & COLOR_WHITE & "#" & COLOR_BLACK & "#" & TextData)
    '                        Case 2
    '                            Process_Send_Update_Status(ProcessName(x), 3)
    '                            Dim TMSPl As TimeSpan = Date2.Subtract(Date1)
    '                            'TextData = ProcessName(x) & " -> " & Date1.ToString("yyyy-MM-dd HH:mm:ss.fff") & " - " & Date2.ToString("yyyy-MM-dd HH:mm:ss.fff") & " Ms:" & TMSP.Seconds & "." & TMSP.Milliseconds
    '                            'Put_Message_To_Show(commander_DISPLAY & COLOR_WHITE & "#" & COLOR_BLACK & "#" & TextData)
    '                        Case Else
    '                            Process_Send_Update_Status(ProcessName(x), 3)
    '                            Dim TMSPl As TimeSpan = Date2.Subtract(Date1)
    '                            TextData = ProcessName(x) & " No está respondiendo.... "
    '                            Put_Message_To_Show(commander_DISPLAY & COLOR_WHITE & "#" & COLOR_BLACK & "#" & TextData)
    '                            If Killing_ServiSwitch(ProcessName(x)) = 0 Then
    '                                Process_Start_Module(ProcessName(x), ExecutableName(x))
    '                                ProcessDate(x) = DateTime.Now
    '                                Process_Send_Update_Status(ProcessName(x), 0)
    '                                Counters(x) = 0
    '                                HASH_pending.Clear()
    '                                'Dim TMSPl As TimeSpan = Date2.Subtract(Date1)
    '                                TextData = ProcessName(x) & " -> " & Date1.ToString("yyyy-MM-dd HH:mm:ss.fff") & " - " & Date2.ToString("yyyy-MM-dd HH:mm:ss.fff") & " Ms:" & TMSP.Seconds & "." & TMSP.Milliseconds
    '                                Put_Message_To_Show(commander_DISPLAY & COLOR_WHITE & "#" & COLOR_BLACK & "#" & TextData)
    '                            End If
    '                    End Select
    '                Else
    '                    Counters(x) = 0
    '                End If
    '            Next
    '            GC.Collect()
    '            Thread.Sleep(3000)
    '        Loop
    '    Catch ex As Exception
    '        If ex.HResult = -2146233040 Then
    '            Exit Sub
    '        End If
    '        Start_Thread_Eval()
    '    End Try

    'End Sub

    'Private Sub Process_Send_ECHO()
    '    Try
    '        Do While True
    '            If MainSequence >= 999999 Then
    '                MainSequence = 1
    '            Else
    '                MainSequence += 1
    '            End If
    '            For x As Int16 = 0 To UBound(ProcessName)
    '                Dim MessageToSendString As String = Build_Message_To_Module(ProcessName(x), MainSequence)
    '                Put_Message_To_Module(MessageToSendString, CommandQueueName(x), MainSequence)
    '                Registry_Unique_Hash(ProcessName(x), MainSequence)
    '            Next
    '            GC.Collect()
    '            Thread.Sleep(2000)
    '        Loop
    '    Catch ex As Exception
    '        If ex.HResult = -2146233040 Then
    '            Exit Sub
    '        End If
    '        Start_Thread_Echo()
    '    End Try
    'End Sub

    Public Sub Process_Start_Module(ByVal ModuleName As String, ByVal ProgramName As String)

        Try
            Dim pHelp As New ProcessStartInfo
            pHelp.FileName = PathStart & "\" & ModuleName & "\" & ProgramName
            pHelp.Arguments = ModuleName
            pHelp.UseShellExecute = True

            'MsgBox("PATH:" & pHelp.FileName)

            pHelp.WindowStyle = ProcessWindowStyle.Hidden
            Dim proc As Process = Process.Start(pHelp)
        Catch ex As Exception
            MessageBox.Show("Excepción iniciando proceso " & ModuleName & " :" & ex.Message, "Start Process", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Function Build_Message_To_Module(ByVal ProcessName As String, ByVal Sequence As Long) As String
        Dim MessageToSend As String = String.Empty

        MessageToSend = "STA" & "|"
        MessageToSend += ProcessName & "|"
        MessageToSend += Sequence.ToString("000000") & "|"
        'Dim MessageToSendByte() As Byte = Encoding.ASCII.GetBytes(MessageToSend)

        Return MessageToSend
    End Function

    'Private Function Registry_Unique_Hash(ByVal ProcessName As String, ByVal Sequence As Long) As Byte
    '    Dim ErrorCode As Byte = 1

    '    Try
    '        HASH_pending.Add(ProcessName & Sequence.ToString("000000"), DateTime.Now)
    '    Catch ex As Exception
    '        MessageBox.Show("No se pudo agregar hash de fecha/hora:" & ex.Message, "Add hash", MessageBoxButtons.OK, MessageBoxIcon.Error)
    '    End Try

    '    Return ErrorCode
    'End Function

    'Private Function Retrieve_Unique_Hash(ByVal KeyReceive As String, ByRef DateReply As DateTime) As Byte
    '    Dim ErrorCode As Byte = 1
    '    Dim lockOBJ_IN As Object = New Object
    '    SyncLock lockOBJ_IN
    '        Try
    '            If HASH_pending.Contains(KeyReceive) Then
    '                For Each DicEnt In HASH_pending
    '                    If DicEnt.key = KeyReceive Then
    '                        DateReply = DicEnt.Value
    '                        HASH_pending.Remove(KeyReceive)
    '                        ErrorCode = 0
    '                        Exit For
    '                    End If
    '                Next
    '            End If
    '        Catch ex As Exception
    '            'MessageBox.Show("No se pudo obtener hash de fecha/hora ", "Get hash", MessageBoxButtons.OK, MessageBoxIcon.Error)
    '        End Try
    '    End SyncLock

    '    Return ErrorCode
    'End Function

    Public Sub Put_Message_To_Module(ByVal CommandMessage As String, ByVal QueueName As String, ByVal Sequence As Long)
        Try
            Dim MessageToSend As Message = New Message
            Dim QueueSendData As New MessageQueue(PrivateQueue & QueueName)
            Dim CommandToModule As New InfoCommands
            CommandToModule.ICM_CommandBuffer = CommandMessage
            MessageToSend.Body = CommandToModule
            QueueSendData.Send(MessageToSend)
            CommandToModule = Nothing
            MessageToSend.Dispose()
            QueueSendData.Dispose()
        Catch ex As Exception
            SaveLogMain("Metodo:" & System.Reflection.MethodBase.GetCurrentMethod.Name.ToString & Chr(13) & "Error:" & ex.Message)
        End Try
    End Sub

    Public Sub Put_Message_To_Command(ByVal CommandData As String)
        Try
            Dim MessageToSend As Message = New Message
            Dim QueueSendData As New MessageQueue(PrivateQueue & CommandQueue)
            Dim TO_COMMANDER As New InfoCommands
            TO_COMMANDER.ICM_CommandBuffer = CommandData
            MessageToSend.Body = TO_COMMANDER
            QueueSendData.Send(MessageToSend)
            QueueSendData.Dispose()
            MessageToSend.Dispose()
            TO_COMMANDER.ICM_CommandBuffer = Nothing
        Catch ex As Exception
            SaveLogMain("Metodo:" & System.Reflection.MethodBase.GetCurrentMethod.Name.ToString & Chr(13) & "Error:" & ex.Message)
        End Try
    End Sub


    Private Sub Main_Receiver_Commands()
        Dim CommandData As String
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
                Dim TS_MessageQueue As New MessageQueue(PrivateQueue & ReplyQueue)
                TS_MessageQueue.MessageReadPropertyFilter = filter
                TS_MessageQueue.Formatter = New XmlMessageFormatter(New Type() {GetType(InfoCommands)})
                Dim myMessage As Message = TS_MessageQueue.Receive
                Dim Struct_Queue_Message As InfoCommands = CType(myMessage.Body, InfoCommands)
                CommandData = Struct_Queue_Message.ICM_CommandBuffer
                '********************************************************************************
                '********************************************************************************
                Dim DiffQueueTime = DateDiff(DateInterval.Second, myMessage.ArrivedTime, DateTime.Now)
                If DiffQueueTime > 5 Then
                    Continue Do
                End If
                '********************************************************************************
                '********************************************************************************

                Process_Decode_Message_REPLY(CommandData)

                '********************************************************************************
                '********************************************************************************
            Catch Sn As System.NullReferenceException
                SaveLogMain("Metodo:" & System.Reflection.MethodBase.GetCurrentMethod.Name.ToString & Chr(13) & "Error:" & Sn.Message)
                GoTo Exiting_Thread
            Catch ex As Exception
                SaveLogMain("Metodo:" & System.Reflection.MethodBase.GetCurrentMethod.Name.ToString & Chr(13) & "Error:" & ex.Message)
                GoTo Exiting_Thread
            End Try
        Loop
Exiting_Thread:
        Start_Thread_Main()

    End Sub

    Private Sub Process_Decode_Message_REPLY(ByVal ReceivedMessage As String)
        Dim Parms() As String = ReceivedMessage.Split("|")

        Select Case Parms(0)
            Case "STA"
                Receive_Reply_Status_Message(ReceivedMessage)
            Case "ONL"
                Receive_Notify_Status_Message(ReceivedMessage)
            Case "ACK"
                Receive_Reply_Status_ACK(ReceivedMessage)
        End Select

    End Sub

    Private Sub Receive_Notify_Status_Message(ByVal ReplyCommandMessage As String)
        Dim Parms() As String = ReplyCommandMessage.Split("|")
        Dim Parms1() As String = Parms(1).Split(",")
        Process_Send_Update_Status(Parms1(0), CInt(Parms1(2)))
    End Sub

    Private Sub Receive_Reply_Status_ACK(ByVal ReplyCommandMessage As String)
        Dim Parms() As String = ReplyCommandMessage.Split("|")
        Dim KeyHash As String = Parms(1) & Parms(2)
        If ProcessName.Contains(Parms(1)) Then
            Dim idx As Int16 = Array.LastIndexOf(ProcessName, Parms(1))
            ProcessDate(idx) = Parms(3)
            MemResource(idx) = CLng(Parms(5))
            Process_Send_Update_Status(Parms(0), CInt(Parms(2)))
        End If
    End Sub

    Private Sub Receive_Reply_Status_Message(ByVal ReplyCommandMessage As String)
        Dim Parms() As String = ReplyCommandMessage.Split("|")
        'Dim KeyHash As String = Parms(1) & Parms(2)
        'Dim DateHash As DateTime
        'If Retrieve_Unique_Hash(KeyHash, DateHash) = 0 Then
        '    If ProcessName.Contains(Parms(1)) Then
        '        Dim idx As Int16 = Array.LastIndexOf(ProcessName, Parms(1))
        '        ProcessDate(idx) = DateHash
        '        Process_Send_Update_Status(Parms(1), CInt(Parms(4)))
        '        MemResource(idx) = CLng(Parms(5))
        '    End If
        'End If

        Process_Send_Update_Status(Parms(1), CInt(Parms(3)))

        Set_Value_Process(Parms(1), Now)

    End Sub

    Private Sub Process_Send_Update_Status(ByVal ProcessName As String, ByVal ImageIndex As Byte)

        Dim CommandMessage As String = "RFH|" & ProcessName & "|" & ImageIndex
        Put_Message_To_Command(CommandMessage)

    End Sub

    Public Function GetDateTime() As String
        Return System.DateTime.Now.Year & "-" & Format(System.DateTime.Now.Month, "00") & "-" & Format(System.DateTime.Now.Day, "00") & " " & Format(System.DateTime.Now.Hour, "00") & ":" & Format(System.DateTime.Now.Minute, "00") & ":" & Format(System.DateTime.Now.Second, "00") & "." & Format(System.DateTime.Now.Millisecond, "000")
    End Function

    Public Function Killing_ServiSwitch(ByVal ProcessName As String) As Byte
        Dim ErrorCode As Byte = 1
        Dim PathProcessName As String = String.Empty
        Try
            Dim ServiProcess As List(Of Process) = (From p As Process In Process.GetProcesses Where p.ProcessName.ToUpper Like "ServiSwitch*".ToUpper).ToList
            For Each p As Process In ServiProcess
                PathProcessName = p.MainModule.FileName()
                If PathProcessName.Contains("\" & ProcessName & "\") Then
                    p.Kill()
                End If
            Next
            ErrorCode = 0
        Catch ex As Exception
            SaveLogMain("Metodo:" & System.Reflection.MethodBase.GetCurrentMethod.Name.ToString & Chr(13) & "Error:" & ex.Message)
            ErrorCode = False
        End Try
        'ErrorCode = SUCCESSFUL
        Return ErrorCode
    End Function

    Public Function Get_Queue_To_Send(ByVal PProcessName As String) As String
        Dim QueueName As String = String.Empty

        If ProcessName.Contains(PProcessName.Trim) Then
            Dim Idx As Int16 = Array.IndexOf(ProcessName, PProcessName.Trim)
            QueueName = CommandQueueName(Idx)
        End If
        Return QueueName
    End Function

    Public Function Get_Last_Reply(ByVal PProcessName As String) As DateTime
        Dim ReplyDate As DateTime
        If ProcessName.Contains(PProcessName) Then
            Dim idx As Int16 = Array.LastIndexOf(ProcessName, PProcessName)
            ReplyDate = ProcessDate(idx)
        End If
        Return ReplyDate
    End Function

    Public Function Get_Path_Name(ByVal PProcessName As String) As String
        Dim PathName As String = String.Empty
        If ProcessName.Contains(PProcessName) Then
            Dim idx As Int16 = Array.LastIndexOf(ProcessName, PProcessName)
            PathName = ExecutableName(idx)
        End If
        Return PathName
    End Function

    Public Function Get_Counters(ByVal PProcessName As String) As Byte
        Dim Counter As Byte = 0
        If ProcessName.Contains(PProcessName) Then
            Dim idx As Int16 = Array.LastIndexOf(ProcessName, PProcessName)
            Counter = Counters(idx)
        End If
        Return Counter
    End Function

    Public Function Get_MemResource(ByVal PProcessName As String) As Int32
        Dim LMemResource As Int32 = 0
        If ProcessName.Contains(PProcessName) Then
            Dim idx As Int16 = Array.LastIndexOf(ProcessName, PProcessName)
            LMemResource = MemResource(idx)
        End If
        Return LMemResource
    End Function

    Public Sub Put_Message_To_Show(ByVal Data As String)

        Try
            Dim MessageToSend As Message = New Message
            Dim QueueSendData As New MessageQueue(PrivateQueue & CommandQueue)
            TO_COMMANDER.ICM_CommandBuffer = Data
            MessageToSend.Body = TO_COMMANDER
            QueueSendData.Send(MessageToSend)
            QueueSendData.Dispose()
            MessageToSend.Dispose()
            TO_COMMANDER.ICM_CommandBuffer = Nothing
        Catch ex As Exception
            SaveLogMain("Metodo:" & System.Reflection.MethodBase.GetCurrentMethod.Name.ToString & Chr(13) & "Error:" & ex.Message)
        End Try
    End Sub


    Public Sub Put_Notify_To_Router(ByVal Data As String)

        Try
            Dim MessageToSend As Message = New Message
            Dim QueueSendData As New MessageQueue(PrivateQueue & RouterCommandQueue)
            TO_COMMANDER.ICM_CommandBuffer = Data
            MessageToSend.Body = TO_COMMANDER
            QueueSendData.Send(MessageToSend)
            QueueSendData.Dispose()
            MessageToSend.Dispose()
            TO_COMMANDER.ICM_CommandBuffer = Nothing
        Catch ex As Exception
            SaveLogMain("Metodo:" & System.Reflection.MethodBase.GetCurrentMethod.Name.ToString & Chr(13) & "Error:" & ex.Message)
        End Try

    End Sub

End Class
