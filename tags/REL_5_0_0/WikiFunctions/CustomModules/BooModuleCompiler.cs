using System;
using System.Reflection;
using System.CodeDom.Compiler;
using System.IO;

namespace WikiFunctions.CustomModules
{
    /// <summary>
    /// Couldn't figure out how to replace the out modifiers that Boo doesn't support.
    /// Hence, disabled.
    /// </summary>
    public class BooModuleCompiler : CustomModuleCompiler
    {
        public BooModuleCompiler()
        {
            var asm = LoadAssembly(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                "Boo\\bin\\Boo.Lang.CodeDom.dll"), 
                "Boo.");
            Compiler = (CodeDomProvider)asm.CreateInstance("Boo.Lang.CodeDom.BooCodeProvider");
        }

        public override string Name
        {
            get { return "Boo"; }
        }

        public override string CodeStart
        {
            get
            {
                return @"namespace AutoWikiBrowser.CustomModules
import System
import System.Collections.Generic
import System.Text
import System.Text.RegularExpressions
import WikiFunctions

class CustomModule (WikiFunctions.Plugin.IModule):
    awb as WikiFunctions.Plugin.IAutoWikiBrowser

    def constructor(mAWB as WikiFunctions.Plugin.IAutoWikiBrowser):
        awb =  mAWB

";
            }
        }

        public override string CodeEnd
        {
            get
            {
                return "";
            }
        }

        public override string CodeExample
        {
            get
            {
                return @"   def ProcessArticle(ArticleText as string, ArticleTitle as string, wikiNamespace as int, ref Summary as string, ref Skip as bool) as string:
        Skip = false
        Summary = ""test""
        ArticleText = ""test \r\n\r\n"" + ArticleText
        return ArticleText
";
            }
        }
    }
}
