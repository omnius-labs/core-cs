namespace Omnius.Core.RocketPack.Remoting;

public interface IErrorMessageFactory<TErrorMessage>
    where TErrorMessage : IRocketMessage<TErrorMessage>
{
    TErrorMessage Create(Exception e);
}
