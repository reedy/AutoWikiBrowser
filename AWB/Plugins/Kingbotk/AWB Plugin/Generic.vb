'Copyright © 2008 Stephen Kennedy (Kingboyk) http://www.sdk-software.com/
'Copyright © 2008 Sam Reed (Reedy) http://www.reedyboy.net/

'This program is free software; you can redistribute it and/or modify it under the terms of Version 2 of the GNU General Public License as published by the Free Software Foundation.

'This program is distributed in the hope that it will be useful,
'but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

'You should have received a copy of the GNU General Public License Version 2 along with this program; if not, write to the Free Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA

Namespace AutoWikiBrowser.Plugins.Kingbotk.Plugins
    Friend NotInheritable Class GenericTemplateSettings
        Implements IGenericSettings

        ' Our name:
        Private mName As String

        ' UI:
        Private WithEvents InsertTemplateCallMenuItem As ToolStripMenuItem

        ' Enums:
        Friend Enum ImportanceSettingEnum
            Imp
            Pri
            None
        End Enum

#Region "Parameter Names"
        Private ReadOnly Property conTemplateNameParm() As String
            Get
                Return mName & "GenericTemplateName"
            End Get
        End Property
        Private ReadOnly Property conTemplateAlternateNamesYNParm() As String
            Get
                Return mName & "GenericTemplateAlternateNamesYN"
            End Get
        End Property
        Private ReadOnly Property conTemplateAlternateNamesParm() As String
            Get
                Return mName & "GenericTemplateAlternateNames"
            End Get
        End Property
        Private ReadOnly Property conTemplateImportanceParm() As String
            Get
                Return mName & "GenericTemplateImp"
            End Get
        End Property
        Private ReadOnly Property conTemplateCatsParm() As String
            Get
                Return mName & "GenericTemplateCats"
            End Get
        End Property
        Private ReadOnly Property conTemplateTemplatesParm() As String
            Get
                Return mName & "GenericTemplateTempl"
            End Get
        End Property
        Private ReadOnly Property conTemplateAutoStubYNParm() As String
            Get
                Return mName & "GenericTemplateAutoStubYN"
            End Get
        End Property
        Private ReadOnly Property conSkipRegexYN() As String
            Get
                Return mName & "GenericSkipRegexYN"
            End Get
        End Property
        Private ReadOnly Property conSkipRegex() As String
            Get
                Return mName & "GenericSkipRegex"
            End Get
        End Property
        Private ReadOnly Property conAutoStubParm() As String
            Get
                Return mName & "GenericAutoStub"
            End Get
        End Property
        Private ReadOnly Property conStubClassParm() As String
            Get
                Return mName & "GenericStubClass"
            End Get
        End Property
#End Region

#Region "Properties"
        Friend Property TemplateName() As String
            Get
                Return WikiFunctions.Parse.Parsers.GetTemplateName(TemplateNameTextBox.Text, True)
            End Get
            Set(ByVal value As String)
                TemplateNameTextBox.Text = value
            End Set
        End Property
        Friend Property HasAlternateNames() As Boolean
            Get
                Return HasAlternateNamesCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                HasAlternateNamesCheckBox.Checked = value
            End Set
        End Property
        Friend Property AlternateNames() As String
            Get
                Return AlternateNamesTextBox.Text.Trim(New Char() {CChar("|"), CChar(" ")})
            End Get
            Set(ByVal value As String)
                AlternateNamesTextBox.Text = value
            End Set
        End Property
        Friend Property ImportanceSetting() As ImportanceSettingEnum
            Get
                If ImportanceCheckedListBox.CheckedIndices.Count = 0 Then
                    Return ImportanceSettingEnum.None
                Else
                    Return DirectCast(ImportanceCheckedListBox.CheckedIndices(0), ImportanceSettingEnum)
                End If
            End Get
            Set(ByVal value As ImportanceSettingEnum)
                ImportanceCheckedListBox.SetItemChecked(DirectCast(value, Integer), True)
            End Set
        End Property
        Friend Property HasCategoryClass() As Boolean
            Get
                If CatsCheckedListBox.CheckedIndices.Count = 0 Then
                    Return False
                Else
                    Return (CatsCheckedListBox.CheckedIndices(0) = 0)
                End If
            End Get
            Set(ByVal value As Boolean)
                If value Then CatsCheckedListBox.SetItemChecked(0, True) Else CatsCheckedListBox.SetItemChecked(1, True)
            End Set
        End Property
        Friend Property HasTemplateClass() As Boolean
            Get
                If TemplatesCheckedListBox.CheckedIndices.Count = 0 Then
                    Return False
                Else
                    Return (TemplatesCheckedListBox.CheckedIndices(0) = 0)
                End If
            End Get
            Set(ByVal value As Boolean)
                If value Then TemplatesCheckedListBox.SetItemChecked(0, True) _
                   Else TemplatesCheckedListBox.SetItemChecked(1, True)
            End Set
        End Property
        Friend Property SkipRegex() As String
            Get
                Return SkipRegexTextBox.Text.Trim
            End Get
            Set(ByVal value As String)
                SkipRegexTextBox.Text = value
            End Set
        End Property
        Friend Property SkipRegexYN() As Boolean
            Get
                Return SkipRegexCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                SkipRegexCheckBox.Checked = value
            End Set
        End Property
        Friend Property AutoStubYN() As Boolean
            Get
                Return AutoStubSupportYNCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                AutoStubSupportYNCheckBox.Checked = value
            End Set
        End Property
        Friend Property StubClass() As Boolean Implements IGenericSettings.StubClass
            Get
                Return StubClassCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                StubClassCheckBox.Checked = value
            End Set
        End Property
        Friend Property AutoStub() As Boolean Implements IGenericSettings.AutoStub
            Get
                Return (AutoStubCheckBox.Checked And AutoStubSupportYNCheckBox.Checked)
            End Get
            Set(ByVal value As Boolean)
                If AutoStubYN Then
                    AutoStubCheckBox.Checked = value
                Else
                    AutoStubCheckBox.Checked = False
                End If
            End Set
        End Property
        Friend ReadOnly Property TextInsertContextMenuStripItems() As ToolStripItemCollection _
        Implements IGenericSettings.TextInsertContextMenuStripItems
            Get
                Return Nothing ' not used by generic template objects
            End Get
        End Property
        Friend WriteOnly Property StubClassModeAllowed() As Boolean _
        Implements IGenericSettings.StubClassModeAllowed
            Set(ByVal value As Boolean)
                StubClassCheckBox.Enabled = value
            End Set
        End Property
#End Region

        ' Initialisation and goodbye:
        Friend Sub New(ByVal OurPluginName As String)

            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            mName = OurPluginName
            InsertTemplateCallMenuItem = New ToolStripMenuItem(mName & _
               " (you haven't entered a template name yet)")
            PluginManager.AddItemToTextBoxInsertionContextMenu(InsertTemplateCallMenuItem)
        End Sub
        Friend Sub Goodbye()
            PluginManager.GetTextBoxDropDownItems.Remove(InsertTemplateCallMenuItem)
            InsertTemplateCallMenuItem.Dispose()
            InsertTemplateCallMenuItem = Nothing
        End Sub

#Region "XML interface"
        Friend Sub ReadXML(ByVal Reader As System.Xml.XmlTextReader) Implements IGenericSettings.ReadXML
            AutoStub = PluginManager.XMLReadBoolean(Reader, conAutoStubParm, AutoStub)
            StubClass = PluginManager.XMLReadBoolean(Reader, conStubClassParm, StubClass)
            TemplateName = PluginManager.XMLReadString(Reader, conTemplateNameParm, TemplateName)
            HasAlternateNames = PluginManager.XMLReadBoolean( _
               Reader, conTemplateAlternateNamesYNParm, HasAlternateNames)
            AlternateNames = PluginManager.XMLReadString( _
               Reader, conTemplateAlternateNamesParm, AlternateNames)
            ImportanceSetting = CType(ImportanceSettingEnum.Parse(GetType(ImportanceSettingEnum), _
               PluginManager.XMLReadString(Reader, conTemplateImportanceParm, ImportanceSetting.ToString), True),  _
               ImportanceSettingEnum)
            HasCategoryClass = PluginManager.XMLReadBoolean(Reader, conTemplateCatsParm, HasCategoryClass)
            HasTemplateClass = PluginManager.XMLReadBoolean(Reader, conTemplateTemplatesParm, HasTemplateClass)
            AutoStubYN = PluginManager.XMLReadBoolean(Reader, conTemplateAutoStubYNParm, AutoStubYN)
            SkipRegexYN = PluginManager.XMLReadBoolean(Reader, conSkipRegexYN, SkipRegexYN)
            SkipRegex = PluginManager.XMLReadString(Reader, conSkipRegex, SkipRegex)
        End Sub
        Friend Sub WriteXML(ByVal Writer As System.Xml.XmlTextWriter) Implements IGenericSettings.WriteXML
            With Writer
                .WriteAttributeString(conTemplateNameParm, TemplateName)
                .WriteAttributeString(conAutoStubParm, AutoStub.ToString)
                .WriteAttributeString(conStubClassParm, StubClass.ToString)
                .WriteAttributeString(conTemplateAlternateNamesYNParm, HasAlternateNames.ToString)
                .WriteAttributeString(conTemplateAlternateNamesParm, AlternateNames)
                .WriteAttributeString(conTemplateImportanceParm, ImportanceSetting.ToString)
                .WriteAttributeString(conTemplateCatsParm, HasCategoryClass.ToString)
                .WriteAttributeString(conTemplateTemplatesParm, HasTemplateClass.ToString)
                .WriteAttributeString(conTemplateAutoStubYNParm, AutoStubYN.ToString)
                .WriteAttributeString(conSkipRegexYN, SkipRegexYN.ToString)
                .WriteAttributeString(conSkipRegex, SkipRegex)
            End With
        End Sub
        Friend Sub Reset() Implements IGenericSettings.XMLReset
            TemplateName = ""
            AutoStub = False
            StubClass = False
            HasAlternateNames = False
            AlternateNames = ""
            ImportanceSetting = ImportanceSettingEnum.None
            HasCategoryClass = False
            HasTemplateClass = False
            AutoStubYN = False
            SkipRegexYN = False
            SkipRegex = ""
        End Sub
#End Region

#Region "Event handlers"
        Private Sub LinkClicked(ByVal sender As Object, ByVal e As LinkLabelLinkClickedEventArgs) _
        Handles LinkLabel1.LinkClicked
            Tools.OpenENArticleInBrowser("Kingbotk/Plugin/Generic_WikiProject_templates", True)
        End Sub
        Private Sub ImportanceCheckedListBox_ItemCheck(ByVal sender As Object, ByVal e As ItemCheckEventArgs) _
        Handles ImportanceCheckedListBox.ItemCheck
            AssessmentForm.AllowOnlyOneCheckedItem(sender, e)
        End Sub
        Private Sub CatsCheckedListBox_ItemCheck(ByVal sender As Object, ByVal e As ItemCheckEventArgs) _
        Handles CatsCheckedListBox.ItemCheck
            AssessmentForm.AllowOnlyOneCheckedItem(sender, e)
        End Sub
        Private Sub TemplatesCheckedListBox_ItemCheck(ByVal sender As Object, ByVal e As ItemCheckEventArgs) _
        Handles TemplatesCheckedListBox.ItemCheck
            AssessmentForm.AllowOnlyOneCheckedItem(sender, e)
        End Sub
        Private Sub HasAlternateNamesCheckBox_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) _
        Handles HasAlternateNamesCheckBox.CheckedChanged
            AlternateNamesTextBox.Enabled = HasAlternateNamesCheckBox.Checked
        End Sub
        Private Sub AutoStubSupportYNCheckBox_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) _
        Handles AutoStubSupportYNCheckBox.CheckedChanged
            If AutoStubSupportYNCheckBox.Checked Then
                AutoStubCheckBox.Enabled = True
            Else
                AutoStubCheckBox.Checked = False
                AutoStubCheckBox.Enabled = False
            End If
        End Sub
        Private Sub SkipRegexCheckBox_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) _
        Handles SkipRegexCheckBox.CheckedChanged
            SkipRegexTextBox.Enabled = SkipRegexCheckBox.Checked
        End Sub
        Private Sub TemplateNameTextBox_TextChanged(ByVal sender As Object, ByVal e As EventArgs) _
        Handles TemplateNameTextBox.TextChanged
            InsertTemplateCallMenuItem.Text = "{{" & TemplateName & "}}"
            GetRedirectsButton.Enabled = Not (TemplateName = "")
        End Sub
        Private Sub InsertTemplateCallMenuItem_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles InsertTemplateCallMenuItem.Click
            PluginManager.EditBoxInsert("{{" & TemplateName & "}}")
        End Sub
#End Region
    End Class

    Friend NotInheritable Class GenericTemplatePlugin
        Inherits PluginBase
        Implements IGenericTemplatePlugin, IDisposable

        ' Objects:
        Private OurTab As TabPage
        Private WithEvents OurSettingsControl As GenericTemplateSettings
        Private WithEvents DeleteMeMenuItem As New ToolStripMenuItem("Delete")

        ' Settings:
        Private OurName As String
        Private ReadOnly Property conEnabled() As String
            Get
                Return OurName & "GenericEnabled"
            End Get
        End Property

        ' Regex:
        Private SkipRegex As Regex

        ' Initialisation:
        Friend Sub New(ByVal MyName As String)
            MyBase.New(True)
            OurSettingsControl = New GenericTemplateSettings(MyName)
            OurTab = New TabPage(MyName)
            OurName = MyName

            ' Keep track of changing configuration by suscribing to various events:
            AddHandler OurSettingsControl.SkipRegexCheckBox.CheckedChanged, AddressOf Me.SkipRegexChanged
            AddHandler OurSettingsControl.SkipRegexTextBox.TextChanged, AddressOf Me.SkipRegexChanged
            AddHandler OurSettingsControl.TemplateNameTextBox.TextChanged, AddressOf Me.TemplateNamesChanged
            AddHandler OurSettingsControl.HasAlternateNamesCheckBox.CheckedChanged, AddressOf Me.TemplateNamesChanged
            AddHandler OurSettingsControl.AlternateNamesTextBox.TextChanged, AddressOf Me.TemplateNamesChanged
            'AddHandler OurSettingsControl.AlternateNamesTextBox.EnabledChanged, AddressOf Me.TemplateNamesChanged ' CheckedChanged should covert this
            AddHandler OurSettingsControl.PropertiesButton.Click, AddressOf Me.PropertiesButtonClick
            AddHandler OurSettingsControl.GetRedirectsButton.Click, AddressOf Me.GetRedirectsButtonClick
        End Sub
        Protected Friend Overrides Sub Initialise()
            OurMenuItem = New ToolStripMenuItem(PluginShortName)
            MyBase.InitialiseBase() ' must set menu item object first
            OurTab.UseVisualStyleBackColor = True
            OurSettingsControl.Reset()
            OurTab.Controls.Add(OurSettingsControl)
            DeleteMeMenuItem.ToolTipText = "Delete the " & PluginShortName & " plugin"
            OurMenuItem.DropDownItems.Add(DeleteMeMenuItem)
        End Sub

        ' Properties:
        Protected Friend Overrides ReadOnly Property IAmReady() As Boolean
            Get
                If OurSettingsControl.TemplateName = "" Then Return False
                If MainRegex Is Nothing Then Return False
                If SecondChanceRegex Is Nothing Then Return False
                ' else:
                Return MyBase.IAmReady
            End Get
        End Property
        Protected Friend Overrides ReadOnly Property IAmGeneric() As Boolean
            Get
                Return True
            End Get
        End Property
        Protected Friend Overrides ReadOnly Property PluginShortName() As String
            Get
                Return "Generic (" & OurName & ")"
            End Get
        End Property
        Protected Overrides ReadOnly Property PreferredTemplateName() As String
            Get
                Return OurSettingsControl.TemplateName
            End Get
        End Property
        Protected Overrides Sub ImportanceParameter(ByVal Importance As Importance)
            Select Case OurSettingsControl.ImportanceSetting
                Case GenericTemplateSettings.ImportanceSettingEnum.Imp
                    Template.NewOrReplaceTemplateParm("importance", Importance.ToString, Me.Article, False, False)
                Case GenericTemplateSettings.ImportanceSettingEnum.Pri
                    Template.NewOrReplaceTemplateParm("priority", Importance.ToString, Me.Article, False, False)
                    ' Case GenericTemplateSettings.ImportanceSettingEnum.None ' do nothing
            End Select
        End Sub
        Protected Overrides ReadOnly Property CategoryTalkClassParm() As String
            Get
                If OurSettingsControl.HasCategoryClass Then
                    Return "Cat"
                Else
                    Return "NA"
                End If
            End Get
        End Property
        Protected Overrides ReadOnly Property TemplateTalkClassParm() As String
            Get
                If OurSettingsControl.HasTemplateClass Then
                    Return "Template"
                Else
                    Return "NA"
                End If
            End Get
        End Property
        Protected Friend Overrides ReadOnly Property GenericSettings() As IGenericSettings
            Get
                Return OurSettingsControl
            End Get
        End Property
        Protected Overrides ReadOnly Property ParameterBreak() As String
            Get
                Return ""
            End Get
        End Property

        ' Article processing:
        Protected Overrides Function SkipIfContains() As Boolean
            If Not SkipRegex Is Nothing Then
                Try
                    Return (SkipRegex.Matches(Article.AlteredArticleText).Count > 0)
                Catch ex As Exception
                    MessageBox.Show("Error processing skip regular expression: " & Microsoft.VisualBasic.vbCrLf & _
                       Microsoft.VisualBasic.vbCrLf & ex.ToString, "Error", MessageBoxButtons.OK, _
                       MessageBoxIcon.Error)
                    PluginManager.StopAWB()
                End Try
            End If
        End Function
        Protected Overrides Function TemplateFound() As Boolean
            ' Nothing to do here
        End Function
        Protected Overrides Sub GotTemplateNotPreferredName(ByVal TemplateName As String)
        End Sub
        Protected Overrides Function WriteTemplateHeader(ByRef PutTemplateAtTop As Boolean) As String
            WriteTemplateHeader = "{{" & PreferredTemplateName & WriteOutParameterToHeader("class")

            Select Case OurSettingsControl.ImportanceSetting
                Case GenericTemplateSettings.ImportanceSettingEnum.Imp
                    WriteTemplateHeader += WriteOutParameterToHeader("importance")
                Case GenericTemplateSettings.ImportanceSettingEnum.Pri
                    WriteTemplateHeader += WriteOutParameterToHeader("priority")
            End Select
        End Function
        Protected Overrides Sub ProcessArticleFinish()
            StubClass()
        End Sub

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
        '' These do nothing because generic templates already have a AlternateNames XML property
        Friend Overrides Sub ReadXMLRedirects(ByVal Reader As System.Xml.XmlTextReader)
        End Sub
        Friend Overrides Sub WriteXMLRedirects(ByVal Writer As System.Xml.XmlTextWriter)
        End Sub

        ' Our interface:
        Friend ReadOnly Property GenericTemplateKey() As String _
        Implements IGenericTemplatePlugin.GenericTemplateKey
            Get
                Return OurName
            End Get
        End Property

        ' Settings control event handlers:
        Private Sub SkipRegexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
            With OurSettingsControl
                If .SkipRegexYN = False OrElse .SkipRegex = "" Then
                    SkipRegex = Nothing
                Else
                    SkipRegex = New Regex(.SkipRegex, RegexOptions.IgnoreCase Or RegexOptions.Compiled)
                End If
            End With
        End Sub
        Private Sub PropertiesButtonClick(ByVal sender As Object, ByVal e As EventArgs)
            Dim frm As New GenericTemplatePropertiesForm

            If IAmReady Then
                frm.AmIReadyLabel.Text = "Generic Template Plugin is ready"
            Else
                frm.AmIReadyLabel.Text = "Generic Template Plugin is not properly configured"
            End If

            GenericTemplatePropertiesForm.DoRegexTextBox(frm.MainRegexTextBox, MainRegex)
            GenericTemplatePropertiesForm.DoRegexTextBox( _
               frm.PreferredTemplateNameRegexTextBox, PreferredTemplateNameRegex)
            GenericTemplatePropertiesForm.DoRegexTextBox(frm.SecondChanceRegexTextBox, SecondChanceRegex)
            GenericTemplatePropertiesForm.DoRegexTextBox(frm.SkipRegexTextBox, SkipRegex)

            frm.HasAltNamesLabel.Text += HasAlternateNames.ToString

            With OurSettingsControl
                frm.NameLabel.Text += .TemplateName

                If .SkipRegexYN Then
                    If .SkipRegex = "" Then
                        frm.SkipLabel.Text += Boolean.FalseString
                    Else
                        frm.SkipLabel.Text += Boolean.TrueString
                    End If
                Else
                    frm.SkipLabel.Text += Boolean.FalseString
                End If

                Select Case .ImportanceSetting
                    Case GenericTemplateSettings.ImportanceSettingEnum.Imp
                        frm.ImportanceLabel.Text += "importance="
                    Case GenericTemplateSettings.ImportanceSettingEnum.None
                        frm.ImportanceLabel.Text += "<none>"
                    Case GenericTemplateSettings.ImportanceSettingEnum.Pri
                        frm.ImportanceLabel.Text += "priority="
                End Select

                If .AutoStubYN Then
                    frm.AutoStubLabel.Text += "auto=yes"
                Else
                    frm.AutoStubLabel.Text += "<none>"
                End If

                If .HasTemplateClass Then
                    frm.TemplatesLabel.Text += "Template"
                Else
                    frm.TemplatesLabel.Text += "NA"
                End If

                If .HasCategoryClass Then
                    frm.CatsLabel.Text += "Cat"
                Else
                    frm.CatsLabel.Text += "NA"
                End If
            End With

            frm.ShowDialog()
            frm = Nothing
        End Sub
        Private Sub DeleteMeMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles DeleteMeMenuItem.Click
            If MessageBox.Show("Delete the " & OurName & " plugin?", "Delete?", MessageBoxButtons.YesNo, _
            MessageBoxIcon.Question) = DialogResult.Yes Then
                PluginManager.DeleteGenericPlugin(Me, Me)
            End If
        End Sub
        Private Sub GetRedirectsButtonClick(ByVal sender As System.Object, ByVal e As System.EventArgs)
            If MessageBox.Show("Get the redirects from Wikipedia? Note: This may take a while.", "Get from Wikipedia?", _
               MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) = DialogResult.Yes Then
                Try
                    OurSettingsControl.AlternateNames = ConvertRedirectsToString(GetRedirects(OurSettingsControl.TemplateName))
                    OurSettingsControl.HasAlternateNamesCheckBox.Checked = Not (OurSettingsControl.AlternateNames = "")
                Catch ex As Exception
                    MessageBox.Show("Whoops, we caught an error when trying to get the redirects from Wikipedia." & _
                       Microsoft.VisualBasic.vbCrLf & Microsoft.VisualBasic.vbCrLf & "The error was:" & ex.Message & _
                       Microsoft.VisualBasic.vbCrLf & Microsoft.VisualBasic.vbCrLf & "Depending on the error you might want to " & _
                       "try again by repressing Get. If this shouldn't have happened please report it to the authors.")
                End Try
            End If
        End Sub

        ' Misc:
        Protected Overrides ReadOnly Property InspectUnsetParameters() As Boolean
            Get
                Return False
            End Get
        End Property
        Protected Overrides Sub InspectUnsetParameter(ByVal Param As String)
        End Sub ' will never be called
        Friend Overrides ReadOnly Property HasSharedLogLocation() As Boolean
            Get
                Return False
            End Get
        End Property
        Friend Overrides ReadOnly Property SharedLogLocation() As String
            Get
                Return ""
            End Get
        End Property
        Friend Overrides ReadOnly Property HasReqPhotoParam() As Boolean
            Get
                Return False
            End Get
        End Property
        Friend Overrides Sub ReqPhoto()
        End Sub

#Region "IDisposable"
        Private disposed As Boolean ' To detect redundant calls

        ' This procedure is where the actual cleanup occurs
        Private Sub Dispose(ByVal disposing As Boolean)
            On Error Resume Next

            ' Exit now if the object has already been disposed
            If disposed Then Exit Sub

            If disposing Then
                ' The object is being disposed, not finalized.
                ' It is safe to access other objects (other than the mybase object)
                ' only from inside this block
                PluginManager.AWBForm.TraceManager.WriteBulletedLine("Generic template """ & OurName & _
                   """ finalized.", True, True, True)
                RemoveHandler OurSettingsControl.SkipRegexCheckBox.CheckedChanged, AddressOf Me.SkipRegexChanged
                RemoveHandler OurSettingsControl.SkipRegexTextBox.TextChanged, AddressOf Me.SkipRegexChanged
                RemoveHandler OurSettingsControl.TemplateNameTextBox.TextChanged, AddressOf Me.TemplateNamesChanged
                RemoveHandler OurSettingsControl.HasAlternateNamesCheckBox.CheckedChanged, AddressOf Me.TemplateNamesChanged
                RemoveHandler OurSettingsControl.AlternateNamesTextBox.TextChanged, AddressOf Me.TemplateNamesChanged
                RemoveHandler OurSettingsControl.PropertiesButton.Click, AddressOf Me.PropertiesButtonClick
                ShowHideOurObjects(False)

                OurTab.Dispose()

                OurSettingsControl.Goodbye()
                OurSettingsControl.Dispose()

                PluginManager.AWBForm.PluginsToolStripMenuItem.DropDownItems.Remove(OurMenuItem)
            End If

            ' Perform cleanup that has to be executed in either case:
            OurTab = Nothing
            OurMenuItem = Nothing
            Article = Nothing
            Template = Nothing
            MainRegex = Nothing
            SecondChanceRegex = Nothing
            PreferredTemplateNameRegex = Nothing
            OurTab = Nothing
            OurSettingsControl = Nothing
            DeleteMeMenuItem = Nothing
            SkipRegex = Nothing

            ' Remember that this object has been disposed of:
            Me.disposed = True
        End Sub
        Friend Sub Dispose() Implements IDisposable.Dispose, IGenericTemplatePlugin.Goodbye
            Debug.WriteLine("Disposing of generic plugin " & OurName)
            ' Execute the code that does the cleanup.
            Dispose(True)
            ' Let the CLR know that Finalize doesn't have to be called.
            GC.SuppressFinalize(Me)
        End Sub

        Protected Overrides Sub Finalize()
            MyBase.Finalize()
            Debug.WriteLine("Finalizing generic plugin " & OurName)
            ' Execute the code that does the cleanup.
            Dispose(False)
        End Sub
#End Region

    End Class
End Namespace

Namespace AutoWikiBrowser.Plugins.Kingbotk
    Friend Interface IGenericTemplatePlugin
        Sub Goodbye()
        ReadOnly Property GenericTemplateKey() As String
    End Interface
End Namespace