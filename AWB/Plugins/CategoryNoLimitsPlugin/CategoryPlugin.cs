using WikiFunctions.Lists;
using WikiFunctions.Plugin;

namespace WikiFunctions.Plugins.ListMaker.CategoryNoLimitsPlugin
{
    /// <summary>
    /// Category list getter with no limits for admin and bot users (wont work for non admin/bots)
    /// </summary>
    public class CategoryNoLimitsForAdminAndBotsPlugin : CategoryListProvider, IListMakerPlugin
    {
        public CategoryNoLimitsForAdminAndBotsPlugin()
        {
            Limit = 1000000;
        }

        public override System.Collections.Generic.List<Article> MakeList(string[] searchCriteria)
        {
            if (Variables.User.IsBot || Variables.User.IsAdmin)
                return base.MakeList(searchCriteria);

            Tools.MessageBox("Action only allowed for Admins and Bot accounts");
            return null;
        }

        public override string DisplayText
        { get { return "Category (NL, Admin & Bot)"; } }

        public string Name
        {
            get { return "CategoryNoLimitsForAdminAndBotsPlugin"; }
        }
    }

    /// <summary>
    /// Recursive category list getter with no limits for admin and bot users (wont work for non admin/bots)
    /// </summary>
    public class CategoryRecursiveNoLimitsForAdminAndBotsPlugin : CategoryRecursiveListProvider, IListMakerPlugin
    {
        public CategoryRecursiveNoLimitsForAdminAndBotsPlugin()
        {
            Limit = 1000000;
            Depth = 1000;
        }

        public override System.Collections.Generic.List<Article> MakeList(string[] searchCriteria)
        {
            if (Variables.User.IsBot || Variables.User.IsAdmin)
                return base.MakeList(searchCriteria);

            Tools.MessageBox("Action only allowed for Admins and Bot accounts");
            return null;
        }

        public override string DisplayText
        { get { return "Category (NL, Admin & Bot, recursive)"; } }

        public string Name
        {
            get { return "CategoryRecursiveNoLimitsForAdminAndBotsPlugin"; }
        }
    }
}