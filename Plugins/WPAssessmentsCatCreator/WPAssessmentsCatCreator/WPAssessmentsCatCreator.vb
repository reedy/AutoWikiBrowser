'(C) 2007 Stephen Kennedy (Kingboyk) http://www.sdk-software.com/

'This program is free software; you can redistribute it and/or modify
'it under the terms of the GNU General Public License as published by
'the Free Software Foundation; either version 2 of the License, or
'(at your option) any later version.

'This program is distributed in the hope that it will be useful,
'but WITHOUT ANY WARRANTY; without even the implied warranty of
'MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'GNU General Public License for more details.

'You should have received a copy of the GNU General Public License
'along with this program; if not, write to the Free Software
'Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA

' This is a plugin to create the category tree for a new WikiProject assessments
' department. It was thrown together as quickly as possible and doesn't represent
' the usual coding standard of the author(s)!

Imports Microsoft.VisualBasic.Interaction ' yes yes I know, I said this was quick and dirty :)

Namespace AutoWikiBrowser.Plugins.SDKSoftware.WPAssessmentsCatCreator
    Public NotInheritable Class WPAssessmentsCatCreator
        Implements IAWBPlugin

        ' AWB objects:
        Private Shared AWBForm As IAWBMainForm
        Private Shared AWBList As WikiFunctions.Lists.ListMaker

        ' Menu item:
        Private Const conOurName As String = "WPAssessmentsCatCreator"
        Private Shared WithEvents OurMenuItem As New ToolStripMenuItem(conOurName & "Plugin")

        ' User input:
        Private WikiProjectName As String, TemplateName As String, ArticleType As String, ParentCat As String

        ' Regex:
        Private CatRegex As Regex

        ' AWB interface:
        Public Event Diff() Implements WikiFunctions.Plugin.IAWBPlugin.Diff
        Public Event Save() Implements WikiFunctions.Plugin.IAWBPlugin.Save
        Public Event Skip() Implements WikiFunctions.Plugin.IAWBPlugin.Skip
        Public Event Start() Implements WikiFunctions.Plugin.IAWBPlugin.Start
        Public Event [Stop]() Implements WikiFunctions.Plugin.IAWBPlugin.Stop
        Public Event Preview() Implements WikiFunctions.Plugin.IAWBPlugin.Preview

        Public Sub Initialise(ByVal list As WikiFunctions.Lists.ListMaker, _
        ByVal web As WikiFunctions.Browser.WebControl, ByVal tsmi As System.Windows.Forms.ToolStripMenuItem, _
        ByVal cms As System.Windows.Forms.ContextMenuStrip, ByVal tab As System.Windows.Forms.TabControl, _
        ByVal frm As System.Windows.Forms.Form, ByVal txt As System.Windows.Forms.TextBox) _
        Implements WikiFunctions.Plugin.IAWBPlugin.Initialise
            ' Store object references:
            AWBForm = DirectCast(frm, IAWBMainForm)
            AWBList = list

            ' Get notification of the user clicking Start:
            AddHandler AWBForm.StartButton.Click, AddressOf StartButtonClickHandler

            ' Add our menu item:
            With OurMenuItem
                .CheckOnClick = False
                .Checked = False
                .ToolTipText = "Start the " & conOurName & " plugin"
            End With

            tsmi.DropDownItems.Add(OurMenuItem)
        End Sub
        Public ReadOnly Property Name() As String Implements WikiFunctions.Plugin.IAWBPlugin.Name
            Get
                Return conOurName
            End Get
        End Property
        Public Function ProcessArticle(ByVal ArticleText As String, ByVal ArticleTitle As String, _
        ByVal [Namespace] As Integer, ByRef Summary As String, ByRef Skip As Boolean) As String _
        Implements WikiFunctions.Plugin.IAWBPlugin.ProcessArticle

            ' a regex with capture groups would be better
            If OurMenuItem.Checked Then
                With CatRegex.Match(ArticleTitle)
                    If Not .Groups("class").Captures(0).ToString = "" Then
                        MessageBox.Show(.Groups("class").Captures(0).ToString)
                    End If
                End With

                Throw New ApplicationException("Unexpected page title. Probable fault in plugin")

                Return ArticleText
            Else : Return ArticleText
            End If
        End Function

        ' Private routines:
        Private Sub AddImportanceCats(ByVal Importance As Boolean)
            Dim ImpPri As String

            If Importance Then
                ImpPri = "importance"
                AWBList.Add("Category:" & ArticleType & " articles by importance")
            Else
                ImpPri = "priority"
                AWBList.Add("Category:" & ArticleType & " articles by priority")
            End If

            For Each str2 As String In New String() {"Top", "High", "Mid", "Low", "Unknown"}
                AWBList.Add("Category:" & str2 & "-" & ImpPri & " " & ArticleType & " articles")
            Next
        End Sub

        ' Event handlers:
        Private Sub StartButtonClickHandler(ByVal sender As Object, ByVal e As EventArgs)
            If Not OurMenuItem.Checked Then Exit Sub

            RaiseEvent Stop()
            AWBForm.StopButton.PerformClick()

            If AWBList.Count > 0 Then
                If MessageBox.Show( _
                "The article list is not empty. To empty it and proceed, hit OK. Otherwise, hit Cancel", _
                "Article list not empty", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, _
                MessageBoxDefaultButton.Button2) = DialogResult.OK Then
                    AWBList.Clear()
                Else
                    Exit Sub
                End If
            End If

            AWBForm.SkipNonExistentPages = False

            If MessageBox.Show("This is a simple script - presented as a plugin - for creating article " & _
               "assessment categories. It should not be run with automated edits switched on, and all " & _
               "edits should be reviewed. Other plugins should be switched off.", "Let's start", _
               MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1) _
               = DialogResult.Cancel Then Exit Sub

            WikiProjectName = InputBox("Enter the name of the WikiProject page, for example " & _
               "Wikipedia:WikiProject The Beatles" & Microsoft.VisualBasic.vbCrLf & Microsoft.VisualBasic.vbCrLf & _
               "You may use a piped name if you wish", "WikiProject Name", "Wikipedia:WikiProject").Trim

            TemplateName = Regex.Replace(InputBox( _
               "Please enter the name of your project's template (without the template: prefix)", "Template").Trim, _
               "^template:", "", RegexOptions.IgnoreCase)

            ArticleType = InputBox("Enter the name of your article type so we can build the " & _
               "categories" & Microsoft.VisualBasic.vbCrLf & Microsoft.VisualBasic.vbCrLf & _
               "For example, to build categories like ""FA-Class biography articles"" enter ""biography""" & _
               "(without the quotes)", "Article subject type").Trim

            For Each str As String In New String() {"A", "B", "GA", "FA", "Start", "Stub", "Unassessed"}
                AWBList.Add("Category:" & str & "-Class " & ArticleType & " articles")
            Next
            AWBList.Add("Category:" & ArticleType & " articles with comments")
            AWBList.Add("Category:" & ArticleType & " articles by quality")

            If MessageBox.Show("Does your WikiProject track priority or importance?", "Priority/importance", _
            MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                Select Case (MessageBox.Show("Do you use importance=?" & Microsoft.VisualBasic.vbCrLf & _
                Microsoft.VisualBasic.vbCrLf & "Hit yes for importance, no for priority, cancel to exit", _
                "Importance/priority", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
                    Case DialogResult.Yes
                        AddImportanceCats(True)
                    Case DialogResult.No
                        AddImportanceCats(False)
                    Case DialogResult.Cancel
                        Exit Sub
                End Select
            End If

            ParentCat = Regex.Replace(InputBox("What's the name of the parent category for the by quality/by " & _
               "priority categories?" & Microsoft.VisualBasic.vbCrLf & Microsoft.VisualBasic.vbCrLf & _
               "Example: Category:Military work group articles").Trim, "^category:", "", RegexOptions.IgnoreCase)

            AWBList.Add("Category:" & ParentCat)

            CatRegex = New Regex("^Category:((?<class>[A-Z]*)-Class .* articles|(?<ParentCat>" & _
               Regex.Escape(ParentCat) & ")|(?<comments>" & Regex.Escape(ArticleType) & " articles with comments)" & _
               "|(?<importance>[a-zA-Z]*)-(?<imppri>priority|importance)" & _
               "|(.*(?<art>by quality)|(?<byimp>by importance)|(?<bypri>by priority)$))", _
               RegexOptions.ExplicitCapture)

            RaiseEvent Start()
        End Sub

        ' Do nothing:
        Public Function SaveSettings() As Object() Implements WikiFunctions.Plugin.IAWBPlugin.SaveSettings
            Return Nothing
        End Function
        Public Sub LoadSettings(ByVal Prefs() As Object) Implements WikiFunctions.Plugin.IAWBPlugin.LoadSettings
        End Sub
        Public Sub Reset() Implements WikiFunctions.Plugin.IAWBPlugin.Reset
        End Sub
    End Class
End Namespace