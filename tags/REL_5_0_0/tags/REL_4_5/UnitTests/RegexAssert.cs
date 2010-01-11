/* For full compatibility with NUnit, this source file is licensed under
 * the zlib license:
 * 
 * Copyright © 2008 Max Semenik
 * 
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1. The origin of this software must not be misrepresented; you must
 *    not claim that you wrote the original software. If you use this
 *    software in a product, an acknowledgment (see the following) in the
 *    product documentation is required.
 * 
 * 2. Altered source versions must be plainly marked as such, and must
 *    not be misrepresented as being the original software.
 * 
 * 3. This notice may not be removed or altered from any source
 *    distribution.
 */

using System.Text.RegularExpressions;
using NUnit.Framework;

namespace UnitTests
{
    public static class RegexAssert
    {
        #region IsMatch
        public static void IsMatch(string pattern, string input)
        {
            IsMatch(pattern, RegexOptions.None, input, "");
        }

        public static void IsMatch(string pattern, string input, string message)
        {
            IsMatch(pattern, RegexOptions.None, input, message);
        }

        public static void IsMatch(string pattern, RegexOptions options, string input)
        {
            IsMatch(pattern, options, input, "");
        }

        public static void IsMatch(string pattern, RegexOptions options, string input, string message)
        {
            IsMatch(new Regex(pattern, options), input, message);
        }

        public static void IsMatch(Regex regex, string input)
        {
            IsMatch(regex, input, "");
        }

        public static void IsMatch(Regex regex, string input, string message)
        {
            if(!regex.IsMatch(input)) throw new AssertionException(string.Format(
                "The string <{0}> does not match the given regex <{1}>{2}", input, regex,
                (message.Length == 0 ? "" : ": " + message)));
        }
        #endregion

        #region NoMatch
        public static void NoMatch(string pattern, string input)
        {
            NoMatch(pattern, RegexOptions.None, input, "");
        }

        public static void NoMatch(string pattern, string input, string message)
        {
            NoMatch(pattern, RegexOptions.None, input, message);
        }

        public static void NoMatch(string pattern, RegexOptions options, string input)
        {
            NoMatch(pattern, options, input, "");
        }

        public static void NoMatch(string pattern, RegexOptions options, string input, string message)
        {
            NoMatch(new Regex(pattern, options), input, message);
        }

        public static void NoMatch(Regex regex, string input)
        {
            NoMatch(regex, input, "");
        }

        public static void NoMatch(Regex regex, string input, string message)
        {
            AssertionHelper.Expect(!regex.IsMatch(input), "The string matches the given regex"
                + (message.Length == 0 ? "" : ": " + message));
        }
        #endregion

        #region Matches (single result)
        public static void Matches(string expected, string pattern, string input)
        {
            Matches(expected, pattern, RegexOptions.None, input, "");
        }

        public static void Matches(string expected, string pattern, string input, string message)
        {
            Matches(expected, pattern, RegexOptions.None, input, message);
        }

        public static void Matches(string expected, string pattern, RegexOptions options, string input, string message)
        {
            Matches(expected, new Regex(pattern, options), input, message);
        }

        public static void Matches(string expected, Regex regex, string input)
        {
            Matches(expected, regex, input, "");
        }

        public static void Matches(string expected, Regex regex, string input, string message)
        {
            string m = regex.Match(input).Value;
            if (m != expected) throw new AssertionException(string.Format("  Expected match: <{0}>, actual match: <{1}>{2}",
                expected, m, (message.Length == 0 ? "" : ": " + message)));
        }
        #endregion

        #region NotMatches (single result)
        public static void NotMatches(string expected, string pattern, string input)
        {
            NotMatches(expected, pattern, RegexOptions.None, input, "");
        }

        public static void NotMatches(string expected, string pattern, string input, string message)
        {
            NotMatches(expected, pattern, RegexOptions.None, input, message);
        }

        public static void NotMatches(string expected, string pattern, RegexOptions options, string input, string message)
        {
            NotMatches(expected, new Regex(pattern, options), input, message);
        }

        public static void NotMatches(string expected, Regex regex, string input)
        {
            NotMatches(expected, regex, input, "");
        }

        public static void NotMatches(string expected, Regex regex, string input, string message)
        {
            string m = regex.Match(input).Value;
            if (m == expected) throw new AssertionException(string.Format("  Regex unexpectedly matched <{0}>{1}",
                expected, (message.Length == 0 ? "" : ": " + message)));
        }
        #endregion

        #region Matches (multiple results)

        public static void Matches(Regex regex, string input, params string[] expected)
        {
            Matches("", regex, input, expected);
        }

        //FIXME:
        public static void Matches(string message, Regex regex, string input, params string[] expected)
        {
            //MatchCollection mc = regex.Matches(input);

            //int firstDifference = -1;
            //for (int i = 0; i < Math.Min(mc.Count, expected.Length) && firstDifference < 0; i++)
            //{
            //    if (mc[i].Value != expected[i]) firstDifference = i;
            //}

            //if (firstDifference >= 0 || mc.Count != expected.Length)
            //{
            //    string errorMessage = "";
            //    if (message.Length >= 0) errorMessage = message + "";
            //}
            CollectionAssert.AreEqual(expected, MatchesToStrings(regex.Matches(input)), message);
        }
        #endregion

        #region Helpers
        private static string[] MatchesToStrings(MatchCollection matches)
        {
            string[] strings = new string[matches.Count];
            for (int i = 0; i < strings.Length; i++)
            {
                strings[i] = matches[i].Value;
            }

            return strings;
        }
        #endregion
    }
}
