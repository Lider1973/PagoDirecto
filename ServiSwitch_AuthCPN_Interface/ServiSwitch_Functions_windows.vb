Imports System.Runtime.InteropServices
Imports System.Windows.Forms
Imports System.Threading
'Imports System.Drawing

Module ServiSwitch_Functions_windows

    Public Const FLASHW_STOP As UInt32 = 0
    Public Const FLASHW_CAPTION As UInt32 = 1
    Public Const FLASHW_TRAY As UInt32 = 2
    Public Const FLASHW_ALL As UInt32 = 3
    Public Const FLASHW_TIMER As UInt32 = 4
    Public Const FLASHW_TIMERNOFG As UInt32 = 12

    <DllImport("user32.dll")> Private Function FlashWindowEx(ByRef pwfi As FLASHWINFO) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    Public Declare Function GetConsoleWindow Lib "kernel32.dll" () As IntPtr
    Public Declare Function ShowWindow Lib "user32.dll" (ByVal hwnd As IntPtr, ByVal nCmdShow As Int32) As Int32

    <StructLayout(LayoutKind.Sequential)> _
    Public Structure FLASHWINFO
        Public cbSize As UInt32
        Public hwnd As IntPtr
        Public dwFlags As UInt32
        Public uCount As UInt32
        Public dwTimeout As Int32
    End Structure

    Public Sub FlashWindowBlink(hWnd As IntPtr)
        Dim fInfo As New FLASHWINFO()

        fInfo.cbSize = Convert.ToUInt32(Marshal.SizeOf(fInfo))
        fInfo.hwnd = hWnd
        fInfo.dwFlags = FLASHW_ALL
        fInfo.uCount = UInt32.MaxValue
        fInfo.dwTimeout = 0
        FlashWindowEx(fInfo)
    End Sub

    Public Sub FlashWindowNormal(hWnd As IntPtr)
        Dim fInfo As New FLASHWINFO()

        fInfo.cbSize = Convert.ToUInt32(Marshal.SizeOf(fInfo))
        fInfo.hwnd = hWnd
        fInfo.dwFlags = FLASHW_STOP
        fInfo.uCount = UInt32.MaxValue
        fInfo.dwTimeout = 0
        FlashWindowEx(fInfo)

    End Sub

End Module
