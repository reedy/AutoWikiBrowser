'Copyright © 2008 Stephen Kennedy (Kingboyk) http://www.sdk-software.com/
'Copyright © 2008 Sam Reed (Reedy) http://www.reedyboy.net/

'This program is free software; you can redistribute it and/or modify it under the terms of Version 2 of the GNU General Public License as published by the Free Software Foundation.

'This program is distributed in the hope that it will be useful,
'but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

'You should have received a copy of the GNU General Public License Version 2 along with this program; if not, write to the Free Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA

Namespace AutoWikiBrowser.Plugins.Kingbotk.Plugins
    Friend Class GenericWithWorkgroups
        Implements IGenericSettings

        Public Sub New(ByVal template As String, ByVal prefix As String, ByVal autoStubEnabled As Boolean, _
                       ByVal inspectUnsetEnabled As Boolean, ByVal ParamArray params() As TemplateParameters)
            ' This call is required by the designer.
            InitializeComponent()

            Me.Template = template
            Me.Prefix = prefix
            Me.params = params

            AutoStubCheckBox.Enabled = autoStubEnabled
            InspectUnsetCheckBox.Enabled = inspectUnsetEnabled

            LinkLabel1.Text = "{{" & template & "}}"
            InsertTemplateToolStripMenuItem.Text = "{{" & template & "}}"

            ProjectToolStripMenuItem.Text = template

            Dim groupsAndMenus As New Dictionary(Of String, LvGrpTSMenu)

            ListView1.BeginUpdate()

            For Each prop As TemplateParameters In params
                Dim lvi As New ListViewItem(prop.ParamName)
                lvi.Name = prop.StorageKey
                lvi.Tag = prop

                Dim tsmiToAddTo As ToolStripMenuItem

                If prop.Group <> "" Then
                    Dim group As String = prop.Group.Replace(" ", "")
                    If Not groupsAndMenus.ContainsKey(group) Then

                        Dim lvGroup As New ListViewGroup() With {.Header = prop.Group}
                        ListView1.Groups.Add(lvGroup)

                        Dim tsmi As New ToolStripMenuItem(prop.Group)
                        ProjectToolStripMenuItem.DropDownItems.Add(tsmi)

                        groupsAndMenus.Add(group, New LvGrpTSMenu() With {.group = lvGroup, .menu = tsmi}) 'Cache group and menu item

                        tsmiToAddTo = tsmi

                        lvi.Group = lvGroup
                    Else
                        Dim mnu As LvGrpTSMenu = groupsAndMenus(group)
                        tsmiToAddTo = mnu.menu
                        lvi.Group = mnu.group
                    End If
                Else
                    tsmiToAddTo = ProjectToolStripMenuItem
                End If

                Dim tsiSub As ToolStripItem = tsmiToAddTo.DropDownItems.Add(prop.ParamName)
                'tsi.Tag = prop
                AddHandler tsiSub.Click, AddressOf ToolStripMenuItemClickEventHandler

                ListView1.Items.Add(lvi)
            Next
            ListView1.EndUpdate()
        End Sub

        Private Sub ToolStripMenuItemClickEventHandler(ByVal sender As Object, ByVal e As EventArgs)
            Dim tsi As ToolStripItem = DirectCast(sender, ToolStripItem)

            PluginManager.EditBoxInsertYesParam(tsi.Text)
        End Sub

        Protected Prefix As String
        Protected Template As String

        Dim params() As TemplateParameters

        Private Const conForceImportanceRemoval As String = "RmImportance"
        Private Const conStubClassParm As String = "StubClass"
        Private Const conAutoStubParm As String = "AutoStub"

#Region "XML interface"
        Friend Sub ReadXML(ByVal Reader As System.Xml.XmlTextReader) Implements IGenericSettings.ReadXML
            For Each lvi As ListViewItem In ListView1.Items
                Dim tp As TemplateParameters = DirectCast(lvi.Tag, TemplateParameters)
                lvi.Checked = PluginManager.XMLReadBoolean(Reader, Prefix & tp.StorageKey, lvi.Checked)
            Next

            StubClass = PluginManager.XMLReadBoolean(Reader, Prefix & conStubClassParm, StubClass)

            If InspectUnsetCheckBox.Enabled Then
                InspectUnsetParameters = PluginManager.XMLReadBoolean(Reader, Prefix & conForceImportanceRemoval, InspectUnsetParameters)
            End If

            If AutoStubCheckBox.Enabled Then
                PluginManager.XMLReadBoolean(Reader, Prefix & conAutoStubParm, AutoStub)
            End If
        End Sub

        Friend Sub WriteXML(ByVal Writer As System.Xml.XmlTextWriter) Implements IGenericSettings.WriteXML
            With Writer
                For Each lvi As ListViewItem In ListView1.Items
                    Dim tp As TemplateParameters = DirectCast(lvi.Tag, TemplateParameters)
                    .WriteAttributeString(Prefix & tp.StorageKey, lvi.Checked.ToString)
                Next

                .WriteAttributeString(Prefix & conStubClassParm, StubClass.ToString)

                If InspectUnsetCheckBox.Enabled Then
                    .WriteAttributeString(Prefix & conForceImportanceRemoval, InspectUnsetParameters.ToString)
                End If

                If AutoStubCheckBox.Enabled Then
                    .WriteAttributeString(Prefix & conAutoStubParm, AutoStub.ToString)
                End If
            End With
        End Sub

        Friend Sub Reset() Implements IGenericSettings.XMLReset
            StubClass = False
            AutoStub = False
            InspectUnsetParameters = False

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
                Return AutoStubCheckBox.Enabled AndAlso AutoStubCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                AutoStubCheckBox.Checked = value
            End Set
        End Property

        Friend Property InspectUnsetParameters() As Boolean
            Get
                Return InspectUnsetCheckBox.Enabled AndAlso InspectUnsetCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                InspectUnsetCheckBox.Checked = value
            End Set
        End Property

        Friend WriteOnly Property InspectUnsetText() As String
            Set(ByVal value As String)
                InspectUnsetCheckBox.Text = value
            End Set
        End Property

        Friend Property ExtraChecks() As Boolean
            Get
                Return ExtraCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                ExtraCheckBox.Checked = value
            End Set
        End Property

        Friend WriteOnly Property ExtraChecksText() As String
            Set(ByVal value As String)
                ExtraCheckBox.Text = value
                ExtraCheckBox.Visible = value <> ""
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
            Tools.OpenENArticleInBrowser(WikiFunctions.Variables.Namespaces(WikiFunctions.Namespace.Template) & Template, False)
        End Sub

        Private Sub InsertTemplateToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles InsertTemplateToolStripMenuItem.Click
            PluginManager.EditBoxInsert("{{" & Template & "}}")
        End Sub

        Private Sub AutoStubCheckBox_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AutoStubCheckBox.CheckedChanged
            If AutoStubCheckBox.Checked Then StubClassCheckBox.Checked = False
        End Sub
        Private Sub StubClassCheckBox_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles StubClassCheckBox.CheckedChanged
            If StubClassCheckBox.Checked Then AutoStubCheckBox.Checked = False
        End Sub
    End Class
End Namespace