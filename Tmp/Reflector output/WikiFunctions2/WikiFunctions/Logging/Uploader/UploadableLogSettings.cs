namespace WikiFunctions.Logging.Uploader
{
    using System;

    public class UploadableLogSettings
    {
        protected string mLogFolder = Application.StartupPath;
        protected bool mLogVerbose = true;
        protected int mUploadMaxLines = 0x3e8;
        protected bool mUploadOpenInBrowser = false;
        protected bool mUploadYN;

        public virtual string LogFolder
        {
            get
            {
                return this.mLogFolder;
            }
            set
            {
                this.mLogFolder = value;
            }
        }

        public virtual bool LogVerbose
        {
            get
            {
                return this.mLogVerbose;
            }
            set
            {
                this.mLogVerbose = value;
            }
        }

        public virtual int UploadMaxLines
        {
            get
            {
                return this.mUploadMaxLines;
            }
            set
            {
                this.mUploadMaxLines = value;
            }
        }

        public virtual bool UploadOpenInBrowser
        {
            get
            {
                return this.mUploadOpenInBrowser;
            }
            set
            {
                this.mUploadOpenInBrowser = value;
            }
        }

        public virtual bool UploadYN
        {
            get
            {
                return this.mUploadYN;
            }
            set
            {
                this.mUploadYN = value;
            }
        }
    }
}

