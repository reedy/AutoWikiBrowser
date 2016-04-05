using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Microsoft.CSharp;

namespace AutoWikiBrowser
{
    public partial class CSharpEval : Form
    {
        public CSharpEval()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox2.Clear();
            string code = @"using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using WikiFunctions;
using System.Linq;

namespace CSharpEvaluator {
    class CSharpEval {
        public object EvalCode() {
            return " + textBox1.Text + @";
        }
    }
}
";

            CSharpCodeProvider c = new CSharpCodeProvider(new Dictionary<string, string> { { "CompilerVersion", "v3.5" } });

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

            CompilerResults results = c.CompileAssemblyFromSource(cp, code);

            if (results.Errors.Count > 0)
            {
                bool hasErrors = false;
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
                    return;
                }
            }

            Assembly a = results.CompiledAssembly;
            object o = a.CreateInstance("CSharpEvaluator.CSharpEval");

            Type t = o.GetType();
            MethodInfo mi = t.GetMethod("EvalCode");

            object s = mi.Invoke(o, null);
            textBox2.Text = s.ToString();
        }
    }
}
