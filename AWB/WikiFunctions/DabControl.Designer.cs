namespace WikiFunctions
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
            this.cmboChouce = new System.Windows.Forms.ComboBox();
            this.txtCorrection = new System.Windows.Forms.TextBox();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnUndo = new System.Windows.Forms.Button();
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
            this.txtViewer.Size = new System.Drawing.Size(348, 124);
            this.txtViewer.TabIndex = 0;
            this.txtViewer.Text = "";
            // 
            // cmboChouce
            // 
            this.cmboChouce.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmboChouce.FormattingEnabled = true;
            this.cmboChouce.Location = new System.Drawing.Point(357, 3);
            this.cmboChouce.Name = "cmboChouce";
            this.cmboChouce.Size = new System.Drawing.Size(240, 21);
            this.cmboChouce.TabIndex = 1;
            // 
            // txtCorrection
            // 
            this.txtCorrection.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCorrection.Location = new System.Drawing.Point(357, 30);
            this.txtCorrection.Multiline = true;
            this.txtCorrection.Name = "txtCorrection";
            this.txtCorrection.Size = new System.Drawing.Size(240, 68);
            this.txtCorrection.TabIndex = 2;
            // 
            // btnReset
            // 
            this.btnReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReset.Location = new System.Drawing.Point(441, 104);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(75, 23);
            this.btnReset.TabIndex = 3;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            // 
            // btnUndo
            // 
            this.btnUndo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUndo.Location = new System.Drawing.Point(522, 104);
            this.btnUndo.Name = "btnUndo";
            this.btnUndo.Size = new System.Drawing.Size(75, 23);
            this.btnUndo.TabIndex = 4;
            this.btnUndo.Text = "Undo";
            this.btnUndo.UseVisualStyleBackColor = true;
            // 
            // DabControl
            // 
            this.Controls.Add(this.btnUndo);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.txtCorrection);
            this.Controls.Add(this.cmboChouce);
            this.Controls.Add(this.txtViewer);
            this.MinimumSize = new System.Drawing.Size(600, 130);
            this.Name = "DabControl";
            this.Size = new System.Drawing.Size(600, 130);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox txtViewer;
        private System.Windows.Forms.ComboBox cmboChouce;
        private System.Windows.Forms.TextBox txtCorrection;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnUndo;

    }
}
