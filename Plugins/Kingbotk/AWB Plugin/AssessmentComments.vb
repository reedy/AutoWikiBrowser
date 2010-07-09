'Copyright © 2008 Stephen Kennedy (Kingboyk) http://www.sdk-software.com/
'Copyright © 2008 Sam Reed (Reedy) http://www.reedyboy.net/

'This program is free software; you can redistribute it and/or modify it under the terms of Version 2 of the GNU General Public License as published by the Free Software Foundation.

'This program is distributed in the hope that it will be useful,
'but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

'You should have received a copy of the GNU General Public License Version 2 along with this program; if not, write to the Free Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
Imports WikiFunctions.API

Namespace AutoWikiBrowser.Plugins.Kingbotk.ManualAssessments
    Friend NotInheritable Class AssessmentComments

        Private mTalkTitle As String, mClss As Classification, mNeedsInfobox As Boolean, mNeedsPhoto As Boolean
        Private mStats As TimerStats

        Private Const conPhotoText As String = "The article would be improved by the addition of a photograph"
        Private Const conPlaceholder As String = "{{{PLACEHOLDERdeletedwhenyoupresssave}}}"

        Private articleText As String

        Friend WithEvents editor As AsyncApiEdit

        Friend Sub New(ByVal e As AsyncApiEdit)
            InitializeComponent()

            editor = e.Clone()
        End Sub

        Friend Overloads Sub ShowDialog(ByVal Clss As Classification, ByVal NeedsInfobox As Boolean, _
        ByVal NeedsPhoto As Boolean, ByVal TalkTitle As String, ByVal TimerStats As TimerStats)
            mTalkTitle = TalkTitle
            mClss = Clss
            mNeedsInfobox = NeedsInfobox
            mNeedsPhoto = NeedsPhoto
            mStats = TimerStats
            PluginManager.AWBForm.TraceManager.ProcessingArticle(TalkTitle & "/Comments", [Namespace].Talk)
            Me.ShowDialog()
        End Sub

        Private Sub AddLine(ByVal Line As String)
            articleText = articleText.Replace(conPlaceholder, _
               conPlaceholder & Microsoft.VisualBasic.vbCrLf & "* " & Line)
        End Sub

        ' Form events:
        Private Sub AssessmentComments_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
            Application.UseWaitCursor = True

            editor.Open(mTalkTitle & "/Comments", False)

            editor.Wait()

            Application.UseWaitCursor = False

            Dim str As String = editor.Page.Text

            If Not str.Trim = "" Then str += Microsoft.VisualBasic.vbCrLf + Microsoft.VisualBasic.vbCrLf
            str += String.Format("I have assessed this article as {0}-class and identified the " & _
               "following areas for improvement:", mClss.ToString)
            If mNeedsInfobox Then str += Microsoft.VisualBasic.vbCrLf + "* The article needs an infobox"
            If mNeedsPhoto Then str += Microsoft.VisualBasic.vbCrLf + "* " + conPhotoText
            articleText = str & Microsoft.VisualBasic.vbCrLf & conPlaceholder & _
               Microsoft.VisualBasic.vbCrLf & "~~~~"

            btnSave.Enabled = True
        End Sub

        ' Webcontrol events:
        Private Sub editor_Saved(ByVal sender As AsyncApiEdit, ByVal save As SaveInfo) Handles editor.SaveComplete
            Application.UseWaitCursor = False
            mStats.IncrementSavedEdits()
            PluginManager.AWBForm.TraceManager.WriteArticleActionLine("Comments page saved", Assessments.conMe)
            Me.DialogResult = Windows.Forms.DialogResult.OK
            Me.Close()
        End Sub

        Private Sub editor_StatusChanged(ByVal sender As AsyncApiEdit) Handles editor.StateChanged
            ToolStripStatusLabel1.Text = CStr(editor.State)
        End Sub

        ' Save/skip:
        Private Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSave.Click
            Application.UseWaitCursor = True

            For Each ctl As Control In Me.Controls
                Dim btn As Button = CType(ctl, Button)
                If btn IsNot Nothing Then
                    btn.Enabled = False
                End If

            Next

            editor.Save(articleText.Replace(Microsoft.VisualBasic.vbCrLf & conPlaceholder, ""), conWikiPluginBrackets & "Article assessment comments", _
                        False, WikiFunctions.API.WatchOptions.NoChange)

            editor.Wait()
        End Sub
        Private Sub SkipButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles SkipButton.Click
            PluginManager.AWBForm.TraceManager.WriteArticleActionLine("Comments page skipped", Assessments.conMe)
            Me.DialogResult = Windows.Forms.DialogResult.Cancel
            Me.Close()
        End Sub

        ' Text insertion buttons:
        Private Sub ReferencesButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles ReferencesButton.Click
            AddLine("The article needs references")
        End Sub
        Private Sub CitationsButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CitationsButton.Click
            AddLine("The article needs inline citations")
        End Sub
        Private Sub PhotoButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PhotoButton.Click
            AddLine(conPhotoText)
        End Sub
        Private Sub CleanupButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CleanupButton.Click
            AddLine("The article needs cleaning up")
        End Sub
        Private Sub ExpansionButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExpansionButton.Click
            AddLine("The article is not comprehensive and needs expansion")
        End Sub
        Private Sub CopyeditButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CopyeditButton.Click
            AddLine("The article would benefit from copyediting")
        End Sub
        Private Sub LeadButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LeadButton.Click
            AddLine("Expand the lead per [[WP:LEAD]]")
        End Sub
        Private Sub SectionsButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SectionsButton.Click
            AddLine("Add appropriate section headings per [[WP:MOS]]")
        End Sub
        Private Sub ToneButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToneButton.Click
            AddLine("The article needs to be written in an enyclopedic tone")
        End Sub
    End Class
End Namespace