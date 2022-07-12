Imports System
Imports System.Data
Imports System.Text

Namespace Ajax.JSON
	Friend Class DataViewConverter
		Implements IAjaxObjectConverter

        Public ReadOnly Property ClientScriptIdentifier() As String Implements IAjaxObjectConverter.ClientScriptIdentifier
            Get
                Return Nothing
            End Get
        End Property

        Public ReadOnly Property SupportedTypes() As Type() Implements IAjaxObjectConverter.SupportedTypes
            Get
                Return New Type() {GetType(DataView)}
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
            If o.[GetType]() Is GetType(DataView) OrElse o.[GetType]().BaseType Is GetType(DataView) Then
                Dim dataView As DataView = CType(o, DataView)
                sb.Append("{'Name':'" + dataView.Table.TableName + "',")
                sb.Append("'Rows':[")
                For i As Integer = 0 To dataView.Count - 1
                    DefaultConverter.ToJSON(sb, dataView(i))
                    If i < dataView.Count - 1 Then
                        sb.Append(",")
                    End If
                Next
                sb.Append("]}")
            End If
        End Sub


    End Class
End Namespace
