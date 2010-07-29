'Copyright © 2008 Stephen Kennedy (Kingboyk) http://www.sdk-software.com/
'Copyright © 2008 Sam Reed (Reedy) http://www.reedyboy.net/

'This program is free software; you can redistribute it and/or modify it under the terms of Version 2 of the GNU General Public License as published by the Free Software Foundation.

'This program is distributed in the hope that it will be useful,
'but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

'You should have received a copy of the GNU General Public License Version 2 along with this program; if not, write to the Free Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
Imports WikiFunctions.Lists.Providers

Namespace AutoWikiBrowser.Plugins.Kingbotk
    Partial Class PluginBase
        ' Redirects:
        ' SPACES SHOULD BE WRITTEN TO XML AND IN THE GENERIC TL ALTERNATE NAME TEXT BOX AS SPACES ONLY
        ' WHEN READ FROM XML, FROM WIKIPEDIA OR FROM THE TEXT BOX AND FED INTO REGEXES CONVERT THEM TO [ _]
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

            If mHasAlternateNames Then
                PreferredTemplateNameRegex = New Regex(PreferredTemplateNameRegexCreator.Replace(PreferredTemplateName, _
                   AddressOf Me.PreferredTemplateNameWikiMatchEvaluator), RegexOptions.Compiled)
            Else
                PreferredTemplateNameRegex = Nothing
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
                Return True
            End Get
        End Property

        Private Shared ReadOnly rlp As New RedirectsListProvider()

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
                Return rlp.MakeList([Namespace].Template, New String() {Variables.Namespaces([Namespace].Template) & Target})
            Catch
                Throw
            Finally
                PluginManager.DefaultStatusText()
            End Try
        End Function
        Protected Shared Function ConvertRedirectsToString(ByRef Redirects As List(Of WikiFunctions.Article)) As String
            Dim tmp As New List(Of WikiFunctions.Article)
            ConvertRedirectsToString = ""

            For Each redirect As WikiFunctions.Article In Redirects
                If redirect.NameSpaceKey = [Namespace].Template Then
                    ConvertRedirectsToString += redirect.Name.Remove(0, 9) & "|"
                    tmp.Add(redirect)
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