using System.ComponentModel;
using System.Net.Http.Headers;
using ModelContextProtocol.Server;

namespace Curagent.Mcp.Tools;

[McpServerToolType]
public sealed class AnalyzeTool
{
    private readonly IHttpClientFactory _httpFactory;
    private readonly IHttpContextAccessor _http;

    public AnalyzeTool(IHttpClientFactory httpFactory, IHttpContextAccessor http)
    {
        _httpFactory = httpFactory;
        _http = http;
    }

    [McpServerTool, Description(
    "Analyzes Florida real estate title documents and returns a structured risk report: " +
    "a risk score (0-100, higher is safer) and level, findings with verbatim evidence " +
    "from the documents and guidance on how to cure each one, and the Schedule B-I " +
    "requirements extracted from any title commitment in the package. Accepts one or " +
    "more PDF documents as base64 strings (deed, title commitment, mortgage, closing " +
    "disclosure, survey, payoff letter, HOA estoppel, etc.), including a single PDF " +
    "containing a whole closing package, which is split into its constituent " +
    "instruments. Submitting several documents together also enables cross-document " +
    "checks for contradictions in parcel ID, address, and party names. " +
    "Uses one of your 3 free analyses (sandbox tier) or 1 credit (paid tiers). " +
    "Florida properties only — call check_coverage first to confirm scope, and " +
    "get_credit_balance to confirm available credits.")]
    public async Task<object> AnalyzeTitleDocuments(
        [Description("One or more PDF documents, each as a base64-encoded string.")]
        string[] documentsBase64)
    {
        var apiKey = _http.HttpContext?.Request.Headers["X-API-Key"].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(apiKey))
            return new { error = "Missing API key. Get one at https://curagent.io and configure it in your MCP connection." };

        if (documentsBase64 is null || documentsBase64.Length == 0)
            return new { error = "No documents provided." };

        var client = _httpFactory.CreateClient("curagent");
        client.DefaultRequestHeaders.Add("X-API-Key", apiKey);

        using var form = new MultipartFormDataContent();

        for (int i = 0; i < documentsBase64.Length; i++)
        {
            byte[] bytes;
            try
            {
                bytes = Convert.FromBase64String(documentsBase64[i]);
            }
            catch
            {
                return new { error = $"Document {i + 1} is not valid base64." };
            }

            var fileContent = new ByteArrayContent(bytes);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
            form.Add(fileContent, "files", $"document_{i + 1}.pdf");
        }

        var resp = await client.PostAsync("analyze", form);
        var json = await resp.Content.ReadAsStringAsync();

        if (!resp.IsSuccessStatusCode)
            return new { error = $"Curagent API returned {(int)resp.StatusCode}.", detail = json };

        return System.Text.Json.JsonSerializer.Deserialize<object>(json)!;
    }
}