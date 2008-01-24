/*
Copyright (C) 2008 Sam Reed

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

namespace AutoWikiBrowser
{
    public partial class ExternalProgram : Form, WikiFunctions.Plugin.IModule
    {
        public ExternalProgram()
        {
            InitializeComponent();
        }

        public bool ModuleEnabled
        {
            get { return chkEnabled.Checked; }
            set { chkEnabled.Checked = value; }
        }

        private void radio_CheckedChanged(object sender, EventArgs e)
        {
            groupBox2.Enabled = radFile.Checked;
        }

        private void chkEnabled_CheckedChanged(object sender, EventArgs e)
        {
            groupBox1.Enabled = chkSkip.Enabled = chkEnabled.Checked;
        }

        const string file = "article.txt";

        public string ProcessArticle(string ArticleText, string ArticleTitle, int wikiNamespace, out string Summary, out bool Skip)
        {
            string OrigText = ArticleText;
            Skip = false;
            Summary = "";

            try
            {
                System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo();
                psi.WorkingDirectory = txtWorkingDir.Text;
                psi.FileName = txtProgram.Text;

                psi.Arguments = txtParameters.Text;

                if (radFile.Checked)
                {
                    System.IO.StreamWriter writer = new System.IO.StreamWriter(file);
                    writer.Write(ArticleText);

                    writer.Close();
                }
                else
                    psi.Arguments = psi.Arguments.Replace("%%articletext%%", ArticleText);

                System.Diagnostics.Process p = System.Diagnostics.Process.Start(psi);
                p.WaitForExit();

                if (System.IO.File.Exists(psi.WorkingDirectory + "\\" + file))
                {
                    System.IO.StreamReader reader = System.IO.File.OpenText(psi.WorkingDirectory + "\\" + file);

                    ArticleText = reader.ReadToEnd();

                    reader.Close();

                    Skip = (chkSkip.Checked && (ArticleText == OrigText));
                }
                return ArticleText;
            }
            catch
            {
                return OrigText;
            }
        }

        private void ExternalProgram_Load(object sender, EventArgs e)
        {
            groupBox1.Enabled = chkSkip.Enabled = chkEnabled.Checked;
            ToolTip tip = new ToolTip();
            string tooltip = "If you need a paramter of the actual article text, please use \"%%articletext%%\"";

            tip.SetToolTip(txtParameters, tooltip);
            tip.SetToolTip(radParameter, tooltip);
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (!chkEnabled.Checked || !string.IsNullOrEmpty(txtWorkingDir.Text) && !string.IsNullOrEmpty(txtProgram.Text) && (radFile.Checked & !string.IsNullOrEmpty(txtFile.Text)) || (radParameter.Checked && !string.IsNullOrEmpty(txtParameters.Text)))
                this.Close();
            else
                MessageBox.Show("Please make sure all relevant fields are completed");
                
        }

        private void ExternalProgram_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(openFileDialog1.InitialDirectory))
                openFileDialog1.InitialDirectory = Application.StartupPath;

            openFileDialog1.ShowDialog();

            if (!string.IsNullOrEmpty(openFileDialog1.FileName))
            {
                txtProgram.Text = openFileDialog1.FileName.Remove(0, openFileDialog1.FileName.LastIndexOf("\\")).Replace("\\", "");
                txtWorkingDir.Text = openFileDialog1.FileName.Remove(openFileDialog1.FileName.LastIndexOf("\\"));
            }
        }
    }
}