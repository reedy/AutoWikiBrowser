/*
AWB Plugin Manager
Copyright
(C) 2007 Martin Richards
(C) 2008 Stephen Kennedy (Kingboyk) http://www.sdk-software.com/
(C) 2008-2018 Sam Reed

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
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using AutoWikiBrowser.Plugins;
using WikiFunctions.Plugin;

namespace AutoWikiBrowser
{
    internal sealed partial class PluginManager : Form
    {
        private readonly IAutoWikiBrowser _awb;

        private static string _lastPluginLoadedLocation;

        public PluginManager(IAutoWikiBrowser awb)
        {
            InitializeComponent();
            _awb = awb;
        }

        public static void LoadNewPlugin(IAutoWikiBrowser awb)
        {
            OpenFileDialog pluginOpen = new OpenFileDialog();
            if (string.IsNullOrEmpty(_lastPluginLoadedLocation))
                LoadLastPluginLoadedLocation();

            pluginOpen.InitialDirectory = string.IsNullOrEmpty(_lastPluginLoadedLocation) ? Application.StartupPath : _lastPluginLoadedLocation;

            pluginOpen.DefaultExt = "dll";
            pluginOpen.Filter = "DLL files|*.dll";
            pluginOpen.CheckFileExists = pluginOpen.Multiselect = true;

            pluginOpen.ShowDialog();

            if (!string.IsNullOrEmpty(pluginOpen.FileName))
            {
                string newPath = Path.GetDirectoryName(pluginOpen.FileName);
                if (_lastPluginLoadedLocation != newPath)
                {
                    _lastPluginLoadedLocation = newPath;
                    SaveLastPluginLoadedLocation();
                }
            }

            Plugin.LoadPlugins(awb, pluginOpen.FileNames, true);
        }

        //TODO:Use Utils
        static void LoadLastPluginLoadedLocation()
        {
            try
            {
                Microsoft.Win32.RegistryKey reg = Microsoft.Win32.Registry.CurrentUser.
                    OpenSubKey("Software\\AutoWikiBrowser");

                if (reg != null)
                    _lastPluginLoadedLocation = reg.GetValue("RecentPluginLoadedLocation", "").ToString();
            }
            catch
            {
            }
        }

        static void SaveLastPluginLoadedLocation()
        {
            try
            {
                Microsoft.Win32.RegistryKey reg = Microsoft.Win32.Registry.CurrentUser.
                    CreateSubKey("Software\\AutoWikiBrowser");

                if (reg != null)
                    reg.SetValue("RecentPluginLoadedLocation", _lastPluginLoadedLocation);
            }
            catch
            {
            }
        }

        private void PluginManager_Load(object sender, EventArgs e)
        {
            LoadLoadedPluginList();
        }

        private void loadNewPluginsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadNewPlugin(_awb);
            lvPlugin.Items.Clear();
            LoadLoadedPluginList();
        }

        private void LoadLoadedPluginList()
        {
            foreach (string pluginName in Plugin.GetAWBPluginList())
            {
                lvPlugin.Items.Add(new ListViewItem(pluginName) { Group = lvPlugin.Groups["groupAWBLoaded"] });
            }

            foreach (string pluginName in Plugin.GetBasePluginList())
            {
                lvPlugin.Items.Add(new ListViewItem(pluginName) { Group = lvPlugin.Groups["groupBaseLoaded"] });
            }

            foreach (string pluginName in Plugin.GetListMakerPluginList())
            {
                lvPlugin.Items.Add(new ListViewItem(pluginName) { Group = lvPlugin.Groups["groupLMLoaded"] });
            }

            foreach (string pluginName in Plugin.FailedPlugins.Keys)
            {
                lvPlugin.Items.Add(new ListViewItem(pluginName) { Group = lvPlugin.Groups["groupObsolete"] });
            }

            foreach (string assemblyName in Plugin.FailedAssemblies)
            {
                lvPlugin.Items.Add(new ListViewItem(assemblyName) { Group = lvPlugin.Groups["groupFailed"] });
            }

            UpdatePluginCount();
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            loadPluginToolStripMenuItem.Enabled = false;
        }

        private void loadPluginToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] plugins = new string[lvPlugin.SelectedItems.Count];

            for (int i = 0; i < lvPlugin.SelectedItems.Count; i++)
            {
                plugins[i] = lvPlugin.Items[lvPlugin.SelectedIndices[i]].Text;
            }

            Plugin.LoadPlugins(_awb, plugins, true);
        }

        private void UpdatePluginCount()
        {
            lblPluginCount.Text = lvPlugin.Items.Count.ToString();
        }
    }
}