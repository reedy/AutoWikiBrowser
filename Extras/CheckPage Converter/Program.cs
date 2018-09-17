using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using WikiFunctions;
using WikiFunctions.API;
using WikiFunctions.Profiles;

namespace CheckPage_Converter
{
    class Program
    {
        private static readonly Regex Message = new Regex("<!--[Mm]essage:(.*?)-->", RegexOptions.Compiled);
        private static readonly Regex VersionMessage = new Regex("<!--VersionMessage:(.*?)\\|\\|\\|\\|(.*?)-->", RegexOptions.Compiled);
        private static readonly Regex Underscores = new Regex("<!--[Uu]nderscores:(.*?)-->", RegexOptions.Compiled);

        static void Main(string[] args)
        {
            var origCheckPageText =
                Tools.GetHTML(
                    "https://en.wikipedia.org/w/index.php?title=Wikipedia:AutoWikiBrowser/CheckPage&action=raw");

            var checkPageText = Tools.StringBetween(origCheckPageText, "<!--enabledusersbegins-->",
                                                    "<!--enabledusersends-->");

            var botUsers = Tools.StringBetween(checkPageText, "<!--enabledbots-->", "<!--enabledbotsends-->");

            var normalUsers = checkPageText.Replace("<!--enabledbots-->\r\n" + checkPageText + "\r\n<!--enabledbotsends-->", "");

            Regex username = new Regex(@"^\*\s*(.*?)\s*$", RegexOptions.Multiline | RegexOptions.Compiled);

            List<string> users = new List<string>();
            foreach (Match m in username.Matches(normalUsers))
            {
                users.Add(m.Groups[1].Value.Trim());
            }

            List<string> bots = new List<string>();
            foreach (Match m in username.Matches(botUsers))
            {
                bots.Add(m.Groups[1].Value.Trim());
            }

            Dictionary<string, List<string>> checkPageOutput = new Dictionary<string, List<string>> {
                { "enabledusers", users },
                { "enabledbots", bots }
            };


            ApiEdit edit = new ApiEdit("https://en.wikipedia.org/w/");
            var profile = AWBProfiles.GetProfile(1);
            edit.Login(profile.Username, profile.Password);

            edit.Open("Project:AutoWikiBrowser/CheckPageJSON");
            edit.Save(JsonConvert.SerializeObject(checkPageOutput, Formatting.Indented), "Converting from non json page", false, WatchOptions.NoChange);

            // Site Config stuff
            Dictionary<string, List<string>> configOutput = new Dictionary<string, List<string>>();

            Match typoLink = Regex.Match(checkPageText, "<!--[Tt]ypos:(.*?)-->");
            if (typoLink.Success && typoLink.Groups[1].Value.Trim().Length > 0)
            {
                // TODO: Flatten
                configOutput.Add("typolink", new List<string>{ typoLink.Groups[1].Value.Trim() });
            }

            // TODO: Flatten
            configOutput.Add("allusersenabled", new List<string> { checkPageText.Contains("<!--All users enabled-->").ToString() });

            // TODO: Flatten
            configOutput.Add("allusersenabledusermode", new List<string> { checkPageText.Contains("<!--All users enabled user mode-->").ToString() });

            List<string> us = new List<string>();
            foreach (Match underscore in Underscores.Matches(checkPageText))
            {
                if (underscore.Success && underscore.Groups[1].Value.Trim().Length > 0)
                    us.Add(underscore.Groups[1].Value.Trim());
            }
            configOutput.Add("underscoretitles", us);

            List<string> NoParse = new List<string>();

            // Get list of articles not to apply general fixes to.
            Match noGenFix = WikiRegexes.NoGeneralFixes.Match(checkPageText);
            if (noGenFix.Success)
            {
                foreach (Match link in WikiRegexes.UnPipedWikiLink.Matches(noGenFix.Value))
                    if (!NoParse.Contains(link.Groups[1].Value))
                        NoParse.Add(link.Groups[1].Value);
            }
            configOutput.Add("nogenfixes", NoParse);

            List<string> NoRetf = new List<string>();
            // Get list of articles not to apply RETF to.
            Match noRETF = WikiRegexes.NoRETF.Match(checkPageText);
            if (noRETF.Success)
            {
                foreach (Match link in WikiRegexes.UnPipedWikiLink.Matches(noRETF.Value))
                    if (!NoRetf.Contains(link.Groups[1].Value))
                        NoRetf.Add(link.Groups[1].Value);
            }
            configOutput.Add("noregextypofix", NoRetf);

            edit.Open("Project:AutoWikiBrowser/Config");
            edit.Save(JsonConvert.SerializeObject(configOutput, Formatting.Indented), "Converting from non json page", false, WatchOptions.NoChange);
        }
    }
}
