'Namespace AWB.Plugins.SDKSoftware.Kingbotk.Components
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
        NumberOfEdits = 0
        Start = Date.Now
        Timer1.Enabled = True
    End Sub
    Private Sub Timer1_Tick(ByVal sender As Object, ByVal e As EventArgs) Handles Timer1.Tick
        Static regexp As New Regex("\..*")
        Dim result As Double

        TimeSpan = Date.Now - Start
        TimerLabel.Text = regexp.Replace(TimeSpan.ToString, "")
        result = Math.Round(TimeSpan.TotalSeconds / NumberOfEdits, 2)
        If Not Double.IsInfinity(result) Then SpeedLabel.Text = result.ToString & " s/p"
    End Sub

    Private Sub webcontrol_BusyChanged() Handles webcontrol.BusyChanged
        ' TODO: TimerStats.webcontrol_BusyChanged currently disabled because event is incorrect
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
'end namespace