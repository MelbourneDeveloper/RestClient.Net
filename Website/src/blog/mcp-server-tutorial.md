---
layout: layouts/blog.njk
title: Building AI-Powered Applications with MCP Servers
date: 2026-01-10
author: Christian Findlay
excerpt: Learn how to turn your REST APIs into AI-accessible tools using RestClient.Net's MCP Server generator.
tags: posts
---

# Building AI-Powered Applications with MCP Servers

The Model Context Protocol (MCP) is revolutionizing how AI assistants interact with external systems. With RestClient.Net, you can generate MCP servers from your existing OpenAPI specifications, enabling Claude Code to call your APIs directly.

## What is MCP?

MCP (Model Context Protocol) is a standardized way for AI assistants to:

- Call external APIs as "tools"
- Access data sources securely
- Perform actions on behalf of users

Think of it as giving Claude Code hands to interact with the real world.

## The Power of Code Generation

Instead of manually writing MCP server code, RestClient.Net generates it from your OpenAPI spec:

```
OpenAPI Spec → RestClient.Net Generator → Type-Safe C# Client
     ↓
MCP Generator → MCP Server → Claude Code Tools
```

Every change to your API spec automatically updates your AI tools.

## Real-World Example: E-Commerce API

Let's build an MCP server for a simple e-commerce API.

### The OpenAPI Spec

```yaml
openapi: 3.0.3
info:
  title: E-Commerce API
  version: 1.0.0
paths:
  /products/search:
    get:
      operationId: searchProducts
      summary: Search for products by keyword
      parameters:
        - name: query
          in: query
          required: true
          schema:
            type: string
      responses:
        '200':
          description: Search results
          content:
            application/json:
              schema:
                type: array
                items:
                  $ref: '#/components/schemas/Product'

  /products/{id}:
    get:
      operationId: getProduct
      summary: Get product details
      parameters:
        - name: id
          in: path
          required: true
          schema:
            type: string
      responses:
        '200':
          description: Product details
          content:
            application/json:
              schema:
                $ref: '#/components/schemas/Product'

  /orders:
    post:
      operationId: createOrder
      summary: Place a new order
      requestBody:
        required: true
        content:
          application/json:
            schema:
              $ref: '#/components/schemas/CreateOrderRequest'
      responses:
        '201':
          description: Order created
          content:
            application/json:
              schema:
                $ref: '#/components/schemas/Order'

components:
  schemas:
    Product:
      type: object
      properties:
        id:
          type: string
        name:
          type: string
        price:
          type: number
        inStock:
          type: boolean

    CreateOrderRequest:
      type: object
      required: [productId, quantity]
      properties:
        productId:
          type: string
        quantity:
          type: integer

    Order:
      type: object
      properties:
        id:
          type: string
        status:
          type: string
        total:
          type: number
```

### Generate the Client

```bash
restclient-gen \
  --input ecommerce.yaml \
  --output Generated \
  --namespace ECommerce.Client
```

### Generate the MCP Server

```bash
restclient-mcp \
  --openapi ecommerce.yaml \
  --output McpServer \
  --namespace ECommerce.Mcp \
  --server-name ECommerceServer \
  --client-namespace ECommerce.Client
```

### Configure Claude Code

Add to `.claude/mcp.json`:

```json
{
  "mcpServers": {
    "ecommerce": {
      "command": "dotnet",
      "args": ["run", "--project", "ECommerce.McpServer"],
      "env": {
        "API_BASE_URL": "https://api.mystore.com",
        "API_KEY": "your-api-key"
      }
    }
  }
}
```

## Claude Code in Action

Now Claude can interact with your API naturally:

**User:** "Find me a laptop under $1000"

**Claude:** *calls search_products with query="laptop"*

"I found 3 laptops under $1000:
1. ThinkPad E14 - $799
2. HP Pavilion - $649
3. Acer Aspire 5 - $549

Would you like more details on any of these?"

**User:** "Tell me more about the ThinkPad"

**Claude:** *calls get_product with id="thinkpad-e14"*

"The ThinkPad E14 features:
- 14" FHD display
- Intel Core i5-1235U
- 16GB RAM
- 512GB SSD
- Price: $799
- In Stock: Yes

Would you like to order one?"

**User:** "Yes, order 1"

**Claude:** *calls create_order with productId="thinkpad-e14", quantity=1*

"Order placed successfully! Your order #12345 for the ThinkPad E14 has been confirmed. Total: $799."

## Security Considerations

### Authentication

Always authenticate API calls:

```csharp
services.AddHttpClient("api", client =>
{
    client.BaseAddress = new Uri(
        Environment.GetEnvironmentVariable("API_BASE_URL")!);
})
.AddHttpMessageHandler<ApiKeyHandler>();

public class ApiKeyHandler : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var apiKey = Environment.GetEnvironmentVariable("API_KEY");
        request.Headers.Add("X-API-Key", apiKey);
        return base.SendAsync(request, cancellationToken);
    }
}
```

### Rate Limiting

Protect your API from excessive calls:

```csharp
services.AddHttpClient("api")
    .AddPolicyHandler(Policy.RateLimitAsync<HttpResponseMessage>(
        numberOfExecutions: 10,
        perTimeSpan: TimeSpan.FromSeconds(1)
    ));
```

### Input Validation

The generated tools validate inputs, but add extra validation for sensitive operations:

```csharp
public async Task<ToolResult> ExecuteAsync(JsonElement arguments, CancellationToken ct)
{
    var quantity = arguments.GetProperty("quantity").GetInt32();

    if (quantity > 100)
    {
        return ToolResult.Error("Maximum order quantity is 100");
    }

    // Proceed with order
}
```

## Advanced: Custom Tool Descriptions

Enhance tool descriptions for better AI understanding:

```yaml
paths:
  /products/search:
    get:
      operationId: searchProducts
      summary: Search for products by keyword
      description: |
        Searches the product catalog using full-text search.
        Returns up to 20 results sorted by relevance.
        Use specific keywords for better results (e.g., "laptop 16GB RAM" instead of just "computer").
```

## Debugging MCP Servers

### Local Testing

Run the server locally:

```bash
dotnet run --project ECommerce.McpServer
```

Use the MCP inspector:

```bash
npx @anthropic-ai/mcp-inspector
```

### Logging

Add logging to track Claude's requests:

```csharp
public async Task<ToolResult> ExecuteAsync(JsonElement arguments, CancellationToken ct)
{
    _logger.LogInformation(
        "Tool {ToolName} called with arguments: {Arguments}",
        Name,
        arguments.GetRawText()
    );

    // Execute tool...

    _logger.LogInformation(
        "Tool {ToolName} completed successfully",
        Name
    );

    return result;
}
```

## Conclusion

MCP servers powered by RestClient.Net let you:

1. **Generate AI tools from OpenAPI specs** - No manual coding
2. **Maintain type safety** - Result types and exhaustiveness checking
3. **Keep tools in sync** - Regenerate when specs change
4. **Build powerful AI applications** - Claude can interact with any REST API

The future of AI is about giving models access to real-world systems. With RestClient.Net and MCP, you're ready to build that future.

## Next Steps

- Read the [MCP Server documentation](/docs/mcp/)
- Explore the [OpenAPI Generator](/docs/openapi/)
- Check out the [API Reference](/api/)
