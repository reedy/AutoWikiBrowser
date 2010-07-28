Friend NotInheritable Class WPMilitaryHistory
    Inherits PluginBase

    Private Const PluginName As String = "MilHist"
    Private Const TemplateName As String = "WPMILHIST"

    ' Initialisation:
    Friend Sub New()
        MyBase.New("WikiProject Military History") ' Specify alternate names only

        OurSettingsControl = New GenericWithWorkgroups(TemplateName, PluginName, False, True, params)
        OurSettingsControl.InspectUnsetText = "Remove importance"
    End Sub

    ' Settings:
    Private OurTab As New TabPage(PluginName)
    Private WithEvents OurSettingsControl As GenericWithWorkgroups

    Protected Friend Overrides ReadOnly Property PluginShortName() As String
        Get
            Return "MilitaryHistory"
        End Get
    End Property

    Protected Overrides ReadOnly Property PreferredTemplateName() As String
        Get
            Return TemplateName
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

    Friend Overrides ReadOnly Property HasReqPhotoParam() As Boolean
        Get
            Return False
        End Get
    End Property

    Friend Overrides Sub ReqPhoto()
    End Sub

    Const PeriodsAndConflictsGroup As String = "Periods and Conflicts"
    Const GeneralGroup As String = "General Task Forces"
    Const NationsGroup As String = "Nations and Regions"

    Dim params() As TemplateParameters =
    {
       New TemplateParameters() With {.StorageKey = "ACW", .Group = PeriodsAndConflictsGroup, .ParamName = "ACW"}, _
       New TemplateParameters() With {.StorageKey = "ARW", .Group = PeriodsAndConflictsGroup, .ParamName = "ARW"}, _
       New TemplateParameters() With {.StorageKey = "Classic", .Group = PeriodsAndConflictsGroup, .ParamName = "Classical"}, _
       New TemplateParameters() With {.StorageKey = "Crusades", .Group = PeriodsAndConflictsGroup, .ParamName = "Crusades"}, _
       New TemplateParameters() With {.StorageKey = "EarlyModern", .Group = PeriodsAndConflictsGroup, .ParamName = "Early-Modern"}, _
       New TemplateParameters() With {.StorageKey = "Medieval", .Group = PeriodsAndConflictsGroup, .ParamName = "Medieval"}, _
       New TemplateParameters() With {.StorageKey = "Muslim", .Group = PeriodsAndConflictsGroup, .ParamName = "Muslim"}, _
       New TemplateParameters() With {.StorageKey = "Napol", .Group = PeriodsAndConflictsGroup, .ParamName = "Napoleonic"}, _
       New TemplateParameters() With {.StorageKey = "WWI", .Group = PeriodsAndConflictsGroup, .ParamName = "WWI"}, _
       New TemplateParameters() With {.StorageKey = "WWII", .Group = PeriodsAndConflictsGroup, .ParamName = "WWII"}, _
       New TemplateParameters() With {.StorageKey = "Air", .Group = GeneralGroup, .ParamName = "Aviation"}, _
       New TemplateParameters() With {.StorageKey = "Biography", .Group = GeneralGroup, .ParamName = "Biography"}, _
       New TemplateParameters() With {.StorageKey = "Films", .Group = GeneralGroup, .ParamName = "Films"}, _
       New TemplateParameters() With {.StorageKey = "Fortifications", .Group = GeneralGroup, .ParamName = "Fortifications"}, _
       New TemplateParameters() With {.StorageKey = "Historiography", .Group = GeneralGroup, .ParamName = "Historiography"}, _
       New TemplateParameters() With {.StorageKey = "Intel", .Group = GeneralGroup, .ParamName = "Intel"}, _
       New TemplateParameters() With {.StorageKey = "LandVech", .Group = GeneralGroup, .ParamName = "Land Vehicles"}, _
       New TemplateParameters() With {.StorageKey = "Marit", .Group = GeneralGroup, .ParamName = "Martime"}, _
       New TemplateParameters() With {.StorageKey = "Memorial", .Group = GeneralGroup, .ParamName = "Memorials"}, _
       New TemplateParameters() With {.StorageKey = "National", .Group = GeneralGroup, .ParamName = "Nationals"}, _
       New TemplateParameters() With {.StorageKey = "Science", .Group = GeneralGroup, .ParamName = "Science"}, _
       New TemplateParameters() With {.StorageKey = "Tech", .Group = GeneralGroup, .ParamName = "Technology"}, _
       New TemplateParameters() With {.StorageKey = "Weapon", .Group = GeneralGroup, .ParamName = "Weaponry"}, _
       New TemplateParameters() With {.StorageKey = "NTF", .Group = NationsGroup, .ParamName = "No Task Force"}, _
       New TemplateParameters() With {.StorageKey = "African", .Group = NationsGroup, .ParamName = "Africa"}, _
       New TemplateParameters() With {.StorageKey = "Aus", .Group = NationsGroup, .ParamName = "Australia"}, _
       New TemplateParameters() With {.StorageKey = "Balkan", .Group = NationsGroup, .ParamName = "Balkan"}, _
       New TemplateParameters() With {.StorageKey = "Baltic", .Group = NationsGroup, .ParamName = "Baltic"}, _
       New TemplateParameters() With {.StorageKey = "Brit", .Group = NationsGroup, .ParamName = "British"}, _
       New TemplateParameters() With {.StorageKey = "Canuck", .Group = NationsGroup, .ParamName = "Canadian"}, _
       New TemplateParameters() With {.StorageKey = "China", .Group = NationsGroup, .ParamName = "Chinese"}, _
       New TemplateParameters() With {.StorageKey = "Dutch", .Group = NationsGroup, .ParamName = "Dutch"}, _
       New TemplateParameters() With {.StorageKey = "French", .Group = NationsGroup, .ParamName = "French"}, _
       New TemplateParameters() With {.StorageKey = "German", .Group = NationsGroup, .ParamName = "German"}, _
       New TemplateParameters() With {.StorageKey = "India", .Group = NationsGroup, .ParamName = "Indian"}, _
       New TemplateParameters() With {.StorageKey = "Italy", .Group = NationsGroup, .ParamName = "Italian"}, _
       New TemplateParameters() With {.StorageKey = "Japan", .Group = NationsGroup, .ParamName = "Japanese"}, _
       New TemplateParameters() With {.StorageKey = "Korean", .Group = NationsGroup, .ParamName = "Korean"}, _
       New TemplateParameters() With {.StorageKey = "Lebanese", .Group = NationsGroup, .ParamName = "Lebanese"}, _
       New TemplateParameters() With {.StorageKey = "MidEast", .Group = NationsGroup, .ParamName = "Middle-Eastern"}, _
       New TemplateParameters() With {.StorageKey = "NZ", .Group = NationsGroup, .ParamName = "New Zealand"}, _
       New TemplateParameters() With {.StorageKey = "Nordic", .Group = NationsGroup, .ParamName = "Nordic"}, _
       New TemplateParameters() With {.StorageKey = "Ottoman", .Group = NationsGroup, .ParamName = "Ottoman"}, _
       New TemplateParameters() With {.StorageKey = "Poland", .Group = NationsGroup, .ParamName = "Polish"}, _
       New TemplateParameters() With {.StorageKey = "Romanian", .Group = NationsGroup, .ParamName = "Romanian"}, _
       New TemplateParameters() With {.StorageKey = "Russian", .Group = NationsGroup, .ParamName = "Russian"}, _
       New TemplateParameters() With {.StorageKey = "Spanish", .Group = NationsGroup, .ParamName = "Spanish"}, _
       New TemplateParameters() With {.StorageKey = "SAmerican", .Group = NationsGroup, .ParamName = "S American"}, _
       New TemplateParameters() With {.StorageKey = "SEAsian", .Group = NationsGroup, .ParamName = "SE Asian"}, _
       New TemplateParameters() With {.StorageKey = "Taiwanese", .Group = NationsGroup, .ParamName = "Taiwanese"}, _
       New TemplateParameters() With {.StorageKey = "US", .Group = NationsGroup, .ParamName = "US"}
}

    Protected Friend Overrides Sub Initialise()
        OurMenuItem = New ToolStripMenuItem("Military History Plugin")
        MyBase.InitialiseBase() ' must set menu item object first
        OurTab.UseVisualStyleBackColor = True
        OurTab.Controls.Add(OurSettingsControl)
    End Sub

    ' Article processing:
    Protected Overrides ReadOnly Property InspectUnsetParameters() As Boolean
        Get
            Return OurSettingsControl.InspectUnsetParameters
        End Get
    End Property

    Protected Overrides Sub InspectUnsetParameter(ByVal Param As String)
        ' We only get called if InspectUnsetParameters is True
        If String.Equals(Param, "importance", StringComparison.CurrentCultureIgnoreCase) Then
            Article.DoneReplacement("importance=", "", True, PluginShortName)
        End If
    End Sub

    Protected Overrides Function SkipIfContains() As Boolean
    End Function

    Protected Overrides Sub ProcessArticleFinish()
        StubClass()
        With OurSettingsControl
            For Each lvi As ListViewItem In .ListView1.Items
                If lvi.Checked Then
                    Dim tp As TemplateParameters = DirectCast(lvi.Tag, TemplateParameters)
                    AddAndLogNewParamWithAYesValue(tp.ParamName.ToLower().Replace(" ", "-")) 'Probably needs some reformatting
                End If
            Next
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

    Private Const conMedievalTaskForce As String = "Medieval-task-force"
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
        Dim blnNewVal As Boolean = PluginManager.XMLReadBoolean(Reader, PluginName & "Enabled", Enabled)
        If Not blnNewVal = Enabled Then
            Enabled = blnNewVal ' Mustn't set if the same or we get extra tabs
        End If

        OurSettingsControl.ReadXML(Reader)
    End Sub

    Protected Friend Overrides Sub Reset()
        OurSettingsControl.Reset()
    End Sub

    Protected Friend Overrides Sub WriteXML(ByVal Writer As System.Xml.XmlTextWriter)
        Writer.WriteAttributeString(PluginName & "Enabled", Enabled.ToString)
        OurSettingsControl.WriteXML(Writer)
    End Sub
End Class