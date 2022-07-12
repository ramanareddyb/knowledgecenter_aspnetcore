Imports System

Namespace Ajax
	Friend Class AjaxConverterItem
		Friend ConverterType As Type

		Friend Action As AjaxConverterConfigurationAction

		Friend Sub New(converterType As Type, action As AjaxConverterConfigurationAction)
			Me.ConverterType = converterType
			Me.Action = action
		End Sub
	End Class
End Namespace
