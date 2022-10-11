namespace Scrape.NET;

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Html.Dom;
using AngleSharp.Io;
using AngleSharp.Xml.Dom;
using HttpMethod = System.Net.Http.HttpMethod;

/// <summary>
///     Represents set of extensions methods to interact with HTTP requests.
/// </summary>
public static class HttpExtensions
{
    #region EnsureContentType

    /// <summary>
    ///     Use <see cref="System.Net.Mime.MediaTypeNames" /> or <see cref="MimeTypeNames"/>.
    /// </summary>
    public static void EnsureContentType(this HttpResponseMessage response, string mediaType)
    {
        if (response is null) throw new ArgumentNullException(nameof(response));

        response.Content.Headers.EnsureContentType(mediaType);
    }

    /// <summary>
    ///     Use <see cref="System.Net.Mime.MediaTypeNames" /> or <see cref="MimeTypeNames"/>.
    /// </summary>
    public static void EnsureContentType(this HttpContent content, string mediaType)
    {
        if (content is null) throw new ArgumentNullException(nameof(content));

        content.Headers.EnsureContentType(mediaType);
    }

    /// <summary>
    ///     Use <see cref="System.Net.Mime.MediaTypeNames" /> or <see cref="MimeTypeNames"/>.
    /// </summary>
    public static void EnsureContentType(this HttpContentHeaders headers, string mediaType)
    {
        if (headers is null) throw new ArgumentNullException(nameof(headers));

        var responseMediaType = headers.ContentType?.MediaType;

        if (responseMediaType != mediaType)
        {
            throw new ContentTypeMismatchException(null, mediaType, responseMediaType);
        }
    }

    /// <summary>
    ///     Use <see cref="System.Net.Mime.MediaTypeNames" /> or <see cref="MimeTypeNames"/>.
    /// </summary>
    public static void EnsureContentType(this HttpResponseMessage response, params string[] mediaTypes)
    {
        if (response is null) throw new ArgumentNullException(nameof(response));

        response.Content.Headers.EnsureContentType(mediaTypes);
    }

    /// <summary>
    ///     Use <see cref="System.Net.Mime.MediaTypeNames" /> or <see cref="MimeTypeNames"/>.
    /// </summary>
    public static void EnsureContentType(this HttpContent content, params string[] mediaTypes)
    {
        if (content is null) throw new ArgumentNullException(nameof(content));

        content.Headers.EnsureContentType(mediaTypes);
    }

    /// <summary>
    ///     Use <see cref="System.Net.Mime.MediaTypeNames" /> or <see cref="MimeTypeNames"/>.
    /// </summary>
    public static void EnsureContentType(this HttpContentHeaders headers, params string[] mediaTypes)
    {
        if (headers is null) throw new ArgumentNullException(nameof(headers));

        var responseMediaType = headers.ContentType?.MediaType;

        foreach (string item in mediaTypes)
        {
            // we have a match!
            if (responseMediaType == item)
            {
                return;
            }
        }

        throw new ContentTypeMismatchException(null, string.Join(", ", mediaTypes), responseMediaType);
    }

    #endregion

    /// <summary>
    ///     Creates a new <see cref="IHtmlDocument"/>.
    /// </summary>
    /// <param name="response">The response.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <exception cref="ArgumentNullException"><paramref name="response"/> is null.</exception>
    /// <exception cref="ContentTypeMismatchException">The response ContentType must be "text/html" or "application/xhtml+xml"</exception>
    public static async Task<IHtmlDocument> ReadAsHtmlAsync(this HttpResponseMessage response, CancellationToken cancellationToken = default)
    {
        if (response is null) throw new ArgumentNullException(nameof(response));

        // see: https://github.com/AngleSharp/AngleSharp/blob/devel/src/AngleSharp/Dom/DefaultDocumentFactory.cs
        var responseMediaType = response.Content.Headers.ContentType?.MediaType;
        if (responseMediaType != MimeTypeNames.Html && responseMediaType != MimeTypeNames.ApplicationXHtml)
        {
            throw new ContentTypeMismatchException(null, $"{MimeTypeNames.Html}, {MimeTypeNames.ApplicationXHtml}", responseMediaType);
        }

        var documentResponse = await DocumentFactory.CreateResponse(response, cancellationToken).ConfigureAwait(false);

        var document = await BrowsingContext.New(new Configuration().WithXml().WithXPath()).OpenAsync(documentResponse, cancellationToken).ConfigureAwait(false);

        return (IHtmlDocument)document;
    }

    /// <summary>
    ///     Creates a new <see cref="IXmlDocument"/>.
    /// </summary>
    /// <param name="response">The response.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <exception cref="ArgumentNullException"><paramref name="response"/> is null.</exception>
    /// <exception cref="ContentTypeMismatchException">The response ContentType must be "text/xml" or "application/xml" or "image/svg+xml"</exception>
    public static async Task<IXmlDocument> ReadAsXmlAsync(this HttpResponseMessage response, CancellationToken cancellationToken = default)
    {
        if (response is null) throw new ArgumentNullException(nameof(response));

        // see: https://github.com/AngleSharp/AngleSharp.Xml/blob/devel/src/AngleSharp.Xml/XmlConfigurationExtensions.cs
        var responseMediaType = response.Content.Headers.ContentType?.MediaType;
        if (responseMediaType != MimeTypeNames.Xml && responseMediaType != MimeTypeNames.ApplicationXml && responseMediaType != MimeTypeNames.Svg)
        {
            throw new ContentTypeMismatchException(null, $"{MimeTypeNames.Xml}, {MimeTypeNames.ApplicationXml}, {MimeTypeNames.Svg}", responseMediaType);
        }

        var documentResponse = await DocumentFactory.CreateResponse(response, cancellationToken).ConfigureAwait(false);

        var document = await BrowsingContext.New(new Configuration().WithXml().WithXPath()).OpenAsync(documentResponse, cancellationToken).ConfigureAwait(false);

        return (IXmlDocument)document;
    }

    /// <summary>
    ///     Creates a new <see cref="JsonDocument"/>.
    /// </summary>
    /// <param name="response">The response.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <exception cref="ArgumentNullException"><paramref name="response"/> is null.</exception>
    /// <exception cref="ContentTypeMismatchException">The response ContentType must be "application/json"</exception>
    public static async Task<JsonDocument> ReadAsJsonAsync(this HttpResponseMessage response, CancellationToken cancellationToken = default)
    {
        // do not use ReadAsStreamAsync it will not use the proper encoding
        var contentString = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        var responseMediaType = response.Content.Headers.ContentType?.MediaType;
        if (responseMediaType != MimeTypeNames.ApplicationJson)
        {
            throw new ContentTypeMismatchException(null, MimeTypeNames.ApplicationJson, responseMediaType);
        }

        // do not be too strict when parsing the json
        return JsonDocument.Parse(contentString, new JsonDocumentOptions()
        {
            AllowTrailingCommas = true,
            CommentHandling = JsonCommentHandling.Skip
        });
    }

    /// <summary>
    ///     Send an HTTP request as an asynchronous operation.
    /// </summary>
    public static async Task<DefaultResponse> SendAsync(this HttpClient client, Request request)
    {
        if (client is null) throw new ArgumentNullException(nameof(client));
        if (request is null) throw new ArgumentNullException(nameof(request));

        var requestMessage = DocumentFactory.CreateHttpRequestMessage(request);
        var responseMessage = await client.SendAsync(requestMessage).ConfigureAwait(false);
        var response = await DocumentFactory.CreateResponse(responseMessage).ConfigureAwait(false);

        return response;
    }
    /// <summary>
    ///     Send an HTTP request as an asynchronous operation.
    /// </summary>
    public static async Task<DefaultResponse> SendAsync(this HttpClient client, Request request, CancellationToken cancellationToken)
    {
        if (client is null) throw new ArgumentNullException(nameof(client));
        if (request is null) throw new ArgumentNullException(nameof(request));

        var requestMessage = DocumentFactory.CreateHttpRequestMessage(request);
        var responseMessage = await client.SendAsync(requestMessage, cancellationToken).ConfigureAwait(false);
        var response = await DocumentFactory.CreateResponse(responseMessage, cancellationToken).ConfigureAwait(false);

        return response;
    }
    /// <summary>
    ///     Send an HTTP request as an asynchronous operation.
    /// </summary>
    public static async Task<DefaultResponse> SendAsync(this HttpClient client, Request request, HttpCompletionOption completionOption)
    {
        if (client is null) throw new ArgumentNullException(nameof(client));
        if (request is null) throw new ArgumentNullException(nameof(request));

        var requestMessage = DocumentFactory.CreateHttpRequestMessage(request);
        var responseMessage = await client.SendAsync(requestMessage, completionOption).ConfigureAwait(false);
        var response = await DocumentFactory.CreateResponse(responseMessage).ConfigureAwait(false);

        return response;
    }
    /// <summary>
    ///     Send an HTTP request as an asynchronous operation.
    /// </summary>
    public static async Task<DefaultResponse> SendAsync(this HttpClient client, Request request, HttpCompletionOption completionOption, CancellationToken cancellationToken)
    {
        if (client is null) throw new ArgumentNullException(nameof(client));
        if (request is null) throw new ArgumentNullException(nameof(request));

        var requestMessage = DocumentFactory.CreateHttpRequestMessage(request);
        var responseMessage = await client.SendAsync(requestMessage, completionOption, cancellationToken).ConfigureAwait(false);
        var response = await DocumentFactory.CreateResponse(responseMessage, cancellationToken).ConfigureAwait(false);

        return response;
    }

    /// <summary>
    ///     Send an HTTP request as an asynchronous operation.
    /// </summary>
    public static async Task<T> SendAsync<T>(this HttpClient client, Request request) =>
        await SendAsync<T>(client, request, CancellationToken.None);
    /// <summary>
    ///     Send an HTTP request as an asynchronous operation.
    /// </summary>
    public static async Task<T> SendAsync<T>(this HttpClient client, HttpRequestMessage request) =>
        await SendAsync<T>(client, request, CancellationToken.None);
    /// <summary>
    ///     Send an HTTP request as an asynchronous operation.
    /// </summary>
    public static async Task<T> SendAsync<T>(this HttpClient client, Request request, CancellationToken cancellationToken)
    {
        if (client is null) throw new ArgumentNullException(nameof(client));
        if (request is null) throw new ArgumentNullException(nameof(request));

        var requestMessage = DocumentFactory.CreateHttpRequestMessage(request);

        return await SendAsync<T>(client, requestMessage, cancellationToken).ConfigureAwait(false);
    }
    /// <summary>
    ///     Send an HTTP request as an asynchronous operation.
    /// </summary>
    public static async Task<T> SendAsync<T>(this HttpClient client, HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (client is null) throw new ArgumentNullException(nameof(client));
        if (request is null) throw new ArgumentNullException(nameof(request));

        if (typeof(T) == typeof(IHtmlDocument))
        {
            var responseMessage = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
            var html = await ReadAsHtmlAsync(responseMessage, cancellationToken).ConfigureAwait(false);
            return (T)(object)html;
        }
        else if (typeof(T) == typeof(IXmlDocument))
        {
            var responseMessage = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
            var xml = await ReadAsXmlAsync(responseMessage, cancellationToken).ConfigureAwait(false);
            return (T)(object)xml;
        }
        else if (typeof(T) == typeof(JsonDocument))
        {
            var responseMessage = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
            var json = await ReadAsJsonAsync(responseMessage, cancellationToken).ConfigureAwait(false);
            return (T)(object)json;
        }
        else if (typeof(T) == typeof(string))
        {
            var responseMessage = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
            var text = await responseMessage.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            return (T)(object)text;
        }
        else if (typeof(T) == typeof(DefaultResponse) || typeof(T) == typeof(IResponse))
        {
            var responseMessage = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
            var response = await DocumentFactory.CreateResponse(responseMessage, cancellationToken).ConfigureAwait(false);
            return (T)(object)response;
        }
        else if (typeof(T) == typeof(HttpResponseMessage))
        {
            var responseMessage = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
            return (T)(object)responseMessage;
        }
        else
        {
            throw new InvalidOperationException();
        }
    }


    /// <summary>
    ///      Send a GET request as an asynchronous operation.
    /// </summary>
    public static async Task<T> GetAsync<T>(this HttpClient client, string? requestUri) =>
        await SendAsync<T>(client, new HttpRequestMessage(HttpMethod.Get, requestUri), CancellationToken.None);
    /// <summary>
    ///      Send a GET request as an asynchronous operation.
    /// </summary>
    public static async Task<T> GetAsync<T>(this HttpClient client, Uri? requestUri) =>
        await SendAsync<T>(client, new HttpRequestMessage(HttpMethod.Get, requestUri), CancellationToken.None);
    /// <summary>
    ///      Send a GET request as an asynchronous operation.
    /// </summary>
    public static async Task<T> GetAsync<T>(this HttpClient client, string? requestUri, CancellationToken cancellationToken) =>
        await SendAsync<T>(client, new HttpRequestMessage(HttpMethod.Get, requestUri), cancellationToken);
    /// <summary>
    ///      Send a GET request as an asynchronous operation.
    /// </summary>
    public static async Task<T> GetAsync<T>(this HttpClient client, Uri? requestUri, CancellationToken cancellationToken) =>
        await SendAsync<T>(client, new HttpRequestMessage(HttpMethod.Get, requestUri), cancellationToken);

    /// <summary>
    ///      Send a POST request as an asynchronous operation.
    /// </summary>
    public static async Task<T> PostAsync<T>(this HttpClient client, string? requestUri) =>
        await SendAsync<T>(client, new HttpRequestMessage(HttpMethod.Post, requestUri), CancellationToken.None);
    /// <summary>
    ///      Send a POST request as an asynchronous operation.
    /// </summary>
    public static async Task<T> PostAsync<T>(this HttpClient client, Uri? requestUri) =>
        await SendAsync<T>(client, new HttpRequestMessage(HttpMethod.Post, requestUri), CancellationToken.None);
    /// <summary>
    ///      Send a POST request as an asynchronous operation.
    /// </summary>
    public static async Task<T> PostAsync<T>(this HttpClient client, string? requestUri, CancellationToken cancellationToken) =>
        await SendAsync<T>(client, new HttpRequestMessage(HttpMethod.Post, requestUri), cancellationToken);
    /// <summary>
    ///      Send a POST request as an asynchronous operation.
    /// </summary>
    public static async Task<T> PostAsync<T>(this HttpClient client, Uri? requestUri, CancellationToken cancellationToken) =>
        await SendAsync<T>(client, new HttpRequestMessage(HttpMethod.Post, requestUri), cancellationToken);

    /// <summary>
    ///      Send a PUT request as an asynchronous operation.
    /// </summary>
    public static async Task<T> PutAsync<T>(this HttpClient client, string? requestUri) =>
        await SendAsync<T>(client, new HttpRequestMessage(HttpMethod.Put, requestUri), CancellationToken.None);
    /// <summary>
    ///      Send a PUT request as an asynchronous operation.
    /// </summary>
    public static async Task<T> PutAsync<T>(this HttpClient client, Uri? requestUri) =>
        await SendAsync<T>(client, new HttpRequestMessage(HttpMethod.Put, requestUri), CancellationToken.None);
    /// <summary>
    ///      Send a PUT request as an asynchronous operation.
    /// </summary>
    public static async Task<T> PutAsync<T>(this HttpClient client, string? requestUri, CancellationToken cancellationToken) =>
        await SendAsync<T>(client, new HttpRequestMessage(HttpMethod.Put, requestUri), cancellationToken);
    /// <summary>
    ///      Send a PUT request as an asynchronous operation.
    /// </summary>
    public static async Task<T> PutAsync<T>(this HttpClient client, Uri? requestUri, CancellationToken cancellationToken) =>
        await SendAsync<T>(client, new HttpRequestMessage(HttpMethod.Put, requestUri), cancellationToken);

    /// <summary>
    ///      Send a PATCH request as an asynchronous operation.
    /// </summary>
    public static async Task<T> PatchAsync<T>(this HttpClient client, string? requestUri) =>
        await SendAsync<T>(client, new HttpRequestMessage(HttpMethod.Patch, requestUri), CancellationToken.None);
    /// <summary>
    ///      Send a PATCH request as an asynchronous operation.
    /// </summary>
    public static async Task<T> PatchAsync<T>(this HttpClient client, Uri? requestUri) =>
        await SendAsync<T>(client, new HttpRequestMessage(HttpMethod.Patch, requestUri), CancellationToken.None);
    /// <summary>
    ///      Send a PATCH request as an asynchronous operation.
    /// </summary>
    public static async Task<T> PatchAsync<T>(this HttpClient client, string? requestUri, CancellationToken cancellationToken) =>
        await SendAsync<T>(client, new HttpRequestMessage(HttpMethod.Patch, requestUri), cancellationToken);
    /// <summary>
    ///      Send a PATCH request as an asynchronous operation.
    /// </summary>
    public static async Task<T> PatchAsync<T>(this HttpClient client, Uri? requestUri, CancellationToken cancellationToken) =>
        await SendAsync<T>(client, new HttpRequestMessage(HttpMethod.Patch, requestUri), cancellationToken);

    /// <summary>
    ///      Send a DELETE request as an asynchronous operation.
    /// </summary>
    public static async Task<T> DeleteAsync<T>(this HttpClient client, string? requestUri) =>
        await SendAsync<T>(client, new HttpRequestMessage(HttpMethod.Delete, requestUri), CancellationToken.None);
    /// <summary>
    ///      Send a DELETE request as an asynchronous operation.
    /// </summary>
    public static async Task<T> DeleteAsync<T>(this HttpClient client, Uri? requestUri) =>
        await SendAsync<T>(client, new HttpRequestMessage(HttpMethod.Delete, requestUri), CancellationToken.None);
    /// <summary>
    ///      Send a DELETE request as an asynchronous operation.
    /// </summary>
    public static async Task<T> DeleteAsync<T>(this HttpClient client, string? requestUri, CancellationToken cancellationToken) =>
        await SendAsync<T>(client, new HttpRequestMessage(HttpMethod.Delete, requestUri), cancellationToken);
    /// <summary>
    ///      Send a DELETE request as an asynchronous operation.
    /// </summary>
    public static async Task<T> DeleteAsync<T>(this HttpClient client, Uri? requestUri, CancellationToken cancellationToken) =>
        await SendAsync<T>(client, new HttpRequestMessage(HttpMethod.Delete, requestUri), cancellationToken);
}
