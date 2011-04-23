/*
Autowikibrowser
Copyright (C) 2008 Max Semenik, Stephen Kennedy

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
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Threading;
using System.Diagnostics;

namespace WikiFunctions.Controls
{
    public sealed partial class RegexTester : Form
    {
        private readonly Regex NewLineRegex = new Regex("\n", RegexOptions.Compiled);
        private readonly Regex CRNewLineRegex = new Regex("\r\n", RegexOptions.Compiled);

        RegexRunner Runner;

        public RegexTester()
        {
            InitializeComponent();
        }

        public RegexTester(bool ask)
            :this()
        {
            AskToApply = ask;
        }

        public static void Test(Form parent, TextBox find, TextBox replace, 
            CheckBox multiline, CheckBox singleline, CheckBox caseSensitive)
        {
            using (RegexTester t = new RegexTester(true))
            {
                t.Find = find.Text;
                if (replace != null) t.Replace = replace.Text;
                t.Multiline = multiline.Checked;
                t.Singleline = singleline.Checked;
                t.IgnoreCase = !caseSensitive.Checked;

                if (Variables.MainForm != null && Variables.MainForm.EditBox.Enabled)
                    t.ArticleText = Variables.MainForm.EditBox.Text;

                if (t.ShowDialog(parent) == DialogResult.OK)
                {
                    find.Text = t.Find;
                    if (replace != null) replace.Text = t.Replace;
                    multiline.Checked = t.Multiline;
                    singleline.Checked = t.Singleline;
                    caseSensitive.Checked = !t.IgnoreCase;
                }
            }
        }

        #region Properties for external access

        public string ArticleText
        {
            set { txtInput.Text = value; }
        }

        public string Find
        {
            get { return txtFind.Text; }
            set { txtFind.Text = value; }
        }

        public string Replace
        {
            get { return txtReplace.Text; }
            set { txtReplace.Text = value; }
        }

        public RegexOptions RegexOptions
        {
            get
            {
                RegexOptions res = RegexOptions.None;
                if (chkMultiline.Checked) res |= RegexOptions.Multiline;
                if (chkSingleline.Checked) res |= RegexOptions.Singleline;
                if (chkIgnoreCase.Checked) res |= RegexOptions.IgnoreCase;
                if (chkExplicitCapture.Checked) res |= RegexOptions.ExplicitCapture;

                return res;
            }

            set
            {
                chkMultiline.Checked = (value & RegexOptions.Multiline) != 0;
                chkSingleline.Checked = (value & RegexOptions.Singleline) != 0;
                chkIgnoreCase.Checked = (value & RegexOptions.IgnoreCase) != 0;
                chkExplicitCapture.Checked = (value & RegexOptions.ExplicitCapture) != 0;
            }
        }

        public bool Multiline
        {
            get { return chkMultiline.Checked; }
            set { chkMultiline.Checked = value; }
        }

        public bool Singleline
        {
            get { return chkSingleline.Checked; }
            set { chkSingleline.Checked = value; }
        }

        public bool IgnoreCase
        {
            get { return chkIgnoreCase.Checked; }
            set { chkIgnoreCase.Checked = value; }
        }

        public bool ExplicitCapture
        {
            get { return chkExplicitCapture.Checked; }
            set { chkExplicitCapture.Checked = value; }
        }

        public readonly bool AskToApply;
        #endregion

        bool Busy
        {
            get
            {
                return progressBar.Style == ProgressBarStyle.Marquee;
            }
            set
            {
                if (value)
                {
                    progressBar.Style = ProgressBarStyle.Marquee;
                    progressBar.MarqueeAnimationSpeed = 100;
                    Status.Text = "Processing (ESC to cancel)";
                    FindBtn.Enabled = ReplaceBtn.Enabled = false;
                }
                else
                {
                    progressBar.Style = ProgressBarStyle.Blocks;
                    progressBar.MarqueeAnimationSpeed = 0;
                    Status.Text = "";
                    ConditionsChanged(null, null); // update buttons
                }
            }
        }

        private void ConditionsChanged(object sender, EventArgs e)
        {
            bool enabled = (!string.IsNullOrEmpty(txtFind.Text) && !string.IsNullOrEmpty(txtInput.Text));
            ReplaceBtn.Enabled = (!string.IsNullOrEmpty(txtReplace.Text) && enabled);
            FindBtn.Enabled = enabled;
        }
        
        private void KeyPressHandler(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case (char)1: // CTRL+A
                    {
                        TextBox text = (sender as TextBox);

                        if (text != null)
                            text.SelectAll();
                        break;
                    }
                case (char)27:
                    AbortProcessing();
                    break;
            }
        }

        private RegexOptions Options
        {
            get
            {
                RegexOptions res = RegexOptions.None;
                if (chkMultiline.Checked) res |= RegexOptions.Multiline;
                if (chkSingleline.Checked) res |= RegexOptions.Singleline; 
                if (chkIgnoreCase.Checked) res |= RegexOptions.IgnoreCase;
                if (chkExplicitCapture.Checked) res |= RegexOptions.ExplicitCapture;

                return res;
            }
        }

        private void Replace_Click(object sender, EventArgs e)
        {
            try
            {
                Captures.Nodes.Clear();
                ResultText.Text = "";
                Status.Text = "";
                txtInput.Text = CRNewLineRegex.Replace(txtInput.Text, "\n");
                Busy = true;

                Regex r = new Regex(txtFind.Text, Options);

                Runner = new RegexRunner(this, txtInput.Text, CRNewLineRegex.Replace(txtReplace.Text, "\n"), r);
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
                Busy = false;
            }
        }

        private void RegexTester_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 27) // Escape key
            {
                e.Handled = true;

                if (Busy) AbortProcessing();
                else Close();
            }
        }

        private void FindBtn_Click(object sender, EventArgs e)
        {
            try
            {
                Captures.Nodes.Clear();
                Captures.Visible = true;
                ResultText.Text = "";
                ResultText.Visible = false;
                Status.Text = "";
                Busy = true;

                Regex r = new Regex(txtFind.Text, Options);

                Runner = new RegexRunner(this, txtInput.Text, r);
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
                Busy = false;
            }
        }

        private string ReplaceNewLines(string str)
        { // Display line breaks as \n in the results tree so that they're clear
            return NewLineRegex.Replace(str, "\\n");
        }

        private static void ErrorHandler(Exception ex)
        {
            MessageBox.Show(ex.Message, "Whoops!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void RegexTester_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            RegexTesterHelpRequested();
        }

        private static void RegexTesterHelpRequested()
        {
            Tools.OpenURLInBrowser("http://msdn2.microsoft.com/en-us/library/az24scfc.aspx");
        }

        private void RegexTester_FormClosing(object sender, FormClosingEventArgs e)
        {
            AbortProcessing();
            if(e.CloseReason != CloseReason.UserClosing || !AskToApply) return;

            switch (MessageBox.Show(this, "Do you want to apply your changes?", Text, MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question))
            {
                case DialogResult.Yes:
                    DialogResult = DialogResult.OK;
                    break;
                case DialogResult.No:
                    DialogResult = DialogResult.Cancel;
                    break;
                case DialogResult.Cancel:
                    e.Cancel = true;
                    break;
            }
        }

        internal void RegexRunnerFinished(RegexRunner sender)
        {
            Busy = false;
            Runner = null;

            if (sender.Error != null)
            {
                Status.Text = "Error encountered during processing";
                MessageBox.Show(this, sender.Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(sender.Replace)) // find
            {
                foreach (Match m in sender.Matches)
                {
                    TreeNode n = Captures.Nodes.Add("{" + ReplaceNewLines(m.Value) + "}");
                    foreach (Group g in m.Groups)
                    {
                        if (g.Captures.Count > 1)
                        {
                            TreeNode nn = n.Nodes.Add("...");
                            foreach (Capture c in g.Captures)
                                nn.Nodes.Add("{" + ReplaceNewLines(c.Value) + "}");
                        }
                        else if (g.Captures.Count == 1)
                            n.Nodes.Add(ReplaceNewLines("{" + g.Captures[0].Value) + "}");
                    }
                }
                switch (sender.Matches.Count)
                {
                    case 0:
                        Status.Text = "No matches";
                        break;
                    case 1:
                        Status.Text = "1 match found";
                        break;
                    default:
                        Status.Text = sender.Matches.Count + " matches found";
                        break;
                }

                Status.Text += " in " + sender.GetExecutionTime() + " ms";

                Captures.ExpandAll();
            }
            else // replace
            {
                ResultText.Text = sender.Result;
                if (sender.Matches.Count != 1)
                    Status.Text = sender.Matches.Count + " replacements performed";
                else
                    Status.Text = "1 replacement performed";

                Status.Text += " in " + sender.GetExecutionTime() + " ms";
                
                Captures.Visible = false;
                ResultText.Visible = true;

                txtInput.Text = NewLineRegex.Replace(txtInput.Text, "\r\n");
                ResultText.Text = ResultText.Text.Replace("\r\n", "\n");
                ResultText.Text = NewLineRegex.Replace(ResultText.Text, "\r\n");
            }
        }

        private void AbortProcessing()
        {
            Busy = false;
            if (Runner == null) return;

            Runner.Abort();
            Runner = null;
            Status.Text = "Processing aborted";
        }
    }

    internal delegate void RegexRunnerFinishedDelegate(RegexRunner sender);

    internal class RegexRunner
    {
        // in
        public readonly string Input;
        public readonly string Replace;
        public readonly Regex _Regex;

        // out
        public string Result;
        public MatchCollection Matches;
        public Exception Error;

        // private
        readonly Thread Thr;
        readonly RegexTester Parent;

        private long ExecutionTime;

        public long GetExecutionTime()
        {
            return ExecutionTime;
        }

        public RegexRunner(RegexTester parent, string input, Regex regex)
            : this(parent, input, null, regex)
        { }

        public RegexRunner(RegexTester parent, string input, string replace, Regex regex)
        {
            Parent = parent;
            Input = input;
            Replace = replace;
            _Regex = regex;

            Thr = new Thread(ThreadFunc)
                      {
                          Priority = ThreadPriority.BelowNormal, 
                          Name = "RegexRunner"
                      };
            Thr.Start();
        }

        public void Abort()
        {
            if (Thr != null)
                Thr.Abort();
        }

        private void ThreadFunc()
        {
            try
            {
                Stopwatch sw = Stopwatch.StartNew();

                Matches = _Regex.Matches(Input);

                foreach (Match m in Matches)
                { }// force matches to actually run

                if (!string.IsNullOrEmpty(Replace))
                    Result = _Regex.Replace(Input, Replace);

                ExecutionTime = sw.ElapsedMilliseconds;
            }
            catch (Exception ex)
            {
                Error = ex;
            }
            finally
            {
                if (Error == null || !(Error is ThreadAbortException))
                    Parent.Invoke(new RegexRunnerFinishedDelegate(Parent.RegexRunnerFinished), this);
            }
        }
    }
}

/*
 * To test regex hangup:

 * Check IgnoreCase, Singleline and ExplicitCapture

 * Find:

\{\{\s*(?<tl>template *:)?\s*(?<tlname>WikiProject ?Banner ?Shell|WPBS)\b\s*(?<start>\|[^1]*=.*)*\s*\|\s*1\s*=\s*(?<body>.*}}[^{]*?)\s*(?<end>\|[^{]*)?\s*}}

 * Input:

{{WikiProjectBannerShell |blp=yes |1=
{{WikiProject Illinois |class=Start |nested=yes |importance=Low}}
{{ChicagoWikiProject |class=Start |importance=Low |nested=yes}}
{{Baseball-WikiProject |class=start |nested=yes}}
{{WikiProject Boston Red Sox
 |class=Start
 |importance=Low
 |needs-infobox=No
 |needs-photo=Yes
 |attention=
 |auto=
 |nested=yes
}}
{{WPBiography|living=no|class=Start|priority=|sports-work-group=yes|listas=Magadan, Dave|nested=yes|activepol=yes|non-bio=yes|politician-work-group=yes}}
{{WikiProject Texas |class=Start |importance=Low |nested=yes}}
}}
*/
