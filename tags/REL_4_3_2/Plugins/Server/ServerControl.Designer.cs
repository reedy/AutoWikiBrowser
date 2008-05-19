namespace AutoWikiBrowser.Plugins.Server
{
    partial class ServerControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.SettingsButton = new System.Windows.Forms.Button();
            this.HideButton = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // SettingsButton
            // 
            this.SettingsButton.Location = new System.Drawing.Point(41, 312);
            this.SettingsButton.Name = "SettingsButton";
            this.SettingsButton.Size = new System.Drawing.Size(75, 23);
            this.SettingsButton.TabIndex = 0;
            this.SettingsButton.Text = "Settings";
            this.SettingsButton.UseVisualStyleBackColor = true;
            // 
            // HideButton
            // 
            this.HideButton.Location = new System.Drawing.Point(138, 312);
            this.HideButton.Name = "HideButton";
            this.HideButton.Size = new System.Drawing.Size(75, 23);
            this.HideButton.TabIndex = 1;
            this.HideButton.Text = "Hide";
            this.HideButton.UseVisualStyleBackColor = true;
            // 
            // ServerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.HideButton);
            this.Controls.Add(this.SettingsButton);
            this.MaximumSize = new System.Drawing.Size(276, 349);
            this.MinimumSize = new System.Drawing.Size(276, 349);
            this.Name = "ServerControl";
            this.Size = new System.Drawing.Size(276, 349);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolTip toolTip1;
        internal System.Windows.Forms.Button SettingsButton;
        internal System.Windows.Forms.Button HideButton;
    }
}
