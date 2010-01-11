namespace WikiFunctions.Controls.Lists
{
    partial class ListSplitter
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ListSplitter));
            this.numSplitAmount = new System.Windows.Forms.NumericUpDown();
            this.btnSave = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.btnXMLSave = new System.Windows.Forms.Button();
            this.saveXML = new System.Windows.Forms.SaveFileDialog();
            this.saveTXT = new System.Windows.Forms.SaveFileDialog();
            this.listMaker1 = new WikiFunctions.Controls.Lists.ListMaker();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.numSplitAmount)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // numSplitAmount
            // 
            this.numSplitAmount.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numSplitAmount.Location = new System.Drawing.Point(83, 3);
            this.numSplitAmount.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numSplitAmount.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numSplitAmount.Name = "numSplitAmount";
            this.numSplitAmount.Size = new System.Drawing.Size(55, 20);
            this.numSplitAmount.TabIndex = 2;
            this.numSplitAmount.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(3, 29);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(140, 23);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "Save to text files";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Pages per file:";
            // 
            // btnXMLSave
            // 
            this.btnXMLSave.Location = new System.Drawing.Point(3, 58);
            this.btnXMLSave.Name = "btnXMLSave";
            this.btnXMLSave.Size = new System.Drawing.Size(140, 23);
            this.btnXMLSave.TabIndex = 3;
            this.btnXMLSave.Text = "Save to XML settings files";
            this.btnXMLSave.Click += new System.EventHandler(this.btnXMLSave_Click);
            // 
            // saveXML
            // 
            this.saveXML.FileName = "settings";
            this.saveXML.Filter = "XML files|*.xml";
            this.saveXML.SupportMultiDottedExtensions = true;
            // 
            // saveTXT
            // 
            this.saveTXT.Filter = "Text files|*.txt";
            // 
            // listMaker1
            // 
            this.listMaker1.Location = new System.Drawing.Point(12, 12);
            this.listMaker1.Name = "listMaker1";
            this.listMaker1.SelectedSource = 0;
            this.listMaker1.Size = new System.Drawing.Size(205, 349);
            this.listMaker1.SourceText = "";
            this.listMaker1.TabIndex = 0;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.label2);
            this.flowLayoutPanel1.Controls.Add(this.numSplitAmount);
            this.flowLayoutPanel1.Controls.Add(this.btnSave);
            this.flowLayoutPanel1.Controls.Add(this.btnXMLSave);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(223, 12);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(143, 145);
            this.flowLayoutPanel1.TabIndex = 5;
            // 
            // ListSplitter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(378, 369);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.listMaker1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ListSplitter";
            this.Text = "List Splitter";
            this.Load += new System.EventHandler(this.ListSplitter_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numSplitAmount)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private ListMaker listMaker1;
        private System.Windows.Forms.NumericUpDown numSplitAmount;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnXMLSave;
        private System.Windows.Forms.SaveFileDialog saveXML;
        private System.Windows.Forms.SaveFileDialog saveTXT;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
    }
}
