namespace Omnius.Core.Network
{
    public partial class OmniAddress
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public static explicit operator string?(OmniAddress? omniAddress)
        {
            return omniAddress?.Value;
        }

        public static explicit operator OmniAddress?(string? text)
        {
            if (text is null)
            {
                return null;
            }

            return new OmniAddress(text);
        }

        public override string ToString()
        {
            return this.Value;
        }
    }
}
