'Namespace AWB.Plugins.SDKSoftware.Kingbotk.Components
Friend NotInheritable Class Logging
    Private PluginSettingsUC As PluginSettingsControl

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles OK_Button.Click
        With PluginSettingsUC
            .LogBadPages = LogBadTagsCheckBox.Checked
            .LogXHTML = XHTMLLogCheckBox.Checked
            .LogWiki = WikiLogCheckBox.Checked
            .LogVerbose = VerboseCheckBox.Checked
            If System.IO.Directory.Exists(FolderTextBox.Text) Then
                .LogFolder = FolderTextBox.Text
            Else
                If MessageBox.Show("Folder doesn't exist, using previous setting", "No such folder", _
                MessageBoxButtons.OKCancel, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1) _
                = Windows.Forms.DialogResult.Cancel Then
                    Exit Sub
                End If
            End If
        End With
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub
    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub
    Private Sub FolderButton_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles FolderButton.Click
        If FolderBrowserDialog1.ShowDialog(Me) = Windows.Forms.DialogResult.OK Then
            FolderTextBox.Text = FolderBrowserDialog1.SelectedPath
        End If
    End Sub

    Friend Sub New(ByVal UC As PluginSettingsControl)
        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        PluginSettingsUC = UC
        With UC
            LogBadTagsCheckBox.Checked = .LogBadPages
            XHTMLLogCheckBox.Checked = .LogXHTML
            WikiLogCheckBox.Checked = .LogWiki
            VerboseCheckBox.Checked = .LogVerbose
            FolderTextBox.Text = .LogFolder
        End With
    End Sub
End Class
'end namespace

' TODO: Bot mode: upload logs to wiki; buttons or menu items for jobs (e.g. download living people, downloasd tagged list, compare and save both, set settings, etc)

Namespace AWB.Plugins.SDKSoftware.Kingbotk.Components
    Friend Interface MyTraceListener
        Sub Flush()
        Sub Close()
        Sub WriteBulletedLine(ByVal Line As String, ByVal Bold As Boolean, ByVal VerboseOnly As Boolean, _
           Optional ByVal DateStamp As Boolean = False)
        Sub ProcessingArticle(ByVal FullArticleTitle As String, ByVal NS As Namespaces)
        Sub SkippedArticle(ByVal SkippedBy As String, ByVal Reason As String)
        Sub SkippedArticleBadTag(ByVal SkippedBy As String, ByVal FullArticleTitle As String, _
           ByVal NS As Namespaces)
        Sub SkippedArticleRedlink(ByVal FullArticleTitle As String, ByVal NS As Namespaces)
        Sub WriteArticleActionLine(ByVal Line As String, ByVal PluginName As String)
        Sub WriteArticleActionLine(ByVal Line As String, ByVal PluginName As String, ByVal VerboseOnly As Boolean)
        Sub WriteTemplateAdded(ByVal Template As String, ByVal PluginName As String)
    End Interface

    Friend MustInherit Class TraceListenerBase
        Inherits System.IO.StreamWriter
        Implements MyTraceListener

        Protected mVerbose As Boolean

        Public Sub New(ByVal filename As String, ByVal LogVerbose As Boolean)
            MyBase.New(filename, False, System.Text.Encoding.UTF8)
            mVerbose = LogVerbose
        End Sub

        Public Overrides Sub Close() Implements MyTraceListener.Close
            MyBase.Close()
        End Sub
        Public Overrides Sub Flush() Implements MyTraceListener.Flush
            MyBase.Flush()
        End Sub
        Public MustOverride Sub ProcessingArticle(ByVal FullArticleTitle As String, ByVal NS As Namespaces) _
           Implements MyTraceListener.ProcessingArticle
        Public Overridable Sub WriteBulletedLine(ByVal Line As String, ByVal Bold As Boolean, _
        ByVal VerboseOnly As Boolean, Optional ByVal DateStamp As Boolean = False) _
        Implements MyTraceListener.WriteBulletedLine
            If VerboseOnly And Not mVerbose Then Exit Sub
        End Sub
        Public MustOverride Sub SkippedArticle(ByVal SkippedBy As String, ByVal Reason As String) _
           Implements MyTraceListener.SkippedArticle
        Public MustOverride Sub SkippedArticleBadTag(ByVal SkippedBy As String, ByVal FullArticleTitle As String, _
        ByVal NS As Namespaces) Implements MyTraceListener.SkippedArticleBadTag
        Public MustOverride Sub WriteArticleActionLine(ByVal Line As String, ByVal PluginName As String) _
           Implements MyTraceListener.WriteArticleActionLine
        Public MustOverride Sub WriteTemplateAdded(ByVal Template As String, ByVal PluginName As String) _
           Implements MyTraceListener.WriteTemplateAdded
        Public Overridable Sub SkippedArticleRedlink(ByVal FullArticleTitle As String, ByVal NS As Namespaces) _
           Implements MyTraceListener.SkippedArticleRedlink
            SkippedArticle("Plugin manager", "Attached article doesn't exist - maybe deleted?")
        End Sub
        Public Sub WriteArticleActionLineVerbose(ByVal Line As String, ByVal PluginName As String, _
        ByVal VerboseOnly As Boolean) Implements MyTraceListener.WriteArticleActionLine
            If VerboseOnly And Not mVerbose Then Exit Sub
            WriteArticleActionLine(Line, PluginName)
        End Sub
        Protected Shared Function GetArticleTemplate(ByVal ArticleFullTitle As String, ByVal NS As Namespaces) _
        As String
            Dim namesp As Integer
            Static reg As New Regex("( talk)?:"), reg2 As New Regex("[^:].*:")
            Dim strnamespace As String, strtitle As String, templ As String

            Select Case NS
                Case Namespaces.Main
                    Return "#{{subst:la|" & ArticleFullTitle & "}}"
                Case Namespaces.Talk
                    Return "#{{subst:lat|" & reg2.Replace(ArticleFullTitle, "") & "}}"
                Case Else
                    namesp = DirectCast(NS, Integer)
                    strnamespace = reg.Replace(WikiFunctions.Variables.Namespaces.Item(NS), "")
                    strtitle = reg2.Replace(ArticleFullTitle, "")

                    If namesp Mod 2 = 1 Then ' talk
                        templ = "lnt"
                    Else ' not talk
                        templ = "ln"
                    End If

                    Return "#{{subst:" & templ & "|" & strnamespace & "|" & strtitle & "}}"
            End Select
        End Function
    End Class


    Friend NotInheritable Class MyTrace
        Implements MyTraceListener

        Private mHaveOpenFile As Boolean
        Private Listeners As New List(Of MyTraceListener)

        ' State:
        Friend ReadOnly Property HaveOpenFile() As Boolean
            Get
                Return mHaveOpenFile
            End Get
        End Property
        Friend Sub Initialise(ByVal LogBadPages As Boolean, ByVal LogXHTML As Boolean, _
        ByVal LogWiki As Boolean, ByVal LogVerbose As Boolean, ByVal LogFolder As String)
            If LogBadPages Or LogXHTML Or LogWiki Then
                If Not IO.Directory.Exists(LogFolder) Then LogFolder = Application.StartupPath

                If LogBadPages Then
                    Listeners.Add(New BadPagesTraceListener(FilePrefix(LogFolder) & " skipped.txt", LogVerbose))
                End If
                If LogXHTML Then
                    Listeners.Add(New XHTMLTraceListener(FilePrefix(LogFolder) & " log.html", LogVerbose))
                End If
                If LogWiki Then
                    Listeners.Add(New WikiTraceListener(FilePrefix(LogFolder) & " log.txt", LogVerbose))
                End If
            End If
            For Each t As MyTraceListener In Listeners
                t.WriteBulletedLine("Start button clicked. Initialising log.", True, False, True)
            Next
            mHaveOpenFile = True
        End Sub

        ' Interface:
        Public Sub Close() Implements MyTraceListener.Close
            For Each t As MyTraceListener In Listeners
                t.Close()
            Next
        End Sub
        Public Sub Flush() Implements MyTraceListener.Flush
            For Each t As MyTraceListener In Listeners
                t.Flush()
            Next
        End Sub
        Public Sub ProcessingArticle(ByVal FullArticleTitle As String, ByVal NS As Namespaces) _
        Implements MyTraceListener.ProcessingArticle
            For Each t As MyTraceListener In Listeners
                t.ProcessingArticle(FullArticleTitle, NS)
            Next
        End Sub
        Public Sub WriteBulletedLine(ByVal Line As String, ByVal Bold As Boolean, ByVal VerboseOnly As Boolean, _
        Optional ByVal DateStamp As Boolean = False) Implements MyTraceListener.WriteBulletedLine
            For Each t As MyTraceListener In Listeners
                t.WriteBulletedLine(Line, Bold, VerboseOnly, DateStamp)
            Next
        End Sub
        Public Sub SkippedArticle(ByVal SkippedBy As String, ByVal Reason As String) _
        Implements MyTraceListener.SkippedArticle
            For Each t As MyTraceListener In Listeners
                t.SkippedArticle(SkippedBy, Reason)
            Next
        End Sub
        Public Sub SkippedArticleBadTag(ByVal SkippedBy As String, ByVal FullArticleTitle As String, _
        ByVal NS As Namespaces) Implements MyTraceListener.SkippedArticleBadTag
            For Each t As MyTraceListener In Listeners
                t.SkippedArticleBadTag(SkippedBy, FullArticleTitle, NS)
            Next
        End Sub
        Public Sub SkippedArticleRedlink(ByVal FullArticleTitle As String, ByVal NS As Namespaces) Implements MyTraceListener.SkippedArticleRedlink
            For Each t As MyTraceListener In Listeners
                t.SkippedArticleRedlink(FullArticleTitle, NS)
            Next
        End Sub
        Public Sub WriteArticleActionLine(ByVal Line As String, ByVal PluginName As String) _
        Implements MyTraceListener.WriteArticleActionLine
            For Each t As MyTraceListener In Listeners
                t.WriteArticleActionLine(Line, PluginName)
            Next
        End Sub
        Public Sub WriteTemplateAdded(ByVal Template As String, ByVal PluginName As String) _
        Implements MyTraceListener.WriteTemplateAdded
            For Each t As MyTraceListener In Listeners
                t.WriteTemplateAdded(Template, PluginName)
            Next
        End Sub
        Public Sub WriteArticleActionLine1(ByVal Line As String, ByVal PluginName As String, _
        ByVal VerboseOnly As Boolean) Implements MyTraceListener.WriteArticleActionLine
            If VerboseOnly Then
                For Each t As MyTraceListener In Listeners
                    t.WriteArticleActionLine(Line, PluginName, True)
                Next
            Else
                For Each t As MyTraceListener In Listeners
                    t.WriteArticleActionLine(Line, PluginName)
                Next
            End If
        End Sub

        ' Methods:
        Private Shared ReadOnly Property FilePrefix(ByVal LogFolder As String) As String
            Get
                Return String.Format("{1}\{0:MMM-d yyyy HHmm-ss.FF}", Date.Now, LogFolder)
            End Get
        End Property

        ' Trace listener child classes:
        Private NotInheritable Class WikiTraceListener
            Inherits TraceListenerBase

            Private Const mDateFormat As String = "[[d MMMM]] [[yyyy]] HH:mm "

            Public Sub New(ByVal filename As String, ByVal LogVerbose As Boolean)
                MyBase.New(filename, LogVerbose)
            End Sub

            ' Overrides:
            Public Overrides Sub WriteBulletedLine(ByVal Line As String, ByVal Bold As Boolean, _
            ByVal VerboseOnly As Boolean, Optional ByVal DateStamp As Boolean = False)
                MyBase.WriteBulletedLine(Line, Bold, VerboseOnly, DateStamp)
                If DateStamp Then Line = Date.Now.ToString(mDateFormat) & Line
                If Bold Then MyBase.WriteLine("*'''" & Line & "'''") Else MyBase.WriteLine("*" & Line)
            End Sub
            Public Overrides Sub ProcessingArticle(ByVal ArticleFullTitle As String, ByVal NS As Namespaces)
                MyBase.WriteLine(GetArticleTemplate(ArticleFullTitle, NS))
            End Sub
            Public Overrides Sub SkippedArticle(ByVal SkippedBy As String, ByVal Reason As String)
                If Not Reason = "" Then Reason = ": " & Reason
                MyBase.WriteLine("#*''" & SkippedBy & ": Skipped" & Reason & "''")
            End Sub
            Public Overrides Sub SkippedArticleBadTag(ByVal SkippedBy As String, ByVal FullArticleTitle As String, _
            ByVal NS As Namespaces)
                SkippedArticle(SkippedBy, "Bad tag")
            End Sub
            Public Overrides Sub WriteArticleActionLine(ByVal Line As String, ByVal PluginName As String)
                MyBase.WriteLine("#*" & PluginName & ": " & Line.Replace("[[Category:", "[[:Category:"))
            End Sub
            Public Overrides Sub WriteTemplateAdded(ByVal Template As String, ByVal PluginName As String)
                MyBase.WriteLine(String.Format("#*{1}: [[Template:{0}|{0}]] added", Template, PluginName))
            End Sub
        End Class

        Private NotInheritable Class XHTMLTraceListener
            Inherits TraceListenerBase

            Private Shared mArticleCount As Integer = 1

            Public Sub New(ByVal filename As String, ByVal LogVerbose As Boolean)
                MyBase.New(filename, LogVerbose)
                MyBase.WriteLine("<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" " & _
                   """http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">")
                MyBase.WriteLine("<html xmlns=""http://www.w3.org/1999/xhtml"" xml:lang=""en"" " & _
                   "lang=""en"" dir=""ltr"">")
                MyBase.WriteLine("<head>")
                MyBase.WriteLine("<meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"" />")
                MyBase.WriteLine("<meta name=""generator"" content=""Kingbotk AWB Plugin"" />")
                MyBase.WriteLine("<title>AWB log</title>")
                MyBase.WriteLine("</head><body>")
            End Sub

            ' Overrides:
            Public Overrides Sub Close()
                MyBase.WriteLine("</body>")
                MyBase.WriteLine("</html>")
                MyBase.Close()
            End Sub
            Public Overrides Sub WriteBulletedLine(ByVal Line As String, ByVal Bold As Boolean, _
            ByVal VerboseOnly As Boolean, Optional ByVal DateStamp As Boolean = False)
                MyBase.WriteBulletedLine(Line, Bold, VerboseOnly, DateStamp)
                If DateStamp Then Line = String.Format("{0:g}: {1}", Date.Now, Line)
                If Bold Then MyBase.WriteLine("<br/><li><b>" & Line & "</b></li>") Else _
                   MyBase.WriteLine("<li>" & Line & "</li>")
            End Sub
            Public Overrides Sub ProcessingArticle(ByVal ArticleFullTitle As String, ByVal NS As Namespaces)
                MyBase.WriteLine("<br/>" & mArticleCount.ToString & ". <a href=""" & _
                   Article.GetURL(ArticleFullTitle) & """>[[" & ArticleFullTitle & "]]</a>")
                mArticleCount += 1
            End Sub
            Public Overrides Sub SkippedArticle(ByVal SkippedBy As String, ByVal Reason As String)
                If Not Reason = "" Then Reason = ": " & Reason
                MyBase.WriteLine("<li><i>" & SkippedBy & ": Skipped" & Reason & "</i></li>")
            End Sub
            Public Overrides Sub SkippedArticleBadTag(ByVal SkippedBy As String, ByVal FullArticleTitle As String, _
            ByVal NS As Namespaces)
                SkippedArticle(SkippedBy, "Bad tag")
            End Sub
            Public Overrides Sub WriteArticleActionLine(ByVal Line As String, ByVal PluginName As String)
                MyBase.WriteLine("<li><i>" & PluginName & ": " & Line & "</i></li>")
            End Sub
            Public Overrides Sub WriteTemplateAdded(ByVal Template As String, ByVal PluginName As String)
                MyBase.WriteLine("<br/><li><i>" & PluginName & ": " & Template & "</i></li>")
            End Sub
        End Class

        Private NotInheritable Class BadPagesTraceListener
            Inherits TraceListenerBase

            Public Sub New(ByVal filename As String, ByVal LogVerbose As Boolean)
                MyBase.New(filename, LogVerbose)
            End Sub

            ' Overrides:
            Public Overrides Sub SkippedArticleBadTag(ByVal SkippedBy As String, ByVal FullArticleTitle As String, _
            ByVal NS As Namespaces)
                MyBase.WriteLine(GetArticleTemplate(FullArticleTitle, NS) & " (skipped by the " & SkippedBy _
                   & " plugin; bad tag)")
            End Sub
            Public Overrides Sub SkippedArticleRedlink(ByVal FullArticleTitle As String, ByVal NS As Namespaces)
                MyBase.WriteLine(GetArticleTemplate(FullArticleTitle, NS) & " (skipped by the Plugin Manager; " & _
                   "attached article doesn't exist - maybe deleted?)")
            End Sub

            ' Overrides - do nothing:
            Public Overrides Sub WriteBulletedLine(ByVal Line As String, ByVal bold As Boolean, _
               ByVal b As Boolean, Optional ByVal DateStamp As Boolean = False)
            End Sub
            Public Overrides Sub ProcessingArticle(ByVal ArticleFullTitle As String, ByVal NS As Namespaces)
            End Sub
            Public Overrides Sub WriteArticleActionLine(ByVal Line As String, ByVal PluginName As String)
            End Sub
            Public Overrides Sub WriteTemplateAdded(ByVal Template As String, ByVal PluginName As String)
            End Sub
            Public Overrides Sub SkippedArticle(ByVal SkippedBy As String, ByVal Reason As String)
            End Sub
        End Class
    End Class
End Namespace