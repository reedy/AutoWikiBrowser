Imports System.Windows.Forms

Namespace AutoWikiBrowser.Plugins.SDKSoftware.Kingbotk.ManualAssessments
    Friend NotInheritable Class ManualAssessmentsInstructionsDialog
        Private Sub OK_Button_Click(ByVal sender As Object, ByVal e As EventArgs) Handles OK_Button.Click
            If CheckBox1.Checked _
               Then Me.DialogResult = Windows.Forms.DialogResult.Yes _
               Else Me.DialogResult = System.Windows.Forms.DialogResult.OK

            Me.Close()
        End Sub
    End Class
End Namespace