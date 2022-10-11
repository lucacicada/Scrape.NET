namespace Scrape.NET.Serialization;

using System;
using System.Net;
using System.Net.Http;

/// <summary>
///     Represents a serializable <see cref="HttpResponseMessage"/>.
/// </summary>
public sealed class SerializableHttpResponseMessage
{
    /// <summary>
    ///     Gets or sets the HTTP message version.
    /// </summary>
    public string? Version { get; set; }

    /// <summary>
    ///     Gets or sets the status code of the HTTP response.
    /// </summary>
    public HttpStatusCode StatusCode { get; set; }

    /// <summary>
    ///     Gets or sets the reason phrase which typically is sent by servers together with the status code.
    /// </summary>
    public string? ReasonPhrase { get; set; }

    /// <summary>
    ///      Gets or sets the collection of HTTP response content headers.
    /// </summary>
    public SerializableHttpHeaders? ContentHeaders { get; set; }

    /// <summary>
    ///      Gets or sets the collection of HTTP response headers.
    /// </summary>
    public SerializableHttpHeaders? Headers { get; set; }

    /// <summary>
    ///      Gets or sets the collection of trailing headers included in an HTTP response.
    /// </summary>
    public SerializableHttpHeaders? TrailingHeaders { get; set; }

    /// <summary>
    ///     Gets or sets the request message which led to this response message.
    /// </summary>
    public SerializableHttpRequestMessage? RequestMessage { get; set; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="SerializableHttpResponseMessage"/> class.
    /// </summary>
    public SerializableHttpResponseMessage()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="SerializableHttpResponseMessage"/> class.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="responseMessage"/> is null.</exception>
    public SerializableHttpResponseMessage(HttpResponseMessage responseMessage)
    {
        if (responseMessage is null) throw new ArgumentNullException(nameof(responseMessage));

        Version = responseMessage.Version.ToString(2);
        StatusCode = responseMessage.StatusCode;
        ReasonPhrase = responseMessage.ReasonPhrase;
        ContentHeaders = responseMessage.Content?.Headers is null ? null : new SerializableHttpHeaders(responseMessage.Content.Headers);
        Headers = responseMessage.Headers is null ? null : new SerializableHttpHeaders(responseMessage.Headers);
        TrailingHeaders = responseMessage.TrailingHeaders is null ? null : new SerializableHttpHeaders(responseMessage.TrailingHeaders);
        RequestMessage = responseMessage.RequestMessage is null ? null : new SerializableHttpRequestMessage(responseMessage.RequestMessage);
    }

    /// <summary>
    ///     Copy this instance into a new <see cref="HttpResponseMessage"/> instance.
    /// </summary>
    public HttpResponseMessage ToHttpResponseMessage()
    {
        HttpResponseMessage responseMessage = new();
        ToHttpRequestMessage(responseMessage);
        return responseMessage;
    }

    /// <summary>
    ///     Copy this instance into the specified <see cref="HttpResponseMessage"/> instance.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="responseMessage"/> is null.</exception>
    public void ToHttpRequestMessage(HttpResponseMessage responseMessage)
    {
        if (responseMessage is null) throw new ArgumentNullException(nameof(responseMessage));

        if (System.Version.TryParse(Version, out var v)) responseMessage.Version = v;
        responseMessage.StatusCode = StatusCode;
        responseMessage.ReasonPhrase = ReasonPhrase;
        ContentHeaders?.CopyTo(responseMessage.Content.Headers);
        Headers?.CopyTo(responseMessage.Headers);
        TrailingHeaders?.CopyTo(responseMessage.TrailingHeaders);
        responseMessage.RequestMessage = RequestMessage?.ToHttpRequestMessage();
    }
}
