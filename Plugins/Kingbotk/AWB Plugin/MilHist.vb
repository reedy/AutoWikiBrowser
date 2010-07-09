'Copyright © 2008 Stephen Kennedy (Kingboyk) http://www.sdk-software.com/
'Copyright © 2008 Sam Reed (Reedy) http://www.reedyboy.net/

'This program is free software; you can redistribute it and/or modify it under the terms of Version 2 of the GNU General Public License as published by the Free Software Foundation.

'This program is distributed in the hope that it will be useful,
'but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

'You should have received a copy of the GNU General Public License Version 2 along with this program; if not, write to the Free Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA

Namespace AutoWikiBrowser.Plugins.Kingbotk.Plugins
    Friend NotInheritable Class WPMilHistSettings
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

        Private Const conNoTFWGParm As String = "MilHistNTF"
        Private Const conBalticWGParm As String = "MilHistBaltic"
        Private Const conLebaneseWGParm As String = "MilHistLebanese"
        Private Const conMidEastWGParm As String = "MilHistMidEast"
        Private Const conRomanianWGParm As String = "MilHistRomanian"
        Private Const conSAmericanWGParm As String = "MilHistSAmerican"
        Private Const conSEAsianWGParm As String = "MilHistSEAsian"
        Private Const conTaiwaneseWGParm As String = "MilHistTaiwanese"
        Private Const conBiographyWGParm As String = "MilHistBiography"
        Private Const conFilmsWGParm As String = "MilHistFilms"
        Private Const conFortificationsWGParm As String = "MilHistFortifications"
        Private Const conIntelWGParm As String = "MilHistIntel"
        Private Const conLandVehWGParm As String = "MilHistLandVech"
        Private Const conNationalWGParm As String = "MilHistNational"
        Private Const conMuslimWGParm As String = "MilHistMuslim"
        Private Const conCrusadesWGParm As String = "MilHistCrusades"
        Private Const conARWWGParm As String = "MilHistARW"

#Region "XML interface"
        Friend Sub ReadXML(ByVal Reader As System.Xml.XmlTextReader) Implements IGenericSettings.ReadXML
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
            AmericanCivilWar = PluginManager.XMLReadBoolean(Reader, conAmericanCivilWarWGParm, AmericanCivilWar)
            StubClass = PluginManager.XMLReadBoolean(Reader, conStubClassParm, StubClass)
            AutoStub = PluginManager.XMLReadBoolean(Reader, conAutoStubParm, AutoStub)
            ForceImportanceRemoval = PluginManager.XMLReadBoolean(Reader, conForceImportanceRemoval, ForceImportanceRemoval)
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

            NoTaskForce = PluginManager.XMLReadBoolean(Reader, conNoTFWGParm, NoTaskForce)
            Baltic = PluginManager.XMLReadBoolean(Reader, conBalticWGParm, Baltic)
            Lebanese = PluginManager.XMLReadBoolean(Reader, conLebaneseWGParm, Lebanese)
            MiddleEastern = PluginManager.XMLReadBoolean(Reader, conMidEastWGParm, MiddleEastern)
            Romanian = PluginManager.XMLReadBoolean(Reader, conRomanianWGParm, Romanian)
            SouthAmerican = PluginManager.XMLReadBoolean(Reader, conSAmericanWGParm, SouthAmerican)
            SoutheastAsian = PluginManager.XMLReadBoolean(Reader, conSEAsianWGParm, SoutheastAsian)
            Taiwanese = PluginManager.XMLReadBoolean(Reader, conTaiwaneseWGParm, Taiwanese)
            Biography = PluginManager.XMLReadBoolean(Reader, conBiographyWGParm, Biography)
            Films = PluginManager.XMLReadBoolean(Reader, conFilmsWGParm, Films)
            Fortifications = PluginManager.XMLReadBoolean(Reader, conFortificationsWGParm, Fortifications)
            Intel = PluginManager.XMLReadBoolean(Reader, conIntelWGParm, Intel)
            LandVehicles = PluginManager.XMLReadBoolean(Reader, conLandVehWGParm, LandVehicles)
            National = PluginManager.XMLReadBoolean(Reader, conNationalWGParm, National)
            Muslim = PluginManager.XMLReadBoolean(Reader, conMuslimWGParm, Muslim)
            Crusades = PluginManager.XMLReadBoolean(Reader, conCrusadesWGParm, Crusades)
            ARW = PluginManager.XMLReadBoolean(Reader, conARWWGParm, ARW)
        End Sub
        Friend Sub WriteXML(ByVal Writer As System.Xml.XmlTextWriter) Implements IGenericSettings.WriteXML
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

                .WriteAttributeString(conNoTFWGParm, NoTaskForce.ToString)
                .WriteAttributeString(conBalticWGParm, Baltic.ToString)
                .WriteAttributeString(conLebaneseWGParm, Lebanese.ToString)
                .WriteAttributeString(conMidEastWGParm, MiddleEastern.ToString)
                .WriteAttributeString(conRomanianWGParm, Romanian.ToString)
                .WriteAttributeString(conSAmericanWGParm, SouthAmerican.ToString)
                .WriteAttributeString(conSEAsianWGParm, SoutheastAsian.ToString)
                .WriteAttributeString(conTaiwaneseWGParm, Taiwanese.ToString)
                .WriteAttributeString(conBiographyWGParm, Biography.ToString)
                .WriteAttributeString(conFilmsWGParm, Films.ToString)
                .WriteAttributeString(conFortificationsWGParm, Fortifications.ToString)
                .WriteAttributeString(conIntelWGParm, Intel.ToString)
                .WriteAttributeString(conLandVehWGParm, LandVehicles.ToString)
                .WriteAttributeString(conNationalWGParm, National.ToString)
                .WriteAttributeString(conMuslimWGParm, Muslim.ToString)
                .WriteAttributeString(conCrusadesWGParm, Crusades.ToString)
                .WriteAttributeString(conARWWGParm, ARW.ToString)
            End With
        End Sub
        Friend Sub Reset() Implements IGenericSettings.XMLReset
            StubClass = False
            AutoStub = False
            ForceImportanceRemoval = False

            For Each ctl As Control In Me.TabControl1.Controls
                Dim chk As CheckBox = CType(ctl, CheckBox)
                If chk IsNot Nothing Then
                    chk.Checked = False
                End If
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
        Friend Property NoTaskForce() As Boolean
            Get
                Return NoTaskForceCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                NoTaskForceCheckBox.Checked = value
            End Set
        End Property
        Friend Property Baltic() As Boolean
            Get
                Return BalticCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                BalticCheckBox.Checked = value
            End Set
        End Property
        Friend Property Lebanese() As Boolean
            Get
                Return LebaneseCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                LebaneseCheckBox.Checked = value
            End Set
        End Property
        Friend Property MiddleEastern() As Boolean
            Get
                Return MiddleEastCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                MiddleEastCheckBox.Checked = value
            End Set
        End Property
        Friend Property Romanian() As Boolean
            Get
                Return RomanianCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                RomanianCheckBox.Checked = value
            End Set
        End Property
        Friend Property SouthAmerican() As Boolean
            Get
                Return SouthAmericanCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                SouthAmericanCheckBox.Checked = value
            End Set
        End Property
        Friend Property SoutheastAsian() As Boolean
            Get
                Return SoutheastAsianCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                SoutheastAsianCheckBox.Checked = value
            End Set
        End Property
        Friend Property Taiwanese() As Boolean
            Get
                Return TaiwaneseCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                TaiwaneseCheckBox.Checked = value
            End Set
        End Property
        Friend Property Biography() As Boolean
            Get
                Return BiographyCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                BiographyCheckBox.Checked = value
            End Set
        End Property
        Friend Property Films() As Boolean
            Get
                Return FilmsCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                FilmsCheckBox.Checked = value
            End Set
        End Property
        Friend Property Fortifications() As Boolean
            Get
                Return FortificationsCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                FortificationsCheckBox.Checked = value
            End Set
        End Property
        Friend Property Intel() As Boolean
            Get
                Return IntelCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                IntelCheckBox.Checked = value
            End Set
        End Property
        Friend Property LandVehicles() As Boolean
            Get
                Return LandVehiclesCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                LandVehiclesCheckBox.Checked = value
            End Set
        End Property
        Friend Property National() As Boolean
            Get
                Return NationalCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                NationalCheckBox.Checked = value
            End Set
        End Property
        Friend Property Muslim() As Boolean
            Get
                Return MuslimCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                MuslimCheckBox.Checked = value
            End Set
        End Property
        Friend Property Crusades() As Boolean
            Get
                Return CrusadesCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                CrusadesCheckBox.Checked = value
            End Set
        End Property
        Friend Property ARW() As Boolean
            Get
                Return ARWCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                ARWCheckBox.Checked = value
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

        Private Sub AviationToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AviationToolStripMenuItem.Click
            PluginManager.EditBoxInsertYesParam("Aviation")
        End Sub

        Private Sub HistoriographyToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles HistoriographyToolStripMenuItem.Click
            PluginManager.EditBoxInsertYesParam("Historiography")
        End Sub

        Private Sub MaritimeToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MaritimeToolStripMenuItem.Click
            PluginManager.EditBoxInsertYesParam("Maritime")
        End Sub

        Private Sub MemorialsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MemorialsToolStripMenuItem.Click
            PluginManager.EditBoxInsertYesParam("Memorials")
        End Sub

        Private Sub ScienceToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ScienceToolStripMenuItem.Click
            PluginManager.EditBoxInsertYesParam("Science")
        End Sub

        Private Sub TechnologyToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TechnologyToolStripMenuItem.Click
            PluginManager.EditBoxInsertYesParam("Technology")
        End Sub

        Private Sub WeaponryToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles WeaponryToolStripMenuItem.Click
            PluginManager.EditBoxInsertYesParam("Weaponry")
        End Sub

        Private Sub AfricanToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AfricanToolStripMenuItem.Click
            PluginManager.EditBoxInsertYesParam("African")
        End Sub

        Private Sub AustralianToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AustralianToolStripMenuItem.Click
            PluginManager.EditBoxInsertYesParam("Australian")
        End Sub

        Private Sub BalkanToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BalkanToolStripMenuItem.Click
            PluginManager.EditBoxInsertYesParam("Balkan")
        End Sub

        Private Sub BritishToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BritishToolStripMenuItem.Click
            PluginManager.EditBoxInsertYesParam("British")
        End Sub

        Private Sub CanadianToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CanadianToolStripMenuItem.Click
            PluginManager.EditBoxInsertYesParam("Canadian")
        End Sub

        Private Sub ChineseToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ChineseToolStripMenuItem.Click
            PluginManager.EditBoxInsertYesParam("Chinese")
        End Sub

        Private Sub DutchToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DutchToolStripMenuItem.Click
            PluginManager.EditBoxInsertYesParam("Dutch")
        End Sub

        Private Sub GermanToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GermanToolStripMenuItem.Click
            PluginManager.EditBoxInsertYesParam("German")
        End Sub

        Private Sub FrenchToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FrenchToolStripMenuItem.Click
            PluginManager.EditBoxInsertYesParam("French")
        End Sub

        Private Sub IndianToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles IndianToolStripMenuItem.Click
            PluginManager.EditBoxInsertYesParam("Indian")
        End Sub

        Private Sub ItalianToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ItalianToolStripMenuItem.Click
            PluginManager.EditBoxInsertYesParam("Italian")
        End Sub

        Private Sub JapaneseToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles JapaneseToolStripMenuItem.Click
            PluginManager.EditBoxInsertYesParam("Japanese")
        End Sub

        Private Sub KoreanToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles KoreanToolStripMenuItem.Click
            PluginManager.EditBoxInsertYesParam("Korean")
        End Sub

        Private Sub NewZealandToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NewZealandToolStripMenuItem.Click
            PluginManager.EditBoxInsertYesParam("NewZealand")
        End Sub

        Private Sub NordicToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NordicToolStripMenuItem.Click
            PluginManager.EditBoxInsertYesParam("Nordic")
        End Sub

        Private Sub OttomanToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OttomanToolStripMenuItem.Click
            PluginManager.EditBoxInsertYesParam("Ottoman")
        End Sub

        Private Sub PolishToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PolishToolStripMenuItem.Click
            PluginManager.EditBoxInsertYesParam("Polish")
        End Sub

        Private Sub RussianToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RussianToolStripMenuItem.Click
            PluginManager.EditBoxInsertYesParam("Russian")
        End Sub

        Private Sub SpanishToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SpanishToolStripMenuItem.Click
            PluginManager.EditBoxInsertYesParam("Spanish")
        End Sub

        Private Sub USToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles USToolStripMenuItem.Click
            PluginManager.EditBoxInsertYesParam("US")
        End Sub
        Private Sub ClassicalToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ClassicalToolStripMenuItem.Click
            PluginManager.EditBoxInsertYesParam("Classical")
        End Sub

        Private Sub MedievalToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MedievalToolStripMenuItem.Click
            PluginManager.EditBoxInsertYesParam("Medieval")
        End Sub

        Private Sub EarlyModernToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles EarlyModernToolStripMenuItem.Click
            PluginManager.EditBoxInsertYesParam("Early-Modern")
        End Sub

        Private Sub NapoleonicToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NapoleonicToolStripMenuItem.Click
            PluginManager.EditBoxInsertYesParam("Napoleonic")
        End Sub

        Private Sub WWIToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles WWIToolStripMenuItem.Click
            PluginManager.EditBoxInsertYesParam("WWI")
        End Sub

        Private Sub WWIIToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles WWIIToolStripMenuItem.Click
            PluginManager.EditBoxInsertYesParam("WWII")
        End Sub
    End Class

    Friend NotInheritable Class WPMilHist
        Inherits PluginBase

        ' Settings:
        Private OurTab As New TabPage("MilHist")
        Private WithEvents OurSettingsControl As New WPMilHistSettings
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
                If .AmericanCivilWar Then AddAndLogNewParamWithAYesValue("ACW")
                If .Australian Then AddAndLogNewParamWithAYesValue("Australian")
                If .Aviation Then AddAndLogNewParamWithAYesValue("Aviation")
                If .British Then AddAndLogNewParamWithAYesValue("British")
                If .Canadian Then AddAndLogNewParamWithAYesValue("Canadian")
                If .Chinese Then AddAndLogNewParamWithAYesValue("Chinese")
                If .Classical Then AddAndLogNewParamWithAYesValue("Classical")
                If .Dutch Then AddAndLogNewParamWithAYesValue("Dutch")
                If .EarlyModern Then AddAndLogNewParamWithAYesValue("Early-Modern")
                If .French Then AddAndLogNewParamWithAYesValue("French")
                If .German Then AddAndLogNewParamWithAYesValue("German")
                If .Indian Then AddAndLogNewParamWithAYesValue("Indian")
                If .Italian Then AddAndLogNewParamWithAYesValue("Italian")
                If .Japanese Then AddAndLogNewParamWithAYesValue("Japanese")
                If .Maritime Then AddAndLogNewParamWithAYesValue("Maritime")
                If .Memorials Then AddAndLogNewParamWithAYesValue("Memorials")
                If .Medieval Then AddAndLogNewParamWithAYesValue("Medieval")
                If .Napoleonic Then AddAndLogNewParamWithAYesValue("Napoleonic")
                If .Polish Then AddAndLogNewParamWithAYesValue("Polish")
                If .US Then AddAndLogNewParamWithAYesValue("US")
                If .Weaponry Then AddAndLogNewParamWithAYesValue("Weaponry")
                If .WWI Then AddAndLogNewParamWithAYesValue("WWI")
                If .WWII Then AddAndLogNewParamWithAYesValue("WWII")

                If .Historiography Then AddAndLogNewParamWithAYesValue("Historiography")
                If .Science Then AddAndLogNewParamWithAYesValue("Science")
                If .Technology Then AddAndLogNewParamWithAYesValue("Technology")
                If .African Then AddAndLogNewParamWithAYesValue("African")
                If .Balkan Then AddAndLogNewParamWithAYesValue("Balkan")
                If .Korean Then AddAndLogNewParamWithAYesValue("Korean")
                If .NewZealand Then AddAndLogNewParamWithAYesValue("New-Zealand")
                If .Nordic Then AddAndLogNewParamWithAYesValue("Nordic")
                If .Ottoman Then AddAndLogNewParamWithAYesValue("Ottoman")
                If .Russian Then AddAndLogNewParamWithAYesValue("Russian")
                If .Spanish Then AddAndLogNewParamWithAYesValue("Spanish")

                If .NoTaskForce Then AddAndLogNewParamWithAYesValue("no-task-force")
                If .Baltic Then AddAndLogNewParamWithAYesValue("Baltic")
                If .Lebanese Then AddAndLogNewParamWithAYesValue("Lebanese")
                If .MiddleEastern Then AddAndLogNewParamWithAYesValue("Middle-Eastern")
                If .Romanian Then AddAndLogNewParamWithAYesValue("Romanian")
                If .SouthAmerican Then AddAndLogNewParamWithAYesValue("South-American")
                If .SoutheastAsian Then AddAndLogNewParamWithAYesValue("Southeast-Asian")
                If .Taiwanese Then AddAndLogNewParamWithAYesValue("Taiwanese")
                If .Biography Then AddAndLogNewParamWithAYesValue("Biography")
                If .Films Then AddAndLogNewParamWithAYesValue("Films")
                If .Fortifications Then AddAndLogNewParamWithAYesValue("Fortifications")
                If .Intel Then AddAndLogNewParamWithAYesValue("Intel")
                If .LandVehicles Then AddAndLogNewParamWithAYesValue("Land-vehicles")
                If .National Then AddAndLogNewParamWithAYesValue("National")
                If .Muslim Then AddAndLogNewParamWithAYesValue("Muslim")
                If .Crusades Then AddAndLogNewParamWithAYesValue("Crusades")
                If .ARW Then AddAndLogNewParamWithAYesValue("ARW")
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