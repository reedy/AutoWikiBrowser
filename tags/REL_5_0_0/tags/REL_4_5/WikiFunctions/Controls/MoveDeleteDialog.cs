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
using System.Windows.Forms;

namespace WikiFunctions.Controls
{
    public partial class ArticleActionDialog : Form
    {
        public ArticleActionDialog(int MoveDeleteProtect)
        {
            InitializeComponent();
            MoveDelete.TextBoxIndexChanged += MoveDelete_TextBoxIndexChanged;
            string[] messages;

            if (MoveDeleteProtect == 3)
            {
                lblSummary.Location = new Point(8, 15);
                cmboSummary.Location = new Point(62, 12);
                lblNewTitle.Visible = false;
                txtNewTitle.Visible = false;
                Text = "Protect";
                btnOk.Text = "Protect";
                messages = new string[1];
                messages[0] = "Heavy vandalism";
            }
            else
            {
                MoveDelete.Visibility = false;
                lblExpiry.Visible = false;
                txtExpiry.Visible = false;
                chkCascadingProtection.Visible = false;

                if (MoveDeleteProtect == 1)
                {
                    Text = "Move";
                    btnOk.Text = "Move";
                    Size = new Size(Width, 120);

                    messages = new string[2];
                    messages[0] = "Typo in page title";
                    messages[1] = "Reverting vandalism page move";
                }
                else
                {
                    Text = "Delete";
                    btnOk.Text = "Delete";
                    Size = new Size(Width, 100);
                    lblSummary.Location = new Point(8, 15);
                    cmboSummary.Location = new Point(62, 12);

                    lblNewTitle.Visible = false;
                    txtNewTitle.Visible = false;

                    messages = new string[23];

                    messages[0] = "tagged for [[WP:PROD|proposed deletion]] for 5 days";
                    messages[1] = "[[WP:CSD#G1|Patent nonsense]]";
                    messages[2] = "[[WP:CSD#G2|Test page]]";
                    messages[3] = "[[WP:CSD#G3|Pure vandalism]]";
                    messages[4] = "[[WP:CSD#G4|Recreation of deleted material]]";
                    messages[5] = "[[WP:CSD#G5|Banned user]]";
                    messages[6] = "[[WP:CSD#G6|Housekeeping]]";
                    messages[7] = "[[WP:CSD#G7|Author requests deletion]]";
                    messages[8] = "[[WP:CSD#G8|Talk page of page that does not exist]]";
                    messages[9] = "[[WP:CSD#G10|Attack page]]";
                    messages[10] = "[[WP:CSD#G11|Blatant advertising]]";
                    messages[11] = "[[WP:CSD#G12|Blatant copyright infringement]]";
                    messages[12] = "[[WP:CSD#A1|Little or no context]]";
                    messages[13] = "[[WP:CSD#A2|Foreign language article]]";
                    messages[14] = "[[WP:CSD#A3|No content whatsoever]]";
                    messages[15] = "[[WP:CSD#A7|Non-notable person, group, company, or web content]]";
                    messages[16] = "[[WP:CSD#R1|Redirect to non-existent page]]";
                    messages[17] = "[[WP:CSD#R2|Redirect to the User: or User talk: space]]";
                    messages[18] = "[[WP:CSD#R3|Redirect as a result of an implausible typo]]";
                    messages[19] = "[[WP:CSD#C1|Empty category]]";
                    messages[20] = "[[WP:CSD#C2|Speedy renaming]]";
                    messages[21] = "[[WP:CSD#U1|User request]]";
                    messages[22] = "[[WP:CSD#U2|Nonexistent user]]";
                }
                cmboSummary.Items.AddRange(messages);
            }
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

        public int EditProtectionLevel
        {
            get { return MoveDelete.EditProtectionLevel; }
        }

        public int MoveProtectionLevel
        {
            get { return MoveDelete.MoveProtectionLevel; }
        }

        public string ProtectExpiry
        {
            get { return txtExpiry.Text; }
        }

        public bool CascadingProtection
        {
            get { return (chkCascadingProtection.Enabled && chkCascadingProtection.Checked); }
        }
    }
}
