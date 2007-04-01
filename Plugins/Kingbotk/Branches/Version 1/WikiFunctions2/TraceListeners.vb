Namespace Logging
    ''' <summary>
    ''' This class logs in wiki format
    ''' </summary>
    Public Class WikiTraceListener
        Inherits TraceListenerUploadableBase

        Protected Const mDateFormat As String = "[[d MMMM]] [[yyyy]] HH:mm "

        Public Sub New(ByVal UploadSettings As UploadableLogSettings2, ByVal TraceStatus As TraceStatus)
            MyBase.New(UploadSettings, TraceStatus)
            WriteBulletedLine("Logging: [[User:Kingbotk/Plugin/WikiFunctions2|WikiFunctions2]].dll v" & _
               WikiFunctions2.Version.ToString, False, False)
        End Sub

        ' Overrides:
        Public Overrides Sub WriteBulletedLine(ByVal Line As String, ByVal Bold As Boolean, _
        ByVal VerboseOnly As Boolean, ByVal DateStamp As Boolean)
            If VerboseOnly AndAlso Not Verbose Then Exit Sub
            If DateStamp Then Line = Date.Now.ToString(mDateFormat) & Line
            If Bold Then MyBase.WriteLine("*'''" & Line & "'''", CheckCounter:=True) _
               Else MyBase.WriteLine("*" & Line, CheckCounter:=True)
        End Sub
        Public Overrides Sub ProcessingArticle(ByVal ArticleFullTitle As String, ByVal NS As Namespaces)
            CheckCounterForUpload() ' Check counter *before* starting a new article section
            MyBase.WriteLine(GetArticleTemplate(ArticleFullTitle, NS), False)
        End Sub
        Public Overrides Sub SkippedArticle(ByVal SkippedBy As String, ByVal Reason As String)
            If Not Reason = "" Then Reason = ": " & Reason
            MyBase.WriteLine("#*''" & SkippedBy & ": Skipped" & Reason & "''", CheckCounter:=False)
        End Sub
        Public Overrides Sub SkippedArticleBadTag(ByVal SkippedBy As String, ByVal FullArticleTitle As String, _
        ByVal NS As Namespaces)
            SkippedArticle(SkippedBy, "Bad tag")
        End Sub
        Public Overrides Sub WriteArticleActionLine(ByVal Line As String, ByVal PluginName As String)
            MyBase.WriteLine("#*" & PluginName & ": " & Line.Replace("[[Category:", "[[:Category:"), _
               CheckCounter:=False)
        End Sub
        Public Overrides Sub WriteTemplateAdded(ByVal Template As String, ByVal PluginName As String)
            MyBase.WriteLine(String.Format("#*{1}: [[Template:{0}|{0}]] added", Template, PluginName), _
               CheckCounter:=False)
        End Sub
        Public Overrides Sub WriteLine(ByVal value As String, Optional ByVal CheckCounter As Boolean = True)
            MyBase.WriteLine(value, CheckCounter)
        End Sub
        Public Overrides Sub WriteComment(ByVal Line As String)
            MyBase.Write("<!-- " & Line & " -->")
        End Sub
        Public Overrides Sub WriteCommentAndNewLine(ByVal Line As String)
            MyBase.WriteLine("<!-- " & Line & " -->", CheckCounter:=False)
        End Sub
    End Class

    ''' <summary>
    ''' This class logs in XHTML format
    ''' </summary>
    Public Class XHTMLTraceListener
        Inherits TraceListenerBase

        Protected Shared mArticleCount As Integer = 1, mVerbose As Boolean

        Public Sub New(ByVal filename As String, ByVal LogVerbose As Boolean)
            MyBase.New(filename)
            mVerbose = LogVerbose

            MyBase.WriteLine("<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" " & _
               """http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">")
            MyBase.WriteLine("<html xmlns=""http://www.w3.org/1999/xhtml"" xml:lang=""en"" " & _
               "lang=""en"" dir=""ltr"">")
            MyBase.WriteLine("<head>")
            MyBase.WriteLine("<meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"" />")
            MyBase.WriteLine("<meta name=""generator"" content=""SDK Software WikiFunctions2" & _
               WikiFunctions2.Version.ToString & """ />")
            MyBase.WriteLine("<title>AWB log</title>")
            MyBase.WriteLine("</head><body>")
        End Sub

        ' Overrides:
        Public Overrides Sub Close()
            MyBase.WriteLine("</body>")
            MyBase.WriteLine("</html>")
            MyBase.Close()
        End Sub
        Public Overrides ReadOnly Property Verbose() As Boolean
            Get
                Return mVerbose
            End Get
        End Property
        Public Overrides Sub WriteBulletedLine(ByVal Line As String, ByVal Bold As Boolean, _
        ByVal VerboseOnly As Boolean, ByVal DateStamp As Boolean)
            If VerboseOnly AndAlso Not mVerbose Then Exit Sub
            If DateStamp Then Line = String.Format("{0:g}: {1}", Date.Now, Line)
            If Bold Then MyBase.WriteLine("<br/><li><b>" & Line & "</b></li>") Else _
               MyBase.WriteLine("<li>" & Line & "</li>")
        End Sub
        Public Overrides Sub ProcessingArticle(ByVal ArticleFullTitle As String, ByVal NS As Namespaces)
            MyBase.WriteLine("<br/>" & mArticleCount.ToString & ". <a href=""" & _
               GetURL(ArticleFullTitle) & """>[[" & ArticleFullTitle & "]]</a>")
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
        Public Overrides Sub WriteLine(ByVal value As String)
            MyBase.WriteLine(value)
        End Sub
        Public Overrides Sub WriteComment(ByVal Line As String)
            MyBase.Write("<!-- " & Line & " -->")
        End Sub
        Public Overrides Sub WriteCommentAndNewLine(ByVal Line As String)
            MyBase.WriteLine("<!-- " & Line & " -->")
        End Sub
        Public Overrides ReadOnly Property Uploadable() As Boolean
            Get
                Return False
            End Get
        End Property
    End Class
End Namespace