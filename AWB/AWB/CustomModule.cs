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

        public void MakeModule()
        {
            try
            {
                ICodeCompiler compiler;
                if (cmboLang.SelectedIndex == 0)
                {
                    Microsoft.CSharp.CSharpCodeProvider codeProvider = new Microsoft.CSharp.CSharpCodeProvider();
                    compiler = codeProvider.CreateCompiler();
                }
                else
                {
                    Microsoft.VisualBasic.VBCodeProvider codeProvider = new Microsoft.VisualBasic.VBCodeProvider();
                    compiler = codeProvider.CreateCompiler();
                }

                CompilerParameters parameters = new CompilerParameters();

                parameters.GenerateExecutable = false;
                parameters.IncludeDebugInformation = false;

                // Add available assemblies
                foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
                {
                    parameters.ReferencedAssemblies.Add(asm.Location);
                }

                string code = codestart + txtCode.Text + codeend;

                CompilerResults results = compiler.CompileAssemblyFromSource(parameters, code);

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
using WikiFunctions;

namespace AutoWikiBrowser
{
    class Module1 : WikiFunctions.Plugin.IModule
    {
        public string ProcessArticle(string ArticleText, string ArticleTitle, int Namespace, out string Summary, out bool Skip)
        {";
                codeend = @"        }
    }    
}";
            }
            else
            {
                codestart = @"Imports WikiFunctions

Public Class Class1
    Implements WikiFunctions.Plugin.IModule


    Public Function ProcessArticle(ByVal ArticleText As String, ByVal ArticleTitle As String, ByVal [Namespace] As Integer, ByRef Summary As String, ByRef Skip As Boolean) As String Implements WikiFunctions.Plugin.ICS.ProcessArticle";
                codeend = @"
    End Function
End Class";
            }

            lblStart.Text = codestart;
            lblEnd.Text = codeend;
        }
    }    
}