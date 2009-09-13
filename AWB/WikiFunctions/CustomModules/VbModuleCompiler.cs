using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom.Compiler;
using System.Collections;

namespace WikiFunctions.CustomModules
{
    public class VbModuleCompiler : CustomModuleCompiler
    {
        public VbModuleCompiler()
        {
            // For compatibility reasons we can't not use the Microsoft.VisualBasic
            // namespace directly, as some Linux systems may come without the mono-vbnc package,
            // and this may result in "assembly not found" exception where it could be avoided.

            // We assume here that VBCodeProvider is in the same DLL as Component (System.dll) for
            // Windows. Seems to be the case for Mono on Linux, too.
            var asm = typeof(System.ComponentModel.Component).Assembly;

            Compiler = (CodeDomProvider)Instantiate(asm, "Microsoft.VisualBasic.VBCodeProvider");
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
Imports Microsoft.VisualBasic
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
    }
}
