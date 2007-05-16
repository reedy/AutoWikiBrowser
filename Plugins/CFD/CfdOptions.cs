using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WikiFunctions;
using WikiFunctions.Lists;
using System.Text.RegularExpressions;
using WikiFunctions.AWBSettings;
using WikiFunctions.Background;

namespace AutoWikiBrowser.Plugins.CFD
{
    public partial class CfdOptions : Form
    {
        public CfdOptions()
        {
            InitializeComponent();
        }

        ListMaker listMaker;

        static string prevContent = "";

        Dictionary<string, string> ToDo = new Dictionary<string, string>();

        public void Show(CfdSettings cfd, ListMaker lm)
        {
            listMaker = lm;
            chkEnabled.Checked = cfd.Enabled;
            chkSkip.Checked = cfd.Skip;

            Grid.Rows.Clear();
            foreach (KeyValuePair<string, string> p in cfd.Categories)
            {
                Grid.Rows.Add(new string[2] { p.Key, p.Value });
            }

            txtBacklog.Text = prevContent;

            if (ShowDialog() != DialogResult.OK) return;

            cfd.Enabled = chkEnabled.Checked;
            cfd.Skip = chkSkip.Checked;

            cfd.Categories = ToDo;

            prevContent = txtBacklog.Text;
        }

        private void RefreshCats()
        {
           Regex CatReplace = new Regex(@"\*+\s*\[\[:" + Variables.NamespacesCaseInsensitive[14] +
                @"([^\]\|]*)(?:\|[^\]]*|)\]\][^\[]*\[\[:" + Variables.NamespacesCaseInsensitive[14] + @"([^\]\|]*)\]\]");

           Regex CatRemove = new Regex(@"\*+\s*\[\[:" + Variables.NamespacesCaseInsensitive[14] +
               @"\s*([^\]\|]*)(?:\|[^\]]*|)\]\][^\[]*?$");

            Grid.Rows.Clear();
            foreach (string s in txtBacklog.Lines)
            {
                Match m = CatReplace.Match(s);
                //*
                if (m.Success)
                {
                    Grid.Rows.Add(new string[2] { m.Groups[1].Value.Trim(), m.Groups[2].Value.Trim() });
                    continue;
                }//*/

                m = CatRemove.Match(s);
                if (m.Success)
                    Grid.Rows.Add(new string[2] { m.Groups[1].Value.Trim(), "" });
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            timer.Enabled = false;
            RefreshCats();

            ToDo.Clear();
            foreach (DataGridViewRow r in Grid.Rows)
            {
                if (!r.IsNewRow && (string)r.Cells[0].Value != "")
                    ToDo.Add((string)r.Cells[0].Value, (string)r.Cells[1].Value);
            }

            BackgroundRequest req = new BackgroundRequest();
            if (ToDo.Count > 0)
            {
                string[] cats = new string[ToDo.Count];
                ToDo.Keys.CopyTo(cats, 0);
                Enabled = false;

                GetLists.QuietMode = true;
                req.GetFromCategories(cats);

                while (!req.Done) Application.DoEvents();
                GetLists.QuietMode = false;
                Enabled = true;

                if (req.Result != null && req.Result is List<Article>)
                    listMaker.Add((List<Article>)req.Result);
            }

            DialogResult = DialogResult.OK;
        }
    
        private void txtBacklog_TextChanged(object sender, EventArgs e)
        {
            timer.Enabled = true;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            timer.Enabled = false;
            RefreshCats();
        }
    }
}