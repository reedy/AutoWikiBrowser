Namespace AutoWikiBrowser.Plugins.SDKSoftware.Kingbotk.Plugins
    Friend NotInheritable Class MilHistSettings
        Implements IGenericSettings

        Private Const conWWIIWGParm As String = "MilHistWWII"
        Private Const conWWIWGParm As String = "MilHistWWI"
        Private Const conWeaponryWGParm As String = "MilHistWeapon"
        Private Const conUSWGParm As String = "MilHistUS"
        Private Const conPolishWGParm As String = "MilHistPoland"
        Private Const conNapoleonicWGParm As String = "MilHistNapol"
        Private Const conMiddleAgesWGParm As String = "MilHistMidAges"
        Private Const conMemorialsWGParm As String = "MilHistMemorial"
        Private Const conMaritimeWGParm As String = "MilHistMarit"
        Private Const conJapaneseWGParm As String = "MilHistJapan"
        Private Const conItalianWGParm As String = "MilHistItaly"
        Private Const conIndianWGParm As String = "MilHistIndia"
        Private Const conGermanWGParm As String = "MilHistGerman"
        Private Const conFrenchWGParm As String = "MilHistFrench"
        Private Const conDutchWGParm As String = "MilHistDutch"
        Private Const conClassicalWGParm As String = "MilHistClassic"
        Private Const conChineseWGParm As String = "MilHistChina"
        Private Const conCanadianWGParm As String = "MilHistCanuck"
        Private Const conBritishWGParm As String = "MilHistBrit"
        Private Const conAviationWGParm As String = "MilHistAir"
        Private Const conAustralianWGParm As String = "MilHistAus"
        Private Const conAncientNearEastWGParm As String = "MilHistAncNE"
        Private Const conAmericanCivilWarWGParm As String = "MilHistACW"
        Private Const conEarlyModernWGParm As String = "MilHistEarlyModern"
        Private Const conStubClassParm As String = "MilHistStubClass"
        Private Const conAutoStubParm As String = "MilHistAutoStub"
        Private Const conForceImportanceRemoval As String = "MilHistRmImportance"

        ' UI:
        Private txtEdit As TextBox

#Region "XML interface"
        Public Sub ReadXML(ByVal Reader As System.Xml.XmlTextReader) Implements IGenericSettings.ReadXML
            WWII = PluginManager.XMLReadBoolean(Reader, conWWIIWGParm, WWII)
            WWI = PluginManager.XMLReadBoolean(Reader, conWWIWGParm, WWI)
            Weaponry = PluginManager.XMLReadBoolean(Reader, conWeaponryWGParm, Weaponry)
            US = PluginManager.XMLReadBoolean(Reader, conUSWGParm, US)
            Polish = PluginManager.XMLReadBoolean(Reader, conPolishWGParm, Polish)
            Napoleonic = PluginManager.XMLReadBoolean(Reader, conNapoleonicWGParm, Napoleonic)
            MiddleAges = PluginManager.XMLReadBoolean(Reader, conMiddleAgesWGParm, MiddleAges)
            Memorials = PluginManager.XMLReadBoolean(Reader, conMemorialsWGParm, Memorials)
            Maritime = PluginManager.XMLReadBoolean(Reader, conMaritimeWGParm, Maritime)
            Japanese = PluginManager.XMLReadBoolean(Reader, conJapaneseWGParm, Japanese)
            Italian = PluginManager.XMLReadBoolean(Reader, conItalianWGParm, Italian)
            Indian = PluginManager.XMLReadBoolean(Reader, conIndianWGParm, Indian)
            German = PluginManager.XMLReadBoolean(Reader, conGermanWGParm, German)
            French = PluginManager.XMLReadBoolean(Reader, conFrenchWGParm, French)
            Dutch = PluginManager.XMLReadBoolean(Reader, conDutchWGParm, Dutch)
            Classical = PluginManager.XMLReadBoolean(Reader, conClassicalWGParm, Classical)
            Chinese = PluginManager.XMLReadBoolean(Reader, conChineseWGParm, Chinese)
            Canadian = PluginManager.XMLReadBoolean(Reader, conCanadianWGParm, Canadian)
            British = PluginManager.XMLReadBoolean(Reader, conBritishWGParm, British)
            Aviation = PluginManager.XMLReadBoolean(Reader, conAviationWGParm, Aviation)
            Australian = PluginManager.XMLReadBoolean(Reader, conAustralianWGParm, Australian)
            AncientNearEast = PluginManager.XMLReadBoolean(Reader, conAncientNearEastWGParm, AncientNearEast)
            AmericanCivilWar = PluginManager.XMLReadBoolean(Reader, conAmericanCivilWarWGParm, AmericanCivilWar)
            StubClass = PluginManager.XMLReadBoolean(Reader, conStubClassParm, StubClass)
            AutoStub = PluginManager.XMLReadBoolean(Reader, conAutoStubParm, AutoStub)
            ForceImportanceRemoval = _
               PluginManager.XMLReadBoolean(Reader, conForceImportanceRemoval, ForceImportanceRemoval)
            EarlyModern = PluginManager.XMLReadBoolean(Reader, conEarlyModernWGParm, EarlyModern)
        End Sub
        Public Sub WriteXML(ByVal Writer As System.Xml.XmlTextWriter) Implements IGenericSettings.WriteXML
            With Writer
                .WriteAttributeString(conWWIIWGParm, WWII.ToString)
                .WriteAttributeString(conWWIWGParm, WWI.ToString)
                .WriteAttributeString(conWeaponryWGParm, Weaponry.ToString)
                .WriteAttributeString(conUSWGParm, US.ToString)
                .WriteAttributeString(conPolishWGParm, Polish.ToString)
                .WriteAttributeString(conNapoleonicWGParm, Napoleonic.ToString)
                .WriteAttributeString(conMiddleAgesWGParm, MiddleAges.ToString)
                .WriteAttributeString(conMemorialsWGParm, Memorials.ToString)
                .WriteAttributeString(conMaritimeWGParm, Maritime.ToString)
                .WriteAttributeString(conJapaneseWGParm, Japanese.ToString)
                .WriteAttributeString(conItalianWGParm, Italian.ToString)
                .WriteAttributeString(conIndianWGParm, Indian.ToString)
                .WriteAttributeString(conGermanWGParm, German.ToString)
                .WriteAttributeString(conFrenchWGParm, French.ToString)
                .WriteAttributeString(conDutchWGParm, Dutch.ToString)
                .WriteAttributeString(conClassicalWGParm, Classical.ToString)
                .WriteAttributeString(conChineseWGParm, Chinese.ToString)
                .WriteAttributeString(conCanadianWGParm, Canadian.ToString)
                .WriteAttributeString(conBritishWGParm, British.ToString)
                .WriteAttributeString(conAviationWGParm, Aviation.ToString)
                .WriteAttributeString(conAustralianWGParm, Australian.ToString)
                .WriteAttributeString(conAncientNearEastWGParm, AncientNearEast.ToString)
                .WriteAttributeString(conAmericanCivilWarWGParm, AmericanCivilWar.ToString)
                .WriteAttributeString(conStubClassParm, StubClass.ToString)
                .WriteAttributeString(conAutoStubParm, AutoStub.ToString)
                .WriteAttributeString(conForceImportanceRemoval, ForceImportanceRemoval.ToString)
                .WriteAttributeString(conEarlyModernWGParm, EarlyModern.ToString)
            End With
        End Sub
        Public Sub Reset() Implements IGenericSettings.XMLReset
            StubClass = False
            AutoStub = False
            ForceImportanceRemoval = False

            For Each ctl As Control In Me.TaskForcesGroupBox.Controls
                If TypeOf ctl Is CheckBox Then DirectCast(ctl, CheckBox).Checked = False
            Next
        End Sub
#End Region

        ' Properties:
        Friend Property WWII() As Boolean
            Get
                Return WWIICheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                WWIICheckBox.Checked = value
            End Set
        End Property
        Friend Property WWI() As Boolean
            Get
                Return WWICheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                WWICheckBox.Checked = value
            End Set
        End Property
        Friend Property Weaponry() As Boolean
            Get
                Return WeaponryCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                WeaponryCheckBox.Checked = value
            End Set
        End Property
        Friend Property US() As Boolean
            Get
                Return USCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                USCheckBox.Checked = value
            End Set
        End Property
        Friend Property Polish() As Boolean
            Get
                Return PolishCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                PolishCheckBox.Checked = value
            End Set
        End Property
        Friend Property Napoleonic() As Boolean
            Get
                Return NapoleonicCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                NapoleonicCheckBox.Checked = value
            End Set
        End Property
        Friend Property MiddleAges() As Boolean
            Get
                Return MiddleAgesCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                MiddleAgesCheckBox.Checked = value
            End Set
        End Property
        Friend Property Memorials() As Boolean
            Get
                Return MemorialsCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                MemorialsCheckBox.Checked = value
            End Set
        End Property
        Friend Property Maritime() As Boolean
            Get
                Return MaritimeCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                MaritimeCheckBox.Checked = value
            End Set
        End Property
        Friend Property Japanese() As Boolean
            Get
                Return JapaneseCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                JapaneseCheckBox.Checked = value
            End Set
        End Property
        Friend Property Italian() As Boolean
            Get
                Return ItalianCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                ItalianCheckBox.Checked = value
            End Set
        End Property
        Friend Property Indian() As Boolean
            Get
                Return IndianCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                IndianCheckBox.Checked = value
            End Set
        End Property
        Friend Property German() As Boolean
            Get
                Return GermanCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                GermanCheckBox.Checked = value
            End Set
        End Property
        Friend Property French() As Boolean
            Get
                Return FrenchCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                FrenchCheckBox.Checked = value
            End Set
        End Property
        Friend Property Dutch() As Boolean
            Get
                Return DutchCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                DutchCheckBox.Checked = value
            End Set
        End Property
        Friend Property Classical() As Boolean
            Get
                Return ClassicalCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                ClassicalCheckBox.Checked = value
            End Set
        End Property
        Friend Property Chinese() As Boolean
            Get
                Return ChineseCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                ChineseCheckBox.Checked = value
            End Set
        End Property
        Friend Property Canadian() As Boolean
            Get
                Return CanadianCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                CanadianCheckBox.Checked = value
            End Set
        End Property
        Friend Property British() As Boolean
            Get
                Return BritishCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                BritishCheckBox.Checked = value
            End Set
        End Property
        Friend Property Aviation() As Boolean
            Get
                Return AviationCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                AviationCheckBox.Checked = value
            End Set
        End Property
        Friend Property Australian() As Boolean
            Get
                Return AustralianCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                AustralianCheckBox.Checked = value
            End Set
        End Property
        Friend Property AncientNearEast() As Boolean
            Get
                Return AncientNearEastCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                AncientNearEastCheckBox.Checked = value
            End Set
        End Property
        Friend Property AmericanCivilWar() As Boolean
            Get
                Return AmericanCivilWarCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                AmericanCivilWarCheckBox.Checked = value
            End Set
        End Property
        Friend Property EarlyModern() As Boolean
            Get
                Return EarlyModernCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                EarlyModernCheckBox.Checked = value
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
        WriteOnly Property StubClassModeAllowed() As Boolean Implements IGenericSettings.StubClassModeAllowed
            Set(ByVal value As Boolean)
                StubClassCheckBox.Enabled = value
            End Set
        End Property
        Public Property AutoStub() As Boolean Implements IGenericSettings.AutoStub
            Get
                Return AutoStubCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                AutoStubCheckBox.Checked = value
            End Set
        End Property
        Public Property ForceImportanceRemoval() As Boolean
            Get
                Return RemoveImportanceCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                RemoveImportanceCheckBox.Checked = value
            End Set
        End Property
        Friend WriteOnly Property EditTextBox() As TextBox Implements IGenericSettings.EditTextBox
            Set(ByVal value As TextBox)
                txtEdit = value
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
            System.Diagnostics.Process.Start(PluginManager.ENWiki + "Template:WPMILHIST")
        End Sub
        Private Sub AutoStubCheckBox_CheckedChanged(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles AutoStubCheckBox.CheckedChanged
            If AutoStubCheckBox.Checked Then StubClassCheckBox.Checked = False
        End Sub
        Private Sub StubClassCheckBox_CheckedChanged(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles StubClassCheckBox.CheckedChanged
            If StubClassCheckBox.Checked Then AutoStubCheckBox.Checked = False
        End Sub
        Private Sub WPMILHISTToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles WPMILHISTToolStripMenuItem.Click
            txtEdit.SelectedText = "{{WPMILHIST}}"
        End Sub
    End Class

    Friend NotInheritable Class WPMilHist
        Inherits PluginBase

        ' Settings:
        Private OurTab As New TabPage("MilHist")
        Private WithEvents OurSettingsControl As New MilHistSettings
        Private Const conEnabled As String = "MilHistEnabled"

        Protected Friend Overrides ReadOnly Property PluginShortName() As String
            Get
                Return "MilitaryHistory"
            End Get
        End Property
        Protected Overrides ReadOnly Property PreferredTemplateNameWiki() As String
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
        Protected Overrides ReadOnly Property OurTemplateHasAlternateNames() As Boolean
            Get
                Return True
            End Get
        End Property
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

        ' Initialisation:
        Protected Friend Sub New(ByVal Manager As PluginManager)
            MyBase.New(Manager)

            Const RegexpMiddle As String = "WPMILHIST|WikiProject Military History|WikiProject Military history"

            MainRegex = CreateStandardRegex(RegexpMiddle)
            PreferredTemplateNameRegex = New Regex("^[Ww]PMILHIST$", RegexOptions.Compiled)
            SecondChanceRegex = CreateSecondChanceRegex(RegexpMiddle)
        End Sub
        Protected Friend Overrides Sub Initialise(ByVal AWBPluginsMenu As ToolStripMenuItem, ByVal txt As TextBox)
            OurMenuItem = New ToolStripMenuItem("Military History Plugin")
            MyBase.InitialiseBase(AWBPluginsMenu, txt) ' must set menu item object first
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
                If .AmericanCivilWar Then AddAndLogNewParamWithAYesValue("ACW-task-force")
                If .AncientNearEast Then AddAndLogNewParamWithAYesValue("Ancient-Near-East-task-force")
                If .Australian Then AddAndLogNewParamWithAYesValue("Australian-task-force")
                If .Aviation Then AddAndLogNewParamWithAYesValue("Aviation-task-force")
                If .British Then AddAndLogNewParamWithAYesValue("British-task-force")
                If .Canadian Then AddAndLogNewParamWithAYesValue("Canadian-task-force")
                If .Chinese Then AddAndLogNewParamWithAYesValue("Chinese-task-force")
                If .Classical Then AddAndLogNewParamWithAYesValue("Classical-task-force")
                If .Dutch Then AddAndLogNewParamWithAYesValue("Dutch-task-force")
                If .EarlyModern Then AddAndLogNewParamWithAYesValue("Early-Modern-task-force")
                If .French Then AddAndLogNewParamWithAYesValue("French-task-force")
                If .German Then AddAndLogNewParamWithAYesValue("German-task-force")
                If .Indian Then AddAndLogNewParamWithAYesValue("Indian-task-force")
                If .Italian Then AddAndLogNewParamWithAYesValue("Italian-task-force")
                If .Japanese Then AddAndLogNewParamWithAYesValue("Japanese-task-force")
                If .Maritime Then AddAndLogNewParamWithAYesValue("Maritime-task-force")
                If .Memorials Then AddAndLogNewParamWithAYesValue("Memorials-task-force")
                If .MiddleAges Then AddAndLogNewParamWithAYesValue("MiddleAges-task-force")
                If .Napoleonic Then AddAndLogNewParamWithAYesValue("Napoleonic-task-force")
                If .Polish Then AddAndLogNewParamWithAYesValue("Polish-task-force")
                If .US Then AddAndLogNewParamWithAYesValue("US-task-force")
                If .Weaponry Then AddAndLogNewParamWithAYesValue("Weaponry-task-force")
                If .WWI Then AddAndLogNewParamWithAYesValue("WWI-task-force")
                If .WWII Then AddAndLogNewParamWithAYesValue("WWII-task-force")
            End With
            If Template.Parameters.ContainsKey("importance") Then
                Template.Parameters.Remove("importance")
                Article.ArticleHasAMajorChange()
                PluginSettingsControl.MyTrace.WriteArticleActionLine("Removed importance parameter", _
                   PluginShortName)
            End If
        End Sub
        Protected Overrides Function TemplateFound() As Boolean
            ' Nothing to do here
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
            Manager.ShowHidePluginTab(OurTab, Visible)
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