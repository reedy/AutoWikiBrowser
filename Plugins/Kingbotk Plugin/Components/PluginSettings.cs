using System;
using System.Collections.Generic;
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
        private const string conManuallyAssessParm = "ManuallyAssess";
        private const string conCleanupParm = "Cleanup";
        private const string conSkipBadTagsParm = "SkipBadTags";
        private const string conSkipWhenNoChangeParm = "SkipWhenNoChange";
        private const string conAssessmentsAlwaysLeaveACommentParm = "AlwaysLeaveAComment";

        private const string conOpenBadInBrowser = "OpenBadInBrowser";
        // Statistics:
        internal Stats PluginStats = new Stats();

        private readonly List<Label> StatLabels = new List<Label>();

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

            StatLabels.AddRange(new[]
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
            foreach (Label lbl in StatLabels)
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
        internal void ReadXML(XmlTextReader Reader)
        {
            ManuallyAssess = PluginManager.XMLReadBoolean(Reader, conManuallyAssessParm, ManuallyAssess);
            Cleanup = PluginManager.XMLReadBoolean(Reader, conCleanupParm, Cleanup);
            SkipBadTags = PluginManager.XMLReadBoolean(Reader, conSkipBadTagsParm, SkipBadTags);
            SkipWhenNoChange = PluginManager.XMLReadBoolean(Reader, conSkipWhenNoChangeParm, SkipWhenNoChange);
            AssessmentsAlwaysLeaveAComment = PluginManager.XMLReadBoolean(Reader, conAssessmentsAlwaysLeaveACommentParm,
                AssessmentsAlwaysLeaveAComment);
            OpenBadInBrowser = PluginManager.XMLReadBoolean(Reader, conOpenBadInBrowser, OpenBadInBrowser);
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
            Writer.WriteAttributeString(conManuallyAssessParm, ManuallyAssess.ToString());
            Writer.WriteAttributeString(conCleanupParm, Cleanup.ToString());
            Writer.WriteAttributeString(conSkipBadTagsParm, SkipBadTags.ToString());
            Writer.WriteAttributeString(conSkipWhenNoChangeParm, SkipWhenNoChange.ToString());
            Writer.WriteAttributeString(conAssessmentsAlwaysLeaveACommentParm, AssessmentsAlwaysLeaveAComment.ToString());
            Writer.WriteAttributeString(conOpenBadInBrowser, OpenBadInBrowser.ToString());
        }

        // Event handlers - menu items:
        private void SetAWBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var _with2 = PluginManager.AWBForm;
            _with2.SkipNonExistentPages.Checked = false;
            _with2.ApplyGeneralFixesCheckBox.Checked = false;
            _with2.AutoTagCheckBox.Checked = false;
            if (_with2.EditSummaryComboBox.Text == "clean up")
                _with2.EditSummaryComboBox.Text = "";
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
            lblBadTag.Text = val.ToString();
        }

        private void PluginStats_SkipMisc(int val)
        {
            lblSkipped.Text = val.ToString();
        }

        private void PluginStats_SkipNamespace(int val)
        {
            lblNamespace.Text = val.ToString();
        }

        private void PluginStats_SkipNoChange(int val)
        {
            lblNoChange.Text = val.ToString();
        }

        private void PluginStats_Tagged(int val)
        {
            lblTagged.Text = val.ToString();
        }

        private void PluginStats_RedLink(int val)
        {
            lblRedlink.Text = val.ToString();
        }

        // Statistics:
        internal sealed class Stats
        {
            private int mTagged;
            private int mSkipped;
            private int mSkippedNoChange;
            private int mSkippedBadTag;
            private int mSkippedNamespace;

            private int mRedLinks;
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
                get { return mTagged; }
                set
                {
                    mTagged = value;
                    if (evTagged != null)
                    {
                        evTagged(value);
                    }
                }
            }

            internal int Skipped
            {
                get { return mSkipped; }
                private set
                {
                    mSkipped = value;
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

            internal void SkippedMiscellaneousIncrement(bool DeincrementTagged)
            {
                Skipped += 1;
                if (DeincrementTagged)
                    Tagged -= 1;
            }

            internal int SkippedRedLink
            {
                get { return mRedLinks; }
                private set
                {
                    mRedLinks = value;
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
                get { return mSkippedNoChange; }
                set
                {
                    mSkippedNoChange = value;
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
                get { return mSkippedBadTag; }
                set
                {
                    mSkippedBadTag = value;
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
                get { return mSkippedNamespace; }
                set
                {
                    mSkippedNamespace = value;
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