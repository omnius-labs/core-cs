
#nullable enable

namespace Omnix.Network.Connections.Multiplexer.V1.Internal
{
    {
        public static global::Omnix.Serialization.RocketPack.IRocketPackFormatter<ProfileMessage> Formatter { get; }
        public static ProfileMessage Empty { get; }

        static ProfileMessage()
        {
            ProfileMessage.Formatter = new ___CustomFormatter();
            ProfileMessage.Empty = new ProfileMessage(0, 0);
        }


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

        public static ProfileMessage Import(global::System.Buffers.ReadOnlySequence<byte> sequence, global::Omnix.Base.BufferPool bufferPool)
        {
            var reader = new global::Omnix.Serialization.RocketPack.RocketPackReader(sequence, bufferPool);
            return Formatter.Deserialize(ref reader, 0);
        }
        public void Export(global::System.Buffers.IBufferWriter<byte> bufferWriter, global::Omnix.Base.BufferPool bufferPool)
        {
            var writer = new global::Omnix.Serialization.RocketPack.RocketPackWriter(bufferWriter, bufferPool);
            Formatter.Serialize(ref writer, this, 0);
        }

        public override bool Equals(object? other)
        {
            if (!(other is ProfileMessage)) return false;
            return this.Equals((ProfileMessage)other);
        }
        {
            if (this.InitialWindowSize != target.InitialWindowSize) return false;
            if (this.MaxSessionAcceptQueueSize != target.MaxSessionAcceptQueueSize) return false;

            return true;
        }

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

    {
        public static global::Omnix.Serialization.RocketPack.IRocketPackFormatter<SessionConnectMessage> Formatter { get; }
        public static SessionConnectMessage Empty { get; }

        static SessionConnectMessage()
        {
            SessionConnectMessage.Formatter = new ___CustomFormatter();
            SessionConnectMessage.Empty = new SessionConnectMessage(0);
        }


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

        public static SessionConnectMessage Import(global::System.Buffers.ReadOnlySequence<byte> sequence, global::Omnix.Base.BufferPool bufferPool)
        {
            var reader = new global::Omnix.Serialization.RocketPack.RocketPackReader(sequence, bufferPool);
            return Formatter.Deserialize(ref reader, 0);
        }
        public void Export(global::System.Buffers.IBufferWriter<byte> bufferWriter, global::Omnix.Base.BufferPool bufferPool)
        {
            var writer = new global::Omnix.Serialization.RocketPack.RocketPackWriter(bufferWriter, bufferPool);
            Formatter.Serialize(ref writer, this, 0);
        }

        public override bool Equals(object? other)
        {
            if (!(other is SessionConnectMessage)) return false;
            return this.Equals((SessionConnectMessage)other);
        }
        {
            if (this.SessionId != target.SessionId) return false;

            return true;
        }

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

    {
        public static global::Omnix.Serialization.RocketPack.IRocketPackFormatter<SessionAcceptMessage> Formatter { get; }
        public static SessionAcceptMessage Empty { get; }

        static SessionAcceptMessage()
        {
            SessionAcceptMessage.Formatter = new ___CustomFormatter();
            SessionAcceptMessage.Empty = new SessionAcceptMessage(0);
        }


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

        public static SessionAcceptMessage Import(global::System.Buffers.ReadOnlySequence<byte> sequence, global::Omnix.Base.BufferPool bufferPool)
        {
            var reader = new global::Omnix.Serialization.RocketPack.RocketPackReader(sequence, bufferPool);
            return Formatter.Deserialize(ref reader, 0);
        }
        public void Export(global::System.Buffers.IBufferWriter<byte> bufferWriter, global::Omnix.Base.BufferPool bufferPool)
        {
            var writer = new global::Omnix.Serialization.RocketPack.RocketPackWriter(bufferWriter, bufferPool);
            Formatter.Serialize(ref writer, this, 0);
        }

        public override bool Equals(object? other)
        {
            if (!(other is SessionAcceptMessage)) return false;
            return this.Equals((SessionAcceptMessage)other);
        }
        {
            if (this.SessionId != target.SessionId) return false;

            return true;
        }

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

    {
        public static global::Omnix.Serialization.RocketPack.IRocketPackFormatter<SessionUpdateWindowSizeMessage> Formatter { get; }
        public static SessionUpdateWindowSizeMessage Empty { get; }

        static SessionUpdateWindowSizeMessage()
        {
            SessionUpdateWindowSizeMessage.Formatter = new ___CustomFormatter();
            SessionUpdateWindowSizeMessage.Empty = new SessionUpdateWindowSizeMessage(0, 0);
        }


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

        public static SessionUpdateWindowSizeMessage Import(global::System.Buffers.ReadOnlySequence<byte> sequence, global::Omnix.Base.BufferPool bufferPool)
        {
            var reader = new global::Omnix.Serialization.RocketPack.RocketPackReader(sequence, bufferPool);
            return Formatter.Deserialize(ref reader, 0);
        }
        public void Export(global::System.Buffers.IBufferWriter<byte> bufferWriter, global::Omnix.Base.BufferPool bufferPool)
        {
            var writer = new global::Omnix.Serialization.RocketPack.RocketPackWriter(bufferWriter, bufferPool);
            Formatter.Serialize(ref writer, this, 0);
        }

        public override bool Equals(object? other)
        {
            if (!(other is SessionUpdateWindowSizeMessage)) return false;
            return this.Equals((SessionUpdateWindowSizeMessage)other);
        }
        {
            if (this.SessionId != target.SessionId) return false;
            if (this.Size != target.Size) return false;

            return true;
        }

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

}
