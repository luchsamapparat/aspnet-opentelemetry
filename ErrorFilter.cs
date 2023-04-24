namespace Playground;

public class ErrorFilter : IErrorFilter
{
    private readonly ILogger _logger;

    public ErrorFilter(ILogger<ErrorFilter> logger)
    {
        _logger = logger;
    }

    public IError OnError(IError error)
    {
        _logger.LogError(error.Exception, error.Exception?.Message ?? error.Message);
        return error;
    }

}