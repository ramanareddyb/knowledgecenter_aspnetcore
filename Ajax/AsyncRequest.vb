Imports System

Namespace Ajax
	Friend Class AsyncRequest
		Private _asyncRequestState As AsyncRequestState

		Public Sub New(ars As AsyncRequestState)
			Me._asyncRequestState = ars
		End Sub

		Public Sub ProcessRequest()
			Dim ajaxRequestProcessor As AjaxRequestProcessor = New AjaxRequestProcessor(Me._asyncRequestState._ctx)
			ajaxRequestProcessor.Run()
			Me._asyncRequestState.CompleteRequest()
		End Sub
	End Class
End Namespace
