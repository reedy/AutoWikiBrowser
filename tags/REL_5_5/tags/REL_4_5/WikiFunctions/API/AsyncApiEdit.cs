using System;

namespace WikiFunctions.API
{
    /// <summary>
    /// Multithreaded API editor class
    /// </summary>
    public class AsyncApiEdit : IApiEdit
    {
        ApiEdit m_Editor;

        /// <summary>
        /// Provides access to the underlying ApiEdit
        /// </summary>
        public ApiEdit Editor
        { get { return m_Editor; } }

        bool m_AsyncMode = true;

        /// <summary>
        /// Whether all operations should return immediately or wait for completition
        /// </summary>
        public bool AsyncMode
        {
            get { return m_AsyncMode; }
            set 
            {
                m_AsyncMode = false;
                Wait();
            }
        }

        /// <summary>
        /// Waits for asyncronous operation to complete
        /// </summary>
        public void Wait()
        {
            throw new NotImplementedException();
        }

        #region IApiEdit Members

        public string URL
        {
            get { return m_Editor.URL; }
            set { m_Editor = new ApiEdit(value); }
        }

        public string Action
        {
            get { return m_Editor.Action; }
        }

        public string PageTitle
        {
            get { return m_Editor.PageTitle; }
        }

        public string PageText
        {
            get { return m_Editor.PageText; }
        }

        public void Reset()
        {
            Abort();
            m_Editor.Reset();
        }

        public string HttpGet(string url)
        {
            throw new NotImplementedException();
        }

        public void Login(string username, string password)
        {
            throw new NotImplementedException();
        }

        public void Logout()
        {
            throw new NotImplementedException();
        }

        public string Open(string title)
        {
            throw new NotImplementedException();
        }

        public void Save(string pageText, string summary, bool minor, bool watch)
        {
            throw new NotImplementedException();
        }

        public void Delete(string title, string reason)
        {
            throw new NotImplementedException();
        }

        public void Protect(string title, string reason, string expiry, Protection edit, Protection move, bool cascade)
        {
            throw new NotImplementedException();
        }

        public void Protect(string title, string reason, TimeSpan expiry, Protection edit, Protection move, bool cascade)
        {
            throw new NotImplementedException();
        }

        public void Protect(string title, string reason, string expiry, Protection edit, Protection move)
        {
            throw new NotImplementedException();
        }

        public void Protect(string title, string reason, TimeSpan expiry, Protection edit, Protection move)
        {
            throw new NotImplementedException();
        }

        public void MovePage(string title, string newTitle, string reason, bool moveTalk, bool noRedirect)
        {
            throw new NotImplementedException();
        }

        public void Abort()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
