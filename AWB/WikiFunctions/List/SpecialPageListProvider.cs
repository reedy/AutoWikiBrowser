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
        private static BindingList<IListProvider> listItems = new BindingList<IListProvider>();

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

            txtPages.Enabled = cboNamespace.Enabled = ((ISpecialPageProvider)cmboSourceSelect.SelectedItem).UserInputTextBoxEnabled;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    interface ISpecialPageProvider
    {
        List<Article> MakeList(int Namespace, params string[] searchCriteria);
        string UserInputTextBoxText { get; }
        bool UserInputTextBoxEnabled { get; }
        bool PagesNeeded { get; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class AllPagesSpecialPageProvider : ApiListProviderBase, ISpecialPageProvider
    {
        #region Tags: <allpages>/<p>
        static readonly List<string> pe = new List<string>(new string[] { "p" });
        protected override ICollection<string> PageElements
        {
            get { return pe; }
        }

        static readonly List<string> ac = new List<string>(new string[] { "allpages" });
        protected override ICollection<string> Actions
        {
            get { return ac; }
        }
        #endregion

        protected string from = "apfrom";

        #region ISpecialPageProvider Members

        public override List<Article> MakeList(params string[] searchCriteria)
        {
            return MakeList(0, searchCriteria);
        }

        public virtual List<Article> MakeList(int Namespace, params string[] searchCriteria)
        {
            List<Article> list = new List<Article>();

            foreach (string page in searchCriteria)
            {
                string url = Variables.URLLong + "api.php?action=query&list=allpages&" + from + "=" + HttpUtility.UrlEncode(page) + "&apnamespace=" + Namespace + "&aplimit=max&format=xml";

                list.AddRange(ApiMakeList(url, list.Count));
            }
            return list;
        }

        public override string UserInputTextBoxText
        {
            get { return "All Pages"; }
        }

        public virtual bool PagesNeeded
        {
            get { return false; }
        }
        #endregion

        public override bool UserInputTextBoxEnabled
        {
            get { return true; }
        }

        public override void Selected()
        { }

        public override string DisplayText
        {
            get { return UserInputTextBoxText; }
        }
    }

    /// <summary>
    /// 
    /// </summary>
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

        public override bool PagesNeeded
        {
            get { return true; }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class RecentChangesSpecialPageProvider : ApiListProviderBase, ISpecialPageProvider
    {
        #region Tags: <recentchanges>/<rc>
        static readonly List<string> pe = new List<string>(new string[] { "rc" });
        protected override ICollection<string> PageElements
        {
            get { return pe; }
        }

        static readonly List<string> ac = new List<string>(new string[] { "recentchanges" });
        protected override ICollection<string> Actions
        {
            get { return ac; }
        }
        #endregion

        #region ISpecialPageProvider Members
        public override List<Article> MakeList(params string[] searchCriteria)
        {
            return MakeList(0, searchCriteria);
        }

        public List<Article> MakeList(int Namespace, params string[] searchCriteria)
        {
            List<Article> list = new List<Article>();

            foreach (string page in searchCriteria)
            {
                string url = Variables.URLLong + "api.php?action=query&list=recentchanges&rctitles=" + HttpUtility.UrlEncode(page) + "&rcnamespace=" + Namespace + "&rclimit=max&format=xml";

                list.AddRange(ApiMakeList(url, list.Count));
            }
            return list;
        }

        public override string DisplayText
        {
            get { return "Recent Changes"; }
        }

        public bool PagesNeeded
        {
            get { return false; }
        }
        #endregion

        public override string UserInputTextBoxText
        {
            get { return DisplayText; }
        }

        public override bool UserInputTextBoxEnabled
        {
            get { return false; }
        }

        public override void Selected()
        { }
    }

    /// <summary>
    /// Returns a list of new pages
    /// </summary>
    public class NewPagesListProvider : ApiListProviderBase
    {
        #region Tags: <recentchanges>/<rc>
        static readonly List<string> pe = new List<string>(new string[] { "rc" });
        protected override ICollection<string> PageElements
        {
            get { return pe; }
        }

        static readonly List<string> ac = new List<string>(new string[] { "recentchanges" });
        protected override ICollection<string> Actions
        {
            get { return ac; }
        }
        #endregion

        public override List<Article> MakeList(params string[] searchCriteria)
        {
            List<Article> list = new List<Article>();

            string url = Variables.URLLong + "api.php?action=query&list=recentchanges"
                + "&rclimit=max&rctype=new&rcshow=!redirects&rcnamespace=0&format=xml";

            list.AddRange(ApiMakeList(url, list.Count));

            return list;
        }

        #region ListMaker properties
        public override string DisplayText
        { get { return "New articles"; } }

        public override string UserInputTextBoxText
        { get { return ""; } }

        public override bool UserInputTextBoxEnabled
        { get { return false; } }

        public override void Selected() { }
        #endregion
    }
}
