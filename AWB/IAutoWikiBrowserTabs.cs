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

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using WikiFunctions.Plugin;

namespace AutoWikiBrowser
{
    partial class MainForm
    {
        private static List<TabPage> TabPages = new List<TabPage>();

        TabPage IAutoWikiBrowserTabs.MoreOptionsTab { get { return tpMoreOptions; } }
        TabPage IAutoWikiBrowserTabs.OptionsTab { get { return tpSetOptions; } }
        TabPage IAutoWikiBrowserTabs.SkipTab { get { return tpSkip; } }
        TabPage IAutoWikiBrowserTabs.StartTab { get { return tpStart; } }
        TabPage IAutoWikiBrowserTabs.DabTab { get { return tpDab; } }
        TabPage IAutoWikiBrowserTabs.BotTab { get { return tpBots; } }
        TabPage IAutoWikiBrowserTabs.LoggingTab { get { return tpLoggingOptions; } }

        void IAutoWikiBrowserTabs.AddTabPage(TabPage tab)
        {
        }

        void IAutoWikiBrowserTabs.RemoveTabPage(TabPage tab)
        {
        }

        void IAutoWikiBrowserTabs.HideAllTabPages()
        {
        }

        void IAutoWikiBrowserTabs.ShowAllTabPages()
        {
        }
    }
}
