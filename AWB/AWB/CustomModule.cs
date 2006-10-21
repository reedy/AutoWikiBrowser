using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.CodeDom.Compiler;
using System.Reflection;
using WikiFunctions.Plugin;

namespace AutoWikiBrowser
{
    public partial class CustomModule : Form
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

        IModule m = null;
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
                }
                else
                {
                    lblStatus.Text = "Module compiled and loaded";
                    lblStatus.BackColor = Color.LightGreen;
                }
            }
        }

        string codestart = "";
        string codeend = "";
        string codeexample = @"";

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

                string code = codestart + txtCode.Text + codeend;

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
                    string errors = "Compilation failed:\r\n";
                    foreach (CompilerError err in results.Errors)
                    {
                        errors += String.Format("Error: {0}\r\nLine: {1}\r\nNumber: {2}\r\n\r\n", err.ErrorText, err.Line, err.ErrorNumber);
                    }

                    MessageBox.Show(this, errors, "There were compilation errors");

                    Module = null;
                    return;
                }

                Type[] types = results.CompiledAssembly.GetTypes();

                foreach (Type t in types)
                {
                    Type g = t.GetInterface("IModule");

                    if (g != null)
                    {
                        Module = (IModule)Activator.CreateInstance(t);
                    }
                }
            }
            catch (Exception ex)
            {
                Module = null;
                MessageBox.Show(ex.Message);
            }
        }

        private void btnDone_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CSParser_FormClosing(object sender, FormClosingEventArgs e)
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
                codeend = "    }\r\n}";
                codeexample = @"        public string ProcessArticle(string ArticleText, string ArticleTitle, int Namespace, out string Summary, out bool Skip)
        {
            Skip = false;
            Summary = ""test"";

            ArticleText = ""test\r\n\r\n"" + ArticleText;

            return ArticleText;
        }";
            }
            else
            {
                codestart = @"Imports WikiFunctions

Public Class Class1
    Implements WikiFunctions.Plugin.IModule

";
                codeend = "\r\nEnd Class";
                codeexample = @"";
            }

            lblStart.Text = codestart;
            lblEnd.Text = codeend;
            txtCode.Text = codeexample;
        }
    }
}