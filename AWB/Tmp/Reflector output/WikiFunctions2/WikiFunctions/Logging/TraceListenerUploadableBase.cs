namespace WikiFunctions.Logging
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using WikiFunctions.Logging.Uploader;

    public abstract class TraceListenerUploadableBase : TraceListenerBase, ITraceStatusProvider
    {
        protected WikiFunctions.Logging.Uploader.TraceStatus mTraceStatus;
        protected UploadableLogSettings2 mUploadSettings;

        public event UploadEventHandler Upload;

        public TraceListenerUploadableBase(UploadableLogSettings2 UploadSettings, WikiFunctions.Logging.Uploader.TraceStatus TraceStatus) : base(TraceStatus.FileName)
        {
            this.mTraceStatus = TraceStatus;
            this.mUploadSettings = UploadSettings;
        }

        public virtual void CheckCounterForUpload()
        {
            if (this.IsReadyToUpload)
            {
                this.UploadLog(false);
            }
        }

        public void Close(bool Upload)
        {
            if (Upload)
            {
                this.UploadLog(false);
            }
            this.mTraceStatus.Close();
            base.Close();
        }

        public virtual bool UploadLog([Optional, DefaultParameterValue(false)] bool NewJob)
        {
            bool UploadLog;
            UploadEventHandler VB$t_ref$S0 = this.UploadEvent;
            if (VB$t_ref$S0 != null)
            {
                VB$t_ref$S0(this, ref UploadLog);
            }
            if (NewJob)
            {
                this.mTraceStatus.PageNumber = 1;
                this.mTraceStatus.StartDate = DateTime.Now;
                return UploadLog;
            }
            WikiFunctions.Logging.Uploader.TraceStatus VB$t_ref$S1 = this.mTraceStatus;
            VB$t_ref$S1.PageNumber++;
            this.mTraceStatus.LinesWrittenSinceLastUpload = 0;
            this.mTraceStatus.LogUpload = "";
            return UploadLog;
        }

        public virtual void WriteLine(string Line, [Optional, DefaultParameterValue(true)] bool CheckCounter)
        {
            base.WriteLine(Line);
            WikiFunctions.Logging.Uploader.TraceStatus VB$t_ref$S0 = this.mTraceStatus;
            VB$t_ref$S0.LogUpload = VB$t_ref$S0.LogUpload + Line + "\r\n";
            VB$t_ref$S0 = this.mTraceStatus;
            VB$t_ref$S0.LinesWritten++;
            if (CheckCounter)
            {
                this.CheckCounterForUpload();
            }
        }

        protected virtual bool IsReadyToUpload
        {
            get
            {
                return (this.mTraceStatus.LinesWrittenSinceLastUpload >= this.mUploadSettings.UploadMaxLines);
            }
        }

        public virtual string PageName
        {
            get
            {
                return string.Format("{0:ddMMyy} {1}", this.mTraceStatus.StartDate, this.mUploadSettings.UploadJobName);
            }
        }

        public WikiFunctions.Logging.Uploader.TraceStatus TraceStatus
        {
            get
            {
                return this.mTraceStatus;
            }
        }

        public override bool Uploadable
        {
            get
            {
                return true;
            }
        }

        public UploadableLogSettings2 UploadSettings
        {
            get
            {
                return this.mUploadSettings;
            }
        }

        public override bool Verbose
        {
            get
            {
                return this.mUploadSettings.LogVerbose;
            }
        }

        public WikiFunctions.Logging.Uploader.TraceStatus WikiFunctions.Logging.Uploader.ITraceStatusProvider.TraceStatus
        {
            get
            {
                return this.mTraceStatus;
            }
        }

        public delegate void UploadEventHandler(TraceListenerUploadableBase Sender, ref bool Success);
    }
}

