using System;
using System.Threading;
using System.Diagnostics;

namespace WikiFunctions.API
{
    /// <summary>
    /// Multithreaded API editor class
    /// </summary>
    public class AsyncApiEdit : IApiEdit
    {
        Thread m_Thread;

        public AsyncApiEdit(string url)
            :this(url, false)
        {
        }

        public AsyncApiEdit(string url, bool php5)
        {
            Editor = new ApiEdit(url, php5);
        }

        /// <summary>
        /// Provides access to the underlying ApiEdit
        /// </summary>
        public ApiEdit Editor { get; private set; }

        /// <summary>
        /// Waits for asyncronous operation to complete
        /// </summary>
        public void Wait()
        {
            if (m_Thread != null) m_Thread.Join();
        }

        private class InvokeArgs
        {
            public readonly string Function;
            public readonly object[] Arguments;

            public InvokeArgs(string func, params object[] args)
            {
                Function = func;
                Arguments = args;
            }
        };

        private void InvokerThread(object genericArgs)
        {
            try
            {
                InvokeArgs args = (InvokeArgs)genericArgs;

                Thread.CurrentThread.Name = string.Format("InvokerThread ({0})", args.Function);

                Type t = Editor.GetType();

                object res = t.InvokeMember(
                    args.Function,                                  // name
                    System.Reflection.BindingFlags.InvokeMethod,    // invokeAttr
                    null,                                           // binder
                    Editor,                                       // target
                    args.Arguments                                  // args
                    );

                Debug.WriteLine(Thread.CurrentThread.Name + " successfully completed");
            }
            catch (Exception ex)
            {
                // TODO: 
                Tools.WriteDebug("AsyncApiEdit", ex.Message);
            }
        }

        private void InvokeFunction(InvokeArgs args)
        {
            if (m_Thread != null && m_Thread.IsAlive)
                throw new ApiInvokeException("An asynchronous call is already being performed");

            m_Thread = new Thread(InvokerThread);
            m_Thread.Start(args);
        }

        private void InvokeFunction(string name, params object[] args)
        {
            InvokeFunction(new InvokeArgs(name, args));
        }

        #region IApiEdit Members

        public string URL
        {
            get { return Editor.URL; }
            set { Editor = new ApiEdit(value, PHP5); }
        }

        public bool PHP5
        {
            get { return Editor.PHP5; }
        }

        public string Action
        {
            get { return Editor.Action; }
        }

        public string PageTitle
        {
            get { return Editor.PageTitle; }
        }

        public string PageText
        {
            get { return Editor.PageText; }
        }

        public void Reset()
        {
            Abort();
            Editor.Reset();
        }

        public string HttpGet(string url)
        {
            InvokeFunction("HttpGet", url);
            return null;
        }

        public void Login(string username, string password)
        {
            InvokeFunction("Login", username, password);
        }

        public void Logout()
        {
            InvokeFunction("Logout");
        }

        public bool LogInStatus()
        {
            return Editor.LogInStatus();
        }

        public string Open(string title)
        {
            InvokeFunction("Open", title);
            return null;
        }

        public void Save(string pageText, string summary, bool minor, bool watch)
        {
            InvokeFunction("Save", pageText, summary, minor, watch);
        }

        public void Delete(string title, string reason)
        {
            Delete(title, reason, false);
        }

        public void Delete(string title, string reason, bool watch)
        {
            InvokeFunction("Delete", title, reason, watch);
        }

        public void Protect(string title, string reason, string expiry, Protection edit, Protection move, bool cascade, bool watch)
        {
            InvokeFunction("Protect", title, reason, expiry, edit, move, cascade, watch);
        }

        public void Protect(string title, string reason, TimeSpan expiry, Protection edit, Protection move, bool cascade, bool watch)
        {
            Protect(title, reason, expiry.ToString(), edit, move, cascade, watch);
        }

        public void Protect(string title, string reason, string expiry, Protection edit, Protection move)
        {
            Protect(title, reason, expiry, edit, move, false, false);
        }

        public void Protect(string title, string reason, TimeSpan expiry, Protection edit, Protection move)
        {
            Protect(title, reason, expiry.ToString(), edit, move, false, false);
        }

        public void MovePage(string title, string newTitle, string reason, bool moveTalk, bool noRedirect)
        {
            MovePage(title, newTitle, reason, moveTalk, noRedirect);
        }

        public void MovePage(string title, string newTitle, string reason, bool moveTalk, bool noRedirect, bool watch)
        {
            InvokeFunction("MovePage", title, newTitle, reason, moveTalk, noRedirect, watch);
        }

        public string Preview(string pageTitle, string text)
        {
            return Editor.Preview(pageTitle, text);
            //InvokeFunction("Preview", pageTitle, text);
        }

        public string ExpandTemplates(string pageTitle, string text)
        {
            return Editor.ExpandTemplates(pageTitle, text);
            //InvokeFunction("ExpandTemplates", pageTitle, text);
        }

        public void Abort()
        {
            try
            {
                if (m_Thread != null)
                    m_Thread.Abort();
                Editor.Abort();
            }
            catch (ThreadAbortException)
            {
            }
        }

        public bool Asynchronous
        {
            get { return true; }
        }

        #endregion
    }

    public class ApiInvokeException : Exception
    {
        public ApiInvokeException(string message)
            : base(message)
        {
        }

        public ApiInvokeException(Exception innerException)
            : this("There was a problem with an asynchronous API call", innerException)
        {
        }

        public ApiInvokeException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
