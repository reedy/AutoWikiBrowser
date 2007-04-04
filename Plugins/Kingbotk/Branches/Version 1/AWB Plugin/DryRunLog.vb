Imports System.IO
Imports System.Text.encoding

Friend NotInheritable Class DryRunLog
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

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
        Log.Close()
        Log.Dispose()
    End Sub

    Friend Sub WriteArticleLine(ByVal Title As String, ByVal Minor As Boolean)
        If Minor Then
            Log.WriteLine("#[[" & Title & "]] (minor)")
        Else
            Log.WriteLine("#[[" & Title & "]]")
        End If
        Log.Flush()
    End Sub
End Class
