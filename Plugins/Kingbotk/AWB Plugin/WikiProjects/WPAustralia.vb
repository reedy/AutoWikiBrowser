Friend NotInheritable Class WPAustralia
    Inherits PluginBase

    Friend Sub New()
        MyBase.New("") ' Specify alternate names only

        OurSettingsControl = New GenericWithWorkgroups(PluginName, Prefix, True, params)
    End Sub

    Const Prefix As String = "Aus"
    Const PluginName As String = "WikiProject Australia"

    Const PlacesGroup As String = "Places"
    Const SportsGroup As String = "Sports"
    Const OtherGroup As String = "Other topics"

    Dim params() As TemplateParameters =
    {
       New TemplateParameters() With {.StorageKey = "Place", .Group = PlacesGroup, .ParamName = "place"}, _
       New TemplateParameters() With {.StorageKey = "Adel", .Group = PlacesGroup, .ParamName = "Adelaide"}, _
       New TemplateParameters() With {.StorageKey = "Bris", .Group = PlacesGroup, .ParamName = "Brisbane"}, _
       New TemplateParameters() With {.StorageKey = "Canb", .Group = PlacesGroup, .ParamName = "Canberra"}, _
       New TemplateParameters() With {.StorageKey = "Gee", .Group = PlacesGroup, .ParamName = "Geelong"}, _
       New TemplateParameters() With {.StorageKey = "Melb", .Group = PlacesGroup, .ParamName = "Melbourne"}, _
       New TemplateParameters() With {.StorageKey = "Perth", .Group = PlacesGroup, .ParamName = "Perth"}, _
       New TemplateParameters() With {.StorageKey = "Sydney", .Group = PlacesGroup, .ParamName = "Sydney"}, _
       New TemplateParameters() With {.StorageKey = "Sport", .Group = SportsGroup, .ParamName = "Sport"}, _
       New TemplateParameters() With {.StorageKey = "AFL", .Group = SportsGroup, .ParamName = "AFL"}, _
       New TemplateParameters() With {.StorageKey = "NBL", .Group = SportsGroup, .ParamName = "NBL"}, _
       New TemplateParameters() With {.StorageKey = "NRL", .Group = SportsGroup, .ParamName = "NRL"}, _
       New TemplateParameters() With {.StorageKey = "V8", .Group = SportsGroup, .ParamName = "V8"}, _
       New TemplateParameters() With {.StorageKey = "Crime", .Group = OtherGroup, .ParamName = "Crime"}, _
       New TemplateParameters() With {.StorageKey = "Law", .Group = OtherGroup, .ParamName = "Law"}, _
       New TemplateParameters() With {.StorageKey = "Military", .Group = OtherGroup, .ParamName = "Military"}, _
       New TemplateParameters() With {.StorageKey = "Music", .Group = OtherGroup, .ParamName = "Music"}, _
       New TemplateParameters() With {.StorageKey = "Politics", .Group = OtherGroup, .ParamName = "Politics"}
    }

    ' Settings:
    Private OurTab As New TabPage("Australia")
    Private OurSettingsControl As GenericWithWorkgroups

    Protected Friend Overrides ReadOnly Property PluginShortName() As String
        Get
            Return "Australia"
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
            Return PluginName
        End Get
    End Property
    'Protected Overrides ReadOnly Property PreferredTemplateNameRegexString() As String
    '    Get
    '        Return "^[Ww]P Australia$"
    '    End Get
    'End Property

    ' Initialisation:

    Protected Friend Overrides Sub Initialise()
        OurMenuItem = New ToolStripMenuItem("Australia Plugin")
        MyBase.InitialiseBase() ' must set menu item object first
        OurTab.UseVisualStyleBackColor = True
        OurTab.Controls.Add(OurSettingsControl)
    End Sub

    ' Article processing:
    Protected Overrides Function SkipIfContains() As Boolean
        ' None
    End Function
    Protected Overrides Sub ProcessArticleFinish()
        StubClass()
        With OurSettingsControl
            For Each lvi As ListViewItem In .ListView1.Items
                If lvi.Checked Then
                    Dim tp As TemplateParameters = DirectCast(lvi.Tag, TemplateParameters)
                    AddAndLogNewParamWithAYesValue(tp.ParamName.ToLower()) 'Probably needs some reformatting
                End If
            Next
        End With
    End Sub
    Protected Overrides Function TemplateFound() As Boolean
        If CheckForDoublyNamedParameters("V8", "v8") Then Return True ' tag is bad, exit
        If CheckForDoublyNamedParameters("nbl", "NBL") Then Return True
    End Function
    Protected Overrides Sub GotTemplateNotPreferredName(ByVal TemplateName As String)
    End Sub
    Protected Overrides Function WriteTemplateHeader() As String
        Return "{{" & PluginName & _
           Microsoft.VisualBasic.vbCrLf & WriteOutParameterToHeader("class") & _
           WriteOutParameterToHeader("importance")
    End Function

    'User interface:
    Protected Overrides Sub ShowHideOurObjects(ByVal Visible As Boolean)
        PluginManager.ShowHidePluginTab(OurTab, Visible)
    End Sub

    ' XML settings:
    Protected Friend Overrides Sub ReadXML(ByVal Reader As System.Xml.XmlTextReader)
        Dim blnNewVal As Boolean = PluginManager.XMLReadBoolean(Reader, Prefix & "Enabled", Enabled)
        If Not blnNewVal = Enabled Then Enabled = blnNewVal ' Mustn't set if the same or we get extra tabs
        OurSettingsControl.ReadXML(Reader)
    End Sub
    Protected Friend Overrides Sub Reset()
        OurSettingsControl.Reset()
    End Sub
    Protected Friend Overrides Sub WriteXML(ByVal Writer As System.Xml.XmlTextWriter)
        Writer.WriteAttributeString(Prefix & "Enabled", Enabled.ToString)
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