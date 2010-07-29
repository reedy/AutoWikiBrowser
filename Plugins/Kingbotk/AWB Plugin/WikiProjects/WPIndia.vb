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
            Return "WikiProject India"
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
    Friend Sub New()
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
        WriteTemplateHeader = "{{WikiProject India India" & _
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
End Class