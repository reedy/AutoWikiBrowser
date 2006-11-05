using System;
using System.Xml.Serialization;


namespace IRCMonitor
{
    [Serializable, XmlRoot("IRCMonitorPreferences")]
    public class ProjectSettings
    {
        public ProjectSettings()
        {
            SetToEnglish();
        }

        public string Using;
        public string RevertSummary;

        public string ReportURL;
        public string ReportSummary;
        public string ReportAnonTemplate;
        public string ReportRegisteredTemplate;

        public string[] WarningTemplates;
        public string WarningSummary;

        public void SetToEnglish()
        {
            Using = " using [[WP:IRCM|IRCM]]";

            ReportURL = "http://en.wikipedia.org/w/index.php?title=Wikipedia:Administrator_intervention_against_vandalism&action=edit&section=2";
            ReportSummary = "Reporting [[Special:Contributions/%v|%v]] ([[User talk:%v|talk]])";

            RevertSummary = "Reverted edits by [[Special:Contributions/%v|%v]] ([[User talk:%v|talk]]) to last version by %u";
                
            WarningTemplates = new string[] 
            {
                "Simple vandalism",
                "*{{test1}}",
                "*{{test2}}",
                "*{{test3}}",
                "*{{test4}}",
                "*{{test4im}}",
                "*{{blatantvandal}}",
                "*{{test5}}",
                "*{{test6}}",
                "Blanking",
                "*{{blank1}}",
                "*{{blank2}}",
                "*{{blank3}}",
                "*{{blank4}}",
                "*{{blank5}}",
                "Removing content",
                "*{{test1a}}",
                "*{{test2a}}",
                "*{{test2del}}",
                "*{{test3a}}",
                "*{{test4a}}",
                "Userpage vandalism",
                "*{{tpv1}}",
                "*{{tpv2}}",
                "*{{tpv3}}",
                "*{{tpv4}}",
                "*{{tpv5}}",
                "Linkspam",
                "*{{welcomespam}}",
                "*{{spam0}}",
                "*{{spam1}}",
                "*{{spam2}}",
                "*{{spam2a}}",
                "*{{spam3}}",
                "*{{spam4}}",
                "*{{spam4im}}",
                "*{{spam5}}",
                "*{{spam5i}}",
                "Personal attacks",
                "*{{npa2}}",
                "*{{npa3}}",
                "*{{npa4}}",
                "*{{npa5}}",
                "*{{npa6}}",
                "Introducing deliberate factual errors",
                "*{{verror}}",
                "*{{verror2}}",
                "*{{verror3}}",
                "*{{verror4}}",
                "Using improper humor",
                "*{{behave}}",
                "*{{joke}}",
                "*{{funnybut}}",
                "*{{seriously}}",
                "Removing {{afd}} templates",
                "*{{drmafd}}",
                "*{{drmafd2}}",
                "*{{drmafd3}}",
                "*{{drmafd4}}",
                "*{{drmafd}}",
            };

            ReportAnonTemplate = "IPvandal";
            ReportRegisteredTemplate = "vandal";
            WarningSummary = "Warned user with %t";
        }
    };
};