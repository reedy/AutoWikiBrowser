/*
WhatLinkHereAllNSPlugin - work around for Wiki's that wont take blnamespace="" (ie nothing) as all namespaces. So this builds a list and appends it
Copyright (C) 2008 Sam Reed

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

using System.Collections.Generic;
using System.Text;
using WikiFunctions.Lists;
using WikiFunctions.Plugin;

namespace WikiFunctions.Plugins.ListMaker.WhatLinksHereAllNSPlugin
{
    public class WhatLinksHereAllNSPlugin : WhatLinksHereAllNSListProvider, IListMakerPlugin
    {
        public override List<Article> MakeList(string[] searchCriteria)
        {
            StringBuilder builder = new StringBuilder();
            foreach (int id in Variables.Namespaces.Keys)
            {
                if (id > 0)
                    builder.Append(id + "|");
            }

            return MakeList(builder.ToString().Substring(0, builder.Length - 1), searchCriteria);
        }

        #region IListProvider Members
        public string Name
        { get { return "What Links Here All NS Plugin"; } }

        public override string DisplayText
        { get { return "What links here (p)(all NS)"; } }
        #endregion
    }
}
