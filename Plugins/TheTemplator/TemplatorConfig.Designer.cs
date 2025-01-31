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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TemplatorConfig));
            this.OK = new System.Windows.Forms.Button();
            this.cancel = new System.Windows.Forms.Button();
            this.templateParameters = new System.Windows.Forms.ListView();
            this.paramName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.paramRegex = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
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
            this.replacementParamName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.replacementExpression = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.removeExcessPipes = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // OK
            // 
            resources.ApplyResources(this.OK, "OK");
            this.OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OK.Name = "OK";
            this.OK.UseVisualStyleBackColor = true;
            this.OK.Click += new System.EventHandler(this.OK_Click);
            // 
            // cancel
            // 
            resources.ApplyResources(this.cancel, "cancel");
            this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancel.Name = "cancel";
            this.cancel.UseVisualStyleBackColor = true;
            // 
            // templateParameters
            // 
            resources.ApplyResources(this.templateParameters, "templateParameters");
            this.templateParameters.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.paramName,
            this.paramRegex});
            this.templateParameters.FullRowSelect = true;
            this.templateParameters.HideSelection = false;
            this.templateParameters.MultiSelect = false;
            this.templateParameters.Name = "templateParameters";
            this.templateParameters.ShowGroups = false;
            this.templateParameters.UseCompatibleStateImageBehavior = false;
            this.templateParameters.View = System.Windows.Forms.View.Details;
            this.templateParameters.ColumnWidthChanging += new System.Windows.Forms.ColumnWidthChangingEventHandler(this.templateParameters_ColumnWidthChanging);
            this.templateParameters.SelectedIndexChanged += new System.EventHandler(this.OnSelectedIndexChangedTemplateParameters);
            this.templateParameters.DoubleClick += new System.EventHandler(this.OnDoubleClickTemplateParameters);
            // 
            // paramName
            // 
            resources.ApplyResources(this.paramName, "paramName");
            // 
            // paramRegex
            // 
            resources.ApplyResources(this.paramRegex, "paramRegex");
            // 
            // newParam
            // 
            resources.ApplyResources(this.newParam, "newParam");
            this.newParam.Name = "newParam";
            this.newParam.UseVisualStyleBackColor = true;
            this.newParam.Click += new System.EventHandler(this.OnNewParam);
            // 
            // editParam
            // 
            resources.ApplyResources(this.editParam, "editParam");
            this.editParam.Name = "editParam";
            this.editParam.UseVisualStyleBackColor = true;
            this.editParam.Click += new System.EventHandler(this.OnEditParam);
            // 
            // delParam
            // 
            resources.ApplyResources(this.delParam, "delParam");
            this.delParam.Name = "delParam";
            this.delParam.UseVisualStyleBackColor = true;
            this.delParam.Click += new System.EventHandler(this.OnDelParam);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // templateName
            // 
            resources.ApplyResources(this.templateName, "templateName");
            this.templateName.Name = "templateName";
            // 
            // regexHelp
            // 
            resources.ApplyResources(this.regexHelp, "regexHelp");
            this.regexHelp.Name = "regexHelp";
            this.regexHelp.TabStop = true;
            this.regexHelp.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.regexHelp_LinkClicked);
            // 
            // skipIfNone
            // 
            resources.ApplyResources(this.skipIfNone, "skipIfNone");
            this.skipIfNone.Name = "skipIfNone";
            this.skipIfNone.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBox2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.groupBox1);
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Controls.Add(this.templateParameters);
            this.groupBox2.Controls.Add(this.delParam);
            this.groupBox2.Controls.Add(this.editParam);
            this.groupBox2.Controls.Add(this.regexHelp);
            this.groupBox2.Controls.Add(this.newParam);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.delReplacement);
            this.groupBox1.Controls.Add(this.editReplacement);
            this.groupBox1.Controls.Add(this.newReplacement);
            this.groupBox1.Controls.Add(this.replacementParameters);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // delReplacement
            // 
            resources.ApplyResources(this.delReplacement, "delReplacement");
            this.delReplacement.Name = "delReplacement";
            this.delReplacement.UseVisualStyleBackColor = true;
            this.delReplacement.Click += new System.EventHandler(this.OnDelReplacementParam);
            // 
            // editReplacement
            // 
            resources.ApplyResources(this.editReplacement, "editReplacement");
            this.editReplacement.Name = "editReplacement";
            this.editReplacement.UseVisualStyleBackColor = true;
            this.editReplacement.Click += new System.EventHandler(this.OnEditReplacementParam);
            // 
            // newReplacement
            // 
            resources.ApplyResources(this.newReplacement, "newReplacement");
            this.newReplacement.Name = "newReplacement";
            this.newReplacement.UseVisualStyleBackColor = true;
            this.newReplacement.Click += new System.EventHandler(this.OnNewReplacementParam);
            // 
            // replacementParameters
            // 
            resources.ApplyResources(this.replacementParameters, "replacementParameters");
            this.replacementParameters.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.replacementParamName,
            this.replacementExpression});
            this.replacementParameters.FullRowSelect = true;
            this.replacementParameters.HideSelection = false;
            this.replacementParameters.MultiSelect = false;
            this.replacementParameters.Name = "replacementParameters";
            this.replacementParameters.ShowGroups = false;
            this.replacementParameters.UseCompatibleStateImageBehavior = false;
            this.replacementParameters.View = System.Windows.Forms.View.Details;
            this.replacementParameters.ColumnWidthChanging += new System.Windows.Forms.ColumnWidthChangingEventHandler(this.replacementParameters_ColumnWidthChanging);
            this.replacementParameters.SelectedIndexChanged += new System.EventHandler(this.OnSelectedIndexChangedTemplateParameters);
            this.replacementParameters.DoubleClick += new System.EventHandler(this.OnDoubleClickReplacementParameters);
            // 
            // replacementParamName
            // 
            resources.ApplyResources(this.replacementParamName, "replacementParamName");
            // 
            // replacementExpression
            // 
            resources.ApplyResources(this.replacementExpression, "replacementExpression");
            // 
            // removeExcessPipes
            // 
            resources.ApplyResources(this.removeExcessPipes, "removeExcessPipes");
            this.removeExcessPipes.Name = "removeExcessPipes";
            this.removeExcessPipes.UseVisualStyleBackColor = true;
            // 
            // TemplatorConfig
            // 
            this.AcceptButton = this.OK;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancel;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.removeExcessPipes);
            this.Controls.Add(this.OK);
            this.Controls.Add(this.skipIfNone);
            this.Controls.Add(this.cancel);
            this.Controls.Add(this.templateName);
            this.Name = "TemplatorConfig";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
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