namespace WikiFunctions
{
    partial class ListComparer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ListComparer));
            this.lbBoth = new System.Windows.Forms.ListBox();
            this.btnOpen1 = new System.Windows.Forms.Button();
            this.btnOpen2 = new System.Windows.Forms.Button();
            this.btnGo = new System.Windows.Forms.Button();
            this.openListDialog = new System.Windows.Forms.OpenFileDialog();
            this.lblFirst = new System.Windows.Forms.Label();
            this.lblSecond = new System.Windows.Forms.Label();
            this.lblBoth = new System.Windows.Forms.Label();
            this.saveListDialog = new System.Windows.Forms.SaveFileDialog();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnSave2 = new System.Windows.Forms.Button();
            this.btnSave1 = new System.Windows.Forms.Button();
            this.lbSecond = new WikiFunctions.ListBox2();
            this.lbFirst = new WikiFunctions.ListBox2();
            this.SuspendLayout();
            // 
            // lbBoth
            // 
            this.lbBoth.FormattingEnabled = true;
            this.lbBoth.Location = new System.Drawing.Point(396, 28);
            this.lbBoth.Name = "lbBoth";
            this.lbBoth.Size = new System.Drawing.Size(130, 186);
            this.lbBoth.TabIndex = 2;
            // 
            // btnOpen1
            // 
            this.btnOpen1.Location = new System.Drawing.Point(9, 238);
            this.btnOpen1.Name = "btnOpen1";
            this.btnOpen1.Size = new System.Drawing.Size(75, 23);
            this.btnOpen1.TabIndex = 3;
            this.btnOpen1.Text = "Open list 1";
            this.btnOpen1.UseVisualStyleBackColor = true;
            this.btnOpen1.Click += new System.EventHandler(this.btnOpen1_Click);
            // 
            // btnOpen2
            // 
            this.btnOpen2.Location = new System.Drawing.Point(169, 238);
            this.btnOpen2.Name = "btnOpen2";
            this.btnOpen2.Size = new System.Drawing.Size(75, 23);
            this.btnOpen2.TabIndex = 4;
            this.btnOpen2.Text = "Open list 2";
            this.btnOpen2.UseVisualStyleBackColor = true;
            this.btnOpen2.Click += new System.EventHandler(this.btnOpen2_Click);
            // 
            // btnGo
            // 
            this.btnGo.Location = new System.Drawing.Point(322, 88);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(57, 56);
            this.btnGo.TabIndex = 5;
            this.btnGo.Text = "Compare";
            this.btnGo.UseVisualStyleBackColor = true;
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // openListDialog
            // 
            this.openListDialog.Filter = "Text files|*.txt";
            this.openListDialog.SupportMultiDottedExtensions = true;
            // 
            // lblFirst
            // 
            this.lblFirst.AutoSize = true;
            this.lblFirst.Location = new System.Drawing.Point(12, 219);
            this.lblFirst.Name = "lblFirst";
            this.lblFirst.Size = new System.Drawing.Size(13, 13);
            this.lblFirst.TabIndex = 6;
            this.lblFirst.Text = "0";
            // 
            // lblSecond
            // 
            this.lblSecond.AutoSize = true;
            this.lblSecond.Location = new System.Drawing.Point(169, 219);
            this.lblSecond.Name = "lblSecond";
            this.lblSecond.Size = new System.Drawing.Size(13, 13);
            this.lblSecond.TabIndex = 7;
            this.lblSecond.Text = "0";
            // 
            // lblBoth
            // 
            this.lblBoth.AutoSize = true;
            this.lblBoth.Location = new System.Drawing.Point(402, 219);
            this.lblBoth.Name = "lblBoth";
            this.lblBoth.Size = new System.Drawing.Size(13, 13);
            this.lblBoth.TabIndex = 8;
            this.lblBoth.Text = "0";
            // 
            // saveListDialog
            // 
            this.saveListDialog.Filter = "Text files|*.txt";
            this.saveListDialog.SupportMultiDottedExtensions = true;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(396, 267);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 9;
            this.btnSave.Text = "Save list";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(396, 310);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 10;
            this.btnClear.Text = "Clear all";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "List 1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(172, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "List 2";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(405, 7);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "Duplicates";
            // 
            // btnSave2
            // 
            this.btnSave2.Location = new System.Drawing.Point(169, 267);
            this.btnSave2.Name = "btnSave2";
            this.btnSave2.Size = new System.Drawing.Size(75, 23);
            this.btnSave2.TabIndex = 14;
            this.btnSave2.Text = "Save list";
            this.btnSave2.UseVisualStyleBackColor = true;
            this.btnSave2.Click += new System.EventHandler(this.btnSave2_Click);
            // 
            // btnSave1
            // 
            this.btnSave1.Location = new System.Drawing.Point(9, 267);
            this.btnSave1.Name = "btnSave1";
            this.btnSave1.Size = new System.Drawing.Size(75, 23);
            this.btnSave1.TabIndex = 15;
            this.btnSave1.Text = "Save list";
            this.btnSave1.UseVisualStyleBackColor = true;
            this.btnSave1.Click += new System.EventHandler(this.btnSave1_Click);
            // 
            // lbSecond
            // 
            this.lbSecond.FormattingEnabled = true;
            this.lbSecond.Location = new System.Drawing.Point(169, 25);
            this.lbSecond.Name = "lbSecond";
            this.lbSecond.Size = new System.Drawing.Size(137, 186);
            this.lbSecond.TabIndex = 17;
            // 
            // lbFirst
            // 
            this.lbFirst.FormattingEnabled = true;
            this.lbFirst.Location = new System.Drawing.Point(9, 25);
            this.lbFirst.Name = "lbFirst";
            this.lbFirst.Size = new System.Drawing.Size(137, 186);
            this.lbFirst.TabIndex = 16;
            // 
            // ListComparer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(534, 343);
            this.Controls.Add(this.lbSecond);
            this.Controls.Add(this.lbFirst);
            this.Controls.Add(this.btnSave1);
            this.Controls.Add(this.btnSave2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.lblBoth);
            this.Controls.Add(this.lblSecond);
            this.Controls.Add(this.lblFirst);
            this.Controls.Add(this.btnGo);
            this.Controls.Add(this.btnOpen2);
            this.Controls.Add(this.btnOpen1);
            this.Controls.Add(this.lbBoth);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ListComparer";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "List comparer";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lbBoth;
        private System.Windows.Forms.Button btnOpen1;
        private System.Windows.Forms.Button btnOpen2;
        private System.Windows.Forms.Button btnGo;
        private System.Windows.Forms.OpenFileDialog openListDialog;
        private System.Windows.Forms.Label lblFirst;
        private System.Windows.Forms.Label lblSecond;
        private System.Windows.Forms.Label lblBoth;
        private System.Windows.Forms.SaveFileDialog saveListDialog;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnSave2;
        private System.Windows.Forms.Button btnSave1;
        private ListBox2 lbFirst;
        private ListBox2 lbSecond;
    }
}

