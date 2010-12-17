/*
AWBPackager
Copyright (C) 2009 Sam Reed

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
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using ICSharpCode.SharpZipLib.Zip;

namespace AWBPackager
{
    class Program
    {
        private static readonly Regex SvnVersion = new Regex(@"""(\d+).*?""", RegexOptions.Compiled);

        static void Main()
        {
            try
            {
                string filename = "AutoWikiBrowser{0}";

                Console.Write(
                    @"Please ensure a release build has been built before continuing.
Is this SVN (1) or a release (2)? ");

                int selection;
                
                int.TryParse(Console.ReadLine(), out selection);
                
                if (selection == -1)
                {
                    Console.Write("Please select 1 or 2");
                    return;
                }

                string awbDir = Directory.GetCurrentDirectory();
                string tmp = Path.Combine(awbDir, "temp");
                awbDir = awbDir.Remove(awbDir.IndexOf("Extras"));
                Directory.CreateDirectory(tmp);

                string currFolder = Path.Combine(Path.Combine(awbDir, "AWB"), "bin");;

                if (selection == 1)
                {

                    using (StreamReader reader = new StreamReader(Path.Combine(Path.Combine(awbDir, "WikiFunctions"), "SvnInfo.cs")))
                    {
                        string text = reader.ReadToEnd();
                        filename += "_rev" + SvnVersion.Match(text).Groups[1].Value;

                        reader.Close();
                    }
                    currFolder = Path.Combine(currFolder, "Debug");
                }
                else
                {
                    currFolder = Path.Combine(currFolder, "Release");
                }

                filename += ".zip";

                Copy(currFolder, tmp, "AutoWikiBrowser.exe");

                filename = string.Format(filename,
                                         StringToVersion(
                                             FileVersionInfo.GetVersionInfo(Path.Combine(currFolder, "AutoWikiBrowser.exe")).
                                                 FileVersion));

                Copy(currFolder, tmp, "AutoWikiBrowser.exe.config");
                Copy(currFolder, tmp, "WikiFunctions.dll");
                Copy(currFolder, tmp, "AWBUpdater.exe");

                CreateTextFile(tmp + "Diff.dll", "This file is not used anymore, but is preserved for compatibility "
                    + "with older versions of AWBUpdater.");

                string tmpPlugins = Path.Combine(tmp, "Plugins");

                Directory.CreateDirectory(tmp);

                CopyAndCreateDirectory(currFolder, Path.Combine(tmpPlugins, "CFD"), "CFD.dll");
                CopyAndCreateDirectory(currFolder, Path.Combine(tmpPlugins, "IFD"), "IFD.dll");
                CopyAndCreateDirectory(currFolder, Path.Combine(tmpPlugins, "NoLimitsPlugin"), "NoLimitsPlugin.dll");
                CopyAndCreateDirectory(currFolder, Path.Combine(tmpPlugins, "Yahoo Search Plugin"), "YahooSearchPlugin.dll");
                CopyAndCreateDirectory(currFolder, Path.Combine(tmpPlugins, "Bing Search Plugin"), "BingSearchPlugin.dll");
                CopyAndCreateDirectory(currFolder, Path.Combine(tmpPlugins, "TypoScan Plugin"), "TypoScan.dll");
                CopyAndCreateDirectory(currFolder, Path.Combine(tmpPlugins, "Delinker"), "DelinkerPlugin.dll");
                CopyAndCreateDirectory(currFolder, Path.Combine(tmpPlugins, "Fronds"), "Fronds.dll");

                string kingPath = Path.Combine(tmpPlugins, "Kingbotk");
                CopyAndCreateDirectory(currFolder, kingPath, "Kingbotk AWB Plugin.dll");

				currFolder = Path.Combine(Path.Combine(awbDir, "Plugins"), "Kingbotk");
                Copy(currFolder, kingPath, "Physics generic template.xml");
                Copy(currFolder, kingPath, "Film generic template.xml");
                Copy(currFolder, kingPath, "COPYING"); 

                Console.WriteLine("Files copied to temporary directory");

                new FastZip().CreateZip(filename, tmp, true, null);

                Directory.Delete(tmp, true);

                Console.WriteLine("Finished...");
                Console.WriteLine("Zip Created: " + filename);

                Console.Write("Press any key to exit..");
                while (!Console.KeyAvailable)
                {
                    System.Threading.Thread.Sleep(100);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
        }

        static void Copy(string fromPath, string toPath, string filename)
        {
            File.Copy(Path.Combine(fromPath, filename), Path.Combine(toPath, filename), true);
        }

        static void CopyAndCreateDirectory(string fromPath, string toPath, string filename)
        {
            Directory.CreateDirectory(toPath);
            Copy(fromPath, toPath, filename);
        }

        static int StringToVersion(string version)
        {
            int res;
            if (!int.TryParse(version.Replace(".", ""), out res))
                res = 0;

            return res;
        }

        static void CreateTextFile(string fileName, string content)
        {
            using (TextWriter tw = new StreamWriter(fileName))
            {
                tw.WriteLine(content);
            }
        }
    }
}
