namespace WikiFunctions.AutoWikiBrowser.Plugins.SDKSoftware.Kingbotk.Components
{
    using Microsoft.VisualBasic.CompilerServices;
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Runtime.CompilerServices;
    using System.Windows.Forms;

    [DesignerGenerated]
    public class ErrorForm : Form
    {
        [AccessedThroughProperty("lblError")]
        private Label _lblError;
        private IContainer components;

        public ErrorForm(string errorMessage)
        {
            this.InitializeComponent();
            this.lblError.Text = errorMessage;
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

        [DebuggerStepThrough]
        private void InitializeComponent()
        {
            ComponentResourceManager resources = new ComponentResourceManager(typeof(ErrorForm));
            this.lblError = new Label();
            this.SuspendLayout();
            this.lblError.AutoSize = true;
            Point VB$t_struct$S0 = new Point(12, 9);
            this.lblError.Location = VB$t_struct$S0;
            this.lblError.Name = "lblError";
            Size VB$t_struct$S1 = new Size(0, 13);
            this.lblError.Size = VB$t_struct$S1;
            this.lblError.TabIndex = 0;
            SizeF VB$t_struct$S2 = new SizeF(6f, 13f);
            this.AutoScaleDimensions = VB$t_struct$S2;
            this.AutoScaleMode = AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            VB$t_struct$S1 = new Size(0x153, 0x3a);
            this.ClientSize = VB$t_struct$S1;
            this.Controls.Add(this.lblError);
            this.Icon = (Icon) resources.GetObject("$this.Icon");
            this.Name = "ErrorForm";
            this.Text = "Error";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        internal virtual Label lblError
        {
            [DebuggerNonUserCode]
            get
            {
                return this._lblError;
            }
            [MethodImpl(MethodImplOptions.Synchronized), DebuggerNonUserCode]
            set
            {
                this._lblError = value;
            }
        }
    }
}

