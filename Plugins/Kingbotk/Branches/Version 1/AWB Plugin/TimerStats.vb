Namespace AutoWikiBrowser.Plugins.SDKSoftware.Kingbotk.Components
    Friend NotInheritable Class TimerStats
        Private WithEvents webcontrol As WikiFunctions.Browser.WebControl
        Private WithEvents mStats As PluginSettingsControl.Stats
        Private TimeSpan As TimeSpan
        Private Start As Date
        Private mNumberOfEdits As Integer, mSkipped As Integer
        Private mETALabel As Label

        Private Property NumberOfEdits() As Integer
            Get
                Return mNumberOfEdits
            End Get
            Set(ByVal value As Integer)
                mNumberOfEdits = value
                EditsLabel.Text = mNumberOfEdits.ToString
            End Set
        End Property
        Friend Sub Init(ByVal w As WikiFunctions.Browser.WebControl, ByVal ETALabel As Label, _
        ByVal Stats As PluginSettingsControl.Stats)
            webcontrol = w
            ResetVars()
            mETALabel = ETALabel
            TimerEnabled = True
            mStats = Stats
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
            mSkipped = 0
        End Sub
        Private Property ETA() As String
            Get
                Return mETALabel.Text.Replace("ETC: ", "")
            End Get
            Set(ByVal value As String)
                mETALabel.Text = "ETC: " & value
            End Set
        End Property
        Private Sub CalculateETA(ByVal SecondsPerPage As Double)
            Dim Count As Integer = PluginManager.AWBForm.ListMaker.Count

            If Count < 1 Then
                ETA = "Now"
            Else
                Dim ETADateTime As Date = Date.Now.AddSeconds(SecondsPerPage * Count)

                If ETADateTime.Date = Date.Now.Date Then
                    ETA = ETADateTime.ToString("HH:mm") + " today"
                ElseIf Date.Now.AddDays(1).Date = ETADateTime.Date Then
                    ETA = ETADateTime.ToString("HH:mm") + " tomorrow"
                Else
                    ETA = ETADateTime.ToString("HH:mm, ddd d MMM")
                End If
            End If
        End Sub
        Private WriteOnly Property TimerEnabled() As Boolean
            Set(ByVal value As Boolean)
                mETALabel.Visible = value
                Timer1.Enabled = value
            End Set
        End Property

        ' Event handlers
        Private Sub Timer1_Tick(ByVal sender As Object, ByVal e As EventArgs) Handles Timer1.Tick
            Static regexp As New Regex("\..*")
            Static UpdateETACount As Integer
            Dim SecondsPerPage As Double

            UpdateETACount += 1
            TimeSpan = Date.Now - Start
            TimerLabel.Text = regexp.Replace(TimeSpan.ToString, "")
            SecondsPerPage = Math.Round(TimeSpan.TotalSeconds / NumberOfEdits, 2)

            If Double.IsInfinity(SecondsPerPage) Then
                SpeedLabel.Text = "0"
                ETA = "-"
                If UpdateETACount > 9 Then UpdateETACount = 0
            Else
                SpeedLabel.Text = SecondsPerPage.ToString & " s/p"
                If UpdateETACount > 9 OrElse ETA = "-" Then
                    UpdateETACount = 0
                    CalculateETA(TimeSpan.TotalSeconds / (NumberOfEdits + mSkipped))
                End If
            End If
        End Sub
        Private Sub webcontrol_BusyChanged() Handles webcontrol.BusyChanged
            If webcontrol.Busy Then
                mETALabel.Visible = True
            Else
                TimerEnabled = False
                TimeSpan = Nothing
                webcontrol = Nothing
            End If
        End Sub
        Friend Sub IncrementSavedEdits() Handles webcontrol.Saved
            NumberOfEdits += 1
        End Sub
        Private Sub mStats_SkipMisc(ByVal val As Integer) Handles mStats.SkipMisc
            mSkipped += 1
        End Sub
    End Class
End Namespace