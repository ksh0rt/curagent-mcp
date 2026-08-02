var builder = WebApplication.CreateBuilder(args);

// Register the MCP server with HTTP transport and auto-discover tools in this assembly
builder.Services.AddMcpServer()
    .WithHttpTransport()
    .WithToolsFromAssembly();

builder.Services.AddHttpContextAccessor();

// HttpClient so our tools can call the Curagent API
builder.Services.AddHttpClient("curagent", client =>
{
    client.BaseAddress = new Uri("https://api.curagent.io/");
});

var app = builder.Build();

// Serve the Glama domain-verification file
app.MapGet("/.well-known/glama.json", () =>
    Results.Content(
        "{\"$schema\":\"https://glama.ai/mcp/schemas/connector.json\",\"maintainers\":[{\"email\":\"kshort2584@gmail.com\"}]}",
        "application/json"));

// Map the MCP endpoints
app.MapMcp();

app.Run();