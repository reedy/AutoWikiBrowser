/*
Copyright (C) 2007 Martin Richards
(C) 2008 Sam Reed

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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.Text.RegularExpressions;
using System.Xml;
using System.IO;
using System.Web;

namespace WikiFunctions.Lists
{
    /// <summary>
    /// Gets the list of pages on the Named Special Pages
    /// </summary>
    public partial class SpecialPageListProvider : Form, IListProvider
    {
        private static BindingList<ISpecialPageProvider> listItems = new BindingList<ISpecialPageProvider>();

        public SpecialPageListProvider()
        {
            InitializeComponent();

            if (listItems.Count == 0)
            {
                listItems.Add(new PrefixIndexSpecialPageProvider());
                listItems.Add(new AllPagesSpecialPageProvider());
                listItems.Add(new RecentChangesSpecialPageProvider());
            }

            cmboSourceSelect.DataSource = listItems;
            cmboSourceSelect.DisplayMember = "DisplayText";
            cmboSourceSelect.ValueMember = "DisplayText";
        }

        public List<Article> MakeList(params string[] searchCriteria)
        {
            txtPages.Text = "";

            List<Article> list = new List<Article>();

            if (this.ShowDialog() == DialogResult.OK)
            {
                searchCriteria = txtPages.Text.Split(new char[] { '|' });

                ISpecialPageProvider item = (ISpecialPageProvider)cmboSourceSelect.SelectedItem;

                if (!string.IsNullOrEmpty(txtPages.Text))
                    list = item.MakeList(Tools.CalculateNS(cboNamespace.Text), searchCriteria);
                else if (item.PagesNeeded)
                    MessageBox.Show("Pages needed!");
                else
                    list = item.MakeList(Tools.CalculateNS(cboNamespace.Text), new string[] { "" });
            }
            
            return Tools.FilterSomeArticles(list);
        }

        public string DisplayText
        { get { return "Special page"; } }

        public string UserInputTextBoxText
        { get { return ""; } }

        public bool UserInputTextBoxEnabled
        { get { return false; } }

        public void Selected() { }

        public bool RunOnSeparateThread
        { get { return true; } }

        private void SpecialPageListMakerProvider_Load(object sender, EventArgs e)
        {
            int currentSelected = cboNamespace.SelectedIndex;
            cboNamespace.Items.Clear();
            foreach (string name in Variables.Namespaces.Values)
            {
                cboNamespace.Items.Add(name);
            }
            cboNamespace.SelectedIndex = currentSelected;
        }

        private void cmboSourceSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DesignMode) return;

            txtPages.Enabled = cboNamespace.Enabled = ((ISpecialPageProvider)cmboSourceSelect.SelectedItem).PagesTextBoxEnabled;
        }
    }

    interface ISpecialPageProvider
    {
        List<Article> MakeList(int Namespace, params string[] searchCriteria);
        string DisplayText { get; }
        bool PagesTextBoxEnabled { get; }
        bool PagesNeeded { get; }
    }

    public class AllPagesSpecialPageProvider : ISpecialPageProvider
    {
        protected string from = "apfrom";

        #region ISpecialPageProvider Members

        public virtual List<Article> MakeList(int Namespace, params string[] searchCriteria)
        {
            List<Article> list = new List<Article>();

            foreach (string s in searchCriteria)
            {
                string url = Variables.URLLong + "api.php?action=query&list=allpages&" + from + "=" + s + "&apnamespace=" + Namespace + "&aplimit=500&format=xml";
                while (true)
                {
                    string html = Tools.GetHTML(url);

                    bool more = false;

                    using (XmlTextReader reader = new XmlTextReader(new StringReader(html)))
                    {
                        while (reader.Read())
                        {
                            if (reader.Name.Equals("p"))
                            {
                                if (reader.MoveToAttribute("title"))
                                {
                                    list.Add(new WikiFunctions.Article(reader.Value.ToString(), 0));
                                }
                            }
                            else if (reader.Name.Equals("allpages") && from != "apfrom") //dont want all pages loading EVERYTHING
                            {
                                reader.MoveToAttribute("apfrom");
                                if (reader.Value.Length > 0)
                                {
                                    string continueFrom = Tools.WikiEncode(reader.Value.ToString());
                                    url = Variables.URLLong + "api.php?action=query&list=allpages&" + from + "=" + s + "&apnamespace=" + Namespace + "&aplimit=50&format=xml&apfrom=" + continueFrom;
                                    more = true;
                                }
                            }
                        }
                    }

                    if (!more)
                        break;
                }
            }
            return list;
        }

        public virtual string DisplayText
        {
            get { return "All Pages"; }
        }

        public virtual bool PagesTextBoxEnabled
        {
            get { return true; }
        }

        public virtual bool PagesNeeded
        {
            get { return false; }
        }
        #endregion
    }

    public class PrefixIndexSpecialPageProvider : AllPagesSpecialPageProvider
    {
        public PrefixIndexSpecialPageProvider()
        {
            from = "apprefix";
        }

        public override List<Article> MakeList(int Namespace, params string[] searchCriteria)
        {
            return base.MakeList(Namespace, searchCriteria);
        }

        public override string DisplayText
        {
            get { return "All pages with prefix (Prefixindex)"; }
        }

        public override bool PagesTextBoxEnabled
        {
            get { return true; }
        }

        public override bool PagesNeeded
        {
            get { return true; }
        }
    }

    public class RecentChangesSpecialPageProvider : ISpecialPageProvider
    {
        #region ISpecialPageProvider Members

        public List<Article> MakeList(int Namespace, params string[] searchCriteria)
        {
            List<Article> list = new List<Article>();

            foreach (string s in searchCriteria)
            {
                string url = Variables.URLLong + "api.php?action=query&list=recentchanges&rctitles=" + s + "&rcnamespace=" + Namespace + "&rclimit=500&format=xml";
                while (true)
                {
                    string html = Tools.GetHTML(url);

                    bool more = false;

                    using (XmlTextReader reader = new XmlTextReader(new StringReader(html)))
                    {
                        while (reader.Read())
                        {
                            if (reader.Name.Equals("rc"))
                            {
                                if (reader.MoveToAttribute("title"))
                                {
                                    list.Add(new WikiFunctions.Article(reader.Value.ToString(), 0));
                                }
                            }
                        }
                    }

                    if (!more)
                        break;
                }
            }
            return list;
        }

        public string DisplayText
        {
            get { return "Recent Changes"; }
        }

        public bool PagesTextBoxEnabled
        {
            get { return true; }
        }

        public bool PagesNeeded
        {
            get { return false; }
        }
        #endregion
    }
}
