' Manual assessment:
Namespace AutoWikiBrowser.Plugins.SDKSoftware.Kingbotk.ManualAssessments
    Friend NotInheritable Class Assessments
        Implements IDisposable

        Friend Const conMe As String = "Wikipedia Assessments Plugin"

        ' Objects:
        Private WithEvents webcontrol As WikiFunctions.Browser.WebControl
        Private WithEvents AWBSave As Button, OurSave As Button
        Private WithEvents AWBSkip As Button, OurSkip As Button
        Private WithEvents AWBPreview As Button, OurPreview As Button
        Private AWBCleanupCheckboxes As New List(Of CheckBox)
        Private EditSummaryBox As ComboBox, Status As ToolStripStatusLabel
        Private listmaker As WikiFunctions.Lists.ListMaker, PluginSettings As PluginSettingsControl
        Private State As New StateClass

        ' Events:
        Event Preview()
        Event StopAWB()

        ' Regex:
        Private Shared ReqphotoAnyRegex As New Regex("\{\{\s*(template\s*:\s*|)\s*reqphoto", _
           RegexOptions.IgnoreCase Or RegexOptions.Compiled Or RegexOptions.ExplicitCapture)

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

            State.strEditSummary = EditSummaryBox.Text

            ' Get a reference to the cleanup checkboxes:
            For Each ctl As Control In AWBOptionsTab.Controls("groupBox6").Controls
                AWBCleanupCheckboxes.Add(DirectCast(ctl, CheckBox))
            Next
            ToggleAWBCleanup(PluginSettings.Cleanup)

            AddHandler PluginSettings.CleanupCheckBox.CheckedChanged, _
               AddressOf Me.CleanupCheckBox_CheckedChanged
        End Sub

#Region "IDisposable"
        ' TODO: Why is this object getting events after setting to nothing? Why is Finalize() not running until app closes?!

        Private disposed As Boolean = False        ' To detect redundant calls

        ' This procedure is where the actual cleanup occurs
        Private Sub Dispose(ByVal disposing As Boolean)
            ' Exit now if the object has already been disposed
            If disposed Then Exit Sub

            If disposing Then
                ' The object is being disposed, not finalized.
                ' It is safe to access other objects (other than the mybase object)
                ' only from inside this block
                RemoveHandler PluginSettings.CleanupCheckBox.CheckedChanged, _
                   AddressOf Me.CleanupCheckBox_CheckedChanged
            End If

            ' Perform cleanup that has to be executed in either case:
            webcontrol = Nothing
            AWBSave = Nothing
            AWBSkip = Nothing
            AWBPreview = Nothing
            AWBCleanupCheckboxes = Nothing
            EditSummaryBox = Nothing
            Status = Nothing
            listmaker = Nothing
            PluginSettings = Nothing
            State = Nothing

            ' Remember that this object has been disposed of:
            Me.disposed = True
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            Debug.WriteLine("Disposing of AssessmentClass object")
            ' Execute the code that does the cleanup.
            Dispose(True)
            ' Let the CLR know that Finalize doesn't have to be called.
            GC.SuppressFinalize(Me)
        End Sub

        Protected Overrides Sub Finalize()
            MyBase.Finalize()
            Debug.WriteLine("Finalizing AssessmentClass object")
            ' Execute the code that does the cleanup.
            Dispose(False)
        End Sub
#End Region

        ' Friend methods:
        Friend Sub Reset()
            ToggleAWBCleanup(False)

            If listmaker.Count > 0 Then
                If listmaker.Count > 1 AndAlso listmaker(1).Name = State.strNextTalkPageExpected Then _
                   listmaker.RemoveAt(1)

                If listmaker(0).Name = State.strNextTalkPageExpected Then listmaker.RemoveAt(0)
            End If

            PreviewButtonColour(True)
            State = New StateClass
            ResetUserEditSummary()
        End Sub
        Friend Sub ResetUserEditSummary()
            ' Resets the edit summary back to what the user chose
            EditSummaryBox.Text = State.strEditSummary
        End Sub
        Friend Sub ProcessMainSpaceArticle(ByVal ArticleTitle As String)
            If State.blnNextArticleShouldBeTalk Then
                IsThisABug("a talk page")
            End If

            State.strNextTalkPageExpected = "Talk:" & ArticleTitle

            If listmaker.Count < 2 Then
                listmaker.Insert(1, State.strNextTalkPageExpected)
            Else
                If Not listmaker(1).Name = State.strNextTalkPageExpected Then _
                listmaker.Insert(1, State.strNextTalkPageExpected)
            End If

            State.blnNextEventShouldBeMainSpace = True
            State.blnNextArticleShouldBeTalk = True
        End Sub
        Friend Function ProcessTalkPage(ByVal TheArticle As Article, _
        ByVal PluginSettings As PluginSettingsControl, ByVal ActivePlugins As List(Of PluginBase), _
        ByVal Manager As PluginManager) As Boolean

            If Not State.blnNextArticleShouldBeTalk Then
                IsThisABug("an article")
            ElseIf Not State.strNextTalkPageExpected = TheArticle.FullArticleTitle Then
                IsThisABug(State.strNextTalkPageExpected)
            Else
                State.blnNextArticleShouldBeTalk = False

                PluginManager.StatusText.Text = "Assessments plugin: please assess the article or click cancel"

                Dim frmDialog As New AssessmentForm

                ProcessTalkPage = (frmDialog.ShowDialog(State.Classification, State.Importance, _
                   State.NeedsInfobox, State.NeedsAttention, State.ShowComments, _
                   PluginSettings.AssessmentsAlwaysLeaveAComment, State.NeedsPhoto, _
                   State.strNextTalkPageExpected) = DialogResult.OK)

                If ProcessTalkPage Then
                    PluginManager.StatusText.Text = "Processing " & TheArticle.FullArticleTitle

                    For Each p As PluginBase In ActivePlugins
                        p.ProcessTalkPage(TheArticle, State.Classification, State.Importance, State.NeedsInfobox, _
                           State.NeedsAttention, True, PluginBase.ProcessTalkPageMode.ManualAssessment)
                        If TheArticle.PluginManagerGetSkipResults = SkipResults.SkipBadTag Then
                            MessageBox.Show("Bad tag(s), please fix manually.", "Bad tag", MessageBoxButtons.OK, _
                               MessageBoxIcon.Exclamation)
                            Exit Function
                        End If
                    Next

                    If State.NeedsPhoto Then
                        If Manager.HaveReqPhotoEnabledTemplate Then
                            TheArticle.ReplaceReqphotoWithTemplateParams(conMe)
                        ElseIf Not ReqphotoAnyRegex.IsMatch(TheArticle.AlteredArticleText) Then
                            TheArticle.AddReqPhoto(conMe)
                        Else
                            PluginSettingsControl.MyTrace.WriteArticleActionLine1( _
                               "Photo needed: Template already present, no action taken", conMe, True)
                        End If
                    Else
                        If Manager.HaveReqPhotoEnabledTemplate Then
                            ' We haven't marked that we need a photo, but there might still be a replacement to do
                            TheArticle.ReplaceReqphotoWithTemplateParams(conMe)
                        End If

                        For Each p As PluginBase In ActivePlugins
                            p.DisposeofArticle()
                        Next
                    End If
                Else
                    PluginSettings.PluginStats.SkippedMiscellaneousIncrement(False)
                    PluginManager.StatusText.Text = "Skipping this talk page"
                    LoadArticle()
                End If
            End If
        End Function

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

            If Not Cleanup Then EditSummaryBox.Text = State.strEditSummary
        End Sub
        Private Sub LoadTalkPage()
            'State.blnWaitingForATalkPage = True

            ToggleAWBCleanup(False)
            PreviewButtonColour(True)

            PluginManager.StatusText.Text = "Assessments plugin: loading talk page"
        End Sub
        Private Sub LoadArticle()
            ToggleAWBCleanup(PluginSettings.Cleanup)

            If PluginSettings.Cleanup Then
                With EditSummaryBox
                    If Not (.Text = "clean up" OrElse .Text = "") Then State.strEditSummary = .Text
                    .Text = "clean up"
                End With
            End If

            PreviewButtonColour(False)

            If State.ShowComments Then DoShowComments()
        End Sub
        Private Sub DoShowComments()
            Dim frmComments As New AssessmentComments

            State.ShowComments = False
            frmComments.ShowDialog(State.Classification, State.NeedsInfobox, State.NeedsPhoto, _
               State.strNextTalkPageExpected, PluginSettings.TimerStats1)
        End Sub

        ' UI event handlers:
        Private Sub Save_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles AWBSave.Click, OurSave.Click
            If Not disposed Then ' TODO: this is a hack
                If State.blnNextEventShouldBeMainSpace Then
                    LoadTalkPage()
                Else
                    LoadArticle()
                End If

                State.blnNextEventShouldBeMainSpace = Not State.blnNextEventShouldBeMainSpace
            End If
        End Sub
        Private Sub Skip_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles AWBSkip.Click, OurSkip.Click
            If Not disposed Then ' TODO: this is a hack
                If State.blnNextEventShouldBeMainSpace Then
                    LoadTalkPage()
                Else
                    LoadArticle()
                    PluginSettingsControl.MyTrace.SkippedArticle("User", "User clicked Ignore")
                    PluginSettings.PluginStats.SkippedMiscellaneousIncrement(True)
                End If

                State.blnNextEventShouldBeMainSpace = Not State.blnNextEventShouldBeMainSpace
            End If
        End Sub
        Private Sub Preview_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles AWBPreview.Click, OurPreview.Click
            If Not disposed Then ' TODO: this is a hack
                PreviewButtonColour(True)
            End If
        End Sub
        Private Sub CleanupCheckBox_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs)
            ' TODO: this is a hack
            If Not disposed AndAlso PluginSettings.Cleanup AndAlso Not webcontrol.Busy Then ToggleAWBCleanup(True)
        End Sub

        ' Webcontrol event handlers:
        Private Sub webcontrol_BusyChanged() Handles webcontrol.BusyChanged
            If Not disposed Then ' TODO: this is a hack
                If webcontrol.Busy Then
                    LoadArticle()
                Else
                    Reset()
                End If
            End If
        End Sub
        'Private Sub webcontrol_Diffed() Handles webcontrol.Diffed
        '    ' we need to find a way of determining if there are any changes or not, so that (If
        '    ' there aren't) we can request Preview
        'End Sub

        ' State:
        Private NotInheritable Class StateClass
            Friend LastArticle As String
            Friend strNextTalkPageExpected As String
            Friend strEditSummary As String

            Friend blnNextEventShouldBeMainSpace As Boolean
            Friend blnNextArticleShouldBeTalk As Boolean

            ' Assessment:
            Friend Classification As Classification, Importance As Importance, ShowComments As Boolean
            Friend NeedsInfobox As Boolean, NeedsAttention As Boolean, NeedsPhoto As Boolean
        End Class
    End Class
End Namespace