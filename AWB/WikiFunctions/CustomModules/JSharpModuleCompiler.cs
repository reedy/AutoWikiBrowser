using System.CodeDom.Compiler;
using System.IO;

namespace WikiFunctions.CustomModules
{
    class JSharpModuleCompiler : CustomModuleCompiler
    {
        public JSharpModuleCompiler()
        {
            var asm = LoadAssembly(Path.Combine(System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory(),
                                                "VJSharpCodeProvider.dll"), "VisualJSharp.");

            Compiler = (CodeDomProvider)asm.CreateInstance("Microsoft.VJSharp.VJSharpCodeProvider");
        }

        public override string Name
        {
            get { return "J# 2.0"; }
        }

        public override string CodeStart
        {
            get
            {
                return @"package AutoWikiBrowser.CustomModules;

import System.*;
import System.Collections.Generic.*;
import System.Text.*;
import System.Text.RegularExpressions.*;
import WikiFunctions.*;

class CustomModule implements WikiFunctions.Plugin.IModule
{
    WikiFunctions.Plugin.IAutoWikiBrowser awb = null;

    public CustomModule(WikiFunctions.Plugin.IAutoWikiBrowser mAWB)
    {
       awb = mAWB;
    }
";
            }
        }

        public override string CodeEnd
        {
            get { return @"
}"; }
        }

        public override string CodeExample
        {
            get
            {
                return @"	public String ProcessArticle(String ArticleText, String ArticleTitle, int wikiNamespace, String Summary, boolean Skip)
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
