namespace AutoWikiBrowser.Plugins.Delinker
{
    internal sealed class AboutBox : WikiFunctions.Controls.AboutBox
    {
        protected override void Initialise()
        {
            lblVersion.Text = "Version " +
                System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            textBoxDescription.Text = GPLNotice;
            Text = "Delinker";
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutBox));
            this.SuspendLayout();
            // 
            // linkLabel1
            // 
            this.toolTip1.SetToolTip(this.linkLabel1, resources.GetString("linkLabel1.ToolTip"));
            // 
            // textBoxDescription
            // 
            resources.ApplyResources(this.textBoxDescription, "textBoxDescription");
            // 
            // lblVersion
            // 
            resources.ApplyResources(this.lblVersion, "lblVersion");
            // 
            // AboutBox
            // 
            resources.ApplyResources(this, "$this");
            this.Name = "AboutBox";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
