using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom.Compiler;

namespace WikiFunctions.CustomModules
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class CustomModuleCompiler
    {
        /// <summary>
        /// Human-readable language name
        /// </summary>
        public abstract string Name
        { get; }

        /// <summary>
        /// Code to be prepended to module's source text
        /// </summary>
        public abstract string CodeStart
        { get; }

        /// <summary>
        /// Code to be apppended to module's source text
        /// </summary>
        public abstract string CodeEnd
        { get; }

        /// <summary>
        /// Text to be used as default content for code input box
        /// </summary>
        public abstract string CodeExample
        { get; }

        /// <summary>
        /// Compiles given source code
        /// </summary>
        /// <param name="sourceCode">Source code to compile. It will be automatically wrapped between
        /// CodeStart and CodeEnd.</param>
        /// <param name="parameters">Compilation options.</param>
        /// <returns></returns>
        public virtual CompilerResults Compile(string sourceCode, CompilerParameters parameters)
        {
            var src = CodeStart + sourceCode + "\r\n" + CodeEnd;

            return Compiler.CompileAssemblyFromSource(parameters, src);
        }

        /// <summary>
        /// Enforces that every class derived from this one will be properly displayed in a combo box.
        /// </summary>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// To be assigned in descendant class' constructor
        /// </summary>
        protected CodeDomProvider Compiler;

        /// <summary>
        /// Returns the list of currently available compiler modules
        /// </summary>
        public static CustomModuleCompiler[] GetList()
        {
            var modules = new List<CustomModuleCompiler>();

            // for compatibility and user experience reason, we should maintain this order
            modules.Add(new CSharpCustomModule());

            // If an exception is thrown, VB is unavailable - ignore silently
            try
            {
                modules.Add(new VbModuleCompiler());
            }
            catch { }

            return modules.ToArray();
        }
    }
}
