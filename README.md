<!-- mcp-name: io.github.ksh0rt/curagent-mcp -->

<p align="center">
  <img src="https://curagent.io/curagent-icon-180.png" alt="Curagent" width="96" height="96" />
</p>

<h1 align="center">Curagent MCP Server</h1>

<p align="center">AI title risk analysis for Florida real estate closing documents.</p>

**AI-powered title risk analysis for real estate closing documents, exposed as an MCP server.**

Curagent analyzes real estate title documents — deeds, title commitments, mortgages, closing disclosures, surveys, payoff letters, HOA estoppels, and full closing packages — and returns a structured risk report with a composite score, individual findings, and AI-generated cure guidance.

This MCP server lets AI agents and MCP-compatible clients (Claude, Cursor, and others) call Curagent directly.

> **Coverage:** Curagent currently supports **Florida** real estate transactions. Additional states are actively on the roadmap. Properties outside the supported region return a clear "out of scope" response — and are never charged.

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
Returns what Curagent currently supports — supported states, document types, and pricing. No API key required. Call this first to confirm the property's state is in scope.

### `get_credit_balance`
Returns the caller's remaining credit balance and tier. Requires an API key. Call this before analyzing to confirm available usage.

### `analyze_title_documents`
Analyzes one or more title documents and returns a structured risk report (composite score, findings, and cure guidance). Requires an API key.

- **Input:** one or more PDF documents, each as a base64-encoded string.
- **Cost:** uses one analysis from your free monthly allowance (sandbox tier) or 1 credit (paid tiers).
- **Scope:** Florida properties only. Out-of-scope submissions return an "out of scope" response and are not charged.

---

## How it works

Curagent's risk engine evaluates a closing package the way a title professional would — checking for issues like unresolved liens, undisclosed easements, encroachments, probate and authority questions, legal description mismatches, expired documents, and party-name inconsistencies — then returns a scored, itemized report with guidance on how to cure each finding.

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
