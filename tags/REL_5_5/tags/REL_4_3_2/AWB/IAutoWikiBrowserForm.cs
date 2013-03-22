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
using WikiFunctions.Plugin;
using System.Windows.Forms;

namespace AutoWikiBrowser
{
    partial class MainForm
    {
        CheckBox IAutoWikiBrowserForm.BotModeCheckbox { get { return chkAutoMode; } }
        Button IAutoWikiBrowserForm.PreviewButton { get { return btnPreview; } }
        Button IAutoWikiBrowserForm.SaveButton { get { return btnSave; } }
        Button IAutoWikiBrowserForm.SkipButton { get { return btnIgnore; } }
        Button IAutoWikiBrowserForm.StopButton { get { return btnStop; } }
        Button IAutoWikiBrowserForm.DiffButton { get { return btnDiff; } }
        Button IAutoWikiBrowserForm.StartButton { get { return btnStart; } }
        ComboBox IAutoWikiBrowserForm.EditSummaryComboBox { get { return cmboEditSummary; } }
        StatusStrip IAutoWikiBrowserForm.StatusStrip { get { return statusStrip1; } }
        NotifyIcon IAutoWikiBrowserForm.NotifyIcon { get { return ntfyTray; } }
        CheckBox IAutoWikiBrowserForm.SkipNonExistentPagesCheckBox { get { return chkSkipNonExistent; } }
        CheckBox IAutoWikiBrowserForm.ApplyGeneralFixesCheckBox { get { return chkGeneralFixes; } }
        CheckBox IAutoWikiBrowserForm.AutoTagCheckBox { get { return chkAutoTagger; } }
        TextBox IAutoWikiBrowserForm.EditBox { get { return txtEdit; } }
        TextBox IAutoWikiBrowserForm.CategoryTextBox { get { return loggingSettings1.LoggingCategoryTextBox; } }
        Form IAutoWikiBrowserForm.Form { get { return this; } }
        ToolStripMenuItem IAutoWikiBrowserForm.HelpToolStripMenuItem { get { return helpToolStripMenuItem; } }
        ToolStripMenuItem IAutoWikiBrowserForm.PluginsToolStripMenuItem { get { return pluginsToolStripMenuItem; } }
        ToolStripMenuItem IAutoWikiBrowserForm.InsertTagToolStripMenuItem { get { return insertTagToolStripMenuItem; } }
        ToolStripMenuItem IAutoWikiBrowserForm.ToolStripMenuGeneral { get { return ToolStripMenuGeneral; } }
        WikiFunctions.Controls.Lists.ListMaker IAutoWikiBrowserForm.ListMaker { get { return listMaker1; } }
        WikiFunctions.Browser.WebControl IAutoWikiBrowserForm.WebControl { get { return webBrowserEdit; } }
        ContextMenuStrip IAutoWikiBrowserForm.EditBoxContextMenu { get { return mnuTextBox; } }
    }
}
