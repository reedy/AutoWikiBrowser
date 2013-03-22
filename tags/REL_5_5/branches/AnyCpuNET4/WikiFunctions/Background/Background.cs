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
using WikiFunctions.API;
using WikiFunctions.Lists.Providers;

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
                    if (res && UI != null) UI.Close();
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

        PleaseWait UI;

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
            if (UI != null) UI.Close();
            UI = null;

            if (BgThread != null) BgThread.Abort();
            Wait();
            Result = null;
        }

        protected string StrParam;
        protected object ObjParam1, ObjParam2, ObjParam3;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        private void InitThread(ThreadStart start)
        {
            BgThread = new Thread(start)
                           {
                               IsBackground = true,
                               Name =
                                   string.Format(
                                   "BackgroundRequest (StrParam = {0}, ObjParam1 = {1}, ObjParam2 = {2}, ObjParam3 = {3})",
                                   StrParam, ObjParam1, ObjParam2, ObjParam3)
                           };
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
            StrParam = url;

            InitThread(GetHTMLFunc);
        }

        /// <summary>
        /// 
        /// </summary>
        private void GetHTMLFunc()
        {
            try
            {
                Result = Tools.GetHTML(StrParam);
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
            StrParam = url;
            ObjParam1 = postvars;

            InitThread(PostDataFunc);
        }

        /// <summary>
        /// 
        /// </summary>
        private void PostDataFunc()
        {
            try
            {
                Result = Tools.PostData((NameValueCollection)ObjParam1, StrParam);
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
            BgThread = new Thread(ExecuteFunc) {Name = "BackgroundThread", IsBackground = true};
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
        public void BypassRedirects(string article, IApiEdit editor)
        {
            Result = StrParam = article;
            ObjParam1 = editor;

            if (HasUI)
            {
                UI = new PleaseWait();
                UI.Show(Variables.MainForm as Form);
            }

            InitThread(BypassRedirectsFunc);
        }

        /// <summary>
        /// checks wikilinks to make them bypass redirects 
        /// </summary>
        private void BypassRedirectsFunc()
        {//checks links to make them bypass redirects and (TODO) disambigs
            Dictionary<string, string> knownLinks = new Dictionary<string, string>();

            if (HasUI) UI.Worker = Thread.CurrentThread;

            IApiEdit editor = ObjParam1 as IApiEdit;

            if (editor == null)
            {
                Result = "";
                InvokeOnError();
                return;
            }

            try
            {
                if (HasUI) UI.Status = "Loading links";

                MatchCollection links = WikiRegexes.WikiLinksOnlyPossiblePipe.Matches(StrParam);

                if (HasUI)
                {
                    UI.Status = "Processing links";

                    UI.SetProgress(0, links.Count);
                }
                int n = 0;

                foreach (Match m in links)
                {
                    string link = m.Value;
                    string article = m.Groups[1].Value.TrimStart(new[] {':'});

                    // if the link is unpiped, use the target as the new link's pipe text
                    string linkText = (!string.IsNullOrEmpty(m.Groups[2].Value)) ? m.Groups[2].Value : article;

                    string ftu = Tools.TurnFirstToUpper(article);

                    string value;
                    if (!knownLinks.TryGetValue(ftu, out value))
                    {
                        //get text
                        string text;
                        try
                        {
                            text = editor.Open(article, false); //TODO:Resolve redirects betterer
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

                            StrParam = StrParam.Replace(link, directLink);
                        }
                        knownLinks.Add(ftu, Tools.TurnFirstToUpper(dest));
                    }
                    else if (value != ftu)
                    {
                        string directLink = "[[" + value + "|" + linkText + "]]";

                        StrParam = StrParam.Replace(link, directLink);
                    }
                    n++;
                    if (HasUI) UI.SetProgress(n, links.Count);
                }

                Result = StrParam;
                InvokeOnComplete();
                //UI.Close();
            }
            catch (Exception e)
            {
                //UI.Close();
                ErrorException = e;
                InvokeOnError();
            }
        }

        /// <summary>
        /// Returns a list of articles using GetLists.FromVariant
        /// </summary>
        /// <param name="what">Which source to use</param>
        /// <param name="params1">Optional parameters, depend on source</param>
        public void GetList(IListProvider what, params string[] params1)
        {
            ObjParam1 = what;
            ObjParam2 = params1;

            if (HasUI) UI = new PleaseWait();
            if (HasUI) UI.Show(Variables.MainForm as Form);
            InitThread(GetListFunc);
        }

        /// <summary>
        /// 
        /// </summary>
        private void GetListFunc()
        {
            if (HasUI)
            {
                UI.Worker = Thread.CurrentThread;

                UI.Status = "Getting list of pages";
            }

            try
            {
                Result = ((IListProvider)ObjParam1).MakeList((string[])ObjParam2);
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
        private readonly Queue<T> Queue = new Queue<T>();

        public void Add(T value)
        {
            lock (Queue)
            {
                Queue.Enqueue(value);
            }
        }

        public T Remove()
        {
            lock (Queue)
            {
                return Queue.Dequeue();
            }
        }

        public int Count
        {
            get
            {
                lock (Queue)
                {
                    return Queue.Count;
                }
            }
        }
    }
}