Imports System
Imports System.Web.UI.HtmlControls

Namespace Ajax.JSON.HtmlControls
	Friend Class HtmlTextAreaConverter
		Inherits HtmlControlConverter

		Public Overrides ReadOnly Property SupportedTypes() As Type()
			Get
				Return New Type() { GetType(HtmlTextArea) }
			End Get
		End Property
	End Class
End Namespace
