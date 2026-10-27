using Curagent.Mcp;

// One server, two ways to run it. The tools are identical in both.
//
//   (default)  The hosted server at mcp.curagent.io: MCP over streamable HTTP. Each
//              caller's API key arrives in their X-API-Key header.
//   --stdio    A local process speaking MCP over stdin/stdout. This is how Glama and
//              desktop MCP clients launch it; the key comes from CURAGENT_API_KEY.
if (args.Contains("--stdio"))
{
    var stdio = Host.CreateApplicationBuilder(args);

    // stdout carries the MCP protocol itself, so every log line must go to stderr.
    // A stray write to stdout would corrupt the protocol stream.
    stdio.Logging.ClearProviders();
    stdio.Logging.AddConsole(o => o.LogToStandardErrorThreshold = LogLevel.Trace);

    AddCuragentApi(stdio.Services, stdio.Configuration);

    stdio.Services.AddMcpServer()
        .WithStdioServerTransport()
        .WithToolsFromAssembly();

    await stdio.Build().RunAsync();
    return;
}

var builder = WebApplication.CreateBuilder(args);

AddCuragentApi(builder.Services, builder.Configuration);

builder.Services.AddMcpServer()
    .WithHttpTransport()
    .WithToolsFromAssembly();

var app = builder.Build();

// Serve the Glama domain-verification file
app.MapGet("/.well-known/glama.json", () =>
    Results.Content(
        "{\"$schema\":\"https://glama.ai/mcp/schemas/connector.json\",\"maintainers\":[{\"email\":\"kshort2584@gmail.com\"}]}",
        "application/json"));

// Map the MCP endpoints
app.MapMcp();

app.Run();

// Services both modes share: where the Curagent API is, and how a tool finds the
// caller's API key.
static void AddCuragentApi(IServiceCollection services, IConfiguration configuration)
{
    // Configurable so local development calls the local API; production when unset.
    var apiBaseUrl = new Uri(configuration["Curagent:ApiBaseUrl"] ?? "https://api.curagent.io/");

    // Every request carries the caller's API key to this address, so refuse anything
    // but HTTPS. Plain HTTP is allowed only for localhost during development.
    if (apiBaseUrl.Scheme != Uri.UriSchemeHttps && !apiBaseUrl.IsLoopback)
        throw new InvalidOperationException(
            $"Curagent:ApiBaseUrl must use HTTPS, but is '{apiBaseUrl}'.");

    services.AddHttpContextAccessor();
    services.AddSingleton<ApiKeyResolver>();
    services.AddHttpClient("curagent", client => client.BaseAddress = apiBaseUrl);
}