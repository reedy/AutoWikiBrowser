/*

 * This program is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 2 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA

 */

using WikiFunctions.Logging;
using WikiFunctions.API;

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

        public ArticleEX(string title, string text)
            : base(title, text)
        {
            InitialiseLogListener();
        }

        public ArticleEX(PageInfo page)
            : base(page)
        {
            InitialiseLogListener();
        }

        // be careful to call this before creating the next ArticleWithLogging
        // An alternative approach would be to call Close() from the other Log tab, where the AWBLogListener
        // gets added to either the saved or skipped list. We can be fairly sure it will always get called
        // at the right time then.
        internal static void Close()
        { Program.MyTrace.RemoveListener("AWB"); }

        internal static ArticleEX SwitchToNewArticleObject(ArticleEX old, ArticleEX @new)
        {
            if(old != null && old.LogListener != null)
                Close(); // remove old AWBLogListener from MyTrace collection
            @new.InitialiseLogListener(); // create new listener and add to collection
            return @new;
        }
    }
}
