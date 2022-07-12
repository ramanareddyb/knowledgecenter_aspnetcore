Imports MS.Web
Imports System
Imports System.Web

Namespace Ajax
	Friend Class AjaxHandler
		Inherits PageHandler

		Friend Sub New()
		End Sub

        Public Overrides Sub ProcessRequest(context As HttpContext)
            Dim ajaxRequestProcessor As AjaxRequestProcessor = New AjaxRequestProcessor(context)
            Try
                ajaxRequestProcessor.Run()
            Catch ex As Exception
            Finally
                ajaxRequestProcessor = Nothing
            End Try
        End Sub
    End Class
End Namespace
