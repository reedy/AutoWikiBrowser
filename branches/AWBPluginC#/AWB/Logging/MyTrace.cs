/*
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

// From the Kingbotk plugin. Converted from VB to C#

using System;
using System.Collections.Generic;
using WikiFunctions.Logging;
using System.Windows.Forms;

namespace AutoWikiBrowser.Logging
{
    /// <summary>
    /// Logging manager
    /// </summary>
    internal sealed class MyTrace : TraceManager, IAWBTraceListener
    {
        // The most important stuff:
        internal void Initialise()
        {
            try
            {
                foreach (KeyValuePair<string, IMyTraceListener> t in Listeners)
                {
                    if (t.Key != "AWB")
                        t.Value.WriteBulletedLine(AWBLogListener.LoggingStartButtonClicked, true, false, true);
                }
            }
            catch (Exception ex)
            {
                ConfigError(ex);
            }
        }

        internal void ConfigError(Exception ex)
        {
            MessageBox.Show(ex.Message);
            Program.AWB.Stop("AutoWikiBrowser");
        }

        // State:
        internal bool HaveOpenFile
        {
            get
            {
                foreach (KeyValuePair<string, IMyTraceListener> t in Listeners)
                {
                    if (t.Key != "AWB") return true; // we're interested in open files, not AWBLogListeners...
                }
                return false;
            }
        }

        // Overrides:
        public override void AddListener(string key, IMyTraceListener listener)
        {
            if (ContainsKey(key))
            {
                base.RemoveListener(key);
            }

            base.AddListener(key, listener);
        }

        public override void RemoveListener(string key)
        {
            IMyTraceListener listener;
            if (!Listeners.TryGetValue(key, out listener))
                return;

            base.RemoveListener(key);
        }

        public override void Close()
        {
            foreach (KeyValuePair<string, IMyTraceListener> t in Listeners)
            {
                t.Value.WriteCommentAndNewLine("closing all logs");
                t.Value.Close();
            }
            Listeners.Clear();
        }

        #region Generic overrides

        public void AWBSkipped(string reason)
        {
            foreach (KeyValuePair<string, IMyTraceListener> listener in Listeners)
            {
                ((IAWBTraceListener) listener.Value).AWBSkipped(reason);
            }
        }

        public void PluginSkipped()
        {
            foreach (KeyValuePair<string, IMyTraceListener> listener in Listeners)
            {
                ((IAWBTraceListener) listener.Value).PluginSkipped();
            }
        }

        public void UserSkipped()
        {
            foreach (KeyValuePair<string, IMyTraceListener> listener in Listeners)
            {
                ((IAWBTraceListener) listener.Value).UserSkipped();
            }
        }
        #endregion

        protected override string ApplicationName
        {
            get { return "AutoWikiBrowser logging manager"; }
        }
    }
}