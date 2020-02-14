
#nullable enable

namespace Omnius.Core.Data.Tests.Internal
{
    internal readonly struct TestObject : global::Omnius.Core.Serialization.RocketPack.IRocketPackObject<TestObject>
    {
        public static global::Omnius.Core.Serialization.RocketPack.IRocketPackFormatter<TestObject> Formatter { get; }
        public static TestObject Empty { get; }

        static TestObject()
        {
            TestObject.Formatter = new ___CustomFormatter();
            TestObject.Empty = new TestObject(0);
        }

        private readonly int ___hashCode;

        public TestObject(int value)
        {
            this.Value = value;

            {
                var ___h = new global::System.HashCode();
                if (value != default) ___h.Add(value.GetHashCode());
                ___hashCode = ___h.ToHashCode();
            }
        }

        public int Value { get; }

        public static TestObject Import(global::System.Buffers.ReadOnlySequence<byte> sequence, global::Omnius.Core.IBytesPool bytesPool)
        {
            var reader = new global::Omnius.Core.Serialization.RocketPack.RocketPackReader(sequence, bytesPool);
            return Formatter.Deserialize(ref reader, 0);
        }
        public void Export(global::System.Buffers.IBufferWriter<byte> bufferWriter, global::Omnius.Core.IBytesPool bytesPool)
        {
            var writer = new global::Omnius.Core.Serialization.RocketPack.RocketPackWriter(bufferWriter, bytesPool);
            Formatter.Serialize(ref writer, this, 0);
        }

        public static bool operator ==(TestObject left, TestObject right)
        {
            return right.Equals(left);
        }
        public static bool operator !=(TestObject left, TestObject right)
        {
            return !(left == right);
        }
        public override bool Equals(object? other)
        {
            if (!(other is TestObject)) return false;
            return this.Equals((TestObject)other);
        }
        public bool Equals(TestObject target)
        {
            if (this.Value != target.Value) return false;

            return true;
        }
        public override int GetHashCode() => ___hashCode;

        private sealed class ___CustomFormatter : global::Omnius.Core.Serialization.RocketPack.IRocketPackFormatter<TestObject>
        {
            public void Serialize(ref global::Omnius.Core.Serialization.RocketPack.RocketPackWriter w, in TestObject value, in int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                w.Write(value.Value);
            }

            public TestObject Deserialize(ref global::Omnius.Core.Serialization.RocketPack.RocketPackReader r, in int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                int p_value = 0;

                {
                    p_value = r.GetInt32();
                }
                return new TestObject(p_value);
            }
        }
    }

}
