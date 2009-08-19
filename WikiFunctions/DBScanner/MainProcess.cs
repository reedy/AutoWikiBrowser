/*
DumpSearcher
Copyright (C) 2007 Martin Richards

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
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Threading;
using WikiFunctions.Background;

namespace WikiFunctions.DBScanner
{
    public delegate void StopDel();

    public class ArticleInfo
    {
        public string Title, Text, Timestamp, Restrictions;

        public bool IsFullyRead
        {
            get
            {
                return !string.IsNullOrEmpty(Title)
                && !string.IsNullOrEmpty(Timestamp)
                && Text != null;
            }
        }
    }

    class MainProcess
    {
        public event StopDel StoppedEvent;
        public CrossThreadQueue<string> OutputQueue;

        private readonly string FileName = "";
        private readonly string From = "";
        private Stream stream;

        private readonly SendOrPostCallback SOPCstopped;
        private SynchronizationContext Context;

        private Thread ScanThread;
        readonly List<Thread> SecondaryThreads = new List<Thread>();
        private readonly bool MultiThreaded;
        private readonly int ProcessorCount;
        private readonly CrossThreadQueue<ArticleInfo> PendingArticles = new CrossThreadQueue<ArticleInfo>();

        private readonly List<Scan> Scanners;
        private readonly bool IgnoreComments;

        public MainProcess(List<Scan> z, string filename, ThreadPriority tp, bool ignoreComments, string startFrom)
            : this(z, filename, tp, ignoreComments)
        {
            From = startFrom;
        }

        public MainProcess(List<Scan> z, string filename, ThreadPriority tp, bool ignoreComments)
        {
            ProcessorCount = Environment.ProcessorCount; // caching
            FileName = filename;
            SOPCstopped = Stopped;
            Priority = tp;
            IgnoreComments = ignoreComments;
            MultiThreaded = ProcessorCount > 1;

            Scanners = z;

            try
            {
                stream = new FileStream(FileName, FileMode.Open);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Gets the percentage of scan complete, ranging from 0 to 1
        /// </summary>
        public double PercentageComplete
        {
            get
            {
                try
                {
                    lock (ScanThread)
                    {
                        if (stream == null)
                            return 1; // scan complete

                        if (stream.Length == 0)
                            return 0;

                        return (double)stream.Position / stream.Length;
                    }
                }
                // ObjectDisposedException is still possible if we exited the main loop in Process() due to exception
                catch (ObjectDisposedException) { } // ignore

                return 1; // scan ended, probably in fire
            }
        }

        private void Stopped(object o)
        {
            StoppedEvent();
        }

        public void Stop()
        {
            Run = false;
            if (ScanThread == null) return;

            ScanThread.Abort();
            foreach (Thread thr in SecondaryThreads)
            {
                thr.Abort();
            }
            ScanThread.Join();

            foreach (Thread thr in SecondaryThreads)
            {
                // avoid deadlocks when calling from secondary thread
                if (thr.ManagedThreadId != Thread.CurrentThread.ManagedThreadId)
                    thr.Join();
            }
        }

        public void Start()
        {
            Context = SynchronizationContext.Current;

            ScanThread = new Thread(Process)
                             {
                                 Name = "DB Scanner thread", 
                                 IsBackground = true, 
                                 Priority = mPriority
                             };
            ScanThread.Start();

            for (int i = 0; i < ProcessorCount - 1; i++)
            {
                Thread thr = new Thread(SecondaryThread)
                                 {
                                     Name = "DB Scanner thread #" + (i + 2),
                                     IsBackground = true,
                                     Priority = mPriority
                                 };
                SecondaryThreads.Add(thr);
                thr.Start();
            }
        }

        private void ScanArticle(ArticleInfo ai)
        {
            if (IgnoreComments)
                ai.Text = WikiRegexes.Comments.Replace(ai.Text, "");

            foreach (Scan z in Scanners)
            {
                if (!z.Check(ai))
                {
                    return;
                }
            }

            OutputQueue.Add(ai.Title);
        }

        private void Process()
        {
            string articleTitle = "";

            try
            {
                using (XmlTextReader reader = new XmlTextReader(stream))
                {
                    reader.WhitespaceHandling = WhitespaceHandling.None;

                    if (From.Length > 0)
                    {
                        //move to start from article
                        while (Run && reader.Read())
                        {
                            if (reader.NodeType != XmlNodeType.Element)
                                continue;

                            if (reader.Name != "title")
                            {
                                reader.ReadToFollowing("page");
                                continue;
                            }

                            //reader.ReadToFollowing("title");
                            articleTitle = reader.ReadString();

                            if (From == articleTitle)
                                break;
                        }
                    }

                    while (Run)
                    {
                        ArticleInfo ai = ReadArticle(reader);
                        if (ai == null) break;
                        articleTitle = ai.Title;

                        // we must maintain a huge enough buffer to safeguard against fluctuations
                        // of page size
                        if (MultiThreaded && (PendingArticles.Count < ProcessorCount * 10))
                            PendingArticles.Add(ai);
                        else
                            ScanArticle(ai);
                    }

                    lock (ScanThread)
                    {
                        stream = null;
                    }

                    if (MultiThreaded)
                    {
                        while (PendingArticles.Count > 0)
                            Thread.Sleep(10);

                        Run = false;

                        foreach (Thread thr in SecondaryThreads)
                            thr.Join();
                    }
                }
            }
            catch (ThreadAbortException) { }
            catch (Exception ex)
            {
                if (Message)
                    //System.Windows.Forms.MessageBox.Show("Problem on " + articleTitle + "\r\n\r\n" + ex.Message);
                    ErrorHandler.Handle(ex);
            }
            finally
            {
                if (Message)
                    Context.Post(SOPCstopped, articleTitle);
            }
        }

        /// <summary>
        /// Reads a page from the reader, returns ArticleInfo or null if EOF
        /// </summary>
        private ArticleInfo ReadArticle(XmlReader reader)
        {
            do 
                if (!reader.ReadToFollowing("page")) return null;
            while (!reader.IsStartElement());

            ArticleInfo ai = new ArticleInfo();
            while (reader.Read() && reader.Name != "page") // stop on closing element
            {
                if (!reader.IsStartElement()) continue;
                switch (reader.Name)
                {
                    case "title":
                        ai.Title = reader.ReadString();
                        break;
                    case "timestamp":
                        ai.Timestamp = reader.ReadString();
                        break;
                    case "restrictions":
                        ai.Restrictions = reader.ReadString();
                        break;
                    case "text":
                        ai.Text = reader.ReadString();
                        break;
                }
            }

            return ai.IsFullyRead ? ai : null;
        }

        private void SecondaryThread()
        {
            try
            {
                bool sleep;
                //int sleeps = 0;
                while (Run)
                {
                    if (PendingArticles.Count > 0) lock (PendingArticles)
                        {
                            if (PendingArticles.Count > 0)
                            {
                                ArticleInfo ai = PendingArticles.Remove();
                                ScanArticle(ai);
                                sleep = false;
                            }
                            else
                                sleep = true;
                        }
                    else
                        sleep = true;

                    if (sleep)
                    {
                        Thread.Sleep(1);
                        //sleeps++;
                    }
                }
                //System.Windows.Forms.MessageBox.Show(sleeps.ToString());
            }
            catch (ThreadAbortException)
            { }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
            }
        }

        #region Properties

        public bool Run = true;
        public bool Message = true;

        ThreadPriority mPriority = ThreadPriority.BelowNormal;
        public ThreadPriority Priority
        {
            get { return mPriority; }
            set
            {
                mPriority = value;
                if (ScanThread != null)
                    ScanThread.Priority = value;

                foreach (Thread thr in SecondaryThreads)
                {
                    thr.Priority = value;
                }
            }
        }
        #endregion
    }
}
