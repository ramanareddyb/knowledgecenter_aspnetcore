Imports System
Imports System.Web.UI.HtmlControls

Namespace Ajax.JSON.HtmlControls
	Friend Class HtmlInputRadioButtonConverter
		Inherits HtmlControlConverter

		Public Overrides ReadOnly Property SupportedTypes() As Type()
			Get
				Return New Type() { GetType(HtmlInputRadioButton) }
			End Get
		End Property
	End Class
End Namespace
