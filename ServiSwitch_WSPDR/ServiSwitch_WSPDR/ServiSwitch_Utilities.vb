Imports System.Xml
Imports System.IO

Module ServiSwitch_Utilities
    Public PathLog As String
    Public Const SUCCESSFUL As Byte = 0
    Public Const UNSUCCESSFUL As Byte = 1
    Public Const UNKNOW As Byte = 99
    Public Const TIMEOUT As Int64 = -2147467259
    Public Const NAMES_ID As String = "X2"
    Public Const MESSAGE_ID As String = "X3"
    Public Const DOCUMENT_ID As String = "X4"
    Public Const LABEL_ID As String = "X5"
    Public Const SERVICE_ID As String = "X6"
    Public Const ACCT_TYPE_ID As String = "X7"
    Public Const SERVICES As String = "X9"
    Public Const AUTHPD_ID As String = "A1"
    Public Const TIII_ID As String = "A2"
    Public Const ACCT_TYPE_TARGET_ID As String = "A3"
    Public Const TARGET_DOCUMENT_ID As String = "A4"

    Public Function GetArrayXMLFields(ByVal XMLRequestMessage As String) As List(Of String)
        Dim XmlArrayList As New List(Of String)
        Dim Message_XML As New XmlDocument
        Dim ErrorCode As Byte = UNKNOW
        Dim LogMessage As String = ""

        Try
            Message_XML.LoadXml(XMLRequestMessage)
            ErrorCode = SUCCESSFUL
        Catch ex As Exception
            ErrorCode = UNSUCCESSFUL
            Continous_Save_Log("LoadXml Exception:" & ex.Message)
        End Try

        If ErrorCode = UNSUCCESSFUL Then
            Return XmlArrayList
        End If

        Dim TempNODE As XmlNodeList
        Try
            LogMessage = "terminal_id"
            TempNODE = Message_XML.GetElementsByTagName("terminal_id")
            XmlArrayList.Add(TempNODE.ItemOf(0).InnerText)

            LogMessage = "sequence_number"
            TempNODE = Message_XML.GetElementsByTagName("sequence_number")
            XmlArrayList.Add(TempNODE.ItemOf(0).InnerText)

            LogMessage = "transaction_code"
            TempNODE = Message_XML.GetElementsByTagName("transaction_code")
            XmlArrayList.Add(TempNODE.ItemOf(0).InnerText)

            LogMessage = "date_time"
            TempNODE = Message_XML.GetElementsByTagName("date_time")
            XmlArrayList.Add(TempNODE.ItemOf(0).InnerText)

            LogMessage = "host_target_id"
            TempNODE = Message_XML.GetElementsByTagName("host_target_id")
            XmlArrayList.Add(TempNODE.ItemOf(0).InnerText)

            LogMessage = "channel_id"
            TempNODE = Message_XML.GetElementsByTagName("channel_id")
            XmlArrayList.Add(TempNODE.ItemOf(0).InnerText)

            LogMessage = "local_account_name"
            TempNODE = Message_XML.GetElementsByTagName("local_account_name")
            XmlArrayList.Add(TempNODE.ItemOf(0).InnerText)

            LogMessage = "target_account_name"
            TempNODE = Message_XML.GetElementsByTagName("target_account_name")
            XmlArrayList.Add(TempNODE.ItemOf(0).InnerText)

            LogMessage = "transaction_amount"
            TempNODE = Message_XML.GetElementsByTagName("transaction_amount")
            XmlArrayList.Add(TempNODE.ItemOf(0).InnerText)

            LogMessage = "source_account_number"
            TempNODE = Message_XML.GetElementsByTagName("source_account_number")
            XmlArrayList.Add(TempNODE.ItemOf(0).InnerText)

            LogMessage = "target_account_number"
            TempNODE = Message_XML.GetElementsByTagName("target_account_number")
            XmlArrayList.Add(TempNODE.ItemOf(0).InnerText)

            LogMessage = "account_type"
            TempNODE = Message_XML.GetElementsByTagName("account_type")
            XmlArrayList.Add(TempNODE.ItemOf(0).InnerText)

            LogMessage = "service_code"
            TempNODE = Message_XML.GetElementsByTagName("service_code")
            XmlArrayList.Add(TempNODE.ItemOf(0).InnerText)

            LogMessage = "reverse_reason"
            TempNODE = Message_XML.GetElementsByTagName("reverse_reason")
            XmlArrayList.Add(TempNODE.ItemOf(0).InnerText)

            LogMessage = "branch_code"
            TempNODE = Message_XML.GetElementsByTagName("branch_code")
            XmlArrayList.Add(TempNODE.ItemOf(0).InnerText)

            LogMessage = "operator_code"
            TempNODE = Message_XML.GetElementsByTagName("operator_code")
            XmlArrayList.Add(TempNODE.ItemOf(0).InnerText)

            LogMessage = "source_account_type"
            TempNODE = Message_XML.GetElementsByTagName("source_account_type")
            XmlArrayList.Add(TempNODE.ItemOf(0).InnerText)

            LogMessage = "source_contact_number"
            TempNODE = Message_XML.GetElementsByTagName("source_contact_number")
            XmlArrayList.Add(TempNODE.ItemOf(0).InnerText)

            LogMessage = "source_reference"
            TempNODE = Message_XML.GetElementsByTagName("source_reference")
            XmlArrayList.Add(TempNODE.ItemOf(0).InnerText)

            LogMessage = "source_identification"
            TempNODE = Message_XML.GetElementsByTagName("source_identification")
            XmlArrayList.Add(TempNODE.ItemOf(0).InnerText)

            LogMessage = "target_identification"
            TempNODE = Message_XML.GetElementsByTagName("target_identification")
            XmlArrayList.Add(TempNODE.ItemOf(0).InnerText)

        Catch ex As Exception
            XmlArrayList.Clear()
            Continous_Save_Log("LoadXml Exception Field:" & LogMessage & " - " & ex.Message)
        End Try

        Message_XML = Nothing

        Return XmlArrayList
    End Function

    Public Function Generate_XML_Reply(ByVal XmlArrayList As List(Of String)) As String
        Dim XmlStringReply As String
        Dim memory_stream As New MemoryStream
        Dim xml_text_writer As New XmlTextWriter(memory_stream, System.Text.Encoding.UTF8)

        ' Use indentation to make the result look nice.
        xml_text_writer.Formatting = Formatting.Indented
        xml_text_writer.Indentation = 5

        ' Write the XML declaration.
        xml_text_writer.WriteStartDocument()

        ' Start the Employees node.
        xml_text_writer.WriteStartElement("serviswitch_reply")

        ' Write some Employee elements.
        '*******************************************************************
        '*******************************************************************
        MakeXMLReply(xml_text_writer, "terminal_id", XmlArrayList(Constanting_Definition.rep_ID_Terminal))
        MakeXMLReply(xml_text_writer, "response_code", XmlArrayList(Constanting_Definition.rep_ID_Response))
        MakeXMLReply(xml_text_writer, "transaction_code", XmlArrayList(Constanting_Definition.rep_ID_Transaction))
        MakeXMLReply(xml_text_writer, "date_time", XmlArrayList(Constanting_Definition.rep_ID_DateTime))
        MakeXMLReply(xml_text_writer, "sequence_number", XmlArrayList(Constanting_Definition.rep_ID_Sequence))
        MakeXMLReply(xml_text_writer, "switch_sequence_number", XmlArrayList(Constanting_Definition.rep_ID_Switch_Sequence))
        MakeXMLReply(xml_text_writer, "target_account_type", XmlArrayList(Constanting_Definition.rep_ID_TargetAccountType))
        MakeXMLReply(xml_text_writer, "target_account_number", XmlArrayList(Constanting_Definition.rep_ID_TargetAccountNumber))
        MakeXMLReply(xml_text_writer, "target_account_name", XmlArrayList(Constanting_Definition.rep_ID_TargetAccountName))
        MakeXMLReply(xml_text_writer, "target_contact_number", XmlArrayList(Constanting_Definition.rep_ID_TargetContact))
        MakeXMLReply(xml_text_writer, "source_reference", XmlArrayList(Constanting_Definition.rep_ID_SourceReference))
        MakeXMLReply(xml_text_writer, "target_identification", XmlArrayList(Constanting_Definition.rep_ID_TargetIdentification))
        xml_text_writer.WriteStartElement("credit_card_data")
        MakeXMLReply(xml_text_writer, "cc_minimun_amount", XmlArrayList(Constanting_Definition.rep_ID_ccMinimunAmt))
        MakeXMLReply(xml_text_writer, "cc_total_amount", XmlArrayList(Constanting_Definition.rep_ID_ccMinimunAmt))
        MakeXMLReply(xml_text_writer, "cc_limit_date", XmlArrayList(Constanting_Definition.rep_ID_ccLimitDate))
        MakeXMLReply(xml_text_writer, "cc_contact", XmlArrayList(Constanting_Definition.rep_ID_ccContact))
        xml_text_writer.WriteEndElement()
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


    Public Sub Initialize_XmlArrayList(ByRef XmlArrayList As List(Of String))

        XmlArrayList.Add("0")
        XmlArrayList.Add("0")
        XmlArrayList.Add("0")
        XmlArrayList.Add(DateTime.Now.ToShortDateString & " " & DateTime.Now.ToLongTimeString)
        XmlArrayList.Add("0")
        XmlArrayList.Add("0")
        XmlArrayList.Add("0")
        XmlArrayList.Add("0")
        XmlArrayList.Add("   ")
        XmlArrayList.Add("0")
        XmlArrayList.Add("0")
        XmlArrayList.Add("0")
        XmlArrayList.Add("0")
        XmlArrayList.Add("0")
        XmlArrayList.Add("0")
        XmlArrayList.Add("0")

    End Sub


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

    Public Function GetDateTime() As String
        Return System.DateTime.Now.Year & "-" & Format(System.DateTime.Now.Month, "00") & "-" & Format(System.DateTime.Now.Day, "00") & " " & Format(System.DateTime.Now.Hour, "00") & ":" & Format(System.DateTime.Now.Minute, "00") & ":" & Format(System.DateTime.Now.Second, "00") & "." & Format(System.DateTime.Now.Millisecond, "000")
    End Function


    Public Sub Continous_Save_Log(ByVal BufferToSave As String)
        Dim FileName As String = PathLog & "\WEBlog" & Now.ToString("yyyyMMdd") & ".txt"

        Dim lockOBJ_IN As Object = New Object
        SyncLock lockOBJ_IN
            Try
                Dim objWriter As System.IO.StreamWriter
                objWriter = New System.IO.StreamWriter(FileName, True)
                objWriter.WriteLine(GetDateTime() & Chr(13) & BufferToSave)
                objWriter.Flush()
                objWriter.Close()
            Catch ex As Exception
                'DisplayMessage(" Excepcion :" & ex.Message, 2, 1)
            End Try
        End SyncLock

    End Sub


End Module
