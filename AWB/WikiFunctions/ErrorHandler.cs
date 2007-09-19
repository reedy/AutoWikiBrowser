using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Reflection;

namespace WikiFunctions
{
    /// <summary>
    /// This class provides helper functions for handling errors and displaying them to users
    /// </summary>
    public partial class ErrorHandler : Form
    {
        /// <summary>
        /// Title of the page currently being processed
        /// </summary>
        public static string CurrentArticle;

        /// <summary>
        /// Language of the project of the page that is currently being processed
        /// </summary>
        public static string LangCode;

        /// <summary>
        /// Project of which the page is currently being processed
        /// </summary>
        public static string Project;

        /// <summary>
        /// Revision of the page currently being processed
        /// </summary>
        public static int CurrentRevision;

        public ErrorHandler()
        {
            InitializeComponent();
        }

        private void ErrorHandler_Load(object sender, EventArgs e)
        {
            Text = Application.ProductName;
        }

        /// <summary>
        /// Displays exception information. Should be called from try...catch handlers
        /// </summary>
        /// <param name="ex">Exception object to handle</param>
        new public static void Handle(Exception ex)
        {
            ErrorHandler handler = new ErrorHandler();

            // handle invalid regexes
            if (ex.GetType().ToString().Equals("System.ArgumentException") && ex.StackTrace.Contains("System.Text.RegularExpressions"))
            {
                MessageBox.Show(ex.Message, "Invalid regular expression");
            }
            else // other exceptions
            {
                handler.txtError.Text = ex.Message;

                handler.txtDetails.Text = "{{AWB bug\r\n | status      = new <!-- when fixed replace with \"fixed\" -->\r\n | description = <table><tr><td>Exception:<td><code>" + ex.GetType().Name + "</code><tr><td>Message:<td><code>" +
                    ex.Message + "</code><tr><td>Call stack:<td><pre>" + ex.StackTrace + "</pre></table>\r\n~~~~\r\n | OS          = " + Environment.OSVersion.ToString() + "\r\n | version     = " + Assembly.GetExecutingAssembly().GetName().Version.ToString();

                if (CurrentArticle != null && CurrentArticle != "" &&
                    ex.StackTrace.Contains("AutoWikiBrowser.MainForm.ProcessPage("))
                {
                    string link;
                    if (CurrentRevision != 0 && LangCode != "" && Project != "")
                        link = "[http://" + LangCode + "." + Project + ".org/w/index.php?title=" + CurrentArticle + "&oldid=" + CurrentRevision.ToString() + "]";
                    else link = "[[:" + CurrentArticle + "]]";
                    handler.txtDetails.Text +=
                        "\r\n | duplicate = [encountered while processing page ''" + link + "'']";
                }

                handler.txtDetails.Text += "\r\n}}";

                /*
                foreach (StackFrame frame in ex.StackTrace.)
                {
                    string s = "\r\n    " + frame.GetMethod();
                    if (frame.GetFileLineNumber() > 0) s += "(line " + frame.GetFileLineNumber() + ")";
                    Handler.txtDetails.Text += s;
                }*/

                handler.ShowDialog();
            }
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            Clipboard.Clear();
            System.Threading.Thread.Sleep(1000);
            Clipboard.SetText(txtDetails.Text);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkLabel1.LinkVisited = true;
            try
            {
                System.Diagnostics.Process.Start("http://en.wikipedia.org/w/index.php?title=Wikipedia_talk:AutoWikiBrowser/Bugs&action=edit&section=new");
            }
            catch { }
        }
    }
}