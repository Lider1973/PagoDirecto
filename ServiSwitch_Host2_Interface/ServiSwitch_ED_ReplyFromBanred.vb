Imports System.Runtime.InteropServices

Public Class ServiSwitch_ED_ReplyFromBanred

    Public Sub Process_Reply_Inquiry_Transaction(ByVal RequestBuffer As String, ByVal IdMessage As Int64, ByVal Times As String)
        Dim BUSSREPLY As New SharedStructureMessage
        Dim MESSAGE As New Messages
        Dim ErrorMessage As String = Nothing
        Dim ErrorCode As Byte = 0
        Dim TempStr As String = String.Empty
        Dim USER_DATA As String = String.Empty
        Dim RepInq As FixedStrucure.ReplyInquiry

        Try
            Dim pBuf As IntPtr = Marshal.StringToBSTR(RequestBuffer)
            RepInq = CType(Marshal.PtrToStructure(pBuf, GetType(FixedStrucure.ReplyInquiry)), FixedStrucure.ReplyInquiry)
        Catch ex As Exception
            Show_Message_Console(MyName & " Can't decodify the received message :" & ex.Message, COLOR_BLACK, COLOR_RED, 1, TRACE_LOW, 1)
            Exit Sub
        End Try
        'Show_Message_Console("Response Code:" & RepInq.ResultCode, COLOR_BLACK, COLOR_GRAY, 0, TRACE_LOW, 0)
        If RetrieveOriginalRequest(BUSSREPLY, RequestBuffer.Substring(0, 25), from_TRANSACTION) = SUCCESSFUL Then
            Try
                BUSSREPLY.SSM_Transaction_Indicator = "P"
                BUSSREPLY.SSM_Instance_Times = GetDateTime()
                '******************************************************************************
                BUSSREPLY.SSM_Common_Data.CRF_Message_Code = "0210"
                BUSSREPLY.SSM_Common_Data.CRF_Response_Code = RepInq.ResultCode
                BUSSREPLY.SSM_Common_Data.CRF_PhoneNumber = RepInq.SEGo_TargetPhone
                BUSSREPLY.SSM_Common_Data.CRF_Names = RepInq.SEGo_AcctName.Trim
                Dim ED As New ServiSwitch_ED_ReplyToBanred
                Build_User_Token(USER_DATA, ACCT_TYPE_TARGET_ID, RepInq.SEGo_AcctType)
                Build_User_Token(USER_DATA, TARGET_DOCUMENT_ID, RepInq.SEGo_TargetID)
                BUSSREPLY.SSM_Common_Data.CRF_Token_Data += USER_DATA
            Catch ex As Exception
                SaveLogMain("Process_Reply_Inquiry_Transaction Can't fill the main internal buss data:" & ex.Message)
                Show_Message_Console(MyName & " Can't fill the main internal buss data :", COLOR_BLACK, COLOR_YELLOW, 1, TRACE_LOW, 1)
            End Try
            '****************************************************************
            '                    SEND REPLY MESSAGE TO ROUTER
            '****************************************************************
            If Put_Message_To_Router(BUSSREPLY, BUSSREPLY.SSM_Rout_Queue_Reply_Name) <> SUCCESSFUL Then
                Show_Message_Console(" Router not available to Reply Message", COLOR_BLACK, COLOR_RED, 0, TRACE_LOW, 0)
                Exit Sub
            End If
        Else
            Show_Message_Console(MyName & " Can't retrieve original request", COLOR_BLACK, COLOR_YELLOW, 0, TRACE_LOW, 0)
        End If

        Dim MSG As String = "Msg:" & RepInq.MessageType
        MSG += " trx:" & BUSSREPLY.SSM_Common_Data.CRF_Transaction_Code
        MSG += " adq:" & BUSSREPLY.SSM_Common_Data.CRF_Adquirer_Institution_Number
        MSG += " aut:" & BUSSREPLY.SSM_Common_Data.CRF_Issuer_Institution_Number
        MSG += " seq:" & BUSSREPLY.SSM_Common_Data.CRF_Adquirer_Sequence
        MSG += " amt:" & BUSSREPLY.SSM_Common_Data.CRF_Transaction_Amount
        MSG += " cod:" & BUSSREPLY.SSM_Common_Data.CRF_Response_Code
        Show_Message_Console(MSG, COLOR_BLACK, COLOR_WHITE, 0, TRACE_LOW, 0)

        BUSSREPLY = Nothing
        MESSAGE = Nothing
        ErrorMessage = Nothing
        ErrorCode = 0
    End Sub

    Public Sub Process_Reply_Transfer_Transaction(ByVal RequestBuffer As String, ByVal IdMessage As Int64, ByVal Times As String)
        Dim BUSSREPLY As New SharedStructureMessage
        Dim MESSAGE As New Messages
        Dim ErrorMessage As String = Nothing
        Dim ErrorCode As Byte = 0
        Dim TempStr As String = String.Empty
        Dim USER_DATA As String = String.Empty
        Dim RepTran As FixedStrucure.ReplyTransfer

        Try
            Dim pBuf As IntPtr = Marshal.StringToBSTR(RequestBuffer)
            RepTran = CType(Marshal.PtrToStructure(pBuf, GetType(FixedStrucure.ReplyTransfer)), FixedStrucure.ReplyTransfer)
        Catch ex As Exception
            Show_Message_Console(MyName & " Can't decodify the received message :" & ex.Message, COLOR_BLACK, COLOR_RED, 1, TRACE_LOW, 1)
            Exit Sub
        End Try
        'Show_Message_Console("Response Code:" & RepTran.ResultCode, COLOR_BLACK, COLOR_GRAY, 0, TRACE_LOW, 0)
        If RetrieveOriginalRequest(BUSSREPLY, RequestBuffer.Substring(0, 25), from_TRANSACTION) = SUCCESSFUL Then
            Try
                BUSSREPLY.SSM_Transaction_Indicator = "P"
                BUSSREPLY.SSM_Instance_Times = Format_Valid_Times(BUSSREPLY.SSM_Instance_Times, Times)
                '******************************************************************************
                BUSSREPLY.SSM_Common_Data.CRF_Message_Code = "0210"
                BUSSREPLY.SSM_Common_Data.CRF_Response_Code = RepTran.ResultCode
                BUSSREPLY.SSM_Common_Data.CRF_PhoneNumber = RepTran.SEGo_TargetPhone
                BUSSREPLY.SSM_Common_Data.CRF_Names = RepTran.SEGo_AcctName.Trim
                Dim ED As New ServiSwitch_ED_ReplyToBanred
                Build_User_Token(USER_DATA, ACCT_TYPE_TARGET_ID, RepTran.SEGo_AcctType)
                Build_User_Token(USER_DATA, TARGET_DOCUMENT_ID, RepTran.SEGo_TargetID)
                BUSSREPLY.SSM_Common_Data.CRF_Token_Data += USER_DATA
            Catch ex As Exception
                SaveLogMain(MyName & " Can't fill the main internal buss data:" & ex.Message)
                Show_Message_Console(MyName & " Can't fill the main internal buss data :", COLOR_BLACK, COLOR_YELLOW, 1, TRACE_LOW, 1)
            End Try
            '****************************************************************
            '                    SEND REPLY MESSAGE TO ROUTER
            '****************************************************************
            If Put_Message_To_Router(BUSSREPLY, BUSSREPLY.SSM_Rout_Queue_Reply_Name) <> SUCCESSFUL Then
                Show_Message_Console(" Router not available to Reply Message", COLOR_BLACK, COLOR_RED, 0, TRACE_LOW, 0)
                Exit Sub
            End If
        Else
            Show_Message_Console(MyName & " Can't retrieve original request", COLOR_BLACK, COLOR_YELLOW, 0, TRACE_LOW, 0)
        End If

        If BUSSREPLY.SSM_Common_Data.CRF_Response_Code = condError_TIMEOUT Then
            Evaluate_Reverse_Build(BUSSREPLY)
        End If

        Dim MSG As String = "Msg:" & RepTran.MessageType
        MSG += " trx:" & BUSSREPLY.SSM_Common_Data.CRF_Transaction_Code
        MSG += " adq:" & BUSSREPLY.SSM_Common_Data.CRF_Adquirer_Institution_Number
        MSG += " aut:" & BUSSREPLY.SSM_Common_Data.CRF_Issuer_Institution_Number
        MSG += " seq:" & BUSSREPLY.SSM_Common_Data.CRF_Adquirer_Sequence
        MSG += " amt:" & BUSSREPLY.SSM_Common_Data.CRF_Transaction_Amount
        MSG += " cod:" & BUSSREPLY.SSM_Common_Data.CRF_Response_Code
        Show_Message_Console(MSG, COLOR_BLACK, COLOR_WHITE, 0, TRACE_LOW, 0)

        BUSSREPLY = Nothing
        MESSAGE = Nothing
        ErrorMessage = Nothing
        ErrorCode = 0
    End Sub

    Public Sub Process_Reply_Reversal_Transaction(ByVal RequestBuffer As String, ByVal IdMessage As Int64, ByVal Times As String)
        Dim BUSSREPLY As New SharedStructureMessage
        Dim MESSAGE As New Messages
        Dim ErrorMessage As String = Nothing
        Dim ErrorCode As Byte = 0
        Dim TempStr As String = String.Empty
        Dim USER_DATA As String = String.Empty
        Dim RepRev As FixedStrucure.ReplyReversal

        Try
            Dim pBuf As IntPtr = Marshal.StringToBSTR(RequestBuffer)
            RepRev = CType(Marshal.PtrToStructure(pBuf, GetType(FixedStrucure.ReplyReversal)), FixedStrucure.ReplyReversal)
        Catch ex As Exception
            Show_Message_Console(MyName & " Can't decodify the received message :" & ex.Message, COLOR_BLACK, COLOR_RED, 1, TRACE_LOW, 1)
            Exit Sub
        End Try
        'Show_Message_Console("Response Code:" & RepRev.ResultCode, COLOR_BLACK, COLOR_GRAY, 0, TRACE_LOW, 0)
        If RetrieveOriginalRequest(BUSSREPLY, RequestBuffer.Substring(0, 25), from_TRANSACTION) = SUCCESSFUL Then
            Try
                BUSSREPLY.SSM_Transaction_Indicator = "P"
                BUSSREPLY.SSM_Instance_Times = GetDateTime()
                '******************************************************************************
                BUSSREPLY.SSM_Common_Data.CRF_Message_Code = "0430"
                BUSSREPLY.SSM_Common_Data.CRF_Response_Code = RepRev.ResultCode
                BUSSREPLY.SSM_Common_Data.CRF_Token_Data += USER_DATA
            Catch ex As Exception
                SaveLogMain(MyName & " Can't fill the main internal buss data:" & ex.Message)
                Show_Message_Console(MyName & " Can't fill the main internal buss data :", COLOR_BLACK, COLOR_YELLOW, 1, TRACE_LOW, 1)
            End Try
            '****************************************************************
            '                    SEND REPLY MESSAGE TO ROUTER
            '****************************************************************
            If Put_Message_To_Router(BUSSREPLY, BUSSREPLY.SSM_Rout_Queue_Reply_Name) <> SUCCESSFUL Then
                Show_Message_Console(" Router not available to Reply Message", COLOR_BLACK, COLOR_RED, 0, TRACE_LOW, 0)
                Exit Sub
            End If
        Else
            Show_Message_Console(MyName & " Can't retrieve original request", COLOR_BLACK, COLOR_YELLOW, 0, TRACE_LOW, 0)
        End If
        '----------------------------------------------------------------------------------------------
        '2022-10-31
        'If (RequestBuffer.Substring(0, 2) = "XC") AndAlso (GetPendingCountC() > 0) Then
        '    If RetrieveConditionalRequest(BUSSREPLY, RequestBuffer.Substring(0, 25), from_TRANSACTION) = SUCCESSFUL Then
        '        Show_Message_Console(MyName & " Ok, Conditional Request received on good time...", COLOR_BLACK, COLOR_YELLOW, 0, TRACE_LOW, 0)
        '        Put_Notify_To_Router(Router_NOTIFY & "1|1|" & BUSSREPLY.SSM_Common_Data.CRF_Issuer_Institution_Number)
        '    Else
        '        Show_Message_Console(MyName & " Can't retrieve conditional request..............", COLOR_BLACK, COLOR_YELLOW, 0, TRACE_LOW, 0)
        '    End If
        'End If
        '2022-10-31
        '----------------------------------------------------------------------------------------------

        Dim MSG As String = "Msg:" & RepRev.MessageType
        MSG += " trx:" & BUSSREPLY.SSM_Common_Data.CRF_Transaction_Code
        MSG += " adq:" & BUSSREPLY.SSM_Common_Data.CRF_Adquirer_Institution_Number
        MSG += " aut:" & BUSSREPLY.SSM_Common_Data.CRF_Issuer_Institution_Number
        MSG += " seq:" & BUSSREPLY.SSM_Common_Data.CRF_Adquirer_Sequence
        MSG += " amt:" & BUSSREPLY.SSM_Common_Data.CRF_Transaction_Amount
        MSG += " cod:" & BUSSREPLY.SSM_Common_Data.CRF_Response_Code
        Show_Message_Console(MSG, COLOR_BLACK, COLOR_WHITE, 0, TRACE_LOW, 0)

        BUSSREPLY = Nothing
        MESSAGE = Nothing
        ErrorMessage = Nothing
        ErrorCode = 0
    End Sub

End Class
