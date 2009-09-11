using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom.Compiler;

namespace WikiFunctions
{
    public interface ICustomModule
    {
        string Name
        { get; }

        string Language
        { get; }

        CompilerResults Compile(string sourceCode);
    }
}
