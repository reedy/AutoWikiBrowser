/*
AWB Plugin Manager
Copyright (C) 2008 Sam Reed

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

using AutoWikiBrowser.Plugins;
using WikiFunctions.Plugin;

namespace AutoWikiBrowser
{
    public partial class PluginManager : Form
    {
        IAutoWikiBrowser AWB;
        //List<string> prevPlugins;
        ListViewItem lvi;

        public PluginManager(IAutoWikiBrowser iAWB) //, List<string> previousPlugins)
        {
            InitializeComponent();
            AWB = iAWB;
            //prevPlugins = previousPlugins;
        }

        public static void LoadNewPlugin(IAutoWikiBrowser awb)
        {
            OpenFileDialog pluginOpen = new OpenFileDialog();
            pluginOpen.InitialDirectory = Application.StartupPath;
            pluginOpen.DefaultExt = "dll";
            pluginOpen.Filter = "DLL files|*.dll";
            pluginOpen.CheckFileExists = true;
            pluginOpen.Multiselect = true;

            pluginOpen.ShowDialog();

            Plugin.LoadPlugins(awb, pluginOpen.FileNames, true);
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
    }
}