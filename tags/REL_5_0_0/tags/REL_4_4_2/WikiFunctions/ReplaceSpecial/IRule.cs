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
using System.Collections.Generic;
using System.Windows.Forms;

namespace WikiFunctions.MWB
{
    [System.Xml.Serialization.XmlInclude(typeof(Rule))]
    [System.Xml.Serialization.XmlInclude(typeof(TemplateParamRule))]
    [System.Xml.Serialization.XmlInclude(typeof(InTemplateRule))]
    public abstract class IRule : ICloneable
    {
        private string name_ = "";
        public bool enabled_ = true;

        public List<IRule> Children;

        public string Name
        {
            set { name_ = value; }
            get { return name_; }
        }

        public abstract Control GetControl();
        public abstract void ForgetControl();

        public abstract void SelectName();

        public abstract void Save();
        public abstract void Restore();

        public abstract Control CreateControl(IRuleControlOwner owner, Control.ControlCollection collection, System.Drawing.Point pos);

        public void DisposeControl()
        {
            Control old = GetControl();
            if (old == null)
                return;
            ForgetControl();
            old.Hide();
            if (old.Parent != null)
                old.Parent.Controls.Remove(old);
            old.Dispose();
        }

        public abstract string Apply(TreeNode tn, string text, string title);

        public abstract Object Clone();

        public static TreeNode CloneTreeNode(TreeNode tn)
        {
            if (tn == null)
                return null;
            TreeNode res = (TreeNode)tn.Clone();
            CloneTags(res);
            return res;
        }

        private static void CloneTags(TreeNode tn)
        {
            IRule r = (IRule)tn.Tag;
            tn.Tag = r.Clone();
            foreach (TreeNode t in tn.Nodes)
            {
                CloneTags(t);
            }
        }
    }
}
