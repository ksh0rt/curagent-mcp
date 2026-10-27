using System.ComponentModel;
using ModelContextProtocol.Server;

namespace Curagent.Mcp.Tools;

[McpServerToolType]
public sealed class CreditBalanceTool
{
    private readonly IHttpClientFactory _httpFactory;
    private readonly ApiKeyResolver _apiKey;

    public CreditBalanceTool(IHttpClientFactory httpFactory, ApiKeyResolver apiKey)
    {
        _httpFactory = httpFactory;
        _apiKey = apiKey;
    }

    [McpServerTool(ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false), Description(
    "Returns the caller's remaining Curagent credit balance and tier. " +
    "Call this before analyzing to confirm available usage. Sandbox tier includes 3 free " +
    "analyses total, not a recurring allowance; paid tiers use 1 credit per analysis.")]
    public async Task<object> GetCreditBalance()
    {
        var apiKey = _apiKey.Resolve();
        if (string.IsNullOrWhiteSpace(apiKey))
            return new { error = ApiKeyResolver.MissingKeyMessage };

        var client = _httpFactory.CreateClient("curagent");
        client.DefaultRequestHeaders.Add("X-API-Key", apiKey);

        var resp = await client.GetAsync("billing/balance");
        if (!resp.IsSuccessStatusCode)
            return new { error = $"Curagent API returned {(int)resp.StatusCode}." };

        var json = await resp.Content.ReadAsStringAsync();
        return System.Text.Json.JsonSerializer.Deserialize<object>(json)!;
    }
}