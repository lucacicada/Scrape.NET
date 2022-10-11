namespace Scrape.NET.Serialization;

using System;
using System.Net.Http;

/// <summary>
///     Represents a serializable <see cref="HttpRequestMessage"/>.
/// </summary>
public sealed class SerializableHttpRequestMessage
{
    /// <summary>
    ///     Gets or sets the HTTP method used by the HTTP request message.
    /// </summary>
    public string? Method { get; set; }

    /// <summary>
    ///     Gets or sets the System.Uri used for the HTTP request.
    /// </summary>
    public string? RequestUri { get; set; }

    /// <summary>
    ///     Gets or sets the HTTP message version.
    /// </summary>
    public string? Version { get; set; }

    /// <summary>
    ///     Gets or sets the policy that determines how <see cref="Version"/> is interpreted and how the final HTTP version is negotiated with the server.
    /// </summary>
    public HttpVersionPolicy VersionPolicy { get; set; }

    ///// <summary>
    /////     Gets or sets the collection of options to configure the HTTP request.
    ///// </summary>
    //public IDictionary<string, object?>? Options { get; set; }

    /// <summary>
    ///      Gets or sets the collection of HTTP request content headers.
    /// </summary>
    public SerializableHttpHeaders? ContentHeaders { get; set; }

    /// <summary>
    ///      Gets or sets the collection of HTTP request headers.
    /// </summary>
    public SerializableHttpHeaders? Headers { get; set; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="SerializableHttpRequestMessage"/> class.
    /// </summary>
    public SerializableHttpRequestMessage()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="SerializableHttpRequestMessage"/> class.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="requestMessage"/> is null.</exception>
    public SerializableHttpRequestMessage(HttpRequestMessage requestMessage)
    {
        if (requestMessage is null) throw new ArgumentNullException(nameof(requestMessage));

        Method = requestMessage.Method.Method;
        RequestUri = requestMessage.RequestUri?.GetComponents(UriComponents.SerializationInfoString, UriFormat.Unescaped);
        Version = requestMessage.Version.ToString(2);
        VersionPolicy = requestMessage.VersionPolicy;
        // Options = requestMessage.Options;
        ContentHeaders = requestMessage.Content?.Headers is null ? null : new SerializableHttpHeaders(requestMessage.Content.Headers);
        Headers = requestMessage.Headers is null ? null : new SerializableHttpHeaders(requestMessage.Headers);
    }

    /// <summary>
    ///     Copy this instance into a new <see cref="HttpRequestMessage"/> instance.
    /// </summary>
    public HttpRequestMessage ToHttpRequestMessage()
    {
        HttpRequestMessage requestMessage = new();
        ToHttpRequestMessage(requestMessage);
        return requestMessage;
    }

    /// <summary>
    ///     Copy this instance into the specified <see cref="HttpRequestMessage"/> instance.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="requestMessage"/> is null.</exception>
    public void ToHttpRequestMessage(HttpRequestMessage requestMessage)
    {
        if (requestMessage is null) throw new ArgumentNullException(nameof(requestMessage));

        if (Method is not null) requestMessage.Method = new HttpMethod(Method);
        if (RequestUri is not null) requestMessage.RequestUri = new Uri(RequestUri, UriKind.Absolute);
        if (System.Version.TryParse(Version, out var v)) requestMessage.Version = v;
        requestMessage.VersionPolicy = VersionPolicy;
        //if (Options is not null)
        //{
        //    IDictionary<string, object?> options = requestMessage.Options;

        //    options.Clear();
        //    foreach (KeyValuePair<string, object?> kvp in Options)
        //    {
        //        options[kvp.Key] = kvp.Value;
        //    }
        //}
        if (ContentHeaders is not null)
        {
            // we do this since requestMessage.Content is null by default
            requestMessage.Content = new HttpResponseMessage().Content; // HACK: we need EmptyContent to be public
            ContentHeaders.CopyTo(requestMessage.Content.Headers);
        }
        Headers?.CopyTo(requestMessage.Headers);
    }
}
