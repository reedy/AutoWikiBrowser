/*

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
