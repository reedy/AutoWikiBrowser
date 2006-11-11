' Problems with the Editor class:
' No login-status variable
' Minimal or even no error handling
' I think it has to load at least one extra page than is optimal
' OTOH: It's simple and does the job. If anyone can fix this up to make it better, please do...

Namespace Logging.Uploader
    ''' <summary>
    ''' Object which contains details of target pages for log entries
    ''' </summary>
    Public NotInheritable Class LogEntry
        Public Location As String
        Public UserName As Boolean

        Public Sub New(ByVal pLocation As String, ByVal pUserName As Boolean)
            Location = pLocation
            UserName = pUserName
        End Sub
    End Class

    ''' <summary>
    ''' Stores the user's login details/cookies
    ''' </summary>
    Public NotInheritable Class UsernamePassword
        Public Username As String
        Public Password As String
        Public IsSet As Boolean
        Public HaveCookies As Boolean
        Public Cookies As System.Net.CookieCollection
    End Class

    ''' <summary>
    ''' A simple settings class for logging solutions
    ''' </summary>
    Public Class UploadableLogSettings
        Protected mLogVerbose As Boolean = True, mLogFolder As String = System.Windows.Forms.Application.StartupPath, _
           mUploadMaxLines As Integer = 1000, mUploadYN As Boolean, mUploadOpenInBrowser As Boolean = True

        Public Overridable Property LogVerbose() As Boolean
            Get
                Return mLogVerbose
            End Get
            Set(ByVal value As Boolean)
                mLogVerbose = value
            End Set
        End Property
        Public Overridable Property UploadMaxLines() As Integer
            Get
                Return mUploadMaxLines
            End Get
            Set(ByVal value As Integer)
                mUploadMaxLines = value
            End Set
        End Property
        Public Overridable Property UploadYN() As Boolean
            Get
                Return mUploadYN
            End Get
            Set(ByVal value As Boolean)
                mUploadYN = value
            End Set
        End Property
        Public Overridable Property UploadOpenInBrowser() As Boolean
            Get
                Return mUploadOpenInBrowser
            End Get
            Set(ByVal value As Boolean)
                mUploadOpenInBrowser = value
            End Set
        End Property
        Public Overridable Property LogFolder() As String
            Get
                Return mLogFolder
            End Get
            Set(ByVal value As String)
                mLogFolder = value
            End Set
        End Property
    End Class

    ''' <summary>
    ''' An extended base class with extra properties for a comprehensive logging solution
    ''' </summary>
    Public Class UploadableLogSettings2
        Inherits UploadableLogSettings
        Protected mLogXHTML As Boolean = False, mLogWiki As Boolean = True, _
           mUploadAddToWatchlist As Boolean = True, mUploadLocation As String = "", mUploadJobName As String = ""

        Public Overridable Property LogXHTML() As Boolean
            Get
                Return mLogXHTML
            End Get
            Set(ByVal value As Boolean)
                mLogXHTML = value
            End Set
        End Property
        Public Overridable Property LogWiki() As Boolean
            Get
                Return mLogWiki
            End Get
            Set(ByVal value As Boolean)
                mLogWiki = value
            End Set
        End Property
        Public Overridable Property UploadAddToWatchlist() As Boolean
            Get
                Return mUploadAddToWatchlist
            End Get
            Set(ByVal value As Boolean)
                mUploadAddToWatchlist = value
            End Set
        End Property
        Public Overridable Property UploadLocation() As String
            Get
                Return mUploadLocation
            End Get
            Set(ByVal value As String)
                mUploadLocation = value
            End Set
        End Property
        Public Overridable Property UploadJobName() As String
            Get
                Return mUploadJobName
            End Get
            Set(ByVal value As String)
                mUploadJobName = value
            End Set
        End Property
    End Class

    ''' <summary>
    ''' Implemented by classes which expose a TraceStatus object
    ''' </summary>
    Public Interface ITraceStatusProvider
        ReadOnly Property TraceStatus() As TraceStatus
    End Interface

    ''' <summary>
    ''' A class which keeps track of statistics and not-yet-uploaded log entries
    ''' </summary>
    Public Class TraceStatus
        Protected mLinesWritten As Integer, mLinesWrittenSinceLastUpload As Integer, mNextPageNumber As Integer = 1, _
        mFileName As String, mLogUpload As String, mLogName As String, mDate As Date = Date.Now
        Public Sub New(ByVal FileNameIs As String, ByVal LogNameIs As String)
            FileName = FileNameIs
            LogName = LogNameIs
        End Sub
        Public Overridable Sub Close()
        End Sub
        Public Overridable Property LinesWritten() As Integer ' overridden in plugin, but calls mybase
            Get
                Return mLinesWritten
            End Get
            Set(ByVal value As Integer)
                mLinesWritten = value
                LinesWrittenSinceLastUpload += 1
            End Set
        End Property
        Public Overridable Property LinesWrittenSinceLastUpload() As Integer ' overridden in plugin, but calls mybase
            Get
                Return mLinesWrittenSinceLastUpload
            End Get
            Set(ByVal value As Integer)
                mLinesWrittenSinceLastUpload = value
            End Set
        End Property
        Public Overridable Property PageNumber() As Integer
            Get
                Return mNextPageNumber
            End Get
            Set(ByVal value As Integer)
                mNextPageNumber = value
            End Set
        End Property
        Public Overridable Property FileName() As String
            Get
                Return mFileName
            End Get
            Set(ByVal value As String)
                mFileName = value
            End Set
        End Property
        Public Overridable Property LogUpload() As String
            Get
                Return mLogUpload
            End Get
            Set(ByVal value As String)
                mLogUpload = value
            End Set
        End Property
        Public Overridable Property LogName() As String
            Get
                Return mLogName
            End Get
            Set(ByVal value As String)
                mLogName = value
            End Set
        End Property
        Public Overridable Property StartDate() As Date
            Get
                Return mDate
            End Get
            Set(ByVal value As Date)
                mDate = value
            End Set
        End Property
    End Class

    ''' <summary>
    ''' A class which uploads logs to Wikipedia
    ''' </summary>
    Public Class LogUploader
        Inherits WikiFunctions.Editor
        Protected ReadOnly BotTag As String, TableHeaderUserName As String, TableHeaderNoUserName As String
        Protected Const NewLine As String = Microsoft.VisualBasic.vbCrLf
        Protected Const NewCell As String = Microsoft.VisualBasic.vbCrLf & "|"
        Public Const conAddingLogEntryDefaultEditSummary As String = "Adding log entry"
        Public Const conUploadingDefaultEditSummary As String = "Uploading log"

        Public Sub New()
            MyBase.New()
            BotTag = "|}<!--/bottag-->" ' doing it this way OUGHT to allow inherited classes to override
            TableHeaderUserName = "! Job !! Category !! Page # !! Performed By !! Date"
            TableHeaderNoUserName = "! Job !! Category !! Page # !! Date"
        End Sub
        Public Overridable Overloads Function LogIn(ByVal Username As String, ByVal Password As String) As _
        System.Net.CookieCollection
            MyBase.LogIn(Username, Password)
            Return logincookies
        End Function
        Public Overridable Overloads Sub LogIn(ByVal Cookies As System.Net.CookieCollection)
            logincookies = Cookies
        End Sub
        Public Overridable Overloads Sub LogIn(ByVal LoginDetails As UsernamePassword)
            With LoginDetails
                If .HaveCookies Then
                    LogIn(.Cookies)
                ElseIf .IsSet Then
                    .Cookies = LogIn(.Username, .Password)
                Else
                    Throw New System.Configuration.SettingsPropertyNotFoundException("Login details not found")
                End If
            End With
        End Sub
        Public Overridable Sub LogIt(ByVal Log As String, ByVal LogTitle As String, ByVal LogDetails As String, _
        ByVal UploadTo As String, ByVal LinksToLog As List(Of LogEntry), ByVal PageNumber As Integer, _
        ByVal StartDate As Date, ByVal OpenInBrowser As Boolean, ByVal AddToWatchlist As Boolean, _
        ByVal Username As String, ByVal LogHeader As String, Optional ByVal AddLogTemplate As Boolean = True, _
        Optional ByVal EditSummary As String = conUploadingDefaultEditSummary, _
        Optional ByVal LogSummaryEditSummary As String = conAddingLogEntryDefaultEditSummary)
            Dim UploadToNoSpaces As String = UploadTo.Replace(" ", "_")
            Dim strLogText As String = ""

            If AddLogTemplate Then strLogText = "{{log|name=" & UploadToNoSpaces & "|page=" & _
               PageNumber.ToString & "}}" & NewLine
            strLogText += LogHeader & Log

            Application.DoEvents()

            If AddToWatchlist Then
                MyBase.EditPage(UploadToNoSpaces, strLogText, EditSummary, False, True)
            Else
                MyBase.EditPage(UploadToNoSpaces, strLogText, EditSummary, False)
            End If

            Application.DoEvents()

            For Each LogEntry As LogEntry In LinksToLog
                DoLogEntry(LogTitle, LogDetails, PageNumber, StartDate, UploadTo, _
                   LogEntry.Location, LogEntry.UserName, LogSummaryEditSummary, Username)
                Application.DoEvents()
            Next

            If OpenInBrowser Then OpenLogInBrowser(UploadTo)
        End Sub
        Public Overridable Sub LogIt(ByVal Sender As TraceListenerUploadableBase, ByVal LogTitle As String, _
        ByVal LogDetails As String, ByVal UploadToWithoutPageNumber As String, ByVal LinksToLog As List(Of LogEntry), _
        ByVal OpenInBrowser As Boolean, ByVal AddToWatchlist As Boolean, ByVal Username As String, _
        ByVal LogHeader As String, ByVal EditSummary As String, ByVal LogSummaryEditSummary As String)
            LogIt(Sender.TraceStatus.LogUpload, LogTitle, LogDetails, UploadToWithoutPageNumber & " " & _
               Sender.TraceStatus.PageNumber.ToString, LinksToLog, Sender.TraceStatus.PageNumber, _
               Sender.TraceStatus.StartDate, OpenInBrowser, AddToWatchlist, Username, "{{log|name=" & _
               UploadToWithoutPageNumber & "|page=" & Sender.TraceStatus.PageNumber.ToString & "}}" & NewLine & _
               LogHeader, False, EditSummary, LogSummaryEditSummary)
        End Sub
        Public Overridable Sub LogIt(ByVal Log As String, ByVal LogTitle As String, ByVal LogDetails As String, _
        ByVal UploadTo As String, ByVal LinkToLog As LogEntry, ByVal PageNumber As Integer, _
        ByVal StartDate As Date, ByVal OpenInBrowser As Boolean, ByVal AddToWatchlist As Boolean, _
        ByVal Username As String, Optional ByVal LogHeader As String = "")
            LogIt(Log, LogTitle, LogDetails, UploadTo, New List(Of LogEntry)({LinkToLog}), PageNumber, StartDate, _
               OpenInBrowser, AddToWatchlist, Username, LogHeader)
        End Sub
        Public Overridable Sub LogIt(ByVal Log As String, ByVal LogTitle As String, ByVal LogDetails As String, _
        ByVal UploadTo As String, ByVal LogEntryLocation As String, ByVal PageNumber As Integer, _
        ByVal StartDate As Date)
            LogIt(Log, LogTitle, LogDetails, UploadTo, _
               New List(Of LogEntry)({New LogEntry(LogEntryLocation, False)}), PageNumber, StartDate, _
               False, False, "", "")
        End Sub
        Protected Overridable Sub DoLogEntry(ByVal LogTitle As String, ByVal LogDetails As String, _
        ByVal PageNumber As Integer, ByVal StartDate As Date, ByVal UploadTo As String, _
        ByVal Location As String, ByVal UserNameCell As Boolean, ByVal EditSummary As String, _
        Optional ByVal Username As String = "")

            Dim strExistingText As String = WikiFunctions.Editor.GetWikiText(Location)

            Application.DoEvents()

            Dim TableAddition As String = "|-" & NewCell & "[[" & UploadTo & "|" & LogTitle & "]]" & _
                NewCell & LogDetails & NewCell & "[[" & UploadTo & "|" & PageNumber.ToString & "]]" & _
                IIf(UserNameCell, NewCell & "[[User:" & Username & "|" & Username & "]]", "").ToString & _
                NewCell & String.Format("[[{0:d MMMM}]] [[{0:yyyy}]]", StartDate) & NewLine & BotTag

            If strExistingText.Contains(BotTag) Then
                MyBase.EditPage(Location, strExistingText.Replace(BotTag, TableAddition), EditSummary, False, True)
            Else
                MyBase.EditPageAppend(Location, NewLine & "<!--bottag-->" & NewLine & _
                   "{| class=""wikitable"" width=""100%""" & NewLine & _
                   IIf(UserNameCell, TableHeaderUserName, TableHeaderNoUserName).ToString & NewLine & _
                   TableAddition, EditSummary, False)
            End If
        End Sub
        Protected Overridable Sub OpenLogInBrowser(ByVal UploadTo As String)
            System.Diagnostics.Process.Start("http://en.wikipedia.org/wiki/" & UploadTo)
        End Sub
    End Class
End Namespace
