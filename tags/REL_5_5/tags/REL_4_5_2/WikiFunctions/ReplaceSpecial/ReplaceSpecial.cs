/*
    Derived from Autowikibrowser
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
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Xml;
using System.Windows.Forms;

namespace WikiFunctions.ReplaceSpecial
{
    public partial class ReplaceSpecial : Form, IRuleControlOwner
    {
        #region contextmenu
        private void NewRuleContextMenuItem_Click(object sender, EventArgs e)
        {
            NewRule();
        }

        private void NewSubruleContextMenuItem_Click(object sender, EventArgs e)
        {
            NewSubrule();
        }

        private void CutMenuItem_Click(object sender, EventArgs e)
        {
            CutCmd();
        }

        private void CopyMenuItem_Click(object sender, EventArgs e)
        {
            CopyCmd();
        }

        private void PasteMenuItem_Click(object sender, EventArgs e)
        {
            PasteCmd();
        }

        #endregion

        IRule currentRule_;
        Control ruleControl_;
        readonly RuleTreeHistory history_;

        public void Clear()
        {
            RulesTreeView.Nodes.Clear();
            NewRule();
        }

        public ReplaceSpecial()
        {
            InitializeComponent();

            history_ = new RuleTreeHistory(RulesTreeView);

            //NewRule();
            UpdateEnabledStates();
            setTreeViewColours();
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            SaveCurrentRule();
            Hide();
        }

        private void ReplaceSpecial_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveCurrentRule();
            e.Cancel = true;
            Hide();
        }

        private void RulesTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (RulesTreeView.SelectedNode == null)
                return;

            SaveCurrentRule();
            RestoreSelectedRule();
            UpdateEnabledStates();
        }

        private void SaveCurrentRule()
        {
            IRule r = currentRule_;
            if (r == null)
                return;

            r.Save();
            setTreeViewColours();
        }

        private void RestoreSelectedRule()
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

                SuspendLayout();

                Point pos = new Point();
                ruleControl_ = null;
                ruleControl_ = currentRule_.CreateControl(this, RuleControlSpace.Controls, pos);
                ruleControl_.Size = RuleControlSpace.Size;

                currentRule_.Name = RulesTreeView.SelectedNode.Text;

                currentRule_.Restore();

                if (oldrule != null)
                {
                    if (!currentRule_.Equals(oldrule))
                        oldrule.DisposeControl();
                }

                ruleControl_.Visible = true;

                ResumeLayout();
            }
            UpdateEnabledStates();
            setTreeViewColours();
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
            //RulesTreeView.ExpandAll();
            RestoreSelectedRule();
        }

        private TreeNodeCollection GetOwningNodes(TreeNode t)
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
            //RulesTreeView.ExpandAll();
            RestoreSelectedRule();
        }

        private void NewRuleButton_Click(object sender, EventArgs e)
        {
            NewRule();
            setTreeViewColours();
        }

        private void NewSubruleButton_Click(object sender, EventArgs e)
        {
            NewSubrule();
            setTreeViewColours();
        }

        public void NameChanged(Control rc, string name)
        {
            if (RulesTreeView.SelectedNode == null)
                return;

            if (string.IsNullOrEmpty(name))
                return;

            if (RulesTreeView.SelectedNode.Text == name)
                return;

            RulesTreeView.SelectedNode.Text = name;
        }

        private void UpdateEnabledStates()
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

            PasteMenuItem.Enabled = PasteContextMenuItem.Enabled = true;

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

        private void DeleteCmd()
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
            setTreeViewColours();
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

        private void CutCmd()
        {
            if (RulesTreeView.SelectedNode == null)
                return;

            CopyCmd();
            DeleteCmd();
            RestoreSelectedRule();
        }

        private void CopyCmd()
        {
            if (RulesTreeView.SelectedNode == null)
                return;
            SaveCurrentRule();
            history_.Save();

            Tools.CopyToClipboard(Serialize(GetSelectedRule()), true);
            UpdateEnabledStates();
        }

        private void PasteCmd()
        {
            SaveCurrentRule();
            history_.Save();

            AddNewRule(Deserialize(Clipboard.GetDataObject().GetData(typeof(string)).ToString()));

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

        private static void RecurseNode(TreeNode n, IRule r)
        {
            if (n.Nodes.Count == 0) return;

            r.Children = new List<IRule>();
            foreach (TreeNode n1 in n.Nodes)
            {
                IRule r1 = (IRule)n1.Tag;
                if (n1.Nodes.Count > 0) RecurseNode(n1, r1);
                r.Children.Add(r1);
            }
        }

        public List<IRule> GetRules()
        {
            List<IRule> l = new List<IRule>();

            foreach (TreeNode tn in RulesTreeView.Nodes)
            {
                IRule r = (IRule)tn.Tag;
                if (tn.Nodes.Count > 0) RecurseNode(tn, r);
                l.Add(r);
            }

            return l;
        }

        public IRule GetSelectedRule()
        {
            TreeNode tn = RulesTreeView.SelectedNode;
            IRule r = (IRule)tn.Tag;
            if (tn.Nodes.Count > 0) RecurseNode(tn, r);

            return r;
        }

        public void AddNewRule(List<IRule> Rules)
        {
            RulesTreeView.Nodes.Clear();

            foreach (IRule r in Rules)
            {
                AppendRule(r);
            }

            RulesTreeView.ExpandAll();
        }

        private void AddNewRule(IRule r)
        {
            if (r == null)
                return;

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

            if (r.Children != null && r.Children.Count > 0) foreach (IRule rnew in r.Children) AddNewRule(rnew, n);
            else
            {
                RulesTreeView.SelectedNode = n;
                RulesTreeView.Select();
            }

            RestoreSelectedRule();
            currentRule_.SelectName();
        }

        private void AddNewRule(IRule r, TreeNode tn)
        {
            TreeNode n = new TreeNode(r.Name);
            n.Tag = r;

            tn.Nodes.Add(n);

            if (r.Children != null && r.Children.Count > 0) foreach (IRule rnew in r.Children) AddNewRule(rnew, n);
            else
            {
                RulesTreeView.SelectedNode = n;
                RulesTreeView.Select();
            }
        }

        private void AppendRule(IRule r)
        {
            TreeNode n = new TreeNode(r.Name);
            n.Tag = r;

            RulesTreeView.Nodes.Add(n);

            if (r.Children != null && r.Children.Count > 0) foreach (IRule rnew in r.Children) AddNewRule(rnew, n);
            else
            {
                RulesTreeView.SelectedNode = n;
                RulesTreeView.Select();
            }
        }

        private void NewSubrule()
        {
            AddNewSubrule(RuleFactory.CreateRule());
        }

        private void AddNewSubrule(IRule r)
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
            currentRule_.SelectName();
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
            //RulesTreeView.ExpandAll();
        }

        private void RedoMenuItem_Click(object sender, EventArgs e)
        {
            SaveCurrentRule();
            history_.Redo();
            RestoreSelectedRule();
            //RulesTreeView.ExpandAll();
        }

        private void refreshColoursToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setTreeViewColours();
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

            if (targetNode != null)
            {
                targetNode.Nodes.Insert(0, draggedNode);
            }
            else
            {
                RulesTreeView.Nodes.Add(draggedNode);
            }

            RulesTreeView.SelectedNode = draggedNode;
            RestoreSelectedRule();

            e.Effect = DragDropEffects.Move;
        }

        private void setTreeViewColours()
        {
            RulesTreeView.BeginUpdate();
            foreach (TreeNode node in RulesTreeView.Nodes)
            {
                setColours(node);
            }
            RulesTreeView.EndUpdate();
        }

        private void setColours(TreeNode rnode)
        {
            IRule temp = (IRule)rnode.Tag;
            setNodeColour(rnode, temp);

            foreach (TreeNode node in rnode.Nodes)
            {
                IRule temp2 = (IRule)node.Tag;

                setNodeColour(node, temp2);
                setColours(node);
            }
        }

        private static void setNodeColour(TreeNode node, IRule rule)
        {
            node.BackColor = rule.enabled_ ? Color.White : Color.Red;
        }

        private void ReplaceSpecial_Load(object sender, EventArgs e)
        {
            setTreeViewColours();
        }

        private void expandAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Collapsed(false);
        }

        private void collapseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Collapsed(true);
        }

        private void Collapsed(bool collapsed)
        {
            RulesTreeView.BeginUpdate();
            foreach (TreeNode node in RulesTreeView.Nodes)
            {
                if (collapsed) node.Collapse();
                else node.ExpandAll();
            }
            RulesTreeView.EndUpdate();
        }

        #region Serialize/Deserialize for Clipboard work
        //Base code from http://www.dotnetjohn.com/articles.aspx?articleid=173

        private string Serialize(IRule rule)
        {
            System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();
            System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(IRule));
            XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
            xs.Serialize(xmlTextWriter, rule);
            memoryStream = (System.IO.MemoryStream)xmlTextWriter.BaseStream;

            return UTF8ByteArrayToString(memoryStream.ToArray());
        }

        public IRule Deserialize(string pXmlizedString)
        {
            if (!pXmlizedString.Contains("<?xml"))
                return null;

            System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(IRule));
            System.IO.MemoryStream memoryStream = new System.IO.MemoryStream(StringToUTF8ByteArray(pXmlizedString));

            return (IRule)xs.Deserialize(memoryStream);
        }

        private static string UTF8ByteArrayToString(byte[] characters)
        {
            return (new UTF8Encoding().GetString(characters));
        }

        private static byte[] StringToUTF8ByteArray(string pXmlString)
        {
            return (new UTF8Encoding().GetBytes(pXmlString));
        }
        #endregion
    }
}