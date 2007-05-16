using System;
using System.Collections.Generic;
using System.Text;

namespace AutoWikiBrowser.Plugins.CFD
{
    internal sealed class AboutBox : WikiFunctions.Controls.AboutBox
    {
        protected override void Initialise()
        {
            lblVersion.Text = "Version " + 
                System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            textBoxDescription.Text = GFDLNotice;
            lnkDownload.Visible = false;
            Text = "CFD Plugin";
        }
    }
}
