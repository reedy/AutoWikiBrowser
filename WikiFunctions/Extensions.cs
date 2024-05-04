using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace WikiFunctions
{
    public static class Extensions
    {
        public static void AddIfTrue(this Dictionary<string, string> dict, bool input, string key, string value)
        {
            if (input)
            {
                dict.Add(key, value);
            }
        }

        public static void AddRangeIfNotNull<T>(this List<T> list, IEnumerable<T> collection)
        {
            if (collection != null)
            {
                list.AddRange(collection);
            }

        }

        public static bool IsIn<T>(this T @this, params T[] possibles)
        {
            return ((IList) possibles).Contains(@this);
        }

        /// <summary>
        /// Moves the caret to a specific line of a textbox
        /// </summary>
        /// <param name="t"></param>
        /// <param name="lineNumber"></param>
        public static void GoToLine(this TextBoxBase t, int lineNumber)
        {
            int i = 1;
            int intStart = 0;
            int intEnd = 0;

            foreach (Match m in Regex.Matches(t.Text, "^.*?$", RegexOptions.Multiline))
            {
                if (i == lineNumber)
                {
                    intStart = m.Index;
                    intEnd = intStart + m.Length;
                    break;
                }

                i++;
            }

            t.Select(intStart, intEnd - intStart);
            t.ScrollToCaret();
            t.Focus();
        }

        /// <summary>
        /// Resets any custom formatting of text (if copied from syntax highlighted text in edit box etc.),
        /// restoring cursor position
        /// </summary>
        public static void ResetFormatting(this RichTextBox rtb)
        {
            string a = rtb.Text;
            int i = rtb.SelectionStart;
            rtb.ResetText();
            rtb.Text = a;
            rtb.Select(i, 0);
        }

        public static IDictionary<TKey, TValue> SortBy<TKey, TValue>(
            this IDictionary<TKey, TValue> dictionary,
            IEnumerable<TKey> keys
        )
        {
            var sorter = new KeyComparer<TKey>(keys);
            return new SortedDictionary<TKey, TValue>(dictionary, sorter);
        }

        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static List<string> DistinctList(this JToken token)
        {
            return token.Select(item => item.ToString()).Distinct().ToList();
        }
    }
}
