namespace Scrape.NET;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Xml.XPath;
using AngleSharp.Dom;

public static class NodeExtensions
{
    public static string Text(this INode node) => node.TextContent();

    // avoid collision with AngleSharp.NodeExtensions.Text()
    public static string TextContent(this INode node)
    {
        var text = node?.TextContent;

        if (text is null)
        {
            return string.Empty;
        }

        return Uri.UnescapeDataString(WebUtility.HtmlDecode(text)).Trim();
    }

    public static string Html(this INode node)
    {
        var html = node is IElement el ? el.OuterHtml : null;

        if (html is null)
        {
            return string.Empty;
        }

        return Uri.UnescapeDataString(WebUtility.HtmlDecode(html)).Trim();
    }

    public static string HtmlContent(this INode node) => node.Html();

    #region Attr

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

    [return: NotNullIfNotNull("defaultValue")]
    public static string? Attr(this INode node, string name, string? defaultValue)
    {
        var value = node is IElement el ? el.Attributes.GetNamedItem(null, name)?.Value : null;

        if (value is null)
        {
            return defaultValue;
        }

        return Uri.UnescapeDataString(WebUtility.HtmlDecode(value)).Trim();
    }

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

    [return: NotNullIfNotNull("defaultValue")]
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

    public static Uri Src(this INode node)
    {
        var value = node.Attr(AttributeNames.Src);

        return GetUri(node, value);
    }

    [return: NotNullIfNotNull("defaultValue")]
    public static Uri? Src(this INode node, Uri? defaultValue)
    {
        var value = node.Attr(AttributeNames.Src, null);

        if (value is null)
        {
            return defaultValue;
        }

        return GetUri(node, value);
    }

    public static T Src<T>(this INode node)
    {
        var value = node.Src();

        return ChangeType<T>(value.GetComponents(UriComponents.AbsoluteUri, UriFormat.Unescaped));
    }

    [return: NotNullIfNotNull("defaultValue")]
    public static T? Src<T>(this INode node, T? defaultValue)
    {
        var value = node.Src(null);

        if (value is null)
        {
            return defaultValue;
        }

        return ChangeType<T>(value.GetComponents(UriComponents.AbsoluteUri, UriFormat.Unescaped));
    }

    #endregion

    #region Href

    public static Uri Href(this INode node)
    {
        var value = node.Attr(AttributeNames.Href);

        return GetUri(node, value);
    }

    [return: NotNullIfNotNull("defaultValue")]
    public static Uri? Href(this INode node, Uri? defaultValue)
    {
        var value = node.Attr(AttributeNames.Href, null);

        if (value is null)
        {
            return defaultValue;
        }

        return GetUri(node, value);
    }

    public static T Href<T>(this INode node)
    {
        var value = node.Href();

        return ChangeType<T>(value.GetComponents(UriComponents.AbsoluteUri, UriFormat.Unescaped));
    }

    [return: NotNullIfNotNull("defaultValue")]
    public static T? Href<T>(this INode node, T? defaultValue)
    {
        var value = node.Href(null);

        if (value is null)
        {
            return defaultValue;
        }

        return ChangeType<T>(value.GetComponents(UriComponents.AbsoluteUri, UriFormat.Unescaped));
    }

    #endregion

    #region Select from INode

    public static INode? Css(this INode node, string selector) => node.Select(NodeSelector.Css(selector));
    public static INode? XPath(this INode node, string selector) => node.Select(NodeSelector.XPath(selector));
    public static INode? XPath(this INode node, XPathExpression selector) => node.Select(NodeSelector.XPath(selector));
    public static INode? Select(this INode node, NodeSelector selector)
    {
        if (node is IDocument document)
        {
            node = document.DocumentElement;
        }

        return selector.Select(node);
    }

    public static INode CssOrFail(this INode node, string selector) => node.SelectOrFail(NodeSelector.Css(selector));
    public static INode XPathOrFail(this INode node, string selector) => node.SelectOrFail(NodeSelector.XPath(selector));
    public static INode XPathOrFail(this INode node, XPathExpression selector) => node.SelectOrFail(NodeSelector.XPath(selector));
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

    public static INodeList CssAll(this INode node, string selector) => node.SelectAll(NodeSelector.Css(selector));
    public static INodeList XPathAll(this INode node, string selector) => node.SelectAll(NodeSelector.XPath(selector));
    public static INodeList XPathAll(this INode node, XPathExpression selector) => node.SelectAll(NodeSelector.XPath(selector));
    public static INodeList SelectAll(this INode node, NodeSelector selector)
    {
        if (node is IDocument document)
        {
            node = document.DocumentElement;
        }

        return new NodeList(selector.SelectAll(node));
    }

    #endregion

    #region Select from INodeList

    public static INode? Css(this IEnumerable<INode> nodes, string selector) => nodes.Select(NodeSelector.Css(selector));
    public static INode? XPath(this IEnumerable<INode> nodes, string selector) => nodes.Select(NodeSelector.XPath(selector));
    public static INode? XPath(this IEnumerable<INode> nodes, XPathExpression selector) => nodes.Select(NodeSelector.XPath(selector));
    public static INode? Select(this IEnumerable<INode> nodes, NodeSelector selector)
    {
        return selector.Select(nodes);
    }

    public static INode CssOrFail(this IEnumerable<INode> nodes, string selector) => nodes.SelectOrFail(NodeSelector.Css(selector));
    public static INode XPathOrFail(this IEnumerable<INode> nodes, string selector) => nodes.SelectOrFail(NodeSelector.XPath(selector));
    public static INode XPathOrFail(this IEnumerable<INode> nodes, XPathExpression selector) => nodes.SelectOrFail(NodeSelector.XPath(selector));
    public static INode SelectOrFail(this IEnumerable<INode> nodes, NodeSelector selector)
    {
        var selectedNode = selector.Select(nodes);

        if (selectedNode is null)
        {
            throw new NodeNotFoundException($"Select '{selector}' not found.", selector.ToString());
        }

        return selectedNode;
    }

    public static INodeList CssAll(this IEnumerable<INode> nodes, string selector) => nodes.SelectAll(NodeSelector.Css(selector));
    public static INodeList XPathAll(this IEnumerable<INode> nodes, string selector) => nodes.SelectAll(NodeSelector.XPath(selector));
    public static INodeList XPathAll(this IEnumerable<INode> nodes, XPathExpression selector) => nodes.SelectAll(NodeSelector.XPath(selector));
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
