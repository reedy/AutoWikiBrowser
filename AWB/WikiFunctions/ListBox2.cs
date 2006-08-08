using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace WikiFunctions
{
    public partial class ListBox2 : ListBox, IEnumerable<Article>
    {
        public ListBox2()
        {
            InitializeComponent();
        }

        public IEnumerator<Article> GetEnumerator()
        {
            int i = 0;
            while (i < this.Items.Count)
            {
                yield return (Article)this.Items[i];
                i++;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
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
