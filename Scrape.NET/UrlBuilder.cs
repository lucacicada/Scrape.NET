//namespace Scrape.NET;

//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Globalization;
//using System.Linq;
//using System.Text;
//using System.Web;

//public class UrlBuilder
//{
//    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
//    private readonly UriBuilder uriBuilder;

//    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
//    private readonly IList<KeyValuePair<string, string>> query;

//    /// <summary>
//    ///     Initializes a new instance of the <see cref="UrlBuilder"/> class.
//    /// </summary>
//    /// <param name="uri">The base uri.</param>
//    /// <exception cref="ArgumentNullException"><paramref name="uri"/> is null.</exception>
//    /// <exception cref="UriFormatException"><paramref name="uri"/> is not a valid uri.</exception>
//    public UrlBuilder(string uri)
//    {
//        if (uri is null) throw new ArgumentNullException(nameof(uri));

//        // this will throw UriFormatException if the uri is not absolute
//        uriBuilder = new UriBuilder(new Uri(uri, UriKind.Absolute));
//        query = ParseQueryString(uriBuilder.Query);
//    }

//    /// <summary>
//    ///     Initializes a new instance of the <see cref="UrlBuilder"/> class.
//    /// </summary>
//    /// <param name="uri">The base uri.</param>
//    /// <exception cref="ArgumentNullException"><paramref name="uri"/> is null.</exception>
//    /// <exception cref="UriFormatException"><paramref name="uri"/> is not a valid uri.</exception>
//    public UrlBuilder(Uri uri)
//    {
//        if (uri is null) throw new ArgumentNullException(nameof(uri));
//        if (!uri.IsAbsoluteUri) throw new UriFormatException("The URI is not absolute.");

//        uriBuilder = new UriBuilder(uri);
//        query = ParseQueryString(uriBuilder.Query);
//    }

//    /// <summary>
//    ///     Determines if a query parameter is present.
//    /// </summary>
//    public bool Has(string name) => query.Any(q => q.Key == (name ?? string.Empty));

//    /// <summary>
//    ///     Removes a query parameter.
//    /// </summary>
//    public UrlBuilder Remove(string name)
//    {
//        for (int i = query.Count - 1; i >= 0; i--)
//        {
//            if (query[i].Key == (name ?? string.Empty)) query.RemoveAt(i);
//        }

//        return this;
//    }

//    /// <summary>
//    ///     Adds a query parameter.
//    /// </summary>
//    public UrlBuilder Add(string name, string? value)
//    {
//        query.Add(new(name ?? string.Empty, value ?? string.Empty));

//        return this;
//    }

//    /// <summary>
//    ///     Adds a query parameter.
//    /// </summary>
//    public UrlBuilder Add(string name, int value)
//    {
//        return Add(name, value.ToString(CultureInfo.InvariantCulture));
//    }

//    /// <summary>
//    ///     Adds a query parameter.
//    /// </summary>
//    public UrlBuilder Add<T>(string name, T? value)
//    {
//        if (value is IFormattable formattable)
//        {
//            return Add(name, formattable.ToString(null, CultureInfo.InvariantCulture));
//        }
//        else
//        {
//            return Add(name, value?.ToString());
//        }
//    }

//    /// <summary>
//    ///     Sets a query parameter, removing the old one.
//    /// </summary>
//    public UrlBuilder Set(string name, string? value)
//    {
//        for (int i = 0; i < query.Count; i++)
//        {
//            if (query[i].Key == name)
//            {
//                query[i] = new(name ?? string.Empty, value ?? string.Empty);
//            }
//        }

//        return this;
//    }

//    /// <summary>
//    ///     Adds a query parameter.
//    /// </summary>
//    public UrlBuilder Set(string name, int value)
//    {
//        return Set(name, value.ToString(CultureInfo.InvariantCulture));
//    }

//    /// <summary>
//    ///     Sets a query parameter, removing the old one.
//    /// </summary>
//    public UrlBuilder Set<T>(string name, T? value)
//    {
//        if (value is IFormattable formattable)
//        {
//            return Set(name, formattable.ToString(null, CultureInfo.InvariantCulture));
//        }
//        else
//        {
//            return Set(name, value?.ToString());
//        }
//    }

//    /// <summary>
//    ///     Sets the scheme name of the URI.
//    /// </summary>
//    public UrlBuilder Schema(string schema)
//    {
//        uriBuilder.Scheme = schema;
//        return this;
//    }

//    /// <summary>
//    ///     Sets the Domain Name System (DNS) host name or IP address of a server.
//    /// </summary>
//    public UrlBuilder Host(string host)
//    {
//        uriBuilder.Host = host;
//        return this;
//    }

//    /// <summary>
//    ///     Parses a query string using the <see cref="Encoding.UTF8"/> encoding.
//    /// </summary>
//    public static IList<KeyValuePair<string, string>> ParseQueryString(string query) => ParseQueryString(query.AsSpan());

//    /// <summary>
//    ///     Parses a query string using the <see cref="Encoding.UTF8"/> encoding.
//    /// </summary>
//    public static IList<KeyValuePair<string, string>> ParseQueryString(ReadOnlySpan<char> chars) => ParseQueryString(chars, Encoding.UTF8);

//    /// <summary>
//    ///     Parses a query string.
//    /// </summary>
//    public static IList<KeyValuePair<string, string>> ParseQueryString(ReadOnlySpan<char> chars, Encoding encoding)
//    {
//        encoding ??= Encoding.UTF8;

//        // if a query is "&" it return two empty keys
//        // the reason is the query is empty and then empty again (''&'')

//        List<KeyValuePair<string, string>> result = new();

//        int queryLength = chars.Length;

//        if (queryLength is 0)
//        {
//            return result;
//        }

//        int namePos = chars[0] == '?' ? 1 : 0;

//        // fancy way of say if the name pos is 1 and the query len is 1, we have found nothing
//        if (queryLength == namePos)
//        {
//            return result;
//        }

//        while (namePos <= queryLength)
//        {
//            int valuePos = -1, valueEnd = -1;
//            for (int q = namePos; q < queryLength; q++)
//            {
//                if (valuePos == -1 && chars[q] == '=')
//                {
//                    valuePos = q + 1;
//                }
//                else if (chars[q] == '&')
//                {
//                    valueEnd = q;
//                    break;
//                }
//            }

//            string? name;
//            if (valuePos == -1)
//            {
//                name = string.Empty;
//                valuePos = namePos;
//            }
//            else
//            {
//                name = UrlDecode(chars.Slice(namePos, valuePos - namePos - 1), encoding);
//            }

//            if (valueEnd < 0)
//            {
//                valueEnd = chars.Length;
//            }

//            namePos = valueEnd + 1;
//            string value = UrlDecode(chars[valuePos..valueEnd], encoding);
//            result.Add(new(name, value));
//        }

//        return result;
//    }

//    private static string UrlDecode(ReadOnlySpan<char> chars, Encoding encoding)
//    {
//        // UrlEncoder.Default.Encode()

//        if (chars.Length is 0)
//        {
//            return string.Empty;
//        }

//        var count = encoding.GetByteCount(chars);
//        if (count is 0)
//        {
//            return string.Empty;
//        }

//        var buffer = new byte[count];
//        count = encoding.GetBytes(chars, buffer);
//        return HttpUtility.UrlDecode(buffer, 0, count, encoding);
//    }
//}
