namespace Scrape.NET.Serialization;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using Scrape.NET;

/// <summary>
///     Represents a serializable <see cref="HttpHeaders"/>.
/// </summary>
public sealed class SerializableHttpHeaders : List<KeyValuePair<string, IList<string>>>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="SerializableHttpHeaders"/> class.
    /// </summary>
    public SerializableHttpHeaders()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="SerializableHttpHeaders"/> class.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="headers"/> is null.</exception>
    public SerializableHttpHeaders(HttpHeaders headers)
    {
        if (headers is null) throw new ArgumentNullException(nameof(headers));

        AddRange(headers.Select(pair => new KeyValuePair<string, IList<string>>(pair.Key, pair.Value.ToList())));
    }

    /// <summary>
    ///     Copy this instance into the specified <see cref="HttpHeaders"/> instance.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="headers"/> is null.</exception>
    public void CopyTo(HttpHeaders headers)
    {
        if (headers is null) throw new ArgumentNullException(nameof(headers));

        foreach (var item in this)
        {
            _ = headers.TryAddWithoutValidation(item.Key, item.Value);
        }
    }
}
