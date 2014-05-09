using AutoWikiBrowser.Plugins.Kingbotk.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using WikiFunctions;

//Copyright © 2008 Stephen Kennedy (Kingboyk) http://www.sdk-software.com/
//Copyright © 2008 Sam Reed (Reedy) http://www.reedyboy.net/

//This program is free software; you can redistribute it and/or modify it under the terms of Version 2 of the GNU General Public License as published by the Free Software Foundation.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

//You should have received a copy of the GNU General Public License Version 2 along with this program; if not, write to the Free Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA

// Manual assessment:
using WikiFunctions.API;

namespace AutoWikiBrowser.Plugins.Kingbotk.ManualAssessments
{
    internal sealed class Assessments : IDisposable
    {
        internal const string Me = "Wikipedia Assessments Plugin";
        // Objects:
        private List<CheckBox> _awbCleanupCheckboxes = new List<CheckBox>();
        private PluginSettingsControl _pluginSettings;

        private StateClass _state = new StateClass();
        // New:
        internal Assessments(PluginSettingsControl vPluginSettings)
        {
            _pluginSettings = vPluginSettings;

            // Get a reference to the cleanup checkboxes:
            foreach (Control ctl in PluginManager.AWBForm.OptionsTab.Controls["groupBox6"].Controls)
            {
                if (ReferenceEquals(ctl.GetType(), typeof (CheckBox)))
                    _awbCleanupCheckboxes.Add((CheckBox) ctl);
            }
            ToggleAWBCleanup(_pluginSettings.Cleanup);

            _pluginSettings.CleanupCheckBox.CheckedChanged += CleanupCheckBox_CheckedChanged;
            PluginManager.AWBForm.TheSession.StateChanged += EditorStatusChanged;
            PluginManager.AWBForm.SaveButton.Click += Save_Click;
            PluginManager.AWBForm.SkipButton.Click += Skip_Click;
        }

        #region "IDisposable"

        // To detect redundant calls
        private bool _disposed;

        // This procedure is where the actual cleanup occurs
        internal void Dispose(bool disposing)
        {
            // Exit now if the object has already been disposed
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                // The object is being disposed, not finalized.
                // It is safe to access other objects (other than the mybase object)
                // only from inside this block
                _pluginSettings.CleanupCheckBox.CheckedChanged -= CleanupCheckBox_CheckedChanged;
                PluginManager.AWBForm.TheSession.StateChanged -= EditorStatusChanged;
                PluginManager.AWBForm.SaveButton.Click -= Save_Click;
                PluginManager.AWBForm.SkipButton.Click -= Skip_Click;
            }

            // Perform cleanup that has to be executed in either case:
            _awbCleanupCheckboxes = null;
            _pluginSettings = null;
            _state = null;

            // Remember that this object has been disposed of:
            _disposed = true;
        }

        public void Dispose()
        {
            Debug.WriteLine("Disposing of AssessmentClass object");
            // Execute the code that does the cleanup.
            Dispose(true);
        }

        #endregion

        // Friend methods:
        internal void Reset()
        {
            ToggleAWBCleanup(false);

            var listMaker = PluginManager.AWBForm.ListMaker;
            if (listMaker.Count > 1 && _state.IsNextPage(((WikiFunctions.Article) listMaker.Items.Items[1]).Name))
            {
                listMaker.RemoveAt(1);
            }
            else if (listMaker.Count > 0 && _state.IsNextPage(((WikiFunctions.Article) listMaker.Items.Items[0]).Name))
            {
                listMaker.RemoveAt(0);
            }

            _state = new StateClass();
        }

        internal void ProcessMainSpaceArticle(string articleTitle)
        {
            if (_state.NextArticleShouldBeTalk)
            {
                IsThisABug("a talk page");
            }

            _state.NextTalkPageExpected = articleTitle;

            var listMaker = PluginManager.AWBForm.ListMaker;
            if (listMaker.Count < 2)
            {
                listMaker.Insert(1, _state.NextTalkPageExpected);
            }
            else if (!_state.IsNextPage(((WikiFunctions.Article) listMaker.Items.Items[1]).Name))
            {
                listMaker.Insert(1, _state.NextTalkPageExpected);
            }

            _state.NextEventShouldBeMainSpace = true;
            _state.NextArticleShouldBeTalk = true;
        }

        internal bool ProcessTalkPage(Article theArticle, PluginSettingsControl pluginSettings, ref bool reqPhoto)
        {
            bool weAddedAReqPhotoParam = false;
            bool returnVal = false;

            if (!_state.NextArticleShouldBeTalk)
            {
                IsThisABug("an article");
            }
            else if (!_state.IsNextPage(theArticle.FullArticleTitle))
            {
                IsThisABug(_state.NextTalkPageExpected);
            }
            else
            {
                _state.NextArticleShouldBeTalk = false;

                PluginManager.StatusText.Text = "Assessments plugin: please assess the article or click cancel";

                AssessmentForm frmDialog = new AssessmentForm();

                returnVal =
                    (frmDialog.ShowDialog(out _state.Classification, out _state.Importance, out _state.NeedsInfobox,
                        out _state.NeedsAttention, out _state.NeedsPhoto, _state.NextTalkPageExpected) == DialogResult.OK);

                if (returnVal)
                {
                    PluginManager.StatusText.Text = "Processing " + theArticle.FullArticleTitle;

                    foreach (PluginBase p in PluginManager.ActivePlugins)
                    {
                        if (
                            p.ProcessTalkPage(theArticle, _state.Classification, _state.Importance, _state.NeedsInfobox,
                                _state.NeedsAttention, true, ProcessTalkPageMode.ManualAssessment,
                                reqPhoto || _state.NeedsPhoto) && (reqPhoto || _state.NeedsPhoto) && p.HasReqPhotoParam)
                        {
                            weAddedAReqPhotoParam = true;
                        }
                        if (theArticle.PluginManagerGetSkipResults == SkipResults.SkipBadTag)
                        {
                            MessageBox.Show("Bad tag(s). Fix manually.", "Bad tag", MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation);
                            break; // TODO: might not be correct. Was : Exit For
                        }
                    }
                }
                else
                {
                    pluginSettings.PluginStats.SkippedMiscellaneousIncrement(false);
                    PluginManager.StatusText.Text = "Skipping this talk page";
                    LoadArticle();
                }
            }

            if (returnVal)
            {
                switch (_state.Classification)
                {
                    case Classification.Code:
                    case Classification.Unassessed:
                        theArticle.EditSummary = "Assessed article using " + Constants.WikiPlugin;
                        break;
                    default:
                        theArticle.EditSummary = "Assessing as " + _state.Classification + " class, using " +
                                                 Constants.WikiPlugin;
                        break;
                }

                reqPhoto = weAddedAReqPhotoParam;
            }
            else
            {
                reqPhoto = false;
            }

            return returnVal;
        }

        // Private:
        private void IsThisABug(string text)
        {
            PluginManager.StatusText.Text = "Unexpected sequence: Assessments plugin is stopping AWB.";
            MessageBox.Show(
                "The assessments plugin was expecting to receive " + text +
                " next. Is this a bug or is your list messed up?", "Expecting " + text, MessageBoxButtons.OK,
                MessageBoxIcon.Exclamation);
            PluginManager.StopAWB();
        }

        private void ToggleAWBCleanup(bool cleanup)
        {
            foreach (CheckBox chk in _awbCleanupCheckboxes)
            {
                chk.Checked = cleanup;
            }
        }

        private void LoadTalkPage()
        {
            //State.blnWaitingForATalkPage = True

            ToggleAWBCleanup(false);

            PluginManager.StatusText.Text = "Assessments plugin: loading talk page";
        }

        private void LoadArticle()
        {
            ToggleAWBCleanup(_pluginSettings.Cleanup);
        }

        // UI event handlers:
        private void Save_Click(object sender, EventArgs e)
        {
            if (!_disposed)
            {
                if (_state.NextEventShouldBeMainSpace)
                {
                    LoadTalkPage();
                }
                else
                {
                    LoadArticle();
                }

                _state.NextEventShouldBeMainSpace = !_state.NextEventShouldBeMainSpace;
            }
        }

        private void Skip_Click(object sender, EventArgs e)
        {
            if (!_disposed)
            {
                if (_state.NextEventShouldBeMainSpace)
                {
                    LoadTalkPage();
                }
                else
                {
                    LoadArticle();
                    PluginManager.AWBForm.TraceManager.SkippedArticle("User",
                        WikiFunctions.Logging.AWBLogListener.StringUserSkipped);
                    _pluginSettings.PluginStats.SkippedMiscellaneousIncrement(true);
                }

                _state.NextEventShouldBeMainSpace = !_state.NextEventShouldBeMainSpace;
            }
        }

        private void CleanupCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (!_disposed && _pluginSettings.Cleanup)
                ToggleAWBCleanup(true);
        }

        // Webcontrol event handlers:
        private void EditorStatusChanged(AsyncApiEdit sender)
        {
            if (PluginManager.AWBForm.TheSession.Editor.IsActive)
            {
                LoadArticle();
            }
        }

        // State:
        private sealed class StateClass
        {
            private string _page;

            internal string NextTalkPageExpected
            {
                get { return _page; }
                set
                {
                	_pageRegex = new Regex(Variables.NamespacesCaseInsensitive[Namespace.Talk] + Regex.Escape(value));
                    _page = Variables.Namespaces[Namespace.Talk] + value;
                }
            }

            private Regex _pageRegex;

            internal bool IsNextPage(string title)
            {
                return _pageRegex.IsMatch(title);
            }

            internal bool NextEventShouldBeMainSpace;

            internal bool NextArticleShouldBeTalk;
            // Assessment:
            internal Classification Classification;
            internal Importance Importance;
            internal bool NeedsInfobox;
            internal bool NeedsAttention;
            internal bool NeedsPhoto;
        }
    }
}