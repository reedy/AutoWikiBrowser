/*
WikiFunctions
Copyright (C) 2008 Max Semenik, Stephen Kennedy

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 2 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace WikiFunctions
{
    /// <summary>
    /// Provides basic performance profiling
    /// </summary>
    public class Profiler
    {
        #if DEBUG
            Stopwatch Watch = new Stopwatch(); // fail-safe in case Start() wasn't called for some reason
            TextWriter log;

            /// <summary>
            /// Creates a profiler object
            /// </summary>
            /// <param name="filename">Name of file to save profiling log to</param>
            /// <param name="append">True if the file should not be overwritten</param>
            public Profiler(string filename, bool append)
            {
                log = new StreamWriter(filename, append, Encoding.Unicode);
            }

            public Profiler() { }

            /// <summary>
            /// Starts measuring time
            /// </summary>
            /// <param name="message">a message to associate with these measure</param>
            public void Start(string message)
            {
                AddLog("--------------------------------------");
                Watch = Stopwatch.StartNew();
                AddLog("Started profiling: " + message);
            }

            /// <summary>
            /// Outputs time difference between previous time mark and now to the profiling log
            /// </summary>
            /// <param name="message">description of the time interval</param>
            public void Profile(string message)
            {
                AddLog("   " + message + ": " + Watch.ElapsedMilliseconds.ToString());
                Watch = Stopwatch.StartNew();
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
        #else
        /* unfortunately it seems that code within [Conditional] blocks still gets analysed by the compiler; having the class level
         * vars in #if's and all the methods inside these Conditional attribute blocks didn't work. So, I've used #if statements to 
         * get a clean compile, and the attribute to then have the calls totally compiled out in release mode. */
            [Conditional("DEBUG")]
            public void Profile(string message) { }

            [Conditional("DEBUG")]
            public void Flush() { }

            // not currently used outside this class:
            //[Conditional("DEBUG")]
            //public void AddLog(string s) { }
        #endif

        //[Conditional("DEBUG")]
        /// <summary>
        /// Closes profiling log. The profiler will continue in dummy mode
        /// </summary>
        //public void Close()
        //{
        //    if (log == null) return;

        //    log.Close();
        //    log = null;
        //}
    }
}
