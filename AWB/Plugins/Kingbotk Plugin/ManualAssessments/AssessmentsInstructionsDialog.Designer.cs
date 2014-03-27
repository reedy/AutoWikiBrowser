using System;
using AutoWikiBrowser.Plugins.Kingbotk.Properties;

namespace AutoWikiBrowser.Plugins.Kingbotk.ManualAssessments
{
	partial class AssessmentsInstructionsDialog : System.Windows.Forms.Form
	{
        private System.ComponentModel.IContainer components;

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
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AssessmentsInstructionsDialog));
			this.OK_Button = new System.Windows.Forms.Button();
			this.PictureBox1 = new System.Windows.Forms.PictureBox();
			this.Label1 = new System.Windows.Forms.Label();
			this.Label2 = new System.Windows.Forms.Label();
			this.CheckBox1 = new System.Windows.Forms.CheckBox();
			((System.ComponentModel.ISupportInitialize)this.PictureBox1).BeginInit();
			this.SuspendLayout();
			//
			//OK_Button
			//
			this.OK_Button.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.OK_Button.Location = new System.Drawing.Point(51, 259);
			this.OK_Button.Name = "OK_Button";
			this.OK_Button.Size = new System.Drawing.Size(67, 23);
			this.OK_Button.TabIndex = 0;
			this.OK_Button.Text = "OK";
            this.OK_Button.Click += OK_Button_Click;
			//
			//PictureBox1
			//
			this.PictureBox1.Image = Resources.WP1;
			this.PictureBox1.Location = new System.Drawing.Point(12, 12);
			this.PictureBox1.Name = "PictureBox1";
			this.PictureBox1.Size = new System.Drawing.Size(64, 61);
			this.PictureBox1.TabIndex = 1;
			this.PictureBox1.TabStop = false;
			//
			//Label1
			//
			this.Label1.AutoSize = true;
			this.Label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Label1.Location = new System.Drawing.Point(103, 24);
			this.Label1.Name = "Label1";
			this.Label1.Size = new System.Drawing.Size(177, 31);
			this.Label1.TabIndex = 2;
			this.Label1.Text = "Assessments";
			//
			//Label2
			//
			this.Label2.AutoSize = true;
			this.Label2.Location = new System.Drawing.Point(58, 85);
			this.Label2.Name = "Label2";
			this.Label2.Size = new System.Drawing.Size(287, 156);
			this.Label2.TabIndex = 3;
			this.Label2.Text = resources.GetString("Label2.Text");
			//
			//CheckBox1
			//
			this.CheckBox1.AutoSize = true;
			this.CheckBox1.Location = new System.Drawing.Point(124, 265);
			this.CheckBox1.Name = "CheckBox1";
			this.CheckBox1.Size = new System.Drawing.Size(221, 17);
			this.CheckBox1.TabIndex = 4;
			this.CheckBox1.Text = "Don't show again (saved in XML settings)";
			this.CheckBox1.UseVisualStyleBackColor = true;
			//
			//AssessmentsInstructionsDialog
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(365, 296);
			this.Controls.Add(this.CheckBox1);
			this.Controls.Add(this.Label2);
			this.Controls.Add(this.Label1);
			this.Controls.Add(this.PictureBox1);
			this.Controls.Add(this.OK_Button);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AssessmentsInstructionsDialog";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Instructions";
			((System.ComponentModel.ISupportInitialize)this.PictureBox1).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
        #endregion

        private System.Windows.Forms.Button OK_Button;
		private System.Windows.Forms.PictureBox PictureBox1;
		private System.Windows.Forms.Label Label1;
		private System.Windows.Forms.Label Label2;
		private System.Windows.Forms.CheckBox CheckBox1;
	}
}
