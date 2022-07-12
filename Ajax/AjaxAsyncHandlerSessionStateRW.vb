Imports System
Imports System.Web.SessionState

Namespace Ajax
	Friend Class AjaxAsyncHandlerSessionStateRW
		Inherits AjaxAsyncHandler
        Implements IRequiresSessionState
        Friend Sub New()
		End Sub
	End Class
End Namespace
