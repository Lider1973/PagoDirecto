Imports System.Runtime.InteropServices
Imports System.Globalization

Public Class ServiSwitch_ED_RequestFromBanred
    Public Sub Process_Request_To_LocalAuth_Inquiry(ByVal RequestBuffer As String, ByVal IdMessage As Int64, ByVal Times As String)
        Dim BUSS As New SharedStructureMessage
        Dim MESSAGE As New Messages
        Dim ErrorMessage As String = Nothing
        Dim ErrorCode As Byte = 0
        Dim TempStr As String = String.Empty
        Dim USER_DATA As String = String.Empty
        Dim ReqInq As FixedStrucure.RequestInquiry

        Try
            Dim pBuf As IntPtr = Marshal.StringToBSTR(RequestBuffer)
            ReqInq = CType(Marshal.PtrToStructure(pBuf, GetType(FixedStrucure.RequestInquiry)), FixedStrucure.RequestInquiry)
        Catch ex As Exception
            Show_Message_Console(MyName & " Can't decodify the received message :" & ex.Message, COLOR_BLACK, COLOR_RED, 1, TRACE_LOW, 1)
            Exit Sub
        End Try

        Try
            BUSS.SSM_Instance_Times = Times
            '******************************************************************************
            '**********  Decline Transaction for Invalid ApplCode
            'Console.WriteLine("CARD APPL CODE:" & ReqInq.CardApplCode)
            If ReqInq.CardApplCode <> "05" Then
                'Console.WriteLine("INGRESO RUTINA")
                Process_Reply_Inquiry_NotEnabled(RequestBuffer, condError_INVALID_APPL)
                'Console.WriteLine("SALGO RUTINA")
                Exit Sub
            End If
            '**********  
            '******************************************************************************
            BUSS.SSM_Adq_Queue_Reply_Name = g_ReplyQueue
            BUSS.SSM_Rout_Queue_Request_Name = g_RouterRequestQueue
            BUSS.SSM_Adq_Source_Name = MyName
            BUSS.SSM_Queue_Message_ID = GetCorrelationID()
            BUSS.SSM_Transaction_Indicator = TranType_ROUTER_IN_SCP
            BUSS.SSM_Communication_ID = Constanting_Definition.from_INTERFACES
            BUSS.SSM_Rout_Source_Name = Mod_RouterName
            BUSS.SSM_Message_Format = Constanting_Definition.HOST2_format
            '******************************************************************************
            '******************************************************************************
            BUSS.SSM_Common_Data.CRF_Terminal_ID = ReqInq.SourceTrmlNbr
            BUSS.SSM_Common_Data.CRF_Adquirer_Sequence = ReqInq.SourceSeqNbr
            BUSS.SSM_Common_Data.CRF_Switch_Sequence = ReqInq.MessageSeqNbr
            BUSS.SSM_Common_Data.CRF_Transaction_Code = ReqInq.TransactionCode
            BUSS.SSM_Common_Data.CRF_Adquirer_Date_Time = DateTime.ParseExact(ReqInq.SourceDate & ReqInq.SourceTime, "yyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None)
            BUSS.SSM_Common_Data.CRF_Adquirer_Institution_ID = ReqInq.SourceAbaNbr
            BUSS.SSM_Common_Data.CRF_Adquirer_Institution_Number = GetInfoInstitution(CInt(ReqInq.SourceAbaNbr))

            Select Case CInt(ReqInq.TerminalType)
                Case 1
                    BUSS.SSM_Common_Data.CRF_Channel_Id = 3
                Case 2
                    BUSS.SSM_Common_Data.CRF_Channel_Id = 4
                Case Else
                    BUSS.SSM_Common_Data.CRF_Channel_Id = ReqInq.TerminalType
            End Select

            BUSS.SSM_Common_Data.CRF_Transaction_Amount = 0.0
            BUSS.SSM_Common_Data.CRF_Primary_Account = ReqInq.AccountNbr
            BUSS.SSM_Common_Data.CRF_Secondary_Account = ReqInq.AccountNbr
            BUSS.SSM_Common_Data.CRF_Service_Indicator = SERVICE_CPN_PDR
            BUSS.SSM_Common_Data.CRF_Issuer_Institution_ID = FI_CPN_AUT
            BUSS.SSM_Common_Data.CRF_Issuer_Institution_Number = FI_CPN_AUT
            BUSS.SSM_Common_Data.CRF_Branch_ID = ReqInq.SourceBranchNbr
            BUSS.SSM_Common_Data.CRF_Reversal_Indicator = CInt(ReqInq.ReversalIndicator)
            BUSS.SSM_Common_Data.CRF_Names = ReqInq.SEGi_TargetName.Trim
            BUSS.SSM_Common_Data.CRF_Operator_ID = ReqInq.SourceTrmlNbr
            BUSS.SSM_Common_Data.CRF_Reference = ReqInq.SEGi_Reference
            BUSS.SSM_Common_Data.CRF_PhoneNumber = ReqInq.TargetPhone
            '
            '*************************  User Data 
            '
            Build_User_Token(USER_DATA, ACCT_TYPE_ID, ReqInq.SourceApplType)
            Build_User_Token(USER_DATA, ACCT_TYPE_TARGET_ID, ReqInq.CardApplCode)
            Build_User_Token(USER_DATA, NAMES_ID, ReqInq.SourceName)
            '***************************************************************
            Dim TempId As String
            TempId = Convert.ToInt64(ReqInq.SEGi_SourceID)
            If TempId.Length < 10 Then
                TempId = TempId.PadLeft(10, "0")
            ElseIf TempId.Length > 10 Then
                TempId = TempId.PadLeft(13, "0")
            End If
            Build_User_Token(USER_DATA, DOCUMENT_ID, TempId)
            '****************************************************************
            TempId = Convert.ToInt64(ReqInq.SEGi_TargetID)
            If TempId.Length < 10 Then
                TempId = TempId.PadLeft(10, "0")
            ElseIf TempId.Length > 10 Then
                TempId = TempId.PadLeft(13, "0")
            End If
            Build_User_Token(USER_DATA, TARGET_DOCUMENT_ID, TempId)
            '****************************************************************
            Build_User_Token(USER_DATA, SEQ_MESSAGE_ID, ReqInq.MessageSeqNbr)

            BUSS.SSM_Common_Data.CRF_Token_Data = USER_DATA

            '*************************  Putting message
            If Put_Message_To_Router(BUSS, g_RouterRequestQueue) <> SUCCESSFUL Then
                ErrorCode = PROCESS_ERROR
                Process_Reply_Inquiry_NotEnabled(RequestBuffer, condError_ROUTER_UNAVAILABLE)
                Exit Try
            End If
            '*************************  Putting message
            ErrorCode = SUCCESSFUL
        Catch ex As Exception
            Show_Message_Console(MyName & " Error procesing decode & fill inquiry request :" & ex.Message, COLOR_BLACK, COLOR_RED, 1, TRACE_LOW, 1)
            BUSS = Nothing
            ErrorCode = PROCESS_ERROR
        End Try

        Dim MSG As String = "Msg:" & ReqInq.MessageType
        MSG += " trx:" & BUSS.SSM_Common_Data.CRF_Transaction_Code
        MSG += " adq:" & BUSS.SSM_Common_Data.CRF_Adquirer_Institution_Number
        MSG += " aut:" & BUSS.SSM_Common_Data.CRF_Issuer_Institution_Number
        MSG += " seq:" & BUSS.SSM_Common_Data.CRF_Adquirer_Sequence
        MSG += " amt:" & BUSS.SSM_Common_Data.CRF_Transaction_Amount
        Show_Message_Console(MSG, COLOR_BLACK, COLOR_DARK_GRAY, 0, TRACE_LOW, 0)

        BUSS = Nothing
        MESSAGE = Nothing
        ErrorMessage = Nothing
        ErrorCode = 0
    End Sub

    Public Sub Process_Request_To_LocalAuth_Transfer(ByVal RequestBuffer As String, ByVal IdMessage As Int64, ByVal Times As String)
        Dim BUSS As New SharedStructureMessage
        Dim MESSAGE As New Messages
        Dim ErrorMessage As String = Nothing
        Dim ErrorCode As Byte = 0
        Dim TempStr As String = String.Empty
        Dim USER_DATA As String = String.Empty
        Dim ReqTran As FixedStrucure.RequestTransfer

        Try
            Dim pBuf As IntPtr = Marshal.StringToBSTR(RequestBuffer)
            ReqTran = CType(Marshal.PtrToStructure(pBuf, GetType(FixedStrucure.RequestTransfer)), FixedStrucure.RequestTransfer)
        Catch ex As Exception
            Show_Message_Console(MyName & " Can't decodify the received message :" & ex.Message, COLOR_BLACK, COLOR_RED, 1, TRACE_LOW, 1)
            Exit Sub
        End Try

        Try
            BUSS.SSM_Instance_Times = Times
            '******************************************************************************
            '******************************************************************************
            BUSS.SSM_Adq_Queue_Reply_Name = g_ReplyQueue
            BUSS.SSM_Rout_Queue_Request_Name = g_RouterRequestQueue
            BUSS.SSM_Adq_Source_Name = MyName
            BUSS.SSM_Queue_Message_ID = GetCorrelationID()
            BUSS.SSM_Transaction_Indicator = TranType_ROUTER_IN_SCP
            BUSS.SSM_Communication_ID = Constanting_Definition.from_INTERFACES
            BUSS.SSM_Rout_Source_Name = Mod_RouterName
            BUSS.SSM_Message_Format = Constanting_Definition.HOST2_format
            '******************************************************************************
            '******************************************************************************
            BUSS.SSM_Common_Data.CRF_Terminal_ID = ReqTran.SourceTrmlNbr
            BUSS.SSM_Common_Data.CRF_Adquirer_Sequence = ReqTran.SourceSeqNbr
            BUSS.SSM_Common_Data.CRF_Switch_Sequence = ReqTran.MessageSeqNbr
            BUSS.SSM_Common_Data.CRF_Transaction_Code = ReqTran.TransactionCode
            BUSS.SSM_Common_Data.CRF_Adquirer_Date_Time = DateTime.ParseExact(ReqTran.SourceDate & ReqTran.SourceTime, "yyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None)
            BUSS.SSM_Common_Data.CRF_Adquirer_Institution_ID = ReqTran.SourceAbaNbr
            BUSS.SSM_Common_Data.CRF_Adquirer_Institution_Number = GetInfoInstitution(CInt(ReqTran.SourceAbaNbr))

            'Console.WriteLine("TERMINAL TYPE in:" & ReqTran.TerminalType)
            Select Case CInt(ReqTran.TerminalType)
                Case 1
                    BUSS.SSM_Common_Data.CRF_Channel_Id = 3
                Case 2
                    BUSS.SSM_Common_Data.CRF_Channel_Id = 4
                Case Else
                    BUSS.SSM_Common_Data.CRF_Channel_Id = ReqTran.TerminalType
            End Select
            'Console.WriteLine("TERMINAL TYPE in:" & BUSS.SSM_Common_Data.CRF_Channel_Id)

            BUSS.SSM_Common_Data.CRF_Transaction_Amount = CDec(ReqTran.TransactionAmount / 100)

            'BUSS.SSM_Common_Data.CRF_Primary_Account = ReqTran.AccountNbr
            'BUSS.SSM_Common_Data.CRF_Secondary_Account = ReqTran.OtherAcct

            BUSS.SSM_Common_Data.CRF_Primary_Account = ReqTran.OtherAcct
            BUSS.SSM_Common_Data.CRF_Secondary_Account = ReqTran.AccountNbr

            BUSS.SSM_Common_Data.CRF_Service_Indicator = SERVICE_CPN_PDR
            BUSS.SSM_Common_Data.CRF_Issuer_Institution_ID = FI_CPN_AUT
            BUSS.SSM_Common_Data.CRF_Issuer_Institution_Number = FI_CPN_AUT
            BUSS.SSM_Common_Data.CRF_Branch_ID = ReqTran.SourceBranchNbr
            BUSS.SSM_Common_Data.CRF_Reversal_Indicator = CInt(ReqTran.ReversalIndicator)
            BUSS.SSM_Common_Data.CRF_Names = ReqTran.SEGi_TargetName.Trim
            BUSS.SSM_Common_Data.CRF_Operator_ID = ReqTran.SourceTrmlNbr
            BUSS.SSM_Common_Data.CRF_Reference = ReqTran.SEGi_Reference
            BUSS.SSM_Common_Data.CRF_PhoneNumber = ReqTran.TargetPhone

            '
            '*************************  User Data 
            '
            Build_User_Token(USER_DATA, ACCT_TYPE_ID, ReqTran.SourceApplType)
            Build_User_Token(USER_DATA, ACCT_TYPE_TARGET_ID, ReqTran.CardApplCode)
            Build_User_Token(USER_DATA, NAMES_ID, ReqTran.SourceName)

            Dim TempId As String
            TempId = Convert.ToInt64(ReqTran.SEGi_SourceID)
            If TempId.Length < 10 Then
                TempId = TempId.PadLeft(10, "0")
            ElseIf TempId.Length > 10 Then
                TempId = TempId.PadLeft(13, "0")
            End If
            Build_User_Token(USER_DATA, DOCUMENT_ID, TempId)

            TempId = Convert.ToInt64(ReqTran.SEGi_TargetID)
            If TempId.Length < 10 Then
                TempId = TempId.PadLeft(10, "0")
            ElseIf TempId.Length > 10 Then
                TempId = TempId.PadLeft(13, "0")
            End If
            Build_User_Token(USER_DATA, TARGET_DOCUMENT_ID, TempId)

            'Build_User_Token(USER_DATA, DOCUMENT_ID, ReqTran.SEGi_SourceID)
            'Build_User_Token(USER_DATA, TARGET_DOCUMENT_ID, ReqTran.SEGi_TargetID)
            Build_User_Token(USER_DATA, SEQ_MESSAGE_ID, ReqTran.MessageSeqNbr)
            BUSS.SSM_Common_Data.CRF_Token_Data = USER_DATA

            '*************************  Putting message
            If Put_Message_To_Router(BUSS, g_RouterRequestQueue) <> SUCCESSFUL Then
                Process_Reply_Transfer_NotEnabled(RequestBuffer, condError_ROUTER_UNAVAILABLE)
                ErrorCode = PROCESS_ERROR
                Exit Try
            End If
            '*************************  Putting message
            ErrorCode = SUCCESSFUL
        Catch ex As Exception
            Show_Message_Console(MyName & " Error procesing decode & fill transfer request :" & ex.Message, COLOR_BLACK, COLOR_RED, 1, TRACE_LOW, 1)
            BUSS = Nothing
            ErrorCode = PROCESS_ERROR
        End Try


        Dim MSG As String = "Msg:" & ReqTran.MessageType
        MSG += " trx:" & BUSS.SSM_Common_Data.CRF_Transaction_Code
        MSG += " adq:" & BUSS.SSM_Common_Data.CRF_Adquirer_Institution_Number
        MSG += " aut:" & BUSS.SSM_Common_Data.CRF_Issuer_Institution_Number
        MSG += " seq:" & BUSS.SSM_Common_Data.CRF_Adquirer_Sequence
        MSG += " amt:" & BUSS.SSM_Common_Data.CRF_Transaction_Amount
        MSG += " cod:" & BUSS.SSM_Common_Data.CRF_Response_Code
        Show_Message_Console(MSG, COLOR_BLACK, COLOR_DARK_GRAY, 0, TRACE_LOW, 0)

        BUSS = Nothing
        MESSAGE = Nothing
        ErrorMessage = Nothing
        ErrorCode = 0
    End Sub

    Public Sub Process_Request_To_LocalAuth_Reversal_C(ByVal RequestBuffer As String, ByVal IdMessage As Int64, ByVal Times As String)
        Dim BUSS As New SharedStructureMessage
        Dim MESSAGE As New Messages
        Dim ErrorMessage As String = Nothing
        Dim ErrorCode As Byte = 0
        Dim TempStr As String = String.Empty
        Dim USER_DATA As String = String.Empty
        Dim ReqRev As FixedStrucure.RequestReversal_C

        Show_Message_Console(" ----------- EXECUTING CONDITIONAL REVERSE --------------", COLOR_BLACK, COLOR_WHITE, 0, TRACE_LOW, 0)

        Try
            Dim pBuf As IntPtr = Marshal.StringToBSTR(RequestBuffer)
            ReqRev = CType(Marshal.PtrToStructure(pBuf, GetType(FixedStrucure.RequestReversal_C)), FixedStrucure.RequestReversal_C)
        Catch ex As Exception
            Show_Message_Console(MyName & " Can't decodify the received message :" & ex.Message, COLOR_BLACK, COLOR_RED, 1, TRACE_LOW, 1)
            Exit Sub
        End Try

        Try
            BUSS.SSM_Instance_Times = Times
            '******************************************************************************
            '******************************************************************************
            BUSS.SSM_Adq_Queue_Reply_Name = g_ReplyQueue
            BUSS.SSM_Rout_Queue_Request_Name = g_RouterRequestQueue
            BUSS.SSM_Adq_Source_Name = MyName
            BUSS.SSM_Queue_Message_ID = GetCorrelationID()
            BUSS.SSM_Transaction_Indicator = TranType_ROUTER_IN_SCP
            BUSS.SSM_Communication_ID = Constanting_Definition.from_INTERFACES
            BUSS.SSM_Rout_Source_Name = Mod_RouterName
            BUSS.SSM_Message_Format = Constanting_Definition.HOST2_format
            '******************************************************************************
            '******************************************************************************
            BUSS.SSM_Common_Data.CRF_Terminal_ID = ReqRev.SourceTrmlNbr
            BUSS.SSM_Common_Data.CRF_Adquirer_Sequence = ReqRev.SourceSeqNbr
            BUSS.SSM_Common_Data.CRF_Switch_Sequence = ReqRev.MessageSeqNbr

            BUSS.SSM_Common_Data.CRF_Transaction_Code = ReqRev.TransactionCode

            BUSS.SSM_Common_Data.CRF_Adquirer_Date_Time = DateTime.ParseExact(ReqRev.SourceDate & ReqRev.SourceTime, "yyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None)
            BUSS.SSM_Common_Data.CRF_Adquirer_Institution_ID = ReqRev.SourceAbaNbr
            BUSS.SSM_Common_Data.CRF_Adquirer_Institution_Number = GetInfoInstitution(CInt(ReqRev.SourceAbaNbr))

            'Console.WriteLine("TERMINAL TYPE in:" & ReqRev.TerminalType)
            Select Case CInt(ReqRev.TerminalType)
                Case 1
                    BUSS.SSM_Common_Data.CRF_Channel_Id = 3
                Case 2
                    BUSS.SSM_Common_Data.CRF_Channel_Id = 4
                Case Else
                    BUSS.SSM_Common_Data.CRF_Channel_Id = ReqRev.TerminalType
            End Select

            BUSS.SSM_Common_Data.CRF_Transaction_Amount = CDec(ReqRev.TransactionAmount / 100)
            BUSS.SSM_Common_Data.CRF_Primary_Account = ReqRev.OtherAcct
            BUSS.SSM_Common_Data.CRF_Secondary_Account = ReqRev.AccountNbr
            BUSS.SSM_Common_Data.CRF_Service_Indicator = SERVICE_CPN_PDR
            BUSS.SSM_Common_Data.CRF_Issuer_Institution_ID = FI_CPN_AUT
            BUSS.SSM_Common_Data.CRF_Issuer_Institution_Number = FI_CPN_AUT
            BUSS.SSM_Common_Data.CRF_Branch_ID = ReqRev.SourceBranchNbr
            'BUSS.SSM_Common_Data.CRF_Reversal_Indicator = CInt(ReqRev.ReversalIndicator)
            BUSS.SSM_Common_Data.CRF_Reversal_Indicator = REVERSAL_MODE
            BUSS.SSM_Common_Data.CRF_Names = ReqRev.SEGi_TargetName.Trim
            BUSS.SSM_Common_Data.CRF_Operator_ID = ReqRev.SourceTrmlNbr
            BUSS.SSM_Common_Data.CRF_Reference = ReqRev.SEGi_Reference
            BUSS.SSM_Common_Data.CRF_PhoneNumber = ReqRev.TargetPhone
            '
            '*************************  User Data 
            '
            Build_User_Token(USER_DATA, ACCT_TYPE_ID, ReqRev.SourceApplType)
            Build_User_Token(USER_DATA, ACCT_TYPE_TARGET_ID, ReqRev.CardApplCode)
            Build_User_Token(USER_DATA, NAMES_ID, ReqRev.SourceName)

            Dim TempId As String
            TempId = Convert.ToInt64(ReqRev.SEGi_SourceID)
            If TempId.Length < 10 Then
                TempId = TempId.PadLeft(10, "0")
            ElseIf TempId.Length > 10 Then
                TempId = TempId.PadLeft(13, "0")
            End If
            Build_User_Token(USER_DATA, DOCUMENT_ID, TempId)

            TempId = Convert.ToInt64(ReqRev.SEGi_TargetID)
            If TempId.Length < 10 Then
                TempId = TempId.PadLeft(10, "0")
            ElseIf TempId.Length > 10 Then
                TempId = TempId.PadLeft(13, "0")
            End If
            Build_User_Token(USER_DATA, TARGET_DOCUMENT_ID, TempId)

            Build_User_Token(USER_DATA, SEQ_MESSAGE_ID, ReqRev.MessageSeqNbr)
            BUSS.SSM_Common_Data.CRF_Token_Data = USER_DATA

            '*************************  Putting message
            If Put_Message_To_Router(BUSS, g_RouterRequestQueue) <> SUCCESSFUL Then
                Process_Reply_Reversal_NotEnabled(RequestBuffer, condError_ROUTER_UNAVAILABLE)
                ErrorCode = PROCESS_ERROR
                Exit Try
            End If
            '*************************  Putting message
            ErrorCode = SUCCESSFUL
        Catch ex As Exception
            Show_Message_Console(MyName & " Error procesing decode & fill reversal request :" & ex.Message, COLOR_BLACK, COLOR_RED, 1, TRACE_LOW, 1)
            BUSS = Nothing
            ErrorCode = PROCESS_ERROR
        End Try

        Dim MSG As String = "Msg:" & ReqRev.MessageType
        MSG += " trx:" & BUSS.SSM_Common_Data.CRF_Transaction_Code
        MSG += " adq:" & BUSS.SSM_Common_Data.CRF_Adquirer_Institution_Number
        MSG += " aut:" & BUSS.SSM_Common_Data.CRF_Issuer_Institution_Number
        MSG += " seq:" & BUSS.SSM_Common_Data.CRF_Adquirer_Sequence
        MSG += " amt:" & BUSS.SSM_Common_Data.CRF_Transaction_Amount
        Show_Message_Console(MSG, COLOR_BLACK, COLOR_DARK_GRAY, 0, TRACE_LOW, 0)


        BUSS = Nothing
        MESSAGE = Nothing
        ErrorMessage = Nothing
        ErrorCode = 0
    End Sub

    Public Sub Process_Request_To_LocalAuth_Reversal(ByVal RequestBuffer As String, ByVal IdMessage As Int64, ByVal Times As String)
        Dim BUSS As New SharedStructureMessage
        Dim MESSAGE As New Messages
        Dim ErrorMessage As String = Nothing
        Dim ErrorCode As Byte = 0
        Dim TempStr As String = String.Empty
        Dim USER_DATA As String = String.Empty
        Dim ReqRev As FixedStrucure.RequestReversal

        Show_Message_Console(" ----------- EXECUTING REAL REVERSE --------------", COLOR_BLACK, COLOR_WHITE, 0, TRACE_LOW, 0)

        Try
            Dim pBuf As IntPtr = Marshal.StringToBSTR(RequestBuffer)
            ReqRev = CType(Marshal.PtrToStructure(pBuf, GetType(FixedStrucure.RequestReversal)), FixedStrucure.RequestReversal)
        Catch ex As Exception
            Show_Message_Console(MyName & " Can't decodify the received message :" & ex.Message, COLOR_BLACK, COLOR_RED, 1, TRACE_LOW, 1)
            Exit Sub
        End Try

        Try
            BUSS.SSM_Instance_Times = Times
            '******************************************************************************
            '******************************************************************************
            BUSS.SSM_Adq_Queue_Reply_Name = g_ReplyQueue
            BUSS.SSM_Rout_Queue_Request_Name = g_RouterRequestQueue
            BUSS.SSM_Adq_Source_Name = MyName
            BUSS.SSM_Queue_Message_ID = GetCorrelationID()
            BUSS.SSM_Transaction_Indicator = TranType_ROUTER_IN_SCP
            BUSS.SSM_Communication_ID = Constanting_Definition.from_INTERFACES
            BUSS.SSM_Rout_Source_Name = Mod_RouterName
            BUSS.SSM_Message_Format = Constanting_Definition.HOST2_format
            '******************************************************************************
            '******************************************************************************
            BUSS.SSM_Common_Data.CRF_Terminal_ID = ReqRev.SourceTrmlNbr
            'BUSS.SSM_Common_Data.CRF_Adquirer_Sequence = ReqRev.MessageSeqNbr
            'BUSS.SSM_Common_Data.CRF_Switch_Sequence = ReqRev.SourceSeqNbr
            BUSS.SSM_Common_Data.CRF_Adquirer_Sequence = ReqRev.SourceSeqNbr
            BUSS.SSM_Common_Data.CRF_Switch_Sequence = ReqRev.MessageSeqNbr

            If ReqRev.TransactionCode = 524 Then
                BUSS.SSM_Common_Data.CRF_Transaction_Code = 539
            ElseIf ReqRev.TransactionCode = 424 Then
                BUSS.SSM_Common_Data.CRF_Transaction_Code = 439
            ElseIf ReqRev.TransactionCode = 224 Then
                BUSS.SSM_Common_Data.CRF_Transaction_Code = 239
            End If
            'BUSS.SSM_Common_Data.CRF_Transaction_Code = ReqRev.TransactionCode

            BUSS.SSM_Common_Data.CRF_Adquirer_Date_Time = DateTime.ParseExact(ReqRev.SourceDate & ReqRev.SourceTime, "yyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None)
            BUSS.SSM_Common_Data.CRF_Adquirer_Institution_Number = GetInfoInstitution(CInt(ReqRev.SourceAbaNbr))
            BUSS.SSM_Common_Data.CRF_Adquirer_Institution_ID = ReqRev.SourceAbaNbr

            'Console.WriteLine("TERMINAL TYPE in:" & ReqRev.TerminalType)
            Select Case CInt(ReqRev.TerminalType)
                Case 1
                    BUSS.SSM_Common_Data.CRF_Channel_Id = 3
                Case 2
                    BUSS.SSM_Common_Data.CRF_Channel_Id = 4
                Case Else
                    BUSS.SSM_Common_Data.CRF_Channel_Id = ReqRev.TerminalType
            End Select
            'Console.WriteLine("TERMINAL TYPE in:" & BUSS.SSM_Common_Data.CRF_Channel_Id)

            BUSS.SSM_Common_Data.CRF_Transaction_Amount = CDec(ReqRev.TransactionAmount / 100)

            BUSS.SSM_Common_Data.CRF_Primary_Account = 0
            BUSS.SSM_Common_Data.CRF_Secondary_Account = ReqRev.AccountNbr

            'BUSS.SSM_Common_Data.CRF_Primary_Account = ReqRev.AccountNbr
            'BUSS.SSM_Common_Data.CRF_Secondary_Account = 0

            BUSS.SSM_Common_Data.CRF_Service_Indicator = SERVICE_CPN_PDR
            BUSS.SSM_Common_Data.CRF_Issuer_Institution_ID = FI_CPN_AUT
            BUSS.SSM_Common_Data.CRF_Issuer_Institution_Number = FI_CPN_AUT
            BUSS.SSM_Common_Data.CRF_Branch_ID = ReqRev.SourceBranchNbr
            'BUSS.SSM_Common_Data.CRF_Reversal_Indicator = CInt(ReqRev.ReversalIndicator)
            BUSS.SSM_Common_Data.CRF_Reversal_Indicator = REVERSAL_MODE

            BUSS.SSM_Common_Data.CRF_Names = ReqRev.SEGi_TargetName.Trim
            BUSS.SSM_Common_Data.CRF_Operator_ID = ReqRev.SourceTrmlNbr
            BUSS.SSM_Common_Data.CRF_Reference = ReqRev.SEGi_Reference
            BUSS.SSM_Common_Data.CRF_PhoneNumber = ReqRev.TargetPhone
            '
            '*************************  User Data 
            '
            Build_User_Token(USER_DATA, ACCT_TYPE_ID, ReqRev.SourceApplType)
            Build_User_Token(USER_DATA, ACCT_TYPE_TARGET_ID, ReqRev.CardApplCode)
            Build_User_Token(USER_DATA, NAMES_ID, ReqRev.SourceName)

            ' 2021-01-06 Segmento de Reverso incompleto desde Banred
            If Not IsNumeric(ReqRev.SEGi_SourceID) Then
                ReqRev.SEGi_SourceID = "0000000000000"
            End If
            ' 2021-01-06 Segmento de Reverso incompleto desde Banred
            Dim TempId As String
            TempId = Convert.ToInt64(ReqRev.SEGi_SourceID)
            If TempId.Length < 10 Then
                TempId = TempId.PadLeft(10, "0")
            ElseIf TempId.Length > 10 Then
                TempId = TempId.PadLeft(13, "0")
            End If
            Build_User_Token(USER_DATA, DOCUMENT_ID, TempId)

            ' 2021-01-06 Segmento de Reverso incompleto desde Banred
            If Not IsNumeric(ReqRev.SEGi_TargetID) Then
                ReqRev.SEGi_TargetID = "0000000000000"
            End If
            ' 2021-01-06 Segmento de Reverso incompleto desde Banred

            TempId = Convert.ToInt64(ReqRev.SEGi_TargetID)
            If TempId.Length < 10 Then
                TempId = TempId.PadLeft(10, "0")
            ElseIf TempId.Length > 10 Then
                TempId = TempId.PadLeft(13, "0")
            End If
            Build_User_Token(USER_DATA, TARGET_DOCUMENT_ID, TempId)

            'Build_User_Token(USER_DATA, DOCUMENT_ID, ReqRev.SEGi_SourceID)
            Build_User_Token(USER_DATA, ORG_DOC_ID, ReqRev.SEGi_TargetID)
            Build_User_Token(USER_DATA, SEQ_MESSAGE_ID, ReqRev.MessageSeqNbr)
            Dim OrgRevCode As Int16 = ReqRev.TransactionCode
            Build_User_Token(USER_DATA, ORIG_REV_CODE, OrgRevCode.ToString("0000"))

            BUSS.SSM_Common_Data.CRF_Token_Data = USER_DATA
            BUSS.SSM_Instance_Times += "T4_" & GetDateTime() & Concatenator

            '*************************  Putting message
            If Put_Message_To_Router(BUSS, g_RouterRequestQueue) <> SUCCESSFUL Then
                ErrorCode = PROCESS_ERROR
                Exit Try
            End If
            '*************************  Putting message
            ErrorCode = SUCCESSFUL
        Catch ex As Exception
            Show_Message_Console(MyName & " Error procesing decode & fill reversal request :" & ex.Message, COLOR_BLACK, COLOR_RED, 1, TRACE_LOW, 1)
            BUSS = Nothing
            ErrorCode = PROCESS_ERROR
        End Try

        Dim MSG As String = "Msg:" & ReqRev.MessageType
        MSG += " trx:" & BUSS.SSM_Common_Data.CRF_Transaction_Code
        MSG += " adq:" & BUSS.SSM_Common_Data.CRF_Adquirer_Institution_Number
        MSG += " aut:" & BUSS.SSM_Common_Data.CRF_Issuer_Institution_Number
        MSG += " seq:" & BUSS.SSM_Common_Data.CRF_Adquirer_Sequence
        MSG += " amt:" & BUSS.SSM_Common_Data.CRF_Transaction_Amount
        Show_Message_Console(MSG, COLOR_BLACK, COLOR_DARK_GRAY, 0, TRACE_LOW, 0)

        BUSS = Nothing
        MESSAGE = Nothing
        ErrorMessage = Nothing
        ErrorCode = 0
    End Sub

    Public Sub Process_Reply_Inquiry_NotEnabled(ByVal RequestBuffer As String, ByVal ErrorCode As Int32)
        Dim ReqInq As FixedStrucure.RequestInquiry
        Dim RepInq As FixedStrucure.ReplyInquiry
        Dim ReplyMessage As String = String.Empty
        '*******************************************************************************************************************************
        'Console.WriteLine("MUEVO A STRUCTURE")
        Try
            Dim pBuf As IntPtr = Marshal.StringToBSTR(RequestBuffer)
            ReqInq = CType(Marshal.PtrToStructure(pBuf, GetType(FixedStrucure.RequestInquiry)), FixedStrucure.RequestInquiry)
        Catch ex As Exception
            Show_Message_Console(MyName & " Can't decodify the received message :" & ex.Message, COLOR_BLACK, COLOR_RED, 1, TRACE_LOW, 1)
            Exit Sub
        End Try
        '*******************************************************************************************************************************
        'Console.WriteLine("MUEVO OK")
        Try
            'Console.WriteLine("LLENO STRUCT REPLY")
            Dim pBuf As IntPtr
            RepInq = CType(Marshal.PtrToStructure(pBuf, GetType(FixedStrucure.ReplyInquiry)), FixedStrucure.ReplyInquiry)
            RepInq.MessageType = "TC"
            RepInq.SourceFiNbr = ReqInq.SourceFiNbr
            RepInq.SourceTrmlNbr = ReqInq.SourceTrmlNbr
            RepInq.SourceSeqNbr = ReqInq.SourceSeqNbr
            RepInq.MessageSeqNbr = ReqInq.MessageSeqNbr
            RepInq.TransactionCode = ReqInq.TransactionCode
            RepInq.ResultCode = ErrorCode.ToString("0000")
            RepInq.AcctInfoFlag = "#"
            RepInq.HostDataInfoFlag = Chr(34)
            RepInq.AccountNumber = "000000000000000000"
            RepInq.AvailableBalance = "00000000000000000"
            RepInq.SignAvailable = "+"
            RepInq.CurrentBalance = "00000000000000000"
            RepInq.SignCurrent = "+"
            RepInq.ApplCode = ReqInq.CardApplCode
            RepInq.TerminalType = ReqInq.TerminalType
            RepInq.SourceName = ReqInq.SourceName
            RepInq.SEGo_AcctType = "05"
            RepInq.SEGo_AcctNumber = "000000000000000000"
            RepInq.SEGo_AcctName = Space(40)
            RepInq.SEGo_MinPayment = "00000000"
            RepInq.SEGo_TotPayment = "00000000"
            RepInq.SEGo_LimitDate = "00000000"
            RepInq.SEGo_TargetPhone = ReqInq.TargetPhone
            RepInq.SEGo_TargetID = ReqInq.SEGi_TargetID
            ReplyMessage = StructToString(RepInq)
            'Console.WriteLine("REPLY=" & ReplyMessage)
        Catch ex As Exception
            Show_Message_Console(MyName & " Can't build the reply message :" & ex.Message, COLOR_BLACK, COLOR_RED, 1, TRACE_LOW, 1)
            Exit Sub
        End Try
        '******************************************************************************
        '******************************************************************************
        'Console.WriteLine("INICIO ENVIO")
        Send_TCPIP_Message(ReplyMessage)
        SaveLogMain(ReplyMessage)
        'Console.WriteLine("FINAL ENVIO")
        '******************************************************************************
        '******************************************************************************
    End Sub


    Public Sub Process_Reply_Transfer_NotEnabled(ByVal RequestBuffer As String, ByVal ErrorCode As Int32)
        Dim ReqTran As FixedStrucure.RequestTransfer
        Dim RepTran As FixedStrucure.ReplyTransfer
        Dim ReplyMessage As String = String.Empty
        '*******************************************************************************************************************************
        'Console.WriteLine("MUEVO A STRUCTURE")
        Try
            Dim pBuf As IntPtr = Marshal.StringToBSTR(RequestBuffer)
            ReqTran = CType(Marshal.PtrToStructure(pBuf, GetType(FixedStrucure.RequestTransfer)), FixedStrucure.RequestTransfer)
        Catch ex As Exception
            Show_Message_Console(MyName & " Can't decodify the received message :" & ex.Message, COLOR_BLACK, COLOR_RED, 1, TRACE_LOW, 1)
            Exit Sub
        End Try
        '*******************************************************************************************************************************
        'Console.WriteLine("MUEVO OK")
        Try
            'Console.WriteLine("LLENO STRUCT REPLY")
            Dim pBuf As IntPtr
            RepTran = CType(Marshal.PtrToStructure(pBuf, GetType(FixedStrucure.ReplyTransfer)), FixedStrucure.ReplyTransfer)
            RepTran.MessageType = "TC"
            RepTran.SourceFiNbr = ReqTran.SourceFiNbr
            RepTran.SourceTrmlNbr = ReqTran.SourceTrmlNbr
            RepTran.SourceSeqNbr = ReqTran.SourceSeqNbr
            RepTran.MessageSeqNbr = ReqTran.MessageSeqNbr
            RepTran.TransactionCode = ReqTran.TransactionCode
            RepTran.ResultCode = ErrorCode.ToString("0000")
            RepTran.AcctInfoFlag = "#"
            RepTran.HostDataInfoFlag = Chr(34)
            RepTran.AccountNumber1 = "000000000000000000"
            RepTran.AvailableBalance1 = "00000000000000000"
            RepTran.SignAvailable1 = "+"
            RepTran.CurrentBalance1 = "00000000000000000"
            RepTran.SignCurrent1 = "+"
            RepTran.ApplCode = ReqTran.CardApplCode
            RepTran.TerminalType = ReqTran.TerminalType
            RepTran.SourceName = ReqTran.SourceName
            RepTran.AccountNumber2 = "000000000000000000"
            RepTran.AvailableBalance2 = "00000000000000000"
            RepTran.SignAvailable2 = "+"
            RepTran.CurrentBalance2 = "00000000000000000"
            RepTran.SignCurrent2 = "+"
            RepTran.ApplCode2 = ReqTran.OtherAppl
            RepTran.SEGo_AcctType = ReqTran.OtherAppl
            RepTran.SEGo_AcctNumber = "000000000000000000"
            RepTran.SEGo_AcctName = Space(40)
            RepTran.SEGo_MinPayment = "00000000"
            RepTran.SEGo_TotPayment = "00000000"
            RepTran.SEGo_LimitDate = "00000000"
            RepTran.SEGo_TargetPhone = ReqTran.TargetPhone
            RepTran.SEGo_TargetID = ReqTran.SEGi_TargetID
            ReplyMessage = StructToString(RepTran)
            'Console.WriteLine("REPLY=" & ReplyMessage)
        Catch ex As Exception
            Show_Message_Console(MyName & " Can't build the reply message :" & ex.Message, COLOR_BLACK, COLOR_RED, 1, TRACE_LOW, 1)
            Exit Sub
        End Try
        '******************************************************************************
        '******************************************************************************
        'Console.WriteLine("INICIO ENVIO")
        Send_TCPIP_Message(ReplyMessage)
        SaveLogMain(ReplyMessage)
        'Console.WriteLine("FINAL ENVIO")
        '******************************************************************************
        '******************************************************************************
    End Sub


    Public Sub Process_Reply_Reversal_NotEnabled(ByVal RequestBuffer As String, ByVal ErrorCode As Int32)
        Dim ReqRev As FixedStrucure.RequestReversal_C
        Dim RepRev As FixedStrucure.ReplyReversal
        Dim ReplyMessage As String = String.Empty
        '*******************************************************************************************************************************
        Try
            Dim pBuf As IntPtr = Marshal.StringToBSTR(RequestBuffer)
            ReqRev = CType(Marshal.PtrToStructure(pBuf, GetType(FixedStrucure.RequestReversal_C)), FixedStrucure.RequestReversal_C)
        Catch ex As Exception
            Show_Message_Console(MyName & " Can't decodify the received message :" & ex.Message, COLOR_BLACK, COLOR_RED, 1, TRACE_LOW, 1)
            Exit Sub
        End Try
        '*******************************************************************************************************************************
        Try
            Dim pBuf As IntPtr
            RepRev = CType(Marshal.PtrToStructure(pBuf, GetType(FixedStrucure.ReplyReversal)), FixedStrucure.ReplyReversal)
            RepRev.MessageType = "XC"
            RepRev.SourceFiNbr = ReqRev.SourceFiNbr
            RepRev.SourceTrmlNbr = ReqRev.SourceTrmlNbr
            RepRev.SourceSeqNbr = ReqRev.SourceSeqNbr
            RepRev.MessageSeqNbr = ReqRev.MessageSeqNbr
            RepRev.TransactionCode = ReqRev.TransactionCode
            RepRev.TranCodeSign = ReqRev.TranCodeSign
            RepRev.ResultCode = ErrorCode.ToString("0000")
            RepRev.AcctInfoFlag = "#"
            RepRev.HostDataInfoFlag = Chr(34)
            RepRev.AccountNumber1 = ReqRev.AccountNbr
            RepRev.AvailableBalance1 = "00000000000000000"
            RepRev.SignAvailable1 = "+"
            RepRev.CurrentBalance1 = "00000000000000000"
            RepRev.SignCurrent1 = "+"
            RepRev.ApplCode = ReqRev.CardApplCode
            RepRev.TerminalType = ReqRev.TerminalType
            RepRev.SourceName = ReqRev.SourceName
            RepRev.AccountNumber2 = "000000000000000000"
            RepRev.AvailableBalance2 = "00000000000000000"
            RepRev.SignAvailable2 = "+"
            RepRev.CurrentBalance2 = "00000000000000000"
            RepRev.SignCurrent2 = "+"
            RepRev.ApplCode2 = ReqRev.OtherAppl
            RepRev.SEGo_AcctType = ReqRev.OtherAppl
            RepRev.SEGo_AcctNumber = "000000000000000000"
            RepRev.SEGo_AcctName = Space(40)
            RepRev.SEGo_MinPayment = "00000000"
            RepRev.SEGo_TotPayment = "00000000"
            RepRev.SEGo_LimitDate = "00000000"
            RepRev.SEGo_TargetPhone = ReqRev.TargetPhone
            RepRev.SEGo_TargetID = ReqRev.SEGi_TargetID
            ReplyMessage = StructToString(RepRev)
        Catch ex As Exception
            Show_Message_Console(MyName & " Can't build the reply message :" & ex.Message, COLOR_BLACK, COLOR_RED, 1, TRACE_LOW, 1)
            Exit Sub
        End Try
        '******************************************************************************
        '******************************************************************************
        'Console.WriteLine("INICIO ENVIO")
        Send_TCPIP_Message(ReplyMessage)
        SaveLogMain(ReplyMessage)
        'Console.WriteLine("FINAL ENVIO")
        '******************************************************************************
        '******************************************************************************
    End Sub

End Class
