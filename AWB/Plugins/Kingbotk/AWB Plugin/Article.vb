'Copyright © 2008 Stephen Kennedy (Kingboyk) http://www.sdk-software.com/
'Copyright © 2008 Sam Reed (Reedy) http://www.reedyboy.net/

'This program is free software; you can redistribute it and/or modify it under the terms of Version 2 of the GNU General Public License as published by the Free Software Foundation.

'This program is distributed in the hope that it will be useful,
'but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

'You should have received a copy of the GNU General Public License Version 2 along with this program; if not, write to the Free Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA

Namespace AutoWikiBrowser.Plugins.Kingbotk
    ''' <summary>
    ''' An object representing an article which may or may not contain the targetted template
    ''' </summary>
    Friend NotInheritable Class Article
        ' Properties:
        Private mArticleText As String, mFullArticleTitle As String
        Private mNamespace As Integer
        Private mEditSummary As String = conWikiPluginBrackets

        ' Plugin-state:
        Private mSkipResults As SkipResults = SkipResults.NotSet
        Private mProcessIt As Boolean ' gets set by ArticleHasAMajorChange/ArticleHasAMinorChange

        ' New:
        Friend Sub New(ByVal ArticleText As String, ByVal vFullArticleTitle As String, _
        ByVal vNamespace As Integer)
            mArticleText = ArticleText
            mFullArticleTitle = vFullArticleTitle
            mNamespace = vNamespace
            'mFullArticleTitle = GetArticleName(mNamespace, mArticleTitle)
        End Sub

        ' Friend properties:
        Friend Property AlteredArticleText() As String
            Get
                Return mArticleText
            End Get
            Set(ByVal value As String)
                mArticleText = value
            End Set
        End Property
        Friend ReadOnly Property FullArticleTitle() As String
            Get
                Return mFullArticleTitle
            End Get
        End Property
        Friend ReadOnly Property [Namespace]() As Integer
            Get
                Return mNamespace
            End Get
        End Property
        Friend Property EditSummary() As String
            Get
                Return mEditSummary
            End Get
            Set(ByVal value As String)
                mEditSummary = value
            End Set
        End Property
        Friend Sub ArticleHasAMinorChange()
            mProcessIt = True
        End Sub
        Friend Sub ArticleHasAMajorChange()
            mProcessIt = True
        End Sub
        Friend ReadOnly Property ProcessIt() As Boolean
            Get
                Return mProcessIt
            End Get
        End Property

        ' For calling by plugin:
        Friend Sub PluginCheckTemplateCall(ByVal TemplateCall As String, ByVal PluginName As String)
            If Not TemplateCall = "" Then ' we have "template:"
                mProcessIt = True
                'EditSummary += "Remove ""template:"", "
                PluginManager.AWBForm.TraceManager.WriteArticleActionLine("Remove ""template:"" call", PluginName, True)
            End If
        End Sub
        Friend Sub PluginIHaveFinished(ByVal Result As SkipResults, ByVal PluginName As String)
            Select Case Result
                Case SkipResults.SkipBadTag
                    mSkipResults = SkipResults.SkipBadTag
                    PluginManager.AWBForm.TraceManager.SkippedArticleBadTag(PluginName, mFullArticleTitle, mNamespace)
                Case SkipResults.SkipRegex
                    If mSkipResults = SkipResults.NotSet Then mSkipResults = SkipResults.SkipRegex
                    PluginManager.AWBForm.TraceManager.SkippedArticle(PluginName, _
                       "Article text matched skip regex")
                Case SkipResults.SkipNoChange
                    PluginManager.AWBForm.TraceManager.SkippedArticle(PluginName, "No change")
                    mSkipResults = SkipResults.SkipNoChange
            End Select
        End Sub

        ' For calling by manager:
        Friend ReadOnly Property PluginManagerGetSkipResults() As SkipResults
            Get
                Return mSkipResults
            End Get
        End Property
        Friend Sub FinaliseEditSummary()
            EditSummary = Regex.Replace(EditSummary, ", $", "")
        End Sub
        Friend Sub PluginManagerEditSummaryTaggingCategory(ByVal CategoryName As String)
            If Not CategoryName = "" Then EditSummary += "Tag [[Category:" + CategoryName + "]]. "
        End Sub

        ' General article writing and manipulation:
        Friend Sub RenamedATemplate(ByVal OldName As String, ByVal NewName As String, ByVal Caller As String)
            DoneReplacement(OldName, NewName, False)
            PluginManager.AWBForm.TraceManager.WriteArticleActionLine( _
               String.Format("Rename template [[Template:{0}|{0}]]→[[Template:{1}|{1}]]", OldName, NewName), Caller)
        End Sub
        Friend Sub DoneReplacement(ByVal Old As String, ByVal Replacement As String, _
        ByVal LogIt As Boolean, Optional ByVal PluginName As String = "")
            mProcessIt = True
            EditSummary += Old + "→" + Replacement + ", "
            If LogIt Then PluginManager.AWBForm.TraceManager.WriteArticleActionLine("Replacement: " + Old + "→" + _
               Replacement, PluginName)
        End Sub
        Friend Sub TemplateAdded(ByVal Template As String, ByVal PluginName As String)
            mEditSummary += String.Format("Added {{{{[[Template:{0}|{0}]]}}}}, ", Template)
            PluginManager.AWBForm.TraceManager.WriteTemplateAdded(Template, PluginName)
            ArticleHasAMajorChange()
        End Sub
        Friend Sub ParameterAdded(ByVal ParamName As String, ByVal ParamValue As String, _
        ByVal PluginName As String, Optional ByVal MinorEdit As Boolean = False)
            mEditSummary += ParamName & "=" & ParamValue & ", "
            PluginManager.AWBForm.TraceManager.WriteArticleActionLine(ParamName & "=" & ParamValue, PluginName)

            If MinorEdit Then ArticleHasAMinorChange() Else ArticleHasAMajorChange()
        End Sub
        Friend Sub RestoreTemplateToPlaceholderSpot(ByVal TemplateHeader As String)
            ' just write one instance of template even if have multiple conTemplatePlaceholder's
            Static strPlaceholder As String = Regex.Escape(conTemplatePlaceholder)
            Static RestoreTemplateToPlaceholderSpotRegex As New Regex(strPlaceholder)

            AlteredArticleText = RestoreTemplateToPlaceholderSpotRegex.Replace(AlteredArticleText, TemplateHeader, 1)
            AlteredArticleText = RestoreTemplateToPlaceholderSpotRegex.Replace(AlteredArticleText, "")
        End Sub
        Friend Sub EditInBrowser()
            Tools.EditArticleInBrowser(FullArticleTitle)
        End Sub

        ' Enum:
        Private Enum BannerShellsEnum
            NotChecked
            NoneFound
            FoundWikiProjectBannerShell
        End Enum

        'State:
        Private WeFoundBannerShells As BannerShellsEnum
        Private MatchEvaluatorString As String

        ' Regexes:
        ' These could probably be simplified significantly (and extra logic doing things like removing linebreaks) if I learnt more of the magic characters
        Private Shared ReadOnly WikiProjectBannerShellRegex As New Regex(conRegexpLeft & WikiProjectBannerShell & _
           ")\b\s*(?<start>\|[^1]*=.*?)*\s*\|\s*1\s*=\s*(?<body>.*}}[^{]*?)\s*(?<end>\|[^{]*)?\s*}}", _
           RegexOptions.Compiled Or RegexOptions.IgnoreCase Or RegexOptions.Singleline Or RegexOptions.ExplicitCapture)


        Friend Shared ReadOnly LineBreakRegex As New Regex("[\n\r]*")
        Private Shared ReadOnly DoubleLineBreakRegex As New Regex("[\n\r]{2,}")

        ' Regex constant strings:
        Private Const WikiProjectBannerShell As String = "WikiProject ?Banner ?Shell|WP?BS|WPBannerShell|WikiProject ?Banners|WPB" ' IGNORE CASE

        ' Match evaluators:
        Private Function WPBSRegexMatchEvaluator(ByVal match As Match) As String
            Const templatename As String = "WikiProjectBannerShell"
            Dim Ending As String = match.Groups("start").Value & match.Groups("end").Value

            ShellTemplateMatchEvaluatorsCommonTasks(templatename, match)

            If Not Ending = "" Then Ending = Microsoft.VisualBasic.vbCrLf + Ending

            Return DoubleLineBreakRegex.Replace("{{" & templatename & "|1=" & Microsoft.VisualBasic.vbCrLf & LineBreakRegex.Replace(MatchEvaluatorString, "") & _
                Microsoft.VisualBasic.vbCrLf & match.Groups("body").Value & Ending & "}}", Microsoft.VisualBasic.vbCrLf)
        End Function

        Private Sub ShellTemplateMatchEvaluatorsCommonTasks(ByVal templatename As String, ByVal match As Match)
            ' Does the shell contain template: ?
            PluginCheckTemplateCall(match.Groups("tl").Value, templatename)
            ' Does the template have it's primary name:
            If Not match.Groups("tlname").Value = templatename Then
                RenamedATemplate(match.Groups("tlname").Value, templatename, templatename)
            End If
        End Sub

        ' Where we (possibly) add our template to an existing shell:
        Friend Sub PrependTemplateOrWriteIntoShell(ByVal Template As Templating, ByVal ParameterBreak As String, _
        ByVal Text As String)
            If WeFoundBannerShells = BannerShellsEnum.NotChecked Then
                If WikiProjectBannerShellRegex.IsMatch(AlteredArticleText) Then
                    WeFoundBannerShells = BannerShellsEnum.FoundWikiProjectBannerShell
                Else
                    WeFoundBannerShells = BannerShellsEnum.NoneFound
                End If
            End If

            Text += Template.ParametersToString(ParameterBreak)

            Select Case WeFoundBannerShells
                Case BannerShellsEnum.FoundWikiProjectBannerShell
                    MatchEvaluatorString = Text

                    AlteredArticleText = WikiProjectBannerShellRegex.Replace(AlteredArticleText, _
                       AddressOf Me.WPBSRegexMatchEvaluator, 1)
                    MatchEvaluatorString = Nothing
                Case BannerShellsEnum.NoneFound
                    AlteredArticleText = Text + AlteredArticleText
            End Select
        End Sub

        ' Misc:
        Friend Function PageContainsShellTemplate() As Boolean
            ' Currently only WPBio can possibly call this, so it's ok to just run the regex and not cache the results
            ' Later on we want to have dynamic redirects and management of these templates (or it maybe should be in
            ' WikiFunctions.TalkPages
            Return WikiProjectBannerShellRegex.IsMatch(AlteredArticleText)
        End Function
    End Class
End Namespace