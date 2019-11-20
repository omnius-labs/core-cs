using System;
using System.Collections.Concurrent;

namespace Omnius.Core.Helpers
{
    public static class EnumHelper
    {
        public static bool IsValid<T>(T value)
            where T : Enum
        {
            return EnumInfo<T>.IsValid(value);
        }

        private static class EnumInfo<T> 
            where T: Enum
        {
            private static readonly ConcurrentDictionary<T, bool> _resultMap = new ConcurrentDictionary<T, bool>();

            public static bool IsValid(T value)
            {
                return _resultMap.GetOrAdd(value, (_) => Enum.IsDefined(typeof(T), value));
            }
        }
    }
}