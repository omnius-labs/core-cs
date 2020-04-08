using System;
using System.Collections.Generic;

namespace Omnius.Core
{
    // https://qiita.com/s-matsuba/items/1ee6bcc1bc5d721fc978
    public class GenericEqualityComparer<T> : IEqualityComparer<T>
    {
        private readonly Func<T, T, bool> _predicate;
        private readonly Func<T, int> _getHashCode;

        public GenericEqualityComparer(Func<T, T, bool> predicate)
            : this(predicate, obj => obj?.GetHashCode() ?? 0)
        {
        }

        public GenericEqualityComparer(Func<T, T, bool> predicate, Func<T, int> getHashCode)
        {
            _predicate = predicate;
            _getHashCode = getHashCode;
        }

        // nullが入る可能性あり。
        public bool Equals(T x, T y)
        {
            return _predicate(x, y);
        }

        public int GetHashCode(T obj)
        {
            return _getHashCode(obj);
        }
    }
}
