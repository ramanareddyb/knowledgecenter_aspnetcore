Imports System
Imports System.Reflection
Imports System.Text
Imports System.Text.RegularExpressions

Namespace Ajax.JSON
	Public Class DefaultConverter
		Friend Shared sb As StringBuilder

		Friend Shared Function ToJSON(o As Object) As String
			DefaultConverter.sb = New StringBuilder()
			DefaultConverter.ToJSON(DefaultConverter.sb, o)
			Return DefaultConverter.sb.ToString()
		End Function

		Public Shared Function FromString(s As String, t As Type) As Object
			If s Is Nothing Then
				Return Nothing
			End If
			If Utility.AjaxConverters IsNot Nothing Then
				For Each key As Type In Utility.AjaxConverters.Keys
					Dim ajaxObjectConverter As IAjaxObjectConverter = CType(Utility.AjaxConverters(key), IAjaxObjectConverter)
					Dim supportedTypes As Type() = ajaxObjectConverter.SupportedTypes
					For i As Integer = 0 To supportedTypes.Length - 1
						Dim type As Type = supportedTypes(i)
						If type Is t OrElse type Is t.BaseType Then
							Return ajaxObjectConverter.FromString(s, t)
						End If
					Next
				Next
			End If
			If t Is GetType(String) Then
				Return s
			End If
			If t Is GetType(Short) Then
				Return Convert.ToInt16(s)
			End If
			If t Is GetType(Integer) Then
				Return Convert.ToInt32(s)
			End If
			If t Is GetType(Long) Then
				Return Convert.ToInt64(s)
			End If
			If t Is GetType(Double) Then
				Return Convert.ToDouble(s)
			End If
			If t Is GetType(Boolean) Then
				Return Boolean.Parse(s.ToLower())
			End If
			If t Is GetType(Boolean()) Then
				Dim array As String() = s.Split(New Char() { ","c })
				Dim array2 As Boolean() = New Boolean(array.Length - 1) {}
				For j As Integer = 0 To array.Length - 1
					array2(j) = Boolean.Parse(array(j).ToLower())
				Next
				Return array2
			End If
			If t Is GetType(Short()) Then
				Dim array3 As String() = s.Split(New Char() { ","c })
				Dim array4 As Short() = New Short(array3.Length - 1) {}
				For k As Integer = 0 To array3.Length - 1
					array4(k) = Short.Parse(array3(k))
				Next
				Return array4
			End If
			If t Is GetType(Integer()) Then
				Dim array5 As String() = s.Split(New Char() { ","c })
				Dim array6 As Integer() = New Integer(array5.Length - 1) {}
				For l As Integer = 0 To array5.Length - 1
					array6(l) = Integer.Parse(array5(l))
				Next
				Return array6
			End If
			If t Is GetType(Long()) Then
				Dim array7 As String() = s.Split(New Char() { ","c })
				Dim array8 As Long() = New Long(array7.Length - 1) {}
				For m As Integer = 0 To array7.Length - 1
					array8(m) = Long.Parse(array7(m))
				Next
				Return array8
			End If
			If t Is GetType(String()) AndAlso s.StartsWith("[") AndAlso s.EndsWith("]") Then
				s = s.Substring(1, s.Length - 2)
				Dim regex As Regex = New Regex("  ""[^\\""]*""  |  [^,]+  ", RegexOptions.IgnorePatternWhitespace)
				Dim array9 As String() = New String(regex.Matches(s).Count - 1) {}
				Dim num As Integer = 0
				For Each match As Match In regex.Matches(s)
					Dim arg_32C_0 As String() = array9
					Dim expr_30C As Integer = num
					num = expr_30C + 1
					arg_32C_0(expr_30C) = match.Value.Substring(1, match.Value.Length - 2)
				Next
				Return array9
			End If
			Throw New NotImplementedException()
		End Function

		Public Shared Sub PropsFieldsToJSON(ByRef sb As StringBuilder, o As Object)
			DefaultConverter.PropsFieldsToJSON(sb, o, False)
		End Sub

		Public Shared Sub PropsFieldsToJSON(ByRef sb As StringBuilder, o As Object, writeSeperator As Boolean)
			Dim flag As Boolean = False
			Dim properties As PropertyInfo() = o.[GetType]().GetProperties()
			For i As Integer = 0 To properties.Length - 1
				If properties(i).CanRead Then
					If i = 0 AndAlso writeSeperator Then
						sb.Append(",")
					End If
					flag = True
					sb.Append("'" + properties(i).Name + "':")
					Try
						DefaultConverter.ToJSON(sb, properties(i).GetValue(o, New Object(-1) {}))
					Catch ex_67 As Exception
						sb.Append("null")
					End Try
					If i < properties.Length - 1 Then
						sb.Append(",")
					End If
				End If
			Next
			Dim fields As FieldInfo() = o.[GetType]().GetFields()
			For j As Integer = 0 To fields.Length - 1
				If j = 0 AndAlso (flag OrElse writeSeperator) Then
					sb.Append(",")
				End If
				sb.Append("'" + fields(j).Name + "':")
				DefaultConverter.ToJSON(sb, fields(j).GetValue(o))
				If j < fields.Length - 1 Then
					sb.Append(",")
				End If
			Next
		End Sub

		Public Shared Sub ToJSON(ByRef sb As StringBuilder, o As Object)
			If o Is Nothing Then
				sb.Append("null")
				Return
			End If
			Dim type As Type = o.[GetType]()
			If Utility.AjaxConverters IsNot Nothing Then
				For Each key As Type In Utility.AjaxConverters.Keys
					Dim ajaxObjectConverter As IAjaxObjectConverter = CType(Utility.AjaxConverters(key), IAjaxObjectConverter)
					Dim supportedTypes As Type() = ajaxObjectConverter.SupportedTypes
					For i As Integer = 0 To supportedTypes.Length - 1
						Dim type2 As Type = supportedTypes(i)
						If ajaxObjectConverter.IncludeSubclasses AndAlso (type2.IsInstanceOfType(o) OrElse type.GetInterface(type2.FullName, True) IsNot Nothing) Then
							ajaxObjectConverter.ToJSON(sb, o)
							Return
						End If
						If type2 Is type OrElse type2 Is type.BaseType Then
							ajaxObjectConverter.ToJSON(sb, o)
							Return
						End If
					Next
				Next
			End If
			If type.IsArray Then
				Dim array As Array = CType(o, Array)
				sb.Append("[")
				For j As Integer = 0 To array.Length - 1
					DefaultConverter.ToJSON(sb, array.GetValue(j))
					If j < array.Length - 1 Then
						sb.Append(",")
					End If
				Next
				sb.Append("]")
				Return
			End If
			If type Is GetType(Byte) Then
				sb.Append((CByte(o)).ToString())
				Return
			End If
			If type Is GetType(SByte) Then
				sb.Append((CSByte(o)).ToString())
				Return
			End If
			If type Is GetType(Short) OrElse type Is GetType(Integer) OrElse type Is GetType(Long) OrElse type Is GetType(UShort) OrElse type Is GetType(UInteger) OrElse type Is GetType(ULong) Then
				sb.Append(o.ToString())
				Return
			End If
			If type Is GetType(Boolean) Then
				sb.Append(o.ToString().ToLower())
				Return
			End If
			If type Is GetType(Double) OrElse type Is GetType(Decimal) OrElse type Is GetType(Single) Then
				sb.Append(o.ToString().Replace(",", "."))
				Return
			End If
			If type Is GetType(String) OrElse type Is GetType(Char) Then
				sb.Append("'" + o.ToString().Replace("\", "\\").Replace(vbCr, "\r").Replace(vbLf, "\n").Replace("'", "\'") + "'")
				Return
			End If
			If type Is GetType(Guid) Then
				sb.Append("'" + o.ToString() + "'")
				Return
			End If
			If type.IsEnum Then
				sb.Append("'" + o.ToString() + "'")
				Return
			End If
			If type.IsSerializable Then
				sb.Append("{")
				DefaultConverter.PropsFieldsToJSON(sb, o)
				sb.Append("}")
				Return
			End If
			sb.Append("'" + o.ToString().Replace("\", "\\").Replace(vbCr, "\r").Replace(vbLf, "\n").Replace("'", "\'") + "'")
		End Sub
	End Class
End Namespace
