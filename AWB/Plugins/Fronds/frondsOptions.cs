using System;
using System.Collections.Generic;
using System.Windows.Forms;
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
            List<int> indices = new List<int>();
            List<string> filenames = new List<string>();
            foreach (int index in listOptionsFronds.CheckedIndices)
            {
                indices.Add(index);
            }
            foreach (string item in listOptionsFronds.CheckedItems)
            {
                string filename = item.Substring((item.LastIndexOf("(") + 1), (item.Length - item.LastIndexOf("(") - 2));
                filenames.Add(filename);
            }
            Fronds.Settings.EnabledFilenames = filenames;
            OnButtonClicked(new OptionsOKClickedEventArgs(indices));
            Close();
        }
        public event Fronds.OptionsOKClickedEventHandler ButtonClicked;

        // add the event invoker method
        protected virtual void OnButtonClicked(OptionsOKClickedEventArgs e)
        {
            if (ButtonClicked != null) ButtonClicked(this, e);
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
    public class OptionsOKClickedEventArgs : EventArgs
    {
        List<int> enabledIndices;
        public List<int> EnabledIndices { get { return enabledIndices; } }
        public OptionsOKClickedEventArgs(List<int> enabledIndices)
        {
            this.enabledIndices = enabledIndices;
        }
    }
}
