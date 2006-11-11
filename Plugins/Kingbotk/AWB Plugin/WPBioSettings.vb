'Namespace AWB.Plugins.SDKSoftware.Kingbotk.Components

'* WPBio-specific options:
' insert field button or list for the not likely to be autotagged params -- use ContextMenuStrip
' leave some room for additions!
Friend Class WPBioSettings
    Implements XMLSettings
    Private Const conLivingParm As String = "LivingPerson"
    Private Const conAutoStubParm As String = "AutoStub"
    Private Const conStubClassParm As String = "StubClass"
    Private Const conActivePolParm As String = "ActivePol"
    Private Const conArtsEntsWGParm As String = "ArtsEntsWG"
    Private Const conMilitaryWGParm As String = "MilitaryWG"
    Private Const conRoyaltyWGParm As String = "RoyaltyWG"
    Private Const conPoliticianWGParm As String = "PoliticianWG"
    Private Const conCategoryTalkParm As String = "CategoryTalk"
    Private Const conForcePriorityParm As String = "ForcePriorityParm"
    Private Const conCategoryNameParm As String = "CategoryName"

    Private Sub ActivePoliticianCheckBox_CheckedChanged(ByVal sender As System.Object, _
    ByVal e As System.EventArgs) Handles ActivePoliticianCheckBox.CheckedChanged
        If ActivePoliticianCheckBox.Checked Then PoliticianCheckBox.Checked = True
    End Sub
    Private Sub AutoStubCheckBox_CheckedChanged(ByVal sender As System.Object, _
    ByVal e As System.EventArgs) Handles AutoStubCheckBox.CheckedChanged
        If AutoStubCheckBox.Checked Then StubClassCheckBox.Checked = False
    End Sub
    Private Sub StubClassCheckBox_CheckedChanged(ByVal sender As System.Object, _
    ByVal e As System.EventArgs) Handles StubClassCheckBox.CheckedChanged
        If StubClassCheckBox.Checked Then AutoStubCheckBox.Checked = False
    End Sub
    Private Sub CategoryTextBox_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles CategoryTextBox.TextChanged

    End Sub
    Public Sub ReadXML(ByVal Reader As System.Xml.XmlTextReader) Implements XMLSettings.ReadXML
        Living = XMLReadBoolean(Reader, conLivingParm)
        AutoStub = XMLReadBoolean(Reader, conAutoStubParm)
        StubClass = XMLReadBoolean(Reader, conStubClassParm)
        ActivePol = XMLReadBoolean(Reader, conActivePolParm)
        ArtsEntsWG = XMLReadBoolean(Reader, conArtsEntsWGParm)
        MilitaryWG = XMLReadBoolean(Reader, conMilitaryWGParm)
        RoyaltyWG = XMLReadBoolean(Reader, conRoyaltyWGParm)
        PoliticianWG = XMLReadBoolean(Reader, conPoliticianWGParm)
        CategoryTalk = XMLReadBoolean(Reader, conCategoryTalkParm)
        ForcePriorityParm = XMLReadBoolean(Reader, conForcePriorityParm)
        CategoryName = XMLReadString(Reader, conCategoryNameParm)
    End Sub
    Public Sub WriteXML(ByVal Writer As System.Xml.XmlTextWriter) Implements XMLSettings.WriteXML
        With Writer
            .WriteAttributeString(conLivingParm, Living.ToString)
            .WriteAttributeString(conAutoStubParm, AutoStub.ToString)
            .WriteAttributeString(conStubClassParm, StubClass.ToString)
            .WriteAttributeString(conActivePolParm, ActivePol.ToString)
            .WriteAttributeString(conArtsEntsWGParm, ArtsEntsWG.ToString)
            .WriteAttributeString(conMilitaryWGParm, MilitaryWG.ToString)
            .WriteAttributeString(conRoyaltyWGParm, RoyaltyWG.ToString)
            .WriteAttributeString(conPoliticianWGParm, PoliticianWG.ToString)
            .WriteAttributeString(conCategoryTalkParm, CategoryTalk.ToString)
            .WriteAttributeString(conForcePriorityParm, ForcePriorityParm.ToString)
            .WriteAttributeString(conCategoryNameParm, CategoryName)
        End With
    End Sub
    Public Sub Reset() Implements XMLSettings.Reset
        Living = False
        AutoStub = False
        StubClass = False
        ActivePol = False
        ArtsEntsWG = False
        MilitaryWG = False
        RoyaltyWG = False
        PoliticianWG = False
        CategoryTalk = True
        ForcePriorityParm = False
        CategoryName = ""
    End Sub
    Public Property Living() As Boolean
        Get
            Return LivingCheckBox.Checked
        End Get
        Set(ByVal value As Boolean)
            LivingCheckBox.Checked = value
        End Set
    End Property
    Public Property StubClass() As Boolean
        Get
            Return StubClassCheckBox.Checked
        End Get
        Set(ByVal value As Boolean)
            StubClassCheckBox.Checked = value
        End Set
    End Property
    Public Property ActivePol() As Boolean
        Get
            Return ActivePoliticianCheckBox.Checked
        End Get
        Set(ByVal value As Boolean)
            ActivePoliticianCheckBox.Checked = value
        End Set
    End Property
    Public Property ArtsEntsWG() As Boolean
        Get
            Return ArtsEntsCheckBox.Checked
        End Get
        Set(ByVal value As Boolean)
            ArtsEntsCheckBox.Checked = value
        End Set
    End Property
    Public Property MilitaryWG() As Boolean
        Get
            Return MilitaryCheckBox.Checked
        End Get
        Set(ByVal value As Boolean)
            MilitaryCheckBox.Checked = value
        End Set
    End Property
    Public Property RoyaltyWG() As Boolean
        Get
            Return RoyaltyCheckBox.Checked
        End Get
        Set(ByVal value As Boolean)
            RoyaltyCheckBox.Checked = value
        End Set
    End Property
    Public Property PoliticianWG() As Boolean
        Get
            Return PoliticianCheckBox.Checked
        End Get
        Set(ByVal value As Boolean)
            PoliticianCheckBox.Checked = value
        End Set
    End Property
    Public Property CategoryTalk() As Boolean
        Get
            Return CategoryTalkCheckBox.Checked
        End Get
        Set(ByVal value As Boolean)
            CategoryTalkCheckBox.Checked = value
        End Set
    End Property
    Public Property ForcePriorityParm() As Boolean
        Get
            Return ForcePriorityParmCheckBox.Checked
        End Get
        Set(ByVal value As Boolean)
            ForcePriorityParmCheckBox.Checked = value
        End Set
    End Property
    Public Property AutoStub() As Boolean
        Get
            Return AutoStubCheckBox.Checked
        End Get
        Set(ByVal value As Boolean)
            AutoStubCheckBox.Checked = value
        End Set
    End Property
    Public Property CategoryName() As String
        Get
            Return CategoryTextBox.Text
        End Get
        Set(ByVal value As String)
            CategoryTextBox.Text = value
        End Set
    End Property

    'Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
    '    SDKAWBTemplatesPluginBase.HideTabs()
    'End Sub

    Private Sub LinkLabel1_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        System.Diagnostics.Process.Start("http://en.wikipedia.org/wiki/Template:WPBiography")
    End Sub
End Class
'End Namespace