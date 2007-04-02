Namespace TalkPages
    Public Enum DEFAULTSORT
        NoChange
        MoveToTop
        MoveToBottom
    End Enum
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
        Private DefaultSortRegex As New Regex("\{\{\s*(template\s*:\s*)*\s*defaultsort\s*(:|\|)(?<key>[^\}]*)\}\}", _
           RegexOptions.IgnoreCase Or RegexOptions.Compiled Or RegexOptions.ExplicitCapture)
        Private FoundTalkheader As Boolean, FoundSkipTOC As Boolean, FoundDefaultSort As Boolean
        Private DefaultSortKey As String

        Public Function ProcessTalkPage(ByRef ArticleText As String, ByVal Trace As IMyTraceListener, _
        ByVal MoveDEFAULTSORT As DEFAULTSORT, ByVal PluginName As String) As Boolean
            FoundTalkheader = False : FoundSkipTOC = False : FoundDefaultSort = False

            ArticleText = TalkheaderTemplateRegex.Replace(ArticleText, AddressOf TalkheaderMatchEvaluator, 1)
            ArticleText = SkipTOCTemplateRegex.Replace(ArticleText, AddressOf SkipTOCMatchEvaluator, 1)

            If FoundTalkheader Then WriteHeaderTemplate("talkheader", ArticleText, Trace, PluginName)
            If FoundSkipTOC Then WriteHeaderTemplate("skiptotoctalk", ArticleText, Trace, PluginName)

            If Not MoveDEFAULTSORT = DEFAULTSORT.NoChange Then
                ArticleText = DefaultSortRegex.Replace(ArticleText, AddressOf DefaultSortMatchEvaluator, 1)
                If FoundDefaultSort Then
                    If DefaultSortKey = "" Then
                        If Not Trace Is Nothing Then _
                           Trace.WriteArticleActionLine("DEFAULTSORT has no key; removed", PluginName)
                    Else
                        WriteDefaultSortMagicWord(MoveDEFAULTSORT, Trace, PluginName, ArticleText)
                    End If
                End If

                DefaultSortKey = ""
            End If

            Return FoundTalkheader OrElse FoundSkipTOC OrElse FoundDefaultSort
        End Function
        Public Sub ProcessTalkPage(ByRef ArticleText As String, ByVal PluginName As String)
            ProcessTalkPage(ArticleText, Nothing, DEFAULTSORT.NoChange, PluginName)
        End Sub
        Public Sub ProcessTalkPage(ByRef ArticleText As String, ByVal MoveDEFAULTSORT As DEFAULTSORT, _
        ByVal PluginName As String)
            ProcessTalkPage(ArticleText, Nothing, MoveDEFAULTSORT, PluginName)
        End Sub
        Public Function ContainsDefaultSortKeywordOrTemplate(ByVal ArticleText As String) As Boolean
            Return DefaultSortRegex.IsMatch(ArticleText)
        End Function

        Private Sub WriteDefaultSortMagicWord(ByVal Location As DEFAULTSORT, ByVal Trace As IMyTraceListener, _
        ByVal PluginName As String, ByRef ArticleText As String)
            Const conDefSort As String = "{{DEFAULTSORT:"
            Dim strMovedTo As String

            If Location = DEFAULTSORT.MoveToTop Then
                ArticleText = conDefSort & DefaultSortKey & "}}" & Microsoft.VisualBasic.vbCrLf & ArticleText
                strMovedTo = " given top billing"
            Else
                ArticleText += Microsoft.VisualBasic.vbCrLf & conDefSort & DefaultSortKey & "}}"
                strMovedTo = " sent to the bottom"
            End If
            If Not Trace Is Nothing Then Trace.WriteArticleActionLine("{{DEFAULTSORT}}" & strMovedTo, _
                PluginName, False)
        End Sub
        Private Sub WriteHeaderTemplate(ByVal Name As String, ByRef ArticleText As String, _
        ByVal Trace As IMyTraceListener, ByVal PluginName As String)
            ArticleText = "{{" & Name & "}}" & Microsoft.VisualBasic.vbCrLf & ArticleText
            If Not Trace Is Nothing Then Trace.WriteArticleActionLine("{{tl|" & Name & " }} given top billing", _
               PluginName, False)
        End Sub
        Private Function TalkheaderMatchEvaluator(ByVal match As Match) As String
            FoundTalkheader = True
            Return ""
        End Function
        Private Function SkipTOCMatchEvaluator(ByVal match As Match) As String
            FoundSkipTOC = True
            Return ""
        End Function
        Private Function DefaultSortMatchEvaluator(ByVal match As Match) As String
            FoundDefaultSort = True

            If match.Groups("key").Captures.Count > 0 Then _
               DefaultSortKey = match.Groups("key").Captures(0).Value.Trim

            Return ""
        End Function
    End Module
End Namespace