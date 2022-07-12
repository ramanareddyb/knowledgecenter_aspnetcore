Imports System
Imports System.Web

Namespace MS.Web
	Friend Class PageHandler
		Implements IHttpHandler

		Private m_Request As Request

		Private m_Assembly As String

		Private m_HandlerUri As String

		Public Property Request() As Request
			Get
				Return Me.m_Request
			End Get
			Set(value As Request)
				Me.m_Request = value
			End Set
		End Property

		Public Property Assembly() As String
			Get
				Return Me.m_Assembly
			End Get
			Set(value As String)
				Me.m_Assembly = value
			End Set
		End Property

		Public Property HandlerUri() As String
			Get
				Return Me.m_HandlerUri
			End Get
			Set(value As String)
				Me.m_HandlerUri = value
			End Set
		End Property

        Public Overridable ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
            Get
                Return True
            End Get
        End Property



        Friend Sub New()
        End Sub

        Public Overridable Sub ProcessRequest(context As HttpContext) Implements IHttpHandler.ProcessRequest
        End Sub

        Public Sub Write(text As String)
            If HttpContext.Current Is Nothing Then
                Throw New NullReferenceException("The context is not set correct in ProcessRequest().")
            End If
            HttpContext.Current.Response.Write(text)
        End Sub

        Public Sub WriteLine(line As String)
            Me.Write(line)
            Me.Write(vbCrLf)
        End Sub

    End Class
End Namespace
