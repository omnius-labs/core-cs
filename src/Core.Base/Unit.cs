using System;
using System.Runtime.CompilerServices;

namespace Omnius.Core.Base
{
    public readonly struct Unit : IEquatable<Unit>
    {
        public static readonly Unit Value = default;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Unit other) => true;
        public override bool Equals(object? obj) => obj is Unit;
        public override int GetHashCode() => 0;
        public override string ToString() => "()";
        public static bool operator ==(Unit left, Unit right) => true;
        public static bool operator !=(Unit left, Unit right) => false;
    }
}
