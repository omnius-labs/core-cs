namespace Omnius.Core.Serialization;

public unsafe ref struct Crc32_Castagnoli
{
    private static readonly uint[] _table;

    static Crc32_Castagnoli()
    {
        // uint poly = 0xedb88320;
        uint poly = 0x82F63B78;
        _table = new uint[256];

        for (uint i = 0; i < 256; i++)
        {
            uint x = i;

            for (int j = 0; j < 8; j++)
            {
                if ((x & 1) != 0)
                {
                    x = (x >> 1) ^ poly;
                }
                else
                {
                    x >>= 1;
                }
            }

            _table[i] = x;
        }
    }

    private uint _result = 0xFFFFFFFF;

    public Crc32_Castagnoli()
    {
    }

    public void Compute(ReadOnlySpan<byte> memory)
    {
        fixed (uint* p_table = _table)
        fixed (byte* p_src = memory)
        {
            var t_src = p_src;

            for (int i = 0; i < memory.Length; i++)
            {
                _result = (_result >> 8) ^ p_table[(_result & 0xff) ^ *t_src++];
            }
        }
    }

    public uint GetResult()
    {
        return (uint)(_result ^ 0xFFFFFFFF);
    }
}
