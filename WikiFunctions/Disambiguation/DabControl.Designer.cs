namespace WikiFunctions.Disambiguation
{
    partial class DabControl
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DabControl));
            this.txtViewer = new System.Windows.Forms.RichTextBox();
            this.cmboChoice = new System.Windows.Forms.ComboBox();
            this.txtCorrection = new System.Windows.Forms.TextBox();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnUndo = new System.Windows.Forms.Button();
            this.btnUnpipe = new System.Windows.Forms.Button();
            this.btnFlip = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtViewer
            // 
            resources.ApplyResources(this.txtViewer, "txtViewer");
            this.txtViewer.BackColor = System.Drawing.SystemColors.Window;
            this.txtViewer.Name = "txtViewer";
            this.txtViewer.ReadOnly = true;
            // 
            // cmboChoice
            // 
            resources.ApplyResources(this.cmboChoice, "cmboChoice");
            this.cmboChoice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmboChoice.FormattingEnabled = true;
            this.cmboChoice.Items.AddRange(new object[] {
            resources.GetString("cmboChoice.Items"),
            resources.GetString("cmboChoice.Items1"),
            resources.GetString("cmboChoice.Items2")});
            this.cmboChoice.Name = "cmboChoice";
            this.cmboChoice.SelectedIndexChanged += new System.EventHandler(this.cmboChoice_SelectedIndexChanged);
            // 
            // txtCorrection
            // 
            resources.ApplyResources(this.txtCorrection, "txtCorrection");
            this.txtCorrection.Name = "txtCorrection";
            this.txtCorrection.TextChanged += new System.EventHandler(this.txtCorrection_TextChanged);
            // 
            // btnReset
            // 
            resources.ApplyResources(this.btnReset, "btnReset");
            this.btnReset.Name = "btnReset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnUndo
            // 
            resources.ApplyResources(this.btnUndo, "btnUndo");
            this.btnUndo.Name = "btnUndo";
            this.btnUndo.UseVisualStyleBackColor = true;
            this.btnUndo.Click += new System.EventHandler(this.btnUndo_Click);
            // 
            // btnUnpipe
            // 
            resources.ApplyResources(this.btnUnpipe, "btnUnpipe");
            this.btnUnpipe.Name = "btnUnpipe";
            this.btnUnpipe.UseVisualStyleBackColor = true;
            this.btnUnpipe.Click += new System.EventHandler(this.btnUnpipe_Click);
            // 
            // btnFlip
            // 
            resources.ApplyResources(this.btnFlip, "btnFlip");
            this.btnFlip.Name = "btnFlip";
            this.btnFlip.UseVisualStyleBackColor = true;
            this.btnFlip.Click += new System.EventHandler(this.btnFlip_Click);
            // 
            // DabControl
            // 
            this.Controls.Add(this.btnFlip);
            this.Controls.Add(this.btnUnpipe);
            this.Controls.Add(this.btnUndo);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.txtCorrection);
            this.Controls.Add(this.cmboChoice);
            this.Controls.Add(this.txtViewer);
            resources.ApplyResources(this, "$this");
            this.Name = "DabControl";
            this.Load += new System.EventHandler(this.DabControl_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox txtViewer;
        private System.Windows.Forms.ComboBox cmboChoice;
        private System.Windows.Forms.TextBox txtCorrection;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnUndo;
        private System.Windows.Forms.Button btnUnpipe;
        private System.Windows.Forms.Button btnFlip;

    }
}
