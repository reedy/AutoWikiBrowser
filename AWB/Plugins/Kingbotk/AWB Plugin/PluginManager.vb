'Copyright © 2008 Stephen Kennedy (Kingboyk) http://www.sdk-software.com/
'Copyright © 2008 Sam Reed (Reedy) http://www.reedyboy.net/

'This program is free software; you can redistribute it and/or modify it under the terms of Version 2 of the GNU General Public License as published by the Free Software Foundation.

'This program is distributed in the hope that it will be useful,
'but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

'You should have received a copy of the GNU General Public License Version 2 along with this program; if not, write to the Free Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
Imports WikiFunctions.API

<Assembly: CLSCompliant(True)> 
Namespace AutoWikiBrowser.Plugins.Kingbotk
    ''' <summary>
    ''' The plugin manager, which interracts with AWB and manages the individual plugins
    ''' </summary>
    ''' <remarks></remarks>
    Public NotInheritable Class PluginManager
        ' Fields here shouldn't need to be Shared, as there will only ever be one instance - created by AWB at startup
        Implements IAWBPlugin

        Private Const conMe As String = "Kingbotk Plugin Manager"

        ' Regular expressions:
        Private Shared ReadOnly ReqPhotoNoParamsRegex As New Regex(TemplatePrefix & "reqphoto\s*\}\}\s*", _
           RegexOptions.IgnoreCase Or RegexOptions.Compiled Or RegexOptions.ExplicitCapture)

        ' Plugins:
        Friend Shared ActivePlugins As New List(Of PluginBase)
        Private Shared Plugins As New Dictionary(Of String, PluginBase)
        Private WithEvents AssessmentsObject As Assessments

        'AWB objects:
        Friend Shared WithEvents AWBForm As IAutoWikiBrowser
        Friend Shared WithEvents StatusText As New ToolStripStatusLabel("Initialising plugin")

        ' Menu items:
        Private Shared WithEvents AddGenericTemplateMenuItem As New ToolStripMenuItem("Add Generic Template")
        Private Shared WithEvents MenuShowSettingsTabs As New ToolStripMenuItem("Show settings tabs")

        ' Library state and shared objects:
        Private Shared KingbotkPluginTab As New TabPage("Plugin")
        Private Shared PluginSettings As PluginSettingsControl

        ' User settings:
        Private Shared blnShowManualAssessmentsInstructions As Boolean = True

        ' SkipReason:
        Private Enum SkipReason As Byte
            Other
            BadNamespace
            BadTag
            ProcessingMainArticleDoesntExist
            ProcessingTalkPageArticleDoesntExist
            NoChange
            Regex
        End Enum

        ' XML:
        Private Const conShowHideTabsParm As String = "ShowHideTabs"
        Private Const conShowManualAssessmentsInstructions As String = "ShowManualAssessmentsInstructions"
        Private Const conGenericTemplatesCount As String = "GenericTemplatesCount"
        Private Const conGenericTemplate As String = "GenericTemplate"

        ' AWB interface:
        Private ReadOnly Property Name() As String Implements IAWBPlugin.Name
            Get
                Return conAWBPluginName
            End Get
        End Property
        Private Sub Initialise(ByVal MainForm As IAutoWikiBrowser) Implements IAWBPlugin.Initialise
            ' Store AWB object reference:
            AWBForm = MainForm

            ' Initialise our settings object:
            PluginSettings = New PluginSettingsControl()

            With AWBForm
                ' Set up our UI objects:
                With .BotModeCheckbox
                    AddHandler .EnabledChanged, AddressOf PluginManager.AWBBotModeCheckboxEnabledChangedHandler
                    AddHandler .CheckedChanged, AddressOf PluginManager.AWBBotModeCheckboxCheckedChangeHandler
                End With
                .StatusStrip.Items.Insert(2, StatusText)
                StatusText.Margin = New Padding(50, 0, 50, 0)
                StatusText.BorderSides = ToolStripStatusLabelBorderSides.Left Or ToolStripStatusLabelBorderSides.Right
                StatusText.BorderStyle = Border3DStyle.Etched
                .HelpToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() _
                   {New ToolStripSeparator, PluginSettings.MenuHelp, PluginSettings.MenuAbout})
            End With

            ' UI - addhandlers for Start/Stop/Diff/Preview/Save/Ignore buttons/form closing:
            AddHandler AWBForm.Form.FormClosing, AddressOf PluginManager.AWBClosingEventHandler

            ' Handle over events from AWB:
            AddHandler AWBForm.StopButton.Click, AddressOf Me.StopButtonClickEventHandler
            AddHandler AWBForm.TheSession.StateChanged, AddressOf PluginManager.EditorStatusChanged
            AddHandler AWBForm.TheSession.Aborted, AddressOf PluginManager.EditorAborted

            ' Track Manual Assessment checkbox:
            AddHandler PluginSettings.ManuallyAssessCheckBox.CheckedChanged, _
               AddressOf Me.ManuallyAssessCheckBox_CheckChanged

            ' Tabs:
            KingbotkPluginTab.UseVisualStyleBackColor = True
            KingbotkPluginTab.Controls.Add(PluginSettings)

            ' Show/hide tabs menu:
            With MenuShowSettingsTabs
                .CheckOnClick = True
                .Checked = True
            End With
            AWBForm.ToolStripMenuGeneral.DropDownItems.Add(MenuShowSettingsTabs)

            ' Add-Generic-Template menu:
            AWBForm.PluginsToolStripMenuItem.DropDownItems.Add(AddGenericTemplateMenuItem)

            ' Universal context menus:
            'AddItemToTextBoxInsertionContextMenu(PluginSettings.TextInsertContextMenuStrip.Items) ' wasn't working (nor was iteration)
            AddItemToTextBoxInsertionContextMenu(PluginSettings.ClassToolStripMenuItem)
            AddItemToTextBoxInsertionContextMenu(PluginSettings.ImportanceToolStripMenuItem)
            AddItemToTextBoxInsertionContextMenu(PluginSettings.PriorityToolStripMenuItem)

            ' Create plugins:
            Plugins.Add("Albums", New WPAlbums(Me))
            Plugins.Add("Australia", New WPAustralia(Me))
            'Plugins.Add("Film", New Film(Me))
            Plugins.Add("India", New WPIndia(Me))
            Plugins.Add("MilHist", New WPMilHist(Me))
            Plugins.Add("Songs", New WPSongs(Me))
            Plugins.Add("WPNovels", New WPNovels(Me))
            Plugins.Add(Constants.Biography, New WPBiography(Me))
            ' hopefully if add WPBio last it will ensure that the template gets added to the *top* of pages

            ' Initialise plugins:
            For Each plugin As KeyValuePair(Of String, PluginBase) In Plugins
                plugin.Value.Initialise()
            Next

            ' Add our menu items last:
            AWBForm.PluginsToolStripMenuItem.DropDownItems.Add(PluginSettings.PluginToolStripMenuItem)

            ' Reset statusbar text:
            DefaultStatusText()
        End Sub
        Private Sub LoadSettings(ByVal Prefs() As Object) Implements IAWBPlugin.LoadSettings
            If Prefs.Length > 0 Then
                ' Check if we're receiving an old type settings block (XMLTextReader) or new (a serialized string)
                If Prefs(0).GetType Is GetType(XmlTextReader) Then
                    ReadXML(DirectCast(Prefs(0), XmlTextReader))
                ElseIf Prefs(0).GetType Is GetType(String) Then
                    LoadSettingsNewWay(CType(Prefs(0), String))
                End If
            End If
        End Sub
        Private Function ProcessArticle(ByVal sender As IAutoWikiBrowser, _
        ByVal ProcessArticleEventArgs As IProcessArticleEventArgs) As String Implements IAWBPlugin.ProcessArticle
            With ProcessArticleEventArgs
                If ActivePlugins.Count = 0 Then Return .ArticleText

                Dim TheArticle As Article = Nothing, Namesp As Integer = .NameSpaceKey

                PluginSettings.Led1.Colour = WikiFunctions.Controls.Colour.Green
                StatusText.Text = "Processing " & .ArticleTitle
                PluginManager.AWBForm.TraceManager.ProcessingArticle(.ArticleTitle, Namesp)

                For Each p As PluginBase In ActivePlugins
                    Try
                        If Not p.IAmReady AndAlso p.IAmGeneric Then
                            MessageBox.Show("The generic template plugin """ & p.PluginShortName & _
                               """isn't properly configured.", "Can't start", MessageBoxButtons.OK, _
                               MessageBoxIcon.Error)
                            StopAWB()
                            GoTo SkipOrStop
                        End If
                    Catch ex As RedirectsException
                        StopAWB()
                        GoTo SkipOrStop
                    Catch
                        Throw
                    End Try
                Next

                Select Case Namesp
                    Case [Namespace].Mainspace
                        If PluginSettings.ManuallyAssess Then
                            If .Exists = Exists.Yes Then
                                StatusText.Text += ": Click Preview to read the article; " & _
                                   "click Save or Ignore to load the assessments form"
                                AssessmentsObject.ProcessMainSpaceArticle(.ArticleTitle)
                                .EditSummary = "Clean up"
                                GoTo SkipOrStop
                            Else
                                ProcessArticle = Skipping(.EditSummary, "", _
                                   SkipReason.ProcessingMainArticleDoesntExist, .ArticleText, .Skip)
                                GoTo ExitMe
                            End If
                        Else
                            GoTo SkipBadNamespace
                        End If

                    Case [Namespace].Talk
                        Dim editor As AsyncApiEdit = PluginManager.AWBForm.TheSession.Editor.Clone()

                        editor.Open(Tools.ConvertFromTalk(.ArticleTitle), False)

                        editor.Wait()

                        If Not editor.Page.Exists Then
                            ProcessArticle = Skipping(.EditSummary, "", SkipReason.ProcessingTalkPageArticleDoesntExist, _
                               .ArticleText, .Skip, .ArticleTitle, [Namespace].Talk)
                        Else
                            TheArticle = New Article(.ArticleText, .ArticleTitle, Namesp)

                            If Not PluginSettings.ManuallyAssess Then _
                               TheArticle.PluginManagerEditSummaryTaggingCategory(PluginSettings.CategoryName)

                            Dim ReqPhoto As Boolean = ReqPhotoParamNeeded(TheArticle)

                            If PluginSettings.ManuallyAssess Then
                                If Not AssessmentsObject.ProcessTalkPage(TheArticle, PluginSettings, Me, ReqPhoto) Then ' reqphoto byref
                                    .Skip = True
                                    GoTo SkipOrStop
                                End If
                            Else
                                ReqPhoto = ProcessTalkPageAndCheckWeAddedReqPhotoParam(TheArticle, _
                                   ReqPhoto) ' We successfully added a reqphoto param
                            End If

                            ProcessArticle = FinaliseArticleProcessing(TheArticle, .Skip, .EditSummary, .ArticleText, _
                               ReqPhoto)
                        End If

                    Case [Namespace].CategoryTalk, [Namespace].ImageTalk, 101, _
                       [Namespace].ProjectTalk, [Namespace].TemplateTalk '101 is Portal Talk 
                        If PluginSettings.ManuallyAssess Then
                            MessageBox.Show("The plugin has received a non-standard namespace talk page in " & _
                               "manual assessment mode. Please remove this item from the list and start again.", _
                               "Manual Assessments", MessageBoxButtons.OK, MessageBoxIcon.Error)
                            StopAWB()
                            GoTo SkipOrStop
                        Else
                            TheArticle = New Article(.ArticleText, .ArticleTitle, Namesp)
                            TheArticle.PluginManagerEditSummaryTaggingCategory(PluginSettings.CategoryName)

                            For Each p As PluginBase In ActivePlugins
                                p.ProcessTalkPage(TheArticle, Classification.Code, Importance.NA, False, _
                                   False, False, ProcessTalkPageMode.NonStandardTalk, False)
                                If TheArticle.PluginManagerGetSkipResults = SkipResults.SkipBadTag Then _
                                   Exit For
                            Next

                            ProcessArticle = FinaliseArticleProcessing(TheArticle, .Skip, .EditSummary, .ArticleText, False)
                        End If

                    Case Else
                        GoTo SkipBadNamespace
                End Select

                If Not .Skip Then
                    'TempHackInsteadOfDefaultSettings:
                    If AWBForm.EditSummaryComboBox.Text = "clean up" Then AWBForm.EditSummaryComboBox.Text = "Tagging"
                End If
            End With

ExitMe:
            If Not PluginSettings.ManuallyAssess Then DefaultStatusText()
            PluginManager.AWBForm.TraceManager.Flush()

            PluginSettings.Led1.Colour = WikiFunctions.Controls.Colour.Red
            Exit Function

SkipBadNamespace:
            ProcessArticle = Skipping(ProcessArticleEventArgs.EditSummary, "", SkipReason.BadNamespace, ProcessArticleEventArgs.ArticleText, ProcessArticleEventArgs.Skip)
            GoTo ExitMe

SkipOrStop:
            ProcessArticle = ProcessArticleEventArgs.ArticleText
            GoTo ExitMe
        End Function
        Private Sub Reset() Implements IAWBPlugin.Reset
            blnShowManualAssessmentsInstructions = True
            With PluginSettings
                .Reset()
                .SkipBadTags = BotMode
                .SkipWhenNoChange = BotMode
            End With
            For Each plugin As KeyValuePair(Of String, PluginBase) In Plugins
                plugin.Value.Reset()
            Next
        End Sub
        Private Function SaveSettings() As Object() Implements IAWBPlugin.SaveSettings
            Dim st As New IO.StringWriter
            Dim Writer As New System.Xml.XmlTextWriter(st)

            Writer.WriteStartElement("root")
            WriteXML(Writer)
            Writer.WriteEndElement()
            Writer.Flush()

            Return New Object() {st.ToString}
        End Function

        ' Private routines:
        Private Shared Function ProcessTalkPageAndCheckWeAddedReqPhotoParam(ByVal TheArticle As Article, _
        ByVal ReqPhoto As Boolean) As Boolean
            For Each p As PluginBase In ActivePlugins
                If p.ProcessTalkPage(TheArticle, ReqPhoto) AndAlso ReqPhoto AndAlso p.HasReqPhotoParam Then _
                   ProcessTalkPageAndCheckWeAddedReqPhotoParam = True
                If TheArticle.PluginManagerGetSkipResults = SkipResults.SkipBadTag Then Return False
            Next
        End Function
        Private Shared Function ReqPhotoParamNeeded(ByVal TheArticle As Article) As Boolean
            For Each p As PluginBase In ActivePlugins
                If p.HasReqPhotoParam Then
                    If ReqPhotoNoParamsRegex.IsMatch(TheArticle.AlteredArticleText) Then Return True Else Return False
                End If
            Next
        End Function
        Private Shared Function FinaliseArticleProcessing(ByVal TheArticle As Article, ByRef Skip As Boolean, _
        ByRef Summary As String, ByVal ArticleText As String, ByVal ReqPhoto As Boolean) As String

            Dim SkipReason As SkipReason = PluginManager.SkipReason.Other

            If TheArticle.PluginManagerGetSkipResults = SkipResults.NotSet Then
                PluginSettings.PluginStats.Tagged += 1
            Else
                With PluginSettings.PluginStats
                    Select Case TheArticle.PluginManagerGetSkipResults
                        Case SkipResults.SkipBadTag ' always skip
                            If PluginSettings.SkipBadTags Then
                                .SkippedBadTagIncrement()
                                If PluginSettings.OpenBadInBrowser Then TheArticle.EditInBrowser()
                                Skip = True ' always skip
                                SkipReason = PluginManager.SkipReason.BadTag
                            Else
                                ' the plugin manager stops processing when it gets a bad tag. We know however
                                ' that one plugin found a bad template and possibly replaced it with
                                ' conTemplatePlaceholder. We're also not skipping, so we need to remove the placeholder
                                TheArticle.AlteredArticleText = _
                                   TheArticle.AlteredArticleText.Replace(conTemplatePlaceholder, "")
                                MessageBox.Show("Bad tag. Please fix it manually or click ignore.", "Bad tag", _
                                   MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                PluginSettings.PluginStats.Tagged += 1
                            End If
                        Case SkipResults.SkipRegex, SkipResults.SkipNoChange
                            If TheArticle.ProcessIt Then
                                PluginSettings.PluginStats.Tagged += 1
                            Else
                                If TheArticle.PluginManagerGetSkipResults = SkipResults.SkipRegex Then
                                    .SkippedMiscellaneousIncrement()
                                    Skip = True
                                    SkipReason = PluginManager.SkipReason.Regex
                                Else ' No change:
                                    If PluginSettings.SkipWhenNoChange Then
                                        .SkippedNoChangeIncrement()
                                        SkipReason = PluginManager.SkipReason.NoChange
                                        Skip = True
                                    Else
                                        PluginSettings.PluginStats.Tagged += 1
                                        Skip = False
                                    End If
                                End If
                            End If
                    End Select
                End With
            End If

            If Skip Then
                Return Skipping(Summary, TheArticle.EditSummary, SkipReason, ArticleText, Skip)
            Else
                With TheArticle
                    If ReqPhoto Then
                        .AlteredArticleText = ReqPhotoNoParamsRegex.Replace(.AlteredArticleText, "")
                        .DoneReplacement( _
                           "{{[[template:reqphoto|reqphoto]]}}", "template param(s)", True, conMe)
                        .ArticleHasAMajorChange()
                    End If

                    .FinaliseEditSummary()
                    PluginManager.AWBForm.TraceManager.WriteArticleActionLine1("Returning to AWB: Edit summary: " & _
                       .EditSummary, conMe, True)
                    FinaliseArticleProcessing = .AlteredArticleText
                    Summary = .EditSummary
                End With
            End If
        End Function
        Private Shared Function Skipping(ByRef EditSummary As String, ByVal DefaultEditSummary As String, _
        ByVal SkipReason As SkipReason, ByVal ArticleText As String, ByRef Skip As Boolean, _
        Optional ByVal ArticleTitle As String = Nothing, Optional ByVal NS As Integer = [Namespace].Talk) _
        As String

            If BotMode Then EditSummary = "This article should have been skipped" _
               Else EditSummary = DefaultEditSummary

            Select Case SkipReason
                Case PluginManager.SkipReason.BadNamespace
                    PluginSettings.PluginStats.SkippedNamespaceIncrement()
                    PluginManager.AWBForm.TraceManager.SkippedArticle(conMe, "Incorrect namespace")
                Case PluginManager.SkipReason.ProcessingMainArticleDoesntExist
                    PluginSettings.PluginStats.SkippedRedLinkIncrement()
                    PluginManager.AWBForm.TraceManager.SkippedArticle(conMe, "Article doesn't exist")
                Case PluginManager.SkipReason.ProcessingTalkPageArticleDoesntExist
                    PluginSettings.PluginStats.SkippedRedLinkIncrement()
                    PluginManager.AWBForm.TraceManager.SkippedArticleRedlink(conMe, ArticleTitle, NS)
                Case PluginManager.SkipReason.BadTag
                    PluginManager.AWBForm.TraceManager.SkippedArticleBadTag(conMe, ArticleTitle, NS)
                Case PluginManager.SkipReason.NoChange
                    PluginManager.AWBForm.TraceManager.SkippedArticle(conMe, "No change")
                Case PluginManager.SkipReason.Regex
                    PluginManager.AWBForm.TraceManager.SkippedArticle(conMe, _
                       "Article text matched a skip-if-found regular expression")
                Case PluginManager.SkipReason.Other
                    PluginManager.AWBForm.TraceManager.SkippedArticle(conMe, "")
            End Select

            Skip = True
            Return ArticleText
        End Function
        Private Shared Sub CreateNewGenericPlugin(ByVal PluginName As String, ByVal Creator As String)
            Dim plugin As GenericTemplatePlugin

            PluginManager.AWBForm.TraceManager.WriteBulletedLine(Creator & ": Creating generic template """ & _
               PluginName & """", True, True)

            plugin = New GenericTemplatePlugin(PluginName)
            Plugins.Add(PluginName, plugin)
            plugin.Initialise()
            plugin.Enabled = True ' (adds it to activeplugins)
        End Sub

        ' Friend interface exposed to client plugins:
        Friend Shared Sub ShowHidePluginTab(ByVal tabp As TabPage, ByVal Visible As Boolean)
            If Visible Then
                'If Not AWBForm.ContainsTabPage(tabp) Then
                Dim ContainedMainTab As Boolean = AWBForm.ContainsTabPage(KingbotkPluginTab)

                If ContainedMainTab Then AWBForm.RemoveTabPage(KingbotkPluginTab)
                AWBForm.AddTabPage(tabp)
                If ContainedMainTab Then AWBForm.AddTabPage(KingbotkPluginTab)
                'End If
            Else 'If AWBForm.ContainsTabPage(tabp) Then
                AWBForm.RemoveTabPage(tabp)
            End If
        End Sub
        Friend Shared Sub PluginEnabledStateChanged(ByVal Plugin As PluginBase, ByVal IsEnabled As Boolean)
            If IsEnabled Then
                If Not ActivePlugins.Contains(Plugin) Then
                    If Plugin.PluginShortName = Constants.Biography Then ' WPBio must be last in list
                        ActivePlugins.Add(Plugin)
                    Else
                        ActivePlugins.Insert(0, Plugin)
                    End If

                    If ActivePlugins.Count = 1 Then AWBForm.AddTabPage(KingbotkPluginTab)

                End If
            Else
                ActivePlugins.Remove(Plugin)

                If ActivePlugins.Count = 0 Then AWBForm.RemoveTabPage(KingbotkPluginTab)
            End If
            DefaultStatusText()
        End Sub
        Friend Shared Sub StopAWB()
            AWBForm.Stop(conAWBPluginName)
        End Sub
        Friend Shared Sub DeleteGenericPlugin(ByVal PG As IGenericTemplatePlugin, ByVal P As PluginBase)
            PG.Goodbye()
            Plugins.Remove(PG.GenericTemplateKey)
            If ActivePlugins.Contains(P) Then ActivePlugins.Remove(P)
            If ActivePlugins.Count = 0 Then AWBForm.RemoveTabPage(KingbotkPluginTab)
            DefaultStatusText()
        End Sub

        ' Friend static members:
        Friend Shared ReadOnly Property BotMode() As Boolean
            Get
                Return AWBForm.BotModeCheckbox.Checked
            End Get
        End Property
        Friend Shared ReadOnly Property PluginName() As String
            Get
                Return conMe
            End Get
        End Property
        Friend Shared Sub EditBoxInsertYesParam(ByVal ParameterName As String)
            EditBoxInsert("|" + ParameterName + "=yes")
        End Sub
        Friend Shared Sub EditBoxInsert(ByVal Text As String)
            AWBForm.EditBox.SelectedText = Text
        End Sub

        ' User interface management:
        Private Shared Property ShowHideTabs() As Boolean
            Get
                Return MenuShowSettingsTabs.Checked
            End Get
            Set(ByVal Show As Boolean)
                If Show Then
                    AWBForm.ShowAllTabPages()
                    'For Each tabp As TabPage In SettingsTabs ' may not need this now AWB tracks tabs
                    '    AWBForm.AddTabPage(tabp)
                    'Next
                    If ActivePlugins.Count > 0 Then AWBForm.AddTabPage(KingbotkPluginTab)
                Else
                    AWBForm.HideAllTabPages()
                    AWBForm.AddTabPage(KingbotkPluginTab)
                End If
                MenuShowSettingsTabs.Checked = Show
            End Set
        End Property
        Friend Shared Sub DefaultStatusText()
            Select Case ActivePlugins.Count
                Case 0
                    StatusText.Text = "Kingbotk plugin manager ready"
                Case 1
                    StatusText.Text = "Kingbotk plugin ready"
                Case Else
                    StatusText.Text = ActivePlugins.Count.ToString("0 Kingbotk plugins ready")
            End Select
            If PluginSettings.ManuallyAssess Then StatusText.Text += " (manual assessments plugin active)"
        End Sub
        Friend Shared Function GetTextBoxDropDownItems() As ToolStripItemCollection
            Return AWBForm.InsertTagToolStripMenuItem.DropDownItems
        End Function
        Friend Shared Sub AddItemToTextBoxInsertionContextMenu(ByVal ToolStripItems As ToolStripItemCollection)
            GetTextBoxDropDownItems.AddRange(ToolStripItems)
        End Sub
        Friend Shared Sub AddItemToTextBoxInsertionContextMenu(ByVal ToolStripItem As ToolStripItem)
            GetTextBoxDropDownItems.Add(ToolStripItem)
        End Sub
        Friend Shared Sub TestSkipNonExistingPages()
            Static WeCheckedSkipNonExistingPages As Boolean = False

            If Not WeCheckedSkipNonExistingPages AndAlso ActivePlugins.Count > 0 Then
                If AWBForm.SkipNonExistentPages.Checked Then
                    WeCheckedSkipNonExistingPages = True
                    If MessageBox.Show( _
                "The skip non existent pages checkbox is checked. This is not optimal for WikiProject tagging " & _
                "as AWB will skip red-link talk pages. Please note that you will not receive this warning " & _
                "again during this session, even if you load settings which have that box checked." & _
                Microsoft.VisualBasic.vbCrLf & Microsoft.VisualBasic.vbCrLf & _
                "Would you like the plugin to change this setting to false?", "Skip Non Existent Pages", _
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) _
                = DialogResult.Yes Then
                        AWBForm.SkipNonExistentPages.Checked = False
                    End If
                End If
            End If
        End Sub

        ' Event handlers - AWB:
        Private Shared Sub GetLogUploadLocationsEventHandler(ByVal sender As IAutoWikiBrowser, _
        ByVal LinksToLog As List(Of LogEntry)) Handles AWBForm.GetLogUploadLocations
            For Each Plugin As PluginBase In PluginManager.ActivePlugins
                If Plugin.HasSharedLogLocation Then
                    LinksToLog.Add(New LogEntry(Plugin.SharedLogLocation, True))
                End If
            Next
        End Sub
        Private Shared Sub AWBClosingEventHandler(ByVal sender As System.Object, ByVal e As FormClosingEventArgs)
            If e.Cancel Then
                Return
            End If

            With PluginManager.AWBForm.TraceManager()
                .WriteBulletedLine(conAWBPluginName & "Application closing.", True, False, True)
                .Flush()
                .Close()
            End With
        End Sub
        Private Shared Sub AWBBotModeCheckboxCheckedChangeHandler(ByVal sender As System.Object, ByVal e As System.EventArgs)
            Dim Line As String = conAWBPluginName & "Bot-mode "

            If BotMode Then
                Line += "enabled"
                AWBForm.SkipNoChanges = False
            Else
                Line += "disabled"
            End If

            PluginManager.AWBForm.TraceManager.WriteBulletedLine(Line, True, True, True)

            For Each p As PluginBase In ActivePlugins
                p.BotModeChanged(BotMode)
            Next
        End Sub
        Private Shared Sub AWBBotModeCheckboxEnabledChangedHandler(ByVal sender As Object, ByVal e As EventArgs)
            If AWBForm.BotModeCheckbox.Enabled AndAlso PluginSettings.ManuallyAssess Then
                AWBForm.BotModeCheckbox.Checked = False
                AWBForm.BotModeCheckbox.Enabled = False
            End If
        End Sub
        Private Shared Sub EditorStatusChanged(ByVal sender As AsyncApiEdit)
            If AWBForm.TheSession.Editor.IsActive Then
                If ActivePlugins.Count > 0 Then PluginSettings.AWBProcessingStart(sender)
            Else
                DefaultStatusText()
                PluginManager.AWBForm.TraceManager.WriteBulletedLine(conAWBPluginName & "AWB stopped processing", True, True, True)
                ' If AWB has stopped and the list is empty we assume the job is finished, so close the log and upload:
                If AWBForm.ListMaker.Count = 0 Then
                    PluginManager.AWBForm.TraceManager.Close()
                End If
            End If
        End Sub
        Private Shared Sub EditorAborted(ByVal sender As AsyncApiEdit)
            PluginSettings.AWBProcessingAborted(sender)
        End Sub
        Private Sub StopButtonClickEventHandler(ByVal sender As Object, ByVal e As EventArgs)
            DefaultStatusText()
            If Not AssessmentsObject Is Nothing Then AssessmentsObject.Reset()
            PluginSettings.AWBProcessingAborted(Nothing)
        End Sub
        Private Sub OurButtonsClickEventHander(ByVal sender As Object, ByVal e As EventArgs)
            Dim btn As Button = DirectCast(sender, Button)

            With AWBForm
                Select Case btn.Name
                    Case "btnStop"
                        .Stop(conAWBPluginName)
                    Case "btnStart"
                        .Start(conAWBPluginName)
                    Case "btnPreview"
                        .GetPreview(conAWBPluginName)
                    Case "btnSave"
                        .Save(conAWBPluginName)
                    Case "btnDiff"
                        .GetDiff(conAWBPluginName)
                    Case "btnIgnore"
                        .SkipPage(conAWBPluginName, "user")
                End Select
            End With
        End Sub
        Private Shared Sub MenuShowHide_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles MenuShowSettingsTabs.Click
            ShowHideTabs = MenuShowSettingsTabs.Checked
        End Sub
        Private Sub ManuallyAssessCheckBox_CheckChanged(ByVal sender As Object, ByVal e As System.EventArgs)
            If DirectCast(sender, CheckBox).Checked Then
                StatusText.Text = "Initialising assessments plugin"

                If AWBForm.TheSession.Editor.IsActive Then AWBForm.Stop(conAWBPluginName)

                If blnShowManualAssessmentsInstructions Then
                    Dim dialog As New AssessmentsInstructionsDialog

                    blnShowManualAssessmentsInstructions = Not (dialog.ShowDialog = DialogResult.Yes)
                End If

                AssessmentsObject = New Assessments(PluginSettings)

                DefaultStatusText()
            Else
                AssessmentsObject.Dispose()
                AssessmentsObject = Nothing
            End If
        End Sub
        Private Shared Sub AddGenericTemplateMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles AddGenericTemplateMenuItem.Click
            Dim str As String = Microsoft.VisualBasic.Interaction.InputBox( _
               "Enter the name for this generic plugin").Trim

            If Not str = "" Then
                If Plugins.ContainsKey(str) Then
                    MessageBox.Show("A plugin of this name already exists", "Error", _
                       MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                ElseIf CBool(Microsoft.VisualBasic.InStr(str, " ")) Then
                    str = str.Replace(" ", "")
                    CreateNewGenericPlugin(str, "User")
                Else
                    CreateNewGenericPlugin(str, "User")
                End If
            End If
        End Sub

        ' XML:
        Friend Shared Function XMLReadBoolean(ByVal reader As System.Xml.XmlTextReader, ByVal param As String, _
        ByVal ExistingValue As Boolean) As Boolean
            If reader.MoveToAttribute(param) Then Return Boolean.Parse(reader.Value) Else Return ExistingValue
        End Function
        Friend Shared Function XMLReadString(ByVal reader As System.Xml.XmlTextReader, ByVal param As String, _
        ByVal ExistingValue As String) As String
            If reader.MoveToAttribute(param) Then Return reader.Value.Trim Else Return ExistingValue
        End Function
        Friend Shared Function XMLReadInteger(ByVal reader As System.Xml.XmlTextReader, ByVal param As String, _
        ByVal ExistingValue As Integer) As Integer
            If reader.MoveToAttribute(param) Then Return Integer.Parse(reader.Value) Else Return ExistingValue
        End Function
        Private Shared Sub ReadXML(ByVal Reader As XmlTextReader)
            PluginManager.AWBForm.TraceManager.WriteBulletedLine(conAWBPluginName & "Reading settings from XML", False, True, True)

            blnShowManualAssessmentsInstructions = PluginManager.XMLReadBoolean(Reader, _
               conShowManualAssessmentsInstructions, _
               blnShowManualAssessmentsInstructions) ' must happen BEFORE get ManualAssessment yes/no

            PluginSettings.ReadXML(Reader)

            Dim Count As Integer = XMLReadInteger(Reader, conGenericTemplatesCount, 0)
            If Count > 0 Then ReadGenericTemplatesFromXML(Count, Reader) ' Must set up generic templates 
            'before reading in per-template properties, so that the new template receives a ReadXML() of its own

            For Each plugin As KeyValuePair(Of String, PluginBase) In Plugins
                plugin.Value.ReadXML(Reader)
                plugin.Value.ReadXMLRedirects(Reader)
            Next

            Dim blnNewVal As Boolean = PluginManager.XMLReadBoolean(Reader, conShowHideTabsParm, ShowHideTabs)
            If Not blnNewVal = ShowHideTabs Then _
               ShowHideTabs = blnNewVal ' Mustn't set if the same or we get extra tabs; must happen AFTER plugins

            TestSkipNonExistingPages()
        End Sub
        Private Shared Sub WriteXML(ByVal Writer As XmlTextWriter)
            Dim strGenericTemplates As New System.Collections.Specialized.StringCollection, i As Integer

            Writer.WriteAttributeString(conShowHideTabsParm, ShowHideTabs.ToString)
            Writer.WriteAttributeString(conShowManualAssessmentsInstructions, _
               blnShowManualAssessmentsInstructions.ToString)
            PluginSettings.WriteXML(Writer)
            For Each plugin As KeyValuePair(Of String, PluginBase) In Plugins
                plugin.Value.WriteXML(Writer)
                plugin.Value.WriteXMLRedirects(Writer)
                If plugin.Value.IAmGeneric Then _
                   strGenericTemplates.Add(DirectCast(plugin.Value, IGenericTemplatePlugin) _
                   .GenericTemplateKey)
            Next

            Writer.WriteAttributeString(conGenericTemplatesCount, strGenericTemplates.Count.ToString)

            For Each str As String In strGenericTemplates
                Writer.WriteAttributeString(conGenericTemplate & i.ToString, str)
                i += 1
            Next
        End Sub
        Private Shared Sub LoadSettingsNewWay(ByVal XMLString As String)
            Dim st As New IO.StringReader(XMLString)
            Dim Reader As XmlTextReader = New XmlTextReader(st)

            While Reader.Read()
                If Reader.NodeType = XmlNodeType.Element Then
                    ReadXML(Reader)
                    Exit While
                End If
            End While
        End Sub
        Private Shared Sub ReadGenericTemplatesFromXML(ByVal Count As Integer, ByVal Reader As XmlTextReader)
            Dim PluginName As String

            For i As Integer = 0 To Count - 1
                PluginName = PluginManager.XMLReadString(Reader, conGenericTemplate & i.ToString, "").Trim
                If Not Plugins.ContainsKey(PluginName) Then CreateNewGenericPlugin(PluginName, "ReadXML()")
            Next
        End Sub

        ' AWB nudges:
        Private Sub Nudge(ByRef Cancel As Boolean) Implements IAWBPlugin.Nudge
            For Each p As PluginBase In ActivePlugins
                If Not p.IAmReady Then
                    PluginManager.AWBForm.TraceManager.WriteBulletedLine(conAWBPluginName & _
                       "Bot mode: Cancelling AWB nudge as a generic plugin isn't ready yet", True, True, True)
                    Cancel = True
                    Exit For
                End If
            Next

            If Not Cancel Then PluginManager.AWBForm.TraceManager.WriteBulletedLine(conAWBPluginName & _
               "Bot mode: AWB is giving a nudge", True, False, True)
        End Sub
        Private Sub Nudged(ByVal Nudges As Integer) Implements IAWBPlugin.Nudged
            PluginSettings.lblAWBNudges.Text = "Nudges: " & Nudges.ToString
        End Sub

        Friend ReadOnly Property WikiName() As String Implements WikiFunctions.Plugin.IAWBPlugin.WikiName
            Get
                Return conWikiPlugin & " version " & AboutBox.Version
            End Get
        End Property
    End Class
End Namespace
