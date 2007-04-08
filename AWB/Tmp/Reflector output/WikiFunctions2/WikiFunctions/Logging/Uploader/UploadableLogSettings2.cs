namespace WikiFunctions.Logging.Uploader
{
    using System;

    public class UploadableLogSettings2 : UploadableLogSettings
    {
        protected bool mLogWiki = true;
        protected bool mLogXHTML = false;
        protected bool mUploadAddToWatchlist = true;
        protected string mUploadJobName = "";
        protected string mUploadLocation = "";

        public virtual bool LogWiki
        {
            get
            {
                return this.mLogWiki;
            }
            set
            {
                this.mLogWiki = value;
            }
        }

        public virtual bool LogXHTML
        {
            get
            {
                return this.mLogXHTML;
            }
            set
            {
                this.mLogXHTML = value;
            }
        }

        public virtual bool UploadAddToWatchlist
        {
            get
            {
                return this.mUploadAddToWatchlist;
            }
            set
            {
                this.mUploadAddToWatchlist = value;
            }
        }

        public virtual string UploadJobName
        {
            get
            {
                return this.mUploadJobName;
            }
            set
            {
                this.mUploadJobName = value;
            }
        }

        public virtual string UploadLocation
        {
            get
            {
                return this.mUploadLocation;
            }
            set
            {
                this.mUploadLocation = value;
            }
        }
    }
}

