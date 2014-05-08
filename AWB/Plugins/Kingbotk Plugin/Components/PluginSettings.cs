using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using System.Xml;
using WikiFunctions.API;

//Copyright © 2008 Stephen Kennedy (Kingboyk) http://www.sdk-software.com/
//Copyright © 2008 Sam Reed (Reedy) http://www.reedyboy.net/

//This program is free software; you can redistribute it and/or modify it under the terms of Version 2 of the GNU General Public License as published by the Free Software Foundation.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

//You should have received a copy of the GNU General Public License Version 2 along with this program; if not, write to the Free Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA

namespace AutoWikiBrowser.Plugins.Kingbotk.Components
{
    internal sealed partial class PluginSettingsControl
    {
        // XML parm-name constants:
        private const string ManuallyAssessParm = "ManuallyAssess";
        private const string CleanupParm = "Cleanup";
        private const string SkipBadTagsParm = "SkipBadTags";
        private const string SkipWhenNoChangeParm = "SkipWhenNoChange";
        private const string AssessmentsAlwaysLeaveACommentParm = "AlwaysLeaveAComment";

        private const string OpenBadInBrowserText = "OpenBadInBrowser";
        // Statistics:
        internal Stats PluginStats = new Stats();

        private readonly List<Label> _statLabels = new List<Label>();

        internal PluginSettingsControl()
        {
            // This call is required by the Windows Form Designer and must come first:
            InitializeComponent();

            PluginStats.SkipBadTag += PluginStats_SkipBadTag;
            PluginStats.SkipMisc += PluginStats_SkipMisc;
            PluginStats.SkipNamespace += PluginStats_SkipNamespace;
            PluginStats.SkipNoChange += PluginStats_SkipNoChange;
            PluginStats.evTagged += PluginStats_Tagged;
            PluginStats.RedLink += PluginStats_RedLink;

            PluginManager.AWBForm.SkipNoChangesCheckBox.CheckedChanged += SkipNoChangesCheckBoxCheckedChanged;

            _statLabels.AddRange(new[]
            {
                lblTagged,
                lblSkipped,
                lblNoChange,
                lblBadTag,
                lblNamespace,
                lblRedlink
            });
        }

        // AWB processing stopped/started:
        internal void AWBProcessingStart(AsyncApiEdit editor)
        {
            foreach (Label lbl in _statLabels)
            {
                if (string.IsNullOrEmpty(lbl.Text))
                    lbl.Text = "0";
            }

            TimerStats1.Visible = true;
            TimerStats1.Init(editor, ETALabel, PluginStats);

            PluginManager.StatusText.Text = "Started";
        }

        internal void AWBProcessingAborted()
        {
            TimerStats1.StopStats();
        }

        // Properties:

        internal bool ManuallyAssess
        {
            get { return ManuallyAssessCheckBox.Checked; }
            set { ManuallyAssessCheckBox.Checked = value; }
        }

        internal bool Cleanup
        {
            get { return CleanupCheckBox.Checked; }
            set { CleanupCheckBox.Checked = value; }
        }

        internal bool SkipBadTags
        {
            get { return SkipBadTagsCheckBox.Checked; }
            set { SkipBadTagsCheckBox.Checked = value; }
        }

        internal bool SkipWhenNoChange
        {
            get { return SkipNoChangesCheckBox.Checked; }
            set { SkipNoChangesCheckBox.Checked = value; }
        }

        internal bool AssessmentsAlwaysLeaveAComment { get; set; }

        internal bool OpenBadInBrowser
        {
            get { return OpenBadInBrowserCheckBox.Checked; }
            set { OpenBadInBrowserCheckBox.Checked = value; }
        }

        // XML interface:
        internal void ReadXML(XmlTextReader reader)
        {
            ManuallyAssess = PluginManager.XMLReadBoolean(reader, ManuallyAssessParm, ManuallyAssess);
            Cleanup = PluginManager.XMLReadBoolean(reader, CleanupParm, Cleanup);
            SkipBadTags = PluginManager.XMLReadBoolean(reader, SkipBadTagsParm, SkipBadTags);
            SkipWhenNoChange = PluginManager.XMLReadBoolean(reader, SkipWhenNoChangeParm, SkipWhenNoChange);
            AssessmentsAlwaysLeaveAComment = PluginManager.XMLReadBoolean(reader, AssessmentsAlwaysLeaveACommentParm,
                AssessmentsAlwaysLeaveAComment);
            OpenBadInBrowser = PluginManager.XMLReadBoolean(reader, OpenBadInBrowserText, OpenBadInBrowser);
        }

        internal void Reset()
        {
            ManuallyAssess = false;
            Cleanup = false;
            PluginStats = new Stats();
            AssessmentsAlwaysLeaveAComment = false;
            OpenBadInBrowser = false;
        }

        internal void WriteXML(XmlTextWriter Writer)
        {
            Writer.WriteAttributeString(ManuallyAssessParm, ManuallyAssess.ToString());
            Writer.WriteAttributeString(CleanupParm, Cleanup.ToString());
            Writer.WriteAttributeString(SkipBadTagsParm, SkipBadTags.ToString());
            Writer.WriteAttributeString(SkipWhenNoChangeParm, SkipWhenNoChange.ToString());
            Writer.WriteAttributeString(AssessmentsAlwaysLeaveACommentParm, AssessmentsAlwaysLeaveAComment.ToString());
            Writer.WriteAttributeString(OpenBadInBrowserText, OpenBadInBrowser.ToString());
        }

        // Event handlers - menu items:
        private void SetAWBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PluginManager.AWBForm.SkipNonExistentPages.Checked = false;
            PluginManager.AWBForm.ApplyGeneralFixesCheckBox.Checked = false;
            PluginManager.AWBForm.AutoTagCheckBox.Checked = false;
            if (PluginManager.AWBForm.EditSummaryComboBox.Text == "clean up")
                PluginManager.AWBForm.EditSummaryComboBox.Text = "";
        }

        private void MenuAbout_Click(object sender, EventArgs e)
        {
            AboutBox about = new AboutBox();
            about.Show();
        }

        private void MenuHelp_Click(object sender, EventArgs e)
        {
            PluginManager.AWBForm.ShowHelpEnWiki("User:Kingbotk/Plugin/User guide");
        }

        // Event handlers - our controls:
        private void ManuallyAssessCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (ManuallyAssess)
            {
                PluginManager.AWBForm.BotModeCheckbox.Enabled = false;
                PluginManager.AWBForm.BotModeCheckbox.Checked = false;
                SkipBadTagsCheckBox.Checked = false;
                SkipBadTagsCheckBox.Enabled = false;
                SkipNoChangesCheckBox.Checked = false;
                SkipNoChangesCheckBox.Enabled = false;
            }
            else
            {
                PluginManager.AWBForm.BotModeCheckbox.Enabled = PluginManager.AWBForm.TheSession.User.IsBot;
                SkipBadTagsCheckBox.Enabled = true;
                SkipNoChangesCheckBox.Enabled = true;
            }

            CleanupCheckBox.Checked = ManuallyAssess;
            CleanupCheckBox.Enabled = ManuallyAssess;
        }

        private void ResetTimerButton_Click(object sender, EventArgs e)
        {
            TimerStats1.Reset();
        }

        private void SkipBadTagsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            OpenBadInBrowserCheckBox.Visible = SkipBadTagsCheckBox.Checked;
        }

        private void SkipNoChangesCheckBoxCheckedChanged(object sender, EventArgs e)
        {
            if ((PluginManager.AWBForm.SkipNoChanges != SkipNoChangesCheckBox.Checked))
            {
                SkipNoChangesCheckBox.Checked = PluginManager.AWBForm.SkipNoChanges;
            }
        }

        // Event handlers - plugin stats:
        private void PluginStats_SkipBadTag(int val)
        {
            lblBadTag.Text = val.ToString(CultureInfo.InvariantCulture);
        }

        private void PluginStats_SkipMisc(int val)
        {
            lblSkipped.Text = val.ToString(CultureInfo.InvariantCulture);
        }

        private void PluginStats_SkipNamespace(int val)
        {
            lblNamespace.Text = val.ToString(CultureInfo.InvariantCulture);
        }

        private void PluginStats_SkipNoChange(int val)
        {
            lblNoChange.Text = val.ToString(CultureInfo.InvariantCulture);
        }

        private void PluginStats_Tagged(int val)
        {
            lblTagged.Text = val.ToString(CultureInfo.InvariantCulture);
        }

        private void PluginStats_RedLink(int val)
        {
            lblRedlink.Text = val.ToString(CultureInfo.InvariantCulture);
        }

        // Statistics:
        internal sealed class Stats
        {
            private int _tagged;
            private int _skipped;
            private int _skippedNoChange;
            private int _skippedBadTag;
            private int _skippedNamespace;

            private int _redLinks;
            internal event SkipMiscEventHandler SkipMisc;

            internal delegate void SkipMiscEventHandler(int val);

            internal event SkipNoChangeEventHandler SkipNoChange;

            internal delegate void SkipNoChangeEventHandler(int val);

            internal event SkipBadTagEventHandler SkipBadTag;

            internal delegate void SkipBadTagEventHandler(int val);

            internal event SkipNamespaceEventHandler SkipNamespace;

            internal delegate void SkipNamespaceEventHandler(int val);

            internal event evTaggedEventHandler evTagged;

            internal delegate void evTaggedEventHandler(int val);

            internal event RedLinkEventHandler RedLink;

            internal delegate void RedLinkEventHandler(int val);

            internal int Tagged
            {
                get { return _tagged; }
                set
                {
                    _tagged = value;
                    if (evTagged != null)
                    {
                        evTagged(value);
                    }
                }
            }

            internal int Skipped
            {
                get { return _skipped; }
                private set
                {
                    _skipped = value;
                    if (SkipMisc != null)
                    {
                        SkipMisc(value);
                    }
                }
            }

            internal void SkippedMiscellaneousIncrement()
            {
                Skipped += 1;
            }

            internal void SkippedMiscellaneousIncrement(bool deincrementTagged)
            {
                Skipped += 1;
                if (deincrementTagged)
                    Tagged -= 1;
            }

            internal int SkippedRedLink
            {
                get { return _redLinks; }
                private set
                {
                    _redLinks = value;
                    if (RedLink != null)
                    {
                        RedLink(value);
                    }
                }
            }

            internal void SkippedRedLinkIncrement()
            {
                Skipped += 1;
                SkippedRedLink += 1;
            }

            private int SkippedNoChange
            {
                get { return _skippedNoChange; }
                set
                {
                    _skippedNoChange = value;
                    if (SkipNoChange != null)
                    {
                        SkipNoChange(value);
                    }
                }
            }

            internal void SkippedNoChangeIncrement()
            {
                SkippedNoChange += 1;
                Skipped += 1;
            }

            private int SkippedBadTag
            {
                get { return _skippedBadTag; }
                set
                {
                    _skippedBadTag = value;
                    if (SkipBadTag != null)
                    {
                        SkipBadTag(value);
                    }
                }
            }

            internal void SkippedBadTagIncrement()
            {
                SkippedBadTag += 1;
                Skipped += 1;
            }

            private int SkippedNamespace
            {
                get { return _skippedNamespace; }
                set
                {
                    _skippedNamespace = value;
                    if (SkipNamespace != null)
                    {
                        SkipNamespace(value);
                    }
                }
            }

            internal void SkippedNamespaceIncrement()
            {
                SkippedNamespace += 1;
                Skipped += 1;
            }

            internal Stats()
            {
                Skipped = 0;
                SkippedBadTag = 0;
                SkippedNamespace = 0;
                SkippedNoChange = 0;
                Tagged = 0;
                SkippedRedLink = 0;
            }
        }

        private void SkipNoChangesCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if ((PluginManager.AWBForm.SkipNoChanges != SkipNoChangesCheckBox.Checked))
            {
                PluginManager.AWBForm.SkipNoChanges = SkipNoChangesCheckBox.Checked;
            }
        }
    }
}