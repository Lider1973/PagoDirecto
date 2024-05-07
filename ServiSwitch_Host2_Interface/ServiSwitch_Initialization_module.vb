Imports System.Threading
Imports System.Configuration
Imports System.IO
Imports System.Reflection
Imports System.Reflection.Emit
Imports System.Globalization
Imports System.Management
Imports System.Attribute
Imports System.Threading.Tasks

Module ServiSwitch_Initialization_module
    Dim dato As String
    Dim Continuing As New AutoResetEvent(False)
    Dim EVENT_ROOT As String
    Dim Pmem As Int64
    Public ValMem As Int64
    Dim HT_Fi_Down As New Hashtable

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
        Show_Message_Console("         Iniciando sistema, Modulo Host2 Interface          ", COLOR_BLACK, COLOR_GREEN, 2, TRACE_LOW, 0)
        Show_Message_Console("      Fecha & Hora de creacion:" & infoReader.LastWriteTime, COLOR_BLACK, COLOR_GREEN, 2, TRACE_LOW, 0)
        Show_Message_Console("-------------------------------------------------------------", COLOR_BLACK, COLOR_GREEN, 2, TRACE_LOW, 0)
        Console.Title = "HOST2 Interface - Build:" & infoReader.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss") & " Start:" & Now.ToString("yyyy-MM-dd HH:mm:ss")
        infoReader = Nothing
        PathInfo = Nothing
        '************************************************************************************************
        lTOUTinq = My.Settings.repToutInq
        lTOUTtrf = My.Settings.repToutTrf
        lTOUTrev = My.Settings.repToutRev
        '************************************************************************************************
        If Load_Setting_Database() <> SUCCESSFUL Then
            GoTo MainExit
        End If
        '**********************************************************************************************
        '              INICIA TAREA PRINCIPAL DE RECEPCION DE REQUERIMIENTOS & RESPUESTAS
        '**********************************************************************************************
        Dim ProcessCommand As New ServiSwitch_ProcessCommand
        ProcessCommand.Start_Task_Process_Command()

        Dim ProcessMessages As New ServiSwitch_ProcessMessages
        ProcessMessages.Init_Task_Process_Request(Mod_TaskNumbers)

        '*************************************************************
        'INICIA TAREA PRINCIPAL DE CONEXION AL HOST SWITCH
        '*************************************************************
        Init_Thread_Comms_Socket(MyName)

        '*************************************************************
        'INICIA TAREA DE REGISTRO DE LOG DIARIO
        '*************************************************************
        Init_Task_Save_Log()
        '*************************************************************

        'HT_Fi_Down.Add(701, Now)
        'Put_Notify_To_Router(Router_NOTIFY & "1|0|701")
        'ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf Process_Check_Exceeded_Out))

        Show_Message_Console(MyName & "  SUCCESSFULL SYSTEM STARTUP ", COLOR_WHITE, COLOR_BLACK, 0, TRACE_LOW, 1)

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

    Private Sub Auto_Killing_Process()

        Dim timerSeconds As Integer = 20
        While System.Math.Max(System.Threading.Interlocked.Decrement(timerSeconds), timerSeconds + 1) > 0
            Console.Out.Write(vbCr & " Seconds before exiting :" & timerSeconds)
            System.Threading.Thread.Sleep(1000)
        End While
        System.Environment.Exit(0)

    End Sub


    Public Function Application_ConsoleEvent(ByVal [event] As ConsoleEvent) As Boolean
        Dim cancel As Boolean = False
        Dim CMD As New ServiSwitch_ProcessCommand
        CMD.Send_Status_Commander(3)

        'Notify_Process_Status(MyName & "," & listview_CONTROL & "," & UNKNOW_status)
        Select Case [event]
            Case ConsoleEvent.CTRL_C_EVENT
                Exiting = True
                Notify_Process_Status(MyName & "," & listview_CONTROL & "," & UNKNOW_status)
                SaveLogMain(" El sistema ha sido concluido de manera insegura ")
                'MsgBox(" El sistema ha sido concluido de manera insegura ")
                Show_Message_Console(MyName & " El sistema ha sido concluido de manera insegura ", COLOR_OWNER1, COLOR_RED, 0, TRACE_LOW, 1)
                'DeleteAllQueues()
                Return False
            Case ConsoleEvent.CTRL_BREAK_EVENT
                Show_Message_Console(MyName & " El sistema ha sido concluido de manera insegura ", COLOR_OWNER1, COLOR_RED, 0, TRACE_LOW, 1)
                'MsgBox("CTRL+BREAK received!")
            Case ConsoleEvent.CTRL_CLOSE_EVENT
                Exiting = True
                Notify_Process_Status(MyName & "," & listview_CONTROL & "," & UNKNOW_status)
                SaveLogMain(" El sistema ha sido concluido de manera insegura ")
                'MsgBox(" El sistema ha sido concluido de manera insegura ")
                Show_Message_Console(MyName & " El sistema ha sido concluido de manera insegura ", COLOR_OWNER1, COLOR_RED, 0, TRACE_LOW, 1)
                'DeleteAllQueues()
                Return False
            Case ConsoleEvent.CTRL_LOGOFF_EVENT
                MsgBox("User is logging off!")
            Case ConsoleEvent.CTRL_SHUTDOWN_EVENT
                MsgBox("Windows is shutting down.")
                ' My cleanup code here
        End Select

        Thread.Sleep(1000)
        Return cancel ' handling the event.

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

    Public Async Sub Wait_Pending_For_Timeout(ByVal KeyHT As String)
        'Console.WriteLine("Wait_Pending_For_Timeout OK")
        Dim ErrorCode As Byte
        Await Task.Run(Async Function()
                           ErrorCode = Await Waiting_TimeOut_Request(KeyHT)
                       End Function)
    End Sub

    Private Async Function Waiting_TimeOut_Request(ByVal KeyHT As String) As Task(Of Byte)

        Await Task.Delay(CLng(Mod_Timeout) * 1000)

        If GetPendingCount() > 0 Then
            Dim MainQueueStruct As New SharedStructureMessage
            If RetrieveOriginalRequest(MainQueueStruct, KeyHT, from_TIMEOUT) = SUCCESSFUL Then
                Reply_Exception_To_Source(MainQueueStruct, condError_TIMEOUT)
                Show_Message_Console(MyName & " Time out of response Key:" & KeyHT, COLOR_BLACK, COLOR_YELLOW, 0, TRACE_LOW, 1)
                Evaluate_Reverse_Build(MainQueueStruct)
            End If
        End If
        Return SUCCESSFUL
    End Function

    Public Sub Evaluate_Reverse_Build(ByVal MainQueueStruct As SharedStructureMessage)
        Select Case MainQueueStruct.SSM_Common_Data.CRF_Transaction_Code
            Case 239, 439, 539
                If MainQueueStruct.SSM_Common_Data.CRF_Reversal_Indicator = 0 Then
                    MainQueueStruct.SSM_Common_Data.CRF_Reversal_Indicator = 1
                    Dim RequestBANRED As New ServiSwitch_ED_RequestToBanred
                    Dim RequestMessage As String = RequestBANRED.Encode_Reversal_Request(MainQueueStruct)
                    If Get_Socket_Status() = CONNECTED_status Then
                        If Send_TCPIP_Message(RequestMessage) = SUCCESSFUL Then
                            Dim KeyHT As String = "XC" & RequestMessage.Substring(2, 23)
                            RegistryFinantialRequest(MainQueueStruct, KeyHT)
                            Wait_Pending_For_Timeout(KeyHT)
                            'SaveLogMain(RequestMessage)
                            Show_Message_Console(MyName & "***** Conditional reverse has been sended *****", COLOR_BLACK, COLOR_YELLOW, 0, TRACE_LOW, 0)
                            SaveLogMain("***** Conditional reverse has been sended *****")
                            Put_Notify_To_Router(Router_NOTIFY & "1|0|" & MainQueueStruct.SSM_Common_Data.CRF_Issuer_Institution_Number)
                        End If
                    End If
                End If
        End Select
    End Sub

    Public Sub Reply_Exception_To_Source(ByVal Struct_Request_Message As SharedStructureMessage, ByVal ErrorCode As String)
        Struct_Request_Message.SSM_Instance_Times += "T10_" & GetDateTime() & Concatenator
        Struct_Request_Message.SSM_Common_Data.CRF_Response_Code = ErrorCode
        Struct_Request_Message.SSM_Common_Data.CRF_Authorization_Code = "000000"
        Struct_Request_Message.SSM_Transaction_Indicator = TranType_REPLY
        Put_Message_To_Router(Struct_Request_Message, Struct_Request_Message.SSM_Rout_Queue_Reply_Name)
    End Sub


End Module
