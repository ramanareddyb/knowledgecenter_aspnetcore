Imports System

Namespace Ajax
	<AttributeUsage(AttributeTargets.Method), Obsolete("Please use [AjaxMethod(HttpSessionStateRequirement.ReadWrite)] instead.", True)>
	Public Class AjaxRequireSessionStateAttribute
		Inherits Attribute

	End Class
End Namespace
