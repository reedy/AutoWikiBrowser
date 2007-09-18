/*
    Copyright (C) 2007 Martin Richards

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

using System;
using System.Collections.Generic;
using System.Text;

namespace WikiFunctions.MWB
{
    sealed class Utility
    {
        public static string GetFilenameFromPath(string path)
        {
            string res = path;
            res = res.Remove(0, res.LastIndexOf("\\") + 1);
            return res;
        }

        // returns true if testnode is the same or a subnode of refnode
        public static bool IsSubnodeOf(
          System.Windows.Forms.TreeNode refnode,
          System.Windows.Forms.TreeNode testnode)
        {
            for (
              System.Windows.Forms.TreeNode t = testnode;
              t != null; t = t.Parent)
            {
                if (ReferenceEquals(refnode, t))
                    return true;
            }
            return false;
        }

        public static string ReadAllElementContent(ref System.Xml.XmlTextReader r)
        {
            System.Xml.WhitespaceHandling saved_handling = r.WhitespaceHandling;
            try
            {
                r.WhitespaceHandling = System.Xml.WhitespaceHandling.All;
                string res = r.ReadElementContentAsString();
                if (r.NodeType == System.Xml.XmlNodeType.Whitespace)
                    r.Read();
                return res;
            }
            finally
            {
                r.WhitespaceHandling = saved_handling;
            }
        }
    }
}
