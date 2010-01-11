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
    public abstract class ApiListProviderBase : IListProvider
    {
        #region Internals
        int m_Limit = 25000;

        #endregion

        /// <summary>
        /// Gets the list of XML elements that represent pages,
        /// e.g. <p>, <cm>, <bl> etc
        /// </summary>
        protected abstract ICollection<string> PageElements { get; }

        protected abstract ICollection<string> Actions { get; }

        /// <summary>
        /// Upper limit for number of pages returned, could be a bit exceeded by number of pages in the last request
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
        {// TODO: error handling
            List<Article> list = new List<Article>();
            string postfix = "";

            string newUrl = url;
            if (Hack1_12) newUrl = RemoveCmcategory.Replace(newUrl, "");

            while (list.Count + haveSoFar < Limit)
            {
                string text = Tools.GetHTML(newUrl + postfix);
                if (text.Contains("code=\"cmtitleandcategory\""))
                {
                    if (Hack1_12) throw new ListProviderException("cmtitleandcategory persists.");

                    Hack1_12 = true;
                    newUrl = RemoveCmcategory.Replace(url, "");
                    text = Tools.GetHTML(newUrl + postfix);
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
                    else if (PageElements.Contains(xml.Name) && xml.IsStartElement())
                    {
                        int ns = -1;
                        int.TryParse(xml.GetAttribute("ns"), out ns);
                        string name = xml.GetAttribute("title");

                        if (string.IsNullOrEmpty(name))
                        {
                            System.Windows.Forms.MessageBox.Show(xml.ReadInnerXml());
                            break;
                        }

                        // HACK: commented out until we make AWB always load namespaces from the wiki,
                        // to avoid problems with unknown namespace
                        //if (ns >= 0) list.Add(new Article(name, ns));
                        //else
                            list.Add(new Article(name));

                    }
                }
                if (string.IsNullOrEmpty(postfix)) break;
            }

            return list;
        }

        #region To be overridden

        public abstract List<Article> MakeList(params string[] searchCriteria);

        public abstract string DisplayText { get; }

        public abstract string UserInputTextBoxText { get; }

        public abstract bool UserInputTextBoxEnabled { get; }

        public abstract void Selected();

        public virtual bool RunOnSeparateThread
        { get { return true; } }

        #endregion
    }

    public abstract class CategoryProviderBase : ApiListProviderBase
    {
        #region Overrides: <categorymembers>/<cm>
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

        public override void Selected() { }

        public override bool UserInputTextBoxEnabled
        { get { return true; } }

        #endregion

        /// <summary>
        /// Gets the content of the given categor(y/ies)
        /// </summary>
        /// <param name="category">Category name. Must be WITHOUT the Category: prefix</param>
        /// <param name="haveSoFar">Number of pages already retrieved, for upper limit control</param>
        /// <returns>List of pages</returns>
        public List<Article> GetListing(string category, int haveSoFar)
        {
            string title = HttpUtility.UrlEncode(category);

            string url = Variables.URLLong + 
                "api.php?action=query&list=categorymembers&cmtitle=Category:" + title + "&cmcategory=" + title 
                + "&format=xml&cmlimit=max";

            return ApiMakeList(url, 0);
        }

        public List<Article> GetListing(string category)
        {
            return GetListing(category, 0);
        }

        protected List<string> Visited = new List<string>();

        public List<Article> RecurseCategory(string category, int haveSoFar, int depth)
        {
            if (haveSoFar > Limit || depth < 0) return new List<Article>();

            // normalise category name
            category = Tools.TurnFirstToUpper(Tools.WikiDecode(category));
            if (!Visited.Contains(category))
                Visited.Add(category);
            else
                return new List<Article>();

            List<Article> list = GetListing(category, haveSoFar);

            List<Article> fromSubcats = null;
            if (depth > 0 && haveSoFar + list.Count < Limit)
            {
                foreach (Article pg in list)
                {
                    if (haveSoFar + list.Count > Limit) break;

                    if (pg.NameSpaceKey == 14 /*Category*/ && !Visited.Contains(pg.Name))
                    {
                        if (fromSubcats == null) fromSubcats = new List<Article>();
                        fromSubcats.AddRange(RecurseCategory(pg.NamespacelessName, haveSoFar + list.Count, depth - 1));
                    }
                }
            }
            if (fromSubcats != null && fromSubcats.Count > 0) list.AddRange(fromSubcats);

            return list;
        }

        /// <summary>
        /// Normalises category names, removes Category: prefix
        /// </summary>
        /// <param name="source">List of category names</param>
        public static IEnumerable<string> PrepareCategories(IEnumerable<string> source)
        {
            List<string> cats = new List<string>();

            foreach (string cat in source)
            {
                cats.Add(Regex.Replace(Tools.RemoveHashFromPageTitle(Tools.WikiDecode(cat)).Trim(),
                    "^" + Variables.NamespacesCaseInsensitive[14], "").Trim());
                    
            }

            return cats;
        }

    }

    [Serializable]
    public class ListProviderException : Exception
    {
        public ListProviderException(string message)
            : base(message)
        { }
    }
}
