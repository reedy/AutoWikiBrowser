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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CustomModule));
            this.mnuTextBox = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuitemMakeFromTextBoxUndo = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator27 = new System.Windows.Forms.ToolStripSeparator();
            this.menuitemMakeFromTextBoxCut = new System.Windows.Forms.ToolStripMenuItem();
            this.menuitemMakeFromTextBoxCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.menuitemMakeFromTextBoxPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.goToLineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripTextBox1 = new System.Windows.Forms.ToolStripTextBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnMake = new System.Windows.Forms.Button();
            this.cmboLang = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.chkModuleEnabled = new System.Windows.Forms.CheckBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showOnlyCodeBoxToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.guideToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.manualToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.chkFixedwidth = new System.Windows.Forms.CheckBox();
            this.lblBuilt = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblStart = new System.Windows.Forms.Label();
            this.txtCode = new System.Windows.Forms.TextBox();
            this.lblEnd = new System.Windows.Forms.Label();
            this.mnuTextBox.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
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
            this.selectAllToolStripMenuItem,
            this.toolStripSeparator2,
            this.goToLineToolStripMenuItem});
            this.mnuTextBox.Name = "mnuMakeFromTextBox";
            resources.ApplyResources(this.mnuTextBox, "mnuTextBox");
            // 
            // menuitemMakeFromTextBoxUndo
            // 
            this.menuitemMakeFromTextBoxUndo.Name = "menuitemMakeFromTextBoxUndo";
            resources.ApplyResources(this.menuitemMakeFromTextBoxUndo, "menuitemMakeFromTextBoxUndo");
            this.menuitemMakeFromTextBoxUndo.Click += new System.EventHandler(this.menuitemMakeFromTextBoxUndo_Click);
            // 
            // toolStripSeparator27
            // 
            this.toolStripSeparator27.Name = "toolStripSeparator27";
            resources.ApplyResources(this.toolStripSeparator27, "toolStripSeparator27");
            // 
            // menuitemMakeFromTextBoxCut
            // 
            this.menuitemMakeFromTextBoxCut.Name = "menuitemMakeFromTextBoxCut";
            resources.ApplyResources(this.menuitemMakeFromTextBoxCut, "menuitemMakeFromTextBoxCut");
            this.menuitemMakeFromTextBoxCut.Click += new System.EventHandler(this.menuitemMakeFromTextBoxCut_Click);
            // 
            // menuitemMakeFromTextBoxCopy
            // 
            this.menuitemMakeFromTextBoxCopy.Name = "menuitemMakeFromTextBoxCopy";
            resources.ApplyResources(this.menuitemMakeFromTextBoxCopy, "menuitemMakeFromTextBoxCopy");
            this.menuitemMakeFromTextBoxCopy.Click += new System.EventHandler(this.menuitemMakeFromTextBoxCopy_Click);
            // 
            // menuitemMakeFromTextBoxPaste
            // 
            this.menuitemMakeFromTextBoxPaste.Name = "menuitemMakeFromTextBoxPaste";
            resources.ApplyResources(this.menuitemMakeFromTextBoxPaste, "menuitemMakeFromTextBoxPaste");
            this.menuitemMakeFromTextBoxPaste.Click += new System.EventHandler(this.menuitemMakeFromTextBoxPaste_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            resources.ApplyResources(this.selectAllToolStripMenuItem, "selectAllToolStripMenuItem");
            this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.selectAllToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // goToLineToolStripMenuItem
            // 
            this.goToLineToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripTextBox1});
            this.goToLineToolStripMenuItem.Name = "goToLineToolStripMenuItem";
            resources.ApplyResources(this.goToLineToolStripMenuItem, "goToLineToolStripMenuItem");
            // 
            // toolStripTextBox1
            // 
            resources.ApplyResources(this.toolStripTextBox1, "toolStripTextBox1");
            this.toolStripTextBox1.Name = "toolStripTextBox1";
            this.toolStripTextBox1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.toolStripTextBox1_KeyPress);
            this.toolStripTextBox1.Click += new System.EventHandler(this.toolStripTextBox1_Click);
            // 
            // btnClose
            // 
            resources.ApplyResources(this.btnClose, "btnClose");
            this.btnClose.Name = "btnClose";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnMake
            // 
            resources.ApplyResources(this.btnMake, "btnMake");
            this.btnMake.Name = "btnMake";
            this.btnMake.UseVisualStyleBackColor = true;
            this.btnMake.Click += new System.EventHandler(this.btnMake_Click);
            // 
            // cmboLang
            // 
            resources.ApplyResources(this.cmboLang, "cmboLang");
            this.cmboLang.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmboLang.FormattingEnabled = true;
            this.cmboLang.Items.AddRange(new object[] {
            resources.GetString("cmboLang.Items"),
            resources.GetString("cmboLang.Items1")});
            this.cmboLang.Name = "cmboLang";
            this.cmboLang.SelectedIndexChanged += new System.EventHandler(this.cmboLang_SelectedIndexChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // lblStatus
            // 
            resources.ApplyResources(this.lblStatus, "lblStatus");
            this.lblStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.lblStatus.Name = "lblStatus";
            // 
            // chkModuleEnabled
            // 
            resources.ApplyResources(this.chkModuleEnabled, "chkModuleEnabled");
            this.chkModuleEnabled.Name = "chkModuleEnabled";
            this.chkModuleEnabled.UseVisualStyleBackColor = true;
            this.chkModuleEnabled.CheckedChanged += new System.EventHandler(this.chkModuleEnabled_CheckedChanged);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewToolStripMenuItem,
            this.helpToolStripMenuItem});
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
            this.menuStrip1.Name = "menuStrip1";
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showOnlyCodeBoxToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            resources.ApplyResources(this.viewToolStripMenuItem, "viewToolStripMenuItem");
            // 
            // showOnlyCodeBoxToolStripMenuItem
            // 
            this.showOnlyCodeBoxToolStripMenuItem.CheckOnClick = true;
            this.showOnlyCodeBoxToolStripMenuItem.Name = "showOnlyCodeBoxToolStripMenuItem";
            resources.ApplyResources(this.showOnlyCodeBoxToolStripMenuItem, "showOnlyCodeBoxToolStripMenuItem");
            this.showOnlyCodeBoxToolStripMenuItem.CheckedChanged += new System.EventHandler(this.showOnlyCodeBoxToolStripMenuItem_CheckedChanged);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.guideToolStripMenuItem,
            this.manualToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            resources.ApplyResources(this.helpToolStripMenuItem, "helpToolStripMenuItem");
            // 
            // guideToolStripMenuItem
            // 
            this.guideToolStripMenuItem.Name = "guideToolStripMenuItem";
            resources.ApplyResources(this.guideToolStripMenuItem, "guideToolStripMenuItem");
            this.guideToolStripMenuItem.Click += new System.EventHandler(this.guideToolStripMenuItem_Click);
            // 
            // manualToolStripMenuItem
            // 
            this.manualToolStripMenuItem.Name = "manualToolStripMenuItem";
            resources.ApplyResources(this.manualToolStripMenuItem, "manualToolStripMenuItem");
            this.manualToolStripMenuItem.Click += new System.EventHandler(this.manualToolStripMenuItem_Click);
            // 
            // chkFixedwidth
            // 
            resources.ApplyResources(this.chkFixedwidth, "chkFixedwidth");
            this.chkFixedwidth.Checked = true;
            this.chkFixedwidth.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkFixedwidth.Name = "chkFixedwidth";
            this.chkFixedwidth.UseVisualStyleBackColor = true;
            this.chkFixedwidth.CheckedChanged += new System.EventHandler(this.chkFixedwidth_CheckedChanged);
            // 
            // lblBuilt
            // 
            resources.ApplyResources(this.lblBuilt, "lblBuilt");
            this.lblBuilt.Name = "lblBuilt";
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Controls.Add(this.lblStart);
            this.panel1.Controls.Add(this.txtCode);
            this.panel1.Controls.Add(this.lblEnd);
            this.panel1.Name = "panel1";
            // 
            // lblStart
            // 
            resources.ApplyResources(this.lblStart, "lblStart");
            this.lblStart.Name = "lblStart";
            // 
            // txtCode
            // 
            this.txtCode.AcceptsTab = true;
            resources.ApplyResources(this.txtCode, "txtCode");
            this.txtCode.ContextMenuStrip = this.mnuTextBox;
            this.txtCode.Name = "txtCode";
            this.txtCode.TabStop = false;
            // 
            // lblEnd
            // 
            resources.ApplyResources(this.lblEnd, "lblEnd");
            this.lblEnd.Name = "lblEnd";
            // 
            // CustomModule
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.lblBuilt);
            this.Controls.Add(this.chkFixedwidth);
            this.Controls.Add(this.chkModuleEnabled);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmboLang);
            this.Controls.Add(this.btnMake);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "CustomModule";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CustomModule_FormClosing);
            this.mnuTextBox.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnMake;
        private System.Windows.Forms.ComboBox cmboLang;
        private System.Windows.Forms.Label label1;
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
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblStart;
        private System.Windows.Forms.TextBox txtCode;
        private System.Windows.Forms.Label lblEnd;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showOnlyCodeBoxToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem goToLineToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox1;
        private System.Windows.Forms.ToolStripMenuItem manualToolStripMenuItem;
    }
}