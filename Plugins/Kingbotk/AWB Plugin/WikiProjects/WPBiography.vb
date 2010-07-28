'Copyright © 2008 Stephen Kennedy (Kingboyk) http://www.sdk-software.com/
'Copyright © 2008 Sam Reed (Reedy) http://www.reedyboy.net/

'This program is free software; you can redistribute it and/or modify it under the terms of Version 2 of the GNU General Public License as published by the Free Software Foundation.

'This program is distributed in the hope that it will be useful,
'but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

'You should have received a copy of the GNU General Public License Version 2 along with this program; if not, write to the Free Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA

Namespace AutoWikiBrowser.Plugins.Kingbotk.Plugins

    Friend Enum Living
        Unknown
        Living
        Dead
    End Enum

    Friend NotInheritable Class WPBiography
        Inherits PluginBase

        ' Regular expressions:
        Private Shared ReadOnly BLPRegex As New Regex(TemplatePrefix & "blp\s*\}\}\s*", _
           RegexOptions.IgnoreCase Or RegexOptions.Compiled Or RegexOptions.ExplicitCapture)
        Private Shared ReadOnly ActivepolRegex As New Regex(TemplatePrefix & "(Activepolitician|Activepol)\s*\}\}\s*", _
           RegexOptions.IgnoreCase Or RegexOptions.Compiled Or RegexOptions.ExplicitCapture)
        Private Shared ReadOnly SkierBotRegex As New Regex( _
           "<!-- begin Bot added message -->\s*This article has been automatically assessed as.*Biography.*?<!-- end Bot added message -->", _
           RegexOptions.Compiled Or RegexOptions.Singleline) ' Bizarrely, Singleline causes a "." to match linebreaks, Multiline doesn't! :) http://www.thescripts.com/forum/thread223868.html
        Private Shared ReadOnly SkierBotPlaceholderRegex As New Regex(Regex.Escape(conSkierBotPlaceholder), RegexOptions.Compiled)

        ' Strings:
        Private Const conStringsRoyaltyWorkGroup As String = "royalty-work-group"
        Private Const conStringsBritishRoyaltyWorkGroup As String = "british-royalty"
        Private Const conStringsMusicianWorkGroup As String = "musician-work-group"
        Private Const conStringsArtsWorkGroup As String = "a&e-work-group"
        Private Const conStringsFilmWorkGroup As String = "filmbio-work-group"
        Private Const conSkierBotPlaceholder As String = "<xxxSKIERBOTPLACEHOLDERxxx>"

        ' Settings:
        Private OurTab As New TabPage(Constants.Biography)
        Private WithEvents OurSettingsControl As New WPBiographySettings
        Private Const conEnabled As String = "BioEnabled"
        Protected Friend Overrides ReadOnly Property PluginShortName() As String
            Get
                Return Constants.Biography
            End Get
        End Property
        Protected Overrides ReadOnly Property PreferredTemplateName() As String
            Get
                Return "WPBiography"
            End Get
        End Property

        Protected Overrides Sub ImportanceParameter(ByVal Importance As Importance)
            Template.NewOrReplaceTemplateParm("priority", Importance.ToString, Me.Article, False, False)
        End Sub
        Protected Friend Overrides ReadOnly Property GenericSettings() As IGenericSettings
            Get
                Return OurSettingsControl
            End Get
        End Property
        Protected Overrides ReadOnly Property CategoryTalkClassParm() As String
            Get
                Return "Cat"
            End Get
        End Property
        Protected Overrides ReadOnly Property TemplateTalkClassParm() As String
            Get
                Return "Template"
            End Get
        End Property
        Friend Overrides ReadOnly Property HasReqPhotoParam() As Boolean
            Get
                Return True
            End Get
        End Property
        Friend Overrides Sub ReqPhoto()
            AddNewParamWithAYesValue("needs-photo")
        End Sub
        'Protected Overrides ReadOnly Property PreferredTemplateNameRegexString() As String
        '    Get
        '        Return "^[Ww]PBiography$"
        '    End Get
        'End Property

        ' Initialisation:
        Friend Sub New()
            MyBase.New("WikiProject Biography|Wpbiography|WPBIO|WP Biography|WPbiography|Wikiproject Biography|WP Bio|Bio") ' Specify alternate names only
        End Sub
        Protected Friend Overrides Sub Initialise()
            OurMenuItem = New ToolStripMenuItem("Biography Plugin")
            MyBase.InitialiseBase() ' must set menu item object first
            OurTab.UseVisualStyleBackColor = True
            OurTab.Controls.Add(OurSettingsControl)
        End Sub

        ' Article processing:
        Protected Overrides ReadOnly Property InspectUnsetParameters() As Boolean
            Get
                Return OurSettingsControl.ForcePriorityParm
            End Get
        End Property
        Protected Overrides Sub InspectUnsetParameter(ByVal Param As String)
            ' We only get called if InspectUnsetParameters is True
            If String.Equals(Param, "importance", StringComparison.CurrentCultureIgnoreCase) Then
                Template.AddTemplateParmFromExistingTemplate("priority", "") ' NewTemplateParm could throw an error when importance= existed and was changed to priority and then we find another and do the same
                Article.DoneReplacement("importance", "priority", True, PluginShortName)
            End If
        End Sub
        Protected Overrides Function SkipIfContains() As Boolean
            ' We'll also do SkierBot tidying here, as this is the first chance we get to process the article outside Pluginbase
            Article.AlteredArticleText = SkierBotRegex.Replace(Article.AlteredArticleText, conSkierBotPlaceholder)
            Return False
        End Function
        Protected Overrides Sub ProcessArticleFinish()
            Dim Living As Living = OurSettingsControl.DeadOrAlive, LivingAlreadyAddedToEditSummary As Boolean
            Const conContainsDefaultSortKeyword As String = "Page contains DEFAULTSORT keyword: "

            With Article
                If BLPRegex.Matches(.AlteredArticleText).Count > 0 Then
                    .AlteredArticleText = BLPRegex.Replace(.AlteredArticleText, "")
                    .DoneReplacement("{{[[Template:Blp|Blp]]}}", "living=yes", True, PluginShortName)
                    Living = Plugins.Living.Living
                    LivingAlreadyAddedToEditSummary = True
                    .ArticleHasAMinorChange()
                End If

                If ActivepolRegex.Matches(.AlteredArticleText).Count > 0 Then
                    .AlteredArticleText = ActivepolRegex.Replace(.AlteredArticleText, "")
                    .DoneReplacement("{{[[Template:Active politician|Activepolitician]]}}", "activepol=yes", _
                       True, PluginShortName)
                    AddNewParamWithAYesValue("activepol")
                    .ArticleHasAMinorChange()
                End If
            End With

            Select Case Living
                Case Plugins.Living.Living
                    If Not Template.HasYesParamLowerOrTitleCase(True, "living") Then
                        If LivingAlreadyAddedToEditSummary Then
                            AddNewParamWithAYesValue("living")
                        Else
                            AddAndLogNewParamWithAYesValue("living")
                        End If
                    End If
                Case Plugins.Living.Dead
                    If Not Template.HasYesParamLowerOrTitleCase(False, "living") Then
                        Template.NewOrReplaceTemplateParm("living", "no", Article, True, False, False, _
                        "", PluginShortName, True)
                    End If
            End Select

            StubClass()

            With OurSettingsControl
                If .ActivePol Then AddAndLogNewParamWithAYesValue("activepol")
                If .NonBio Then AddAndLogNewParamWithAYesValue("non-bio")
                If .ArtsEntsWG AndAlso Not Template.HasYesParam(conStringsMusicianWorkGroup) _
                AndAlso Not Template.HasYesParam(conStringsFilmWorkGroup) Then _
                   AddAndLogNewParamWithAYesValue(conStringsArtsWorkGroup)
                If .MilitaryWG Then AddAndLogNewParamWithAYesValue("military-work-group")
                If .RoyaltyWG AndAlso Not Template.HasYesParam(conStringsBritishRoyaltyWorkGroup) Then _
                   AddAndLogNewParamWithAYesValue(conStringsRoyaltyWorkGroup)
                If .ScientistWG Then AddAndLogNewParamWithAYesValue("s&a-work-group")
                If .PoliticianWG Then AddAndLogNewParamWithAYesValue("politician-work-group")
                If .SportsWG Then AddAndLogNewParamWithAYesValue("sports-work-group")
                If .BaronetsWG Then AddAndLogNewParamWithAYesValue("baronets-work-group")
                If .PeerageWG Then AddAndLogNewParamWithAYesValue("peerage-work-group")
                Template.RemoveParentWorkgroup(conStringsBritishRoyaltyWorkGroup, conStringsRoyaltyWorkGroup, _
                   .BritishRoyaltyWG, Article, PluginShortName)
                Template.RemoveParentWorkgroup(conStringsMusicianWorkGroup, conStringsArtsWorkGroup, _
                   .MusiciansWG, Article, PluginShortName)
                Template.RemoveParentWorkgroup(conStringsFilmWorkGroup, conStringsArtsWorkGroup, _
                   .FilmWG, Article, PluginShortName)
            End With

            With Article

                If .Namespace = [Namespace].Talk AndAlso (.ProcessIt OrElse OurSettingsControl.ForceListAsParm) Then
                    If WikiFunctions.TalkPages.TalkPageHeaders.ContainsDefaultSortKeywordOrTemplate( _
                    .AlteredArticleText) Then
                        If Template.Parameters.ContainsKey("listas") Then
                            Template.Parameters.Remove("listas")
                            PluginManager.AWBForm.TraceManager.WriteArticleActionLine(conContainsDefaultSortKeyword & _
                               "removing listas parameter", PluginShortName)
                            .ArticleHasAMajorChange()
                        ElseIf Not PluginManager.BotMode Then
                            PluginManager.AWBForm.TraceManager.WriteArticleActionLine1(conContainsDefaultSortKeyword & _
                               "not adding listas parameter", PluginShortName, True)
                        End If
                    ElseIf Not PluginManager.BotMode Then
                        ' Since we're dealing with talk pages, we want a listas= even if it's the same as the
                        ' article title without namespace (otherwise it sorts to namespace)
                        Template.NewOrReplaceTemplateParm("listas", _
                        WikiFunctions.Tools.MakeHumanCatKey(Article.FullArticleTitle), Article, _
                        True, False, True, "", PluginShortName)
                    End If
                End If
            End With

            ReplaceATemplateWithAYesParameter(SkierBotPlaceholderRegex, "auto", "boilerplate text")

        End Sub
        ''' <summary>
        ''' Send the template to the plugin for preinspection
        ''' </summary>
        ''' <returns>False if OK, TRUE IF BAD TAG</returns>
        Protected Overrides Function TemplateFound() As Boolean
            With Template
                If .Parameters.ContainsKey("importance") Then
                    If .Parameters.ContainsKey("priority") Then
                        If Not .Parameters("importance").Value = .Parameters("priority").Value AndAlso _
                        Not .Parameters("importance").Value = "" AndAlso Not .Parameters("priority").Value = "" Then
                            ' if they're not equal and both are not empty we have a bad tag
                            Return True
                        Else
                            Article.EditSummary += "rm importance param, has priority=, "
                            PluginManager.AWBForm.TraceManager.WriteArticleActionLine( _
                               "importance parameter removed, has priority=", PluginShortName)
                        End If
                    Else
                        .Parameters.Add("priority", _
                           New Templating.TemplateParametersObject("priority", _
                           .Parameters("importance").Value))
                        Article.DoneReplacement("importance", "priority", True, PluginShortName)
                    End If
                    .Parameters.Remove("importance")
                    Article.ArticleHasAMinorChange()
                End If
            End With
        End Function
        Protected Overrides Sub GotTemplateNotPreferredName(ByVal TemplateName As String)
        End Sub
        Protected Overrides Function WriteTemplateHeader(ByRef PutTemplateAtTop As Boolean) As String
            Dim Living As Boolean
            WriteTemplateHeader = "{{WPBiography" & Microsoft.VisualBasic.vbCrLf

            With Template
                If .Parameters.ContainsKey("living") Then
                    .Parameters("living").Value = .Parameters("living").Value.ToLower
                    WriteTemplateHeader += "|living=" + .Parameters("living").Value + ParameterBreak

                    Living = .Parameters("living").Value = "yes"

                    If Living Then
                        PluginManager.AWBForm.TraceManager.WriteArticleActionLine1( _
       "Template contains living=yes, placing at top", PluginShortName, True)
                        PutTemplateAtTop = True ' otherwise, leave as False
                    End If

                    .Parameters.Remove("living") ' we've written this parameter; if we leave it in the collection PluginBase.TemplateWritingAndPlacement() will write it again
                End If

                WriteTemplateHeader += WriteOutParameterToHeader("class") & _
                   WriteOutParameterToHeader("priority")
            End With
        End Function

        'User interface:
        Protected Overrides Sub ShowHideOurObjects(ByVal Visible As Boolean)
            PluginManager.ShowHidePluginTab(OurTab, Visible)
        End Sub

        ' XML settings:
        Protected Friend Overrides Sub ReadXML(ByVal Reader As System.Xml.XmlTextReader)
            Dim blnNewVal As Boolean = PluginManager.XMLReadBoolean(Reader, conEnabled, Enabled)
            If Not blnNewVal = Enabled Then Enabled = blnNewVal ' Mustn't set if the same or we get extra tabs
            OurSettingsControl.ReadXML(Reader)
        End Sub
        Protected Friend Overrides Sub Reset()
            OurSettingsControl.Reset()
        End Sub
        Protected Friend Overrides Sub WriteXML(ByVal Writer As System.Xml.XmlTextWriter)
            Writer.WriteAttributeString(conEnabled, Enabled.ToString)
            OurSettingsControl.WriteXML(Writer)
        End Sub

        ' Other overrides:
        Protected Friend Overrides Sub BotModeChanged(ByVal BotMode As Boolean)
            MyBase.BotModeChanged(BotMode)
            OurSettingsControl.BotMode = BotMode
        End Sub
    End Class
End Namespace