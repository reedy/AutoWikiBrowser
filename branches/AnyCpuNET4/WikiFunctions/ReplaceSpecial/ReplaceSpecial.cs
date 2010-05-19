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
    //TODO: Use IArticleComparer derivatives where possible

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

        IRule CurrentRule;
        Control ruleControl_;
        private readonly RuleTreeHistory History;

        public void Clear()
        {
            if (NoOfRules > 0)
                RulesTreeView.Nodes.Clear();
        }

        public ReplaceSpecial()
        {
            InitializeComponent();

            History = new RuleTreeHistory(RulesTreeView);

            UpdateEnabledStates();
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
            if (CurrentRule == null)
                return;

            CurrentRule.Save();
            SetTreeViewColours();
        }

        private void RestoreSelectedRule()
        {
            if (RulesTreeView.SelectedNode == null)
            {
                if (CurrentRule != null)
                {
                    CurrentRule.DisposeControl();
                    CurrentRule = null;
                    NoRuleSelectedLabel.Show();
                }
            }
            else
            {
                NoRuleSelectedLabel.Hide();

                IRule oldrule = CurrentRule;

                CurrentRule = (IRule)RulesTreeView.SelectedNode.Tag;

                SuspendLayout();

                ruleControl_ = CurrentRule.CreateControl(this, RuleControlSpace.Controls, new Point());
                ruleControl_.Size = RuleControlSpace.Size;

                CurrentRule.Name = RulesTreeView.SelectedNode.Text;

                CurrentRule.Restore();

                if (oldrule != null)
                {
                    if (!CurrentRule.Equals(oldrule))
                        oldrule.DisposeControl();
                }

                ruleControl_.Visible = true;

                ResumeLayout();
            }
            UpdateEnabledStates();
            SetTreeViewColours();
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

            History.Save();

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
            return p != null ? p.Nodes : RulesTreeView.Nodes;
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

            History.Save();

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
            SetTreeViewColours();
        }

        private void NewSubruleButton_Click(object sender, EventArgs e)
        {
            NewSubrule();
            SetTreeViewColours();
        }

        public void NameChanged(Control rc, string name)
        {
            if (RulesTreeView.SelectedNode == null
                || string.IsNullOrEmpty(name)
                || RulesTreeView.SelectedNode.Text == name)
                return;

            RulesTreeView.SelectedNode.Text = name;
        }

        private void UpdateEnabledStates()
        {
            bool hasSelection = RulesTreeView.SelectedNode != null;

            if (ruleControl_ != null)
                ruleControl_.Enabled = hasSelection;

            DeleteButton.Enabled = hasSelection;
            UpButton.Enabled = hasSelection;
            DownButton.Enabled = hasSelection;
            NewSubruleButton.Enabled = hasSelection;

            DeleteMenuItem.Enabled = hasSelection;
            DeleteContextMenuItem.Enabled = hasSelection;

            NewSubruleMenu.Enabled = hasSelection;
            NewSubruleContextMenuItem.Enabled = hasSelection;

            NewSubruleMenuItem.Enabled = hasSelection;
            NewSubruleInTemplateCallMenuItem.Enabled = hasSelection;
            NewSubruleTemplateParameterMenuItem.Enabled = hasSelection;

            PasteMenuItem.Enabled = PasteContextMenuItem.Enabled = true;

            CutMenuItem.Enabled = hasSelection;
            CutContextMenuItem.Enabled = hasSelection;

            CopyMenuItem.Enabled = hasSelection;
            CopyContextMenuItem.Enabled = hasSelection;

            UndoMenuItem.Enabled = History.CanUndo;
            RedoMenuItem.Enabled = History.CanRedo;
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

            History.Save();

            TreeNode nt = st.NextNode;

            RulesTreeView.Nodes.Remove(st);

            RulesTreeView.SelectedNode = nt;
            RulesTreeView.Select();
            RestoreSelectedRule();
            SetTreeViewColours();
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

        /// <summary>
        /// 
        /// </summary>
        public int NoOfRules { get { return RulesTreeView.Nodes.Count; } }

        /// <summary>
        /// 
        /// </summary>
        public bool HasRules { get { return NoOfRules != 0; } }

        /// <summary>
        /// Applys the Replace Special Rules
        /// </summary>
        /// <param name="text">Article title</param>
        /// <param name="title">Article text</param>
        /// <returns>Amended text</returns>
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
            History.Save();

            Tools.CopyToClipboard(Serialize(GetSelectedRule()), true);
            UpdateEnabledStates();
        }

        private void PasteCmd()
        {
            SaveCurrentRule();
            History.Save();

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

        public void AddNewRule(List<IRule> rules)
        {
            RulesTreeView.Nodes.Clear();

            foreach (IRule r in rules)
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
            History.Save();

            TreeNode n = new TreeNode(r.Name) {Tag = r};

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

            if (r.Children != null && r.Children.Count > 0) 
                foreach (IRule rnew in r.Children) 
                    AddNewRule(rnew, n);
            else
            {
                RulesTreeView.SelectedNode = n;
                RulesTreeView.Select();
            }

            RestoreSelectedRule();
            CurrentRule.SelectName();
        }

        private void AddNewRule(IRule r, TreeNode tn)
        {
            TreeNode n = new TreeNode(r.Name) {Tag = r};

            tn.Nodes.Add(n);

            if (r.Children != null && r.Children.Count > 0) 
                foreach (IRule rnew in r.Children) 
                    AddNewRule(rnew, n);
            else
            {
                RulesTreeView.SelectedNode = n;
                RulesTreeView.Select();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="r"></param>
        private void AppendRule(IRule r)
        {
            TreeNode n = new TreeNode(r.Name) {Tag = r};

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

            History.Save();

            TreeNode n = new TreeNode(r.Name) {Tag = r};

            s.Nodes.Add(n);
            RulesTreeView.SelectedNode = n;
            RulesTreeView.Select();

            RestoreSelectedRule();
            CurrentRule.SelectName();
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
            History.Undo();
            RestoreSelectedRule();
        }

        private void RedoMenuItem_Click(object sender, EventArgs e)
        {
            SaveCurrentRule();
            History.Redo();
            RestoreSelectedRule();
        }

        private void refreshColoursToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetTreeViewColours();
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

            if (Tools.IsSubnodeOf(draggedNode, targetNode))
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

            if (Tools.IsSubnodeOf(draggedNode, targetNode))
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

        private void SetTreeViewColours()
        {
            RulesTreeView.BeginUpdate();
            foreach (TreeNode node in RulesTreeView.Nodes)
            {
                SetColours(node);
            }
            RulesTreeView.EndUpdate();
        }

        private static void SetColours(TreeNode rnode)
        {
            IRule temp = (IRule)rnode.Tag;
            SetNodeColour(rnode, temp);

            foreach (TreeNode node in rnode.Nodes)
            {
                IRule temp2 = (IRule)node.Tag;

                SetNodeColour(node, temp2);
                SetColours(node);
            }
        }

        private static void SetNodeColour(TreeNode node, IRule rule)
        {
            node.BackColor = rule.enabled_ ? Color.White : Color.Red;
        }

        private void ReplaceSpecial_Load(object sender, EventArgs e)
        {
            SetTreeViewColours();
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