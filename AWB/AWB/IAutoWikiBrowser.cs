/*
Copyright (C) 2007 Stephen Kennedy <steve@sdk-software.com>

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
using WikiFunctions.Plugin;
using WikiFunctions.Logging;
using System.Windows.Forms;
using WikiFunctions;

namespace AutoWikiBrowser
{
    partial class MainForm
    {
        // Objects:
        TraceManager IAutoWikiBrowser.TraceManager { get { return Program.MyTrace; } }
        WikiFunctions.Logging.Uploader.UploadableLogSettings2 IAutoWikiBrowser.LoggingSettings { get { return Program.MyTrace.LS.Settings; } }
        TabPage IAutoWikiBrowser.MoreOptionsTab { get { return tpMoreOptions; } }
        TabPage IAutoWikiBrowser.OptionsTab { get { return tpSetOptions; } }
        TabPage IAutoWikiBrowser.SkipTab { get { return tpSkip; } }
        TabPage IAutoWikiBrowser.StartTab { get { return tpStart; } }
        TabPage IAutoWikiBrowser.DabTab { get { return tpDab; } }
        TabPage IAutoWikiBrowser.BotTab { get { return tpBots; } }
        TabPage IAutoWikiBrowser.LoggingTab { get { return tpLoggingOptions; } }
        CheckBox IAutoWikiBrowser.BotModeCheckbox { get { return chkAutoMode; } }
        Button IAutoWikiBrowser.PreviewButton { get { return btnPreview; } }
        Button IAutoWikiBrowser.SaveButton { get { return btnSave; } }
        Button IAutoWikiBrowser.SkipButton { get { return btnIgnore; } }
        Button IAutoWikiBrowser.StopButton { get { return btnStop; } }
        Button IAutoWikiBrowser.DiffButton { get { return btnDiff; } }
        Button IAutoWikiBrowser.StartButton { get { return btnStart; } }
        ComboBox IAutoWikiBrowser.EditSummary { get { return cmboEditSummary; } }
        StatusStrip IAutoWikiBrowser.StatusStrip { get { return statusStrip1; } }
        NotifyIcon IAutoWikiBrowser.NotifyIcon { get { return ntfyTray; } }
        CheckBox IAutoWikiBrowser.SkipNonExistentPagesCheckBox { get { return chkSkipNonExistent; } }
        CheckBox IAutoWikiBrowser.ApplyGeneralFixesCheckBox { get { return chkGeneralFixes; } }
        CheckBox IAutoWikiBrowser.AutoTagCheckBox { get { return chkAutoTagger; } }
        bool IAutoWikiBrowser.SkipNoChanges { get { return chkSkipNoChanges.Checked; } set { chkSkipNoChanges.Checked = value; } }
        ToolStripMenuItem IAutoWikiBrowser.HelpToolStripMenuItem { get { return helpToolStripMenuItem; } }
        TextBox IAutoWikiBrowser.EditBox { get { return txtEdit; } }
        TextBox IAutoWikiBrowser.CategoryTextBox { get { return loggingSettings1.LoggingCategoryTextBox; } }
        Form IAutoWikiBrowser.Form { get { return this; } }
        ToolStripMenuItem IAutoWikiBrowser.PluginsToolStripMenuItem { get { return pluginsToolStripMenuItem; } }
        WikiFunctions.Controls.Lists.ListMaker IAutoWikiBrowser.ListMaker { get { return listMaker1; } }
        WikiFunctions.Browser.WebControl IAutoWikiBrowser.WebControl { get { return webBrowserEdit; } }
        ContextMenuStrip IAutoWikiBrowser.EditBoxContextMenu { get { return mnuTextBox; } }
        TabControl IAutoWikiBrowser.Tab { get { return tabControl1; } }
        WikiFunctions.Parse.FindandReplace IAutoWikiBrowser.FindandReplace { get { return findAndReplace; } }
        WikiFunctions.SubstTemplates IAutoWikiBrowser.SubstTemplates { get { return substTemplates; } }
        string IAutoWikiBrowser.CustomModule { get { if (cModule.ModuleEnabled && cModule.Module != null) return cModule.Code; else return null; } }

        // Settings & misc:
        System.Version IAutoWikiBrowser.AWBVersion { get { return Program.Version; } }
        System.Version IAutoWikiBrowser.WikiFunctionsVersion { get { return Tools.Version; } }
        string IAutoWikiBrowser.AWBVersionString { get { return Program.VersionString; } }
        string IAutoWikiBrowser.WikiFunctionsVersionString { get { return Tools.VersionString; } }
        string IAutoWikiBrowser.WikiDiffVersionString { get { return "(internal)"; } }
        /* void IAutoWikiBrowser.AddLogItem(ArticleEx article) //
            { LogControl1.AddLog(article); } */
        void IAutoWikiBrowser.AddLogItem(bool Skipped, AWBLogListener LogListener)
        { LogControl1.AddLog(Skipped, LogListener); }
        void IAutoWikiBrowser.TurnOffLogging() { Program.MyTrace.TurnOffLogging(); }
        void IAutoWikiBrowser.ShowHelp(string URL) { Help.ShowHelp(h, URL); }
        void IAutoWikiBrowser.ShowHelpEnWiki(string Article) { Help.ShowHelpEN(h, Article); }
        LangCodeEnum IAutoWikiBrowser.LangCode { get { return Variables.LangCode; } }
        ProjectEnum IAutoWikiBrowser.Project { get { return Variables.Project; } }

        // "Events":
        void IAutoWikiBrowser.SkipPage(IAWBPlugin sender, string reason) { ((IAutoWikiBrowser)this).SkipPage(sender.Name, reason); }
        void IAutoWikiBrowser.Start(IAWBPlugin sender) { ((IAutoWikiBrowser)this).Start(sender.Name); }
        void IAutoWikiBrowser.Stop(IAWBPlugin sender) { ((IAutoWikiBrowser)this).Stop(sender.Name); }
        void IAutoWikiBrowser.GetDiff(IAWBPlugin sender) { ((IAutoWikiBrowser)this).GetDiff(sender.Name); }
        void IAutoWikiBrowser.GetPreview(IAWBPlugin sender) { ((IAutoWikiBrowser)this).GetPreview(sender.Name); }
        void IAutoWikiBrowser.Save(IAWBPlugin sender) { ((IAutoWikiBrowser)this).Save(sender.Name); }

        public event GetLogUploadLocationsEvent GetLogUploadLocations;

        /* In the (perhaps unlikely) event we need to know the name of the plugin which calls these subroutines,
         * the code is here and ready to go. */
        void IAutoWikiBrowser.SkipPage(string sender, string reason) { SkipPage(reason); }
        void IAutoWikiBrowser.Start(string sender) { Start(); }
        void IAutoWikiBrowser.Stop(string sender) { Stop(); }
        void IAutoWikiBrowser.GetDiff(string sender) { GetDiff(); }
        void IAutoWikiBrowser.GetPreview(string sender) { GetPreview(); }
        void IAutoWikiBrowser.Save(string sender) { Save(); }

        // Fire GetLogUploadLocations event
        internal void GetLogUploadLocationsEvent(object sender, List<WikiFunctions.Logging.Uploader.LogEntry> locations)
        {
            if (GetLogUploadLocations != null)
                GetLogUploadLocations(this, locations);
        }
    }
}
