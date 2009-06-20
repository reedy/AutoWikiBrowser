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

    internal class ArticleInfo
    {
        public string Title;
        public string Text;
        public string Timestamp;
        public string Restrictions;
    }

    class MainProcess
    {
        public event StopDel StoppedEvent;
        public CrossThreadQueue<string> OutputQueue;

        string FileName = "";
        string From = "";
        Stream stream;

        SendOrPostCallback SOPCstopped;
        private SynchronizationContext context;

        Thread ScanThread;
        List<Thread> SecondaryThreads = new List<Thread>();
        public bool MultiThreaded = false;
        int ProcessorCount;
        CrossThreadQueue<ArticleInfo> PendingArticles = new CrossThreadQueue<ArticleInfo>();

        List<Scan> Scanners;
        bool IgnoreComments = false;

        public MainProcess(List<Scan> z, string filename, ThreadPriority tp, bool ignoreComments, string StartFrom)
            : this(z, filename, tp, ignoreComments)
        {
            From = StartFrom;
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
            if (ScanThread != null)
            {
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
        }

        public void Start()
        {
            context = SynchronizationContext.Current;

            ScanThread = new Thread(Process);
            ScanThread.Name = "DB Scanner thread";
            ScanThread.IsBackground = true;
            ScanThread.Priority = mPriority;
            ScanThread.Start();

            for (int i = 0; i < ProcessorCount - 1; i++)
            {
                Thread thr = new Thread(SecondaryThread);
                thr.Name = "DB Scanner thread #" + (i + 2);
                thr.IsBackground = true;
                thr.Priority = mPriority;
                SecondaryThreads.Add(thr);
                thr.Start();
            }
        }

        private void ScanArticle(ArticleInfo ai)
        {
            foreach (Scan z in Scanners)
            {
                if (!z.Check(ref ai.Text, ref ai.Title, ai.Timestamp, ai.Restrictions))
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
                        while (reader.Read() && mRun)
                        {
                            if (reader.Name == "page")
                            {
                                reader.ReadToFollowing("title");
                                articleTitle = reader.ReadString();

                                if (From == articleTitle)
                                    break;
                            }
                        }
                    }

                    while (reader.Read() && mRun)
                    {
                        if (reader.Name == "page")
                        {
                            ArticleInfo ai = new ArticleInfo();

                            reader.ReadToFollowing("title");
                            ai.Title = articleTitle = reader.ReadString();

                            //reader.ReadToFollowing(restriction); //TODO:This is wrong. Only want to read the restriction if in that <page></page>

                            if (reader.Name == "restrictions")
                                ai.Restrictions = reader.ReadString();
                            else
                                ai.Restrictions = "";

                            reader.ReadToFollowing("timestamp");
                            ai.Timestamp = reader.ReadString();
                            reader.ReadToFollowing("text");
                            ai.Text = reader.ReadString();

                            if (IgnoreComments)
                                ai.Text = WikiRegexes.Comments.Replace(ai.Text, "");

                            if (MultiThreaded)
                            {
                                if (PendingArticles.Count < ProcessorCount * 4 + 5)
                                {
                                    PendingArticles.Add(ai);
                                }
                                else
                                {
                                    ScanArticle(ai);
                                }
                            }
                            else
                            {
                                ScanArticle(ai);
                            }
                        }
                    }

                    lock (ScanThread)
                    {
                        stream = null;
                    }

                    if (MultiThreaded)
                    {
                        while (PendingArticles.Count > 0)
                            Thread.Sleep(10);

                        mRun = false;

                        foreach (Thread thr in SecondaryThreads)
                            thr.Join();
                    }
                }
            }
            catch (ThreadAbortException) { }
            catch (Exception ex)
            {
                if (boolMessage)
                    //System.Windows.Forms.MessageBox.Show("Problem on " + articleTitle + "\r\n\r\n" + ex.Message);
                    ErrorHandler.Handle(ex);
            }
            finally
            {
                if (boolMessage)
                    context.Post(SOPCstopped, articleTitle);
            }
        }

        private void SecondaryThread()
        {
            try
            {
                bool sleep;
                int sleeps = 0;
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
                        sleeps++;
                    }
                }
                //System.Windows.Forms.MessageBox.Show(sleeps.ToString());
            }
            catch (ThreadAbortException)
            { }
            catch (Exception ex)
            {
                //TODO:
                ErrorHandler.Handle(ex);
            }
        }

        #region Properties
        bool mRun = true;
        public bool Run
        {
            get { return mRun; }
            set { mRun = value; }
        }

        bool boolMessage = true;
        public bool Message
        {
            get { return boolMessage; }
            set { boolMessage = value; }
        }

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
