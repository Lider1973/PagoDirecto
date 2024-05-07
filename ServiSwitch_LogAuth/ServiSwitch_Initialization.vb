Imports System.Messaging
Imports System.Threading
Imports System.IO
Imports System.Windows.Forms
Imports System.Threading.Tasks
'Imports System.Runtime.InteropServices
Imports System.Management

Module ServiSwitch_Initialization

    Dim Continuing As New AutoResetEvent(False)

    Const QUEUE_ERROR As Byte = 95
    Const INTERFACE_ERROR As Byte = 96
    Const USER_ERROR As Byte = 97
    Const ROUTE_ERROR As Byte = 98
    Dim TH_Refresh As Thread
    Dim Pmem As Int64
    Public ValMem As Int64

    Private Declare Function SetConsoleCtrlHandler Lib "kernel32" (ByVal handlerRoutine As ConsoleEventDelegate, ByVal add As Boolean) As Boolean
    Public Delegate Function ConsoleEventDelegate(ByVal MyEvent As ConsoleEvent) As Boolean

    Public Enum ConsoleEvent
        CTRL_C_EVENT = 0
        CTRL_BREAK_EVENT = 1
        CTRL_CLOSE_EVENT = 2
        CTRL_LOGOFF_EVENT = 5
        CTRL_SHUTDOWN_EVENT = 6
    End Enum

    Sub Main(ByVal args() As String)

        Console.WriteLine(GetDateTime() & " Wait....")
        Validate_Parallel_Process()
        Console.WriteLine(GetDateTime() & " Runnig....")

        'ShowWindow(GetConsoleWindow(), SW_HIDE)
        '************************************************************************************************
        If Not SetConsoleCtrlHandler(AddressOf Application_ConsoleEvent, True) Then
            Console.Write("Unable to install console event handler.")
        End If
        '************************************************************************************************

        Dim ErrorMessage As String = Nothing
        Dim ErrorCode As Int16 = 0

        Try
            MyName = args(0)
        Catch ex As Exception
            MyName = "Logger (*)"
        End Try

        If IsNothing(MyName) Then
            Show_Message_Console(" No se ha recibido parametro de Nombre de Interface", COLOR_OWNER1, COLOR_RED, 0, TRACE_LOW, 0)
            Show_Message_Console("  -------- Presione una tecla para salir ----------", COLOR_BLACK, COLOR_YELLOW, 0, TRACE_LOW, 0)
            Console.WriteLine()
            Console.Read()
            Exit Sub
        End If
        '************************************************************************************************
        '************************************************************************************************
        Send2Manager = True
        Dim PathInfo As String = My.Application.Info.DirectoryPath & "\" & My.Application.Info.AssemblyName & ".exe"
        Dim infoReader As System.IO.FileInfo
        infoReader = My.Computer.FileSystem.GetFileInfo(PathInfo)
        Show_Message_Console("-------------------------------------------------------------", COLOR_BLACK, COLOR_GREEN, 2, TRACE_LOW, 0)
        Show_Message_Console("           Iniciando sistema, Modulo " & MyName, COLOR_BLACK, COLOR_GREEN, 2, TRACE_LOW, 0)
        Show_Message_Console("      Fecha & Hora de creacion:" & infoReader.LastWriteTime, COLOR_BLACK, COLOR_GREEN, 2, TRACE_LOW, 0)
        Show_Message_Console("-------------------------------------------------------------", COLOR_BLACK, COLOR_GREEN, 2, TRACE_LOW, 0)
        Dim DatoReader As String = infoReader.LastWriteTime
        infoReader = Nothing
        PathInfo = Nothing
        '************************************************************************************************
        Try
            Console.Title = MyName & ": Router of requirement (" & DatoReader & ")"
        Catch ex As Exception
            Console.Title = "LOGGER"
        End Try
        '************************************************************************************************

        If Load_Setting_Database() <> SUCCESSFUL Then
            GoTo MainExit
        End If


        '************************************************************************************************
        Notify_Process_Status(MyName & "," & listview_CONTROL & "," & UNKNOW_status)
        '************************************************************************************************

        Dim ProcessingMessagesData As New ServiSwitch_ProcessCommand
        ProcessingMessagesData.Start_Task_Process_Command()
        ProcessingMessagesData = Nothing
        '************************************************************************************************
        Dim ProcessinRequierementData As New ServiSwitch_ProcessMessage
        ProcessinRequierementData.Start_Main_Process_Routers()
        ProcessinRequierementData = Nothing

        'Logger_Request_Queue_Name = Get_Logger_Queue()

        Init_Task_Save_Log()

        Notify_Process_Status(MyName & "," & listview_CONTROL & "," & STARTED_status)

        'ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf MemRes))

        '*************************************************************
        Show_Message_Console(MyName & " SUCCESSFULL SYSTEM STARTUP ", COLOR_BLACK, COLOR_WHITE, 0, TRACE_LOW, 1)
        Thread.Sleep(5000)
        'ShowWindow(GetConsoleWindow(), SW_HIDE)   'OCULTA LA VENTANA TEMPORAL
        '******************************************************************************        
        Dim CurrentProcess As Process = Process.GetCurrentProcess
        CurrentProcess.PriorityClass = ProcessPriorityClass.BelowNormal
        '******************************************************************************        

        GC.Collect()
        ClearMemory()

AlwaysWait:
        Show_Message_Console(MyName & " Carga Exitosa del sistema", COLOR_BLACK, COLOR_WHITE, 0, TRACE_LOW, 1)
        Thread.Sleep(-1)

MainExit:
        Console.WriteLine("Aplicacion terminada.....")
        Console.ReadKey()
        Console.WriteLine("BYE.....")
        Thread.Sleep(1000)
    End Sub


    Public Function GetDateTime() As String

        Return System.DateTime.Now.Year & "-" & Format(System.DateTime.Now.Month, "00") & "-" & Format(System.DateTime.Now.Day, "00") & " " & Format(System.DateTime.Now.Hour, "00") & ":" & Format(System.DateTime.Now.Minute, "00") & ":" & Format(System.DateTime.Now.Second, "00") & "." & Format(System.DateTime.Now.Millisecond, "000")

    End Function


    Private Function ProcessAvailable(ByVal ExeName As String) As Boolean

Looping:
        Dim RcpProcess As List(Of Process) = (From p As Process In Process.GetProcesses Where p.ProcessName.ToUpper Like ExeName.ToUpper).ToList
        If RcpProcess.Count = 0 Then
            Return False
        Else
            Return True
        End If

    End Function

    Public Function Application_ConsoleEvent(ByVal [event] As ConsoleEvent) As Boolean
        Dim cancel As Boolean = False
        Dim CMD As New ServiSwitch_ProcessCommand
        CMD.Send_Status_Commander(3)

        'Notify_Process_Status(MyName & "," & listview_CONTROL & "," & UNKNOW_status)

        Console.Write(Chr(13))
        Select Case [event]
            Case ConsoleEvent.CTRL_C_EVENT
                Console.WriteLine("********************************************************")
                Console.WriteLine("******           APLICACION TERMINADA           ********")
                Console.WriteLine("********************************************************")
                Thread.Sleep(2000)
                Return False
            Case ConsoleEvent.CTRL_BREAK_EVENT
                Console.WriteLine("********************************************************")
                Console.WriteLine("******           APLICACION TERMINADA           ********")
                Console.WriteLine("********************************************************")
                Thread.Sleep(2000)
                Return False
            Case ConsoleEvent.CTRL_CLOSE_EVENT
                Console.WriteLine("********************************************************")
                Console.WriteLine("******           APLICACION TERMINADA           ********")
                Console.WriteLine("********************************************************")
                Thread.Sleep(2000)
                Return False
            Case ConsoleEvent.CTRL_LOGOFF_EVENT
                Console.WriteLine("********************************************************")
                Console.WriteLine("******           APLICACION TERMINADA           ********")
                Console.WriteLine("********************************************************")
                Thread.Sleep(2000)
                Return False
            Case ConsoleEvent.CTRL_SHUTDOWN_EVENT
                Console.WriteLine("********************************************************")
                Console.WriteLine("******           APLICACION TERMINADA           ********")
                Console.WriteLine("********************************************************")
                Thread.Sleep(2000)
                Return False
        End Select

        Return cancel ' handling the event.

    End Function

    Public Function RemoveQueueFromLocal(ByVal QueueName As String) As Byte
        Dim ErrorCode As Byte

        Try
            If MessageQueue.Exists(QueueName) Then
                MessageQueue.Delete(QueueName)
                Show_Message_Console(MyName & " Queue:" & QueueName & " Deleted", COLOR_BLACK, COLOR_GRAY, 0, TRACE_LOW, 1)
            End If
        Catch ex As Exception
            Show_Message_Console(MyName & " Cant delete Queue:" & QueueName & " On system", COLOR_OWNER1, COLOR_RED, 0, TRACE_LOW, 1)
            ErrorCode = 1
        End Try

        Return ErrorCode
    End Function

    Private Sub MemRes()
        Dim p As Process = Process.GetCurrentProcess
        Dim pID As Int32 = p.Id
        p.Dispose()
        Dim RetrieveName As String
        '**********************************************************************
        Dim CurrentThread As Thread = Thread.CurrentThread
        CurrentThread.Priority = ThreadPriority.BelowNormal
        '**********************************************************************
        Do While True
            Using searcher As New ManagementObjectSearcher(New ManagementScope, New ObjectQuery("SELECT Name, WorkingSetPrivate FROM Win32_PerfFormattedData_PerfProc_Process  WHERE IDProcess =" & pID))
                Using results As ManagementObjectCollection = searcher.Get
                    For Each result As ManagementObject In results
                        RetrieveName = (DirectCast(result.GetPropertyValue("Name"), String))
                        Pmem = DirectCast(result.GetPropertyValue("WorkingSetPrivate"), ULong)
                        Pmem = Pmem / 1024
                        ValMem = Pmem
                        If Pmem > 5000 Then
                            'Console.Write(" MEM:" & Pmem.ToString & " ")
                            ClearMemory()
                        End If
                        Exit For
                    Next
                    searcher.Dispose()
                    results.Dispose()
                End Using
                GC.Collect()
                Thread.Sleep(5000)
            End Using
        Loop
    End Sub


    Private Sub Validate_Parallel_Process()

        Dim AppName As String = My.Application.Info.AssemblyName
        Dim process As System.Diagnostics.Process = Process.GetCurrentProcess

        For Each prog As Process In Process.GetProcesses()
            If prog.ProcessName = AppName Then
                If prog.Id <> process.Id Then
                    Console.WriteLine(GetDateTime() & " Stopping process " & prog.ProcessName & ":" & prog.Id)
                    prog.Kill()
                End If
            End If
        Next

        ClearMemory()
    End Sub

End Module
