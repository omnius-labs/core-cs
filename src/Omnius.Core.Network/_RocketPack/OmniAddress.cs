namespace Omnius.Core.Network
{
    public partial class OmniAddress
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public override string ToString()
        {
            return this.Value;
        }
    }
}
