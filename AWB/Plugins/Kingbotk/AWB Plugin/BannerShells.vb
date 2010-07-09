'Copyright © 2008 Stephen Kennedy (Kingboyk) http://www.sdk-software.com/
'Copyright © 2008 Sam Reed (Reedy) http://www.reedyboy.net/

'This program is free software; you can redistribute it and/or modify it under the terms of Version 2 of the GNU General Public License as published by the Free Software Foundation.

'This program is distributed in the hope that it will be useful,
'but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

'You should have received a copy of the GNU General Public License Version 2 along with this program; if not, write to the Free Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA

Namespace AutoWikiBrowser.Plugins.Kingbotk
    Partial Class Article
        ' Enum:
        Private Enum BannerShellsEnum
            NotChecked
            NoneFound
            FoundWikiProjectBannerShell
            FoundWikiProjectBanners ' if both are present, well, tough titty! :)
        End Enum

        'State:
        Private WeFoundBannerShells As BannerShellsEnum
        Private MatchEvaluatorString As String

        ' Regexes:
        ' These could probably be simplified significantly (and extra logic doing things like removing linebreaks) if I learnt more of the magic characters
        Private Shared ReadOnly WikiProjectBannerShellRegex As New Regex(conRegexpLeft & WikiProjectBannerShell & _
           ")\b\s*(?<start>\|[^1]*=.*?)*\s*\|\s*1\s*=\s*(?<body>.*}}[^{]*?)\s*(?<end>\|[^{]*)?\s*}}", _
           RegexOptions.Compiled Or RegexOptions.IgnoreCase Or RegexOptions.Singleline Or RegexOptions.ExplicitCapture)
        ' last known reasonably good version (no catching blp= etc at the start of regex): '")\b\s*\|\s*1\s*=\s*(?<body>.*}}[^{]*?)(?<end>\|[^{]*)?}}"
        Private Shared ReadOnly WikiProjectBannersRegex As New Regex(conRegexpLeft & WikiProjectBanners & _
           ")\b(\s*\|\s*[0-9]+\s*=\s*(?<body>(\{\{\s*[^{]*}}[^{]*?)|\s*))*\s*}}", RegexOptions.Compiled _
           Or RegexOptions.IgnoreCase Or RegexOptions.Singleline Or RegexOptions.ExplicitCapture)
        ' last known reasonably good version: no catching of empty numbered params: ")\b(\s*\|\s*[0-9]+\s*=\s*(?<body>\{\{\s*[^{]*}}[^{]*?))*}}"
        Private Shared ReadOnly ShellRegex As New Regex(conRegexpLeft & WikiProjectBannerShell & "|" & _
           WikiProjectBanners & ")\b\s*\|", RegexOptions.Singleline Or RegexOptions.Compiled Or RegexOptions.IgnoreCase _
           Or RegexOptions.ExplicitCapture)
        Friend Shared ReadOnly LineBreakRegex As New Regex("[\n\r]*")
        Private Shared ReadOnly DoubleLineBreakRegex As New Regex("[\n\r]{2,}")

        ' Regex constant strings:
        Private Const WikiProjectBannerShell As String = "WikiProject ?Banner ?Shell|WP?BS|WPBannerShell" ' IGNORE CASE
        Private Const WikiProjectBanners As String = "WikiProject ?Banners|WPB" ' IGNORE CASE

        ' Match evaluators:
        Private Function WPBSRegexMatchEvaluator(ByVal match As Match) As String
            Const templatename As String = "WikiProjectBannerShell"
            Dim Ending As String = match.Groups("start").Value & match.Groups("end").Value

            ShellTemplateMatchEvaluatorsCommonTasks(templatename, match)

            If Not Ending = "" Then Ending = Microsoft.VisualBasic.vbCrLf + Ending

            Return DoubleLineBreakRegex.Replace("{{" & templatename & "|1=" & Microsoft.VisualBasic.vbCrLf & LineBreakRegex.Replace(MatchEvaluatorString, "") & _
               Microsoft.VisualBasic.vbCrLf & match.Groups("body").Value & Ending & "}}", Microsoft.VisualBasic.vbCrLf)
        End Function
        Private Function WikiProjectBannersRegexMatchEvaluator(ByVal match As Match) As String
            Const templatename As String = "WikiProjectBanners"
            Dim i As Integer = 1

            ShellTemplateMatchEvaluatorsCommonTasks(templatename, match)

            WikiProjectBannersRegexMatchEvaluator = "{{" & templatename

            For Each c As Capture In match.Groups("body").Captures
                If Not c.Value.Trim = "" Then
                    WikiProjectBannersRegexMatchEvaluator += Microsoft.VisualBasic.vbCrLf + "|" + i.ToString + "=" + c.Value
                    i += 1
                End If
            Next
            WikiProjectBannersRegexMatchEvaluator += Microsoft.VisualBasic.vbCrLf + "|" + i.ToString + "=" + _
               LineBreakRegex.Replace(MatchEvaluatorString, "") + Microsoft.VisualBasic.vbCrLf + "}}"
        End Function
        Private Sub ShellTemplateMatchEvaluatorsCommonTasks(ByVal templatename As String, ByVal match As Match)
            ' Does the shell contain template: ?
            PluginCheckTemplateCall(match.Groups("tl").Value, templatename)
            ' Does the template have it's primary name:
            If Not match.Groups("tlname").Value = templatename Then RenamedATemplate(match.Groups("tlname").Value, _
               templatename, templatename)
        End Sub

        ' Where we (possibly) add our template to an existing shell:
        Friend Sub PrependTemplateOrWriteIntoShell(ByVal Template As Templating, ByVal ParameterBreak As String, _
        ByVal Text As String, ByVal PluginName As String)
            If WeFoundBannerShells = BannerShellsEnum.NotChecked Then
                If WikiProjectBannerShellRegex.IsMatch(AlteredArticleText) Then
                    WeFoundBannerShells = BannerShellsEnum.FoundWikiProjectBannerShell
                ElseIf WikiProjectBannersRegex.IsMatch(AlteredArticleText) Then
                    WeFoundBannerShells = BannerShellsEnum.FoundWikiProjectBanners
                Else
                    WeFoundBannerShells = BannerShellsEnum.NoneFound
                End If
            End If

            Text += Template.ParametersToString(ParameterBreak)

            Select Case WeFoundBannerShells
                Case BannerShellsEnum.FoundWikiProjectBanners
                    MatchEvaluatorString = Text
                    AlteredArticleText = WikiProjectBannersRegex.Replace(AlteredArticleText, _
                       AddressOf Me.WikiProjectBannersRegexMatchEvaluator, 1)
                    MatchEvaluatorString = Nothing
                Case BannerShellsEnum.FoundWikiProjectBannerShell
                    MatchEvaluatorString = Text

                    AlteredArticleText = WikiProjectBannerShellRegex.Replace(AlteredArticleText, _
                       AddressOf Me.WPBSRegexMatchEvaluator, 1)
                    MatchEvaluatorString = Nothing
                Case BannerShellsEnum.NoneFound
                    AlteredArticleText = Text + AlteredArticleText
                Case Else
                    Throw New ArgumentException
            End Select
        End Sub

        ' Misc:
        Friend Function PageContainsShellTemplate() As Boolean
            ' Currently only WPBio can possibly call this, so it's ok to just run the regex and not cache the results
            ' Later on we want to have dynamic redirects and management of these templates (or it maybe should be in
            ' WikiFunctions.TalkPages
            Return ShellRegex.IsMatch(AlteredArticleText)
        End Function
    End Class
End Namespace
