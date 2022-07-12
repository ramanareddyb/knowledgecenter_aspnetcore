Imports System
Imports System.Text

Namespace Ajax.JSON
	Public Interface IAjaxObjectConverter
		ReadOnly Property SupportedTypes() As Type()

		ReadOnly Property IncludeSubclasses() As Boolean

		ReadOnly Property ClientScriptIdentifier() As String

		Sub ToJSON(ByRef sb As StringBuilder, o As Object)

		Function FromString(s As String, t As Type) As Object

		Sub RenderClientScript(ByRef sb As StringBuilder)
	End Interface
End Namespace
