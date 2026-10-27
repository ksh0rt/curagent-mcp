namespace Curagent.Mcp;

/// <summary>
/// Where the caller's Curagent API key comes from.
///
/// Hosted (HTTP): the X-API-Key header on the incoming MCP request, so every caller
/// spends their own credits.
/// Local (stdio): the CURAGENT_API_KEY environment variable, because a server launched
/// as a process over stdin/stdout has no request headers. This is how Glama and
/// desktop MCP clients run it.
///
/// The environment variable is NEVER used when a request is present. If it were, a
/// hosted server with the variable set would let anonymous callers spend that key.
/// </summary>
public sealed class ApiKeyResolver
{
    public const string EnvironmentVariable = "CURAGENT_API_KEY";

    public const string MissingKeyMessage =
        "Missing API key. Get one at https://curagent.io. Hosted connections send it as the " +
        "X-API-Key header; local (stdio) connections set the CURAGENT_API_KEY environment variable.";

    private readonly IHttpContextAccessor _http;

    public ApiKeyResolver(IHttpContextAccessor http) => _http = http;

    public string? Resolve()
    {
        var context = _http.HttpContext;

        if (context is not null)
            return context.Request.Headers["X-API-Key"].FirstOrDefault();

        return Environment.GetEnvironmentVariable(EnvironmentVariable);
    }
}