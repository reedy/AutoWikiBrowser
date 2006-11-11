' TODO: Bot mode: buttons or menu items for jobs (e.g. download living people, download tagged list, compare and save both, set settings including logging, etc)
' TODO: Logging: Job name/category changed - upload last page of old job, reset upload string and page number
Namespace AutoWikiBrowser.Plugins.SDKSoftware.Kingbotk
    Friend NotInheritable Class PluginLogging
        ' TODO: Job box might later need to get info from bot tab jobs (and have messagebox for auto-apply)
        Friend Settings As New Props
        Friend LoginDetails As New UsernamePassword
        Private mCategoryTextBox As TextBox, mInitialised As Boolean
        Private MyTrace As MyTrace
        Private mStartingUp As Boolean

        ' Properties
        Public WriteOnly Property LogFolder() As String
            Set(ByVal value As String)
                Settings.LogFolder = value
                FolderTextBox.Text = value
            End Set
        End Property
        Friend Property Initialised() As Boolean
            Get
                Return mInitialised
            End Get
            Set(ByVal value As Boolean)
                mInitialised = value
            End Set
        End Property

#Region "XML"
        Public Sub ReadXML(ByVal Reader As System.Xml.XmlTextReader, ByVal MyTrace As MyTrace)
            Dim NewProps As New Props

            NewProps.ReadXML(Reader) ' ReadXML values into a new settings object
            If Not Settings.Equals(NewProps) Then
                ApplySettingsToControls(NewProps)

                If mInitialised AndAlso (Not MyTrace.HaveOpenFile OrElse MessageBox.Show( _
                "Apply the new logging settings immediately?", "Logging settings", MessageBoxButtons.YesNo, _
                MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) = DialogResult.Yes) Then
                    GetSettingsFromControls()
                Else
                    WeHaveUnappliedChanges()
                End If
            End If
        End Sub
        Public Sub WriteXML(ByVal Writer As System.Xml.XmlTextWriter)
            Settings.WriteXML(Writer)
        End Sub
        Public Sub Reset()
            Dim NewProps As New Props

            If Not ApplyButton.Enabled AndAlso Not Settings.Equals(NewProps) Then WeHaveUnappliedChanges()

            ' Settings = NewProps ' No, that doesn't happen until the apply button is clicked
            ApplySettingsToControls(NewProps)
        End Sub
#End Region

#Region "Event handlers - supporting routines"
        Private Sub ApplySettingsToControls(ByVal SettingsObject As Props)
            With SettingsObject
                LogBadTagsCheckBox.Checked = .LogBadPages
                FolderTextBox.Text = .LogFolder
                VerboseCheckBox.Checked = .LogVerbose
                WikiLogCheckBox.Checked = .LogWiki
                XHTMLLogCheckBox.Checked = .LogXHTML
                UploadCheckBox.Checked = .UploadYN
                UploadWatchlistCheckBox.Checked = .UploadAddToWatchlist
                UploadJobNameTextBox.Text = .UploadJobName
                UploadLocationTextBox.Text = .UploadLocation
                UploadMaxLinesControl.Value = CDec(.UploadMaxLines)
                UploadOpenInBrowserCheckBox.Checked = .UploadOpenInBrowser
                UploadWikiProjectCheckBox.Checked = .UploadToWikiProjects
            End With
        End Sub
        Private Sub GetSettingsFromControls()
            DisableApplyButton()
            With Settings
                If System.IO.Directory.Exists(FolderTextBox.Text) Then
                    LogFolder = FolderTextBox.Text
                Else
                    If MessageBox.Show("Folder doesn't exist, using previous setting", "No such folder", _
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1) _
                    = Windows.Forms.DialogResult.Cancel Then
                        FolderTextBox.Text = .LogFolder
                    End If
                End If
                Dim blnJobNameHasChanged As Boolean = _
                   (Not .UploadJobName = _UploadJobNameTextBox.Text) _
                   OrElse (_UploadJobNameTextBox.Text = Props.conUploadCategoryIsJobName _
                   AndAlso Not .Category = mCategoryTextBox.Text)

                .LogBadPages = _LogBadTagsCheckBox.Checked
                .LogVerbose = _VerboseCheckBox.Checked
                .LogWiki = _WikiLogCheckBox.Checked
                .LogXHTML = _XHTMLLogCheckBox.Checked
                .UploadYN = _UploadCheckBox.Checked
                .UploadAddToWatchlist = _UploadWatchlistCheckBox.Checked
                .UploadJobName = _UploadJobNameTextBox.Text
                .UploadLocation = _UploadLocationTextBox.Text
                .UploadMaxLines = CInt(_UploadMaxLinesControl.Value)
                .Category = mCategoryTextBox.Text
                _UploadOpenInBrowserCheckBox.Checked = .UploadOpenInBrowser
                _UploadWikiProjectCheckBox.Checked = .UploadToWikiProjects

                If mInitialised Then MyTrace.PropertiesChange(blnJobNameHasChanged)
            End With
        End Sub
        Friend Sub WeHaveUnappliedChanges()
            If Not mStartingUp Then
                If MyTrace.HaveOpenFile Then
                    ApplyButton.Enabled = True
                    ApplyButton.BackColor = Drawing.Color.Red
                Else
                    GetSettingsFromControls()
                End If
            End If
        End Sub
        Private Sub DisableApplyButton()
            ApplyButton.Enabled = False
            ApplyButton.BackColor = System.Drawing.Color.FromKnownColor(Drawing.KnownColor.Control)
        End Sub
        Private Sub EnableDisableUploadCheckbox()
            If LogBadTagsCheckBox.Checked OrElse WikiLogCheckBox.Checked AndAlso Not UploadCheckBox.Enabled Then
                UploadCheckBox.Enabled = True
                EnableDisableUploadControls(UploadCheckBox.Checked)
            ElseIf Not LogBadTagsCheckBox.Checked AndAlso Not WikiLogCheckBox.Checked AndAlso UploadCheckBox.Enabled Then
                UploadCheckBox.Enabled = False
                EnableDisableUploadControls(False)
            End If
        End Sub
        Private Sub EnableDisableUploadControls(ByVal Enabled As Boolean)
            UploadJobNameTextBox.Enabled = Enabled
            UploadLocationTextBox.Enabled = Enabled
            UploadOpenInBrowserCheckBox.Enabled = Enabled
            UploadWatchlistCheckBox.Enabled = Enabled
            UploadWikiProjectCheckBox.Enabled = Enabled
            UploadMaxLinesControl.Enabled = Enabled
        End Sub
#End Region

#Region "Event Handlers"
        Private Sub ApplyButton_Click(ByVal sender As Object, ByVal e As EventArgs) Handles ApplyButton.Click
            If UploadJobNameTextBox.Text.Trim = "" Then
                MessageBox.Show("Please enter a job name", "Job name missing", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            Else
                GetSettingsFromControls()
            End If
        End Sub
        Private Sub LocationReset(ByVal sender As Object, ByVal e As EventArgs) Handles LocationResetToolStripMenuItem.Click
            UploadLocationTextBox.Text = Props.conUploadToUserSlashLogsToken
        End Sub
        Private Sub JobNameReset(ByVal sender As Object, ByVal e As EventArgs) Handles JobNameResetToolStripMenuItem.Click
            UploadJobNameTextBox.Text = Props.conUploadCategoryIsJobName
        End Sub
        Private Sub MaxLinesReset(ByVal sender As Object, ByVal e As EventArgs) Handles MaxLinesResetToolStripMenuItem.Click
            UploadMaxLinesControl.Value = 1000
        End Sub
        Private Sub SetLinesToMaximum(ByVal sender As Object, ByVal e As EventArgs) Handles SetToMaximumToolStripMenuItem.Click
            UploadMaxLinesControl.Value = UploadMaxLinesControl.Maximum
        End Sub
        Private Sub CloseAllButton_Click(ByVal sender As Object, ByVal e As EventArgs) Handles CloseAllButton.Click
            MyTrace.Close()
        End Sub
        Private Sub LogBadTagsCheckBox_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles LogBadTagsCheckBox.CheckedChanged
            EnableDisableUploadCheckbox()
            WeHaveUnappliedChanges()
        End Sub
        Private Sub WikiLogCheckBox_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles WikiLogCheckBox.CheckedChanged
            EnableDisableUploadCheckbox()
            WeHaveUnappliedChanges()
        End Sub
        Private Sub UploadCheckBox_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles UploadCheckBox.CheckedChanged
            EnableDisableUploadControls(UploadCheckBox.Checked)
            WeHaveUnappliedChanges()
        End Sub
        Private Sub XHTMLLogCheckBox_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles XHTMLLogCheckBox.CheckedChanged
            WeHaveUnappliedChanges()
        End Sub
        Private Sub VerboseCheckBox_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles VerboseCheckBox.CheckedChanged
            WeHaveUnappliedChanges()
        End Sub
        Private Sub FolderTextBox_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles FolderTextBox.TextChanged
            WeHaveUnappliedChanges()
        End Sub
        Private Sub FolderButton_Click(ByVal sender As Object, ByVal e As EventArgs) Handles FolderButton.Click
            If FolderBrowserDialog1.ShowDialog(Me) = Windows.Forms.DialogResult.OK Then
                FolderTextBox.Text = FolderBrowserDialog1.SelectedPath
                WeHaveUnappliedChanges()
            End If
        End Sub
        Private Sub UploadWikiProjectCheckBox_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles UploadWikiProjectCheckBox.CheckedChanged
            WeHaveUnappliedChanges()
        End Sub
        Private Sub UploadWatchlistCheckBox_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles UploadWatchlistCheckBox.CheckedChanged
            WeHaveUnappliedChanges()
        End Sub
        Private Sub UploadOpenInBrowserCheckBox_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles UploadOpenInBrowserCheckBox.CheckedChanged
            WeHaveUnappliedChanges()
        End Sub
        Private Sub UploadLocationTextBox_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles UploadLocationTextBox.TextChanged
            WeHaveUnappliedChanges()
        End Sub
        Private Sub UploadJobNameTextBox_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles UploadJobNameTextBox.TextChanged
            WeHaveUnappliedChanges()
        End Sub
        Private Sub UploadMaxLines_ValueChanged(ByVal sender As Object, ByVal e As EventArgs) Handles UploadMaxLinesControl.ValueChanged
            WeHaveUnappliedChanges()
        End Sub
#End Region

        Friend NotInheritable Class Props
            Inherits UploadableLogSettings2

            Private mLogBadPages As Boolean = True, mUploadToWikiProjects As Boolean = True, mCategory As String = ""
            Friend Const conUploadToUserSlashLogsToken As String = "$USER/Logs"
            Friend Const conUploadCategoryIsJobName As String = "$CATEGORY"
            Private Shared ReadOnly mPluginVersion As String = _
               System.Reflection.Assembly.GetExecutingAssembly.GetName.Version.ToString

            Public Sub New()
                MyBase.New()
                mUploadLocation = conUploadToUserSlashLogsToken
                mUploadJobName = conUploadCategoryIsJobName
            End Sub

            Public Shadows Function Equals(ByVal Compare As Props) As Boolean
                With Compare
                    If .LogBadPages = LogBadPages AndAlso .LogFolder = LogFolder AndAlso .LogVerbose = LogVerbose _
                    AndAlso .LogWiki = LogWiki AndAlso .LogXHTML = LogXHTML _
                    AndAlso .UploadAddToWatchlist = UploadAddToWatchlist AndAlso .UploadJobName = UploadJobName _
                    AndAlso .UploadLocation = UploadLocation AndAlso .UploadMaxLines = UploadMaxLines _
                    AndAlso .UploadOpenInBrowser = UploadOpenInBrowser _
                    AndAlso .UploadToWikiProjects = UploadToWikiProjects AndAlso .UploadYN = UploadYN _
                    AndAlso .Category = Category Then
                        Return True
                    End If
                End With
            End Function

            ' Additional properties:
            Public Shared ReadOnly Property PluginVersion() As String
                Get
                    Return mPluginVersion
                End Get
            End Property
            Public Shared ReadOnly Property AWBVersion() As String
                Get
                    Return Application.ProductVersion.ToString
                End Get
            End Property
            Public Property LogBadPages() As Boolean
                Get
                    Return mLogBadPages
                End Get
                Set(ByVal value As Boolean)
                    mLogBadPages = value
                End Set
            End Property
            Public Property UploadToWikiProjects() As Boolean
                Get
                    Return mUploadToWikiProjects
                End Get
                Private Set(ByVal value As Boolean)
                    mUploadToWikiProjects = value
                End Set
            End Property
            Public ReadOnly Property LinksToLog(ByVal ActivePlugins As List(Of PluginBase)) As List(Of LogEntry)
                Get
                    LinksToLog = New List(Of LogEntry)
                    LinksToLog.Add(New LogEntry(GlobbedUploadLocation, False))
                    If mUploadToWikiProjects Then
                        For Each Plugin As PluginBase In ActivePlugins
                            If Plugin.HasSharedLogLocation Then
                                LinksToLog.Add(New LogEntry(Plugin.SharedLogLocation, True))
                            End If
                        Next
                    End If
                    If LinksToLog.Count > 1 AndAlso Not PluginManager.LoggedIn Then _
                       Throw New System.Configuration.SettingsPropertyNotFoundException( _
                       "The plugin hasn't received the username from AWB")
                End Get
            End Property
            Friend ReadOnly Property GlobbedUploadLocation() As String
                Get
                    If mUploadLocation = conUploadToUserSlashLogsToken Then
                        Return "User:" & PluginManager.UserName & "/Logs"
                    Else
                        Return mUploadLocation
                    End If
                End Get
            End Property
            Public Property Category() As String
                Get
                    Return mCategory
                End Get
                Set(ByVal value As String)
                    mCategory = value
                End Set
            End Property
            Public ReadOnly Property WikifiedCategory() As String
                Get
                    If mCategory = "" Then Return "" Else Return "[[:Category:" & mCategory & "|" & mCategory & "]]"
                End Get
            End Property
            Public ReadOnly Property LogTitle() As String
                Get
                    LogTitle = mUploadJobName
                    If LogTitle = conUploadCategoryIsJobName Then LogTitle = mCategory
                End Get
            End Property

#Region "XML interface"
            Private Const conLogBadPages As String = "LogBadPages"
            Private Const conLogXHTML As String = "LogXHTML"
            Private Const conLogWiki As String = "LogWiki"
            Private Const conLogVerbose As String = "LogVerbose"
            Private Const conLogFolder As String = "LogFolder"
            Private Const conLogUploadMaxLines As String = "LogUploadMaxLines"
            Private Const conLogUploadYN As String = "LogUploadYN"
            Private Const conLogUploadToWikiProjects As String = "LogUploadWPs"
            Private Const conLogUploadWatchlist As String = "LogUploadWatch"
            Private Const conLogUploadOpenInBrowser As String = "LogUploadBrowser"
            Private Const conLogUploadLocation As String = "LogUploadLoc"
            Private Const conLogUploadJobName As String = "LogUploadJob"
            Public Sub ReadXML(ByVal Reader As System.Xml.XmlTextReader)
                mLogBadPages = PluginManager.XMLReadBoolean(Reader, conLogBadPages, LogBadPages)
                mLogFolder = PluginManager.XMLReadString(Reader, conLogFolder, LogFolder)
                mLogVerbose = PluginManager.XMLReadBoolean(Reader, conLogVerbose, LogVerbose)
                mLogWiki = PluginManager.XMLReadBoolean(Reader, conLogWiki, LogWiki)
                mLogXHTML = PluginManager.XMLReadBoolean(Reader, conLogXHTML, LogXHTML)
                mUploadYN = PluginManager.XMLReadBoolean(Reader, conLogUploadYN, UploadYN)
                mUploadAddToWatchlist = PluginManager.XMLReadBoolean(Reader, conLogUploadWatchlist, UploadAddToWatchlist)
                mUploadJobName = PluginManager.XMLReadString(Reader, conLogUploadJobName, UploadJobName)
                mUploadLocation = PluginManager.XMLReadString(Reader, conLogUploadLocation, UploadLocation)
                mUploadMaxLines = PluginManager.XMLReadInteger(Reader, conLogUploadMaxLines, UploadMaxLines)
                mUploadOpenInBrowser = PluginManager.XMLReadBoolean(Reader, conLogUploadOpenInBrowser, UploadOpenInBrowser)
                mUploadToWikiProjects = PluginManager.XMLReadBoolean(Reader, conLogUploadToWikiProjects, UploadToWikiProjects)
            End Sub
            Public Sub WriteXML(ByVal Writer As System.Xml.XmlTextWriter)
                Writer.WriteAttributeString(conLogBadPages, LogBadPages.ToString)
                Writer.WriteAttributeString(conLogFolder, LogFolder)
                Writer.WriteAttributeString(conLogVerbose, LogVerbose.ToString)
                Writer.WriteAttributeString(conLogWiki, LogWiki.ToString)
                Writer.WriteAttributeString(conLogXHTML, LogXHTML.ToString)
                Writer.WriteAttributeString(conLogUploadJobName, UploadJobName)
                Writer.WriteAttributeString(conLogUploadLocation, UploadLocation)
                Writer.WriteAttributeString(conLogUploadMaxLines, UploadMaxLines.ToString)
                Writer.WriteAttributeString(conLogUploadOpenInBrowser, UploadOpenInBrowser.ToString)
                Writer.WriteAttributeString(conLogUploadToWikiProjects, UploadToWikiProjects.ToString)
                Writer.WriteAttributeString(conLogUploadWatchlist, UploadAddToWatchlist.ToString)
                Writer.WriteAttributeString(conLogUploadYN, UploadYN.ToString)
            End Sub
#End Region
        End Class

        Public Sub New(ByVal pMyTrace As MyTrace, ByVal CategoryTextBox As TextBox)
            mStartingUp = True
            MyTrace = pMyTrace
            mCategoryTextBox = CategoryTextBox

            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            mStartingUp = False
        End Sub
    End Class
End Namespace