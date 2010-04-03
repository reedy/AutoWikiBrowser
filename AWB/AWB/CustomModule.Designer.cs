namespace AutoWikiBrowser
{
    partial class CustomModule
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.txtCode = new System.Windows.Forms.TextBox();
            this.mnuTextBox = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuitemMakeFromTextBoxUndo = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator27 = new System.Windows.Forms.ToolStripSeparator();
            this.menuitemMakeFromTextBoxCut = new System.Windows.Forms.ToolStripMenuItem();
            this.menuitemMakeFromTextBoxCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.menuitemMakeFromTextBoxPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnMake = new System.Windows.Forms.Button();
            this.cmboLang = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblStart = new System.Windows.Forms.Label();
            this.lblEnd = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.chkModuleEnabled = new System.Windows.Forms.CheckBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.guideToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.chkFixedwidth = new System.Windows.Forms.CheckBox();
            this.lblBuilt = new System.Windows.Forms.Label();
            this.mnuTextBox.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtCode
            // 
            this.txtCode.AcceptsTab = true;
            this.txtCode.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCode.ContextMenuStrip = this.mnuTextBox;
            this.txtCode.Font = new System.Drawing.Font("Courier New", 9F);
            this.txtCode.Location = new System.Drawing.Point(11, 332);
            this.txtCode.MaxLength = 0;
            this.txtCode.Multiline = true;
            this.txtCode.Name = "txtCode";
            this.txtCode.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtCode.Size = new System.Drawing.Size(640, 160);
            this.txtCode.TabIndex = 10;
            this.txtCode.TabStop = false;
            this.txtCode.WordWrap = false;
            // 
            // mnuTextBox
            // 
            this.mnuTextBox.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuitemMakeFromTextBoxUndo,
            this.toolStripSeparator27,
            this.menuitemMakeFromTextBoxCut,
            this.menuitemMakeFromTextBoxCopy,
            this.menuitemMakeFromTextBoxPaste,
            this.toolStripSeparator1,
            this.selectAllToolStripMenuItem});
            this.mnuTextBox.Name = "mnuMakeFromTextBox";
            this.mnuTextBox.Size = new System.Drawing.Size(168, 126);
            // 
            // menuitemMakeFromTextBoxUndo
            // 
            this.menuitemMakeFromTextBoxUndo.Name = "menuitemMakeFromTextBoxUndo";
            this.menuitemMakeFromTextBoxUndo.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.menuitemMakeFromTextBoxUndo.Size = new System.Drawing.Size(167, 22);
            this.menuitemMakeFromTextBoxUndo.Text = "&Undo";
            this.menuitemMakeFromTextBoxUndo.Click += new System.EventHandler(this.menuitemMakeFromTextBoxUndo_Click);
            // 
            // toolStripSeparator27
            // 
            this.toolStripSeparator27.Name = "toolStripSeparator27";
            this.toolStripSeparator27.Size = new System.Drawing.Size(164, 6);
            // 
            // menuitemMakeFromTextBoxCut
            // 
            this.menuitemMakeFromTextBoxCut.Name = "menuitemMakeFromTextBoxCut";
            this.menuitemMakeFromTextBoxCut.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.menuitemMakeFromTextBoxCut.Size = new System.Drawing.Size(167, 22);
            this.menuitemMakeFromTextBoxCut.Text = "Cu&t";
            this.menuitemMakeFromTextBoxCut.Click += new System.EventHandler(this.menuitemMakeFromTextBoxCut_Click);
            // 
            // menuitemMakeFromTextBoxCopy
            // 
            this.menuitemMakeFromTextBoxCopy.Name = "menuitemMakeFromTextBoxCopy";
            this.menuitemMakeFromTextBoxCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.menuitemMakeFromTextBoxCopy.Size = new System.Drawing.Size(167, 22);
            this.menuitemMakeFromTextBoxCopy.Text = "&Copy";
            this.menuitemMakeFromTextBoxCopy.Click += new System.EventHandler(this.menuitemMakeFromTextBoxCopy_Click);
            // 
            // menuitemMakeFromTextBoxPaste
            // 
            this.menuitemMakeFromTextBoxPaste.Name = "menuitemMakeFromTextBoxPaste";
            this.menuitemMakeFromTextBoxPaste.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.menuitemMakeFromTextBoxPaste.Size = new System.Drawing.Size(167, 22);
            this.menuitemMakeFromTextBoxPaste.Text = "&Paste";
            this.menuitemMakeFromTextBoxPaste.Click += new System.EventHandler(this.menuitemMakeFromTextBoxPaste_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(164, 6);
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            this.selectAllToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.selectAllToolStripMenuItem.Text = "&Select All";
            this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.selectAllToolStripMenuItem_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(577, 30);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 8;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnMake
            // 
            this.btnMake.Enabled = false;
            this.btnMake.Location = new System.Drawing.Point(484, 30);
            this.btnMake.Name = "btnMake";
            this.btnMake.Size = new System.Drawing.Size(87, 23);
            this.btnMake.TabIndex = 7;
            this.btnMake.Text = "&Make module";
            this.btnMake.UseVisualStyleBackColor = true;
            this.btnMake.Click += new System.EventHandler(this.btnMake_Click);
            // 
            // cmboLang
            // 
            this.cmboLang.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmboLang.FormattingEnabled = true;
            this.cmboLang.Items.AddRange(new object[] {
            "C# 2.0",
            "VB .NET 2.0"});
            this.cmboLang.Location = new System.Drawing.Point(341, 32);
            this.cmboLang.Name = "cmboLang";
            this.cmboLang.Size = new System.Drawing.Size(137, 21);
            this.cmboLang.TabIndex = 5;
            this.cmboLang.SelectedIndexChanged += new System.EventHandler(this.cmboLang_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(280, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "&Language";
            // 
            // lblStart
            // 
            this.lblStart.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblStart.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStart.Location = new System.Drawing.Point(12, 76);
            this.lblStart.Name = "lblStart";
            this.lblStart.Size = new System.Drawing.Size(643, 253);
            this.lblStart.TabIndex = 9;
            this.lblStart.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // lblEnd
            // 
            this.lblEnd.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblEnd.Font = new System.Drawing.Font("Courier New", 9F);
            this.lblEnd.Location = new System.Drawing.Point(9, 495);
            this.lblEnd.Name = "lblEnd";
            this.lblEnd.Size = new System.Drawing.Size(643, 39);
            this.lblEnd.TabIndex = 11;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.lblStatus.Location = new System.Drawing.Point(82, 35);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(93, 13);
            this.lblStatus.TabIndex = 2;
            this.lblStatus.Text = "No module loaded";
            // 
            // chkModuleEnabled
            // 
            this.chkModuleEnabled.AutoSize = true;
            this.chkModuleEnabled.Location = new System.Drawing.Point(11, 34);
            this.chkModuleEnabled.Name = "chkModuleEnabled";
            this.chkModuleEnabled.Size = new System.Drawing.Size(65, 17);
            this.chkModuleEnabled.TabIndex = 1;
            this.chkModuleEnabled.Text = "&Enabled";
            this.chkModuleEnabled.UseVisualStyleBackColor = true;
            this.chkModuleEnabled.CheckedChanged += new System.EventHandler(this.chkModuleEnabled_CheckedChanged);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(664, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.guideToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // guideToolStripMenuItem
            // 
            this.guideToolStripMenuItem.Name = "guideToolStripMenuItem";
            this.guideToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.guideToolStripMenuItem.Text = "&Guide";
            this.guideToolStripMenuItem.Click += new System.EventHandler(this.guideToolStripMenuItem_Click);
            // 
            // chkFixedwidth
            // 
            this.chkFixedwidth.AutoSize = true;
            this.chkFixedwidth.Checked = true;
            this.chkFixedwidth.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkFixedwidth.Location = new System.Drawing.Point(11, 56);
            this.chkFixedwidth.Name = "chkFixedwidth";
            this.chkFixedwidth.Size = new System.Drawing.Size(100, 17);
            this.chkFixedwidth.TabIndex = 3;
            this.chkFixedwidth.Text = "&Fixed width font";
            this.chkFixedwidth.UseVisualStyleBackColor = true;
            this.chkFixedwidth.CheckedChanged += new System.EventHandler(this.chkFixedwidth_CheckedChanged);
            // 
            // lblBuilt
            // 
            this.lblBuilt.AutoSize = true;
            this.lblBuilt.Location = new System.Drawing.Point(280, 57);
            this.lblBuilt.Name = "lblBuilt";
            this.lblBuilt.Size = new System.Drawing.Size(139, 13);
            this.lblBuilt.TabIndex = 6;
            this.lblBuilt.Text = "Custom Module Built At: n/a";
            // 
            // CustomModule
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(664, 543);
            this.Controls.Add(this.lblBuilt);
            this.Controls.Add(this.chkFixedwidth);
            this.Controls.Add(this.chkModuleEnabled);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.lblEnd);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmboLang);
            this.Controls.Add(this.lblStart);
            this.Controls.Add(this.btnMake);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.txtCode);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "CustomModule";
            this.ShowIcon = false;
            this.Text = "Module";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CustomModule_FormClosing);
            this.mnuTextBox.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtCode;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnMake;
        private System.Windows.Forms.ComboBox cmboLang;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblStart;
        private System.Windows.Forms.Label lblEnd;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.CheckBox chkModuleEnabled;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem guideToolStripMenuItem;
        private System.Windows.Forms.CheckBox chkFixedwidth;
        private System.Windows.Forms.Label lblBuilt;
        private System.Windows.Forms.ContextMenuStrip mnuTextBox;
        private System.Windows.Forms.ToolStripMenuItem menuitemMakeFromTextBoxUndo;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator27;
        private System.Windows.Forms.ToolStripMenuItem menuitemMakeFromTextBoxCut;
        private System.Windows.Forms.ToolStripMenuItem menuitemMakeFromTextBoxCopy;
        private System.Windows.Forms.ToolStripMenuItem menuitemMakeFromTextBoxPaste;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
    }
}