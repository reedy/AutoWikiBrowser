Namespace AutoWikiBrowser.Plugins.SDKSoftware.Kingbotk.Plugins
    Friend NotInheritable Class IndiaSettings
        Implements IGenericSettings

        Private Const conMapsParm As String = "IndMaps"
        Private Const conCitiesParm As String = "IndCities"
        Private Const conDistrictsParm As String = "IndDistricts"
        Private Const conStatesParm As String = "IndStates"
        Private Const conAndhraParm As String = "IndAndhra"
        Private Const conBengalParm As String = "IndBengal"
        Private Const conGoaParm As String = "IndGoa"
        Private Const conKarnatakaParm As String = "IndKarnataka"
        Private Const conKeralaParm As String = "IndKerala"
        Private Const conMaharashtraParm As String = "IndMaharashtra"
        Private Const conTamilnaduParm As String = "IndTamilnadu"
        Private Const conPoliticsParm As String = "IndPolitics"
        Private Const conHistoryParm As String = "IndHistory"
        Private Const conCinemaParm As String = "IndCinema"
        Private Const conTamilParm As String = "IndTamil"

        ' UI:
        Private txtEdit As TextBox

#Region "XML interface:"
        Friend Sub ReadXML(ByVal Reader As System.Xml.XmlTextReader) Implements IGenericSettings.ReadXML
            Maps = PluginManager.XMLReadBoolean(Reader, conMapsParm, Maps)
            Cities = PluginManager.XMLReadBoolean(Reader, conCitiesParm, Cities)
            Districts = PluginManager.XMLReadBoolean(Reader, conDistrictsParm, Districts)
            States = PluginManager.XMLReadBoolean(Reader, conStatesParm, States)
            Andhra = PluginManager.XMLReadBoolean(Reader, conAndhraParm, Andhra)
            Bengal = PluginManager.XMLReadBoolean(Reader, conBengalParm, Bengal)
            Goa = PluginManager.XMLReadBoolean(Reader, conGoaParm, Goa)
            Karnataka = PluginManager.XMLReadBoolean(Reader, conKarnatakaParm, Karnataka)
            Kerala = PluginManager.XMLReadBoolean(Reader, conKeralaParm, Kerala)
            Maharashtra = PluginManager.XMLReadBoolean(Reader, conMaharashtraParm, Maharashtra)
            Tamilnadu = PluginManager.XMLReadBoolean(Reader, conTamilnaduParm, Tamilnadu)
            Politics = PluginManager.XMLReadBoolean(Reader, conPoliticsParm, Politics)
            History = PluginManager.XMLReadBoolean(Reader, conHistoryParm, History)
            Cinema = PluginManager.XMLReadBoolean(Reader, conCinemaParm, Cinema)
            Tamil = PluginManager.XMLReadBoolean(Reader, conTamilParm, Tamil)
        End Sub
        Friend Sub WriteXML(ByVal Writer As System.Xml.XmlTextWriter) Implements IGenericSettings.WriteXML
            With Writer
                .WriteAttributeString(conMapsParm, Maps.ToString)
                .WriteAttributeString(conCitiesParm, Cities.ToString)
                .WriteAttributeString(conDistrictsParm, States.ToString)
                .WriteAttributeString(conStatesParm, States.ToString)
                .WriteAttributeString(conAndhraParm, Andhra.ToString)
                .WriteAttributeString(conBengalParm, Bengal.ToString)
                .WriteAttributeString(conGoaParm, Goa.ToString)
                .WriteAttributeString(conKarnatakaParm, Karnataka.ToString)
                .WriteAttributeString(conKeralaParm, Kerala.ToString)
                .WriteAttributeString(conMaharashtraParm, Maharashtra.ToString)
                .WriteAttributeString(conTamilnaduParm, Tamilnadu.ToString)
                .WriteAttributeString(conPoliticsParm, Politics.ToString)
                .WriteAttributeString(conHistoryParm, History.ToString)
                .WriteAttributeString(conCinemaParm, Cinema.ToString)
                .WriteAttributeString(conTamilParm, Tamil.ToString)
            End With
        End Sub
        Friend Sub Reset() Implements IGenericSettings.XMLReset
            AutoStub = False
            StubClass = False

            For Each ctl As Control In Me.TopicsGroupBox.Controls
                If TypeOf ctl Is CheckBox Then DirectCast(ctl, CheckBox).Checked = False
            Next
        End Sub
#End Region

        ' Properties:
        Friend Property Maps() As Boolean
            Get
                Return MapsCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                MapsCheckBox.Checked = value
            End Set
        End Property
        Friend Property Cities() As Boolean
            Get
                Return CitiesCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                CitiesCheckBox.Checked = value
            End Set
        End Property
        Friend Property Districts() As Boolean
            Get
                Return DistrictsCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                DistrictsCheckBox.Checked = value
            End Set
        End Property
        Friend Property States() As Boolean
            Get
                Return StatesCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                StatesCheckBox.Checked = value
            End Set
        End Property
        Friend Property Andhra() As Boolean
            Get
                Return AndhraCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                AndhraCheckBox.Checked = value
            End Set
        End Property
        Friend Property Bengal() As Boolean
            Get
                Return BengalCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                BengalCheckBox.Checked = value
            End Set
        End Property
        Friend Property Goa() As Boolean
            Get
                Return GoaCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                GoaCheckBox.Checked = value
            End Set
        End Property
        Friend Property Karnataka() As Boolean
            Get
                Return KarnatakaCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                KarnatakaCheckBox.Checked = value
            End Set
        End Property
        Friend Property Kerala() As Boolean
            Get
                Return KeralaCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                KeralaCheckBox.Checked = value
            End Set
        End Property
        Friend Property Maharashtra() As Boolean
            Get
                Return MaharashtraCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                MaharashtraCheckBox.Checked = value
            End Set
        End Property
        Friend Property Tamilnadu() As Boolean
            Get
                Return TamilnaduCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                TamilnaduCheckBox.Checked = value
            End Set
        End Property
        Friend Property Politics() As Boolean
            Get
                Return PoliticsCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                PoliticsCheckBox.Checked = value
            End Set
        End Property
        Friend Property History() As Boolean
            Get
                Return HistoryCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                HistoryCheckBox.Checked = value
            End Set
        End Property
        Friend Property Cinema() As Boolean
            Get
                Return CinemaCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                CinemaCheckBox.Checked = value
            End Set
        End Property
        Friend Property Tamil() As Boolean
            Get
                Return TamilCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                TamilCheckBox.Checked = value
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
        Friend Property AutoStub() As Boolean Implements IGenericSettings.AutoStub
            Get
                Return AutoStubCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                AutoStubCheckBox.Checked = value
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
            System.Diagnostics.Process.Start("http://en.wikipedia.org/wiki/Template:WP_India")
        End Sub
        Private Sub AutoStubCheckBox_CheckedChanged(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles AutoStubCheckBox.CheckedChanged
            If AutoStubCheckBox.Checked Then StubClassCheckBox.Checked = False
        End Sub
        Private Sub StubClassCheckBox_CheckedChanged(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles StubClassCheckBox.CheckedChanged
            If StubClassCheckBox.Checked Then AutoStubCheckBox.Checked = False
        End Sub
    End Class

    Friend NotInheritable Class WPIndia
        Inherits PluginBase

        ' Settings:
        Private OurTab As New TabPage("India")
        Private WithEvents OurSettingsControl As New IndiaSettings
        Private Const conEnabled As String = "IndEnabled"

        Protected Friend Overrides ReadOnly Property conPluginShortName() As String
            Get
                Return "India"
            End Get
        End Property
        Protected Overrides ReadOnly Property ParameterBreak() As String
            Get
                Return Microsoft.VisualBasic.vbCrLf
            End Get
        End Property
        Protected Overrides Sub ImportanceParameter(ByVal Importance As Importance)
            Template.NewOrReplaceTemplateParm("importance", Importance.ToString, Me.Article, False, False)
        End Sub
        Protected Overrides ReadOnly Property OurTemplateHasAlternateNames() As Boolean
            Get
                Return False
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

        Protected Overrides ReadOnly Property PreferredTemplateNameWiki() As String
            Get
                Return "WP India"
            End Get
        End Property

        ' Initialisation:
        Protected Friend Sub New(ByVal Manager As PluginManager)
            MyBase.New(Manager)
            Const RegexpMiddle As String = "WP India"
            MainRegex = New Regex(conRegexpLeft & RegexpMiddle & conRegexpRight, conRegexpOptions)
            SecondChanceRegex = New Regex(conRegexpLeft & RegexpMiddle & conRegexpRightNotStrict, conRegexpOptions)
        End Sub
        Protected Friend Overrides Sub Initialise(ByVal AWBPluginsMenu As ToolStripMenuItem, ByVal txt As TextBox)
            OurMenuItem = New ToolStripMenuItem("India Plugin")
            MyBase.InitialiseBase(AWBPluginsMenu, txt) ' must set menu item object first
            OurTab.UseVisualStyleBackColor = True
            OurTab.Controls.Add(OurSettingsControl)
        End Sub

        ' Article processing:
        Protected Overrides ReadOnly Property InspectUnsetParameters() As Boolean
            Get
                Return False
            End Get
        End Property
        Protected Overrides Sub InspectUnsetParameter(ByVal Param As String)
        End Sub
        Protected Overrides Function SkipIfContains() As Boolean
            ' None
        End Function
        Protected Overrides Sub ProcessArticleFinish()
            StubClass()
            With OurSettingsControl
                If .Maps Then AddAndLogNewParamWithAYesValue("Maps")
                If .Cities Then AddAndLogNewParamWithAYesValue("Cities")
                If .Districts Then AddAndLogNewParamWithAYesValue("Districts")
                If .States Then AddAndLogNewParamWithAYesValue("States")
                If .Andhra Then AddAndLogNewParamWithAYesValue("Andhra")
                If .Bengal Then AddAndLogNewParamWithAYesValue("Bengal")
                If .Goa Then AddAndLogNewParamWithAYesValue("Goa")
                If .Karnataka Then AddAndLogNewParamWithAYesValue("Karnataka")
                If .Kerala Then AddAndLogNewParamWithAYesValue("Kerala")
                If .Maharashtra Then AddAndLogNewParamWithAYesValue("Maharashtra")
                If .Tamilnadu Then AddAndLogNewParamWithAYesValue("Tamilnadu")
                If .Politics Then AddAndLogNewParamWithAYesValue("politics")
                If .History Then AddAndLogNewParamWithAYesValue("History")
                If .Cinema Then AddAndLogNewParamWithAYesValue("cinema")
                If .Tamil Then AddAndLogNewParamWithAYesValue("Tamil")
            End With
        End Sub
        Protected Overrides Function TemplateFound() As Boolean
            If CheckForDoublyNamedParameters("V8", "v8") Then Return True ' tag is bad, exit
            If CheckForDoublyNamedParameters("nbl", "NBL") Then Return True
        End Function
        Protected Overrides Sub GotTemplateNotPreferredName(ByVal TemplateName As String)
        End Sub
        Protected Overrides Function WriteTemplateHeader(ByRef PutTemplateAtTop As Boolean) As String
            WriteTemplateHeader = "{{WP India" & _
               Microsoft.VisualBasic.vbCrLf & WriteOutParameterToHeader("class") & _
               WriteOutParameterToHeader("importance")
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

        ' Not implemented:
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
    End Class
End Namespace