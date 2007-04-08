namespace WikiFunctions.Logging.Uploader
{
    using Microsoft.VisualBasic.CompilerServices;
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Runtime.CompilerServices;
    using System.Windows.Forms;

    [DesignerGenerated]
    public class UploadingPleaseWaitForm : Form
    {
        [AccessedThroughProperty("Label1")]
        private Label _Label1;
        [AccessedThroughProperty("PictureBox1")]
        private PictureBox _PictureBox1;
        private IContainer components;
        private Cursor oldCursor;

        [DebuggerNonUserCode]
        public UploadingPleaseWaitForm()
        {
            base.Shown += new EventHandler(this.Form_Shown);
            base.FormClosing += new FormClosingEventHandler(this.Form_Closing);
            this.InitializeComponent();
        }

        [DebuggerNonUserCode]
        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void Form_Closing(object sender, FormClosingEventArgs e)
        {
            this.Cursor = this.oldCursor;
        }

        private void Form_Shown(object sender, EventArgs e)
        {
            this.oldCursor = this.Cursor;
            this.Cursor = Cursors.WaitCursor;
        }

        [DebuggerStepThrough]
        private void InitializeComponent()
        {
            ComponentResourceManager resources = new ComponentResourceManager(typeof(UploadingPleaseWaitForm));
            this.PictureBox1 = new PictureBox();
            this.Label1 = new Label();
            ((ISupportInitialize) this.PictureBox1).BeginInit();
            this.SuspendLayout();
            this.PictureBox1.Image = (Image) resources.GetObject("PictureBox1.Image");
            Point VB$t_struct$S0 = new Point(12, 12);
            this.PictureBox1.Location = VB$t_struct$S0;
            this.PictureBox1.Name = "PictureBox1";
            Size VB$t_struct$S1 = new Size(70, 70);
            this.PictureBox1.Size = VB$t_struct$S1;
            this.PictureBox1.TabIndex = 0;
            this.PictureBox1.TabStop = false;
            this.Label1.AutoSize = true;
            VB$t_struct$S0 = new Point(0x59, 0x17);
            this.Label1.Location = VB$t_struct$S0;
            this.Label1.Name = "Label1";
            VB$t_struct$S1 = new Size(0xbf, 13);
            this.Label1.Size = VB$t_struct$S1;
            this.Label1.TabIndex = 1;
            this.Label1.Text = "Your log is being uploaded, please wait";
            SizeF VB$t_struct$S2 = new SizeF(6f, 13f);
            this.AutoScaleDimensions = VB$t_struct$S2;
            this.AutoScaleMode = AutoScaleMode.Font;
            VB$t_struct$S1 = new Size(0x124, 0x5f);
            this.ClientSize = VB$t_struct$S1;
            this.ControlBox = false;
            this.Controls.Add(this.Label1);
            this.Controls.Add(this.PictureBox1);
            this.Icon = (Icon) resources.GetObject("$this.Icon");
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UploadingPleaseWaitForm";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Uploading...";
            this.TopMost = true;
            ((ISupportInitialize) this.PictureBox1).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        protected internal virtual Label Label1
        {
            [DebuggerNonUserCode]
            get
            {
                return this._Label1;
            }
            [MethodImpl(MethodImplOptions.Synchronized), DebuggerNonUserCode]
            set
            {
                this._Label1 = value;
            }
        }

        protected internal virtual PictureBox PictureBox1
        {
            [DebuggerNonUserCode]
            get
            {
                return this._PictureBox1;
            }
            [MethodImpl(MethodImplOptions.Synchronized), DebuggerNonUserCode]
            set
            {
                this._PictureBox1 = value;
            }
        }
    }
}

