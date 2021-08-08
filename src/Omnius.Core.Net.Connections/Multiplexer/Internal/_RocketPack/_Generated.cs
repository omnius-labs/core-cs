// <auto-generated/>
#nullable enable

namespace Omnius.Core.Net.Connections.Multiplexer.Internal
{
    internal sealed partial class HelloMessage : global::Omnius.Core.RocketPack.IRocketMessage<global::Omnius.Core.Net.Connections.Multiplexer.Internal.HelloMessage>
    {
        public static global::Omnius.Core.RocketPack.IRocketMessageFormatter<global::Omnius.Core.Net.Connections.Multiplexer.Internal.HelloMessage> Formatter => global::Omnius.Core.RocketPack.IRocketMessage<global::Omnius.Core.Net.Connections.Multiplexer.Internal.HelloMessage>.Formatter;
        public static global::Omnius.Core.Net.Connections.Multiplexer.Internal.HelloMessage Empty => global::Omnius.Core.RocketPack.IRocketMessage<global::Omnius.Core.Net.Connections.Multiplexer.Internal.HelloMessage>.Empty;

        static HelloMessage()
        {
            global::Omnius.Core.RocketPack.IRocketMessage<global::Omnius.Core.Net.Connections.Multiplexer.Internal.HelloMessage>.Formatter = new ___CustomFormatter();
            global::Omnius.Core.RocketPack.IRocketMessage<global::Omnius.Core.Net.Connections.Multiplexer.Internal.HelloMessage>.Empty = new global::Omnius.Core.Net.Connections.Multiplexer.Internal.HelloMessage(global::System.Array.Empty<global::Omnius.Core.Net.Connections.Multiplexer.OmniConnectionMultiplexerVersion>());
        }

        private readonly global::System.Lazy<int> ___hashCode;

        public static readonly int MaxVersionsCount = 32;

        public HelloMessage(global::Omnius.Core.Net.Connections.Multiplexer.OmniConnectionMultiplexerVersion[] versions)
        {
            if (versions is null) throw new global::System.ArgumentNullException("versions");
            if (versions.Length > 32) throw new global::System.ArgumentOutOfRangeException("versions");

            this.Versions = new global::Omnius.Core.Collections.ReadOnlyListSlim<global::Omnius.Core.Net.Connections.Multiplexer.OmniConnectionMultiplexerVersion>(versions);

            ___hashCode = new global::System.Lazy<int>(() =>
            {
                var ___h = new global::System.HashCode();
                foreach (var n in versions)
                {
                    if (n != default) ___h.Add(n.GetHashCode());
                }
                return ___h.ToHashCode();
            });
        }

        public global::Omnius.Core.Collections.ReadOnlyListSlim<global::Omnius.Core.Net.Connections.Multiplexer.OmniConnectionMultiplexerVersion> Versions { get; }

        public static global::Omnius.Core.Net.Connections.Multiplexer.Internal.HelloMessage Import(global::System.Buffers.ReadOnlySequence<byte> sequence, global::Omnius.Core.IBytesPool bytesPool)
        {
            var reader = new global::Omnius.Core.RocketPack.RocketMessageReader(sequence, bytesPool);
            return Formatter.Deserialize(ref reader, 0);
        }
        public void Export(global::System.Buffers.IBufferWriter<byte> bufferWriter, global::Omnius.Core.IBytesPool bytesPool)
        {
            var writer = new global::Omnius.Core.RocketPack.RocketMessageWriter(bufferWriter, bytesPool);
            Formatter.Serialize(ref writer, this, 0);
        }

        public static bool operator ==(global::Omnius.Core.Net.Connections.Multiplexer.Internal.HelloMessage? left, global::Omnius.Core.Net.Connections.Multiplexer.Internal.HelloMessage? right)
        {
            return (right is null) ? (left is null) : right.Equals(left);
        }
        public static bool operator !=(global::Omnius.Core.Net.Connections.Multiplexer.Internal.HelloMessage? left, global::Omnius.Core.Net.Connections.Multiplexer.Internal.HelloMessage? right)
        {
            return !(left == right);
        }
        public override bool Equals(object? other)
        {
            if (other is not global::Omnius.Core.Net.Connections.Multiplexer.Internal.HelloMessage) return false;
            return this.Equals((global::Omnius.Core.Net.Connections.Multiplexer.Internal.HelloMessage)other);
        }
        public bool Equals(global::Omnius.Core.Net.Connections.Multiplexer.Internal.HelloMessage? target)
        {
            if (target is null) return false;
            if (object.ReferenceEquals(this, target)) return true;
            if (!global::Omnius.Core.Helpers.CollectionHelper.Equals(this.Versions, target.Versions)) return false;

            return true;
        }
        public override int GetHashCode() => ___hashCode.Value;

        private sealed class ___CustomFormatter : global::Omnius.Core.RocketPack.IRocketMessageFormatter<global::Omnius.Core.Net.Connections.Multiplexer.Internal.HelloMessage>
        {
            public void Serialize(ref global::Omnius.Core.RocketPack.RocketMessageWriter w, in global::Omnius.Core.Net.Connections.Multiplexer.Internal.HelloMessage value, in int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                if (value.Versions.Count != 0)
                {
                    w.Write((uint)1);
                    w.Write((uint)value.Versions.Count);
                    foreach (var n in value.Versions)
                    {
                        w.Write((ulong)n);
                    }
                }
                w.Write((uint)0);
            }
            public global::Omnius.Core.Net.Connections.Multiplexer.Internal.HelloMessage Deserialize(ref global::Omnius.Core.RocketPack.RocketMessageReader r, in int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                global::Omnius.Core.Net.Connections.Multiplexer.OmniConnectionMultiplexerVersion[] p_versions = global::System.Array.Empty<global::Omnius.Core.Net.Connections.Multiplexer.OmniConnectionMultiplexerVersion>();

                for (; ; )
                {
                    uint id = r.GetUInt32();
                    if (id == 0) break;
                    switch (id)
                    {
                        case 1:
                            {
                                var length = r.GetUInt32();
                                p_versions = new global::Omnius.Core.Net.Connections.Multiplexer.OmniConnectionMultiplexerVersion[length];
                                for (int i = 0; i < p_versions.Length; i++)
                                {
                                    p_versions[i] = (global::Omnius.Core.Net.Connections.Multiplexer.OmniConnectionMultiplexerVersion)r.GetUInt64();
                                }
                                break;
                            }
                    }
                }

                return new global::Omnius.Core.Net.Connections.Multiplexer.Internal.HelloMessage(p_versions);
            }
        }
    }
}
