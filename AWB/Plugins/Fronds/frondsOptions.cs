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
        public FrondsOptions(List<string> possibleFronds)
        {
            InitializeComponent();

            //Also very inefficient, but does work.
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
            // Preserve enabled filenames
            List<string> filenames = new List<string>();
            foreach (string item in listOptionsFronds.CheckedItems)
            {
                string filename = item.Substring((item.LastIndexOf("(") + 1), (item.Length - item.LastIndexOf("(") - 2));
                filenames.Add(filename);
            }
            Fronds.Settings.EnabledFilenames = filenames;

            //Loaded selected fronds
            foreach (int index in listOptionsFronds.CheckedIndices)
            {
                XmlTextReader objXmlTextReader =
                      new XmlTextReader("http://toolserver.org/~jarry/fronds/" + Fronds.PossibleFilenames[index]);
                string sName = "";

                string f = "", r = "", c = "";
                while (objXmlTextReader.Read())
                {
                    switch (objXmlTextReader.NodeType)
                    {
                        case XmlNodeType.Element:
                            sName = objXmlTextReader.Name;
                            break;
                        case XmlNodeType.Text:
                            string value = objXmlTextReader.Value;
                            switch (sName)
                            {
                                case "find":
                                    f = value;
                                    break;
                                case "replace":
                                    r = value;
                                    break;
                                case "casesensitive":
                                    c = value.ToLower();
                                    break;
                            }
                            break;
                    }

                    if (!string.IsNullOrEmpty(f) && !string.IsNullOrEmpty(r) && !string.IsNullOrEmpty(c))
                    {
                        Fronds.Replacements.Add(new Frond(f,
                                                          (c.ToLower() == "yes")
                                                              ? RegexOptions.None
                                                              : RegexOptions.IgnoreCase, r));

                        f = "";
                        r = "";
                        c = "";
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
