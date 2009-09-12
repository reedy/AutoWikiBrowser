using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom.Compiler;
using System.Text.RegularExpressions;
using System.Collections;

namespace WikiFunctions.CustomModules
{
    public class VbModuleCompiler : CustomModuleCompiler
    {
        public VbModuleCompiler()
        {
            // TODO: for compatibility reasons we must not use the Microsoft.VisualBasic
            // namespace directly, as some Linux systems may come without the mono-vbnc package,
            // and this may result in "assembly not found" exception where it could be avoided.
            Compiler = new Microsoft.VisualBasic.VBCodeProvider();
        }

        public override string Name
        {
            get { return "VB.NET 2.0"; }
        }

        public override string CodeStart
        {
            get
            {
                return @"Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.Text.RegularExpressions
Imports WikiFunctions

Namespace AutoWikiBrowser.CustomModules
    Class CustomModule
        Implements WikiFunctions.Plugin.IModule

        Dim awb As WikiFunctions.Plugin.IAutoWikiBrowser

        Public Sub New(ByRef mAWB As WikiFunctions.Plugin.IAutoWikiBrowser)
            awb = mAWB
        End Sub
";
            }
        }

        public override string CodeEnd
        {
            get
            {
                return @"     End Class
End Namespace";
            }
        }

        public override string CodeExample
        {
            get
            {
                return @"        Public Function ProcessArticle(ByVal ArticleText As String, ByVal ArticleTitle As String, ByVal wikiNamespace As Integer, ByRef Summary As String, ByRef Skip As Boolean) As String Implements WikiFunctions.Plugin.IModule.ProcessArticle
            Skip = False
            Summary = ""test""

            ArticleText = ""test "" & VbCrLf & VbCrLf & ArticleText
            
            Return ArticleText
        End Function";
            }
        }

        public override CompilerResults Compile(string sourceCode, CompilerParameters parameters)
        {
            sourceCode = Regex.Replace(sourceCode, "VbCrLf", "\"/r/n\"", RegexOptions.IgnoreCase);
            return base.Compile(sourceCode, parameters);
        }
    }
}
