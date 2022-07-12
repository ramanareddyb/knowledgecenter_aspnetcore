Imports System
Imports System.Data
Imports System.Text
Imports Ajax.JSON
Namespace Ajax.JSON

    Friend Class DataTableConverter
        Implements IAjaxObjectConverter

        Public ReadOnly Property ClientScriptIdentifier() As String Implements IAjaxObjectConverter.ClientScriptIdentifier
            Get
                Return Nothing
            End Get
        End Property

        Public ReadOnly Property SupportedTypes() As Type() Implements IAjaxObjectConverter.SupportedTypes
            Get
                Return New Type() {GetType(DataTable)}
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
            If o.[GetType]() Is GetType(DataTable) OrElse o.[GetType]().BaseType Is GetType(DataTable) Then
                Dim dataTable As DataTable = CType(o, DataTable)
                sb.Append("{'Name':'" + dataTable.TableName + "',")
                sb.Append("'Rows':[")
                For i As Integer = 0 To dataTable.Rows.Count - 1
                    DefaultConverter.ToJSON(sb, dataTable.Rows(i))
                    If i < dataTable.Rows.Count - 1 Then
                        sb.Append(",")
                    End If
                Next
                sb.Append("]}")
            End If
        End Sub


    End Class
End Namespace
