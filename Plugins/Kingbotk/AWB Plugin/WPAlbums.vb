Friend NotInheritable Class WPAlbums
    Inherits PluginBase

    Friend Sub New()
        MyBase.New("Albums") ' Specify alternate names only

        Dim params(-1) As TemplateParameters

        OurSettingsControl = New GenericWithWorkgroups("WikiProject Albums", "Album", True, False, params)
    End Sub

    ' Settings:
    Private OurTab As New TabPage("Albums")
    Private WithEvents OurSettingsControl As GenericWithWorkgroups
    Private Const conEnabled As String = "AlbumEnabled"
    Protected Friend Overrides ReadOnly Property PluginShortName() As String
        Get
            Return "Albums"
        End Get
    End Property
    Protected Overrides ReadOnly Property PreferredTemplateName() As String
        Get
            Return "Album"
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
            Return "NA"
        End Get
    End Property
    Protected Overrides ReadOnly Property TemplateTalkClassParm() As String
        Get
            Return "NA"
        End Get
    End Property
    'Protected Overrides ReadOnly Property PreferredTemplateNameRegexString() As String
    '    Get
    '        Return "^[Aa]lbum$"
    '    End Get
    'End Property

    ' Initialisation:
    Protected Friend Overrides Sub Initialise()
        OurMenuItem = New ToolStripMenuItem("Albums Plugin")
        MyBase.InitialiseBase() ' must set menu item object first
        OurTab.UseVisualStyleBackColor = True
        OurTab.Controls.Add(OurSettingsControl)
    End Sub

    ' Article processing:
    Protected Overrides Function SkipIfContains() As Boolean
        Return False
    End Function
    Protected Overrides Sub ProcessArticleFinish()
        StubClass()
    End Sub
    Protected Overrides Function TemplateFound() As Boolean
        ' Nothing to do here
    End Function
    Protected Overrides Sub GotTemplateNotPreferredName(ByVal TemplateName As String)
        ' Only WPBio used to do something here (if {{musician}} add to musician-work-group)
    End Sub
    Protected Overrides Function WriteTemplateHeader(ByRef PutTemplateAtTop As Boolean) As String
        WriteTemplateHeader = "{{album" & WriteOutParameterToHeader("class") & _
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

    ' Misc:
    Protected Overrides ReadOnly Property InspectUnsetParameters() As Boolean
        Get
            Return False
        End Get
    End Property
    Protected Overrides Sub InspectUnsetParameter(ByVal Param As String)
    End Sub ' will never be called
    Friend Overrides ReadOnly Property HasReqPhotoParam() As Boolean
        Get
            Return False
        End Get
    End Property
    Friend Overrides Sub ReqPhoto()
    End Sub
End Class