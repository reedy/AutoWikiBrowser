Friend NotInheritable Class WPIndia
    Inherits PluginBase

    ' Settings:
    Private OurTab As New TabPage("India")
    Private OurSettingsControl As GenericWithWorkgroups

    Private Const Prefix As String = "Ind"
    Const PluginName As String = "WikiProject India"

    Const GeographyGroup As String = "Geography"
    Const OthersGroup As String = "Others"

    Dim params() As TemplateParameters =
    {
           New TemplateParameters() With {.StorageKey = "Geography", .Group = GeographyGroup, .ParamName = "Geography"}, _
           New TemplateParameters() With {.StorageKey = "Maps", .Group = GeographyGroup, .ParamName = "Maps"}, _
           New TemplateParameters() With {.StorageKey = "Cities", .Group = GeographyGroup, .ParamName = "Cities"}, _
           New TemplateParameters() With {.StorageKey = "Chennai", .Group = GeographyGroup, .ParamName = "Chennai"}, _
           New TemplateParameters() With {.StorageKey = "Mumbai", .Group = GeographyGroup, .ParamName = "Mumbai"}, _
           New TemplateParameters() With {.StorageKey = "Hyderabad", .Group = GeographyGroup, .ParamName = "Hyderabad"}, _
           New TemplateParameters() With {.StorageKey = "Districts", .Group = GeographyGroup, .ParamName = "Districts"}, _
           New TemplateParameters() With {.StorageKey = "States", .Group = GeographyGroup, .ParamName = "States"}, _
           New TemplateParameters() With {.StorageKey = "Andhra", .Group = GeographyGroup, .ParamName = "Andhra Pradesh"}, _
           New TemplateParameters() With {.StorageKey = "Arunachal", .Group = GeographyGroup, .ParamName = "Arunachal Pradesh"}, _
           New TemplateParameters() With {.StorageKey = "Assam", .Group = GeographyGroup, .ParamName = "Assam"}, _
           New TemplateParameters() With {.StorageKey = "Bengal", .Group = GeographyGroup, .ParamName = "Bengal"}, _
           New TemplateParameters() With {.StorageKey = "Bihar", .Group = GeographyGroup, .ParamName = "Bihar"}, _
           New TemplateParameters() With {.StorageKey = "Chhattisgarh", .Group = GeographyGroup, .ParamName = "Chhattisgarh"}, _
           New TemplateParameters() With {.StorageKey = "Goa", .Group = GeographyGroup, .ParamName = "Goa"}, _
           New TemplateParameters() With {.StorageKey = "Gujarat", .Group = GeographyGroup, .ParamName = "Gujarat"}, _
           New TemplateParameters() With {.StorageKey = "Haryana", .Group = GeographyGroup, .ParamName = "Haryana"}, _
           New TemplateParameters() With {.StorageKey = "Himachal", .Group = GeographyGroup, .ParamName = "Himachal Pradesh"}, _
           New TemplateParameters() With {.StorageKey = "JandK", .Group = GeographyGroup, .ParamName = "Jammu and Kashmir"}, _
           New TemplateParameters() With {.StorageKey = "Jharkhand", .Group = GeographyGroup, .ParamName = "Jharkhand"}, _
           New TemplateParameters() With {.StorageKey = "Karnataka", .Group = GeographyGroup, .ParamName = "Karnataka"}, _
           New TemplateParameters() With {.StorageKey = "Kerala", .Group = GeographyGroup, .ParamName = "Kerala"}, _
           New TemplateParameters() With {.StorageKey = "Madhya", .Group = GeographyGroup, .ParamName = "Madhya Pradesh"}, _
           New TemplateParameters() With {.StorageKey = "Maharashtra", .Group = GeographyGroup, .ParamName = "Maharashtra"}, _
           New TemplateParameters() With {.StorageKey = "Manipur", .Group = GeographyGroup, .ParamName = "Manipur"}, _
           New TemplateParameters() With {.StorageKey = "Meghalaya", .Group = GeographyGroup, .ParamName = "Meghalaya"}, _
           New TemplateParameters() With {.StorageKey = "Mizoram", .Group = GeographyGroup, .ParamName = "Mizoram"}, _
           New TemplateParameters() With {.StorageKey = "Nagaland", .Group = GeographyGroup, .ParamName = "Nagaland"}, _
           New TemplateParameters() With {.StorageKey = "Orissa", .Group = GeographyGroup, .ParamName = "Orissa"}, _
           New TemplateParameters() With {.StorageKey = "Punjab", .Group = GeographyGroup, .ParamName = "Punjab"}, _
           New TemplateParameters() With {.StorageKey = "Rajasthan", .Group = GeographyGroup, .ParamName = "Rajasthan"}, _
           New TemplateParameters() With {.StorageKey = "Sikkim", .Group = GeographyGroup, .ParamName = "Sikkim"}, _
           New TemplateParameters() With {.StorageKey = "Tamilnadu", .Group = GeographyGroup, .ParamName = "Tamil Nadu"}, _
           New TemplateParameters() With {.StorageKey = "Tripura", .Group = GeographyGroup, .ParamName = "Tripura"}, _
           New TemplateParameters() With {.StorageKey = "Uttar", .Group = GeographyGroup, .ParamName = "Uttar Pradesh"}, _
           New TemplateParameters() With {.StorageKey = "Uttarakand", .Group = GeographyGroup, .ParamName = "Uttarakand"}, _
           New TemplateParameters() With {.StorageKey = "Andaman", .Group = GeographyGroup, .ParamName = "Andaman and Nicobar Island"}, _
           New TemplateParameters() With {.StorageKey = "Chandigarh", .Group = GeographyGroup, .ParamName = "Chandigarh"}, _
           New TemplateParameters() With {.StorageKey = "Dadra", .Group = GeographyGroup, .ParamName = "Dadra and Nagar Haveli"}, _
           New TemplateParameters() With {.StorageKey = "Daman", .Group = GeographyGroup, .ParamName = "Daman and Diu"}, _
           New TemplateParameters() With {.StorageKey = "Delhi", .Group = GeographyGroup, .ParamName = "Delhi"}, _
           New TemplateParameters() With {.StorageKey = "Lakshadweep", .Group = GeographyGroup, .ParamName = "Lakshadweep"}, _
           New TemplateParameters() With {.StorageKey = "Puducherry", .Group = GeographyGroup, .ParamName = "Puducherry"}, _
           New TemplateParameters() With {.StorageKey = "Cinema", .Group = OthersGroup, .ParamName = "Cinema"}, _
           New TemplateParameters() With {.StorageKey = "History", .Group = OthersGroup, .ParamName = "History"}, _
           New TemplateParameters() With {.StorageKey = "Literature", .Group = OthersGroup, .ParamName = "Literature"}, _
           New TemplateParameters() With {.StorageKey = "Politics", .Group = OthersGroup, .ParamName = "Politics"}, _
           New TemplateParameters() With {.StorageKey = "ProtectedAreas", .Group = OthersGroup, .ParamName = "Protected Areas"}, _
           New TemplateParameters() With {.StorageKey = "Tamil", .Group = OthersGroup, .ParamName = "Tamil"}, _
           New TemplateParameters() With {.StorageKey = "Tele", .Group = OthersGroup, .ParamName = "Television"}
       }

    Friend Sub New()
        MyBase.New("") ' Specify alternate names only

        OurSettingsControl = New GenericWithWorkgroups(PluginName, Prefix, True, params)
    End Sub

    Protected Friend Overrides ReadOnly Property PluginShortName() As String
        Get
            Return "India"
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
            Return PluginName
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
    Protected Friend Overrides Sub Initialise()
        OurMenuItem = New ToolStripMenuItem("India Plugin")
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
                    Dim param As String = tp.ParamName.ToLower().Replace(" ", "-")
                    AddAndLogNewParamWithAYesValue(param)
                    AddEmptyParam(param & "-importance")
                End If
            Next
        End With
    End Sub
    Protected Overrides Function TemplateFound() As Boolean
    End Function
    Protected Overrides Sub GotTemplateNotPreferredName(ByVal TemplateName As String)
    End Sub
    Protected Overrides Function WriteTemplateHeader(ByRef PutTemplateAtTop As Boolean) As String
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
End Class