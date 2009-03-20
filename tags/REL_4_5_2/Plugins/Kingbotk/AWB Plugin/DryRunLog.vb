'Copyright © 2008 Stephen Kennedy (Kingboyk) http://www.sdk-software.com/
'Copyright © 2008 Sam Reed (Reedy) http://www.reedyboy.net/

'This program is free software; you can redistribute it and/or modify it under the terms of Version 2 of the GNU General Public License as published by the Free Software Foundation.

'This program is distributed in the hope that it will be useful,
'but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

'You should have received a copy of the GNU General Public License Version 2 along with this program; if not, write to the Free Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA

Imports System.IO
Imports System.Text.encoding

Namespace AutoWikiBrowser.Plugins.Kingbotk
    Friend NotInheritable Class DryRunLog
        ' If there comes a request to log more info in the dry run, this could maybe implement IMyTraceListener.
        ' I think for now though this solution is perfectly adequate.
        Private Log As StreamWriter
        Private mFileName As String
        Private mPluginSettings As PluginSettingsControl

        Friend Function Initialise(ByVal PluginSettings As PluginSettingsControl) As Boolean
            mFileName = PluginSettings.SaveFileDialog1.FileName
            mPluginSettings = PluginSettings
            Initialise = OpenLog()

            If Initialise Then
                With PluginSettings
                    .ManuallyAssessCheckBox.Enabled = False
                    .CleanupCheckBox.Enabled = False
                End With
            End If
        End Function
        Friend Sub Close()
            mPluginSettings.ManuallyAssessCheckBox.Enabled = Not PluginManager.BotMode
            mPluginSettings = Nothing
            Log.Close()
            Log.Dispose()
            System.Diagnostics.Process.Start(mFileName)
        End Sub
        Friend Sub WriteArticleLine(ByVal Title As String, ByVal Minor As Boolean, _
        ByVal Sender As String)
            If Minor Then
                Log.WriteLine("#[[" & Title & "]] (minor)")
            Else
                Log.WriteLine("#[[" & Title & "]]")
            End If
            Log.Flush()
            PluginManager.AWBForm.TraceManager.SkippedArticle(Sender, "Dry run")
        End Sub

        Private Function OpenLog() As Boolean
            OpenLog = True

            Try
                Log = New StreamWriter(mFileName, True, UTF8)
            Catch ex As Exception
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                OpenLog = False
            End Try
        End Function
    End Class
End Namespace