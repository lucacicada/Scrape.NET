namespace Scrape.NET;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.Web;

/// <summary>
///     Provides a way to build an uri query.
/// </summary>
public class QueryBuilder : IEnumerable<KeyValuePair<string?, string[]>>
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly UriBuilder uriBuilder;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly NameValueCollection query;

    /// <summary>
    ///     The constructed uri.
    /// </summary>
    public Uri Uri
    {
        get
        {
            uriBuilder.Query = query.ToString();
            return uriBuilder.Uri;
        }
    }

    /// <summary>
    ///     Gets all the keys.
    /// </summary>
    public string?[] Keys => query.AllKeys;

    /// <summary>
    ///     Initializes a new instance of the <see cref="QueryBuilder"/> class.
    /// </summary>
    /// <param name="uri">The base uri.</param>
    /// <exception cref="ArgumentNullException"><paramref name="uri"/> is null.</exception>
    /// <exception cref="UriFormatException"><paramref name="uri"/> is not a valid uri.</exception>
    public QueryBuilder(string uri)
    {
        if (uri is null) throw new ArgumentNullException(nameof(uri));

        // this will throw UriFormatException if the uri is not absolute
        uriBuilder = new UriBuilder(new Uri(uri, UriKind.Absolute));
        query = HttpUtility.ParseQueryString(uriBuilder.Query); // HttpUtility is kinda obsolete
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="QueryBuilder"/> class.
    /// </summary>
    /// <param name="uri">The base uri.</param>
    /// <exception cref="ArgumentNullException"><paramref name="uri"/> is null.</exception>
    /// <exception cref="UriFormatException"><paramref name="uri"/> is not a valid uri.</exception>
    public QueryBuilder(Uri uri)
    {
        if (uri is null) throw new ArgumentNullException(nameof(uri));
        if (!uri.IsAbsoluteUri) throw new UriFormatException("The URI is not absolute.");

        uriBuilder = new UriBuilder(uri);
        query = HttpUtility.ParseQueryString(uriBuilder.Query); // HttpUtility is kinda obsolete
    }

    /// <summary>
    ///     Determines if a query parameter is present.
    /// </summary>
    public bool Has(string name)
    {
        return query[name] is not null;
    }

    /// <summary>
    ///     Removes a query parameter.
    /// </summary>
    public QueryBuilder Remove(string name)
    {
        query.Remove(name);

        return this;
    }

    /// <summary>
    ///     Adds a query parameter.
    /// </summary>
    public QueryBuilder Add(string name, string? value)
    {
        query.Add(name, value);

        return this;
    }

    /// <summary>
    ///     Adds a query parameter.
    /// </summary>
    public QueryBuilder Add(string name, int value)
    {
        return Add(name, value.ToString(CultureInfo.InvariantCulture));
    }

    /// <summary>
    ///     Adds a query parameter.
    /// </summary>
    public QueryBuilder Add<T>(string name, T? value)
    {
        if (value is IFormattable formattable)
        {
            return Add(name, formattable.ToString(null, CultureInfo.InvariantCulture));
        }
        else
        {
            return Add(name, value?.ToString());
        }
    }

    /// <summary>
    ///     Sets a query parameter, removing the old one.
    /// </summary>
    public QueryBuilder Set(string name, string? value)
    {
        query.Set(name, value);

        return this;
    }

    /// <summary>
    ///     Adds a query parameter.
    /// </summary>
    public QueryBuilder Set(string name, int value)
    {
        return Set(name, value.ToString(CultureInfo.InvariantCulture));
    }

    /// <summary>
    ///     Sets a query parameter, removing the old one.
    /// </summary>
    public QueryBuilder Set<T>(string name, T? value)
    {
        if (value is IFormattable formattable)
        {
            return Set(name, formattable.ToString(null, CultureInfo.InvariantCulture));
        }
        else
        {
            return Set(name, value?.ToString());
        }
    }

    /// <summary>
    ///     Gets a query parameter values, or null if not present.
    /// </summary>
    public string[]? Get(string name)
    {
        return query.GetValues(name);
    }

    /// <summary>
    ///     Removes all query parameters.
    /// </summary>
    public QueryBuilder Clear()
    {
        query.Clear();

        return this;
    }

    /// <summary>
    ///     Returns the constructed absolute uri.
    /// </summary>
    /// <returns>The constructed absolute uri.</returns>
    public override string ToString()
    {
        return Uri.GetComponents(UriComponents.AbsoluteUri, UriFormat.SafeUnescaped);
    }

    /// <inheritdoc />
    IEnumerator<KeyValuePair<string?, string[]>> IEnumerable<KeyValuePair<string?, string[]>>.GetEnumerator()
    {
        return Enumerate().GetEnumerator();
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
        return Enumerate().GetEnumerator();
    }

    IEnumerable<KeyValuePair<string?, string[]>> Enumerate()
    {
        string?[] keys = query.AllKeys;

        foreach (string? key in keys)
        {
            var values = query.GetValues(key);
            if (values is not null) // this should never happen
            {
                yield return new KeyValuePair<string?, string[]>(key, values);
            }
        }
    }
}
