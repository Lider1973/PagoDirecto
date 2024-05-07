Imports System.Threading
Imports System.Configuration
Imports System.IO
Imports System.Reflection
Imports System.Reflection.Emit
Imports System.Globalization
Imports System.Management
Imports System.Attribute

Module ServiSwitch_Initialization_module
    Dim Pmem As Int64
    Public ValMem As Int64

    Public Enum ConsoleEvent
        CTRL_C_EVENT = 0
        CTRL_BREAK_EVENT = 1
        CTRL_CLOSE_EVENT = 2
        CTRL_LOGOFF_EVENT = 5
        CTRL_SHUTDOWN_EVENT = 6
    End Enum

    Private Declare Function SetConsoleCtrlHandler Lib "kernel32" (ByVal handlerRoutine As ConsoleEventDelegate, ByVal add As Boolean) As Boolean
    Public Delegate Function ConsoleEventDelegate(ByVal MyEvent As ConsoleEvent) As Boolean

    Public Const File_Main As Byte = 0
    Public Const File_Mess As Byte = 1
    Public Const File_Tran As Byte = 2
    Public Const File_Route As Byte = 3

    Public ConfigPath As String
    Public ApplType As String = "ISO"
    Public MyName As String
    Public TaskNumbers As Int32
    Public Send2Manager As Boolean = False

    Dim ProcessMessageData As New ServiSwitch_ProcessCommand

    Private Declare Auto Function SetProcessWorkingSetSize Lib "kernel32.dll" (ByVal procHandle As IntPtr, ByVal min As Int32, ByVal max As Int32) As Boolean
    Declare Function SetWindowText Lib "user32" Alias "SetWindowTextA" (ByVal hWnd As IntPtr, ByVal lpString As String) As Boolean

    Sub Main(ByVal args() As String)


        Console.WriteLine(GetDateTime() & " Wait....")
        Validate_Parallel_Process()
        Console.WriteLine(GetDateTime() & " Runnig....")

        If Not SetConsoleCtrlHandler(AddressOf Application_ConsoleEvent, True) Then
            Console.Write("Unable to install console event handler.")
        End If
        '************************************************************************************************
        Try
            MyName = args(0)
        Catch ex As Exception
            MyName = "HOST2"
        End Try
        '************************************************************************************************
        If IsNothing(MyName) Then
            Show_Message_Console(" No se ha recibido parametro de Nombre de Interface", COLOR_OWNER1, COLOR_RED, 0, TRACE_LOW, 1)
            Show_Message_Console("  -------- Presione una tecla para salir ----------", COLOR_BLACK, COLOR_YELLOW, 0, TRACE_LOW, 1)
            GoTo MainExit
        End If
        '************************************************************************************************
        Send2Manager = True
        Dim PathInfo As String = My.Application.Info.DirectoryPath & "\" & My.Application.Info.AssemblyName & ".exe"
        Dim infoReader As System.IO.FileInfo
        infoReader = My.Computer.FileSystem.GetFileInfo(PathInfo)
        Show_Message_Console("-------------------------------------------------------------", COLOR_BLACK, COLOR_GREEN, 2, TRACE_LOW, 0)
        Show_Message_Console("         Iniciando sistema, Modulo CPNPDR Interface          ", COLOR_BLACK, COLOR_GREEN, 2, TRACE_LOW, 0)
        Show_Message_Console("      Fecha & Hora de creacion:" & infoReader.LastWriteTime, COLOR_BLACK, COLOR_GREEN, 2, TRACE_LOW, 0)
        Show_Message_Console("-------------------------------------------------------------", COLOR_BLACK, COLOR_GREEN, 2, TRACE_LOW, 0)
        Console.Title = MyName & " : CPN Autorizador Pago Directo Builded On " & infoReader.LastWriteTime
        infoReader = Nothing
        PathInfo = Nothing
        '************************************************************************************************

        If Load_Setting_Database() <> SUCCESSFUL Then
            GoTo MainExit
        End If

        '************************************************************************************************
        '**********************************************************************************************
        '              INICIA TAREA PRINCIPAL DE RECEPCION DE REQUERIMIENTOS & RESPUESTAS
        '**********************************************************************************************
        Dim ProcessCommand As New ServiSwitch_ProcessCommand
        ProcessCommand.Start_Task_Process_Command()

        '************************************************************************************************
        Notify_Process_Status(MyName & "," & listview_CONTROL & "," & UNKNOW_status)
        '************************************************************************************************

        Dim ProcessMessages As New ServiSwitch_ProcessMessages
        ProcessMessages.Init_Task_Process_Request(Mod_TaskNumbers)

        '*************************************************************
        'INICIA TAREA PRINCIPAL DE CONEXION AL HOST SWITCH
        '*************************************************************
        'Init_Thread_Comms_Socket(MyName)

        '*************************************************************
        'INICIA TAREA DE REGISTRO DE LOG DIARIO
        '*************************************************************
        'ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf MemRes))

        Thread.Sleep(5000)
        'ShowWindow(GetConsoleWindow(), SW_HIDE)   'OCULTA LA VENTANA TEMPORAL

        Init_Task_Save_Log()
        '******************************************************************************        
        Dim CurrentProcess As Process = Process.GetCurrentProcess
        CurrentProcess.PriorityClass = ProcessPriorityClass.BelowNormal
        '******************************************************************************        
        '************************************************************************************************

        Notify_Process_Status(MyName & "," & listview_CONTROL & "," & STARTED_status)

        GC.Collect()
        ClearMemory()

AlwaysWait:
        Show_Message_Console(MyName & "  SUCCESSFULL SYSTEM STARTUP ", COLOR_WHITE, COLOR_BLACK, 0, TRACE_LOW, 1)
        Thread.Sleep(-1)

MainExit:
        Console.WriteLine("Aplicacion terminada.....")
        Console.ReadKey()
        Console.WriteLine("BYE.....")
        Thread.Sleep(1000)
    End Sub


    Public Function Application_ConsoleEvent(ByVal [event] As ConsoleEvent) As Boolean
        Dim CANCEL As Boolean = False

        Notify_Process_Status(MyName & "," & listview_CONTROL & "," & UNKNOW_status)
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

        Return CANCEL ' handling the event.

    End Function

    Friend Sub ClearMemory()
        Try
            GC.Collect()
            GC.WaitForPendingFinalizers()
            If Environment.OSVersion.Platform = PlatformID.Win32NT Then
                SetProcessWorkingSetSize(System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1)
            End If
        Catch ex As Exception
            Show_Message_Console(MyName & " Exception:" & ex.Message, COLOR_BLACK, COLOR_RED, 0, TRACE_LOW, 1)
        End Try
    End Sub

    Private Sub ProcessInitHideProcess()

        ShowWindow(GetConsoleWindow(), SW_HIDE)

    End Sub


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
                        'If ProcessName = RetrieveName Then
                        Pmem = DirectCast(result.GetPropertyValue("WorkingSetPrivate"), ULong)
                        Pmem = Pmem / 1024
                        ValMem = Pmem
                        If Pmem > 5000 Then
                            'Console.WriteLine(" MEM:" & Pmem.ToString & " ")
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
