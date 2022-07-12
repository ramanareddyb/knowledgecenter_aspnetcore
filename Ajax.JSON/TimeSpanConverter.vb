Imports System
Imports System.Text

Namespace Ajax.JSON
	Friend Class TimeSpanConverter
		Implements IAjaxObjectConverter

        Public ReadOnly Property ClientScriptIdentifier() As String Implements IAjaxObjectConverter.ClientScriptIdentifier
            Get
                Return "AjaxTimeSpan"
            End Get
        End Property

        Public ReadOnly Property SupportedTypes() As Type() Implements IAjaxObjectConverter.SupportedTypes
            Get
                Return New Type() {GetType(TimeSpan)}
            End Get
        End Property

        Public ReadOnly Property IncludeSubclasses() As Boolean Implements IAjaxObjectConverter.IncludeSubclasses
            Get
                Return True
            End Get
        End Property


        Public Sub RenderClientScript(ByRef sb As StringBuilder) Implements IAjaxObjectConverter.RenderClientScript
            sb.Append("function TimeSpan(){this.Days=0;this.Hours=0;this.Minutes=0;this.Seconds=0;this.Milliseconds=0;}" & vbCrLf)
            sb.Append("TimeSpan.prototype.toString = function(){return this.Days+'.'+this.Hours+':'+this.Minutes+':'+this.Seconds+'.'+this.Milliseconds;}" & vbCrLf)
        End Sub

        Public Function FromString(s As String, t As Type) As Object Implements IAjaxObjectConverter.FromString
            Dim timeSpan As TimeSpan = TimeSpan.Parse(s)
            Return timeSpan
        End Function

        Public Sub ToJSON(ByRef sb As StringBuilder, o As Object) Implements IAjaxObjectConverter.ToJSON
            If o.[GetType]() Is GetType(TimeSpan) OrElse o.[GetType]().BaseType Is GetType(TimeSpan) Then
                Dim timeSpan As TimeSpan = CType(o, TimeSpan)
                sb.Append("{")
                sb.Append("'TotalDays':" + DefaultConverter.ToJSON(timeSpan.TotalDays) + ",")
                sb.Append("'TotalHours':" + DefaultConverter.ToJSON(timeSpan.TotalHours) + ",")
                sb.Append("'TotalMinutes':" + DefaultConverter.ToJSON(timeSpan.TotalMinutes) + ",")
                sb.Append("'TotalSeconds':" + DefaultConverter.ToJSON(timeSpan.TotalSeconds) + ",")
                sb.Append("'TotalMilliseconds':" + DefaultConverter.ToJSON(timeSpan.TotalMilliseconds) + ",")
                sb.Append("'Ticks':" + timeSpan.Ticks + ",")
                sb.Append("'Days':" + timeSpan.Days + ",")
                sb.Append("'Hours':" + timeSpan.Hours + ",")
                sb.Append("'Minutes':" + timeSpan.Minutes + ",")
                sb.Append("'Seconds':" + timeSpan.Seconds + ",")
                sb.Append("'Milliseconds':" + timeSpan.Milliseconds + "")
                sb.Append("}")
            End If
        End Sub


    End Class
End Namespace
