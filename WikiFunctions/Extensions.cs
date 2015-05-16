using System.Collections;
using System.Collections.Generic;

namespace WikiFunctions
{
    public static class Extensions
    {
        public static void AddIfTrue(this Dictionary<string, string> dict, bool input, string key, string value)
        {
            if (input)
            {
                dict.Add(key, value);
            }
        }

        public static bool IsIn<T>(this T @this, params T[] possibles)
        {
            return ((IList) possibles).Contains(@this);
        }
    }
}
