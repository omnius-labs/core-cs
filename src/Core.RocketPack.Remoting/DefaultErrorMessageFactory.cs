namespace Core.RocketPack.Remoting;

public class DefaultErrorMessageFactory : IErrorMessageFactory<DefaultErrorMessage>
{
    public static DefaultErrorMessageFactory Default { get; } = new();

    public DefaultErrorMessage Create(Exception e)
    {
        return new DefaultErrorMessage(e.GetType()?.FullName ?? string.Empty, e.Message, e.StackTrace ?? string.Empty);
    }
}
