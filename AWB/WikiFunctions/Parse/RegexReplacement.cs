/*

Copyright (C) 

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

namespace WikiFunctions.Parse
{
    struct RegexReplacement
    {
        public RegexReplacement(Regex regex, string replacement)
        {
            Regex = regex;
            Replacement = replacement;
        }

        public RegexReplacement(string pattern, RegexOptions options, string replacement)
            : this(new Regex(pattern, options), replacement)
        {
        }

        public RegexReplacement(string pattern, string replacement)
            : this(pattern, RegexOptions.Compiled, replacement)
        {
        }

        public readonly Regex Regex;
        public readonly string Replacement;
        // This could get extended to be a class with some of the Regex methods, such as IsMatch, Replace, etc, but there's little point
    }
}