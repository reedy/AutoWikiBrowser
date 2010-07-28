'Copyright © 2008 Stephen Kennedy (Kingboyk) http://www.sdk-software.com/
'Copyright © 2008 Sam Reed (Reedy) http://www.reedyboy.net/

'This program is free software; you can redistribute it and/or modify it under the terms of Version 2 of the GNU General Public License as published by the Free Software Foundation.

'This program is distributed in the hope that it will be useful,
'but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

'You should have received a copy of the GNU General Public License Version 2 along with this program; if not, write to the Free Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA

Namespace AutoWikiBrowser.Plugins.Kingbotk
    ''' <summary>
    ''' SDK Software's base class for template-manipulating AWB plugins
    ''' </summary>
    Friend MustInherit Class PluginBase
        ' Settings:
        Protected Friend MustOverride ReadOnly Property PluginShortName() As String
        Protected MustOverride ReadOnly Property InspectUnsetParameters() As Boolean
        Protected Const ForceAddition As Boolean = True ' we might want to parameterise this later
        Protected MustOverride ReadOnly Property ParameterBreak() As String
        Protected Friend MustOverride ReadOnly Property GenericSettings() As IGenericSettings
        Protected MustOverride ReadOnly Property CategoryTalkClassParm() As String
        Protected MustOverride ReadOnly Property TemplateTalkClassParm() As String
        Friend MustOverride ReadOnly Property HasReqPhotoParam() As Boolean
        Friend MustOverride Sub ReqPhoto()

        ' Objects:
        Protected WithEvents OurMenuItem As ToolStripMenuItem
        Protected Article As Article
        Protected Template As Templating

        ' Regular expressions:
        Protected MainRegex As Regex
        Protected SecondChanceRegex As Regex
        Protected PreferredTemplateNameRegex As Regex
        Protected MustOverride ReadOnly Property PreferredTemplateName() As String
        Private Shared ReadOnly StubClassTemplateRegex As New Regex(conRegexpLeft & "Stubclass" & _
           ")\s*((\s*\|\s*(?<parm>[^}{|\s=]*)\s*)+(=\s*" & _
           "(?<val>[^|\n\r]*)\s*)?)*\}\}\s*", conRegexpOptions) ' value might contain {{!}} and spaces
        Protected Shared ReadOnly PreferredTemplateNameRegexCreator As _
           New Regex("^(?<first>[a-zA-Z0-9]{1})(?<second>.*)", RegexOptions.Compiled)

        Protected Function PreferredTemplateNameWikiMatchEvaluator(ByVal match As Match) As String
            Return "^[" & match.Groups("first").Value.ToUpper & match.Groups("first").Value.ToLower & "]" & _
               match.Groups("second").Value & "$"
        End Function

        ' AWB pass through:
        Protected Sub InitialiseBase()
            With OurMenuItem
                .CheckOnClick = True
                .Checked = False
                .ToolTipText = "Enable/disable the " & PluginShortName & " plugin"
            End With
            PluginManager.AWBForm.PluginsToolStripMenuItem.DropDownItems.Add(OurMenuItem)

            If Not Me.IAmGeneric Then _
               PluginManager.AddItemToTextBoxInsertionContextMenu(GenericSettings.TextInsertContextMenuStripItems)
        End Sub
        Protected Friend MustOverride Sub Initialise()
        Protected Friend MustOverride Sub ReadXML(ByVal Reader As XmlTextReader)
        Protected Friend MustOverride Sub Reset()
        Protected Friend MustOverride Sub WriteXML(ByVal Writer As XmlTextWriter)
        Protected Friend Function ProcessTalkPage(ByVal A As Article, ByVal AddReqPhotoParm As Boolean) As Boolean
            Return ProcessTalkPage(A, Classification.Code, Importance.Code, False, False, False, _
               ProcessTalkPageMode.Normal, AddReqPhotoParm)
        End Function
        Protected Friend Function ProcessTalkPage(ByVal A As Article, ByVal Classification As Classification, _
        ByVal Importance As Importance, ByVal ForceNeedsInfobox As Boolean, _
        ByVal ForceNeedsAttention As Boolean, ByVal RemoveAutoStub As Boolean, _
        ByVal ProcessTalkPageMode As ProcessTalkPageMode, ByVal AddReqPhotoParm As Boolean) As Boolean

            Dim BadTemplate As Boolean

            Me.Article = A

            If SkipIfContains() Then
                A.PluginIHaveFinished(SkipResults.SkipRegex, PluginShortName)
            Else
                ' MAIN
                Dim OriginalArticleText As String = A.AlteredArticleText

                Template = New Templating
                A.AlteredArticleText = MainRegex.Replace(A.AlteredArticleText, AddressOf Me.MatchEvaluator)

                If Template.BadTemplate Then
                    BadTemplate = True
                ElseIf Template.FoundTemplate Then
                    ' Even if we've found a good template bizarrely the page could still contain a bad template too 
                    If SecondChanceRegex.IsMatch(A.AlteredArticleText) OrElse TemplateFound() Then
                        BadTemplate = True
                    End If
                Else
                    If SecondChanceRegex.IsMatch(OriginalArticleText) Then
                        BadTemplate = True
                    ElseIf ForceAddition Then
                        TemplateNotFound()
                    End If
                End If

                ' OK, we're in business:
                ProcessTalkPage = True
                If Me.HasReqPhotoParam AndAlso AddReqPhotoParm Then Me.ReqPhoto()

                ProcessArticleFinish()
                If Not ProcessTalkPageMode = ProcessTalkPageMode.Normal Then
                    ProcessArticleFinishNonStandardMode(Classification, Importance, ForceNeedsInfobox, _
                       ForceNeedsAttention, RemoveAutoStub, ProcessTalkPageMode)
                End If

                If Article.ProcessIt Then
                    TemplateWritingAndPlacement()
                Else
                    A.AlteredArticleText = OriginalArticleText
                    A.PluginIHaveFinished(SkipResults.SkipNoChange, PluginShortName)
                End If
            End If

            If BadTemplate Then
                A.PluginIHaveFinished(SkipResults.SkipBadTag, PluginShortName) ' TODO: We could get the template placeholder here
            End If

            Article = Nothing
            Exit Function
        End Function

        ' Article processing:
        Protected MustOverride Sub InspectUnsetParameter(ByVal Param As String)
        Protected MustOverride Function SkipIfContains() As Boolean
        ''' <summary>
        ''' Send the template to the plugin for preinspection
        ''' </summary>
        ''' <returns>False if OK, TRUE IF BAD TAG</returns>
        Protected MustOverride Function TemplateFound() As Boolean
        Protected MustOverride Sub ProcessArticleFinish()
        Protected MustOverride Function WriteTemplateHeader(ByRef PutTemplateAtTop As Boolean) As String
        Protected MustOverride Sub ImportanceParameter(ByVal Importance As Importance)
        Protected Function MatchEvaluator(ByVal match As Match) As String
            If Not match.Groups("parm").Captures.Count = match.Groups("val").Captures.Count Then
                Template.BadTemplate = True
            Else
                Template.FoundTemplate = True
                Article.PluginCheckTemplateCall(match.Groups("tl").Value, PluginShortName)

                If HasAlternateNames Then PluginCheckTemplateName(match.Groups("tlname").Value) '.Trim)

                If match.Groups("parm").Captures.Count > 0 Then
                    For i As Integer = 0 To match.Groups("parm").Captures.Count - 1

                        Dim value As String = match.Groups("val").Captures(i).Value
                        Dim parm As String = match.Groups("parm").Captures(i).Value

                        If value = "" Then
                            If InspectUnsetParameters Then InspectUnsetParameter(parm)
                        Else
                            Template.AddTemplateParmFromExistingTemplate(parm, value)
                        End If
                    Next
                End If
            End If

            Return conTemplatePlaceholder
        End Function
        Protected Sub PluginCheckTemplateName(ByVal TemplateName As String)
            If HasAlternateNames Then
                If Not PreferredTemplateNameRegex.Match(TemplateName).Success Then
                    Article.RenamedATemplate(TemplateName, PreferredTemplateName, PluginShortName)
                    GotTemplateNotPreferredName(TemplateName)
                End If
            End If
        End Sub
        Protected MustOverride Sub GotTemplateNotPreferredName(ByVal TemplateName As String)
        Protected Overridable Sub TemplateNotFound()
            Article.ArticleHasAMajorChange()
            Template.NewTemplateParm("class", "")
            Article.TemplateAdded(PreferredTemplateName, PluginShortName)
        End Sub
        Private Sub TemplateWritingAndPlacement()
            Dim PutTemplateAtTop As Boolean
            Dim TemplateHeader As String = WriteTemplateHeader(PutTemplateAtTop)

            With Me.Article
                If Template.FoundTemplate Then
                    If Article.PageContainsShellTemplate() Then ' We're putting an existing template back into the shell where we found it
                        PluginManager.AWBForm.TraceManager.WriteArticleActionLine1( _
                          "Shell template found; leaving " & PreferredTemplateName & " where we found it", PluginShortName, True)
                        TemplateHeader = Article.LineBreakRegex.Replace(TemplateHeader, "") & Template.ParametersToString("")
                        .RestoreTemplateToPlaceholderSpot(TemplateHeader)
                    ElseIf PutTemplateAtTop Then ' moving existing tl to top
                        TemplateHeader += Template.ParametersToString(ParameterBreak)
                        .AlteredArticleText = TemplateHeader + .AlteredArticleText.Replace(conTemplatePlaceholder, "")
                    Else ' writing it back where it was
                        TemplateHeader += Template.ParametersToString(ParameterBreak)
                        .RestoreTemplateToPlaceholderSpot(TemplateHeader)
                    End If
                Else ' Our template wasn't found, write it into a shell or to the top of the page
                    .PrependTemplateOrWriteIntoShell(Template, ParameterBreak, TemplateHeader)
                End If
            End With
        End Sub
        Protected Sub AddAndLogNewParamWithAYesValue(ByVal ParamName As String)
            Template.NewOrReplaceTemplateParm(ParamName, "yes", Article, True, False, PluginName:=PluginShortName)
        End Sub
        Protected Sub AddNewParamWithAYesValue(ByVal ParamName As String)
            Template.NewOrReplaceTemplateParm(ParamName, "yes", Article, False, False, PluginName:=PluginShortName)
        End Sub
        Protected Sub AddAndLogNewParamWithAYesValue(ByVal ParamName As String, ByVal ParamAlternativeName As String)
            Template.NewOrReplaceTemplateParm(ParamName, "yes", Article, True, True, _
               ParamAlternativeName:=ParamAlternativeName, PluginName:=PluginShortName)
        End Sub
        Protected Sub AddAndLogEmptyParam(ByVal ParamName As String)
            If Not Template.Parameters.ContainsKey(ParamName) Then Template.NewTemplateParm(ParamName, "", True, _
            Article, PluginShortName)
        End Sub
        Protected Sub AddEmptyParam(ByVal ParamName As String)
            If Not Template.Parameters.ContainsKey(ParamName) Then Template.NewTemplateParm(ParamName, "", _
               False, Article, PluginShortName)
        End Sub
        Protected Sub ProcessArticleFinishNonStandardMode(ByVal Classification As Classification, _
        ByVal Importance As Importance, ByVal ForceNeedsInfobox As Boolean, _
        ByVal ForceNeedsAttention As Boolean, ByVal RemoveAutoStub As Boolean, _
        ByVal ProcessTalkPageMode As ProcessTalkPageMode)
            Select Case Classification
                Case Kingbotk.Classification.Code
                    If ProcessTalkPageMode = ProcessTalkPageMode.NonStandardTalk Then
                        Select Case Me.Article.Namespace
                            Case [Namespace].CategoryTalk
                                Template.NewOrReplaceTemplateParm( _
                                   "class", CategoryTalkClassParm, Me.Article, True, False, _
                                   PluginName:=PluginShortName)
                            Case [Namespace].TemplateTalk
                                Template.NewOrReplaceTemplateParm( _
                                   "class", TemplateTalkClassParm, Me.Article, True, False, _
                                   PluginName:=PluginShortName)
                            Case [Namespace].ImageTalk, 101, [Namespace].ProjectTalk '101 is Portal Talk
                                Template.NewOrReplaceTemplateParm( _
                                   "class", "NA", Me.Article, True, False, PluginName:=PluginShortName)
                        End Select
                    End If
                Case Kingbotk.Classification.Unassessed
                Case Else
                    Template.NewOrReplaceTemplateParm("class", Classification.ToString, Me.Article, False, False)
            End Select

            Select Case Importance
                Case Kingbotk.Importance.Code, Kingbotk.Importance.Unassessed
                Case Else
                    ImportanceParameter(Importance)
            End Select

            If ForceNeedsInfobox Then AddAndLogNewParamWithAYesValue("needs-infobox")

            If ForceNeedsAttention Then AddAndLogNewParamWithAYesValue("attention")

            If RemoveAutoStub Then
                With Me.Article
                    If Template.Parameters.ContainsKey("auto") Then
                        Template.Parameters.Remove("auto")
                        .ArticleHasAMajorChange()
                    End If

                    If StubClassTemplateRegex.IsMatch(.AlteredArticleText) Then
                        .AlteredArticleText = StubClassTemplateRegex.Replace(.AlteredArticleText, "")
                        .ArticleHasAMajorChange()
                    End If
                End With
            End If
        End Sub
        Protected Function WriteOutParameterToHeader(ByVal ParamName As String) As String
            With Template
                WriteOutParameterToHeader = "|" & ParamName & "="
                If .Parameters.ContainsKey(ParamName) Then
                    WriteOutParameterToHeader += .Parameters(ParamName).Value + ParameterBreak
                    .Parameters.Remove(ParamName)
                Else
                    WriteOutParameterToHeader += ParameterBreak
                End If
            End With
        End Function
        Protected Sub StubClass()
            If Me.Article.Namespace = [Namespace].Talk Then
                If GenericSettings.StubClass Then Template.NewOrReplaceTemplateParm("class", "Stub", Article, _
                   True, False, PluginName:=PluginShortName, DontChangeIfSet:=True)

                If GenericSettings.AutoStub _
                AndAlso Template.NewOrReplaceTemplateParm("class", "Stub", Article, True, False, _
                    PluginName:=PluginShortName, DontChangeIfSet:=True) _
                       Then AddAndLogNewParamWithAYesValue("auto")
                ' If add class=Stub (we don't change if set) add auto
            Else
                PluginManager.AWBForm.TraceManager.WriteArticleActionLine1( _
                   "Ignoring Stub-Class and Auto-Stub options; not a mainspace talk page", PluginShortName, True)
            End If
        End Sub
        Protected Sub ReplaceATemplateWithAYesParameter(ByVal R As Regex, ByVal ParamName As String, _
        ByVal TemplateCall As String, Optional ByVal Replace As Boolean = True)
            With Article
                If (R.Matches(.AlteredArticleText).Count > 0) Then
                    If Replace Then .AlteredArticleText = R.Replace(.AlteredArticleText, "")
                    .DoneReplacement(TemplateCall, ParamName & "=yes", True, PluginShortName)
                    Template.NewOrReplaceTemplateParm(ParamName, "yes", Article, False, False)
                    .ArticleHasAMinorChange()
                End If
            End With
        End Sub
        ''' <summary>
        ''' Checks if params which have two names (V8, v8) exist under both names
        ''' </summary>
        ''' <returns>True if BAD TAG</returns>
        Protected Function CheckForDoublyNamedParameters(ByVal Name1 As String, ByVal Name2 As String) As Boolean
            With Template.Parameters
                If .ContainsKey(Name1) AndAlso .ContainsKey(Name2) Then
                    If .Item(Name1).Value = .Item(Name2).Value Then
                        .Remove(Name2)
                        Article.DoneReplacement(Name2, "", True, PluginShortName)
                    Else
                        Return True
                    End If
                End If
            End With
        End Function

        ' Interraction with manager:
        Friend Property Enabled() As Boolean
            Get
                Return OurMenuItem.Checked
            End Get
            Set(ByVal IsEnabled As Boolean)
                OurMenuItem.Checked = IsEnabled
                ShowHideOurObjects(IsEnabled)
                PluginManager.PluginEnabledStateChanged(Me, IsEnabled)
            End Set
        End Property
        Protected Friend Overridable Sub BotModeChanged(ByVal BotMode As Boolean)
            If BotMode AndAlso GenericSettings.StubClass Then
                GenericSettings.AutoStub = True
                GenericSettings.StubClass = False
            End If
            GenericSettings.StubClassModeAllowed = Not BotMode
        End Sub
        Protected Friend Overridable ReadOnly Property IAmGeneric() As Boolean
            Get
                Return False
            End Get
        End Property

        ' User interface:
        Protected MustOverride Sub ShowHideOurObjects(ByVal Visible As Boolean)

        ' Event handlers:
        Private Sub ourmenuitem_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles OurMenuItem.CheckedChanged
            Enabled = OurMenuItem.Checked
        End Sub

        Friend Sub New(ByVal IAmAGenericTemplate As Boolean)
            If Not IAmAGenericTemplate Then Throw New NotSupportedException
        End Sub
    End Class

    Friend Interface IGenericSettings
        Property AutoStub() As Boolean
        Property StubClass() As Boolean
        WriteOnly Property StubClassModeAllowed() As Boolean
        ReadOnly Property TextInsertContextMenuStripItems() As ToolStripItemCollection
        Sub ReadXML(ByVal Reader As System.Xml.XmlTextReader)
        Sub WriteXML(ByVal Writer As System.Xml.XmlTextWriter)
        Sub XMLReset()
    End Interface
End Namespace