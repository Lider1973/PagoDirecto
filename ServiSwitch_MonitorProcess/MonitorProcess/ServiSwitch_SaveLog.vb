Imports System.IO
Imports System.Text
Imports System.Threading

Module mod_SaveLog
    Dim Unlockers_2 As Object = New Object
    Public LogQueue As New Queue
    Dim LogEvent As New AutoResetEvent(False)
    Dim ThreadLog As Thread
    Dim PathDirectory As String
    Dim PathFile As String
    Dim objWriter As System.IO.StreamWriter
    Dim OldFileName, NewFileName As String

    Public Sub Init_Task_Save_Log()

        NewFileName = "\log" & Now.Year & Format(Now.Month, "00") & Format(Now.Day, "00") & ".txt"
        OldFileName = "\log" & Now.Year & Format(Now.Month, "00") & Format(Now.Day, "00") & ".txt"

        VerifyLogDir()

        VerifyLogFile()

        ThreadLog = New Thread(AddressOf DequeueLogData)
        ThreadLog.Name = "Reading"
        ThreadLog.Start()

    End Sub


    Private Sub VerifyLogDir()

        PathDirectory = System.AppDomain.CurrentDomain.BaseDirectory & "LOGS"
        If Not Directory.Exists(PathDirectory) Then
            Directory.CreateDirectory(PathDirectory)
        End If

    End Sub

    Private Sub VerifyLogFile()

        PathFile = PathDirectory & "\log" & Now.Year & Format(Now.Month, "00") & Format(Now.Day, "00") & ".txt"
        objWriter = New System.IO.StreamWriter(PathFile, True)

    End Sub

    Private Sub SaveLog(ByVal BufferToSave As String)

        If IsNothing(BufferToSave) Then
            Exit Sub
        End If

        BufferToSave = Now.ToString("yyyy-MM-dd HH:mm:ss.fff") & " " & BufferToSave

        Try
            NewFileName = "\log" & Now.Year & Format(Now.Month, "00") & Format(Now.Day, "00") & ".txt"
            If Not NewFileName = OldFileName Then
                objWriter.Close()
                VerifyLogFile()
            End If
            objWriter.WriteLine(BufferToSave)
            objWriter.Flush()

            OldFileName = "\log" & Now.Year & Format(Now.Month, "00") & Format(Now.Day, "00") & ".txt"
        Catch ex As Exception
            'DisplayMessage(" Excepcion :" & ex.Message, 2, 1)
        End Try

    End Sub


    Public Sub SaveLogMain(ByVal BufferToSave As String)

        Try
            LogQueue.Enqueue(BufferToSave)
            LogEvent.Set()
        Catch ex As Exception

        End Try

    End Sub

    Private Sub DequeueLogData()
        Dim BufferToSave As String

BeginWait:
        Try
            LogEvent.WaitOne()
            While LogQueue.Count > 0
                BufferToSave = LogQueue.Dequeue()
                Call SaveLog(BufferToSave)
            End While
            GoTo BeginWait
        Catch ex As Exception

        End Try

    End Sub

End Module
