using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace WikiFunctions
{
    public partial class ListBox2 : ListBox
    {
        public ListBox2()
        {
            InitializeComponent();
        }

        public System.Collections.Generic.IEnumerable<Article> Enumerate()
        {
            int i = 0;
            while (i < this.Items.Count)
            {
                yield return (Article)this.Items[i];
                i++;
            }
        }
    }
}
