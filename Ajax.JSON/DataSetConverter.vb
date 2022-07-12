Imports System
Imports System.Data
Imports System.Text

Namespace Ajax.JSON
	Friend Class DataSetConverter
		Implements IAjaxObjectConverter

        Public ReadOnly Property ClientScriptIdentifier() As String Implements IAjaxObjectConverter.ClientScriptIdentifier
            Get
                Return "AjaxDataSet"
            End Get
        End Property

        Public ReadOnly Property SupportedTypes() As Type() Implements IAjaxObjectConverter.SupportedTypes
            Get
                Return New Type() {GetType(DataSet)}
            End Get
        End Property

        Public ReadOnly Property IncludeSubclasses() As Boolean Implements IAjaxObjectConverter.IncludeSubclasses
            Get
                Return True
            End Get
        End Property



        Public Sub RenderClientScript(ByRef sb As StringBuilder) Implements IAjaxObjectConverter.RenderClientScript
            sb.Append("function _getTable(n,e){for(var i=0; i<e.Tables.length; i++){if(e.Tables[i].Name == n)return e.Tables[i];}return null;}" & vbCrLf)
        End Sub

        Public Function FromString(s As String, t As Type) As Object Implements IAjaxObjectConverter.FromString
            Throw New NotImplementedException()
        End Function

        Public Sub ToJSON(ByRef sb As StringBuilder, o As Object) Implements IAjaxObjectConverter.ToJSON
            If o.[GetType]() Is GetType(DataSet) OrElse o.[GetType]().BaseType Is GetType(DataSet) Then
                Dim dataSet As DataSet = CType(o, DataSet)
                sb.Append("{'Tables':[")
                For i As Integer = 0 To dataSet.Tables.Count - 1
                    DefaultConverter.ToJSON(sb, dataSet.Tables(i))
                    If i < dataSet.Tables.Count - 1 Then
                        sb.Append(",")
                    End If
                Next
                sb.Append("]")
                sb.Append(",'getTable':function(n){return _getTable(n,this);}")
                sb.Append("}")
            End If
        End Sub


    End Class
End Namespace
