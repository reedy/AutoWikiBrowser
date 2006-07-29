using System;
using System.Collections.Generic;
using System.Text;

namespace WikiFunctions
{
    public struct Article
    {
        public Article(string Name)
        {
            strName = Name;
            intNameSpaceKey = Tools.CalculateNS(Name);
        }

        public Article(string Name, int NameSpaceKey)
        {
            strName = Name;
            intNameSpaceKey = NameSpaceKey;
        }

        public int intNameSpaceKey;
        public string strName;

        public string Name
        {
            get { return strName; }
            set { strName = value; }
        }

        public string URLEncodedName
        {
            get { return System.Web.HttpUtility.UrlEncode(Name); }
        }

        public int NameSpaceKey
        {
            get { return intNameSpaceKey; }
        }       

        public override string ToString()
        {
            return Name;
        }
    }
}
