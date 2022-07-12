Imports System
Imports System.Text

Namespace Ajax.JSON
	Friend Class GuidConverter
		Implements IAjaxObjectConverter

        Public ReadOnly Property ClientScriptIdentifier() As String Implements IAjaxObjectConverter.ClientScriptIdentifier
            Get
                Return Nothing
            End Get
        End Property

        Public ReadOnly Property SupportedTypes() As Type() Implements IAjaxObjectConverter.SupportedTypes
            Get
                Return New Type() {GetType(Guid)}
            End Get
        End Property

        Public ReadOnly Property IncludeSubclasses() As Boolean Implements IAjaxObjectConverter.IncludeSubclasses
            Get
                Return False
            End Get
        End Property


        Public Sub RenderClientScript(ByRef sb As StringBuilder) Implements IAjaxObjectConverter.RenderClientScript
        End Sub

        Public Function FromString(s As String, t As Type) As Object Implements IAjaxObjectConverter.FromString
            Throw New NotImplementedException()
        End Function

        Public Sub ToJSON(ByRef sb As StringBuilder, o As Object) Implements IAjaxObjectConverter.ToJSON
            DefaultConverter.ToJSON(sb, (CType(o, Guid)).ToString())
        End Sub

    End Class
End Namespace
