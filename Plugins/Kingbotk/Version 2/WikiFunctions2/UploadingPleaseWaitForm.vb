Namespace Logging.Uploader
    ''' <summary>
    ''' A form for displaying when the application is busy uploading
    ''' </summary>
    Public Class UploadingPleaseWaitForm
        Private oldCursor As Cursor

        Private Sub Form_Closing(ByVal sender As Object, ByVal e As FormClosingEventArgs) Handles Me.FormClosing
            Me.Cursor = oldCursor
        End Sub
        Private Sub Form_Shown(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Shown
            oldCursor = Me.Cursor
            Me.Cursor = Cursors.WaitCursor
        End Sub
    End Class
End Namespace