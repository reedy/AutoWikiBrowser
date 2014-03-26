using AutoWikiBrowser.Plugins.Kingbotk;
using AutoWikiBrowser.Plugins.Kingbotk.Components;
using AutoWikiBrowser.Plugins.Kingbotk.ManualAssessments;
using AutoWikiBrowser.Plugins.Kingbotk.Plugins;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using WikiFunctions;
using WikiFunctions.Plugin;
namespace AutoWikiBrowser.Plugins.Kingbotk.Plugins
{
	partial class GenericWithWorkgroups : System.Windows.Forms.UserControl
	{
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
		protected void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.ToolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.AutoStubCheckBox = new System.Windows.Forms.CheckBox();
			this.StubClassCheckBox = new System.Windows.Forms.CheckBox();
			this.LinkLabel1 = new System.Windows.Forms.LinkLabel();
			this.ListView1 = new System.Windows.Forms.ListView();
			this.colWG = (System.Windows.Forms.ColumnHeader)new System.Windows.Forms.ColumnHeader();
			this.ParametersGroup = new System.Windows.Forms.GroupBox();
			this.ParametersGroup.SuspendLayout();
			this.SuspendLayout();
			//
			//AutoStubCheckBox
			//
			this.AutoStubCheckBox.AutoSize = true;
			this.AutoStubCheckBox.Location = new System.Drawing.Point(88, 14);
			this.AutoStubCheckBox.Name = "AutoStubCheckBox";
			this.AutoStubCheckBox.Size = new System.Drawing.Size(73, 17);
			this.AutoStubCheckBox.TabIndex = 4;
			this.AutoStubCheckBox.Text = "Auto-Stub";
			this.ToolTip1.SetToolTip(this.AutoStubCheckBox, "class=Stub|auto=yes");
			this.AutoStubCheckBox.UseVisualStyleBackColor = true;
			//
			//StubClassCheckBox
			//
			this.StubClassCheckBox.AutoSize = true;
			this.StubClassCheckBox.Location = new System.Drawing.Point(6, 14);
			this.StubClassCheckBox.Name = "StubClassCheckBox";
			this.StubClassCheckBox.Size = new System.Drawing.Size(76, 17);
			this.StubClassCheckBox.TabIndex = 3;
			this.StubClassCheckBox.Text = "Stub-Class";
			this.ToolTip1.SetToolTip(this.StubClassCheckBox, "class=Stub (not for use in bot mode; use Auto-Stub)");
			this.StubClassCheckBox.UseVisualStyleBackColor = true;
			//
			//LinkLabel1
			//
			this.LinkLabel1.AutoSize = true;
			this.LinkLabel1.Location = new System.Drawing.Point(182, 307);
			this.LinkLabel1.Name = "LinkLabel1";
			this.LinkLabel1.Size = new System.Drawing.Size(23, 13);
			this.LinkLabel1.TabIndex = 8;
			this.LinkLabel1.TabStop = true;
			this.LinkLabel1.Text = "{{}}";
			//
			//ListView1
			//
			this.ListView1.CheckBoxes = true;
			this.ListView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { this.colWG });
			this.ListView1.Location = new System.Drawing.Point(3, 3);
			this.ListView1.Name = "ListView1";
			this.ListView1.Size = new System.Drawing.Size(270, 284);
			this.ListView1.TabIndex = 10;
			this.ListView1.UseCompatibleStateImageBehavior = false;
			this.ListView1.View = System.Windows.Forms.View.Details;
			//
			//colWG
			//
			this.colWG.Text = "Workgroup";
			this.colWG.Width = 266;
			//
			//ParametersGroup
			//
			this.ParametersGroup.Controls.Add(this.AutoStubCheckBox);
			this.ParametersGroup.Controls.Add(this.StubClassCheckBox);
			this.ParametersGroup.Location = new System.Drawing.Point(3, 293);
			this.ParametersGroup.Name = "ParametersGroup";
			this.ParametersGroup.Size = new System.Drawing.Size(172, 53);
			this.ParametersGroup.TabIndex = 11;
			this.ParametersGroup.TabStop = false;
			this.ParametersGroup.Text = "Template Parameters";
			//
			//GenericWithWorkgroups
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.ButtonFace;
			this.Controls.Add(this.ParametersGroup);
			this.Controls.Add(this.ListView1);
			this.Controls.Add(this.LinkLabel1);
			this.MaximumSize = new System.Drawing.Size(276, 349);
			this.MinimumSize = new System.Drawing.Size(276, 349);
			this.Name = "GenericWithWorkgroups";
			this.Size = new System.Drawing.Size(276, 349);
			this.ParametersGroup.ResumeLayout(false);
			this.ParametersGroup.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
#endregion
		private System.Windows.Forms.ToolTip ToolTip1;
		private System.Windows.Forms.LinkLabel withEventsField_LinkLabel1;
        private System.Windows.Forms.LinkLabel LinkLabel1;
		internal System.Windows.Forms.ListView ListView1;
		internal System.Windows.Forms.ColumnHeader colWG;
		private System.Windows.Forms.GroupBox ParametersGroup;
		private System.Windows.Forms.CheckBox withEventsField_AutoStubCheckBox;
        private System.Windows.Forms.CheckBox AutoStubCheckBox;
        private System.Windows.Forms.CheckBox StubClassCheckBox;
	}
}
