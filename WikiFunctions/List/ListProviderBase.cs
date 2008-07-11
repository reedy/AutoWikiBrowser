using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.IO;

namespace WikiFunctions.Lists
{
    /// <summary>
    /// Parent abstract class for all API-based providers
    /// currently simultaneous call of more than one API generator is not fully supported
    /// </summary>
    public abstract class ApiListMakerProvider : IListProvider
    {
        #region Internals
        int m_Limit = 5000;
        #endregion

        /// <summary>
        /// Gets the list of XML elements that represent pages,
        /// e.g. <p>, <cm>, <bl> etc
        /// </summary>
        protected abstract ICollection<string> PageElements { get; }

        protected abstract ICollection<string> Actions { get; }

        /// <summary>
        /// Upper limit for number of pages returned, could be a bit exceeded by number of pages the last request
        /// </summary>
        public int Limit
        {
            get { return m_Limit; }
            set { m_Limit = value; }
        }

        #region Dirty hack for 1.12's inability to accept cmtitle and cmcategory at the same time
        string m_LastURL;

        /// <summary>
        /// Don't friggin access directly!
        /// </summary>
        bool m_1dot12hack;

        protected bool Hack1_12
        {
            get
            {
                if (Variables.URL != m_LastURL)
                {
                    m_LastURL = Variables.URL;
                    m_1dot12hack = false;
                }

                return m_1dot12hack;
            }
            set
            {
                m_1dot12hack = value;
                m_LastURL = Variables.URL;
            }
        }

        static readonly Regex RemoveCmcategory = new Regex("&cmcategory=[^&]*", RegexOptions.Compiled);

        #endregion

        /// <summary>
        /// Main function that retrieves the list from API, including paging
        /// </summary>
        /// <param name="url">URL of API request</param>
        /// <param name="haveSoFar">Number of pages already retrieved, for upper limit control</param>
        /// <returns>List of pages</returns>
        public List<Article> ApiMakeList(string url, int haveSoFar)
        {
            List<Article> lst = new List<Article>();
            string postfix = "";

            if (Hack1_12) url = RemoveCmcategory.Replace(url, "");

            while (lst.Count + haveSoFar < Limit)
            {
                string text = Tools.GetHTML(url + postfix);
                if (text.Contains("code=\"cmtitleandcategory\""))
                {
                    if (Hack1_12) throw new ListProviderException("cmtitleandcategory persists.");

                    Hack1_12 = true;
                    url = RemoveCmcategory.Replace(url, "");
                    text = Tools.GetHTML(url + postfix);
                }

                XmlTextReader xml = new XmlTextReader(new StringReader(text));
                xml.MoveToContent();
                postfix = "";

                while (xml.Read())
                {
                    if (xml.Name == "query-continue")
                    {
                        XmlReader r = xml.ReadSubtree();

                        r.Read();

                        while (r.Read())
                        {
                            if (!r.IsStartElement()) continue;
                            if (!r.MoveToFirstAttribute()) 
                                throw new FormatException("Malformed element '" + r.Name + "' in <query-continue>");
                            postfix += "&" + r.Name + "=" + HttpUtility.UrlEncode(r.Value);
                        }
                    }
                    else if (PageElements.Contains(xml.Name))
                    {
                        int ns = -1;
                        int.TryParse(xml.GetAttribute("ns"), out ns);
                        string name = xml.GetAttribute("title");
                        if (!string.IsNullOrEmpty(name))
                        {
                            if (ns >= 0) lst.Add(new Article(name, ns));
                            else 
                                lst.Add(new Article(name));
                        }
                    }
                }
                if (string.IsNullOrEmpty(postfix)) break;
            }

            return lst;
        }

        #region To be overridden

        public abstract List<Article> MakeList(params string[] searchCriteria);

        public abstract string DisplayText { get; }

        public abstract string UserInputTextBoxText { get; }

        public abstract bool UserInputTextBoxEnabled { get; }

        public abstract void Selected();

        public abstract bool RunOnSeparateThread { get; }

        #endregion
    }

    public abstract class CategoryProviderBase : ApiListMakerProvider
    {
        #region Overrides
        List<string> pe = new List<string>(new string[] { "cm" });
        protected override ICollection<string> PageElements
        {
            get { return pe; }
        }

        List<string> ac = new List<string>(new string[] { "categorymembers" });
        protected override ICollection<string> Actions
        {
            get { return ac; }
        }

        public override string UserInputTextBoxText
        {
            get { return Variables.Namespaces[14]; }
        }

        public override bool RunOnSeparateThread
        {
            get { return true; }
        }

        #endregion

        public List<Article> GetListing(string category)
        {
            //List<Article> lst = new List<Article>();

            string title = HttpUtility.UrlEncode(category);

            string url = Variables.URLLong + 
                "api.php?action=query&list=categorymembers&cmtitle=Category:" + title + "&cmcategory=" + title + "&format=xml&cmlimit=500";

            return ApiMakeList(url, 0);
        }

        public List<Article> RecurseCategory(string category, int levels)
        {
            List<Article> lst = new List<Article>();

            return lst;
        }
    }

    public class TestCategoryProvider : CategoryProviderBase
    {

        public override List<Article> MakeList(params string[] searchCriteria)
        {
            List<Article> lst = new List<Article>();
            foreach (string cat in searchCriteria)
            {
                lst.AddRange(GetListing(cat));
            }

            return lst;
        }

        public override string DisplayText
        {
            get { return "[Test]Category"; }
        }

        public override bool UserInputTextBoxEnabled
        {
            get { return true; }
        }

        public override void Selected()
        { }
    }

    public class ListProviderException : Exception
    {
        public ListProviderException(string message)
            : base(message)
        {
        }
    }
}
