Imports System
Imports System.Web.Caching

Namespace Ajax
    <AttributeUsage(AttributeTargets.Method)>
    Public Class AjaxMethodAttribute
        Inherits Attribute

        Private methodName1 As String

        Private isCacheEnabled1 As Boolean

        Private ReadOnly cacheDuration1 As TimeSpan = Cache.NoSlidingExpiration

        Private ReadOnly requireSessionState1 As HttpSessionStateRequirement = HttpSessionStateRequirement.None

        Private ReadOnly httpConnectionProtocol1 As HttpConnectionProtocolType = HttpConnectionProtocolType.[Default]

        Friend Property MethodName() As String
            Get
                Return Me.methodName1
            End Get
            Set(value As String)
                Me.methodName1 = value
            End Set
        End Property

        Friend ReadOnly Property CacheDuration() As TimeSpan
            Get
                Return Me.cacheDuration1
            End Get
        End Property

        Friend ReadOnly Property IsCacheEnabled() As Boolean
            Get
                Return Me.isCacheEnabled1
            End Get
        End Property

        Friend ReadOnly Property RequireSessionState() As HttpSessionStateRequirement
            Get
                Return Me.requireSessionState1
            End Get
        End Property

        Friend ReadOnly Property HttpConnectionProtocol() As HttpConnectionProtocolType
            Get
                Return Me.httpConnectionProtocol1
            End Get
        End Property

        Public Sub New()
        End Sub

        Public Sub New(methodName As String)
            Me.methodName1 = methodName
        End Sub

        Public Sub New(requireSessionState As HttpSessionStateRequirement)
            Me.requireSessionState1 = requireSessionState
        End Sub

        Public Sub New(cacheDuration As TimeSpan)
            Me.isCacheEnabled1 = True
            Me.cacheDuration1 = cacheDuration
        End Sub

        Public Sub New(cacheSeconds As Integer)
            Me.isCacheEnabled1 = True
            Me.cacheDuration1 = New TimeSpan(0, 0, 0, cacheSeconds, 0)
        End Sub

        Public Sub New(methodName As String, cacheSeconds As Integer)
            Me.isCacheEnabled1 = True
            Me.cacheDuration1 = New TimeSpan(0, 0, 0, cacheSeconds, 0)
            Me.methodName1 = methodName
        End Sub

        Public Sub New(cacheSeconds As Integer, requireSessionState As HttpSessionStateRequirement)
            Me.isCacheEnabled1 = True
            Me.cacheDuration1 = New TimeSpan(0, 0, 0, cacheSeconds, 0)
            Me.requireSessionState1 = requireSessionState
        End Sub

        Public Sub New(methodName As String, cacheSeconds As Integer, requireSessionState As HttpSessionStateRequirement)
            Me.methodName1 = methodName
            Me.isCacheEnabled1 = True
            Me.cacheDuration1 = New TimeSpan(0, 0, 0, cacheSeconds, 0)
            Me.requireSessionState1 = requireSessionState
        End Sub

        Public Sub New(methodName As String, requireSessionState As HttpSessionStateRequirement)
            Me.methodName1 = methodName
            Me.requireSessionState1 = requireSessionState
        End Sub

        <Obsolete("Most browsers do not accept changing protocol type.")>
        Public Sub New(httpConnectionProtocol As HttpConnectionProtocolType)
            Me.httpConnectionProtocol1 = httpConnectionProtocol
        End Sub

        <Obsolete("Most browsers do not accept changing protocol type.")>
        Public Sub New(methodName As String, httpConnectionProtocol As HttpConnectionProtocolType)
            Me.methodName1 = methodName
            Me.httpConnectionProtocol1 = httpConnectionProtocol
        End Sub

        <Obsolete("Most browsers do not accept changing protocol type.")>
        Public Sub New(requireSessionState As HttpSessionStateRequirement, HttpConnectionProtocol As HttpConnectionProtocolType)
            Me.requireSessionState1 = requireSessionState
            Me.httpConnectionProtocol1 = Me.httpConnectionProtocol
        End Sub

        <Obsolete("Most browsers do not accept changing protocol type.")>
        Public Sub New(methodName As String, httpConnectionProtocol As HttpConnectionProtocolType, requireSessionState As HttpSessionStateRequirement)
            Me.methodName1 = methodName
            Me.httpConnectionProtocol1 = httpConnectionProtocol
            Me.requireSessionState1 = requireSessionState
        End Sub
    End Class
End Namespace
