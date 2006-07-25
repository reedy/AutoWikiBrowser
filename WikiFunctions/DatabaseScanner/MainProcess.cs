/*
DumpSearcher
Copyright (C) 2006 Martin Richards

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

namespace WikiFunctions.DatabaseScanner
{
    public delegate void FoundDel(object article);
    public delegate void StopDel();

    class MainProcess
    {
        public event FoundDel FoundArticle;
        public event StopDel StoppedEvent;

        Scanners scanners;
        string FileName = "";
        Stream stream;

        SendOrPostCallback SOPC;
        SendOrPostCallback SOPCstopped;
        private SynchronizationContext context;
        Thread ScanThread = null;

        public MainProcess(Scanners scns, string filename, ThreadPriority tp)
        {
            scanners = scns;
            FileName = filename;
            SOPC = new SendOrPostCallback(NewArticle);
            SOPCstopped = new SendOrPostCallback(Stopped);
            Priority = tp;
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
            try
            {
                stream = new FileStream(FileName, FileMode.Open);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

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

            string page = "page";
            string title = "title";
            string text = "text";

            try
            {
                using (XmlTextReader reader = new XmlTextReader(stream))
                {
                    reader.WhitespaceHandling = WhitespaceHandling.None;

                    while (reader.Read() && boolRun)
                    {
                        if (reader.Name == page)
                        {
                            reader.ReadToFollowing(title);
                            articleTitle = reader.ReadString();
                            reader.ReadToFollowing(text);
                            articleText = reader.ReadString();

                            if (scanners.Test(ref articleText, ref articleTitle))
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

        //System.Windows.Forms.MessageBox.Show("articles: " + articles.ToString() + " total stubs: " + stubs.ToString() + " stubbed: " + stubbedarticles.ToString());

        //articles++;

        //                        m = reg.Matches(articleText);

        //                        if (m.Count > 0)
        //                        {
        //                            stubbedarticles++;
        //                            stubs = stubs + m.Count;
        //                        }

        //Regex reg = new Regex("stub\\}\\}", RegexOptions.Compiled);
        //MatchCollection m;

        //int articles = 0;
        //int stubs = 0;
        //int stubbedarticles = 0;
    }
}
