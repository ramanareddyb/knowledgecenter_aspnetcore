Imports Ajax.JSON
Imports System
Imports System.Collections
Imports System.IO
Imports System.Reflection
Imports System.Text
Imports System.Web
Imports System.Web.Caching
Imports System.Xml

Namespace Ajax
	Friend Class AjaxProcessor
		Private context As HttpContext

		Friend Sub Write(text As String)
			If Me.context Is Nothing Then
				Throw New NullReferenceException("The context is not set correct in ProcessRequest().")
			End If
			Me.context.Response.Write(text)
		End Sub

		Friend Sub WriteLine(line As String)
			Me.Write(line)
			Me.Write(vbCrLf)
		End Sub

		Friend Sub InitializeContext(ByRef context As HttpContext)
			Me.context = context
		End Sub

		Friend Function GetMethodName() As String
			If Me.context IsNot Nothing AndAlso Me.context.Request("_method") IsNot Nothing Then
				Return Me.context.Request("_method")
			End If
			Return Nothing
		End Function

		Friend Sub RetreiveParameters(ByRef sr As StreamReader, para As ParameterInfo(), ByRef po As Object())
			For i As Integer = 0 To para.Length - 1
				po(i) = para(i).DefaultValue
			Next
			If Me.context.Request("_return") IsNot Nothing AndAlso Me.context.Request("_return") = "xml" Then
				For j As Integer = 0 To para.Length - 1
					If Me.context.Request(para(j).Name) IsNot Nothing Then
						po(j) = DefaultConverter.FromString(HttpUtility.UrlDecode(Me.context.Request(para(j).Name)), para(j).ParameterType)
					End If
				Next
				Return
			End If
			Dim hashtable As Hashtable = New Hashtable()
			Try
				Dim text As String = sr.ReadLine()
				Dim text2 As String = Nothing
				While text IsNot Nothing
					text = text.Replace("%26", "%").Replace("%3D", "=")
					Dim text3 As String = Nothing
					For k As Integer = 0 To para.Length - 1
						If text.StartsWith(para(k).Name + "=") Then
							text3 = para(k).Name
							Exit For
						End If
					Next
					If text3 IsNot Nothing Then
						Dim value As String = text.Substring(text3.Length + 1)
						hashtable.Add(text3, value)
						text2 = text3
					Else If text2 IsNot Nothing Then
						hashtable(text2) = hashtable(text2) + vbCrLf + text
					End If
					text = sr.ReadLine()
				End While
			Catch ex_170 As Exception
			Finally
				sr.Close()
			End Try
            For l As Integer = 0 To para.Length - 1
                If hashtable(para(l).Name) IsNot Nothing Then
                    po(l) = DefaultConverter.FromString(hashtable(para(l).Name).ToString(), para(l).ParameterType)
                End If
            Next
            hashtable=Nothing
        End Sub

		Friend Sub RenderCommonScript()
            Dim str As String = "ajax.js"
            Try
                If Me.context.Request.UserAgent.IndexOf("Windows CE; PPC;") >= 0 Then
                    str = "ajax_mobile.js"
                End If
                'Dim streamReader As StreamReader = New StreamReader(Assembly.GetCallingAssembly().GetManifestResourceStream("Ajax." + str))
                Dim streamReader As StreamReader = New StreamReader(Assembly.GetCallingAssembly().GetManifestResourceStream(str))
                Me.Write(streamReader.ReadToEnd())
                streamReader.Close()
                Me.Write("var ajaxVersion = '5.7.22.2';")
            Catch ex As Exception
            End Try

        End Sub

		Public Sub RenderClientScript(mi As MethodInfo(), type As Type)
			If Me.context.Cache(type.AssemblyQualifiedName) IsNot Nothing Then
				Me.WriteLine(vbCrLf & "// cached javascript")
				Me.Write(CStr(Me.context.Cache(type.AssemblyQualifiedName)))
				Return
			End If
            Dim stringBuilder As StringBuilder = New StringBuilder()
            Try
                stringBuilder.Append("var " + type.Name + " = {" & vbCrLf)
                For i As Integer = 0 To mi.Length - 1
                    Dim methodInfo As MethodInfo = mi(i)
                    If methodInfo.GetCustomAttributes(GetType(AjaxMethodAttribute), True).Length <> 0 Then
                        Dim customAttributes As Object() = methodInfo.GetCustomAttributes(GetType(AjaxMethodAttribute), True)
                        Dim ajaxMethodAttribute As AjaxMethodAttribute = CType(customAttributes(0), AjaxMethodAttribute)
                        Dim parameters As ParameterInfo() = methodInfo.GetParameters()
                        stringBuilder.Append((If((ajaxMethodAttribute.MethodName IsNot Nothing), ajaxMethodAttribute.MethodName.Replace(" ", "_"), methodInfo.Name)) + ":function(")
                        For j As Integer = 0 To parameters.Length - 1
                            stringBuilder.Append(parameters(j).Name)
                            stringBuilder.Append(",")
                        Next
                        stringBuilder.Append("callback,context)")
                        stringBuilder.Append("{")
                        stringBuilder.Append("return new ajax_request(")
                        If ajaxMethodAttribute.HttpConnectionProtocol = HttpConnectionProtocolType.HTTP Then
                            stringBuilder.Append("'http://" + Me.context.Request.ServerVariables("SERVER_NAME") + "' + ")
                        ElseIf ajaxMethodAttribute.HttpConnectionProtocol = HttpConnectionProtocolType.HTTPS Then
                            stringBuilder.Append("'https://" + Me.context.Request.ServerVariables("SERVER_NAME") + "' + ")
                        End If
                        stringBuilder.Append("this.url + '?_method=" + methodInfo.Name)
                        If ajaxMethodAttribute.RequireSessionState = HttpSessionStateRequirement.ReadWrite Then
                            stringBuilder.Append("&_session=rw")
                        ElseIf ajaxMethodAttribute.RequireSessionState = HttpSessionStateRequirement.Read Then
                            stringBuilder.Append("&_session=r")
                        Else
                            stringBuilder.Append("&_session=no")
                        End If
                        stringBuilder.Append("','")
                        For k As Integer = 0 To parameters.Length - 1
                            If parameters(k).ParameterType Is GetType(String()) Then
                                stringBuilder.Append(parameters(k).Name + "=' + json_from_object(" + parameters(k).Name + ")")
                            Else
                                stringBuilder.Append(parameters(k).Name + "=' + enc(" + parameters(k).Name + ")")
                            End If
                            If k < parameters.Length - 1 Then
                                stringBuilder.Append("+ '\r\n")
                            End If
                        Next
                        If parameters.Length = 0 Then
                            stringBuilder.Append("'")
                        End If
                        stringBuilder.Append(",callback, context);")
                        stringBuilder.Append("}," & vbCrLf)
                    End If
                Next
                Dim text As String = type.FullName + "," + type.Assembly.FullName.Substring(0, type.Assembly.FullName.IndexOf(","))
                If Utility.Settings IsNot Nothing AndAlso Utility.Settings.UrlNamespaceMappings.ContainsValue(text) Then
                    For Each text2 As String In Utility.Settings.UrlNamespaceMappings.Keys
                        If Utility.Settings.UrlNamespaceMappings(text2).ToString() = text Then
                            text = text2
                            Exit For
                        End If
                    Next
                End If
                stringBuilder.Append(String.Concat(New String() {"url:'", Me.context.Request.ApplicationPath, If(Me.context.Request.ApplicationPath.EndsWith("/"), "", "/"), Utility.HandlerPath, "/", If((Me.context.Session IsNot Nothing AndAlso Me.context.Session.IsCookieless), ("(" + Me.context.Session.SessionID + ")/"), ""), text, Utility.HandlerExtension, "'" & vbCrLf}))
                stringBuilder.Append("}" & vbCrLf)
                Dim dependencies As CacheDependency = New CacheDependency(type.Assembly.Location)
                Me.context.Cache.Add(type.AssemblyQualifiedName, stringBuilder.ToString(), dependencies, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.Normal, Nothing)
                Me.Write(stringBuilder.ToString())
                dependencies = Nothing
            Catch ex As Exception

            Finally
                stringBuilder = Nothing
            End Try

        End Sub

		Friend Sub HandleException(ex As Exception, message As String)
			Me.context.Trace.Warn("Ajax.NET", "HandleException", ex)
			Dim text As String = String.Concat(New String() { "new Object();r.error = new ajax_error('", ex.[GetType]().FullName, "','", ex.Message.Replace("'", "\'").Replace(vbCr, "\r").Replace(vbLf, "\n"), If((message IsNot Nothing), ("\r\n" + message), ""), "',0)" })
			If Me.context.Request("_return") IsNot Nothing AndAlso Me.context.Request("_return") = "xml" Then
				Dim xmlDocument As XmlDocument = New XmlDocument()
				xmlDocument.LoadXml("<Ajax/>")
				xmlDocument.DocumentElement.InnerText = text
				xmlDocument.Save(Me.context.Response.OutputStream)
				Return
			End If
			Me.context.Response.Write(text)
		End Sub
	End Class
End Namespace
