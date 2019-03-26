using Omnix.Base;
using Omnix.Base.Helpers;
using Omnix.Collections;
using Omnix.Serialization;
using Omnix.Serialization.RocketPack;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Omnix.Network.Connection.Multiplexer.V1.Internal
{
    internal enum SessionErrorType : byte
    {
        ConnectFailed = 0,
        MemoryOverflow = 1,
        NotFoundSessionId = 2,
    }

    internal sealed partial class ProfileMessage : RocketPackMessageBase<ProfileMessage>
    {
        static ProfileMessage()
        {
            ProfileMessage.Formatter = new CustomFormatter();
        }

        public ProfileMessage(ulong initialWindowSize, uint maxSessionAcceptQueueSize)
        {
            this.InitialWindowSize = initialWindowSize;
            this.MaxSessionAcceptQueueSize = maxSessionAcceptQueueSize;

            {
                var hashCode = new HashCode();
                if (this.InitialWindowSize != default) hashCode.Add(this.InitialWindowSize.GetHashCode());
                if (this.MaxSessionAcceptQueueSize != default) hashCode.Add(this.MaxSessionAcceptQueueSize.GetHashCode());
                _hashCode = hashCode.ToHashCode();
            }
        }

        public ulong InitialWindowSize { get; }
        public uint MaxSessionAcceptQueueSize { get; }

        public override bool Equals(ProfileMessage target)
        {
            if ((object)target == null) return false;
            if (Object.ReferenceEquals(this, target)) return true;
            if (this.InitialWindowSize != target.InitialWindowSize) return false;
            if (this.MaxSessionAcceptQueueSize != target.MaxSessionAcceptQueueSize) return false;

            return true;
        }

        private readonly int _hashCode;
        public override int GetHashCode() => _hashCode;

        private sealed class CustomFormatter : IRocketPackFormatter<ProfileMessage>
        {
            public void Serialize(RocketPackWriter w, ProfileMessage value, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Write property count
                {
                    uint propertyCount = 0;
                    if (value.InitialWindowSize != default) propertyCount++;
                    if (value.MaxSessionAcceptQueueSize != default) propertyCount++;
                    w.Write(propertyCount);
                }

                // InitialWindowSize
                if (value.InitialWindowSize != default)
                {
                    w.Write((uint)0);
                    w.Write(value.InitialWindowSize);
                }
                // MaxSessionAcceptQueueSize
                if (value.MaxSessionAcceptQueueSize != default)
                {
                    w.Write((uint)1);
                    w.Write(value.MaxSessionAcceptQueueSize);
                }
            }

            public ProfileMessage Deserialize(RocketPackReader r, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Read property count
                uint propertyCount = r.GetUInt32();

                ulong p_initialWindowSize = default;
                uint p_maxSessionAcceptQueueSize = default;

                for (; propertyCount > 0; propertyCount--)
                {
                    uint id = r.GetUInt32();
                    switch (id)
                    {
                        case 0: // InitialWindowSize
                            {
                                p_initialWindowSize = r.GetUInt64();
                                break;
                            }
                        case 1: // MaxSessionAcceptQueueSize
                            {
                                p_maxSessionAcceptQueueSize = r.GetUInt32();
                                break;
                            }
                    }
                }

                return new ProfileMessage(p_initialWindowSize, p_maxSessionAcceptQueueSize);
            }
        }
    }

    internal sealed partial class SessionConnectMessage : RocketPackMessageBase<SessionConnectMessage>
    {
        static SessionConnectMessage()
        {
            SessionConnectMessage.Formatter = new CustomFormatter();
        }

        public SessionConnectMessage(ulong sessionId)
        {
            this.SessionId = sessionId;

            {
                var hashCode = new HashCode();
                if (this.SessionId != default) hashCode.Add(this.SessionId.GetHashCode());
                _hashCode = hashCode.ToHashCode();
            }
        }

        public ulong SessionId { get; }

        public override bool Equals(SessionConnectMessage target)
        {
            if ((object)target == null) return false;
            if (Object.ReferenceEquals(this, target)) return true;
            if (this.SessionId != target.SessionId) return false;

            return true;
        }

        private readonly int _hashCode;
        public override int GetHashCode() => _hashCode;

        private sealed class CustomFormatter : IRocketPackFormatter<SessionConnectMessage>
        {
            public void Serialize(RocketPackWriter w, SessionConnectMessage value, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Write property count
                {
                    uint propertyCount = 0;
                    if (value.SessionId != default) propertyCount++;
                    w.Write(propertyCount);
                }

                // SessionId
                if (value.SessionId != default)
                {
                    w.Write((uint)0);
                    w.Write(value.SessionId);
                }
            }

            public SessionConnectMessage Deserialize(RocketPackReader r, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Read property count
                uint propertyCount = r.GetUInt32();

                ulong p_sessionId = default;

                for (; propertyCount > 0; propertyCount--)
                {
                    uint id = r.GetUInt32();
                    switch (id)
                    {
                        case 0: // SessionId
                            {
                                p_sessionId = r.GetUInt64();
                                break;
                            }
                    }
                }

                return new SessionConnectMessage(p_sessionId);
            }
        }
    }

    internal sealed partial class SessionAcceptMessage : RocketPackMessageBase<SessionAcceptMessage>
    {
        static SessionAcceptMessage()
        {
            SessionAcceptMessage.Formatter = new CustomFormatter();
        }

        public SessionAcceptMessage(ulong sessionId)
        {
            this.SessionId = sessionId;

            {
                var hashCode = new HashCode();
                if (this.SessionId != default) hashCode.Add(this.SessionId.GetHashCode());
                _hashCode = hashCode.ToHashCode();
            }
        }

        public ulong SessionId { get; }

        public override bool Equals(SessionAcceptMessage target)
        {
            if ((object)target == null) return false;
            if (Object.ReferenceEquals(this, target)) return true;
            if (this.SessionId != target.SessionId) return false;

            return true;
        }

        private readonly int _hashCode;
        public override int GetHashCode() => _hashCode;

        private sealed class CustomFormatter : IRocketPackFormatter<SessionAcceptMessage>
        {
            public void Serialize(RocketPackWriter w, SessionAcceptMessage value, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Write property count
                {
                    uint propertyCount = 0;
                    if (value.SessionId != default) propertyCount++;
                    w.Write(propertyCount);
                }

                // SessionId
                if (value.SessionId != default)
                {
                    w.Write((uint)0);
                    w.Write(value.SessionId);
                }
            }

            public SessionAcceptMessage Deserialize(RocketPackReader r, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Read property count
                uint propertyCount = r.GetUInt32();

                ulong p_sessionId = default;

                for (; propertyCount > 0; propertyCount--)
                {
                    uint id = r.GetUInt32();
                    switch (id)
                    {
                        case 0: // SessionId
                            {
                                p_sessionId = r.GetUInt64();
                                break;
                            }
                    }
                }

                return new SessionAcceptMessage(p_sessionId);
            }
        }
    }

    internal sealed partial class SessionUpdateWindowSizeMessage : RocketPackMessageBase<SessionUpdateWindowSizeMessage>
    {
        static SessionUpdateWindowSizeMessage()
        {
            SessionUpdateWindowSizeMessage.Formatter = new CustomFormatter();
        }

        public SessionUpdateWindowSizeMessage(ulong sessionId, ulong size)
        {
            this.SessionId = sessionId;
            this.Size = size;

            {
                var hashCode = new HashCode();
                if (this.SessionId != default) hashCode.Add(this.SessionId.GetHashCode());
                if (this.Size != default) hashCode.Add(this.Size.GetHashCode());
                _hashCode = hashCode.ToHashCode();
            }
        }

        public ulong SessionId { get; }
        public ulong Size { get; }

        public override bool Equals(SessionUpdateWindowSizeMessage target)
        {
            if ((object)target == null) return false;
            if (Object.ReferenceEquals(this, target)) return true;
            if (this.SessionId != target.SessionId) return false;
            if (this.Size != target.Size) return false;

            return true;
        }

        private readonly int _hashCode;
        public override int GetHashCode() => _hashCode;

        private sealed class CustomFormatter : IRocketPackFormatter<SessionUpdateWindowSizeMessage>
        {
            public void Serialize(RocketPackWriter w, SessionUpdateWindowSizeMessage value, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Write property count
                {
                    uint propertyCount = 0;
                    if (value.SessionId != default) propertyCount++;
                    if (value.Size != default) propertyCount++;
                    w.Write(propertyCount);
                }

                // SessionId
                if (value.SessionId != default)
                {
                    w.Write((uint)0);
                    w.Write(value.SessionId);
                }
                // Size
                if (value.Size != default)
                {
                    w.Write((uint)1);
                    w.Write(value.Size);
                }
            }

            public SessionUpdateWindowSizeMessage Deserialize(RocketPackReader r, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Read property count
                uint propertyCount = r.GetUInt32();

                ulong p_sessionId = default;
                ulong p_size = default;

                for (; propertyCount > 0; propertyCount--)
                {
                    uint id = r.GetUInt32();
                    switch (id)
                    {
                        case 0: // SessionId
                            {
                                p_sessionId = r.GetUInt64();
                                break;
                            }
                        case 1: // Size
                            {
                                p_size = r.GetUInt64();
                                break;
                            }
                    }
                }

                return new SessionUpdateWindowSizeMessage(p_sessionId, p_size);
            }
        }
    }

    internal sealed partial class SessionDataMessage : RocketPackMessageBase<SessionDataMessage>, IDisposable
    {
        static SessionDataMessage()
        {
            SessionDataMessage.Formatter = new CustomFormatter();
        }

        public static readonly int MaxDataLength = 262144;

        public SessionDataMessage(ulong sessionId, bool isCompleted, IMemoryOwner<byte> data)
        {
            if (data is null) throw new ArgumentNullException("data");
            if (data.Memory.Length > 262144) throw new ArgumentOutOfRangeException("data");

            this.SessionId = sessionId;
            this.IsCompleted = isCompleted;
            _data = data;

            {
                var hashCode = new HashCode();
                if (this.SessionId != default) hashCode.Add(this.SessionId.GetHashCode());
                if (this.IsCompleted != default) hashCode.Add(this.IsCompleted.GetHashCode());
                if (!this.Data.IsEmpty) hashCode.Add(ObjectHelper.GetHashCode(this.Data.Span));
                _hashCode = hashCode.ToHashCode();
            }
        }

        public ulong SessionId { get; }
        public bool IsCompleted { get; }
        private readonly IMemoryOwner<byte> _data;
        public ReadOnlyMemory<byte> Data => _data.Memory;

        public override bool Equals(SessionDataMessage target)
        {
            if ((object)target == null) return false;
            if (Object.ReferenceEquals(this, target)) return true;
            if (this.SessionId != target.SessionId) return false;
            if (this.IsCompleted != target.IsCompleted) return false;
            if (!BytesOperations.SequenceEqual(this.Data.Span, target.Data.Span)) return false;

            return true;
        }

        private readonly int _hashCode;
        public override int GetHashCode() => _hashCode;

        public void Dispose()
        {
            _data?.Dispose();
        }

        private sealed class CustomFormatter : IRocketPackFormatter<SessionDataMessage>
        {
            public void Serialize(RocketPackWriter w, SessionDataMessage value, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Write property count
                {
                    uint propertyCount = 0;
                    if (value.SessionId != default) propertyCount++;
                    if (value.IsCompleted != default) propertyCount++;
                    if (!value.Data.IsEmpty) propertyCount++;
                    w.Write(propertyCount);
                }

                // SessionId
                if (value.SessionId != default)
                {
                    w.Write((uint)0);
                    w.Write(value.SessionId);
                }
                // IsCompleted
                if (value.IsCompleted != default)
                {
                    w.Write((uint)1);
                    w.Write(value.IsCompleted);
                }
                // Data
                if (!value.Data.IsEmpty)
                {
                    w.Write((uint)2);
                    w.Write(value.Data.Span);
                }
            }

            public SessionDataMessage Deserialize(RocketPackReader r, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Read property count
                uint propertyCount = r.GetUInt32();

                ulong p_sessionId = default;
                bool p_isCompleted = default;
                IMemoryOwner<byte> p_data = default;

                for (; propertyCount > 0; propertyCount--)
                {
                    uint id = r.GetUInt32();
                    switch (id)
                    {
                        case 0: // SessionId
                            {
                                p_sessionId = r.GetUInt64();
                                break;
                            }
                        case 1: // IsCompleted
                            {
                                p_isCompleted = r.GetBoolean();
                                break;
                            }
                        case 2: // Data
                            {
                                p_data = r.GetRecyclableMemory(262144);
                                break;
                            }
                    }
                }

                return new SessionDataMessage(p_sessionId, p_isCompleted, p_data);
            }
        }
    }

    internal sealed partial class SessionCloseMessage : RocketPackMessageBase<SessionCloseMessage>
    {
        static SessionCloseMessage()
        {
            SessionCloseMessage.Formatter = new CustomFormatter();
        }

        public SessionCloseMessage(ulong sessionId)
        {
            this.SessionId = sessionId;

            {
                var hashCode = new HashCode();
                if (this.SessionId != default) hashCode.Add(this.SessionId.GetHashCode());
                _hashCode = hashCode.ToHashCode();
            }
        }

        public ulong SessionId { get; }

        public override bool Equals(SessionCloseMessage target)
        {
            if ((object)target == null) return false;
            if (Object.ReferenceEquals(this, target)) return true;
            if (this.SessionId != target.SessionId) return false;

            return true;
        }

        private readonly int _hashCode;
        public override int GetHashCode() => _hashCode;

        private sealed class CustomFormatter : IRocketPackFormatter<SessionCloseMessage>
        {
            public void Serialize(RocketPackWriter w, SessionCloseMessage value, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Write property count
                {
                    uint propertyCount = 0;
                    if (value.SessionId != default) propertyCount++;
                    w.Write(propertyCount);
                }

                // SessionId
                if (value.SessionId != default)
                {
                    w.Write((uint)0);
                    w.Write(value.SessionId);
                }
            }

            public SessionCloseMessage Deserialize(RocketPackReader r, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Read property count
                uint propertyCount = r.GetUInt32();

                ulong p_sessionId = default;

                for (; propertyCount > 0; propertyCount--)
                {
                    uint id = r.GetUInt32();
                    switch (id)
                    {
                        case 0: // SessionId
                            {
                                p_sessionId = r.GetUInt64();
                                break;
                            }
                    }
                }

                return new SessionCloseMessage(p_sessionId);
            }
        }
    }

    internal sealed partial class SessionErrorMessage : RocketPackMessageBase<SessionErrorMessage>
    {
        static SessionErrorMessage()
        {
            SessionErrorMessage.Formatter = new CustomFormatter();
        }

        public SessionErrorMessage(ulong sessionId, SessionErrorType errorType)
        {
            this.SessionId = sessionId;
            this.ErrorType = errorType;

            {
                var hashCode = new HashCode();
                if (this.SessionId != default) hashCode.Add(this.SessionId.GetHashCode());
                if (this.ErrorType != default) hashCode.Add(this.ErrorType.GetHashCode());
                _hashCode = hashCode.ToHashCode();
            }
        }

        public ulong SessionId { get; }
        public SessionErrorType ErrorType { get; }

        public override bool Equals(SessionErrorMessage target)
        {
            if ((object)target == null) return false;
            if (Object.ReferenceEquals(this, target)) return true;
            if (this.SessionId != target.SessionId) return false;
            if (this.ErrorType != target.ErrorType) return false;

            return true;
        }

        private readonly int _hashCode;
        public override int GetHashCode() => _hashCode;

        private sealed class CustomFormatter : IRocketPackFormatter<SessionErrorMessage>
        {
            public void Serialize(RocketPackWriter w, SessionErrorMessage value, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Write property count
                {
                    uint propertyCount = 0;
                    if (value.SessionId != default) propertyCount++;
                    if (value.ErrorType != default) propertyCount++;
                    w.Write(propertyCount);
                }

                // SessionId
                if (value.SessionId != default)
                {
                    w.Write((uint)0);
                    w.Write(value.SessionId);
                }
                // ErrorType
                if (value.ErrorType != default)
                {
                    w.Write((uint)1);
                    w.Write((ulong)value.ErrorType);
                }
            }

            public SessionErrorMessage Deserialize(RocketPackReader r, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Read property count
                uint propertyCount = r.GetUInt32();

                ulong p_sessionId = default;
                SessionErrorType p_errorType = default;

                for (; propertyCount > 0; propertyCount--)
                {
                    uint id = r.GetUInt32();
                    switch (id)
                    {
                        case 0: // SessionId
                            {
                                p_sessionId = r.GetUInt64();
                                break;
                            }
                        case 1: // ErrorType
                            {
                                p_errorType = (SessionErrorType)r.GetUInt64();
                                break;
                            }
                    }
                }

                return new SessionErrorMessage(p_sessionId, p_errorType);
            }
        }
    }

}
