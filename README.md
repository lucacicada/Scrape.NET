# Scrape.NET

A flexible .NET scraping library, with a powerful exceptions system.

## Quick start

This library uses extensions method extensively, do not forget to `using Scrape.NET;`

```cs
using System.Net.Http;
using AngleSharp.Html.Dom;
using Scrape.NET; // <- make sure to use the library

HttpClient httpClient = new HttpClient();

// send a [GET] request and retrive an IHtmlDocument
IHtmlDocument html = await httpClient.GetAsync<IHtmlDocument>("https://...");

// css selector, or fail with NodeNotFoundException
html.CssOrFail("title");
```

## More flexible example

```cs
using System.Net.Http;
using AngleSharp.Html.Dom;
using Scrape.NET; // <- make sure to use the library

// create an http client
HttpClient httpClient = new HttpClient();

// create a request
HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, "https://...");

// send the request but do not actually consume the content, read only the headers
HttpResponseMessage responseMessage = await httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

// ensure the request is any 2xx status
// without ResponseHeadersRead, we read more that what whe actually need
responseMessage.EnsureSuccessStatusCode();

// we can now consume the rest of the response
IHtmlDocument html = await responseMessage.ReadAsHtmlAsync(cancellationToken);

// as an alternative of the line above
{
    // create the AngleSharp IResponse
    IResponse documentResponse = await DocumentFactory.CreateResponse(responseMessage, cancellationToken);

    // use AngleSharp to create the actual document
    IDocument document = await BrowsingContext.New().OpenAsync(documentResponse, cancellationToken);

    IHtmlDocument html = (IHtmlDocument)document;
}

// css selector, or fail with NodeNotFoundException
html.CssOrFail("title");
```

## Table of Contents

- [Quick start](#quick-start)
- [More flexible example](#more-flexible-example)
- [Query Selector Utilities](#query-selector-utilities)
  - [Css and XPath selector](#css-and-xpath-selector)
    - [Sub-select](#sub-select)
  - [Select an attribute](#select-an-attribute)
  - [Get the normalized href attribute](#get-the-normalized-href-attribute)
- [Example](#example)
- [Caveats](#caveats)

## Query Selector Utilities

> All the available extensions methods source code: [NodeExtensions.cs](Scrape.NET/NodeExtensions.cs)

All the utilities automatically decoded, unescape and trim the result, a `&lt; hey` is trimmed and converted to `< hey`.

### Css and XPath selector

```cs
// return the first .container, or null, similar to document.querySelector()
INode node = html.Css(".container");

// throw it a .container does not exists
INode node = html.CssOrFail(".container");

// select all the .container
INodeList nodes = html.CssAll(".container");

// same as before
html.XPath("/container/a[2]");
html.XPathOrFail("/container/a[2]");
html.XPathAll("/container/a");

// or something more complex
html.XPath("/html/body/div[7]/div[1]/div[1]/h2[8]");
```

#### Sub-select

You can select from any node, therefore:

```cs
html.Css("body").CssAll(".container").CssAll("a");

// or
html.Css("body").Css(".container").Css("a");
```

You can also use a css selector:

```cs
html.Css("body .container > a");
```

### Select an attribute

When selecting an attribute, text content, or html content, the result string will be decoded and unescaped automatically, therefore `&lt;` will become `<`.

```cs
// <div class=".container" name="The container name">...

html.CssOrFail(".container").Attr("name"); // -> The container name

html.CssOrFail(".container").Attr("id"); // -> null

// with a default value
html.CssOrFail(".container").Attr("style", "Invalid Style"); // -> Invalid Style
```

If you want to use use AngleSharp, you need to cast the result manually:

```cs
 ((IHtmlElement)html.CssOrFail(".container")).GetAttribute("name");
```

### Get the normalized href attribute

```cs
string href = html.CssOrFail("a").Href();
Uri href = html.CssOrFail("a").Href<Uri>();

// with a default value
string href = html.CssOrFail("a").Href("https://...");
Uri href = html.CssOrFail("a").Href<Uri>("https://...");
```

## Example

For now, the only available example is [Scrape.NET.Example](Scrape.NET.Example/Program.cs)

## Caveats

If you plan to use the default `System.Net.Http.HttpClient`, be aware that the `HttpRequestMessage.RequestUri` will change when the http client is configured to follow redirects.

Make sure you keep a copy of the original `HttpRequestMessage.RequestUri` if you need it later, as an example, for a caching system.
