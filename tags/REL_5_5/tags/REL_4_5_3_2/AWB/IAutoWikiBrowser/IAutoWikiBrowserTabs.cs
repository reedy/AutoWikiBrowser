/*
Copyright (C) 2008 Stephen Kennedy <steve@sdk-software.com>

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

using System.Collections.Generic;
using System.Windows.Forms;
using WikiFunctions.Plugin;

namespace AutoWikiBrowser
{
    partial class MainForm
    {
        private static List<TabPage> HiddenTabPages = new List<TabPage>();

        TabPage IAutoWikiBrowserTabs.MoreOptionsTab { get { return tpMoreOptions; } }
        TabPage IAutoWikiBrowserTabs.OptionsTab { get { return tpOptions; } }
        TabPage IAutoWikiBrowserTabs.SkipTab { get { return tpSkip; } }
        TabPage IAutoWikiBrowserTabs.StartTab { get { return tpStart; } }
        TabPage IAutoWikiBrowserTabs.DabTab { get { return tpDab; } }
        TabPage IAutoWikiBrowserTabs.BotTab { get { return tpBots; } }
        TabPage IAutoWikiBrowserTabs.LoggingTab { get { return tpLoggingOptions; } }
        bool IAutoWikiBrowserTabs.ContainsTabPage(TabPage tabp) { return MainTab.TabPages.Contains(tabp); }

        private void AddTabPage(TabPage tabp)
        {
            if (!MainTab.TabPages.Contains(tabp))
                MainTab.TabPages.Add(tabp);
        }

        void IAutoWikiBrowserTabs.AddTabPage(TabPage tabp)
        { AddTabPage(tabp); }

        void IAutoWikiBrowserTabs.RemoveTabPage(TabPage tabp)
        {
            if (MainTab.TabPages.Contains(tabp))
                MainTab.TabPages.Remove(tabp);
            if (HiddenTabPages.Contains(tabp))
                HiddenTabPages.Remove(tabp);
        }

        void IAutoWikiBrowserTabs.HideAllTabPages()
        {
            HiddenTabPages = new List<TabPage>();
            // HiddenTabPages.AddRange((IEnumerable<TabPage>)MainTab.TabPages); // why doesn't this work?
            foreach (TabPage tabp in MainTab.TabPages)
            {
                HiddenTabPages.Add(tabp);
            }
            MainTab.TabPages.Clear();
        }

        void IAutoWikiBrowserTabs.ShowAllTabPages()
        {
            foreach (TabPage tabp in MainTab.TabPages)
            {
                if (!HiddenTabPages.Contains(tabp))
                    HiddenTabPages.Add(tabp);
            }
            MainTab.TabPages.Clear();

            AddTabPage(tpOptions);
            AddTabPage(tpMoreOptions);
            AddTabPage(tpSkip);
            AddTabPage(tpDab);
            AddTabPage(tpBots);
            AddTabPage(tpStart);

            HiddenTabPages.Remove(tpOptions);
            HiddenTabPages.Remove(tpMoreOptions);
            HiddenTabPages.Remove(tpSkip);
            HiddenTabPages.Remove(tpDab);
            HiddenTabPages.Remove(tpBots);
            HiddenTabPages.Remove(tpStart);

            foreach (TabPage tabp in HiddenTabPages)
            { MainTab.TabPages.Add(tabp); }

            HiddenTabPages.Clear();
        }
    }
}
