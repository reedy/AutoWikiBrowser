using System.Collections.Generic;
using WikiFunctions.Lists.Providers;
using WikiFunctions.Plugin;

namespace WikiFunctions.Plugins.ListMaker.NoLimitsPlugin
{
    class Pages
    {
        /// <summary>
        /// 
        /// </summary>
        public class AllPagesAllNSNoLimitsProvider : AllPagesSpecialPageProvider, IListMakerPlugin
        {
            public AllPagesAllNSNoLimitsProvider()
            {
                Limit = 1000000;
            }

            public override List<Article> MakeList(params string[] searchCriteria)
            {
                if (!Base.CanUsePlugin())
                {
                    return null;
                }
                List<Article> list = new List<Article>();
                list.AddRange(GetArticles(0, list.Count));
                foreach (int ns in Variables.Namespaces.Keys)
                {
                    if (ns < 0) // Stupid not proper namespaces...
                        continue;
                    list.AddRange(GetArticles(ns, list.Count));
                }
                return list;
            }

            protected List<Article> GetArticles(int ns, int count)
            {
                return ApiMakeList("list=allpages&apnamespace=" + ns + "&aplimit=max", count);
            }

            public override string DisplayText
            { get { return "All Pages (NL, in all NS)"; } }

            public override string UserInputTextBoxText
            { get { return ""; } }

            public override bool UserInputTextBoxEnabled
            { get { return false; } }

            public string Name
            { get { return "AllPageInAllNSNLForAdminAndBotsPlugin"; } }
        }
    }
}
