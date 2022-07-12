Imports System
Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Web.UI
Imports System.Web.UI.HtmlControls

Namespace Ajax
	Friend Class AjaxHtmlControlConverter
		Friend Shared Function CorrectAttributes(input As String) As String
			Dim pattern As String = "selected=""selected"""
			Dim regex As Regex = New Regex(pattern, RegexOptions.IgnoreCase Or RegexOptions.Singleline)
			input = regex.Replace(input, "selected=""true""")
			pattern = "multiple=""multiple"""
			regex = New Regex(pattern, RegexOptions.IgnoreCase Or RegexOptions.Singleline)
			input = regex.Replace(input, "multiple=""true""")
			pattern = "disabled=""disabled"""
			regex = New Regex(pattern, RegexOptions.IgnoreCase Or RegexOptions.Singleline)
			input = regex.Replace(input, "disabled=""true""")
			Return input
		End Function

		Friend Shared Function ControlToString(control As Control) As String
			Dim stringWriter As StringWriter = New StringWriter(New StringBuilder())
			control.RenderControl(New HtmlTextWriter(stringWriter))
			Return stringWriter.ToString()
		End Function

		Friend Shared Function HtmlToHtmlControl(html As String, htmlControlType As Type) As HtmlControl
			Dim obj As Object = Activator.CreateInstance(htmlControlType)
			If TypeOf obj Is HtmlControl Then
				html = AjaxHtmlControlConverter.AddRunAtServer(html, (TryCast(obj, HtmlControl)).TagName)
				If TypeOf obj Is HtmlSelect Then
					html = AjaxHtmlControlConverter.CorrectAttributes(html)
				End If
				Dim templateControl As TemplateControl = New UserControl()
				Dim control As Control = templateControl.ParseControl(html)
				Dim result As HtmlControl = Nothing
				If control.[GetType]() Is htmlControlType Then
					result = (TryCast(control, HtmlControl))
				Else
					For Each control2 As Control In control.Controls
						If control2.[GetType]() Is htmlControlType Then
							result = (TryCast(control2, HtmlControl))
							Exit For
						End If
					Next
				End If
				Return result
			End If
			Throw New InvalidCastException("The target-type is not a HtmlControlType")
		End Function

		Friend Shared Function AddRunAtServer(input As String, tagName As String) As String
            Dim pattern As String = "" '"<" + Regex.Escape(tagName) + "[^>]*?(?<InsertPos>\s*)>"
            Dim regex As Regex = New Regex(pattern, RegexOptions.IgnoreCase Or RegexOptions.Singleline)
			Dim match As Match = regex.Match(input)
			If match.Success Then
				Dim group As Group = match.Groups("InsertPos")
				Return input.Insert(group.Index + group.Length, " runat=""server""")
			End If
			Return input
		End Function
	End Class
End Namespace
