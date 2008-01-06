/*
Autowikibrowser
Copyright (C) 2007 Martin Richards
(C) 2007 Stephen Kennedy (Kingboyk) http://www.sdk-software.com/

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
using WikiFunctions;
using WikiFunctions.Plugin;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

namespace AutoWikiBrowser
{
    internal static class Find
    {
        public static Regex RegexObj;
        public static Match MatchObj;

        public static void ResetFind()
        {
            RegexObj = null;
            MatchObj = null;
        }

        public static void Find1(string strRegex, bool isRegex, bool caseSensive,
            System.Windows.Forms.TextBox txtEdit, string ArticleName)
        {
            string ArticleText = txtEdit.Text;

            RegexOptions regOptions;

            if (caseSensive)
                regOptions = RegexOptions.None;
            else
                regOptions = RegexOptions.IgnoreCase;

            strRegex = Tools.ApplyKeyWords(ArticleName, strRegex);

            if (!isRegex)
                strRegex = Regex.Escape(strRegex);

            if (MatchObj == null || RegexObj == null)
            {
                int findStart = txtEdit.SelectionStart;

                RegexObj = new Regex(strRegex, regOptions);
                MatchObj = RegexObj.Match(ArticleText, findStart);
                txtEdit.SelectionStart = MatchObj.Index;
                txtEdit.SelectionLength = MatchObj.Length;
                txtEdit.Focus();
                txtEdit.ScrollToCaret();
                return;
            }
            else
            {
                if (MatchObj.NextMatch().Success)
                {
                    MatchObj = MatchObj.NextMatch();
                    txtEdit.SelectionStart = MatchObj.Index;
                    txtEdit.SelectionLength = MatchObj.Length;
                    txtEdit.Focus();
                    txtEdit.ScrollToCaret();
                }
                else
                {
                    txtEdit.SelectionStart = 0;
                    txtEdit.SelectionLength = 0;
                    txtEdit.Focus();
                    txtEdit.ScrollToCaret();
                    ResetFind();
                }
            }
        }
    }

    namespace Plugins
    {
        internal static class Plugin
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

            /// <summary>
            /// 
            /// </summary>
            /// <param name="awb"></param>
            /// <returns></returns>
            internal static void LoadPlugins(IAutoWikiBrowser awb)
            {
                string path = Application.StartupPath;
                string[] pluginFiles = Directory.GetFiles(path, "*.DLL");

                LoadPlugins(awb, pluginFiles, false);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="awb"></param>
            /// <param name="Plugin"></param>
            internal static void LoadPlugin(IAutoWikiBrowser awb, string Plugin)
            {
                LoadPlugins(awb, new string[] { Plugin }, true);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="awb"></param>
            /// <param name="Plugins"></param>
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

                                    //Load Plugin one off if not loading at startup
                                    if (afterStartup)
                                        Items[plugin.Name].Initialise(awb);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Problem loading plugin");
                }

                //Load all Plugins as loading at startup
                if (!afterStartup)
                {
                    foreach (KeyValuePair<string, IAWBPlugin> a in Items)
                    {
                        a.Value.Initialise(awb);
                    }
                }
            }
        }
    }
}
