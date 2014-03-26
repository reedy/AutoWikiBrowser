using AutoWikiBrowser.Plugins.Kingbotk;
using AutoWikiBrowser.Plugins.Kingbotk.Components;
using AutoWikiBrowser.Plugins.Kingbotk.ManualAssessments;
using AutoWikiBrowser.Plugins.Kingbotk.Plugins;
using My;
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
namespace AutoWikiBrowser.Plugins.Kingbotk.Components
{
	[Microsoft.VisualBasic.CompilerServices.DesignerGenerated()]
	partial class TimerStats : System.Windows.Forms.UserControl
	{

		//UserControl overrides dispose to clean up the component list.
		[System.Diagnostics.DebuggerNonUserCode()]
		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		//Required by the Windows Form Designer

		private System.ComponentModel.IContainer components;
		//NOTE: The following procedure is required by the Windows Form Designer
		//It can be modified using the Windows Form Designer.  
		//Do not modify it using the code editor.
		[System.Diagnostics.DebuggerStepThrough()]
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.Timer1 = new System.Windows.Forms.Timer(this.components);
			this.TimerLabel = new System.Windows.Forms.Label();
			this.SpeedLabel = new System.Windows.Forms.Label();
			this.EditsLabel = new System.Windows.Forms.Label();
			this.ToolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.SuspendLayout();
			//
			//Timer1
			//
			this.Timer1.Interval = 1000;
			//
			//TimerLabel
			//
			this.TimerLabel.AutoSize = true;
			this.TimerLabel.Location = new System.Drawing.Point(3, 9);
			this.TimerLabel.Name = "TimerLabel";
			this.TimerLabel.Size = new System.Drawing.Size(49, 13);
			this.TimerLabel.TabIndex = 0;
			this.TimerLabel.Text = "00:00:00";
			this.ToolTip1.SetToolTip(this.TimerLabel, "Time running");
			//
			//SpeedLabel
			//
			this.SpeedLabel.AutoSize = true;
			this.SpeedLabel.Location = new System.Drawing.Point(3, 30);
			this.SpeedLabel.Name = "SpeedLabel";
			this.SpeedLabel.Size = new System.Drawing.Size(45, 13);
			this.SpeedLabel.TabIndex = 1;
			this.SpeedLabel.Text = "0 saved";
			this.ToolTip1.SetToolTip(this.SpeedLabel, "Seconds per save");
			//
			//EditsLabel
			//
			this.EditsLabel.AutoSize = true;
			this.EditsLabel.Location = new System.Drawing.Point(3, 51);
			this.EditsLabel.Name = "EditsLabel";
			this.EditsLabel.Size = new System.Drawing.Size(13, 13);
			this.EditsLabel.TabIndex = 2;
			this.EditsLabel.Text = "0";
			this.ToolTip1.SetToolTip(this.EditsLabel, "Number of pages saved");
			//
			//TimerStats
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.EditsLabel);
			this.Controls.Add(this.SpeedLabel);
			this.Controls.Add(this.TimerLabel);
			this.MaximumSize = new System.Drawing.Size(63, 70);
			this.Name = "TimerStats";
			this.Size = new System.Drawing.Size(63, 70);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		private System.Windows.Forms.Timer withEventsField_Timer1;
		private System.Windows.Forms.Timer Timer1 {
			get { return withEventsField_Timer1; }
			set {
				if (withEventsField_Timer1 != null) {
					withEventsField_Timer1.Tick -= Timer1_Tick;
				}
				withEventsField_Timer1 = value;
				if (withEventsField_Timer1 != null) {
					withEventsField_Timer1.Tick += Timer1_Tick;
				}
			}
		}
		private System.Windows.Forms.Label TimerLabel;
		private System.Windows.Forms.Label SpeedLabel;
		private System.Windows.Forms.Label EditsLabel;

		private System.Windows.Forms.ToolTip ToolTip1;
	}
}
