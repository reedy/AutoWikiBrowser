// EXCLUDED FROM COMPILATION
using System;
using System.Reflection;
using System.CodeDom.Compiler;
using System.IO;

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
            var asm = LoadAssembly(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                "Nemerle\\Nemerle.Compiler.dll"),
                "Nemerle");
            Compiler = (CodeDomProvider)asm.CreateInstance("Nemerle.Compiler.NemerleCodeProvider");
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
using WikiFunctions.Plugin;

namespace AutoWikiBrowser.CustomModules
{
    class CustomModule : WikiFunctions.Plugin.IModule
    {
        mutable mAWB: WikiFunctions.Plugin.IAutoWikiBrowser;

        public CustomModule(awb: IAutoWikiBrowser)
        {
           mAWB = awb;
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
                return @"        public ProcessArticle(ArticleText: string, ArticleTitle: string, wikiNamespace: int, Summary: out string, Skip: out bool): string
        {
            Skip = false;
            Summary = ""test"";

            ArticleText = ""test \r\n\r\n"" + ArticleText;

            ArticleText;
        }";
            }
        }
    }
}
