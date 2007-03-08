Namespace AutoWikiBrowser.Plugins.SDKSoftware.Kingbotk.Components
    ''' <summary>
    ''' Logging manager
    ''' </summary>
    ''' <remarks></remarks>
    Friend NotInheritable Class MyTrace
        Inherits TraceManager

        Private WithEvents LoggingSettings As PluginLogging
        Private ActivePlugins As List(Of PluginBase)
        Private Shared LogFolder As String
        Private webcontrol As WikiFunctions.Browser.WebControl
        Private LoggedIn As Boolean, LoggedInUser As String
        Private BusyCounter As Integer = 0

        Private Const conBadPages As String = "Bad"
        Private Const conWiki As String = "Wiki"
        Private Const conXHTML As String = "XHTML"

        ' The most important stuff:
        Private Sub Trace_Upload(ByVal Sender As TraceListenerUploadableBase, ByRef Success As Boolean)
            Dim frm As New UploadingPleaseWaitForm
            Dim Uploader As New Uploader.LogUploader

            LoggingSettings.Led1.Colour = Colour.Blue
            Application.DoEvents()
            frm.Show()

            If Sender.TraceStatus.LogName = conBadPages Then
                ' TODO: BadPages upload
            Else
                With LoggingSettings.Settings
                    Dim UploadTo As String = .GlobbedUploadLocation & "/" & Sender.PageName.Replace( _
                        PluginLogging.Props.conUploadCategoryIsJobName, .Category)

                    Try
                        Uploader.LogIn(LoggingSettings.LoginDetails)
                        Application.DoEvents()

                        Uploader.LogIt(Sender, .LogTitle, .WikifiedCategory, UploadTo, .LinksToLog(ActivePlugins), _
                        .UploadOpenInBrowser, .UploadAddToWatchlist, PluginManager.UserName, _
                        "*" & PluginManager.conWikiPlugin & " version " & PluginLogging.Props.PluginVersion & _
                        Microsoft.VisualBasic.vbCrLf & "*[[WP:AWB|AWB]] version " & _
                        PluginLogging.Props.AWBVersion & Microsoft.VisualBasic.vbCrLf, _
                        PluginManager.conWikiPluginBrackets & " " & LogUploader.conUploadingDefaultEditSummary, _
                        PluginManager.conWikiPluginBrackets & " " & LogUploader.conAddingLogEntryDefaultEditSummary)

                        Success = True
                        DirectCast(Sender.TraceStatus, TraceStatus).UploadsCount += 1
                    Catch
                        Success = False
                    Finally
                        UploadTo += " " & Sender.TraceStatus.PageNumber.ToString
                        If Success Then
                            Sender.WriteCommentAndNewLine("Log uploaded to " & UploadTo)
                        Else
                            Sender.WriteCommentAndNewLine( _
                               "LOG UPLOADING FAILED. Please manually upload this section to " & UploadTo)
                        End If
                    End Try
                End With
            End If

            frm.Dispose()
            If BusyCounter = 0 Then LoggingSettings.Led1.Colour = Colour.Red _
               Else LoggingSettings.Led1.Colour = Colour.Green
        End Sub
        Friend Sub Initialise()
            LoggingSettings.Initialised = True
            LogFolder = LoggingSettings.Settings.LogFolder

            With LoggingSettings.Settings
                If .LogBadPages OrElse .LogXHTML OrElse .LogWiki Then
                    If Not IO.Directory.Exists(.LogFolder) Then LogFolder = Application.StartupPath

                    If .LogBadPages Then NewBadPagesTraceListener()
                    If .LogXHTML Then NewXHTMLTraceListener()
                    If .LogWiki Then NewWikiTraceListener()
                End If
                For Each t As KeyValuePair(Of String, IMyTraceListener) In Listeners
                    t.Value.WriteBulletedLine("Start button clicked. Initialising log.", True, False, True)
                Next
            End With
            CheckWeHaveLogInDetails()
        End Sub

        ' State:
        Friend ReadOnly Property HaveOpenFile() As Boolean
            Get
                Return Listeners.Count > 0
            End Get
        End Property

        ' Private:
        Private Sub CheckWeHaveLogInDetails()
            If Me.Uploadable AndAlso Not LoggingSettings.LoginDetails.IsSet Then
                LoggingSettings.LoginDetails = New LoginForm().GetUsernamePassword
                If Not LoggingSettings.LoginDetails.IsSet Then _
                   Throw New System.Configuration.ConfigurationErrorsException("Error getting login details")
            End If
        End Sub
        Private Shared ReadOnly Property FilePrefix(ByVal LogFolder As String) As String
            Get
                Return String.Format("{1}\{0:MMM-d yyyy HHmm-ss.FF}", Date.Now, LogFolder)
            End Get
        End Property
        Private Sub NewBadPagesTraceListener()
            AddListener(conBadPages, New BadPagesTraceListener(FilePrefix(LoggingSettings.Settings.LogFolder) & _
               " skipped.txt", LoggingSettings.Settings, LoggingSettings))
        End Sub
        Private Sub NewXHTMLTraceListener()
            AddListener(conXHTML, New XHTMLTraceListener(FilePrefix(LoggingSettings.Settings.LogFolder) & _
               " log.html", LoggingSettings))
        End Sub
        Private Sub NewWikiTraceListener()
            AddListener(conWiki, New WikiTraceListener(FilePrefix(LoggingSettings.Settings.LogFolder) & " log.txt", _
               LoggingSettings))
        End Sub
        Private ReadOnly Property FileNameFromActiveListener(ByVal Key As String) As String
            Get
                Return DirectCast(Listeners(Key), ITraceStatusProvider).TraceStatus.FileName
            End Get
        End Property
        Private Sub RemoveListenerAndReplaceWithSameType(ByVal Key As String)
            Dim str As String = FileNameFromActiveListener(Key)
            RemoveListener(Key)
            NewWikiTraceListener()
            Listeners(Key).WriteCommentAndNewLine("logging continued from " & str)
        End Sub
        Private Sub Busy()
            If Listeners.Count = 0 Then Exit Sub
            BusyCounter += 1
            LoggingSettings.Led1.Colour = Colour.Green
        End Sub
        Private Sub NotBusy()
            If Listeners.Count = 0 Then Exit Sub
            BusyCounter -= 1
            If BusyCounter = 0 AndAlso Not LoggingSettings.Led1.Colour = Colour.Blue _
               Then LoggingSettings.Led1.Colour = Colour.Red
        End Sub

        ' Overrides:
        Public Overrides Sub AddListener(ByVal Key As String, ByVal Listener As IMyTraceListener)
            MyBase.AddListener(Key, Listener)
            If Listener.Uploadable Then _
               AddHandler (DirectCast(Listener, TraceListenerUploadableBase)).Upload, AddressOf Me.Trace_Upload
        End Sub
        Public Overrides Sub RemoveListener(ByVal Key As String)
            Dim Listener As IMyTraceListener = Listeners(Key)

            If Listener.Uploadable Then _
               RemoveHandler (DirectCast(Listener, TraceListenerUploadableBase)).Upload, AddressOf Me.Trace_Upload

            MyBase.RemoveListener(Key)
        End Sub
        Public Overrides Sub Close()
            Busy()

            Dim upload As Boolean

            If (ContainsKey(conBadPages) OrElse ContainsKey(conWiki)) Then
                If LoggingSettings.Settings.UploadYN AndAlso MessageBox.Show("Upload logs?", _
            "Logging", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) _
            = DialogResult.Yes Then Upload = True

                If Upload Then
                    If ContainsKey(conBadPages) Then DirectCast(Listeners(conBadPages), BadPagesTraceListener).UploadLog()
                    If ContainsKey(conWiki) Then DirectCast(Listeners(conWiki), WikiTraceListener).UploadLog()
                End If
            End If

            For Each t As KeyValuePair(Of String, IMyTraceListener) In Listeners
                t.Value.WriteCommentAndNewLine("closing all logs")
                If t.Value.Uploadable Then DirectCast(t.Value, TraceListenerUploadableBase).Close(Upload) _
                   Else t.Value.Close()
            Next
            Listeners.Clear()

            NotBusy()
        End Sub
#Region "Generic overrides"
        Public Overrides Sub ProcessingArticle(ByVal FullArticleTitle As String, ByVal NS As WikiFunctions.Namespaces)
            Busy()
            MyBase.ProcessingArticle(FullArticleTitle, NS)
            NotBusy()
        End Sub
        Public Overrides Sub SkippedArticle(ByVal SkippedBy As String, ByVal Reason As String)
            Busy()
            MyBase.SkippedArticle(SkippedBy, Reason)
            NotBusy()
        End Sub
        Public Overrides Sub SkippedArticleBadTag(ByVal SkippedBy As String, ByVal FullArticleTitle As String, ByVal NS As WikiFunctions.Namespaces)
            Busy()
            MyBase.SkippedArticleBadTag(SkippedBy, FullArticleTitle, NS)
            NotBusy()
        End Sub
        Public Overrides Sub SkippedArticleRedlink(ByVal FullArticleTitle As String, ByVal NS As WikiFunctions.Namespaces)
            Busy()
            MyBase.SkippedArticleRedlink(FullArticleTitle, NS)
            NotBusy()
        End Sub
        Public Overrides Sub Write(ByVal Text As String)
            Busy()
            MyBase.Write(Text)
            NotBusy()
        End Sub
        Public Overrides Sub WriteArticleActionLine(ByVal Line As String, ByVal PluginName As String)
            Busy()
            MyBase.WriteArticleActionLine(Line, PluginName)
            NotBusy()
        End Sub
        Public Overrides Sub WriteArticleActionLine1(ByVal Line As String, ByVal PluginName As String, ByVal VerboseOnly As Boolean)
            Busy()
            MyBase.WriteArticleActionLine1(Line, PluginName, VerboseOnly)
            NotBusy()
        End Sub
        Public Overrides Sub WriteBulletedLine(ByVal Line As String, ByVal Bold As Boolean, ByVal VerboseOnly As Boolean, Optional ByVal DateStamp As Boolean = False)
            Busy()
            MyBase.WriteBulletedLine(Line, Bold, VerboseOnly, DateStamp)
            NotBusy()
        End Sub
        Public Overrides Sub WriteComment(ByVal Line As String)
            Busy()
            MyBase.WriteComment(Line)
            NotBusy()
        End Sub
        Public Overrides Sub WriteCommentAndNewLine(ByVal Line As String)
            Busy()
            MyBase.WriteCommentAndNewLine(Line)
            NotBusy()
        End Sub
        Public Overrides Sub WriteLine(ByVal Line As String)
            Busy()
            MyBase.WriteLine(Line)
            NotBusy()
        End Sub
        Public Overrides Sub WriteTemplateAdded(ByVal Template As String, ByVal PluginName As String)
            Busy()
            MyBase.WriteTemplateAdded(Template, PluginName)
            NotBusy()
        End Sub
#End Region

        ' Callback from Settings control:
        Friend Sub PropertiesChange(ByVal JobNameHasChanged As Boolean)
            If LoggingSettings.Settings.LogFolder = LogFolder Then

                If LoggingSettings.Settings.LogXHTML Then
                    If Not ContainsKey(conXHTML) Then NewXHTMLTraceListener()
                ElseIf ContainsKey(conXHTML) Then
                    RemoveListener(conXHTML)
                End If

                If LoggingSettings.Settings.LogWiki Then
                    If ContainsKey(conWiki) Then
                        DirectCast(Listeners.Item(conWiki), WikiTraceListener).UploadLog(True)
                    Else
                        NewWikiTraceListener()
                    End If
                ElseIf ContainsKey(conWiki) Then
                    RemoveListener(conWiki)
                End If

                If LoggingSettings.Settings.LogBadPages Then
                    If ContainsKey(conBadPages) Then
                        DirectCast(Listeners.Item(conBadPages), BadPagesTraceListener).UploadLog(True)
                    Else
                        NewBadPagesTraceListener()
                    End If
                ElseIf ContainsKey(conBadPages) Then
                    RemoveListener(conBadPages)
                End If

            ElseIf HaveOpenFile Then ' folder has changed, close and reopen all active logs
                If ContainsKey(conWiki) Then RemoveListenerAndReplaceWithSameType(conWiki)
                If ContainsKey(conXHTML) Then RemoveListenerAndReplaceWithSameType(conXHTML)
                If ContainsKey(conBadPages) Then RemoveListenerAndReplaceWithSameType(conBadPages)
            End If

            CheckWeHaveLogInDetails()
        End Sub

        ' Trace listener child classes:
        ''' <summary>
        ''' Keeps track of logging statistics
        ''' </summary>
        ''' <remarks></remarks>
        Private NotInheritable Class TraceStatus
            Inherits Uploader.TraceStatus
            Private LinesLabel As Label, LinesSinceUploadLabel As Label, NumberOfUploadsLabel As Label
            Private Shared mUploadCount As Integer

            ' Initialisation
            Public Sub New(ByVal pLinesLabel As Label, ByVal pLinesSinceUploadLabel As Label, _
            ByVal pNumberOfUploadsLabel As Label, ByVal Uploadable As Boolean, ByVal FileN As String, _
            ByVal LogNameIs As String)
                MyBase.New(FileN, LogNameIs)
                LinesLabel = pLinesLabel
                LinesSinceUploadLabel = pLinesSinceUploadLabel
                If Not pNumberOfUploadsLabel Is Nothing Then
                    NumberOfUploadsLabel = pNumberOfUploadsLabel
                    NumberOfUploadsLabel.Text = mUploadCount.ToString
                End If
                pLinesLabel.Text = "0"
                If Uploadable Then pLinesSinceUploadLabel.Text = "0"
            End Sub

            ' Overrides:
            Public Overrides Sub Close()
                LinesLabel.Text = "N/A"
                LinesSinceUploadLabel.Text = "N/A"
            End Sub
            Public Overrides Property LinesWritten() As Integer
                Get
                    Return MyBase.LinesWritten
                End Get
                Set(ByVal value As Integer)
                    MyBase.LinesWritten = value
                    LinesLabel.Text = value.ToString
                End Set
            End Property
            Public Overrides Property LinesWrittenSinceLastUpload() As Integer
                Get
                    Return MyBase.LinesWrittenSinceLastUpload
                End Get
                Set(ByVal value As Integer)
                    MyBase.LinesWrittenSinceLastUpload = value
                    If Not LinesSinceUploadLabel Is Nothing Then LinesSinceUploadLabel.Text = value.ToString
                End Set
            End Property

            ' Extra:
            Public Property UploadsCount() As Integer
                Get
                    Return mUploadCount
                End Get
                Set(ByVal value As Integer)
                    mUploadCount = value
                    NumberOfUploadsLabel.Text = value.ToString
                End Set
            End Property
        End Class

        ''' <summary>
        ''' Logs in XHTML
        ''' </summary>
        ''' <remarks></remarks>
        Private NotInheritable Class XHTMLTraceListener
            Inherits Logging.XHTMLTraceListener
            Implements ITraceStatusProvider

            Private mTraceStatus As TraceStatus

            Public Sub New(ByVal FileName As String, ByVal LS As PluginLogging)
                MyBase.New(FileName, LS.Settings.LogVerbose)
                mTraceStatus = New TraceStatus(LS.XHTMLLinesLabel, Nothing, Nothing, False, FileName, conXHTML)
            End Sub
            Public ReadOnly Property TraceStatus() As WikiFunctions.Logging.Uploader.TraceStatus _
            Implements ITraceStatusProvider.TraceStatus
                Get
                    Return mTraceStatus
                End Get
            End Property
        End Class

        ''' <summary>
        ''' Logs in wiki format
        ''' </summary>
        ''' <remarks></remarks>
        Private NotInheritable Class WikiTraceListener
            Inherits Logging.WikiTraceListener

            Public Sub New(ByVal FileName As String, ByVal LS As PluginLogging)
                MyBase.New(LS.Settings, _
                   New TraceStatus(LS.WikiLinesLabel, LS.WikiLinesSinceUploadLabel, LS.UploadsCountLabel, _
                      LS.Settings.UploadYN, FileName, conWiki))
            End Sub
        End Class

        ''' <summary>
        ''' Logs pages which have bad templates
        ''' </summary>
        ''' <remarks></remarks>
        Private NotInheritable Class BadPagesTraceListener
            Inherits TraceListenerUploadableBase

            Public Sub New(ByVal FileName As String, ByVal UploadSettings As UploadableLogSettings2, ByVal LS As PluginLogging)
                MyBase.New(UploadSettings, _
                   New TraceStatus(LS.BadTagsLinesLabel, LS.BadTagsLinesSinceUploadLabel, LS.UploadsCountLabel, _
                   LS.Settings.UploadYN, FileName, conBadPages))
                WriteBulletedLine("Logging: [[User:Kingbotk/Plugin/WikiFunctions2|WikiFunctions2]].dll v" & _
                   WikiFunctions2.Version.ToString, False, False)
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
            Public Overrides Sub WriteLine(ByVal value As String, Optional ByVal CheckCounter As Boolean = True)
                MyBase.WriteLine(value, CheckCounter)
            End Sub
            Public Overrides Sub WriteComment(ByVal Line As String)
                MyBase.Write("<!-- " & Line & " -->")
            End Sub
            Public Overrides Sub WriteCommentAndNewLine(ByVal Line As String)
                MyBase.WriteLine("<!-- " & Line & " -->")
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

        Public Sub New(ByVal pActivePlugins As List(Of PluginBase))
            MyBase.new()
            ActivePlugins = pActivePlugins
        End Sub
        Friend WriteOnly Property LS() As PluginLogging
            Set(ByVal value As PluginLogging)
                LoggingSettings = value
            End Set
        End Property
    End Class
End Namespace
