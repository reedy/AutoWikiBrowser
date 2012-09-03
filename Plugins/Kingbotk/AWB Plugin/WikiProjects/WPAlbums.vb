Friend NotInheritable Class WPAlbums
    Inherits PluginBase

    Private Const PluginName As String = "WikiProject Albums"
    Const Prefix As String = "Albums"

    Friend Sub New()
        MyBase.New("Album|Albums|WP Albums|WPAlbums") ' Specify alternate names only

        Dim params(-1) As TemplateParameters

        OurSettingsControl = New GenericWithWorkgroups("WikiProject Albums", Prefix, True, params)
    End Sub

    ' Settings:
    Private ReadOnly OurTab As New TabPage("Albums")
    Private WithEvents OurSettingsControl As GenericWithWorkgroups

    Protected Friend Overrides ReadOnly Property PluginShortName() As String
        Get
            Return "Albums"
        End Get
    End Property
    Protected Overrides ReadOnly Property PreferredTemplateName() As String
        Get
            Return PluginName
        End Get
    End Property

    Protected Overrides Sub ImportanceParameter(ByVal Importance As Importance)
        Template.NewOrReplaceTemplateParm("importance", Importance.ToString, article, False, False)
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
    Protected Overrides ReadOnly Property FileTalkClassParm() As String
        Get
            Return "File"
        End Get
    End Property

    ' Initialisation:
    Protected Friend Overrides Sub Initialise()
        OurMenuItem = New ToolStripMenuItem("Albums Plugin")
        InitialiseBase() ' must set menu item object first
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

    Protected Overrides Function WriteTemplateHeader() As String
        Return "{{album" & WriteOutParameterToHeader("class") & _
           WriteOutParameterToHeader("importance")
    End Function

    'User interface:
    Protected Overrides Sub ShowHideOurObjects(ByVal Visible As Boolean)
        PluginManager.ShowHidePluginTab(OurTab, Visible)
    End Sub

    ' XML settings:
    Protected Friend Overrides Sub ReadXML(ByVal Reader As XmlTextReader)
        Dim blnNewVal As Boolean = PluginManager.XMLReadBoolean(Reader, Prefix & "Enabled", Enabled)
        If Not blnNewVal = Enabled Then Enabled = blnNewVal ' Mustn't set if the same or we get extra tabs
        OurSettingsControl.ReadXML(Reader)
    End Sub
    Protected Friend Overrides Sub Reset()
        OurSettingsControl.Reset()
    End Sub
    Protected Friend Overrides Sub WriteXML(ByVal Writer As XmlTextWriter)
        Writer.WriteAttributeString(Prefix & "Enabled", Enabled.ToString)
        OurSettingsControl.WriteXML(Writer)
    End Sub

    ' Misc:
    Friend Overrides ReadOnly Property HasReqPhotoParam() As Boolean
        Get
            Return False
        End Get
    End Property
    Friend Overrides Sub ReqPhoto()
    End Sub
End Class