Module MainShared
    Public DIC_CTRL_time As New Dictionary(Of String, DateTime)

    Public ModuleList As New List(Of String)

    Dim lockOBJ_IN As Object = New Object
    'Public aNamesP() As String
    Public aNamesP As String() = New String() {}

    Public Function GetDateTime() As String
        Return System.DateTime.Now.Year & "-" & Format(System.DateTime.Now.Month, "00") & "-" & Format(System.DateTime.Now.Day, "00") & " " & Format(System.DateTime.Now.Hour, "00") & ":" & Format(System.DateTime.Now.Minute, "00") & ":" & Format(System.DateTime.Now.Second, "00") & "." & Format(System.DateTime.Now.Millisecond, "000 ")
    End Function

    Public Function Get_Value_Process(ByVal ProcessName As String) As DateTime
        Dim lDate1 As DateTime
        Dim lDate2 As DateTime

        SyncLock lockOBJ_IN
            If DIC_CTRL_time.TryGetValue(ProcessName, lDate1) Then
                lDate2 = lDate1
            End If
        End SyncLock

        Return lDate2

    End Function

    Public Sub Set_Value_Process(ByVal ProcessName As String, ByVal lDate As DateTime)

        SyncLock lockOBJ_IN
            If DIC_CTRL_time.ContainsKey(ProcessName) Then
                DIC_CTRL_time(ProcessName) = lDate
            End If
        End SyncLock

    End Sub

    'Public Function Get_Index_Process(ByVal oIndex As Byte) As DateTime
    '    Dim DTT As DateTime
    '    SyncLock lockOBJ_IN
    '        DTT = DIC_CTRL_time.ElementAt(oIndex)
    '    End SyncLock

    '    Return DTT
    'End Function


End Module
