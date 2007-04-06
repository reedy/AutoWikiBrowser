Namespace Logging
    ''' <summary>
    ''' This abstract class can be used to build trace listener classes, or you can build a class from scratch and implement IMyTraceListener
    ''' </summary>
    Public MustInherit Class TraceListenerBase
        Inherits System.IO.StreamWriter
        Implements IMyTraceListener

        ' Initialisation
        Public Sub New(ByVal filename As String)
            MyBase.New(filename, False, System.Text.Encoding.UTF8)
        End Sub

#Region "IMyTraceListener interface"
        Public Overrides Sub Close() Implements IMyTraceListener.Close
            MyBase.Close()
        End Sub
        Public Overrides Sub Flush() Implements IMyTraceListener.Flush
            MyBase.Flush()
        End Sub
        Public MustOverride Sub ProcessingArticle(ByVal FullArticleTitle As String, ByVal NS As Namespaces) _
           Implements IMyTraceListener.ProcessingArticle
        Public MustOverride Sub WriteBulletedLine(ByVal Line As String, ByVal Bold As Boolean, _
        ByVal VerboseOnly As Boolean, ByVal DateStamp As Boolean) _
        Implements IMyTraceListener.WriteBulletedLine
        Public Sub WriteBulletedLine(ByVal Line As String, ByVal Bold As Boolean, _
        ByVal VerboseOnly As Boolean) Implements IMyTraceListener.WriteBulletedLine
            WriteBulletedLine(Line, Bold, VerboseOnly, False)
        End Sub
        Public MustOverride Sub SkippedArticle(ByVal SkippedBy As String, ByVal Reason As String) _
           Implements IMyTraceListener.SkippedArticle
        Public MustOverride Sub SkippedArticleBadTag(ByVal SkippedBy As String, ByVal FullArticleTitle As String, _
        ByVal NS As Namespaces) Implements IMyTraceListener.SkippedArticleBadTag
        Public MustOverride Sub WriteArticleActionLine(ByVal Line As String, ByVal PluginName As String) _
           Implements IMyTraceListener.WriteArticleActionLine
        Public MustOverride Sub WriteTemplateAdded(ByVal Template As String, ByVal PluginName As String) _
           Implements IMyTraceListener.WriteTemplateAdded
        Public MustOverride Sub WriteComment(ByVal Line As String) Implements IMyTraceListener.WriteComment
        Public MustOverride Sub WriteCommentAndNewLine(ByVal Line As String) _
           Implements IMyTraceListener.WriteCommentAndNewLine
        Public Overridable Sub SkippedArticleRedlink(ByVal SkippedBy As String, ByVal FullArticleTitle As String, ByVal NS As Namespaces) _
           Implements IMyTraceListener.SkippedArticleRedlink
            SkippedArticle(SkippedBy, "Attached article doesn't exist - maybe deleted?")
        End Sub
        Public Sub WriteArticleActionLineVerbose(ByVal Line As String, ByVal PluginName As String, _
        ByVal VerboseOnly As Boolean) Implements IMyTraceListener.WriteArticleActionLine
            If VerboseOnly AndAlso Not Verbose Then Exit Sub
            WriteArticleActionLine(Line, PluginName)
        End Sub
        Public MustOverride ReadOnly Property Uploadable() As Boolean Implements IMyTraceListener.Uploadable
        Public Overrides Sub Write(ByVal value As String) Implements IMyTraceListener.Write
            MyBase.Write(value)
        End Sub
        Public Overrides Sub WriteLine(ByVal value As String) Implements IMyTraceListener.WriteLine
            MyBase.WriteLine(value)
        End Sub
#End Region

        ' Protected and public members:
        Public Shared Function GetArticleTemplate(ByVal ArticleFullTitle As String, ByVal NS As Namespaces) As String
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
        Public Shared Function GetURL(ByVal ArticleFullTitle As String) As String
            Return Variables.URL & "index.php?title=" & _
               System.Web.HttpUtility.UrlEncode(ArticleFullTitle.Replace(" ", "_"))
        End Function
        Public MustOverride ReadOnly Property Verbose() As Boolean
    End Class

    ''' <summary>
    ''' An abstract class for building auto-uploading trace listeners
    ''' </summary>
    Public MustInherit Class TraceListenerUploadableBase
        Inherits TraceListenerBase
        Implements ITraceStatusProvider

        Protected mTraceStatus As TraceStatus, mUploadSettings As UploadableLogSettings2

        Event Upload(ByVal Sender As TraceListenerUploadableBase, ByRef Success As Boolean)

        ' Initialisation:
        Public Sub New(ByVal UploadSettings As UploadableLogSettings2, ByVal TraceStatus As TraceStatus)
            MyBase.New(TraceStatus.FileName)
            mTraceStatus = TraceStatus
            mUploadSettings = UploadSettings
        End Sub

        ' Overrides & Shadowing:
        Public Overrides ReadOnly Property Uploadable() As Boolean
            Get
                Return True
            End Get
        End Property
        Public Overrides ReadOnly Property Verbose() As Boolean
            Get
                Return mUploadSettings.LogVerbose
            End Get
        End Property
        Public Overridable Shadows Sub WriteLine(ByVal Line As String, Optional ByVal CheckCounter As Boolean = True)
            MyBase.WriteLine(Line)

            mTraceStatus.LogUpload += Line & Microsoft.VisualBasic.vbCrLf
            mTraceStatus.LinesWritten += 1

            If CheckCounter Then CheckCounterForUpload()
        End Sub
        Protected Overridable ReadOnly Property IsReadyToUpload() As Boolean
            Get
                Return mTraceStatus.LinesWrittenSinceLastUpload >= mUploadSettings.UploadMaxLines
            End Get
        End Property
        Public Overridable Sub CheckCounterForUpload()
            If IsReadyToUpload Then UploadLog()
        End Sub
        Public Shadows Sub Close(ByVal Upload As Boolean)
            If Upload Then UploadLog()
            mTraceStatus.Close()
            MyBase.Close()
        End Sub

        ' Methods:
        Public Overridable Function UploadLog(Optional ByVal NewJob As Boolean = False) As Boolean
            RaiseEvent Upload(Me, UploadLog)
            ' TODO: Logging: Get result, reset TraceStatus, write success/failure to log
            If NewJob Then
                mTraceStatus.PageNumber = 1
                mTraceStatus.StartDate = Date.Now
            Else
                mTraceStatus.PageNumber += 1
                mTraceStatus.LinesWrittenSinceLastUpload = 0
                mTraceStatus.LogUpload = ""
            End If
        End Function

        ' Properties:
        Public ReadOnly Property TraceStatus() As Uploader.TraceStatus Implements _
        Uploader.ITraceStatusProvider.TraceStatus
            Get
                Return mTraceStatus
            End Get
        End Property
        Public ReadOnly Property UploadSettings() As UploadableLogSettings2
            Get
                Return mUploadSettings
            End Get
        End Property
        Public Overridable ReadOnly Property PageName() As String
            Get
                Return String.Format("{0:ddMMyy} {1}", mTraceStatus.StartDate, mUploadSettings.UploadJobName)
            End Get
        End Property
    End Class
End Namespace