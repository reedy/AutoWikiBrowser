using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Reflection;
using System.Threading;

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

        /// <summary>
        /// Current text that the list is being made from in ListMaker
        /// </summary>
        public static string ListMakerText;

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
            if (ex != null)
            {
                // invalid regex - only ArgumentException, without subclasses
                if (ex.GetType().ToString().Equals("System.ArgumentException")
                    && ex.StackTrace.Contains("System.Text.RegularExpressions"))
                {
                    MessageBox.Show(ex.Message, "Invalid regular expression",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                // network access error
                else if (ex is System.Net.WebException)
                {
                    MessageBox.Show(ex.Message, "Network access error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                // out of memory error
                else if (ex is System.OutOfMemoryException)
                {
                    MessageBox.Show(ex.Message, "Out of Memory error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else // suggest a bug report for other exceptions
                {
                    ErrorHandler handler = new ErrorHandler();

                    handler.txtError.Text = ex.Message;

                    handler.txtDetails.Text = "{{AWB bug\r\n | status      = new <!-- when fixed replace with \"fixed\" -->\r\n | description = <table><tr><td>Exception:<td><code>" + ex.GetType().Name + "</code><tr><td>Message:<td><code>" +
                        ex.Message + "</code><tr><td>Call stack:<td><pre>" + ex.StackTrace + "</pre>\r\nThread:" + Thread.CurrentThread.Name + "</table>\r\n~~~~\r\n | OS          = " + Environment.OSVersion.ToString() + "\r\n | version     = " + Assembly.GetExecutingAssembly().GetName().Version.ToString();

                    if (!string.IsNullOrEmpty(CurrentArticle) &&
                        ex.StackTrace.Contains("AutoWikiBrowser.MainForm.ProcessPage("))
                    {
                        string link;
                        if (CurrentRevision != 0 && !string.IsNullOrEmpty(LangCode) && !string.IsNullOrEmpty(Project))
                            link = "[http://" + LangCode + "." + Project + ".org/w/index.php?title=" + CurrentArticle.Replace(" ", "_") + "&oldid=" + CurrentRevision.ToString() + "]";
                        else link = "[[:" + CurrentArticle + "]]";
                        handler.txtDetails.Text +=
                            "\r\n | duplicate = [encountered while processing page ''" + link + "'']";
                    }

                    if (!string.IsNullOrEmpty(ListMakerText))
                        handler.txtDetails.Text +=
                            "\r\n | duplicate = '''ListMaker Text:''' " + ListMakerText;

                    handler.txtDetails.Text += "\r\n}}";

                    handler.textBox1.Text = "AWB encountered " + ex.GetType().Name;

                    handler.ShowDialog();
                }
            }
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.Clear();
                System.Threading.Thread.Sleep(1000);
                Clipboard.SetText(txtDetails.Text);
            }
            catch { }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkLabel1.LinkVisited = true;
            Tools.OpenURLInBrowser("http://en.wikipedia.org/w/index.php?title=Wikipedia_talk:AutoWikiBrowser/Bugs&action=edit&section=new");
        }
    }
}
