using System;
using System.Threading;
using WikiFunctions;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Web;

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
    }
}