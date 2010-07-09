Namespace AutoWikiBrowser.Plugins.Kingbotk.ManualAssessments
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class AssessmentForm
        Inherits System.Windows.Forms.Form

        'Form overrides dispose to clean up the component list.
        <System.Diagnostics.DebuggerNonUserCode()> _
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
            MyBase.Dispose(disposing)
        End Sub

        'Required by the Windows Form Designer
        Private components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.  
        'Do not modify it using the code editor.
        <System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
            Me.components = New System.ComponentModel.Container()
            Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
            Me.OK_Button = New System.Windows.Forms.Button()
            Me.Cancel_Button = New System.Windows.Forms.Button()
            Me.PictureBox1 = New System.Windows.Forms.PictureBox()
            Me.ClassCheckedListBox = New System.Windows.Forms.CheckedListBox()
            Me.ImportanceCheckedListBox = New System.Windows.Forms.CheckedListBox()
            Me.Label1 = New System.Windows.Forms.Label()
            Me.Label2 = New System.Windows.Forms.Label()
            Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
            Me.SettingsCheckedListBox = New System.Windows.Forms.CheckedListBox()
            Me.Label3 = New System.Windows.Forms.Label()
            Me.TableLayoutPanel1.SuspendLayout()
            CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'TableLayoutPanel1
            '
            Me.TableLayoutPanel1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.TableLayoutPanel1.ColumnCount = 2
            Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
            Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
            Me.TableLayoutPanel1.Controls.Add(Me.OK_Button, 0, 0)
            Me.TableLayoutPanel1.Controls.Add(Me.Cancel_Button, 1, 0)
            Me.TableLayoutPanel1.Location = New System.Drawing.Point(148, 251)
            Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
            Me.TableLayoutPanel1.RowCount = 1
            Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
            Me.TableLayoutPanel1.Size = New System.Drawing.Size(146, 29)
            Me.TableLayoutPanel1.TabIndex = 0
            '
            'OK_Button
            '
            Me.OK_Button.Anchor = System.Windows.Forms.AnchorStyles.None
            Me.OK_Button.DialogResult = System.Windows.Forms.DialogResult.OK
            Me.OK_Button.Location = New System.Drawing.Point(3, 3)
            Me.OK_Button.Name = "OK_Button"
            Me.OK_Button.Size = New System.Drawing.Size(67, 23)
            Me.OK_Button.TabIndex = 0
            Me.OK_Button.Text = "OK"
            '
            'Cancel_Button
            '
            Me.Cancel_Button.Anchor = System.Windows.Forms.AnchorStyles.None
            Me.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.Cancel_Button.Location = New System.Drawing.Point(76, 3)
            Me.Cancel_Button.Name = "Cancel_Button"
            Me.Cancel_Button.Size = New System.Drawing.Size(67, 23)
            Me.Cancel_Button.TabIndex = 1
            Me.Cancel_Button.Text = "Skip"
            '
            'PictureBox1
            '
            Me.PictureBox1.Image = Global.My.Resources.Resources.WP1
            Me.PictureBox1.Location = New System.Drawing.Point(12, 216)
            Me.PictureBox1.Name = "PictureBox1"
            Me.PictureBox1.Size = New System.Drawing.Size(64, 61)
            Me.PictureBox1.TabIndex = 2
            Me.PictureBox1.TabStop = False
            '
            'ClassCheckedListBox
            '
            Me.ClassCheckedListBox.CheckOnClick = True
            Me.ClassCheckedListBox.FormattingEnabled = True
            Me.ClassCheckedListBox.Items.AddRange(New Object() {"Unassessed", "Stub", "Start", "C", "B", "GA", "A", "FA", "Not Applicable", "Dab", "List", "FL"})
            Me.ClassCheckedListBox.Location = New System.Drawing.Point(12, 26)
            Me.ClassCheckedListBox.Name = "ClassCheckedListBox"
            Me.ClassCheckedListBox.Size = New System.Drawing.Size(120, 184)
            Me.ClassCheckedListBox.TabIndex = 3
            Me.ToolTip1.SetToolTip(Me.ClassCheckedListBox, "Article classification")
            '
            'ImportanceCheckedListBox
            '
            Me.ImportanceCheckedListBox.CheckOnClick = True
            Me.ImportanceCheckedListBox.FormattingEnabled = True
            Me.ImportanceCheckedListBox.Items.AddRange(New Object() {"Unassessed", "Low", "Mid", "High", "Top", "Not Applicable"})
            Me.ImportanceCheckedListBox.Location = New System.Drawing.Point(149, 26)
            Me.ImportanceCheckedListBox.Name = "ImportanceCheckedListBox"
            Me.ImportanceCheckedListBox.Size = New System.Drawing.Size(120, 94)
            Me.ImportanceCheckedListBox.TabIndex = 4
            Me.ToolTip1.SetToolTip(Me.ImportanceCheckedListBox, "Article importance/priority. Note: If you're tagging the talk page for more than " & _
                    "one WikiProject and the importance levels differ you'll have to tweak it manuall" & _
                    "y.")
            '
            'Label1
            '
            Me.Label1.AutoSize = True
            Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.Label1.Location = New System.Drawing.Point(8, 4)
            Me.Label1.Name = "Label1"
            Me.Label1.Size = New System.Drawing.Size(48, 20)
            Me.Label1.TabIndex = 5
            Me.Label1.Text = "Class"
            '
            'Label2
            '
            Me.Label2.AutoSize = True
            Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.Label2.Location = New System.Drawing.Point(145, 4)
            Me.Label2.Name = "Label2"
            Me.Label2.Size = New System.Drawing.Size(141, 20)
            Me.Label2.TabIndex = 6
            Me.Label2.Text = "Importance/Priority"
            '
            'SettingsCheckedListBox
            '
            Me.SettingsCheckedListBox.CheckOnClick = True
            Me.SettingsCheckedListBox.FormattingEnabled = True
            Me.SettingsCheckedListBox.Items.AddRange(New Object() {"Needs infobox", "Needs attention", "Photo requested", "Enter a comment", "Always enter a comment"})
            Me.SettingsCheckedListBox.Location = New System.Drawing.Point(149, 153)
            Me.SettingsCheckedListBox.Name = "SettingsCheckedListBox"
            Me.SettingsCheckedListBox.Size = New System.Drawing.Size(137, 79)
            Me.SettingsCheckedListBox.TabIndex = 7
            Me.ToolTip1.SetToolTip(Me.SettingsCheckedListBox, "Other settings")
            '
            'Label3
            '
            Me.Label3.AutoSize = True
            Me.Label3.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.Label3.Location = New System.Drawing.Point(145, 130)
            Me.Label3.Name = "Label3"
            Me.Label3.Size = New System.Drawing.Size(68, 20)
            Me.Label3.TabIndex = 8
            Me.Label3.Text = "Settings"
            '
            'AssessmentForm
            '
            Me.AcceptButton = Me.OK_Button
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.CancelButton = Me.Cancel_Button
            Me.ClientSize = New System.Drawing.Size(306, 292)
            Me.Controls.Add(Me.Label3)
            Me.Controls.Add(Me.SettingsCheckedListBox)
            Me.Controls.Add(Me.Label2)
            Me.Controls.Add(Me.Label1)
            Me.Controls.Add(Me.ImportanceCheckedListBox)
            Me.Controls.Add(Me.ClassCheckedListBox)
            Me.Controls.Add(Me.PictureBox1)
            Me.Controls.Add(Me.TableLayoutPanel1)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "AssessmentForm"
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
            Me.Text = "AssessmentForm"
            Me.TableLayoutPanel1.ResumeLayout(False)
            CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
        Private WithEvents OK_Button As System.Windows.Forms.Button
        Private WithEvents Cancel_Button As System.Windows.Forms.Button
        Private WithEvents PictureBox1 As System.Windows.Forms.PictureBox
        Private WithEvents ClassCheckedListBox As System.Windows.Forms.CheckedListBox
        Private WithEvents ImportanceCheckedListBox As System.Windows.Forms.CheckedListBox
        Private WithEvents Label1 As System.Windows.Forms.Label
        Private WithEvents Label2 As System.Windows.Forms.Label
        Private WithEvents ToolTip1 As System.Windows.Forms.ToolTip
        Private WithEvents SettingsCheckedListBox As System.Windows.Forms.CheckedListBox
        Private WithEvents Label3 As System.Windows.Forms.Label

    End Class
End Namespace