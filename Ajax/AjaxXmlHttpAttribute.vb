Imports System

Namespace Ajax
	<AttributeUsage(AttributeTargets.Method), Obsolete("Please remove this Attribute. There is no restriction for concurrent requests.", True)>
	Public Class AjaxXmlHttpAttribute
		Inherits Attribute

        Private ReadOnly xmlHttpVariable1 As String

        Public ReadOnly Property XmlHttpVariable() As String
			Get
                Return Me.xmlHttpVariable1
            End Get
		End Property

		Public Sub New(xmlHttpVariable As String)
            Me.xmlHttpVariable1 = xmlHttpVariable
        End Sub
	End Class
End Namespace
