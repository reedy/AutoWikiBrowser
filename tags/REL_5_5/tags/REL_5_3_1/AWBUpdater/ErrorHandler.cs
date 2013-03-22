using System;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Threading;
using System.Text.RegularExpressions;

//////////////////////////////////////////////////////////////////////////////////////////////
// Don't use anything WikiFunctions-specific here, for source-compatibility with Updater  ///
//////////////////////////////////////////////////////////////////////////////////////////////

namespace AwbUpdater
{
    /// <summary>
    /// This class provides helper functions for handling errors and displaying them to users
    /// </summary>
    public partial class ErrorHandler : Form
    {
        /// <summary>
        /// Displays exception information. Should be called from try...catch handlers
        /// </summary>
        /// <param name="ex">Exception object to handle</param>
        new public static void Handle(Exception ex)
        {
            if (ex == null) return;

            if (ex is System.Net.WebException)
            {
                MessageBox.Show(ex.Message, "Network access error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
                // out of memory error
            else if (ex is OutOfMemoryException)
            {
                MessageBox.Show(ex.Message, "Out of Memory error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else // suggest a bug report for other exceptions
            {
                ErrorHandler handler = new ErrorHandler {txtError = {Text = ex.Message}};

                StringBuilder errorMessage = new StringBuilder("{{AWB bug\r\n | status      = new <!-- when fixed replace with \"fixed\" -->\r\n | description = ");

                if (Thread.CurrentThread.Name != "Main thread")
                    errorMessage.AppendLine("Thread: " + Thread.CurrentThread.Name);

                errorMessage.Append("<table>");
                FormatException(ex, errorMessage, ExceptionKind.TopLevel);
                errorMessage.AppendLine("</table>\r\n~~~~");
                errorMessage.AppendLine(" | OS          = " + Environment.OSVersion);

                AssemblyName hostingApp = Assembly.GetExecutingAssembly().GetName();

                errorMessage.Append(string.Format(" | version     = {0} ({1}), {2} ({3})", Application.ProductName,
                                                  Application.ProductVersion,
                                                  hostingApp.Name, hostingApp.Version));

                errorMessage.AppendLine(" | net = " + Environment.Version);
                errorMessage.AppendLine(" | workaround     = <!-- Any workaround for the problem -->");
                errorMessage.AppendLine(" | fix_version    = <!-- Version of AWB the fix will be included in; AWB developer will complete when it's fixed -->");
                errorMessage.AppendLine("}}");

                handler.txtDetails.Text = errorMessage.ToString();

                handler.txtSubject.Text = ex.GetType().Name + " in " + Thrower(ex);

                handler.ShowDialog();
            }
        }

        enum ExceptionKind { TopLevel, Inner, LoaderException };

        #region Static helper functions
        /// <summary>
        /// Formats exception information for bug report
        /// </summary>
        /// <param name="ex">Exception to process</param>
        /// <param name="sb">StringBuilder used for output</param>
        /// <param name="kind">what kind of exception is thise</param>
        private static void FormatException(Exception ex, StringBuilder sb, ExceptionKind kind)
        {
            sb.Append("<tr><td>" + KindToString(kind) + ":<td><code>"
                + ex.GetType().Name + "</code><tr><td>Message:<td><code>"
                + ex.Message + "</code><tr><td>Call stack:<td><pre>" + ex.StackTrace + "</pre></tr>\r\n");

            if (ex.InnerException != null)
            {
                FormatException(ex.InnerException, sb, ExceptionKind.Inner);
            }
            if (ex is ReflectionTypeLoadException)
            {
                foreach (Exception e in ((ReflectionTypeLoadException)ex).LoaderExceptions)
                {
                    FormatException(e, sb, ExceptionKind.LoaderException);
                }
            }
        }

        private static string KindToString(ExceptionKind ek)
        {
            switch (ek)
            {
                case ExceptionKind.Inner:
                    return "Inner exception";
                case ExceptionKind.LoaderException:
                    return "Loader exception";
                default:
                    return "Exception";
            }
        }

        /// <summary>
        /// Returns names of functions in stack trace of an exception
        /// </summary>
        /// <param name="ex">Exception to process</param>
        /// <returns>List of fully qualified function names</returns>
        private static string[] MethodNames(Exception ex)
        {
            MatchCollection mc = Regex.Matches(ex.StackTrace, @"([a-zA-Z_0-9.]+)(?=\()");

            string[] res = new string[mc.Count];

            for (int i = 0; i < res.Length; i++) res[i] = mc[i].Groups[1].Value;

            return res;
        }

        private static readonly string[] PresetNamespaces =
            new [] { "System.", "Microsoft.", "Mono." };

        /// <summary>
        /// Returns the name of our function where supposedly error resides;
        /// it's the last non-framework function in the stack
        /// </summary>
        /// <param name="ex">Exception to process</param>
        /// <returns>Function names without namespace</returns>
        private static string Thrower(Exception ex)
        {
            string[] trace = MethodNames(ex);

            if (trace.Length == 0) return "unknown function";

            string res = "";
            for (int i = 0; i < trace.Length; i++)
            {
                bool match = false;
                foreach (string ns in PresetNamespaces)
                {
                    if (trace[i].StartsWith(ns)) match = true;
                }
                if (match)
                    res = trace[0];
                else
                {
                    res = trace[i];
                    break;
                }
            }

            // strip namespace for clarity
            res = Regex.Match(res, @"\w+\.\w+$").Value;

            return res;
        }

        #endregion

        protected ErrorHandler()
        {
            InitializeComponent();
        }

        private void ErrorHandler_Load(object sender, EventArgs e)
        {
            Text = Application.ProductName;
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.Clear();
                Thread.Sleep(1000);
                Clipboard.SetText(txtDetails.Text);
            }
            catch { }
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
