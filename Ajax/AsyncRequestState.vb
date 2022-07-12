Imports System
Imports System.Threading
Imports System.Web

Namespace Ajax
	Friend Class AsyncRequestState
		Implements IAsyncResult

		Friend _ctx As HttpContext

		Friend _cb As AsyncCallback

		Friend _extraData As Object

		Private _isCompleted As Boolean

		Private _callCompleteEvent As ManualResetEvent

        Public ReadOnly Property AsyncState() As Object Implements IAsyncResult.AsyncState
            Get
                Return Me._extraData
            End Get
        End Property

        Public ReadOnly Property CompletedSynchronously() As Boolean Implements IAsyncResult.CompletedSynchronously
            Get
                Return False
            End Get
        End Property

        Public ReadOnly Property IsCompleted() As Boolean Implements IAsyncResult.IsCompleted
            Get
                Return Me._isCompleted
            End Get
        End Property

        Public ReadOnly Property AsyncWaitHandle() As WaitHandle Implements IAsyncResult.AsyncWaitHandle
            Get
                Dim callCompleteEvent As WaitHandle
                SyncLock Me
                    If Me._callCompleteEvent Is Nothing Then
                        Me._callCompleteEvent = New ManualResetEvent(False)
                    End If
                    callCompleteEvent = Me._callCompleteEvent
                End SyncLock
                Return callCompleteEvent
            End Get
        End Property



        Public Sub New(ctx As HttpContext, cb As AsyncCallback, extraData As Object)
            Me._ctx = ctx
            Me._cb = cb
            Me._extraData = extraData
        End Sub

        Friend Sub CompleteRequest()
			Me._isCompleted = True
			SyncLock Me
				If Me._callCompleteEvent IsNot Nothing Then
					Me._callCompleteEvent.[Set]()
				End If
			End SyncLock
			If Me._cb IsNot Nothing Then
				Me._cb(Me)
			End If
		End Sub
	End Class
End Namespace
