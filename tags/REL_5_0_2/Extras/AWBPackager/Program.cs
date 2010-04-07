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
                string tmp = awbDir + "\\temp\\";
                awbDir = awbDir.Remove(awbDir.IndexOf("Extras"));
                Directory.CreateDirectory(tmp);

                if (selection == 1)
                {
                    using (StreamReader reader = new StreamReader(awbDir + "\\WikiFunctions\\SvnInfo.cs"))
                    {
                        string text = reader.ReadToEnd();
                        filename += "_rev" + SvnVersion.Match(text).Groups[1].Value;

                        reader.Close();
                    }
                }

                filename += ".zip";

                string currFolder = awbDir + "AWB\\bin\\Release\\";

                File.Copy(currFolder + "AutoWikiBrowser.exe", tmp + "AutoWikiBrowser.exe", true);

                filename = string.Format(filename,
                                         StringToVersion(
                                             FileVersionInfo.GetVersionInfo(currFolder + "AutoWikiBrowser.exe").
                                                 FileVersion));

                File.Copy(currFolder + "AutoWikiBrowser.exe.config", tmp + "AutoWikiBrowser.exe.config", true);
                File.Copy(currFolder + "WikiFunctions.dll", tmp + "WikiFunctions.dll", true);
                File.Copy(currFolder + "AWBUpdater.exe", tmp + "AWBUpdater.exe", true);

                CreateTextFile(tmp + "Diff.dll", "This file is not used anymore, but is preserved for compatibility "
                    + "with older versions of AWBUpdater.");

                Directory.CreateDirectory(tmp + "Plugins\\");

                Directory.CreateDirectory(tmp + "Plugins\\CFD\\");
                File.Copy(currFolder + "CFD.dll", tmp + "Plugins\\CFD\\CFD.dll", true);

                Directory.CreateDirectory(tmp + "Plugins\\IFD\\");
                File.Copy(currFolder + "IFD.dll", tmp + "Plugins\\IFD\\IFD.dll", true);

                Directory.CreateDirectory(tmp + "Plugins\\NoLimitsPlugin\\");
                File.Copy(currFolder + "NoLimitsPlugin.dll", tmp + "Plugins\\NoLimitsPlugin\\NoLimitsPlugin.dll", true);

                Directory.CreateDirectory(tmp + "Plugins\\Yahoo Search Plugin\\");
                File.Copy(currFolder + "YahooSearchPlugin.dll", tmp + "Plugins\\Yahoo Search Plugin\\YahooSearchPlugin.dll", true);

                Directory.CreateDirectory(tmp + "Plugins\\Bing Search Plugin\\");
                File.Copy(currFolder + "BingSearchPlugin.dll", tmp + "Plugins\\Bing Search Plugin\\BingSearchPlugin.dll", true);

                Directory.CreateDirectory(tmp + "Plugins\\TypoScan Plugin\\");
                File.Copy(currFolder + "TypoScan.dll", tmp + "Plugins\\TypoScan Plugin\\TypoScan.dll", true);

                Directory.CreateDirectory(tmp + "Plugins\\Delinker\\");
                File.Copy(currFolder + "DelinkerPlugin.dll", tmp + "Plugins\\Delinker\\DelinkerPlugin.dll", true);

                Directory.CreateDirectory(tmp + "Plugins\\Fronds\\");
                File.Copy(currFolder + "Fronds.dll", tmp + "Plugins\\Fronds\\Fronds.dll", true);

                Directory.CreateDirectory(tmp + "Plugins\\Kingbotk\\");
                currFolder = awbDir + "Plugins\\Kingbotk\\";

                File.Copy(currFolder + "Physics generic template.xml", tmp + "Plugins\\Kingbotk\\Physics generic template.xml", true);
                File.Copy(currFolder + "Film generic template.xml", tmp + "Plugins\\Kingbotk\\Film generic template.xml", true);
                File.Copy(currFolder + "COPYING", tmp + "Plugins\\Kingbotk\\COPYING", true);

                currFolder += "AWB Plugin\\bin\\Release\\";

                File.Copy(currFolder + "Kingbotk AWB Plugin.dll", tmp + "Plugins\\Kingbotk\\Kingbotk AWB Plugin.dll", true);

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
