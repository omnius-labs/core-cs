﻿using Omnix.Base;
using Omnix.Base.Helpers;
using Omnix.Serialization;
using Omnix.Serialization.RocketPack;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Omnix.Remoting
{
    public sealed partial class OmniRpcErrorMessage : RocketPackMessageBase<OmniRpcErrorMessage>
    {
        static OmniRpcErrorMessage()
        {
            OmniRpcErrorMessage.Formatter = new CustomFormatter();
        }

        public static readonly int MaxTypeLength = 8192;
        public static readonly int MaxMessageLength = 8192;
        public static readonly int MaxStackTraceLength = 8192;

        public OmniRpcErrorMessage(string type, string message, string stackTrace)
        {
            if (type is null) throw new ArgumentNullException("type");
            if (type.Length > 8192) throw new ArgumentOutOfRangeException("type");
            if (message is null) throw new ArgumentNullException("message");
            if (message.Length > 8192) throw new ArgumentOutOfRangeException("message");
            if (stackTrace is null) throw new ArgumentNullException("stackTrace");
            if (stackTrace.Length > 8192) throw new ArgumentOutOfRangeException("stackTrace");

            this.Type = type;
            this.Message = message;
            this.StackTrace = stackTrace;

            {
                var hashCode = new HashCode();
                if (this.Type != default) hashCode.Add(this.Type.GetHashCode());
                if (this.Message != default) hashCode.Add(this.Message.GetHashCode());
                if (this.StackTrace != default) hashCode.Add(this.StackTrace.GetHashCode());
                _hashCode = hashCode.ToHashCode();
            }
        }

        public string Type { get; }
        public string Message { get; }
        public string StackTrace { get; }

        public override bool Equals(OmniRpcErrorMessage target)
        {
            if ((object)target == null) return false;
            if (Object.ReferenceEquals(this, target)) return true;
            if (this.Type != target.Type) return false;
            if (this.Message != target.Message) return false;
            if (this.StackTrace != target.StackTrace) return false;

            return true;
        }

        private readonly int _hashCode;
        public override int GetHashCode() => _hashCode;

        private sealed class CustomFormatter : IRocketPackFormatter<OmniRpcErrorMessage>
        {
            public void Serialize(RocketPackWriter w, OmniRpcErrorMessage value, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Write property count
                {
                    uint propertyCount = 0;
                    if (value.Type != default) propertyCount++;
                    if (value.Message != default) propertyCount++;
                    if (value.StackTrace != default) propertyCount++;
                    w.Write(propertyCount);
                }

                // Type
                if (value.Type != default)
                {
                    w.Write((uint)0);
                    w.Write(value.Type);
                }
                // Message
                if (value.Message != default)
                {
                    w.Write((uint)1);
                    w.Write(value.Message);
                }
                // StackTrace
                if (value.StackTrace != default)
                {
                    w.Write((uint)2);
                    w.Write(value.StackTrace);
                }
            }

            public OmniRpcErrorMessage Deserialize(RocketPackReader r, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Read property count
                uint propertyCount = r.GetUInt32();

                string p_type = default;
                string p_message = default;
                string p_stackTrace = default;

                for (; propertyCount > 0; propertyCount--)
                {
                    uint id = r.GetUInt32();
                    switch (id)
                    {
                        case 0: // Type
                            {
                                p_type = r.GetString(8192);
                                break;
                            }
                        case 1: // Message
                            {
                                p_message = r.GetString(8192);
                                break;
                            }
                        case 2: // StackTrace
                            {
                                p_stackTrace = r.GetString(8192);
                                break;
                            }
                    }
                }

                return new OmniRpcErrorMessage(p_type, p_message, p_stackTrace);
            }
        }
    }

}
