using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    }
}
