Imports System
Imports System.Security.Cryptography

Namespace MS.Utilities
	Friend Class MD5Helper
		Friend Shared Function GetHash(data As Byte()) As String
			Dim text As String = ""
			Dim array As String() = New String(15) {}
			Dim mD As MD5 = New MD5CryptoServiceProvider()
			Dim array2 As Byte() = mD.ComputeHash(data)
			For i As Integer = 0 To array2.Length - 1
				array(i) = array2(i).ToString("x")
				text += array(i)
			Next
			Return text
		End Function
	End Class
End Namespace
