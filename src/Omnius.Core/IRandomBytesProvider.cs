namespace Omnius.Core;

public interface IRandomBytesProvider
{
    byte[] GetBytes(int length);
}
