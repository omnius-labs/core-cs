using System;

namespace Omnius.Core.RocketPack.Remoting
{
    public class DefaultErrorMessageFactory : IErrorMessageFactory<DefaultRocketRemotingErrorMessage>
    {
        public DefaultRocketRemotingErrorMessage Create(Exception e)
        {
            return new DefaultRocketRemotingErrorMessage(e.GetType()?.FullName ?? string.Empty, e.Message, e.StackTrace);
        }
    }
}
