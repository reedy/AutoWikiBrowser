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

namespace AutoWikiBrowser
{
    partial class MainForm
    {
        // Unfortunately, NotifyIcon is sealed, otherwise I would inherit from that and do tooltiptext/stats management there
        // Even more unfortunately, it seems it's tooltip is limited to 64 chars. Stinking great, Microsoft!
        // TODO: Maybe an alternative approach using mouse events? - doesn't seem to be a reliable way of doing even that! see e.g. http://64.233.183.104/search?q=cache:34QVls9xRoUJ:www.experts-exchange.com/Programming/Languages/.NET/Visual_Basic.NET/Q_21161863.html+notifyicon+mouseover&hl=en&ct=clnk&cd=1&gl=uk&lr=lang_en
        int intEdits;
        private int NumberOfEdits
        {
            get { return intEdits; }
            set
            {
                intEdits = value;
                lblEditCount.Text = "Edits: " + value.ToString();
                //UpdateNotifyIconTooltip();
            }
        }

        int intIgnoredEdits;
        private int NumberOfIgnoredEdits
        {
            get { return intIgnoredEdits; }
            set
            {
                intIgnoredEdits = value;
                lblIgnoredArticles.Text = "Ignored: " + value.ToString();
                //UpdateNotifyIconTooltip();
            }
        }

        int intEditsPerMin;
        private int NumberOfEditsPerMinute
        {
            get { return intEditsPerMin; }
            set
            {
                intEditsPerMin = value;
                lblEditsPerMin.Text = "Edits/min: " + value.ToString();
                //UpdateNotifyIconTooltip();
            }
        }

        internal void UpdateNotifyIconTooltip()
        {
            //ntfyTray.Text = "AutoWikiBrowser " + SettingsFile + "\r\n" + lblEditCount.Text + "\r\n" +
            //    lblIgnoredArticles.Text + "\r\n" + lblEditsPerMin.Text; // + current article if any
            string text = "AutoWikiBrowser - " + SettingsFile;
            if (text.Length > 64)
                ntfyTray.Text = text.Substring(0, 63); // 64 char limit
            else
                ntfyTray.Text = text;
        }
    }
}
