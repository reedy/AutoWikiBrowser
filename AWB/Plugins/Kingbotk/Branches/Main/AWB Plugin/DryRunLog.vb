Imports System.IO
Imports System.Text.encoding

Namespace AutoWikiBrowser.Plugins.SDKSoftware.Kingbotk
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