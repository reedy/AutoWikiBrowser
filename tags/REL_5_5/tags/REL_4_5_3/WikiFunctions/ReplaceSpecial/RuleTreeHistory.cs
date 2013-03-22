/*

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

using System.Collections.Generic;
using System.Windows.Forms;

namespace WikiFunctions.ReplaceSpecial
{
    public class RuleTreeHistory
    {
        readonly List<List<TreeNode>> history_ = new List<List<TreeNode>>();
        int index_ = -1;

        readonly TreeView treeView_;

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

        private void InternalSave()
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

        private void Restore()
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

        private static void UpdateNames(TreeNode t)
        {
            if (t == null)
                return;
            IRule r = (IRule)t.Tag;
            t.Text = r.Name;
            foreach (TreeNode sub_t in t.Nodes)
                UpdateNames(sub_t);
        }

        private static List<TreeNode> Copy(TreeNodeCollection col)
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
