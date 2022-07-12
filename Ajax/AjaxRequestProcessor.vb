Imports Ajax.JSON
Imports MS.Utilities
Imports System
Imports System.Collections.Specialized
Imports System.IO
Imports System.Reflection
Imports System.Text
Imports System.Web
Imports System.Web.Caching
Imports System.Xml

Namespace Ajax
	Friend Class AjaxRequestProcessor
		Private context As HttpContext

		Friend Sub New(context As HttpContext)
			Me.context = context
		End Sub

		Public Sub Run()
			If Me.context.Trace.IsEnabled Then
				Me.context.Trace.Write("Ajax.NET", "Begin ProcessRequest")
			End If
			Dim now As DateTime = DateTime.Now
			Dim text As String = Path.GetFileNameWithoutExtension(Me.context.Request.FilePath)
			If Utility.Settings IsNot Nothing AndAlso Utility.Settings.UrlNamespaceMappings.Contains(text) Then
				text = Utility.Settings.UrlNamespaceMappings(text).ToString()
			End If
			Dim ajaxProcessor As AjaxProcessor = New AjaxProcessor()
			ajaxProcessor.InitializeContext(Me.context)
			If text.ToLower() = "common" Then
				If Me.context.Trace.IsEnabled Then
					Me.context.Trace.Write("Ajax.NET", "Render common Javascript")
				End If
				Me.context.Response.Expires = 1
				Me.context.Response.ContentType = "text/plain"
				ajaxProcessor.RenderCommonScript()
				If Me.context.Trace.IsEnabled Then
					Me.context.Trace.Write("Ajax.NET", "End ProcessRequest")
				End If
				Return
			End If
			Me.context.Response.Expires = 0
			Me.context.Response.AddHeader("cache-control", "no-cache")
			Dim text2 As String = Nothing
			Dim array As Object() = New Object(-1) {}
			Dim type As Type = Type.[GetType](text)
			Dim methods As MethodInfo() = type.GetMethods()
			text2 = ajaxProcessor.GetMethodName()
			Dim text3 As String = Nothing
			Dim streamReader As StreamReader = Nothing
			Dim array2 As Byte() = New Byte(Me.context.Request.ContentLength - 1) {}
			If Me.context.Request.HttpMethod = "POST" AndAlso Me.context.Request.InputStream.Read(array2, 0, array2.Length) >= 0 Then
				text3 = MD5Helper.GetHash(array2)
				streamReader = New StreamReader(New MemoryStream(array2), Encoding.UTF8)
				If Me.context.Cache(type.FullName + "|" + text3) IsNot Nothing Then
					Me.context.Response.Write(Me.context.Cache(type.FullName + "|" + text3).ToString())
					If Me.context.Trace.IsEnabled Then
						Me.context.Trace.Write("Ajax.NET", "End ProcessRequest")
					End If
					Return
				End If
			End If
			If text2 IsNot Nothing Then
				Dim method As MethodInfo = type.GetMethod(text2)
				If method IsNot Nothing Then
					Dim customAttributes As Object() = method.GetCustomAttributes(GetType(AjaxMethodAttribute), True)
					If customAttributes.Length <> 1 Then
						ajaxProcessor.HandleException(New NotImplementedException("The method '" + Me.context.Request("m") + "' is not implemented or access refused."), Nothing)
						If Me.context.Trace.IsEnabled Then
							Me.context.Trace.Write("Ajax.NET", "End ProcessRequest")
						End If
						Return
					End If
					Dim parameters As ParameterInfo() = method.GetParameters()
					array = New Object(parameters.Length - 1) {}
					Try
						ajaxProcessor.RetreiveParameters(streamReader, parameters, array)
					Catch ex As Exception
						ajaxProcessor.HandleException(ex, "Could not retreive parameters from HTTP request.")
					End Try
					Dim obj As Object = Nothing
                    Try
                        If Me.context.Trace.IsEnabled Then
                            Me.context.Trace.Write("Ajax.NET", "Invoking " + type.FullName + "." + method.Name)
                        End If
                        If method.IsStatic Then
                            Try
                                obj = type.InvokeMember(text2, BindingFlags.IgnoreCase Or BindingFlags.[Static] Or BindingFlags.[Public] Or BindingFlags.InvokeMethod, Nothing, Nothing, array)
                                GoTo IL_46F
                            Catch ex2 As Exception
                                If ex2.InnerException IsNot Nothing Then
                                    ajaxProcessor.HandleException(ex2.InnerException, Nothing)
                                Else
                                    ajaxProcessor.HandleException(ex2, Nothing)
                                End If
                                If Me.context.Trace.IsEnabled Then
                                    Me.context.Trace.Write("Ajax.NET", "End ProcessRequest")
                                End If
                                Return
                            Finally

                            End Try

                        End If
                        Try
                            Dim obj2 As Object = Activator.CreateInstance(Type.[GetType](text), New Object(-1) {})
                            If obj2 IsNot Nothing Then
                                obj = method.Invoke(obj2, array)
                            End If
                        Catch ex3 As Exception
                            If ex3.InnerException IsNot Nothing Then
                                ajaxProcessor.HandleException(ex3.InnerException, Nothing)
                            Else
                                ajaxProcessor.HandleException(ex3, Nothing)
                            End If
                            If Me.context.Trace.IsEnabled Then
                                Me.context.Trace.Write("Ajax.NET", "End ProcessRequest")
                            End If
                            Return
                        Finally
                            array = Nothing
                        End Try
IL_46F:
                    Catch ex4 As Exception
                        If ex4.InnerException IsNot Nothing Then
                            ajaxProcessor.HandleException(ex4.InnerException, Nothing)
                        Else
                            ajaxProcessor.HandleException(ex4, Nothing)
                        End If
                        If Me.context.Trace.IsEnabled Then
                            Me.context.Trace.Write("Ajax.NET", "End ProcessRequest")
                        End If
                        Return
                    Finally

                    End Try
					If Not Utility.ConverterRegistered Then
						Utility.RegisterConverterForAjax(Nothing)
					End If
                    Try
                        If obj IsNot Nothing AndAlso obj.[GetType]() Is GetType(XmlDocument) Then
                            Me.context.Response.ContentType = "text/xml"
                            CType(obj, XmlDocument).Save(Me.context.Response.OutputStream)
                            If Me.context.Trace.IsEnabled Then
                                Me.context.Trace.Write("Ajax.NET", "End ProcessRequest")
                            End If
                            Return
                        End If
                        Dim stringBuilder As StringBuilder = New StringBuilder()
                        Try
                            DefaultConverter.ToJSON(stringBuilder, obj)
                        Catch ex5 As StackOverflowException
                            ajaxProcessor.HandleException(ex5, "The class you are returning is not supported.")
                            Return
                        Catch ex6 As Exception
                            ajaxProcessor.HandleException(ex6, "AjaxRequestProcessor throw exception while running ToJSON")
                            Return
                        End Try
                        If Me.context.Request("_return") IsNot Nothing AndAlso Me.context.Request("_return") = "xml" Then
                            Dim xmlDocument As XmlDocument = New XmlDocument()
                            xmlDocument.LoadXml("<Ajax/>")
                            xmlDocument.DocumentElement.InnerText = stringBuilder.ToString()
                            xmlDocument.Save(Me.context.Response.OutputStream)
                        Else
                            If text3 IsNot Nothing Then
                                Dim customAttributes2 As Object() = method.GetCustomAttributes(GetType(AjaxMethodAttribute), True)
                                If customAttributes2.Length <> 0 Then
                                    Dim ajaxMethodAttribute As AjaxMethodAttribute = CType(customAttributes2(0), AjaxMethodAttribute)
                                    If ajaxMethodAttribute.IsCacheEnabled Then
                                        Me.context.Cache.Add(type.FullName + "|" + text3, stringBuilder.ToString(), Nothing, now.AddSeconds(ajaxMethodAttribute.CacheDuration.TotalSeconds), Cache.NoSlidingExpiration, CacheItemPriority.Normal, Nothing)
                                    End If
                                End If
                            End If
                            If Me.context.Trace.IsEnabled Then
                                Me.context.Trace.Write("Ajax.NET", "JSON string: " + stringBuilder.ToString())
                            End If
                            Me.context.Response.Write(stringBuilder.ToString())
                            stringBuilder = Nothing
                        End If
                        If Me.context.Trace.IsEnabled Then
                            Me.context.Trace.Write("Ajax.NET", "End ProcessRequest")
                        End If
                        Return
                    Catch ex7 As Exception
                        ajaxProcessor.HandleException(ex7, "Error while converting object to JSON.")
                        If Me.context.Trace.IsEnabled Then
                            Me.context.Trace.Write("Ajax.NET", "End ProcessRequest")
                        End If
                        Return
                    Finally

                    End Try
				End If
				ajaxProcessor.HandleException(New NotImplementedException("The method '" + Me.context.Request("m") + "' is not implemented or access refused."), Nothing)
				If Me.context.Trace.IsEnabled Then
					Me.context.Trace.Write("Ajax.NET", "End ProcessRequest")
				End If
				Return
			End If
			If Me.context.Trace.IsEnabled Then
				Me.context.Trace.Write("Ajax.NET", "Render class proxy Javascript")
			End If
			Me.context.Response.ContentType = "text/plain"
            ajaxProcessor.RenderClientScript(methods, type)

            Dim stringBuilder2 As StringBuilder = New StringBuilder()
            Dim stringCollection As StringCollection = New StringCollection()
            ''Try

            If Utility.AjaxConverters IsNot Nothing Then
            For Each key As Type In Utility.AjaxConverters.Keys
                        Dim ajaxObjectConverter As IAjaxObjectConverter = CType(Utility.AjaxConverters(key), IAjaxObjectConverter)
                        If ajaxObjectConverter.ClientScriptIdentifier Is Nothing OrElse Not StringCollection.Contains(ajaxObjectConverter.ClientScriptIdentifier) Then
                            ajaxObjectConverter.RenderClientScript(stringBuilder2)
                            StringCollection.Add(ajaxObjectConverter.ClientScriptIdentifier)
                        End If
                    Next
            End If

            If stringBuilder2.Length > 0 Then
                    Me.context.Response.Write(stringBuilder2.ToString())
                End If
                If Me.context.Trace.IsEnabled Then
                    Me.context.Trace.Write("Ajax.NET", "End ProcessRequest")
                End If
            ''Catch ex As Exception
            ''Finally
            stringBuilder2 = Nothing
                stringCollection = Nothing
            ''End Try
        End Sub
	End Class
End Namespace
