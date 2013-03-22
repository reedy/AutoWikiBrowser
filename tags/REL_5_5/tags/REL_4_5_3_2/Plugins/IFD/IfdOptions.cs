/*
Copyright (C) 2007

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
using System.Windows.Forms;
using WikiFunctions;
using System.Text.RegularExpressions;
using WikiFunctions.Background;

namespace AutoWikiBrowser.Plugins.IFD
{
    internal sealed partial class IfdOptions : Form
    {
        public IfdOptions()
        {
            InitializeComponent();
        }

        static string prevContent = "";

        readonly Dictionary<string, string> ToDo = new Dictionary<string, string>();

        public new void Show()
        {
            chkSkip.Checked = IfdAWBPlugin.Settings.Skip;
            chkComment.Checked = IfdAWBPlugin.Settings.Comment;

            Grid.Rows.Clear();
            foreach (KeyValuePair<string, string> p in IfdAWBPlugin.Settings.Images)
            {
                Grid.Rows.Add(new [] { p.Key, p.Value });
            }

            txtBacklog.Text = prevContent;

            if (ShowDialog() != DialogResult.OK) return;

            IfdAWBPlugin.Settings.Skip = chkSkip.Checked;
            IfdAWBPlugin.Settings.Comment = chkComment.Checked;

            IfdAWBPlugin.Settings.Images = ToDo;

            prevContent = txtBacklog.Text;
        }

        private void RefreshImgs()
        {
           Regex imgReplace = new Regex(@"\*+\s*\[\[:" + Variables.NamespacesCaseInsensitive[6] +
                @"([^\]\|]*)(?:\|[^\]]*|)\]\][^\[]*\[\[:" + Variables.NamespacesCaseInsensitive[6] + @"([^\]\|]*)\]\]");

           Regex imgRemove = new Regex(@"\*+\s*\[\[:" + Variables.NamespacesCaseInsensitive[6] +
               @"\s*([^\]\|]*)(?:\|[^\]]*|)\]\][^\[]*?$");

            Grid.Rows.Clear();
            foreach (string s in txtBacklog.Lines)
            {
                Match m = imgReplace.Match(s.Replace("‎", ""));
                if (m.Success)
                {
                    Grid.Rows.Add(new [] { m.Groups[1].Value.Trim().Replace("_", " "), m.Groups[2].Value.Trim().Replace("_", "") });
                    continue;
                }
                m = imgRemove.Match(s.Replace("‎", ""));
                if (m.Success)
                    Grid.Rows.Add(new [] { m.Groups[1].Value.Trim().Replace("_", " "), "" });
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            try
            {
                timer.Enabled = false;
                RefreshImgs();
                txtBacklog.Text = txtBacklog.Text.Replace("_", " ");

                ToDo.Clear();
                foreach (DataGridViewRow r in Grid.Rows)
                {
                    if (!r.IsNewRow && !string.IsNullOrEmpty((string)r.Cells[0].Value)
                        && !ToDo.ContainsKey((string)r.Cells[0].Value))
                        ToDo.Add((string)r.Cells[0].Value, (string)r.Cells[1].Value);
                }

                BackgroundRequest req = new BackgroundRequest();
                if (ToDo.Count > 0)
                {
                    string[] imgs = new string[ToDo.Count];
                    ToDo.Keys.CopyTo(imgs, 0);
                    Enabled = false;

                    req.GetList(new WikiFunctions.Lists.ImageFileLinksListProvider(), imgs);
                    req.Wait();

                    Enabled = true;

                    if (req.Result != null && req.Result is List<Article>)
                        IfdAWBPlugin.AWB.ListMaker.Add((List<Article>)req.Result);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
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
            RefreshImgs();
        }
    }

    [Serializable]
    internal sealed class IfdSettings
    {
        public bool Enabled;
        public Dictionary<string, string> Images = new Dictionary<string, string>();
        public bool Comment = true;
        public bool Skip = true;
    }
}