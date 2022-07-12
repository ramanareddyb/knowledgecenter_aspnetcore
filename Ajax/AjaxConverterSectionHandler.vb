Imports System
Imports System.Configuration
Imports System.Xml

Namespace Ajax
	Friend Class AjaxConverterSectionHandler
		Implements IConfigurationSectionHandler

		Friend Sub New()
		End Sub

        Public Function Create(parent As Object, configContext As Object, section As XmlNode) As Object Implements IConfigurationSectionHandler.Create
            Dim ajaxConverterConfiguration As AjaxConverterConfiguration = New AjaxConverterConfiguration()
            For Each xmlNode As XmlNode In section.ChildNodes
                If xmlNode.SelectSingleNode("@type") IsNot Nothing Then
                    Dim type As Type = Type.[GetType](xmlNode.SelectSingleNode("@type").InnerText)
                    If type IsNot Nothing Then
                        If xmlNode.Name = "add" Then
                            ajaxConverterConfiguration.Add(New AjaxConverterItem(type, AjaxConverterConfigurationAction.Add))
                        ElseIf xmlNode.Name = "remove" Then
                            ajaxConverterConfiguration.Add(New AjaxConverterItem(type, AjaxConverterConfigurationAction.Remove))
                        End If
                    End If
                End If
            Next
            Return ajaxConverterConfiguration
        End Function


    End Class
End Namespace
