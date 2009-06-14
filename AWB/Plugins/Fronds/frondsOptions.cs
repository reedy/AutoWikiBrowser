using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using WikiFunctions;

namespace Fronds
{
    public partial class FrondsOptions : Form
    {
        public FrondsOptions()
        {
            InitializeComponent();
        }

        public void AddOption(string text)
        {
            //Also very inefficient, but does work.
            bool checkme = false;
            foreach (string filename in Fronds.Settings.EnabledFilenames)
            {
                if (text.Contains(filename))
                {
                    checkme = true;
                    break;
                }
            }
            listOptionsFronds.Items.Add(text, checkme);
        }

        private void btnOptionsCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnOptionsOK_Click(object sender, EventArgs e)
        {
            // Preserve enabled filenames
            List<string> filenames = new List<string>();
            foreach (string item in listOptionsFronds.CheckedItems)
            {
                string filename = item.Substring((item.LastIndexOf("(") + 1), (item.Length - item.LastIndexOf("(") - 2));
                filenames.Add(filename);
            }
            Fronds.Settings.EnabledFilenames = filenames;

            //Loaded selected fronds
            Fronds.loadedFinds = new List<String>();
            Fronds.loadedReplaces = new List<String>();
            Fronds.loadedCases = new List<Boolean>();

            foreach (int index in listOptionsFronds.CheckedIndices)
            {
                string html = Tools.GetHTML(("http://toolserver.org/~jarry/fronds/" + Fronds.possibleFilenames[index]));
                string[] parts = Regex.Split(html, "@#@");
                foreach (string chunk in parts)
                {
                    if (chunk.Contains("Find:"))
                    {
                        Fronds.loadedFinds.Add(chunk.Substring(5));
                    }
                    else if (chunk.Contains("Replace:"))
                    {
                        Fronds.loadedReplaces.Add(chunk.Substring(8));
                    }
                    else if (chunk.Contains("CaseSensitive:"))
                    {
                        Fronds.loadedCases.Add(chunk.Substring(14).Trim() == "yes");
                    }
                }
            }
            Close();
        }

        private void linkWikipedia_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Tools.OpenURLInBrowser("http://en.wikipedia.org/wiki/WP:FRONDS");
        }

        private void linkToolserver_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Tools.OpenURLInBrowser("http://toolserver.org/~jarry/fronds/");
        }

    }
}
