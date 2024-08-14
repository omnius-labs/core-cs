namespace Omnius.Core.Omnikit.Connections.Secure.V1;

static class Utils
{
    public static void IncrementBytes(byte[] bytes)
    {
        for (int i = 0; i < bytes.Length; i++)
        {
            if (bytes[i] == 0xFF)
            {
                bytes[i] = 0;
            }
            else
            {
                bytes[i]++;
                break;
            }
        }
    }
}
