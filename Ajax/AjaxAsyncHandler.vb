Imports System
Imports System.Threading
Imports System.Web
Imports System.Web.SessionState

Namespace Ajax
	Friend Class AjaxAsyncHandler
        Implements IRequiresSessionState, IHttpAsyncHandler, IHttpHandler

        Public ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
            Get
                Return False
            End Get
        End Property



        Public Sub ProcessRequest(ctx As HttpContext) Implements IHttpHandler.ProcessRequest
        End Sub

        Public Function BeginProcessRequest(ctx As HttpContext, cb As AsyncCallback, obj As Object) As IAsyncResult Implements IHttpAsyncHandler.BeginProcessRequest
            Dim asyncRequestState As AsyncRequestState = New AsyncRequestState(ctx, cb, obj)
            Dim [object] As AsyncRequest = New AsyncRequest(asyncRequestState)
            Dim start As ThreadStart = AddressOf [object].ProcessRequest
            Dim thread As Thread = New Thread(start)
            thread.Start()
            Return asyncRequestState
        End Function

        Public Sub EndProcessRequest(ar As IAsyncResult) Implements IHttpAsyncHandler.EndProcessRequest
            Dim asyncRequestState As AsyncRequestState = TryCast(ar, AsyncRequestState)
        End Sub


    End Class
End Namespace
