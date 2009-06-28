'Copyright © 2008 Stephen Kennedy (Kingboyk) http://www.sdk-software.com/
'Copyright © 2008 Sam Reed (Reedy) http://www.reedyboy.net/

'This program is free software; you can redistribute it and/or modify it under the terms of Version 2 of the GNU General Public License as published by the Free Software Foundation.

'This program is distributed in the hope that it will be useful,
'but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

'You should have received a copy of the GNU General Public License Version 2 along with this program; if not, write to the Free Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA

Namespace AutoWikiBrowser.Plugins.Kingbotk
    Partial Class PluginBase
        ' Redirects:
        ' SPACES SHOULD BE WRITTEN TO XML AND IN THE GENERIC TL ALTERNATE NAME TEXT BOX AS SPACES ONLY
        ' WHEN READ FROM XML, FROM WIKIPEDIA OR FROM THE TEXT BOX AND FED INTO REGEXES CONVERT THEM TO [ _]
        Protected mGotRedirectsFromWikipedia As Boolean
        Protected mLastKnownGoodRedirects As String = "" ' Should contain spaces not [ _]. We always try to use an up-to-date list from the server, but we can at user's choice fall back to a recent list (generally from XML settings) at user's bidding


        Friend Sub New(ByVal DefaultRegexpmiddle As String)
            GotNewAlternateNamesString(DefaultRegexpmiddle)
        End Sub

        ' Properties and regex construction:

        ''' <summary>
        ''' Called when we have a new Redirects list (at startup, from Wikipedia, or from the user in the case of generic templates)
        ''' </summary>
        ''' <param name="AlternateNames"></param>
        ''' <param name="SenderIsGenericTemplateForm"></param>
        ''' <remarks></remarks>
        Protected Sub GotNewAlternateNamesString(ByVal AlternateNames As String, _
        Optional ByVal SenderIsGenericTemplateForm As Boolean = False)
            Dim RegexpMiddle As String ' Less efficient to transfer to a new string but makes code easier to understand
            Dim mHasAlternateNames As Boolean

            ' Argument should NOT contain the default name at this stage; should contain spaces not [ _] or _
            If SenderIsGenericTemplateForm Then
                AlternateNames = AlternateNames.Replace("_", " ")
            Else
                CheckNoUnderscores(AlternateNames)
            End If

            AlternateNames = AlternateNames.Trim

            PluginManager.AWBForm.TraceManager.WriteBulletedLine("Template:" & PreferredTemplateName & " redirects: " & _
               AlternateNames, False, True, False)

            If AlternateNames = "" Then
                mHasAlternateNames = False
                RegexpMiddle = PreferredTemplateName
            Else
                mHasAlternateNames = True
                mLastKnownGoodRedirects = AlternateNames
                RegexpMiddle = PreferredTemplateName & "|" & AlternateNames
            End If
            RegexpMiddle = RegexpMiddle.Replace(" ", "[ _]")

            MainRegex = New Regex(conRegexpLeft & RegexpMiddle & conRegexpRight, conRegexpOptions)
            SecondChanceRegex = New Regex(conRegexpLeft & RegexpMiddle & conRegexpRightNotStrict, conRegexpOptions)

#If DEBUG Then
            Debug.Print("LastKnownGoodRedirects: " & mLastKnownGoodRedirects)
            Debug.Print("MainRegex: " & MainRegex.ToString)
            Debug.Print("SecondChanceRegex: " & SecondChanceRegex.ToString)
#End If

            If mHasAlternateNames Then
                PreferredTemplateNameRegex = New Regex(PreferredTemplateNameRegexCreator.Replace(PreferredTemplateName, _
                   AddressOf Me.PreferredTemplateNameWikiMatchEvaluator), RegexOptions.Compiled)
#If DEBUG Then
                Debug.Print("PreferredTemplateNameRegex: " & PreferredTemplateNameRegex.ToString)
#End If
            Else
                PreferredTemplateNameRegex = Nothing
#If DEBUG Then
                Debug.Print("PreferredTemplateNameRegex: Null")
#End If
            End If
        End Sub
        ''' <summary>
        ''' Do any redirects point to the template?
        ''' </summary>
        ''' <returns>True if the template has alternate names</returns>
        Protected ReadOnly Property HasAlternateNames() As Boolean
            Get
                Return (Not PreferredTemplateNameRegex Is Nothing)
            End Get
        End Property
        ''' <summary>
        ''' Returns true if the templating plugin is ready to start processing
        ''' </summary>
        Protected Friend Overridable ReadOnly Property IAmReady() As Boolean
            ' In compiled templates, this is where we check if we've got an up-to-date redirects list from Wikipedia
            ' In generic templates, we also check whether the generic template has enough configuration to start tagging
            Get
#If DEBUG Then
                Return True
#Else
                If Not mGotRedirectsFromWikipedia Then ' we've not checked redirects
                    CheckRedirects() ' check them, and check the variable again
                    If mGotRedirectsFromWikipedia Then Return True Else Throw New RedirectsException
                Else
                    Return True
                End If
#End If
            End Get
        End Property
        Private Sub CheckNoUnderscores(ByVal text As String)
            ' TODO: Once verified that code is good, this can be removed
            If text.Contains("_") Then Throw New ArgumentException("AlternateNames string should not contain underscores at this stage. Please make a note of exactly what you were doing and report this as a bug")
        End Sub

        ' Get the redirects from Wikipedia:
        ''' <summary>
        ''' Load the redirects for a template from Wikipedia
        ''' </summary>
        ''' <param name="Target">Template name</param>
        Protected Shared Function GetRedirects(ByVal Target As String) As List(Of WikiFunctions.Article)
            Dim message As String = "Loading redirects for Template:" & Target

            PluginManager.StatusText.Text = message
            PluginManager.AWBForm.TraceManager.WriteBulletedLine(conAWBPluginName & ":" & message, False, False, True)
            System.Windows.Forms.Application.DoEvents() ' the statusbar text wasn't updating without this; if happens elsewhere may need to write a small subroutine

            Try
                Return New WikiFunctions.Lists.RedirectsListProvider().MakeList([Namespace].Template, New String() {"Template:" & Target})
            Catch
                Throw
            Finally
                PluginManager.DefaultStatusText()
            End Try
        End Function
        Private Sub CheckRedirects()
            Dim Redirects As List(Of WikiFunctions.Article)
            Dim gotredirects As Boolean

            Try
                Redirects = GetRedirects(PreferredTemplateName)
                gotredirects = True

                If Redirects.Count = 0 Then
                    GotNewAlternateNamesString("")
                Else
                    GotNewAlternateNamesString(ConvertRedirectsToString(Redirects))
                End If

                mGotRedirectsFromWikipedia = True
            Catch When gotredirects
                Throw
            Catch ex As Exception
                Select Case MessageBox.Show("We caught an error when attempting to get the incoming redirects for Template:" & _
                PreferredTemplateName & "." & Microsoft.VisualBasic.vbCrLf & Microsoft.VisualBasic.vbCrLf & "* Press Abort to stop AWB" & _
                Microsoft.VisualBasic.vbCrLf & "* Press Retry to try again" & Microsoft.VisualBasic.vbCrLf & _
                "* Press Ignore to use the default redirects list. This may be dangerous if the list is out of date but is perfectly fine if you know or suspect it's up to date. The list is:" & _
                Microsoft.VisualBasic.vbCrLf & mLastKnownGoodRedirects & Microsoft.VisualBasic.vbCrLf & Microsoft.VisualBasic.vbCrLf & _
                "The error was:" & Microsoft.VisualBasic.vbCrLf & ex.Message, "Error", MessageBoxButtons.AbortRetryIgnore, _
                MessageBoxIcon.Error, MessageBoxDefaultButton.Button3)
                    Case DialogResult.Abort
                        mGotRedirectsFromWikipedia = False
                    Case DialogResult.Retry
                        CheckRedirects()
                    Case DialogResult.Ignore
                        GotNewAlternateNamesString(mLastKnownGoodRedirects) ' This may be different to default if we loaded from settings
                        mGotRedirectsFromWikipedia = True
                End Select
            End Try
        End Sub
        Protected Shared Function ConvertRedirectsToString(ByRef Redirects As List(Of WikiFunctions.Article)) As String
            Dim tmp As New List(Of WikiFunctions.Article)
            ConvertRedirectsToString = ""

            For Each redirect As WikiFunctions.Article In Redirects
                If redirect.NameSpaceKey = 10 Then
                    ConvertRedirectsToString += redirect.Name.Remove(0, 9) & "|"
                    tmp.Add(redirect) ' hack because can't remove from a collection within a foreach block iterating through it
                End If
            Next

            Redirects = tmp

            Return ConvertRedirectsToString.Trim(New Char() {CChar("|")}) ' would .Remove be quicker? or declaring this as static?
        End Function

        ' XML:
        Friend Overridable Sub ReadXMLRedirects(ByVal Reader As System.Xml.XmlTextReader)
            ' For compiled template plugins, a Redirect string read in from XML is for backup use only if getting from WP fails
            ' Generic templates already support AlternateNames property so will override this
            Dim Redirs As String = PluginManager.XMLReadString(Reader, RedirectsParm, mLastKnownGoodRedirects)

            CheckNoUnderscores(Redirs)
            If Not Redirs = "" Then mLastKnownGoodRedirects = Redirs
        End Sub
        Friend Overridable Sub WriteXMLRedirects(ByVal Writer As System.Xml.XmlTextWriter)
            If Not mLastKnownGoodRedirects = "" Then Writer.WriteAttributeString(RedirectsParm, mLastKnownGoodRedirects)
        End Sub
        Protected ReadOnly Property RedirectsParm() As String
            Get
                Return PreferredTemplateName.Replace(" ", "") & "RedirN"
            End Get
        End Property
    End Class
End Namespace

Namespace AutoWikiBrowser.Plugins.Kingbotk.Plugins
    Partial Class GenericTemplatePlugin
        ''' <summary>
        ''' This is called when the contents of TemplateNameTextBox or AlternateNamesTextBox changes, or when HasAlternateNamesCheckBox is (un)checked
        ''' </summary>
        Private Sub TemplateNamesChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
            With OurSettingsControl
                If .TemplateName = "" Then
                    MainRegex = Nothing
                    PreferredTemplateNameRegex = Nothing
                    SecondChanceRegex = Nothing
                Else
                    If .HasAlternateNames Then
                        GotNewAlternateNamesString(.AlternateNames)
                    Else
                        GotNewAlternateNamesString("")
                    End If
                End If
            End With
        End Sub
    End Class
End Namespace