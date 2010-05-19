Imports WikiFunctions

'Copyright © 2008 Stephen Kennedy (Kingboyk) http://www.sdk-software.com/
'Copyright © 2008 Sam Reed (Reedy) http://www.reedyboy.net/

'This program is free software; you can redistribute it and/or modify it under the terms of Version 2 of the GNU General Public License as published by the Free Software Foundation.

'This program is distributed in the hope that it will be useful,
'but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

'You should have received a copy of the GNU General Public License Version 2 along with this program; if not, write to the Free Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA

Namespace AutoWikiBrowser.Plugins.Kingbotk.Components
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

        Friend Shared ReadOnly Property Version() As String
            Get
                Return System.Reflection.Assembly.GetExecutingAssembly.GetName.Version.ToString
            End Get
        End Property

        Friend Sub New()
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
            Tools.OpenENArticleInBrowser("Reedy", True)
        End Sub

        Private Sub LicencingButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LicencingButton.Click
            Dim GPL As New GPLAboutBox
            GPL.ShowDialog(PluginManager.AWBForm.Form)
        End Sub

        Private Class GPLAboutBox
            Inherits WikiFunctions.Controls.AboutBox

            Protected Overrides Sub Initialise()
                Text = conAWBPluginName
                linkLabel1.Visible = False
                lblMadeBy.Text = "Made by Stephen Kennedy with Sam Reed"
                lblVersion.Text = "Version " & AboutBox.Version
                textBoxDescription.Text = _
                   AssemblyDescription(System.Reflection.Assembly.GetExecutingAssembly) & _
                   Environment.NewLine & Environment.NewLine & My.Resources.GPL
            End Sub
        End Class

    End Class
End Namespace