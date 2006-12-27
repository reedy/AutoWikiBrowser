using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WikiFunctions.Controls
{
    public partial class MoveDeleteDialog : Form
    {
        public MoveDeleteDialog(bool IsMove)
        {
            InitializeComponent();

            if (IsMove)
            {
                this.Text = "Move";
                btnOk.Text = "Move";
                string[] movemessages = new string[2];
                movemessages[0] = "typo in page title";
                movemessages[1] = "reverting vandalism page move";
                cmboSummary.Items.AddRange(movemessages);
            }
            else
            {
                this.Text = "Delete";
                btnOk.Text = "Delete";
                this.Size = new Size(this.Width, 115);
                cmboSummary.Location = new System.Drawing.Point(62, 12);

                label1.Visible = false;
                txtNewTitle.Visible = false;

                string[] deletemessages = new string[23];

                deletemessages[0] = "tagged for [[WP:PROD|proposed deletion]] for 5 days";
                deletemessages[1] = "[[WP:CSD#G1|Patent nonsense]]";
                deletemessages[2] = "[[WP:CSD#G2|Test page]]";
                deletemessages[3] = "[[WP:CSD#G3|Pure vandalism]]";
                deletemessages[4] = "[[WP:CSD#G4|Recreation of deleted material]]";
                deletemessages[5] = "[[WP:CSD#G5|Banned user]]";
                deletemessages[6] = "[[WP:CSD#G6|Housekeeping]]";
                deletemessages[7] = "[[WP:CSD#G7|Author requests deletion]]";
                deletemessages[8] = "[[WP:CSD#G8|Talk page of page that does not exist]]";
                deletemessages[9] = "[[WP:CSD#G10|Attack page]]";
                deletemessages[10] = "[[WP:CSD#G11|Blatant advertising]]";
                deletemessages[11] = "[[WP:CSD#G12|Blatant copyright infringement]]";
                deletemessages[12] = "[[WP:CSD#A1|Little or no context]]";
                deletemessages[13] = "[[WP:CSD#A2|Foreign language article]]";
                deletemessages[14] = "[[WP:CSD#A3|No content whatsoever]]";
                deletemessages[15] = "[[WP:CSD#A7|Non-notable person, group, company, or web content]]";
                deletemessages[16] = "[[WP:CSD#R1|Redirect to non-existent page]]";
                deletemessages[17] = "[[WP:CSD#R2|Redirect to the User: or User talk: space]]";
                deletemessages[18] = "[[WP:CSD#R3|Redirect as a result of an implausible typo]]";
                deletemessages[19] = "[[WP:CSD#C1|Empty category]]";
                deletemessages[20] = "[[WP:CSD#C2|Speedy renaming]]";
                deletemessages[21] = "[[WP:CSD#U1|User request]]";
                deletemessages[22] = "[[WP:CSD#U2|Nonexistent user]]";
                cmboSummary.Items.AddRange(deletemessages);
            }
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
    }
}