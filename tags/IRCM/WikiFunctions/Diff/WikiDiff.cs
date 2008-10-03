using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using Algorithm.Diff;
using System.Text.RegularExpressions;
using System.Collections;

namespace WikiFunctions
{
    /// <summary>
    /// This class renders MediaWiki-like HTML diffs
    /// </summary>
    public class WikiDiff
    {
        string[] LeftLines;
        string[] RightLines;
        Diff diff;
        StringBuilder Result;
        int ContextLines;

        /// <summary>
        /// Renders diff
        /// </summary>
        /// <param name="leftText">Earlier version of the text</param>
        /// <param name="rightText">Later version of the text</param>
        /// <param name="contextLines">Number of unchanged lines to show around changed ones</param>
        /// <returns>HTML diff</returns>
        public string GetDiff(string leftText, string rightText, int contextLines)
        {
            Result = new StringBuilder(500000);
            LeftLines = leftText.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            RightLines = rightText.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            ContextLines = contextLines;

            diff = new Diff(LeftLines, RightLines, true, true);
            foreach (Diff.Hunk h in diff)
            {
                if (h.Same) RenderContext(h);
                else RenderDifference(h);

            }
            return Result.ToString();
        }

        #region High-level diff stuff
        void RenderDifference(Diff.Hunk hunk)
        {
            Range left = hunk.Left;
            Range right = hunk.Right;

            if (right.Start == 0) ContextHeader(0, 0);
            int changes = Math.Min(left.Count, right.Count);
            for (int i = 0; i < changes; i++)
            {
                LineChanged(left.Start + i, right.Start + i);
            }
            if (left.Count > right.Count)
                for (int i = changes; i < left.Count; i++)
                    LineDeleted(left.Start + i, right.Start + changes);
            else if (left.Count < right.Count)
                for (int i = changes; i < right.Count; i++)
                    LineAdded(right.Start + i);
        }

        void RenderContext(Diff.Hunk hunk)
        {
            Range left = hunk.Left;
            Range right = hunk.Right;
            int displayed = 0;

            if (Result.Length > 0) // not the first hunk, adding context for previous change
            {
                displayed = Math.Min(ContextLines, right.Count);
                for (int i = 0; i < displayed; i++) ContextLine(right.Start + i);
            }

            int toDisplay = Math.Min(right.Count - displayed, ContextLines);
            if ((left.End < LeftLines.Length - 1 || right.End < RightLines.Length - 1) && toDisplay > 0)
            // not the last hunk, adding context for next change
            {
                if (right.Count > displayed + toDisplay) ContextHeader(left.End - toDisplay + 1, right.End - toDisplay + 1);
                for (int i = 0; i < toDisplay; i++) ContextLine(right.End - toDisplay + i + 1);
            }
        }

        void LineChanged(int leftLine, int rightLine)
        {
            // some kind of glitch with the diff engine
            if (LeftLines[leftLine] == RightLines[rightLine])
            {
                ContextLine(rightLine);
                return;
            }

            StringBuilder left = new StringBuilder();
            StringBuilder right = new StringBuilder();

            List<Word> leftList = Word.SplitString(LeftLines[leftLine]);
            List<Word> rightList = Word.SplitString(RightLines[rightLine]);

            Diff diff = new Diff(leftList, rightList, Word.Comparer, Word.Comparer);

            foreach (Diff.Hunk h in diff)
            {
                if (h.Same)
                {
                    for (int i = 0; i < h.Left.Count; i++)
                    {
                        WhitespaceDiff(left, rightList[h.Right.Start + i], leftList[h.Left.Start + i]);
                        WhitespaceDiff(right, leftList[h.Left.Start + i], rightList[h.Right.Start + i]);
                    }
                }
                else
                {
                    WordDiff(left, h.Left, h.Right, leftList, rightList);

                    WordDiff(right, h.Right, h.Left, rightList, leftList);
                }
            }

            Result.AppendFormat(@"<tr onclick='window.external.GoTo({1})' ondblclick='window.external.UndoChange({0},{1})'>
  <td>-</td>
  <td class='diff-deletedline'>", leftLine, rightLine);
            Result.Append(left);
            Result.Append(@"  </td>
  <td>+</td>
  <td class='diff-addedline'>");
            Result.Append(right);
            Result.Append(@"  </td>
		</tr>");
        }

        static void WordDiff(StringBuilder res, Range range, Range otherRange, IList<Word> words, IList<Word> otherWords)
        {
            bool open = false;

            for (int i = 0; i < range.Count; i++)
            {
                if (i >= otherRange.Count || words[range.Start + i].ToString() != otherWords[otherRange.Start + i].ToString())
                {
                    if (!open) res.Append("<span class='diffchange'>");
                    open = true;
                }
                else
                {
                    if (open) res.Append("</span>");
                    open = false;
                }
                res.Append(HttpUtility.HtmlEncode(words[range.Start + i].ToString()));
            }

            if (open) res.Append("</span>");
        }

        static void WhitespaceDiff(StringBuilder res, Word left, Word right)
        {
            if (left.Whitespace == right.Whitespace) res.Append(HttpUtility.HtmlEncode(right.ToString()));
            else
            {
                res.Append(HttpUtility.HtmlEncode(right.TheWord));
                char[] leftChars = left.Whitespace.ToCharArray();
                char[] rightChars = right.Whitespace.ToCharArray();

                Diff diff = new Diff(leftChars, rightChars, Word.Comparer, Word.Comparer);
                foreach (Diff.Hunk h in diff)
                {
                    if (h.Same)
                        res.Append(rightChars, h.Right.Start, h.Right.Count);
                    else
                    {
                        res.Append("<span class='diffchange'>");
                        res.Append('\x00A0', h.Right.Count); // replace spaces with NBSPs to make 'em visible
                        res.Append("</span>");
                    }
                }
            }
        }
        #endregion

        #region Visualisation primitives
        /// <summary>
        /// Renders a context row
        /// </summary>
        /// <param name="line">Number of line in the RIGHT text</param>
        void ContextLine(int line)
        {
            string html = HttpUtility.HtmlEncode(RightLines[line]);
            Result.AppendFormat(@"<tr onclick='window.external.GoTo({0});'>
  <td class='diff-marker'> </td>
  <td class='diff-context'>", line);
            Result.Append(html);
            Result.Append(@"</td>
  <td class='diff-marker'> </td>
  <td class='diff-context'>");
            Result.Append(html);
            Result.Append(@"</td>
</tr>");
        }

        void LineDeleted(int left, int right)
        {
            Result.AppendFormat(@"<tr>
  <td>-</td>
  <td class='diff-deletedline' onclick='window.external.GoTo({1})' ondblclick='window.external.UndoDeletion({0}, {1})'>",
                left, right);
            Result.Append(HttpUtility.HtmlEncode(LeftLines[left]));
            Result.Append(@"  </td>
  <td> </td>
  <td> </td>
</tr>");
        }

        void LineAdded(int line)
        {
            Result.AppendFormat(@"<tr>
  <td> </td>
  <td> </td>
  <td>+</td>
  <td class='diff-addedline' onclick='window.external.GoTo({0})' ondblclick='window.external.UndoAddition({0})'>", line);
            Result.Append(HttpUtility.HtmlEncode(RightLines[line]));
            Result.Append(@"  </td>
</tr>");
        }

        void ContextHeader(int left, int right)
        {
            Result.AppendFormat(@"<tr onclick='window.external.GoTo({2})'>
  <td colspan='2' class='diff-lineno'>Line {0}:</td>
  <td colspan='2' class='diff-lineno'>Line {1}:</td>
</tr>", left + 1, right + 1, right);
        }
        #endregion

        #region Undo
        public string UndoChange(int left, int right)
        {
            RightLines[right] = LeftLines[left];

            return string.Join("\r\n", RightLines);
        }

        public string UndoAddition(int right)
        {
            StringBuilder s = new StringBuilder();

            for (int i = 0; i < RightLines.Length; i++)
                if (i != right)
                {
                    if (s.Length > 0) s.Append("\r\n");
                        s.Append(RightLines[i]);
                }

            //s.Remove("

            return s.ToString();
        }

        public string UndoDeletion(int left, int right)
        {
            StringBuilder s = new StringBuilder();

            for (int i = 0; i < RightLines.Length; i++)
            {
                if (i == right)
                {
                    if (s.Length > 0) s.Append("\r\n");
                    s.Append(LeftLines[left]);
                }
                if (s.Length > 0) s.Append("\r\n");
                s.Append(RightLines[i]);
            }

            if (left >= RightLines.Length)
            {
                if (s.Length > 0) s.Append("\r\n");
                s.Append(LeftLines[left]);
            }

            return s.ToString();
        }

        #endregion

        #region Static methods
        public static string TableHeader
        {
            get
            {
                return @"<p style='margin: 0px;'>Double click on a line to undo all changes on that line, or single click to focus the edit box to that line.</p>
<table id='wikiDiff' class='diff'>
<col class='diff-marker' />
<col class='diff-content' />
<col class='diff-marker' />
<col class='diff-content' />
<thead>
  <tr valign='top'>
    <td colspan='2' width='50%' class='diff-otitle'>Current revision</td>
	<td colspan='2' width='50%' class='diff-ntitle'>Your text</td>
  </tr>
</thead>
";
            }
        }

        public static string DefaultStyles
        {
            get
            {
                return @"
body {
	font-family: Arial, Helvetica, sans-serif;
	font-size:95%;
    margin-left:2px;
    margin-right:2px;
    margin-top: 0px;
    margin-bottom: 0px;
}

p {
    font-family:arial;
    size:75%;
}

table.diff {
	background:white;
    border:1px solid gray;
    width:100%;
}
table.diff td {
}
td.diff-otitle, td.diff-ntitle {
    border: 1px solid gray;
    text-align:center;
    font-weight:bold;
}
td.diff-lineno {
	font: bold 80%;
}
/* line display */
td.diff-addedline {
	background: #cfc;
	font-size: smaller;
}
td.diff-deletedline {
	background: #ffa;
	font-size: smaller;
}
td.diff-context {
	background: #eee;
	font-size: smaller;
}
.diffchange {
	color: red;
	font-weight: bold;
	text-decoration: none;
}

td.diff-deletedline span.diffchange {
    background-color: #FFD754; color:black;
}

td.diff-addedline span.diffchange {
    background-color: #73E5A1; color:black;
}

.d {
    overflow: auto;
}
";
            }
        }

        static string CustomStyles;

        public static string DiffHead()
        {
            string styles = DefaultStyles;

            if (!string.IsNullOrEmpty(CustomStyles))
                styles = CustomStyles;
            else if (System.IO.File.Exists("style.css") && CustomStyles == null)
            {
                try
                {
                    System.IO.StreamReader reader = System.IO.File.OpenText("style.css");
                    CustomStyles = reader.ReadToEnd();
                    styles = CustomStyles;
                }
                catch
                {
                    CustomStyles = "";
                }
            }

            return "<style type='text/css'>" + styles + "</style>";
        }

        public static void ResetCustomStyles()
        {
            CustomStyles = null;
        }
        #endregion
    }

    public sealed class Word
    {
        private string m_Word;
        private string m_Whitespace;
        private string m_ToString;
        private int m_HashCode;

        public string TheWord
        {
            get { return m_Word; }
        }

        public string Whitespace
        {
            get { return m_Whitespace; }
        }

        public Word(string word, string white)
        {
            m_Word = word;
            m_Whitespace = white;
            m_ToString = word + white;
            m_HashCode = (word/* + white*/).GetHashCode();
        }

        /// <summary>
        /// Too slow, don't use
        /// </summary>
        /// <param name="all">Word with whitespace</param>
        //public Word(string all)
        //    : this(Regex.Match(all, @"\S*").Value, Regex.Match(all, @"\s*").Value)
        //{
        //}

        public static WordComparer Comparer = new WordComparer();

        //static readonly Regex Splitter = new Regex(//@"([\p{Sm}\p{P}]+|[^\s\p{P}\p{Sm}]*)(\s*)",
        //    @"([\p{Sm}\p{P}]|[^\s\p{P}\p{Sm}]*)(\s*)", RegexOptions.Compiled);

        /// borrowed from wikidiff2 for consistency
        private static bool IsText(char ch)
        {
            // Standard alphanumeric
            if ((ch >= '0' && ch <= '9') ||
               (ch == '_') ||
               (ch >= 'A' && ch <= 'Z') ||
               (ch >= 'a' && ch <= 'z'))
            {
                return true;
            }
            // Punctuation and control characters
            if (ch < 0xc0) return false;
            // Thai, return false so it gets split up
            if (ch >= 0xe00 && ch <= 0xee7) return false;
            // Chinese/Japanese, same
            if (ch >= 0x3000 && ch <= 0x9fff) return false;
            //if (ch >= 0x20000 && ch <= 0x2a000) return false;
            // Otherwise assume it's from a language that uses spaces
            return true;
        }

        /// borrowed from wikidiff2 for consistency
        private static bool IsSpace(char ch)
        {
            return ch == ' ' || ch == '\t';
        }

        public static List<Word> SplitString(string s)
        {
            List<Word> lst = new List<Word>();

            int pos = 0;
            int len = s.Length;
            while (pos < len)
            {
                char ch = s[pos];
                int i = pos;

                // first group has three different opportunities:
                if (IsSpace(ch))
                {
                    // one or more whitespace characters
                    while (i < len && IsSpace(s[i])) i++;
                }
                else if (IsText(ch))
                {
                    // one or more text characters
                    while (i < len && IsText(s[i])) i++;
                }
                else
                {
                    // one character, no matter what it is
                    i++;
                }

                string wordBody = s.Substring(pos, i - pos);
                pos = i;

                // second group: any whitespace character
                while (i < len && IsSpace(s[i])) i++;
                string trail = s.Substring(pos, i - pos);

                lst.Add(new Word(wordBody, trail));
                pos = i;
            }

            return lst;
        }
        //{
        //    List<Word> lst = new List<Word>();

        //    foreach (Match m in Splitter.Matches(s))
        //        if (m.Value.Length > 0) lst.Add(new Word(m.Groups[1].Value, m.Groups[2].Value));

        //    return lst;
        //}

        #region Overrides
        public override string ToString()
        {
            return m_ToString;
        }

        public override bool Equals(object obj)
        {
            return TheWord.Equals(((Word)obj).TheWord);
        }

        public override int GetHashCode()
        {
            return m_HashCode;
        }
        #endregion
    }

#pragma warning disable 0618
    public class WordComparer : IComparer, IHashCodeProvider
    {
        public int GetHashCode(object obj)
        {
            return obj.GetHashCode();
        }

        public int Compare(object x, object y)
        {
            return x.Equals(y) ? 0 : 1;
        }
    }
}
