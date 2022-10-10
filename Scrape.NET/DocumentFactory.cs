namespace Scrape.NET;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Io;
using HttpMethod = System.Net.Http.HttpMethod;

public static class DocumentFactory
{
    public static HttpMethod CreateHttpMethod(AngleSharp.Io.HttpMethod method) => method switch
    {
        AngleSharp.Io.HttpMethod.Get => HttpMethod.Get,
        AngleSharp.Io.HttpMethod.Delete => HttpMethod.Delete,
        AngleSharp.Io.HttpMethod.Post => HttpMethod.Post,
        AngleSharp.Io.HttpMethod.Put => HttpMethod.Put,
        AngleSharp.Io.HttpMethod.Options => HttpMethod.Options,
        AngleSharp.Io.HttpMethod.Head => HttpMethod.Head,
        AngleSharp.Io.HttpMethod.Trace => HttpMethod.Trace,
        AngleSharp.Io.HttpMethod.Connect => new HttpMethod("CONNECT"),
        _ => throw new ArgumentOutOfRangeException(nameof(method), $"The method '{method}' is not a valid HTTP method."),
    };

    /// <summary>
    /// 
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="request"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The <paramref name="request"/> <see cref="Request.Method"/> is not a valid <see cref="AngleSharp.Io.HttpMethod"/>.</exception>
    public static HttpRequestMessage CreateHttpRequestMessage(Request request)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        // from: https://github.com/AngleSharp/AngleSharp.Io/blob/devel/src/AngleSharp.Io/Network/HttpClientRequester.cs

        var method = CreateHttpMethod(request.Method);
        var requestMessage = new HttpRequestMessage(method, request.Address);
        var contentHeaders = new List<KeyValuePair<string, string>>();

        foreach (var header in request.Headers)
        {
            if (!requestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value))
            {
                contentHeaders.Add(new KeyValuePair<string, string>(header.Key, header.Value));
            }
        }

        // set up the content
        if (request.Content is not null && method != HttpMethod.Get && method != HttpMethod.Head)
        {
            requestMessage.Content = new StreamContent(request.Content);

            foreach (var header in contentHeaders)
            {
                _ = requestMessage.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
        }

        return requestMessage;
    }

    /// <summary>
    ///     Use <see cref="BrowsingContextExtensions.OpenAsync(IBrowsingContext, IResponse, CancellationToken)"/>.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="responseMessage"/> is null.</exception>
    public static async Task<DefaultResponse> CreateResponse(HttpResponseMessage responseMessage, CancellationToken cancellationToken = default)
    {
        // from: https://github.com/AngleSharp/AngleSharp.Io/blob/devel/src/AngleSharp.Io/Network/HttpClientRequester.cs

        if (responseMessage is null) throw new ArgumentNullException(nameof(responseMessage));

        var uri = responseMessage.RequestMessage?.RequestUri;

        var response = new DefaultResponse
        {
            Address = uri is null ? new Url("about:blank") : Url.Convert(uri),
            StatusCode = responseMessage.StatusCode
        };

        // set the response headers
        foreach (var header in responseMessage.Headers)
        {
            response.Headers[header.Key] = string.Join(", ", header.Value);
        }

        var content = responseMessage.Content;

        if (content is not null)
        {
            response.Content = await content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);

            // set the response content headers
            foreach (var pair in content.Headers)
            {
                response.Headers[pair.Key] = string.Join(", ", pair.Value);
            }
        }

        if (IsRedirected(response) && !response.Headers.ContainsKey(HeaderNames.SetCookie))
        {
            response.Headers[HeaderNames.SetCookie] = string.Empty;
        }

        return response;
    }

    private static bool IsRedirected(IResponse response)
    {
        return response.StatusCode
            is HttpStatusCode.Redirect
            or HttpStatusCode.RedirectKeepVerb
            or HttpStatusCode.RedirectMethod
            or HttpStatusCode.TemporaryRedirect
            or HttpStatusCode.MovedPermanently
            or HttpStatusCode.MultipleChoices;
    }
}
