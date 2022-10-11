namespace Scrape.NET;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Xml.XPath;
using AngleSharp.Dom;

/// <summary>
///     Extensions methods to interact with an <see cref="INode"/>.
/// </summary>
public static class NodeExtensions
{
    /// <summary>
    ///     Gets the text content of an element and all its descendants.
    ///     The result string is normalized, trimmed, escaped and always not null.
    /// </summary>
    /// <remarks>
    ///     Different from <see cref="AngleSharp.Dom.NodeExtensions.Text(INode)"/>.
    /// </remarks>
    public static string Text(this INode node)
    {
        var text = node?.TextContent;

        if (text is null)
        {
            return string.Empty;
        }

        return Uri.UnescapeDataString(WebUtility.HtmlDecode(text)).Trim();
    }

    /// <summary>
    ///     Gets the text content of an element and all its descendants.
    ///     The result string is normalized, trimmed, escaped and always not null.
    /// </summary>
    /// <remarks>
    ///     Different from <see cref="INode.TextContent"/>.
    /// </remarks>
    public static string TextContent(this INode node) => Text(node);

    /// <summary>
    ///     Gets the outer HTML (including the current element) of the element.
    ///     The result string is normalized, trimmed, escaped and always not null.
    /// </summary>
    /// <remarks>
    ///     Uses <see cref="IElement.OuterHtml"/>.
    /// </remarks>
    public static string Html(this INode node)
    {
        var html = node is IElement el ? el.OuterHtml : null;

        if (html is null)
        {
            return string.Empty;
        }

        return Uri.UnescapeDataString(WebUtility.HtmlDecode(html)).Trim();
    }

    /// <summary>
    ///     Gets the outer HTML (including the current element) of the element.
    ///     The result string is normalized, trimmed, escaped and always not null.
    /// </summary>
    /// <remarks>
    ///     Uses <see cref="IElement.OuterHtml"/>.
    /// </remarks>
    public static string HtmlContent(this INode node) => Html(node);

    /// <summary>
    ///     Gets the inner HTML (excluding the current element) of the element.
    ///     The result string is normalized, trimmed, escaped and always not null.
    /// </summary>
    /// <remarks>
    ///     Uses <see cref="IElement.InnerHtml"/>.
    /// </remarks>
    public static string InnerHtml(this INode node)
    {
        var html = node is IElement el ? el.InnerHtml : null;

        if (html is null)
        {
            return string.Empty;
        }

        return Uri.UnescapeDataString(WebUtility.HtmlDecode(html)).Trim();
    }

    /// <summary>
    ///     Gets the inner HTML (excluding the current element) of the element.
    ///     The result string is normalized, trimmed, escaped and always not null.
    /// </summary>
    /// <remarks>
    ///     Uses <see cref="IElement.InnerHtml"/>.
    /// </remarks>
    public static string InnerHtmlContent(this INode node) => InnerHtml(node);

    #region Attr

    /// <summary>
    ///     Gets an attribute from the element.
    ///     The result text is normalized, trimmed, escaped and always not null.
    /// </summary>
    /// <exception cref="AttributeNotFoundException">The attribute <paramref name="name"/> is not present.</exception>
    /// <remarks>
    ///     Use <see cref="AttributeNames"/>
    /// </remarks>
    public static string Attr(this INode node, string name)
    {
        var value = node is IElement el ? el.Attributes.GetNamedItem(null, name)?.Value : null;

        if (value is null)
        {
            throw new AttributeNotFoundException($"Missing '{name}' attribute.", name);
        }

        return Uri.UnescapeDataString(WebUtility.HtmlDecode(value)).Trim();
    }

    /// <summary>
    ///     Gets an attribute from the element.
    ///     The result text is normalized, trimmed, escaped and always not null.
    /// </summary>
    /// <remarks>
    ///     Use <see cref="AttributeNames"/>
    /// </remarks>
    [return: NotNullIfNotNull(nameof(defaultValue))]
    public static string? Attr(this INode node, string name, string? defaultValue)
    {
        var value = node is IElement el ? el.Attributes.GetNamedItem(null, name)?.Value : null;

        if (value is null)
        {
            return defaultValue;
        }

        return Uri.UnescapeDataString(WebUtility.HtmlDecode(value)).Trim();
    }

    /// <summary>
    ///     Gets an attribute from the element.
    ///     The result text is normalized, trimmed, escaped and always not null.
    /// </summary>
    /// <exception cref="AttributeNotFoundException">The attribute <paramref name="name"/> is not present.</exception>
    /// <remarks>
    ///     Use <see cref="AttributeNames"/>
    /// </remarks>
    public static T Attr<T>(this INode node, string name)
    {
        var value = node is IElement el ? el.Attributes.GetNamedItem(null, name)?.Value : null;

        if (value is null)
        {
            throw new AttributeNotFoundException($"Missing '{name}' attribute.", name);
        }

        value = Uri.UnescapeDataString(WebUtility.HtmlDecode(value)).Trim();

        return ChangeType<T>(value);
    }

    /// <summary>
    ///     Gets an attribute from the element.
    ///     The result text is normalized, trimmed, escaped and always not null.
    /// </summary>
    /// <remarks>
    ///     Use <see cref="AttributeNames"/>
    /// </remarks>
    [return: NotNullIfNotNull(nameof(defaultValue))]
    public static T? Attr<T>(this INode node, string name, T? defaultValue)
    {
        var value = node is IElement el ? el.Attributes.GetNamedItem(null, name)?.Value : null;

        if (value is null)
        {
            return defaultValue;
        }

        value = Uri.UnescapeDataString(WebUtility.HtmlDecode(value)).Trim();

        return ChangeType<T>(value);
    }

    #endregion

    #region Src

    /// <summary>
    ///     Gets the <see cref="AttributeNames.Src"/> attribute from the element.
    ///     The result is the absolute unescaped uri.
    /// </summary>
    /// <exception cref="AttributeNotFoundException">The attribute <see cref="AttributeNames.Src"/> is not present.</exception>
    public static Uri Src(this INode node)
    {
        var value = Attr(node, AttributeNames.Src);

        return GetUri(node, value);
    }

    /// <summary>
    ///     Gets the <see cref="AttributeNames.Src"/> attribute from the element.
    ///     The result is the absolute unescaped uri.
    /// </summary>
    [return: NotNullIfNotNull(nameof(defaultValue))]
    public static Uri? Src(this INode node, Uri? defaultValue)
    {
        var value = Attr(node, AttributeNames.Src, null);

        if (value is null)
        {
            return defaultValue;
        }

        return GetUri(node, value);
    }

    /// <summary>
    ///     Gets the <see cref="AttributeNames.Src"/> attribute from the element.
    ///     The result is the absolute unescaped uri.
    /// </summary>
    /// <exception cref="AttributeNotFoundException">The attribute <see cref="AttributeNames.Src"/> is not present.</exception>
    public static T Src<T>(this INode node)
    {
        var value = Src(node);

        return ChangeType<T>(value.GetComponents(UriComponents.AbsoluteUri, UriFormat.Unescaped));
    }

    /// <summary>
    ///     Gets the <see cref="AttributeNames.Src"/> attribute from the element.
    ///     The result is the absolute unescaped uri.
    /// </summary>
    [return: NotNullIfNotNull(nameof(defaultValue))]
    public static T? Src<T>(this INode node, T? defaultValue)
    {
        var value = Src(node, null);

        if (value is null)
        {
            return defaultValue;
        }

        return ChangeType<T>(value.GetComponents(UriComponents.AbsoluteUri, UriFormat.Unescaped));
    }

    #endregion

    #region Href

    /// <summary>
    ///     Gets the <see cref="AttributeNames.Href"/> attribute from the element.
    ///     The result is the absolute unescaped uri.
    /// </summary>
    /// <exception cref="AttributeNotFoundException">The attribute <see cref="AttributeNames.Href"/> is not present.</exception>
    public static Uri Href(this INode node)
    {
        var value = Attr(node, AttributeNames.Href);

        return GetUri(node, value);
    }

    /// <summary>
    ///     Gets the <see cref="AttributeNames.Href"/> attribute from the element.
    ///     The result is the absolute unescaped uri.
    /// </summary>
    [return: NotNullIfNotNull(nameof(defaultValue))]
    public static Uri? Href(this INode node, Uri? defaultValue)
    {
        var value = Attr(node, AttributeNames.Href, null);

        if (value is null)
        {
            return defaultValue;
        }

        return GetUri(node, value);
    }

    /// <summary>
    ///     Gets the <see cref="AttributeNames.Href"/> attribute from the element.
    ///     The result is the absolute unescaped uri.
    /// </summary>
    /// <exception cref="AttributeNotFoundException">The attribute <see cref="AttributeNames.Href"/> is not present.</exception>
    public static T Href<T>(this INode node)
    {
        var value = Href(node);

        return ChangeType<T>(value.GetComponents(UriComponents.AbsoluteUri, UriFormat.Unescaped));
    }

    /// <summary>
    ///     Gets the <see cref="AttributeNames.Href"/> attribute from the element.
    ///     The result is the absolute unescaped uri.
    /// </summary>
    [return: NotNullIfNotNull(nameof(defaultValue))]
    public static T? Href<T>(this INode node, T? defaultValue)
    {
        var value = Href(node, null);

        if (value is null)
        {
            return defaultValue;
        }

        return ChangeType<T>(value.GetComponents(UriComponents.AbsoluteUri, UriFormat.Unescaped));
    }

    #endregion

    #region Select from INode

    /// <summary>
    ///     Returns the first element that matches the specified Css selector.
    /// </summary>
    public static INode? Css(this INode node, string selector) => Select(node, NodeSelector.Css(selector));

    /// <summary>
    ///     Returns the first element that matches the specified XPath selector.
    /// </summary>
    public static INode? XPath(this INode node, string selector) => Select(node, NodeSelector.XPath(selector));

    /// <summary>
    ///     Returns the first element that matches the specified XPath selector.
    /// </summary>
    public static INode? XPath(this INode node, XPathExpression selector) => Select(node, NodeSelector.XPath(selector));

    /// <summary>
    ///     Returns the first element found by the specified selector.
    /// </summary>
    public static INode? Select(this INode node, NodeSelector selector)
    {
        if (node is IDocument document)
        {
            node = document.DocumentElement;
        }

        return selector.Select(node);
    }

    /// <summary>
    ///     Returns the first element that matches the specified Css selector, or fail.
    /// </summary>
    /// <exception cref="NodeNotFoundException"></exception>
    public static INode CssOrFail(this INode node, string selector) => SelectOrFail(node, NodeSelector.Css(selector));

    /// <summary>
    ///     Returns the first element that matches the specified XPath selector, or fail.
    /// </summary>
    /// <exception cref="NodeNotFoundException"></exception>
    public static INode XPathOrFail(this INode node, string selector) => SelectOrFail(node, NodeSelector.XPath(selector));

    /// <summary>
    ///     Returns the first element that matches the specified XPath selector, or fail.
    /// </summary>
    /// <exception cref="NodeNotFoundException"></exception>
    public static INode XPathOrFail(this INode node, XPathExpression selector) => SelectOrFail(node, NodeSelector.XPath(selector));

    /// <summary>
    ///     Returns the first element found by the specified selector, or fail.
    /// </summary>
    /// <exception cref="NodeNotFoundException"></exception>
    public static INode SelectOrFail(this INode node, NodeSelector selector)
    {
        if (node is IDocument document)
        {
            node = document.DocumentElement;
        }

        var selectedNode = selector.Select(node);

        if (selectedNode is null)
        {
            throw new NodeNotFoundException($"Select '{selector}' not found.", selector.ToString());
        }

        return selectedNode;
    }

    /// <summary>
    ///     Returns all the elements that matches the specified Css selector.
    /// </summary>
    public static INodeList CssAll(this INode node, string selector) => SelectAll(node, NodeSelector.Css(selector));

    /// <summary>
    ///     Returns all the elements that matches the specified XPath selector.
    /// </summary>
    public static INodeList XPathAll(this INode node, string selector) => SelectAll(node, NodeSelector.XPath(selector));

    /// <summary>
    ///     Returns all the elements that matches the specified XPath selector.
    /// </summary>
    public static INodeList XPathAll(this INode node, XPathExpression selector) => SelectAll(node, NodeSelector.XPath(selector));

    /// <summary>
    ///     Returns all the elements found by the specified selector.
    /// </summary>
    public static INodeList SelectAll(this INode node, NodeSelector selector)
    {
        if (node is IDocument document)
        {
            node = document.DocumentElement;
        }

        return new NodeList(selector.SelectAll(node));
    }

    #endregion

    #region Select <T> from INode

    /// <summary>
    ///     Returns the first element that matches the specified Css selector.
    /// </summary>
    public static T? Css<T>(this INode node, string selector) where T : INode => Select<T>(node, NodeSelector.Css(selector));

    /// <summary>
    ///     Returns the first element that matches the specified XPath selector.
    /// </summary>
    public static T? XPath<T>(this INode node, string selector) where T : INode => Select<T>(node, NodeSelector.XPath(selector));

    /// <summary>
    ///     Returns the first element that matches the specified XPath selector.
    /// </summary>
    public static T? XPath<T>(this INode node, XPathExpression selector) where T : INode => Select<T>(node, NodeSelector.XPath(selector));

    /// <summary>
    ///     Returns the first element found by the specified selector.
    /// </summary>
    public static T? Select<T>(this INode node, NodeSelector selector) where T : INode
    {
        if (node is IDocument document)
        {
            node = document.DocumentElement;
        }

        return selector.Select(node) is T t ? t : default;
    }

    /// <summary>
    ///     Returns the first element that matches the specified Css selector, or fail.
    /// </summary>
    /// <exception cref="NodeNotFoundException"></exception>
    public static T CssOrFail<T>(this INode node, string selector) where T : INode => SelectOrFail<T>(node, NodeSelector.Css(selector));

    /// <summary>
    ///     Returns the first element that matches the specified XPath selector, or fail.
    /// </summary>
    /// <exception cref="NodeNotFoundException"></exception>
    public static T XPathOrFail<T>(this INode node, string selector) where T : INode => SelectOrFail<T>(node, NodeSelector.XPath(selector));

    /// <summary>
    ///     Returns the first element that matches the specified XPath selector, or fail.
    /// </summary>
    /// <exception cref="NodeNotFoundException"></exception>
    public static T XPathOrFail<T>(this INode node, XPathExpression selector) where T : INode => SelectOrFail<T>(node, NodeSelector.XPath(selector));

    /// <summary>
    ///     Returns the first element found by the specified selector, or fail.
    /// </summary>
    /// <exception cref="NodeNotFoundException"></exception>
    public static T SelectOrFail<T>(this INode node, NodeSelector selector) where T : INode
    {
        if (node is IDocument document)
        {
            node = document.DocumentElement;
        }

        var selectedNode = selector.Select(node);

        if (selectedNode is null)
        {
            throw new NodeNotFoundException($"Select '{selector}' not found.", selector.ToString());
        }

        return selectedNode is T t ? t : throw new InvalidOperationException();
    }

    /// <summary>
    ///     Returns all the elements that matches the specified Css selector.
    /// </summary>
    public static INodeList CssAll<T>(this INode node, string selector) where T : INode => SelectAll<T>(node, NodeSelector.Css(selector));

    /// <summary>
    ///     Returns all the elements that matches the specified XPath selector.
    /// </summary>
    public static INodeList XPathAll<T>(this INode node, string selector) where T : INode => SelectAll<T>(node, NodeSelector.XPath(selector));

    /// <summary>
    ///     Returns all the elements that matches the specified XPath selector.
    /// </summary>
    public static INodeList XPathAll<T>(this INode node, XPathExpression selector) where T : INode => SelectAll<T>(node, NodeSelector.XPath(selector));

    /// <summary>
    ///     Returns all the elements found by the specified selector.
    /// </summary>
    public static INodeList SelectAll<T>(this INode node, NodeSelector selector) where T : INode
    {
        if (node is IDocument document)
        {
            node = document.DocumentElement;
        }

        return new NodeList((IEnumerable<INode>)selector.SelectAll(node).OfType<T>());
    }

    #endregion

    #region Select from INodeList

    /// <summary>
    ///     Returns the first element that matches the specified Css selector.
    /// </summary>
    public static INode? Css(this IEnumerable<INode> nodes, string selector) => Select(nodes, NodeSelector.Css(selector));

    /// <summary>
    ///     Returns the first element that matches the specified XPath selector.
    /// </summary>
    public static INode? XPath(this IEnumerable<INode> nodes, string selector) => Select(nodes, NodeSelector.XPath(selector));

    /// <summary>
    ///     Returns the first element that matches the specified XPath selector.
    /// </summary>
    public static INode? XPath(this IEnumerable<INode> nodes, XPathExpression selector) => Select(nodes, NodeSelector.XPath(selector));

    /// <summary>
    ///     Returns the first element found by the specified selector.
    /// </summary>
    public static INode? Select(this IEnumerable<INode> nodes, NodeSelector selector)
    {
        return selector.Select(nodes);
    }

    /// <summary>
    ///     Returns the first element that matches the specified Css selector, or fail.
    /// </summary>
    /// <exception cref="NodeNotFoundException"></exception>
    public static INode CssOrFail(this IEnumerable<INode> nodes, string selector) => SelectOrFail(nodes, NodeSelector.Css(selector));

    /// <summary>
    ///     Returns the first element that matches the specified XPath selector, or fail.
    /// </summary>
    /// <exception cref="NodeNotFoundException"></exception>
    public static INode XPathOrFail(this IEnumerable<INode> nodes, string selector) => SelectOrFail(nodes, NodeSelector.XPath(selector));

    /// <summary>
    ///     Returns the first element that matches the specified XPath selector, or fail.
    /// </summary>
    /// <exception cref="NodeNotFoundException"></exception>
    public static INode XPathOrFail(this IEnumerable<INode> nodes, XPathExpression selector) => SelectOrFail(nodes, NodeSelector.XPath(selector));

    /// <summary>
    ///     Returns the first element found by the specified selector, or fail.
    /// </summary>
    /// <exception cref="NodeNotFoundException"></exception>
    public static INode SelectOrFail(this IEnumerable<INode> nodes, NodeSelector selector)
    {
        var selectedNode = selector.Select(nodes);

        if (selectedNode is null)
        {
            throw new NodeNotFoundException($"Select '{selector}' not found.", selector.ToString());
        }

        return selectedNode;
    }

    /// <summary>
    ///     Returns all the elements that matches the specified Css selector.
    /// </summary>
    public static INodeList CssAll(this IEnumerable<INode> nodes, string selector) => SelectAll(nodes, NodeSelector.Css(selector));

    /// <summary>
    ///     Returns all the elements that matches the specified XPath selector.
    /// </summary>
    public static INodeList XPathAll(this IEnumerable<INode> nodes, string selector) => SelectAll(nodes, NodeSelector.XPath(selector));

    /// <summary>
    ///     Returns all the elements that matches the specified XPath selector.
    /// </summary>
    public static INodeList XPathAll(this IEnumerable<INode> nodes, XPathExpression selector) => SelectAll(nodes, NodeSelector.XPath(selector));

    /// <summary>
    ///     Returns all the elements found by the specified selector.
    /// </summary>
    public static INodeList SelectAll(this IEnumerable<INode> nodes, NodeSelector selector)
    {
        return new NodeList(selector.SelectAll(nodes));
    }

    #endregion

    private static Uri GetUri(INode node, string value)
    {
        if (node is not IElement el)
        {
            throw new NodeException();
        }

        Uri baseUri = new(el.BaseUrl!.Href, UriKind.Absolute);

        // UriComponents.NormalizedHost will translate punycode
        // this can cause problems in some rare scenario
        return new Uri(baseUri, Uri.UnescapeDataString(WebUtility.HtmlDecode(value)));
    }

    private static T ChangeType<T>(string value)
    {
        Type stringType = typeof(string); // value.GetType()
        Type targetType = typeof(T);

        if (targetType == stringType)
        {
            return (T)(object)value;
        }

        // special case
        if (targetType == typeof(Uri))
        {
            return (T)(object)new Uri(value, UriKind.Absolute);
        }

        TypeConverter converter = TypeDescriptor.GetConverter(@stringType);
        if (converter is not null)
        {
            if (converter.CanConvertTo(targetType))
            {
                return (T)converter.ConvertTo(stringType, targetType)!;
            }
        }

        converter = TypeDescriptor.GetConverter(targetType);
        if (converter is not null)
        {
            if (converter.CanConvertFrom(stringType))
            {
                return (T)converter.ConvertFrom(stringType)!;
            }
        }

        return (T)Convert.ChangeType(value, targetType);
    }
}
