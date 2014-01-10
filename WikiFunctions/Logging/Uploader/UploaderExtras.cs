namespace WikiFunctions.Logging.Uploader
{
    /// <summary>
    /// Object which contains details of target pages for log entries
    /// </summary>
    public class LogEntry
    {
        public LogEntry(string pLocation, bool pUserName)
        {
            Location = pLocation;
            LogUserName = pUserName;
        }

        public string Location { get; private set; }
        public bool LogUserName { get; private set; }
        public bool Success { get; protected internal set; }
    }

}