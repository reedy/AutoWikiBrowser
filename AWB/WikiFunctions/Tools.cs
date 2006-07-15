/*
WikiFunctions
Copyright (C) 2006 Martin Richards

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
using System.Net;
using System.Web;
using System.IO;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Collections;

namespace WikiFunctions
{
    /// <summary>
    /// Provides some static methods, such as getting the html of a page, 
    /// </summary>
    public static class Tools
    {
        /// <summary>
        /// Calculates the namespace index.
        /// </summary>
        public static int CalculateNS(string ArticleTitle)
        {
            if (!ArticleTitle.Contains(":"))
                return 0;

            foreach (KeyValuePair<int, string> k in Variables.Namespaces)
            {
                if (ArticleTitle.StartsWith(k.Value))
                    return k.Key;
            }

            return 0;
        }

        /// <summary>
        /// Tests title to make sure it is main space
        /// </summary>
        /// <param name="ArticleTitle">The title.</param>
        public static bool IsMainSpace(string ArticleTitle)
        {
            if (CalculateNS(ArticleTitle) == 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Tests title to make sure it is an editable namespace
        /// </summary>
        /// <param name="ArticleTitle">The title.</param>
        public static bool IsEditableSpace(string ArticleTitle)
        {
            if (ArticleTitle.StartsWith("Commons:"))
                return false;

            int i = CalculateNS(ArticleTitle);
            if (i < 0 || i > 99 || i == 7 || i == 8)
                return false;

            return true;
        }

        /// <summary>
        /// Tests title to make sure it is either main, image, category or template namespace.
        /// </summary>
        /// <param name="ArticleTitle">The title.</param>
        public static bool IsImportantNamespace(string ArticleTitle)
        {
            int i = CalculateNS(ArticleTitle);
            if (i == 0 || i == 6 || i == 10 || i == 14)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Tests title to make sure it is not a talk page.
        /// </summary>
        /// <param name="ArticleTitle">The title.</param>
        public static bool IsNotTalk(string ArticleTitle)
        {
            int i = CalculateNS(ArticleTitle);

            if (i % 2 == 1)
                return false;
            else return true;
        }

        /// <summary>
        /// Returns Category key from article name e.g. "David Smith" returns "Smith, David".
        /// </summary>
        public static string MakeHumanCatKey(string Name)
        {
            string OrigName = Name;

            Name = Regex.Replace(Name, "\\(.*?\\)$", "").Trim();

            if (!Name.Contains(" "))
                return OrigName;

            int intLast = Name.LastIndexOf(" ") + 1;
            string LastName = Name.Substring(intLast);
            Name = Name.Remove(intLast);
            Name = LastName + ", " + Name.Trim();

            return Name;
        }

        public static string ReadWebPage(string url)
        {
            WebRequest request = HttpWebRequest.Create(url);
            Stream stream = request.GetResponse().GetResponseStream();
            StreamReader sr = new StreamReader(stream);
            string webPage = sr.ReadToEnd();
            sr.Close();

            return webPage;
        }

        /// <summary>
        /// Gets the HTML from the given web address.
        /// </summary>
        /// <param name="URL">The URL of the webpage.</param>
        /// <returns>The HTML.</returns>
        public static string GetHTML(string URL)
        {
            return GetHTML(URL, Encoding.UTF8);
        }

        /// <summary>
        /// Gets the HTML from the given web address.
        /// </summary>
        /// <param name="URL">The URL of the webpage.</param>
        /// <param name="Enc">The ecoding to use.</param>
        /// <returns>The HTML.</returns>
        public static string GetHTML(string URL, Encoding Enc)
        {
            string text = "";
            try
            {
                HttpWebRequest rq = (HttpWebRequest)WebRequest.Create(URL);

                rq.Proxy.Credentials = CredentialCache.DefaultCredentials;
                rq.UserAgent = "WikiFunctions " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

                HttpWebResponse response = (HttpWebResponse)rq.GetResponse();

                Stream stream = response.GetResponseStream();
                StreamReader sr = new StreamReader(stream, Enc);

                text = sr.ReadToEnd();

                sr.Close();
                stream.Close();
                response.Close();

                return text;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Gets the wiki text of the given article.
        /// </summary>
        /// <param name="ArticleTitle">The name of the article.</param>
        /// <returns>The wiki text of the article.</returns>
        public static string GetArticleText(string ArticleTitle)
        {
            string text = GetHTML(Variables.URL + "index.php?title=" + ArticleTitle + "&action=raw&ctype=text/plain&dontcountme=s");
            return text;
        }

        [DllImport("user32.dll")]
        private static extern bool FlashWindow(IntPtr hwnd, bool bInvert);

        public static void FlashWindow(System.Windows.Forms.Form window)
        {
            FlashWindow(window.Handle, true);
        }

        public static string TurnFirstToUpper(string input)
        {//turns first character to uppercase
            //other projects don't always start with capital
            if (Variables.Project == "wiktionary")
                return input;

            if (input.Length == 0)
                return "";

            input = char.ToUpper(input[0]) + input.Substring(1, input.Length - 1);

            return input;
        }

        public static string linkChecker(string articleText)
        {//checks links to make them bypass redirects and (TODO) disambigs
            string link = "";
            string article = "";

            foreach (Match m in Regex.Matches(articleText, "(\\[\\[)(.*?)(\\]\\])"))
            {
                //make link
                link = m.Value;
                article = m.Groups[2].Value;
                //MessageBox.Show(link);

                //get text
                string text = "";
                text = GetArticleText(article);

                //test if redirect
                if (Regex.IsMatch(text, "^#Redirect:? ?", RegexOptions.IgnoreCase))
                {
                    string directLink = Regex.Match(text, "\\[\\[.*?\\]\\]").ToString().Replace("[[", "").Replace("]]", "");
                    directLink = "[[" + directLink + "|" + article + "]]";

                    articleText = articleText.Replace(link, directLink);
                }
            }
            return articleText;
        }

        /// <summary>
        /// Writes a message to the given file in the directory of the application.
        /// </summary>
        /// <param name="Message">The message to write.</param>
        /// <param name="File">The name of the file, e.g. "Log.txt".</param>
        public static void WriteLog(string Message, string File)
        {
            try
            {
                StreamWriter writer = new StreamWriter(Environment.CurrentDirectory + "\\" + File, true, Encoding.UTF8);
                writer.Write(Message);
                writer.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static string HTMLToWiki(string text, string bullet)
        {//converts wiki/html/plain text list to wiki formatted list, bullet should be * or #
            text = text.Replace("\r\n\r\n", "\r\n");
            text = Regex.Replace(text, "<br ?/?>", "", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "</?(ol|ul|li)>", "", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "^</?(ol|ul|li)>\r\n", "", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "^(<li>|\\:|\\*|#|\\(? ?[0-9]{1,3} ?\\)|[0-9]{1,3}\\.?)", "", RegexOptions.Multiline);
            text = Regex.Replace(text, "^", bullet, RegexOptions.Multiline);

            return text;
        }

        public static void Beep1()
        {//public domain sounds from http://www.partnersinrhyme.com/soundfx/PDsoundfx/beep.shtml
            System.Media.SoundPlayer sound = new System.Media.SoundPlayer();
            sound.Stream = MyResource.beep1;
            sound.Play();
        }
        public static void Beep2()
        {
            System.Media.SoundPlayer sound = new System.Media.SoundPlayer();
            sound.Stream = MyResource.beep2;
            sound.Play();
        }
    }
}
