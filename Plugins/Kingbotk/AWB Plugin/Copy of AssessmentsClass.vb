' Manual assessment:
Namespace AWB.Plugins.SDKSoftware.Kingbotk
    Friend Class Assessments
        ' Objects:
        Private WithEvents webcontrol As WikiFunctions.Browser.WebControl
        Private WithEvents AWBSave As Button, OurSave As Button
        Private WithEvents AWBSkip As Button, OurSkip As Button
        Private WithEvents AWBPreview As Button, OurPreview As Button
        Private AWBCleanupCheckboxes As New List(Of CheckBox)
        Private EditSummaryBox As ComboBox, Status As ToolStripStatusLabel
        Private listmaker As WikiFunctions.Lists.ListMaker, PluginSettings As PluginSettingsControl

        ' State:
        Private strEditSummary As String
        Private strNextTalkPageExpected As String
        Private blnNextEventShouldBeMainSpace As Boolean = False
        Private blnNextArticleShouldBeTalk As Boolean = False
        Private blnSkipNextTalk As Boolean = False, blnWaitingForATalkPage As Boolean = False
        Friend Sub Reset()
            ToggleAWBCleanup(False)
            blnNextEventShouldBeMainSpace = False
            blnNextArticleShouldBeTalk = False
            blnSkipNextTalk = False
            blnWaitingForATalkPage = False
            If listmaker.Count > 0 Then
                If listmaker.Count > 1 Then _
                   If listmaker(1).Name = strNextTalkPageExpected Then listmaker.RemoveAt(1)
                If listmaker(0).Name = strNextTalkPageExpected Then listmaker.RemoveAt(0)
            End If
            PreviewButtonColour(True)
        End Sub

        ' Assessment:
        Private Classification As Classification, Importance As Importance, ShowComments As Boolean
        Private NeedsInfobox As Boolean, NeedsAttention As Boolean, NeedsPhoto As Boolean

        ' Events:
        Event Preview()
        Event StopAWB()
        Event Skip()

        ' New:
        Friend Sub New(ByVal web As WikiFunctions.Browser.WebControl, ByVal vAWBSave As Button, _
        ByVal vOurSave As Button, ByVal vAWBSkip As Button, ByVal vOurSkip As Button, _
        ByVal AWBOptionsTab As TabPage, ByVal vEditSummaryBox As ComboBox, _
        ByVal vlistmaker As WikiFunctions.Lists.ListMaker, ByVal vPluginSettings As PluginSettingsControl, _
        ByVal vAWBPreview As Button, ByVal vOurPreview As Button)
            webcontrol = web
            AWBPreview = vAWBPreview
            AWBSave = vAWBSave
            AWBSkip = vAWBSkip
            OurPreview = vOurPreview
            OurSave = vOurSave
            OurSkip = vOurSkip
            EditSummaryBox = vEditSummaryBox
            listmaker = vlistmaker
            PluginSettings = vPluginSettings

            strEditSummary = EditSummaryBox.Text

            ' Get a reference to the cleanup checkboxes:
            For Each ctl As Control In AWBOptionsTab.Controls("groupBox6").Controls
                AWBCleanupCheckboxes.Add(DirectCast(ctl, CheckBox))
            Next
            ToggleAWBCleanup(PluginSettings.Cleanup)

            AddHandler PluginSettings.CleanupCheckBox.CheckedChanged, _
               AddressOf Me.CleanupCheckBox_CheckedChanged
        End Sub

        ' Friend methods:
        Friend Sub ResetUserEditSummary()
            ' Resets the edit summary back to what the user chose
            EditSummaryBox.Text = strEditSummary
        End Sub
        Friend Sub ProcessMainSpaceArticle(ByVal ArticleTitle As String)
            If blnNextArticleShouldBeTalk Then
                IsThisABug("a talk page")
            End If

            strNextTalkPageExpected = "Talk:" & ArticleTitle

            If listmaker.Count < 2 Then
                listmaker.Insert(1, strNextTalkPageExpected)
            Else
                If Not listmaker(1).Name = strNextTalkPageExpected Then _
                listmaker.Insert(1, strNextTalkPageExpected)
            End If

            blnNextEventShouldBeMainSpace = True
            blnNextArticleShouldBeTalk = True
        End Sub
        Friend Sub ProcessTalkPage(ByVal TheArticle As Article, _
        ByVal PluginSettings As PluginSettingsControl, ByVal ActivePlugins As List(Of PluginBase))
            If Not blnNextArticleShouldBeTalk Then
                IsThisABug("an article")
            ElseIf Not strNextTalkPageExpected = TheArticle.FullArticleTitle Then
                IsThisABug(strNextTalkPageExpected)
            Else
                blnNextArticleShouldBeTalk = False

                For Each p As PluginBase In ActivePlugins
                    p.ProcessTalkPage(TheArticle, Classification, Importance, NeedsInfobox, _
                       NeedsAttention, True, PluginBase.ProcessTalkPageMode.ManualAssessment)
                    If TheArticle.PluginManagerGetSkipResults = SkipResults.SkipBadTag Then
                        MessageBox.Show("Bad tag(s), please fix manually.", "Bad tag", MessageBoxButtons.OK, _
                           MessageBoxIcon.Exclamation)
                        Exit Sub
                    End If
                Next

                If NeedsPhoto Then
                    ' TODO
                End If
            End If
        End Sub

        ' Private:
        Private Sub PreviewButtonColour(ByVal Reset As Boolean)
            If Reset Then
                AWBPreview.BackColor = Drawing.Color.Transparent
                OurPreview.BackColor = Drawing.Color.Transparent
            Else
                AWBPreview.BackColor = Drawing.Color.Red
                OurPreview.BackColor = Drawing.Color.Red
            End If
        End Sub
        Private Sub IsThisABug(ByVal text As String)
            PluginManager.StatusText.Text = "Unexpected sequence: Assessments plugin is stopping AWB."
            MessageBox.Show("The assessments plugin was expecting to receive " & text & _
               " next. Is this a bug or is your list messed up?", "Expecting " & text, _
               MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            RaiseEvent StopAWB()
        End Sub
        Private Sub ToggleAWBCleanup(ByVal Cleanup As Boolean)
            For Each chk As CheckBox In AWBCleanupCheckboxes
                chk.Checked = Cleanup
            Next

            If Not Cleanup Then EditSummaryBox.Text = strEditSummary
        End Sub
        Private Sub SaveOrSkipMainSpaceArticle()
            blnWaitingForATalkPage = True

            ToggleAWBCleanup(False)
            PreviewButtonColour(True)
            PluginManager.StatusText.Text = "Assessments plugin: please assess the article or click cancel"

            Dim frmDialog As New AssessmentForm

            blnSkipNextTalk = Not (frmDialog.ShowDialog(Classification, Importance, NeedsInfobox, _
               NeedsAttention, ShowComments, PluginSettings.AssessmentsAlwaysLeaveAComment, NeedsPhoto, _
               strNextTalkPageExpected) = DialogResult.OK)

            If blnSkipNextTalk And Not blnWaitingForATalkPage Then
                webcontrol_Loaded()
            Else
                PluginManager.StatusText.Text = "Assessments plugin: loading talk page"
            End If
        End Sub
        Private Sub SaveOrSkipTalkPage()
            ToggleAWBCleanup(PluginSettings.Cleanup)

            If PluginSettings.Cleanup Then
                With EditSummaryBox
                    If Not .Text = "clean up" Then strEditSummary = .Text
                    .Text = "clean up"
                End With
            End If

            PreviewButtonColour(False)

            If ShowComments Then DoShowComments
        End Sub
        Private Sub DoShowComments()
            Dim frmComments As New AssessmentComments

            ShowComments = False
            frmComments.ShowDialog(Classification, NeedsInfobox, NeedsPhoto, strNextTalkPageExpected)
        End Sub

        ' Event handlers:
        Private Sub Save_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles AWBSave.Click, OurSave.Click
            If blnNextEventShouldBeMainSpace Then
                SaveOrSkipMainSpaceArticle()
            Else
                SaveOrSkipTalkPage()
            End If

            blnNextEventShouldBeMainSpace = Not blnNextEventShouldBeMainSpace
        End Sub
        Private Sub Skip_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles AWBSkip.Click, OurSkip.Click
            If blnNextEventShouldBeMainSpace Then
                SaveOrSkipMainSpaceArticle()
            Else
                SaveOrSkipTalkPage()
                PluginSettingsControl.MyTrace.SkippedArticle("User", "User clicked Ignore")
                PluginSettings.PluginStats.SkippedMiscellaneousIncrement(True)
            End If

            blnNextEventShouldBeMainSpace = Not blnNextEventShouldBeMainSpace
        End Sub
        Private Sub webcontrol_BusyChanged() Handles webcontrol.BusyChanged
            If webcontrol.Busy Then
                SaveOrSkipTalkPage()
            Else
                Reset()
            End If
        End Sub
        'Private Sub webcontrol_Diffed() Handles webcontrol.Diffed
        '    ' we need to find a way of determining if there are any changes or not, so that (If
        '    ' there aren't) we can request Preview
        'End Sub
        'Private Sub webcontrol_Saved() Handles webcontrol.Saved

        'End Sub
        Private Sub CleanupCheckBox_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs)
            If PluginSettings.Cleanup And Not webcontrol.Busy Then ToggleAWBCleanup(True)
        End Sub
        Private Sub webcontrol_Loaded() Handles webcontrol.Loaded
            If blnWaitingForATalkPage Then blnWaitingForATalkPage = False
            If blnSkipNextTalk Then
                RaiseEvent Skip()
                blnSkipNextTalk = False
                PluginSettings.PluginStats.SkippedMiscellaneousIncrement(True)
                PluginManager.StatusText.Text = "Skipping this talk page"
                SaveOrSkipTalkPage()
            End If
        End Sub

        Private Sub Preview_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles AWBPreview.Click, OurPreview.Click
            PreviewButtonColour(True)
        End Sub
    End Class
End Namespace