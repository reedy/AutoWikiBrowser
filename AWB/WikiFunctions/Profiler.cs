using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace WikiFunctions
{
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

        public void Start()
        {
            Ticks = DateTime.Now.Ticks;
        }

        public void Start(string message)
        {
            AddLog("Started profiling: " + message);
            Start();
        }

        public void Profile(string message)
        {
            AddLog("   " + message + ": " + ((DateTime.Now.Ticks - Ticks) / 10000).ToString());
            Ticks = DateTime.Now.Ticks;
        }

        public void AddLog(string s)
        {
            if (log != null) log.WriteLine(s);
        }

        public void Flush()
        {
            if (log != null) log.Flush();
        }

        TextWriter log;
        long Ticks;
    }
}
