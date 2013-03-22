namespace AutoWikiBrowser.Plugins.TheTemplator
{
    partial class TemplatorConfig
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
            this.OK = new System.Windows.Forms.Button();
            this.cancel = new System.Windows.Forms.Button();
            this.templateParameters = new System.Windows.Forms.ListView();
            this.paramName = new System.Windows.Forms.ColumnHeader();
            this.paramRegex = new System.Windows.Forms.ColumnHeader();
            this.newParam = new System.Windows.Forms.Button();
            this.editParam = new System.Windows.Forms.Button();
            this.delParam = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.templateName = new System.Windows.Forms.TextBox();
            this.regexHelp = new System.Windows.Forms.LinkLabel();
            this.skipIfNone = new System.Windows.Forms.CheckBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.delReplacement = new System.Windows.Forms.Button();
            this.editReplacement = new System.Windows.Forms.Button();
            this.newReplacement = new System.Windows.Forms.Button();
            this.replacementParameters = new System.Windows.Forms.ListView();
            this.replacementParamName = new System.Windows.Forms.ColumnHeader();
            this.replacementExpression = new System.Windows.Forms.ColumnHeader();
            this.removeExcessPipes = new System.Windows.Forms.CheckBox();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // OK
            // 
            this.OK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OK.Location = new System.Drawing.Point(249, 360);
            this.OK.Name = "OK";
            this.OK.Size = new System.Drawing.Size(75, 23);
            this.OK.TabIndex = 1;
            this.OK.Text = "OK";
            this.OK.UseVisualStyleBackColor = true;
            this.OK.Click += new System.EventHandler(this.OK_Click);
            // 
            // cancel
            // 
            this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancel.Location = new System.Drawing.Point(330, 360);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(75, 23);
            this.cancel.TabIndex = 2;
            this.cancel.Text = "Cancel";
            this.cancel.UseVisualStyleBackColor = true;
            // 
            // templateParameters
            // 
            this.templateParameters.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.templateParameters.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.paramName,
            this.paramRegex});
            this.templateParameters.FullRowSelect = true;
            this.templateParameters.HideSelection = false;
            this.templateParameters.LabelWrap = false;
            this.templateParameters.Location = new System.Drawing.Point(6, 19);
            this.templateParameters.MultiSelect = false;
            this.templateParameters.Name = "templateParameters";
            this.templateParameters.ShowGroups = false;
            this.templateParameters.Size = new System.Drawing.Size(405, 70);
            this.templateParameters.TabIndex = 0;
            this.templateParameters.UseCompatibleStateImageBehavior = false;
            this.templateParameters.View = System.Windows.Forms.View.Details;
            this.templateParameters.SelectedIndexChanged += new System.EventHandler(this.OnSelectedIndexChangedTemplateParameters);
            this.templateParameters.DoubleClick += new System.EventHandler(this.OnDoubleClickTemplateParameters);
            this.templateParameters.ColumnWidthChanging += new System.Windows.Forms.ColumnWidthChangingEventHandler(this.templateParameters_ColumnWidthChanging);
            // 
            // paramName
            // 
            this.paramName.Text = "Parameter name";
            this.paramName.Width = 119;
            // 
            // paramRegex
            // 
            this.paramRegex.Text = "Regular expression";
            this.paramRegex.Width = 262;
            // 
            // newParam
            // 
            this.newParam.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.newParam.Location = new System.Drawing.Point(174, 95);
            this.newParam.Name = "newParam";
            this.newParam.Size = new System.Drawing.Size(75, 23);
            this.newParam.TabIndex = 2;
            this.newParam.Text = "&New...";
            this.newParam.UseVisualStyleBackColor = true;
            this.newParam.Click += new System.EventHandler(this.OnNewParam);
            // 
            // editParam
            // 
            this.editParam.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.editParam.Location = new System.Drawing.Point(255, 95);
            this.editParam.Name = "editParam";
            this.editParam.Size = new System.Drawing.Size(75, 23);
            this.editParam.TabIndex = 3;
            this.editParam.Text = "&Edit...";
            this.editParam.UseVisualStyleBackColor = true;
            this.editParam.Click += new System.EventHandler(this.OnEditParam);
            // 
            // delParam
            // 
            this.delParam.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.delParam.Location = new System.Drawing.Point(336, 95);
            this.delParam.Name = "delParam";
            this.delParam.Size = new System.Drawing.Size(75, 23);
            this.delParam.TabIndex = 4;
            this.delParam.Text = "&Delete";
            this.delParam.UseVisualStyleBackColor = true;
            this.delParam.Click += new System.EventHandler(this.OnDelParam);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "&Template:";
            // 
            // templateName
            // 
            this.templateName.Location = new System.Drawing.Point(69, 6);
            this.templateName.Name = "templateName";
            this.templateName.Size = new System.Drawing.Size(163, 20);
            this.templateName.TabIndex = 1;
            // 
            // regexHelp
            // 
            this.regexHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.regexHelp.AutoSize = true;
            this.regexHelp.Location = new System.Drawing.Point(6, 105);
            this.regexHelp.Name = "regexHelp";
            this.regexHelp.Size = new System.Drawing.Size(120, 13);
            this.regexHelp.TabIndex = 1;
            this.regexHelp.TabStop = true;
            this.regexHelp.Text = "Regular expression help";
            this.regexHelp.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.regexHelp_LinkClicked);
            // 
            // skipIfNone
            // 
            this.skipIfNone.AutoSize = true;
            this.skipIfNone.Location = new System.Drawing.Point(9, 32);
            this.skipIfNone.Name = "skipIfNone";
            this.skipIfNone.Size = new System.Drawing.Size(201, 17);
            this.skipIfNone.TabIndex = 2;
            this.skipIfNone.Text = "&Skip article if no templates to process";
            this.skipIfNone.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(0, 78);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBox2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.groupBox1);
            this.splitContainer1.Size = new System.Drawing.Size(423, 276);
            this.splitContainer1.SplitterDistance = 137;
            this.splitContainer1.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.templateParameters);
            this.groupBox2.Controls.Add(this.delParam);
            this.groupBox2.Controls.Add(this.editParam);
            this.groupBox2.Controls.Add(this.regexHelp);
            this.groupBox2.Controls.Add(this.newParam);
            this.groupBox2.Location = new System.Drawing.Point(3, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(417, 131);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Template &parameters";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.delReplacement);
            this.groupBox1.Controls.Add(this.editReplacement);
            this.groupBox1.Controls.Add(this.newReplacement);
            this.groupBox1.Controls.Add(this.replacementParameters);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(417, 129);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "&Replacement parameters";
            // 
            // delReplacement
            // 
            this.delReplacement.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.delReplacement.Location = new System.Drawing.Point(336, 100);
            this.delReplacement.Name = "delReplacement";
            this.delReplacement.Size = new System.Drawing.Size(75, 23);
            this.delReplacement.TabIndex = 3;
            this.delReplacement.Text = "De&lete";
            this.delReplacement.UseVisualStyleBackColor = true;
            this.delReplacement.Click += new System.EventHandler(this.OnDelReplacementParam);
            // 
            // editReplacement
            // 
            this.editReplacement.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.editReplacement.Location = new System.Drawing.Point(255, 100);
            this.editReplacement.Name = "editReplacement";
            this.editReplacement.Size = new System.Drawing.Size(75, 23);
            this.editReplacement.TabIndex = 2;
            this.editReplacement.Text = "Ed&it...";
            this.editReplacement.UseVisualStyleBackColor = true;
            this.editReplacement.Click += new System.EventHandler(this.OnEditReplacementParam);
            // 
            // newReplacement
            // 
            this.newReplacement.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.newReplacement.Location = new System.Drawing.Point(174, 100);
            this.newReplacement.Name = "newReplacement";
            this.newReplacement.Size = new System.Drawing.Size(75, 23);
            this.newReplacement.TabIndex = 1;
            this.newReplacement.Text = "Ne&w...";
            this.newReplacement.UseVisualStyleBackColor = true;
            this.newReplacement.Click += new System.EventHandler(this.OnNewReplacementParam);
            // 
            // replacementParameters
            // 
            this.replacementParameters.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.replacementParameters.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.replacementParamName,
            this.replacementExpression});
            this.replacementParameters.FullRowSelect = true;
            this.replacementParameters.HideSelection = false;
            this.replacementParameters.LabelWrap = false;
            this.replacementParameters.Location = new System.Drawing.Point(5, 19);
            this.replacementParameters.MultiSelect = false;
            this.replacementParameters.Name = "replacementParameters";
            this.replacementParameters.ShowGroups = false;
            this.replacementParameters.Size = new System.Drawing.Size(406, 75);
            this.replacementParameters.TabIndex = 0;
            this.replacementParameters.UseCompatibleStateImageBehavior = false;
            this.replacementParameters.View = System.Windows.Forms.View.Details;
            this.replacementParameters.SelectedIndexChanged += new System.EventHandler(this.OnSelectedIndexChangedTemplateParameters);
            this.replacementParameters.DoubleClick += new System.EventHandler(this.OnDoubleClickReplacementParameters);
            this.replacementParameters.ColumnWidthChanging += new System.Windows.Forms.ColumnWidthChangingEventHandler(this.replacementParameters_ColumnWidthChanging);
            // 
            // replacementParamName
            // 
            this.replacementParamName.Text = "Parameter name";
            this.replacementParamName.Width = 119;
            // 
            // replacementExpression
            // 
            this.replacementExpression.Text = "Replacement expression";
            this.replacementExpression.Width = 264;
            // 
            // removeExcessPipes
            // 
            this.removeExcessPipes.AutoSize = true;
            this.removeExcessPipes.Location = new System.Drawing.Point(9, 55);
            this.removeExcessPipes.Name = "removeExcessPipes";
            this.removeExcessPipes.Size = new System.Drawing.Size(130, 17);
            this.removeExcessPipes.TabIndex = 2;
            this.removeExcessPipes.Text = "Remove e&xcess pipes";
            this.removeExcessPipes.UseVisualStyleBackColor = true;
            // 
            // TemplatorConfig
            // 
            this.AcceptButton = this.OK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancel;
            this.ClientSize = new System.Drawing.Size(423, 388);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.removeExcessPipes);
            this.Controls.Add(this.OK);
            this.Controls.Add(this.skipIfNone);
            this.Controls.Add(this.cancel);
            this.Controls.Add(this.templateName);
            this.MinimumSize = new System.Drawing.Size(405, 343);
            this.Name = "TemplatorConfig";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "TheTemplator Configuration";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button OK;
        private System.Windows.Forms.Button cancel;
        private System.Windows.Forms.ListView templateParameters;
        private System.Windows.Forms.Button newParam;
        private System.Windows.Forms.Button editParam;
        private System.Windows.Forms.Button delParam;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox templateName;
        private System.Windows.Forms.LinkLabel regexHelp;
        private System.Windows.Forms.CheckBox skipIfNone;
        private System.Windows.Forms.SplitContainer splitContainer1;
        internal System.Windows.Forms.ColumnHeader paramName;
        internal System.Windows.Forms.ColumnHeader paramRegex;
        private System.Windows.Forms.ListView replacementParameters;
        internal System.Windows.Forms.ColumnHeader replacementParamName;
        internal System.Windows.Forms.ColumnHeader replacementExpression;
        private System.Windows.Forms.Button newReplacement;
        private System.Windows.Forms.Button delReplacement;
        private System.Windows.Forms.Button editReplacement;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox removeExcessPipes;
    }
}