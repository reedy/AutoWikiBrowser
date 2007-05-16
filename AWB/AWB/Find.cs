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

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using WikiFunctions;

namespace AutoWikiBrowser
{
    internal static class Find
    {
        public static Regex regexObj;
        public static Match matchObj;

        public static void resetFind()
        {
            regexObj = null;
            matchObj = null;
        }

        public static void find(string strRegex, bool isRegex, bool caseSensive, 
            System.Windows.Forms.TextBox txtEdit, string ArticleName)
        {
            string ArticleText = txtEdit.Text;

            RegexOptions regOptions;

            if (caseSensive)
                regOptions = RegexOptions.None;
            else
                regOptions = RegexOptions.IgnoreCase;

            strRegex = Tools.ApplyKeyWords(ArticleName, strRegex);

            if (!isRegex)
                strRegex = Regex.Escape(strRegex);

            if (matchObj == null || regexObj == null)
            {
                int findStart = txtEdit.SelectionStart;

                regexObj = new Regex(strRegex, regOptions);
                matchObj = regexObj.Match(ArticleText, findStart);
                txtEdit.SelectionStart = matchObj.Index;
                txtEdit.SelectionLength = matchObj.Length;
                txtEdit.Focus();
                txtEdit.ScrollToCaret();
                return;
            }
            else
            {
                if (matchObj.NextMatch().Success)
                {
                    matchObj = matchObj.NextMatch();
                    txtEdit.SelectionStart = matchObj.Index;
                    txtEdit.SelectionLength = matchObj.Length;
                    txtEdit.Focus();
                    txtEdit.ScrollToCaret();
                }
                else
                {
                    txtEdit.SelectionStart = 0;
                    txtEdit.SelectionLength = 0;
                    txtEdit.Focus();
                    txtEdit.ScrollToCaret();
                    resetFind();
                }
            }
        }
    }
}
