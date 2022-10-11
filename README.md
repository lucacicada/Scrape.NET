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
- [FileStreamContent](#filestreamcontent)
- [HttpResponseMessage Serialization to JSON](#httpresponsemessage-serialization-to-json)
- [Uber-fast concurrent Console.Write over the current line](#uber-fast-concurrent-consolewrite-over-the-current-line)
  - [Prevent the blinking of the console](#prevent-the-blinking-of-the-console)
- [Hide the cursor](#hide-the-cursor)
- [Turn off Quick Edit on Windows](#turn-off-quick-edit-on-windows)
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

## FileStreamContent

Sometimes you may need to download a file while still conserving a reference to that operation in the Http library.

```cs
// send a request
HttpResponseMessage responseMessage = await httpClient.SendAsync(..., HttpCompletionOption.ResponseHeadersRead);

// make sure we have a 2xx status code
responseMessage.EnsureSuccessStatusCode();

// create a temp file
string tempFile = Path.GetTempFileName();

using (HttpContent content = responseMessage.Content)
{
    // download the file
    using (FileStream fs = File.OpenWrite(tempFile))
    {
        await responseMessage.Content.CopyToAsync(fs);
    }

    // replace the consumed content with a new HttpContent that points to the file on the disk
    responseMessage.Content = new FileStreamContent(tempFile, deleteOnDispose: false);

    // copy the headers to the new content also
    foreach (KeyValuePair<string, IEnumerable<string>> header in content.Headers)
    {
        responseMessage.Content.Headers.Add(header.Key, header.Value);
    }
}

// we can still use responseMessage.Content for other reasons, for example, down-scaling an image
responseMessage.Content;

// deleteOnDispose is false in this example, so calling dispose will not delete the file
responseMessage.Content.Dispose();

File.Exists(tempFile); // is true
```

## HttpResponseMessage Serialization to JSON

> The response content stream will not be serialized to JSON!!!

You can serialize an instance of HttpResponseMessage in JSON with:

```cs
HttpResponseMessage responseMessage = await httpClient.GetAsync("...");

string json = JsonSerializer.Serialize(responseMessage, new JsonSerializerOptions
{
    WriteIndented = true,
    Converters =
    {
        new HttpResponseMessageJsonConverter(),
    }
});

// HttpResponseMessageJsonConverter works on Deserialize also!
```

`HttpResponseMessageJsonConverter` serialize and deserialize the `HttpResponseMessage`.

The result JSON string:

```json
{
  "Version": "1.1",
  "StatusCode": 200,
  "ReasonPhrase": "OK",
  "ContentHeaders": [
    {
      "Key": "Content-Type",
      "Value": [
        "text/html; charset=utf-8"
      ]
    },
    {
      "Key": "Expires",
      "Value": [
        "Mon, 01 Jan 1990 00:00:00 GMT"
      ]
    }
  ],
  "Headers": [
    {
      "Key": "Cache-Control",
      "Value": [
        "no-store, must-revalidate, no-cache, max-age=0"
      ]
    },
  ],
  "TrailingHeaders": [],
  "RequestMessage": {
    "Method": "GET",
    "RequestUri": "https://...",
    "Version": "1.1",
    "VersionPolicy": 0,
    "ContentHeaders": null,
    "Headers": []
  }
}
```

You can also serialize/deserialize a request with `HttpRequestMessageJsonConverter`.

The library do not hide the implementation details from you:

- [SerializableHttpHeaders](Scrape.NET/Serialization/SerializableHttpHeaders.cs)
- [SerializableHttpRequestMessage](Scrape.NET/Serialization/SerializableHttpRequestMessage.cs)
- [SerializableHttpResponseMessage](Scrape.NET/Serialization/SerializableHttpResponseMessage.cs)

You can manually serialize a request with:

```cs
var serializable = new SerializableHttpRequestMessage(requestMessage);

string json = JsonSerializer.Serialize(serializable);

// get an instance of HttpRequestMessage back
HttpRequestMessage requestMessage = serializable.ToHttpRequestMessage();
```

It is very useful when you wish to store an `HttpRequestMessage` in a format that is not JSON, as it basically clone the request into a class that can be easily serialized.

## Uber-fast concurrent Console.Write over the current line

Sometimes you may wish to rewrite very fast over the current line, in a multithreaded highly concurrent operation, in order to do so, you can use:

```cs
ConsoleEx.ReWrite("Hello World!");
```

`ConsoleEx.ReWrite` normalize the string, prevents it from overflowing the console width, clean the old line properly, and its extraordinarily fast!

It uses `Interlocked.Exchange` behind the scenes so it drops overlapping calls, make sure at the end of your program to use `Console.WriteLine()` to create a clean new line in which you can write.

Writing to the console can be very slow, `ConsoleEx.ReWrite` helps tremendously but is still limited, if you need to rely on the console output for communicating with other programs, its best if you stick with the traditional console, or if you use a logging library.

### Prevent the blinking of the console

`ConsoleEx.ReWrite` does prevent the console from flashing when used exclusively, this is a way you can test your specific system:

```cs
Stopwatch stopwatch = Stopwatch.StartNew();
for (int i = 0; i < 100_000; i++)
{
    ConsoleEx.ReWrite($"{i}");
}
stopwatch.Stop();

Console.WriteLine(); // start from a new line
Console.WriteLine("Time elapsed: {0}", stopwatch.Elapsed);
```

> 99999
> Time elapsed: 00:00:12.6130886

The speed of the operation depends mostly on your hardware.

## Hide the cursor

It can be useful to hide the cursor:

```cs
using (ConsoleEx.HideCursor())
{
    ExecuteSomeCode();
}
```

## Turn off Quick Edit on Windows

Quick Edit is a feature of the windows console to suspend the program execution when the user click on the terminal.

```cs
// disable Quick Edit
ConsoleEx.EnableQuickEdit(false);
```

The code does nothing on a non Windows platform, however it still return false. To check if the current system is Windows, use:

```cs
bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
```

## Example

For now, the only available example is [Scrape.NET.Example](Scrape.NET.Example/Program.cs)

## Caveats

If you plan to use the default `System.Net.Http.HttpClient`, be aware that the `HttpRequestMessage.RequestUri` will change when the http client is configured to follow redirects.

Make sure you keep a copy of the original `HttpRequestMessage.RequestUri` if you need it later, as an example, for a caching system.
