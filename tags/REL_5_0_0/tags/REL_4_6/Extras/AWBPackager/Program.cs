/*
AWBPackager
Copyright (C) 2007 Sam Reed

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

using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace AWBPackager
{
    class Program
    {
        static string AWBDir = "";
        static string Tmp = "temp\\";

        static void Main()
        {
            try
            {
                string filename = "AWB.zip";

                Console.Write(@"Please ensure a release build has been built before continuing.
Is this SVN (1) or a release (2)? ");

                switch (int.Parse(Console.ReadLine()))
                {
                    case 1:
                        Console.Write("Please enter the current SVN revision: ");
                        string svnRev = Console.ReadLine();
                        filename = "AutoWikiBrowser_rev" + svnRev + ".zip";
                        break;
                    case 2:
                        Console.Write("Please enter the version: ");
                        string ver = Console.ReadLine();
                        filename = "AutoWikiBrowser" + ver.Replace(".", "") + ".zip";
                        break;
                }

                AWBDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).Replace("file:\\", "");
                Tmp = AWBDir + "\\" + Tmp;
                AWBDir = AWBDir.Remove(AWBDir.IndexOf("Extras"));
                Directory.CreateDirectory(Tmp);

                string currFolder = AWBDir + "AWB\\bin\\Release\\";

                File.Copy(currFolder + "AutoWikiBrowser.exe", Tmp + "AutoWikiBrowser.exe", true);
                File.Copy(currFolder + "AutoWikiBrowser.exe.config", Tmp + "AutoWikiBrowser.exe.config", true);
                File.Copy(currFolder + "WikiFunctions.dll", Tmp + "WikiFunctions.dll", true);
                File.Copy(currFolder + "AWBUpdater.exe", Tmp + "AWBUpdater.exe", true);
                File.Copy(currFolder + "Diff.dll", Tmp + "Diff.dll", true);

                Directory.CreateDirectory(Tmp + "Plugins\\");

                Directory.CreateDirectory(Tmp + "Plugins\\CFD\\");
                File.Copy(currFolder + "CFD.dll", Tmp + "Plugins\\CFD\\CFD.dll", true);

                Directory.CreateDirectory(Tmp + "Plugins\\IFD\\");
                File.Copy(currFolder + "IFD.dll", Tmp + "Plugins\\IFD\\IFD.dll", true);

                Directory.CreateDirectory(Tmp + "Plugins\\NoLimitsPlugin\\");
                File.Copy(currFolder + "NoLimitsPlugin.dll", Tmp + "Plugins\\NoLimitsPlugin\\NoLimitsPlugin.dll", true);

                Directory.CreateDirectory(Tmp + "Plugins\\Yahoo Search Plugin\\");
                File.Copy(currFolder + "YahooSearchPlugin.dll", Tmp + "Plugins\\Yahoo Search Plugin\\YahooSearchPlugin.dll", true);
				
				Directory.CreateDirectory(Tmp + "Plugins\\TypoScan Plugin\\");
                File.Copy(currFolder + "TypoScan.dll", Tmp + "Plugins\\TypoScan Plugin\\TypoScan.dll", true);

                Directory.CreateDirectory(Tmp + "Plugins\\Delinker\\");
                File.Copy(currFolder + "DelinkerPlugin.dll", Tmp + "Plugins\\Delinker\\DelinkerPlugin.dll", true);

                Directory.CreateDirectory(Tmp + "Plugins\\Fronds\\");
                File.Copy(currFolder + "Fronds.dll", Tmp + "Plugins\\Fronds\\Fronds.dll", true);

                Directory.CreateDirectory(Tmp + "Plugins\\Kingbotk\\");
                currFolder = AWBDir + "Plugins\\Kingbotk\\";

                File.Copy(currFolder + "Physics generic template.xml", Tmp + "Plugins\\Kingbotk\\Physics generic template.xml", true);
                File.Copy(currFolder + "Film generic template.xml", Tmp + "Plugins\\Kingbotk\\Film generic template.xml", true);
                File.Copy(currFolder + "COPYING", Tmp + "Plugins\\Kingbotk\\COPYING", true);

                currFolder += "AWB Plugin\\bin\\Release\\";

                File.Copy(currFolder + "Kingbotk AWB Plugin.dll", Tmp + "Plugins\\Kingbotk\\Kingbotk AWB Plugin.dll", true);

                Console.WriteLine("Files copied to temporary directory");

                FastZip zip = new FastZip();

                zip.CreateZip(filename, Tmp, true, null);

                Directory.Delete(Tmp, true);

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
    }
}
