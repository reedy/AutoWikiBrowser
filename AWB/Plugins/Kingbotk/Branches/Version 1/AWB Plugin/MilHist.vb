Namespace AutoWikiBrowser.Plugins.SDKSoftware.Kingbotk.Plugins
    Friend NotInheritable Class MilHistSettings
        Implements IGenericSettings

        Private Const conWWIIWGParm As String = "MilHistWWII"
        Private Const conWWIWGParm As String = "MilHistWWI"
        Private Const conWeaponryWGParm As String = "MilHistWeapon"
        Private Const conUSWGParm As String = "MilHistUS"
        Private Const conPolishWGParm As String = "MilHistPoland"
        Private Const conNapoleonicWGParm As String = "MilHistNapol"
        Private Const conMedievalWGParm As String = "MilHistMedieval"
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

        Private Const conHistoriographyWGParm As String = "MilHistHistoriography"
        Private Const conScienceWGParm As String = "MilHistScience"
        Private Const conTechnologyWGParm As String = "MilHistTech"
        Private Const conAfricanWGParm As String = "MilHistAfrican"
        Private Const conBalkanWGParm As String = "MilHistBalkan"
        Private Const conKoreanWGParm As String = "MilHistKorean"
        Private Const conNewZealandWGParm As String = "MilHistNZ"
        Private Const conNordicWGParm As String = "MilHistNordic"
        Private Const conOttomanWGParm As String = "MilHistOttoman"
        Private Const conRussianWGParm As String = "MilHistRussian"
        Private Const conSpanishWGParm As String = "MilHistSpanish"

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
            Medieval = PluginManager.XMLReadBoolean(Reader, conMedievalWGParm, Medieval)
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

            Historiography = PluginManager.XMLReadBoolean(Reader, conHistoriographyWGParm, Historiography)
            Science = PluginManager.XMLReadBoolean(Reader, conScienceWGParm, Science)
            Technology = PluginManager.XMLReadBoolean(Reader, conTechnologyWGParm, Technology)
            African = PluginManager.XMLReadBoolean(Reader, conAfricanWGParm, African)
            Balkan = PluginManager.XMLReadBoolean(Reader, conBalkanWGParm, Balkan)
            Korean = PluginManager.XMLReadBoolean(Reader, conKoreanWGParm, Korean)
            NewZealand = PluginManager.XMLReadBoolean(Reader, conNewZealandWGParm, NewZealand)
            Nordic = PluginManager.XMLReadBoolean(Reader, conNordicWGParm, Nordic)
            Ottoman = PluginManager.XMLReadBoolean(Reader, conOttomanWGParm, Ottoman)
            Russian = PluginManager.XMLReadBoolean(Reader, conRussianWGParm, Russian)
            Spanish = PluginManager.XMLReadBoolean(Reader, conSpanishWGParm, Spanish)
        End Sub
        Public Sub WriteXML(ByVal Writer As System.Xml.XmlTextWriter) Implements IGenericSettings.WriteXML
            With Writer
                .WriteAttributeString(conWWIIWGParm, WWII.ToString)
                .WriteAttributeString(conWWIWGParm, WWI.ToString)
                .WriteAttributeString(conWeaponryWGParm, Weaponry.ToString)
                .WriteAttributeString(conUSWGParm, US.ToString)
                .WriteAttributeString(conPolishWGParm, Polish.ToString)
                .WriteAttributeString(conNapoleonicWGParm, Napoleonic.ToString)
                .WriteAttributeString(conMedievalWGParm, Medieval.ToString)
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

                .WriteAttributeString(conHistoriographyWGParm, Historiography.ToString)
                .WriteAttributeString(conScienceWGParm, Science.ToString)
                .WriteAttributeString(conTechnologyWGParm, Technology.ToString)
                .WriteAttributeString(conAfricanWGParm, African.ToString)
                .WriteAttributeString(conBalkanWGParm, Balkan.ToString)
                .WriteAttributeString(conKoreanWGParm, Korean.ToString)
                .WriteAttributeString(conNewZealandWGParm, NewZealand.ToString)
                .WriteAttributeString(conNordicWGParm, Nordic.ToString)
                .WriteAttributeString(conOttomanWGParm, Ottoman.ToString)
                .WriteAttributeString(conRussianWGParm, Russian.ToString)
                .WriteAttributeString(conSpanishWGParm, Spanish.ToString)
            End With
        End Sub
        Public Sub Reset() Implements IGenericSettings.XMLReset
            StubClass = False
            AutoStub = False
            ForceImportanceRemoval = False

            For Each ctl As Control In Me.TabControl1.Controls
                If TypeOf ctl Is CheckBox Then DirectCast(ctl, CheckBox).Checked = False
            Next
        End Sub
#End Region

        ' TODO: Replace old param with new or at least ensure we don't add both. Should be able to use overrides or existing code to achieve this very easily.
        '    Does the parameter for the middle ages taskforce need to be converted to medieval taskforce? Are they one and the same under different names? --kingboyk 18:06, 7 April 2007 (UTC)

        'They're functionally equivalent. The medieval parameter is probably the one that should be used in the future, as it's the documented one, but the older one will still work as expected. Kirill Lokshin 21:02, 7 April 2007 (UTC) 

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
        Friend Property Medieval() As Boolean
            Get
                Return MedievalCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                MedievalCheckBox.Checked = value
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
        Friend Property Historiography() As Boolean
            Get
                Return HistoriographyCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                HistoriographyCheckBox.Checked = value
            End Set
        End Property
        Friend Property Science() As Boolean
            Get
                Return ScienceCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                ScienceCheckBox.Checked = value
            End Set
        End Property
        Friend Property Technology() As Boolean
            Get
                Return TechnologyCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                TechnologyCheckBox.Checked = value
            End Set
        End Property
        Friend Property African() As Boolean
            Get
                Return AfricanCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                AfricanCheckBox.Checked = value
            End Set
        End Property
        Friend Property Balkan() As Boolean
            Get
                Return BalkanCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                BalkanCheckBox.Checked = value
            End Set
        End Property
        Friend Property Korean() As Boolean
            Get
                Return KoreanCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                KoreanCheckBox.Checked = value
            End Set
        End Property
        Friend Property NewZealand() As Boolean
            Get
                Return NewZealandCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                NewZealandCheckBox.Checked = value
            End Set
        End Property
        Friend Property Nordic() As Boolean
            Get
                Return NordicCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                NordicCheckBox.Checked = value
            End Set
        End Property
        Friend Property Ottoman() As Boolean
            Get
                Return OttomanCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                OttomanCheckBox.Checked = value
            End Set
        End Property
        Friend Property Russian() As Boolean
            Get
                Return RussianCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                RussianCheckBox.Checked = value
            End Set
        End Property
        Friend Property Spanish() As Boolean
            Get
                Return SpanishCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                SpanishCheckBox.Checked = value
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
            Tools.OpenENArticleInBrowser("Template:WPMILHIST", False)
        End Sub
        Private Sub AutoStubCheckBox_CheckedChanged(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles AutoStubCheckBox.CheckedChanged
            If AutoStubCheckBox.Checked Then StubClassCheckBox.Checked = False
        End Sub
        Private Sub StubClassCheckBox_CheckedChanged(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles StubClassCheckBox.CheckedChanged
            If StubClassCheckBox.Checked Then AutoStubCheckBox.Checked = False
        End Sub

        Private Sub WPMILHISTToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles WPMILHISTToolStripMenuItem.Click
            txtEdit.SelectedText = "{{WPMILHIST}}"
        End Sub

        Private Sub AviationToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AviationToolStripMenuItem.Click
            txtEdit.SelectedText = "|Aviation-task-force=yes"
        End Sub

        Private Sub HistoriographyToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles HistoriographyToolStripMenuItem.Click
            txtEdit.SelectedText = "|Historiography-task-force=yes"
        End Sub

        Private Sub MaritimeToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MaritimeToolStripMenuItem.Click
            txtEdit.SelectedText = "|Maritime-task-force=yes"
        End Sub

        Private Sub MemorialsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MemorialsToolStripMenuItem.Click
            txtEdit.SelectedText = "|Memorials-task-force=yes"
        End Sub

        Private Sub ScienceToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ScienceToolStripMenuItem.Click
            txtEdit.SelectedText = "|Science-task-force=yes"
        End Sub

        Private Sub TechnologyToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TechnologyToolStripMenuItem.Click
            txtEdit.SelectedText = "|Technology-task-force=yes"
        End Sub

        Private Sub WeaponryToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles WeaponryToolStripMenuItem.Click
            txtEdit.SelectedText = "|Weaponry-task-force=yes"
        End Sub

        Private Sub AfricanToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AfricanToolStripMenuItem.Click
            txtEdit.SelectedText = "|African-task-force=yes"
        End Sub

        Private Sub AustralianToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AustralianToolStripMenuItem.Click
            txtEdit.SelectedText = "|Australian-task-force=yes"
        End Sub

        Private Sub BalkanToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BalkanToolStripMenuItem.Click
            txtEdit.SelectedText = "|Balkan-task-force=yes"
        End Sub

        Private Sub BritishToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BritishToolStripMenuItem.Click
            txtEdit.SelectedText = "|British-task-force=yes"
        End Sub

        Private Sub CanadianToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CanadianToolStripMenuItem.Click
            txtEdit.SelectedText = "|Canadian-task-force=yes"
        End Sub

        Private Sub ChineseToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ChineseToolStripMenuItem.Click
            txtEdit.SelectedText = "|Chinese-task-force=yes"
        End Sub

        Private Sub DutchToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DutchToolStripMenuItem.Click
            txtEdit.SelectedText = "|Dutch-task-force=yes"
        End Sub

        Private Sub GermanToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GermanToolStripMenuItem.Click
            txtEdit.SelectedText = "|-task-force=yes"
        End Sub

        Private Sub FrenchToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FrenchToolStripMenuItem.Click
            txtEdit.SelectedText = "|French-task-force=yes"
        End Sub

        Private Sub IndianToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles IndianToolStripMenuItem.Click
            txtEdit.SelectedText = "|Indian-task-force=yes"
        End Sub

        Private Sub ItalianToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ItalianToolStripMenuItem.Click
            txtEdit.SelectedText = "|Italian-task-force=yes"
        End Sub

        Private Sub JapaneseToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles JapaneseToolStripMenuItem.Click
            txtEdit.SelectedText = "|Japanese-task-force=yes"
        End Sub

        Private Sub KoreanToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles KoreanToolStripMenuItem.Click
            txtEdit.SelectedText = "|Korean-task-force=yes"
        End Sub

        Private Sub NewZealandToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NewZealandToolStripMenuItem.Click
            txtEdit.SelectedText = "|NewZealand-task-force=yes"
        End Sub

        Private Sub NordicToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NordicToolStripMenuItem.Click
            txtEdit.SelectedText = "|Nordic-task-force=yes"
        End Sub

        Private Sub OttomanToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OttomanToolStripMenuItem.Click
            txtEdit.SelectedText = "|Ottoman-task-force=yes"
        End Sub

        Private Sub PolishToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PolishToolStripMenuItem.Click
            txtEdit.SelectedText = "|Polish-task-force=yes"
        End Sub

        Private Sub RussianToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RussianToolStripMenuItem.Click
            txtEdit.SelectedText = "|Russian-task-force=yes"
        End Sub

        Private Sub SpanishToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SpanishToolStripMenuItem.Click
            txtEdit.SelectedText = "|Spanish-task-force=yes"
        End Sub

        Private Sub USToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles USToolStripMenuItem.Click
            txtEdit.SelectedText = "|US-task-force=yes"
        End Sub

        Private Sub ACWToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ACWToolStripMenuItem.Click
            txtEdit.SelectedText = "|ACW-task-force=yes"
        End Sub

        Private Sub AncientNearEasterToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AncientNearEasterToolStripMenuItem.Click
            txtEdit.SelectedText = "|Ancient-task-force=yes"
        End Sub

        Private Sub ClassicalToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ClassicalToolStripMenuItem.Click
            txtEdit.SelectedText = "|Classical-task-force=yes"
        End Sub

        Private Sub MedievalToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MedievalToolStripMenuItem.Click
            txtEdit.SelectedText = "|Medieval-task-force=yes"
        End Sub

        Private Sub EarlyModernToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles EarlyModernToolStripMenuItem.Click
            txtEdit.SelectedText = "|Early-Modern-task-force=yes"
        End Sub

        Private Sub NapoleonicToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NapoleonicToolStripMenuItem.Click
            txtEdit.SelectedText = "|Napoleonic-task-force=yes"
        End Sub

        Private Sub WWIToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles WWIToolStripMenuItem.Click
            txtEdit.SelectedText = "|WWI-task-force=yes"
        End Sub

        Private Sub WWIIToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles WWIIToolStripMenuItem.Click
            txtEdit.SelectedText = "|WWII-task-force=yes"
        End Sub
    End Class

    Friend NotInheritable Class WPMilHist
        Inherits PluginBase

        ' Settings:
        Private OurTab As New TabPage("MilHist")
        Private WithEvents OurSettingsControl As New MilHistSettings
        Private Const conEnabled As String = "MilHistEnabled"
        Private Const conMedievalTaskForce As String = "Medieval-task-force"

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
                If .Medieval Then AddAndLogNewParamWithAYesValue("Medieval-task-force")
                If .Napoleonic Then AddAndLogNewParamWithAYesValue("Napoleonic-task-force")
                If .Polish Then AddAndLogNewParamWithAYesValue("Polish-task-force")
                If .US Then AddAndLogNewParamWithAYesValue("US-task-force")
                If .Weaponry Then AddAndLogNewParamWithAYesValue("Weaponry-task-force")
                If .WWI Then AddAndLogNewParamWithAYesValue("WWI-task-force")
                If .WWII Then AddAndLogNewParamWithAYesValue("WWII-task-force")

                If .Historiography Then AddAndLogNewParamWithAYesValue("Historiography-task-force")
                If .Science Then AddAndLogNewParamWithAYesValue("Science-task-force")
                If .Technology Then AddAndLogNewParamWithAYesValue("Technology-task-force")
                If .African Then AddAndLogNewParamWithAYesValue("African-task-force")
                If .Balkan Then AddAndLogNewParamWithAYesValue("Balkan-task-force")
                If .Korean Then AddAndLogNewParamWithAYesValue("Korean-task-force")
                If .NewZealand Then AddAndLogNewParamWithAYesValue("New-Zealand-task-force")
                If .Nordic Then AddAndLogNewParamWithAYesValue("Nordic-task-force")
                If .Ottoman Then AddAndLogNewParamWithAYesValue("Ottoman-task-force")
                If .Russian Then AddAndLogNewParamWithAYesValue("Russian-task-force")
                If .Spanish Then AddAndLogNewParamWithAYesValue("Spanish-task-force")
            End With
            If Template.Parameters.ContainsKey("importance") Then
                Template.Parameters.Remove("importance")
                Article.ArticleHasAMajorChange()
                PluginSettingsControl.MyTrace.WriteArticleActionLine("Removed importance parameter", _
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
                        PluginSettingsControl.MyTrace.WriteArticleActionLine( _
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