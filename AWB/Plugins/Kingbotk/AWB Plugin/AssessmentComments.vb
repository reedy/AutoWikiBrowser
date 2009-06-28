'Copyright © 2008 Stephen Kennedy (Kingboyk) http://www.sdk-software.com/
'Copyright © 2008 Sam Reed (Reedy) http://www.reedyboy.net/

'This program is free software; you can redistribute it and/or modify it under the terms of Version 2 of the GNU General Public License as published by the Free Software Foundation.

'This program is distributed in the hope that it will be useful,
'but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

'You should have received a copy of the GNU General Public License Version 2 along with this program; if not, write to the Free Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA

Namespace AutoWikiBrowser.Plugins.Kingbotk.ManualAssessments
    Friend NotInheritable Class AssessmentComments

        Private mTalkTitle As String, mClss As Classification, mNeedsInfobox As Boolean, mNeedsPhoto As Boolean
        Private mStats As TimerStats

        Private Const conPhotoText As String = "The article would be improved by the addition of a photograph"
        Private Const conPlaceholder As String = "{{{PLACEHOLDERdeletedwhenyoupresssave}}}"

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
            WebControl1.SetArticleText(WebControl1.GetArticleText.Replace(conPlaceholder, _
               conPlaceholder & Microsoft.VisualBasic.vbCrLf & "* " & Line))
        End Sub

        ' Form events:
        Private Sub AssessmentComments_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
            Application.UseWaitCursor = True
            WebControl1.LoadEditPage(mTalkTitle & "/Comments")
        End Sub

        ' Webcontrol events:
        Private Sub WebControl1_Loaded(ByVal sender As Object, ByVal e As System.EventArgs) Handles WebControl1.Loaded
            Dim str As String

            Application.UseWaitCursor = False

            With WebControl1
                If Not .HasArticleTextBox Then
                    MessageBox.Show("We don't seem to have an editable text box.", "Error", MessageBoxButtons.OK, _
                       MessageBoxIcon.Error)
                    Me.DialogResult = Windows.Forms.DialogResult.Abort
                    Me.Close()
                Else
                    str = .GetArticleText
                    If Not str.Trim = "" Then str += Microsoft.VisualBasic.vbCrLf + Microsoft.VisualBasic.vbCrLf
                    str += String.Format("I have assessed this article as {0}-class and identified the " & _
                       "following areas for improvement:", mClss.ToString)
                    If mNeedsInfobox Then str += Microsoft.VisualBasic.vbCrLf + "* The article needs an infobox"
                    If mNeedsPhoto Then str += Microsoft.VisualBasic.vbCrLf + "* " + conPhotoText
                    .SetArticleText(str & Microsoft.VisualBasic.vbCrLf & conPlaceholder & _
                       Microsoft.VisualBasic.vbCrLf & "~~~~")
                End If
            End With

            btnSave.Enabled = True
        End Sub
        Private Sub WebControl1_Saved(ByVal sender As Object, ByVal e As System.EventArgs) Handles WebControl1.Saved
            Application.UseWaitCursor = False
            mStats.IncrementSavedEdits(sender, e)
            PluginManager.AWBForm.TraceManager.WriteArticleActionLine("Comments page saved", Assessments.conMe)
            Me.DialogResult = Windows.Forms.DialogResult.OK
            Me.Close()
        End Sub
        Private Sub WebControl1_StatusChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles WebControl1.StatusChanged
            ToolStripStatusLabel1.Text = WebControl1.Status
        End Sub

        ' Save/skip:
        Private Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSave.Click
            Application.UseWaitCursor = True

            For Each ctl As Control In Me.Controls
                If TypeOf ctl Is Button Then DirectCast(ctl, Button).Enabled = False
            Next

            WebControl1.SetArticleText(WebControl1.GetArticleText.Replace( _
               Microsoft.VisualBasic.vbCrLf & conPlaceholder, ""))
            WebControl1.SetSummary(conWikiPluginBrackets & "Article assessment comments")
            WebControl1.Save()
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