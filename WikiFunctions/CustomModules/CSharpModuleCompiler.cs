using Microsoft.CSharp;
using System.Collections.Generic;

namespace WikiFunctions.CustomModules
{

    public class CSharpCustomModule : CustomModuleCompiler
    {
        public CSharpCustomModule()
        {
            Compiler = Globals.SystemCore3500Available
                // Parameterised constructor only available in .NET >= 3.5
                // https://msdn.microsoft.com/en-us/library/bb537926(v=vs.90).aspx
                ? new CSharpCodeProvider(new Dictionary<string, string> {{"CompilerVersion", "v3.5"}})
                : new CSharpCodeProvider();
        }

        public override string Name
        {
            get { return (Globals.SystemCore3500Available ? "C# 3.5" : "C# 2.0"); }
        }

        public override string CodeStart
        {
            get
            {
                return @"using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using WikiFunctions;" + (Globals.SystemCore3500Available ? @"
using System.Linq;" : "") +
                       @"

namespace AutoWikiBrowser.CustomModules
{
    class CustomModule : WikiFunctions.Plugin.IModule
    {
        WikiFunctions.Plugin.IAutoWikiBrowser awb;

        public CustomModule(WikiFunctions.Plugin.IAutoWikiBrowser _awb)
        {
           awb =  _awb;
        }
";
            }
        }

        public override string CodeEnd
        {
            get { return @"    }
}"; }
        }

        public override string CodeExample
        {
            get
            {
                return
                    @"        public string ProcessArticle(string ArticleText, string ArticleTitle, int wikiNamespace, out string Summary, out bool Skip)
        {
            Skip = false;
            Summary = ""test"";

            ArticleText = ""test \r\n\r\n"" + ArticleText;

            return ArticleText;
        }";
            }
        }
    }
}