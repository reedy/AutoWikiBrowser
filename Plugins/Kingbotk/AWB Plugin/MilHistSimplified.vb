'Copyright © 2008 Stephen Kennedy (Kingboyk) http://www.sdk-software.com/
'Copyright © 2008 Sam Reed (Reedy) http://www.reedyboy.net/

'This program is free software; you can redistribute it and/or modify it under the terms of Version 2 of the GNU General Public License as published by the Free Software Foundation.

'This program is distributed in the hope that it will be useful,
'but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

'You should have received a copy of the GNU General Public License Version 2 along with this program; if not, write to the Free Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA

Namespace AutoWikiBrowser.Plugins.Kingbotk.Plugins


    Friend NotInheritable Class WPMilHistSimplfiedSettings
        Implements IGenericSettings

        Public Sub New()

            ' This call is required by the designer.
            InitializeComponent()

            Dim ts As New Dictionary(Of String, ToolStripMenuItem)

            For Each prop As TemplateProperties In params
                Dim lvi As New ListViewItem(prop.ParamName)
                lvi.Tag = prop
                If prop.Group <> "" Then
                    Dim lvGroup As New ListViewGroup(prop.Group.Replace(" ", "")) With {.Header = prop.Group}

                    If Not ListView1.Groups.Contains(lvGroup) Then
                        ListView1.Groups.Add(lvGroup)

                        Dim tsmi As New ToolStripMenuItem(prop.Group)
                        ts.Add(prop.Group, tsmi)
                        WPMilHistToolStripMenuItem1.DropDownItems.Add(tsmi)
                    End If

                    Dim tsi As ToolStripMenuItem = ts(prop.Group)
                    Dim tsiSub As ToolStripItem = tsi.DropDownItems.Add(prop.ParamName)
                    'tsi.Tag = prop
                    AddHandler tsiSub.Click, AddressOf ToolStripMenuItemClickEventHandler

                    lvi.Group = lvGroup
                End If

                ListView1.Items.Add(lvi)
            Next
        End Sub

        Private Sub ToolStripMenuItemClickEventHandler(ByVal sender As Object, ByVal e As EventArgs)
            Dim tsi As ToolStripItem = DirectCast(sender, ToolStripItem)

            PluginManager.EditBoxInsertYesParam(tsi.Text)
        End Sub

        Class TemplateProperties
            Public StorageKey As String
            Public Group As String
            Public ParamName As String
        End Class

        Const PeriodsAndConflictsGroup As String = "Periods and Conflicts"
        Const GeneralGroup As String = "General Task Forces"
        Const NationsGroup As String = "Nations and Regions"

        Dim params() As TemplateProperties =
        {
            New TemplateProperties() With {.StorageKey = "MilHistWWII", .Group = PeriodsAndConflictsGroup, .ParamName = "WWII"}, _
            New TemplateProperties() With {.StorageKey = "MilHistAir", .Group = GeneralGroup, .ParamName = "Aviation"}, _
            New TemplateProperties() With {.StorageKey = "MilHistAfrican", .Group = NationsGroup, .ParamName = "Africa"}
        }
        'New TemplateProperties() With {.StorageKey = "", .Group = "", .ParamName = ""}

#Region "XML interface"
        Friend Sub ReadXML(ByVal Reader As System.Xml.XmlTextReader) Implements IGenericSettings.ReadXML
            For Each lvi As ListViewItem In ListView1.Items
                Dim tp As TemplateProperties = DirectCast(lvi.Tag, TemplateProperties)
                lvi.Checked = PluginManager.XMLReadBoolean(Reader, tp.StorageKey, lvi.Checked)
            Next

        End Sub
        Friend Sub WriteXML(ByVal Writer As System.Xml.XmlTextWriter) Implements IGenericSettings.WriteXML
            With Writer
                For Each lvi As ListViewItem In ListView1.Items
                    Dim tp As TemplateProperties = DirectCast(lvi.Tag, TemplateProperties)
                    .WriteAttributeString(tp.StorageKey, lvi.Checked.ToString)
                Next
            End With
        End Sub
        Friend Sub Reset() Implements IGenericSettings.XMLReset
            StubClass = False
            AutoStub = False
            ForceImportanceRemoval = False

            For Each lvi As ListViewItem In ListView1.Items
                lvi.Checked = False
            Next
        End Sub
#End Region
        ' Properties:
        Friend Property StubClass() As Boolean Implements IGenericSettings.StubClass
            Get
                Return StubClassCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                StubClassCheckBox.Checked = value
            End Set
        End Property
        WriteOnly Property StubClassModeAllowed() As Boolean Implements IGenericSettings.StubClassModeAllowed
            Set(ByVal value As Boolean)
                StubClassCheckBox.Enabled = value
            End Set
        End Property
        Friend Property AutoStub() As Boolean Implements IGenericSettings.AutoStub
            Get
                Return False
            End Get
            Set(ByVal value As Boolean)
            End Set
        End Property
        Friend Property ForceImportanceRemoval() As Boolean
            Get
                Return RemoveImportanceCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                RemoveImportanceCheckBox.Checked = value
            End Set
        End Property
        Friend ReadOnly Property TextInsertContextMenuStripItems() As ToolStripItemCollection _
        Implements IGenericSettings.TextInsertContextMenuStripItems
            Get
                Return TextInsertContextMenuStrip.Items
            End Get
        End Property

        ' Event handlers:
        Private Sub LinkClicked(ByVal sender As Object, ByVal e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
            Tools.OpenENArticleInBrowser("Template:WPMILHIST", False)
        End Sub

        Private Sub WPMILHISTToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles WPMILHISTToolStripMenuItem.Click
            PluginManager.EditBoxInsert("{{WPMILHIST}}")
        End Sub
    End Class

    Friend NotInheritable Class WPMilHistSimple
        Inherits PluginBase

        ' Settings:
        Private OurTab As New TabPage("MilHist")
        Private WithEvents OurSettingsControl As New WPMilHistSimplfiedSettings
        Private Const conEnabled As String = "MilHistEnabled"
        Private Const conMedievalTaskForce As String = "Medieval-task-force"

        Protected Friend Overrides ReadOnly Property PluginShortName() As String
            Get
                Return "MilitaryHistory"
            End Get
        End Property
        Protected Overrides ReadOnly Property PreferredTemplateName() As String
            Get
                Return "WPMILHIST"
            End Get
        End Property
        Protected Overrides ReadOnly Property ParameterBreak() As String
            Get
                Return Microsoft.VisualBasic.vbCrLf
            End Get
        End Property
        Protected Overrides Sub ImportanceParameter(ByVal Importance As Importance)
            ' WPMILHIST doesn't do importance
        End Sub
        Protected Friend Overrides ReadOnly Property GenericSettings() As IGenericSettings
            Get
                Return OurSettingsControl
            End Get
        End Property
        Protected Overrides ReadOnly Property CategoryTalkClassParm() As String
            Get
                Return "NA"
            End Get
        End Property
        Protected Overrides ReadOnly Property TemplateTalkClassParm() As String
            Get
                Return "NA"
            End Get
        End Property
        Friend Overrides ReadOnly Property HasSharedLogLocation() As Boolean
            Get
                Return True
            End Get
        End Property
        Friend Overrides ReadOnly Property SharedLogLocation() As String
            Get
                Return "Wikipedia:WikiProject Military history/Automation/Logs"
            End Get
        End Property
        Friend Overrides ReadOnly Property HasReqPhotoParam() As Boolean
            Get
                Return False
            End Get
        End Property
        Friend Overrides Sub ReqPhoto()
        End Sub
        'Protected Overrides ReadOnly Property PreferredTemplateNameRegexString() As String
        '    Get
        '        Return "^[Ww]PMILHIST$"
        '    End Get
        'End Property

        ' Initialisation:
        Friend Sub New()
            MyBase.New("WikiProject Military History") ' Specify alternate names only
        End Sub
        Protected Friend Overrides Sub Initialise()
            OurMenuItem = New ToolStripMenuItem("Military History Plugin")
            MyBase.InitialiseBase() ' must set menu item object first
            OurTab.UseVisualStyleBackColor = True
            OurTab.Controls.Add(OurSettingsControl)
        End Sub

        ' Article processing:
        Protected Overrides ReadOnly Property InspectUnsetParameters() As Boolean
            Get
                Return OurSettingsControl.ForceImportanceRemoval
            End Get
        End Property
        Protected Overrides Sub InspectUnsetParameter(ByVal Param As String)
            ' We only get called if InspectUnsetParameters is True
            If String.Equals(Param, "importance", StringComparison.CurrentCultureIgnoreCase) Then
                Article.DoneReplacement("importance=", "", True, PluginShortName)
            End If
        End Sub
        Protected Overrides Function SkipIfContains() As Boolean
            ' None
        End Function
        Protected Overrides Sub ProcessArticleFinish()
            StubClass()
            With OurSettingsControl
                ' If .WWII Then AddAndLogNewParamWithAYesValue("WWII")
            End With
            If Template.Parameters.ContainsKey("importance") Then
                Template.Parameters.Remove("importance")
                Article.ArticleHasAMajorChange()
                PluginManager.AWBForm.TraceManager.WriteArticleActionLine("Removed importance parameter", _
                   PluginShortName)
            End If

            If Template.Parameters.ContainsKey("AncientNE") Then
                Template.Parameters.Remove("AncientNE")
                Article.ArticleHasAMajorChange()
                PluginManager.AWBForm.TraceManager.WriteArticleActionLine("Removed AncientNE parameter", _
                   PluginShortName)
            End If

            If Template.Parameters.ContainsKey("auto") Then
                Template.Parameters.Remove("auto")
                Article.ArticleHasAMajorChange()
                PluginManager.AWBForm.TraceManager.WriteArticleActionLine("Removed auto parameter", _
                   PluginShortName)
            End If
        End Sub
        Protected Overrides Function TemplateFound() As Boolean
            Const conMiddleAges As String = "Middle-Ages-task-force"

            With Template
                If .Parameters.ContainsKey(conMiddleAges) Then
                    If .Parameters(conMiddleAges).Value.ToLower = "yes" Then
                        .NewOrReplaceTemplateParm(conMedievalTaskForce, "yes", Article, False, False, False, "", _
                           PluginShortName)
                        Article.DoneReplacement(conMiddleAges, conMedievalTaskForce, True, PluginShortName)
                    Else
                        Article.EditSummary += "deprecated Middle-Ages-task-force removed"
                        PluginManager.AWBForm.TraceManager.WriteArticleActionLine( _
                           "Middle-Ages-task-force parameter removed, not set to yes", PluginShortName)
                    End If
                    .Parameters.Remove(conMiddleAges)
                    Article.ArticleHasAMinorChange()
                End If
            End With
        End Function
        Protected Overrides Sub GotTemplateNotPreferredName(ByVal TemplateName As String)
            ' Currently only WPBio does anything here (if {{musician}} add to musician-work-group)
        End Sub
        Protected Overrides Function WriteTemplateHeader(ByRef PutTemplateAtTop As Boolean) As String
            WriteTemplateHeader = "{{WPMILHIST" & _
               Microsoft.VisualBasic.vbCrLf & WriteOutParameterToHeader("class")
        End Function

        'User interface:
        Protected Overrides Sub ShowHideOurObjects(ByVal Visible As Boolean)
            PluginManager.ShowHidePluginTab(OurTab, Visible)
        End Sub

        ' XML settings:
        Protected Friend Overrides Sub ReadXML(ByVal Reader As System.Xml.XmlTextReader)
            Dim blnNewVal As Boolean = PluginManager.XMLReadBoolean(Reader, conEnabled, Enabled)
            If Not blnNewVal = Enabled Then Enabled = blnNewVal ' Mustn't set if the same or we get extra tabs
            OurSettingsControl.ReadXML(Reader)
        End Sub
        Protected Friend Overrides Sub Reset()
            OurSettingsControl.Reset()
        End Sub
        Protected Friend Overrides Sub WriteXML(ByVal Writer As System.Xml.XmlTextWriter)
            Writer.WriteAttributeString(conEnabled, Enabled.ToString)
            OurSettingsControl.WriteXML(Writer)
        End Sub
    End Class
End Namespace