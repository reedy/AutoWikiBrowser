using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        private static readonly Regex UsernameRegex = new Regex(@"^\*\s*(.*?)\s*$", RegexOptions.Multiline | RegexOptions.Compiled);

        private const string PHAB_TASK = "https://phabricator.wikimedia.org/T241196";

        static void Main(string[] args)
        {
            Console.SetBufferSize(140, 2000);
            Console.SetWindowSize(140, 25);

            Console.WriteLine("AutoWikiBrowser CheckPage migration to structured JSON pages");
            Console.WriteLine();
            var profiles = AWBProfiles.GetProfiles();

            if (profiles.Count == 0)
            {
                Console.WriteLine("No profiles to use, open AutoWikiBrowser and create one!");
                Console.ReadLine();
                return;
            }

            Console.WriteLine("Profiles that can be used for migration:");
            foreach (var p in profiles)
            {
                Console.WriteLine("{0}) {1}", p.ID, p.Username);
            }

            string input;
            int id;
            do
            {
                Console.WriteLine();
                Console.Write("Enter number of profile to use: ");
                input = Console.ReadLine().Trim();

            } while (!int.TryParse(input, out id));

            bool load = true;
            if (File.Exists("output.json"))
            {
                do
                {
                    Console.WriteLine();
                    Console.Write("Re-do failed wikis from last run Y/N? ");
                    input = Console.ReadLine().Trim().ToLower();
                } while (input != "y" && input != "n");

                load = input != "y";
            }

            var profile = AWBProfiles.GetProfile(id);
            var wikis = GetWikiList(load);

            Dictionary<string, List<string>> results = new Dictionary<string, List<string>>();

            foreach (string wiki in wikis)
            {
                try
                {
                    Console.Write("Converting checkpage format using User:{0} on {1}... ", profile.Username, wiki);
                    var res = UpdateWiki(wiki, profile.Username, profile.Password);

                    if (!results.ContainsKey(res))
                    {
                        results.Add(res, new List<string>());
                    }
                    results[res].Add(wiki);

                    Console.WriteLine(res);
                }
                catch (LoginException)
                {
                    Console.WriteLine("Unable to login with credentials provided.");
                    if (!results.ContainsKey("loginfailed"))
                    {
                        results.Add("loginfailed", new List<string>());
                    }
                    results["loginfailed"].Add(wiki);

                    // Sleep for two minutes
                    Thread.Sleep(2 * 60 * 1000);
                }
                catch (MaxlagException)
                {
                    Console.WriteLine("lag :(");
                    if (!results.ContainsKey("maxlag"))
                    {
                        results.Add("maxlag", new List<string>());
                    }
                    results["maxlag"].Add(wiki);
                }
                catch (ApiErrorException ex)
                {
                    Console.WriteLine(ex.Message);
                    if (!results.ContainsKey(ex.ErrorCode))
                    {
                        results.Add(ex.ErrorCode, new List<string>());
                    }
                    results[ex.ErrorCode].Add(wiki);
                }
            }

            Console.WriteLine();
            Console.WriteLine("Done!");
            Console.WriteLine();

            foreach (string key in results.Keys)
            {
                Console.WriteLine("{0}: {1}", key, results[key].Count);
            }

            Console.ReadLine();

            do
            {
                Console.WriteLine();
                Console.Write("Output results Y/N? ");
                input = Console.ReadLine().Trim().ToLower();
            } while (input != "y" && input != "n");

            if (input == "y")
            {
                foreach (string key in results.Keys)
                {
                    Console.WriteLine(key + ":");
                    foreach (string w in wikis)
                    {
                        Console.WriteLine(w);
                    }

                    Console.WriteLine();
                }

                using (StreamWriter sw = new StreamWriter("output.json"))
                {
                    sw.Write(JsonConvert.SerializeObject(results, Formatting.Indented));
                    sw.Close();
                }

                Console.ReadLine();
            }

        }

        private static IEnumerable<string> GetWikiList(bool load)
        {
            if (load)
            {
                var sitematrix = JObject.Parse(Tools.GetHTML("https://meta.wikimedia.org/w/api.php?action=sitematrix&smsiteprop=url&smlimit=max&format=json"));

                var wikis = sitematrix.Descendants()
                    .Where(x => x is JObject)
                    .Where(x => x["site"] != null || x["url"] != null)
                    .Select(x => x["url"])
                    .Where(x => x != null)
                    .Values<string>()
                    .ToList();

                wikis.Sort();
                return wikis;
            }

            using (var reader = new StreamReader("output.json"))
            {
                var json = reader.ReadToEnd();
                var wikis = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(json);

                List<string> urls = new List<string>();
                foreach(string key in wikis.Keys)
                {
                    if (key == "Done!" || key == "No check page")
                    {
                        continue;
                    }

                    urls.AddRange(wikis[key]);
                }
                urls.Sort();
                return urls;
            }
        }

        private static string UpdateWiki(string url, string username, string password)
        {
            ApiEdit edit = new ApiEdit($"{url}/w/");

            var origCheckPageTitle = "Project:AutoWikiBrowser/CheckPage";

            try
            {
                // Cheap, non logged in check for page existence
                if (!edit.PageExists(origCheckPageTitle))
                {
                    return "No check page";
                }
            }
            catch (ApiErrorException ape)
            {
                // Cheap, non logged in check failed. Try and login anyway
                // If it wasn't readapidenied, fail
                if (ape.ErrorCode != "readapidenied")
                {
                    throw;
                }
            }

            edit.Login(username, password);

            var origCheckPageText = edit.Open(origCheckPageTitle);

            if (!edit.Page.Exists || string.IsNullOrEmpty(origCheckPageText))
            {
                return "No check page";
            }

            var editProtection = edit.Page.EditProtection;
            var moveProtection = edit.Page.MoveProtection;

            var enabledUsers = Tools.StringBetween(origCheckPageText, "<!--enabledusersbegins-->",
                "<!--enabledusersends-->");

            var botUsers = Tools.StringBetween(enabledUsers, "<!--enabledbots-->", "<!--enabledbotsends-->");

            var normalUsers = enabledUsers.Replace("<!--enabledbots-->" + botUsers + "<!--enabledbotsends-->", "");

            List<string> users = new List<string>();
            foreach (Match m in UsernameRegex.Matches(normalUsers))
            {
                if (!string.IsNullOrEmpty(m.Groups[1].Value.Trim()))
                {
                    users.Add(m.Groups[1].Value.Trim());
                }
            }

            users.Sort();

            List<string> bots = new List<string>();
            foreach (Match m in UsernameRegex.Matches(botUsers))
            {
                if (!string.IsNullOrEmpty(m.Groups[1].Value.Trim()))
                {
                    bots.Add(m.Groups[1].Value.Trim());
                }
            }

            bots.Sort();

            Dictionary<string, List<string>> checkPageOutput = new Dictionary<string, List<string>>
            {
                {"enabledusers", users.Distinct().ToList()},
                {"enabledbots", bots.Distinct().ToList()}
            };

            var title = "Project:AutoWikiBrowser/CheckPageJSON";

            edit.Open(title);

            edit.Save(JsonConvert.SerializeObject(checkPageOutput, Formatting.Indented),
                $"Converting AutoWikiBrowser CheckPage to json - {PHAB_TASK}", false,
                WatchOptions.NoChange, "json");

            if (!string.IsNullOrEmpty(editProtection) || !string.IsNullOrEmpty(moveProtection))
            {
                edit.Open(title);

                edit.Protect(title, $"Copying protection from [[{origCheckPageTitle}]] - {PHAB_TASK}", "infinite",
                    editProtection, moveProtection);
            }

            // Site Config stuff
            Dictionary<string, object> configOutput = new Dictionary<string, object>();

            Match typoLink = Regex.Match(origCheckPageText, "<!--[Tt]ypos:(.*?)-->");
            configOutput.Add("typolink",
                typoLink.Success && typoLink.Groups[1].Value.Trim().Length > 0 ? typoLink.Groups[1].Value.Trim() : "");

            configOutput.Add("allusersenabled", origCheckPageText.Contains("<!--All users enabled-->"));

            configOutput.Add("allusersenabledusermode",
                origCheckPageText.Contains("<!--All users enabled user mode-->"));

            List<Dictionary<string, string>> awbMessages = new List<Dictionary<string, string>>();

            // see if there is a message
            foreach (Match m in Message.Matches(origCheckPageText))
            {
                if (m.Groups[1].Value.Trim().Length == 0)
                {
                    continue;
                }

                awbMessages.Add(new Dictionary<string, string>
                {
                    {"version", "*"},
                    {"message", m.Groups[1].Value.Trim()}
                });
            }

            // see if there is a version-specific message
            foreach (Match m in VersionMessage.Matches(origCheckPageText))
            {
                if (m.Groups[2].Value.Trim().Length == 0 || m.Groups[1].Value == "x.x.x.x")
                {
                    continue;
                }

                awbMessages.Add(new Dictionary<string, string>
                {
                    {"version", m.Groups[1].Value},
                    {"message", m.Groups[2].Value.Trim()}
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

            title = "Project:AutoWikiBrowser/Config";
            edit.Open(title);
            edit.Save(JsonConvert.SerializeObject(configOutput, Formatting.Indented),
                $"Converting AutoWikiBrowser wiki config to json - {PHAB_TASK}", false,
                WatchOptions.NoChange, "json");

            if (!string.IsNullOrEmpty(editProtection) || !string.IsNullOrEmpty(moveProtection))
            {
                edit.Open(title);

                edit.Protect(title, $"Copying protection from [[{origCheckPageTitle}]] - {PHAB_TASK}", "infinite",
                    editProtection, moveProtection);
            }

            return "Done!";
        }
    }
}
