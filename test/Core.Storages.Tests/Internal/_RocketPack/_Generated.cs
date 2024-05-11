// <auto-generated/>
#nullable enable

namespace Core.Storages.Tests.Internal;

internal readonly partial struct TestMessage : global::Core.RocketPack.IRocketMessage<global::Core.Storages.Tests.Internal.TestMessage>
{
    public static global::Core.RocketPack.IRocketMessageFormatter<global::Core.Storages.Tests.Internal.TestMessage> Formatter => global::Core.RocketPack.IRocketMessage<global::Core.Storages.Tests.Internal.TestMessage>.Formatter;
    public static global::Core.Storages.Tests.Internal.TestMessage Empty => global::Core.RocketPack.IRocketMessage<global::Core.Storages.Tests.Internal.TestMessage>.Empty;

    static TestMessage()
    {
        global::Core.RocketPack.IRocketMessage<global::Core.Storages.Tests.Internal.TestMessage>.Formatter = new ___CustomFormatter();
        global::Core.RocketPack.IRocketMessage<global::Core.Storages.Tests.Internal.TestMessage>.Empty = new global::Core.Storages.Tests.Internal.TestMessage(global::Core.RocketPack.Utf8String.Empty);
    }

    private readonly int ___hashCode;

    public static readonly int MaxCommentLength = 2147483647;

    public TestMessage(global::Core.RocketPack.Utf8String comment)
    {
        if (comment is null) throw new global::System.ArgumentNullException("comment");
        if (comment.Length > 2147483647) throw new global::System.ArgumentOutOfRangeException("comment");

        this.Comment = comment;

        {
            var ___h = new global::System.HashCode();
            if (!comment.IsEmpty) ___h.Add(comment.GetHashCode());
            ___hashCode = ___h.ToHashCode();
        }
    }

    public global::Core.RocketPack.Utf8String Comment { get; }

    public static global::Core.Storages.Tests.Internal.TestMessage Import(global::System.Buffers.ReadOnlySequence<byte> sequence, global::Core.Base.IBytesPool bytesPool)
    {
        var reader = new global::Core.RocketPack.RocketMessageReader(sequence, bytesPool);
        return Formatter.Deserialize(ref reader, 0);
    }
    public void Export(global::System.Buffers.IBufferWriter<byte> bufferWriter, global::Core.Base.IBytesPool bytesPool)
    {
        var writer = new global::Core.RocketPack.RocketMessageWriter(bufferWriter, bytesPool);
        Formatter.Serialize(ref writer, this, 0);
    }

    public static bool operator ==(global::Core.Storages.Tests.Internal.TestMessage left, global::Core.Storages.Tests.Internal.TestMessage right)
    {
        return right.Equals(left);
    }
    public static bool operator !=(global::Core.Storages.Tests.Internal.TestMessage left, global::Core.Storages.Tests.Internal.TestMessage right)
    {
        return !(left == right);
    }
    public override bool Equals(object? other)
    {
        if (other is not global::Core.Storages.Tests.Internal.TestMessage) return false;
        return this.Equals((global::Core.Storages.Tests.Internal.TestMessage)other);
    }
    public bool Equals(global::Core.Storages.Tests.Internal.TestMessage target)
    {
        if (this.Comment != target.Comment) return false;

        return true;
    }
    public override int GetHashCode() => ___hashCode;

    private sealed class ___CustomFormatter : global::Core.RocketPack.IRocketMessageFormatter<global::Core.Storages.Tests.Internal.TestMessage>
    {
        public void Serialize(ref global::Core.RocketPack.RocketMessageWriter w, scoped in global::Core.Storages.Tests.Internal.TestMessage value, scoped in int rank)
        {
            if (rank > 256) throw new global::System.FormatException();

            w.Write(value.Comment);
        }
        public global::Core.Storages.Tests.Internal.TestMessage Deserialize(ref global::Core.RocketPack.RocketMessageReader r, scoped in int rank)
        {
            if (rank > 256) throw new global::System.FormatException();

            global::Core.RocketPack.Utf8String p_comment = global::Core.RocketPack.Utf8String.Empty;

            {
                p_comment = r.GetString(2147483647);
            }
            return new global::Core.Storages.Tests.Internal.TestMessage(p_comment);
        }
    }
}
