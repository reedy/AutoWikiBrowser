using System;
using System.Collections.Generic;
using System.Text;

using WikiFunctions;
using WikiFunctions.Plugin;

namespace WikiFunctions.Plugins.ListMaker.TypoScan
{
    class TypoScanAWBPlugin : IAWBPlugin
    {
        #region IAWBPlugin Members

        internal static IAutoWikiBrowser AWB;

        public void Initialise(IAutoWikiBrowser sender)
        {
            AWB = sender;
        }

        public string Name
        {
            get { return "TypoScan AWB Plugin]]"; }
        }

        public string WikiName
        {
            get
            {
                return "[[Wikipedia:TypoScan|TypoScan AWB Plugin]], Plugin version " +
                    System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public string ProcessArticle(IAutoWikiBrowser sender, ProcessArticleEventArgs eventargs)
        {
            return eventargs.ArticleText;
        }

        public void LoadSettings(object[] prefs)
        { }

        public object[] SaveSettings()
        {
            return new object[0];
        }

        public void Reset()
        { }

        public void Nudge(out bool Cancel)
        {
            Cancel = false;
        }

        public void Nudged(int Nudges)
        { }

        #endregion
    }
}
