Imports System.Net
Imports System.IO
Imports System.Text
Imports System
Imports System.Xml


Public Class ServiSwitch_WSClient
	Public Function Call_WSClient(ByVal URL As String, ByVal RequestXMLMessage As String) As String

		RequestXMLMessage = RequestXMLMessage.Replace("<", "&lt;")
		RequestXMLMessage = RequestXMLMessage.Replace(">", "&gt;")


		Dim ReplyXMLMessage As String = ""

		Console.WriteLine("URL:" & URL)


		Dim docXML As XmlDocument = New XmlDocument()

		Try
			docXML.InnerXml = Generate_Cobis(RequestXMLMessage)

			'Your webservice URL (usually a WSDL web address):		
			Dim req As HttpWebRequest = DirectCast(WebRequest.Create(URL), HttpWebRequest)
			req.Headers.Add("SOAPAction", "http://tempuri.org/service.directpayment.cpn.ecobis.cobiscorp.ws/DirectPaymentExecutor/ExecutePayment")
			req.KeepAlive = True
			req.Method = "POST"
			req.ContentType = "text/xml; charset=utf-8"
			req.Accept = "text/xml"

			'write request data in stream:		
			Dim stm As Stream = req.GetRequestStream()
			docXML.Save(stm)
			stm.Close()
			docXML = Nothing
			Dim resp As WebResponse = req.GetResponse()

			'Process the response returned and return string fomat:		
			stm = resp.GetResponseStream()
			Dim r As StreamReader = New StreamReader(stm)
			ReplyXMLMessage = r.ReadToEnd
			'*********************************************************
			docXML = New XmlDocument
			docXML.InnerXml = ReplyXMLMessage
			Dim NODE As XmlNodeList
			NODE = docXML.GetElementsByTagName("return")
			ReplyXMLMessage = NODE.ItemOf(0).InnerText

			'*********************************************************
		Catch we As WebException
			Dim message As String = New StreamReader(we.Response.GetResponseStream()).ReadToEnd()
			Show_Message_Console("Call_WSClient exception:" & we.Message & "->" & message, COLOR_BLACK, COLOR_RED, 0, TRACE_LOW, 0)
		Catch ex As Exception
			Show_Message_Console("Call_WSClient exception:" & ex.Message, COLOR_BLACK, COLOR_RED, 0, TRACE_LOW, 0)
		End Try

		Console.WriteLine("RESPUESTA ------------------------" & Chr(13))
		Console.WriteLine(ReplyXMLMessage)

		Return ReplyXMLMessage
	End Function


	Private Function Generate_Cobis(ByVal RequestXML As String) As String
		Dim docXmlstr As String

		'To create following SOAP Request XML, you can use Altova XMLSpy or any 	
		'other webservice testing/creating application:	

		Console.WriteLine("XML:" & RequestXML)

		docXmlstr = "<?xml version=" & Chr(34) & "1.0" & Chr(34) & " encoding=" & Chr(34) & "utf-8" & Chr(34) & "?>"
		docXmlstr += "<"
		docXmlstr = docXmlstr + "soap:Envelope xmlns:soap=" + Chr(34)
		docXmlstr = docXmlstr + "http://schemas.xmlsoap.org/soap/envelope/" + Chr(34)
		docXmlstr = docXmlstr + " xmlns:xsi=" + Chr(34)
		docXmlstr = docXmlstr + "http://www.w3.org/2001/XMLSchema-instance" + Chr(34)
		docXmlstr = docXmlstr + " xmlns:xsd=" + Chr(34)
		docXmlstr = docXmlstr + "http://www.w3.org/2001/XMLSchema" + Chr(34) + ">"

		docXmlstr = docXmlstr + "<soap:Body>"

		'ReservationImport is the function name provided by webservice:
		docXmlstr = docXmlstr + "<ExecutePayment xmlns=" + Chr(34) + "http://service.directpayment.cpn.ecobis.cobiscorp.ws/" + Chr(34) + ">"
		docXmlstr = docXmlstr + "<inDirectPaymentRequest xmlns=" + Chr(34) + "http://service.directpayment.cpn.ecobis.cobiscorp" + Chr(34) + ">"
		docXmlstr = docXmlstr + RequestXML
		docXmlstr = docXmlstr + "</inDirectPaymentRequest>"
		docXmlstr = docXmlstr + "</ExecutePayment>"
		docXmlstr = docXmlstr + "</soap:Body>"
		docXmlstr = docXmlstr + "</soap:Envelope>"

		Console.WriteLine("XML:" & docXmlstr)

		Return docXmlstr

	End Function


End Class
