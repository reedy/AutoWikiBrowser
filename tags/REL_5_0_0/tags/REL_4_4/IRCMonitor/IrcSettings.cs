/*

Copyright (C) 2007 Martin Richards

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
using System.Xml.Serialization;
using System.Resources;
using System.IO;
using WikiFunctions;


namespace IRCM
{
    [Serializable, XmlRoot("IRCMonitorPreferences")]
    public class ProjectSettings
    {
        public ProjectSettings()
        {
            //SetToEnglish();
        }

        private string Using1;
        public string Using
        {
            get { return Using1; }
            set { Using1 = value; }
        }

        private string RevertSummary1;
        public string RevertSummary
        {
            get { return RevertSummary1; }
            set { RevertSummary1 = value; }
        }

        private string ReportUrl1;
        public string ReportUrl
        {
            get { return ReportUrl1; }
            set { ReportUrl1 = value; }
        }

        private string ReportSummary1;
        public string ReportSummary
        {
            get { return ReportSummary1; }
            set { ReportSummary1 = value; }
        }

        private string ReportAnonTemplate1;
        public string ReportAnonTemplate
        {
            get { return ReportAnonTemplate1; }
            set { ReportAnonTemplate1 = value; }
        }

        private string ReportRegisteredTemplate1;
        public string ReportRegisteredTemplate
        {
            get { return ReportRegisteredTemplate1; }
            set { ReportRegisteredTemplate1 = value; }
        }

        public string[] WarningTemplates;

        private string WarningSummary1;
        public string WarningSummary
        {
            get { return WarningSummary1; }
            set { WarningSummary1 = value; }
        }

        private string AppendedTagSummary1;
        public string AppendedTagSummary
        {
            get { return AppendedTagSummary1; }
            set { AppendedTagSummary1 = value; }
        }

        private string PrependedTagSummary1;
        public string PrependedTagSummary
        {
            get { return PrependedTagSummary1; }
            set { PrependedTagSummary1 = value; }
        }

        //private string[] StubTypes;
        public string[] PageTags;

        public string AppendTag(string pageContent, string tagToAdd, out string summary)
        {
            pageContent += "\r\n" + tagToAdd;
            summary = AppendedTagSummary.Replace("%1", tagToAdd);
            return pageContent;
        }

        public string PrependTag(string pageContent, string tagToAdd, out string summary)
        {
            pageContent = tagToAdd + "\r\n" + pageContent;
            summary = PrependedTagSummary.Replace("%1", tagToAdd);
            return pageContent;
        }

        public static string[] LoadStubs(string fileName, int initialLevel)
        {
            string[] stubs = new string[] { };

            try
            {
                StreamReader sr = new StreamReader(fileName);
                string contents = sr.ReadToEnd();
                stubs = contents.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                int level = initialLevel;
                for (int i = 0; i < stubs.Length; i++)
                {
                    string s = stubs[i];
                    s = s.TrimEnd(new char[] { '=', ' ' });
                    if (s[0] == '=')
                    {
                        level = s.LastIndexOf('=') + 1;
                        s = s.TrimStart(new char[] { '=', ' ' });
                    }

                    s = "".PadLeft(level - initialLevel, '*') + s;
                    stubs[i] = s;
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
            }

            return stubs;
        }

        public static ProjectSettings LoadPrefs(string file)
        {
            try
            {
                using (FileStream fStream = new FileStream(file, FileMode.Create))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(ProjectSettings));
                    return (ProjectSettings)xs.Deserialize(new StringReader(file));
                }
            }
            catch { throw; }
        }

        public static void SavePrefs(ProjectSettings prefs, string file)
        {
            try
            {
                using (FileStream fStream = new FileStream(file, FileMode.Create))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(ProjectSettings));
                    xs.Serialize(fStream, prefs);
                }
            }
            catch { throw; }
        }
    }

    public class ENWikipediaSettings : ProjectSettings
    {
        public ENWikipediaSettings()
        {
            Using = " using [[WP:IRCM|IRCM]]";

            ReportUrl = "http://en.wikipedia.org/w/index.php?title=Wikipedia:Administrator_intervention_against_vandalism&action=edit&section=2";
            ReportSummary = "Reporting [[Special:Contributions/%v|%v]] ([[User talk:%v|talk]])";

            RevertSummary = "Reverted edits by [[Special:Contributions/%v|%v]] ([[User talk:%v|talk]]) to last version by %u";

            WarningTemplates = new string[] 
            {
                "Simple vandalism",
                "*[0]{{test0}} - Just to inform that some actions could be considered vandalism",
                "*{{test1}} - AGF: Thank you for experimenting with Wikipedia",
                "*{{test2}} - Please do not add nonsense to Wikipedia. It is considered vandalism",
                "*{{test3}} - Please stop",
                "*{{test4}} - This is your last warning",
                "*{{test4im}} - This is the only warning you will receive",
                "*{{blatantvandal}} - You may be blocked from editing without further warning",
                "*{{test5}} - You have been temporarily blocked from editing",
                "*{{test6}} - You have been blocked from editing Wikipedia for repeated vandalism",
                "Blanking",
                "*{{blank1}} - Please do not replace Wikipedia pages with blank content",
                "*{{blank2}} - It is considered vandalism",
                "*{{blank3}} - If you continue to blank pages, you will be blocked from editing Wikipedia",
                "*{{blank4}} - This is your last warning",
                "*{{blank5}} - You have been blocked from editing Wikipedia for blanking pages",
                "Removing content",
                "*{{test1a}} - Your recent edit removed content from an article",
                "*{{test2a}} - Please do not remove content from Wikipedia. It is considered vandalism",
                "*{{test2del}} - Please do not delete sections of text or valid links. It is considered vandalism",
                "*{{test3a}} - If you continue to remove content from pages, you will be blocked",
                "*{{test4a}} - This is your last warning",
                "Userpage vandalism",
                "*{{tpv1}} - Please do not edit the user pages of other contributors without their approval",
                "*{{tpv2}} - Please do not target one or more user's pages or talk pages for abuse or insults",
                "*{{tpv3}} - If you continue you will be blocked",
                "*{{tpv4}} - This is your last warning",
                "*{{tpv5}} - You have been temporarily blocked",
                "Linkspam",
                "*{{welcomespam}} - Welcoming + links to policies",
                "*{{spam0}} - The commercial links/content you added were inappropriate",
                "*{{spam1}} - Please do not add inappropriate external links to Wikipedia",
                "*{{spam2}} - Please stop adding inappropriate external links to Wikipedia",
                "*{{spam2a}} - Adding unrelated external links to articles is considered vandalism",
                "*{{spam3}} - If you continue spamming you will be blocked",
                "*{{spam4}} - This is your last warning",
                "*{{spam4im}} - This is the only warning you will receive",
                "*{{spam5}} - You've been temporarily blocked for continuing to add spam links",
                "*{{spam5i}} - You have been blocked indefinitely for continuing to add spam links",
                "Personal attacks",
                "*{{npa2}} - First warning",
                "*{{npa3}} - If you continue to make personal attacks on other people, you will be blocked",
                "*{{npa4}} - This is your last warning",
                "*{{npa5}} - You have been temporarily blocked from editing for disrupting Wikipedia",
                "*{{npa6}} - Block and severe warning",
                "Introducing deliberate factual errors",
                "*{{verror}} - Your test of deliberately adding incorrect information...",
                "*{{verror2}} - It is considered vandalism",
                "*{{verror3}} - If you continue, you will be blocked",
                "*{{verror4}} - This is your last warning",
                "Using improper humor",
                "*{{behave}} - Don't make joke edits",
                "*{{joke}} - Please keep your edits factual and neutral",
                "*{{funnybut}} - It is time to straighten up and make serious contributions",
                "*{{seriously}} - You might not get another warning before having a block imposed",
                "Creating inappropriate pages",
                "*{{test1article}} - Thank you for experimenting with Wikipedia",
                "*{{test2article}} - Please refrain from creating inappropriate pages",
                "*{{test3article}} - If you continue to create inappropriate pages, you will be blocked",
                "*{{test4article}} - This is your last warning",
                "*{{test5article}} - You've been blocked for continually creating inappropriate pages",
                "Removing speedy deletion templates",
                "*{{drmspeedy}} - Please do not remove speedy deletion tags",
                "*{{drmspeedy2}} - Please stop removing speedy deletion notices",
                "*{{drmspeedy3}} - If you continue to remove them, you will be blocked",
                "*{{drmspeedy4}} - This is your last warning",
                "*{{drmspeedy5}} - You have been temporarily blocked",
                "Removing {{afd}} templates",
                "*{{drmafd}} - Informing not to do it",
                "*{{drmafd2}} - Please stop removing...",
                "*{{drmafd3}} - If you continue to remove them, you will be blocked",
                "*{{drmafd4}} - This is your last warning",
                "*{{drmafd5}} - You have been temporarily blocked",
            };

            ReportAnonTemplate = "IPvandal";
            ReportRegisteredTemplate = "vandal";
            WarningSummary = "Warned user with %t";
            AppendedTagSummary = "Added %1";
            PrependedTagSummary = "Tagged with %1";

            //StubTypes = LoadStubs("enwiki.stubs.txt", 3);

            PageTags = new string[]
            {
                "Cleanup",
                "*{{subst:cleanup-now}}",
                "*{{cleanup-list}}",
                "*{{cleanup-image}}",
                "*{{disambig-cleanup}}",
                "*{{technical}}",
                "*{{cleanup-english}}",
                "*{{uncategorized}}",
                "*{{wikify}}",
                "Disputes",
                "*{{advert}}",
                "*{{Autobiography}}",
                "*{{citecheck}}",
                "*{{contradict}}",
                "*{{disputed}}",
                "*{{hoax}}",
                "*{{OriginalResearch}}",
                "*{{POV}}",
                "*{{POV-check}}",
                "*{{TotallyDisputed}}",
                "*{{weasel}}",
                "Notability",
                "*{{notability}}",
                "*{{notability|Biographies}}",
                "*{{notability|Books}}",
                "*{{notability|Companies}}",
                "*{{notability|Fiction}}",
                "*{{notability|Music}}",
                "*{{notability|Neologisms}}",
                "*{{notability|Numbers}}",
                "*{{notability|Web}}",
                "*{{notability|Notability}}",
                "Speedy deletion",
                "*{{db-nonsense}}",
                "*{{db-test}}",
                "*{{db-vandalism}}",
                "*{{db-pagemove}}",
                "*{{db-repost}}",
                "*{{db-banned}}",
                "*{{db-afd}}",
                "*{{db-g6}}",
                "*{{db-author}}",
                "*{{db-blanked}}",
                "*{{db-talk}}",
                "*{{db-attack}}",
                "*{{db-spam}}",
                "*{{db-empty}}",
                "*{{db-nocontext}}",
                "*{{db-foreign}}",
                "*{{db-nocontent}}",
                "*{{db-transwiki}}",
                "*{{db-bio}}",
                "*{{db-band}}",
                "*{{db-club}}",
                "*{{db-group}}",
                "*{{db-web}}",
                "*{{db-redirnone}}",
                "*{{db-rediruser}}",
                "*{{db-redirtypo}}",
                "*{{db-noimage}}",
                "*{{db-noncom}}",
                "*{{db-unksource}}",
                "*{{db-unfree}}",
                "*{{db-norat}}",
                "*{{db-badfairuse}}",
                "*{{db-catempty}}",
                "*{{db-catfd}}",
                "*{{db-userreq}}",
                "*{{db-nouser}}",
                "*{{db-disparage}}",
                "*{{db-emptyportal}}"
            };
        }
    }
}