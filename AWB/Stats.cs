/*
Autowikibrowser
Copyright (C) 2007 Martin Richards
(C) 2008 Stephen Kennedy (Kingboyk) http://www.sdk-software.com/

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

#undef INSTASTATS // turn on here and in Main.cs to make AWB log (empty) stats at startup

using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Text;
using WikiFunctions;
using System.Net;
using System.IO;
using WikiFunctions.Plugin;
using System.Xml;

namespace AutoWikiBrowser
{
    partial class MainForm
    {
        // Unfortunately, NotifyIcon is sealed, otherwise I would inherit from that and do tooltiptext/stats management there
        // Even more unfortunately, it seems it's tooltip is limited to 64 chars. Stinking great, Microsoft!
        // TODO: Maybe an alternative approach using mouse events? - doesn't seem to be a reliable way of doing even that! see e.g. http://64.233.183.104/search?q=cache:34QVls9xRoUJ:www.experts-exchange.com/Programming/Languages/.NET/Visual_Basic.NET/Q_21161863.html+notifyicon+mouseover&hl=en&ct=clnk&cd=1&gl=uk&lr=lang_en
        private int intEdits;
        public int NumberOfEdits
        {
            get { return intEdits; }
            private set
            {
                intEdits = value;
                lblEditCount.Text = "Edits: " + value.ToString();
                //UpdateNotifyIconTooltip();
                if (value == 100 || (value > 0 && value % 1000 == 0)) // we'll first report to remote db when we have 100 saves or app is exiting, whichever comes first; we'll also update db at 1000 and each 1000 thereafter
                    UsageStats.Do(false);
            }
        }

        private int intIgnoredEdits;
        public int NumberOfIgnoredEdits
        {
            get { return intIgnoredEdits; }
            private set
            {
                intIgnoredEdits = value;
                lblIgnoredArticles.Text = "Ignored: " + value.ToString();
                //UpdateNotifyIconTooltip();
            }
        }

        private int intEditsPerMin;
        public int NumberOfEditsPerMinute
        {
            get { return intEditsPerMin; }
            private set
            {
                intEditsPerMin = value;
                lblEditsPerMin.Text = "Edits/min: " + value.ToString();
                //UpdateNotifyIconTooltip();
            }
        }
    }

    /// <summary>
    /// A class to collect and submit some non-invasive usage stats, to help AWB developers track usage and plan development
    /// Stats can be viewed at http://awb.kingboyk.com/
    /// SQL and FTP access is available to Sourceforge-registered AWB developers only: please ask Reedy Boy or Kingboyk for
    /// the login details.
    /// </summary>
    internal static class UsageStats
    {
        // TODO: Add other stuff we'd like to track. e.g. I'd quite like to know if anybody is using log to file.

        private static int RecordId = 0;
        private static int SecretNumber = 0;
        private static int LastEditCount = 0;
        private static string mUserName = "";
        private static List<IAWBPlugin> newplugins = new List<IAWBPlugin>();

 #region Public
        internal static void Initialise()
        {
            // static constructor only gets called when an member of the class is first accessed, not at startup (surprised me!)
            // so, we'll have to call this code at startup manually instead
            Variables.User.UserNameChanged += UserNameChanged;
        }

        /// <summary>
        /// Call this when it's time to consider submitting some data
        /// </summary>
        internal static void Do(bool appexit)
        {
            try
            {
                if (EstablishedContact)
                {
                    if (Program.AWB.NumberOfEdits > LastEditCount || newplugins.Count > 0)
                        SubsequentContact();
                        // success:
                        newplugins.Clear();    
                }
                else
                    FirstContact();

                // success:
                LastEditCount = Program.AWB.NumberOfEdits;
            }
            catch (Exception ex)
            {
                if (appexit) ErrorHandler.Handle(ex); // else try again later
            }
        }

        /// <summary>
        /// Call when a plugin got added *after* application startup
        /// </summary>
        internal static void AddedPlugin(IAWBPlugin plugin)
        {
            // if we've already written to the remote database, we'll need to add details of this plugin when we next contact it, otherwise do nothing
            if (EstablishedContact) newplugins.Add(plugin);
        }
#endregion

#region Server Contact
        /// <summary>
        /// Send usage stats to server
        /// </summary>
        private static void FirstContact()
        {
#if DEBUG && INSTASTATS
#else
            if (Program.AWB.NumberOfEdits == 0) return;
#endif
            try
            {
                NameValueCollection postvars = new NameValueCollection();

                postvars.Add("Action", "Hello");
                postvars.Add("Version", Program.VersionString);

                switch (Variables.Project)
                {
                    case ProjectEnum.wikia:
                    case ProjectEnum.custom:
                        postvars.Add("Wiki", new Uri(Variables.URL).Host);

                        if (Variables.Project == ProjectEnum.custom)
                            postvars.Add("Language", "CUS");
                        else
                            postvars.Add("Language", "WIK");
                        break;
                    default:
                        postvars.Add("Wiki", Variables.Project.ToString()); // This returns a short string such as "wikipedia"; may want to convert to int and then to string so we store less in the db
                        postvars.Add("Language", Variables.LangCode.ToString());
                        break;
                }

                postvars.Add("Culture", System.Threading.Thread.CurrentThread.CurrentCulture.ToString());

                if (Properties.Settings.Default.Privacy)
                    postvars.Add("User", "<Withheld>");
                else
                    postvars.Add("User", mUserName);

                postvars.Add("Saves", Program.AWB.NumberOfEdits.ToString());
                postvars.Add("OS", Environment.OSVersion.VersionString);
#if DEBUG
                postvars.Add("Debug", "Y");
#else
            postvars.Add("Debug", "N");
#endif
                EnumeratePlugins(postvars, Plugins.Plugin.Items.Values);

                ReadXML(PostData(postvars));
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Send updated usage stats to server
        /// </summary>
        private static void SubsequentContact()
        {
            try
            {
                NameValueCollection postvars = new NameValueCollection();

                postvars.Add("Action", "Update");
                postvars.Add("RecordID", RecordId.ToString());
                postvars.Add("Verify", SecretNumber.ToString());

                EnumeratePlugins(postvars, newplugins);

                if (Program.AWB.NumberOfEdits > LastEditCount)
                    postvars.Add("Saves", Program.AWB.NumberOfEdits.ToString());

                PostData(postvars);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Returns true if we've sent initial stats to server
        /// </summary>
        private static bool EstablishedContact
        { get { return (RecordId > 0); } }

        /// <summary>
        /// Post a collection of names/values to server
        /// </summary>
        /// <param name="postvars"></param>
        /// <returns></returns>
        private static string PostData(NameValueCollection postvars)
        {
            const string url = "http://awb.kingboyk.com/";
            // echo scripts which just print out the POST vars, handy for early stages of testing:
            //const string url = "http://www.cs.tut.fi/cgi-bin/run/~jkorpela/echo.cgi";
            //const string url = "http://www.tipjar.com/cgi-bin/test";

            HttpWebRequest rq = Variables.PrepareWebRequest(url, Program.UserAgentString);
            rq.Method = "POST";
            rq.ContentType = "application/x-www-form-urlencoded";

            Stream RequestStream = rq.GetRequestStream();
            byte[] data = Encoding.UTF8.GetBytes(BuildPostDataString(postvars));
            RequestStream.Write(data, 0, data.Length);
            RequestStream.Close();

            HttpWebResponse rs = (HttpWebResponse)rq.GetResponse();
            if (rs.StatusCode == HttpStatusCode.OK)
                return new StreamReader(rs.GetResponseStream()).ReadToEnd();
            else
                throw new WebException(rs.StatusDescription, WebExceptionStatus.UnknownError);
        }
#endregion

#region Helper routines
        private static string BuildPostDataString(NameValueCollection postvars)
        {
            string tmp = string.Empty;
            for (int i = 0; i < postvars.Keys.Count; i++)
            {
                if (i > 0)
                    tmp += "&";

                tmp += postvars.Keys[i] + "=" + postvars[postvars.Keys[i]];
            }

            return tmp;
        }

        private static void EnumeratePlugins(NameValueCollection postvars, ICollection<IAWBPlugin> plugins)
        {
            int i = 0;

            postvars.Add("PluginCount", plugins.Count.ToString());

            foreach (IAWBPlugin plugin in plugins)
            {
                i++;
                string P = "P" + i.ToString();
                postvars.Add(P + "N", plugin.Name);
                postvars.Add(P + "V", Plugins.Plugin.GetPluginVersionString(plugin));
            }
        }

        private static void ReadXML(string xml)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);
                XmlNodeList nodes = doc.GetElementsByTagName("DB");
                if (nodes.Count == 1 && nodes[0].Attributes.Count == 2)
                {
                    RecordId = int.Parse(nodes[0].Attributes["Record"].Value);
                    SecretNumber = int.Parse(nodes[0].Attributes["Verify"].Value);
                }
                else
                {
                    throw new System.Data.ConstraintException();
                }
            }
            catch (System.Data.ConstraintException)
            {
                throw new XmlException("Error parsing XML returned from UsageStats server");
            }
            catch (Exception ex)
            {
                throw new XmlException("Error parsing XML returned from UsageStats server", ex);
            }
        }

        private static void UserNameChanged(object sender, EventArgs e)
        {
            if (Variables.User.Name != "")
                mUserName = Variables.User.Name;
        }
#endregion
    }
}
