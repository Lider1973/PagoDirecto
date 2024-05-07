Imports System.Runtime.InteropServices

Public Class ServiSwitch_ED_RequestToBanred

    '--------------------------------------
    'REQUERIMIENTO DE CONSULTA HACIA BANRED
    '--------------------------------------
    Public Function Encode_Transfer_Inquiry(ByVal Struct_Queue_Message As SharedStructureMessage) As String
        Dim RequestMessage As String = String.Empty
        Dim Token_Data As String = String.Empty
        Dim TKNid(20) As String
        Dim TKNdata(20) As String

        Struct_Queue_Message.SSM_Common_Data.CRF_Message_Code = 200
        'Console.WriteLine("Inicio Paso Tokens")
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
                    'Console.WriteLine("TOKEN id:" & TKNid(x) & " " & " TOKEN DATA:" & TKNdata(x))
                    x += 1
                Loop
                Array.Resize(TKNid, x)
                Array.Resize(TKNdata, x)
            End If
        Catch ex As Exception
            Return "ERROR"
        End Try
        'Console.WriteLine("Final Paso Tokens")
        Try
            Dim pBuf As IntPtr
            Dim ReqInq As FixedStrucure.RequestInquiry
            ReqInq = CType(Marshal.PtrToStructure(pBuf, GetType(FixedStrucure.RequestInquiry)), FixedStrucure.RequestInquiry)

            ReqInq.MessageType = "TR"
            ReqInq.SourceFiNbr = FI_CPN_ADQ.ToString("0000")
            ReqInq.SourceTrmlNbr = Struct_Queue_Message.SSM_Common_Data.CRF_Terminal_ID.PadLeft(5, "0")
            ReqInq.SourceSeqNbr = Struct_Queue_Message.SSM_Common_Data.CRF_Adquirer_Sequence.ToString("000000")
            ReqInq.MessageSeqNbr = Struct_Queue_Message.SSM_Common_Data.CRF_Switch_Sequence.ToString("0000")
            ReqInq.TransactionCode = Struct_Queue_Message.SSM_Common_Data.CRF_Transaction_Code.ToString("0000") & "+"

            ReqInq.SourceDate = Struct_Queue_Message.SSM_Common_Data.CRF_Adquirer_Date_Time.Year.ToString.Substring(2, 2) &
                                Struct_Queue_Message.SSM_Common_Data.CRF_Adquirer_Date_Time.Month.ToString("00") &
                                Struct_Queue_Message.SSM_Common_Data.CRF_Adquirer_Date_Time.Day.ToString("00")
            ReqInq.SourceTime = Struct_Queue_Message.SSM_Common_Data.CRF_Adquirer_Date_Time.Hour.ToString("00") &
                                Struct_Queue_Message.SSM_Common_Data.CRF_Adquirer_Date_Time.Minute.ToString("00") &
                                Struct_Queue_Message.SSM_Common_Data.CRF_Adquirer_Date_Time.Second.ToString("00")

            ReqInq.SourceAbaNbr = ABA_CPN_ADQ.ToString("0000000000")
            ReqInq.SourceBranchNbr = Struct_Queue_Message.SSM_Common_Data.CRF_Branch_ID.PadLeft(4, "0")
            ReqInq.SourceBussDate = Struct_Queue_Message.SSM_Common_Data.CRF_Adquirer_Date_Time.Year.ToString.Substring(2, 2) &
                                Struct_Queue_Message.SSM_Common_Data.CRF_Adquirer_Date_Time.Month.ToString("00") &
                                Struct_Queue_Message.SSM_Common_Data.CRF_Adquirer_Date_Time.Day.ToString("00")

            Select Case CInt(Struct_Queue_Message.SSM_Common_Data.CRF_Channel_Id)
                Case 3
                    ReqInq.TerminalType = 1
                Case 4, 7
                    ReqInq.TerminalType = 2
                Case Else
                    ReqInq.TerminalType = Struct_Queue_Message.SSM_Common_Data.CRF_Channel_Id
            End Select
            'ReqInq.TerminalType = Struct_Queue_Message.SSM_Common_Data.CRF_Channel_Id.ToString

            Dim SourceName As String = Get_Token_Info(NAMES_ID, TKNid, TKNdata)
            If SourceName.Length >= 31 Then
                ReqInq.SourceName = SourceName.Substring(0, 31).ToUpper
            Else
                ReqInq.SourceName = SourceName.PadRight(31, " ").ToUpper
            End If

            ReqInq.ForcedIndicator = "0"
            ReqInq.ReversalIndicator = "0"
            ReqInq.AccountNbr = Struct_Queue_Message.SSM_Common_Data.CRF_Secondary_Account.ToString("000000000000000000")
            ReqInq.TransactionAmount = "00000000000000000"
            ReqInq.TransactionSign = "+"
            ReqInq.AuthFiNbr = Get_Token_Info(AUTHPD_ID, TKNid, TKNdata).PadLeft(4, "0")
            ReqInq.HostBussDate = ReqInq.SourceDate


            ReqInq.StandinAuthType = "0"
            ReqInq.StandinAuthMethod = "4"
            ReqInq.StandinResultCode = "0000"
            ReqInq.PinVerifyFlag = "0"
            ReqInq.TrackIIValidFlag = "1"

            If Struct_Queue_Message.SSM_Common_Data.CRF_PhoneNumber.Length = 10 Then
                ReqInq.TargetPhone = Struct_Queue_Message.SSM_Common_Data.CRF_PhoneNumber
            ElseIf Struct_Queue_Message.SSM_Common_Data.CRF_PhoneNumber.Length < 10 Then
                ReqInq.TargetPhone = Struct_Queue_Message.SSM_Common_Data.CRF_PhoneNumber.PadLeft(10, "0")
            ElseIf Struct_Queue_Message.SSM_Common_Data.CRF_PhoneNumber.Length > 10 Then
                ReqInq.TargetPhone = Struct_Queue_Message.SSM_Common_Data.CRF_PhoneNumber.Substring(0, 10)
            End If

            ReqInq.SourceApplType = Get_Token_Info(ACCT_TYPE_ID, TKNid, TKNdata)
            ReqInq.FlagRevPD = "PD"
            ReqInq.Filler = "   "

            Dim Aba As Int32
            Dim Name As String = String.Empty
            Dim AutPDid As Int16 = Get_Token_Info(AUTHPD_ID, TKNid, TKNdata)

            'Console.WriteLine("Codigo de banco Autorizador:" & AutPDid)

            Dim Tappl As Integer = Get_Token_Info(ACCT_TYPE_TARGET_ID, TKNid, TKNdata).PadLeft(2, "0")
            If Tappl = 2 Then
                Select Case Struct_Queue_Message.SSM_Common_Data.CRF_Secondary_Account.ToString.Length
                    Case 14
                        ReqInq.TrackIIData = ";" & Struct_Queue_Message.SSM_Common_Data.CRF_Secondary_Account & "=00000000000000000000000?"
                    Case 15
                        ReqInq.TrackIIData = ";" & Struct_Queue_Message.SSM_Common_Data.CRF_Secondary_Account & "=0000000000000000000000?"
                    Case 16
                        ReqInq.TrackIIData = ";" & Struct_Queue_Message.SSM_Common_Data.CRF_Secondary_Account & "=000000000000000000000?"
                End Select
            Else
                If GetInfoInstitution(0, AutPDid, Aba, Name) = SUCCESSFUL Then
                    ReqInq.TrackIIData = ";" & Format(Aba, "000000") & "0000000000=000000000000000000000?"
                Else
                    ReqInq.TrackIIData = ";0000000000000000=000000000000000000000?"
                End If
            End If

            'Console.WriteLine("Track Info:" & ReqInq.TrackIIData)

            ReqInq.CardApplCode = Tappl.ToString.PadLeft(2, "0")

            If Struct_Queue_Message.SSM_Common_Data.CRF_Names.Length >= 35 Then
                ReqInq.SEGi_TargetName = Struct_Queue_Message.SSM_Common_Data.CRF_Names.Substring(0, 35).ToUpper
            Else
                ReqInq.SEGi_TargetName = Struct_Queue_Message.SSM_Common_Data.CRF_Names.PadRight(35, " ").ToUpper
            End If
            Dim TempInt64 As Int64
            ReqInq.SEGi_Observation = "PAGO DIREC"
            TempInt64 = Get_Token_Info(DOCUMENT_ID, TKNid, TKNdata)
            ReqInq.SEGi_SourceID = TempInt64.ToString("0000000000").PadRight(13, " ")
            ReqInq.SEGi_City = "PICH/QUITO"

            If Struct_Queue_Message.SSM_Common_Data.CRF_Reference.Length >= 20 Then
                ReqInq.SEGi_Reference = Struct_Queue_Message.SSM_Common_Data.CRF_Reference.Substring(0, 20)
            Else
                ReqInq.SEGi_Reference = Struct_Queue_Message.SSM_Common_Data.CRF_Reference.PadRight(20, " ")
            End If

            'TempInt64 = Get_Token_Info(TARGET_DOCUMENT_ID, TKNid, TKNdata)
            'ReqInq.SEGi_TargetID = TempInt64.ToString("0000000000000")
            ReqInq.SEGi_TargetID = Get_Token_Info(TARGET_DOCUMENT_ID, TKNid, TKNdata).PadRight(13, " ")
            ReqInq.SEGi_Filler = "      "
            RequestMessage = StructToString(ReqInq)
        Catch ex As Exception
            Return "ERROR"
        End Try

        Show_Message_Console("ORDENANTE:Policia Nacional", COLOR_WHITE, COLOR_BLUE, 0, TRACE_LOW, 0)

        Return RequestMessage
    End Function

    '-------------------------------------------
    'REQUERIMIENTO DE TRANSFERENCIA HACIA BANRED
    '-------------------------------------------
    Public Function Encode_Transfer_Request(ByVal Struct_Queue_Message As SharedStructureMessage) As String
        Dim RequestMessage As String = String.Empty
        Dim Token_Data As String = String.Empty
        Dim TKNid(20) As String
        Dim TKNdata(20) As String
        Struct_Queue_Message.SSM_Common_Data.CRF_Message_Code = 200

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
            Return "ERROR"
        End Try


        Try
            Dim pBuf As IntPtr
            Dim ReqTransfer As FixedStrucure.RequestTransfer
            ReqTransfer = CType(Marshal.PtrToStructure(pBuf, GetType(FixedStrucure.RequestTransfer)), FixedStrucure.RequestTransfer)

            ReqTransfer.MessageType = "TR"
            ReqTransfer.SourceFiNbr = FI_CPN_ADQ.ToString("0000")
            ReqTransfer.SourceTrmlNbr = Struct_Queue_Message.SSM_Common_Data.CRF_Terminal_ID.PadLeft(5, "0")
            ReqTransfer.SourceSeqNbr = Struct_Queue_Message.SSM_Common_Data.CRF_Adquirer_Sequence.ToString("000000")
            ReqTransfer.MessageSeqNbr = Struct_Queue_Message.SSM_Common_Data.CRF_Switch_Sequence.ToString("0000")
            ReqTransfer.TransactionCode = Struct_Queue_Message.SSM_Common_Data.CRF_Transaction_Code.ToString("0000") & "+"

            ReqTransfer.SourceDate = Struct_Queue_Message.SSM_Common_Data.CRF_Adquirer_Date_Time.Year.ToString.Substring(2, 2) &
                                Struct_Queue_Message.SSM_Common_Data.CRF_Adquirer_Date_Time.Month.ToString("00") &
                                Struct_Queue_Message.SSM_Common_Data.CRF_Adquirer_Date_Time.Day.ToString("00")
            ReqTransfer.SourceTime = Struct_Queue_Message.SSM_Common_Data.CRF_Adquirer_Date_Time.Hour.ToString("00") &
                                Struct_Queue_Message.SSM_Common_Data.CRF_Adquirer_Date_Time.Minute.ToString("00") &
                                Struct_Queue_Message.SSM_Common_Data.CRF_Adquirer_Date_Time.Second.ToString("00")

            ReqTransfer.SourceAbaNbr = ABA_CPN_ADQ.ToString("0000000000")
            ReqTransfer.SourceBranchNbr = Struct_Queue_Message.SSM_Common_Data.CRF_Branch_ID.PadLeft(4, "0")
            ReqTransfer.SourceBussDate = Struct_Queue_Message.SSM_Common_Data.CRF_Adquirer_Date_Time.Year.ToString.Substring(2, 2) &
                                Struct_Queue_Message.SSM_Common_Data.CRF_Adquirer_Date_Time.Month.ToString("00") &
                                Struct_Queue_Message.SSM_Common_Data.CRF_Adquirer_Date_Time.Day.ToString("00")

            Select Case CInt(Struct_Queue_Message.SSM_Common_Data.CRF_Channel_Id)
                Case 3
                    ReqTransfer.TerminalType = 1
                Case 4, 7
                    ReqTransfer.TerminalType = 2
                Case Else
                    ReqTransfer.TerminalType = Struct_Queue_Message.SSM_Common_Data.CRF_Channel_Id
            End Select
            'ReqTransfer.TerminalType = Struct_Queue_Message.SSM_Common_Data.CRF_Channel_Id.ToString

            Dim SourceName As String = Get_Token_Info(NAMES_ID, TKNid, TKNdata)
            If SourceName.Length >= 31 Then
                ReqTransfer.SourceName = SourceName.Substring(0, 31).ToUpper
            Else
                ReqTransfer.SourceName = SourceName.PadRight(31, " ").ToUpper
            End If

            ReqTransfer.ForcedIndicator = "0"
            ReqTransfer.ReversalIndicator = "0"
            ReqTransfer.AccountNbr = Struct_Queue_Message.SSM_Common_Data.CRF_Secondary_Account.ToString("000000000000000000")
            ReqTransfer.TransactionAmount = (Struct_Queue_Message.SSM_Common_Data.CRF_Transaction_Amount * 100).ToString("00000000000000000")
            ReqTransfer.TransactionSign = "+"
            ReqTransfer.AuthFiNbr = Get_Token_Info(AUTHPD_ID, TKNid, TKNdata).PadLeft(4, "0")
            ReqTransfer.HostBussDate = ReqTransfer.SourceDate

            '2022-10-31
            Struct_Queue_Message.SSM_Common_Data.CRF_Issuer_Institution_Number = ReqTransfer.AuthFiNbr
            '2022-10-31


            ReqTransfer.StandinAuthType = "0"
            ReqTransfer.StandinAuthMethod = "4"
            ReqTransfer.StandinResultCode = "0000"
            ReqTransfer.PinVerifyFlag = "0"
            ReqTransfer.TrackIIValidFlag = "1"

            If Struct_Queue_Message.SSM_Common_Data.CRF_PhoneNumber.Length = 10 Then
                ReqTransfer.TargetPhone = Struct_Queue_Message.SSM_Common_Data.CRF_PhoneNumber
            ElseIf Struct_Queue_Message.SSM_Common_Data.CRF_PhoneNumber.Length < 10 Then
                ReqTransfer.TargetPhone = Struct_Queue_Message.SSM_Common_Data.CRF_PhoneNumber.PadLeft(10, "0")
            ElseIf Struct_Queue_Message.SSM_Common_Data.CRF_PhoneNumber.Length > 10 Then
                ReqTransfer.TargetPhone = Struct_Queue_Message.SSM_Common_Data.CRF_PhoneNumber.Substring(0, 10)
            End If

            ReqTransfer.SourceApplType = Get_Token_Info(ACCT_TYPE_ID, TKNid, TKNdata)
            ReqTransfer.FlagRevPD = "PD"
            ReqTransfer.Filler = "   "

            Dim Aba As Int32
            Dim Name As String = String.Empty
            Dim AutPDid As Int16 = Get_Token_Info(AUTHPD_ID, TKNid, TKNdata)

            Dim Tappl As Integer = Get_Token_Info(ACCT_TYPE_TARGET_ID, TKNid, TKNdata).PadLeft(2, "0")
            If Tappl = 2 Then
                Select Case Struct_Queue_Message.SSM_Common_Data.CRF_Secondary_Account.ToString.Length
                    Case 14
                        ReqTransfer.TrackIIData = ";" & Struct_Queue_Message.SSM_Common_Data.CRF_Secondary_Account & "=00000000000000000000000?"
                    Case 15
                        ReqTransfer.TrackIIData = ";" & Struct_Queue_Message.SSM_Common_Data.CRF_Secondary_Account & "=0000000000000000000000?"
                    Case 16
                        ReqTransfer.TrackIIData = ";" & Struct_Queue_Message.SSM_Common_Data.CRF_Secondary_Account & "=000000000000000000000?"
                End Select
            Else
                If GetInfoInstitution(0, AutPDid, Aba, Name) = SUCCESSFUL Then
                    ReqTransfer.TrackIIData = ";" & Format(Aba, "000000") & "0000000000=000000000000000000000?"
                Else
                    ReqTransfer.TrackIIData = ";0000000000000000=000000000000000000000?"
                End If
            End If

            ReqTransfer.CardApplCode = Tappl.ToString.PadLeft(2, "0")

            '' ********************************************************** ''
            ''                         TRANSFER DATA
            '' ********************************************************** ''
            ReqTransfer.OtherFi = Get_Token_Info(AUTHPD_ID, TKNid, TKNdata).PadLeft(4, "0")
            ReqTransfer.OtherAppl = Tappl.ToString.PadLeft(2, "0")
            ReqTransfer.OtherAcct = Struct_Queue_Message.SSM_Common_Data.CRF_Primary_Account.ToString("000000000000000000")
            ReqTransfer.InternalInd = "0"

            If Struct_Queue_Message.SSM_Common_Data.CRF_Names.Length >= 35 Then
                ReqTransfer.SEGi_TargetName = Struct_Queue_Message.SSM_Common_Data.CRF_Names.Substring(0, 35).ToUpper
            Else
                ReqTransfer.SEGi_TargetName = Struct_Queue_Message.SSM_Common_Data.CRF_Names.PadRight(35, " ").ToUpper
            End If
            Dim TempInt64 As Int64
            ReqTransfer.SEGi_Observation = "PAGO DIREC"
            TempInt64 = Get_Token_Info(DOCUMENT_ID, TKNid, TKNdata)
            ReqTransfer.SEGi_SourceID = TempInt64.ToString("0000000000").PadRight(13, " ")
            ReqTransfer.SEGi_City = "PICH/QUITO"

            If Struct_Queue_Message.SSM_Common_Data.CRF_Reference.Length >= 20 Then
                ReqTransfer.SEGi_Reference = Struct_Queue_Message.SSM_Common_Data.CRF_Reference.Substring(0, 20)
            Else
                ReqTransfer.SEGi_Reference = Struct_Queue_Message.SSM_Common_Data.CRF_Reference.PadRight(20, " ")
            End If

            'TempInt64 = Get_Token_Info(TARGET_DOCUMENT_ID, TKNid, TKNdata)
            'ReqTransfer.SEGi_TargetID = TempInt64.ToString("0000000000000")
            ReqTransfer.SEGi_TargetID = Get_Token_Info(TARGET_DOCUMENT_ID, TKNid, TKNdata).PadRight(13, " ")

            ReqTransfer.SEGi_Filler = "      "
            RequestMessage = StructToString(ReqTransfer)
        Catch ex As Exception
            Return "ERROR"
        End Try

        'Show_Message_Console("ORDENANTE:Policia Nacional", COLOR_WHITE, COLOR_BLUE, 0, TRACE_LOW, 0)

        Return RequestMessage
    End Function

    '-------------------------------------------
    'REQUERIMIENTO DE REVERSO HACIA BANRED
    '-------------------------------------------
    Public Function Encode_Reversal_Request(ByVal Struct_Queue_Message As SharedStructureMessage) As String
        Dim RequestMessage As String = String.Empty
        Dim Token_Data As String = String.Empty
        Dim TKNid(20) As String
        Dim TKNdata(20) As String
        Struct_Queue_Message.SSM_Common_Data.CRF_Message_Code = 420

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

        Try
            Dim pBuf As IntPtr
            Dim ReqReversal As FixedStrucure.RequestReversal_C
            ReqReversal = CType(Marshal.PtrToStructure(pBuf, GetType(FixedStrucure.RequestReversal)), FixedStrucure.RequestReversal_C)

            ReqReversal.MessageType = "XR"
            ReqReversal.SourceFiNbr = FI_CPN_ADQ.ToString("0000")
            ReqReversal.SourceTrmlNbr = Struct_Queue_Message.SSM_Common_Data.CRF_Terminal_ID.PadLeft(5, "0")
            ReqReversal.SourceSeqNbr = Struct_Queue_Message.SSM_Common_Data.CRF_Adquirer_Sequence.ToString("000000")
            ReqReversal.MessageSeqNbr = Struct_Queue_Message.SSM_Common_Data.CRF_Switch_Sequence.ToString("0000")
            ReqReversal.TransactionCode = Struct_Queue_Message.SSM_Common_Data.CRF_Transaction_Code.ToString("0000") & "-"

            ReqReversal.SourceDate = Struct_Queue_Message.SSM_Common_Data.CRF_Adquirer_Date_Time.Year.ToString.Substring(2, 2) &
                                Struct_Queue_Message.SSM_Common_Data.CRF_Adquirer_Date_Time.Month.ToString("00") &
                                Struct_Queue_Message.SSM_Common_Data.CRF_Adquirer_Date_Time.Day.ToString("00")
            ReqReversal.SourceTime = Struct_Queue_Message.SSM_Common_Data.CRF_Adquirer_Date_Time.Hour.ToString("00") &
                                Struct_Queue_Message.SSM_Common_Data.CRF_Adquirer_Date_Time.Minute.ToString("00") &
                                Struct_Queue_Message.SSM_Common_Data.CRF_Adquirer_Date_Time.Second.ToString("00")

            ReqReversal.SourceAbaNbr = ABA_CPN_ADQ.ToString("0000000000")
            ReqReversal.SourceBranchNbr = Struct_Queue_Message.SSM_Common_Data.CRF_Branch_ID.PadLeft(4, "0")
            ReqReversal.SourceBussDate = Struct_Queue_Message.SSM_Common_Data.CRF_Adquirer_Date_Time.Year.ToString.Substring(2, 2) &
                                Struct_Queue_Message.SSM_Common_Data.CRF_Adquirer_Date_Time.Month.ToString("00") &
                                Struct_Queue_Message.SSM_Common_Data.CRF_Adquirer_Date_Time.Day.ToString("00")

            Select Case CInt(Struct_Queue_Message.SSM_Common_Data.CRF_Channel_Id)
                Case 3
                    ReqReversal.TerminalType = 1
                Case 4, 7
                    ReqReversal.TerminalType = 2
                Case Else
                    ReqReversal.TerminalType = Struct_Queue_Message.SSM_Common_Data.CRF_Channel_Id
            End Select
            'ReqReversal.TerminalType = Struct_Queue_Message.SSM_Common_Data.CRF_Channel_Id.ToString

            Dim SourceName As String = Get_Token_Info(NAMES_ID, TKNid, TKNdata)
            If SourceName.Length >= 31 Then
                ReqReversal.SourceName = SourceName.Substring(0, 31).ToUpper
            Else
                ReqReversal.SourceName = SourceName.PadRight(31, " ").ToUpper
            End If

            ReqReversal.ForcedIndicator = "0"
            'ReqReversal.ReversalIndicator = "3"
            ReqReversal.ReversalIndicator = "1"
            ReqReversal.AccountNbr = Struct_Queue_Message.SSM_Common_Data.CRF_Secondary_Account.ToString("000000000000000000")
            ReqReversal.TransactionAmount = (Struct_Queue_Message.SSM_Common_Data.CRF_Transaction_Amount * 100).ToString("00000000000000000")
            ReqReversal.TransactionSign = "+"
            ReqReversal.AuthFiNbr = Get_Token_Info(AUTHPD_ID, TKNid, TKNdata).PadLeft(4, "0")
            ReqReversal.HostBussDate = ReqReversal.SourceDate


            ReqReversal.StandinAuthType = "0"
            ReqReversal.StandinAuthMethod = "4"
            ReqReversal.StandinResultCode = "0000"
            ReqReversal.PinVerifyFlag = "0"
            ReqReversal.TrackIIValidFlag = "1"

            If Struct_Queue_Message.SSM_Common_Data.CRF_PhoneNumber.Length = 10 Then
                ReqReversal.TargetPhone = Struct_Queue_Message.SSM_Common_Data.CRF_PhoneNumber
            ElseIf Struct_Queue_Message.SSM_Common_Data.CRF_PhoneNumber.Length < 10 Then
                ReqReversal.TargetPhone = Struct_Queue_Message.SSM_Common_Data.CRF_PhoneNumber.PadLeft(10, "0")
            ElseIf Struct_Queue_Message.SSM_Common_Data.CRF_PhoneNumber.Length > 10 Then
                ReqReversal.TargetPhone = Struct_Queue_Message.SSM_Common_Data.CRF_PhoneNumber.Substring(0, 10)
            End If
            'ReqReversal.TargetPhone = Struct_Queue_Message.SSM_Common_Data.CRF_PhoneNumber.PadLeft(10, "0")

            ReqReversal.SourceApplType = Get_Token_Info(ACCT_TYPE_ID, TKNid, TKNdata)
            ReqReversal.FlagRevPD = "PD"
            ReqReversal.Filler = "   "

            Dim Aba As Int32
            Dim Name As String = String.Empty
            Dim AutPDid As Int16 = Get_Token_Info(AUTHPD_ID, TKNid, TKNdata)

            Dim Tappl As Integer = Get_Token_Info(ACCT_TYPE_TARGET_ID, TKNid, TKNdata).PadLeft(2, "0")
            If Tappl = 2 Then
                Select Case Struct_Queue_Message.SSM_Common_Data.CRF_Secondary_Account.ToString.Length
                    Case 14
                        ReqReversal.TrackIIData = ";" & Struct_Queue_Message.SSM_Common_Data.CRF_Secondary_Account & "=00000000000000000000000?"
                    Case 15
                        ReqReversal.TrackIIData = ";" & Struct_Queue_Message.SSM_Common_Data.CRF_Secondary_Account & "=0000000000000000000000?"
                    Case 16
                        ReqReversal.TrackIIData = ";" & Struct_Queue_Message.SSM_Common_Data.CRF_Secondary_Account & "=000000000000000000000?"
                End Select
            Else
                If GetInfoInstitution(0, AutPDid, Aba, Name) = SUCCESSFUL Then
                    ReqReversal.TrackIIData = ";" & Format(Aba, "000000") & "0000000000=000000000000000000000?"
                Else
                    ReqReversal.TrackIIData = ";0000000000000000=000000000000000000000?"
                End If
            End If

            ReqReversal.CardApplCode = Tappl.ToString.PadLeft(2, "0")

            '' ********************************************************** ''
            ''                         TRANSFER DATA
            '' ********************************************************** ''
            ReqReversal.OtherFi = Get_Token_Info(AUTHPD_ID, TKNid, TKNdata).PadLeft(4, "0")
            ReqReversal.OtherAppl = Tappl.ToString.PadLeft(2, "0")
            ReqReversal.OtherAcct = Struct_Queue_Message.SSM_Common_Data.CRF_Primary_Account.ToString("000000000000000000")
            ReqReversal.InternalInd = "0"

            '' **********************************************************
            ''                         FINAL AMOUNT
            '' **********************************************************
            ReqReversal.ReversalAmount = "00000000000000000"
            ReqReversal.ReversalSign = "+"

            '' **********************************************************
            ''                         SEGMENT OUT
            '' **********************************************************
            If Struct_Queue_Message.SSM_Common_Data.CRF_Names.Length >= 35 Then
                ReqReversal.SEGi_TargetName = Struct_Queue_Message.SSM_Common_Data.CRF_Names.Substring(0, 35).ToUpper
            Else
                ReqReversal.SEGi_TargetName = Struct_Queue_Message.SSM_Common_Data.CRF_Names.PadRight(35, " ").ToUpper
            End If
            Dim TempInt64 As Int64
            ReqReversal.SEGi_Observation = "PAGO DIREC"
            TempInt64 = Get_Token_Info(DOCUMENT_ID, TKNid, TKNdata)

            ReqReversal.SEGi_SourceID = TempInt64.ToString("0000000000").PadRight(13, " ")
            ReqReversal.SEGi_City = "PICH/QUITO"

            If Struct_Queue_Message.SSM_Common_Data.CRF_Reference.Length >= 20 Then
                ReqReversal.SEGi_Reference = Struct_Queue_Message.SSM_Common_Data.CRF_Reference.Substring(0, 20)
            Else
                ReqReversal.SEGi_Reference = Struct_Queue_Message.SSM_Common_Data.CRF_Reference.PadRight(20, " ")
            End If

            'TempInt64 = Get_Token_Info(TARGET_DOCUMENT_ID, TKNid, TKNdata)
            'ReqReversal.SEGi_TargetID = TempInt64.ToString("0000000000000")
            ReqReversal.SEGi_TargetID = Get_Token_Info(TARGET_DOCUMENT_ID, TKNid, TKNdata).PadRight(13, " ")
            ReqReversal.SEGi_Filler = "      "
            RequestMessage = StructToString(ReqReversal)
        Catch ex As Exception
            Return "ERROR"
        End Try

        Show_Message_Console("ORDENANTE:Policia Nacional", COLOR_WHITE, COLOR_BLUE, 0, TRACE_LOW, 0)

        Return RequestMessage
    End Function

End Class
