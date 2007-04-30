namespace WikiFunctions
{
    using Microsoft.VisualBasic.CompilerServices;
    using System;
    using System.Reflection;

    [StandardModule]
    public sealed class WikiFunctions2
    {
        public static System.Version Version
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version;
            }
        }
    }
}

