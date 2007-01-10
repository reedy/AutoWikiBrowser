namespace AutoWikiBrowser
{
    partial class DabForm
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
            this.tableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.btnDone = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOpenInBrowser = new System.Windows.Forms.Button();
            this.btnResetAll = new System.Windows.Forms.Button();
            this.btnUndoAll = new System.Windows.Forms.Button();
            this.btnAbort = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tableLayout
            // 
            this.tableLayout.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayout.AutoScroll = true;
            this.tableLayout.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.InsetDouble;
            this.tableLayout.ColumnCount = 1;
            this.tableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayout.Location = new System.Drawing.Point(12, 12);
            this.tableLayout.Name = "tableLayout";
            this.tableLayout.RowCount = 1;
            this.tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayout.Size = new System.Drawing.Size(637, 399);
            this.tableLayout.TabIndex = 0;
            // 
            // btnDone
            // 
            this.btnDone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDone.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnDone.Location = new System.Drawing.Point(574, 417);
            this.btnDone.Name = "btnDone";
            this.btnDone.Size = new System.Drawing.Size(75, 23);
            this.btnDone.TabIndex = 1;
            this.btnDone.Text = "Done";
            this.btnDone.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(493, 417);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOpenInBrowser
            // 
            this.btnOpenInBrowser.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnOpenInBrowser.Location = new System.Drawing.Point(12, 417);
            this.btnOpenInBrowser.Name = "btnOpenInBrowser";
            this.btnOpenInBrowser.Size = new System.Drawing.Size(110, 23);
            this.btnOpenInBrowser.TabIndex = 6;
            this.btnOpenInBrowser.Text = "Open in browser";
            this.btnOpenInBrowser.UseVisualStyleBackColor = true;
            this.btnOpenInBrowser.Click += new System.EventHandler(this.btnOpenInBrowser_Click);
            // 
            // btnResetAll
            // 
            this.btnResetAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnResetAll.Location = new System.Drawing.Point(128, 417);
            this.btnResetAll.Name = "btnResetAll";
            this.btnResetAll.Size = new System.Drawing.Size(110, 23);
            this.btnResetAll.TabIndex = 5;
            this.btnResetAll.Text = "Reset all";
            this.btnResetAll.UseVisualStyleBackColor = true;
            this.btnResetAll.Click += new System.EventHandler(this.btnResetAll_Click);
            // 
            // btnUndoAll
            // 
            this.btnUndoAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnUndoAll.Location = new System.Drawing.Point(244, 417);
            this.btnUndoAll.Name = "btnUndoAll";
            this.btnUndoAll.Size = new System.Drawing.Size(110, 23);
            this.btnUndoAll.TabIndex = 4;
            this.btnUndoAll.Text = "Undo all";
            this.btnUndoAll.UseVisualStyleBackColor = true;
            this.btnUndoAll.Click += new System.EventHandler(this.btnUndoAll_Click);
            // 
            // btnAbort
            // 
            this.btnAbort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAbort.DialogResult = System.Windows.Forms.DialogResult.Abort;
            this.btnAbort.Location = new System.Drawing.Point(412, 417);
            this.btnAbort.Name = "btnAbort";
            this.btnAbort.Size = new System.Drawing.Size(75, 23);
            this.btnAbort.TabIndex = 3;
            this.btnAbort.Text = "Abort";
            this.btnAbort.UseVisualStyleBackColor = true;
            // 
            // DabForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(661, 452);
            this.Controls.Add(this.btnAbort);
            this.Controls.Add(this.btnUndoAll);
            this.Controls.Add(this.btnResetAll);
            this.Controls.Add(this.btnOpenInBrowser);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnDone);
            this.Controls.Add(this.tableLayout);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "DabForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Disambiguation";
            this.Resize += new System.EventHandler(this.DabForm_Resize);
            this.Move += new System.EventHandler(this.DabForm_Move);
            this.Load += new System.EventHandler(this.DabForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayout;
        private System.Windows.Forms.Button btnDone;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOpenInBrowser;
        private System.Windows.Forms.Button btnResetAll;
        private System.Windows.Forms.Button btnUndoAll;
        private System.Windows.Forms.Button btnAbort;
    }
}