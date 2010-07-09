'Copyright © 2008 Stephen Kennedy (Kingboyk) http://www.sdk-software.com/
'Copyright © 2008 Sam Reed (Reedy) http://www.reedyboy.net/

'This program is free software; you can redistribute it and/or modify it under the terms of Version 2 of the GNU General Public License as published by the Free Software Foundation.

'This program is distributed in the hope that it will be useful,
'but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

'You should have received a copy of the GNU General Public License Version 2 along with this program; if not, write to the Free Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA

Namespace AutoWikiBrowser.Plugins.Kingbotk.Plugins
    Friend NotInheritable Class WPAustraliaSettings
        Implements IGenericSettings

        Private Const conSportParm As String = "AusSport"
        Private Const conPoliticsParm As String = "AusPolitics"
        Private Const conPlaceParm As String = "AusPlace"
        Private Const conMilitaryParm As String = "AusMilitary"
        Private Const conLawParm As String = "AusLaw"
        Private Const conCrimeParm As String = "AusCrime"
        Private Const conMusicParm As String = "AusMusic"
        Private Const conV8Parm As String = "AusV8"
        Private Const conNRLParm As String = "AusNRL"
        Private Const conNBLParm As String = "AusNBL"
        Private Const conALeagueParm As String = "AusALeague"
        Private Const conAFLParm As String = "AusAFL"
        Private Const conSydneyParm As String = "AusSydney"
        Private Const conPerthParm As String = "AusPerth"
        Private Const conMelbourneParm As String = "AusMelb"
        Private Const conLakeMacquarieParm As String = "AusLakeM"
        Private Const conHobartParm As String = "AusHobart"
        Private Const conGeelongParm As String = "AusGee"
        Private Const conCanberraParm As String = "AusCanb"
        Private Const conBrisbaneParm As String = "AusBris"
        Private Const conAdelaideParm As String = "AusAdel"
        Private Const conAutoStubParm As String = "AusAutoStub"
        Private Const conStubClassParm As String = "AusStubClass"

#Region "XML interface:"
        Friend Sub ReadXML(ByVal Reader As System.Xml.XmlTextReader) Implements IGenericSettings.ReadXML
            Sports = PluginManager.XMLReadBoolean(Reader, conSportParm, Sports)
            Politics = PluginManager.XMLReadBoolean(Reader, conPoliticsParm, Politics)
            Place = PluginManager.XMLReadBoolean(Reader, conPlaceParm, Place)
            Military = PluginManager.XMLReadBoolean(Reader, conMilitaryParm, Military)
            Law = PluginManager.XMLReadBoolean(Reader, conLawParm, Law)
            Music = PluginManager.XMLReadBoolean(Reader, conMusicParm, Music)
            Crime = PluginManager.XMLReadBoolean(Reader, conCrimeParm, Crime)
            V8 = PluginManager.XMLReadBoolean(Reader, conV8Parm, V8)
            NRL = PluginManager.XMLReadBoolean(Reader, conNRLParm, NRL)
            NBL = PluginManager.XMLReadBoolean(Reader, conNBLParm, NBL)
            ALeague = PluginManager.XMLReadBoolean(Reader, conALeagueParm, ALeague)
            AFL = PluginManager.XMLReadBoolean(Reader, conAFLParm, AFL)
            Sydney = PluginManager.XMLReadBoolean(Reader, conSydneyParm, Sydney)
            Perth = PluginManager.XMLReadBoolean(Reader, conPerthParm, Perth)
            Melbourne = PluginManager.XMLReadBoolean(Reader, conMelbourneParm, Melbourne)
            LakeMacquarie = PluginManager.XMLReadBoolean(Reader, conLakeMacquarieParm, LakeMacquarie)
            Hobart = PluginManager.XMLReadBoolean(Reader, conHobartParm, Hobart)
            Geelong = PluginManager.XMLReadBoolean(Reader, conGeelongParm, Geelong)
            Canberra = PluginManager.XMLReadBoolean(Reader, conCanberraParm, Canberra)
            Brisbane = PluginManager.XMLReadBoolean(Reader, conBrisbaneParm, Brisbane)
            Adelaide = PluginManager.XMLReadBoolean(Reader, conAdelaideParm, Adelaide)
            AutoStub = PluginManager.XMLReadBoolean(Reader, conAutoStubParm, AutoStub)
            StubClass = PluginManager.XMLReadBoolean(Reader, conStubClassParm, StubClass)
        End Sub
        Friend Sub WriteXML(ByVal Writer As System.Xml.XmlTextWriter) Implements IGenericSettings.WriteXML
            With Writer
                .WriteAttributeString(conSportParm, Sports.ToString)
                .WriteAttributeString(conPoliticsParm, Politics.ToString)
                .WriteAttributeString(conPlaceParm, Place.ToString)
                .WriteAttributeString(conMilitaryParm, Military.ToString)
                .WriteAttributeString(conLawParm, Law.ToString)
                .WriteAttributeString(conMusicParm, Music.ToString())
                .WriteAttributeString(conCrimeParm, Crime.ToString)
                .WriteAttributeString(conV8Parm, V8.ToString)
                .WriteAttributeString(conNRLParm, NRL.ToString)
                .WriteAttributeString(conNBLParm, NBL.ToString)
                .WriteAttributeString(conALeagueParm, ALeague.ToString)
                .WriteAttributeString(conAFLParm, AFL.ToString)
                .WriteAttributeString(conSydneyParm, Sydney.ToString)
                .WriteAttributeString(conPerthParm, Perth.ToString)
                .WriteAttributeString(conMelbourneParm, Melbourne.ToString)
                .WriteAttributeString(conLakeMacquarieParm, LakeMacquarie.ToString)
                .WriteAttributeString(conHobartParm, Hobart.ToString)
                .WriteAttributeString(conGeelongParm, Geelong.ToString)
                .WriteAttributeString(conCanberraParm, Canberra.ToString)
                .WriteAttributeString(conBrisbaneParm, Brisbane.ToString)
                .WriteAttributeString(conAdelaideParm, Adelaide.ToString)
                .WriteAttributeString(conAutoStubParm, AutoStub.ToString)
                .WriteAttributeString(conStubClassParm, StubClass.ToString)
            End With
        End Sub
        Friend Sub Reset() Implements IGenericSettings.XMLReset
            AutoStub = False
            StubClass = False

            For Each ctl As Control In Me.TopicsGroupBox.Controls
                Dim chk As CheckBox = CType(ctl, CheckBox)
                If chk IsNot Nothing Then
                    chk.Checked = False
                End If
            Next
        End Sub
#End Region

        ' Properties:
        Friend Property Sports() As Boolean
            Get
                Return SportCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                SportCheckBox.Checked = value
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
        Friend Property Place() As Boolean
            Get
                Return PlaceCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                PlaceCheckBox.Checked = value
            End Set
        End Property
        Friend Property Music() As Boolean
            Get
                Return MusicCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                MusicCheckBox.Checked = value
            End Set
        End Property
        Friend Property Military() As Boolean
            Get
                Return MilitaryCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                MilitaryCheckBox.Checked = value
            End Set
        End Property
        Friend Property Law() As Boolean
            Get
                Return LawCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                LawCheckBox.Checked = value
            End Set
        End Property
        Friend Property Crime() As Boolean
            Get
                Return CrimeCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                CrimeCheckBox.Checked = value
            End Set
        End Property
        Friend Property V8() As Boolean
            Get
                Return V8CheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                V8CheckBox.Checked = value
            End Set
        End Property
        Friend Property NBL() As Boolean
            Get
                Return NBLCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                NBLCheckBox.Checked = value
            End Set
        End Property
        Friend Property NRL() As Boolean
            Get
                Return NRLCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                NRLCheckBox.Checked = value
            End Set
        End Property
        Friend Property ALeague() As Boolean
            Get
                Return ALeagueCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                ALeagueCheckBox.Checked = value
            End Set
        End Property
        Friend Property AFL() As Boolean
            Get
                Return AFLCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                AFLCheckBox.Checked = value
            End Set
        End Property
        Friend Property Sydney() As Boolean
            Get
                Return SydneyCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                SydneyCheckBox.Checked = value
            End Set
        End Property
        Friend Property Perth() As Boolean
            Get
                Return PerthCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                PerthCheckBox.Checked = value
            End Set
        End Property
        Friend Property Melbourne() As Boolean
            Get
                Return MelbourneCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                MelbourneCheckBox.Checked = value
            End Set
        End Property
        Friend Property LakeMacquarie() As Boolean
            Get
                Return LakeMacquarieCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                LakeMacquarieCheckBox.Checked = value
            End Set
        End Property
        Friend Property Hobart() As Boolean
            Get
                Return HobartCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                HobartCheckBox.Checked = value
            End Set
        End Property
        Friend Property Geelong() As Boolean
            Get
                Return GeelongCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                GeelongCheckBox.Checked = value
            End Set
        End Property
        Friend Property Canberra() As Boolean
            Get
                Return CanberraCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                CanberraCheckBox.Checked = value
            End Set
        End Property
        Friend Property Brisbane() As Boolean
            Get
                Return BrisbaneCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                BrisbaneCheckBox.Checked = value
            End Set
        End Property
        Friend Property Adelaide() As Boolean
            Get
                Return AdelaideCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                AdelaideCheckBox.Checked = value
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
            Tools.OpenENArticleInBrowser("Template:WikiProject_Australia", False)
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

    Friend NotInheritable Class WPAustralia
        Inherits PluginBase

        ' Settings:
        Private OurTab As New TabPage("Australia")
        Private WithEvents OurSettingsControl As New WPAustraliaSettings
        Private Const conEnabled As String = "AusEnabled"

        Protected Friend Overrides ReadOnly Property PluginShortName() As String
            Get
                Return "Australia"
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
                Return "cat"
            End Get
        End Property
        Protected Overrides ReadOnly Property TemplateTalkClassParm() As String
            Get
                Return "NA"
            End Get
        End Property

        Protected Overrides ReadOnly Property PreferredTemplateName() As String
            Get
                Return "WikiProject Australia"
            End Get
        End Property
        'Protected Overrides ReadOnly Property PreferredTemplateNameRegexString() As String
        '    Get
        '        Return "^[Ww]P Australia$"
        '    End Get
        'End Property

        ' Initialisation:
        Friend Sub New()
            MyBase.New("") ' Specify alternate names only
        End Sub
        Protected Friend Overrides Sub Initialise()
            OurMenuItem = New ToolStripMenuItem("Australia Plugin")
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
                If .Adelaide Then AddAndLogNewParamWithAYesValue("Adelaide")
                If .Brisbane Then AddAndLogNewParamWithAYesValue("Brisbane")
                If .Geelong Then AddAndLogNewParamWithAYesValue("Geelong")
                If .Hobart Then AddAndLogNewParamWithAYesValue("Hobart")
                If .LakeMacquarie Then AddAndLogNewParamWithAYesValue("Macquarie")
                If .Melbourne Then AddAndLogNewParamWithAYesValue("Melbourne")
                If .Perth Then AddAndLogNewParamWithAYesValue("Perth")
                If .Sydney Then AddAndLogNewParamWithAYesValue("Sydney")
                If .Sports Then AddAndLogNewParamWithAYesValue("sports")
                If .AFL Then AddAndLogNewParamWithAYesValue("afl")
                If .ALeague Then AddAndLogNewParamWithAYesValue("aleague")
                If .NBL Then AddAndLogNewParamWithAYesValue("nbl", "NBL")
                If .NRL Then AddAndLogNewParamWithAYesValue("nrl")
                If .V8 Then AddAndLogNewParamWithAYesValue("V8", "v8")
                If .Crime Then AddAndLogNewParamWithAYesValue("crime")
                If .Law Then AddAndLogNewParamWithAYesValue("law")
                If .Military Then AddAndLogNewParamWithAYesValue("military")
                If .Place Then AddAndLogNewParamWithAYesValue("place")
                If .Politics Then AddAndLogNewParamWithAYesValue("politics")
                If .Music Then AddAndLogNewParamWithAYesValue("music")
            End With
        End Sub
        Protected Overrides Function TemplateFound() As Boolean
            If CheckForDoublyNamedParameters("V8", "v8") Then Return True ' tag is bad, exit
            If CheckForDoublyNamedParameters("nbl", "NBL") Then Return True
        End Function
        Protected Overrides Sub GotTemplateNotPreferredName(ByVal TemplateName As String)
        End Sub
        Protected Overrides Function WriteTemplateHeader(ByRef PutTemplateAtTop As Boolean) As String
            WriteTemplateHeader = "{{WikiProject Australia" & _
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
        Friend Overrides ReadOnly Property HasReqPhotoParam() As Boolean
            Get
                Return False
            End Get
        End Property
        Friend Overrides Sub ReqPhoto()
        End Sub
    End Class
End Namespace