using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace WikiFunctions.MWB
{
    public class RuleTreeHistory
    {
        List<List<TreeNode>> history_ = new List<List<TreeNode>>();
        int index_ = -1;

        TreeView treeView_ = null;


        public RuleTreeHistory(TreeView tv)
        {
            treeView_ = tv;
        }

        public void Clear()
        {
            history_.Clear();
            index_ = -1;
        }

        public void Save()
        {
            if (index_ != -1)
            {
                Clear();
                index_ = -1;
            }
            InternalSave();
        }

        void InternalSave()
        {
            List<TreeNode> cp = Copy(treeView_.Nodes);
            history_.Insert(0, cp);
        }

        public bool CanUndo
        {
            get
            {
                return (history_.Count > 0) && (index_ == -1 || index_ + 1 < history_.Count);
            }
        }

        public void Undo()
        {
            if (!CanUndo)
                return;

            if (index_ == -1)
            {
                InternalSave();
                index_ = 1;
            }
            else
            {
                ++index_;
            }

            Restore();
        }

        public bool CanRedo { get { return (history_.Count > 0) && (index_ > 0); } }

        public void Redo()
        {
            if (!CanRedo)
                return;
            --index_;
            Restore();
        }

        void Restore()
        {
            treeView_.Nodes.Clear();

            List<TreeNode> hcol = history_[index_];

            foreach (TreeNode t in hcol)
            {
                TreeNode copy_t = (TreeNode)t.Clone();
                treeView_.Nodes.Add(copy_t);
                UpdateNames(copy_t);
            }
        }

        static void UpdateNames(TreeNode t)
        {
            if (t == null)
                return;
            IRule r = (IRule)t.Tag;
            t.Text = r.Name;
            foreach (TreeNode sub_t in t.Nodes)
                UpdateNames(sub_t);
        }

        static List<TreeNode> Copy(TreeNodeCollection col)
        {
            List<TreeNode> new_col = new List<TreeNode>();
            foreach (TreeNode t in col)
            {
                TreeNode copy_t = (TreeNode)t.Clone();
                new_col.Add(copy_t);
            }
            return new_col;
        }
    }
}
