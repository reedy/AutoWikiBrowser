using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace AutoWikiBrowser
{
    public partial class Help : Form
    {
        public Help()
        {
            InitializeComponent();
        }

        private void Help_Load(object sender, EventArgs e)
        {
            webBrowserHelp.Navigate("http://en.wikipedia.org/wiki/Wikipedia:AutoWikiBrowser/User_manual");
        }

        Regex TOC = new Regex(@"<LI class=toclevel-([12345])><A href=""#(.*?)""><SPAN class=tocnumber>.*?</SPAN> <SPAN class=toctext>.*?</SPAN></A>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        
        private void webBrowserHelp_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (webBrowserHelp.Url == new Uri("http://en.wikipedia.org/wiki/Wikipedia:AutoWikiBrowser/User_manual"))
            {
                lbTopics.Items.Clear();
                string HTML = webBrowserHelp.Document.Body.InnerHtml;
                foreach (Match m in TOC.Matches(HTML))
                {
                    if (m.Groups[1].Value == "1")
                        lbTopics.Items.Add(m.Groups[2].Value.Replace("_", " ").Replace(".28", "(").Replace(".29", ")"));
                    else if (m.Groups[1].Value == "2")
                        lbTopics.Items.Add("  • " + m.Groups[2].Value.Replace("_", " ").Replace(".28", "(").Replace(".29", ")"));
                    else if (m.Groups[1].Value == "3")
                        lbTopics.Items.Add("      • " + m.Groups[2].Value.Replace("_", " ").Replace(".28", "(").Replace(".29", ")"));
                    else if (m.Groups[1].Value == "4")
                        lbTopics.Items.Add("          • " + m.Groups[2].Value.Replace("_", " ").Replace(".28", "(").Replace(".29", ")"));
                    else if (m.Groups[1].Value == "5")
                        lbTopics.Items.Add("              • " + m.Groups[2].Value.Replace("_", " ").Replace(".28", "(").Replace(".29", ")"));
                }
            }
        }

        private void lbTopics_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                webBrowserHelp.Navigate("http://en.wikipedia.org/wiki/Wikipedia:AutoWikiBrowser/User_manual#" + lbTopics.SelectedItem.ToString().Replace("• ", "").Trim().Replace(" ", "_").Replace("(", ".28").Replace(")", ".29"));
            }
            catch
            {
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
            webBrowserHelp.Navigate("http://en.wikipedia.org/wiki/Wikipedia:AutoWikiBrowser/User_manual#toc");
        }
    }
}