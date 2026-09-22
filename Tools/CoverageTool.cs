using System.ComponentModel;
using ModelContextProtocol.Server;

namespace Curagent.Mcp.Tools;

[McpServerToolType]
public sealed class CoverageTool
{
    private readonly IHttpClientFactory _httpFactory;

    public CoverageTool(IHttpClientFactory httpFactory)
    {
        _httpFactory = httpFactory;
    }

    [McpServerTool(ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false), Description(
         "Returns what Curagent supports: every analyzed US jurisdiction with its maturity, which document " +
        "types, and how analysis is priced. Curagent runs its full analysis for properties in all 50 states " +
        "and DC using each state's own terminology; jurisdictions at production maturity also include " +
        "hand-verified statute citations in cure guidance. No API key is needed for this call.")]
    public async Task<object> CheckCoverage()
    {
        // Coverage comes from the API's registry rather than a copy here, so the
        // MCP server can never disagree with what the API actually supports.
        var client = _httpFactory.CreateClient("curagent");
        var resp = await client.GetAsync("coverage");
        if (!resp.IsSuccessStatusCode)
            return new { error = $"Curagent API returned {(int)resp.StatusCode}." };

        using var doc = System.Text.Json.JsonDocument.Parse(await resp.Content.ReadAsStringAsync());

        return new
        {
            coverage = doc.RootElement.Clone(),
            supportedDocumentTypes = new[]
            {
                "Warranty Deed", "Title Commitment", "Mortgage", "Closing Disclosure",
                "Survey", "Payoff Letter", "HOA Estoppel", "and related closing documents"
            },
            pricing = "Sandbox tier includes 3 free analyses to start; paid tiers use 1 credit per " +
                      "analysis (credits purchased in bundles). Use get_credit_balance to check usage.",
            getAccess = "Request an API key at https://curagent.io"
        };
    }
}