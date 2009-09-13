// EXCLUDED FROM COMPILATION
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.CodeDom.Compiler;

namespace WikiFunctions.CustomModules
{
    /// <summary>
    /// Doesn't work with both latest stable and CTP releases of Nemerle: compiler throws
    /// exceptions. Therefore even the syntax of code snippets is not fully verified.
    /// </summary>
    public class NemerleModuleCompiler : CustomModuleCompiler
    {
        public NemerleModuleCompiler()
        {
//#pragma warning disable 0618
//            var asm = Assembly.LoadWithPartialName("Nemerle.Compiler");
//#pragma warning restore 0618
//            Compiler = (CodeDomProvider)Instantiate(asm, "Nemerle.Compiler.NemerleCodeProvider");

            var path = "C:\\Program Files\\Nemerle";
            var asm = Assembly.LoadFile(System.IO.Path.Combine(path, "Nemerle.Compiler.dll"));
            Compiler = (CodeDomProvider)Instantiate(asm, "Nemerle.Compiler.NemerleCodeProvider");
        }

        public override string Name
        {
            get { return "Nemerle"; }
        }

        public override string CodeStart
        {
            get
            {
                return @"using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using WikiFunctions;

namespace AutoWikiBrowser.CustomModules
{
    class CustomModule : WikiFunctions.Plugin.IModule
    {
        awb: WikiFunctions.Plugin.IAutoWikiBrowser;

        public CustomModule(WikiFunctions.Plugin.IAutoWikiBrowser mAWB)
        {
           awb =  mAWB;
        }
";
            }
        }

        public override string CodeEnd
        {
            get
            {
                return @"    }
}";
            }
        }

        public override string CodeExample
        {
            get
            {
                return @"        public ProcessArticle(ArticleText: string, ArticleTitle: string, wikiNamespace: int, out Summary: string, out Skip: bool): string
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
