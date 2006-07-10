//$Header: /cvsroot/mwiki-browser/main/mwiki-browser/ReplaceSpecial.cs,v 1.22 2006/07/06 11:20:45 ligulem Exp $
/*
    Derived from Autowikibrowser
    Copyright (C) 2006 Martin Richards

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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace WikiFunctions.MWB
{
    public partial class ReplaceSpecial : Form, IRuleControlOwner
    {
        public const string XmlName = "replacerules";

        IRule currentRule_;
        TreeNode clipboard_;
        Control ruleControl_;
        RuleTreeHistory history_;


        public ReplaceSpecial()
        {
            InitializeComponent();

            history_ = new RuleTreeHistory(RulesTreeView);

            //NewRule();
            UpdateEnbabledStates();
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            SaveCurrentRule();
            this.Hide();
        }

        private void ReplaceSpecial_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveCurrentRule();
            e.Cancel = true;
            this.Hide();
        }

        private void RulesTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (RulesTreeView.SelectedNode == null)
                return;

            SaveCurrentRule();
            RestoreSelectedRule();
            UpdateEnbabledStates();
        }


        void SaveCurrentRule()
        {
            IRule r = currentRule_;
            if (r == null)
                return;

            r.Save();
        }


        void RestoreSelectedRule()
        {
            if (RulesTreeView.SelectedNode == null)
            {
                if (currentRule_ != null)
                {
                    currentRule_.DisposeControl();
                    currentRule_ = null;
                    NoRuleSelectedLabel.Show();
                }
            }
            else
            {
                NoRuleSelectedLabel.Hide();

                IRule oldrule = currentRule_;

                currentRule_ = (IRule)RulesTreeView.SelectedNode.Tag;

                this.SuspendLayout();

                Point pos = new Point();
                ruleControl_ = null;
                ruleControl_ = currentRule_.CreateControl(this, this.RuleControlSpace.Controls, pos);
                ruleControl_.Size = this.RuleControlSpace.Size;

                currentRule_.Name = RulesTreeView.SelectedNode.Text;

                currentRule_.Restore();

                if (oldrule != null)
                {
                    if (!currentRule_.Equals(oldrule))
                        oldrule.DisposeControl();
                }

                ruleControl_.Visible = true;

                this.ResumeLayout();
            }
            UpdateEnbabledStates();
        }

        private void UpButton_Click(object sender, EventArgs e)
        {
            MoveSelectedUp();
        }

        private void MoveSelectedUp()
        {
            TreeNode tn = RulesTreeView.SelectedNode;

            if (tn == null)
                return;

            RulesTreeView.Select();

            TreeNodeCollection col = GetOwningNodes(tn);

            if (col.Count < 2)
                return;

            TreeNode p = tn.PrevNode;
            if (p == null)
                return;

            history_.Save();

            col.Remove(tn);
            int i = col.IndexOf(p);
            col.Insert(i, tn);

            RulesTreeView.SelectedNode = tn;
            RulesTreeView.Select();
            RulesTreeView.ExpandAll();
            RestoreSelectedRule();
        }

        TreeNodeCollection GetOwningNodes(TreeNode t)
        {
            TreeNode p = t.Parent;
            if (p != null)
                return p.Nodes;
            return RulesTreeView.Nodes;
        }

        private void DownButton_Click(object sender, EventArgs e)
        {
            TreeNode tn = RulesTreeView.SelectedNode;

            if (tn == null)
                return;

            RulesTreeView.Select();

            TreeNodeCollection col = GetOwningNodes(tn);

            if (col.Count < 2)
                return;

            TreeNode p = tn.NextNode;
            if (p == null)
                return;

            history_.Save();

            int i = col.IndexOf(p);
            col.Remove(tn);
            col.Insert(i, tn);

            RulesTreeView.SelectedNode = tn;
            RulesTreeView.ExpandAll();
            RestoreSelectedRule();
        }

        private void InsideWhatCombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void InsideCheckBox_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void NewRuleButton_Click(object sender, EventArgs e)
        {
            NewRule();
        }

        private void NewSubruleButton_Click(object sender, EventArgs e)
        {
            NewSubrule();
        }

        public void NameChanged(Control rc, string name)
        {
            if (RulesTreeView.SelectedNode == null)
                return;

            if (name == "")
                return;

            if (RulesTreeView.SelectedNode.Text == name)
                return;

            RulesTreeView.SelectedNode.Text = name;
        }


        void UpdateEnbabledStates()
        {
            bool has_selection = RulesTreeView.SelectedNode != null;

            if (ruleControl_ != null)
                ruleControl_.Enabled = has_selection;

            DeleteButton.Enabled = has_selection;
            UpButton.Enabled = has_selection;
            DownButton.Enabled = has_selection;
            NewSubruleButton.Enabled = has_selection;

            DeleteMenuItem.Enabled = has_selection;
            DeleteContextMenuItem.Enabled = has_selection;

            NewSubruleMenu.Enabled = has_selection;
            NewSubruleContextMenuItem.Enabled = has_selection;

            NewSubruleMenuItem.Enabled = has_selection;
            NewSubruleInTemplateCallMenuItem.Enabled = has_selection;
            NewSubruleTemplateParameterMenuItem.Enabled = has_selection;

            PasteMenuItem.Enabled = clipboard_ != null;
            PasteContextMenuItem.Enabled = clipboard_ != null;

            CutMenuItem.Enabled = has_selection;
            CutContextMenuItem.Enabled = has_selection;

            CopyMenuItem.Enabled = has_selection;
            CopyContextMenuItem.Enabled = has_selection;

            UndoMenuItem.Enabled = history_.CanUndo;
            RedoMenuItem.Enabled = history_.CanRedo;
        }


        private void DeleteButton_Click(object sender, EventArgs e)
        {
            DeleteCmd();
        }

        void DeleteCmd()
        {
            TreeNode st = RulesTreeView.SelectedNode;
            if (st == null)
                return;

            SaveCurrentRule();

            history_.Save();

            TreeNode nt = st.NextNode;

            RulesTreeView.Nodes.Remove(st);

            RulesTreeView.SelectedNode = nt;
            RulesTreeView.Select();
            RestoreSelectedRule();
        }

        private void ReplaceSpecial_VisibleChanged(object sender, EventArgs e)
        {
            SaveCurrentRule();
        }

        private void ReplaceSpecial_Leave(object sender, EventArgs e)
        {
            SaveCurrentRule();
        }

        private void ReplaceSpecial_Deactivate(object sender, EventArgs e)
        {
            SaveCurrentRule();
        }


        public string ApplyRules(string text, string title)
        {
            foreach (TreeNode tn in RulesTreeView.Nodes)
            {
                IRule r = (IRule)tn.Tag;
                text = r.Apply(tn, text, title);
            }

            return text;
        }

        private void DeleteMenuItem_Click(object sender, EventArgs e)
        {
            DeleteCmd();
        }

        public void WriteToXml(XmlTextWriter w, bool enabled)
        {
            SaveCurrentRule();
            w.WriteStartElement(XmlName);

            w.WriteAttributeString("enabled", enabled.ToString());

            foreach (TreeNode tn in RulesTreeView.Nodes)
            {
                IRule r = (IRule)tn.Tag;
                r.WriteToXml(tn, w);
            }

            w.WriteEndElement();
        }

        public void ReadFromXml(XmlTextReader rd, ref bool enabled)
        {
            if (rd.Name == "FindAndReplaceSettings")
            {
                if (!rd.Read())
                    return;
            }

            if (rd.Name != ReplaceSpecial.XmlName)
                return;

            history_.Clear();

            RulesTreeView.Nodes.Clear();

            if (rd.MoveToAttribute("enabled"))
                enabled = Convert.ToBoolean(rd.Value);

            if (rd.Read())
                RuleFactory.ReadFromXml(RulesTreeView.Nodes, rd);

            if (RulesTreeView.Nodes.Count != 0)
                RulesTreeView.SelectedNode = RulesTreeView.Nodes[0];

            RulesTreeView.ExpandAll();
        }        

        private void CutMenuItem_Click(object sender, EventArgs e)
        {
            CutCmd();
        }

        void CutCmd()
        {
            TreeNode s = RulesTreeView.SelectedNode;
            if (s == null)
                return;
            SaveCurrentRule();
            clipboard_ = IRule.CloneTreeNode(s);
            DeleteCmd();
            RestoreSelectedRule();
        }

        private void CopyMenuItem_Click(object sender, EventArgs e)
        {
            CopyCmd();
        }

        void CopyCmd()
        {
            TreeNode s = RulesTreeView.SelectedNode;
            if (s == null)
                return;
            SaveCurrentRule();
            history_.Save();

            clipboard_ = IRule.CloneTreeNode(s);
            UpdateEnbabledStates();
        }

        private void PasteMenuItem_Click(object sender, EventArgs e)
        {
            PasteCmd();
        }

        void PasteCmd()
        {
            SaveCurrentRule();
            if (clipboard_ == null)
                return;
            history_.Save();
            TreeNode c = IRule.CloneTreeNode(clipboard_);
            TreeNode s = RulesTreeView.SelectedNode;
            if (s == null)
            {
                RulesTreeView.Nodes.Add(c);
            }
            else
            {
                TreeNodeCollection col = this.GetOwningNodes(s);
                col.Insert(col.IndexOf(s), c);
            }
            RulesTreeView.SelectedNode = c;
            RulesTreeView.Select();
            RestoreSelectedRule();
            RulesTreeView.ExpandAll();
        }

        private void NewRule()
        {
            AddNewRule(RuleFactory.CreateRule());
        }

        private void NewInTemplateRuleMenuItem_Click(object sender, EventArgs e)
        {
            AddNewRule(RuleFactory.CreateInTemplateRule());
        }

        private void NewTemplateParameterRuleMenuItem_Click(object sender, EventArgs e)
        {
            AddNewRule(RuleFactory.CreateTemplateParamRule());
        }

        private void AddNewRule(IRule r)
        {
            SaveCurrentRule();
            history_.Save();

            TreeNode n = new TreeNode(r.Name);
            n.Tag = r;

            TreeNode s = RulesTreeView.SelectedNode;
            if (s != null)
            {
                TreeNode p = s.Parent;
                if (p == null)
                    RulesTreeView.Nodes.Insert(RulesTreeView.Nodes.IndexOf(s) + 1, n);
                else
                    p.Nodes.Insert(p.Nodes.IndexOf(s) + 1, n);
            }
            else
            {
                RulesTreeView.Nodes.Add(n);
            }

            RulesTreeView.SelectedNode = n;
            RulesTreeView.Select();

            RestoreSelectedRule();
            this.currentRule_.SelectName();
        }

        void NewSubrule()
        {
            AddNewSubrule(RuleFactory.CreateRule());
        }

        void AddNewSubrule(IRule r)
        {
            SaveCurrentRule();

            TreeNode s = RulesTreeView.SelectedNode;
            if (s == null)
                return;

            history_.Save();

            TreeNode n = new TreeNode(r.Name);
            n.Tag = r;

            s.Nodes.Add(n);
            RulesTreeView.SelectedNode = n;
            RulesTreeView.Select();

            RestoreSelectedRule();
            this.currentRule_.SelectName();
        }

        private void NewSubruleInTemplateCallMenuItem_Click(object sender, EventArgs e)
        {
            AddNewSubrule(RuleFactory.CreateInTemplateRule());
        }

        private void NewSubruleTemplateParameterMenuItem_Click(object sender, EventArgs e)
        {
            AddNewSubrule(RuleFactory.CreateTemplateParamRule());
        }

        private void NewRuleMenuItem_Click(object sender, EventArgs e)
        {
            NewRule();
        }

        private void NewSubruleMenuItem_Click(object sender, EventArgs e)
        {
            NewSubrule();
        }

        private void UndoMenuItem_Click(object sender, EventArgs e)
        {
            SaveCurrentRule();
            history_.Undo();
            RestoreSelectedRule();
            RulesTreeView.ExpandAll();
        }

        private void RedoMenuItem_Click(object sender, EventArgs e)
        {
            SaveCurrentRule();
            history_.Redo();
            RestoreSelectedRule();
            RulesTreeView.ExpandAll();
        }

        private void ReplaceSpecial_KeyDown(object sender, KeyEventArgs e)
        {
            if (!RulesTreeView.Focused)
                return;

            if (e.KeyCode == Keys.Delete)
            {
                e.Handled = true;
                DeleteCmd();
                return;
            }

            if (!e.Control)
                return;

            e.Handled = true;
            switch (e.KeyCode)
            {
                case Keys.C:
                    CopyCmd(); break;
                case Keys.V:
                    PasteCmd(); break;
                case Keys.X:
                    CutCmd(); break;
                default:
                    e.Handled = false;
                    break;
            }

        }


        private void RulesTreeView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            DoDragDrop(e.Item, DragDropEffects.Move);
        }


        private void RulesTreeView_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.AllowedEffect;
        }


        private void RulesTreeView_DragOver(object sender, DragEventArgs e)
        {
            Point targetPoint = RulesTreeView.PointToClient(new Point(e.X, e.Y));

            TreeNode targetNode = RulesTreeView.GetNodeAt(targetPoint);
            RulesTreeView.SelectedNode = targetNode;

            TreeNode draggedNode = (TreeNode)e.Data.GetData(typeof(TreeNode));

            if (Utility.IsSubnodeOf(draggedNode, targetNode))
            {
                e.Effect = DragDropEffects.None;
                return;
            }

            e.Effect = DragDropEffects.Move;
        }


        private void RulesTreeView_DragDrop(object sender, DragEventArgs e)
        {
            Point targetPoint = RulesTreeView.PointToClient(new Point(e.X, e.Y));

            TreeNode targetNode = RulesTreeView.GetNodeAt(targetPoint);

            TreeNode draggedNode = (TreeNode)e.Data.GetData(typeof(TreeNode));

            if (Utility.IsSubnodeOf(draggedNode, targetNode))
            {
                e.Effect = DragDropEffects.None;
                return;
            }

            RulesTreeView.Nodes.Remove(draggedNode);

            targetNode.Nodes.Insert(0, draggedNode);

            RulesTreeView.SelectedNode = draggedNode;
            RestoreSelectedRule();

            e.Effect = DragDropEffects.Move;
        }

    }

}