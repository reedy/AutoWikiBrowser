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
namespace AutoWikiBrowser.Plugins.Kingbotk.ManualAssessments
{
	[Microsoft.VisualBasic.CompilerServices.DesignerGenerated()]
	partial class AssessmentForm : System.Windows.Forms.Form
	{

		//Form overrides dispose to clean up the component list.
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
			this.TableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.OK_Button = new System.Windows.Forms.Button();
			this.Cancel_Button = new System.Windows.Forms.Button();
			this.PictureBox1 = new System.Windows.Forms.PictureBox();
			this.ClassCheckedListBox = new System.Windows.Forms.CheckedListBox();
			this.ImportanceCheckedListBox = new System.Windows.Forms.CheckedListBox();
			this.Label1 = new System.Windows.Forms.Label();
			this.Label2 = new System.Windows.Forms.Label();
			this.ToolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.SettingsCheckedListBox = new System.Windows.Forms.CheckedListBox();
			this.Label3 = new System.Windows.Forms.Label();
			this.TableLayoutPanel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.PictureBox1).BeginInit();
			this.SuspendLayout();
			//
			//TableLayoutPanel1
			//
			this.TableLayoutPanel1.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.TableLayoutPanel1.ColumnCount = 2;
			this.TableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50f));
			this.TableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50f));
			this.TableLayoutPanel1.Controls.Add(this.OK_Button, 0, 0);
			this.TableLayoutPanel1.Controls.Add(this.Cancel_Button, 1, 0);
			this.TableLayoutPanel1.Location = new System.Drawing.Point(148, 251);
			this.TableLayoutPanel1.Name = "TableLayoutPanel1";
			this.TableLayoutPanel1.RowCount = 1;
			this.TableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50f));
			this.TableLayoutPanel1.Size = new System.Drawing.Size(146, 29);
			this.TableLayoutPanel1.TabIndex = 0;
			//
			//OK_Button
			//
			this.OK_Button.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.OK_Button.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.OK_Button.Location = new System.Drawing.Point(3, 3);
			this.OK_Button.Name = "OK_Button";
			this.OK_Button.Size = new System.Drawing.Size(67, 23);
			this.OK_Button.TabIndex = 0;
			this.OK_Button.Text = "OK";
			//
			//Cancel_Button
			//
			this.Cancel_Button.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.Cancel_Button.Location = new System.Drawing.Point(76, 3);
			this.Cancel_Button.Name = "Cancel_Button";
			this.Cancel_Button.Size = new System.Drawing.Size(67, 23);
			this.Cancel_Button.TabIndex = 1;
			this.Cancel_Button.Text = "Skip";
			//
			//PictureBox1
			//
			this.PictureBox1.Image = global::My.Resources.Resources.WP1;
			this.PictureBox1.Location = new System.Drawing.Point(12, 216);
			this.PictureBox1.Name = "PictureBox1";
			this.PictureBox1.Size = new System.Drawing.Size(64, 61);
			this.PictureBox1.TabIndex = 2;
			this.PictureBox1.TabStop = false;
			//
			//ClassCheckedListBox
			//
			this.ClassCheckedListBox.CheckOnClick = true;
			this.ClassCheckedListBox.FormattingEnabled = true;
			this.ClassCheckedListBox.Items.AddRange(new object[] {
				"Unassessed",
				"Stub",
				"Start",
				"C",
				"B",
				"GA",
				"A",
				"FA",
				"Not Applicable",
				"Dab",
				"List",
				"FL"
			});
			this.ClassCheckedListBox.Location = new System.Drawing.Point(12, 26);
			this.ClassCheckedListBox.Name = "ClassCheckedListBox";
			this.ClassCheckedListBox.Size = new System.Drawing.Size(120, 184);
			this.ClassCheckedListBox.TabIndex = 3;
			this.ToolTip1.SetToolTip(this.ClassCheckedListBox, "Article classification");
			//
			//ImportanceCheckedListBox
			//
			this.ImportanceCheckedListBox.CheckOnClick = true;
			this.ImportanceCheckedListBox.FormattingEnabled = true;
			this.ImportanceCheckedListBox.Items.AddRange(new object[] {
				"Unassessed",
				"Low",
				"Mid",
				"High",
				"Top",
				"Not Applicable"
			});
			this.ImportanceCheckedListBox.Location = new System.Drawing.Point(149, 26);
			this.ImportanceCheckedListBox.Name = "ImportanceCheckedListBox";
			this.ImportanceCheckedListBox.Size = new System.Drawing.Size(120, 94);
			this.ImportanceCheckedListBox.TabIndex = 4;
			this.ToolTip1.SetToolTip(this.ImportanceCheckedListBox, "Article importance/priority. Note: If you're tagging the talk page for more than " + "one WikiProject and the importance levels differ you'll have to tweak it manuall" + "y.");
			//
			//Label1
			//
			this.Label1.AutoSize = true;
			this.Label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, Convert.ToByte(0));
			this.Label1.Location = new System.Drawing.Point(8, 4);
			this.Label1.Name = "Label1";
			this.Label1.Size = new System.Drawing.Size(48, 20);
			this.Label1.TabIndex = 5;
			this.Label1.Text = "Class";
			//
			//Label2
			//
			this.Label2.AutoSize = true;
			this.Label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, Convert.ToByte(0));
			this.Label2.Location = new System.Drawing.Point(145, 4);
			this.Label2.Name = "Label2";
			this.Label2.Size = new System.Drawing.Size(141, 20);
			this.Label2.TabIndex = 6;
			this.Label2.Text = "Importance/Priority";
			//
			//SettingsCheckedListBox
			//
			this.SettingsCheckedListBox.CheckOnClick = true;
			this.SettingsCheckedListBox.FormattingEnabled = true;
			this.SettingsCheckedListBox.Items.AddRange(new object[] {
				"Needs infobox",
				"Needs attention",
				"Photo requested"
			});
			this.SettingsCheckedListBox.Location = new System.Drawing.Point(149, 153);
			this.SettingsCheckedListBox.Name = "SettingsCheckedListBox";
			this.SettingsCheckedListBox.Size = new System.Drawing.Size(137, 79);
			this.SettingsCheckedListBox.TabIndex = 7;
			this.ToolTip1.SetToolTip(this.SettingsCheckedListBox, "Other settings");
			//
			//Label3
			//
			this.Label3.AutoSize = true;
			this.Label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, Convert.ToByte(0));
			this.Label3.Location = new System.Drawing.Point(145, 130);
			this.Label3.Name = "Label3";
			this.Label3.Size = new System.Drawing.Size(68, 20);
			this.Label3.TabIndex = 8;
			this.Label3.Text = "Settings";
			//
			//AssessmentForm
			//
			this.AcceptButton = this.OK_Button;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.Cancel_Button;
			this.ClientSize = new System.Drawing.Size(306, 292);
			this.Controls.Add(this.Label3);
			this.Controls.Add(this.SettingsCheckedListBox);
			this.Controls.Add(this.Label2);
			this.Controls.Add(this.Label1);
			this.Controls.Add(this.ImportanceCheckedListBox);
			this.Controls.Add(this.ClassCheckedListBox);
			this.Controls.Add(this.PictureBox1);
			this.Controls.Add(this.TableLayoutPanel1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AssessmentForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "AssessmentForm";
			this.TableLayoutPanel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)this.PictureBox1).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.TableLayoutPanel TableLayoutPanel1;
		private System.Windows.Forms.Button withEventsField_OK_Button;
		private System.Windows.Forms.Button OK_Button {
			get { return withEventsField_OK_Button; }
			set {
				if (withEventsField_OK_Button != null) {
					withEventsField_OK_Button.Click -= OK_Button_Click;
				}
				withEventsField_OK_Button = value;
				if (withEventsField_OK_Button != null) {
					withEventsField_OK_Button.Click += OK_Button_Click;
				}
			}
		}
		private System.Windows.Forms.Button withEventsField_Cancel_Button;
		private System.Windows.Forms.Button Cancel_Button {
			get { return withEventsField_Cancel_Button; }
			set {
				if (withEventsField_Cancel_Button != null) {
					withEventsField_Cancel_Button.Click -= Cancel_Button_Click;
				}
				withEventsField_Cancel_Button = value;
				if (withEventsField_Cancel_Button != null) {
					withEventsField_Cancel_Button.Click += Cancel_Button_Click;
				}
			}
		}
		private System.Windows.Forms.PictureBox PictureBox1;
		private System.Windows.Forms.CheckedListBox withEventsField_ClassCheckedListBox;
		private System.Windows.Forms.CheckedListBox ClassCheckedListBox {
			get { return withEventsField_ClassCheckedListBox; }
			set {
				if (withEventsField_ClassCheckedListBox != null) {
					withEventsField_ClassCheckedListBox.ItemCheck -= AllowOnlyOneCheckedItem;
				}
				withEventsField_ClassCheckedListBox = value;
				if (withEventsField_ClassCheckedListBox != null) {
					withEventsField_ClassCheckedListBox.ItemCheck += AllowOnlyOneCheckedItem;
				}
			}
		}
		private System.Windows.Forms.CheckedListBox withEventsField_ImportanceCheckedListBox;
		private System.Windows.Forms.CheckedListBox ImportanceCheckedListBox {
			get { return withEventsField_ImportanceCheckedListBox; }
			set {
				if (withEventsField_ImportanceCheckedListBox != null) {
					withEventsField_ImportanceCheckedListBox.ItemCheck -= AllowOnlyOneCheckedItem;
				}
				withEventsField_ImportanceCheckedListBox = value;
				if (withEventsField_ImportanceCheckedListBox != null) {
					withEventsField_ImportanceCheckedListBox.ItemCheck += AllowOnlyOneCheckedItem;
				}
			}
		}
		private System.Windows.Forms.Label Label1;
		private System.Windows.Forms.Label Label2;
		private System.Windows.Forms.ToolTip ToolTip1;
		private System.Windows.Forms.CheckedListBox SettingsCheckedListBox;

		private System.Windows.Forms.Label Label3;
	}
}
