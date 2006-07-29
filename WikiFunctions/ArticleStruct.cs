using System;
using System.Collections.Generic;
using System.Text;

namespace WikiFunctions
{
    public struct Article
    {
        public Article(string Name)
        {
            this.Name = Name;
            this.NameSpaceKey = Tools.CalculateNS(Name);
        }

        public Article(string Name, int NameSpaceKey)
        {
            this.Name = Name;
            this.NameSpaceKey = NameSpaceKey;
        }

        public string URLEncodedName
        {
            get { return System.Web.HttpUtility.UrlEncode(Name); }
        }

        public int NameSpaceKey;
        public string Name;

        public override string ToString()
        {
            return Name;
        }
    }
}
