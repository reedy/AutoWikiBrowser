using System.Text.RegularExpressions;

namespace Fronds
{
    /// <summary>
    /// 
    /// </summary>
    class Frond
    {
        private readonly Regex Find;
        private readonly string Replace;

        public Frond(Regex find, string replace)
        {
            Find = find;
            Replace = replace;
        }

        public Frond(string find, RegexOptions options, string replace)
            : this(new Regex(find, options | RegexOptions.Compiled), replace)
        {
        }

        public string Preform(string articleText)
        {
            return Find.Replace(articleText, Replace);
        }
    }
}
