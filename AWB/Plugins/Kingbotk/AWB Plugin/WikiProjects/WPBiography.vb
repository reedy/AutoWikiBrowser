Imports AutoWikiBrowser.Plugins.Kingbotk.Templating

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

        Friend Sub New()
            MyBase.New("WikiProject Biography|Wpbiography|WPBIO|WP Biography|WPbiography|Wikiproject Biography|WP Bio|Bio") ' Specify alternate names only

            OurSettingsControl = New BioWithWorkgroups(PluginName, Prefix, True, False, params)
            OurSettingsControl.ExtraChecksText = "Force listas"
        End Sub

        Private Const PluginName As String = "WPBiography"
        Private Const Prefix As String = "Bio"

        Private Const WorkgroupsGroups As String = "Work Groups"
        Private Const OthersGroup As String = "Others"

        Dim params() As TemplateParameters =
        {
           New TemplateParameters() With {.StorageKey = "ActivePol", .Group = WorkgroupsGroups, .ParamName = "Politician"}, _
           New TemplateParameters() With {.StorageKey = "ArtsEntsWG", .Group = WorkgroupsGroups, .ParamName = "A&E"}, _
           New TemplateParameters() With {.StorageKey = "FilmWG", .Group = WorkgroupsGroups, .ParamName = "Film Bio"}, _
           New TemplateParameters() With {.StorageKey = "MilitaryWG", .Group = WorkgroupsGroups, .ParamName = "Military"}, _
           New TemplateParameters() With {.StorageKey = "PeerageWG", .Group = WorkgroupsGroups, .ParamName = "Peerage"}, _
           New TemplateParameters() With {.StorageKey = "RoyaltyWG", .Group = WorkgroupsGroups, .ParamName = "Royalty"}, _
           New TemplateParameters() With {.StorageKey = "MusiciansWG", .Group = WorkgroupsGroups, .ParamName = "Musician"}, _
           New TemplateParameters() With {.StorageKey = "ScienceWG", .Group = WorkgroupsGroups, .ParamName = "S&A"}, _
           New TemplateParameters() With {.StorageKey = "SportWG", .Group = WorkgroupsGroups, .ParamName = "Sports"}, _
           New TemplateParameters() With {.StorageKey = "LivingPerson", .Group = OthersGroup, .ParamName = "Living"}, _
           New TemplateParameters() With {.StorageKey = "NotLivingPerson", .Group = OthersGroup, .ParamName = "Not Living"}
       }

        ' Regular expressions:
        Private Shared ReadOnly BLPRegex As New Regex(TemplatePrefix & "blp\s*\}\}\s*", _
           RegexOptions.IgnoreCase Or RegexOptions.Compiled Or RegexOptions.ExplicitCapture)
        Private Shared ReadOnly ActivepolRegex As New Regex(TemplatePrefix & "(Activepolitician|Activepol)\s*\}\}\s*", _
           RegexOptions.IgnoreCase Or RegexOptions.Compiled Or RegexOptions.ExplicitCapture)

        ' Settings:
        Private OurTab As New TabPage(Constants.Biography)
        Private WithEvents OurSettingsControl As GenericWithWorkgroups

        Protected Friend Overrides ReadOnly Property PluginShortName() As String
            Get
                Return Constants.Biography
            End Get
        End Property
        Protected Overrides ReadOnly Property PreferredTemplateName() As String
            Get
                Return PluginName
            End Get
        End Property

        Protected Overrides Sub ImportanceParameter(ByVal Importance As Importance)
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

        ' Initialisation:
        Protected Friend Overrides Sub Initialise()
            OurMenuItem = New ToolStripMenuItem("Biography Plugin")
            MyBase.InitialiseBase() ' must set menu item object first
            OurTab.UseVisualStyleBackColor = True
            OurTab.Controls.Add(OurSettingsControl)
        End Sub

        ' Article processing:
        Protected Overrides ReadOnly Property InspectUnsetParameters() As Boolean
            Get
                Return False
            End Get
        End Property
        Protected Overrides Sub InspectUnsetParameter(ByVal Param As String)
        End Sub
        Protected Overrides Function SkipIfContains() As Boolean
            Return False
        End Function
        Protected Overrides Sub ProcessArticleFinish()
            Dim Living As Living = Living.Unknown, LivingAlreadyAddedToEditSummary As Boolean
            Const conContainsDefaultSortKeyword As String = "Page contains DEFAULTSORT keyword: "

            With article
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

            StubClass()

            With OurSettingsControl
                For Each lvi As ListViewItem In .ListView1.Items
                    If lvi.Checked Then
                        Dim tp As TemplateParameters = DirectCast(lvi.Tag, TemplateParameters)

                        If tp.Group = WorkgroupsGroups Then
                            Dim param As String = tp.ParamName.ToLower().Replace(" ", "-")
                            AddAndLogNewParamWithAYesValue(param & "-work-group") 'Probably needs some reformatting
                            AddEmptyParam(param & "-priority")
                        Else
                            Select Case tp.ParamName
                                Case "Not Living"
                                    Living = Plugins.Living.Dead
                                Case "Living"
                                    Living = Plugins.Living.Living
                            End Select
                        End If
                    End If
                Next
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
                        Template.NewOrReplaceTemplateParm("living", "no", article, True, False, False, _
                        "", PluginShortName, True)
                    End If
                Case Plugins.Living.Unknown
                    Template.NewOrReplaceTemplateParm("living", "", article, False, False, True, _
                        "", PluginShortName, True)
            End Select

            With article

                If .Namespace = [Namespace].Talk AndAlso (.ProcessIt OrElse OurSettingsControl.ExtraChecks) Then
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
                        WikiFunctions.Tools.MakeHumanCatKey(article.FullArticleTitle), article, _
                        True, False, True, "", PluginShortName)
                    End If
                End If
            End With
        End Sub
        ''' <summary>
        ''' Send the template to the plugin for preinspection
        ''' </summary>
        ''' <returns>False if OK, TRUE IF BAD TAG</returns>
        Protected Overrides Function TemplateFound() As Boolean
            With Template
                If .Parameters.ContainsKey("importance") Then
                    .Parameters.Remove("importance")
                    article.ArticleHasAMinorChange()
                End If
                If .Parameters.ContainsKey("priority") Then
                    Dim priorityValue As String = .Parameters("priority").Value

                    For Each kvp As KeyValuePair(Of String, TemplateParametersObject) In .Parameters
                        If kvp.Key.Contains("-priority") AndAlso kvp.Value.Value <> "" Then
                            kvp.Value.Value = priorityValue
                        End If
                    Next

                    .Parameters.Remove("priority")
                    article.ArticleHasAMinorChange()
                End If
            End With

            Return False
        End Function
        Protected Overrides Sub GotTemplateNotPreferredName(ByVal TemplateName As String)
        End Sub
        Protected Overrides Function WriteTemplateHeader(ByRef PutTemplateAtTop As Boolean) As String
            Dim Living As Boolean
            Dim res As String = "{{WPBiography" & Microsoft.VisualBasic.vbCrLf

            With Template
                If .Parameters.ContainsKey("living") Then
                    .Parameters("living").Value = .Parameters("living").Value.ToLower
                    res += "|living=" + .Parameters("living").Value + ParameterBreak

                    Living = .Parameters("living").Value = "yes"

                    If Living Then
                        PluginManager.AWBForm.TraceManager.WriteArticleActionLine1( _
       "Template contains living=yes, placing at top", PluginShortName, True)
                        PutTemplateAtTop = True ' otherwise, leave as False
                    End If

                    .Parameters.Remove("living") ' we've written this parameter; if we leave it in the collection PluginBase.TemplateWritingAndPlacement() will write it again
                End If

                res += WriteOutParameterToHeader("class")
            End With

            Return res
        End Function

        'User interface:
        Protected Overrides Sub ShowHideOurObjects(ByVal Visible As Boolean)
            PluginManager.ShowHidePluginTab(OurTab, Visible)
        End Sub

        ' XML settings:
        Protected Friend Overrides Sub ReadXML(ByVal Reader As System.Xml.XmlTextReader)
            Dim blnNewVal As Boolean = PluginManager.XMLReadBoolean(Reader, Prefix & "Enabled", Enabled)
            If Not blnNewVal = Enabled Then Enabled = blnNewVal ' Mustn't set if the same or we get extra tabs

            OurSettingsControl.ExtraChecks = PluginManager.XMLReadBoolean(Reader, Prefix & "ForceListasParm", OurSettingsControl.ExtraChecks)
            OurSettingsControl.ReadXML(Reader)
        End Sub
        Protected Friend Overrides Sub Reset()
            OurSettingsControl.Reset()
        End Sub
        Protected Friend Overrides Sub WriteXML(ByVal Writer As System.Xml.XmlTextWriter)
            Writer.WriteAttributeString(Prefix & "Enabled", Enabled.ToString)
            Writer.WriteAttributeString(Prefix & "ForceListasParm", OurSettingsControl.ExtraChecks.ToString)
            OurSettingsControl.WriteXML(Writer)
        End Sub

        ' Other overrides:
        Protected Friend Overrides Sub BotModeChanged(ByVal BotMode As Boolean)
            MyBase.BotModeChanged(BotMode)

            OurSettingsControl.InspectUnsetParameters = Not BotMode
            If BotMode Then OurSettingsControl.ExtraChecks = False
        End Sub

        Private Function conSkierBotPlaceholder() As Object
            Throw New NotImplementedException
        End Function

    End Class
End Namespace