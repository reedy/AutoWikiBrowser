using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Net;
using System.Diagnostics;

namespace WikiFunctions
{
    public class Updater
    {
        /// <summary>
        /// Checks to see if AWBUpdater.exe.new exists, if it does, replace it.
        /// If not, see if the version of AWB Updater is older than the version on the checkpage, and run AWBUpdater if so
        /// </summary>
        public void Update()
        {
            try
            {
                String tempPath = ".\\";
                if (File.Exists(tempPath + "AWBUpdater.exe.new"))
                {
                    File.Copy(tempPath + "AWBUpdater.exe.new", tempPath + "AWBUpdater.exe", true);
                    File.Delete(tempPath + "AWBUpdater.exe.new");
                }
                else
                {
                    string text = "";
                    HttpWebRequest rq = (HttpWebRequest)WebRequest.Create("http://en.wikipedia.org/w/index.php?title=Wikipedia:AutoWikiBrowser/CheckPage/Version&action=edit");

                    rq.Proxy.Credentials = CredentialCache.DefaultCredentials;
                    rq.UserAgent = "WikiFunctions " + Tools.VersionString;

                    HttpWebResponse response = (HttpWebResponse)rq.GetResponse();

                    Stream stream = response.GetResponseStream();
                    StreamReader sr = new StreamReader(stream, Encoding.UTF8);

                    text = sr.ReadToEnd();

                    sr.Close();
                    stream.Close();
                    response.Close();

                    Match m_updversion = Regex.Match(text, @"&lt;!-- Updater version: (.*?) --&gt;");

                    if (m_updversion.Success && m_updversion.Groups[1].Value.Length == 4)
                    {
                        FileVersionInfo versionUpdater = FileVersionInfo.GetVersionInfo(".\\AWBUpdater.exe");

                        if (Convert.ToInt32(m_updversion.Groups[1].Value) > Convert.ToInt32(versionUpdater.FileVersion.Replace(".", "")))
                            System.Diagnostics.Process.Start(".\\AWBUpdater.exe");
                    }
                }
            }
            catch { }
        }
    }
}
