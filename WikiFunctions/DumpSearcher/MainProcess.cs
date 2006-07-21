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

namespace WikiFunctions.DumpSearcher
{
    public delegate void FoundDel(string article);
    public delegate void StopDel();

    class MainProcess
    {        
        public event FoundDel FoundArticle;
                
        public event StopDel Stopped;

        Scanners scanners;
        string FileName = "";
        int Limit = 100000;
        Stream stream;

        SendOrPostCallback SOPC;
        private SynchronizationContext context;
        Thread ScanThread = null;

        public MainProcess(Scanners scns, string filename, int ResultLimit)
        {
            scanners = scns;
            FileName = filename;
            Limit = ResultLimit;
            SOPC = new SendOrPostCallback(NewArticle);
        }

        private void NewArticle(object o)
        {
            this.FoundArticle(o.ToString());
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

            try
            {
                using (XmlTextReader reader = new XmlTextReader(stream))
                {
                    reader.WhitespaceHandling = WhitespaceHandling.None;
                    while (reader.Read() && boolRun)
                    {
                        if (reader.LocalName == "page")
                        {
                            reader.ReadToFollowing("title");
                            articleTitle = reader.ReadInnerXml();
                            reader.ReadToFollowing("text");
                            articleText = reader.ReadInnerXml();
                                                  
                            if (scanners.Test(articleText, articleTitle))
                            {
                                context.Post(SOPC, articleTitle);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (boolMessage)
                    throw new ApplicationException("Problem on " + articleTitle + "\r\n\r\n" + ex.Message);
            }
            finally
            {
                this.Stopped();
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

        ThreadPriority tpriority = ThreadPriority.Normal;
        public ThreadPriority Priority
        {
            get { return tpriority; }
            set
            {
                tpriority = value;
                if(ScanThread != null)
                    ScanThread.Priority = value; 
            }
        }


    }
}
