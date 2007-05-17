Namespace AutoWikiBrowser.Plugins.SDKSoftware.Kingbotk.Components
    Friend NotInheritable Class AboutBox
        Private Sub AboutBox_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Me.TextBoxDescription.Text = _
               "An AWB plugin for adding and updating WikiProject templates on Wikipedia talk pages. " & _
               Microsoft.VisualBasic.vbCrLf & Microsoft.VisualBasic.vbCrLf & "AWB Version: " & _
               Application.ProductVersion.ToString & Microsoft.VisualBasic.vbCrLf & Microsoft.VisualBasic.vbCrLf & _
               "Made in England. Store in a dry place and consume within 7 days of opening. COMES WITH NO WARRANTY - " _
               & "check your edits and use sensibly!"
        End Sub

        Private Sub OKButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OKButton.Click
            Me.Close()
        End Sub

        Public Shared ReadOnly Property Version() As String
            Get
                Return System.Reflection.Assembly.GetExecutingAssembly.GetName.Version.ToString
            End Get
        End Property

        Public Sub New()
            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            Me.LabelVersion.Text = String.Format("Version {0}", Version)
        End Sub
        Private Sub linkKingboy_LinkClicked(ByVal sender As System.Object, _
        ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles linkKingboy.LinkClicked
            linkKingboy.LinkVisited = True
            Tools.OpenENArticleInBrowser("Kingboyk", True)
        End Sub
        Private Sub linkReedy_LinkClicked(ByVal sender As System.Object, _
        ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles linkReedy.LinkClicked
            linkReedy.LinkVisited = True
            Tools.OpenENArticleInBrowser("Reedy Boy", True)
        End Sub
    End Class
End Namespace