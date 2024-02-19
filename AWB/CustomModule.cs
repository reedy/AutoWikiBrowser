﻿/*
Autowikibrowser
Copyright (C) 2007 Martin Richards
(C) 2007 Stephen Kennedy (Kingboyk) http://www.sdk-software.com/

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
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.CodeDom.Compiler;
using WikiFunctions.Plugin;
using WikiFunctions;
using WikiFunctions.CustomModules;

namespace AutoWikiBrowser
{
    internal sealed partial class CustomModule : Form
    {
        public CustomModule()
        {
            InitializeComponent();
            cmboLang.Items.Clear();
            cmboLang.Items.AddRange(CustomModuleCompiler.GetList());
            cmboLang.SelectedIndex = 0;
            txtCode.Text = _codeExample;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Code
        {
            get { return txtCode.Text; }
            set { txtCode.Text = value.Replace("\r\n\r\n", "\r\n"); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Language
        {
            get { return cmboLang.SelectedItem.ToString(); }
            set
            {
                foreach (
                    CustomModuleCompiler c in
                        from CustomModuleCompiler c in cmboLang.Items where c.CanHandleLanguage(value) select c)
                {
                    cmboLang.SelectedItem = c;
                    return;
                }

                // All older configs that specified index instead of language name
                // could have used only C#.
                cmboLang.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public CustomModuleCompiler Compiler
        {
            get { return (CustomModuleCompiler) cmboLang.SelectedItem; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool ModuleEnabled
        {
            get { return chkModuleEnabled.Checked; }
            set
            {
                chkModuleEnabled.Checked = value;
                if (value)
                    MakeModule();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool ModuleUsable
        {
            get { return ModuleEnabled && Module != null; }
        }

        private const string BuiltPrefix = "Custom Module Built At: ";

        private IModule _m;

        /// <summary>
        /// 
        /// </summary>
        public IModule Module
        {
            get { return _m; }
            private set
            {
                _m = value;

                if (value == null)
                {
                    lblStatus.Text = "No module loaded";
                    lblStatus.BackColor = Color.Orange;
                    lblBuilt.Text = BuiltPrefix + "n/a";
                }
                else
                {
                    lblStatus.Text = "Module compiled and loaded";
                    lblStatus.BackColor = Color.LightGreen;
                    lblBuilt.Text = BuiltPrefix + DateTime.Now;
                }
            }
        }

        private string _codeStart = "", _codeEnd = "", _codeExample = @"";

        /// <summary>
        /// 
        /// </summary>
        public void MakeModule()
        {
            try
            {
                CompilerParameters cp = new CompilerParameters
                {
                    GenerateExecutable = false,
                    IncludeDebugInformation = false
                };

                // Microsoft.GeneratedCode check is for Mono compatibility
                foreach (
                    var path in
                        AppDomain.CurrentDomain.GetAssemblies()
                            .Where(
                                asm =>
                                    !asm.FullName.Contains("Microsoft.GeneratedCode") &&
                                    !asm.Location.Contains("mscorlib") &&
                                    !string.IsNullOrEmpty(asm.Location))
                            .Select(asm => asm.Location))
                {
                    cp.ReferencedAssemblies.Add(path);
                }

                CompilerResults results = Compiler.Compile(txtCode.Text, cp);

                bool hasErrors = false;
                if (results.Errors.Count > 0)
                {
                    StringBuilder builder = new StringBuilder(); // "Compilation messages:\r\n");
                    foreach (CompilerError err in results.Errors)
                    {
                        hasErrors |= !err.IsWarning;

                        if (err.Line > 0)
                            builder.AppendFormat("Line {0}, col {1}: ", err.Line, err.Column);

                        if (!string.IsNullOrEmpty(err.ErrorNumber))
                            builder.AppendFormat("[{0}] ", err.ErrorNumber);

                        builder.Append(err.ErrorText);
                        builder.Append("\r\n");
                    }

                    using (CustomModuleErrors error = new CustomModuleErrors())
                    {
                        error.ErrorText = builder.ToString();
                        error.Text = "Compilation " + (hasErrors ? "errors" : "warnings");
                        error.ShowDialog(this);
                    }

                    if (hasErrors)
                    {
                        Module = null;
                        return;
                    }
                }

                foreach (Type t in results.CompiledAssembly.GetTypes().Where(t => t.GetInterface("IModule") != null))
                {
                    Module = (IModule) Activator.CreateInstance(t, Program.AWB);
                }
            }
            catch (Exception ex)
            {
                Module = null;
                ErrorHandler.HandleException(ex);
            }
        }

        public void SetModuleNotBuilt()
        {
            Module = null;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void CustomModule_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void btnMake_Click(object sender, EventArgs e)
        {
            MakeModule();
        }

        private void cmboLang_SelectedIndexChanged(object sender, EventArgs e)
        {
            var c = Compiler;
            _codeStart = c.CodeStart;
            _codeExample = c.CodeExample;
            _codeEnd = c.CodeEnd;

            lblStart.Text = _codeStart;
            txtCode.Text = _codeExample;
            lblEnd.Text = _codeEnd;
        }

        private void guideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(@"A module allows you to process the article text using your own .NET code.

Use the ""Make module"" button to compile and load the code.

The method ""ProcessArticle"" is called when AWB is applying all its own processes. Do not change the signature of this method.

The int value ""Namespace"" gives you the key of the namespace, e.g. mainspace is 0 etc., the string ""Summary"" must be set to the message to append to the summary (or can be an empty string), the bool ""Skip"" must be set whether to skip the article or not.

For more detailed information, click Help -> Manual on the Custom Module window.",
                "Guide", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void chkModuleEnabled_CheckedChanged(object sender, EventArgs e)
        {
            btnMake.Enabled = chkModuleEnabled.Checked;
        }

        private void chkFixedwidth_CheckedChanged(object sender, EventArgs e)
        {
            txtCode.Font =
                lblStart.Font =
                    lblEnd.Font =
                        chkFixedwidth.Checked ? new Font("Courier New", 9) : new Font("Microsoft Sans Serif", 8);
        }

        #region txtCode Context Menu

        private void menuitemMakeFromTextBoxUndo_Click(object sender, EventArgs e)
        {
            txtCode.Undo();
        }

        private void menuitemMakeFromTextBoxCut_Click(object sender, EventArgs e)
        {
            txtCode.Cut();
        }

        private void menuitemMakeFromTextBoxCopy_Click(object sender, EventArgs e)
        {
            txtCode.Copy();
        }

        private void menuitemMakeFromTextBoxPaste_Click(object sender, EventArgs e)
        {
            txtCode.Paste();
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtCode.SelectAll();
        }

        #endregion

        private Point _oldPosition;
        private Size _oldSize;

        private void showOnlyCodeBoxToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            var check = showOnlyCodeBoxToolStripMenuItem.Checked;
            lblStart.Visible = !check;
            lblEnd.Visible = !check;
            if (check)
            {
                // remember current
                _oldPosition = txtCode.Location;
                _oldSize = txtCode.Size;
                txtCode.Dock = DockStyle.Fill;
            }
            else
            {
                txtCode.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
                // reinstate previous position, box doesn't resize itself properly
                txtCode.Location = _oldPosition;
                txtCode.Size = _oldSize;
            }
        }

        private void toolStripTextBox1_Click(object sender, EventArgs e)
        {
            toolStripTextBox1.Text = "";
        }

        private void toolStripTextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsNumber(e.KeyChar) && e.KeyChar != 8)
                e.Handled = true;

            if (e.KeyChar == '\r' && !string.IsNullOrEmpty(toolStripTextBox1.Text))
            {
                e.Handled = true;
                txtCode.GoToLine(int.Parse(toolStripTextBox1.Text));
                mnuTextBox.Hide();
            }
        }

        private void manualToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Tools.OpenENArticleInBrowser("Wikipedia:AutoWikiBrowser/Custom_Modules", false);
        }
    }
}