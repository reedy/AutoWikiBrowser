'Copyright © 2008 Stephen Kennedy (Kingboyk) http://www.sdk-software.com/
'Copyright © 2008 Sam Reed (Reedy) http://www.reedyboy.net/

'This program is free software; you can redistribute it and/or modify it under the terms of Version 2 of the GNU General Public License as published by the Free Software Foundation.

'This program is distributed in the hope that it will be useful,
'but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

'You should have received a copy of the GNU General Public License Version 2 along with this program; if not, write to the Free Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA

Imports System.Windows.Forms

Namespace AutoWikiBrowser.Plugins.Kingbotk.ManualAssessments
    Friend NotInheritable Class AssessmentForm

        Friend Shared Sub AllowOnlyOneCheckedItem(ByVal sender As Object, ByVal e As ItemCheckEventArgs) _
        Handles ClassCheckedListBox.ItemCheck, ImportanceCheckedListBox.ItemCheck
            Dim ListBox As CheckedListBox = DirectCast(sender, CheckedListBox)

            If e.NewValue = CheckState.Checked Then
                For Each i As Integer In ListBox.CheckedIndices
                    ListBox.SetItemChecked(i, False)
                Next
            End If
        End Sub
        Friend Overloads Function ShowDialog(ByRef Clss As Classification, ByRef Imp As Importance, _
        ByRef Infobox As Boolean, ByRef Attention As Boolean, ByRef Comments As Boolean, _
        ByRef NeedsPhoto As Boolean, ByVal Title As String) As DialogResult
            Const conComments As Integer = 3

            Me.Text += ": " & Title

            ShowDialog = Me.ShowDialog()
            If ClassCheckedListBox.SelectedIndices.Count = 0 Then
                Clss = Classification.Unassessed
            Else
                Clss = DirectCast(ClassCheckedListBox.SelectedIndex, Classification)
            End If
            If ImportanceCheckedListBox.SelectedIndices.Count = 0 Then
                Imp = Importance.Unassessed
            Else
                Imp = DirectCast(ImportanceCheckedListBox.SelectedIndex, Importance)
            End If
            Infobox = (SettingsCheckedListBox.GetItemCheckState(0) = CheckState.Checked)
            Attention = (SettingsCheckedListBox.GetItemCheckState(1) = CheckState.Checked)
            NeedsPhoto = (SettingsCheckedListBox.GetItemCheckState(2) = CheckState.Checked)
        End Function

        ' Button event handlers:
        Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles OK_Button.Click
            Me.Close()
        End Sub

        Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles Cancel_Button.Click
            Me.Close()
        End Sub
        
    End Class
End Namespace