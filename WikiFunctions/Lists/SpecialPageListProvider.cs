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
using System.Windows.Forms;

namespace WikiFunctions.Lists
{
    /// <summary>
    /// Gets the list of pages on the Named Special Pages
    /// </summary>
    public partial class SpecialPageListProvider : Form, IListProvider
    {
        private static readonly BindingList<IListProvider> listItems = new BindingList<IListProvider>();

        public SpecialPageListProvider()
        {
            InitializeComponent();

            if (listItems.Count == 0)
            {
                listItems.Add(new PrefixIndexSpecialPageProvider());
                listItems.Add(new AllPagesSpecialPageProvider());
                listItems.Add(new RecentChangesSpecialPageProvider());
                listItems.Add(new LinkSearchListProvider());
                listItems.Add(new RandomPagesListProvider());
            }

            cmboSourceSelect.DataSource = listItems;
            cmboSourceSelect.DisplayMember = "DisplayText";
            cmboSourceSelect.ValueMember = "DisplayText";
        }

        public SpecialPageListProvider(params IListProvider[] providers)
            :this()
        {
            foreach (IListProvider prov in providers)
            {
                if (prov is ISpecialPageProvider)
                    listItems.Add(prov);
            }
        }

        public List<Article> MakeList(params string[] searchCriteria)
        {
            txtPages.Text = "";

            List<Article> list = new List<Article>();

            if (ShowDialog() == DialogResult.OK)
            {
                searchCriteria = txtPages.Text.Split(new [] { '|' });

                ISpecialPageProvider item = (ISpecialPageProvider)cmboSourceSelect.SelectedItem;

                if (!string.IsNullOrEmpty(txtPages.Text))
                    list = item.MakeList(Tools.CalculateNS(cboNamespace.Text), searchCriteria);
                else if (item.PagesNeeded)
                    MessageBox.Show("Pages needed!");
                else
                    list = item.MakeList(Tools.CalculateNS(cboNamespace.Text), new [] { "" });
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

        private void SpecialPageListProvider_Load(object sender, EventArgs e)
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

            ISpecialPageProvider prov = (ISpecialPageProvider)cmboSourceSelect.SelectedItem;

            txtPages.Enabled = prov.UserInputTextBoxEnabled;
            cboNamespace.Enabled = prov.NamespacesEnabled;
        }
    }
}
