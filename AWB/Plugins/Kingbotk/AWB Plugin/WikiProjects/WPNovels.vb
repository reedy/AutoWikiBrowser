'Copyright © 2008 Stephen Kennedy (Kingboyk) http://www.sdk-software.com/
'Copyright © 2008 Sam Reed (Reedy) http://www.reedyboy.net/

'This program is free software; you can redistribute it and/or modify it under the terms of Version 2 of the GNU General Public License as published by the Free Software Foundation.

'This program is distributed in the hope that it will be useful,
'but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

'You should have received a copy of the GNU General Public License Version 2 along with this program; if not, write to the Free Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA

Namespace AutoWikiBrowser.Plugins.Kingbotk.Plugins

    Friend NotInheritable Class WPNovels
        Inherits PluginBase

        ' Settings:
        Private OurTab As New TabPage("Novels")
        Private WithEvents OurSettingsControl As New WPNovelSettings
        Private Const conEnabled As String = "NovEnabled"
        Protected Friend Overrides ReadOnly Property PluginShortName() As String
            Get
                Return "Novels"
            End Get
        End Property
        Protected Overrides ReadOnly Property PreferredTemplateName() As String
            Get
                Return "WikiProject Novels"
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
        Friend Overrides ReadOnly Property HasReqPhotoParam() As Boolean
            Get
                Return True
            End Get
        End Property
        Friend Overrides Sub ReqPhoto()
            AddNewParamWithAYesValue("needs-infobox-cover")
        End Sub
        'Protected Overrides ReadOnly Property PreferredTemplateNameRegexString() As String
        '    Get
        '        Return "^[Nn]ovelsWikiProject$"
        '    End Get
        'End Property

        ' Initialisation:
        Friend Sub New()
            MyBase.New("Novels|WPNovels") ' Specify alternate names only
        End Sub
        Protected Friend Overrides Sub Initialise()
            OurMenuItem = New ToolStripMenuItem("Novels Plugin")
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
                If .CrimeWG Then
                    AddAndLogNewParamWithAYesValue("crime-task-force")
                    AddEmptyParam("crime-importance")
                End If
                If .ShortStoryWG Then
                    AddAndLogNewParamWithAYesValue("short-story-task-force")
                    AddEmptyParam("short-story-importance")
                End If
                If .SFWG Then
                    AddAndLogNewParamWithAYesValue("sf-task-force")
                    AddEmptyParam("sf-importance")
                End If
                If .AusWG Then
                    AddAndLogNewParamWithAYesValue("australian-task-force")
                    AddEmptyParam("australian-importance")
                End If
                If .FantWG Then
                    AddAndLogNewParamWithAYesValue("fantasy-task-force")
                    AddEmptyParam("fantasy-importance")
                End If
                If .NineteenthCWG Then
                    AddAndLogNewParamWithAYesValue("19thC-task-force")
                    AddEmptyParam("19thC-importance")
                End If
                If .NarniaWG Then
                    AddAndLogNewParamWithAYesValue("narnia-task-force")
                    AddEmptyParam("narnia-importance")
                End If
                If .LemonyWG Then
                    AddAndLogNewParamWithAYesValue("lemony-snicket-task-force")
                    AddEmptyParam("lemony-snicket-importance")
                End If
                If .ShannaraWG Then
                    AddAndLogNewParamWithAYesValue("shannara-task-force")
                    AddEmptyParam("shannara-importance")
                End If
                If .SwordWG Then
                    AddAndLogNewParamWithAYesValue("sword-of-truth-task-force")
                    AddEmptyParam("sword-of-truth-importance")
                End If
                If .TwilightWG Then
                    AddAndLogNewParamWithAYesValue("twilight-task-force")
                    AddEmptyParam("twilight-importance")
                End If
            End With
        End Sub
        Protected Overrides Function TemplateFound() As Boolean
        End Function
        Protected Overrides Sub GotTemplateNotPreferredName(ByVal TemplateName As String)
            ' Currently only WPBio does anything here (if {{musician}} add to musician-work-group)
        End Sub
        Protected Overrides Function WriteTemplateHeader(ByRef PutTemplateAtTop As Boolean) As String
            WriteTemplateHeader = "{{WikiProject Novels" & Microsoft.VisualBasic.vbCrLf

            WriteTemplateHeader += WriteOutParameterToHeader("class") & _
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
End Namespace