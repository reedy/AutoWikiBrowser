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

#undef INSTASTATS // turn on here and in Main.cs to make AWB log (empty) stats at startup (The scope of a symbol created by using #define is the file in which it was defined)

using System;
using System.Collections.Specialized;
using System.Collections.Generic;
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
        // T-O-D-O: Maybe an alternative approach using mouse events? - doesn't seem to be a reliable way of doing even that! see e.g. http://64.233.183.104/search?q=cache:34QVls9xRoUJ:www.experts-exchange.com/Programming/Languages/.NET/Visual_Basic.NET/Q_21161863.html+notifyicon+mouseover&hl=en&ct=clnk&cd=1&gl=uk&lr=lang_en
        private int NoEdits;
        public int NumberOfEdits
        {
            get { return NoEdits; }
            private set
            {
                NoEdits = value;
                lblEditCount.Text = "Edits: " + value;
                //UpdateNotifyIconTooltip();
                if (value == 100 || (value > 0 && value % 1000 == 0)) // we'll first report to remote db when we have 100 saves or app is exiting, whichever comes first; we'll also update db at 1000 and each 1000 thereafter
                    UsageStats.Do(false);
            }
        }

        private int NoNewPages;
        public int NumberOfNewPages
        {
            get { return NoNewPages; }
            private set
            {
                NoNewPages = value;
                lblNewArticles.Text = "New: " + value;
            }
        }

        private int NoIgnoredEdits;
        public int NumberOfIgnoredEdits
        {
            get { return NoIgnoredEdits; }
            private set
            {
                NoIgnoredEdits = value;
                lblIgnoredArticles.Text = "Skipped: " + value;
            }
        }

        private int NoEditsPerMin;
        public int NumberOfEditsPerMinute
        {
            get { return NoEditsPerMin; }
            private set
            {
                NoEditsPerMin = value;
                lblEditsPerMin.Text = "Edits/min: " + value;
            }
        }

        private int NoPagesPerMin;
        public int NumberOfPagesPerMinute
        {
            get { return NoPagesPerMin; }
            private set
            {
                NoPagesPerMin = value;
                lblPagesPerMin.Text = "Pages/min: " + value;
            }
        }
    }

    /// <summary>
    /// A class to collect and submit some non-invasive usage stats, to help AWB developers track usage and plan development
    /// Stats can be viewed at http://awb.kingboyk.com/
    /// SQL and FTP access is available to Sourceforge-registered AWB developers only: please ask Reedy or Kingboyk for
    /// the login details.
    /// </summary>
    internal static class UsageStats
    {
        // TODO: Add other stuff we'd like to track. e.g. I'd quite like to know if anybody is using log to file.

        private const string StatsURL = "http://awb.kingboyk.com/";
        private static int RecordId;
        private static int SecretNumber;
        private static int LastEditCount;
        private static bool SentUserName;
        private static readonly List<IAWBPlugin> NewAWBPlugins = new List<IAWBPlugin>();
        private static readonly List<IAWBBasePlugin> NewAWBBasePlugins = new List<IAWBBasePlugin>();
        private static readonly List<IListMakerPlugin> NewListMakerPlugins = new List<IListMakerPlugin>();

        private static string UserName
        {
            get { return Variables.MainForm.TheSession.User.Name; }
        }

        #region Public

        /// <summary>
        /// Call this when it's time to consider submitting some data
        /// </summary>
        internal static void Do(bool appexit)
        {
            try
            {
                if (EstablishedContact)
                {
                    if (Program.AWB.NumberOfEdits > LastEditCount || NewAWBPlugins.Count > 0
                        || HaveUserNameToSend) SubsequentContact();
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
        /// Call when a plugin was added *after* application startup
        /// </summary>
        internal static void AddedPlugin(IAWBPlugin plugin)
        {
            // if we've already written to the remote database, we'll need to add details of this plugin when we next contact it, otherwise do nothing
            if (EstablishedContact) NewAWBPlugins.Add(plugin);
        }

        /// <summary>
        /// Call when a plugin was added *after* application startup
        /// </summary>
        internal static void AddedPlugin(IAWBBasePlugin plugin)
        {
            if (EstablishedContact) NewAWBBasePlugins.Add(plugin);
        }

        /// <summary>
        /// Call when a plugin was added *after* application startup
        /// </summary>
        internal static void AddedPlugin(IListMakerPlugin plugin)
        {
            if (EstablishedContact) NewListMakerPlugins.Add(plugin);
        }
        #endregion

        #region Server Contact
        /// <summary>
        /// Send usage stats to server
        /// </summary>
        private static void FirstContact()
        {
#if !DEBUG && !INSTASTATS
            if (Program.AWB.NumberOfEdits == 0) return;
#endif
            NameValueCollection postvars = new NameValueCollection
                                               {
                                                   {"Action", "Hello"},
                                                   {"Version", Program.VersionString}
                                               };

            // Greetings and AWB version:

            // Site/project name:
            // TODO: Here or in PHP: tl.wikipedia.org  	CUS: Translate to site name/lang code any Wikimedia site set up as custom
            if (Variables.IsCustomProject || Variables.IsWikia)
                postvars.Add("Wiki", Variables.Host);
            else
                postvars.Add("Wiki", Variables.Project.ToString());
            // This returns a short string such as "wikipedia"; may want to convert to int and then to string so we store less in the db

            // Language code:
            if (Variables.IsWikia)
            {
                postvars.Add("Language", "WIK");
            }
            else if (Variables.IsCustomProject || Variables.IsWikimediaMonolingualProject)
            {
                postvars.Add("Language", "CUS");
            }
            else
            {
                postvars.Add("Language", Variables.LangCode);
            }

            // UI culture:
            postvars.Add("Culture", System.Threading.Thread.CurrentThread.CurrentCulture.ToString());

            // Username:
            ProcessUsername(postvars);

            // Other details:
            postvars.Add("Saves", Program.AWB.NumberOfEdits.ToString());
            postvars.Add("OS", Environment.OSVersion.VersionString);
#if DEBUG
            postvars.Add("Debug", "Y");
#else
            postvars.Add("Debug", "N");
#endif
            EnumeratePlugins(postvars,
                             Plugins.Plugin.AWBPlugins.Values,
                             Plugins.Plugin.AWBBasePlugins.Values,
                             Plugins.Plugin.ListMakerPlugins.Values);

            ReadXML(PostData(postvars));
        }

        /// <summary>
        /// Send updated usage stats to server
        /// </summary>
        private static void SubsequentContact()
        {
            NameValueCollection postvars = new NameValueCollection
                                               {
                                                   {"Action", "Update"},
                                                   {"RecordID", RecordId.ToString()},
                                                   {"Verify", SecretNumber.ToString()}
                                               };

            EnumeratePlugins(postvars, NewAWBPlugins, NewAWBBasePlugins, NewListMakerPlugins);
            ProcessUsername(postvars);

            if (Program.AWB.NumberOfEdits > LastEditCount)
                postvars.Add("Saves", Program.AWB.NumberOfEdits.ToString());

            PostData(postvars);
            NewAWBPlugins.Clear();
            NewListMakerPlugins.Clear();
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
            try
            {
                Program.AWB.StartProgressBar();
                StatusLabelText = "Contacting stats server...";
                Program.AWB.Form.Cursor = System.Windows.Forms.Cursors.WaitCursor;

                return Tools.PostData(postvars, StatsURL);
            }
            catch (WebException ex)
            {
                Tools.WriteDebug("UsageStats", ex.Message);
            }
            catch (IOException ex)
            {
                Tools.WriteDebug("UsageStats", ex.Message);
            }
            finally
            {
                Program.AWB.StopProgressBar();
                StatusLabelText = "";
                Program.AWB.Form.Cursor = System.Windows.Forms.Cursors.Default;
            }
            return null;
        }
        #endregion

        #region Helper routines
        private static string StatusLabelText { set { Program.AWB.StatusLabelText = value; } }

        private static void EnumeratePlugins(NameValueCollection postvars, ICollection<IAWBPlugin> awbPlugins, ICollection<IAWBBasePlugin> awbBasePlugins, ICollection<IListMakerPlugin> listMakerPlugins)
        {
            int i = 0;

            postvars.Add("PluginCount", (awbPlugins.Count + awbBasePlugins.Count + listMakerPlugins.Count).ToString());

            foreach (IAWBPlugin plugin in awbPlugins)
            {
                i++;
                string p = "P" + i;
                postvars.Add(p + "N", plugin.Name);
                postvars.Add(p + "V", Plugins.Plugin.GetPluginVersionString(plugin));
                postvars.Add(p + "T", "0");
            }

            foreach (IListMakerPlugin plugin in listMakerPlugins)
            {
                i++;
                string p = "P" + i;
                postvars.Add(p + "N", plugin.Name);
                postvars.Add(p + "V", Plugins.Plugin.GetPluginVersionString(plugin));
                postvars.Add(p + "T", "1");
            }

            foreach (IAWBBasePlugin plugin in awbBasePlugins)
            {
                i++;
                string p = "P" + i;
                postvars.Add(p + "N", plugin.Name);
                postvars.Add(p + "V", Plugins.Plugin.GetPluginVersionString(plugin));
                postvars.Add(p + "T", "2");
            }
        }

        private static void ReadXML(string xml)
        {
            try
            {
                if (string.IsNullOrEmpty(xml)) return; // handle network errors

                // we don't *need* these IDs if we're exiting, but I think it does no harm to check we received a valid response
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
                    throw new XmlException("Error parsing XML returned from UsageStats server");
                }
            }
            catch (Exception ex)
            {
                if (ex is XmlException)
                    throw;
                
                throw new XmlException("Error parsing XML returned from UsageStats server", ex);
            }
        }

        private static void ProcessUsername(NameValueCollection postvars)
        {
            if (!SentUserName)
            {
                if (Properties.Settings.Default.Privacy)
                {
                    postvars.Add("User", "<Withheld>");
                    SentUserName = true;
                }
                else if (!string.IsNullOrEmpty(UserName))
                {
                    postvars.Add("User", UserName);
                    SentUserName = true;
                }
            }
        }

        private static bool HaveUserNameToSend
        {
            get
            {
                return (!SentUserName &&
                    (Properties.Settings.Default.Privacy || !string.IsNullOrEmpty(UserName)));
            }
        }

        #endregion

        internal static void OpenUsageStatsURL()
        { Tools.OpenURLInBrowser(StatsURL); }
    }
}
