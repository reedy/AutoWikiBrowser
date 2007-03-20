Namespace AutoWikiBrowser.Plugins.SDKSoftware.Kingbotk.Components
    Friend NotInheritable Class TimerStats
        Private WithEvents webcontrol As WikiFunctions.Browser.WebControl
        Private TimeSpan As TimeSpan
        Private Start As Date
        Private mNumberOfEdits As Integer

        Private Property NumberOfEdits() As Integer
            Get
                Return mNumberOfEdits
            End Get
            Set(ByVal value As Integer)
                mNumberOfEdits = value
                EditsLabel.Text = mNumberOfEdits.ToString
            End Set
        End Property
        Friend Sub Init(ByVal w As WikiFunctions.Browser.WebControl)
            webcontrol = w
            ResetVars()
            Timer1.Enabled = True
        End Sub
        Friend Sub Reset()
            ResetVars()
            TimerLabel.Text = "0:00"
            SpeedLabel.Text = "0"
            EditsLabel.Text = "0"
        End Sub
        Private Sub ResetVars()
            NumberOfEdits = 0
            Start = Date.Now
        End Sub
        Private Sub Timer1_Tick(ByVal sender As Object, ByVal e As EventArgs) Handles Timer1.Tick
            Static regexp As New Regex("\..*")
            Dim result As Double

            TimeSpan = Date.Now - Start
            TimerLabel.Text = regexp.Replace(TimeSpan.ToString, "")
            result = Math.Round(TimeSpan.TotalSeconds / NumberOfEdits, 2)
            If Double.IsInfinity(result) Then
                SpeedLabel.Text = "0"
            Else
                SpeedLabel.Text = result.ToString & " s/p"
            End If
        End Sub

        Private Sub webcontrol_BusyChanged() Handles webcontrol.BusyChanged
            If Not webcontrol.Busy Then
                Timer1.Enabled = False
                TimeSpan = Nothing
                webcontrol = Nothing
            End If
        End Sub
        Friend Sub IncrementSavedEdits() Handles webcontrol.Saved
            NumberOfEdits += 1
        End Sub
    End Class
End Namespace