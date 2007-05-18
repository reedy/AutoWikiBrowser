using System;
using System.Collections.Generic;
using System.Text;
using WikiFunctions.Logging;

namespace AutoWikiBrowser
{
    // A bit of a kludge this, as Article wasn't designed to write to a Trace listener collection, 
    // and I don't want to make the underlying object too complicated.
    internal sealed class ArticleWithLogging : WikiFunctions.Article
    {
        public override IAWBTraceListener Trace
        { get { return GlobalObjects.MyTrace; }
        }

        public override AWBLogListener InitialiseLogListener()
        {
            InitialiseLogListener("AWB", GlobalObjects.MyTrace);
            GlobalObjects.MyTrace.AddListener("AWB", mAWBLogListener);
            return mAWBLogListener;
        }

        public ArticleWithLogging(string mName)
            : base(mName)
        { InitialiseLogListener(); }

        // be careful to call this before creating the next ArticleWithLogging
        // An alternative approach would be to call Close() from the other Log tab, where the AWBLogListener
        // gets added to either the saved or skipped list. We can be fairly sure it will always get called
        // at the right time then.
        internal void Close()
        { GlobalObjects.MyTrace.RemoveListener("AWB"); }

        internal static ArticleWithLogging SwitchToNewArticleObject(ArticleWithLogging Old, 
            ArticleWithLogging New)
        {
            if(Old.LogListener != null)
                Old.Close(); // remove old AWBLogListener from MyTrace collection
            New.InitialiseLogListener(); // create new listener and add to collection
            return New;
        }
    }
}
