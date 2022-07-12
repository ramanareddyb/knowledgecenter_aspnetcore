Imports System

Namespace Ajax
	Friend Class AjaxSettingsTemporaryFiles
		Private m_Path As String = "~/images"

		Private m_DeleteAfter As Integer = 60

		Friend Property Path() As String
			Get
				Return Me.m_Path
			End Get
			Set(value As String)
				Me.m_Path = value
			End Set
		End Property

		Friend Property DeleteAfter() As Integer
			Get
				Return Me.m_DeleteAfter
			End Get
			Set(value As Integer)
				Me.m_DeleteAfter = value
			End Set
		End Property

		Friend Sub New()
		End Sub

		Friend Sub New(path As String, deleteAfter As Integer)
			Me.m_Path = path
			Me.m_DeleteAfter = deleteAfter
		End Sub
	End Class
End Namespace
