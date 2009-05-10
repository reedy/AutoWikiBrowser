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
            this.txtViewer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtViewer.BackColor = System.Drawing.SystemColors.Window;
            this.txtViewer.Location = new System.Drawing.Point(3, 3);
            this.txtViewer.Name = "txtViewer";
            this.txtViewer.ReadOnly = true;
            this.txtViewer.Size = new System.Drawing.Size(351, 124);
            this.txtViewer.TabIndex = 5;
            this.txtViewer.Text = "";
            // 
            // cmboChoice
            // 
            this.cmboChoice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmboChoice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmboChoice.FormattingEnabled = true;
            this.cmboChoice.Location = new System.Drawing.Point(360, 0);
            this.cmboChoice.MaxDropDownItems = 15;
            this.cmboChoice.Name = "cmboChoice";
            this.cmboChoice.Size = new System.Drawing.Size(237, 22);
            this.cmboChoice.TabIndex = 0;
            this.cmboChoice.SelectedIndexChanged += new System.EventHandler(this.cmboChoice_SelectedIndexChanged);
            // 
            // txtCorrection
            // 
            this.txtCorrection.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCorrection.Location = new System.Drawing.Point(360, 28);
            this.txtCorrection.Multiline = true;
            this.txtCorrection.Name = "txtCorrection";
            this.txtCorrection.Size = new System.Drawing.Size(237, 68);
            this.txtCorrection.TabIndex = 1;
            this.txtCorrection.TextChanged += new System.EventHandler(this.txtCorrection_TextChanged);
            // 
            // btnReset
            // 
            this.btnReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReset.Location = new System.Drawing.Point(482, 104);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(55, 23);
            this.btnReset.TabIndex = 3;
            this.btnReset.Text = "&Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnUndo
            // 
            this.btnUndo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUndo.Location = new System.Drawing.Point(543, 104);
            this.btnUndo.Name = "btnUndo";
            this.btnUndo.Size = new System.Drawing.Size(55, 23);
            this.btnUndo.TabIndex = 4;
            this.btnUndo.Text = "&Undo";
            this.btnUndo.UseVisualStyleBackColor = true;
            this.btnUndo.Click += new System.EventHandler(this.btnUndo_Click);
            // 
            // btnUnpipe
            // 
            this.btnUnpipe.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUnpipe.Location = new System.Drawing.Point(360, 104);
            this.btnUnpipe.Name = "btnUnpipe";
            this.btnUnpipe.Size = new System.Drawing.Size(55, 23);
            this.btnUnpipe.TabIndex = 2;
            this.btnUnpipe.Text = "Un&pipe";
            this.btnUnpipe.UseVisualStyleBackColor = true;
            this.btnUnpipe.Click += new System.EventHandler(this.btnUnpipe_Click);
            // 
            // btnFlip
            // 
            this.btnFlip.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFlip.Location = new System.Drawing.Point(421, 104);
            this.btnFlip.Name = "btnFlip";
            this.btnFlip.Size = new System.Drawing.Size(55, 23);
            this.btnFlip.TabIndex = 6;
            this.btnFlip.Text = "&Flip";
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
            this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.MaximumSize = new System.Drawing.Size(0, 130);
            this.MinimumSize = new System.Drawing.Size(600, 130);
            this.Name = "DabControl";
            this.Size = new System.Drawing.Size(600, 130);
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
