/*
WikiFunctions
Copyright (C) 2007 Max Semenik

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
using System.Threading;
using WikiFunctions;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Web;
using WikiFunctions.Lists;

namespace WikiFunctions.Background
{

    public class BackgroundRequest
    {
        public object Result;
        public bool Done
        {
            get
            {
                bool res=  (BgThread != null && (BgThread.ThreadState == ThreadState.Stopped ||
                    BgThread.ThreadState == ThreadState.Aborted));

                try
                {
                    if (res && ui != null) ui.Close();
                }
                catch
                {
                }
                return res;
            }
        }

        public Exception Error;
        PleaseWait ui;

        Thread BgThread;

        public BackgroundRequest()
        {
        }

        protected string strParam;
        protected object objParam;

        public void GetHTML(string url)
        {
            strParam = url;

            BgThread = new Thread(new ThreadStart(GetHTMLFunc));
            BgThread.IsBackground = true;
            BgThread.Start();
        }

        private void GetHTMLFunc()
        {
            try
            {
                Result = Tools.GetHTML(strParam);
            }
            catch (Exception e)
            {
                Error = e;
            }
        }

        public void Execute(Delegate d)
        {
            BgThread = new Thread(new ParameterizedThreadStart(ExecuteFunc));
            BgThread.IsBackground = true;
            BgThread.Start(d);
        }

        private void ExecuteFunc(object d)
        {
            try
            {
                Result = (d as Delegate).DynamicInvoke();
            }
            catch (Exception e)
            {
                Error = e;
            }
        }

        /// <summary>
        /// Bypasses all redirects in the article
        /// </summary>
        public void BypassRedirects(string Article)
        {
            Result = strParam = Article;

            ui = new PleaseWait();

            BgThread = new Thread(new ThreadStart(BypassRedirectsFunc));
            BgThread.IsBackground = true;
            ui.Show(Application.OpenForms[0]);
            BgThread.Start();
        }

        private void BypassRedirectsFunc()
        {//checks links to make them bypass redirects and (TODO) disambigs
            string link = "";
            string article = "";

            Regex WikiLinksOnly = new Regex("\\[\\[([^:|]*?)\\]\\]", RegexOptions.Compiled);

            Dictionary<string, string> KnownLinks = new Dictionary<string, string>();

            ui.Worker = Thread.CurrentThread;

            try
            {
                ui.Status = "Loading links";

                MatchCollection simple = WikiLinksOnly.Matches(strParam);
                MatchCollection piped = WikiRegexes.PipedWikiLink.Matches(strParam);

                ui.Status = "Processing links";

                ui.SetProgress(0, simple.Count + piped.Count);
                int n = 0;
                foreach (Match m in simple)
                {
                    //make link
                    link = m.Value;
                    article = m.Groups[1].Value;

                    if(!KnownLinks.ContainsKey(Tools.TurnFirstToUpper(article)))
                    {
                        //get text
                        string text = "";
                        try
                        {
                            text = Tools.GetArticleText(article);
                        }
                        catch
                        {
                            continue;
                        }

                        string dest = article;

                        //test if redirect
                        if (Tools.IsRedirect(text))
                        {
                            dest = HttpUtility.UrlDecode(Tools.RedirectTarget(text).Replace("_", " "));
                            string directLink = "[[" + dest + "|" + article + "]]";

                            strParam = strParam.Replace(link, directLink);
                        }
                        KnownLinks.Add(Tools.TurnFirstToUpper(article), Tools.TurnFirstToUpper(dest));
                    }
                    else if (KnownLinks[Tools.TurnFirstToUpper(article)] != Tools.TurnFirstToUpper(article))
                    {
                        string directLink = "[[" + KnownLinks[Tools.TurnFirstToUpper(article)] + "|" + article + "]]";

                        strParam = strParam.Replace(link, directLink);
                    }
                    n++;
                    ui.SetProgress(n, simple.Count + piped.Count);
                }

                foreach (Match m in piped)
                {
                    //make link
                    link = m.Value;
                    article = m.Groups[1].Value;
                    string linkText = m.Groups[2].Value;

                    if(!KnownLinks.ContainsKey(Tools.TurnFirstToUpper(article)))
                    {
                        //get text
                        string text = "";
                        try
                        {
                            text = Tools.GetArticleText(article);
                        }
                        catch
                        {
                            continue;
                        }

                        string dest = article;

                        //test if redirect
                        if (Tools.IsRedirect(text))
                        {
                            dest = HttpUtility.UrlDecode(Tools.RedirectTarget(text).Replace("_", " "));
                            string directLink = "[[" + dest + "|" + linkText + "]]";

                            strParam = strParam.Replace(link, directLink);
                        }
                        KnownLinks.Add(Tools.TurnFirstToUpper(article), Tools.TurnFirstToUpper(dest));
                    }
                    else if (KnownLinks[Tools.TurnFirstToUpper(article)] != Tools.TurnFirstToUpper(article))
                    {
                        string directLink = "[[" + KnownLinks[Tools.TurnFirstToUpper(article)] + "|" + linkText + "]]";

                        strParam = strParam.Replace(link, directLink);
                    }
                    n++;
                    ui.SetProgress(n, simple.Count + piped.Count);
                }

                
                Result = strParam;
                //ui.Close();
            }
            catch(Exception e)
            {
                //ui.Close();
                Error = e;
            }
        }


        /// <summary>
        /// returns list of pages from a list of categories
        /// </summary>
        /// <param name="categories"></param>
        public void GetFromCategories(string[] categories)
        {
            objParam = categories;

            ui = new PleaseWait();

            BgThread = new Thread(new ThreadStart(GetFromCategoriesFunc));
            BgThread.IsBackground = true;
            ui.Show(Application.OpenForms[0]);
            BgThread.Start();
        }

        private void GetFromCategoriesFunc()
        {
            List<Article> list = new List<Article>();
            ui.Worker = Thread.CurrentThread;

            ui.Status = "Getting category contents";
            try
            {
                int n=0;
                
                foreach (string s in (string[])objParam)
                {
                    ui.SetProgress(n, ((string[])objParam).Length);
                    list.AddRange(GetLists.FromCategory(false, new string[1] {s}));
                }

                Result = list;
            }
            catch (Exception e)
            {
                Error = e;
            }
        }
    }
}