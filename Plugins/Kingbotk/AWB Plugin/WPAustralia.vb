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
    Friend Overrides ReadOnly Property HasReqPhotoParam() As Boolean
        Get
            Return False
        End Get
    End Property
    Friend Overrides Sub ReqPhoto()
    End Sub
End Class