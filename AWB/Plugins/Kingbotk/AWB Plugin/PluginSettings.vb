Imports WikiFunctions.API

'Copyright © 2008 Stephen Kennedy (Kingboyk) http://www.sdk-software.com/
'Copyright © 2008 Sam Reed (Reedy) http://www.reedyboy.net/

'This program is free software; you can redistribute it and/or modify it under the terms of Version 2 of the GNU General Public License as published by the Free Software Foundation.

'This program is distributed in the hope that it will be useful,
'but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

'You should have received a copy of the GNU General Public License Version 2 along with this program; if not, write to the Free Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA

Namespace AutoWikiBrowser.Plugins.Kingbotk.Components
    Friend NotInheritable Class PluginSettingsControl
        Private WithEvents LivingPeopleToolStripMenuItem As New ToolStripMenuItem("Living people")

        ' XML parm-name constants:
        Private Const conCategoryNameParm As String = "CategoryName"
        Private Const conManuallyAssessParm As String = "ManuallyAssess"
        Private Const conCleanupParm As String = "Cleanup"
        Private Const conSkipBadTagsParm As String = "SkipBadTags"
        Private Const conSkipWhenNoChangeParm As String = "SkipWhenNoChange"
        Private Const conAssessmentsAlwaysLeaveACommentParm As String = "AlwaysLeaveAComment"
        Private Const conOpenBadInBrowser As String = "OpenBadInBrowser"

        ' Statistics:
        Friend WithEvents PluginStats As New Stats
        Private StatLabels As New List(Of Label)

        Friend Sub New()
            ' This call is required by the Windows Form Designer and must come first:
            InitializeComponent()

            With PluginManager.AWBForm
                AddHandler .SkipButton.Click, AddressOf Me.AWBSkipButtonClickEventHandler
                ' Get notification when AWB buttons enabled-state changes:
                AddHandler .DiffButton.EnabledChanged, AddressOf Me.AWBButtonsEnabledHandler
                AddHandler .StopButton.EnabledChanged, AddressOf Me.AWBButtonsEnabledHandler
                AddHandler .StartButton.EnabledChanged, AddressOf Me.AWBStartButtonEnabledHandler
                AddHandler .PreviewButton.EnabledChanged, AddressOf Me.AWBButtonsEnabledHandler
                AddHandler .SaveButton.EnabledChanged, AddressOf Me.AWBButtonsEnabledHandler
                AddHandler .SkipButton.EnabledChanged, AddressOf Me.AWBButtonsEnabledHandler
                AddHandler .BotModeCheckbox.CheckedChanged, AddressOf Me.BotStatusChangedHandler
                AddHandler .BotModeCheckbox.EnabledChanged, AddressOf Me.AWBBotModeEnabledChanged
                AddHandler .BotModeCheckbox.CheckedChanged, AddressOf Me.AWBBotModeCheckedChanged
                .CategoryTextBox.ContextMenuStrip.Items.Insert(0, LivingPeopleToolStripMenuItem)
                .CategoryTextBox.ContextMenuStrip.Items.Insert(1, New ToolStripSeparator())

                AddHandler .SkipNoChangesCheckBox.CheckedChanged, AddressOf Me.SkipNoChangesCheckBoxCheckedChanged
            End With

            StatLabels.AddRange(New Label() {lblTagged, lblSkipped, lblNoChange, lblBadTag, lblNamespace, lblNew, _
               lblRedlink})

            ' Initialise enabled state of our replica buttons:
            AWBButtonsEnabledHandler(PluginManager.AWBForm.StartTab.Controls("btnDiff"), Nothing)
            AWBButtonsEnabledHandler(PluginManager.AWBForm.StartTab.Controls("btnStop"), Nothing)
            AWBStartButtonEnabledHandler(PluginManager.AWBForm.StartTab.Controls("btnStart"), Nothing)
            AWBButtonsEnabledHandler(PluginManager.AWBForm.StartTab.Controls("btnPreview"), Nothing)
            AWBButtonsEnabledHandler(PluginManager.AWBForm.SaveButton, Nothing)
            AWBButtonsEnabledHandler(PluginManager.AWBForm.SkipButton, Nothing)

            ' Initialise bot checkbox:
            With PluginManager.AWBForm.BotModeCheckbox
                BotCheckBox.Enabled = .Enabled
                BotCheckBox.Checked = .Checked
                BotCheckBox.Visible = PluginManager.AWBForm.TheSession.User.IsBot
            End With
        End Sub

        ' AWB processing stopped/started:
        Friend Sub AWBProcessingStart(ByVal editor As AsyncApiEdit)
            For Each lbl As Label In StatLabels
                If lbl.Text = "" Then lbl.Text = "0"
            Next
            TimerStats1.Visible = True
            TimerStats1.Init(editor, ETALabel, PluginStats)

            PluginManager.StatusText.Text = "Started"
        End Sub

        ' Properties:
        Private mAssessmentsAlwaysLeaveAComment As Boolean

        Friend Property CategoryName() As String
            Get
                Return Regex.Replace(PluginManager.AWBForm.CategoryTextBox.Text, "^category:", "", RegexOptions.IgnoreCase)
            End Get
            Set(ByVal value As String)
                PluginManager.AWBForm.CategoryTextBox.Text = value
            End Set
        End Property
        Friend Property ManuallyAssess() As Boolean
            Get
                Return ManuallyAssessCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                ManuallyAssessCheckBox.Checked = value
            End Set
        End Property
        Friend Property Cleanup() As Boolean
            Get
                Return CleanupCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                CleanupCheckBox.Checked = value
            End Set
        End Property
        Friend Property SkipBadTags() As Boolean
            Get
                Return SkipBadTagsCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                SkipBadTagsCheckBox.Checked = value
            End Set
        End Property
        Friend Property SkipWhenNoChange() As Boolean
            Get
                Return SkipNoChangesCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                SkipNoChangesCheckBox.Checked = value
            End Set
        End Property
        Friend Property AssessmentsAlwaysLeaveAComment() As Boolean
            Get
                Return mAssessmentsAlwaysLeaveAComment
            End Get
            Set(ByVal value As Boolean)
                mAssessmentsAlwaysLeaveAComment = value
            End Set
        End Property
        Friend Property OpenBadInBrowser() As Boolean
            Get
                Return OpenBadInBrowserCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                OpenBadInBrowserCheckBox.Checked = value
            End Set
        End Property

        ' XML interface:
        Friend Sub ReadXML(ByVal Reader As System.Xml.XmlTextReader)
            ManuallyAssess = PluginManager.XMLReadBoolean(Reader, conManuallyAssessParm, ManuallyAssess)
            Cleanup = PluginManager.XMLReadBoolean(Reader, conCleanupParm, Cleanup)
            SkipBadTags = PluginManager.XMLReadBoolean(Reader, conSkipBadTagsParm, SkipBadTags)
            SkipWhenNoChange = PluginManager.XMLReadBoolean(Reader, conSkipWhenNoChangeParm, SkipWhenNoChange)
            AssessmentsAlwaysLeaveAComment = PluginManager.XMLReadBoolean(Reader, _
               conAssessmentsAlwaysLeaveACommentParm, AssessmentsAlwaysLeaveAComment)
            OpenBadInBrowser = PluginManager.XMLReadBoolean(Reader, conOpenBadInBrowser, OpenBadInBrowser)
        End Sub
        Friend Sub Reset()
            ManuallyAssess = False
            Cleanup = False
            PluginStats = New Stats
            ' don't change logging settings
            PluginManager.AWBForm.TraceManager.WriteBulletedLine("Reset", False, True, True)
            AssessmentsAlwaysLeaveAComment = False
            OpenBadInBrowser = False
        End Sub
        Friend Sub WriteXML(ByVal Writer As System.Xml.XmlTextWriter)
            Writer.WriteAttributeString(conManuallyAssessParm, ManuallyAssess.ToString)
            Writer.WriteAttributeString(conCleanupParm, Cleanup.ToString)
            Writer.WriteAttributeString(conSkipBadTagsParm, SkipBadTags.ToString)
            Writer.WriteAttributeString(conSkipWhenNoChangeParm, SkipWhenNoChange.ToString)
            Writer.WriteAttributeString(conAssessmentsAlwaysLeaveACommentParm, _
               AssessmentsAlwaysLeaveAComment.ToString)
            Writer.WriteAttributeString(conOpenBadInBrowser, OpenBadInBrowser.ToString)
        End Sub

        ' Event handlers - menu items:
        Private Sub SetAWBToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles SetAWBToolStripMenuItem.Click
            With PluginManager.AWBForm
                .SkipNonExistentPages.Checked = False
                .ApplyGeneralFixesCheckBox.Checked = False
                .AutoTagCheckBox.Checked = False
                If .EditSummaryComboBox.Text = "clean up" Then .EditSummaryComboBox.Text = ""
            End With
        End Sub
        Private Sub LivingPeopleToolStripMenuItemClick(ByVal sender As Object, ByVal e As EventArgs)

            CategoryName = "Living people"
        End Sub
        Private Sub MenuAbout_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles MenuAbout.Click
            Dim about As New AboutBox()
            about.Show()
        End Sub
        Private Sub MenuHelp_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles MenuHelp.Click
            PluginManager.AWBForm.ShowHelpEnWiki("User:Kingbotk/Plugin/User guide")
        End Sub
        ' Event handlers - our controls:
        Private Sub ManuallyAssessCheckBox_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) _
        Handles ManuallyAssessCheckBox.CheckedChanged
            If ManuallyAssess Then
                PluginManager.AWBForm.BotModeCheckbox.Enabled = False
                PluginManager.AWBForm.BotModeCheckbox.Checked = False
                SkipBadTagsCheckBox.Checked = False
                SkipBadTagsCheckBox.Enabled = False
                SkipNoChangesCheckBox.Checked = False
                SkipNoChangesCheckBox.Enabled = False
            Else
                PluginManager.AWBForm.BotModeCheckbox.Enabled = PluginManager.AWBForm.TheSession.User.IsBot
                BotCheckBox.Visible = PluginManager.AWBForm.TheSession.User.IsBot
                SkipBadTagsCheckBox.Enabled = True
                SkipNoChangesCheckBox.Enabled = True
            End If

            CleanupCheckBox.Checked = ManuallyAssess
            CleanupCheckBox.Enabled = ManuallyAssess
            PluginManager.AWBForm.TraceManager.WriteBulletedLine(String.Format("Manual assessments mode on: {0}", _
               ManuallyAssess.ToString), True, True, True)
        End Sub
        Private Sub ResetTimerButton_Click(ByVal sender As Object, ByVal e As EventArgs) Handles ResetTimerButton.Click
            TimerStats1.Reset()
        End Sub
        Private Sub btnStart_EnabledChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles btnStart.EnabledChanged
            If DirectCast(sender, Button).Enabled Then PluginManager.TestSkipNonExistingPages()
        End Sub
        Private Sub BotCheckBox_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles BotCheckBox.CheckedChanged
            PluginManager.AWBForm.BotModeCheckbox.Checked = BotCheckBox.Checked
        End Sub
        Private Sub SkipBadTagsCheckBox_CheckedChanged(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles SkipBadTagsCheckBox.CheckedChanged
            OpenBadInBrowserCheckBox.Visible = SkipBadTagsCheckBox.Checked
        End Sub

        ' Event handlers - AWB components (some additionally double-handled in Plugin Manager):
        Private Sub AWBButtonsEnabledHandler(ByVal sender As Object, ByVal e As EventArgs)
            Dim btn As Button = DirectCast(sender, Button)

            DirectCast(Me.AWBGroupBox.Controls(btn.Name), Button).Enabled = btn.Enabled
        End Sub
        Private Sub AWBStartButtonEnabledHandler(ByVal sender As Object, ByVal e As EventArgs)
            Dim btn As Button = DirectCast(sender, Button)

            btnStart.Enabled = btn.Enabled
        End Sub
        Private Sub AWBSkipButtonClickEventHandler(ByVal sender As Object, ByVal e As EventArgs) _
        Handles btnIgnore.Click
            If Not ManuallyAssess AndAlso PluginManager.ActivePlugins.Count > 0 Then ' If ManuallyAssess is True, defer to the handler in Assessments class
                If Not sender Is PluginManager.AWBForm.SkipButton Then
                    PluginManager.AWBForm.TraceManager.SkippedArticle("User", WikiFunctions.Logging.AWBLogListener.StringUserSkipped)
                End If
                PluginStats.SkippedMiscellaneousIncrement(True)

            End If
        End Sub
        Friend Sub AWBArticleStatsLabelChangeEventHandler(ByVal sender As Object, ByVal e As EventArgs)
            Dim lbl As Label = DirectCast(sender, Label)

            DirectCast(Me.ArticleStatsGroupBox.Controls(lbl.Name), Label).Text = lbl.Text
        End Sub
        Private Sub AWBBotModeCheckedChanged(ByVal sender As Object, ByVal e As EventArgs)
            If PluginManager.AWBForm.BotModeCheckbox.Checked Then
                SkipBadTagsCheckBox.Checked = True
                SkipBadTagsCheckBox.Enabled = False
                SkipNoChangesCheckBox.Checked = True
                'SkipNoChangesCheckBox.Enabled = False
                lblAWBNudges.Visible = True
                BotCheckBox.Checked = True
            Else
                SkipBadTagsCheckBox.Enabled = True
                'SkipNoChangesCheckBox.Enabled = True
                lblAWBNudges.Visible = False
                BotCheckBox.Checked = False
            End If
        End Sub
        Private Sub SkipNoChangesCheckBoxCheckedChanged(ByVal sender As Object, ByVal e As EventArgs)
            If (PluginManager.AWBForm.SkipNoChanges <> SkipNoChangesCheckBox.Checked) Then
                SkipNoChangesCheckBox.Checked = PluginManager.AWBForm.SkipNoChanges
            End If
        End Sub
        Private Sub AWBBotModeEnabledChanged(ByVal sender As Object, ByVal e As EventArgs)
            BotCheckBox.Enabled = PluginManager.AWBForm.BotModeCheckbox.Enabled
        End Sub
        Private Sub BotStatusChangedHandler(ByVal sender As Object, ByVal e As EventArgs)
            BotCheckBox.Visible = PluginManager.AWBForm.TheSession.User.IsBot
        End Sub
        ' Event handlers - plugin stats:
        Private Sub PluginStats_New(ByVal val As Integer) Handles PluginStats.New
            lblNew.Text = val.ToString
        End Sub
        Private Sub PluginStats_SkipBadTag(ByVal val As Integer) Handles PluginStats.SkipBadTag
            lblBadTag.Text = val.ToString
        End Sub
        Private Sub PluginStats_SkipMisc(ByVal val As Integer) Handles PluginStats.SkipMisc
            lblSkipped.Text = val.ToString
        End Sub
        Private Sub PluginStats_SkipNamespace(ByVal val As Integer) Handles PluginStats.SkipNamespace
            lblNamespace.Text = val.ToString
        End Sub
        Private Sub PluginStats_SkipNoChange(ByVal val As Integer) Handles PluginStats.SkipNoChange
            lblNoChange.Text = val.ToString
        End Sub
        Private Sub PluginStats_Tagged(ByVal val As Integer) Handles PluginStats.evTagged
            lblTagged.Text = val.ToString
        End Sub
        Private Sub PluginStats_RedLink(ByVal val As Integer) Handles PluginStats.RedLink
            lblRedlink.Text = val.ToString
        End Sub
#Region "TextInsertHandlers"
        ' Event handlers: Insert-text context menu:
        Private Sub StubClassMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles StubClassMenuItem.Click
            PluginManager.EditBoxInsert("|class=Stub")
        End Sub
        Private Sub StartClassMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles StartClassMenuItem.Click
            PluginManager.EditBoxInsert("|class=Start")
        End Sub
        Private Sub BClassMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BClassMenuItem.Click
            PluginManager.EditBoxInsert("|class=B")
        End Sub
        Private Sub GAClassMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles GAClassMenuItem.Click
            PluginManager.EditBoxInsert("|class=GA")
        End Sub
        Private Sub AClassMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles AClassMenuItem.Click
            PluginManager.EditBoxInsert("|class=A")
        End Sub
        Private Sub FAClassMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles FAClassMenuItem.Click
            PluginManager.EditBoxInsert("|class=FA")
        End Sub
        Private Sub NeededClassMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles NeededClassMenuItem.Click
            PluginManager.EditBoxInsert("|class=Needed")
        End Sub
        Private Sub CatClassMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles CatClassMenuItem.Click
            PluginManager.EditBoxInsert("|class=Cat")
        End Sub
        Private Sub DabClassMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles DabClassMenuItem.Click
            PluginManager.EditBoxInsert("|class=Dab")
        End Sub
        Private Sub TemplateClassMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles TemplateClassMenuItem.Click
            PluginManager.EditBoxInsert("|class=Template")
        End Sub
        Private Sub NAClassMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles NAClassMenuItem.Click
            PluginManager.EditBoxInsert("|class=NA")
        End Sub
        Private Sub LowImportanceMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles LowImportanceMenuItem.Click
            PluginManager.EditBoxInsert("|importance=Low")
        End Sub
        Private Sub MidImportanceMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles MidImportanceMenuItem.Click
            PluginManager.EditBoxInsert("|importance=Mid")
        End Sub
        Private Sub HighImportanceMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles HighImportanceMenuItem.Click
            PluginManager.EditBoxInsert("|importance=High")
        End Sub
        Private Sub TopImportanceMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles TopImportanceMenuItem.Click
            PluginManager.EditBoxInsert("|importance=Top")
        End Sub
        Private Sub NAImportanceMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles NAImportanceMenuItem.Click
            PluginManager.EditBoxInsert("|importance=NA")
        End Sub
        Private Sub LowPriorityMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles LowPriorityMenuItem.Click
            PluginManager.EditBoxInsert("|priority=Low")
        End Sub
        Private Sub MidPriorityMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles MidPriorityMenuItem.Click
            PluginManager.EditBoxInsert("|priority=Mid")
        End Sub
        Private Sub HighPriorityMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles HighPriorityMenuItem.Click
            PluginManager.EditBoxInsert("|priority=High")
        End Sub
        Private Sub TopPriorityMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles TopPriorityMenuItem.Click
            PluginManager.EditBoxInsert("|priority=Top")
        End Sub
        Private Sub NAPriorityMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles NAPriorityMenuItem.Click
            PluginManager.EditBoxInsert("|priority=NA")
        End Sub
#End Region

        ' Statistics:
        Friend NotInheritable Class Stats
            Private mTagged As Integer, mNewArticles As Integer, mSkipped As Integer, mSkippedNoChange As Integer
            Private mSkippedBadTag As Integer, mSkippedNamespace As Integer, mRedLinks As Integer

            Friend Event SkipMisc(ByVal val As Integer)
            Friend Event SkipNoChange(ByVal val As Integer)
            Friend Event SkipBadTag(ByVal val As Integer)
            Friend Event SkipNamespace(ByVal val As Integer)
            Friend Event evTagged(ByVal val As Integer)
            Friend Event [New](ByVal val As Integer)
            Friend Event RedLink(ByVal val As Integer)

            Friend Property Tagged() As Integer
                Get
                    Return mTagged
                End Get
                Set(ByVal value As Integer)
                    mTagged = value
                    RaiseEvent evTagged(value)
                End Set
            End Property
            Friend Property NewArticles() As Integer
                Get
                    Return mNewArticles
                End Get
                Set(ByVal value As Integer)
                    mNewArticles = value
                    RaiseEvent [New](value)
                End Set
            End Property
            Friend Property Skipped() As Integer
                Get
                    Return mSkipped
                End Get
                Private Set(ByVal value As Integer)
                    mSkipped = value
                    RaiseEvent SkipMisc(value)
                End Set
            End Property
            Friend Sub SkippedMiscellaneousIncrement()
                Skipped += 1
            End Sub
            Friend Sub SkippedMiscellaneousIncrement(ByVal DeincrementTagged As Boolean)
                Skipped += 1
                If DeincrementTagged Then Tagged -= 1
            End Sub
            Friend Property SkippedRedLink() As Integer
                Get
                    Return mRedLinks
                End Get
                Private Set(ByVal value As Integer)
                    mRedLinks = value
                    RaiseEvent RedLink(value)
                End Set
            End Property
            Friend Sub SkippedRedLinkIncrement()
                Skipped += 1
                SkippedRedLink += 1
            End Sub
            Private Property SkippedNoChange() As Integer
                Get
                    Return mSkippedNoChange
                End Get
                Set(ByVal value As Integer)
                    mSkippedNoChange = value
                    RaiseEvent SkipNoChange(value)
                End Set
            End Property
            Friend Sub SkippedNoChangeIncrement()
                SkippedNoChange += 1
                Skipped += 1
            End Sub
            Private Property SkippedBadTag() As Integer
                Get
                    Return mSkippedBadTag
                End Get
                Set(ByVal value As Integer)
                    mSkippedBadTag = value
                    RaiseEvent SkipBadTag(value)
                End Set
            End Property
            Friend Sub SkippedBadTagIncrement()
                SkippedBadTag += 1
                Skipped += 1
            End Sub
            Private Property SkippedNamespace() As Integer
                Get
                    Return mSkippedNamespace
                End Get
                Set(ByVal value As Integer)
                    mSkippedNamespace = value
                    RaiseEvent SkipNamespace(value)
                End Set
            End Property
            Friend Sub SkippedNamespaceIncrement()
                SkippedNamespace += 1
                Skipped += 1
            End Sub

            Friend Sub New()
                Skipped = 0
                SkippedBadTag = 0
                SkippedNamespace = 0
                SkippedNoChange = 0
                NewArticles = 0
                Tagged = 0
                SkippedRedLink = 0
            End Sub
        End Class

        Private Sub SkipNoChangesCheckBox_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SkipNoChangesCheckBox.CheckedChanged
            If (PluginManager.AWBForm.SkipNoChanges <> SkipNoChangesCheckBox.Checked) Then
                PluginManager.AWBForm.SkipNoChanges = SkipNoChangesCheckBox.Checked
            End If
        End Sub
    End Class
End Namespace