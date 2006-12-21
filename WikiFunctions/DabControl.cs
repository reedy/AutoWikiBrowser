using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace WikiFunctions
{
    public partial class DabControl : UserControl
    {
        public DabControl()
        {
            InitializeComponent();
        }

        // input data
        public string ArticleTitle;
        public string ArticleText;
        public Match Match;
        public List<string> Variants;

        // output data
        public string Surroundings;
        public int SurroundingsStart = 0;
        public string Result;

        //internal
        bool StartOfSentence = false;

        public DabControl(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        //public void 

        private void DabControl_Load(object sender, EventArgs e)
        {
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right ;
        }
    }
}
