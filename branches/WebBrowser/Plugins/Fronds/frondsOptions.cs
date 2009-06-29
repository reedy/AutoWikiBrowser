using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Xml;
using WikiFunctions;

namespace Fronds
{
    public partial class FrondsOptions : Form
    {
        public FrondsOptions(IEnumerable<string> possibleFronds)
        {
            InitializeComponent();

            //Inefficient, but does work..
            foreach (string text in possibleFronds)
            {
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
        }

        private void btnOptionsCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnOptionsOK_Click(object sender, EventArgs e)
        {
            //TODO:Improve this code so we're not reloading Fronds unnecesserily
            Fronds.Replacements.Clear();
            Fronds.Settings.EnabledFilenames.Clear();

            // Preserve enabled filenames
            foreach (string item in listOptionsFronds.CheckedItems)
            {
                Fronds.Settings.EnabledFilenames.Add(item.Substring((item.LastIndexOf("(") + 1),
                                    (item.Length - item.LastIndexOf("(") - 2)));
            }

            //Loaded selected fronds
            foreach (int index in listOptionsFronds.CheckedIndices)
            {
                XmlDocument xd = new XmlDocument();
                xd.LoadXml(Tools.GetHTML(Fronds.BaseURL + Fronds.PossibleFilenames[index]));

                if (xd["fronds"] == null || xd["fronds"]["frond"] == null)
                    return;

                foreach (XmlNode xn in xd["fronds"]["frond"].GetElementsByTagName("item"))
                {
                    if (xn.ChildNodes.Count != 4)
                        continue;

                    Fronds.Replacements.Add(new Frond(xn.ChildNodes[0].InnerText,
                                  (xn.ChildNodes[2].InnerText.ToLower() == "yes")
                                      ? RegexOptions.None
                                      : RegexOptions.IgnoreCase, xn.ChildNodes[1].InnerText));
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
            Tools.OpenURLInBrowser(Fronds.BaseURL);
        }
    }
}
