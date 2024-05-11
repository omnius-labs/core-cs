using System.Buffers;
using Core.Base;
using Core.Cryptography.Functions;
using Core.Pipelines;
using Core.Serialization;

namespace Core.Cryptography;

public partial struct OmniHash
{
    public static OmniHash Create(OmniHashAlgorithmType algorithmType, ReadOnlySpan<byte> message)
    {
        return algorithmType switch
        {
            OmniHashAlgorithmType.Sha2_256 => new OmniHash(algorithmType, Sha2_256.ComputeHash(message)),
            _ => throw new NotSupportedException(),
        };
    }

    public bool Validate(Span<byte> message)
    {
        switch (this.AlgorithmType)
        {
            case OmniHashAlgorithmType.Sha2_256:
                Span<byte> v = stackalloc byte[32];
                Sha2_256.TryComputeHash(message, v);
                return BytesOperations.Equals(this.Value.Span, v);
            default:
                return false;
        }
    }

    public override string ToString() => this.ToString(ConvertBaseType.Base16Lower);

    public string ToString(ConvertBaseType convertStringType)
    {
        var algorithmType = this.AlgorithmType switch
        {
            OmniHashAlgorithmType.Sha2_256 => "sha2-256",
            _ => throw new NotSupportedException()
        };
        var value = OmniBase.Encode(new ReadOnlySequence<byte>(this.Value), convertStringType);

        return algorithmType + ":" + value;
    }

    public static OmniHash Parse(string text)
    {
        if (TryParse(text, out var value)) return value;
        throw new FormatException("Invalid format");
    }

    public static bool TryParse(string text, out OmniHash result)
    {
        result = default;

        var sp = text.IndexOf(':');
        if (sp == -1) return false;

        var algorithmTypeString = text[..sp];
        var valueString = text[(sp + 1)..];

        var algorithmType = algorithmTypeString switch
        {
            "sha2-256" => OmniHashAlgorithmType.Sha2_256,
            _ => throw new NotSupportedException()
        };
        var valueLength = algorithmType switch
        {
            OmniHashAlgorithmType.Sha2_256 => 32,
            _ => throw new NotSupportedException()
        };

        var bufferWriter = new ArrayBufferWriter<byte>(valueLength);
        if (!OmniBase.TryDecode(valueString, bufferWriter)) return false;
        var value = bufferWriter.WrittenMemory;

        result = new OmniHash(algorithmType, value);

        return true;
    }
}
