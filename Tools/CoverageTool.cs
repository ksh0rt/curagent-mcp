using System.ComponentModel;
using ModelContextProtocol.Server;

namespace Curagent.Mcp.Tools;

[McpServerToolType]
public sealed class CoverageTool
{
    [McpServerTool, Description(
        "Returns what Curagent currently supports: which US states, which document types, " +
        "and how analysis is priced. Call this before analyzing to confirm the property's " +
        "state is in scope. Curagent currently supports Florida real estate transactions only.")]
    public static object CheckCoverage()
    {
        return new
        {
            supportedStates = new[] { "FL" },
            supportedStateNames = new[] { "Florida" },
            note = "Curagent currently analyzes Florida real estate title documents only. " +
                   "Properties in other states will be rejected as out of scope. " +
                   "Additional states are on the roadmap.",
            supportedDocumentTypes = new[]
            {
                "Warranty Deed", "Title Commitment", "Mortgage", "Closing Disclosure",
                "Survey", "Payoff Letter", "HOA Estoppel", "and related closing documents"
            },
            pricing = "Sandbox tier includes free monthly analyses; paid tiers use 1 credit per " +
            "analysis (credits purchased in bundles). Use get_credit_balance to check usage." 
        };
    }
}