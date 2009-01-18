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
using System.Collections.Specialized;
using System.Threading;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Web;

namespace WikiFunctions.Background
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="req"></param>
    public delegate void BackgroundRequestComplete(BackgroundRequest req);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="req"></param>
    public delegate void BackgroundRequestErrored(BackgroundRequest req);

    /// <summary>
    /// 
    /// </summary>
    public delegate void ExecuteFunctionDelegate();

    /// <summary>
    /// 
    /// </summary>
    public class BackgroundRequest
    {
        /// <summary>
        /// 
        /// </summary>
        public object Result;

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        public bool HasUI = true;

        /// <summary>
        /// 
        /// </summary>
        public Exception ErrorException { get; private set; }

        PleaseWait ui;

        Thread BgThread;

        /// <summary>
        /// 
        /// </summary>
        public event BackgroundRequestComplete Complete;

        /// <summary>
        /// 
        /// </summary>
        public event BackgroundRequestErrored Errored;

        /// <summary>
        /// 
        /// </summary>
        public BackgroundRequest()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handler"></param>
        public BackgroundRequest(BackgroundRequestComplete handler)
        {
            Complete += handler;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="completeHandler"></param>
        /// <param name="errorHandler"></param>
        public BackgroundRequest(BackgroundRequestComplete completeHandler, BackgroundRequestErrored errorHandler)
            : this (completeHandler)
        {
            Errored += errorHandler;
        }

        /// <summary>
        /// Waits for request to complete
        /// </summary>
        public void Wait()
        {
            while (!Done) Application.DoEvents();
        }

        /// <summary>
        /// aborts processing and terminates the thread
        /// </summary>
        public void Abort()
        {
            if (ui != null) ui.Close();
            ui = null;

            if (BgThread != null) BgThread.Abort();
            Wait();
            Result = null;
        }

        protected string strParam;
        protected object objParam1, objParam2, objParam3;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        private void InitThread(ThreadStart start)
        {
            BgThread = new Thread(start);
            BgThread.IsBackground = true;
            BgThread.Name = string.Format("BackgroundRequest (strParam = {0}, objParam1 = {1}, objParam2 = {2}, objParam3 = {3})",
                strParam, objParam1, objParam2, objParam3);
            BgThread.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        private void InvokeOnComplete()
        {
            if (Complete != null) Complete(this);
        }

        /// <summary>
        /// 
        /// </summary>
        private void InvokeOnError()
        {
            if (Errored != null) Errored(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        public void GetHTML(string url)
        {
            strParam = url;

            InitThread(GetHTMLFunc);
        }

        /// <summary>
        /// 
        /// </summary>
        private void GetHTMLFunc()
        {
            try
            {
                Result = Tools.GetHTML(strParam);
                InvokeOnComplete();
            }
            catch (Exception e)
            {
                ErrorException = e;
                InvokeOnError();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postvars"></param>
        public void PostData(string url, NameValueCollection postvars)
        {
            strParam = url;
            objParam1 = postvars;

            InitThread(PostDataFunc);
        }

        /// <summary>
        /// 
        /// </summary>
        private void PostDataFunc()
        {
            try
            {
                Result = Tools.PostData((NameValueCollection)objParam1, strParam);
                InvokeOnComplete();
            }
            catch (Exception e)
            {
                ErrorException = e;
                InvokeOnError();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        public void Execute(ExecuteFunctionDelegate d)
        {
            BgThread = new Thread(ExecuteFunc);
            BgThread.Name = "BackgroundThread";
            BgThread.IsBackground = true;
            BgThread.Start(d);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        private void ExecuteFunc(object d)
        {
            try
            {
                ((ExecuteFunctionDelegate)d)();
                InvokeOnComplete();
            }
            catch (Exception e)
            {
                ErrorException = e;
                InvokeOnError();
            }
        }

        /// <summary>
        /// Bypasses all redirects in the article
        /// </summary>
        public void BypassRedirects(string Article)
        {
            Result = strParam = Article;

            if (HasUI) ui = new PleaseWait();

            if (HasUI) ui.Show(Variables.MainForm as Form);
            InitThread(BypassRedirectsFunc);
        }

        /// <summary>
        /// 
        /// </summary>
        private void BypassRedirectsFunc()
        {//checks links to make them bypass redirects and (TODO) disambigs
            Regex wikiLinksOnly = new Regex("\\[\\[([^:|]*?)\\]\\]", RegexOptions.Compiled);

            Dictionary<string, string> knownLinks = new Dictionary<string, string>();

            if (HasUI) ui.Worker = Thread.CurrentThread;

            try
            {
                if (HasUI) ui.Status = "Loading links";

                MatchCollection simple = wikiLinksOnly.Matches(strParam);
                MatchCollection piped = WikiRegexes.PipedWikiLink.Matches(strParam);

                if (HasUI)
                {
                    ui.Status = "Processing links";

                    ui.SetProgress(0, simple.Count + piped.Count);
                }
                int n = 0;
                string link, article;
                foreach (Match m in simple)
                {
                    //make link
                    link = m.Value;
                    article = m.Groups[1].Value;

                    if (!knownLinks.ContainsKey(Tools.TurnFirstToUpper(article)))
                    {
                        //get text
                        string text;
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
                        knownLinks.Add(Tools.TurnFirstToUpper(article), Tools.TurnFirstToUpper(dest));
                    }
                    else if (knownLinks[Tools.TurnFirstToUpper(article)] != Tools.TurnFirstToUpper(article))
                    {
                        string directLink = "[[" + knownLinks[Tools.TurnFirstToUpper(article)] + "|" + article + "]]";

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

                    if (!knownLinks.ContainsKey(Tools.TurnFirstToUpper(article)))
                    {
                        //get text
                        string text;
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
                        knownLinks.Add(Tools.TurnFirstToUpper(article), Tools.TurnFirstToUpper(dest));
                    }
                    else if (knownLinks[Tools.TurnFirstToUpper(article)] != Tools.TurnFirstToUpper(article))
                    {
                        string directLink = "[[" + knownLinks[Tools.TurnFirstToUpper(article)] + "|" + linkText + "]]";

                        strParam = strParam.Replace(link, directLink);
                    }
                    n++;
                    if (HasUI) ui.SetProgress(n, simple.Count + piped.Count);
                }


                Result = strParam;
                InvokeOnComplete();
                //ui.Close();
            }
            catch (Exception e)
            {
                //ui.Close();
                ErrorException = e;
                InvokeOnError();
            }
        }

        /// <summary>
        /// Returns a list of articles using GetLists.FromVariant
        /// </summary>
        /// <param name="what">Which source to use</param>
        /// <param name="params1">Optional parameters, depend on source</param>
        public void GetList(Lists.IListProvider what, params string[] params1)
        {
            objParam1 = what;
            objParam2 = params1;

            if (HasUI) ui = new PleaseWait();
            if (HasUI) ui.Show(Variables.MainForm as Form);
            InitThread(GetListFunc);
        }

        /// <summary>
        /// 
        /// </summary>
        private void GetListFunc()
        {
            if (HasUI)
            {
                ui.Worker = Thread.CurrentThread;

                ui.Status = "Getting list of pages";
            }

            try
            {
                Result = ((Lists.IListProvider)objParam1).MakeList((string[])objParam2);
                InvokeOnComplete();
            }
            catch (Exception e)
            {
                ErrorException = e;
                InvokeOnError();
            }
        }
    }

    /// <summary>
    /// Thread-safe Queue-style container. Supports multiple writers and single reader.
    /// </summary>
    /// <typeparam name="T">Type to store</typeparam>
    public class CrossThreadQueue<T>
    {
        private readonly Queue<T> queue = new Queue<T>();

        public void Add(T value)
        {
            lock (queue)
            {
                queue.Enqueue(value);
            }
        }

        public T Remove()
        {
            lock (queue)
            {
                return queue.Dequeue();
            }
        }

        public int Count
        {
            get
            {
                lock (queue)
                {
                    return queue.Count;
                }
            }
        }
    }
}