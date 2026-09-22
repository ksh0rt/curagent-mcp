<!-- mcp-name: io.github.ksh0rt/curagent-mcp -->

<p align="center">
  <img src="https://curagent.io/curagent-icon-180.png" alt="Curagent" width="96" height="96" />
</p>

<h1 align="center">Curagent MCP Server</h1>

<p align="center">AI title risk analysis for US real estate closing documents.</p>

**AI-powered title risk analysis for real estate closing documents, exposed as an MCP server.**

Curagent analyzes real estate title documents — deeds, title commitments, mortgages, closing disclosures, surveys, payoff letters, HOA estoppels, and full closing packages — and returns a structured risk report with a composite score, individual findings, and AI-generated cure guidance.

This MCP server lets AI agents and MCP-compatible clients (Claude, Cursor, and others) call Curagent directly.

> **Coverage:** Curagent analyzes properties in **all 50 states and DC**. Each jurisdiction has a maturity level, and every analysis reports the one it applied in `jurisdictionApplied`:
>
> - **production:** full analysis, validated against real documents from the state, with hand-verified statute citations in cure guidance. Currently **Florida**.
> - **draft:** full analysis (every check, cross-document contradiction detection and cure guidance) using the state's own terminology, such as deed of trust vs. mortgage. Statute citations are added as each state is validated.
>
> Call `check_coverage` for the live list. It reads from the same registry the API uses, so it's always current.

---

## Connection

This is a **hosted remote MCP server** — there's nothing to install or run. Point your MCP client at the hosted endpoint:

```
https://mcp.curagent.io/
```

Transport: **Streamable HTTP**

### Authentication

Curagent authenticates with an API key, passed as an `X-API-Key` header on the connection. You configure it once when you set up the connection; the server forwards it to the Curagent API on each call.

You'll need a Curagent API key to use the analysis tools. Request access at **[curagent.io](https://curagent.io)**.

### Example client configuration

```json
{
  "mcpServers": {
    "curagent": {
      "url": "https://mcp.curagent.io/",
      "transport": "streamable-http",
      "headers": {
        "X-API-Key": "your_curagent_api_key"
      }
    }
  }
}
```

> Configuration format varies by client. Consult your MCP client's documentation for how it accepts a remote server URL and custom headers.

---

## Tools

The server exposes three tools, designed so an agent can confirm fit and available balance *before* running an analysis.

### `check_coverage`
Returns what Curagent supports: every analyzed jurisdiction with its maturity and whether statute citations are enabled, plus document types and pricing. No API key required. Call this first to see how the property's state is supported.

### `get_credit_balance`
Returns the caller's remaining credit balance and tier. Requires an API key. Call this before analyzing to confirm available usage.

### `analyze_title_documents`
Analyzes one or more title documents and returns a structured risk report: risk score and level, findings with verbatim evidence and cure guidance, extracted Schedule B-I requirements, and the jurisdiction applied (`jurisdictionApplied`). Requires an API key.

- **Input:** one or more PDF documents, each as a base64-encoded string.
- **Cost:** uses one of your 3 free analyses (sandbox tier) or 1 credit (paid tiers).
- **Scope:** all 50 states and DC. Statute citations appear only for jurisdictions at production maturity.

---

## How it works

Curagent's risk engine evaluates a closing package the way a title professional would — checking for issues like unresolved liens, undisclosed easements, encroachments, probate and authority questions, legal description mismatches, expired documents, and party-name inconsistencies, and cross-document contradictions in parcel ID, address, and party names across a full package — then returns a scored, itemized report with guidance on how to cure each finding.

The MCP server is a thin layer over the Curagent API. Your API key flows through it to the API, which does the analysis and meters usage.

---

## Pricing

- **Sandbox tier** — 3 free analyses to start, for evaluation.
- **Credit bundles** — prepaid credits, 1 per analysis. Credits don't expire.
- **Volume** — for higher-throughput or platform use, get in touch.

See **[curagent.io](https://curagent.io)** for current details and to request access.

---

## Links

- Website: [curagent.io](https://curagent.io)
- API base: `https://api.curagent.io`
- MCP endpoint: `https://mcp.curagent.io/`

---

Curagent is a product of Caldira LLC.
