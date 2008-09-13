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
using System.Text;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

namespace WikiFunctions.DBScanner
{
    public delegate void FoundDel(object article);
    public delegate void StopDel();

    class MainProcess
    {
        public event FoundDel FoundArticle;
        public event StopDel StoppedEvent;

        string FileName = "";
        string From = "";
        internal Stream stream;

        SendOrPostCallback SOPC;
        SendOrPostCallback SOPCstopped;
        private SynchronizationContext context;
        Thread ScanThread;

        List<Scan> s;
        bool ignore = false;

        public MainProcess(List<Scan> z, string filename, ThreadPriority tp, bool ignoreComments, string StartFrom)
            : this(z, filename, tp, ignoreComments)
        {
            From = StartFrom;
        }

        public MainProcess(List<Scan> z, string filename, ThreadPriority tp, bool ignoreComments)
        {
            FileName = filename;
            SOPC = new SendOrPostCallback(NewArticle);
            SOPCstopped = new SendOrPostCallback(Stopped);
            Priority = tp;
            ignore = ignoreComments;

            s = z;

            try
            {
                stream = new FileStream(FileName, FileMode.Open);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void NewArticle(object o)
        {
            this.FoundArticle(o);
        }

        private void Stopped(object o)
        {
            this.StoppedEvent();
        }

        public void Stop()
        {
            Run = false;
            if (ScanThread != null)
            {
                ScanThread.Abort();
                ScanThread.Join();
            }
        }

        public void Start()
        {
            context = SynchronizationContext.Current;

            ThreadStart thr_Process = new ThreadStart(Process);
            ScanThread = new Thread(thr_Process);
            ScanThread.IsBackground = true;
            ScanThread.Priority = tpriority;
            ScanThread.Start();
        }

        private void Process()
        {
            string articleText = "";
            string articleTitle = "";
            string articleTimestamp = "";
            string articleRestriction = "";

            string timestamp = "timestamp";
            string page = "page";
            string title = "title";
            string text = "text";
            //string restriction = "restrictions";
 
            bool test = true;        

            try
            {
                using (XmlTextReader reader = new XmlTextReader(stream))
                {
                    reader.WhitespaceHandling = WhitespaceHandling.None;
                    
                    if (From.Length > 0)
                    {//move to start from article
                        while (reader.Read() && boolRun)
                        {
                            if (reader.Name == page)
                            {
                                reader.ReadToFollowing(title);
                                articleTitle = reader.ReadString();

                                if (From == articleTitle)
                                    break;
                            }
                        }
                    }

                    while (reader.Read() && boolRun)
                    {
                        test = true;

                        if (reader.Name == page)
                        {
                            reader.ReadToFollowing(title);
                            articleTitle = reader.ReadString();

                            //reader.ReadToFollowing(restriction); //TODO:This is wrong. Only want to read the restriction if in that <page></page>

                            //if (reader.Name == restriction)
                            //    articleRestriction = reader.ReadString();
                            //else
                            //    articleRestriction = "";

                            reader.ReadToFollowing(timestamp);
                            articleTimestamp = reader.ReadString();
                            reader.ReadToFollowing(text);
                            articleText = reader.ReadString();

                            if(ignore)
                                articleText = WikiRegexes.Comments.Replace(articleText, "");

                            foreach (Scan z in s)
                            {
                                if (!z.Check(ref articleText, ref articleTitle, articleTimestamp, articleRestriction))
                                {
                                    test = false;
                                    break;
                                }                                    
                            }                           
                         
                            if (test)
                            {
                                context.Post(SOPC, articleTitle);
                            }
                        }
                    }
                }
            }
            catch (ThreadAbortException) { }
            catch (Exception ex)
            {
                if (boolMessage)
                    System.Windows.Forms.MessageBox.Show("Problem on " + articleTitle + "\r\n\r\n" + ex.Message);
            }
            finally
            {
                if (boolMessage)
                    context.Post(SOPCstopped, articleTitle);
            }
        }

        bool boolRun = true;
        public bool Run
        {
            get { return boolRun; }
            set { boolRun = value; }
        }

        bool boolMessage = true;
        public bool Message
        {
            get { return boolMessage; }
            set { boolMessage = value; }
        }

        ThreadPriority tpriority = ThreadPriority.BelowNormal;
        public ThreadPriority Priority
        {
            get { return tpriority; }
            set
            {
                tpriority = value;
                if (ScanThread != null)
                    ScanThread.Priority = value;
            }
        }
    }
}
