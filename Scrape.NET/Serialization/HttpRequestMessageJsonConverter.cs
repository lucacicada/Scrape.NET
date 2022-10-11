namespace Scrape.NET.Serialization;

using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
///     Represents a <see cref="HttpRequestMessage"/> <see cref="JsonConverter{T}"/>.
/// </summary>
public sealed class HttpRequestMessageJsonConverter : JsonConverter<HttpRequestMessage>
{
    /// <inheritdoc />
    public override HttpRequestMessage? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var proxy = JsonSerializer.Deserialize<SerializableHttpRequestMessage>(ref reader, options);

        if (proxy is null)
            return null;

        return proxy.ToHttpRequestMessage();
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, HttpRequestMessage value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, new SerializableHttpRequestMessage(value), options);
    }
}
