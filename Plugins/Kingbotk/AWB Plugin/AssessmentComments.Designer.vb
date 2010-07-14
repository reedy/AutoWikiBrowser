Namespace AutoWikiBrowser.Plugins.Kingbotk.ManualAssessments
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class AssessmentComments
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
        	Me.components = New System.ComponentModel.Container
        	Me.PictureBox1 = New System.Windows.Forms.PictureBox
        	Me.PhotoButton = New System.Windows.Forms.Button
        	Me.btnSave = New System.Windows.Forms.Button
        	Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        	Me.LeadButton = New System.Windows.Forms.Button
        	Me.SectionsButton = New System.Windows.Forms.Button
        	Me.SkipButton = New System.Windows.Forms.Button
        	Me.ToolStripStatusLabel1 = New System.Windows.Forms.ToolStripStatusLabel
        	Me.StatusStrip1 = New System.Windows.Forms.StatusStrip
        	CType(Me.PictureBox1,System.ComponentModel.ISupportInitialize).BeginInit
        	Me.StatusStrip1.SuspendLayout
        	Me.SuspendLayout
        	'
        	'PictureBox1
        	'
        	Me.PictureBox1.Image = Global.My.Resources.Resources.WP1
        	Me.PictureBox1.Location = New System.Drawing.Point(12, 12)
        	Me.PictureBox1.Name = "PictureBox1"
        	Me.PictureBox1.Size = New System.Drawing.Size(64, 61)
        	Me.PictureBox1.TabIndex = 3
        	Me.PictureBox1.TabStop = false
        	'
        	'PhotoButton
        	'
        	Me.PhotoButton.Location = New System.Drawing.Point(420, 33)
        	Me.PhotoButton.Name = "PhotoButton"
        	Me.PhotoButton.Size = New System.Drawing.Size(75, 23)
        	Me.PhotoButton.TabIndex = 7
        	Me.PhotoButton.Text = "Photo"
        	Me.ToolTip1.SetToolTip(Me.PhotoButton, "Article needs a photo")
        	Me.PhotoButton.UseVisualStyleBackColor = true
        	'
        	'btnSave
        	'
        	Me.btnSave.Enabled = false
        	Me.btnSave.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        	Me.btnSave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        	Me.btnSave.Location = New System.Drawing.Point(681, 12)
        	Me.btnSave.Name = "btnSave"
        	Me.btnSave.Size = New System.Drawing.Size(102, 32)
        	Me.btnSave.TabIndex = 31
        	Me.btnSave.Tag = "Save the Comments"
        	Me.btnSave.Text = "Save"
        	Me.ToolTip1.SetToolTip(Me.btnSave, "Save the Comments")
        	Me.btnSave.UseVisualStyleBackColor = true
        	'
        	'LeadButton
        	'
        	Me.LeadButton.Location = New System.Drawing.Point(119, 33)
        	Me.LeadButton.Name = "LeadButton"
        	Me.LeadButton.Size = New System.Drawing.Size(75, 23)
        	Me.LeadButton.TabIndex = 35
        	Me.LeadButton.Text = "Lead"
        	Me.ToolTip1.SetToolTip(Me.LeadButton, "Lead needs work")
        	Me.LeadButton.UseVisualStyleBackColor = true
        	'
        	'SectionsButton
        	'
        	Me.SectionsButton.Location = New System.Drawing.Point(261, 33)
        	Me.SectionsButton.Name = "SectionsButton"
        	Me.SectionsButton.Size = New System.Drawing.Size(75, 23)
        	Me.SectionsButton.TabIndex = 36
        	Me.SectionsButton.Text = "Sections"
        	Me.ToolTip1.SetToolTip(Me.SectionsButton, "Article needs sections")
        	Me.SectionsButton.UseVisualStyleBackColor = true
        	'
        	'SkipButton
        	'
        	Me.SkipButton.Location = New System.Drawing.Point(565, 12)
        	Me.SkipButton.Name = "SkipButton"
        	Me.SkipButton.Size = New System.Drawing.Size(100, 32)
        	Me.SkipButton.TabIndex = 34
        	Me.SkipButton.Text = "Skip"
        	Me.SkipButton.UseVisualStyleBackColor = true
        	'
        	'ToolStripStatusLabel1
        	'
        	Me.ToolStripStatusLabel1.Name = "ToolStripStatusLabel1"
        	Me.ToolStripStatusLabel1.Size = New System.Drawing.Size(0, 17)
        	'
        	'StatusStrip1
        	'
        	Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripStatusLabel1})
        	Me.StatusStrip1.Location = New System.Drawing.Point(0, 102)
        	Me.StatusStrip1.Name = "StatusStrip1"
        	Me.StatusStrip1.Size = New System.Drawing.Size(803, 22)
        	Me.StatusStrip1.TabIndex = 5
        	Me.StatusStrip1.Text = "StatusStrip1"
        	'
        	'AssessmentComments
        	'
        	Me.AutoScaleDimensions = New System.Drawing.SizeF(6!, 13!)
        	Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        	Me.ClientSize = New System.Drawing.Size(803, 124)
        	Me.Controls.Add(Me.SectionsButton)
        	Me.Controls.Add(Me.LeadButton)
        	Me.Controls.Add(Me.SkipButton)
        	Me.Controls.Add(Me.btnSave)
        	Me.Controls.Add(Me.PhotoButton)
        	Me.Controls.Add(Me.StatusStrip1)
        	Me.Controls.Add(Me.PictureBox1)
        	Me.Name = "AssessmentComments"
        	Me.ShowIcon = false
        	Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        	Me.Text = "Assessment Comments"
        	CType(Me.PictureBox1,System.ComponentModel.ISupportInitialize).EndInit
        	Me.StatusStrip1.ResumeLayout(false)
        	Me.StatusStrip1.PerformLayout
        	Me.ResumeLayout(false)
        	Me.PerformLayout
        End Sub
        Private WithEvents PictureBox1 As System.Windows.Forms.PictureBox
        Private WithEvents PhotoButton As System.Windows.Forms.Button
        Private WithEvents btnSave As System.Windows.Forms.Button
        Private WithEvents ToolTip1 As System.Windows.Forms.ToolTip
        Private WithEvents SkipButton As System.Windows.Forms.Button
        Private WithEvents LeadButton As System.Windows.Forms.Button
        Private WithEvents SectionsButton As System.Windows.Forms.Button
        Private WithEvents ToolStripStatusLabel1 As System.Windows.Forms.ToolStripStatusLabel
        Private WithEvents StatusStrip1 As System.Windows.Forms.StatusStrip
    End Class
End Namespace