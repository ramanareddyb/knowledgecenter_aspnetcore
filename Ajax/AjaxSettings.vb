Imports System
Imports System.Collections

Namespace Ajax
	Friend Class AjaxSettings
		Private m_CommonScript As String

		Private m_CommonLanguage As String = "javascript"

		Private m_TemporaryFiles As AjaxSettingsTemporaryFiles = New AjaxSettingsTemporaryFiles()

		Private m_UrlNamespaceMappings As Hashtable = New Hashtable()

		Friend Property CommonScript() As String
			Get
				Return Me.m_CommonScript
			End Get
			Set(value As String)
				Me.m_CommonLanguage = value
			End Set
		End Property

		Friend Property ScriptLanguage() As String
			Get
				Return Me.m_CommonLanguage
			End Get
			Set(value As String)
				Me.m_CommonLanguage = value
			End Set
		End Property

		Friend Property TemporaryFiles() As AjaxSettingsTemporaryFiles
			Get
				Return Me.m_TemporaryFiles
			End Get
			Set(value As AjaxSettingsTemporaryFiles)
				Me.m_TemporaryFiles = value
			End Set
		End Property

		Friend Property UrlNamespaceMappings() As Hashtable
			Get
				Return Me.m_UrlNamespaceMappings
			End Get
			Set(value As Hashtable)
				Me.m_UrlNamespaceMappings = value
			End Set
		End Property

		Friend Sub New()
		End Sub
	End Class
End Namespace
