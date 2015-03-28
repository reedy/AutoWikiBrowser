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

using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace WikiFunctions
{
    /// <summary>
    /// Provides some common static regexes
    /// </summary>
    public static class WikiRegexes
    {
        public static void MakeLangSpecificRegexes()
        {
            NamespacesCaseInsensitive = new Dictionary<int,Regex>();
            foreach (var p in Variables.NamespacesCaseInsensitive)
            {
                NamespacesCaseInsensitive.Add(p.Key, new Regex(p.Value));
            }

            string category = Variables.NamespacesCaseInsensitive[Namespace.Category],
            image = Variables.NamespacesCaseInsensitive[Namespace.File],
            template = Variables.NamespacesCaseInsensitive[Namespace.Template],
            userns = Variables.NamespacesCaseInsensitive[Namespace.User],
            usertalkns = Variables.NamespacesCaseInsensitive[Namespace.UserTalk];

            TemplateStart = @"\{\{\s*(:?" + template + ")?";

            TemplateNameRegex = Tools.TemplateNameRegex();

            Category = new Regex(@"\[\[[\s_]*" + category +
                                 @"[ _]*([^[\]|\r\n]*?)[ _]*(?:\|([^\|\]]*))?[ _]*\]\]");

			// allow comments between categories, and keep them in the same place, only grab any comment after the last category if on same line
			// whitespace: remove all whitespace after, but leave a blank newline before a heading (rare case where category not in last section)
            RemoveCatsAllCats = new Regex(@"<!-- [^<>]*?\[\[\s*" + category + @".*?(\]\]|\|.*?\]\]).*?-->|\[\[" + category
			                    + @".*?(\]\]|\|.*?\]\])(\s*⌊⌊⌊⌊\d{1,4}⌋⌋⌋⌋| *<!--.*?-->|\s*<!--.*?-->(?=\r\n\[\[\s*" + category
			                    + @")|\s*(?=\r\n==)|\s*)?", RegexOptions.Singleline);

            CategoryQuick = new Regex(@"\[\[[\s_]*" + category);

            // Match file name by using allowed character list, [October 2012 any Unicode word characters] then a file extension (these are mandatory on mediawiki), then optional closing ]]
            // this allows typo fixing and find&replace to operate on image descriptions
            // or, alternatively, an image filename has to have a pipe or ]] after it if using the [[Image: start, so just set first one to
            // @"[^\[\]\|\{\}]+\.[a-zA-Z]{3,4}\b(?:\s*(?:\]\]|\|))
            // handles images within <gallery> and matches all of {{gallery}} too
            // Supported file extensions taken from https://commons.wikimedia.org/wiki/Commons:File_types
            string ImagesString = @"(?:\[\[\s*)?" + image +
                    @"[ \%\!""$&'’\(\)\*,\-.\/0-9:;=\?@\w\\\^_`~\x80-\xFF\+]+\.[a-zA-Z]{3,4}\b(?:\s*(?:\]\]|\|))?";
            const string ImageInTemplateString = @"|{{\s*[Gg]allery\s*(?:\|(?>[^\{\}]+|\{(?<DEPTH>)|\}(?<-DEPTH>))*(?(DEPTH)(?!)))?}}|(?<=\|\s*(?:[a-zA-Z\d_ ]+\s*=)?)[^\|{}=\r\n]+?\.(?i:djvu|gif|jpe?g|og[agv]|pdf|png|svg|tiff?|mid|xcf)(?=\s*(?:<!--[^>]*?-->\s*|⌊⌊⌊⌊M?\d+⌋⌋⌋⌋\s*)?(?:\||}}))";

            Images = new Regex(ImagesString + ImageInTemplateString);

            ImagesCountOnly = new Regex(ImagesString + ImageInTemplateString.Replace(@"(?<=", @"(?:"));

            ImagesNotTemplates = new Regex(ImagesString);

            FileNamespaceLink = new Regex(@"\[\[\s*" + image +
                                          @"((?>[^\[\]]+|\[\[(?<DEPTH>)|\]\](?<-DEPTH>))*(?(DEPTH)(?!)))\]\]");

            Stub = new Regex(@"{{" + Variables.Stub + @"\s*(?:\|[^{}]+)?}}");
            
			UserSignature = new Regex(@"\[\[\s*(?:"+ userns + @"|" + usertalkns + @")");

            TemplateCall = new Regex(TemplateStart + @"\s*([^\]\|]*)\s*(.*)}}", RegexOptions.Singleline);

            LooseCategory = new Regex(@"\[\[[\s_]*" + category + @"[\s_]*([^\|]*?)(\|.*?)?\]\]");

            LooseImage = new Regex(@"\[\[\s*?(" + image + @")\s*([^\|\]]+)(.*?)\]\]");

            Months = "(" + string.Join("|", Variables.MonthNames) + ")";
            MonthsNoGroup = "(?:" + string.Join("|", Variables.MonthNames) + ")";

            Dates = new Regex("^(0?[1-9]|[12][0-9]|3[01]) " + Months + "$");
            Dates2 = new Regex("^" + Months + " (0?[1-9]|[12][0-9]|3[01])$");
            
            InternationalDates = new Regex(@"\b([1-9]|[12][0-9]|3[01])(?: +|&nbsp;)" + Months + @" +([12]\d{3})\b");
            AmericanDates = new Regex(Months + @"(?: +|&nbsp;)([1-9]|[12][0-9]|3[01]),? +([12]\d{3})\b");

            DayMonth = new Regex(@"\b([1-9]|[12][0-9]|3[01])(?: +|&nbsp;)" + Months + @"\b");
            MonthDay = new Regex(Months + @"(?: +|&nbsp;)([1-9]|[12][0-9]|3[01])\b");
            
            DayMonthRangeSpan = new Regex(@"\b((?:[1-9]|[12][0-9]|3[01])(?:–|&ndash;|{{ndash}}|\/)(?:[1-9]|[12][0-9]|3[01])) " + Months + @"\b");
            
            MonthDayRangeSpan = new Regex(Months + @" ((?:[1-9]|[12][0-9]|3[01])(?:–|&ndash;|{{ndash}}|\/)(?:[1-9]|[12][0-9]|3[01]))\b");

            List<string> magic;
            string s = Variables.MagicWords.TryGetValue("redirect", out magic)
                ? string.Join("|", magic.ToArray()).Replace("#", "")
                : "REDIRECT";

            //Regex contains extra opening/closing brackets and double bot, equal sign so that we fix with FixSyntaxRedirects
            Redirect = new Regex(@"#(?:" + s + @")\s*[:|=]?\s*\[?\[?\[\[\s*:?\s*([^\|\[\]]*?)\s*(\|.*?)?\]\]\]?\]?", RegexOptions.IgnoreCase);

            switch (Variables.LangCode)
            {
                case "ar":
                    s = "([Dd]isambig|توضيح|صفحة توضيح)";
                    break;
                case "arz":
                    s = "([Dd]isambig|صفحة توضيح|توضيح)";
                    break;
                case "ca":
                    s = "([Dd]esambiguació|[Dd]esambigua|[Dd]isambig)";
                    break;
                case "de":
                    s = "([Bb]egriffsklärung)";
                    break;
                case "el":
                    s = "([Αα]ποσαφήνιση|[Αα]ποσαφ|[Dd]isambig)";
                    break;
                case "es":
                    s = "([Dd]esambiguación|[Dd]esambig|[Dd]es|[Dd]esambiguacion|[Dd]isambig)";
                    break;
                case "pl":
                    s = "([Dd]isambig)";
                    break;
                case "ru":
                    s = "([Dd]isambiguation|[Dd]isambig|[Нн]еоднозначность|[Мm]ногозначность)";
                    break;
                case "sv":
                    s = "(4LA|[Bb]etydelselista|[Dd]ab|[Dd]isambig|[Dd]isambiguation|[Ee]fternamn|[Ff]örgrening|[Ff]örgreningssida|[Ff]lertydig|[Ff]örnamn|[Gg]affel|[Gg]ren|[Gg]rensida|[Hh]ndis||[Nn]amnförgrening|[Nn]amngrensida|[Oo]rtnamn|[Rr]obotskapad förgrening|[Tt]rebokstavsförkortning|[Tt]rebokstavsförgrening)";
                    break;
                default:
                    s = "([Dd]isamb(?:ig(?:uation)?)?|[Dd]ab|[Cc]hinese title disambiguation|[Gg]enus disambiguation|[Gg]enus disambig|[Mm]athdab|[Mm]athematics disambiguation|[Mm]athematical disambiguation|[Mm]il-unit-dis|(?:[Nn]umber|[Hh]ospital|[Gg]eo|[Hh]n|[Ss]chool)dis|[Ll]etter-disambig|[[Aa]irport disambig(?:uation)?|[Cc]allsigndis|[Cc]all sign disambiguation|[Dd]isambig-cleanup|[Dd]isambig-cleanup|[Dd]isambiguation cleanup|[Mm]olFormDisambig|[Mm]olecular formula disambiguation|[Rr]oad disambiguation|([Ss]pecies|)LatinNameDisambig|[Ss]pecies Latin name disambiguation|[[Ss]pecies Latin name abbreviation disambiguation|[Ll]etter-NumberComb[Dd]isambig|[Ll]etter-Number Combination Disambiguation|[Hh]ndis|[Hh]ndis-cleanup|[Gg]enus disambiguation|[Tt]axonomy disambiguation|[Hh]urricane season disambiguation|[Hh]ospital disambiguation|[Ss]chool disambiguation)";
                    break;
            }
            Disambigs = new Regex(TemplateStart + s + @"\s*(?:\|[^{}]*?)?}}(?: *<!--.*?-->(?=\r\n|$))?", RegexOptions.Multiline);

            DisambigsGeneral = new Regex(TemplateStart + @"([Dd]isamb(?:ig(?:uation)?)?|[Dd]ab)" + @"\s*(?:\|[^{}]*?)?}}");
            DisambigsCleanup = new Regex(TemplateStart + @"([Dd]isambig-cleanup|[Dd]isambig cleanup|[Dd]isambiguation cleanup)" + @"\s*(?:\|[^{}]*?)?}}");


        	s = "([Ss]urnames?|SIA|[Ss]ia|[Ss]et index article|[Ss]et ?index|[Ss]hip ?index|[Mm]ountain ?index|[[Rr]oad ?index|[Ss]port ?index|[Gg]iven name|[Mm]olForm ?Index|[Mm]olecular formula index|[Cc]hemistry index|[Ee]nzyme index|[Mm]edia set index|[Ll]ake ?index|[Pp]lant common name)";
            SIAs = new Regex(TemplateStart + s + @"\s*(?:\|[^{}]*?)?}}");
            
            if (Variables.MagicWords.TryGetValue("defaultsort", out magic))
                s = "(?i:" + string.Join("|", magic.ToArray()).Replace(":", "") + ")";
            else
                s = (Variables.LangCode == "en")
                    ? "(?:(?i:defaultsort(key|CATEGORYSORT)?))"
                    : "(?i:defaultsort)";

            // sv-wiki: allow comment on same line as DEFAULTSORT
            if(Variables.LangCode.Equals("sv"))
                Defaultsort = new Regex(TemplateStart + s + @"\s*[:\|]\s*(?<key>(?>[^\{\}\r\n]+|\{(?<DEPTH>)|\}(?<-DEPTH>))*(?(DEPTH)(?!))|[^\}\r\n]*?)(?<end>\s*}}(?: *<!--[^<>]+-->)?|\r|\n)",
                                        RegexOptions.ExplicitCapture);
            else
                Defaultsort = new Regex(TemplateStart + s + @"\s*[:\|]\s*(?<key>(?>[^\{\}\r\n]+|\{(?<DEPTH>)|\}(?<-DEPTH>))*(?(DEPTH)(?!))|[^\}\r\n]*?)(?<end>\s*}}|\r|\n)",
                                        RegexOptions.ExplicitCapture);                    

            Persondata = (Variables.LangCode.Equals("de") ? Tools.NestedTemplateRegex("personendaten") : Tools.NestedTemplateRegex("persondata"));

            //if (Variables.URL == Variables.URLLong)
            //    s = Regex.Escape(Variables.URL);
            //else
            //{
            int pos = Tools.FirstDifference(Variables.URL, Variables.URLLong);
            s = Regex.Escape(Variables.URLLong.Substring(0, pos)).Replace(@"https://", @"https?://");
            s += "(?:" + Regex.Escape(Variables.URLLong.Substring(pos)) + @"index\.php(?:\?title=|/)|"
                + Regex.Escape(Variables.URL.Substring(pos)) + "/wiki/" + ")";
            //}
            ExtractTitle = new Regex("^" + s + "([^?&]*)$");

            EmptyLink = new Regex(@"\[\[\s*(?:(:?" + category + "|" + image + @")\s*:?\s*(\|.*?)?|[|\s]*)\]\]");
            EmptyTemplate = new Regex(@"{{(" + template + @")?[|\s]*}}");
            
            // set orphan, wikify, uncat, inuse templates, dateparameter & Link FA/GA/GL strings
            string uncattemplate = UncatTemplatesEN;
            switch(Variables.LangCode)
            {
                case "an":
                    LinkFGAs = Tools.NestedTemplateRegex(new [] {"link FA", "Destacato", "Destacau" });
                    break;
                case "ar":
                    Orphan = Tools.NestedTemplateRegex(@"يتيمة");
                    uncattemplate = UncatTemplatesAR;
                    DateYearMonthParameter = @"تاريخ={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}";
					DeadEnd = new Regex(@"(?:{{\s*(?:[Dd]ead ?end|[Ii]nternal ?links|نهاية مسدودة)(?:\|(?:[^{}]+|" + DateYearMonthParameter + @"))?}}|({{\s*(?:[Aa]rticle|[Mm]ultiple)\s*issues\b[^{}]*?(?:{{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}})?[^{}]*?)*\|\s*dead ?end\s*=\s*(?:{{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|[^{}\|]+))");
                    Wikify =Tools.NestedTemplateRegex(@"ويكي");
                    InUse = Tools.NestedTemplateRegex(new[] {"إنشاء", "تحرر", "Underconstruction", "تحت الإنشاء", "تحت الأنشاء", "يحرر", "إنشاء مقالة", "انشاء مقالة", "Inuse", "تحرير كثيف", "يحرر المقالة", "تحت التحرير", "قيد الاستخدام" });
                    break;
                case "arz":
                    Orphan = Tools.NestedTemplateRegex(@"يتيمه");
                    uncattemplate = UncatTemplatesARZ;
                    DateYearMonthParameter = @"تاريخ={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}";
					DeadEnd = new Regex(@"(?:{{\s*(?:[Dd]ead ?end|نهايه مسدوده)(?:\|(?:[^{}]+|" + DateYearMonthParameter + @"))?}}|({{\s*(?:[Aa]rticle|[Mm]ultiple)\s*issues\b[^{}]*?(?:{{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}})?[^{}]*?)*\|\s*dead ?end\s*=\s*(?:{{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|[^{}\|]+))");
                    Wikify =Tools.NestedTemplateRegex(@"ويكى");
                    LinkFGAs = Tools.NestedTemplateRegex(new [] {"link FA", "لينك مقاله مختاره", "link GA", "لينك مقاله جيده" });
                    break;
                case "br":
                    LinkFGAs = Tools.NestedTemplateRegex(new [] {"link FA", "liamm PuB", "lien AdQ", "lien BA" });
                    break;
                case "ca":
                    InUse = Tools.NestedTemplateRegex(new[] {"Modificant", "Editant-se", "Editant" });
                    LinkFGAs = Tools.NestedTemplateRegex(new [] {"link FA", "enllaç AD" });
                    break;
                case "el":
                    Orphan = Tools.NestedTemplateRegex(@"Ορφανό");
                    uncattemplate = "([Αα]κατηγοριοποίητο)";
                    DateYearMonthParameter = @"ημερομηνία={{subst:CURRENTYEAR}} {{subst:CURRENTMONTH}}";
					DeadEnd = new Regex(@"(?:{{\s*(?:[Dd]ead ?end)(?:\|(?:[^{}]+|" + DateYearMonthParameter + @"))?}})");
                    Wikify = new Regex(@"(?:{{\s*(?:Underlinked)(?:\s*\|\s*(?:" +DateYearMonthParameter +@"|.*?))?}})", RegexOptions.IgnoreCase);
                    InUse = Tools.NestedTemplateRegex(new[] {"Inuse", "Σε χρήση" });
                    break;
                case "eo":
                    InUse = Tools.NestedTemplateRegex(new[] {"Redaktas", "Redaktata", "Uzata" });
                    LinkFGAs = Tools.NestedTemplateRegex(new [] {"link FA", "ligoElstara" });
                    break;
                case "es":
                    InUse = Tools.NestedTemplateRegex(new[] {"En uso", "Enuso" });
                    break;
                case "fr":
                    InUse = Tools.NestedTemplateRegex(new[] {"En cours" });
                    break;
                case "hu":
                    InUse = Tools.NestedTemplateRegex(new[] {"Építés alatt", "Fejlesztés"});
                    break;
                case "hy":
                    Orphan = Tools.NestedTemplateRegex(@"Որբ");
                    uncattemplate = "(Կատեգորիա չկա|Կչ|[Uu]ncategorized)";
					DeadEnd = new Regex(@"(?:{{\s*(?:[Dd]ead ?end|[Uu]nderlinked|Փակ)(?:\|(?:[^{}]+|" +DateYearMonthParameter +@"))?}}|\s*Փակ\s*=\s*(?:{{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|[^{}\|]+))");
					Wikify = new Regex(@"{{\s*Վիքիֆիկացում(?:\s*\|\s*(" + DateYearMonthParameter + @"|.*?))?}}", RegexOptions.IgnoreCase);
                    InUse = Tools.NestedTemplateRegex(new[] {"Խմբագրում եմ"});
                    break;
                case "it":
                    InUse = Tools.NestedTemplateRegex(new[] {"WIP", "Wip" });
                    LinkFGAs = Tools.NestedTemplateRegex(new [] {"link FA", "link FL", "link AdQ", "link V", "link VdQ", "link GA" });
                    break;
                case "pt":
                    InUse = Tools.NestedTemplateRegex(new[] {"Em edição", "Emuso", "Emedição"});
                    LinkFGAs = Tools.NestedTemplateRegex(new [] {"link FA", "link GA", "bom interwiki", "interwiki destacado", "FA"});
                    break;
                case "ro":
                    InUse = Tools.NestedTemplateRegex(new[] {"S-dezvoltare"});
                    break;
                case "ru":
                    Orphan = Tools.NestedTemplateRegex(@"изолированная статья");
                    DateYearMonthParameter = @"date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}";
					DeadEnd = new Regex(@"(?:{{\s*(?:[Tt]упиковая статья|[Dd]ead ?end)(?:\|(?:[^{}]+|" + DateYearMonthParameter + @"))?}}|({{\s*(?:[Aa]rticle|[Mm]ultiple)\s*issues\b[^{}]*?(?:{{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}})?[^{}]*?)*\|\s*dead ?end\s*=\s*(?:{{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|[^{}\|]+))");
                    Wikify = new Regex(@"({{\s*Wikify(?:\s*\|\s*(" +DateYearMonthParameter +@"|.*?))?}}|(?<={{\s*(?:Article|Multiple)\s*issues\b[^{}]*?)\|\s*wikify\s*=[^{}\|]+)", RegexOptions.IgnoreCase);
                    InUse = Tools.NestedTemplateRegex(new[] {"Редактирую", "Перерабатываю", "Inuse-by", "Пишу", "Inuse", "Правлю", "Перевожу", "In-use", "Processing", "Process", "Статья редактируется", "Викифицирую", "Under construction" });
                    break;
                case "sv":
                    Orphan = Tools.NestedTemplateRegex(@"Föräldralös");
                    uncattemplate = "([Oo]kategoriserad|[Uu]ncategori[sz]ed|[Uu]ncategori[sz]ed ?stub)";
                    DateYearMonthParameter = @"datum={{subst:CURRENTYEAR}}-{{subst:CURRENTMONTH}}";
					DeadEnd = new Regex(@"(?:{{\s*(?:[Dd]ead ?end)(?:\|(?:[^{}]+|" + DateYearMonthParameter + @"))?}})");
                    Wikify = new Regex(@"{{\s*Ickewiki(?:\s*\|\s*(" + DateYearMonthParameter + @"|.*?))?}}", RegexOptions.IgnoreCase);
                    InUse = Tools.NestedTemplateRegex(new[] {"Pågår", "Information kommer", "Pågående uppdateringar", "Ständiga uppdateringar", "PÅGÅR", "Påbörjad", "Bearbetning pågår"});
                    LinkFGAs = Tools.NestedTemplateRegex(new [] {"link FA", "link GA", "länk UA", "lank UA", "UA", "GA"});
                    break;
                case "zh":
                    DateYearMonthParameter = @"time={{subst:#time:c}}";
                    Orphan = new Regex(@"(?:{{\s*[Oo]rphan(?:\s*\|(?:[^{}]+|" +DateYearMonthParameter +@"))?}}|(?<MI>{{\s*(?:[Aa]rticle|[Mm]ultiple)\s*issues\b[^{}]*?(?:{{subst:CURRENTYEAR}}-{{subst:CURRENTMONTH}}-{{subst:CURRENTDAY2}})?[^{}]*?)*\|\s*orphan\s*=\s*(?:{{subst:CURRENTYEAR}}-{{subst:CURRENTMONTH}}-{{subst:CURRENTDAY2}}|[^{}\|]+))");
                    InUse = Tools.NestedTemplateRegex(new[] {"Inuse", "UnderConstruction", "工事中", "Inedit", "Editing", "使用中", "2小时内重大修改 " });
                    break;
                default:
                    DateYearMonthParameter = @"date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}";
                    Orphan = new Regex(@"(?:{{\s*[Oo]rphan(?:\s*\|(?:[^{}]+|" +DateYearMonthParameter +@"))?}}|(?<MI>{{\s*(?:[Aa]rticle|[Mm]ultiple)\s*issues\b[^{}]*?(?:{{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}})?[^{}]*?)*\|\s*orphan\s*=\s*(?:{{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|[^{}\|]+))");
                    uncattemplate = UncatTemplatesEN;
					DeadEnd = new Regex(@"(?:{{\s*(?:[Dd]ead ?end|[Ii]nternal ?links|[Nn]uevointernallinks|[Dd]ep)(?:\|(?:[^{}]+|" +DateYearMonthParameter +@"))?}}|({{\s*(?:[Aa]rticle|[Mm]ultiple)\s*issues\b[^{}]*?(?:{{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}})?[^{}]*?)*\|\s*dead ?end\s*=\s*(?:{{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|[^{}\|]+))");
                    Wikify = new Regex(@"(?:{{\s*(?:Wikify|Underlinked)(?:\s*\|\s*(?:" +DateYearMonthParameter +@"|.*?))?}}|({{\s*(?:Article|Multiple)\s*issues\b[^{}]*?)\|\s*(?:wikify|underlinked)\s*=\s*(?:{{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|[^{}\|]+))", RegexOptions.IgnoreCase);
                    InUse = Tools.NestedTemplateRegex(new[] {"Inuse", "In use", "GOCEinuse", "goceinuse", "in creation", "increation" });
                    LinkFGAs =  Tools.NestedTemplateRegex(new [] {"link FA", "link GA", "link FL"});
                    break;

            }
            
            Uncat = new Regex(@"{{\s*" + uncattemplate + @"((\s*\|[^{}]+)?\s*|\s*\|((?>[^\{\}]+|\{\{(?<DEPTH>)|\}\}(?<-DEPTH>))*(?(DEPTH)(?!))))\}\}");

            PossiblyCommentedStub =
                new Regex(
                    @"(<!-- ?\{\{" + Variables.Stub + @"\b\}\}.*?-->|\{\{" + Variables.Stub + @"\s*(?:\|(?:[^{}]+|" + DateYearMonthParameter + @"))?}})");

            if(Variables.LangCode.Equals("fr"))
                ReferenceList = Tools.NestedTemplateRegex(new [] { "références", "references", "reflist" });
            else
                ReferenceList = Tools.NestedTemplateRegex(new [] { "reflist", "references-small", "references-2column"});
        }
        
        private const string UncatTemplatesAR = @"(غير مصنفة|غير مصنف|[Uu]ncategori[sz]ed|[Uu]ncategori[sz]ed ?stub|بذرة غير مصنفة)";
        private const string UncatTemplatesARZ = @"(مش متصنفه|[Uu]ncategori[sz]ed|[Uu]ncategori[sz]ed ?stub|تقاوى مش متصنفه)";
        private const string UncatTemplatesEN = @"([Uu]ncat|[Cc]lassify|[Cc]at[Nn]eeded|[Uu]ncategori[sz]ed|[Cc]ategori[sz]e|[Cc]ategories needed|[Cc]ategory ?needed|[Cc]ategory requested|[Cc]ategories requested|[Nn]ocats?|[Uu]ncat-date|[Uu]ncategorized-date|[Nn]eeds cats?|[Cc]ats? needed|[Uu]ncategori[sz]ed ?stub)";

        /// <summary>
        /// Matches the month names and provides a capturing group when used in a regular expression
        /// </summary>
        public static string Months;

        /// <summary>
        /// Matches the month names, without providing a capturing group when used in a regular expression
        /// </summary>
        public static string MonthsNoGroup;

        // Covered by RegexTests.GenerateNamespaceRegex()
        /// <summary>
        /// Generates a regex template for all variants of one or more namespace,
        /// e.g. "File|Image"
        /// </summary>
        /// <param name="namespaces">One or more namespaces to process</param>
        public static string GenerateNamespaceRegex(params int[] namespaces)
        {
            StringBuilder sb = new StringBuilder(100 * namespaces.Length);
            foreach (int ns in namespaces)
            {
                if (ns == Namespace.Article) continue;

                if (sb.Length > 0) sb.Append('|');

                string nsName = Variables.Namespaces[ns];
                sb.Append(Tools.StripNamespaceColon(nsName));
                string canNS;
                if (Variables.CanonicalNamespaces.TryGetValue(ns, out canNS)
                    && canNS != nsName)
                {
                    sb.Append('|');
                    sb.Append(Tools.StripNamespaceColon(canNS));
                }

                List<string> nsAlias;
                if (Variables.NamespaceAliases.TryGetValue(ns, out nsAlias))
                    foreach (string s in nsAlias)
                {
                    sb.Append('|');
                    sb.Append(Tools.StripNamespaceColon(s));
                }
            }

            sb.Replace(" ", "[ _]");
            return sb.ToString();
        }

        /// <summary>
        /// Variables.NamespacesCaseInsensitive compiled into regexes
        /// </summary>
        public static Dictionary<int, Regex> NamespacesCaseInsensitive;
        
        /// <summary>
        /// Dictionary of template redirects (as a nested template regex) and the actual template name
        /// </summary>
        public static Dictionary<Regex, string> TemplateRedirects = new Dictionary<Regex, string>();
        
        /// <summary>
        /// Nested template regex to match all loaded template redirects from [[WP:AWB/TR]]
        /// </summary>
        public static Regex AllTemplateRedirects;
        
        /// <summary>
        /// HashSet of all loaded template redirects from [[WP:AWB/TR]]
        /// </summary>
        public static HashSet<string> AllTemplateRedirectsHS;
        
        /// <summary>
        /// List of all loaded template redirects from [[WP:AWB/TR]], used when HashSet AllTemplateRedirectsHS cannot be
        /// </summary>
        public static List<string> AllTemplateRedirectsList;

        /// <summary>
        /// List of templates that should be dated (with 'date=Month YYYY' on en-wiki), loaded as first letter upper from https://en.wikipedia.org/wiki/Wikipedia:AWB/Dated_templates, see Category:Wikipedia maintenance categories sorted by month
        /// </summary>
        public static List<string> DatedTemplates = new List<string>();
        
        /// <summary>
        /// Structure of template name, old parameter, new parameter for parameter renaming
        /// </summary>
        public struct TemplateParameters
        {
            public string TemplateName;
            public string OldParameter;
            public string NewParameter;
        }
        
        /// <summary>
        /// List of templates with old parameter and new for parameter renaming
        /// </summary>
        public static List<TemplateParameters> RenamedTemplateParameters = new List<TemplateParameters>();

        /// <summary>
        /// Piece of template call, including curly brace and possible namespace
        /// </summary>
        public static string TemplateStart;

        /// <summary>
        /// Matches all wikilinks, categories, images etc. with nested links on same line
        /// </summary>
        public static readonly Regex SimpleWikiLink = new Regex(@"\[\[((?>[^\[\]\n]+|\[\[(?<DEPTH>)|\]\](?<-DEPTH>))*(?(DEPTH)(?!)))\]\]");

        /// <summary>
        /// Matches only internal wiki links
        /// </summary>
        public static readonly Regex WikiLinksOnly = new Regex(@"\[\[[^[\]\n]+(?<!\[\[[A-Z]?[a-z-]{2,}:[^[\]\n]+)\]\]");

        /// <summary>
        /// Matches only internal wiki links, possibly with pipe; group 1 is target, group 2 is pipe text, if piped link
        /// </summary>
        public static readonly Regex WikiLinksOnlyPossiblePipe = new Regex(@"\[\[([^[\]\|\n]+)(?<!\[\[[A-Z]?[a-z-]{2,}:[^[\]\n]+)(?:\|([^[\]\|\n]+))?\]\]");

        /// <summary>
        /// Matches only internal wikilinks (with or without pipe) with extra word character(s) e.g. [[link]]age or [[here|link]]age
        /// https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Improve_HideText.HideMore.28.29
        /// </summary>
        public static readonly Regex WikiLinksOnlyPlusWord = new Regex(@"\[\[[^\[\]\n]+\]\](\w+)");

        /// <summary>
        /// Matches all wikilinks (including interwikis, images, categories etc.) up to pipe or end of wikilink. Group 1 is the target of the wikilink
        /// </summary>
        public static readonly Regex WikiLink = new Regex(@"\[\[([^\]\|]+)(?:\]\]|\|)");

        /// <summary>
        /// Matches piped wikilinks, group 1 is target, group 2 the text
        /// </summary>
        public static readonly Regex PipedWikiLink = new Regex(@"\[\[([^[\]\n]*?)\|([^[\]\n]*?)\]\]");

        /// <summary>
        /// Matches unpiped wikilinks, group 1 is text
        /// </summary>
        public static readonly Regex UnPipedWikiLink = new Regex(@"\[\[([^\|\n]*?)\]\]");

        /// <summary>
        /// Matches {{DEFAULTSORT}}, "key" group being the sortkey
        /// </summary>
        public static Regex Defaultsort;
      
        /// <summary>
        /// List of Magic words from [[Category:Pages_which_use_a_template_in_place_of_a_magic_word]]
        /// </summary>
        public static readonly Regex MagicWordTemplates = Tools.NestedTemplateRegex(new[] { "BASEPAGENAME", "DEFAULTSORT", "DISPLAYTITLE", "Displaytitle", "FULLPAGENAME", "Fullpagename", "Namespace",
                                                                                    	"Numberofarticles", "PAGENAME", "PAGESIZE", "PROTECTIONLEVEL", "Pagename", "SUBPAGENAME", "Subpagename", "padleft" }, false);
        /// <summary>
        /// List of magic words behaviour switches from https://en.wikipedia.org/wiki/Help:Magic_words#Behavior_switches
        /// </summary>
        public static readonly Regex MagicWordsBehaviourSwitches = new Regex (@"__(NOTOC|FORCETOC|TOC|NOEDITSECTION|NEWSECTIONLINK|NONEWSECTIONLINK|NOGALLERY|HIDDENCAT|INDEX|NOINDEX|STATICREDIRECT)__", RegexOptions.IgnoreCase);
        /// <summary>
        /// Matches headings of all levels, group 1 being the heading name
        /// </summary>
        public static readonly Regex Headings = new Regex(@"^={1,6} *(.*?) *={1,6}(?: *⌊⌊⌊⌊\d{1,4}⌋⌋⌋⌋| *<!--.*?-->|< *[Bb][Rr] */ *>)?\s*$", RegexOptions.Multiline);
        
        public static readonly Regex HeadingsWhitespaceBefore = new Regex(@"\s+(?:< *[Bb][Rr] *\/? *>\s*)*^ *(={1,6}(.*?)={1,6}[\t ]*)(?=\r\n)", RegexOptions.Multiline);
        
        /// <summary>
        /// Matches level 2 headings
        /// </summary>
        public static readonly Regex HeadingLevelTwo = new Regex(@"^==([^=](?:.*?[^=])?)==(?: *⌊⌊⌊⌊\d{1,4}⌋⌋⌋⌋| *<!--.*?-->)?\s*$", RegexOptions.Multiline);
        
        /// <summary>
        /// Matches level 3 headings
        /// </summary>
        public static readonly Regex HeadingLevelThree = new Regex(@"^===([^=](?:.*?[^=])?)===(?: *⌊⌊⌊⌊\d{1,4}⌋⌋⌋⌋| *<!--.*?-->)?\s*$", RegexOptions.Multiline);
        
        /// <summary>
        /// Matches the whole of a level 2 section including heading and any subsections up to but not including the next level 2 section
        /// </summary>
        public static readonly Regex SectionLevelTwo = new Regex(@"^==[^=][^\r\n]*?[^=]==.*?(?=^==[^=][^\r\n]*?[^=]==(\r\n?|\n)$)", RegexOptions.Multiline | RegexOptions.Singleline);
        
        /// <summary>
        /// Matches the first section of an article, if the article has sections, else the whole article
        /// </summary>
        public static readonly Regex ZerothSection = new Regex("(^.+?(?===+)|^.+$)", RegexOptions.Singleline);
        
        /// <summary>
        /// Matches article text up to but not including first level 2 heading
        /// </summary>
        public static readonly Regex ArticleToFirstLevelTwoHeading = new Regex(@"^.*?(?=[^=]==[^=][^\r\n]*?[^=]==(?: *⌊⌊⌊⌊\d{1,4}⌋⌋⌋⌋| *<!--.*?-->)?(\r\n?|\n))", RegexOptions.Singleline);
        
        /// <summary>
        /// Matches text indented with a :
        /// </summary>
        public static readonly Regex IndentedText = new Regex(@"^:.*", RegexOptions.Multiline);

        /// <summary>
        /// Matches indented, bulleted or numbered text
        /// </summary>
        public static readonly Regex BulletedText = new Regex(@"^[\*#: ].*", RegexOptions.Multiline);

        /// <summary>
        /// Matches single line templates, NOT nested templates
        /// </summary>
        public static readonly Regex Template = new Regex(@"{{[^{\n]*?}}");

        /// <summary>
        /// Matches the end of a template call including trailing whitespace
        /// </summary>
        public static readonly Regex TemplateEnd = new Regex(@" *((?:\r\n)* *)}}$");

        /// <summary>
        /// Matches single and multiline templates, NOT nested templates
        /// </summary>
        public static readonly Regex TemplateMultiline = new Regex(@"{{[^{]*?}}");

        /// <summary>
        /// Matches single and multiline templates, AND those with nested templates
        /// </summary>
        public static readonly Regex NestedTemplates = new Regex(@"{{(?>[^\{\}]+|\{(?<DEPTH>)|\}(?<-DEPTH>))*(?(DEPTH)(?!))}}");

        /// <summary>
        /// Matches templates: group 1 matches the names of templates. Not for templates including the template namespace prefix
        /// </summary>
        public static readonly Regex TemplateName = new Regex(@"{{\s*([^\|{}]+?)(?:\s*<!--.*?-->\s*)?\s*(?:\||}})");

        /// <summary>
        /// Matches start of links to all Wikimedia-supported protocols except http, https: ftp, mailto, irc, gopher, telnet, nntp, worldwind, news, svn
        /// </summary>
        public static readonly Regex NonHTTPProtocols = new Regex(@"(?<=ftp|mailto|irc|gopher|telnet|nntp|worldwind|news|svn)://", RegexOptions.IgnoreCase);

        /// <summary>
        /// (Slowly) Matches external links to all Wikimedia-supported protocols http, https, ftp, mailto, irc, gopher, telnet, nntp, worldwind, news, svn
        /// </summary>
        public static readonly Regex ExternalLinks = new Regex(@"(https?|ftp|mailto|irc|gopher|telnet|nntp|worldwind|news|svn)://(?:[\w\._\-~!/\*""'\(\):;@&=+$,\\\?%#\[\]]+?(?=}})|[\w\._\-~!/\*""'\(\):;@&=+$,\\\?%#\[\]]*)|\[(https?|ftp|mailto|irc|gopher|telnet|nntp|worldwind|news|svn)://.*?\]", RegexOptions.IgnoreCase);

        /// <summary>
        /// Matches external links only to http, https protocols
        /// </summary>
        public static readonly Regex ExternalLinksHTTPOnly = new Regex(@"(https?)://(?:[\w\._\-~!/\*""'\(\):;@&=+$,\\\?%#\[\]]+?(?=}})|[\w\._\-~!/\*""'\(\):;@&=+$,\\\?%#\[\]]*)|(\[https?)://.*?\]", RegexOptions.IgnoreCase);

        /// <summary>
        /// Matches external links only to http, https protocols. For performance, protocol is not part of match value
        /// </summary>
        public static readonly Regex ExternalLinksHTTPOnlyQuick = new Regex(@"(?<=https?)://(?:[\w\._\-~!/\*""'\(\):;@&=+$,\\\?%#\[\]]+?(?=}})|[\w\._\-~!/\*""'\(\):;@&=+$,\\\?%#\[\]]*)|(?<=\[https?)://.*?\]", RegexOptions.IgnoreCase);

        /// <summary>
        /// Matches links that may be interwikis, i.e. containing colon, group 1 being the wiki language, group 2 being the link target, group 3 any comment after the link
        /// </summary>
        public static readonly Regex PossibleInterwikis = new Regex(@"\[\[\s*([-a-zA-Z]{2,12})(?<!File|Image|Media)\s*:+\s*([^\]\[]*?)\s*\]\]( *<!--.*?-->)?", RegexOptions.Singleline);

        /// <summary>
        /// Matches unformatted text regions: nowiki, pre, math, html comments, timelines
        /// </summary>
        public static readonly Regex UnformattedText = new Regex(@"<nowiki>.*?</\s*nowiki>|<(pre|math|timeline)\b.*?>.*?</\s*\1>|<!--.*?-->", RegexOptions.Singleline);

        /// <summary>
        /// Matches &lt;blockquote> tags
        /// </summary>
        public static readonly Regex Blockquote = new Regex(@"<\s*blockquote[^<>]*>(.*?)<\s*/\s*blockquote\s*>", RegexOptions.IgnoreCase | RegexOptions.Singleline);

        /// <summary>
        /// Matches &lt;poem> tags
        /// </summary>
        public static readonly Regex Poem = new Regex(@"<\s*poem[^<>]*>(.*?)<\s*/\s*poem\s*>", RegexOptions.IgnoreCase | RegexOptions.Singleline);        
        
        /// <summary>
        /// Matches &lt;imagemap&gt; tags
        /// </summary>
        public static readonly Regex ImageMap = new Regex(@"<\s*imagemap\b[^<>]*>(.*?)<\s*/\s*imagemap\s*>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        
        /// <summary>
        /// Matches all tags: &lt;nowiki>, &lt;pre>, &lt;math>, &lt;timeline>, &lt;code>, &lt;source>, &lt;cite>, &lt;syntaxhighlight>, &lt;blockquote>, &lt;poem>, &lt;imagemap>, &lt;includeonly>, &lt;onlyinclude>, &lt;noinclude>, &lt;hiero>, &lt;score>
        /// </summary>
        public static readonly Regex AllTags = new Regex(@"<\s*(nowiki|pre|math|timeline|code|source|cite|syntaxhighlight|blockquote|poem|imagemap|includeonly|onlyinclude|noinclude|hiero|score)[^<>]*>(?>(?!<\s*\1[^<>]*>|<\s*/\s*\1\b[^<>]*>).|<\s*\1[^<>]*>(?<Depth>)|<\s*/\s*\1\b[^<>]*>(?<-Depth>))*(?(Depth)(?!))<\s*/\s*\1\b[^<>]*>", RegexOptions.Singleline | RegexOptions.IgnoreCase);

        /// <summary>
        /// Matches &lt;gallery&gt; tags, group 1 is any tag parameters, group 2 is the tag contents
        /// </summary>
        public static readonly Regex GalleryTag = new Regex(@"< *gallery\b([^>]*?)>([\s\S]*?)</ *gallery *>", RegexOptions.IgnoreCase);

        /// <summary>
        /// Matches three or more consecutive new lines
        /// </summary>
        public static readonly Regex ThreeOrMoreNewlines = new Regex("(\r\n){3,}");

        /// <summary>
        /// Matches &lt;noinclude&gt; tags
        /// </summary>
        public static readonly Regex Noinclude = new Regex(@"<\s*noinclude\s*>(.*?)<\s*/\s*noinclude\s*>", RegexOptions.IgnoreCase | RegexOptions.Singleline);

        /// <summary>
        /// Matches &lt;includeonly&gt; and &lt;onlyinclude&gt; tags
        /// </summary>
        public static readonly Regex Includeonly = new Regex(@"<\s*(includeonly|onlyinclude)\s*>.*?<\s*/\s*\1\s*>", RegexOptions.IgnoreCase | RegexOptions.Singleline);

        /// <summary>
        /// Matches &lt;includeonly&gt; and &lt;onlyinclude&gt; and &lt;noinclude&gt; tags
        /// </summary>
        public static readonly Regex IncludeonlyNoinclude = new Regex(@"<\s*(includeonly|onlyinclude|noinclude)\s*>.*?<\s*/\s*\1\s*>", RegexOptions.IgnoreCase | RegexOptions.Singleline);

        /// <summary>
        /// Matches redirects
        /// Don't use directly, use Tools.IsRedirect() and Tools.RedirectTarget instead
        /// </summary>
        public static Regex Redirect;
        
        public const string RFromModificationString = @"{{R from modification}}";

        public static readonly string[] RFromModificationList =
        {
            "R from alternative punctuation", "R mod",
            "R from modification",
            "R from alternate punctuation"
        };
        
        public const string RFromTitleWithoutDiacriticsString = @"{{R from title without diacritics}}";

        public static readonly string[] RFromTitleWithoutDiacriticsList =
        {
            "R from name without diacritics",
            "R from original name without diacritics",
            "R from title without diacritics",
            "R to accents", "R to diacritics",
            "R to title with diacritics",
            "R to unicode", "R to unicode name",
            "R without diacritics", "RDiacr",
            "Redirects from title without diacritics"
        };
        
        public const string RFromOtherCapitalisationString = @"{{R from other capitalisation}}";

        public static readonly string[] RFromOtherCapitaliastionList =
        {
            "R cap", "R for alternate capitalisation",
            "R for alternate capitalization",
            "R for alternative capitalisation",
            "R for alternative capitalization",
            "R from Capitalisation",
            "R from Capitalization", "R from alt cap",
            "R from alt case",
            "R from alternate capitalisation",
            "R from alternate capitalization",
            "R from alternative capitalisation",
            "R from alternative capitalization",
            "R from another capitalisation",
            "R from another capitalization",
            "R from cap", "R from capitalisation",
            "R from capitalization", "R from caps",
            "R from different capitalization ",
            "R from lowercase",
            "R from miscapitalization",
            "R from other Capitalization",
            "R from other capitalisation",
            "R from other capitalization",
            "R from other caps", "R to Capitalization",
            "R to alternate capitalisation",
            "Redirect from capitalisation"
        };
        
        ///// <summary>
        ///// Matches words
        ///// </summary>
        //public static readonly Regex RegexWord = new Regex(@"\w+");

        /// <summary>
        /// 
        /// </summary>
        public static readonly Regex Newline = new Regex("\\n");

        /// <summary>
        /// Matches words, and allows words with apostrophes to be treated as one whole word
        /// </summary>
        public static readonly Regex RegexWordApostrophes = new Regex(@"\w+(?:['’]\w+)?");

        /// <summary>
        /// Matches &lt;source&gt;&lt;/source&gt;, &lt;syntaxhighlight&gt;, &lt;code&gt;&lt;/code&gt;, &lt;tt&gt;&lt;/tt&gt; tags, group 1 is the tag contents
        /// </summary>
        public static readonly Regex SourceCode = new Regex(@"<\s*(?<tag>source|syntaxhighlight|code|tt)(?:\s.*?|)>(.*?)<\s*/\s*\k<tag>\s*>", RegexOptions.IgnoreCase | RegexOptions.Singleline);        

        /// <summary>
        /// Matches math, pre, source, code, syntaxhighlight tags or comments
        /// </summary>
        public static readonly Regex MathPreSourceCodeComments = new Regex(@"<pre>.*?</pre>|<!--.*?-->|<math>.*?</math>|" + SourceCode, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        /// <summary>
        /// Matches anything starting with Meanings of minor planet names
        /// </summary>
        public static readonly Regex MeaningsOfMinorPlanetNames = new Regex(@"^Meanings of minor planet names", RegexOptions.Compiled);

        /// <summary>
        /// Matches Dates like 21 January
        /// </summary>
        public static Regex Dates;

        /// <summary>
        /// Matches Dates like January 21
        /// </summary>
        public static Regex Dates2;

        /// <summary>
        /// Matches categories, group 1 being the category name
        /// </summary>
        public static Regex Category;

        public static Regex RemoveCatsAllCats;

        /// <summary>
        /// Matches the start of a category link
        /// </summary>
        public static Regex CategoryQuick;

        /// <summary>
        /// Matches images: file namespace links (includes images within gallery tags), {{gallery}} template, image name parameters in infoboxes/templates
        /// </summary>
        public static Regex Images;
        
        /// <summary>
        /// Faster version of Images, matches same items. For counting of matches only
        /// </summary>
        public static Regex ImagesCountOnly;
        
        /// <summary>
        /// Matches images: file namespace links (includes images within gallery tags), {{gallery}} template, but NOT image name parameters in infoboxes/templates
        /// </summary>
        public static Regex ImagesNotTemplates;
        
        /// <summary>
        /// Matches links to the file namespace (images etc.), group 1 is the filename
        /// </summary>
        public static Regex FileNamespaceLink;

        /// <summary>
        /// Matches links to use or user talk namespace
        /// </summary>
        public static Regex UserSignature;


        /// <summary>
        /// Matches disambig templates, supports language variants e.g. for en-wiki {{disambig}}, {{dab}}.
        /// Matches wiki comment on same line after template if present
        /// </summary>
        public static Regex Disambigs;

        /// <summary>
        /// Matches general disambig templates
        /// </summary>
        public static Regex DisambigsGeneral;

        /// <summary>
        /// Matches disambig cleanup templates
        /// </summary>
        public static Regex DisambigsCleanup;

        /// <summary>
        /// Matches SIA templates (en only)
        /// </summary>
        public static Regex SIAs;

        /// <summary>
        /// Matches Wiktionary redirect and Wikiquote redirect templates (en only)
        /// </summary>
        public static readonly Regex Wi = Tools.NestedTemplateRegex(new [] { "Wiktionary redirect", "Wi", "Widirect", "Moved to Wiktionary", "RedirecttoWiktionary", "Seewiktionary", "Wikiquote redirect"});

        /// <summary>
        /// Matches Current events header in arwiki
        /// </summary>
        public static readonly Regex CEHar = Tools.NestedTemplateRegex(new [] { "رأس الأحداث الجارية"});

        /// <summary>
        /// Matches templates with many wikilinks to avoid tagging the transcluding page as dead-end and/or stub (en only)
        /// </summary>
        public static readonly Regex NonDeadEndPageTemplates = Tools.NestedTemplateRegex(new [] { "Events by year for decade", "Events by year for decade BC", "SCOTUSRow", "ATC codes lead", "Portal:Current events/Month Inclusion", "Ukrsrow", "PBB"});

        /// <summary>
        /// Matches stubs
        /// </summary>
        public static Regex Stub;

        /// <summary>
        /// Matches both commented and uncommented stubs
        /// </summary>
        public static Regex PossiblyCommentedStub;

        /// <summary>
        /// Matches Category links. Group 1 is the category name, group 2 is the pipe plus sortkey, if present.
        /// </summary>
        public static Regex LooseCategory;

        /// <summary>
        /// Matches wikilinks to files/images, group 1 being the namespace, group 2 the image name, group 3 the description/any extra arguments
        /// </summary>
        public static Regex LooseImage;

        /// <summary>
        /// Matches quotations outside of templates but within a pair of quotation marks, notably exlcuding straight single quotes
        /// </summary>
        /// see https://en.wikipedia.org/wiki/Quotation_mark_glyphs
        /// https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Ignoring_spelling_errors_within_quotation_marks.3F
        public static readonly Regex UntemplatedQuotes = new Regex(@"(?<=[^\w]|^)[""«»‘’“”‛‟‹›“”„‘’`’“‘”].{1,2000}?[""«»‘’“”‛‟‹›“”„‘’`’“‘”](?=[^\w])", RegexOptions.Singleline);
        
        /// <summary>
        /// Matches common curly double quotes, see [[MOS:PUNCT]]
        /// </summary>
        public static readonly Regex CurlyDoubleQuotes = new Regex(@"[“”‟“”„“”″]");

        // covered by TestFixNonBreakingSpaces
        /// <summary>
        /// Matches abbreviated SI units without a non-breaking space, notably does not correct millimetres without a space due to firearms articles using this convention
        /// </summary>
        /// https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Non_breaking_spaces
        public static readonly Regex UnitsWithoutNonBreakingSpaces = new Regex(@"(\b\d?\.?\d+)[\s\u00a0]*([cmknuµ][mgWN]|m?mol|cd|mi|lb[fs]?|b?hp|mph|ft|dB|[kGM]?Hz|m/s|°[CF])\b(?<!(\d?\.?\d+)mm)");

        // covered by TestFixNonBreakingSpaces
        /// <summary>
        /// Matches "50m (170&amp;nbsp;ft)"
        /// </summary>
        public static readonly Regex MetresFeetConversionNonBreakingSpaces = new Regex(@"(\d+(?:\.\d+)?)[\s\u00a0]?m(?= \(\d+(?:\.\d+)?&nbsp;ft\.?\))");

        /// <summary>
        /// Matches abbreviated in, oz, feet when in brackets e.g. (3 in); avoids false positives such as "3 in 4..."
        /// </summary>
        public static readonly Regex ImperialUnitsInBracketsWithoutNonBreakingSpaces = new Regex(@"(\(\d+(?:\.\d+)?(?:\s*(?:-|–|&mdash;)[\s\u00a0]*\d+(?:\.\d+)?)?)[\s\u00a0]*((?:in|ft|oz)\))");

        #region en only
        /// <summary>
        /// Matches sic either in template or as bracketed text, also related {{typo}} template
        /// </summary>
        public static readonly Regex SicTag = new Regex(@"({{\s*(?:[Ss]ic|[Tt]ypo)(?:\||}})|([\(\[{]\s*[Ss]ic!?\s*[\)\]}]))");
        
        /// <summary>
        /// Matches {{Not a typo}} template and redirects
        /// </summary>
        public static readonly Regex NotATypo = Tools.NestedTemplateRegex(new [] { "As written", "Notatypo", "Not a typo", "Proper name", "Typo" });

        /// <summary>
        /// Matches the {{use dmy dates}} family of templates and redirects
        /// </summary>
        public static readonly Regex UseDatesTemplate = Tools.NestedTemplateRegex(new [] { "DMY", "dmy", "use dmy dates", "MDY", "mdy", "use mdy dates", "ISO", "use ymd dates" }, true );
        
        /// <summary>
        /// Matches dates in American format – "Month dd, YYYY"
        /// </summary>
        public static Regex AmericanDates;
        
        /// <summary>
        /// Matches dates in International format – "dd Month YYYY"
        /// </summary>
        public static Regex InternationalDates;
        
        /// <summary>
        /// Matches month day combinations in American format – "Month dd"
        /// </summary>
        public static Regex MonthDay;
        
        /// <summary>
        /// Matches day month combinations in International format – "dd Month"
        /// </summary>
        public static Regex DayMonth;
        
        /// <summary>
        /// Matches month day ranges – "May 13–17"
        /// </summary>
        public static Regex MonthDayRangeSpan;
        
        /// <summary>
        /// Matches day month ranges – "13–17 May"
        /// </summary>
        public static Regex DayMonthRangeSpan;
        
        /// <summary>
        /// Matches the start of template calls to extract the template name, group 1 is the template name
        /// </summary>
        public static Regex TemplateNameRegex;

        // strictly should accept year form 1583
        /// <summary>
        /// Matches ISO 8601 format dates – YYYY-DD-MM – between 1600 and 2099
        /// </summary>
        public static readonly Regex ISODates = new Regex(@"\b(20\d\d|1[6-9]\d\d)-(0[1-9]|1[0-2])-(0[1-9]|[12]\d|3[01])\b");

        /// <summary>
        /// Quickly matches ISO 8601 format dates – YYYY-DD-MM – between 1600 and 2099. For counting matches
        /// </summary>
        public static readonly Regex ISODatesQuick = new Regex(@"(?<=\b(20\d\d|1[6-9]\d\d))-(0[1-9]|1[0-2])-(0[1-9]|[12]\d|3[01])\b");

        /// <summary>
        /// Matches the {{talk header}} templates and its redirects. Also matches to whitespace after template, but not newline before a heading following
        /// </summary>
        public static readonly Regex TalkHeaderTemplate = new Regex(@"\{\{\s*(?:template *:)?\s*(talk[ _]?(page)?(header)?)\s*(?:\|[^{}]*)?\}\}(?:\s*?(?=\r\n==)|\s*)",
                                                                    RegexOptions.IgnoreCase);

        /// <summary>
        /// Matches the {{skip to talk}} template and its redirects
        /// </summary>
        public static readonly Regex SkipTOCTemplateRegex = new Regex(
            @"\{\{\s*(template *:)?\s*(skiptotoctalk|Skiptotoc|Skip to talk)\s*\}\}\s*",
            RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);

        /// <summary>
        /// Matches the {{WikiProjectBannerShell}} templates and its redirects
        /// </summary>
        public static readonly Regex WikiProjectBannerShellTemplate =
            Tools.NestedTemplateRegex(new[]
                                      {
                                          "WikiProject Banners", "WikiProjectBanners", "WikiProjectBannerShell", "WPBS"
                                              , "WPB", "Wpb", "Wpbs", "Wikiprojectbannershell", "Shell", "Bannershell", "WPBannerShell"
                                      }, true);
        
        /// <summary>
        /// Matches {{no footnotes}} OR {{more footnotes}} templates
        /// </summary>
        public static readonly Regex MoreNoFootnotes = Tools.NestedTemplateRegex(new[] { "no footnotes", "nofootnotes", "more footnotes", "morefootnotes" });

        /// <summary>
        /// Matches the various {{BLP unsourced}} templates
        /// </summary>
        public static readonly Regex BLPSources = new Regex(@"{{\s*([Bb](LP|lp) ?(sources|[Uu]n(sourced|ref(?:erenced)?))|[Uu]n(sourced|referenced) ?[Bb](LP|lp))\b");

        public const string ReferencesTemplates = @"(\{\{\s*(?:[Rr]ef(?:-?li(?:st|nk)|erence)|[Ll]istaref)(?>[^\{\}]+|\{(?<DEPTH>)|\}(?<-DEPTH>))*(?(DEPTH)(?!))\}\}|<\s*[Rr]eferences\s*/>|\{\{refs|<\s*references\s*>.*</\s*references\s*>)";
        
        /// <summary>
        /// Matches a closing &lt;/ref&gt; tag or the {{GR}} template
        /// </summary>
        public const string ReferenceEndGR = @"(?:</ref>|{{GR\|\d}})";

        /// <summary>
        /// Matches any of the recognised templates for displaying cite references e.g. {{reflist}}, {{refs}}, &lt;references/&gt;
        /// </summary>
        public static readonly Regex ReferencesTemplate = new Regex(ReferencesTemplates, RegexOptions.Singleline);

        /// <summary>
        /// Matches any of the recognised templates for displaying cite references followed by a &gt;ref&lt; reference
        /// </summary>
        public static readonly Regex RefAfterReflist = new Regex(ReferencesTemplates + ".*?" + ReferenceEndGR, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        /// <summary>
        /// Matches a line with a bare external link (no description or name of link)
        /// </summary>
        public static readonly Regex BareExternalLink = new Regex(@"^ *\*? *(?:[Hh]ttps?|[Ff]tp|[Mm]ailto)://[^\ \n\r<>]+\s+$", RegexOptions.Multiline);
        
        /// <summary>
        /// Matches a bare external link (URL only, no title) within a &lt;ref&gt; tag, group 1 being the URL
        /// </summary>
        public static readonly Regex BareRefExternalLink = new Regex(@"<\s*ref\b[^<>]*>\s*(?<brace>\[+)?\s*((?:https?|ftp|mailto)://[^\ \n\r<>]+?)\s*(?(brace)\]+|)[\.,:;\s]*<\s*/\s*ref\s*>", RegexOptions.IgnoreCase);
        // (?<test>a)?b(?(test)c|d)
        /// <summary>
        /// Matches an external link (URL only, no title) within a &lt;ref&gt; tag with a bot generated title or no title, group 1 being the URL, group 2 being the title, if any
        /// </summary>
        public static readonly Regex BareRefExternalLinkBotGenTitle = new Regex(@"<\s*ref\b[^<>]*>\s*\[*\s*((?:https?|ftp|mailto)://[^\ \n\r<>]+?)\s*(?:\s+([^<>{}]+?)\s*<!--\s*Bot generated title\s*-->)?\]*[\.,:;\s""]*<\s*/\s*ref\s*>", RegexOptions.IgnoreCase);
        
        /// <summary>
        /// Matches the various citation templates {{citation}}, {{cite web}} etc. on en-wiki
        /// </summary>
        public static readonly Regex CiteTemplate = Tools.NestedTemplateRegex(new [] { "cite web", "cite news", "cite journal", "cite book", "citation", "cite conference", "cite hansard", "cite manual", "cite paper", "cite press release", "cite encyclopedia", "cite AV media", "vcite2 journal" }, false);
        
        /// <summary>
        /// Matches the various url templates {{URL}} etc. on en-wiki
        /// </summary>
        public static readonly Regex UrlTemplate = Tools.NestedTemplateRegex(new [] { "URL", "Website", "Url", "Official website" }, false);

        /// <summary>
        /// Matches the various Harvard citation templates on en-wiki
        /// </summary>
        public static readonly Regex HarvTemplate = Tools.NestedTemplateRegex(new [] { "Harvard citation", "harv", "harvsp", "Harvard citation no brackets", "harvnb", "Harvard citation text", "harvtxt", "Harvcol", "Harvcolnb", "Harvcoltxt" }, false);
        
        /// <summary>
        /// Matches {{persondata}} template, or {{personendaten}} on de-wiki
        /// </summary>
        public static Regex Persondata;
        
        /// <summary>
        /// The default blank Persondata template for en-wiki, from [[Template:Persondata#Usage]]
        /// </summary>
        public const string PersonDataDefault = @"{{Persondata
| NAME              =
| ALTERNATIVE NAMES =
| SHORT DESCRIPTION =
| DATE OF BIRTH     =
| PLACE OF BIRTH    =
| DATE OF DEATH     =
| PLACE OF DEATH    =
}}";

        /// <summary>
        /// Comment often put on the line before the Persondata template on the en-wiki
        /// </summary>
        public const string PersonDataCommentEN = @"<!-- Metadata: see [[Wikipedia:Persondata]] -->
";
        /// <summary>
        /// Matches the various categories for dead people on en wiki, and the living people category
        /// </summary>
        public static readonly Regex DeathsOrLivingCategory = new Regex(@"\[\[\s*Category *:[ _]?(\d{1,2}\w{0,2}[- _]century(?: BC)?[ _]deaths|[0-9s]{3,5}(?: BC)?[ _]deaths|Missing[ _]people|Living[ _]people|(?:Date|Year)[ _]of[ _]death[ _](?:missing|unknown|uncertain)|Possibly[ _]living[ _]people|People[ _]declared[ _]dead[ _]in[ _]absentia) *(?:\|.*?)?\]\]", RegexOptions.IgnoreCase | RegexOptions.RightToLeft);
        
        /// <summary>
        /// Matches the {{recentlydeceased}} templates and its redirects
        /// </summary>
        public static readonly Regex LivingPeopleRegex2 = new Regex(@"\{\{\s*(Template:)?(Recent ?death|Recentlydeceased)\}\}", RegexOptions.IgnoreCase);
        
        /// <summary>
        /// Matches the XXXX births / xxth Century / XXXX BC births categories (en only)
        /// </summary>
        public static readonly Regex BirthsCategory = new Regex(@"\[\[ ?Category ?:[ _]?(?:(\d{3,4})(?:s| BC)?|\d{1,2}\w{0,2}[- _]century)[ _]births(\|.*?)?\]\]", RegexOptions.IgnoreCase | RegexOptions.RightToLeft);
        
        /// <summary>
        /// Matches the "People from ..." en-wiki categories
        /// </summary>
        public static readonly Regex PeopleFromCategory = new Regex(@"\[\[ ?Category ?: *People from .*?\]\]");

        /// <summary>
        /// Matches the various {{birth date and age}} templates, group 1 being the year of birth
        /// </summary>
        public static readonly Regex DateBirthAndAge = new Regex(@"{{\s*[Bb](?:(?:da|irth ?date(?: and age)?)\s*(?:\|\s*[md]f\s*=\s*y(?:es)?\s*)?\|\s*|irth-date\s*\|[^{}\|]*?)(?:year *= *)?\b([12]\d{3})\s*(?:\||}})");

        /// <summary>
        /// Matches the various {{death date}} templates, group 1 being the year of death
        /// </summary>
        public static readonly Regex DeathDate = new Regex(@"{{\s*[Dd]eath(?: date(?: and age)?\s*(?:\|\s*[md]f\s*=\s*y(?:es)?\s*)?\|\s*|-date\s*\|[^{}\|]*?)(?:year *= *)?\b([12]\d{3})\s*(?:\||}})");

        /// <summary>
        /// Matches the {{death date and age}} template, group 1 being the year of death, group 2 being the year of birth
        /// </summary>
        public static readonly Regex DeathDateAndAge = new Regex(@"{{\s*[Dd](?:eath[ -]date and age|da)\s*\|(?:[^{}]*?\|)?\s*([12]\d{3})\s*\|[^{}]+?\|\s*([12]\d{3})\s*\|.*}}");

        /// <summary>
        /// Matches {{Link FA|xxx}}, {{Link GA|xxx}}, {{Link FL|xxx}}
        /// </summary>
        public static Regex LinkFGAs;

        /// <summary>
        /// Matches {{Deadend|xxx}}, including in {{Multiple issues}}, group 1 is the {{Multiple issues}} template call up to the dead end tag
        /// </summary>
        public static Regex DeadEnd;

        /// <summary>
        /// Matches {{wikify}}, {{underlinked}} tag (including within {{multiple issues}} for en-wiki)
        /// </summary>
        public static Regex Wikify;

        /// <summary>
        /// Matches {{Centuryinbox}} template and its redirects
        /// </summary>
        public static readonly Regex Centuryinbox = Tools.NestedTemplateRegex(new [] { "Centuryinbox" });

        /// <summary>
        /// Matches {{dead link}} template and its redirects
        /// </summary>
        public static readonly Regex DeadLink = Tools.NestedTemplateRegex(new [] { "dead link", "deadlink", "broken link", "brokenlink", "link broken", "linkbroken", "404", "dl", "dl-s", "cleanup-link" });

        /// <summary>
        /// Matches wikilinks with no target e.g. [[|foo]]
        /// </summary>
        private const string AllowedCharacters = @"([\w\s\-–\+\(\),\'\.&\!\?\$]*)";
        public static readonly Regex TargetLessLink =  new Regex(@"\[\[\|"+AllowedCharacters+@"\]\]");

        /// <summary>
        /// Matches wikilinks with double pipes e.g. [[text|text2|text3]] and [[text||text3]]
        /// </summary>
        public static readonly Regex DoublePipeLink =  new Regex(@"\[\["+AllowedCharacters+@"\|"+AllowedCharacters+@"\|"+AllowedCharacters+@"\]\]");

        /// <summary>
        /// 
        /// </summary>
        public static readonly Regex CircaLinkTemplate = new Regex(@"({{[Cc]irca}}|\[\[[Cc]irca *(?:\|[Cc]a?\.?)?\]\]|\[\[[Cc]a?\.?\]*\.?)");

        /// <summary>
        /// 
        /// </summary>
        public static readonly Regex UnlinkedFloruit = new Regex(@"\(\s*(?:[Ff]l)\.*\s*(\d\d)");

        /// <summary>
        /// Matches {{orphan}} tag, including in {{Multiple issues}}, named group MI is the {{Multiple issues}} template call up to the orphan tag
        /// </summary>
        public static Regex Orphan;

        /// <summary>
        /// Matches uncategorised templates: {{Uncat}}, {{Uncategorised}}, {{Uncategorised stub}} allowing for nested subst: {{uncategorised|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}
        /// </summary>
        public static Regex Uncat;
        
        /// <summary>
        /// Matches the {{In use}}/{{in creation}} templates
        /// </summary>
        public static Regex InUse;

        /// <summary>
        /// Matches the {{Cat improve}} template and its redirects
        /// </summary>
        public static readonly Regex CatImprove = Tools.NestedTemplateRegex(new [] { "CI", "Cleanup-cat", "Cleanup cat", "Few categories", "Few cats", "Fewcategories", "Fewcats", "Improve-categories", "Improve-cats", "Improve categories", "Improve cats",
                                                                                "Improvecategories", "Improvecats", "More categories", "More category", "Morecat", "Morecategories", "Morecats", "Cat-improve", "Category-improve",
                                                                                "Categories-improve", "Category improve", "Categories improve", "Catimprove", "More cats" });

        /// <summary>
        /// Matches the {{Multiple issues}} template
        /// </summary>
        public static readonly Regex MultipleIssues = Tools.NestedTemplateRegex(new [] { "multipleissues", "multiple issues", "mi", "MI", "multiple", "issues", "Articleissues", "Article issues" });

        /// <summary>
        /// Matches the {{New unreviewed article}} template
        /// </summary>
        public static readonly Regex NewUnReviewedArticle = Tools.NestedTemplateRegex("new unreviewed article");

        /// <summary>
        /// The cleanup templates that can be moved into the {{multiple issues}} template
        /// </summary>
        public const string MultipleIssuesTemplatesString = @"([Aa]bbreviations|[Aa]dvert|[Aa]utobiography|[Bb]iased|[Bb]lpdispute|BLPrefimprove|BLP IMDB refimprove|BLP ?sources|BLPunref(?:erenced)?|BLP ?unsourced|[Cc]itations missing|[Cc]itation ?style|[Cc]ite ?check|[Cc]leanup|[Cc]leanup-laundry|[Cc]leanup-link rot|[Cc]leanup-reorgani[sz]e|[Cc]leanup-spam|COI|[Cc]oi|[Cc]olloquial|[Cc]onfusing|[Cc]ontext|[Cc]ontradict|[Cc]opy ?edit|[Cc]riticisms|[Cc]riticism section|[Cc]rystal|[Dd]ead ?end|[Dd]isputed|[Dd]o-attempt|[Ee]ssay(?:\-like)?|[Ee]xample ?farm|[Ee]xpand|[Ee]xpert|[Ee]xpert-subject|[Ee]xternal links|[Ff]ancruft|[Ff]anpov|[Ff]ansite|[Ff]iction|[Gg]ame ?guide|[Gg]lobalize|[Gg]rammar|[Hh]istinfo|[Hh]oax|[Hh]owto|[Ii]nappropriate person|[Ii]n-universe|[Ii]ncomplete|[Ii]ntro(?: length|\-too(?:long|short))|[Ii]ntromissing|[Ii]ntrorewrite|[Cc]leanup-jargon|[Ll]aundry(?:lists)?|[Ll]ead (?:missing|rewrite|too long|too short)|[Ll]ike ?resume|(?:[Vv]ery ?l|[Ll])ong|[Mm]ore footnotes|[Nn]ews ?release|[Nn]o footnotes|[Nn]otab(?:le|ility)|[Oo]ne ?source|[Oo]riginal[ -][Rr]esearch|[Oo]rphan|[Oo]ut of date|[Oo]verly detailed|[Oo]ver-quotation|[Pp]eacock|[Pp]lot|N?POV|n?pov|[Pp]ov\-check|POV-check|[Pp]rimary ?sources|[Pp]rose(?:line)?|[Qq]uotefarm|[Rr]ecent(?:ism)?|[Rr]efimprove(?:BLP)?|[Rr]estructure|[Rr]eview|(?:[Cc]leanup\-r|[Rr])ewrite|[Ss]ections|[Ss]elf-published|[Ss]pam|[Ss]tory|[Ss]ynthesis|[Tt]echnical|[Ii]nappropriate tone|[Tt]one|[Tt]oo(?:short|long)|[Tt]ravel ?guide|[Tt]rivia|[Uu]nbalanced|[Uu]nderlinked|[Uu]nencyclopedic|[Uu]nref(?:erenced(?:BLP)?|BLP)?|[Uu]nreliable sources|[Uu]pdate|[Ww]easel|[Ww]ikify)";

        /// <summary>
        /// Matches the cleanup templates that can be moved into the {{multiple issues}} template
        /// </summary>
        public static readonly Regex MultipleIssuesTemplateNameRegex = new Regex(MultipleIssuesTemplatesString);

        /// <summary>
        /// Matches COI|OR|POV|BLP
        /// </summary>
        public static readonly Regex CoiOrPovBlp = new Regex("(COI|OR|POV|BLP)");

        /// <summary>
        /// Localized version of date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}
        /// </summary>
        public static string DateYearMonthParameter;
        
        /// <summary>
        /// Matches the cleanup templates that can be moved into the {{multiple issues}} template, notably does not match templates with multiple parameters
        /// </summary>
        public static readonly Regex MultipleIssuesTemplates = new Regex(@"{{" + MultipleIssuesTemplatesString + @"\s*(?:(?:\|\s*for\s*=\s*grammar\s*)?\|\s*([^{}\|]*?(?:{{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}[^{}\|]*?)?))?\s*}}");

        /// <summary>
        /// Matches the cleanup templates that can be moved into the {{multiple issues}} article-level template
        /// </summary>
        public static readonly Regex MultipleIssuesArticleMaintenanceTemplates = Tools.NestedTemplateRegex(new [] { "Abbreviations", "Advert", "All plot", "Alumni", "Autobiography", "BLP IMDb refimprove", "BLP primary sources", "BLP sources", "BLP unsourced", "Biblio", "Buzzword", "Citation style", "Citations broken", "Cite check", "Cleanup", "Cleanup red links", "Cleanup-biography", "Cleanup-laundry", "Cleanup-bare URLs", "Cleanup-list", "Cleanup-reorganize", "Cleanup-rewrite", "Cleanup-school", "Cleanup-spam", "Cleanup-tense", "COI", "Colloquial", "Condense", "Confusing", "Context", "Contradict", "Contradict-other", "Copy edit", "Criticism section", "Crystal", "Dead end", "Disputed", "Editorial", "Essay", "Essay-like", "Example farm", "Expand article", "Expert-subject", "External links", "Fanpov", "Fiction", "Format footnotes", "Game guide", "Generalize", "Globalize", "Globalize/North America", "Globalize/US", "Historical information needed", "Hoax", "Howto", "Inadequate lead", "Inappropriate person", "Incoherent", "Incomplete", "In-universe", "Lead missing", "Lead rewrite", "Lead too long", "Lead too short", "Like resume", "Local", "Magazine", "Manual", "Medref", "Missing information", "More footnotes", "MOS", "MOSLOW", "Neologism", "News release", "No footnotes", "Non-free", "Notability", "NPOV language", "Obituary", "One source", "Original research", "Orphan", "Out of date", "Over coverage", "Over-quotation", "Overlinked", "Overly detailed", "Page numbers needed", "Peacock", "Plot", "POV", "POV-check", "Primary sources", "Prose", "Prune", "Recentism", "Refimprove", "Religious text primary", "Repetition", "Review", "Sections", "Self-published", "Story", "Synthesis", "Technical", "Third-party", "Tone", "Too few opinions", "Travel guide", "Trivia", "Unbalanced", "Underlinked", "Undue", "Unreferenced", "Unreliable sources", "Update", "USgovtPOV", "Very long", "Video game cleanup", "Weasel", "Wikify" } );

        /// <summary>
        /// Matches the cleanup templates that can be moved into the {{multiple issues}} section-level template
        /// </summary>
        public static readonly Regex MultipleIssuesSectionMaintenanceTemplates = Tools.NestedTemplateRegex(new [] { "BLP sources section", "BLP unsourced section", "Cleanup section", "Confusing section", "Copy edit-section", "Criticism section", "Disputed-section", "Expand section", "Importance-section", "POV-section", "Refimprove section", "Rewrite section", "Unreferenced section", "Update section", "Wikify section" } );

        /// <summary>
        /// Matches the "reflist", "references-small", "references-2column" references display templates
        /// </summary>
        public static Regex ReferenceList;

        /// <summary>
        /// Matches infoboxes, group 1 being the template name of the infobox
        /// </summary>
        public static readonly Regex InfoBox = new Regex(@"(?:{{[\s_]*)(?:[Tt]emplate[\s_]*:[\s_]*)?([Ii]nfobox(?:[\s_]+[^{}\|\s][^{}\|]+?)?|[^{}\|]+?[Ii]nfobox)\s*\|(?>[^\{\}]+|\{(?<DEPTH>)|\}(?<-DEPTH>))*(?(DEPTH)(?!))}}");
        
        /// <summary>
        /// Matches people infoxboxes from Category:People infobox templates
        /// </summary>
        public static readonly Regex PeopleInfoboxTemplates = Tools.NestedTemplateRegex(new [] { "Infobox college coach", "Infobox American Indian chief", "Infobox Native American leader", "Infobox Calvinist theologian",
                                                                                        	"Infobox Chinese-language singer and actor", "Infobox Christian leader", "Infobox FBI Ten Most Wanted", "Infobox Jewish leader",
                                                                                            "Infobox Playboy Cyber Girl", "Infobox Playboy Playmate", "Infobox actor", "Infobox adult biography", "Infobox adult female", "Infobox adult male",
                                                                                            "Infobox architect", "Infobox artist", "Infobox astronaut", "Infobox aviator", "Infobox bishop", "Infobox minister of religion", "Infobox religious biography",
                                                                                            "Infobox cardinal", "Infobox chef", "Infobox chess player", "Infobox clergy", "Infobox comedian",
                                                                                            "Infobox comics creator", "Infobox criminal", "Infobox dancer", "Infobox economist", "Infobox engineer", "Infobox fashion designer", "Infobox go player",
                                                                                            "Infobox imam", "Infobox journalist", "Infobox LDSGA", "Infobox male model", "Infobox martyrs", "Infobox mass murderer", "Infobox medical person",
                                                                                            "Infobox member of the Knesset", "Infobox military person", "Infobox model", "Infobox musical artist", "Infobox officeholder",
                                                                                            "Infobox performer", "Infobox person", "Infobox philosopher", "Infobox pirate", "Infobox poker player", "Infobox police officer", "Infobox pope",
                                                                                            "Infobox presenter", "Infobox rebbe", "Infobox religious biography", "Infobox saint", "Infobox scientist",
                                                                                            "Infobox serial killer", "Infobox sports announcer", "Infobox spy", "Infobox theologian", "Infobox murderer",
                                                                                            "Infobox university chancellor", "Infobox vandal", "Infobox writer", "Infobox Governor",
                                                                                            "Infobox senator", "Infobox Mayor", "Infobox Politician", "Infobox Chancellor", "Infobox US Chief Justice", "Infobox Vice President", "Infobox US Associate Justice",
                                                                                            "Infobox Indian politician", "Infobox Congressman", "Infobox Governor General", "Infobox US Cabinet official", "Infobox Prime Minister",
                                                                                            "Infobox US Territorial Governor", "Infobox Congressional Candidate", "Infobox CanadianMP", "Infobox Lt Governor", "Infobox Eritrea Cabinet official", "Infobox PM",
                                                                                            "Infobox President", "Infobox CanadianSenator", "Infobox Governor-General", "Infobox SCC Puisne Justice", "Infobox SCC Chief Justice", "Infobox Representative Elect",
                                                                                            "Infobox Senator-Elect", "Infobox Governor-Elect", "Infobox State SC Justice", "Infobox State SC Associate Justice", "Infobox Politician (general)", "Infobox US Ambassador",
                                                                                            "Infobox MP", "Infobox State Representative", "Infobox State Senator", "Infobox Judge", "Infobox Premier", "Infobox Secretary-General", "Infobox Ambassador", "Infobox President-elect",
                                                                                            "Infobox Prime Minister-elect", "Infobox AM", "Infobox Speaker", "Infobox First Minister", "Infobox Minister", "Infobox Deputy Prime Minister", "Infobox Deputy First Minister",
                                                                                            "Infobox Representative-elect", "Infobox Senator-elect", "Infobox Governor-elect", "Infobox QuebecMNA", "Infobox MEP", "Infobox MLA", "Infobox Congresswoman", "Infobox Senator",
                                                                                            "Infobox Defense Minister", "Infobox OntarioMPP", "Infobox Uruguayan politician", "Infobox New York State Senator", "Infobox First Lady", "Infobox Chief Justice", "Infobox Indian government official",
                                                                                            "Infobox Doge", "Infobox MSP", "Infobox candidate", "Infobox Officeholder", "Infobox politician", "Infobox judge", "Infobox Member of Parliament", "Infobox president", "Infobox civil servant",
                                                                                            "Infobox Welsh Assembly member", "Infobox General Secretary", "Infobox Chief Executive", "Infobox ambassador", "Infobox prime minister", "Infobox Canadian MP", "Infobox Canadian senator", "Infobox chancellor",
                                                                                            "Infobox congressional candidate", "Infobox congressman", "Infobox defense minister", "Infobox deputy first minister", "Infobox deputy prime minister", "Infobox doge", "Infobox first lady",
                                                                                            "Infobox first minister", "Infobox governor", "Infobox governor-elect", "Infobox governor general", "Infobox governor-general", "Infobox lt governor", "Infobox mayor", "Infobox minister",
                                                                                            "Infobox politician (general)", "Infobox premier", "Infobox president-elect", "Infobox prime minister-elect", "Infobox representative-elect", "Infobox SCC chief justice", "Infobox SCC puisne justice",
                                                                                            "Infobox secretary-general", "Infobox senator-elect", "Infobox speaker", "Infobox state representative", "Infobox state SC associate justice", "Infobox state SC justice", "Infobox state senator",
                                                                                            "Infobox US associate justice", "Infobox US cabinet official", "Infobox US chief justice", "Infobox US territorial governor", "Infobox vice president", "Infobox US ambassador", "Infobox Eritrea cabinet official",
                                                                                            "Infobox sportsperson", "Infobox NFL player", "Infobox football biography", "Infobox football official", "Infobox golfer", "Infobox gridiron football person", "Infobox gymnast", "Infobox handball biography,",
                                                                                            "Infobox ice hockey player", "Infobox motorcycle rider", "Infobox rugby biography", "Infobox rugby league biography", "Infobox Rugby Union biography", "Infobox rugby union biography", "Infobox cricketer", "Infobox nobility",
                                                                                            "Infobox noble", "Infobox tennis biography", "Infobox pro football player", "Infobox badminton player", "Infobox basketball official", "Infobox bodybuilder", "Infobox boxer", "Infobox MLB player", "Infobox basketball biography",
                                                                                        	"Infobox NCAA athlete", "Infobox netball biography", "Infobox swimmer", "Infobox WNBA biography", "Infobox Muslim scholar", "Infobox sumo wrestler"}, false);

        /// <summary>
        /// Matches the {{circa}} template
        /// </summary>
        public static readonly Regex CircaTemplate = Tools.NestedTemplateRegex(new[] {"Circa", "c."}, true);
        
        /// <summary>
        /// Matches named references in format &lt;ref name="foo"&gt;text&lt;/ref&gt; Ref name is group 2, ref value is group 3
        /// </summary>
        public static readonly Regex NamedReferences = new Regex(@"(<\s*ref\s+name\s*=\s*(?:""|')?([^<>=\r\n/]+?)(?:""|')?\s*>\s*(.*?)\s*<\s*/\s*ref\s*>)", RegexOptions.Singleline | RegexOptions.IgnoreCase);

        /// <summary>
        /// Matches named references in format &lt;ref name="foo"&gt;text&lt;/ref&gt; or &lt;ref name="foo" /&gt; Ref name is group 2, ref value is group 3
        /// </summary>
        public static readonly Regex NamedReferencesIncludingCondensed = new Regex(@"(<\s*ref\s+(?i)name(?-i)\s*=\s*(?:""|')?([^<>=\r\n]+?)(?:""|')?\s*(?:>\s*(.*?)\s*<\s*/\s*ref|/)\s*>)", RegexOptions.Singleline);

        /// <summary>
        /// Matches unnamed references in format &lt;ref&gt;...&lt;/ref&gt; group 1 being the reference text
        /// </summary>
        public static readonly Regex UnnamedReferences = new Regex(@"<\s*ref\s*>((?>.(?<!<\s*ref\b[^>/]*>|<\s*/\s*ref\s*>)|<\s*ref\b[^>/]*>(?<DEPTH>)|<\s*/\s*ref\s*>(?<-DEPTH>))*(?(DEPTH)(?!)))<\s*/\s*ref\s*>", RegexOptions.Singleline);        

        // covered by DablinksTests
        /// <summary>
        /// Finds article disamiguation links from https://en.wikipedia.org/wiki/Wikipedia:Template_messages/General#Disambiguation_and_redirection (en only)
        /// </summary>
        public static readonly Regex Dablinks = Tools.NestedTemplateRegex(new [] { "about", "ambiguous link", "for", "for2", "for3", "dablink", "distinguish", "distinguish2", "hatnote", "otherpeople", "otherpeople1", "otherpeople2", "otherpeople3", "other people", "other people1", "other people2", "other people3", "other persons", "otherpersons", "otherpersons2", "otherpersons3", "otherplaces", "other places", "otherplaces3", "other places3", "otherships", "other ships", "otheruses-number", "other uses", "other uses2", "other uses3", "other uses4", "other uses6", "otheruses", "otheruses2", "otheruses3", "otheruses4", "other uses of", "otheruse", "2otheruses", "redirect-acronym", "redirect", "redirect2", "redirect3", "redirect4", "this", "two other uses", "three other uses", "disambig-acronym", "selfref" }, false);

        /// <summary>
        /// Matches the sister links templates such as {{wiktionary}}
        /// </summary>
        public static readonly Regex SisterLinks = Tools.NestedTemplateRegex(new[] { "wiktionary", "sisterlinks", "sister links", "sister project links", "wikibooks", "wikimedia", "wikiversity" }, false );
        
        /// <summary>
        /// Matches the maintenance tags (en-wiki only) such as orphan, cleanup
        /// </summary>
        public static readonly Regex MaintenanceTemplates = Tools.NestedTemplateRegex(new[] { "orphan", "BLP unsourced", "BLP sources", "cleanup", "underlinked", "dead end", "notability", "refimprove", "unreferenced" }, false );
        
        /// <summary>
        /// Matches the {{Unreferenced}} template, or parameter within old-style multiple issues template
        /// </summary>
        public static readonly Regex Unreferenced = new Regex(@"(?:{{\s*([Uu]nreferenced( stub)?)\s*(?:\|.*?)?}}|({{\s*(?:[Aa]rticle|[Mm]ultiple)\s*issues\b[^{}]*?(?:{{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}})?[^{}]*?)*\|\s*unreferenced\s*=\s*(?:{{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|[^{}\|]+))", RegexOptions.Singleline);
        
        /// <summary>
        /// Matches {{Portal}} template
        /// </summary>
        public static readonly Regex PortalTemplate = Tools.NestedTemplateRegex("portal");
        #endregion

        // to find only matching comments use <!--(?>(?!<!--|-->).|<!--(?<Depth>)|-->(?<-Depth>))*(?(Depth)(?!))-->
        /// <summary>
        /// Matches &lt;!-- comments --&gt;
        /// </summary>
        public static readonly Regex Comments = new Regex(@"<!--.*?-->", RegexOptions.Singleline);

        /// <summary>
        /// Matches text within &lt;p style...&gt;...&lt;/p&gt; HTML tags
        /// </summary>
        public static readonly Regex Pstyles = new Regex(@"<p style\s*=\s*[^<>]+>.*?<\s*/p>", RegexOptions.IgnoreCase | RegexOptions.Singleline);

        /// <summary>
        /// Matches empty comments (zero or more whitespace)
        /// </summary>
        public static readonly Regex EmptyComments = new Regex(@"<!--[^\S\r\n]*-->");

        /// <summary>
        /// Matches empty bold  wikitags (zero or more whitespace)
        /// </summary>
        public static readonly Regex EmptyBold = new Regex(@"'{3}\s*'{3}");

        /// <summary>
        /// Matches {{Short pages monitor}} plus comment, generated from {{subst:Long comment}}
        /// </summary>
        public static readonly Regex ShortPagesMonitor = new Regex(@"\s*{{[sS]hort pages monitor}}<!--[^<>]+-->");

        /// <summary>
        /// Matches &lt;ref&gt; tags, including named references and condensed named references
        /// </summary>
        public static readonly Regex Refs = new Regex(@"(<\s*ref\s+(?:name|group)\s*=\s*[^<>]*?/\s*>|<\s*ref\b[^<>]*>(?>.(?<!<\s*ref\b[^>/]*?>|<\s*/\s*ref\s*>)|<\s*ref\b[^>/]*>(?<DEPTH>)|<\s*/\s*ref\s*>(?<-DEPTH>))*(?(DEPTH)(?!))<\s*/\s*ref\s*>)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        
        /// <summary>
        /// Matches &lt;ref&gt; tags with group parameter, optionally named as well. Does not match regular named references
        /// </summary>
        public static readonly Regex RefsGrouped = new Regex(@"(<\s*ref\s+(?:name\s*=\s*[^<>]*?\s*)?group\s*=\s*[^<>]*?/\s*>|<\s*ref\s+(?:name\s*=\s*[^<>]*?\s*)?group\s*=\s*[^<>]*?>(?>.(?<!<\s*ref\s+group\b[^>/]*?>|<\s*/\s*ref\s*>)|<\s*ref\s+group\b[^>/]*?>(?<DEPTH>)|<\s*/\s*ref\s*>(?<-DEPTH>))*(?(DEPTH)(?!))<\s*/\s*ref\s*>)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        
        /// <summary>
        /// Matches &lt;cite&gt; tags
        /// </summary>
        public static readonly Regex Cites = new Regex(@"<cite[^>]*?>[^<]*<\s*/cite\s*>", RegexOptions.Singleline);
        
        /// <summary>
        /// Matches &lt;/i&gt;...&lt;i&gt; reversed italics tags, group 1 being the content between the tags
        /// </summary>
        public static readonly Regex ReversedItalics = new Regex(@"< */ *i *>([^<]*)< *i *>(?![^<]*< */ *i *>)");

        /// <summary>
        /// Matches &lt;nowiki&gt; tags
        /// </summary>
        public static readonly Regex Nowiki = new Regex(@"<nowiki\s*>.*?</nowiki\s*>", RegexOptions.Singleline | RegexOptions.IgnoreCase);

        /// <summary>
        /// Matches &lt;small&gt; tags, including nested tags. Group 1 is the text within the tags
        /// </summary>
        public static readonly Regex Small = new Regex(@"<\s*small\s*>((?>(?!<\s*/?\s*small\s*>).|<\s*small\s*>(?<DEPTH>)|<\s*/\s*small\s*>(?<-DEPTH>))*(?(DEPTH)(?!)))<\s*/\s*small\s*>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
        
        /// <summary>
        /// Matches &lt;big&gt; tags
        /// </summary>
        public static readonly Regex Big = new Regex(@"<\s*big\s*>((?>(?!<\s*big\s*>|<\s*/\s*big\s*>).|<\s*big\s*>(?<DEPTH>)|<\s*/\s*big\s*>(?<-DEPTH>))*(?(DEPTH)(?!)))<\s*/\s*big\s*>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
        
        /// <summary>
        /// Matches &lt;sup&gt; and &lt;sub&gt; tags
        /// </summary>
        public static readonly Regex SupSub = new Regex(@"<(?<key>su(?:p|b))>(.*?)</\k<key>>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
        
        /// <summary>
        /// Matches templates, including templates with the template namespace prefix
        /// </summary>
        public static Regex TemplateCall;

        /// <summary>
        /// No General Fixes regex for checkpage parsing
        /// </summary>
        public static readonly Regex NoGeneralFixes = new Regex("<!--No general fixes:.*?-->", RegexOptions.Singleline);

        /// <summary>
        /// No RegexTypoFix regex for checkpage parsing
        /// </summary>
        public static readonly Regex NoRETF = new Regex("<!--No RETF:.*?-->", RegexOptions.Singleline);

        /// <summary>
        /// For extraction of page title from URLs
        /// </summary>
        public static Regex ExtractTitle;

        /// <summary>
        /// 
        /// </summary>
        public static Regex EmptyLink;

        /// <summary>
        /// 
        /// </summary>
        public static Regex EmptyTemplate;

        /// <summary>
        /// Matches bold italic text, group 1 being the text in bold italics (wiki format ''''' only)
        /// </summary>
        public static readonly Regex BoldItalics = new Regex(@"(?<!')'{5}((?:[^']+|.*?[^'])?)'{5}(?!')");

        /// <summary>
        /// Matches italic text, group 1 being the text in italics (wiki format '' only)
        /// </summary>
        public static readonly Regex Italics = new Regex(@"(?<!')'{2}((?:[^']+|[^'].*?[^'])?)'{2}(?!')");

        /// <summary>
        /// Matches bold text, group 1 being the text in bold (wiki format ''' only)
        /// </summary>
        public static readonly Regex Bold = new Regex(@"(?<!')'{3}((?:[^']+|.*?[^'])?)'{3}(?!')");
        
        /// <summary>
        /// Matches the &lt;br/&gt; tag and valid variants
        /// </summary>
        public static readonly Regex Br = new Regex("< *br */?>", RegexOptions.IgnoreCase);
        
        /// <summary>
        /// Matches a row beginning with an asterisk, allowing for spaces before
        /// </summary>
        public static readonly Regex StarRows = new Regex(@"^ *(\*)(.*)", RegexOptions.Multiline);

        /// <summary>
        /// Matches the References level 2 heading
        /// </summary>
        public static readonly Regex ReferencesHeader = new Regex(@"== *References *==", RegexOptions.IgnoreCase | RegexOptions.RightToLeft);
        
        /// <summary>
        /// Matches the Notes level 2 heading
        /// </summary>
        public static readonly Regex NotesHeader = new Regex(@"== *Notes *==", RegexOptions.IgnoreCase | RegexOptions.RightToLeft);

        /// <summary>
        /// Matches the external links level 2 heading
        /// </summary>
        public static readonly Regex ExternalLinksHeader = new Regex(@"== *External +links? *==", RegexOptions.IgnoreCase | RegexOptions.RightToLeft);

        /// <summary>
        /// Matches the 'See also' level 2 heading
        /// </summary>
        public static readonly Regex SeeAlso = new Regex(@"(==+)\s*see +also\s*\1", RegexOptions.IgnoreCase);
        
        /// <summary>
        /// Matches parameters within the {{multiple issues}} template using title case (invalid casing)
        /// </summary>
        public static readonly Regex MultipleIssuesInTitleCase = new Regex(@"({{\s*(?:[Aa]rticle|[Mm]ultiple) ?issues\|\s*(?:[^{}]+?\|\s*)?)([A-Z])([a-z]+[ a-zA-Z]*\s*=)");

        /// <summary>
        /// Matches a number between 1000 and 2999
        /// </summary>
        public static readonly Regex GregorianYear = new Regex(@"\b[12]\d{3}\b");
        
        /// <summary>
        /// Matches a percentage with a space or a non-breaking space
        /// </summary>
        public static readonly Regex Percent = new Regex(@"\s(\(?[-±\+]?\d+\.?\d*)(\s|\s?\&nbsp;)%(\p{P}|\)?\s)");

        /// <summary>
        /// Matches a currency symbol with a space or a non-breaking space
        /// </summary>
        public static readonly Regex Currency = new Regex(@"(€|£|\$)(\s|\s?\&nbsp;)(\d+\.?\d*(\p{P}|\)?\s))");

        /// <summary>
        /// Matches 12-hour clock time without a space
        /// </summary>
        public static readonly Regex ClockTime = new Regex(@"(\b[01]?\d\:[0-5]\d) ?([ap]\.m\.)");

        /// <summary>
        /// List of known infobox fields holding date of birth
        /// </summary>
        public static readonly List<string> InfoBoxDOBFields = new List<string>(new [] {"yearofbirth", "dateofbirth", "date of birth", "datebirth", "born", "birth date", "birthdate", "birth_date", "birth"});
        
        /// <summary>
        /// List of known infobox fields holding date of death
        /// </summary>
        public static readonly List<string> InfoBoxDODFields = new List<string>(new [] {"yearofdeath", "datedeath", "dateofdeath", "date of death", "died", "death date", "deathdate", "death_date", "death"});
        
        public static readonly List<string> InfoBoxPOBFields = new List<string>(new [] {"birthplace", "Birthplace", "birth_place", "placeofbirth", "place of birth", "place_of_birth", "placebirth"});
        
        public static readonly List<string> InfoBoxPODFields = new List<string>(new [] {"deathplace", "Deathplace", "death_place", "placeofdeath", "place of death", "place_of_death", "placedeath", "place_death"});
        
        /// <summary>
        /// Matches "ibid" and "op cit"
        /// </summary>
        public static readonly Regex IbidLocCitation = new Regex(@"\b(ibid|loc.{1,4}cit|Page ?needed)\b", RegexOptions.IgnoreCase | RegexOptions.Singleline);

        /// <summary>
        /// Matches the {{Ibid}} template
        /// </summary>
        public static readonly Regex Ibid = Tools.NestedTemplateRegex(@"Ibid");

        /// <summary>
        /// Matches the {{Wikipedia books}} template
        /// </summary>
        public static readonly Regex WikipediaBooks = Tools.NestedTemplateRegex(new [] {"Wikipedia-Books", "wikipedia books"});

        /// <summary>
        /// Matches consecutive whitespace
        /// </summary>
        public static readonly Regex WhiteSpace = new Regex(@"\s+");
        
        /// <summary>
        /// List of PAGENAME, PAGENAMEE, BASEPAGENAME, BASEPAGENAMEE templates
        /// </summary>
        public static readonly List<string> BASEPAGENAMETemplatesL = new List<string>(new [] {"PAGENAME", "PAGENAMEE", "BASEPAGENAME", "BASEPAGENAMEE"});
        
        /// <summary>
        /// Matches PAGENAME, PAGENAMEE, BASEPAGENAME, BASEPAGENAMEE templates
        /// </summary>
        public static readonly Regex BASEPAGENAMETemplates = Tools.NestedTemplateRegex(BASEPAGENAMETemplatesL);
        
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// From http://www.dreamincode.net/code/snippet3490.htm
        /// </remarks>
        public static readonly Regex UrlValidator =
            new Regex(
                @"^(https?|ftp)\://[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3}(:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~])*[^\.\,\)\(\s]$",
                RegexOptions.Compiled);
        
        /// <summary>
        /// Matches templates from [[Category:Hatnote templates for names]], excluding name order templates
        /// </summary>
        public static readonly Regex SurnameClarificationTemplates = Tools.NestedTemplateRegex(new [] {"Arabic name", "Basque name", "Cambodian name", "Catalan name", "Chinese Indonesian name", "Chinese name", "Dinka name", "Dutch name", "Eastern Slavic name", "Galician name", "Germanic name", "Habesha name", "Hmong name", "Icelandic name", "Indian name", "Indonesian name", "Japanese name", "Korean name", "Malay name", "Mongolian name", "Multi-word family name", "Philippine name", "Portuguese name", "Romance name", "Slavic name", "Spanish name", "Turkic name", "Vietnamese name", "Welsh name", "Western name order"});
    }
}
