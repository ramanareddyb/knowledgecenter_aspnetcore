Imports System
Imports System.Configuration
Imports System.Xml

Namespace Ajax
	Friend Class AjaxSettingsSectionHandler
		Implements IConfigurationSectionHandler

		Friend Sub New()
		End Sub

        Public Function Create(parent As Object, configContext As Object, section As XmlNode) As Object Implements IConfigurationSectionHandler.Create
            Dim ajaxSettings As AjaxSettings = New AjaxSettings()
            For Each xmlNode As XmlNode In section.ChildNodes
                If xmlNode.Name = "commonAjax" Then
                    If xmlNode.SelectSingleNode("@enabled") IsNot Nothing AndAlso xmlNode.SelectSingleNode("@enabled").InnerText = "true" Then
                        If xmlNode.SelectSingleNode("@path") IsNot Nothing AndAlso xmlNode.SelectSingleNode("@path").InnerText <> "" Then
                            ajaxSettings.CommonScript = xmlNode.SelectSingleNode("@path").InnerText
                        End If
                        If xmlNode.SelectSingleNode("@language") IsNot Nothing AndAlso xmlNode.SelectSingleNode("@language").InnerText <> "" Then
                            ajaxSettings.ScriptLanguage = xmlNode.SelectSingleNode("@language").InnerText
                        End If
                    End If
                ElseIf xmlNode.Name = "temporaryFiles" Then
                    Dim text As String = Nothing
                    Dim num As Integer = -1
                    If xmlNode.SelectSingleNode("@path") IsNot Nothing AndAlso xmlNode.SelectSingleNode("@path").InnerText <> "" Then
                        text = xmlNode.SelectSingleNode("@path").InnerText
                    End If
                    If xmlNode.SelectSingleNode("@deleteAfter") IsNot Nothing AndAlso xmlNode.SelectSingleNode("@deleteAfter").InnerText <> "" Then
                        Try
                            num = Integer.Parse(xmlNode.SelectSingleNode("@deleteAfter").InnerText)
                        Catch ex_186 As SystemException
                        End Try
                    End If
                    If text IsNot Nothing OrElse num >= 0 Then
                        If text IsNot Nothing Then
                            ajaxSettings.TemporaryFiles.Path = text
                        End If
                        If num >= 0 Then
                            ajaxSettings.TemporaryFiles.DeleteAfter = num
                        End If
                    End If
                ElseIf xmlNode.Name = "urlNamespaceMappings" Then
                    For Each xmlNode2 As XmlNode In xmlNode.SelectNodes("add")
                        Dim xmlNode3 As XmlNode = xmlNode2.SelectSingleNode("@namespace")
                        Dim xmlNode4 As XmlNode = xmlNode2.SelectSingleNode("@path")
                        If xmlNode3 IsNot Nothing AndAlso Not (xmlNode3.InnerText = "") AndAlso xmlNode4 IsNot Nothing AndAlso Not (xmlNode4.InnerText = "") Then
                            ajaxSettings.UrlNamespaceMappings.Add(xmlNode4.InnerText, xmlNode3.InnerText)
                        End If
                    Next
                End If
            Next
            Return ajaxSettings
        End Function


    End Class
End Namespace
