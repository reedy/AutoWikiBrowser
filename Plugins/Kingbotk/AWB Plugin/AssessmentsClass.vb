'Copyright © 2008 Stephen Kennedy (Kingboyk) http://www.sdk-software.com/
'Copyright © 2008 Sam Reed (Reedy) http://www.reedyboy.net/

'This program is free software; you can redistribute it and/or modify it under the terms of Version 2 of the GNU General Public License as published by the Free Software Foundation.

'This program is distributed in the hope that it will be useful,
'but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

'You should have received a copy of the GNU General Public License Version 2 along with this program; if not, write to the Free Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA

' Manual assessment:
Imports WikiFunctions.API

Namespace AutoWikiBrowser.Plugins.Kingbotk.ManualAssessments
    Friend NotInheritable Class Assessments
        Implements IDisposable

        Friend Const conMe As String = "Wikipedia Assessments Plugin"

        ' Objects:
        Private AWBCleanupCheckboxes As New List(Of CheckBox)
        Private Status As ToolStripStatusLabel
        Private PluginSettings As PluginSettingsControl
        Private State As New StateClass

        ' Regex:
        Private Shared ReadOnly ReqphotoAnyRegex As New Regex(TemplatePrefix & "reqphoto", _
           RegexOptions.IgnoreCase Or RegexOptions.Compiled Or RegexOptions.ExplicitCapture)

        ' New:
        Friend Sub New(ByVal vPluginSettings As PluginSettingsControl)
            PluginSettings = vPluginSettings

            ' Get a reference to the cleanup checkboxes:
            For Each ctl As Control In PluginManager.AWBForm.OptionsTab.Controls("groupBox6").Controls
                If ctl.GetType Is GetType(CheckBox) Then AWBCleanupCheckboxes.Add(DirectCast(ctl, CheckBox))
            Next
            ToggleAWBCleanup(PluginSettings.Cleanup)

            AddHandler PluginSettings.CleanupCheckBox.CheckedChanged, AddressOf Me.CleanupCheckBox_CheckedChanged
            AddHandler PluginManager.AWBForm.TheSession.StateChanged, AddressOf Me.EditorStatusChanged
        End Sub

#Region "IDisposable"
            Private disposed As Boolean        ' To detect redundant calls

            ' This procedure is where the actual cleanup occurs
        Private Sub Dispose(ByVal disposing As Boolean)
            ' Exit now if the object has already been disposed
            If disposed Then Exit Sub

            If disposing Then
                ' The object is being disposed, not finalized.
                ' It is safe to access other objects (other than the mybase object)
                ' only from inside this block
                RemoveHandler PluginSettings.CleanupCheckBox.CheckedChanged, AddressOf Me.CleanupCheckBox_CheckedChanged
                RemoveHandler PluginManager.AWBForm.TheSession.StateChanged, AddressOf Me.EditorStatusChanged
            End If

            ' Perform cleanup that has to be executed in either case:
            AWBCleanupCheckboxes = Nothing
            Status = Nothing
            PluginSettings = Nothing
            State = Nothing

            ' Remember that this object has been disposed of:
            Me.disposed = True
        End Sub

        Friend Sub Dispose() Implements IDisposable.Dispose
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

            With PluginManager.AWBForm.ListMaker
                If .Count > 0 Then
                    If .Count > 1 AndAlso State.IsNextPage(.Item(1).Name) Then .RemoveAt(1)

                    If State.IsNextPage(.Item(0).Name) Then .RemoveAt(0)
                End If
            End With

            State = New StateClass
        End Sub
        Friend Sub ProcessMainSpaceArticle(ByVal ArticleTitle As String)
            If State.NextArticleShouldBeTalk Then
                IsThisABug("a talk page")
            End If

            State.NextTalkPageExpected = ArticleTitle

            With PluginManager.AWBForm.ListMaker
                If .Count < 2 Then
                    .Insert(1, State.NextTalkPageExpected)
                ElseIf Not State.IsNextPage(.Item(1).Name) Then
                    .Insert(1, State.NextTalkPageExpected)
                End If
            End With

            State.NextEventShouldBeMainSpace = True
            State.NextArticleShouldBeTalk = True
        End Sub
        Friend Function ProcessTalkPage(ByVal TheArticle As Article, ByVal PluginSettings As PluginSettingsControl, _
        ByVal Manager As PluginManager, ByRef ReqPhoto As Boolean) As Boolean
            Dim WeAddedAReqPhotoParam As Boolean

            If Not State.NextArticleShouldBeTalk Then
                IsThisABug("an article")
            ElseIf Not State.IsNextPage(TheArticle.FullArticleTitle) Then
                IsThisABug(State.NextTalkPageExpected)
            Else
                State.NextArticleShouldBeTalk = False

                PluginManager.StatusText.Text = "Assessments plugin: please assess the article or click cancel"

                Dim frmDialog As New AssessmentForm

                ProcessTalkPage = (frmDialog.ShowDialog(State.Classification, State.Importance, _
                   State.NeedsInfobox, State.NeedsAttention, State.ShowComments, _
                   PluginSettings.AssessmentsAlwaysLeaveAComment, State.NeedsPhoto, _
                   State.NextTalkPageExpected) = DialogResult.OK)

                If ProcessTalkPage Then
                    PluginManager.StatusText.Text = "Processing " & TheArticle.FullArticleTitle

                    If State.NeedsPhoto AndAlso ReqphotoAnyRegex.IsMatch(TheArticle.AlteredArticleText) Then
                        PluginManager.AWBForm.TraceManager.WriteArticleActionLine1( _
                           "Photo needed: Template already present", conMe, True)
                    End If

                    For Each p As PluginBase In PluginManager.ActivePlugins
                        If p.ProcessTalkPage(TheArticle, State.Classification, State.Importance, State.NeedsInfobox, _
                           State.NeedsAttention, True, ProcessTalkPageMode.ManualAssessment, _
                           ReqPhoto OrElse State.NeedsPhoto) AndAlso (ReqPhoto OrElse State.NeedsPhoto) AndAlso _
                           p.HasReqPhotoParam Then WeAddedAReqPhotoParam = True
                        If TheArticle.PluginManagerGetSkipResults = SkipResults.SkipBadTag Then
                            MessageBox.Show("Bad tag(s), please fix manually.", "Bad tag", MessageBoxButtons.OK, _
                               MessageBoxIcon.Exclamation)
                            Exit For
                        End If
                    Next
                Else
                    PluginSettings.PluginStats.SkippedMiscellaneousIncrement(False)
                    PluginManager.StatusText.Text = "Skipping this talk page"
                    LoadArticle()
                End If
            End If

            If ProcessTalkPage Then
                Select Case State.Classification
                    Case Classification.Code, Classification.Unassessed
                        TheArticle.EditSummary = "Assessed article using " & conWikiPlugin
                    Case Else
                        TheArticle.EditSummary = "Assessing as " & State.Classification.ToString & " class, using " & _
                           conWikiPlugin
                End Select

                ReqPhoto = WeAddedAReqPhotoParam
            Else
                ReqPhoto = False
            End If
        End Function

        ' Private:
        Private Sub IsThisABug(ByVal text As String)
            PluginManager.StatusText.Text = "Unexpected sequence: Assessments plugin is stopping AWB."
            MessageBox.Show("The assessments plugin was expecting to receive " & text & _
               " next. Is this a bug or is your list messed up?", "Expecting " & text, _
               MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            PluginManager.StopAWB()
        End Sub
        Private Sub ToggleAWBCleanup(ByVal Cleanup As Boolean)
            For Each chk As CheckBox In AWBCleanupCheckboxes
                chk.Checked = Cleanup
            Next
        End Sub
        Private Sub LoadTalkPage()
            'State.blnWaitingForATalkPage = True

            ToggleAWBCleanup(False)

            PluginManager.StatusText.Text = "Assessments plugin: loading talk page"
        End Sub
        Private Sub LoadArticle()
            ToggleAWBCleanup(PluginSettings.Cleanup)

            If State.ShowComments Then DoShowComments()
        End Sub
        Private Sub DoShowComments()
            Dim frmComments As New AssessmentComments(PluginManager.AWBForm.TheSession.Editor)

            State.ShowComments = False
            frmComments.ShowDialog(State.Classification, State.NeedsInfobox, State.NeedsPhoto, _
               State.NextTalkPageExpected, PluginSettings.TimerStats1)
        End Sub
        Private Sub CleanupCheckBox_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs)
            If Not disposed AndAlso PluginSettings.Cleanup Then _
               ToggleAWBCleanup(True)
        End Sub

        ' Webcontrol event handlers:
        Private Sub EditorStatusChanged(ByVal sender As AsyncApiEdit)
            If PluginManager.AWBForm.TheSession.Editor.IsActive Then
                LoadArticle()
            End If
        End Sub

        ' State:
        Private NotInheritable Class StateClass
            Friend LastArticle As String

            Dim page As String

            Friend Property NextTalkPageExpected() As String
                Get
                    Return page
                End Get
                Set(ByVal value As String)
                    pageRegex = New Regex(Variables.NamespacesCaseInsensitive([Namespace].Talk) & value)
                    page = Variables.Namespaces([Namespace].Talk) & value
                End Set
            End Property

            Friend EditSummary As String

            Friend pageRegex As Regex

            Friend Function IsNextPage(ByVal title As String) As Boolean
                Return pageRegex.IsMatch(title)
            End Function

            Friend NextEventShouldBeMainSpace As Boolean, NextArticleShouldBeTalk As Boolean

            ' Assessment:
            Friend Classification As Classification, Importance As Importance, ShowComments As Boolean
            Friend NeedsInfobox As Boolean, NeedsAttention As Boolean, NeedsPhoto As Boolean
        End Class
    End Class
End Namespace