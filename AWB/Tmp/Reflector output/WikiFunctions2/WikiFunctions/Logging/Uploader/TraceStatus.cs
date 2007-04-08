namespace WikiFunctions.Logging.Uploader
{
    using System;

    public class TraceStatus
    {
        protected DateTime mDate = DateTime.Now;
        protected string mFileName;
        protected int mLinesWritten;
        protected int mLinesWrittenSinceLastUpload;
        protected string mLogName;
        protected string mLogUpload;
        protected int mNextPageNumber = 1;

        public TraceStatus(string FileNameIs, string LogNameIs)
        {
            this.FileName = FileNameIs;
            this.LogName = LogNameIs;
        }

        public virtual void Close()
        {
        }

        public virtual string FileName
        {
            get
            {
                return this.mFileName;
            }
            set
            {
                this.mFileName = value;
            }
        }

        public virtual int LinesWritten
        {
            get
            {
                return this.mLinesWritten;
            }
            set
            {
                this.mLinesWritten = value;
                this.LinesWrittenSinceLastUpload++;
            }
        }

        public virtual int LinesWrittenSinceLastUpload
        {
            get
            {
                return this.mLinesWrittenSinceLastUpload;
            }
            set
            {
                this.mLinesWrittenSinceLastUpload = value;
            }
        }

        public virtual string LogName
        {
            get
            {
                return this.mLogName;
            }
            set
            {
                this.mLogName = value;
            }
        }

        public virtual string LogUpload
        {
            get
            {
                return this.mLogUpload;
            }
            set
            {
                this.mLogUpload = value;
            }
        }

        public virtual int PageNumber
        {
            get
            {
                return this.mNextPageNumber;
            }
            set
            {
                this.mNextPageNumber = value;
            }
        }

        public virtual DateTime StartDate
        {
            get
            {
                return this.mDate;
            }
            set
            {
                this.mDate = value;
            }
        }
    }
}

