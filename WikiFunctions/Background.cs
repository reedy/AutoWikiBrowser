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
        public object Result = null;
        public bool Done
        {
            get
            {
                bool res = (BgThread != null && (BgThread.ThreadState == ThreadState.Stopped ||
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

        public bool HasUI = true;

        public Exception Error;
        PleaseWait ui;

        Thread BgThread;

        public BackgroundRequest()
        {
        }

        /// <summary>
        /// Waits for request to complete
        /// </summary>
        public void Wait()
        {
            while (!Done) Application.DoEvents();
        }

        protected string strParam;
        protected object objParam1;
        protected object objParam2;
        protected object objParam3;
        protected int intParam;

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

            if (HasUI) ui = new PleaseWait();

            BgThread = new Thread(new ThreadStart(BypassRedirectsFunc));
            BgThread.IsBackground = true;
            if (HasUI) ui.Show(Variables.MainForm as Form);
            BgThread.Start();
        }

        private void BypassRedirectsFunc()
        {//checks links to make them bypass redirects and (TODO) disambigs
            string link = "";
            string article = "";

            Regex WikiLinksOnly = new Regex("\\[\\[([^:|]*?)\\]\\]", RegexOptions.Compiled);

            Dictionary<string, string> KnownLinks = new Dictionary<string, string>();

            if (HasUI) ui.Worker = Thread.CurrentThread;

            try
            {
                if (HasUI) ui.Status = "Loading links";

                MatchCollection simple = WikiLinksOnly.Matches(strParam);
                MatchCollection piped = WikiRegexes.PipedWikiLink.Matches(strParam);

                if (HasUI)
                {
                    ui.Status = "Processing links";

                    ui.SetProgress(0, simple.Count + piped.Count);
                }
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
                    if (HasUI) ui.SetProgress(n, simple.Count + piped.Count);
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
                    if (HasUI) ui.SetProgress(n, simple.Count + piped.Count);
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
        /// Returns a list of articles using GetLists.FromVariant
        /// </summary>
        /// <param name="What">Which source to use</param>
        /// <param name="Limit">Max. number of pages to return, -1 if no limit</param>
        /// <param name="Params">Optional parameters, depend on source</param>
        public void GetList(GetLists.From What, int Limit, params string[] Params)
        {
            objParam1 = What;
            objParam2 = Params;
            intParam = Limit;

            if (HasUI) ui = new PleaseWait();
            BgThread = new Thread(new ThreadStart(GetListFunc));

            BgThread.IsBackground = true;
            if (HasUI) ui.Show(Variables.MainForm as Form);
            BgThread.Start();
        }

        /// <summary>
        /// Returns a list of articles using GetLists.FromVariant
        /// </summary>
        /// <param name="What">Which source to use</param>
        /// <param name="Params">Optional parameters, depend on source</param>
        public void GetList(GetLists.From What, params string[] Params)
        {
            GetList(What, -1, Params);
        }

        private void GetListFunc()
        {
            if (HasUI)
            {
                ui.Worker = Thread.CurrentThread;

                ui.Status = "Getting list of pages";
            }

            try
            {
                Result = GetLists.FromVariant((GetLists.From)objParam1, intParam, (string[])objParam2);
            }
            catch (Exception e)
            {
                Error = e;
            }
        }
    }
}