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
        public static Regex regexObj;
        public static Match matchObj;

        public static void resetFind()
        {
            regexObj = null;
            matchObj = null;
        }

        public static void find(string strRegex, bool isRegex, bool caseSensive, 
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

            if (matchObj == null || regexObj == null)
            {
                int findStart = txtEdit.SelectionStart;

                regexObj = new Regex(strRegex, regOptions);
                matchObj = regexObj.Match(ArticleText, findStart);
                txtEdit.SelectionStart = matchObj.Index;
                txtEdit.SelectionLength = matchObj.Length;
                txtEdit.Focus();
                txtEdit.ScrollToCaret();
                return;
            }
            else
            {
                if (matchObj.NextMatch().Success)
                {
                    matchObj = matchObj.NextMatch();
                    txtEdit.SelectionStart = matchObj.Index;
                    txtEdit.SelectionLength = matchObj.Length;
                    txtEdit.Focus();
                    txtEdit.ScrollToCaret();
                }
                else
                {
                    txtEdit.SelectionStart = 0;
                    txtEdit.SelectionLength = 0;
                    txtEdit.Focus();
                    txtEdit.ScrollToCaret();
                    resetFind();
                }
            }
        }
    }

    namespace Plugins
    {
        internal static class Plugin
        {
            internal static Dictionary<string, IAWBPlugin> Items = new Dictionary<string, IAWBPlugin>();

            // TODO: Make sure this gets called when AWB initialises a log; currently kingbotk plugin creates headers itself, of course
            internal static string GetPluginsWikiTextBlock()
            {
                string retval = "";
                foreach (KeyValuePair<string, IAWBPlugin> Plugin in Items)
                {
                    retval += Plugin.Value.WikiName + System.Environment.NewLine;
                }
                return retval;
            }

            internal static bool LoadPlugins(IAutoWikiBrowser AWB)
            {
                try
                {
                    string path = Application.StartupPath;
                    string[] pluginFiles = Directory.GetFiles(path, "*.DLL");

                    foreach (string s in pluginFiles)
                    {
                        if (s.EndsWith("DotNetWikiBot.dll") || s.EndsWith("Wikidiff2.dll"))
                            continue;

                        string imFile = Path.GetFileName(s);

                        Assembly asm = null;
                        try
                        {
                            asm = Assembly.LoadFile(path + "\\" + imFile);
                        }
                        catch { }

                        if (asm != null)
                        {
                            Type[] types = asm.GetTypes();

                            foreach (Type t in types)
                            {
                                Type g = t.GetInterface("IAWBPlugin");

                                if (g != null)
                                {
                                    IAWBPlugin plugin = (IAWBPlugin)Activator.CreateInstance(t);
                                    Items.Add(plugin.Name, plugin);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Problem loading plugin");
                }

                foreach (KeyValuePair<string, IAWBPlugin> a in Items)
                {
                    a.Value.Initialise(AWB);
                }

                return (Items.Count > 0);
            }
        }
    }
}
