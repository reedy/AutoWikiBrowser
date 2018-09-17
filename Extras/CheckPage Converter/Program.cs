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
        static void Main(string[] args)
        {
            var checkPageText =
                Tools.GetHTML(
                    "https://en.wikipedia.org/w/index.php?title=Wikipedia:AutoWikiBrowser/CheckPage&action=raw");

            checkPageText = Tools.StringBetween(checkPageText, "<!--enabledusersbegins-->",
                                                    "<!--enabledusersends-->");

            string botUsers = Tools.StringBetween(checkPageText, "<!--enabledbots-->", "<!--enabledbotsends-->");

            checkPageText = checkPageText.Replace("<!--enabledbots-->\r\n" + checkPageText + "\r\n<!--enabledbotsends-->", "");

            Regex username = new Regex(@"^\*\s*(.*?)\s*$", RegexOptions.Multiline | RegexOptions.Compiled);

            List<string> users = new List<string>();
            foreach (Match m in username.Matches(checkPageText)) {
                users.Add(m.Groups[1].Value.Trim);
            }

            List<string> bots = new List<string>();
            foreach (Match m in username.Matches(botUsers))
            {
                bots.Add(m.Groups[1].Value.Trim());
            }

            Dictionary<string, List<string>> output = new Dictionary<string, List<string>> {
                { "enabledusers", users },
                { "enabledbots", bots }
            };

            string json = JsonConvert.SerializeObject(output, Formatting.Indented);

            ApiEdit edit = new ApiEdit("https://en.wikipedia.org/w/");
            var profile = AWBProfiles.GetProfile(1);
            edit.Login(profile.Username, profile.Password);
            edit.Open("Project:AutoWikiBrowser/CheckPageJSON");
            edit.Save(json, "Converting from non json page", false, WatchOptions.NoChange);
        }
    }
}
