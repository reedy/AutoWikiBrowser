/*
AWB Plugin Manager
Copyright
(C) 2007 Martin Richards
(C) 2008 Stephen Kennedy (Kingboyk) http://www.sdk-software.com/
(C) 2008 Sam Reed

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
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using AutoWikiBrowser.Plugins;
using WikiFunctions.Plugin;
using WikiFunctions;

namespace AutoWikiBrowser
{
    internal sealed partial class PluginManager : Form
    {
        private readonly IAutoWikiBrowser AWB;
        //List<string> prevArticlePlugins;
        //List<string> prevLMPlugins;
        private ListViewItem Lvi;

        private static string LastPluginLoadedLocation;

        public PluginManager(IAutoWikiBrowser awb) //, List<string> previousPlugins)
        {
            InitializeComponent();
            AWB = awb;
            //prevPlugins = previousPlugins;
        }

        public static void LoadNewPlugin(IAutoWikiBrowser awb)
        {
            OpenFileDialog pluginOpen = new OpenFileDialog();
            if (string.IsNullOrEmpty(LastPluginLoadedLocation))
                LoadLastPluginLoadedLocation();

            pluginOpen.InitialDirectory = string.IsNullOrEmpty(LastPluginLoadedLocation) ? Application.StartupPath : LastPluginLoadedLocation;
            
            pluginOpen.DefaultExt = "dll";
            pluginOpen.Filter = "DLL files|*.dll";
            pluginOpen.CheckFileExists = pluginOpen.Multiselect = /*pluginOpen.AutoUpgradeEnabled =*/ true;

            pluginOpen.ShowDialog();
            
            if (!string.IsNullOrEmpty(pluginOpen.FileName))
            {
                string newPath = Path.GetDirectoryName(pluginOpen.FileName);
                if (LastPluginLoadedLocation != newPath)
                {
                    LastPluginLoadedLocation = newPath;
                    SaveLastPluginLoadedLocation();
                }
            }

            Plugin.LoadPlugins(awb, pluginOpen.FileNames, true);
        }

        static void LoadLastPluginLoadedLocation()
        {
            try
            {
                Microsoft.Win32.RegistryKey reg = Microsoft.Win32.Registry.CurrentUser.
                    OpenSubKey("Software\\AutoWikiBrowser");

                if (reg != null)
                    LastPluginLoadedLocation = reg.GetValue("RecentPluginLoadedLocation", "").ToString();
            }
            catch { }
        }

        static void SaveLastPluginLoadedLocation()
        {
            try
            {
                Microsoft.Win32.RegistryKey reg = Microsoft.Win32.Registry.CurrentUser.
            CreateSubKey("Software\\AutoWikiBrowser");

                if (reg != null)
                    reg.SetValue("RecentPluginLoadedLocation", LastPluginLoadedLocation);
            }
            catch { }
        }

        private void PluginManager_Load(object sender, EventArgs e)
        {
            LoadLoadedPluginList();
            //LoadPreviouslyLoadedPluginList();
        }

        private void loadNewPluginsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadNewPlugin(AWB);
            lvPlugin.Items.Clear();
            LoadLoadedPluginList();
            //LoadPreviouslyLoadedPluginList();
        }

        private void LoadLoadedPluginList()
        {
            foreach (string pluginName in Plugin.GetPluginList())
            {
                Lvi = new ListViewItem(pluginName) {Group = lvPlugin.Groups["groupArticleLoaded"]};
                lvPlugin.Items.Add(Lvi);
            }
            foreach (string pluginName in WikiFunctions.Controls.Lists.ListMaker.GetListMakerPluginNames())
            {
                Lvi = new ListViewItem(pluginName) {Group = lvPlugin.Groups["groupLMLoaded"]};
                lvPlugin.Items.Add(Lvi);
            }

            UpdatePluginCount();
        }

        //private void LoadPreviouslyLoadedPluginList()
        //{
        //    foreach (string pluginName in prevArticlePlugins)
        //    {
        //        if (System.IO.File.Exists(pluginName))
        //        {
        //            lvi = new ListViewItem(pluginName);
        //            lvi.Group = lvPlugin.Groups["groupArticlePrevious"];
        //            lvPlugin.Items.Add(lvi);
        //        }
        //    }
        //    foreach (string pluginName in prevLMPlugins)
        //    {
        //        if (System.IO.File.Exists(pluginName))
        //        {
        //            lvi = new ListViewItem(pluginName);
        //            lvi.Group = lvPlugin.Groups["groupArticlePrevious"];
        //            lvPlugin.Items.Add(lvi);
        //        }
        //    }
        //}

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            //foreach (ListViewItem item in lvPlugin.SelectedItems)
            //{
            //    if (item.Group == lvPlugin.Groups["groupArticleLoaded"])
            //    {
            //        loadPluginToolStripMenuItem.Enabled = false;
            //        return;
            //    }
            //}
            //loadPluginToolStripMenuItem.Enabled = true;
            loadPluginToolStripMenuItem.Enabled = false;
        }

        private void loadPluginToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] plugins = new string[lvPlugin.SelectedItems.Count];

            for (int i = 0; i < lvPlugin.SelectedItems.Count; i++)
            {
                plugins[i] = lvPlugin.Items[lvPlugin.SelectedIndices[i]].Text;
            }

            Plugin.LoadPlugins(AWB, plugins, true);
        }

        private void UpdatePluginCount()
        {
            lblPluginCount.Text = lvPlugin.Items.Count.ToString();
        }
    }

    namespace Plugins
    {
        internal static class Plugin
        {
            /// <summary>
            /// Dictionary of Plugins, name, and reference to plugin
            /// </summary>
            internal static Dictionary<string, IAWBPlugin> Items = new Dictionary<string, IAWBPlugin>();

            public static List<IAWBPlugin> FailedPlugins = new List<IAWBPlugin>();

            /// <summary>
            /// String of plugins formatted like
            /// * [[WP:AWB/Plugin]]
            /// </summary>
            /// <returns></returns>
            internal static string GetPluginsWikiTextBlock()
            {
                StringBuilder retval = new StringBuilder();
                foreach (KeyValuePair<string, IAWBPlugin> plugin in Items)
                {
                    retval.AppendLine("* " + plugin.Value.WikiName);
                }
                return retval.ToString();
            }

            /// <summary>
            /// Gets the Number of Plugins currently Loaded
            /// </summary>
            /// <returns>Number of Plugins</returns>
            internal static int Count()
            {
                return Items.Count;
            }

            /// <summary>
            /// Gets a List of all the plugin names currently loaded
            /// </summary>
            /// <returns>List of Plugin Names</returns>
            internal static List<string> GetPluginList()
            {
                List<string> plugins = new List<string>();

                foreach (KeyValuePair<string, IAWBPlugin> a in Items)
                {
                    plugins.Add(a.Key);
                }

                return plugins;
            }

            /// <summary>
            /// Loads the plugin at startup, and updates the splash screen
            /// </summary>
            /// <param name="awb">IAutoWikiBrowser instance of AWB</param>
            /// <param name="splash">Splash Screen instance</param>
            internal static void LoadPluginsStartup(IAutoWikiBrowser awb, Splash splash)
            {
                splash.SetProgress(75);
                string path = Application.StartupPath;
                string[] pluginFiles = Directory.GetFiles(path, "*.DLL");

                LoadPlugins(awb, pluginFiles, false);
                splash.SetProgress(89);
            }

            public static void PluginObsolete(IAWBPlugin plugin)
            {
                if (!FailedPlugins.Contains(plugin))
                    FailedPlugins.Add(plugin);

                PluginObsolete(plugin.GetType().Assembly.Location);
            }

            static void PluginObsolete(string name)
            {
                MessageBox.Show("The plugin '" + name + "' is out-of date and needs to be updated.",
                    "Plugin error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            public static void PurgeFailedPlugins()
            {
                if (FailedPlugins.Count == 0) return;

                foreach (IAWBPlugin p in FailedPlugins)
                {
                    foreach (string s in Items.Keys)
                    {
                        if (Items[s] == p)
                        {
                            Items.Remove(s);
                            break;
                        }
                    }
                }
                FailedPlugins.Clear();
            }

            /// <summary>
            /// Loads all the plugins from the directory where AWB resides
            /// </summary>
            /// <param name="awb">IAutoWikiBrowser instance of AWB</param>
            /// <param name="plugins">Array of Plugin Names</param>
            /// <param name="afterStartup">Whether the plugin(s) are being loaded post-startup</param>
            internal static void LoadPlugins(IAutoWikiBrowser awb, string[] plugins, bool afterStartup)
            {
                try
                {
                    foreach (string plugin in plugins)
                    {
                        if (plugin.EndsWith("DotNetWikiBot.dll") || plugin.EndsWith("Diff.dll")
                            || plugin.EndsWith("WikiFunctions.dll"))
                            continue;

                        Assembly asm = null;
                        try
                        {
                            asm = Assembly.LoadFile(plugin);
                        }
                        catch
                        {
                        }

                        if (asm == null)
                            continue;

                        try
                        {
                            foreach (Type t in asm.GetTypes())
                            {
                                if (t.GetInterface("IAWBPlugin") != null)
                                {
                                    IAWBPlugin awbPlugin = (IAWBPlugin) Activator.CreateInstance(t);

                                    if (Items.ContainsKey(awbPlugin.Name))
                                    {
                                        MessageBox.Show("A plugin with the name \"" + awbPlugin.Name + "\", has already been added.\r\nPlease remove old duplicates from your AutoWikiBrowser Directory, and restart AWB.", "Duplicate AWB Plugin");
                                        continue;
                                    }

                                    InitialisePlugin(awbPlugin, awb);

                                    Items.Add(awbPlugin.Name, awbPlugin);

                                    if (afterStartup) UsageStats.AddedPlugin(awbPlugin);
                                }
                                else if (t.GetInterface("IListMakerPlugin") != null)
                                {
                                    IListMakerPlugin listMakerPlugin = (IListMakerPlugin) Activator.CreateInstance(t);
                                    WikiFunctions.Controls.Lists.ListMaker.AddProvider(listMakerPlugin);

                                    if (afterStartup) UsageStats.AddedPlugin(listMakerPlugin);
                                }
                            }
                        }
                        catch (MissingMemberException)
                        {
                            PluginObsolete(plugin);
                        }
                        catch (Exception ex)
                        {
                            ErrorHandler.Handle(ex);
                        }
                    }
                }
                catch (Exception ex)
                {
#if debug
                    ErrorHandler.Handle(ex);
#else
                    MessageBox.Show(ex.Message, "Problem loading plugins");
#endif
                }

            }

            /// <summary>
            /// Passes a reference of the main form to the plugin for initialisation
            /// </summary>
            /// <param name="plugin">IAWBPlugin to initialise</param>
            /// <param name="awb">IAutoWikiBrowser instance of AWB</param>
            private static void InitialisePlugin(IAWBPlugin plugin, IAutoWikiBrowser awb)
            {
                plugin.Initialise(awb);
            }

            /// <summary>
            /// Gets the Version string of a IAWBPlugin
            /// </summary>
            /// <param name="plugin">IAWBPlugin to get Version of</param>
            /// <returns>Version String</returns>
            internal static string GetPluginVersionString(IAWBPlugin plugin)
            { return Assembly.GetAssembly(plugin.GetType()).GetName().Version.ToString(); }

            /// <summary>
            /// Gets the Version string of a IListMakerPlugin
            /// </summary>
            /// <param name="plugin">IListMakerPlugin to get Version of</param>
            /// <returns>Version String</returns>
            internal static string GetPluginVersionString(IListMakerPlugin plugin)
            { return Assembly.GetAssembly(plugin.GetType()).GetName().Version.ToString(); }
        }
    }
}
