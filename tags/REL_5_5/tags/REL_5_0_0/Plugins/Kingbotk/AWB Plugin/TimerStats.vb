'Copyright © 2008 Stephen Kennedy (Kingboyk) http://www.sdk-software.com/
'Copyright © 2008 Sam Reed (Reedy) http://www.reedyboy.net/

'This program is free software; you can redistribute it and/or modify it under the terms of Version 2 of the GNU General Public License as published by the Free Software Foundation.

'This program is distributed in the hope that it will be useful,
'but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

'You should have received a copy of the GNU General Public License Version 2 along with this program; if not, write to the Free Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
Imports WikiFunctions.API

Namespace AutoWikiBrowser.Plugins.Kingbotk.Components
    Friend NotInheritable Class TimerStats
        Private WithEvents editor As AsyncApiEdit
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
        Friend Sub Init(ByVal e As AsyncApiEdit, ByVal ETALabel As Label, _
        ByVal Stats As PluginSettingsControl.Stats)

            If Not TimerEnabled Then
                editor = e

                ResetVars()
                mETALabel = ETALabel

                TimerEnabled = True
                mStats = Stats

                Timer1_Tick(Nothing, Nothing)
            End If
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
        Friend Sub StopStats()
            ResetVars()
            TimerEnabled = False
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
        Private Property TimerEnabled() As Boolean
            Set(ByVal value As Boolean)
                If mETALabel IsNot Nothing Then
                    mETALabel.Visible = value
                End If
                Timer1.Enabled = value
            End Set
            Get
                Return Timer1.Enabled
            End Get
        End Property

        ' Event handlers
        Private ReadOnly timerregexp As New Regex("\..*")
        Private Sub Timer1_Tick(ByVal sender As Object, ByVal e As EventArgs) Handles Timer1.Tick
            Static UpdateETACount As Integer
            Dim SecondsPerPage As Double

            UpdateETACount += 1
            TimeSpan = Date.Now - Start
            TimerLabel.Text = timerregexp.Replace(TimeSpan.ToString, "")
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
        Friend Sub IncrementSavedEdits(ByVal sender As AsyncApiEdit, ByVal save As SaveInfo) Handles editor.SaveComplete
            IncrementSavedEdits()
        End Sub
        Friend Sub IncrementSavedEdits()
            NumberOfEdits += 1
        End Sub
        Private Sub mStats_SkipMisc(ByVal val As Integer) Handles mStats.SkipMisc
            mSkipped += 1
        End Sub
    End Class
End Namespace