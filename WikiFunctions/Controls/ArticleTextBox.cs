/*
Copyright (C) 2009

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
using System.Drawing;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Windows.Forms;

namespace WikiFunctions.Controls
{
    /// <summary>
    /// Wrapped EditBox to conveniently manage the automatic summary reset conditions
    /// </summary>
    public class ArticleTextBox : RichTextBox
    {
        public ArticleTextBox()
        {
            LanguageOption = RichTextBoxLanguageOptions.DualFont;
            InitializeComponent();
        }

        bool Locked;

        public override string Text
        {
            get { return base.Text.Replace("\n", "\r\n"); }
            set
            {
                Locked = true;
                base.Text = value;
                Locked = false;
            }
        }
        
        public override string SelectedText
        {
            get { return base.SelectedText.Replace("\n", "\r\n"); }
            set
            {
                Locked = true;
                base.SelectedText = value;
                Locked = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string RawText { get { return base.Text; } }

        protected override void OnTextChanged(EventArgs e)
        {
            // Prohibits triggering the TextChanged event if the text is changed programmatically
            if (!Locked) base.OnTextChanged(e);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            //Bug fix for AutoWordSelection - http://msdn.microsoft.com/en-us/library/system.windows.forms.richtextbox.autowordselection.aspx
            if (!AutoWordSelection)
            {
                AutoWordSelection = true;
                AutoWordSelection = false;
            }
        }

        bool AutoKeyboardDisabled;

        protected override void OnEnter(EventArgs e)
        {
            // A hack for the annoying bug with this option being mysteriously enabled to switch
            // user's kb layout for no good reason. Probably, there is a better place for doing this, 
            // but can't figure out where.
            if (!AutoKeyboardDisabled)
            {
                LanguageOption &= ~RichTextBoxLanguageOptions.AutoKeyboard;
                AutoKeyboardDisabled = true;
            }

            base.OnEnter(e);
        }
        
        /// <summary>
        /// Forces pasted text to be in same font as edit box, without trailing newline
        /// </summary>
        public void PasteUnformatted()
        {
            int ss = SelectionStart, sl = SelectionLength;
            string newText = Regex.Replace(Clipboard.GetText(), @"(?m)\r?\n$", "");
            // where text already selected
            if(sl > 0)
            {
                base.Text = base.Text.Remove(ss, sl);
                Select(ss, sl);
            }
            
            // paste unformatted, newline trimmed text at current cursor position
            base.Text = base.Text.Insert(SelectionStart, newText);
            Select(ss + newText.Length, 0);
            ScrollToCaret();
        }

        private Regex RegexObj;
        private Match MatchObj;

        /// <summary>
        /// Resets the Find Objects
        /// </summary>
        public void ResetFind()
        {
            RegexObj = null;
            MatchObj = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strRegex"></param>
        /// <param name="isRegex"></param>
        /// <param name="caseSensitive"></param>
        /// <param name="articleName"></param>
        public void Find(string strRegex, bool isRegex, bool caseSensitive, string articleName)
        {
            string articleText = RawText;

            RegexOptions regOptions = caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase;

            strRegex = Tools.ApplyKeyWords(articleName, strRegex);

            if (!isRegex)
                strRegex = Regex.Escape(strRegex);

            if (MatchObj == null || RegexObj == null)
            {
                int findStart = SelectionStart;

                RegexObj = new Regex(strRegex, regOptions);
                MatchObj = RegexObj.Match(articleText, findStart);
                SelectionStart = MatchObj.Index;
                SelectionLength = MatchObj.Length;
            }
            else
            {
                if (MatchObj.NextMatch().Success)
                {
                    MatchObj = MatchObj.NextMatch();
                    SelectionStart = MatchObj.Index;
                    SelectionLength = MatchObj.Length;
                }
                else
                {
                    SelectionStart = 0;
                    SelectionLength = 0;
                    ResetFind();
                }
            }
            Focus();
            ScrollToCaret();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strRegex"></param>
        /// <param name="isRegex"></param>
        /// <param name="caseSensitive"></param>
        /// <param name="articleName"></param>
        public Dictionary<int, int> FindAll(string strRegex, bool isRegex, bool caseSensitive, string articleName)
        {
            Dictionary<int, int> found = new Dictionary<int, int>();

            if (string.IsNullOrEmpty(strRegex))
                return found;

            string articleText = RawText;

            strRegex = Tools.ApplyKeyWords(articleName, strRegex);

            if (!isRegex)
                strRegex = Regex.Escape(strRegex);

            RegexObj = new Regex(strRegex, caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase);
            foreach (Match m in RegexObj.Matches(articleText))
            {
                found.Add(m.Index, m.Length);
            }

            return found;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputIndex"></param>
        /// <param name="inputLength"></param>
        public void SetEditBoxSelection(int inputIndex, int inputLength)
        {
            if (inputIndex >= 0 && inputLength > 0 && (inputIndex + inputLength) <= TextLength)
            {
                SelectionStart = inputIndex;
                SelectionLength = inputLength;
            }
            ScrollToCaret();
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            DetectUrls = false;
            ResumeLayout(false);
        }

        /// <summary>
        /// Applies syntax highlighting to the input ArticleTextBox 
        /// </summary>
        /// <returns></returns>
        public void HighlightSyntax()
        {
            // reset background colour to avoid issues on re-parse
            SetEditBoxSelection(0, RawText.Length);
            SelectionBackColor = Color.White;
                        
            Font currentFont = SelectionFont;
            Font boldFont = new Font(currentFont.FontFamily, currentFont.Size, FontStyle.Bold);
            Font italicFont = new Font(currentFont.FontFamily, currentFont.Size, FontStyle.Italic);
            Font boldItalicFont = new Font(currentFont.FontFamily, currentFont.Size, FontStyle.Bold | FontStyle.Italic);

            // headings text in bold
            foreach (Match m in WikiRegexes.Headings.Matches(RawText))
            {
                SetEditBoxSelection(m.Groups[1].Index, m.Groups[1].Length);
                SelectionFont = boldFont;
            }

            // templates grey background
            foreach (Match m in WikiRegexes.NestedTemplates.Matches(RawText))
            {
                SetEditBoxSelection(m.Index, m.Length);
                SelectionBackColor = Color.LightGray;
            }

            // * items grey background
            foreach (Match m in WikiRegexes.StarRows.Matches(RawText))
            {
                SetEditBoxSelection(m.Index, m.Length);
                SelectionBackColor = Color.LightGray;

                SetEditBoxSelection(m.Groups[1].Index, m.Groups[1].Length);
                SelectionFont = boldFont;
            }

            // template names dark blue font
            foreach (Match m in WikiRegexes.TemplateName.Matches(RawText))
            {
                SetEditBoxSelection(m.Groups[1].Index, m.Groups[1].Length);
                SelectionColor = Color.DarkBlue;
            }

            // refs grey background
            foreach (Match m in WikiRegexes.Refs.Matches(RawText))
            {
                SetEditBoxSelection(m.Index, m.Length);
                SelectionBackColor = Color.LightGray;
            }

            // external links grey background, blue bold
            foreach (Match m in WikiRegexes.ExternalLinks.Matches(RawText))
            {
                SetEditBoxSelection(m.Index, m.Length);
                SelectionColor = Color.Blue;
                SelectionFont = boldFont;
            }

            // Image/file links green background
            foreach (Match m in WikiRegexes.FileNamespaceLink.Matches(RawText))
            {
                SetEditBoxSelection(m.Index, m.Length);
                SelectionBackColor = Color.LightGreen;
            }

            // italics
            foreach (Match m in WikiRegexes.Italics.Matches(RawText))
            {
                SetEditBoxSelection(m.Groups[1].Index, m.Groups[1].Length);
                SelectionFont = italicFont;
            }

            // bold  
            foreach (Match m in WikiRegexes.Bold.Matches(RawText))
            {
                // reset anything incorrectly done by italics earlier
                SetEditBoxSelection(m.Index, m.Length);
                SelectionFont = currentFont;

                SetEditBoxSelection(m.Groups[1].Index, m.Groups[1].Length);
                SelectionFont = boldFont;
            }

            // bold italics 
            foreach (Match m in WikiRegexes.BoldItalics.Matches(RawText))
            {
                // reset anything incorrectly done by italics/bold earlier
                SetEditBoxSelection(m.Index, m.Length);
                SelectionFont = currentFont;

                SetEditBoxSelection(m.Groups[1].Index, m.Groups[1].Length);
                SelectionFont = boldItalicFont;
            }

            // piped wikilink text in blue, piped part in bold
            foreach (Match m in WikiRegexes.PipedWikiLink.Matches(RawText))
            {
                SetEditBoxSelection(m.Groups[2].Index, m.Groups[2].Length);
                SelectionColor = Color.Blue;
                SelectionFont = boldFont;

                SetEditBoxSelection(m.Groups[1].Index, m.Groups[1].Length);
                SelectionColor = Color.Blue;
            }

            // unpiped wikilinks in blue and bold
            foreach (Match m in WikiRegexes.UnPipedWikiLink.Matches(RawText))
            {
                SetEditBoxSelection(m.Groups[1].Index, m.Groups[1].Length);
                SelectionColor = Color.Blue;
                SelectionFont = boldFont;
            }

            // pipe trick: in blue bold too
            foreach (Match m in WikiRegexes.WikiLinksOnlyPlusWord.Matches(RawText))
            {
                SetEditBoxSelection(m.Groups[1].Index, m.Groups[1].Length);
                SelectionColor = Color.Blue;
                SelectionFont = boldFont;
            }

            // cats grey background
            foreach (Match m in WikiRegexes.Category.Matches(RawText))
            {
                SetEditBoxSelection(m.Index, m.Length);
                SelectionBackColor = Color.LightGray;
                SelectionFont = currentFont;
                SelectionColor = Color.Black;

                SetEditBoxSelection(m.Groups[1].Index, m.Groups[1].Length);
                SelectionColor = Color.Blue;
            }

            // interwikis dark grey background
            foreach (Match m in WikiRegexes.PossibleInterwikis.Matches(RawText))
            {
                SetEditBoxSelection(m.Index, m.Length);
                SelectionBackColor = Color.Gray;
                SelectionFont = currentFont;

                SetEditBoxSelection(m.Groups[2].Index, m.Groups[2].Length);
                SelectionColor = Color.Blue;

                SetEditBoxSelection(m.Groups[1].Index, m.Groups[1].Length);
                SelectionColor = Color.Black;
            }

            // comments dark orange background
            foreach (Match m in WikiRegexes.Comments.Matches(RawText))
            {
                SetEditBoxSelection(m.Index, m.Length);
                SelectionBackColor = Color.PaleGoldenrod;
            }
        }
    }
}
