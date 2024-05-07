Imports System.Globalization
Imports System.IO
Imports System.Xml

Public Class ServiSwitch_EncodeDecodeClass
    
    Private Const NOT_FOUND As Integer = -1
    Const status_ON As Byte = 0
    Const FieldType_Fixed As Byte = 0
    Const FieldType_TwoBytes As Byte = 1
    Const FieldType_ThreeBytes As Byte = 2
    Const DT_numeric As Byte = 0
    Const DT_BCD As Byte = 2
    Dim TagName(125) As String
    Dim TagDesc(125) As String


    Public Function FormatNumericData(ByVal DecimalValue As Decimal, ByVal Places As Byte) As String
        Dim StringValue As String = Nothing

        If Places = 10 Then
            StringValue = Format(DecimalValue, "0000000000.00")
        ElseIf Places = 8 Then
            StringValue = Format(DecimalValue, "00000000.00")
        End If

        StringValue = StringValue.Replace(".", "")

        If DecimalValue < 0 Then
            StringValue = "-" & StringValue.Substring(2)
        End If

        Return StringValue

    End Function

    Public Sub Fill_User_Data(ByVal Field As Byte, ByVal IsoData As String, ByRef UserData As String)

        UserData += Format(Field, "000") & Format(IsoData.Length, "000") & IsoData

    End Sub

    Public Function StructToString(obj As Object) As String
        Return String.Join(Nothing, obj.GetType().GetFields().Select(Function(field) field.GetValue(obj)))
    End Function


    Public Function Generate_XML_Request(ByVal Terminal As String,
                                         ByVal Sequence As Int32,
                                         ByVal TransactionCode As Int16,
                                         ByVal DateTimeTran As DateTime,
                                         ByVal BankID As Int64,
                                         ByVal ChannelID As Int16,
                                         ByVal LocalAccountName As String,
                                         ByVal TransactionAmount As Decimal,
                                         ByVal LocalAcctNumber As Int64,
                                         ByVal TargetAcctNumber As Int64,
                                         ByVal AccountType As Int16,
                                         ByVal branch As Int16,
                                         ByVal Operatore As String,
                                         ByVal Saccttype As Int16,
                                         ByVal Scontactnbr As String,
                                         ByVal Rev As Char,
                                         ByVal Target_Account_name As String,
                                         ByVal SourceReference As String,
                                         ByVal TargetIndetification As String,
                                         ByVal SourceIdentification As String) As String
        Dim XmlStringReply As String
        Dim memory_stream As New MemoryStream
        Dim xml_text_writer As New XmlTextWriter(memory_stream, System.Text.Encoding.UTF8)

        ' Use indentation to make the result look nice.
        xml_text_writer.Formatting = Formatting.Indented
        xml_text_writer.Indentation = 5

        ' Write the XML declaration.
        xml_text_writer.WriteStartDocument()

        ' Start the Employees node.
        xml_text_writer.WriteStartElement("serviswitch_request")

        ' Write some Employee elements.
        '*******************************************************************
        Dim TempNumber As String = Scontactnbr.Replace(" ", Nothing)
        If TempNumber.Length = 0 Then
            Scontactnbr = "0000000000"
        End If

        TempNumber = SourceIdentification.Replace(" ", Nothing)
        If TempNumber.Length = 0 Then
            SourceIdentification = "000000000000"
        End If

        TempNumber = SourceReference.Replace(" ", Nothing)
        If TempNumber.Length = 0 Then
            SourceReference = "00000000000000000000"
        End If
        '*******************************************************************
        MakeXMLReply(xml_text_writer, "terminal_id", Terminal)
        MakeXMLReply(xml_text_writer, "sequence_number", Sequence)
        MakeXMLReply(xml_text_writer, "transaction_code", TransactionCode)
        Dim StringDateTime = DateTimeTran.ToString("yyyy-MM-dd HH:mm:ss")
        MakeXMLReply(xml_text_writer, "date_time", StringDateTime)
        MakeXMLReply(xml_text_writer, "host_target_id", BankID)
        MakeXMLReply(xml_text_writer, "channel_id", ChannelID)
        MakeXMLReply(xml_text_writer, "local_account_name", LocalAccountName)
        MakeXMLReply(xml_text_writer, "target_account_name", Target_Account_name)
        MakeXMLReply(xml_text_writer, "transaction_amount", TransactionAmount)
        MakeXMLReply(xml_text_writer, "source_account_number", LocalAcctNumber)
        MakeXMLReply(xml_text_writer, "target_account_number", TargetAcctNumber)
        MakeXMLReply(xml_text_writer, "account_type", AccountType)
        MakeXMLReply(xml_text_writer, "service_code", "100100")
        MakeXMLReply(xml_text_writer, "reverse_reason", Rev)
        MakeXMLReply(xml_text_writer, "branch_code", branch)
        MakeXMLReply(xml_text_writer, "operator_code", Operatore)
        MakeXMLReply(xml_text_writer, "source_account_type", Saccttype)
        MakeXMLReply(xml_text_writer, "source_contact_number", Scontactnbr)
        MakeXMLReply(xml_text_writer, "source_reference", SourceReference)
        MakeXMLReply(xml_text_writer, "target_identification", TargetIndetification)
        MakeXMLReply(xml_text_writer, "source_identification", SourceIdentification)

        '*******************************************************************
        '*******************************************************************
        ' End the Employees node.
        xml_text_writer.WriteEndElement()
        ' End the document.
        xml_text_writer.WriteEndDocument()
        xml_text_writer.Flush()

        ' Use a StreamReader to display the result.
        Dim stream_reader As New StreamReader(memory_stream)

        memory_stream.Seek(0, SeekOrigin.Begin)
        'Dim TextoXML As String = stream_reader.ReadToEnd()
        XmlStringReply = stream_reader.ReadToEnd()
        ' Close the XmlTextWriter.
        xml_text_writer.Close()

        Return XmlStringReply

    End Function


    Public Function Generate_XML_Broadcast(ByVal response_code As String,
                                          ByVal Terminal As String,
                                          ByVal Sequence As Int32,
                                          ByVal TransactionCode As Int16,
                                          ByVal DateTimeTran As DateTime,
                                          ByVal BankID As Int64,
                                          ByVal TransactionAmount As Decimal,
                                          ByVal LocalAcctNumber As Int64,
                                          ByVal TargetAcctNumber As Int64,
                                          ByVal ChannelID As Int16,
                                          ByVal TokenInfo As String,
                                          ByVal ServiceCode As String,
                                          ByVal ReverseInd As Byte) As String

        Dim XmlStringReply As String
        Dim memory_stream As New MemoryStream
        Dim xml_text_writer As New XmlTextWriter(memory_stream, System.Text.Encoding.UTF8)

        ' Use indentation to make the result look nice.
        xml_text_writer.Formatting = Formatting.Indented
        xml_text_writer.Indentation = 5

        ' Write the XML declaration.
        xml_text_writer.WriteStartDocument()

        ' Start the Employees node.
        xml_text_writer.WriteStartElement("serviswitch_request")

        ' Write some Employee elements.
        '*******************************************************************
        '*******************************************************************
        'MakeXMLReply(xml_text_writer, "response_code", response_code)
        MakeXMLReply(xml_text_writer, "terminal_id", Terminal)
        MakeXMLReply(xml_text_writer, "sequence_number", Sequence)
        MakeXMLReply(xml_text_writer, "transaction_code", TransactionCode)
        Dim StringDateTime = DateTimeTran.ToString("yyyy-MM-dd HH:mm:ss")
        MakeXMLReply(xml_text_writer, "date_time", StringDateTime)
        MakeXMLReply(xml_text_writer, "host_target_id", BankID)
        MakeXMLReply(xml_text_writer, "channel_id", ChannelID)
        MakeXMLReply(xml_text_writer, "transaction_amount", TransactionAmount)
        MakeXMLReply(xml_text_writer, "source_account_number", LocalAcctNumber)
        MakeXMLReply(xml_text_writer, "target_account_number", TargetAcctNumber)
        MakeXMLReply(xml_text_writer, "account_type", Get_Token_Info("X7", TokenInfo))
        MakeXMLReply(xml_text_writer, "service_code", ServiceCode)
        MakeXMLReply(xml_text_writer, "reverse_reason", ReverseInd)
        MakeXMLReply(xml_text_writer, "source_account_type", Get_Token_Info("A3", TokenInfo))
        '*******************************************************************
        '*******************************************************************
        ' End the Employees node.
        xml_text_writer.WriteEndElement()
        ' End the document.
        xml_text_writer.WriteEndDocument()
        xml_text_writer.Flush()

        ' Use a StreamReader to display the result.
        Dim stream_reader As New StreamReader(memory_stream)

        memory_stream.Seek(0, SeekOrigin.Begin)
        'Dim TextoXML As String = stream_reader.ReadToEnd()
        XmlStringReply = stream_reader.ReadToEnd()
        ' Close the XmlTextWriter.
        xml_text_writer.Close()

        Return XmlStringReply

    End Function




    Private Sub MakeXMLReply(ByVal xml_text_writer As XmlTextWriter, ByVal FieldName As String, ByVal FieldValue As String)

        xml_text_writer.WriteStartElement(FieldName)
        xml_text_writer.WriteString(FieldValue)
        xml_text_writer.WriteEndElement()

    End Sub

    Public Function Get_Token_Info(ByVal Search_Token As String, ByVal TKN_id() As String, ByVal TKN_val() As String) As String
        Dim TKN_reply As String = "0"

        If TKN_id.Contains(Search_Token) Then
            TKN_reply = TKN_val(Array.IndexOf(TKN_id, Search_Token))
        End If

        Return TKN_reply
    End Function

    Public Function Get_Token_Info(ByVal TokenId As String, ByVal PassTokenField As String) As String
        Dim TokenData As String = ""
        Dim TknId As New List(Of String)
        Dim TknData As New List(Of String)
        Try
            If PassTokenField.Length > 0 Then
                Dim x, y As Integer
                Do While PassTokenField.Length > 0
                    TknId.Add(PassTokenField.Substring(0, 2))
                    PassTokenField = PassTokenField.Remove(0, 2)
                    y = PassTokenField.Substring(0, 3)
                    PassTokenField = PassTokenField.Remove(0, 3)
                    TknData.Add(PassTokenField.Substring(0, y))
                    PassTokenField = PassTokenField.Remove(0, y)
                    x += 1
                Loop
            End If
        Catch ex As Exception
            Console.WriteLine("Error # 001:" & ex.Message)
        End Try

        If TknId.Contains(TokenId) Then
            Dim Idx As Byte = TknId.IndexOf(TokenId)
            TokenData = TknData.Item(Idx)
        End If

        Return TokenData
    End Function


    Public Sub Build_User_Token(ByRef USER_DATA As String, ByVal Token_Id As String, ByRef InputData As String)
        If IsNothing(InputData) Or (InputData.Length = 0) Then
            InputData = " "
        End If
        Try
            USER_DATA += Token_Id & InputData.Length.ToString("000") & InputData
        Catch ex As Exception
            USER_DATA += Token_Id & "000"
        End Try
    End Sub

    Public Sub Update_User_Token(ByRef USER_DATA As String, ByVal Token_Id As String, ByRef InputData As String)
        If IsNothing(InputData) Or (InputData.Length = 0) Then
            InputData = " "
        End If
        Try
            Dim idx, len As Integer
            idx = USER_DATA.IndexOf(Token_Id)
            If idx = NOT_FOUND Then
                Exit Sub
            End If
            len = USER_DATA.Substring(idx + 2, 3)
            USER_DATA = USER_DATA.Remove(idx, 5 + len)
            USER_DATA += Token_Id & InputData.Length.ToString("000") & InputData
        Catch ex As Exception
            USER_DATA += Token_Id & "000"
        End Try
    End Sub


End Class
