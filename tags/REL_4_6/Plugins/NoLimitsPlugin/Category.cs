/*

This program is free software; you can redistribute it and/or modify
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

using WikiFunctions.Lists;
using WikiFunctions.Plugin;

namespace WikiFunctions.Plugins.ListMaker.NoLimitsPlugin
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

        public override System.Collections.Generic.List<Article> MakeList(params string[] searchCriteria)
        {
            if (Variables.User.IsBot || Variables.User.IsAdmin)
                return base.MakeList(searchCriteria);

            Tools.MessageBox("Action only allowed for Admins and Bot accounts");
            return null;
        }

        public override string DisplayText
        { get { return "Category (NL, Admin & Bot)"; } }

        public string Name
        { get { return "CategoryNoLimitsForAdminAndBotsPlugin"; } }
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

        public override System.Collections.Generic.List<Article> MakeList(params string[] searchCriteria)
        {
            if (Variables.User.IsBot || Variables.User.IsAdmin)
                return base.MakeList(searchCriteria);

            Tools.MessageBox("Action only allowed for Admins and Bot accounts");
            return null;
        }

        public override string DisplayText
        { get { return "Category (NL, Admin & Bot, recursive)"; } }

        public string Name
        { get { return "CategoryRecursiveNoLimitsForAdminAndBotsPlugin"; } }
    }
}