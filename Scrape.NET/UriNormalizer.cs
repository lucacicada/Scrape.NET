namespace Scrape.NET;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
///     Represents a way to normalize an <see cref="Uri"/>.
/// </summary>
public static class UriNormalizer
{
    /// <summary>
    ///     Normalize an <see cref="Uri"/>.
    ///     <list type="number">
    ///         <item>The query parameters are sorted alphabetically.</item>
    ///         <item>The query values are sorted alphabetically.</item>
    ///         <item>The domain is lowercased.</item>
    ///         <item>The default ports are removed.</item>
    ///         <item>The uri is unescaped.</item>
    ///     </list>
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="uri"/> is null.</exception>
    /// <exception cref="ArgumentException">The uri is not absolute.</exception>
    public static string NormalizeUriAsString(Uri uri)
    {
        if (uri is null) throw new ArgumentNullException(nameof(uri));
        if (!uri.IsAbsoluteUri) throw new ArgumentException("The uri is not absolute.", nameof(uri));

        var uriBuilder = new UriBuilder(uri);
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);

        // clone and sort the query parameters
        var list = query
            .AllKeys
            .Select(key => new KeyValuePair<string?, string[]?>(key, query.GetValues(key)?.OrderBy(static _ => _, StringComparer.Ordinal).ToArray()))
            .OrderBy(static key => key.Key, StringComparer.Ordinal)
            .ToList();

        // clear the query
        query.Clear();

        foreach (KeyValuePair<string?, string[]?> parameters in list)
        {
            var values = parameters.Value;

            if (values is not null) // this should never happen
            {
                var key = parameters.Key;

                foreach (string value in values)
                {
                    // ignore empty query key and values
                    if (string.IsNullOrEmpty(key) && string.IsNullOrEmpty(value))
                    {
                        continue;
                    }

                    query.Add(key, value);
                }
            }
        }

        uriBuilder.Query = query.ToString();

        return uriBuilder.Uri.GetComponents(UriComponents.AbsoluteUri, UriFormat.SafeUnescaped);
    }

    /// <inheritdoc cref="NormalizeUriAsString" />
    public static Uri NormalizeUri(Uri uri) => new(NormalizeUriAsString(uri), UriKind.Absolute);
}
