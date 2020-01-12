namespace Omnius.Core.Helpers
{
    public static class MathHelper
    {
        public static ulong RoundUp(ulong value, ulong unit)
        {
            if (value % unit == 0)
            {
                return value;
            }
            else
            {
                return ((value / unit) + 1) * unit;
            }
        }
    }
}
