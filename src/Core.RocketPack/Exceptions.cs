namespace Omnius.Core.RocketPack;

public enum RocketMessageErrorCode
{
    EndOfInput,
    LimitExceeded,
    TooSmallBody,
    InvalidUtf8,
}

public sealed class RocketMessageException : Exception
{
    internal RocketMessageException(RocketMessageErrorCode errorCode)
        : base(GetErrorMessage(errorCode))
    {
        this.ErrorCode = errorCode;
    }

    public RocketMessageErrorCode ErrorCode { get; }

    private static string GetErrorMessage(RocketMessageErrorCode errorCode)
    {
        return errorCode switch
        {
            RocketMessageErrorCode.EndOfInput => "The end of the input has been reached.",
            RocketMessageErrorCode.LimitExceeded => "The limit has been exceeded.",
            RocketMessageErrorCode.TooSmallBody => "The body is too small.",
            RocketMessageErrorCode.InvalidUtf8 => "The message contains invalid UTF-8 characters.",
            _ => throw new NotSupportedException(),
        };
    }
}

public enum VarintErrorCode
{
    EndOfInput,
    InvalidHeader,
    TooSmallBody,
}

public sealed class VarintException : Exception
{
    internal VarintException(VarintErrorCode errorCode)
        : base(GetErrorMessage(errorCode))
    {
        this.ErrorCode = errorCode;
    }

    public VarintErrorCode ErrorCode { get; }

    private static string GetErrorMessage(VarintErrorCode errorCode)
    {
        return errorCode switch
        {
            VarintErrorCode.EndOfInput => "The end of the input has been reached.",
            VarintErrorCode.InvalidHeader => "The header is invalid.",
            VarintErrorCode.TooSmallBody => "The body is too small.",
            _ => throw new NotSupportedException(),
        };
    }
}
