using System;
using System.Collections.Generic;
using System.Text;
using java.util;
using java.util.zip;
using java.io;

namespace AWBUpdater
{
    //Code from: http://www.dotnetbips.com/articles/displayarticledetails.aspx?articleid=526
    //Needed Items only Included
    class zipHelper
    {
        public static void ExtractZipFile(string zipfilename, string destination)
        {
            ZipFile zipfile = new ZipFile(zipfilename);
            List<ZipEntry> entries = GetZippedItems(zipfile);
            foreach (ZipEntry entry in entries)
            {
                if (!entry.isDirectory())
                {
                    InputStream s = zipfile.getInputStream(entry);
                    try
                    {
                        string fname = System.IO.Path.GetFileName(entry.getName());
                        string dir = System.IO.Path.GetDirectoryName(entry.getName());
                        string newpath = destination + @"\" + dir;
                        System.IO.Directory.CreateDirectory(newpath);
                        FileOutputStream dest = new FileOutputStream
                        (System.IO.Path.Combine(newpath, fname));
                        try
                        {
                            CopyStream(s, dest);
                        }
                        finally
                        {
                            dest.close();
                        }
                    }
                    finally
                    {
                        s.close();
                    }
                }
            }
        }

        private static List<ZipEntry> GetZippedItems(ZipFile file)
        {
            List<ZipEntry> entries = new List<ZipEntry>();
            Enumeration e = file.entries();
            while (true)
            {
                if (e.hasMoreElements())
                {
                    ZipEntry entry = (ZipEntry)e.nextElement();
                    entries.Add(entry);
                }
                else
                {
                    break;
                }
            }
            return entries;
        }

        private static void CopyStream(InputStream source, OutputStream destination)
        {
            sbyte[] buffer = new sbyte[8000];
            int data;
            while (true)
            {
                try
                {
                    data = source.read(buffer, 0, buffer.Length);
                    if (data > 0)
                    {
                        destination.write(buffer, 0, data);
                    }
                    else
                    {
                        return;
                    }
                }
                catch (Exception ex)
                {
                    string msg = ex.Message;
                }
            }
        }
    }
}
