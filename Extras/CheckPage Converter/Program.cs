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
            var profile = AWBProfiles.GetProfile(1);
            UpdateWiki("https://en.wikipedia.org/w/", profile.Username, profile.Password);
        }

        static void UpdateWiki(string url, string username, string password)
        {
            var origCheckPageText =
                Tools.GetHTML(
                    url + "index.php?title=Wikipedia:AutoWikiBrowser/CheckPage&action=raw");

            var enabledUsers = Tools.StringBetween(origCheckPageText, "<!--enabledusersbegins-->",
                                                    "<!--enabledusersends-->");

            var botUsers = Tools.StringBetween(enabledUsers, "<!--enabledbots-->", "<!--enabledbotsends-->");

            var normalUsers = enabledUsers.Replace("<!--enabledbots-->" + botUsers + "<!--enabledbotsends-->", "");

            Regex usernameRegex = new Regex(@"^\*\s*(.*?)\s*$", RegexOptions.Multiline | RegexOptions.Compiled);

            List<string> users = new List<string>();
            foreach (Match m in usernameRegex.Matches(normalUsers))
            {
                users.Add(m.Groups[1].Value.Trim());
            }

            users.Sort();

            List<string> bots = new List<string>();
            foreach (Match m in usernameRegex.Matches(botUsers))
            {
                bots.Add(m.Groups[1].Value.Trim());
            }

            bots.Sort();

            Dictionary<string, List<string>> checkPageOutput = new Dictionary<string, List<string>> {
                { "enabledusers", users },
                { "enabledbots", bots }
            };

            ApiEdit edit = new ApiEdit(url);

            
            edit.Login(username, uassword);

            edit.Open("Project:AutoWikiBrowser/CheckPageJSON");
            edit.Save(JsonConvert.SerializeObject(checkPageOutput, Formatting.Indented), "Converting from non json page", false, WatchOptions.NoChange);

            // Site Config stuff
            Dictionary<string, object> configOutput = new Dictionary<string, object>();

            Match typoLink = Regex.Match(origCheckPageText, "<!--[Tt]ypos:(.*?)-->");
            configOutput.Add("typolink", typoLink.Success && typoLink.Groups[1].Value.Trim().Length > 0 ? typoLink.Groups[1].Value.Trim() : "");

            configOutput.Add("allusersenabled", origCheckPageText.Contains("<!--All users enabled-->"));

            configOutput.Add("allusersenabledusermode", origCheckPageText.Contains("<!--All users enabled user mode-->"));

            List<Dictionary<string, string>> awbMessages = new List<Dictionary<string, string>>();

            // see if there is a message
            foreach (Match m in Message.Matches(origCheckPageText))
            {
                if (m.Groups[1].Value.Trim().Length == 0)
                {
                    continue;
                }

                awbMessages.Add(new Dictionary<string, string> {
                    { "version", "*" },
                    { "message", m.Groups[1].Value.Trim() }
                });
            }

            // see if there is a version-specific message
            foreach (Match m in VersionMessage.Matches(origCheckPageText))
            {
                if (m.Groups[2].Value.Trim().Length == 0 || m.Groups[1].Value == "x.x.x.x")
                {
                    continue;
                }

                awbMessages.Add(new Dictionary<string, string> {
                    { "version", m.Groups[1].Value },
                    { "message", m.Groups[2].Value.Trim() }
                });
            }

            configOutput.Add("messages", awbMessages);

            List<string> us = new List<string>();
            foreach (Match underscore in Underscores.Matches(origCheckPageText))
            {
                if (underscore.Success && underscore.Groups[1].Value.Trim().Length > 0)
                {
                    us.Add(underscore.Groups[1].Value.Trim());
                }
            }

            us.Sort();
            configOutput.Add("underscoretitles", us);

            List<string> NoParse = new List<string>();

            // Get list of articles not to apply general fixes to.
            Match noGenFix = WikiRegexes.NoGeneralFixes.Match(origCheckPageText);
            if (noGenFix.Success)
            {
                foreach (Match link in WikiRegexes.UnPipedWikiLink.Matches(noGenFix.Value))
                {
                    if (!NoParse.Contains(link.Groups[1].Value))
                    {
                        NoParse.Add(link.Groups[1].Value);
                    }
                }
            }

            NoParse.Sort();
            configOutput.Add("nogenfixes", NoParse);

            List<string> NoRetf = new List<string>();

            // Get list of articles not to apply RETF to.
            Match noRETF = WikiRegexes.NoRETF.Match(origCheckPageText);
            if (noRETF.Success)
            {
                foreach (Match link in WikiRegexes.UnPipedWikiLink.Matches(noRETF.Value))
                {
                    if (!NoRetf.Contains(link.Groups[1].Value))
                    {
                        NoRetf.Add(link.Groups[1].Value);
                    }
                }
            }

            NoRetf.Sort();
            configOutput.Add("noregextypofix", NoRetf);

            edit.Open("Project:AutoWikiBrowser/Config");
            edit.Save(JsonConvert.SerializeObject(configOutput, Formatting.Indented), "Converting from non json page", false, WatchOptions.NoChange);
        }
    }
}
