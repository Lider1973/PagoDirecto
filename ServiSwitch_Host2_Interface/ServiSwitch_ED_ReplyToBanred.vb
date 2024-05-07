Imports System.Runtime.InteropServices
Imports System.Globalization
Imports System.IO

Public Class ServiSwitch_ED_ReplyToBanred

    Public Function Encode_Transfer_Inquiry_Reply(ByVal Struct_Queue_Message As SharedStructureMessage) As String
        Dim Token_Data As String = String.Empty
        Dim TKNid(20) As String
        Dim TKNdata(20) As String

        'Console.WriteLine("TOKEN DATA:" & Struct_Queue_Message.SSM_Common_Data.CRF_Token_Data)

        Try
            If Struct_Queue_Message.SSM_Common_Data.CRF_Token_Data.Length > 0 Then
                Dim x, y As Integer
                Token_Data = Struct_Queue_Message.SSM_Common_Data.CRF_Token_Data
                Do While Token_Data.Length > 0
                    TKNid(x) = Token_Data.Substring(0, 2)
                    Token_Data = Token_Data.Remove(0, 2)
                    y = Token_Data.Substring(0, 3)
                    Token_Data = Token_Data.Remove(0, 3)
                    TKNdata(x) = Token_Data.Substring(0, y)
                    Token_Data = Token_Data.Remove(0, y)
                    x += 1
                Loop
                Array.Resize(TKNid, x)
                Array.Resize(TKNdata, x)
            End If
        Catch ex As Exception
            Console.WriteLine("Error # 001:" & ex.Message)
        End Try

        Dim ReplyMessage As String = String.Empty
        Dim pBuf As IntPtr
        Dim RepInq As FixedStrucure.ReplyInquiry
        RepInq = CType(Marshal.PtrToStructure(pBuf, GetType(FixedStrucure.ReplyInquiry)), FixedStrucure.ReplyInquiry)
        RepInq.MessageType = "TC"
        RepInq.SourceFiNbr = FI_CPN_AUT.ToString("0000")

        RepInq.SourceTrmlNbr = Struct_Queue_Message.SSM_Common_Data.CRF_Terminal_ID
        RepInq.SourceSeqNbr = Struct_Queue_Message.SSM_Common_Data.CRF_Adquirer_Sequence.ToString("000000")

        'RepInq.MessageSeqNbr = Struct_Queue_Message.SSM_Common_Data.CRF_Switch_Sequence.ToString("0000")
        RepInq.MessageSeqNbr = Get_Token_Info(SEQ_MESSAGE_ID, TKNid, TKNdata)

        RepInq.TransactionCode = Struct_Queue_Message.SSM_Common_Data.CRF_Transaction_Code.ToString("0000") & "+"
        RepInq.ResultCode = CInt(Struct_Queue_Message.SSM_Common_Data.CRF_Response_Code).ToString("0000")
        RepInq.AcctInfoFlag = "#"
        RepInq.HostDataInfoFlag = Chr(34)

        'Console.WriteLine("PRIMARY ACCT:" & Struct_Queue_Message.SSM_Common_Data.CRF_Primary_Account)

        RepInq.AccountNumber = Struct_Queue_Message.SSM_Common_Data.CRF_Primary_Account.ToString("000000000000000000")
        RepInq.AvailableBalance = "00000000000000000"
        RepInq.SignAvailable = "+"
        RepInq.CurrentBalance = "00000000000000000"
        RepInq.SignCurrent = "+"
        'RepInq.ApplCode = Get_Token_Info(ACCT_TYPE_ID, TKNid, TKNdata).PadLeft(2, "0")
        RepInq.ApplCode = "05"

        Select Case CInt(Struct_Queue_Message.SSM_Common_Data.CRF_Channel_Id)
            Case 3
                RepInq.TerminalType = 1
            Case 4
                RepInq.TerminalType = 2
            Case Else
                RepInq.TerminalType = Struct_Queue_Message.SSM_Common_Data.CRF_Channel_Id
        End Select
        'RepInq.TerminalType = Struct_Queue_Message.SSM_Common_Data.CRF_Channel_Id

        Dim SNames As String = Get_Token_Info(NAMES_ID, TKNid, TKNdata)
        If SNames.Length >= 31 Then
            RepInq.SourceName = SNames.Substring(0, 31)
        Else
            RepInq.SourceName = SNames.PadRight(31, " ")
        End If
        'RepInq.SEGo_AcctType = Get_Token_Info(ACCT_TYPE_TARGET_ID, TKNid, TKNdata)
        RepInq.SEGo_AcctType = "05"
        RepInq.SEGo_AcctNumber = Struct_Queue_Message.SSM_Common_Data.CRF_Secondary_Account.ToString("000000000000000000")
        Dim TNames As String = Struct_Queue_Message.SSM_Common_Data.CRF_Names
        If TNames.Length >= 40 Then
            RepInq.SEGo_AcctName = TNames.Substring(0, 40)
        Else
            RepInq.SEGo_AcctName = TNames.PadRight(40, " ")
        End If

        'Console.WriteLine("VALOR PARA EL NOMBRE:" & Struct_Queue_Message.SSM_Common_Data.CRF_Names)
        'Console.WriteLine("VALOR PARA EL NOMBRE:" & RepInq.SEGo_AcctName)

        RepInq.SEGo_MinPayment = "00000000"
        RepInq.SEGo_TotPayment = "00000000"
        RepInq.SEGo_LimitDate = "00000000"

        If Struct_Queue_Message.SSM_Common_Data.CRF_PhoneNumber.Length = 0 Then
            RepInq.SEGo_TargetPhone = "0000000000"
        Else
            RepInq.SEGo_TargetPhone = Struct_Queue_Message.SSM_Common_Data.CRF_PhoneNumber
        End If
        'RepInq.SEGo_TargetPhone = Struct_Queue_Message.SSM_Common_Data.CRF_PhoneNumber
        RepInq.SEGo_TargetID = Get_Token_Info(TARGET_DOCUMENT_ID, TKNid, TKNdata).PadRight(13, " ")

        'Console.WriteLine("Informacion de Tokens:" & Struct_Queue_Message.SSM_Common_Data.CRF_Token_Data)
        'Console.WriteLine("Informacion de IDtarget:" & RepInq.SEGo_TargetID)

        ReplyMessage = StructToString(RepInq)
        Return ReplyMessage
    End Function

    Public Function Encode_Transfer_Reply(ByVal Struct_Queue_Message As SharedStructureMessage) As String
        Dim Token_Data As String = String.Empty
        Dim TKNid(20) As String
        Dim TKNdata(20) As String
        Try
            If Struct_Queue_Message.SSM_Common_Data.CRF_Token_Data.Length > 0 Then
                Dim x, y As Integer
                Token_Data = Struct_Queue_Message.SSM_Common_Data.CRF_Token_Data
                Do While Token_Data.Length > 0
                    TKNid(x) = Token_Data.Substring(0, 2)
                    Token_Data = Token_Data.Remove(0, 2)
                    y = Token_Data.Substring(0, 3)
                    Token_Data = Token_Data.Remove(0, 3)
                    TKNdata(x) = Token_Data.Substring(0, y)
                    Token_Data = Token_Data.Remove(0, y)
                    x += 1
                Loop
                Array.Resize(TKNid, x)
                Array.Resize(TKNdata, x)
            End If
        Catch ex As Exception
            Console.WriteLine("Error # 001:" & ex.Message)
        End Try
        Dim ReplyMessage As String = String.Empty
        Dim pBuf As IntPtr
        Dim RepTran As FixedStrucure.ReplyTransfer
        RepTran = CType(Marshal.PtrToStructure(pBuf, GetType(FixedStrucure.ReplyTransfer)), FixedStrucure.ReplyTransfer)
        RepTran.MessageType = "TC"
        RepTran.SourceFiNbr = FI_CPN_AUT.ToString("0000")
        RepTran.SourceTrmlNbr = Struct_Queue_Message.SSM_Common_Data.CRF_Terminal_ID
        RepTran.SourceSeqNbr = Struct_Queue_Message.SSM_Common_Data.CRF_Adquirer_Sequence.ToString("000000")
        'RepTran.MessageSeqNbr = Struct_Queue_Message.SSM_Common_Data.CRF_Switch_Sequence.ToString("0000")
        RepTran.MessageSeqNbr = Get_Token_Info(SEQ_MESSAGE_ID, TKNid, TKNdata)
        'Console.WriteLine("Struct_Queue_Message.SSM_Common_Data.CRF_Transaction_Code:" & Struct_Queue_Message.SSM_Common_Data.CRF_Transaction_Code)
        RepTran.TransactionCode = "0539+"
        'RepTran.TransactionCode = Struct_Queue_Message.SSM_Common_Data.CRF_Transaction_Code.ToString("0000") & "+"
        RepTran.ResultCode = CInt(Struct_Queue_Message.SSM_Common_Data.CRF_Response_Code).ToString("0000")
        RepTran.AcctInfoFlag = "#"
        RepTran.HostDataInfoFlag = Chr(34)
        RepTran.AccountNumber1 = Struct_Queue_Message.SSM_Common_Data.CRF_Secondary_Account.ToString("000000000000000000")
        RepTran.AvailableBalance1 = "00000000000000000"
        RepTran.SignAvailable1 = "+"
        RepTran.CurrentBalance1 = "00000000000000000"
        RepTran.SignCurrent1 = "+"
        RepTran.ApplCode = Get_Token_Info(ACCT_TYPE_ID, TKNid, TKNdata).PadLeft(2, "0")
        RepTran.SourceName = Get_Token_Info(NAMES_ID, TKNid, TKNdata)
        RepTran.AccountNumber2 = Struct_Queue_Message.SSM_Common_Data.CRF_Primary_Account.ToString("000000000000000000")
        RepTran.AvailableBalance2 = "00000000000000000"
        RepTran.SignAvailable2 = "+"
        RepTran.CurrentBalance2 = "00000000000000000"
        RepTran.SignCurrent2 = "+"
        RepTran.ApplCode2 = Get_Token_Info(ACCT_TYPE_TARGET_ID, TKNid, TKNdata).PadLeft(2, "0")

        RepTran.SEGo_AcctType = "05"
        RepTran.SEGo_AcctNumber = Struct_Queue_Message.SSM_Common_Data.CRF_Secondary_Account.ToString("000000000000000000")

        Select Case CInt(Struct_Queue_Message.SSM_Common_Data.CRF_Channel_Id)
            Case 3
                RepTran.TerminalType = 1
            Case 4
                RepTran.TerminalType = 2
            Case Else
                RepTran.TerminalType = Struct_Queue_Message.SSM_Common_Data.CRF_Channel_Id
        End Select

        Dim TNames As String = Struct_Queue_Message.SSM_Common_Data.CRF_Names
        If TNames.Length >= 40 Then
            RepTran.SEGo_AcctName = TNames.Substring(0, 40)
        Else
            RepTran.SEGo_AcctName = TNames.PadRight(40, " ")
        End If
        'Console.WriteLine("VALOR PARA EL NOMBRE:" & Struct_Queue_Message.SSM_Common_Data.CRF_Names)
        'Console.WriteLine("VALOR PARA EL NOMBRE:" & RepTran.SEGo_AcctName)
        RepTran.SEGo_MinPayment = "00000000"
        RepTran.SEGo_TotPayment = "00000000"
        RepTran.SEGo_LimitDate = "00000000"

        If Struct_Queue_Message.SSM_Common_Data.CRF_PhoneNumber.Length = 0 Then
            RepTran.SEGo_TargetPhone = "0000000000"
        Else
            RepTran.SEGo_TargetPhone = Struct_Queue_Message.SSM_Common_Data.CRF_PhoneNumber
        End If
        'RepTran.SEGo_TargetPhone = Struct_Queue_Message.SSM_Common_Data.CRF_PhoneNumber
        RepTran.SEGo_TargetID = Get_Token_Info(TARGET_DOCUMENT_ID, TKNid, TKNdata).PadRight(13, " ")

        ReplyMessage = StructToString(RepTran)
        Return ReplyMessage
    End Function


    Public Function Encode_Reverse_Reply(ByVal Struct_Queue_Message As SharedStructureMessage) As String
        Dim Token_Data As String = String.Empty
        Dim TKNid(20) As String
        Dim TKNdata(20) As String
        Try
            If Struct_Queue_Message.SSM_Common_Data.CRF_Token_Data.Length > 0 Then
                Dim x, y As Integer
                Token_Data = Struct_Queue_Message.SSM_Common_Data.CRF_Token_Data
                Do While Token_Data.Length > 0
                    TKNid(x) = Token_Data.Substring(0, 2)
                    Token_Data = Token_Data.Remove(0, 2)
                    y = Token_Data.Substring(0, 3)
                    Token_Data = Token_Data.Remove(0, 3)
                    TKNdata(x) = Token_Data.Substring(0, y)
                    Token_Data = Token_Data.Remove(0, y)
                    x += 1
                Loop
                Array.Resize(TKNid, x)
                Array.Resize(TKNdata, x)
            End If
        Catch ex As Exception
            Console.WriteLine("Error # 001:" & ex.Message)
        End Try

        Dim ReplyMessage As String = String.Empty
        Dim pBuf As IntPtr
        Dim RepRever As FixedStrucure.ReplyReversal
        RepRever = CType(Marshal.PtrToStructure(pBuf, GetType(FixedStrucure.ReplyReversal)), FixedStrucure.ReplyReversal)
        RepRever.MessageType = "XC"
        RepRever.SourceFiNbr = FI_CPN_AUT.ToString("0000")
        RepRever.SourceTrmlNbr = Struct_Queue_Message.SSM_Common_Data.CRF_Terminal_ID
        RepRever.SourceSeqNbr = Struct_Queue_Message.SSM_Common_Data.CRF_Adquirer_Sequence.ToString("000000")
        RepRever.MessageSeqNbr = Get_Token_Info(SEQ_MESSAGE_ID, TKNid, TKNdata)
        'RepRever.MessageSeqNbr = Struct_Queue_Message.SSM_Common_Data.CRF_Switch_Sequence.ToString("0000")

        Dim CRF_Transaction_Code As String = Get_Token_Info(ORIG_REV_CODE, TKNid, TKNdata)
        'RepRever.TransactionCode = Struct_Queue_Message.SSM_Common_Data.CRF_Transaction_Code.ToString("0000") & "-"
        'RepRever.TransactionCode = CRF_Transaction_Code & "-"
        'Console.WriteLine("Struct_Queue_Message.SSM_Common_Data.CRF_Transaction_Code:" & Struct_Queue_Message.SSM_Common_Data.CRF_Transaction_Code)
        RepRever.TransactionCode = "0539-"
        RepRever.ResultCode = CInt(Struct_Queue_Message.SSM_Common_Data.CRF_Response_Code).ToString("0000")
        RepRever.AcctInfoFlag = "#"
        RepRever.HostDataInfoFlag = Chr(34)
        RepRever.AccountNumber1 = Struct_Queue_Message.SSM_Common_Data.CRF_Secondary_Account.ToString("000000000000000000")
        RepRever.AvailableBalance1 = "00000000000000000"
        RepRever.SignAvailable1 = "+"
        RepRever.CurrentBalance1 = "00000000000000000"
        RepRever.SignCurrent1 = "+"
        RepRever.ApplCode = Get_Token_Info(ACCT_TYPE_ID, TKNid, TKNdata).PadLeft(2, "0")
        RepRever.SourceName = Get_Token_Info(NAMES_ID, TKNid, TKNdata)
        RepRever.AccountNumber2 = Struct_Queue_Message.SSM_Common_Data.CRF_Primary_Account.ToString("000000000000000000")
        RepRever.AvailableBalance2 = "00000000000000000"
        RepRever.SignAvailable2 = "+"
        RepRever.CurrentBalance2 = "00000000000000000"
        RepRever.SignCurrent2 = "+"
        RepRever.ApplCode2 = Get_Token_Info(ACCT_TYPE_TARGET_ID, TKNid, TKNdata).PadLeft(2, "0")
        RepRever.SEGo_AcctType = "05"
        RepRever.SEGo_AcctNumber = Struct_Queue_Message.SSM_Common_Data.CRF_Secondary_Account.ToString("000000000000000000")
        Select Case CInt(Struct_Queue_Message.SSM_Common_Data.CRF_Channel_Id)
            Case 3
                RepRever.TerminalType = 1
            Case 4
                RepRever.TerminalType = 2
            Case Else
                RepRever.TerminalType = Struct_Queue_Message.SSM_Common_Data.CRF_Channel_Id
        End Select
        Dim TNames As String = Struct_Queue_Message.SSM_Common_Data.CRF_Names
        If TNames.Length >= 40 Then
            RepRever.SEGo_AcctName = TNames.Substring(0, 40)
        Else
            RepRever.SEGo_AcctName = TNames.PadRight(40, " ")
        End If
        'Console.WriteLine("VALOR PARA EL NOMBRE:" & Struct_Queue_Message.SSM_Common_Data.CRF_Names)
        'Console.WriteLine("VALOR PARA EL NOMBRE:" & RepRever.SEGo_AcctName)
        RepRever.SEGo_MinPayment = "00000000"
        RepRever.SEGo_TotPayment = "00000000"
        RepRever.SEGo_LimitDate = "00000000"
        If Struct_Queue_Message.SSM_Common_Data.CRF_PhoneNumber.Length = 0 Then
            RepRever.SEGo_TargetPhone = "0000000000"
        Else
            RepRever.SEGo_TargetPhone = Struct_Queue_Message.SSM_Common_Data.CRF_PhoneNumber
        End If
        'RepRever.SEGo_TargetPhone = Struct_Queue_Message.SSM_Common_Data.CRF_PhoneNumber
        'RepRever.SEGo_TargetID = Get_Token_Info(TARGET_DOCUMENT_ID, TKNid, TKNdata)
        RepRever.SEGo_TargetID = Get_Token_Info(ORG_DOC_ID, TKNid, TKNdata)

        ReplyMessage = StructToString(RepRever)
        Return ReplyMessage
    End Function

End Class
