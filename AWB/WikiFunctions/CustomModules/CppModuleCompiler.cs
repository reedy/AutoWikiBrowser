// EXCLUDED FROM COMPILATION
using System.IO;
using System.Reflection;
using System.CodeDom.Compiler;

namespace WikiFunctions.CustomModules
{
    class CppModuleCompiler : CustomModuleCompiler
    {
        public CppModuleCompiler()
        {
            var asm = Assembly.LoadFile(Path.Combine(@"D:\Program Files (x86)\Microsoft Visual Studio 9.0\Common7\IDE\PublicAssemblies", "cppcodeprovider.dll"));
            Compiler = (CodeDomProvider)Instantiate(asm, "Microsoft.VisualC.CppCodeProvider");
        }

        public override string Name
        {
            get { return "C++ 2.0"; }
        }

        public override string CodeStart
        {
            get
            {
                return @"using namespace System;
using namespace System::Collections::Generic;
using namespace System::Text;
using namespace System::Text::RegularExpressions;
using namespace WikiFunctions;
namespace AutoWikiBrowser
{
	namespace CustomModules
	{
		private ref class CustomModule : WikiFunctions::Plugin::IModule
		{
		private:
			WikiFunctions::Plugin::IAutoWikiBrowser ^awb;

		public:
			CustomModule(WikiFunctions::Plugin::IAutoWikiBrowser ^mAWB);
			System::String ^ProcessArticle(System::String ^ArticleText, System::String ^ArticleTitle, int wikiNamespace, [System::Runtime::InteropServices::Out] System::String ^%Summary, [System::Runtime::InteropServices::Out] bool %Skip);
		};

		CustomModule::CustomModule(WikiFunctions::Plugin::IAutoWikiBrowser ^mAWB)
		{
		   awb = mAWB;
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
                return @"		System::String ^CustomModule::ProcessArticle(System::String ^ArticleText, System::String ^ArticleTitle, int wikiNamespace, [System::Runtime::InteropServices::Out] System::String ^%Summary, [System::Runtime::InteropServices::Out] bool %Skip)
		{
			Skip = false;
			Summary = ""test"";

			ArticleText = ""test \r\n\r\n"" + ArticleText;

			return ArticleText;
		}
}";
            }
        }
    }
}
