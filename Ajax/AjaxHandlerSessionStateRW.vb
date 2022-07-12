Imports System
Imports System.Web.SessionState

Namespace Ajax
	Friend Class AjaxHandlerSessionStateRW
		Inherits AjaxHandler
		Implements IRequiresSessionState

		Friend Sub New()
		End Sub
	End Class
End Namespace
