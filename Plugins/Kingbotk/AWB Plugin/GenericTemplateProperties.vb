Namespace AutoWikiBrowser.Plugins.SDKSoftware.Kingbotk.Components
    Friend NotInheritable Class GenericTemplatePropertiesForm

        Private Sub OK_Button_Click(ByVal sender As Object, ByVal e As EventArgs) Handles OK_Button.Click
            Me.Close()
        End Sub

        Friend Shared Sub DoRegexTextBox(ByVal txt As TextBox, ByVal regx As Regex)
            If regx Is Nothing Then
                txt.Text = "<not set>"
            Else
                txt.Text = regx.ToString
            End If
        End Sub
    End Class
End Namespace