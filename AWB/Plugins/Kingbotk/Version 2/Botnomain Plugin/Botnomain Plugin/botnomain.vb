' A simple proof-of-concept plugin to add a template to bluelink talk pages attached to redlink articles.
' (c) 2006 Kingboyk, released under the GPL.

Public Class BotNoMainPlugin
    Implements IAWBPlugin
    ' NOTE: This plugin doesn't currently skip if the talk page has already been tagged. This might be done by
    ' searching for the template name in the article text (using .Contains or a regex), or by checking if the page
    ' is in Category:Talk pages with no main page. It may also be done just as easily - or more easily - by using
    ' the AWB "skip if..." feature.

    Private Const conPluginName As String = "BotNoMain Plugin"

    ' UI objects:
    Private StatusText As New ToolStripStatusLabel("Initialising " & conPluginName)
    Private WithEvents OurMenuItem As New ToolStripMenuItem(conPluginName)
    Private WithEvents WebControl As WikiFunctions.Browser.WebControl
    Private cmboEditSummary As ComboBox

    ' Properties:
    Public ReadOnly Property Name() As String Implements WikiFunctions.Plugin.IAWBPlugin.Name
        Get
            Return conPluginName
        End Get
    End Property
    Private Property Enabled() As Boolean
        Get
            Return OurMenuItem.Checked
        End Get
        Set(ByVal IsEnabled As Boolean)
            OurMenuItem.Checked = IsEnabled
            If IsEnabled Then
                StatusText.Text = conPluginName & " enabled"
                ' Set edit summary:
                If Not cmboEditSummary.Text.ToLower.Contains("botnomain") _
                   Then cmboEditSummary.Text = "{{db-botnomain}}"
            Else
                StatusText.Text = conPluginName & " disabled"
            End If
        End Set
    End Property

    ' Initialisation:
    Public Sub Initialise(ByVal list As WikiFunctions.Lists.ListMaker, ByVal web As WikiFunctions.Browser.WebControl, _
    ByVal tsmi As ToolStripMenuItem, ByVal cms As ContextMenuStrip, ByVal tab As TabControl, ByVal frm As Form, _
    ByVal txt As TextBox) Implements WikiFunctions.Plugin.IAWBPlugin.Initialise
        ' Initialise and add our menu item:
        With OurMenuItem
            .CheckOnClick = True
            .Checked = False
            .ToolTipText = "Enable/disable the " & conPluginName
        End With
        tsmi.DropDownItems.Add(OurMenuItem)

        ' Set up our status text panel:
        DirectCast(frm.Controls("statusStrip1"), StatusStrip).Items.Insert(2, StatusText)
        StatusText.Margin = New Padding(35, 0, 35, 0)
        StatusText.BorderSides = ToolStripStatusLabelBorderSides.Left Or ToolStripStatusLabelBorderSides.Right
        StatusText.BorderStyle = Border3DStyle.Etched

        ' Store UI refs:
        cmboEditSummary = DirectCast(tab.TabPages("tpStart").Controls("cmboEditSummary"), ComboBox)
        WebControl = web
    End Sub

    ' Article processing:
    Public Function ProcessArticle(ByVal ArticleText As String, ByVal ArticleTitle As String, _
    ByVal NS As Integer, ByRef Summary As String, ByRef Skip As Boolean) As String _
    Implements WikiFunctions.Plugin.IAWBPlugin.ProcessArticle
        Const conTalkNamespace As Integer = 1

        If Enabled Then
            ' Process the page if it's a talk page, it exists, and the attached article *doesn't* exist:
            If (NS = conTalkNamespace) And WebControl.TalkPageExists And Not WebControl.ArticlePageExists Then
                StatusText.Text = "Processing " & ArticleTitle
                Return "{{db-botnomain}}" & Microsoft.VisualBasic.vbCrLf & ArticleText
            Else
                StatusText.Text = "Skipped " & ArticleTitle
                Skip = True
                Return ArticleText
            End If
        Else
            Return ArticleText
        End If
    End Function

    ' XML settings:
    Private Const conEnabled As String = "BotNoMainEnabled"

    Public Sub ReadXML(ByVal Reader As System.Xml.XmlTextReader) Implements WikiFunctions.Plugin.IAWBPlugin.ReadXML
        ' Read our enabled state from XML settings, if the attribute doesn't exist turn this plugin off:
        If Reader.MoveToAttribute(conEnabled) Then Enabled = Boolean.Parse(Reader.Value) Else Enabled = False
    End Sub
    Public Sub Reset() Implements WikiFunctions.Plugin.IAWBPlugin.Reset
    End Sub
    Public Sub WriteXML(ByVal Writer As System.Xml.XmlTextWriter) Implements WikiFunctions.Plugin.IAWBPlugin.WriteXML
        Writer.WriteAttributeString(conEnabled, Enabled.ToString)
    End Sub

    ' AWB events (we don't use these but must declare them) :
    Public Event Diff() Implements WikiFunctions.Plugin.IAWBPlugin.Diff
    Public Event Preview() Implements WikiFunctions.Plugin.IAWBPlugin.Preview
    Public Event Save() Implements WikiFunctions.Plugin.IAWBPlugin.Save
    Public Event Skip() Implements WikiFunctions.Plugin.IAWBPlugin.Skip
    Public Event Start() Implements WikiFunctions.Plugin.IAWBPlugin.Start
    Public Event [Stop]() Implements WikiFunctions.Plugin.IAWBPlugin.Stop

    ' UI event handlers:
    Private Sub OurMenuItem_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) _
    Handles OurMenuItem.CheckedChanged
        Enabled = OurMenuItem.Checked
    End Sub
    Private Sub WebControl_BusyChanged() Handles WebControl.BusyChanged
        If WebControl.Busy Then StatusText.Text = conPluginName & " waiting" Else StatusText.Text = "Stopped"
    End Sub
End Class
