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
using System.Windows.Forms;
using System.IO;

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

        public WikiFunctions.AWBSettings.ExternalProgramPrefs Settings
        {
            get
            {
                return new WikiFunctions.AWBSettings.ExternalProgramPrefs
                {
                    Enabled = chkEnabled.Checked,
                    Skip = chkSkip.Checked,
                    Program = txtProgram.Text,
                    Parameters = txtParameters.Text,
                    PassAsFile = radFile.Checked,
                    OutputFile = txtFile.Text
                };
            }
            set
            {
                chkEnabled.Checked = value.Enabled;
                chkSkip.Checked = value.Skip;
                txtProgram.Text = value.Program;
                txtParameters.Text = value.Parameters;

                radFile.Checked = value.PassAsFile;
                radParameter.Checked = !value.PassAsFile;
                txtFile.Text = value.OutputFile;
            }
        }

        private void chkEnabled_CheckedChanged(object sender, EventArgs e)
        {
            groupBox1.Enabled = chkSkip.Enabled = chkEnabled.Checked;
        }

        //Look at User:Pseudomonas/AWBPerlWrapperPlugin
        public string ProcessArticle(string articleText, string articleTitle, int @namespace, out string summary, out bool skip)
        {
            string origText = articleText;
            skip = false;
            summary = "";

            string ioFile = txtFile.Text;

            try
            {
                System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo
                                                              {
                                                                  WorkingDirectory = Path.GetDirectoryName(txtProgram.Text),
                                                                  FileName = Path.GetFileName(txtProgram.Text),
                                                                  Arguments = txtParameters.Text.Replace("%%file%%", txtFile.Text)
                                                              };

                if (radFile.Checked)
                {
                    if (txtFile.Text.Contains("\\"))
                        WikiFunctions.Tools.WriteTextFileAbsolutePath(articleText, ioFile, false);
                    else
                        WikiFunctions.Tools.WriteTextFile(articleText, ioFile, false);
                }
                else
                    psi.Arguments = psi.Arguments.Replace("%%articletext%%", articleText);

                System.Diagnostics.Process p = System.Diagnostics.Process.Start(psi);

                p.WaitForExit();

                if (File.Exists(ioFile))
                {
                    using (StreamReader reader = File.OpenText(ioFile))
                    {
                        articleText = reader.ReadToEnd();
                        reader.Close();
                    }

                    skip = (chkSkip.Checked && (articleText == origText));

                    File.Delete(ioFile);
                }
                return articleText;
            }
            catch (Exception ex)
            {
                // Most, if not all exceptions here are related to user wrong user input
                // or environment specifics, so ErrorHandler is not needed.
                MessageBox.Show(ActiveForm, ex.Message, "External processing error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                return origText;
            }
        }

        private void ExternalProgram_Load(object sender, EventArgs e)
        {
            groupBox1.Enabled = chkSkip.Enabled = chkEnabled.Checked;
            ToolTip tip = new ToolTip();

            string tooltip = "If you need a paramter of the actual article text, please use \"%%articletext%%\". If you want to use the value of the Input/Output file, please use \"%%file%%\"";
            tip.SetToolTip(txtParameters, tooltip);
            tip.SetToolTip(radParameter, tooltip);

            tooltip = "This is the file that AWB will output to if necessary, and also the file it will try and read back in";
            tip.SetToolTip(txtFile, tooltip);
            tip.SetToolTip(label4, tooltip);
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (!chkEnabled.Checked || !string.IsNullOrEmpty(txtProgram.Text) && !string.IsNullOrEmpty(txtFile.Text) || (radParameter.Checked && !string.IsNullOrEmpty(txtParameters.Text)))
                Close();
            else
                MessageBox.Show("Please make sure all relevant fields are completed");
        }

        private void ExternalProgram_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void btnSelectProgram_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(openProgram.InitialDirectory))
                openProgram.InitialDirectory = Application.StartupPath;

            if (openProgram.ShowDialog() == DialogResult.OK)
            {
                txtProgram.Text = openProgram.FileName;
            }
        }

        private void btnSelectIO_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(openIO.InitialDirectory))
                openIO.InitialDirectory = Application.StartupPath;

            if (openIO.ShowDialog() == DialogResult.OK)
            {
                txtFile.Text = openIO.FileName;
            }
        }

        private void RadioButtonCheckedChanged(object sender, EventArgs e)
        {
            btnSelectIO.Enabled = txtFile.Enabled = radFile.Checked;
        }
    }
}