using System;
using System.Collections.Generic;
using System.Text;
using WikiFunctions.Logging;

namespace AutoWikiBrowser
{
    // A bit of a kludge this, as Article wasn't designed to write to a Trace listener collection, 
    // and I don't want to make the underlying object too complicated.
    internal sealed class ArticleEX : WikiFunctions.Article
    {
        public override IAWBTraceListener Trace
        { get { return Program.MyTrace; } }

        public override AWBLogListener InitialiseLogListener()
        {
            InitialiseLogListener("AWB", Program.MyTrace);
            Program.MyTrace.AddListener("AWB", mAWBLogListener);
            return mAWBLogListener;
        }

        public ArticleEX(string mName)
            : base(mName)
        { InitialiseLogListener(); }

        // be careful to call this before creating the next ArticleWithLogging
        // An alternative approach would be to call Close() from the other Log tab, where the AWBLogListener
        // gets added to either the saved or skipped list. We can be fairly sure it will always get called
        // at the right time then.
        internal void Close()
        { Program.MyTrace.RemoveListener("AWB"); }

        internal static ArticleEX SwitchToNewArticleObject(ArticleEX Old,
            ArticleEX New)
        {
            if(Old != null && Old.LogListener != null)
                Old.Close(); // remove old AWBLogListener from MyTrace collection
            New.InitialiseLogListener(); // create new listener and add to collection
            return New;
        }
    }
}
