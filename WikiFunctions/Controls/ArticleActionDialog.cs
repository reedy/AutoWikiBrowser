/*

Copyright (C) 2007 Martin Richards

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
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WikiFunctions.Controls
{
    public enum ArticleAction
    {
        Move,
        Delete,
        Protect
    }

    public partial class ArticleActionDialog : Form
    {
        private readonly ArticleAction CurrentAction;

        public ArticleActionDialog(ArticleAction moveDeleteProtect)
        {
            InitializeComponent();
            string[] messages;
            CurrentAction = moveDeleteProtect;

            if (moveDeleteProtect == ArticleAction.Protect)
            {
                lblSummary.Location = new Point(8, 15);
                cmboSummary.Location = new Point(62, 12);
                lblNewTitle.Visible = false;
                txtNewTitle.Visible = false;

                toolTip.SetToolTip(chkCascadingProtection, "Automatically protect any pages transcluded in this page");

                messages = new[]
                               {
                                   "Excessive vandalism",
                                   "High traffic page",
                                   "Excessive spamming",
                                   "Edit warring"
                               };
            }
            else
            {
                MoveDelete.Visibility = false;
                lblExpiry.Visible = false;
                txtExpiry.Visible = false;
                chkCascadingProtection.Visible = false;

                if (moveDeleteProtect == ArticleAction.Move)
                {
                    Size = new Size(Width, 240);
                    chkNoRedirect.Visible = true;
                    chkWatch.Visible = true;
                    chkDealWithAssoc.Visible = true;

                    messages = new[]
                                   {
                                       "Typo in page title",
                                       "Wikipedia naming convention",
                                       "Reverting vandalism page move",
                                       "Facilitating concordance with a category's name"
                                   };
                }
                else // if (moveDeleteProtect == ArticleAction.Delete)
                {
                    Size = new Size(Width, 100);
                    lblSummary.Location = new Point(8, 15);
                    cmboSummary.Location = new Point(62, 12);

                    lblNewTitle.Visible = false;
                    txtNewTitle.Visible = false;

                    messages = new[]
                                   {
                                       "tagged for [[WP:PROD|proposed deletion]] for 7 days",
                                       "[[WP:CSD#G1|G1]]: [[WP:PN|Patent nonsense]], meaningless, or incomprehensible",
                                       "[[WP:CSD#G2|G2]]: Test page",
                                       "[[WP:CSD#G3|G3]]: [[WP:Vandalism|Vandalism]]",
                                       "[[WP:CSD#G3|G3]]: Blatant [[WP:Do not create hoaxes|hoax]]",
                                       "[[WP:CSD#G4|G4]]: Recreation of a page that was [[WP:DEL|deleted]] per a [[WP:XFD|deletion discussion]]",
                                       "[[WP:CSD#G5|G5]]: Creation by a [[WP:BLOCK|blocked]] or [[WP:BAN|banned]] user in violation of block or ban",
                                       "[[WP:CSD#G6|G6]]: Housekeeping and routine (non-controversial) cleanup",
                                       "[[WP:CSD#G7|G7]]: One author who has requested deletion or blanked the page",
                                       "[[WP:CSD#G8|G8]]: Page dependent on a deleted or nonexistent page",
                                       "[[WP:CSD#G10|G10]]: [[WP:ATP|Attack page]] or negative unsourced [[WP:BLP|BLP]]",
                                       "[[WP:CSD#G11|G11]]: Unambiguous [[WP:NOTADVERTISING|advertising]] or promotion",
                                       "[[WP:CSD#G12|G12]]: Unambiguous [[WP:CV|copyright infringement]]",
                                       "[[WP:CSD#G13|G13]]: Abandoned [[WP:AFC|Article for creation]] – to retrieve it, see [[WP:REFUND/G13]]",
                                       "[[WP:CSD#A1|A1]]: Short article without enough context to identify the subject",
                                       "[[WP:CSD#A2|A2]]: Article in a foreign language that exists on another project",
                                       "[[WP:CSD#A3|A3]]: Article that has no meaningful, substantive content",
                                       "[[WP:CSD#A5|A5]]: Article that has been transwikied to another project",
                                       "[[WP:CSD#A7|A7]]: No credible indication of importance (individuals, animals, organizations, web content, events)",
                                       "[[WP:CSD#A9|A9]]: Music recording by redlinked artist and no indication of importance or significance",
                                       "[[WP:CSD#A10|A10]]: Recently created article that duplicates an existing topic",
                                       "[[WP:CSD#A11|A11]]: Made up by article creator or an associate, and no indication of importance/significance",
                                       "[[WP:CSD#R1|Redirect to non-existent page]]",
                                       "[[WP:CSD#R2|Cross namespace redirect from mainspace]]",
                                       "[[WP:CSD#R3|Recently created, implausible redirect]]",
                                       "[[WP:CSD#C1|C1]]: Empty category",
                                       "[[WP:CSD#C2|C2]]: Speedy renaming",
                                       "[[WP:CSD#U1|U1]]: User request to delete page in own userspace",
                                       "[[WP:CSD#U2|U2]]: Userpage or subpage of a nonexistent user",
                                       "[[WP:CSD#U3|U3]]: [[WP:NFC|Non-free]] [[Help:Gallery|gallery]]",
                                       "[[WP:CSD#U5|U5]]: [[WP:NOTWEBHOST|Misuse of Wikipedia as a web host]]",
                                       "[[WP:CSD#T2|T2]]: Template that unambiguously misrepresents established policy",
                                       "[[WP:CSD#T3|T3]]: Unused, redundant template",
                                       "[[WP:CSD#G8|G8]]: Component or documentation of a deleted template"
                                   };

                }
            }
            cmboSummary.Items.AddRange(messages);
        }

        private void MoveDelete_TextBoxIndexChanged(object sender, EventArgs e)
        {
            chkCascadingProtection.Enabled = MoveDelete.CascadingEnabled;
        }

        public bool AutoProtectAll
        {
            get { return chkAutoProtect.Checked; }
        }

        public string NewTitle
        {
            get { return txtNewTitle.Text; }
            set { txtNewTitle.Text = value; }
        }

        public string Summary
        {
            get { return cmboSummary.Text; }
            set { cmboSummary.Text = value; }
        }

        public string EditProtectionLevel
        {
            get { return CurrentAction == ArticleAction.Protect ? MoveDelete.EditProtectionLevel : ""; }
            set { MoveDelete.EditProtectionLevel = value; }
        }

        public string MoveProtectionLevel
        {
            get { return CurrentAction == ArticleAction.Protect ? MoveDelete.MoveProtectionLevel : ""; }
            set { MoveDelete.MoveProtectionLevel = value; }
        }

        public string ProtectExpiry
        {
            get { return txtExpiry.Text; }
        }

        public bool CascadingProtection
        {
            get { return chkCascadingProtection.Checked; }
        }

        public bool NoRedirect
        {
            get { return chkNoRedirect.Checked; }
        }

        public bool Watch
        {
            get { return chkWatch.Checked; }
        }

        public bool DealWithAssocTalkPage
        {
            get { return chkDealWithAssoc.Checked; }
        }

        private void ArticleActionDialog_Load(object sender, EventArgs e)
        {
            switch (CurrentAction)
            {
                case ArticleAction.Delete:
                    Text = btnOk.Text = "Delete";
                    break;
                case ArticleAction.Move:
                    Text = btnOk.Text = "Move";
                    break;
                case ArticleAction.Protect:
                    Text = btnOk.Text = "Protect";
                    break;
            }
        }

        private void ArticleActionDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DialogResult != DialogResult.OK)
                return;

            string errorTitle = "";

            StringBuilder errorMessage = new StringBuilder();

            switch (CurrentAction)
            {
                case ArticleAction.Move:
                    errorTitle = "Move error";

                    if (string.IsNullOrEmpty(cmboSummary.Text))
                        errorMessage.AppendLine("Please enter/select a move reason.");

                    if (string.IsNullOrEmpty(txtNewTitle.Text))
                        errorMessage.AppendLine("Please enter a new/target title.");

                    break;
                case ArticleAction.Protect:
                    errorTitle = "Protection error";

                    if (!string.IsNullOrEmpty(txtExpiry.Text) && Tools.DateBeforeToday(txtExpiry.Text))
                    {
                        errorMessage.AppendLine("Please enter an expiry date in the future");
                    }

                    if (string.IsNullOrEmpty(cmboSummary.Text))
                        errorMessage.AppendLine("Please enter/select a protection reason.");

                    if ((MoveProtectionLevel != "" || EditProtectionLevel != "") &&
                        string.IsNullOrEmpty(txtExpiry.Text))
                        errorMessage.AppendLine("Please enter an expiry time.");
                    break;
                case ArticleAction.Delete:
                    if (string.IsNullOrEmpty(cmboSummary.Text))
                    {
                        errorMessage.AppendLine("Please enter/select a deletion reason");
                        errorTitle = "Deletion reason required";
                    }
                    break;
            }

            if (errorMessage.Length > 0)
            {
                MessageBox.Show(errorMessage.ToString(), errorTitle);
                e.Cancel = true;
            }
        }
    }
}