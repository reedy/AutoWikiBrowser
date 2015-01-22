using System;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Threading;
using System.Text.RegularExpressions;
using System.Web;
using WikiFunctions.API;

//////////////////////////////////////////////////////////////////////////////////////////////
/* Don't use anything WikiFunctions-specific here, for source-compatibility with Updater  */
//////////////////////////////////////////////////////////////////////////////////////////////

namespace WikiFunctions
{
    public delegate string ErrorHandlerAddition();

    /// <summary>
    /// This class provides helper functions for handling errors and displaying them to users
    /// </summary>
    public partial class ErrorHandler : Form
    {
        public static event ErrorHandlerAddition AppendToErrorHandler;

        /// <summary>
        /// Title of the page currently being processed
        /// </summary>
        public static string CurrentPage;

        /// <summary>
        /// Revision of the page currently being processed
        /// </summary>
        public static long CurrentRevision;

        /// <summary>
        /// Current text that the list is being made from in ListMaker
        /// </summary>
        public static string ListMakerText;

        private static bool HandleKnownExceptions(Exception ex)
        {
            Tools.WriteDebug("HandleKnownExceptions", ex.StackTrace.ToString());
            // invalid regex - only ArgumentException, without subclasses
            if (ex is ArgumentException &&
                (ex.StackTrace.Contains("System.Text.RegularExpressions") ||
                 ex.ToString().StartsWith(@"System.ArgumentException: parsing")))
            {
                MessageBox.Show(ex.Message, "Invalid regular expression",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            //Unsupported Culture, possibly bn-BD
            else if (ex is ArgumentException && Thrower(ex) == "CultureTableRecord.GetCultureTableRecord")
            {
                MessageBox.Show(
                    "Microsoft unfortunately don't support your locale culture. Please try a more common one",
                    "Unsupported culture");
            }
            // network access error
            else if (ex is System.Net.WebException || ex.InnerException is System.Net.WebException)
            {
                // if AWB starts up offline we'll hit here, so provide clear network related message
                string msg = ex.Message.StartsWith(@"The type initializer for") ? ex.InnerException.Message : ex.Message;

                MessageBox.Show(msg, "Network access error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            // out of memory error
            else if (ex is OutOfMemoryException)
            {
                MessageBox.Show(ex.Message, "Out of Memory error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            // disk writer error / full
            else if (ex is System.IO.IOException || ex is ConfigurationErrorsException
                     && (ex.InnerException != null && ex.InnerException.InnerException != null
                         && ex.InnerException.InnerException is System.IO.IOException))
            {
                MessageBox.Show(ex.Message, "I/O error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            // BackGroundRequest Abort called as user pressed stop, this is OK
            else if (ex is ThreadAbortException &&
                     (ex.StackTrace.Contains("AutoWikiBrowser.MainForm.ProcessPage") ||
                      ex.StackTrace.Contains("Parsers.TagOrphans")))
            {
                return true;
            }
            else
            {
                return false; // We didn't handle the exception
            }

            return true; // We handled the exception
        }

        /// <summary>
        /// Displays exception information. Should be called from try...catch handlers
        /// </summary>
        /// <param name="ex">Exception object to handle</param>
        public static void HandleException(Exception ex)
        {
            if (ex == null || HandleKnownExceptions(ex)) return;

            // TODO: suggest a bug report for other exceptions
            ErrorHandler handler = new ErrorHandler {txtError = {Text = ex.Message}};

            var errorMessage = new BugReport(ex).PrintForPhabricator();
            handler.txtDetails.Text = errorMessage;

            handler.txtSubject.Text = ex.GetType().Name + " in " + Thrower(ex);

            Tools.WriteDebug("HandleException", errorMessage);
            handler.ShowDialog();
        }

        class BugReport
        {
            private string Thread,
                OS = Environment.OSVersion.ToString(),
                StackTrace,
                ApiExtra,
                AppendedInfo,
                Version,
                DotNetVersion,
                Duplicate;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="ex"></param>
            public BugReport(Exception ex)
            {
                var thread = ex is ApiException ? ((ApiException) ex).ThrowingThread : System.Threading.Thread.CurrentThread;
                if (thread.Name != "Main thread")
                {
                    Thread = thread.Name;
                }

                StringBuilder stackTrace = new StringBuilder();
                FormatException(ex, stackTrace, ExceptionKind.TopLevel);
                StackTrace = stackTrace.ToString();

                var exception = ex as ApiException;
                if (exception != null)
                {
                    ApiExtra = exception.GetExtraSpecificInformation();
                }

                if (AppendToErrorHandler != null)
                {
                    StringBuilder append = new StringBuilder();
                    foreach (Delegate d in AppendToErrorHandler.GetInvocationList())
                    {
                        string retval = d.DynamicInvoke().ToString();
                        if (!string.IsNullOrEmpty(retval))
                            append.AppendLine(retval);
                    }
                    AppendedInfo = append.ToString();
                }

                AssemblyName hostingApp = Assembly.GetExecutingAssembly().GetName();
                Version = string.Format("{0} ({1}), {2} ({3})", Application.ProductName, Application.ProductVersion,
                    hostingApp.Name, hostingApp.Version);

                DotNetVersion = Environment.Version.ToString();

                // suppress unhandled exception if Variables constructor says 'ouch'
                try
                {
                    Version += ", revision " + Variables.Revision;
                }
                catch
                {
                }

                // TODO: Phab urls
                if (!string.IsNullOrEmpty(CurrentPage))
                {
                    // don't use Tools.WikiEncode here, to keep code portable to updater
                    // as it's not a pretty URL, we don't need to follow the MediaWiki encoding rules
                    string link = "[" + Variables.URLIndex + "?title=" + HttpUtility.UrlEncode(CurrentPage) + "&oldid=" +
                                  CurrentRevision + "]";

                    Duplicate = "[encountered while processing page ''" + link + "'']";
                }
                else if (!string.IsNullOrEmpty(ListMakerText))
                {
                    Duplicate = "'''ListMaker Text:''' " + ListMakerText;
                }
            }

            /// <summary>
            /// Prints a wiki formatted bug report table
            /// </summary>
            /// <returns>String using {{AWB bug}} for reporting bugs</returns>
            public string PrintForWiki()
            {
                return Print(new WikiBugFormatter());
            }

            public string PrintForPhabricator()
            {
                return Print(new PhabricatorBugFormatter());
            }

            public string Print(BugFormatter formatter)
            {
                StringBuilder errorMessage = new StringBuilder();

                if (formatter.HasHeaderFooter())
                {
                    errorMessage.AppendLine(formatter.PrintHeader());
                }
                errorMessage.AppendLine(formatter.PrintLine("description", ""));

                errorMessage.Append("<table>");
                errorMessage.AppendLine(StackTrace);
                errorMessage.AppendLine("</table>");

                if (!string.IsNullOrEmpty(ApiExtra))
                {
                    errorMessage.AppendLine(ApiExtra);
                }

                if (!string.IsNullOrEmpty(AppendedInfo))
                {
                    errorMessage.AppendLine(AppendedInfo);
                }

                if (formatter is WikiBugFormatter)
                {
                    errorMessage.AppendLine("~~~~");
                }

                if (!string.IsNullOrEmpty(Thread))
                {
                    errorMessage.AppendLine(formatter.PrintLine("thread", Thread));
                }

                errorMessage.AppendLine(formatter.PrintLine("OS", OS));
                errorMessage.AppendLine(formatter.PrintLine("version", Version));
                errorMessage.AppendLine(formatter.PrintLine("net", DotNetVersion));

                if (!string.IsNullOrEmpty(Duplicate))
                {
                    errorMessage.AppendLine(formatter.PrintLine("duplicate", Duplicate));
                }

                if (!string.IsNullOrEmpty(Variables.URL))
                {
                    errorMessage.AppendLine(formatter.PrintLine("site", Variables.URL));
                }

                errorMessage.AppendLine(formatter.PrintLine("workaround", ""));
                if (formatter.HasHeaderFooter())
                {
                    errorMessage.AppendLine(formatter.PrintFooter());
                }

                return errorMessage.ToString();
            }

            /// <summary>
            /// Formats exception information for bug report
            /// </summary>
            /// <param name="ex">Exception to process</param>
            /// <param name="sb">StringBuilder used for output</param>
            /// <param name="kind">what kind of exception is this</param>
            private static void FormatException(Exception ex, StringBuilder sb, ExceptionKind kind)
            {
                sb.AppendFormat("<tr><th>{0}:</th><td>`{1}`</td></tr>\r\n", KindToString(kind), ex.GetType().Name);
                sb.AppendFormat("<tr><th>Message:</th><td>`{0}`</td></tr>\r\n", ex.Message);
                sb.AppendFormat("<tr><th>Call stack:</th><td><pre>{0}</pre></td></tr>\r\n", ex.StackTrace);

                if (ex.InnerException != null)
                {
                    FormatException(ex.InnerException, sb, ExceptionKind.Inner);
                }
                if (ex is ReflectionTypeLoadException)
                {
                    foreach (Exception e in ((ReflectionTypeLoadException) ex).LoaderExceptions)
                    {
                        FormatException(e, sb, ExceptionKind.LoaderException);
                    }
                }
            }

            private enum ExceptionKind
            {
                TopLevel,
                Inner,
                LoaderException
            };

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

            public abstract class BugFormatter
            {
                public abstract string PrintHeader();
                public abstract string PrintFooter();
                public abstract string PrintLine(string key, string value);

                public virtual bool HasHeaderFooter()
                {
                    return false;
                }
            }

            public class WikiBugFormatter : BugFormatter
            {
                public override string PrintHeader()
                {
                    return @"{{AWB bug\r\n" + PrintLine("status", "new <!-- when fixed replace with \"fixed\" -->");
                }

                public override string PrintFooter()
                {
                    return
                        PrintLine("fix_version",
                            "<!-- Version of AWB the fix will be included in; AWB developer will complete when it's fixed -->") +
                        "\r\n}}";
                }

                public override string PrintLine(string key, string value)
                {
                    return string.Format(" | {0,-14} = {1}", key, value);
                }

                public override bool HasHeaderFooter()
                {
                    return true;
                }
            }

            public class PhabricatorBugFormatter : BugFormatter
            {
                public override string PrintHeader()
                {
                    return "";
                }

                public override string PrintFooter()
                {
                    return "";
                }

                public override string PrintLine(string key, string value)
                {
                    return string.Format("**{0}**: {1}", key, value);
                }
            }
        }

        #region Static helper functions

        private static readonly Regex StackTrace = new Regex(@"([a-zA-Z_0-9\.`]+)(?=\()", RegexOptions.Compiled);

        /// <summary>
        /// Returns names of functions in stack trace of an exception
        /// </summary>
        /// <param name="stackTrace">Exception's StackTrace</param>
        /// <returns>List of fully qualified function names</returns>
        public static string[] MethodNames(string stackTrace)
        {
            MatchCollection mc = StackTrace.Matches(stackTrace);

            string[] res = new string[mc.Count];

            for (int i = 0; i < res.Length; i++) res[i] = mc[i].Groups[1].Value;

            return res;
        }

        /// <summary>
        /// Returns the name of our function where supposedly error resides;
        /// it's the last non-framework function in the stack
        /// </summary>
        /// <param name="ex">Exception to process</param>
        /// <returns>Function names without namespace</returns>
        public static string Thrower(Exception ex)
        {
            string[] trace = MethodNames(ex.StackTrace);

            if (trace.Length == 0)
            {
                return "unknown function";
            }

            string res = "";
            foreach (string t in trace)
            {
                if (PresetNamespaces.Any(ns => t.StartsWith(ns)))
                {
                    res = trace[0];
                }
                else
                {
                    res = t;
                    break;
                }
            }

            // strip namespace for clarity
            var res2 = Regex.Match(res, @"\w+\.{1,2}\w+$").Value;
            if (res2.Length > 0)
            {
                return res2;
            }

            return res;
        }

        private static readonly string[] PresetNamespaces =
        {"System.", "Microsoft.", "Mono."};

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
                Thread.Sleep(50); // give it some time to clear
                Clipboard.SetText(txtDetails.Text);
            }
            catch
            {
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkLabel1.LinkVisited = true;
            try
            {
                System.Diagnostics.Process.Start(
                    "https://phabricator.wikimedia.org/maniphest/task/create/?projects=AutoWikiBrowser");
            }
            catch
            {
            }
        }
    }
}
