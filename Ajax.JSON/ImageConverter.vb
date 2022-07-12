Imports System
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.IO
Imports System.Text
Imports System.Web

Namespace Ajax.JSON
	Friend Class ImageConverter
		Implements IAjaxObjectConverter

        Public ReadOnly Property ClientScriptIdentifier() As String Implements IAjaxObjectConverter.ClientScriptIdentifier
            Get
                Return "AjaxImage"
            End Get
        End Property

        Public ReadOnly Property SupportedTypes() As Type() Implements IAjaxObjectConverter.SupportedTypes
            Get
                Return New Type() {GetType(Bitmap)}
            End Get
        End Property

        Public ReadOnly Property IncludeSubclasses() As Boolean Implements IAjaxObjectConverter.IncludeSubclasses
            Get
                Return True
            End Get
        End Property


        Public Sub RenderClientScript(ByRef sb As StringBuilder) Implements IAjaxObjectConverter.RenderClientScript
            sb.Append("function AjaxImage(url){var img=new Image();img.src=url;return img;}" & vbCrLf)
        End Sub

        Public Function FromString(s As String, t As Type) As Object Implements IAjaxObjectConverter.FromString
            Throw New NotImplementedException()
        End Function

        Public Sub ToJSON(ByRef sb As StringBuilder, o As Object) Implements IAjaxObjectConverter.ToJSON
            If o.[GetType]() Is GetType(Bitmap) Then
                Try
                    Dim text As String = HttpContext.Current.Server.MapPath(Utility.Settings.TemporaryFiles.Path)
                    If Not Directory.Exists(text) Then
                        sb.Append("null")
                    Else
                        Dim text2 As String = Guid.NewGuid().ToString() + ".jpg"
                        Dim bitmap As Bitmap = CType(o, Bitmap)
                        bitmap.Save(Path.Combine(text, text2), ImageFormat.Jpeg)
                        Dim text3 As String
                        If Utility.Settings.TemporaryFiles.Path.StartsWith("~/") OrElse Utility.Settings.TemporaryFiles.Path.StartsWith("~\") Then
                            text3 = Path.Combine(HttpContext.Current.Request.ApplicationPath, Utility.Settings.TemporaryFiles.Path.Substring(2))
                        Else
                            text3 = Utility.Settings.TemporaryFiles.Path
                        End If
                        text3 = text3.Replace("\", "/")
                        sb.Append("new AjaxImage('" + text3)
                        If Not text3.EndsWith("/") Then
                            sb.Append("/")
                        End If
                        sb.Append(text2 + "')")
                        Try
                            Dim t As DateTime = DateTime.Now.AddMinutes(CDec((-1 * Utility.Settings.TemporaryFiles.DeleteAfter)))
                            Dim files As String() = Directory.GetFiles(text, "*.jpg")
                            For i As Integer = 0 To files.Length - 1
                                Dim text4 As String = files(i)
                                Dim fileInfo As FileInfo = New FileInfo(text4)
                                If fileInfo.CreationTime < t Then
                                    File.Delete(text4)
                                End If
                            Next
                        Catch ex_1BA As Exception
                        End Try
                    End If
                Catch ex_1BF As Exception
                    sb.Append("null")
                End Try
            End If
        End Sub


    End Class
End Namespace
