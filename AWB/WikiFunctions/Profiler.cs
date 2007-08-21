using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace WikiFunctions
{
    /// <summary>
    /// Provides basic performance profiling
    /// </summary>
    public class Profiler
    {
        /// <summary>
        /// Creates a profiler object
        /// </summary>
        /// <param name="filename">Name of file to save profiling log to</param>
        /// <param name="append">True if the file should not be overwritten</param>
        public Profiler(string filename, bool append)
        {
            log = new StreamWriter(filename, append, Encoding.Unicode);
        }

        /// <summary>
        /// Dummy constructor when no profiling is needed
        /// </summary>
        public Profiler()
        {
        }

        /// <summary>
        /// Specifies precision of profiler output
        /// defaults to milliseconds
        /// </summary>
        public int Divisor = 10000;

        /// <summary>
        /// Starts measuring time
        /// </summary>
        public void Start()
        {
            Ticks = DateTime.Now.Ticks;
        }

        /// <summary>
        /// Starts measuring time
        /// </summary>
        /// <param name="message">a message to associate with these measure</param>
        public void Start(string message)
        {
            AddLog("Started profiling: " + message);
            Start();
        }

        /// <summary>
        /// Outputs time difference between previous time mark and now to the profiling log
        /// </summary>
        /// <param name="message">description of the time interval</param>
        public void Profile(string message)
        {
            AddLog("   " + message + ": " + ((DateTime.Now.Ticks - Ticks) / Divisor).ToString());
            Ticks = DateTime.Now.Ticks;
        }

        /// <summary>
        /// Adds a line to the log
        /// </summary>
        /// <param name="s"></param>
        public void AddLog(string s)
        {
            if (log != null) log.WriteLine(s);
        }

        /// <summary>
        /// Flushes profiling log on disk
        /// </summary>
        public void Flush()
        {
            if (log != null) log.Flush();
        }

        TextWriter log;
        long Ticks;
    }
}
