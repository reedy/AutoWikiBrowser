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

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;

namespace WikiFunctions.MWB
{
    /*
    class RegexXml
    {
        public const string Name = "regex";
        public static void Write(XmlWriter w, RegexOptions o)
        {
            w.WriteStartElement(Name);
            if (o != RegexOptions.None)
                w.WriteAttributeString("options", ((int)o).ToString());
            w.WriteEndElement();
        }
        public static RegexOptions Read(XmlTextReader rd)
        {
            RegexOptions res = RegexOptions.None;
            if (rd.Name == Name)
            {
                if (rd.MoveToAttribute("options"))
                    res = (RegexOptions)Convert.ToInt32(rd.Value);
            }
            return res;
        }
    }
     */

}
