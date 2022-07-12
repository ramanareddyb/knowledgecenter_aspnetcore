Imports System
Imports System.Web.SessionState

Namespace Ajax
	Friend Class AjaxHandlerSessionStateR
		Inherits AjaxHandler
		Implements IReadOnlySessionState, IRequiresSessionState

		Friend Sub New()
		End Sub
	End Class
End Namespace
