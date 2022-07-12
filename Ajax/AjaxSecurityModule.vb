Imports System
Imports System.Web

Namespace Ajax
	Friend Class AjaxSecurityModule
		Implements IHttpModule

        Public Sub Init(context As HttpApplication) Implements IHttpModule.Init
            AddHandler context.BeginRequest, AddressOf Me.context_BeginRequest
        End Sub

        Public Sub Dispose() Implements IHttpModule.Dispose
        End Sub

        Private Sub context_BeginRequest(sender As Object, e As EventArgs)
			Dim request As HttpRequest = HttpContext.Current.Request
			If request.HttpMethod <> "POST" OrElse Not request.RawUrl.ToLower().StartsWith(request.ApplicationPath.ToLower() + "/ajax/") OrElse Not request.Url.AbsolutePath.ToLower().EndsWith(".ashx") Then
				Return
			End If
			If request.UserHostAddress = "127.0.0.1" Then
				Dim response As HttpResponse = HttpContext.Current.Response
				response.Write("new Object();r.error = new ajax_error('error','description',0)")
				response.[End]()
			End If
		End Sub


    End Class
End Namespace
