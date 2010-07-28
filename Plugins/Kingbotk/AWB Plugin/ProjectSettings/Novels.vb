'Copyright © 2008 Stephen Kennedy (Kingboyk) http://www.sdk-software.com/
'Copyright © 2008 Sam Reed (Reedy) http://www.reedyboy.net/

'This program is free software; you can redistribute it and/or modify it under the terms of Version 2 of the GNU General Public License as published by the Free Software Foundation.

'This program is distributed in the hope that it will be useful,
'but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

'You should have received a copy of the GNU General Public License Version 2 along with this program; if not, write to the Free Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA

Namespace AutoWikiBrowser.Plugins.Kingbotk.Plugins

    Friend NotInheritable Class WPNovelSettings
        Implements IGenericSettings

        Private Const conAutoStubParm As String = "NovelsAutoStub"
        Private Const conStubClassParm As String = "NovelsStubClass"
        Private Const conOldPeerReviewParm As String = "NovelsOldPeerReview"
        Private Const conCrimeWGParm As String = "NovelsCrimeWG"
        Private Const conShortStoryWGParm As String = "NovelsShortStoryWG"
        Private Const conSFWGParm As String = "NovelsSFWG"
        Private Const conAusParm As String = "NovelsAusWG"
        Private Const conFanParm As String = "NovelsFantWG"
        Private Const con19thParm As String = "Novels19thWG"
        Private Const conNarniaParm As String = "NovelsNarniaWG"
        Private Const conLemonyParm As String = "NovelsLemonyWG"
        Private Const conShannaraParm As String = "NovelsShannaraWG"
        Private Const conSwordParm As String = "NovelsSwordWG"
        Private Const conTwilightParm As String = "NovelsTwilightWG"

#Region "XML interface"
        Friend Sub ReadXML(ByVal Reader As System.Xml.XmlTextReader) Implements IGenericSettings.ReadXML
            AutoStub = PluginManager.XMLReadBoolean(Reader, conAutoStubParm, AutoStub)
            StubClass = PluginManager.XMLReadBoolean(Reader, conStubClassParm, StubClass)
            CrimeWG = PluginManager.XMLReadBoolean(Reader, conCrimeWGParm, CrimeWG)
            ShortStoryWG = PluginManager.XMLReadBoolean(Reader, conShortStoryWGParm, ShortStoryWG)
            SFWG = PluginManager.XMLReadBoolean(Reader, conSFWGParm, SFWG)
            AusWG = PluginManager.XMLReadBoolean(Reader, conAusParm, AusWG)
            FantWG = PluginManager.XMLReadBoolean(Reader, conFanParm, FantWG)
            NineteenthCWG = PluginManager.XMLReadBoolean(Reader, con19thParm, NineteenthCWG)
            NarniaWG = PluginManager.XMLReadBoolean(Reader, conNarniaParm, NarniaWG)
            LemonyWG = PluginManager.XMLReadBoolean(Reader, conLemonyParm, LemonyWG)
            ShannaraWG = PluginManager.XMLReadBoolean(Reader, conShannaraParm, ShannaraWG)
            SwordWG = PluginManager.XMLReadBoolean(Reader, conSwordParm, SwordWG)
            TwilightWG = PluginManager.XMLReadBoolean(Reader, conTwilightParm, TwilightWG)
        End Sub
        Friend Sub WriteXML(ByVal Writer As System.Xml.XmlTextWriter) Implements IGenericSettings.WriteXML
            With Writer
                .WriteAttributeString(conCrimeWGParm, CrimeWG.ToString)
                .WriteAttributeString(conShortStoryWGParm, ShortStoryWG.ToString)
                .WriteAttributeString(conSFWGParm, SFWG.ToString)
                .WriteAttributeString(conAutoStubParm, AutoStub.ToString)
                .WriteAttributeString(conStubClassParm, StubClass.ToString)
                .WriteAttributeString(conAusParm, AusWG.ToString())
                .WriteAttributeString(conFanParm, FantWG.ToString())
                .WriteAttributeString(con19thParm, NineteenthCWG.ToString())
                .WriteAttributeString(conNarniaParm, NarniaWG.ToString())
                .WriteAttributeString(conLemonyParm, LemonyWG.ToString())
                .WriteAttributeString(conShannaraParm, ShannaraWG.ToString())
                .WriteAttributeString(conSwordParm, SwordWG.ToString())
                .WriteAttributeString(conTwilightParm, TwilightWG.ToString())
            End With
        End Sub
        Friend Sub Reset() Implements IGenericSettings.XMLReset

        End Sub
#End Region

#Region "Properties"
        Friend Property CrimeWG() As Boolean
            Get
                Return CrimeCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                CrimeCheckBox.Checked = value
            End Set
        End Property
        Friend Property ShortStoryWG() As Boolean
            Get
                Return ShortStoryCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                ShortStoryCheckBox.Checked = value
            End Set
        End Property
        Friend Property SFWG() As Boolean
            Get
                Return SFCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                SFCheckBox.Checked = value
            End Set
        End Property
        Friend Property AusWG() As Boolean
            Get
                Return AustralianCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                AustralianCheckBox.Checked = value
            End Set
        End Property
        Friend Property FantWG() As Boolean
            Get
                Return FantasyCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                FantasyCheckBox.Checked = value
            End Set
        End Property
        Friend Property NineteenthCWG() As Boolean
            Get
                Return NineteenthCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                NineteenthCheckBox.Checked = value
            End Set
        End Property
        Friend Property NarniaWG() As Boolean
            Get
                Return NarniaCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                NarniaCheckBox.Checked = value
            End Set
        End Property
        Friend Property LemonyWG() As Boolean
            Get
                Return LemonyCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                LemonyCheckBox.Checked = value
            End Set
        End Property
        Friend Property ShannaraWG() As Boolean
            Get
                Return ShannaraCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                ShannaraCheckBox.Checked = value
            End Set
        End Property
        Friend Property SwordWG() As Boolean
            Get
                Return SwordCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                SwordCheckBox.Checked = value
            End Set
        End Property
        Friend Property TwilightWG() As Boolean
            Get
                Return TwilightCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                TwilightCheckBox.Checked = value
            End Set
        End Property
        Friend Property AutoStub() As Boolean Implements IGenericSettings.AutoStub
            Get
                Return AutoStubCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                AutoStubCheckBox.Checked = value
            End Set
        End Property
        Friend Property StubClass() As Boolean Implements IGenericSettings.StubClass
            Get
                Return StubClassCheckBox.Checked
            End Get
            Set(ByVal value As Boolean)
                StubClassCheckBox.Checked = value
            End Set
        End Property
        WriteOnly Property StubClassModeAllowed() As Boolean Implements IGenericSettings.StubClassModeAllowed
            Set(ByVal value As Boolean)
                StubClassCheckBox.Enabled = value
            End Set
        End Property
        Friend ReadOnly Property TextInsertContextMenuStripItems() As ToolStripItemCollection _
        Implements IGenericSettings.TextInsertContextMenuStripItems
            Get
                Return TextInsertContextMenuStrip.Items
            End Get
        End Property
#End Region

        ' Event handlers:
        Private Sub LinkClicked(ByVal sender As Object, ByVal e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
            Tools.OpenENArticleInBrowser("Template:NovelsWikiProject", False)
        End Sub
        Private Sub NovelsWikiProjectToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles NovelsWikiProjectToolStripMenuItem.Click
            PluginManager.EditBoxInsert("{{NovelsWikiProject}}")
        End Sub
        Private Sub NovelinfoboxneededToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles NovelinfoboxneededToolStripMenuItem.Click
            PluginManager.EditBoxInsertYesParam("needs-infobox")
        End Sub
        Private Sub NovelinfoboxincompToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles NovelinfoboxincompToolStripMenuItem.Click
            PluginManager.EditBoxInsertYesParam("incomp-infobox")
        End Sub
        Private Sub NovelsClassListToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles ClassListToolStripMenuItem.Click
            PluginManager.EditBoxInsert("|class=List")
        End Sub
        Private Sub CoverNeededToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles CoverNeededToolStripMenuItem.Click
            PluginManager.EditBoxInsertYesParam("needs-infobox-cover")
        End Sub
        Private Sub StCoverNeededToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles StCoverNeededToolStripMenuItem.Click
            PluginManager.EditBoxInsert("|needs-infobox-cover=1st")
        End Sub
        Private Sub ShortStoriesToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ShortStoriesToolStripMenuItem.Click
            PluginManager.EditBoxInsertYesParam("short-story-task-force")
        End Sub
        Private Sub CrimeToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CrimeToolStripMenuItem.Click
            PluginManager.EditBoxInsertYesParam("crime-task-force")
        End Sub
        Private Sub ScienceFictionToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ScienceFictionToolStripMenuItem.Click
            PluginManager.EditBoxInsertYesParam("sf-task-force")
        End Sub
        Private Sub NeedsAttentionToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NeedsAttentionToolStripMenuItem.Click
            PluginManager.EditBoxInsertYesParam("attention")
        End Sub
        Private Sub CollaborationCandidateToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CollaborationCandidateToolStripMenuItem.Click
            PluginManager.EditBoxInsertYesParam("collaboration-candidate")
        End Sub
        Private Sub PastCollaborationToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PastCollaborationToolStripMenuItem.Click
            PluginManager.EditBoxInsertYesParam("past-collaboration")
        End Sub
        Private Sub PeerReviewToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PeerReviewToolStripMenuItem.Click
            PluginManager.EditBoxInsertYesParam("peer-review")
        End Sub
        Private Sub OldPeerReviewToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OldPeerReviewToolStripMenuItem.Click
            PluginManager.EditBoxInsertYesParam("old-peer-review")
        End Sub
        Private Sub AutotaggedToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AutotaggedToolStripMenuItem.Click
            PluginManager.EditBoxInsertYesParam("auto")
        End Sub
        Private Sub AutoStubCheckBox_CheckedChanged(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles AutoStubCheckBox.CheckedChanged
            If AutoStubCheckBox.Checked Then StubClassCheckBox.Checked = False
        End Sub
        Private Sub StubClassCheckBox_CheckedChanged(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles StubClassCheckBox.CheckedChanged
            If StubClassCheckBox.Checked Then AutoStubCheckBox.Checked = False
        End Sub
        Private Sub AustralianToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AustralianToolStripMenuItem.Click
            PluginManager.EditBoxInsertYesParam("australian-task-force")
        End Sub
        Private Sub FantasyToolStripMenuItem_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FantasyToolStripMenuItem.Click
            PluginManager.EditBoxInsertYesParam("fantasy-task-force")
        End Sub
        Private Sub ThCenturyToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ThCenturyToolStripMenuItem.Click
            PluginManager.EditBoxInsertYesParam("19thC-task-force")
        End Sub
        Private Sub NarniaToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NarniaToolStripMenuItem.Click
            PluginManager.EditBoxInsertYesParam("narnia-task-force")
        End Sub
        Private Sub LemonySnicketToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LemonySnicketToolStripMenuItem.Click
            PluginManager.EditBoxInsertYesParam("lemony-snicket-task-force")
        End Sub
        Private Sub ShannaraToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ShannaraToolStripMenuItem.Click
            PluginManager.EditBoxInsertYesParam("shannara-task-force")
        End Sub
        Private Sub SwordOfTruthToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SwordOfTruthToolStripMenuItem.Click
            PluginManager.EditBoxInsertYesParam("sword-of-truth-task-force")
        End Sub
        Private Sub TwilightToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TwilightToolStripMenuItem.Click
            PluginManager.EditBoxInsertYesParam("twilight-task-force")
        End Sub
    End Class
End Namespace