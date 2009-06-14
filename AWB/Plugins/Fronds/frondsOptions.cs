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
            //TODO: don't wipe and start again; just add/remove.
            List<String> loadedFinds = new List<String>();
            List<Boolean> loadedCases = new List<Boolean>();
            Fronds.loadedReplaces = new List<String>();
            Fronds.loadedRegexes = new List<Regex>();

            foreach (int index in listOptionsFronds.CheckedIndices)
            {
                XmlTextReader objXmlTextReader =
                      new XmlTextReader("http://toolserver.org/~jarry/fronds/" + Fronds.possibleFilenames[index]);
                string sName = "";
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
                                    loadedFinds.Add(value);
                                    break;
                                case "replace":
                                    Fronds.loadedReplaces.Add(value);
                                    break;
                                case "casesensitive":
                                    loadedCases.Add(value.ToLower() == "yes");
                                    break;
                            }
                            break;
                    }
                }
            }
            for (int i = 0; i < loadedFinds.Count; i++)
            {
                Regex re = new Regex(loadedFinds[i],
                    loadedCases[i] ? RegexOptions.None : RegexOptions.IgnoreCase | RegexOptions.Compiled);
                Fronds.loadedRegexes.Add(re);
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
