Namespace AutoWikiBrowser.Plugins.SDKSoftware.Kingbotk.Components
    Friend NotInheritable Class PluginSettingsControl
        ' XML parm-name constants:
        Private Const conCategoryNameParm As String = "CategoryName"
        Private Const conManuallyAssessParm As String = "ManuallyAssess"
        Private Const conCleanupParm As String = "Cleanup"
        Private Const conSkipBadTagsParm As String = "SkipBadTags"
        Private Const conSkipWhenNoChangeParm As String = "SkipWhenNoChange"
        Private Const conAssessmentsAlwaysLeaveACommentParm As String = "AlwaysLeaveAComment"

        ' Statistics:
        Friend WithEvents PluginStats As New Stats
        Private StatLabels As New List(Of Label)

        ' AWB objects:
        Private txtEdit As TextBox
        Private blnBotModeHasBeenOn As Boolean
        Private WithEvents chkBotMode As CheckBox

        'Tracing:
        Friend Shared WithEvents MyTrace As MyTrace
        Friend LoggingSettings As PluginLogging

        ' AWB processing stopped/started:
        Friend Sub AWBProcessingStart(ByVal webcontrol As WikiFunctions.Browser.WebControl)
            For Each lbl As Label In StatLabels
                If lbl.Text = "" Then lbl.Text = "0"
            Next
            TimerStats1.Visible = True
            TimerStats1.Init(webcontrol, ETALabel, PluginStats)

            If MyTrace.HaveOpenFile Then
                MyTrace.WriteBulletedLine("AWB started processing", True, True, True)
            Else
                MyTrace.Initialise()
            End If
            PluginManager.StatusText.Text = "Started"
        End Sub

        ' Properties:
        Private mAssessmentsAlwaysLeaveAComment As Boolean

        Public Property CategoryName() As String
            Get
                Return Regex.Replace(CategoryTextBox.Text, "^category:", "", RegexOptions.IgnoreCase)
            End Get
            Set(ByVal value As String)
                CategoryTextBox.Text = value
            End Set
        End Property
        Public Property ManuallyAssess() As Boolean
            Get
                Return ManuallyAssessCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                ManuallyAssessCheckBox.Checked = value
            End Set
        End Property
        Public Property Cleanup() As Boolean
            Get
                Return CleanupCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                CleanupCheckBox.Checked = value
            End Set
        End Property
        Public Property SkipBadTags() As Boolean
            Get
                Return SkipBadTagsCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                SkipBadTagsCheckBox.Checked = value
            End Set
        End Property
        Public Property SkipWhenNoChange() As Boolean
            Get
                Return SkipNoChangesCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                SkipNoChangesCheckBox.Checked = value
            End Set
        End Property
        Public Property AssessmentsAlwaysLeaveAComment() As Boolean
            Get
                Return mAssessmentsAlwaysLeaveAComment
            End Get
            Set(ByVal value As Boolean)
                mAssessmentsAlwaysLeaveAComment = value
            End Set
        End Property

        ' XML interface:
        Public Sub ReadXML(ByVal Reader As System.Xml.XmlTextReader)
            CategoryName = PluginManager.XMLReadString(Reader, conCategoryNameParm, CategoryName)
            ManuallyAssess = PluginManager.XMLReadBoolean(Reader, conManuallyAssessParm, ManuallyAssess)
            Cleanup = PluginManager.XMLReadBoolean(Reader, conCleanupParm, Cleanup)
            SkipBadTags = PluginManager.XMLReadBoolean(Reader, conSkipBadTagsParm, SkipBadTags)
            SkipWhenNoChange = PluginManager.XMLReadBoolean(Reader, conSkipWhenNoChangeParm, SkipWhenNoChange)
            AssessmentsAlwaysLeaveAComment = PluginManager.XMLReadBoolean(Reader, _
               conAssessmentsAlwaysLeaveACommentParm, AssessmentsAlwaysLeaveAComment)
            LoggingSettings.ReadXML(Reader, MyTrace)
        End Sub
        Public Sub Reset()
            CategoryName = ""
            ManuallyAssess = False
            Cleanup = False
            PluginStats = New Stats
            ' don't change logging settings
            MyTrace.WriteBulletedLine("Reset", False, True, True)
            AssessmentsAlwaysLeaveAComment = False
            LoggingSettings.Reset()
        End Sub
        Public Sub WriteXML(ByVal Writer As System.Xml.XmlTextWriter)
            Writer.WriteAttributeString(conCategoryNameParm, CategoryName)
            Writer.WriteAttributeString(conManuallyAssessParm, ManuallyAssess.ToString)
            Writer.WriteAttributeString(conCleanupParm, Cleanup.ToString)
            Writer.WriteAttributeString(conSkipBadTagsParm, SkipBadTags.ToString)
            Writer.WriteAttributeString(conSkipWhenNoChangeParm, SkipWhenNoChange.ToString)
            Writer.WriteAttributeString(conAssessmentsAlwaysLeaveACommentParm, AssessmentsAlwaysLeaveAComment.ToString)
            LoggingSettings.WriteXML(Writer)
        End Sub

        ' Event handlers - menu items:
        Private Sub SetAWBToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles SetAWBToolStripMenuItem.Click
            With PluginManager.AWBForm
                .SkipNonExistentPagesCheckBox.Checked = False
                .ApplyGeneralFixesCheckBox.Checked = False
                .AutoTagCheckBox.Checked = False
                If .EditSummary.Text = "clean up" Then .EditSummary.Text = ""
            End With
        End Sub
        Private Sub LivingPeopleToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles LivingPeopleToolStripMenuItem.Click
            CategoryName = "Living people"
        End Sub
        Private Sub ClearMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles ClearToolStripMenuItem.Click
            CategoryName = ""
        End Sub
        Private Sub MenuAbout_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles MenuAbout.Click
            Dim about As New AboutBox(String.Format("Version {0}", PluginLogging.Props.PluginVersion))
            about.Show()
        End Sub
        Private Sub MenuHelp_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles MenuHelp.Click
            Tools.OpenENArticleInBrowser("Kingbotk/Plugin/User guide", True)
        End Sub
        Private Sub MenuHelpReleaseNotes_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles MenuHelpReleaseNotes.Click
            Tools.OpenENArticleInBrowser("Kingbotk/Plugin", True)
        End Sub
        Private Sub CutToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles CutToolStripMenuItem.Click
            CategoryTextBox.Cut()
        End Sub
        Private Sub PasteToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles PasteToolStripMenuItem.Click
            CategoryTextBox.Paste()
        End Sub
        Private Sub CopyToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles CopyToolStripMenuItem.Click
            CategoryTextBox.Copy()
        End Sub
        Private Sub UsernameToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles UploadUsernameToolStripMenuItem.Click
            LoggingSettings.LoginDetails = New LoginForm().GetFromForm
        End Sub

        ' Event handlers - our controls:
        Private Sub ManuallyAssessCheckBox_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) _
        Handles ManuallyAssessCheckBox.CheckedChanged
            If ManuallyAssess Then
                chkBotMode.Enabled = False
                chkBotMode.Checked = False
                SkipBadTagsCheckBox.Checked = False
                SkipBadTagsCheckBox.Enabled = False
                SkipNoChangesCheckBox.Checked = False
                SkipNoChangesCheckBox.Enabled = False
                btnDryRun.Enabled = False
            Else
                If blnBotModeHasBeenOn Then ' ManuallyAssessed is now unchecked, bot has been previously enabled
                    chkBotMode.Enabled = True
                End If
                SkipBadTagsCheckBox.Enabled = True
                SkipNoChangesCheckBox.Enabled = True
                btnDryRun.Enabled = btnStart.Enabled
            End If

            CleanupCheckBox.Checked = ManuallyAssess
            CleanupCheckBox.Enabled = ManuallyAssess
            MyTrace.WriteBulletedLine(String.Format("Manual assessments mode on: {0}", _
               ManuallyAssess.ToString), True, True, True)
        End Sub
        Private Sub ResetTimerButton_Click(ByVal sender As Object, ByVal e As EventArgs) Handles ResetTimerButton.Click
            TimerStats1.Reset()
        End Sub
        Private Sub CategoryTextBox_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles CategoryTextBox.TextChanged
            LoggingSettings.WeHaveUnappliedChanges()
        End Sub
        Private Sub btnStart_EnabledChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles btnStart.EnabledChanged
            If DirectCast(sender, Button).Enabled Then PluginManager.TestSkipNonExistingPages()
        End Sub

        ' Event handlers - AWB components (some additionally double-handled in Plugin Manager):
        Private Sub AWBButtonsEnabledHandler(ByVal sender As Object, ByVal e As EventArgs)
            Dim btn As Button = DirectCast(sender, Button)

            DirectCast(Me.AWBGroupBox.Controls(btn.Name), Button).Enabled = btn.Enabled
        End Sub
        Private Sub AWBStartButtonEnabledHandler(ByVal sender As Object, ByVal e As EventArgs)
            Dim btn As Button = DirectCast(sender, Button)

            btnStart.Enabled = btn.Enabled
            btnDryRun.Enabled = btn.Enabled
        End Sub
        Private Sub AWBSkipButtonClickEventHandler(ByVal sender As Object, ByVal e As EventArgs) _
        Handles btnIgnore.Click
            If Not ManuallyAssess Then ' If ManuallyAssess is True, defer to the handler in Assessments class
                MyTrace.SkippedArticle("User", "User clicked Ignore")
                PluginStats.SkippedMiscellaneousIncrement(True)
            End If
        End Sub
        Friend Sub AWBArticleStatsLabelChangeEventHandler(ByVal sender As Object, ByVal e As EventArgs)
            Dim lbl As Label = DirectCast(sender, Label)

            DirectCast(Me.ArticleStatsGroupBox.Controls(lbl.Name), Label).Text = lbl.Text
        End Sub
        Private Sub chkBotMode_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) _
        Handles chkBotMode.CheckedChanged
            If DirectCast(sender, CheckBox).Checked Then
                SkipBadTagsCheckBox.Checked = True
                SkipBadTagsCheckBox.Enabled = False
                SkipNoChangesCheckBox.Checked = True
                SkipNoChangesCheckBox.Enabled = False
                lblAWBNudges.Visible = True
            Else
                SkipBadTagsCheckBox.Enabled = True
                SkipNoChangesCheckBox.Enabled = True
                lblAWBNudges.Visible = False
            End If
        End Sub
        Private Sub chkBotMode_EnabledChanged(ByVal sender As Object, ByVal e As EventArgs) _
        Handles chkBotMode.EnabledChanged
            If DirectCast(sender, CheckBox).Enabled Then blnBotModeHasBeenOn = True
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
            txtEdit.SelectedText = "|class=Stub"
        End Sub
        Private Sub StartClassMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles StartClassMenuItem.Click
            txtEdit.SelectedText = "|class=Start"
        End Sub
        Private Sub BClassMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BClassMenuItem.Click
            txtEdit.SelectedText = "|class=B"
        End Sub
        Private Sub GAClassMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles GAClassMenuItem.Click
            txtEdit.SelectedText = "|class=GA"
        End Sub
        Private Sub AClassMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles AClassMenuItem.Click
            txtEdit.SelectedText = "|class=A"
        End Sub
        Private Sub FAClassMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles FAClassMenuItem.Click
            txtEdit.SelectedText = "|class=FA"
        End Sub
        Private Sub NeededClassMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles NeededClassMenuItem.Click
            txtEdit.SelectedText = "|class=Needed"
        End Sub
        Private Sub CatClassMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles CatClassMenuItem.Click
            txtEdit.SelectedText = "|class=Cat"
        End Sub
        Private Sub DabClassMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles DabClassMenuItem.Click
            txtEdit.SelectedText = "|class=Dab"
        End Sub
        Private Sub TemplateClassMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles TemplateClassMenuItem.Click
            txtEdit.SelectedText = "|class=Template"
        End Sub
        Private Sub NAClassMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles NAClassMenuItem.Click
            txtEdit.SelectedText = "|class=NA"
        End Sub
        Private Sub LowImportanceMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles LowImportanceMenuItem.Click
            txtEdit.SelectedText = "|importance=Low"
        End Sub
        Private Sub MidImportanceMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles MidImportanceMenuItem.Click
            txtEdit.SelectedText = "|importance=Mid"
        End Sub
        Private Sub HighImportanceMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles HighImportanceMenuItem.Click
            txtEdit.SelectedText = "|importance=High"
        End Sub
        Private Sub TopImportanceMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles TopImportanceMenuItem.Click
            txtEdit.SelectedText = "|importance=Top"
        End Sub
        Private Sub NAImportanceMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles NAImportanceMenuItem.Click
            txtEdit.SelectedText = "|importance=NA"
        End Sub
        Private Sub LowPriorityMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles LowPriorityMenuItem.Click
            txtEdit.SelectedText = "|priority=Low"
        End Sub
        Private Sub MidPriorityMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles MidPriorityMenuItem.Click
            txtEdit.SelectedText = "|priority=Mid"
        End Sub
        Private Sub HighPriorityMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles HighPriorityMenuItem.Click
            txtEdit.SelectedText = "|priority=High"
        End Sub
        Private Sub TopPriorityMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles TopPriorityMenuItem.Click
            txtEdit.SelectedText = "|priority=Top"
        End Sub
        Private Sub NAPriorityMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles NAPriorityMenuItem.Click
            txtEdit.SelectedText = "|priority=NA"
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
            Public Sub SkippedMiscellaneousIncrement()
                Skipped += 1
            End Sub
            Public Sub SkippedMiscellaneousIncrement(ByVal DeincrementTagged As Boolean)
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
            Public Sub SkippedRedLinkIncrement()
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
            Public Sub SkippedNoChangeIncrement()
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
            Public Sub SkippedBadTagIncrement()
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
            Public Sub SkippedNamespaceIncrement()
                SkippedNamespace += 1
                Skipped += 1
            End Sub

            Public Sub New()
                Skipped = 0
                SkippedBadTag = 0
                SkippedNamespace = 0
                SkippedNoChange = 0
                NewArticles = 0
                Tagged = 0
                SkippedRedLink = 0
            End Sub
        End Class

        Public Sub New(ByVal txt As TextBox, ByVal chk As CheckBox)

            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.

            With PluginManager.AWBForm
                AddHandler .SkipButton.Click, AddressOf Me.AWBSkipButtonClickEventHandler
                ' Get notification when AWB buttons enabled-state changes:
                AddHandler .DiffButton.EnabledChanged, AddressOf Me.AWBButtonsEnabledHandler
                AddHandler .StopButton.EnabledChanged, AddressOf Me.AWBButtonsEnabledHandler
                AddHandler .StartButton.EnabledChanged, AddressOf Me.AWBStartButtonEnabledHandler
                AddHandler .PreviewButton.EnabledChanged, AddressOf Me.AWBButtonsEnabledHandler
                AddHandler .SaveButton.EnabledChanged, AddressOf Me.AWBButtonsEnabledHandler
                AddHandler .SkipButton.EnabledChanged, AddressOf Me.AWBButtonsEnabledHandler
            End With

            StatLabels.AddRange(New Label() {lblTagged, lblSkipped, lblNoChange, lblBadTag, lblNamespace, lblNew, _
               lblRedlink})

            txtEdit = txt
            chkBotMode = chk

            MyTrace = New MyTrace()
            LoggingSettings = New PluginLogging(MyTrace, CategoryTextBox)
            MyTrace.LS = LoggingSettings

            ' Initialise enabled state of our replica buttons:
            AWBButtonsEnabledHandler(PluginManager.AWBForm.StartTab.Controls("btnDiff"), Nothing)
            AWBButtonsEnabledHandler(PluginManager.AWBForm.StartTab.Controls("btnStop"), Nothing)
            AWBStartButtonEnabledHandler(PluginManager.AWBForm.StartTab.Controls("btnStart"), Nothing)
            AWBButtonsEnabledHandler(PluginManager.AWBForm.StartTab.Controls("btnPreview"), Nothing)
            AWBButtonsEnabledHandler(PluginManager.AWBForm.SaveButton, Nothing)
            AWBButtonsEnabledHandler(PluginManager.AWBForm.SkipButton, Nothing)
        End Sub
    End Class
End Namespace