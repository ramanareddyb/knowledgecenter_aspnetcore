Imports System
Imports System.Web.SessionState

Namespace Ajax
	Friend Class AjaxAsyncHandlerSessionStateR
		Inherits AjaxAsyncHandler
        Implements IReadOnlySessionState, IRequiresSessionState
        Friend Sub New()

        End Sub
    End Class
End Namespace
