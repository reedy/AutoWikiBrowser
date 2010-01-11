/*
Autowikibrowser
Copyright (C) 2007 Mets501, Stephen Kennedy

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 2 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
*/

using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace WikiFunctions.Controls
{
    /// <summary>
    /// Provides a web-based help browser. Must be inherited.
    /// </summary>
    public partial class Help : Form
    {
        // TODO: Add menu and buttons for back, forward, home (=URL property), etc
        protected Help()
        {
            InitializeComponent();
        }

        protected virtual void Help_Load(object sender, EventArgs e)
        {
            Navigate(URL);
        }

        public virtual void Navigate(string url)
        {
            webBrowserHelp.Navigate(url);
        }

        public virtual void NavigateEn(string Article)
        {
            webBrowserHelp.Navigate(Tools.GetENLinkWithSimpleSkinAndLocalLanguage(Article));
        }

        protected internal virtual string URL { get { return ""; } }

        protected Regex TOC = new Regex(@"<LI class=toclevel-([12345])><A href=""#(.*?)""><SPAN class=tocnumber>.*?</SPAN> <SPAN class=toctext>.*?</SPAN></A>", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        protected virtual void webBrowserHelp_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (webBrowserHelp.Url == new Uri(URL))
            {
                lbTopics.Items.Clear();
                string html = webBrowserHelp.Document.Body.InnerHtml;
                foreach (Match m in TOC.Matches(html))
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

        protected virtual void lbTopics_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                webBrowserHelp.Navigate(URL + "#" + lbTopics.SelectedItem.ToString().Replace("• ", "").Trim().Replace(" ", "_").Replace("(", ".28").Replace(")", ".29"));
            }
            catch { }
        }

        protected virtual void label1_Click(object sender, EventArgs e)
        { webBrowserHelp.Navigate(URL + "#toc"); }
    }
}