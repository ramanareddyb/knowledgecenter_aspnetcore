Imports System
Imports System.Text

Namespace Ajax.JSON
	Friend Class DateTimeConverter
		Implements IAjaxObjectConverter

        Public ReadOnly Property ClientScriptIdentifier() As String Implements IAjaxObjectConverter.ClientScriptIdentifier
            Get
                Return "AjaxDateTime"
            End Get
        End Property

        Public ReadOnly Property SupportedTypes() As Type() Implements IAjaxObjectConverter.SupportedTypes
            Get
                Return New Type() {GetType(DateTime)}
            End Get
        End Property

        Public ReadOnly Property IncludeSubclasses() As Boolean Implements IAjaxObjectConverter.IncludeSubclasses
            Get
                Return True
            End Get
        End Property


        Public Sub RenderClientScript(ByRef sb As StringBuilder) Implements IAjaxObjectConverter.RenderClientScript
            sb.Append("function digi(v, c){v = v + """";var n = ""0000"";if(v.length < c) return n.substr(0, c-v.length) + v;return v;}" & vbCrLf & "function DateTime(year,month,day,hours,minutes,seconds){if(year>9999||year<1970||month<1||month>12||day<0||day>31||hours<0||hours>23||minutes<0||minutes>59||seconds<0||seconds>59)throw(""ArgumentException"");this.Year = year;this.Month = month;this.Day = day;this.Hours = hours;this.Minutes = minutes;this.Seconds = seconds;}" & vbCrLf & "DateTime.prototype.toString = function(){return digi(this.Year,4) + digi(this.Month,2) + digi(this.Day,2) + digi(this.Hours,2) + digi(this.Minutes,2) + digi(this.Seconds,2);}" & vbCrLf)
        End Sub

        Public Function FromString(s As String, t As Type) As Object Implements IAjaxObjectConverter.FromString
            If s Is Nothing OrElse s = "" Then
                Return DateTime.Now
            End If
            If s.Length = 8 Then
                Return New DateTime(Integer.Parse(s.Substring(0, 4)), Integer.Parse(s.Substring(4, 2)), Integer.Parse(s.Substring(6, 2)))
            End If
            If s.Length = 14 Then
                Return New DateTime(Integer.Parse(s.Substring(0, 4)), Integer.Parse(s.Substring(4, 2)), Integer.Parse(s.Substring(6, 2)), Integer.Parse(s.Substring(8, 2)), Integer.Parse(s.Substring(10, 2)), Integer.Parse(s.Substring(12, 2)))
            End If
            Throw New NotImplementedException()
        End Function

        Public Sub ToJSON(ByRef sb As StringBuilder, o As Object) Implements IAjaxObjectConverter.ToJSON
            If o.[GetType]() Is GetType(DateTime) OrElse o.[GetType]().BaseType Is GetType(DateTime) Then
                Dim dateTime As DateTime = CType(o, DateTime)
                sb.Append(String.Concat(New Object() {"new Date(", dateTime.Year, ",", dateTime.Month - 1, ",", dateTime.Day, ",", dateTime.Hour, ",", dateTime.Minute, ",", dateTime.Second, ")"}))
            End If
        End Sub

    End Class
End Namespace
