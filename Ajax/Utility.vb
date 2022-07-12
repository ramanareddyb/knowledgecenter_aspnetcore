Imports Ajax.JSON
Imports Ajax.JSON.HtmlControls
Imports System
Imports System.Collections
Imports System.Configuration
Imports System.IO
Imports System.Web
Imports System.Web.UI

Namespace Ajax
	Public Class Utility
		Private Shared m_Settings As AjaxSettings = Nothing

		Public Shared HandlerExtension As String = ".ashx"

		Public Shared HandlerPath As String = "ajax"

		Friend Shared ConverterRegistered As Boolean = False

		Friend Shared AjaxConverters As Hashtable = Nothing

		Friend Shared ReadOnly Property Settings() As AjaxSettings
			Get
				If Utility.m_Settings IsNot Nothing Then
					Return Utility.m_Settings
				End If
                Try
                    Utility.m_Settings = CType(ConfigurationSettings.GetConfig("ajaxNet/ajaxSettings"), AjaxSettings)
                    Return Utility.m_Settings
                Catch ex_29 As SystemException
                    Utility.m_Settings = New AjaxSettings()
				End Try
				Return Nothing
			End Get
		End Property

		Friend Shared Sub RegisterCommonAjax()
			Utility.RegisterCommonAjax(CType(HttpContext.Current.Handler, Page))
		End Sub

		Friend Shared Sub RegisterCommonAjax(page As Page)
			If Not Utility.ConverterRegistered Then
				Utility.RegisterConverterForAjax(Nothing)
			End If
			If page Is Nothing Then
				Return
			End If
			If Utility.Settings IsNot Nothing AndAlso Utility.Settings.CommonScript IsNot Nothing Then
				page.RegisterClientScriptBlock("AJAX.common", String.Concat(New String() { "<script type=""text/", Utility.Settings.ScriptLanguage, """ src=""", Path.GetDirectoryName(Utility.Settings.CommonScript), "/", If((HttpContext.Current.Session IsNot Nothing AndAlso HttpContext.Current.Session.IsCookieless), ("(" + HttpContext.Current.Session.SessionID + ")/"), ""), Path.GetFileName(Utility.Settings.CommonScript), """></script>" }))
				Return
			End If
			page.RegisterClientScriptBlock("AJAX.common", String.Concat(New String() { "<script type=""text/javascript"" src=""", HttpContext.Current.Request.ApplicationPath, If(HttpContext.Current.Request.ApplicationPath.EndsWith("/"), "", "/"), Utility.HandlerPath, "/", If((HttpContext.Current.Session IsNot Nothing AndAlso HttpContext.Current.Session.IsCookieless), ("(" + HttpContext.Current.Session.SessionID + ")/"), ""), "common", Utility.HandlerExtension, """></script>" }))
		End Sub

		Public Shared Sub RegisterTypeForAjax(t As Type)
			Dim page As Page = CType(HttpContext.Current.Handler, Page)
			Utility.RegisterTypeForAjax(t, page)
		End Sub

		Public Shared Sub RegisterTypeForAjax(t As Type, page As Page)
			Utility.RegisterCommonAjax(page)
			Dim text As String = t.FullName + "," + t.Assembly.FullName.Substring(0, t.Assembly.FullName.IndexOf(","))
			If Utility.Settings IsNot Nothing AndAlso Utility.Settings.UrlNamespaceMappings.ContainsValue(text) Then
				For Each text2 As String In Utility.Settings.UrlNamespaceMappings.Keys
					If Utility.Settings.UrlNamespaceMappings(text2).ToString() = text Then
						text = text2
						Exit For
					End If
				Next
			End If
			page.RegisterClientScriptBlock("AJAX." + t.FullName, String.Concat(New String() { "<script type=""text/javascript"" src=""", HttpContext.Current.Request.ApplicationPath, If(HttpContext.Current.Request.ApplicationPath.EndsWith("/"), "", "/"), Utility.HandlerPath, "/", If((HttpContext.Current.Session IsNot Nothing AndAlso HttpContext.Current.Session.IsCookieless), ("(" + HttpContext.Current.Session.SessionID + ")/"), ""), text, Utility.HandlerExtension, """></script>" }))
		End Sub

		Public Shared Sub RegisterConverterForAjax(converter As IAjaxObjectConverter)
			Utility.RegisterConverterForAjax(converter, True)
		End Sub

		Friend Shared Sub RegisterConverterForAjax(converter As IAjaxObjectConverter, overrideExisting As Boolean)
			If Utility.AjaxConverters Is Nothing OrElse converter Is Nothing Then
				HttpContext.Current.Trace.Write("Ajax.NET", "Begin Register Converter for Ajax.NET")
				Utility.AjaxConverters = New Hashtable()
				Utility.RegisterConverterForAjax(New ImageConverter())
				Utility.RegisterConverterForAjax(New DataRowConverter())
				Utility.RegisterConverterForAjax(New DataSetConverter())
				Utility.RegisterConverterForAjax(New DataTableConverter())
				Utility.RegisterConverterForAjax(New DateTimeConverter())
				Utility.RegisterConverterForAjax(New TimeSpanConverter())
				Utility.RegisterConverterForAjax(New ArrayListConverter())
				Utility.RegisterConverterForAjax(New ICollectionConverter())
				Utility.RegisterConverterForAjax(New DataViewConverter())
				Utility.RegisterConverterForAjax(New DataRowViewConverter())
				Utility.RegisterConverterForAjax(New HtmlAnchorConverter())
				Utility.RegisterConverterForAjax(New HtmlButtonConverter())
				Utility.RegisterConverterForAjax(New HtmlImageConverter())
				Utility.RegisterConverterForAjax(New HtmlInputButtonConverter())
				Utility.RegisterConverterForAjax(New HtmlInputCheckBoxConverter())
				Utility.RegisterConverterForAjax(New HtmlInputRadioButtonConverter())
				Utility.RegisterConverterForAjax(New HtmlInputTextConverter())
				Utility.RegisterConverterForAjax(New HtmlSelectConverter())
				Utility.RegisterConverterForAjax(New HtmlTableCellConverter())
				Utility.RegisterConverterForAjax(New HtmlTableConverter())
				Utility.RegisterConverterForAjax(New HtmlTableRowConverter())
				Utility.RegisterConverterForAjax(New HtmlTextAreaConverter())
				Utility.RegisterConverterForAjax(New HtmlControlConverter())
				Dim ajaxConverterConfiguration As AjaxConverterConfiguration = Nothing
                Try
                    ajaxConverterConfiguration = CType(ConfigurationSettings.GetConfig("ajaxNet/ajaxConverters"), AjaxConverterConfiguration)
                Catch ex_12A As SystemException
                End Try
				If ajaxConverterConfiguration IsNot Nothing Then
					For Each ajaxConverterItem As AjaxConverterItem In ajaxConverterConfiguration
						If ajaxConverterItem.Action = AjaxConverterConfigurationAction.Add Then
							Dim ajaxObjectConverter As IAjaxObjectConverter = Nothing
                            Try
                                Dim obj As Object = Activator.CreateInstance(ajaxConverterItem.ConverterType, New Object(-1) {})
                                ajaxObjectConverter = CType(obj, IAjaxObjectConverter)
                            Catch ex_16F As SystemException
                                ajaxObjectConverter = Nothing
							End Try
							If ajaxObjectConverter IsNot Nothing Then
								Utility.RegisterConverterForAjax(ajaxObjectConverter)
							End If
						Else If Utility.AjaxConverters.Contains(ajaxConverterItem.ConverterType) Then
							Utility.AjaxConverters.Remove(ajaxConverterItem.ConverterType)
						End If
					Next
				End If
				HttpContext.Current.Trace.Write("Ajax.NET", "End Register Converter for Ajax.NET")
			End If
			If converter IsNot Nothing Then
				Utility.AjaxConverters.Contains(converter.[GetType]())
				If Utility.AjaxConverters.Contains(converter.[GetType]()) Then
					If overrideExisting Then
						Utility.AjaxConverters(converter.[GetType]()) = converter
					End If
				Else
					Utility.AjaxConverters.Add(converter.[GetType](), converter)
				End If
			End If
			Utility.ConverterRegistered = True
		End Sub
	End Class
End Namespace
