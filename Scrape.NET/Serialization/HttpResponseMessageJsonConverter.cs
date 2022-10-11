namespace Scrape.NET.Serialization;

using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
///     Represents a <see cref="HttpResponseMessage"/> <see cref="JsonConverter{T}"/>.
/// </summary>
public sealed class HttpResponseMessageJsonConverter : JsonConverter<HttpResponseMessage>
{
    /// <inheritdoc />
    public override HttpResponseMessage? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var proxy = JsonSerializer.Deserialize<SerializableHttpResponseMessage>(ref reader, options);

        if (proxy is null)
            return null;

        return proxy.ToHttpResponseMessage();
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, HttpResponseMessage value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, new SerializableHttpResponseMessage(value), options);
    }
}
