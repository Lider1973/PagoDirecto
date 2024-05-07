﻿'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'     Runtime Version:4.0.30319.42000
'
'     Changes to this file may cause incorrect behavior and will be lost if
'     the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict On
Option Explicit On


Namespace ServiceReference1
    
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0"),  _
     System.ServiceModel.ServiceContractAttribute([Namespace]:="http://service.directpayment.cpn.ecobis.cobiscorp.ws/", ConfigurationName:="ServiceReference1.DirectPaymentExecutor")>  _
    Public Interface DirectPaymentExecutor
        
        'CODEGEN: Generating message contract since element name inDirectPaymentRequest from namespace  is not marked nillable
        <System.ServiceModel.OperationContractAttribute(Action:="http://service.directpayment.cpn.ecobis.cobiscorp.ws/DirectPaymentExecutor/Execut"& _ 
            "ePayment", ReplyAction:="http://service.directpayment.cpn.ecobis.cobiscorp.ws/DirectPaymentExecutor/Execut"& _ 
            "ePaymentResponse")>  _
        Function ExecutePayment(ByVal request As ServiceReference1.ExecutePaymentRequest) As ServiceReference1.ExecutePaymentResponse
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://service.directpayment.cpn.ecobis.cobiscorp.ws/DirectPaymentExecutor/Execut"& _ 
            "ePayment", ReplyAction:="http://service.directpayment.cpn.ecobis.cobiscorp.ws/DirectPaymentExecutor/Execut"& _ 
            "ePaymentResponse")>  _
        Function ExecutePaymentAsync(ByVal request As ServiceReference1.ExecutePaymentRequest) As System.Threading.Tasks.Task(Of ServiceReference1.ExecutePaymentResponse)
    End Interface
    
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0"),  _
     System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced),  _
     System.ServiceModel.MessageContractAttribute(IsWrapped:=false)>  _
    Partial Public Class ExecutePaymentRequest
        
        <System.ServiceModel.MessageBodyMemberAttribute(Name:="ExecutePayment", [Namespace]:="http://service.directpayment.cpn.ecobis.cobiscorp.ws/", Order:=0)>  _
        Public Body As ServiceReference1.ExecutePaymentRequestBody
        
        Public Sub New()
            MyBase.New
        End Sub
        
        Public Sub New(ByVal Body As ServiceReference1.ExecutePaymentRequestBody)
            MyBase.New
            Me.Body = Body
        End Sub
    End Class
    
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0"),  _
     System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced),  _
     System.Runtime.Serialization.DataContractAttribute([Namespace]:="")>  _
    Partial Public Class ExecutePaymentRequestBody
        
        <System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue:=false, Order:=0)>  _
        Public inDirectPaymentRequest As String
        
        Public Sub New()
            MyBase.New
        End Sub
        
        Public Sub New(ByVal inDirectPaymentRequest As String)
            MyBase.New
            Me.inDirectPaymentRequest = inDirectPaymentRequest
        End Sub
    End Class
    
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0"),  _
     System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced),  _
     System.ServiceModel.MessageContractAttribute(IsWrapped:=false)>  _
    Partial Public Class ExecutePaymentResponse
        
        <System.ServiceModel.MessageBodyMemberAttribute(Name:="ExecutePaymentResponse", [Namespace]:="http://service.directpayment.cpn.ecobis.cobiscorp.ws/", Order:=0)>  _
        Public Body As ServiceReference1.ExecutePaymentResponseBody
        
        Public Sub New()
            MyBase.New
        End Sub
        
        Public Sub New(ByVal Body As ServiceReference1.ExecutePaymentResponseBody)
            MyBase.New
            Me.Body = Body
        End Sub
    End Class
    
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0"),  _
     System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced),  _
     System.Runtime.Serialization.DataContractAttribute([Namespace]:="")>  _
    Partial Public Class ExecutePaymentResponseBody
        
        <System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue:=false, Order:=0)>  _
        Public [return] As String
        
        Public Sub New()
            MyBase.New
        End Sub
        
        Public Sub New(ByVal [return] As String)
            MyBase.New
            Me.[return] = [return]
        End Sub
    End Class
    
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")>  _
    Public Interface DirectPaymentExecutorChannel
        Inherits ServiceReference1.DirectPaymentExecutor, System.ServiceModel.IClientChannel
    End Interface
    
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")>  _
    Partial Public Class DirectPaymentExecutorClient
        Inherits System.ServiceModel.ClientBase(Of ServiceReference1.DirectPaymentExecutor)
        Implements ServiceReference1.DirectPaymentExecutor
        
        Public Sub New()
            MyBase.New
        End Sub
        
        Public Sub New(ByVal endpointConfigurationName As String)
            MyBase.New(endpointConfigurationName)
        End Sub
        
        Public Sub New(ByVal endpointConfigurationName As String, ByVal remoteAddress As String)
            MyBase.New(endpointConfigurationName, remoteAddress)
        End Sub
        
        Public Sub New(ByVal endpointConfigurationName As String, ByVal remoteAddress As System.ServiceModel.EndpointAddress)
            MyBase.New(endpointConfigurationName, remoteAddress)
        End Sub
        
        Public Sub New(ByVal binding As System.ServiceModel.Channels.Binding, ByVal remoteAddress As System.ServiceModel.EndpointAddress)
            MyBase.New(binding, remoteAddress)
        End Sub
        
        <System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Function ServiceReference1_DirectPaymentExecutor_ExecutePayment(ByVal request As ServiceReference1.ExecutePaymentRequest) As ServiceReference1.ExecutePaymentResponse Implements ServiceReference1.DirectPaymentExecutor.ExecutePayment
            Return MyBase.Channel.ExecutePayment(request)
        End Function
        
        Public Function ExecutePayment(ByVal inDirectPaymentRequest As String) As String
            Dim inValue As ServiceReference1.ExecutePaymentRequest = New ServiceReference1.ExecutePaymentRequest()
            inValue.Body = New ServiceReference1.ExecutePaymentRequestBody()
            inValue.Body.inDirectPaymentRequest = inDirectPaymentRequest
            Dim retVal As ServiceReference1.ExecutePaymentResponse = CType(Me,ServiceReference1.DirectPaymentExecutor).ExecutePayment(inValue)
            Return retVal.Body.[return]
        End Function
        
        <System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Function ServiceReference1_DirectPaymentExecutor_ExecutePaymentAsync(ByVal request As ServiceReference1.ExecutePaymentRequest) As System.Threading.Tasks.Task(Of ServiceReference1.ExecutePaymentResponse) Implements ServiceReference1.DirectPaymentExecutor.ExecutePaymentAsync
            Return MyBase.Channel.ExecutePaymentAsync(request)
        End Function
        
        Public Function ExecutePaymentAsync(ByVal inDirectPaymentRequest As String) As System.Threading.Tasks.Task(Of ServiceReference1.ExecutePaymentResponse)
            Dim inValue As ServiceReference1.ExecutePaymentRequest = New ServiceReference1.ExecutePaymentRequest()
            inValue.Body = New ServiceReference1.ExecutePaymentRequestBody()
            inValue.Body.inDirectPaymentRequest = inDirectPaymentRequest
            Return CType(Me,ServiceReference1.DirectPaymentExecutor).ExecutePaymentAsync(inValue)
        End Function
    End Class
End Namespace
