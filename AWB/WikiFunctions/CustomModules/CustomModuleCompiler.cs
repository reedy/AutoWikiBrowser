using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom.Compiler;
using System.Reflection;

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
        /// This function checks if the current compiler can compile sources in a given language.
        /// By default, the language should match the current compiler's language name exactly,
        /// but descendants can override it so that, for example, C# 4.0 compiler could accept
        /// C# 2.0 cources.
        /// </summary>
        /// <param name="language">Language name to check</param>
        public virtual bool CanHandleLanguage(string language)
        {
            return Name == language;
        }

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

            AddToList(modules, typeof(VbModuleCompiler));
            //AddToList(modules, typeof(NemerleModuleCompiler));

            return modules.ToArray();
        }

        #region Helpers

        private static void AddToList(List<CustomModuleCompiler> modules, Type type)
        {
            // If an exception is thrown, the language is unavailable - ignore silently
            try
            {
                modules.Add((CustomModuleCompiler)Instantiate(type));
            }
            catch { }
        }

        protected static object Instantiate(Type type)
        {
            return type.GetConstructor(new Type[] { }).Invoke(new Type[] { });
        }

        protected static T Instantiate<T>()
        {
            return (T)Instantiate(typeof(T));
        }

        protected static object Instantiate(Assembly asm, string typeName)
        {
            return Instantiate(asm.GetType(typeName, true));
        }

        #endregion
    }
}
