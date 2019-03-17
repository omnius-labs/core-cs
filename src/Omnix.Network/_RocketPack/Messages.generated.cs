using Omnix.Base;
using Omnix.Base.Helpers;
using Omnix.Serialization;
using Omnix.Serialization.RocketPack;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Omnix.Network
{
    public sealed partial class OmniAddress : RocketPackMessageBase<OmniAddress>
    {
        static OmniAddress()
        {
            OmniAddress.Formatter = new CustomFormatter();
        }

        public static readonly int MaxValueLength = 8192;

        public OmniAddress(string value)
        {
            if (value is null) throw new ArgumentNullException("value");
            if (value.Length > 8192) throw new ArgumentOutOfRangeException("value");

            this.Value = value;

            {
                var hashCode = new HashCode();
                if (this.Value != default) hashCode.Add(this.Value.GetHashCode());
                _hashCode = hashCode.ToHashCode();
            }
        }

        public string Value { get; }

        public override bool Equals(OmniAddress target)
        {
            if ((object)target == null) return false;
            if (Object.ReferenceEquals(this, target)) return true;
            if (this.Value != target.Value) return false;

            return true;
        }

        private readonly int _hashCode;
        public override int GetHashCode() => _hashCode;

        private sealed class CustomFormatter : IRocketPackFormatter<OmniAddress>
        {
            public void Serialize(RocketPackWriter w, OmniAddress value, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Write property count
                {
                    uint propertyCount = 0;
                    if (value.Value != default) propertyCount++;
                    w.Write(propertyCount);
                }

                // Value
                if (value.Value != default)
                {
                    w.Write((uint)0);
                    w.Write(value.Value);
                }
            }

            public OmniAddress Deserialize(RocketPackReader r, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Read property count
                uint propertyCount = r.GetUInt32();

                string p_value = default;

                for (; propertyCount > 0; propertyCount--)
                {
                    uint id = r.GetUInt32();
                    switch (id)
                    {
                        case 0: // Value
                            {
                                p_value = r.GetString(8192);
                                break;
                            }
                    }
                }

                return new OmniAddress(p_value);
            }
        }
    }

}
