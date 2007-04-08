namespace WikiFunctions.Logging.Uploader
{
    using System;

    public sealed class LogEntry
    {
        public string Location;
        public bool UserName;

        public LogEntry(string pLocation, bool pUserName)
        {
            this.Location = pLocation;
            this.UserName = pUserName;
        }
    }
}

