namespace Core.Base;

public interface IRandomBytesProvider
{
    byte[] GetBytes(int length);
}
