Imports System
Imports System.IO
Imports System.Web

Namespace MS.Web
	Friend Class Request
		Private m_RequestType As String

		Private m_URL As String

		Private context As HttpContext

		Public ReadOnly Property Extension() As String
			Get
				Return System.IO.Path.GetExtension(Me.m_URL).ToLower()
			End Get
		End Property

		Public ReadOnly Property FileName() As String
			Get
				Return System.IO.Path.GetFileName(Me.m_URL).ToLower()
			End Get
		End Property

		Public ReadOnly Property FileNameWithoutExtension() As String
			Get
				Return System.IO.Path.GetFileNameWithoutExtension(Me.m_URL).ToLower()
			End Get
		End Property

		Public ReadOnly Property ApplicationPath() As String
			Get
                Dim applicationPath1 As String = Me.context.Request.ApplicationPath
                If applicationPath1.Equals("/") Then
                    Return ""
                End If
                Return applicationPath1
            End Get
		End Property

		Public ReadOnly Property FilePath() As String
			Get
				Return Me.context.Request.FilePath
			End Get
		End Property

		Public ReadOnly Property FilePathWithoutExtension() As String
			Get
				Return Me.FilePath.Substring(0, Me.FilePath.Length - Me.Extension.Length)
			End Get
		End Property

		Public ReadOnly Property VirtualFilePath() As String
			Get
				Return Me.FilePathWithoutExtension.Substring(Me.ApplicationPath.Length)
			End Get
		End Property

		Public ReadOnly Property PhysicalApplicationPath() As String
			Get
				Return Me.context.Request.PhysicalApplicationPath
			End Get
		End Property

		Public ReadOnly Property PhysicalPath() As String
			Get
				Return Me.context.Request.PhysicalPath
			End Get
		End Property

		Public ReadOnly Property PhysicalPathWithoutExtension() As String
			Get
				Return Me.PhysicalPath.Substring(0, Me.PhysicalPath.Length - Me.Extension.Length)
			End Get
		End Property

		Public ReadOnly Property Path() As String
			Get
				Dim text As String = Me.FilePath.Substring(Me.ApplicationPath.Length, Me.FilePath.Length - Me.ApplicationPath.Length - Me.FileName.Length)
				Return text.Trim(New Char() { "/"c })
			End Get
		End Property

		Public Sub New(context As HttpContext, requestType As String, url As String)
			Me.context = context
			Me.m_RequestType = requestType
			Me.m_URL = url
		End Sub
	End Class
End Namespace
