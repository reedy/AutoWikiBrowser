/*
Autowikibrowser
Copyright (C) 2007 Martin Richards
(C) 2007 Stephen Kennedy (Kingboyk) http://www.sdk-software.com/

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

using System.Text.RegularExpressions;
using WikiFunctions;

namespace AutoWikiBrowser
{
    internal static class Find
    {
        public static Regex RegexObj;
        public static Match MatchObj;

        public static void ResetFind()
        {
            RegexObj = null;
            MatchObj = null;
        }

        public static void Find1(string strRegex, bool isRegex, bool caseSensitive,
            System.Windows.Forms.TextBox txtEdit, string ArticleName)
        {
            string ArticleText = txtEdit.Text;

            RegexOptions regOptions = caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase;

            strRegex = Tools.ApplyKeyWords(ArticleName, strRegex);

            if (!isRegex)
                strRegex = Regex.Escape(strRegex);

            if (MatchObj == null || RegexObj == null)
            {
                int findStart = txtEdit.SelectionStart;

                RegexObj = new Regex(strRegex, regOptions);
                MatchObj = RegexObj.Match(ArticleText, findStart);
                txtEdit.SelectionStart = MatchObj.Index;
                txtEdit.SelectionLength = MatchObj.Length;
            }
            else
            {
                if (MatchObj.NextMatch().Success)
                {
                    MatchObj = MatchObj.NextMatch();
                    txtEdit.SelectionStart = MatchObj.Index;
                    txtEdit.SelectionLength = MatchObj.Length;
                }
                else
                {
                    txtEdit.SelectionStart = 0;
                    txtEdit.SelectionLength = 0;
                    ResetFind();
                }
            }
            txtEdit.Focus();
            txtEdit.ScrollToCaret();
        }
    }
}

