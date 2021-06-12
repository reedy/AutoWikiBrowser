using System.Collections.Generic;

namespace WikiFunctions
{
    /// <summary>
    /// 
    /// </summary>
    class UsernameComparer : IEqualityComparer<string>
    {
        public bool Equals(string x, string y)
        {
            return Tools.TurnFirstToUpperNoProjectCheck(x) == Tools.TurnFirstToUpperNoProjectCheck(y);
        }

        public int GetHashCode(string obj)
        {
            return Tools.TurnFirstToUpper(obj).GetHashCode();
        }
    }
}
