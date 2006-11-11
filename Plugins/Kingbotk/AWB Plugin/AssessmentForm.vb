Imports System.Windows.Forms

Friend NotInheritable Class AssessmentForm

    Friend Shared Sub AllowOnlyOneCheckedItem(ByVal sender As Object, ByVal e As ItemCheckEventArgs) _
    Handles ClassCheckedListBox.ItemCheck, ImportanceCheckedListBox.ItemCheck
        Dim ListBox = DirectCast(sender, CheckedListBox)

        If e.NewValue = CheckState.Checked Then
            For Each i As Integer In ListBox.CheckedIndices
                ListBox.SetItemChecked(i, False)
            Next
        End If
    End Sub
    Friend Overloads Function ShowDialog(ByRef Clss As Classification, ByRef Imp As Importance, _
    ByRef Infobox As Boolean, ByRef Attention As Boolean, ByRef Comments As Boolean, _
    ByRef AlwaysComments As Boolean, ByRef NeedsPhoto As Boolean, ByVal Title As String) As DialogResult
        Const conComments As Integer = 3

        Me.Text += ": " & Title

        SettingsCheckedListBox.SetItemChecked(conComments, AlwaysComments)
        SettingsCheckedListBox.SetItemChecked(conComments + 1, AlwaysComments)

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
        Comments = (SettingsCheckedListBox.GetItemCheckState(conComments) = CheckState.Checked)
        AlwaysComments = (SettingsCheckedListBox.GetItemCheckState(conComments + 1) = CheckState.Checked)
    End Function

    ' Button event handlers:
    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles OK_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub
End Class
