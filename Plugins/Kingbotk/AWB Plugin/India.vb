Imports WikiFunctions

'Copyright © 2008 Stephen Kennedy (Kingboyk) http://www.sdk-software.com/
'Copyright © 2008 Sam Reed (Reedy) http://www.reedyboy.net/

'This program is free software; you can redistribute it and/or modify it under the terms of Version 2 of the GNU General Public License as published by the Free Software Foundation.

'This program is distributed in the hope that it will be useful,
'but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

'You should have received a copy of the GNU General Public License Version 2 along with this program; if not, write to the Free Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA

' By Reedy, based on the Australia plugin
Namespace AutoWikiBrowser.Plugins.Kingbotk.Plugins
    Friend NotInheritable Class WPIndiaSettings
        Implements IGenericSettings

        'India 1 
        Private Const conGeogParm As String = "IndGeography"
        Private Const conStatesParm As String = "IndStates"
        Private Const conDistrictsParm As String = "IndDistricts"
        Private Const conCitiesParm As String = "IndCities"
        Private Const conMapsParm As String = "IndMaps"
        Private Const conCinemaParm As String = "IndCinema"
        Private Const conHistoryParm As String = "IndHistory"
        Private Const conLiteratureParm As String = "IndLiterature"
        Private Const conPoliticsParm As String = "IndPolitics"
        Private Const conProtectedAreasParm As String = "IndProtectedAreas"
        Private Const conTamilParm As String = "IndTamil"
        Private Const conTelevisionParm As String = "IndTele"

        'India 2
        Private Const conAndhraParm As String = "IndAndhra"
        Private Const conArunachalParm As String = "IndArunachal"
        Private Const conAssamParm As String = "IndAssam"
        Private Const conBengalParm As String = "IndBengal"
        Private Const conBiharParm As String = "IndBihar"
        Private Const conChhattisgarhParm As String = "IndChhattisgarh"
        Private Const conGoaParm As String = "IndGoa"
        Private Const conGujaratParm As String = "IndGujarat"
        Private Const conHaryanaParm As String = "IndHaryana"
        Private Const conHimachalParm As String = "IndHimachal"
        Private Const conJandKParm As String = "IndJandK"
        Private Const conJharkhandParm As String = "IndJharkhand"
        Private Const conKarnatakaParm As String = "IndKarnataka"
        Private Const conKeralaParm As String = "IndKerala"
        Private Const conMadhyaParm As String = "IndMadhya"
        Private Const conMaharashtraParm As String = "IndMaharashtra"
        Private Const conManipurParm As String = "IndManipur"
        Private Const conMeghalayaParm As String = "IndMeghalaya"
        Private Const conMizoramParm As String = "IndMizoram"
        Private Const conNagalandParm As String = "IndNagaland"
        Private Const conOrissaParm As String = "IndOrissa"
        Private Const conPunjabParm As String = "IndPunjab"
        Private Const conRajasthanParm As String = "IndRajasthan"
        Private Const conSikkimParm As String = "IndSikkim"
        Private Const conTamilnaduParm As String = "IndTamilnadu"
        Private Const conTripuraParm As String = "IndTripura"
        Private Const conUttarParm As String = "IndUttar"
        Private Const conUttarakandParm As String = "IndUttarakand"

        'India 3
        Private Const conAndamanParm As String = "IndAndaman"
        Private Const conChandigarhParm As String = "IndChandigarh"
        Private Const conDadraParm As String = "IndDadra"
        Private Const conDamanParm As String = "IndDaman"
        Private Const conDelhiParm As String = "IndDelhi"
        Private Const conLakshadweepParm As String = "IndLakshadweep"
        Private Const conPuducherryParm As String = "IndPuducherry"
        Private Const conChennaiParm As String = "IndChennai"
        Private Const conMumbaiParm As String = "IndMumbai"

#Region "XML interface:"
        Friend Sub ReadXML(ByVal Reader As System.Xml.XmlTextReader) Implements IGenericSettings.ReadXML
            'India 1
            Geography = PluginManager.XMLReadBoolean(Reader, conGeogParm, Geography)
            States = PluginManager.XMLReadBoolean(Reader, conStatesParm, States)
            Districts = PluginManager.XMLReadBoolean(Reader, conDistrictsParm, Districts)
            Cities = PluginManager.XMLReadBoolean(Reader, conCitiesParm, Cities)
            Maps = PluginManager.XMLReadBoolean(Reader, conMapsParm, Maps)
            Cinema = PluginManager.XMLReadBoolean(Reader, conCinemaParm, Cinema)
            History = PluginManager.XMLReadBoolean(Reader, conHistoryParm, History)
            Literature = PluginManager.XMLReadBoolean(Reader, conLiteratureParm, Literature)
            Politics = PluginManager.XMLReadBoolean(Reader, conPoliticsParm, Politics)
            ProtectedAreas = PluginManager.XMLReadBoolean(Reader, conProtectedAreasParm, ProtectedAreas)
            Tamil = PluginManager.XMLReadBoolean(Reader, conTamilParm, Tamil)
            Television = PluginManager.XMLReadBoolean(Reader, conTelevisionParm, Television)

            'India 2
            Andhra = PluginManager.XMLReadBoolean(Reader, conAndhraParm, Andhra)
            Arunachal = PluginManager.XMLReadBoolean(Reader, conArunachalParm, Arunachal)
            Assam = PluginManager.XMLReadBoolean(Reader, conAssamParm, Assam)
            Bengal = PluginManager.XMLReadBoolean(Reader, conBengalParm, Bengal)
            Bihar = PluginManager.XMLReadBoolean(Reader, conBiharParm, Bihar)
            Chandigarh = PluginManager.XMLReadBoolean(Reader, conChandigarhParm, Chandigarh)
            Goa = PluginManager.XMLReadBoolean(Reader, conGoaParm, Goa)
            Gujarat = PluginManager.XMLReadBoolean(Reader, conGujaratParm, Gujarat)
            Haryana = PluginManager.XMLReadBoolean(Reader, conHaryanaParm, Haryana)
            Himachal = PluginManager.XMLReadBoolean(Reader, conHimachalParm, Himachal)
            JandK = PluginManager.XMLReadBoolean(Reader, conJandKParm, JandK)
            Jharkhand = PluginManager.XMLReadBoolean(Reader, conJharkhandParm, Jharkhand)
            Karnataka = PluginManager.XMLReadBoolean(Reader, conKarnatakaParm, Karnataka)
            Kerala = PluginManager.XMLReadBoolean(Reader, conKeralaParm, Kerala)
            Madhya = PluginManager.XMLReadBoolean(Reader, conMadhyaParm, Madhya)
            Maharashtra = PluginManager.XMLReadBoolean(Reader, conMaharashtraParm, Maharashtra)
            Manipur = PluginManager.XMLReadBoolean(Reader, conManipurParm, Manipur)
            Meghalaya = PluginManager.XMLReadBoolean(Reader, conMeghalayaParm, Meghalaya)
            Mizoram = PluginManager.XMLReadBoolean(Reader, conMizoramParm, Mizoram)
            Nagaland = PluginManager.XMLReadBoolean(Reader, conNagalandParm, Nagaland)
            Orissa = PluginManager.XMLReadBoolean(Reader, conOrissaParm, Orissa)
            Punjab = PluginManager.XMLReadBoolean(Reader, conPunjabParm, Punjab)
            Rajasthan = PluginManager.XMLReadBoolean(Reader, conRajasthanParm, Rajasthan)
            Sikkim = PluginManager.XMLReadBoolean(Reader, conSikkimParm, Sikkim)
            Tamilnadu = PluginManager.XMLReadBoolean(Reader, conTamilnaduParm, Tamilnadu)
            Uttar = PluginManager.XMLReadBoolean(Reader, conUttarParm, Uttar)
            Uttarakand = PluginManager.XMLReadBoolean(Reader, conUttarakandParm, Uttarakand)


            'India 3
            Andaman = PluginManager.XMLReadBoolean(Reader, conAndamanParm, Andaman)
            Chandigarh = PluginManager.XMLReadBoolean(Reader, conChandigarhParm, Chandigarh)
            Dadra = PluginManager.XMLReadBoolean(Reader, conDadraParm, Dadra)
            Daman = PluginManager.XMLReadBoolean(Reader, conDamanParm, Daman)
            Delhi = PluginManager.XMLReadBoolean(Reader, conDelhiParm, Delhi)
            Lakshadweep = PluginManager.XMLReadBoolean(Reader, conLakshadweepParm, Lakshadweep)
            Puducherry = PluginManager.XMLReadBoolean(Reader, conPuducherryParm, Puducherry)
            Chennai = PluginManager.XMLReadBoolean(Reader, conChennaiParm, Chennai)
            Mumbai = PluginManager.XMLReadBoolean(Reader, conMumbaiParm, Mumbai)
        End Sub
        Friend Sub WriteXML(ByVal Writer As System.Xml.XmlTextWriter) Implements IGenericSettings.WriteXML
            With Writer
                'India 1
                .WriteAttributeString(conGeogParm, Geography.ToString)
                .WriteAttributeString(conStatesParm, States.ToString)
                .WriteAttributeString(conDistrictsParm, States.ToString)
                .WriteAttributeString(conCitiesParm, Cities.ToString)
                .WriteAttributeString(conMapsParm, Maps.ToString)
                .WriteAttributeString(conCinemaParm, Cinema.ToString)
                .WriteAttributeString(conHistoryParm, History.ToString)
                .WriteAttributeString(conLiteratureParm, Literature.ToString)
                .WriteAttributeString(conPoliticsParm, Politics.ToString)
                .WriteAttributeString(conProtectedAreasParm, ProtectedAreas.ToString)
                .WriteAttributeString(conTamilParm, Tamil.ToString)
                .WriteAttributeString(conTelevisionParm, Television.ToString)

                'India 2
                .WriteAttributeString(conAndhraParm, Andhra.ToString)
                .WriteAttributeString(conArunachalParm, Arunachal.ToString)
                .WriteAttributeString(conAssamParm, Assam.ToString)
                .WriteAttributeString(conBengalParm, Bengal.ToString)
                .WriteAttributeString(conBiharParm, Bihar.ToString)
                .WriteAttributeString(conChhattisgarhParm, Chhattisgarh.ToString)
                .WriteAttributeString(conGoaParm, Goa.ToString)
                .WriteAttributeString(conGujaratParm, Gujarat.ToString)
                .WriteAttributeString(conHaryanaParm, Haryana.ToString)
                .WriteAttributeString(conHimachalParm, Himachal.ToString)
                .WriteAttributeString(conJandKParm, JandK.ToString)
                .WriteAttributeString(conJharkhandParm, Jharkhand.ToString)
                .WriteAttributeString(conKarnatakaParm, Karnataka.ToString)
                .WriteAttributeString(conKeralaParm, Kerala.ToString)
                .WriteAttributeString(conMadhyaParm, Madhya.ToString)
                .WriteAttributeString(conMaharashtraParm, Maharashtra.ToString)
                .WriteAttributeString(conManipurParm, Manipur.ToString)
                .WriteAttributeString(conMeghalayaParm, Meghalaya.ToString)
                .WriteAttributeString(conMizoramParm, Mizoram.ToString)
                .WriteAttributeString(conNagalandParm, Nagaland.ToString)
                .WriteAttributeString(conOrissaParm, Orissa.ToString)
                .WriteAttributeString(conPunjabParm, Punjab.ToString)
                .WriteAttributeString(conRajasthanParm, Rajasthan.ToString)
                .WriteAttributeString(conSikkimParm, Sikkim.ToString)
                .WriteAttributeString(conTamilnaduParm, Tamilnadu.ToString)
                .WriteAttributeString(conUttarParm, Uttar.ToString)
                .WriteAttributeString(conUttarakandParm, Uttarakand.ToString)

                'India 3
                .WriteAttributeString(conAndamanParm, Andaman.ToString)
                .WriteAttributeString(conChandigarhParm, Chandigarh.ToString)
                .WriteAttributeString(conDadraParm, Dadra.ToString)
                .WriteAttributeString(conDamanParm, Daman.ToString)
                .WriteAttributeString(conDelhiParm, Delhi.ToString)
                .WriteAttributeString(conLakshadweepParm, Lakshadweep.ToString)
                .WriteAttributeString(conPuducherryParm, Puducherry.ToString)
                .WriteAttributeString(conChennaiParm, Chennai.ToString)
                .WriteAttributeString(conMumbaiParm, Mumbai.ToString)
            End With
        End Sub
        Friend Sub Reset() Implements IGenericSettings.XMLReset
            AutoStub = False
            StubClass = False

            For Each ctl As Control In Me.TabControl1.Controls
                If TypeOf ctl Is CheckBox Then DirectCast(ctl, CheckBox).Checked = False
            Next
        End Sub
#End Region
        'Properties:
        'India 1 
        Friend Property Geography() As Boolean
            Get
                Return GeogCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                GeogCheckBox.Checked = value
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

        Friend Property Districts() As Boolean
            Get
                Return DistrictsCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                DistrictsCheckBox.Checked = value
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

        Friend Property Maps() As Boolean
            Get
                Return MapsCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                MapsCheckBox.Checked = value
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

        Friend Property History() As Boolean
            Get
                Return HistoryCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                HistoryCheckBox.Checked = value
            End Set
        End Property

        Friend Property Literature() As Boolean
            Get
                Return LiteratureCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                LiteratureCheckBox.Checked = value
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

        Friend Property ProtectedAreas() As Boolean
            Get
                Return ProtectedCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                ProtectedCheckBox.Checked = value
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

        Friend Property Television() As Boolean
            Get
                Return TelevisionCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                TelevisionCheckBox.Checked = value
            End Set
        End Property

        'India 2
        Friend Property Andhra() As Boolean
            Get
                Return AndhraCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                AndhraCheckBox.Checked = value
            End Set
        End Property

        Friend Property Arunachal() As Boolean
            Get
                Return ArunachalCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                ArunachalCheckBox.Checked = value
            End Set
        End Property

        Friend Property Assam() As Boolean
            Get
                Return AssamCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                AssamCheckBox.Checked = value
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

        Friend Property Bihar() As Boolean
            Get
                Return BiharCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                BiharCheckBox.Checked = value
            End Set
        End Property

        Friend Property Chhattisgarh() As Boolean
            Get
                Return ChhattisgarhCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                ChhattisgarhCheckBox.Checked = value
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

        Friend Property Gujarat() As Boolean
            Get
                Return GujuratCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                GujuratCheckBox.Checked = value
            End Set
        End Property

        Friend Property Haryana() As Boolean
            Get
                Return HaryanaCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                HaryanaCheckBox.Checked = value
            End Set
        End Property

        Friend Property Himachal() As Boolean
            Get
                Return HimachalCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                HimachalCheckBox.Checked = value
            End Set
        End Property

        Friend Property JandK() As Boolean
            Get
                Return JandkCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                JandkCheckBox.Checked = value
            End Set
        End Property

        Friend Property Jharkhand() As Boolean
            Get
                Return JharkhandCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                JharkhandCheckBox.Checked = value
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

        Friend Property Madhya() As Boolean
            Get
                Return MadhyaCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                MadhyaCheckBox.Checked = value
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

        Friend Property Manipur() As Boolean
            Get
                Return ManipurCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                ManipurCheckBox.Checked = value
            End Set
        End Property

        Friend Property Meghalaya() As Boolean
            Get
                Return MeghalayaCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                MeghalayaCheckBox.Checked = value
            End Set
        End Property

        Friend Property Mizoram() As Boolean
            Get
                Return MizoramCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                MizoramCheckBox.Checked = value
            End Set
        End Property

        Friend Property Nagaland() As Boolean
            Get
                Return NagalandCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                NagalandCheckBox.Checked = value
            End Set
        End Property

        Friend Property Orissa() As Boolean
            Get
                Return OrissaCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                OrissaCheckBox.Checked = value
            End Set
        End Property

        Friend Property Punjab() As Boolean
            Get
                Return PunjabCheckbox.Checked
            End Get
            Set(ByVal value As Boolean)
                PunjabCheckbox.Checked = value
            End Set
        End Property

        Friend Property Rajasthan() As Boolean
            Get
                Return RajasthanCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                RajasthanCheckBox.Checked = value
            End Set
        End Property

        Friend Property Sikkim() As Boolean
            Get
                Return SikkimCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                SikkimCheckBox.Checked = value
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

        Friend Property Tripura() As Boolean
            Get
                Return TripuraCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                TripuraCheckBox.Checked = value
            End Set
        End Property

        Friend Property Uttar() As Boolean
            Get
                Return UttarCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                UttarCheckBox.Checked = value
            End Set
        End Property

        Friend Property Uttarakand() As Boolean
            Get
                Return UttarakandCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                UttarakandCheckBox.Checked = value
            End Set
        End Property

        'India 3
        Friend Property Andaman() As Boolean
            Get
                Return AndamanCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                AndamanCheckBox.Checked = value
            End Set
        End Property

        Friend Property Chandigarh() As Boolean
            Get
                Return ChandigarhCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                ChandigarhCheckBox.Checked = value
            End Set
        End Property

        Friend Property Dadra() As Boolean
            Get
                Return DadraCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                DadraCheckBox.Checked = value
            End Set
        End Property

        Friend Property Daman() As Boolean
            Get
                Return DamanCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                DamanCheckBox.Checked = value
            End Set
        End Property

        Friend Property Delhi() As Boolean
            Get
                Return DelhiCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                DelhiCheckBox.Checked = value
            End Set
        End Property

        Friend Property Lakshadweep() As Boolean
            Get
                Return LakshadweepCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                LakshadweepCheckBox.Checked = value
            End Set
        End Property

        Friend Property Puducherry() As Boolean
            Get
                Return PuducherryCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                PuducherryCheckBox.Checked = value
            End Set
        End Property

        Friend Property Chennai() As Boolean
            Get
                Return ChennaiCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                ChennaiCheckBox.Checked = value
            End Set
        End Property

        Friend Property Mumbai() As Boolean
            Get
                Return MumbaiCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                MumbaiCheckBox.Checked = value
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

        Friend ReadOnly Property TextInsertContextMenuStripItems() As ToolStripItemCollection _
        Implements IGenericSettings.TextInsertContextMenuStripItems
            Get
                Return TextInsertContextMenuStrip.Items
            End Get
        End Property

        ' Event handlers:
        Private Sub LinkClicked(ByVal sender As Object, ByVal e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
            Tools.OpenENArticleInBrowser("Template:WP_India", False)
        End Sub

        Private Sub AutoStubCheckBox_CheckedChanged(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles AutoStubCheckBox.CheckedChanged
            If AutoStubCheckBox.Checked Then StubClassCheckBox.Checked = False
        End Sub

        Private Sub StubClassCheckBox_CheckedChanged(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles StubClassCheckBox.CheckedChanged
            If StubClassCheckBox.Checked Then AutoStubCheckBox.Checked = False
        End Sub

        Private Sub WPIndiaSettings_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        End Sub
    End Class

    Friend NotInheritable Class WPIndia
        Inherits PluginBase

        ' Settings:
        Private OurTab As New TabPage("India")
        Private WithEvents OurSettingsControl As New WPIndiaSettings
        Private Const conEnabled As String = "IndEnabled"

        Protected Friend Overrides ReadOnly Property PluginShortName() As String
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
        Protected Friend Overrides ReadOnly Property GenericSettings() As IGenericSettings
            Get
                Return OurSettingsControl
            End Get
        End Property
        Protected Overrides ReadOnly Property CategoryTalkClassParm() As String
            Get
                Return "Cat"
            End Get
        End Property
        Protected Overrides ReadOnly Property TemplateTalkClassParm() As String
            Get
                Return "Template"
            End Get
        End Property
        Protected Overrides ReadOnly Property PreferredTemplateName() As String
            Get
                Return "WP India"
            End Get
        End Property
        Friend Overrides ReadOnly Property HasReqPhotoParam() As Boolean
            Get
                Return True
            End Get
        End Property
        Friend Overrides Sub ReqPhoto()
            AddNewParamWithAYesValue("image-needed")
        End Sub
        'Protected Overrides ReadOnly Property PreferredTemplateNameRegexString() As String
        '    Get
        '        Return "^[Ww]P India$"
        '    End Get
        'End Property

        ' Initialisation:
        Friend Sub New(ByVal Manager As PluginManager)
            MyBase.New("") ' Specify alternate names only
        End Sub
        Protected Friend Overrides Sub Initialise()
            OurMenuItem = New ToolStripMenuItem("India Plugin")
            MyBase.InitialiseBase() ' must set menu item object first
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
                If .Geography Then
                    AddAndLogNewParamWithAYesValue("geography")
                    AddEmptyParam("geography-importance")
                End If
                If .States Then
                    AddAndLogNewParamWithAYesValue("states")
                    AddEmptyParam("states-importance")
                End If
                If .Districts Then
                    AddAndLogNewParamWithAYesValue("districts")
                    AddEmptyParam("districts-importance")
                End If
                If .Cities Then
                    AddAndLogNewParamWithAYesValue("cities")
                    AddEmptyParam("cities-importance")
                End If
                If .Maps Then
                    AddAndLogNewParamWithAYesValue("maps")
                End If
                If .Cinema Then
                    AddAndLogNewParamWithAYesValue("cinema")
                    AddEmptyParam("cinema-importance")
                End If
                If .History Then
                    AddAndLogNewParamWithAYesValue("history")
                    AddEmptyParam("history-importance")
                End If
                If .Literature Then
                    AddAndLogNewParamWithAYesValue("literature")
                    AddEmptyParam("literature-importance")
                End If
                If .Politics Then
                    AddAndLogNewParamWithAYesValue("politics")
                    AddEmptyParam("politics-importance")
                End If
                If .ProtectedAreas Then
                    AddAndLogNewParamWithAYesValue("protected-areas")
                    AddEmptyParam("protected-areas-importance")
                End If
                If .Tamil Then
                    AddAndLogNewParamWithAYesValue("tamil")
                    AddEmptyParam("tamil-importance")
                End If
                If .Television Then
                    AddAndLogNewParamWithAYesValue("television")
                    AddEmptyParam("television-importance")
                End If
                If .Andhra Then
                    AddAndLogNewParamWithAYesValue("andhra")
                    AddEmptyParam("andhra-importance")
                End If
                If .Arunachal Then
                    AddAndLogNewParamWithAYesValue("arunachal")
                    AddEmptyParam("arunachal-importance")
                End If
                If .Assam Then
                    AddAndLogNewParamWithAYesValue("assam")
                    AddEmptyParam("assam-importance")
                End If
                If .Bengal Then
                    AddAndLogNewParamWithAYesValue("bengal")
                    AddEmptyParam("bengal-importance")
                End If
                If .Bihar Then
                    AddAndLogNewParamWithAYesValue("bihar")
                    AddEmptyParam("bihar-importance")
                End If
                If .Chhattisgarh Then
                    AddAndLogNewParamWithAYesValue("chhattisgarh")
                    AddEmptyParam("chhattisgarh-importance")
                End If
                If .Goa Then
                    AddAndLogNewParamWithAYesValue("goa")
                    AddEmptyParam("goa-importance")
                End If
                If .Gujarat Then
                    AddAndLogNewParamWithAYesValue("gujarat")
                    AddEmptyParam("gujarat-importance")
                End If
                If .Haryana Then
                    AddAndLogNewParamWithAYesValue("haryana")
                    AddEmptyParam("haryana-importance")
                End If
                If .Himachal Then
                    AddAndLogNewParamWithAYesValue("himachal")
                    AddEmptyParam("himachal-importance")
                End If
                If .JandK Then
                    AddAndLogNewParamWithAYesValue("jandk")
                    AddEmptyParam("jandk-importance")
                End If
                If .Jharkhand Then
                    AddAndLogNewParamWithAYesValue("jharkhand")
                    AddEmptyParam("jharkhand-importance")
                End If
                If .Kerala Then
                    AddAndLogNewParamWithAYesValue("kerala")
                    AddEmptyParam("kerala-importance")
                End If
                If .Karnataka Then
                    AddAndLogNewParamWithAYesValue("karnataka")
                    AddEmptyParam("karnataka-importance")
                End If
                If .Madhya Then
                    AddAndLogNewParamWithAYesValue("madhya")
                    AddEmptyParam("madhya-importance")
                End If
                If .Maharashtra Then
                    AddAndLogNewParamWithAYesValue("maharashtra")
                    AddEmptyParam("maharashtra-importance")
                End If
                If .Manipur Then
                    AddAndLogNewParamWithAYesValue("manipur")
                    AddEmptyParam("manipur-importance")
                End If
                If .Meghalaya Then
                    AddAndLogNewParamWithAYesValue("meghalaya")
                    AddEmptyParam("meghalaya-importance")
                End If
                If .Mizoram Then
                    AddAndLogNewParamWithAYesValue("mizoram")
                    AddEmptyParam("mizoram-importance")
                End If
                If .Nagaland Then
                    AddAndLogNewParamWithAYesValue("nagaland")
                    AddEmptyParam("nagaland-importance")
                End If
                If .Orissa Then
                    AddAndLogNewParamWithAYesValue("orissa")
                    AddEmptyParam("orissa-importance")
                End If
                If .Punjab Then
                    AddAndLogNewParamWithAYesValue("punjab")
                    AddEmptyParam("punjab-importance")
                End If
                If .Rajasthan Then
                    AddAndLogNewParamWithAYesValue("rajasthan")
                    AddEmptyParam("rajasthan-importance")
                End If
                If .Sikkim Then
                    AddAndLogNewParamWithAYesValue("sikkim")
                    AddEmptyParam("sikkim-importance")
                End If
                If .Tamilnadu Then
                    AddAndLogNewParamWithAYesValue("tamilnadu")
                    AddEmptyParam("tamilnadu-importance")
                End If
                If .Tripura Then
                    AddAndLogNewParamWithAYesValue("tripura")
                    AddEmptyParam("tripura-importance")
                End If
                If .Uttar Then
                    AddAndLogNewParamWithAYesValue("uttar")
                    AddEmptyParam("uttar-importance")
                End If
                If .Uttarakand Then
                    AddAndLogNewParamWithAYesValue("uttarakand")
                    AddEmptyParam("uttarakand-importance")
                End If
                If .Chennai Then
                    AddAndLogNewParamWithAYesValue("chennai")
                    AddAndLogEmptyParam("chennai-importance")
                End If
                If .Mumbai Then
                    AddAndLogNewParamWithAYesValue("mumbai")
                    AddAndLogEmptyParam("mumbai-importance")
                End If
            End With
        End Sub
        Protected Overrides Function TemplateFound() As Boolean
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