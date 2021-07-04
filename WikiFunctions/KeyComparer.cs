using System.Collections.Generic;

namespace WikiFunctions
{
    /// <summary>
    /// Code from https://medium.com/wehkamp-techblog/sorting-a-dictionary-on-a-list-of-keys-68ecadb421a5
    /// </summary>
    public class KeyComparer<T> : IComparer<T>
    {
        private const int TOP = -1;
        private const int BOTTOM = 1;
        private const int EQUAL = 0;

        private readonly Dictionary<T, int> _keys = new Dictionary<T, int>();

        public KeyComparer(IEnumerable<T> keys)
        {
            if (keys != null)
                foreach (var key in keys)
                    _keys.Add(key, _keys.Count);
        }

        public virtual int Compare(T x, T y)
        {
            // x is our hero, does he need to be on
            // the top or the bottom of the list?
            int iX = -1, iY = -1;

            if (_keys.TryGetValue(x, out var aX))
                iX = aX;

            if (_keys.TryGetValue(y, out var aY))
                iY = aY;

            // x is not in the list of fields, so
            // x should go to the bottom
            if (iX == -1)
                return BOTTOM;

            // if x and y have the same position,
            // so they must be equal
            if (iX == iY)
                return EQUAL;

            // if y is not in the list, it means
            // x should go to the top. if the position
            // of x in the list is lower, that means
            // x should go to the top.
            if (iY == -1 || iX < iY)
                return TOP;

            // x should go to the bottom
            return BOTTOM;
        }
    }
}
