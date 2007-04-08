namespace WikiFunctions
{
    using Microsoft.VisualBasic.CompilerServices;
    using System;
    using System.Reflection;
    using System.Threading;

    [StandardModule]
    public sealed class WikiFunctions2
    {
        private static System.Version $STATIC$get_Version$001279$rtn;
        private static StaticLocalInitFlag $STATIC$get_Version$001279$rtn$Init = new StaticLocalInitFlag();

        public static System.Version Version
        {
            get
            {
                Monitor.Enter($STATIC$get_Version$001279$rtn$Init);
                try
                {
                    if ($STATIC$get_Version$001279$rtn$Init.State == 0)
                    {
                        $STATIC$get_Version$001279$rtn$Init.State = 2;
                        $STATIC$get_Version$001279$rtn = Assembly.GetExecutingAssembly().GetName().Version;
                    }
                    else if ($STATIC$get_Version$001279$rtn$Init.State == 2)
                    {
                        throw new IncompleteInitialization();
                    }
                }
                finally
                {
                    $STATIC$get_Version$001279$rtn$Init.State = 1;
                    Monitor.Exit($STATIC$get_Version$001279$rtn$Init);
                }
                return $STATIC$get_Version$001279$rtn;
            }
        }
    }
}

