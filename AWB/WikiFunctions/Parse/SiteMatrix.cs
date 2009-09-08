/*

Copyright (C) 2007 Martin Richards, Max Semenik et al.

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
using System.Xml;

namespace WikiFunctions.Parse
{
    public static class SiteMatrix
    {
        public static List<string> Languages = new List<string>();
        public static List<string> WikipediaLanguages = new List<string>();
        public static List<string> WiktionaryLanguages = new List<string>();
        public static List<string> WikibooksLanguages = new List<string>();
        public static List<string> WikinewsLanguages = new List<string>();
        public static List<string> WikisourceLanguages = new List<string>();
        public static List<string> WikiquoteLanguages = new List<string>();
        public static List<string> WikiversityLanguages = new List<string>();
        public static List<string> Specials = new List<string>();
        public static readonly Dictionary<string, string> LanguageNames = new Dictionary<string, string>();

        static SiteMatrix()
        {
            if (Globals.UnitTestMode) // or unit tests gonna run like a turtle
            {
                Languages.AddRange(new[] { "en", "ru", "sq" });
                WikipediaLanguages.AddRange(Languages);
                WiktionaryLanguages.AddRange(Languages);
                WikibooksLanguages.AddRange(Languages);
                WikinewsLanguages.AddRange(Languages);
                WikisourceLanguages.AddRange(Languages);
                WikiquoteLanguages.AddRange(Languages);
                WikiversityLanguages.AddRange(Languages);
                Specials.AddRange(Languages);
            }
            else
            {
                if (!LoadFromCache())
                {
                    LoadFromNetwork();
                    SaveToCache();
                }

                //should already be sorted, but we must be sure
                Languages.Sort();
                WikipediaLanguages.Sort();
                WiktionaryLanguages.Sort();
                WikibooksLanguages.Sort();
                WikinewsLanguages.Sort();
                WikisourceLanguages.Sort();
                WikiquoteLanguages.Sort();
                WikiversityLanguages.Sort();
                Specials.Sort();
            }
        }

        private static string Key(string what)
        {
            return "SiteMatrix::" + what;
        }

        private static bool Loaded = true;

        private static List<string> Load(string what)
        {
            var result = (List<string>)ObjectCache.Global.Get<List<string>>(Key(what));
            if (result == null)
            {
                Loaded = false;
                return new List<string>();
            }

            return result;
        }

        private static bool LoadFromCache()
        {
            Languages = Load("Languages");
            WikipediaLanguages = Load("WikipediaLanguages");
            WiktionaryLanguages = Load("WiktionaryLanguages");
            WikibooksLanguages = Load("WikibooksLanguages");
            WikinewsLanguages = Load("WikinewsLanguages");
            WikisourceLanguages = Load("WikisourceLanguages");
            WikiquoteLanguages = Load("WikiquoteLanguages");
            WikiversityLanguages = Load("WikiversityLanguages");
            Specials = Load("Specials");
            return Loaded;
        }

        private static void LoadFromNetwork()
        {
            string strMatrix = Tools.GetHTML("http://en.wikipedia.org/w/api.php?action=sitematrix&format=xml");

            XmlDocument matrix = new XmlDocument();
            matrix.LoadXml(strMatrix);

            foreach (XmlNode spec in matrix.GetElementsByTagName("special"))
            {
                Specials.Add(spec.Attributes["url"].Value.Replace("http://", ""));
            }
            
            foreach (XmlNode lang in matrix.GetElementsByTagName("language"))
            {
                string langCode = lang.Attributes["code"].Value;
                string langName = lang.Attributes["name"].Value;

                Languages.Add(langCode);
                LanguageNames[langCode] = langName;

                foreach (XmlNode site in lang.ChildNodes[0].ChildNodes)
                {
                    if (site.Name != "site") continue;

                    switch (site.Attributes["code"].Value)
                    {
                        case "wiki":
                            WikipediaLanguages.Add(langCode);
                            break;
                        case "wiktionary":
                            WiktionaryLanguages.Add(langCode);
                            break;
                        case "wikibooks":
                            WikibooksLanguages.Add(langCode);
                            break;
                        case "wikinews":
                            WikinewsLanguages.Add(langCode);
                            break;
                        case "wikisource":
                            WikisourceLanguages.Add(langCode);
                            break;
                        case "wikiquote":
                            WikiquoteLanguages.Add(langCode);
                            break;
                        case "wikiversity":
                            WikiversityLanguages.Add(langCode);
                            break;
                    }
                }
            }
        }

        private static void SaveToCache()
        {
            ObjectCache.Global.Set(Key("Languages"), Languages);
            ObjectCache.Global.Set(Key("WikipediaLanguages"), WikipediaLanguages);
            ObjectCache.Global.Set(Key("WiktionaryLanguages"), WiktionaryLanguages);
            ObjectCache.Global.Set(Key("WikibooksLanguages"), WikibooksLanguages);
            ObjectCache.Global.Set(Key("WikinewsLanguages"), WikinewsLanguages);
            ObjectCache.Global.Set(Key("WikisourceLanguages"), WikisourceLanguages);
            ObjectCache.Global.Set(Key("WikiquoteLanguages"), WikiquoteLanguages);
            ObjectCache.Global.Set(Key("WikiversityLanguages"), WikiversityLanguages);
            ObjectCache.Global.Set(Key("Specials"), Specials);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public static List<string> GetProjectLanguages(ProjectEnum project)
        {
            switch (project)
            {
                case ProjectEnum.wikipedia:
                case ProjectEnum.meta:
                case ProjectEnum.commons:
                case ProjectEnum.species:
                    return WikipediaLanguages;

                case ProjectEnum.wiktionary:
                    return WiktionaryLanguages;

                case ProjectEnum.wikisource:
                    return WikisourceLanguages;

                case ProjectEnum.wikibooks:
                    return WikibooksLanguages;

                case ProjectEnum.wikinews:
                    return WikinewsLanguages;

                case ProjectEnum.wikiquote:
                    return WikiquoteLanguages;

                case ProjectEnum.wikiversity:
                    return WikiversityLanguages;

                default:
                    return new List<string>();
            }
        }
    }

    internal sealed class InterWikiComparer : IComparer<string>
    {
        readonly Dictionary<string, int> Order = new Dictionary<string, int>();
        public InterWikiComparer(List<string> order, List<string> languages)
        {
            languages = new List<string>(languages); // make a copy
            List<string> unordered = new List<string>(),
                         output = new List<string>();

            // remove unneeded languages from order
            for (int i = 0; i < order.Count; )
            {
                if (languages.Contains(order[i])) i++;
                else order.RemoveAt(i);
            }

            foreach (string s in languages)
                if (!order.Contains(s)) unordered.Add(s);

            if (unordered.Count == 0) 
                output = order;
            else for (int i = 0; i < languages.Count; i++)
            {
                if (unordered.Contains(languages[i]))
                {
                    output.Add(languages[i]);
                    unordered.RemoveAt(0);
                }
                else
                {
                    if (order.Count > 0)
                    {
                        output.Add(order[0]);
                        order.RemoveAt(0);
                    }
                }
            }

            int n = 0;
            foreach (string s in output)
            {
                if (!Order.ContainsKey("[[" + s))
                {
                    Order.Add("[[" + s, n);
                    n++;
                }
            }
        }

        static string RawCode(string iw)
        {
            return iw.Substring(0, iw.IndexOf(':'));
        }

        public int Compare(string x, string y)
        {
            //should NOT be enclosed into try ... catch - I'd like to see exceptions if something goes wrong,
            //not quiet missorting --MaxSem
            int ix = Order[RawCode(x)], iy = Order[RawCode(y)];

            if (ix < iy) return -1;
            return (ix == iy) ? 0 : 1;
        }
    }
}