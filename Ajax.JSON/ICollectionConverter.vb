Imports System
Imports System.Collections
Imports System.Text

Namespace Ajax.JSON
	Friend Class ICollectionConverter
		Implements IAjaxObjectConverter

        Public ReadOnly Property ClientScriptIdentifier() As String Implements IAjaxObjectConverter.ClientScriptIdentifier
            Get
                Return Nothing
            End Get
        End Property

        Public ReadOnly Property SupportedTypes() As Type() Implements IAjaxObjectConverter.SupportedTypes
            Get
                Return New Type() {GetType(ICollection)}
            End Get
        End Property

        Public ReadOnly Property IncludeSubclasses() As Boolean Implements IAjaxObjectConverter.IncludeSubclasses
            Get
                Return True
            End Get
        End Property



        Public Sub RenderClientScript(ByRef sb As StringBuilder) Implements IAjaxObjectConverter.RenderClientScript
        End Sub

        Public Function FromString(s As String, t As Type) As Object Implements IAjaxObjectConverter.FromString
            Throw New NotImplementedException()
        End Function

        Public Sub ToJSON(ByRef sb As StringBuilder, o As Object) Implements IAjaxObjectConverter.ToJSON
            sb.Append("[")
            Dim collection As ICollection = CType(o, ICollection)
            Dim array As Object() = New Object(collection.Count - 1) {}
            collection.CopyTo(array, 0)
            For i As Integer = 0 To array.Length - 1
                DefaultConverter.ToJSON(sb, array(i))
                If i < array.Length - 1 Then
                    sb.Append(",")
                End If
            Next
            sb.Append("]")
        End Sub

    End Class
End Namespace
