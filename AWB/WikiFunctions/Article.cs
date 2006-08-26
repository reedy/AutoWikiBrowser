/*
Autowikibrowser
Copyright (C) 2006 Martin Richards

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

namespace WikiFunctions
{
    public class Article
    {
        public Article()
        { }

        public Article(string Name)
        {
            this.Name = Name;
            this.NameSpaceKey = Tools.CalculateNS(Name);
            this.EditSummary = "";
        }

        public Article(string Name, int NameSpaceKey)
        {
            this.Name = Name;
            this.NameSpaceKey = NameSpaceKey;
            this.EditSummary = "";
        }

        public int NameSpaceKey;
        public string Name;
        public string EditSummary;

        //public string ArticleText;
        //public string OriginalText;
        

        public string URLEncodedName
        {
            get { return System.Web.HttpUtility.UrlEncode(Name); }
        }

        #region Overrides

        public override string ToString()
        {
            return Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj.GetHashCode() == this.GetHashCode())
                return true;
            else
                return false;
        }

        #endregion
    }
}
