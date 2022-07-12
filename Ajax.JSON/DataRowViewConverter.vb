Imports System
Imports System.Data
Imports System.Text

Namespace Ajax.JSON
	Friend Class DataRowViewConverter
		Implements IAjaxObjectConverter

        Public ReadOnly Property ClientScriptIdentifier() As String Implements IAjaxObjectConverter.ClientScriptIdentifier
            Get
                Return Nothing
            End Get
        End Property

        Public ReadOnly Property SupportedTypes() As Type() Implements IAjaxObjectConverter.SupportedTypes
            Get
                Return New Type() {GetType(DataRowView)}
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
            If o.[GetType]() Is GetType(DataRowView) OrElse o.[GetType]().BaseType Is GetType(DataRowView) Then
                Dim dataRowView As DataRowView = CType(o, DataRowView)
                sb.Append("{")
                For i As Integer = 0 To dataRowView.DataView.Table.Columns.Count - 1
                    sb.Append("'" + dataRowView.DataView.Table.Columns(i).ColumnName + "':")
                    If dataRowView(i) Is DBNull.Value Then
                        sb.Append("null")
                    Else
                        DefaultConverter.ToJSON(sb, dataRowView(i))
                    End If
                    If i < dataRowView.DataView.Table.Columns.Count - 1 Then
                        sb.Append(",")
                    End If
                Next
                sb.Append("}")
            End If
        End Sub

    End Class
End Namespace
