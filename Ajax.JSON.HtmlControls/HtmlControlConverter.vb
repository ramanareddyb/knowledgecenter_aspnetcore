Imports System
Imports System.Text
Imports System.Web.UI
Imports System.Web.UI.HtmlControls

Namespace Ajax.JSON.HtmlControls
	Friend Class HtmlControlConverter
		Implements IAjaxObjectConverter

        Public ReadOnly Property ClientScriptIdentifier() As String Implements IAjaxObjectConverter.ClientScriptIdentifier
            Get
                Return "AjaxHtmlControl"
            End Get
        End Property

        Public Overridable ReadOnly Property SupportedTypes() As Type() Implements IAjaxObjectConverter.SupportedTypes
            Get
                Return New Type() {GetType(HtmlControl)}
            End Get
        End Property

        Public ReadOnly Property IncludeSubclasses() As Boolean Implements IAjaxObjectConverter.IncludeSubclasses
            Get
                Return False
            End Get
        End Property


        Public Sub RenderClientScript(ByRef sb As StringBuilder) Implements IAjaxObjectConverter.RenderClientScript
            sb.Append("function HtmlControl(id) {" & vbCrLf & vbTab & "var ele = null;" & vbCrLf & vbTab & "if(typeof(id) == 'object') ele = id; else ele = document.getElementById(id);" & vbCrLf & vbTab & "if(ele == null) return null;" & vbCrLf & vbTab & "var _o = ele.cloneNode(true);" & vbCrLf & vbTab & "var _op = document.createElement('SPAN');" & vbCrLf & vbTab & "_op.appendChild(_o);" & vbTab & vbCrLf & vbTab & "this._source = _op.innerHTML;" & vbCrLf & "}" & vbCrLf & "HtmlControl.prototype.toString = function(){ return this._source; }" & vbCrLf & vbCrLf & "function HtmlControlUpdate(func, parentId) {" & vbCrLf & "var f,i,ff,fa='';" & vbCrLf & "var ele = document.getElementById(parentId);" & vbCrLf & "if(ele == null) return;" & vbCrLf & "var args = [];" & vbCrLf & "for(i=0; i<HtmlControlUpdate.arguments.length; i++)" & vbCrLf & vbTab & "args[args.length] = HtmlControlUpdate.arguments[i];" & vbCrLf & "if(args.length > 2)" & vbCrLf & vbTab & "for(i=2; i<args.length; i++){fa += 'args[' + i + ']';if(i < args.length -1){ fa += ','; }}" & vbCrLf & "f = '{""invoke"":function(args){return ' + func + '(' + fa + ');}}';" & vbCrLf & "ff = null;eval('ff=' + f + ';');" & vbCrLf & "if(ff != null && typeof(ff.invoke) == 'function')" & vbCrLf & "{" & vbCrLf & vbTab & "var res = ff.invoke(args);" & vbCrLf & vbTab & "if(res.error != null){alert(res.error);return;}" & vbCrLf & vbTab & "ele.innerHTML = res.value;" & vbCrLf & "}" & vbCrLf & "}" & vbCrLf)
        End Sub

        Public Function FromString(s As String, t As Type) As Object Implements IAjaxObjectConverter.FromString
            Return AjaxHtmlControlConverter.HtmlToHtmlControl(s, t)
        End Function

        Public Sub ToJSON(ByRef sb As StringBuilder, o As Object) Implements IAjaxObjectConverter.ToJSON
            DefaultConverter.ToJSON(sb, AjaxHtmlControlConverter.ControlToString(CType(o, Control)))
        End Sub


    End Class
End Namespace
