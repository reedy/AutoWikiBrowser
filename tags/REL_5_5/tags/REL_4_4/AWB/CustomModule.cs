/*
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Text.RegularExpressions;
using WikiFunctions.Plugin;
using WikiFunctions;

namespace AutoWikiBrowser
{
    internal sealed partial class CustomModule : Form
    {
        public CustomModule()
        {
            InitializeComponent();
            cmboLang.SelectedIndex = 0;
            txtCode.Text = codeexample;
        }

        public string Code
        {
            get { return txtCode.Text; }
            set { txtCode.Text = value; }
        }

        public int Language
        {
            get { return cmboLang.SelectedIndex; }
            set { cmboLang.SelectedIndex = value; }
        }

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

        IModule m;
        public IModule Module
        {
            get { return m; }
            private set
            {
                m = value;

                if (value == null)
                {
                    lblStatus.Text = "No module loaded";
                    lblStatus.BackColor = Color.Orange;
                    lblBuilt.Text = "Custom Module Built At: n/a";
                }
                else
                {
                    lblStatus.Text = "Module compiled and loaded";
                    lblStatus.BackColor = Color.LightGreen;
                    lblBuilt.Text = "Custom Module Built At: " + DateTime.Now;
                }
            }
        }

        string codestart = "", codeend = "", codeexample = @"";

        public void MakeModule()
        {
            try
            {
                CompilerParameters cp = new CompilerParameters();
                cp.GenerateExecutable = false;
                cp.IncludeDebugInformation = false;

                foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
                {
                    cp.ReferencedAssemblies.Add(asm.Location);
                }

                string code = codestart + Regex.Replace(txtCode.Text, "VbCrLf", "\"/r/n\"", RegexOptions.IgnoreCase) + "\r\n" + codeend;

                CompilerResults results;
                if (cmboLang.SelectedIndex == 0)
                {
                    Microsoft.CSharp.CSharpCodeProvider codeProvider = new Microsoft.CSharp.CSharpCodeProvider();
                    results = codeProvider.CompileAssemblyFromSource(cp, code);
                }
                else
                {
                    Microsoft.VisualBasic.VBCodeProvider codeProvider = new Microsoft.VisualBasic.VBCodeProvider();
                    results = codeProvider.CompileAssemblyFromSource(cp, code);
                }
                if (results.Errors.Count > 0)
                {
                    StringBuilder builder = new StringBuilder("Compilation failed:");
                    foreach (CompilerError err in results.Errors)
                    {
                        builder.AppendLine(String.Format("Error: {0}\r\nLine: {1}\r\nNumber: {2}\r\n", err.ErrorText, err.Line, err.ErrorNumber));
                    }

                    MessageBox.Show(this, builder.ToString(), "Compilation errors");

                    Module = null;
                    return;
                }

                Type[] types = results.CompiledAssembly.GetTypes();

                foreach (Type t in types)
                {
                    Type g = t.GetInterface("IModule");

                    if (g != null)
                        Module = (IModule)Activator.CreateInstance(t);
                }
            }
            catch (Exception ex)
            {
                Module = null;
                ErrorHandler.Handle(ex);
            }
        }

        public void SetModuleNotBuilt()
        {
            Module = null;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CustomModule_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void btnMake_Click(object sender, EventArgs e)
        {
            MakeModule();
        }

        private void cmboLang_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmboLang.SelectedIndex == 0)
            {
                codestart = @"using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using WikiFunctions;

namespace AutoWikiBrowser
{
    class Module1 : WikiFunctions.Plugin.IModule
    {
";
                codeexample = @"        public string ProcessArticle(string ArticleText, string ArticleTitle, int wikiNamespace, out string Summary, out bool Skip)
        {
            Skip = false;
            Summary = ""test"";

            ArticleText = ""test \r\n\r\n"" + ArticleText;

            return ArticleText;
        }";
                codeend = @"    }
}";
            }
            else
            {
                codestart = @"Imports System.Collections.Generic
Imports System.Text
Imports System.Text.RegularExpressions
Imports WikiFunctions

Namespace AutoWikiBrowser
    Class Module1
        Implements WikiFunctions.Plugin.IModule
";

                codeexample = @"        Public Function ProcessArticle(ByVal ArticleText As String, ByVal ArticleTitle As String, ByVal wikiNamespace As Integer, ByRef Summary As String, ByRef Skip As Boolean) As String Implements WikiFunctions.Plugin.IModule.ProcessArticle
            Skip = False
            Summary = ""test""

            ArticleText = ""test "" & VbCrLf & VbCrLf & ArticleText
            
            Return ArticleText
        End Function";

                codeend = @"     End Class
End Namespace";
            }

            lblStart.Text = codestart;
            txtCode.Text = codeexample;
            lblEnd.Text = codeend;
        }

        private void guideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(@"A module allows you to process the article text using your own dotnet code.

Use the ""Makes module"" button to compile and load the code.

The method ""ProcessArticle"" is called when AWB is applying all its own processes. Do not change the sigature of this method.

The int value ""Namespace"" gives you the key of the namespace, e.g. mainspace is 0 etc., the string ""Summary"" must be set to the message to append to the summary (or can be an empty string), the bool ""Skip"" must be set whether to skip the article or not.", "Guide", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void chkModuleEnabled_CheckedChanged(object sender, EventArgs e)
        {
            btnMake.Enabled = chkModuleEnabled.Checked;
        }

        private void chkFixedwidth_CheckedChanged(object sender, EventArgs e)
        {
            Font a;
            if (chkFixedwidth.Checked)
                a = new Font("Courier New", 9);
            else
                a = new Font("Microsoft Sans Serif", 8);

            txtCode.Font = lblStart.Font = lblEnd.Font = a;
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
    }
}