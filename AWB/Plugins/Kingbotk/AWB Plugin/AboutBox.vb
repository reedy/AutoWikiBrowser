Public NotInheritable Class AboutBox

    Private Sub AboutBox_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ' Set the title of the form.
        'Dim ApplicationTitle As String
        'If My.Application.Info.Title <> "" Then
        '    ApplicationTitle = My.Application.Info.Title
        'Else
        '    ApplicationTitle = System.IO.Path.GetFileNameWithoutExtension(My.Application.Info.AssemblyName)
        'End If
        'Me.Text = String.Format("About {0}", ApplicationTitle)
        ' Initialize all of the text displayed on the About Box.
        ' TODO: Customize the application's assembly information in the "Application" pane of the project 
        '    properties dialog (under the "Project" menu).
        'Me.LabelProductName.Text = My.Application.Info.ProductName
        'Me.LabelVersion.Text = String.Format("Version {0}", System.Reflection.Assembly.GetExecutingAssembly.GetName.Version.ToString) 'My.Application.Info.Version.ToString)
        Me.LabelCopyright.Text = "Copyright © SDK Software 2006"
        Me.LabelCompanyName.Text = "Written by Kingboyk"
        Me.TextBoxDescription.Text = _
           "An AWB plugin for adding and updating WikiProject templates on Wikipedia talk pages"
        Me.LabelProductName.Text = "Kingbotk Templating Plugin"
        Me.Text = "About the Kingbotk Templating Plugin"
    End Sub

    Private Sub OKButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OKButton.Click
        Me.Close()
    End Sub

    Public Sub New(ByVal Version As String)

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.LabelVersion.Text = Version
    End Sub
End Class
