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
        Private OurTab As New TabPage(Prefix)
        Private WithEvents OurSettingsControl As GenericWithWorkgroups

        Private Const Prefix As String = "Novels"
        Private Const PluginName As String = "WikiProject Novels"

        Friend Sub New()
            MyBase.New("Novels|WPNovels") ' Specify alternate names only

            OurSettingsControl = New GenericWithWorkgroups(PluginName, Prefix, True, params)
        End Sub

        Dim params() As TemplateParameters =
        {
             New TemplateParameters() With {.StorageKey = "CrimeWG", .Group = "", .ParamName = "Crime"}, _
             New TemplateParameters() With {.StorageKey = "ShortStoryWG", .Group = "", .ParamName = "Short Story"}, _
             New TemplateParameters() With {.StorageKey = "SFWG", .Group = "", .ParamName = "SF"}, _
             New TemplateParameters() With {.StorageKey = "AusWG", .Group = "", .ParamName = "Australian"}, _
             New TemplateParameters() With {.StorageKey = "FantWG", .Group = "", .ParamName = "Fantasy"}, _
             New TemplateParameters() With {.StorageKey = "19thWG", .Group = "", .ParamName = "19thC"}, _
             New TemplateParameters() With {.StorageKey = "NarniaWG", .Group = "", .ParamName = "Narnia"}, _
             New TemplateParameters() With {.StorageKey = "LemonyWG", .Group = "", .ParamName = "Lemony Snicket"}, _
             New TemplateParameters() With {.StorageKey = "ShannaraWG", .Group = "", .ParamName = "Shannara"}, _
             New TemplateParameters() With {.StorageKey = "SwordWG", .Group = "", .ParamName = "Sword of Truth"}, _
             New TemplateParameters() With {.StorageKey = "TwilightWG", .Group = "", .ParamName = "Twilight"}, _
             New TemplateParameters() With {.StorageKey = "OldPeerReview", .Group = "", .ParamName = "Old Peer Review"}
        }

        Protected Friend Overrides ReadOnly Property PluginShortName() As String
            Get
                Return Prefix
            End Get
        End Property
        Protected Overrides ReadOnly Property PreferredTemplateName() As String
            Get
                Return PluginName
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
        Protected Friend Overrides Sub Initialise()
            OurMenuItem = New ToolStripMenuItem("Novels Plugin")
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
                        AddAndLogNewParamWithAYesValue(param & "-task-force") 'Probably needs some reformatting
                        AddEmptyParam(param & "-importance")
                    End If
                Next
            End With
        End Sub
        Protected Overrides Function TemplateFound() As Boolean
        End Function
        Protected Overrides Sub GotTemplateNotPreferredName(ByVal TemplateName As String)
            ' Currently only WPBio does anything here (if {{musician}} add to musician-work-group)
        End Sub
        Protected Overrides Function WriteTemplateHeader(ByRef PutTemplateAtTop As Boolean) As String
            Return "{{" & PluginName & Microsoft.VisualBasic.vbCrLf & _
                WriteOutParameterToHeader("class") & _
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
End Namespace