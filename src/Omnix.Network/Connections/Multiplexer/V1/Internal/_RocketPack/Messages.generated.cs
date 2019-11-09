
#nullable enable

namespace Omnix.Network.Connections.Multiplexer.V1.Internal
{
    internal enum SessionErrorType : byte
    {
        ConnectFailed = 0,
        MemoryOverflow = 1,
        NotFoundSessionId = 2,
    }

    internal sealed partial class ProfileMessage : global::Omnix.Serialization.RocketPack.IRocketPackMessage<ProfileMessage>
    {
        public static global::Omnix.Serialization.RocketPack.IRocketPackFormatter<ProfileMessage> Formatter { get; }
        public static ProfileMessage Empty { get; }

        static ProfileMessage()
        {
            ProfileMessage.Formatter = new ___CustomFormatter();
            ProfileMessage.Empty = new ProfileMessage(0, 0);
        }

        private readonly global::System.Lazy<int> ___hashCode;

        public ProfileMessage(ulong initialWindowSize, uint maxSessionAcceptQueueSize)
        {
            this.InitialWindowSize = initialWindowSize;
            this.MaxSessionAcceptQueueSize = maxSessionAcceptQueueSize;

            ___hashCode = new global::System.Lazy<int>(() =>
            {
                var ___h = new global::System.HashCode();
                if (initialWindowSize != default) ___h.Add(initialWindowSize.GetHashCode());
                if (maxSessionAcceptQueueSize != default) ___h.Add(maxSessionAcceptQueueSize.GetHashCode());
                return ___h.ToHashCode();
            });
        }

        public ulong InitialWindowSize { get; }
        public uint MaxSessionAcceptQueueSize { get; }

        public static ProfileMessage Import(global::System.Buffers.ReadOnlySequence<byte> sequence, global::Omnix.Base.IBufferPool<byte> bufferPool)
        {
            var reader = new global::Omnix.Serialization.RocketPack.RocketPackReader(sequence, bufferPool);
            return Formatter.Deserialize(ref reader, 0);
        }
        public void Export(global::System.Buffers.IBufferWriter<byte> bufferWriter, global::Omnix.Base.IBufferPool<byte> bufferPool)
        {
            var writer = new global::Omnix.Serialization.RocketPack.RocketPackWriter(bufferWriter, bufferPool);
            Formatter.Serialize(ref writer, this, 0);
        }

        public static bool operator ==(ProfileMessage? left, ProfileMessage? right)
        {
            return (right is null) ? (left is null) : right.Equals(left);
        }
        public static bool operator !=(ProfileMessage? left, ProfileMessage? right)
        {
            return !(left == right);
        }
        public override bool Equals(object? other)
        {
            if (!(other is ProfileMessage)) return false;
            return this.Equals((ProfileMessage)other);
        }
        public bool Equals(ProfileMessage? target)
        {
            if (target is null) return false;
            if (object.ReferenceEquals(this, target)) return true;
            if (this.InitialWindowSize != target.InitialWindowSize) return false;
            if (this.MaxSessionAcceptQueueSize != target.MaxSessionAcceptQueueSize) return false;

            return true;
        }
        public override int GetHashCode() => ___hashCode.Value;

        private sealed class ___CustomFormatter : global::Omnix.Serialization.RocketPack.IRocketPackFormatter<ProfileMessage>
        {
            public void Serialize(ref global::Omnix.Serialization.RocketPack.RocketPackWriter w, in ProfileMessage value, in int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                {
                    uint propertyCount = 0;
                    if (value.InitialWindowSize != 0)
                    {
                        propertyCount++;
                    }
                    if (value.MaxSessionAcceptQueueSize != 0)
                    {
                        propertyCount++;
                    }
                    w.Write(propertyCount);
                }

                if (value.InitialWindowSize != 0)
                {
                    w.Write((uint)0);
                    w.Write(value.InitialWindowSize);
                }
                if (value.MaxSessionAcceptQueueSize != 0)
                {
                    w.Write((uint)1);
                    w.Write(value.MaxSessionAcceptQueueSize);
                }
            }

            public ProfileMessage Deserialize(ref global::Omnix.Serialization.RocketPack.RocketPackReader r, in int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                uint propertyCount = r.GetUInt32();

                ulong p_initialWindowSize = 0;
                uint p_maxSessionAcceptQueueSize = 0;

                for (; propertyCount > 0; propertyCount--)
                {
                    uint id = r.GetUInt32();
                    switch (id)
                    {
                        case 0:
                            {
                                p_initialWindowSize = r.GetUInt64();
                                break;
                            }
                        case 1:
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

    internal sealed partial class SessionConnectMessage : global::Omnix.Serialization.RocketPack.IRocketPackMessage<SessionConnectMessage>
    {
        public static global::Omnix.Serialization.RocketPack.IRocketPackFormatter<SessionConnectMessage> Formatter { get; }
        public static SessionConnectMessage Empty { get; }

        static SessionConnectMessage()
        {
            SessionConnectMessage.Formatter = new ___CustomFormatter();
            SessionConnectMessage.Empty = new SessionConnectMessage(0);
        }

        private readonly global::System.Lazy<int> ___hashCode;

        public SessionConnectMessage(ulong sessionId)
        {
            this.SessionId = sessionId;

            ___hashCode = new global::System.Lazy<int>(() =>
            {
                var ___h = new global::System.HashCode();
                if (sessionId != default) ___h.Add(sessionId.GetHashCode());
                return ___h.ToHashCode();
            });
        }

        public ulong SessionId { get; }

        public static SessionConnectMessage Import(global::System.Buffers.ReadOnlySequence<byte> sequence, global::Omnix.Base.IBufferPool<byte> bufferPool)
        {
            var reader = new global::Omnix.Serialization.RocketPack.RocketPackReader(sequence, bufferPool);
            return Formatter.Deserialize(ref reader, 0);
        }
        public void Export(global::System.Buffers.IBufferWriter<byte> bufferWriter, global::Omnix.Base.IBufferPool<byte> bufferPool)
        {
            var writer = new global::Omnix.Serialization.RocketPack.RocketPackWriter(bufferWriter, bufferPool);
            Formatter.Serialize(ref writer, this, 0);
        }

        public static bool operator ==(SessionConnectMessage? left, SessionConnectMessage? right)
        {
            return (right is null) ? (left is null) : right.Equals(left);
        }
        public static bool operator !=(SessionConnectMessage? left, SessionConnectMessage? right)
        {
            return !(left == right);
        }
        public override bool Equals(object? other)
        {
            if (!(other is SessionConnectMessage)) return false;
            return this.Equals((SessionConnectMessage)other);
        }
        public bool Equals(SessionConnectMessage? target)
        {
            if (target is null) return false;
            if (object.ReferenceEquals(this, target)) return true;
            if (this.SessionId != target.SessionId) return false;

            return true;
        }
        public override int GetHashCode() => ___hashCode.Value;

        private sealed class ___CustomFormatter : global::Omnix.Serialization.RocketPack.IRocketPackFormatter<SessionConnectMessage>
        {
            public void Serialize(ref global::Omnix.Serialization.RocketPack.RocketPackWriter w, in SessionConnectMessage value, in int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                {
                    uint propertyCount = 0;
                    if (value.SessionId != 0)
                    {
                        propertyCount++;
                    }
                    w.Write(propertyCount);
                }

                if (value.SessionId != 0)
                {
                    w.Write((uint)0);
                    w.Write(value.SessionId);
                }
            }

            public SessionConnectMessage Deserialize(ref global::Omnix.Serialization.RocketPack.RocketPackReader r, in int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                uint propertyCount = r.GetUInt32();

                ulong p_sessionId = 0;

                for (; propertyCount > 0; propertyCount--)
                {
                    uint id = r.GetUInt32();
                    switch (id)
                    {
                        case 0:
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

    internal sealed partial class SessionAcceptMessage : global::Omnix.Serialization.RocketPack.IRocketPackMessage<SessionAcceptMessage>
    {
        public static global::Omnix.Serialization.RocketPack.IRocketPackFormatter<SessionAcceptMessage> Formatter { get; }
        public static SessionAcceptMessage Empty { get; }

        static SessionAcceptMessage()
        {
            SessionAcceptMessage.Formatter = new ___CustomFormatter();
            SessionAcceptMessage.Empty = new SessionAcceptMessage(0);
        }

        private readonly global::System.Lazy<int> ___hashCode;

        public SessionAcceptMessage(ulong sessionId)
        {
            this.SessionId = sessionId;

            ___hashCode = new global::System.Lazy<int>(() =>
            {
                var ___h = new global::System.HashCode();
                if (sessionId != default) ___h.Add(sessionId.GetHashCode());
                return ___h.ToHashCode();
            });
        }

        public ulong SessionId { get; }

        public static SessionAcceptMessage Import(global::System.Buffers.ReadOnlySequence<byte> sequence, global::Omnix.Base.IBufferPool<byte> bufferPool)
        {
            var reader = new global::Omnix.Serialization.RocketPack.RocketPackReader(sequence, bufferPool);
            return Formatter.Deserialize(ref reader, 0);
        }
        public void Export(global::System.Buffers.IBufferWriter<byte> bufferWriter, global::Omnix.Base.IBufferPool<byte> bufferPool)
        {
            var writer = new global::Omnix.Serialization.RocketPack.RocketPackWriter(bufferWriter, bufferPool);
            Formatter.Serialize(ref writer, this, 0);
        }

        public static bool operator ==(SessionAcceptMessage? left, SessionAcceptMessage? right)
        {
            return (right is null) ? (left is null) : right.Equals(left);
        }
        public static bool operator !=(SessionAcceptMessage? left, SessionAcceptMessage? right)
        {
            return !(left == right);
        }
        public override bool Equals(object? other)
        {
            if (!(other is SessionAcceptMessage)) return false;
            return this.Equals((SessionAcceptMessage)other);
        }
        public bool Equals(SessionAcceptMessage? target)
        {
            if (target is null) return false;
            if (object.ReferenceEquals(this, target)) return true;
            if (this.SessionId != target.SessionId) return false;

            return true;
        }
        public override int GetHashCode() => ___hashCode.Value;

        private sealed class ___CustomFormatter : global::Omnix.Serialization.RocketPack.IRocketPackFormatter<SessionAcceptMessage>
        {
            public void Serialize(ref global::Omnix.Serialization.RocketPack.RocketPackWriter w, in SessionAcceptMessage value, in int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                {
                    uint propertyCount = 0;
                    if (value.SessionId != 0)
                    {
                        propertyCount++;
                    }
                    w.Write(propertyCount);
                }

                if (value.SessionId != 0)
                {
                    w.Write((uint)0);
                    w.Write(value.SessionId);
                }
            }

            public SessionAcceptMessage Deserialize(ref global::Omnix.Serialization.RocketPack.RocketPackReader r, in int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                uint propertyCount = r.GetUInt32();

                ulong p_sessionId = 0;

                for (; propertyCount > 0; propertyCount--)
                {
                    uint id = r.GetUInt32();
                    switch (id)
                    {
                        case 0:
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

    internal sealed partial class SessionUpdateWindowSizeMessage : global::Omnix.Serialization.RocketPack.IRocketPackMessage<SessionUpdateWindowSizeMessage>
    {
        public static global::Omnix.Serialization.RocketPack.IRocketPackFormatter<SessionUpdateWindowSizeMessage> Formatter { get; }
        public static SessionUpdateWindowSizeMessage Empty { get; }

        static SessionUpdateWindowSizeMessage()
        {
            SessionUpdateWindowSizeMessage.Formatter = new ___CustomFormatter();
            SessionUpdateWindowSizeMessage.Empty = new SessionUpdateWindowSizeMessage(0, 0);
        }

        private readonly global::System.Lazy<int> ___hashCode;

        public SessionUpdateWindowSizeMessage(ulong sessionId, ulong size)
        {
            this.SessionId = sessionId;
            this.Size = size;

            ___hashCode = new global::System.Lazy<int>(() =>
            {
                var ___h = new global::System.HashCode();
                if (sessionId != default) ___h.Add(sessionId.GetHashCode());
                if (size != default) ___h.Add(size.GetHashCode());
                return ___h.ToHashCode();
            });
        }

        public ulong SessionId { get; }
        public ulong Size { get; }

        public static SessionUpdateWindowSizeMessage Import(global::System.Buffers.ReadOnlySequence<byte> sequence, global::Omnix.Base.IBufferPool<byte> bufferPool)
        {
            var reader = new global::Omnix.Serialization.RocketPack.RocketPackReader(sequence, bufferPool);
            return Formatter.Deserialize(ref reader, 0);
        }
        public void Export(global::System.Buffers.IBufferWriter<byte> bufferWriter, global::Omnix.Base.IBufferPool<byte> bufferPool)
        {
            var writer = new global::Omnix.Serialization.RocketPack.RocketPackWriter(bufferWriter, bufferPool);
            Formatter.Serialize(ref writer, this, 0);
        }

        public static bool operator ==(SessionUpdateWindowSizeMessage? left, SessionUpdateWindowSizeMessage? right)
        {
            return (right is null) ? (left is null) : right.Equals(left);
        }
        public static bool operator !=(SessionUpdateWindowSizeMessage? left, SessionUpdateWindowSizeMessage? right)
        {
            return !(left == right);
        }
        public override bool Equals(object? other)
        {
            if (!(other is SessionUpdateWindowSizeMessage)) return false;
            return this.Equals((SessionUpdateWindowSizeMessage)other);
        }
        public bool Equals(SessionUpdateWindowSizeMessage? target)
        {
            if (target is null) return false;
            if (object.ReferenceEquals(this, target)) return true;
            if (this.SessionId != target.SessionId) return false;
            if (this.Size != target.Size) return false;

            return true;
        }
        public override int GetHashCode() => ___hashCode.Value;

        private sealed class ___CustomFormatter : global::Omnix.Serialization.RocketPack.IRocketPackFormatter<SessionUpdateWindowSizeMessage>
        {
            public void Serialize(ref global::Omnix.Serialization.RocketPack.RocketPackWriter w, in SessionUpdateWindowSizeMessage value, in int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                {
                    uint propertyCount = 0;
                    if (value.SessionId != 0)
                    {
                        propertyCount++;
                    }
                    if (value.Size != 0)
                    {
                        propertyCount++;
                    }
                    w.Write(propertyCount);
                }

                if (value.SessionId != 0)
                {
                    w.Write((uint)0);
                    w.Write(value.SessionId);
                }
                if (value.Size != 0)
                {
                    w.Write((uint)1);
                    w.Write(value.Size);
                }
            }

            public SessionUpdateWindowSizeMessage Deserialize(ref global::Omnix.Serialization.RocketPack.RocketPackReader r, in int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                uint propertyCount = r.GetUInt32();

                ulong p_sessionId = 0;
                ulong p_size = 0;

                for (; propertyCount > 0; propertyCount--)
                {
                    uint id = r.GetUInt32();
                    switch (id)
                    {
                        case 0:
                            {
                                p_sessionId = r.GetUInt64();
                                break;
                            }
                        case 1:
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

    internal sealed partial class SessionDataMessage : global::Omnix.Serialization.RocketPack.IRocketPackMessage<SessionDataMessage>, global::System.IDisposable
    {
        public static global::Omnix.Serialization.RocketPack.IRocketPackFormatter<SessionDataMessage> Formatter { get; }
        public static SessionDataMessage Empty { get; }

        static SessionDataMessage()
        {
            SessionDataMessage.Formatter = new ___CustomFormatter();
            SessionDataMessage.Empty = new SessionDataMessage(0, false, global::Omnix.Base.SimpleMemoryOwner<byte>.Empty);
        }

        private readonly global::System.Lazy<int> ___hashCode;

        public static readonly int MaxDataLength = 262144;

        public SessionDataMessage(ulong sessionId, bool isCompleted, global::System.Buffers.IMemoryOwner<byte> data)
        {
            if (data is null) throw new global::System.ArgumentNullException("data");
            if (data.Memory.Length > 262144) throw new global::System.ArgumentOutOfRangeException("data");

            this.SessionId = sessionId;
            this.IsCompleted = isCompleted;
            _data = data;

            ___hashCode = new global::System.Lazy<int>(() =>
            {
                var ___h = new global::System.HashCode();
                if (sessionId != default) ___h.Add(sessionId.GetHashCode());
                if (isCompleted != default) ___h.Add(isCompleted.GetHashCode());
                if (!data.Memory.IsEmpty) ___h.Add(global::Omnix.Base.Helpers.ObjectHelper.GetHashCode(data.Memory.Span));
                return ___h.ToHashCode();
            });
        }

        public ulong SessionId { get; }
        public bool IsCompleted { get; }
        private readonly global::System.Buffers.IMemoryOwner<byte> _data;
        public global::System.ReadOnlyMemory<byte> Data => _data.Memory;

        public static SessionDataMessage Import(global::System.Buffers.ReadOnlySequence<byte> sequence, global::Omnix.Base.IBufferPool<byte> bufferPool)
        {
            var reader = new global::Omnix.Serialization.RocketPack.RocketPackReader(sequence, bufferPool);
            return Formatter.Deserialize(ref reader, 0);
        }
        public void Export(global::System.Buffers.IBufferWriter<byte> bufferWriter, global::Omnix.Base.IBufferPool<byte> bufferPool)
        {
            var writer = new global::Omnix.Serialization.RocketPack.RocketPackWriter(bufferWriter, bufferPool);
            Formatter.Serialize(ref writer, this, 0);
        }

        public static bool operator ==(SessionDataMessage? left, SessionDataMessage? right)
        {
            return (right is null) ? (left is null) : right.Equals(left);
        }
        public static bool operator !=(SessionDataMessage? left, SessionDataMessage? right)
        {
            return !(left == right);
        }
        public override bool Equals(object? other)
        {
            if (!(other is SessionDataMessage)) return false;
            return this.Equals((SessionDataMessage)other);
        }
        public bool Equals(SessionDataMessage? target)
        {
            if (target is null) return false;
            if (object.ReferenceEquals(this, target)) return true;
            if (this.SessionId != target.SessionId) return false;
            if (this.IsCompleted != target.IsCompleted) return false;
            if (!global::Omnix.Base.BytesOperations.Equals(this.Data.Span, target.Data.Span)) return false;

            return true;
        }
        public override int GetHashCode() => ___hashCode.Value;

        public void Dispose()
        {
            _data?.Dispose();
        }

        private sealed class ___CustomFormatter : global::Omnix.Serialization.RocketPack.IRocketPackFormatter<SessionDataMessage>
        {
            public void Serialize(ref global::Omnix.Serialization.RocketPack.RocketPackWriter w, in SessionDataMessage value, in int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                {
                    uint propertyCount = 0;
                    if (value.SessionId != 0)
                    {
                        propertyCount++;
                    }
                    if (value.IsCompleted != false)
                    {
                        propertyCount++;
                    }
                    if (!value.Data.IsEmpty)
                    {
                        propertyCount++;
                    }
                    w.Write(propertyCount);
                }

                if (value.SessionId != 0)
                {
                    w.Write((uint)0);
                    w.Write(value.SessionId);
                }
                if (value.IsCompleted != false)
                {
                    w.Write((uint)1);
                    w.Write(value.IsCompleted);
                }
                if (!value.Data.IsEmpty)
                {
                    w.Write((uint)2);
                    w.Write(value.Data.Span);
                }
            }

            public SessionDataMessage Deserialize(ref global::Omnix.Serialization.RocketPack.RocketPackReader r, in int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                uint propertyCount = r.GetUInt32();

                ulong p_sessionId = 0;
                bool p_isCompleted = false;
                global::System.Buffers.IMemoryOwner<byte> p_data = global::Omnix.Base.SimpleMemoryOwner<byte>.Empty;

                for (; propertyCount > 0; propertyCount--)
                {
                    uint id = r.GetUInt32();
                    switch (id)
                    {
                        case 0:
                            {
                                p_sessionId = r.GetUInt64();
                                break;
                            }
                        case 1:
                            {
                                p_isCompleted = r.GetBoolean();
                                break;
                            }
                        case 2:
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

    internal sealed partial class SessionCloseMessage : global::Omnix.Serialization.RocketPack.IRocketPackMessage<SessionCloseMessage>
    {
        public static global::Omnix.Serialization.RocketPack.IRocketPackFormatter<SessionCloseMessage> Formatter { get; }
        public static SessionCloseMessage Empty { get; }

        static SessionCloseMessage()
        {
            SessionCloseMessage.Formatter = new ___CustomFormatter();
            SessionCloseMessage.Empty = new SessionCloseMessage(0);
        }

        private readonly global::System.Lazy<int> ___hashCode;

        public SessionCloseMessage(ulong sessionId)
        {
            this.SessionId = sessionId;

            ___hashCode = new global::System.Lazy<int>(() =>
            {
                var ___h = new global::System.HashCode();
                if (sessionId != default) ___h.Add(sessionId.GetHashCode());
                return ___h.ToHashCode();
            });
        }

        public ulong SessionId { get; }

        public static SessionCloseMessage Import(global::System.Buffers.ReadOnlySequence<byte> sequence, global::Omnix.Base.IBufferPool<byte> bufferPool)
        {
            var reader = new global::Omnix.Serialization.RocketPack.RocketPackReader(sequence, bufferPool);
            return Formatter.Deserialize(ref reader, 0);
        }
        public void Export(global::System.Buffers.IBufferWriter<byte> bufferWriter, global::Omnix.Base.IBufferPool<byte> bufferPool)
        {
            var writer = new global::Omnix.Serialization.RocketPack.RocketPackWriter(bufferWriter, bufferPool);
            Formatter.Serialize(ref writer, this, 0);
        }

        public static bool operator ==(SessionCloseMessage? left, SessionCloseMessage? right)
        {
            return (right is null) ? (left is null) : right.Equals(left);
        }
        public static bool operator !=(SessionCloseMessage? left, SessionCloseMessage? right)
        {
            return !(left == right);
        }
        public override bool Equals(object? other)
        {
            if (!(other is SessionCloseMessage)) return false;
            return this.Equals((SessionCloseMessage)other);
        }
        public bool Equals(SessionCloseMessage? target)
        {
            if (target is null) return false;
            if (object.ReferenceEquals(this, target)) return true;
            if (this.SessionId != target.SessionId) return false;

            return true;
        }
        public override int GetHashCode() => ___hashCode.Value;

        private sealed class ___CustomFormatter : global::Omnix.Serialization.RocketPack.IRocketPackFormatter<SessionCloseMessage>
        {
            public void Serialize(ref global::Omnix.Serialization.RocketPack.RocketPackWriter w, in SessionCloseMessage value, in int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                {
                    uint propertyCount = 0;
                    if (value.SessionId != 0)
                    {
                        propertyCount++;
                    }
                    w.Write(propertyCount);
                }

                if (value.SessionId != 0)
                {
                    w.Write((uint)0);
                    w.Write(value.SessionId);
                }
            }

            public SessionCloseMessage Deserialize(ref global::Omnix.Serialization.RocketPack.RocketPackReader r, in int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                uint propertyCount = r.GetUInt32();

                ulong p_sessionId = 0;

                for (; propertyCount > 0; propertyCount--)
                {
                    uint id = r.GetUInt32();
                    switch (id)
                    {
                        case 0:
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

    internal sealed partial class SessionErrorMessage : global::Omnix.Serialization.RocketPack.IRocketPackMessage<SessionErrorMessage>
    {
        public static global::Omnix.Serialization.RocketPack.IRocketPackFormatter<SessionErrorMessage> Formatter { get; }
        public static SessionErrorMessage Empty { get; }

        static SessionErrorMessage()
        {
            SessionErrorMessage.Formatter = new ___CustomFormatter();
            SessionErrorMessage.Empty = new SessionErrorMessage(0, (SessionErrorType)0);
        }

        private readonly global::System.Lazy<int> ___hashCode;

        public SessionErrorMessage(ulong sessionId, SessionErrorType errorType)
        {
            this.SessionId = sessionId;
            this.ErrorType = errorType;

            ___hashCode = new global::System.Lazy<int>(() =>
            {
                var ___h = new global::System.HashCode();
                if (sessionId != default) ___h.Add(sessionId.GetHashCode());
                if (errorType != default) ___h.Add(errorType.GetHashCode());
                return ___h.ToHashCode();
            });
        }

        public ulong SessionId { get; }
        public SessionErrorType ErrorType { get; }

        public static SessionErrorMessage Import(global::System.Buffers.ReadOnlySequence<byte> sequence, global::Omnix.Base.IBufferPool<byte> bufferPool)
        {
            var reader = new global::Omnix.Serialization.RocketPack.RocketPackReader(sequence, bufferPool);
            return Formatter.Deserialize(ref reader, 0);
        }
        public void Export(global::System.Buffers.IBufferWriter<byte> bufferWriter, global::Omnix.Base.IBufferPool<byte> bufferPool)
        {
            var writer = new global::Omnix.Serialization.RocketPack.RocketPackWriter(bufferWriter, bufferPool);
            Formatter.Serialize(ref writer, this, 0);
        }

        public static bool operator ==(SessionErrorMessage? left, SessionErrorMessage? right)
        {
            return (right is null) ? (left is null) : right.Equals(left);
        }
        public static bool operator !=(SessionErrorMessage? left, SessionErrorMessage? right)
        {
            return !(left == right);
        }
        public override bool Equals(object? other)
        {
            if (!(other is SessionErrorMessage)) return false;
            return this.Equals((SessionErrorMessage)other);
        }
        public bool Equals(SessionErrorMessage? target)
        {
            if (target is null) return false;
            if (object.ReferenceEquals(this, target)) return true;
            if (this.SessionId != target.SessionId) return false;
            if (this.ErrorType != target.ErrorType) return false;

            return true;
        }
        public override int GetHashCode() => ___hashCode.Value;

        private sealed class ___CustomFormatter : global::Omnix.Serialization.RocketPack.IRocketPackFormatter<SessionErrorMessage>
        {
            public void Serialize(ref global::Omnix.Serialization.RocketPack.RocketPackWriter w, in SessionErrorMessage value, in int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                {
                    uint propertyCount = 0;
                    if (value.SessionId != 0)
                    {
                        propertyCount++;
                    }
                    if (value.ErrorType != (SessionErrorType)0)
                    {
                        propertyCount++;
                    }
                    w.Write(propertyCount);
                }

                if (value.SessionId != 0)
                {
                    w.Write((uint)0);
                    w.Write(value.SessionId);
                }
                if (value.ErrorType != (SessionErrorType)0)
                {
                    w.Write((uint)1);
                    w.Write((ulong)value.ErrorType);
                }
            }

            public SessionErrorMessage Deserialize(ref global::Omnix.Serialization.RocketPack.RocketPackReader r, in int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                uint propertyCount = r.GetUInt32();

                ulong p_sessionId = 0;
                SessionErrorType p_errorType = (SessionErrorType)0;

                for (; propertyCount > 0; propertyCount--)
                {
                    uint id = r.GetUInt32();
                    switch (id)
                    {
                        case 0:
                            {
                                p_sessionId = r.GetUInt64();
                                break;
                            }
                        case 1:
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
