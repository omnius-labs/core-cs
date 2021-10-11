// <auto-generated/>
#nullable enable

namespace Omnius.Core.Storages.Tests.Internal
{
    internal readonly partial struct TestMessage : global::Omnius.Core.RocketPack.IRocketMessage<global::Omnius.Core.Storages.Tests.Internal.TestMessage>
    {
        public static global::Omnius.Core.RocketPack.IRocketMessageFormatter<global::Omnius.Core.Storages.Tests.Internal.TestMessage> Formatter => global::Omnius.Core.RocketPack.IRocketMessage<global::Omnius.Core.Storages.Tests.Internal.TestMessage>.Formatter;
        public static global::Omnius.Core.Storages.Tests.Internal.TestMessage Empty => global::Omnius.Core.RocketPack.IRocketMessage<global::Omnius.Core.Storages.Tests.Internal.TestMessage>.Empty;

        static TestMessage()
        {
            global::Omnius.Core.RocketPack.IRocketMessage<global::Omnius.Core.Storages.Tests.Internal.TestMessage>.Formatter = new ___CustomFormatter();
            global::Omnius.Core.RocketPack.IRocketMessage<global::Omnius.Core.Storages.Tests.Internal.TestMessage>.Empty = new global::Omnius.Core.Storages.Tests.Internal.TestMessage(string.Empty);
        }

        private readonly int ___hashCode;

        public static readonly int MaxCommentLength = 2147483647;

        public TestMessage(string comment)
        {
            if (comment is null) throw new global::System.ArgumentNullException("comment");
            if (comment.Length > 2147483647) throw new global::System.ArgumentOutOfRangeException("comment");

            this.Comment = comment;

            {
                var ___h = new global::System.HashCode();
                if (comment != default) ___h.Add(comment.GetHashCode());
                ___hashCode = ___h.ToHashCode();
            }
        }

        public string Comment { get; }

        public static global::Omnius.Core.Storages.Tests.Internal.TestMessage Import(global::System.Buffers.ReadOnlySequence<byte> sequence, global::Omnius.Core.IBytesPool bytesPool)
        {
            var reader = new global::Omnius.Core.RocketPack.RocketMessageReader(sequence, bytesPool);
            return Formatter.Deserialize(ref reader, 0);
        }
        public void Export(global::System.Buffers.IBufferWriter<byte> bufferWriter, global::Omnius.Core.IBytesPool bytesPool)
        {
            var writer = new global::Omnius.Core.RocketPack.RocketMessageWriter(bufferWriter, bytesPool);
            Formatter.Serialize(ref writer, this, 0);
        }

        public static bool operator ==(global::Omnius.Core.Storages.Tests.Internal.TestMessage left, global::Omnius.Core.Storages.Tests.Internal.TestMessage right)
        {
            return right.Equals(left);
        }
        public static bool operator !=(global::Omnius.Core.Storages.Tests.Internal.TestMessage left, global::Omnius.Core.Storages.Tests.Internal.TestMessage right)
        {
            return !(left == right);
        }
        public override bool Equals(object? other)
        {
            if (other is not global::Omnius.Core.Storages.Tests.Internal.TestMessage) return false;
            return this.Equals((global::Omnius.Core.Storages.Tests.Internal.TestMessage)other);
        }
        public bool Equals(global::Omnius.Core.Storages.Tests.Internal.TestMessage target)
        {
            if (this.Comment != target.Comment) return false;

            return true;
        }
        public override int GetHashCode() => ___hashCode;

        private sealed class ___CustomFormatter : global::Omnius.Core.RocketPack.IRocketMessageFormatter<global::Omnius.Core.Storages.Tests.Internal.TestMessage>
        {
            public void Serialize(ref global::Omnius.Core.RocketPack.RocketMessageWriter w, in global::Omnius.Core.Storages.Tests.Internal.TestMessage value, in int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                w.Write(value.Comment);
            }
            public global::Omnius.Core.Storages.Tests.Internal.TestMessage Deserialize(ref global::Omnius.Core.RocketPack.RocketMessageReader r, in int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                string p_comment = string.Empty;

                {
                    p_comment = r.GetString(2147483647);
                }
                return new global::Omnius.Core.Storages.Tests.Internal.TestMessage(p_comment);
            }
        }
    }
}