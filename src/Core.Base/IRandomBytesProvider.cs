namespace Omnius.Core.Base;

public interface IRandomBytesProvider
{
    byte[] GetBytes(int length);
    void Fill(Span<byte> data);
}
