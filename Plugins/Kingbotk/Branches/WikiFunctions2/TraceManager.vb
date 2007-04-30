Namespace Logging
    ''' <summary>
    ''' An inheritable implementation of a Logging manager, built around a generic collection of IMyTraceListener objects and String keys
    ''' </summary>
    Public Class TraceManager
        Implements IMyTraceListener

        ' Listeners:
        Protected Listeners As New Dictionary(Of String, IMyTraceListener)

        Public Overridable Sub AddListener(ByVal Key As String, ByVal Listener As IMyTraceListener)
            ' Override this if you want to programatically add an event handler
            Listeners.Add(Key, Listener)
        End Sub
        Public Overridable Sub RemoveListener(ByVal Key As String)
            ' Override this if you want to programatically remove an event handler
            Listeners(Key).Close()
            Listeners.Remove(Key)
        End Sub
        Public Function ContainsKey(ByVal Key As String) As Boolean
            Return Listeners.ContainsKey(Key)
        End Function
        Public Function ContainsValue(ByVal Listener As IMyTraceListener) As Boolean
            Return Listeners.ContainsValue(Listener)
        End Function

        ' IMyTraceListener:
        Public Overridable Sub Close() Implements IMyTraceListener.Close
            For Each t As KeyValuePair(Of String, IMyTraceListener) In Listeners
                t.Value.Close()
            Next
        End Sub
        Public Overridable Sub Flush() Implements IMyTraceListener.Flush
            For Each t As KeyValuePair(Of String, IMyTraceListener) In Listeners
                t.Value.Flush()
            Next
        End Sub
        Public Overridable Sub ProcessingArticle(ByVal FullArticleTitle As String, ByVal NS As Namespaces) _
        Implements IMyTraceListener.ProcessingArticle
            For Each t As KeyValuePair(Of String, IMyTraceListener) In Listeners
                t.Value.ProcessingArticle(FullArticleTitle, NS)
            Next
        End Sub
        Public Overridable Sub WriteBulletedLine(ByVal Line As String, ByVal Bold As Boolean, _
        ByVal VerboseOnly As Boolean, ByVal DateStamp As Boolean) Implements IMyTraceListener.WriteBulletedLine
            For Each t As KeyValuePair(Of String, IMyTraceListener) In Listeners
                t.Value.WriteBulletedLine(Line, Bold, VerboseOnly, DateStamp)
            Next
        End Sub
        Public Overridable Sub WriteBulletedLine(ByVal Line As String, ByVal Bold As Boolean, _
        ByVal VerboseOnly As Boolean) Implements IMyTraceListener.WriteBulletedLine
            WriteBulletedLine(Line, Bold, VerboseOnly, False)
        End Sub
        Public Overridable Sub SkippedArticle(ByVal SkippedBy As String, ByVal Reason As String) _
        Implements IMyTraceListener.SkippedArticle
            For Each t As KeyValuePair(Of String, IMyTraceListener) In Listeners
                t.Value.SkippedArticle(SkippedBy, Reason)
            Next
        End Sub
        Public Overridable Sub SkippedArticleBadTag(ByVal SkippedBy As String, ByVal FullArticleTitle As String, _
        ByVal NS As Namespaces) Implements IMyTraceListener.SkippedArticleBadTag
            For Each t As KeyValuePair(Of String, IMyTraceListener) In Listeners
                t.Value.SkippedArticleBadTag(SkippedBy, FullArticleTitle, NS)
            Next
        End Sub
        Public Overridable Sub SkippedArticleRedlink(ByVal SkippedBy As String, ByVal FullArticleTitle As String, _
        ByVal NS As Namespaces) Implements IMyTraceListener.SkippedArticleRedlink
            For Each t As KeyValuePair(Of String, IMyTraceListener) In Listeners
                t.Value.SkippedArticleRedlink(SkippedBy, FullArticleTitle, NS)
            Next
        End Sub
        Public Overridable Sub WriteArticleActionLine(ByVal Line As String, ByVal PluginName As String) _
        Implements IMyTraceListener.WriteArticleActionLine
            For Each t As KeyValuePair(Of String, IMyTraceListener) In Listeners
                t.Value.WriteArticleActionLine(Line, PluginName)
            Next
        End Sub
        Public Overridable Sub WriteTemplateAdded(ByVal Template As String, ByVal PluginName As String) _
        Implements IMyTraceListener.WriteTemplateAdded
            For Each t As KeyValuePair(Of String, IMyTraceListener) In Listeners
                t.Value.WriteTemplateAdded(Template, PluginName)
            Next
        End Sub
        Public Overridable Sub WriteArticleActionLine1(ByVal Line As String, ByVal PluginName As String, _
        ByVal VerboseOnly As Boolean) Implements IMyTraceListener.WriteArticleActionLine
            If VerboseOnly Then
                For Each t As KeyValuePair(Of String, IMyTraceListener) In Listeners
                    t.Value.WriteArticleActionLine(Line, PluginName, True)
                Next
            Else
                For Each t As KeyValuePair(Of String, IMyTraceListener) In Listeners
                    t.Value.WriteArticleActionLine(Line, PluginName)
                Next
            End If
        End Sub
        Public Overridable ReadOnly Property Uploadable() As Boolean Implements IMyTraceListener.Uploadable
            Get
                For Each t As KeyValuePair(Of String, IMyTraceListener) In Listeners
                    If t.Value.Uploadable Then Return True
                Next
            End Get
        End Property
        Public Overridable Sub Write(ByVal Text As String) Implements IMyTraceListener.Write
            For Each t As KeyValuePair(Of String, IMyTraceListener) In Listeners
                t.Value.Write(Text)
            Next
        End Sub
        Public Overridable Sub WriteComment(ByVal Line As String) Implements IMyTraceListener.WriteComment
            For Each t As KeyValuePair(Of String, IMyTraceListener) In Listeners
                t.Value.WriteComment(Line)
            Next
        End Sub
        Public Overridable Sub WriteCommentAndNewLine(ByVal Line As String) Implements IMyTraceListener.WriteCommentAndNewLine
            For Each t As KeyValuePair(Of String, IMyTraceListener) In Listeners
                t.Value.WriteCommentAndNewLine(Line)
            Next
        End Sub
        Public Overridable Sub WriteLine(ByVal Line As String) Implements IMyTraceListener.WriteLine
            For Each t As KeyValuePair(Of String, IMyTraceListener) In Listeners
                t.Value.WriteLine(Line)
            Next
        End Sub
    End Class
End Namespace
