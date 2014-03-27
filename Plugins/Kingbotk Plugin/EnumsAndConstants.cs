using System.Text.RegularExpressions;

//Copyright © 2008 Stephen Kennedy (Kingboyk) http://www.sdk-software.com/
//Copyright © 2008 Sam Reed (Reedy) http://www.reedyboy.net/

//This program is free software; you can redistribute it and/or modify it under the terms of Version 2 of the GNU General Public License as published by the Free Software Foundation.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

//You should have received a copy of the GNU General Public License Version 2 along with this program; if not, write to the Free Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA

namespace AutoWikiBrowser.Plugins.Kingbotk
{
    internal enum SkipResults
    {
        NotSet = -1,
        //Processed = 0
        SkipNoChange = 1,
        SkipBadTag,
        SkipRegex
    }

    internal enum Classification
    {
        Unassessed = 0,
        Stub,
        Start,
        C,
        B,
        GA,
        A,
        FA,
        NA,
        Dab,
        List,
        FL,
        Code = 100
    }

    internal enum Importance
    {
        Unassessed = 0,
        Low,
        Mid,
        High,
        Top,
        NA,
        Code = 100
    }

    internal enum ProcessTalkPageMode
    {
        Normal,
        ManualAssessment,
        NonStandardTalk
    }

    internal static class Constants
    {
        // Regular expression strings:
        internal const string TemplatePrefix = "\\{\\{\\s*(?<tl>template *:)?\\s*";
        // put "(?<!<nowiki[\n\r]*>\s*)" at the start to ignore nowiki, but was difficult to get secondchanceregex adapted (without becoming too strict) so I gave up. It seemed the engine was trying to do it's best to *avoid* matching this negative group
        internal const string conRegexpLeft = TemplatePrefix + "(?<tlname>";
        // put "\s(?!</nowiki[\n\r]*>)" at the end to ignore nowiki
        internal const string conRegexpRight =
            ")\\b\\s*(((\\|\\|*| \\||\\| |\\s*\\|\\s*(?<parm>[^}{|\\s=]*))\\s*)+(=\\s*(?<val>[^}{|\\n\\r]*?)\\s*)?)*\\}\\}\\s*";

        //")\b[^}]*"
        internal const string conRegexpRightNotStrict = ")\\b";

        internal const RegexOptions conRegexpOptions =
            RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture;

        // Identifiers:
        internal const string Biography = "Biography";
        internal const string conWikiPlugin = "[[WP:Plugin++|Plugin++]]";
        internal const string conWikiPluginBrackets = "(" + conWikiPlugin + ") ";
        internal const string conAWBPluginName = "Kingbotk Plugin";
        //Placeholders:
        internal const string conTemplatePlaceholder = "{{xxxTEMPLATExxx}}";
    }
}