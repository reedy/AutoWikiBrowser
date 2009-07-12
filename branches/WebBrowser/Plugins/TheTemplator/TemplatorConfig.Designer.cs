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
			this.label1 = new System.Windows.Forms.Label();
			this.templateParameters = new System.Windows.Forms.ListView();
			this.paramName = new System.Windows.Forms.ColumnHeader();
			this.paramRegex = new System.Windows.Forms.ColumnHeader();
			this.label2 = new System.Windows.Forms.Label();
			this.replacementText = new System.Windows.Forms.TextBox();
			this.addParam = new System.Windows.Forms.Button();
			this.editParam = new System.Windows.Forms.Button();
			this.delParam = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.templateName = new System.Windows.Forms.TextBox();
			this.regexHelp = new System.Windows.Forms.LinkLabel();
			this.skipIfNone = new System.Windows.Forms.CheckBox();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.SuspendLayout();
			// 
			// OK
			// 
			this.OK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.OK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.OK.Location = new System.Drawing.Point(396, 112);
			this.OK.Name = "OK";
			this.OK.Size = new System.Drawing.Size(75, 23);
			this.OK.TabIndex = 11;
			this.OK.Text = "OK";
			this.OK.UseVisualStyleBackColor = true;
			this.OK.Click += new System.EventHandler(this.OK_Click);
			// 
			// cancel
			// 
			this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancel.Location = new System.Drawing.Point(477, 112);
			this.cancel.Name = "cancel";
			this.cancel.Size = new System.Drawing.Size(75, 23);
			this.cancel.TabIndex = 12;
			this.cancel.Text = "Cancel";
			this.cancel.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(14, 72);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(106, 13);
			this.label1.TabIndex = 3;
			this.label1.Text = "Template &parameters";
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
			this.templateParameters.Location = new System.Drawing.Point(14, 88);
			this.templateParameters.MultiSelect = false;
			this.templateParameters.Name = "templateParameters";
			this.templateParameters.ShowGroups = false;
			this.templateParameters.Size = new System.Drawing.Size(538, 131);
			this.templateParameters.TabIndex = 4;
			this.templateParameters.UseCompatibleStateImageBehavior = false;
			this.templateParameters.View = System.Windows.Forms.View.Details;
			this.templateParameters.SelectedIndexChanged += new System.EventHandler(this.OnSelectedIndexChangedTemplateParameters);
			this.templateParameters.DoubleClick += new System.EventHandler(this.OnDoubleClickTemplateParameters);
			// 
			// paramName
			// 
			this.paramName.Text = "Parameter name";
			this.paramName.Width = 119;
			// 
			// paramRegex
			// 
			this.paramRegex.Text = "Regular expression";
			this.paramRegex.Width = 395;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(11, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(90, 13);
			this.label2.TabIndex = 9;
			this.label2.Text = "&Replacement text";
			// 
			// replacementText
			// 
			this.replacementText.AcceptsReturn = true;
			this.replacementText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.replacementText.Location = new System.Drawing.Point(14, 16);
			this.replacementText.Multiline = true;
			this.replacementText.Name = "replacementText";
			this.replacementText.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.replacementText.Size = new System.Drawing.Size(538, 90);
			this.replacementText.TabIndex = 10;
			// 
			// addParam
			// 
			this.addParam.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.addParam.Location = new System.Drawing.Point(315, 228);
			this.addParam.Name = "addParam";
			this.addParam.Size = new System.Drawing.Size(75, 23);
			this.addParam.TabIndex = 6;
			this.addParam.Text = "&Add...";
			this.addParam.UseVisualStyleBackColor = true;
			this.addParam.Click += new System.EventHandler(this.OnAddParam);
			// 
			// editParam
			// 
			this.editParam.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.editParam.Location = new System.Drawing.Point(396, 228);
			this.editParam.Name = "editParam";
			this.editParam.Size = new System.Drawing.Size(75, 23);
			this.editParam.TabIndex = 7;
			this.editParam.Text = "&Edit...";
			this.editParam.UseVisualStyleBackColor = true;
			this.editParam.Click += new System.EventHandler(this.OnEditParam);
			// 
			// delParam
			// 
			this.delParam.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.delParam.Location = new System.Drawing.Point(477, 228);
			this.delParam.Name = "delParam";
			this.delParam.Size = new System.Drawing.Size(75, 23);
			this.delParam.TabIndex = 8;
			this.delParam.Text = "&Delete";
			this.delParam.UseVisualStyleBackColor = true;
			this.delParam.Click += new System.EventHandler(this.OnDelParam);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(14, 17);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(54, 13);
			this.label3.TabIndex = 0;
			this.label3.Text = "&Template:";
			// 
			// templateName
			// 
			this.templateName.Location = new System.Drawing.Point(74, 14);
			this.templateName.Name = "templateName";
			this.templateName.Size = new System.Drawing.Size(163, 20);
			this.templateName.TabIndex = 1;
			// 
			// regexHelp
			// 
			this.regexHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.regexHelp.AutoSize = true;
			this.regexHelp.Location = new System.Drawing.Point(14, 228);
			this.regexHelp.Name = "regexHelp";
			this.regexHelp.Size = new System.Drawing.Size(120, 13);
			this.regexHelp.TabIndex = 5;
			this.regexHelp.TabStop = true;
			this.regexHelp.Text = "Regular expression help";
			this.regexHelp.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.regexHelp_LinkClicked);
			// 
			// skipIfNone
			// 
			this.skipIfNone.AutoSize = true;
			this.skipIfNone.Location = new System.Drawing.Point(14, 40);
			this.skipIfNone.Name = "skipIfNone";
			this.skipIfNone.Size = new System.Drawing.Size(201, 17);
			this.skipIfNone.TabIndex = 2;
			this.skipIfNone.Text = "&Skip article if no templates to process";
			this.skipIfNone.UseVisualStyleBackColor = true;
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.label3);
			this.splitContainer1.Panel1.Controls.Add(this.label1);
			this.splitContainer1.Panel1.Controls.Add(this.skipIfNone);
			this.splitContainer1.Panel1.Controls.Add(this.templateParameters);
			this.splitContainer1.Panel1.Controls.Add(this.regexHelp);
			this.splitContainer1.Panel1.Controls.Add(this.addParam);
			this.splitContainer1.Panel1.Controls.Add(this.templateName);
			this.splitContainer1.Panel1.Controls.Add(this.editParam);
			this.splitContainer1.Panel1.Controls.Add(this.delParam);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.replacementText);
			this.splitContainer1.Panel2.Controls.Add(this.OK);
			this.splitContainer1.Panel2.Controls.Add(this.cancel);
			this.splitContainer1.Panel2.Controls.Add(this.label2);
			this.splitContainer1.Size = new System.Drawing.Size(564, 416);
			this.splitContainer1.SplitterDistance = 265;
			this.splitContainer1.TabIndex = 14;
			// 
			// TemplatorConfig
			// 
			this.AcceptButton = this.OK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancel;
			this.ClientSize = new System.Drawing.Size(564, 416);
			this.Controls.Add(this.splitContainer1);
			this.MinimumSize = new System.Drawing.Size(405, 343);
			this.Name = "TemplatorConfig";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "TheTemplator Configuration";
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel1.PerformLayout();
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.Panel2.PerformLayout();
			this.splitContainer1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button OK;
		private System.Windows.Forms.Button cancel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ListView templateParameters;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox replacementText;
		private System.Windows.Forms.Button addParam;
		private System.Windows.Forms.Button editParam;
		private System.Windows.Forms.Button delParam;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox templateName;
		private System.Windows.Forms.LinkLabel regexHelp;
		private System.Windows.Forms.CheckBox skipIfNone;
		private System.Windows.Forms.SplitContainer splitContainer1;
		internal System.Windows.Forms.ColumnHeader paramName;
		internal System.Windows.Forms.ColumnHeader paramRegex;
	}
}