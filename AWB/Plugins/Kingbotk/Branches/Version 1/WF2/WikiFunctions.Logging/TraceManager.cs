namespace WikiFunctions.Logging
{
    using System;
    using System.Collections.Generic;
    using WikiFunctions;

    public class TraceManager : IMyTraceListener
    {
        protected Dictionary<string, IMyTraceListener> Listeners = new Dictionary<string, IMyTraceListener>();

        public virtual void AddListener(string Key, IMyTraceListener Listener)
        {
            this.Listeners.Add(Key, Listener);
        }

        public virtual void Close()
        {
            Dictionary<string, IMyTraceListener>.Enumerator VB$t_struct$L0;
            try
            {
                VB$t_struct$L0 = this.Listeners.GetEnumerator();
                while (VB$t_struct$L0.MoveNext())
                {
                    VB$t_struct$L0.Current.Value.Close();
                }
            }
            finally
            {
                VB$t_struct$L0.Dispose();
            }
        }

        public bool ContainsKey(string Key)
        {
            return this.Listeners.ContainsKey(Key);
        }

        public bool ContainsValue(IMyTraceListener Listener)
        {
            return this.Listeners.ContainsValue(Listener);
        }

        public virtual void Flush()
        {
            Dictionary<string, IMyTraceListener>.Enumerator VB$t_struct$L0;
            try
            {
                VB$t_struct$L0 = this.Listeners.GetEnumerator();
                while (VB$t_struct$L0.MoveNext())
                {
                    VB$t_struct$L0.Current.Value.Flush();
                }
            }
            finally
            {
                VB$t_struct$L0.Dispose();
            }
        }

        public virtual void ProcessingArticle(string FullArticleTitle, Namespaces NS)
        {
            Dictionary<string, IMyTraceListener>.Enumerator VB$t_struct$L0;
            try
            {
                VB$t_struct$L0 = this.Listeners.GetEnumerator();
                while (VB$t_struct$L0.MoveNext())
                {
                    VB$t_struct$L0.Current.Value.ProcessingArticle(FullArticleTitle, NS);
                }
            }
            finally
            {
                VB$t_struct$L0.Dispose();
            }
        }

        public virtual void RemoveListener(string Key)
        {
            this.Listeners[Key].Close();
            this.Listeners.Remove(Key);
        }

        public virtual void SkippedArticle(string SkippedBy, string Reason)
        {
            Dictionary<string, IMyTraceListener>.Enumerator VB$t_struct$L0;
            try
            {
                VB$t_struct$L0 = this.Listeners.GetEnumerator();
                while (VB$t_struct$L0.MoveNext())
                {
                    VB$t_struct$L0.Current.Value.SkippedArticle(SkippedBy, Reason);
                }
            }
            finally
            {
                VB$t_struct$L0.Dispose();
            }
        }

        public virtual void SkippedArticleBadTag(string SkippedBy, string FullArticleTitle, Namespaces NS)
        {
            Dictionary<string, IMyTraceListener>.Enumerator VB$t_struct$L0;
            try
            {
                VB$t_struct$L0 = this.Listeners.GetEnumerator();
                while (VB$t_struct$L0.MoveNext())
                {
                    VB$t_struct$L0.Current.Value.SkippedArticleBadTag(SkippedBy, FullArticleTitle, NS);
                }
            }
            finally
            {
                VB$t_struct$L0.Dispose();
            }
        }

        public virtual void SkippedArticleRedlink(string SkippedBy, string FullArticleTitle, Namespaces NS)
        {
            Dictionary<string, IMyTraceListener>.Enumerator VB$t_struct$L0;
            try
            {
                VB$t_struct$L0 = this.Listeners.GetEnumerator();
                while (VB$t_struct$L0.MoveNext())
                {
                    VB$t_struct$L0.Current.Value.SkippedArticleRedlink(SkippedBy, FullArticleTitle, NS);
                }
            }
            finally
            {
                VB$t_struct$L0.Dispose();
            }
        }

        public virtual void Write(string Text)
        {
            Dictionary<string, IMyTraceListener>.Enumerator VB$t_struct$L0;
            try
            {
                VB$t_struct$L0 = this.Listeners.GetEnumerator();
                while (VB$t_struct$L0.MoveNext())
                {
                    VB$t_struct$L0.Current.Value.Write(Text);
                }
            }
            finally
            {
                VB$t_struct$L0.Dispose();
            }
        }

        public virtual void WriteArticleActionLine(string Line, string PluginName)
        {
            Dictionary<string, IMyTraceListener>.Enumerator VB$t_struct$L0;
            try
            {
                VB$t_struct$L0 = this.Listeners.GetEnumerator();
                while (VB$t_struct$L0.MoveNext())
                {
                    VB$t_struct$L0.Current.Value.WriteArticleActionLine(Line, PluginName);
                }
            }
            finally
            {
                VB$t_struct$L0.Dispose();
            }
        }

        public virtual void WriteArticleActionLine1(string Line, string PluginName, bool VerboseOnly)
        {
            if (VerboseOnly)
            {
                Dictionary<string, IMyTraceListener>.Enumerator VB$t_struct$L0;
                try
                {
                    VB$t_struct$L0 = this.Listeners.GetEnumerator();
                    while (VB$t_struct$L0.MoveNext())
                    {
                        VB$t_struct$L0.Current.Value.WriteArticleActionLine(Line, PluginName, true);
                    }
                }
                finally
                {
                    VB$t_struct$L0.Dispose();
                }
            }
            else
            {
                Dictionary<string, IMyTraceListener>.Enumerator VB$t_struct$L1;
                try
                {
                    VB$t_struct$L1 = this.Listeners.GetEnumerator();
                    while (VB$t_struct$L1.MoveNext())
                    {
                        VB$t_struct$L1.Current.Value.WriteArticleActionLine(Line, PluginName);
                    }
                }
                finally
                {
                    VB$t_struct$L1.Dispose();
                }
            }
        }

        public virtual void WriteBulletedLine(string Line, bool Bold, bool VerboseOnly)
        {
            this.WriteBulletedLine(Line, Bold, VerboseOnly, false);
        }

        public virtual void WriteBulletedLine(string Line, bool Bold, bool VerboseOnly, bool DateStamp)
        {
            Dictionary<string, IMyTraceListener>.Enumerator VB$t_struct$L0;
            try
            {
                VB$t_struct$L0 = this.Listeners.GetEnumerator();
                while (VB$t_struct$L0.MoveNext())
                {
                    VB$t_struct$L0.Current.Value.WriteBulletedLine(Line, Bold, VerboseOnly, DateStamp);
                }
            }
            finally
            {
                VB$t_struct$L0.Dispose();
            }
        }

        public virtual void WriteComment(string Line)
        {
            Dictionary<string, IMyTraceListener>.Enumerator VB$t_struct$L0;
            try
            {
                VB$t_struct$L0 = this.Listeners.GetEnumerator();
                while (VB$t_struct$L0.MoveNext())
                {
                    VB$t_struct$L0.Current.Value.WriteComment(Line);
                }
            }
            finally
            {
                VB$t_struct$L0.Dispose();
            }
        }

        public virtual void WriteCommentAndNewLine(string Line)
        {
            Dictionary<string, IMyTraceListener>.Enumerator VB$t_struct$L0;
            try
            {
                VB$t_struct$L0 = this.Listeners.GetEnumerator();
                while (VB$t_struct$L0.MoveNext())
                {
                    VB$t_struct$L0.Current.Value.WriteCommentAndNewLine(Line);
                }
            }
            finally
            {
                VB$t_struct$L0.Dispose();
            }
        }

        public virtual void WriteLine(string Line)
        {
            Dictionary<string, IMyTraceListener>.Enumerator VB$t_struct$L0;
            try
            {
                VB$t_struct$L0 = this.Listeners.GetEnumerator();
                while (VB$t_struct$L0.MoveNext())
                {
                    VB$t_struct$L0.Current.Value.WriteLine(Line);
                }
            }
            finally
            {
                VB$t_struct$L0.Dispose();
            }
        }

        public virtual void WriteTemplateAdded(string Template, string PluginName)
        {
            Dictionary<string, IMyTraceListener>.Enumerator VB$t_struct$L0;
            try
            {
                VB$t_struct$L0 = this.Listeners.GetEnumerator();
                while (VB$t_struct$L0.MoveNext())
                {
                    VB$t_struct$L0.Current.Value.WriteTemplateAdded(Template, PluginName);
                }
            }
            finally
            {
                VB$t_struct$L0.Dispose();
            }
        }

        public virtual bool Uploadable
        {
            get
            {
                bool Uploadable;
                Dictionary<string, IMyTraceListener>.Enumerator VB$t_struct$L0;
                try
                {
                    VB$t_struct$L0 = this.Listeners.GetEnumerator();
                    while (VB$t_struct$L0.MoveNext())
                    {
                        if (VB$t_struct$L0.Current.Value.Uploadable)
                        {
                            return true;
                        }
                    }
                }
                finally
                {
                    VB$t_struct$L0.Dispose();
                }
                return Uploadable;
            }
        }

        public virtual bool WikiFunctions.Logging.IMyTraceListener.Uploadable
        {
            get
            {
                bool Uploadable;
                Dictionary<string, IMyTraceListener>.Enumerator VB$t_struct$L0;
                try
                {
                    VB$t_struct$L0 = this.Listeners.GetEnumerator();
                    while (VB$t_struct$L0.MoveNext())
                    {
                        if (VB$t_struct$L0.Current.Value.Uploadable)
                        {
                            return true;
                        }
                    }
                }
                finally
                {
                    VB$t_struct$L0.Dispose();
                }
                return Uploadable;
            }
        }
    }
}

