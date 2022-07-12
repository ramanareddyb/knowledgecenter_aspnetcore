Imports MS.Web
Imports System
Imports System.Web
Imports System.Web.Caching
Imports Ajax

Namespace Ajax
    Friend Class PageHandlerFactory
        Implements IHttpHandlerFactory

        Public Sub ReleaseHandler(handler As IHttpHandler) Implements IHttpHandlerFactory.ReleaseHandler
            'Throw New NotImplementedException()
        End Sub

        Public Function GetHandler(context As HttpContext, requestType As String, url As String, pathTranslated As String) As IHttpHandler Implements IHttpHandlerFactory.GetHandler
            Dim request As Request = New Request(context, requestType, url)
            Try
                If Not (request.Extension = Utility.HandlerExtension) Then
                    Throw New HttpException(403, "The type of page you have requested is not served because it has been explicitly forbidden. The extension '." + request.Extension + "' may be incorrect. Please review the URL below and make sure that it is spelled correctly.")
                End If
                If context.Request("_session") IsNot Nothing Then
                    If context.Request("_session") = "rw" Then
                        Return New AjaxHandlerSessionStateRW()
                    End If
                    If context.Request("_session") = "r" Then
                        Return New AjaxHandlerSessionStateR()
                    End If
                    If context.Request("_session") = "no" Then
                        Return New AjaxHandler()
                    End If
                End If
                If context.Session IsNot Nothing AndAlso Not context.Session.IsCookieless Then
                    Return New AjaxHandler()
                End If
                Return New AjaxHandlerSessionStateR()
            Catch ex As Exception

            Finally
                request = Nothing
            End Try

        End Function

    End Class
End Namespace
