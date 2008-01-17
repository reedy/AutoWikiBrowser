Namespace AutoWikiBrowser.Plugins.SDKSoftware.Kingbotk
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
        Private mLiving As Boolean, mActivePol As Boolean
        Private SearchedForLivingInShell As Boolean, SearchedForActivePolInShell As Boolean

        ' Regexes:
        Private Shared ReadOnly WikiProjectBannerShellRegex As New Regex(PluginBase.conRegexpLeft & PluginBase.WikiProjectBannerShell & _
           ")\b[\s\n\r]*\|[\s\n\r]*1[\s\n\r]*=[\s\n\r]*(?<body>.*}}[^{]*?)(?<end>\|[^{]*)?}}", RegexOptions.Compiled _
           Or RegexOptions.IgnoreCase Or RegexOptions.Singleline Or RegexOptions.ExplicitCapture) '")\b[\s\n\r]*\|[\s\n\r]*1[\s\n\r]*=[\s\n\r]*(?<body>.*}}[^{]*?)(?<end>\|[^{|]*)*?}}"
        Private Shared ReadOnly WikiProjectBannersRegex As New Regex("")
        Private Shared ReadOnly BlpWikiProjectBannerShellRegex As New Regex("[\s\n\r]*\|[\s\n\r]*blp[\s\n\r]*=[\s\n\r]*[Yy]es", _
           RegexOptions.Compiled Or RegexOptions.Singleline)
        Private Shared ReadOnly ActivePolWikiProjectBannerShellRegex As New  _
           Regex("[\s\n\r]*\|[\s\n\r]*activepol[\s\n\r]*=[\s\n\r]*[Yy]es", RegexOptions.Compiled Or RegexOptions.Singleline)

        ' Match evaluators:
        Private Function WPBSRegexMatchEvaluator(ByVal match As Match) As String
            Const templatename As String = "WikiProjectBannerShell"
            Dim Ending As String = match.Groups("end").Value

            ' Does the shell contain template: ?
            PluginCheckTemplateCall(match.Groups("tl").Value, templatename)
            ' Does the template have it's primary name:
            If Not match.Groups("tlname").Value = templatename Then RenamedATemplate(match.Groups("tlname").Value, _
               templatename, templatename)
            ' Do we need to add blp or activepol?
            If mActivePol AndAlso Not SearchedForActivePolInShell Then
                SearchedForActivePolInShell = True
                If Not ActivePolWikiProjectBannerShellRegex.IsMatch(Ending) Then
                    Ending = Microsoft.VisualBasic.vbCrLf & "|activepol=yes" & Ending
                    ArticleHasAMajorChange()
                    BannerShellParameterAdded("activepol")
                End If
            End If
            If mLiving AndAlso Not SearchedForLivingInShell Then
                SearchedForLivingInShell = True
                If Not BlpWikiProjectBannerShellRegex.IsMatch(Ending) Then
                    Ending = Microsoft.VisualBasic.vbCrLf & "|blp=yes" & Ending
                    ArticleHasAMajorChange()
                    BannerShellParameterAdded("blp")
                End If
            End If

            Return "{{" & templatename & "|1=" & Microsoft.VisualBasic.vbCrLf & MatchEvaluatorString & _
               match.Groups("body").Value & Ending & "}}"
        End Function
        Private Function WikiProjectBannersRegexMatchEvaluator(ByVal match As Match) As String

        End Function

        ' Where we (possibly) add our template to an existing shell:
        Friend Sub PrependTemplateOrWriteIntoShell(ByVal Template As Templating, ByVal ParameterBreak As String, _
        ByVal Text As String, ByVal PluginName As String)
            If WeFoundBannerShells = BannerShellsEnum.NotChecked Then
                If WikiProjectBannerShellRegex.IsMatch(AlteredArticleText) Then
                    WeFoundBannerShells = BannerShellsEnum.FoundWikiProjectBannerShell
                    'ElseIf WikiProjectBannersRegex.IsMatch(AlteredArticleText) Then
                    '    WeFoundBannerShells = BannerShellsEnum.FoundWikiProjectBanners
                Else
                    WeFoundBannerShells = BannerShellsEnum.NoneFound
                End If
            End If

            If WeFoundBannerShells = BannerShellsEnum.FoundWikiProjectBannerShell Then _
               Template.NewOrReplaceTemplateParm("nested", "yes", Me, True, False, PluginName:=PluginName)

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
        Private Sub BannerShellParameterAdded(ByVal Parameter As String)
            ParameterAdded(Parameter, "yes (shell)", "WikiProjectBannerShell")
        End Sub
        Friend Sub CheckLivingAndActivePolInWikiProjectBannerShell()
            ' We're writing our template to an existing shell, but we might be able to be good citizens and add blp or activepol
            ' TODO: Unfortunately, if we've already decided we have no substantial changes and are skipping the article we'll never get here (e.g. WPBio is present and already has all required params including living=yes, but blp=yes is missing)
            If (mLiving OrElse mActivePol) AndAlso WikiProjectBannerShellRegex.IsMatch(AlteredArticleText) Then
                AlteredArticleText = WikiProjectBannerShellRegex.Replace(AlteredArticleText, _
                   AddressOf Me.WPBSRegexMatchEvaluator, 1)
            End If
        End Sub
    End Class
End Namespace
