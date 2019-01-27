using System;
using System.Collections.Generic;
using System.Text;

namespace Omnix.Base
{
    // https://qiita.com/s-matsuba/items/1ee6bcc1bc5d721fc978
    public class GenericEqualityComparer<T> : IEqualityComparer<T>
    {
        private Func<T, T, bool> _predicate;
        private Func<T, int> _getHashCode;

        public GenericEqualityComparer(Func<T, T, bool> predicate)
            : this(predicate, obj => obj.GetHashCode())
        {
        }
        public GenericEqualityComparer(Func<T, T, bool> predicate, Func<T, int> getHashCode)
        {
            _predicate = predicate;
            _getHashCode = getHashCode;
        }

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
