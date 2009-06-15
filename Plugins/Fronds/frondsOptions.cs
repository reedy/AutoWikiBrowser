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
            foreach (string item in listOptionsFronds.CheckedItems)
            {
                Fronds.Settings.EnabledFilenames.Add(item.Substring((item.LastIndexOf("(") + 1),
                                                                    (item.Length - item.LastIndexOf("(") - 2)));
            }

            //Loaded selected fronds
            foreach (int index in listOptionsFronds.CheckedIndices)
            {
                XmlTextReader objXmlTextReader =
                    new XmlTextReader(Fronds.BaseURL + Fronds.PossibleFilenames[index]);
                string sName = "";

                string find = "", replace = "", caseSensitive = "";
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
                                    find = value;
                                    break;
                                case "replace":
                                    replace = value;
                                    break;
                                case "casesensitive":
                                    caseSensitive = value.ToLower();
                                    break;
                            }
                            break;
                    }

                    if (string.IsNullOrEmpty(find) || string.IsNullOrEmpty(replace) || string.IsNullOrEmpty(caseSensitive)) continue;

                    Fronds.Replacements.Add(new Frond(find,
                                                      (caseSensitive.ToLower() == "yes")
                                                          ? RegexOptions.None
                                                          : RegexOptions.IgnoreCase, replace));

                    find = "";
                    replace = "";
                    caseSensitive = "";
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
