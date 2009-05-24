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

using System.Windows.Forms;
using WikiFunctions.Logging;
using WikiFunctions.Plugin;

namespace AutoWikiBrowser
{
    partial class MainForm
    {
        void IAutoWikiBrowserCommands.SkipPage(IAWBPlugin sender, string reason) { ((IAutoWikiBrowserCommands)this).SkipPage(sender.Name, reason); }
        void IAutoWikiBrowserCommands.Start(IAWBPlugin sender) { ((IAutoWikiBrowserCommands)this).Start(sender.Name); }
        void IAutoWikiBrowserCommands.Stop(IAWBPlugin sender) { ((IAutoWikiBrowserCommands)this).Stop(sender.Name); }
        void IAutoWikiBrowserCommands.GetDiff(IAWBPlugin sender) { ((IAutoWikiBrowserCommands)this).GetDiff(sender.Name); }
        void IAutoWikiBrowserCommands.GetPreview(IAWBPlugin sender) { ((IAutoWikiBrowserCommands)this).GetPreview(sender.Name); }
        void IAutoWikiBrowserCommands.Save(IAWBPlugin sender) { ((IAutoWikiBrowserCommands)this).Save(sender.Name); }
        void IAutoWikiBrowserCommands.SkipPage(string sender, string reason) { SkipPage(reason); }
        void IAutoWikiBrowserCommands.Start(string sender) { StopProcessing = false; Start(); }
        void IAutoWikiBrowserCommands.Stop(string sender) { Stop(); }
        void IAutoWikiBrowserCommands.GetDiff(string sender) { GetDiff(); }
        void IAutoWikiBrowserCommands.GetPreview(string sender) { GetPreview(); }
        void IAutoWikiBrowserCommands.Save(string sender) { Save(); }

        void IAutoWikiBrowserCommands.AddLogItem(bool skipped, AWBLogListener logListener)
        { logControl.AddLog(skipped, logListener); }
        void IAutoWikiBrowserCommands.TurnOffLogging() { Program.MyTrace.TurnOffLogging(); }
        void IAutoWikiBrowserCommands.ShowHelp(string url) { HelpForm.ShowHelp(HelpForm, url); }
        void IAutoWikiBrowserCommands.ShowHelpEnWiki(string article) { HelpForm.ShowHelpEN(HelpForm, article); }

        void IAutoWikiBrowserCommands.AddMainFormClosingEventHandler(FormClosingEventHandler handler) { FormClosing += handler; }

        void IAutoWikiBrowserCommands.StartProgressBar() { StartProgressBar(); }
        void IAutoWikiBrowserCommands.StopProgressBar() { StopProgressBar(); }

        void IAutoWikiBrowserCommands.AddArticleRedirectedEventHandler(WikiFunctions.ArticleRedirected handler) { ArticleWasRedirected += handler; }
    }
}
