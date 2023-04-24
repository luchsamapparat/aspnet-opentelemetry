namespace Playground;

public class Query
{
    private readonly ILogger _logger;

    public Query(ILogger<Query> logger)
    {
        _logger = logger;
    }

    public Response GetInfo()
    {
        var message = "my info log message 🤓";
        _logger.LogInformation(message);
        return new Response
        {
            Message = message
        };
    }

    public Response GetDebug()
    {
        var message = "my debug log message 🤨";
        _logger.LogDebug(message);
        return new Response
        {
            Message = message
        };
    }

    public Response GetError()
    {
        var message = "my error log message 😢";
        _logger.LogError(message);
        return new Response
        {
            Message = message
        };
    }

    public Response GetCrash()
    {
        throw new ApplicationException("my application exception 🔥");
    }
}
