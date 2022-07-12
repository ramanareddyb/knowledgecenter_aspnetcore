Imports System
Imports System.Data
Imports System.Text

Namespace Ajax.JSON
	Friend Class DataRowConverter
		Implements IAjaxObjectConverter

        Public ReadOnly Property ClientScriptIdentifier() As String Implements IAjaxObjectConverter.ClientScriptIdentifier
            Get
                Return Nothing
            End Get
        End Property

        Public ReadOnly Property SupportedTypes() As Type() Implements IAjaxObjectConverter.SupportedTypes
            Get
                Return New Type() {GetType(DataRow)}
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
            If o.[GetType]() Is GetType(DataRow) OrElse o.[GetType]().BaseType Is GetType(DataRow) Then
                Dim dataRow As DataRow = CType(o, DataRow)
                sb.Append("{")
                For i As Integer = 0 To dataRow.Table.Columns.Count - 1
                    sb.Append("'" + dataRow.Table.Columns(i).ColumnName + "':")
                    If dataRow(i) Is DBNull.Value Then
                        sb.Append("null")
                    Else
                        DefaultConverter.ToJSON(sb, dataRow(i))
                    End If
                    If i < dataRow.Table.Columns.Count - 1 Then
                        sb.Append(",")
                    End If
                Next
                sb.Append("}")
            End If
        End Sub


    End Class
End Namespace
