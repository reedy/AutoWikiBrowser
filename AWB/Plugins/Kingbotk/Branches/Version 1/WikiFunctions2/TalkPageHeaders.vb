Public Module TalkPageHeaders
    ''' <summary>
    ''' Parses for {{talkheader}} and {{skiptotoctalk}} and moves them to the top
    ''' </summary>
    ''' <remarks></remarks>
    Private TalkheaderTemplateRegex As New Regex("\{\{\s*(template:)?talkheader\s*\}\}[\s\n\r]*", _
       RegexOptions.ExplicitCapture Or RegexOptions.Compiled Or RegexOptions.IgnoreCase)
    Private SkipTOCTemplateRegex As New Regex( _
       "\{\{\s*(template:)?(skiptotoctalk|Skiptotoc|Skiptotoc-talk)\s*\}\}[\s\n\r]*", _
       RegexOptions.ExplicitCapture Or RegexOptions.Compiled Or RegexOptions.IgnoreCase)
    Private FoundTalkheader As Boolean, FoundSkipTOC As Boolean

    Public Sub ProcessTalkPage(ByRef ArticleText As String, ByVal Trace As TraceManager)
        FoundTalkheader = False : FoundSkipTOC = False

        ArticleText = TalkheaderTemplateRegex.Replace(ArticleText, AddressOf MatchEvaluator, 1)
        ArticleText = SkipTOCTemplateRegex.Replace(ArticleText, AddressOf MatchEvaluator2, 1)

        If FoundTalkheader Then WriteHeaderTemplate("talkheader", ArticleText, Trace)
        If FoundSkipTOC Then WriteHeaderTemplate("skiptotoctalk", ArticleText, Trace)
    End Sub
    Public Sub ProcessTalkPage(ByRef ArticleText As String)
        ProcessTalkPage(ArticleText, Nothing)
    End Sub

    Private Sub WriteHeaderTemplate(ByVal Name As String, ByRef ArticleText As String, ByVal Trace As TraceManager)
        ArticleText = "{{" & Name & "}}" & Microsoft.VisualBasic.vbCrLf & ArticleText
        If Not Trace Is Nothing Then Trace.WriteArticleActionLine1("{{tl|" & Name & " }} given top billing", _
           "Plugin manager", True)
    End Sub
    Private Function MatchEvaluator(ByVal match As Match) As String
        FoundTalkheader = True
        Return ""
    End Function
    Private Function MatchEvaluator2(ByVal match As Match) As String
        FoundSkipTOC = True
        Return ""
    End Function
End Module
