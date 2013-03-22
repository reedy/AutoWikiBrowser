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
using System.Data;
using System.Drawing;
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
        IAutoWikiBrowser AWB;
        //List<string> prevPlugins;
        ListViewItem lvi;

        static string LastPluginLoadedLocation;

        public PluginManager(IAutoWikiBrowser iAWB) //, List<string> previousPlugins)
        {
            InitializeComponent();
            AWB = iAWB;
            //prevPlugins = previousPlugins;
        }

        public static void LoadNewPlugin(IAutoWikiBrowser awb)
        {
            OpenFileDialog pluginOpen = new OpenFileDialog();
            if (string.IsNullOrEmpty(LastPluginLoadedLocation))
                LoadLastPluginLoadedLocation();

            if (string.IsNullOrEmpty(LastPluginLoadedLocation))
                pluginOpen.InitialDirectory = Application.StartupPath;
            else
                pluginOpen.InitialDirectory = LastPluginLoadedLocation;
            
            pluginOpen.DefaultExt = "dll";
            pluginOpen.Filter = "DLL files|*.dll";
            pluginOpen.CheckFileExists = pluginOpen.Multiselect = /*pluginOpen.AutoUpgradeEnabled =*/ true;

            pluginOpen.ShowDialog();
            
            string newPath = "";
            if (!string.IsNullOrEmpty(pluginOpen.FileName))
            {
                newPath = Path.GetDirectoryName(pluginOpen.FileName);
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
                    OpenSubKey("Software\\Wikipedia\\AutoWikiBrowser");

                LastPluginLoadedLocation = reg.GetValue("RecentPluginLoadedLocation", "").ToString();
            }
            catch { }
        }

        static void SaveLastPluginLoadedLocation()
        {
            try
            {
                Microsoft.Win32.RegistryKey reg = Microsoft.Win32.Registry.CurrentUser.
            CreateSubKey("Software\\Wikipedia\\AutoWikiBrowser");

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
                lvi = new ListViewItem(pluginName);
                lvi.Group = lvPlugin.Groups["groupLoaded"];
                lvPlugin.Items.Add(lvi);
            }
            UpdatePluginCount();
        }

        //private void LoadPreviouslyLoadedPluginList()
        //{
        //    foreach (string pluginName in prevPlugins)
        //    {
        //        if (System.IO.File.Exists(pluginName))
        //        {
        //            lvi = new ListViewItem(pluginName);
        //            lvi.Group = lvPlugin.Groups["groupPrevious"];
        //            lvPlugin.Items.Add(lvi);
        //        }
        //    }
        //}

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            foreach (ListViewItem item in lvPlugin.SelectedItems)
            {
                if (item.Group == lvPlugin.Groups["groupLoaded"])
                {
                    loadPluginToolStripMenuItem.Enabled = false;
                    break;
                }
            }
            loadPluginToolStripMenuItem.Enabled = true;
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
            lblPluginCount.Text = Plugin.Count().ToString();
        }
    }

    namespace Plugins
    {
        internal static class Plugin
        // TODO: Document me
        {
            internal static Dictionary<string, IAWBPlugin> Items = new Dictionary<string, IAWBPlugin>();

            internal static string GetPluginsWikiTextBlock()
            {
                string retval = "";
                foreach (KeyValuePair<string, IAWBPlugin> plugin in Items)
                {
                    retval += "* " + plugin.Value.WikiName + System.Environment.NewLine;
                }
                return retval;
            }

            internal static int Count()
            {
                return Items.Count;
            }

            internal static List<string> GetPluginList()
            {
                List<string> plugins = new List<string>();

                foreach (KeyValuePair<string, IAWBPlugin> a in Items)
                {
                    plugins.Add(a.Key);
                }

                return plugins;
            }

            internal static void LoadPluginsStartup(IAutoWikiBrowser awb, Splash splash)
            {
                splash.SetProgress(75);
                string path = Application.StartupPath;
                string[] pluginFiles = Directory.GetFiles(path, "*.DLL");

                LoadPlugins(awb, pluginFiles, false);
                splash.SetProgress(89);
            }

            internal static void LoadPlugins(IAutoWikiBrowser awb, string[] Plugins, bool afterStartup)
            {
                try
                {
                    foreach (string Plugin in Plugins)
                    {
                        if (Plugin.EndsWith("DotNetWikiBot.dll") || Plugin.EndsWith("Diff.dll"))
                            continue;

                        Assembly asm = null;
                        try
                        {
                            asm = Assembly.LoadFile(Plugin);
                        }
                        catch { }

                        if (asm != null)
                        {
                            Type[] types = asm.GetTypes();

                            foreach (Type t in types)
                            {
                                if (t.GetInterface("IAWBPlugin") != null)
                                {
                                    IAWBPlugin plugin = (IAWBPlugin)Activator.CreateInstance(t);
                                    Items.Add(plugin.Name, plugin);
                                    if (plugin.Name == "Kingbotk Plugin" && t.Assembly.GetName().Version.Major < 2)
                                        MessageBox.Show("You are using an out of date version of the Kingbotk Plugin. Please upgrade.",
                                            "Kingbotk Plugin", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                                    InitialisePlugin(plugin, awb);

                                    if (afterStartup) UsageStats.AddedPlugin(plugin);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Problem loading plugins");
                }
            }

            private static void InitialisePlugin(IAWBPlugin plugin, IAutoWikiBrowser awb)
            {
                try
                {
                    plugin.Initialise(awb);
                }
                catch (Exception ex)
                {
                    ErrorHandler.Handle(ex);
                }
            }

            internal static string GetPluginVersionString(IAWBPlugin plugin)
            { return System.Reflection.Assembly.GetAssembly(plugin.GetType()).GetName().Version.ToString(); }
        }
    }
}