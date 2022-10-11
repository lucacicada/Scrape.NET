namespace Scrape.NET;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.XPath;
using AngleSharp.Dom;
using AngleSharp.XPath;

/// <summary>
///     Provides a way to select nodes using a Css or XPath selector string.
/// </summary>
public class NodeSelector : Selector<INode, INode>
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private const string DISPLAY_TEXT = "Selector()";

    /// <summary>
    ///     Creates a new Css selector.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="selector"/> is null.</exception>
    public static NodeSelector Css(string selector)
    {
        if (selector is null) throw new ArgumentNullException(nameof(selector));

        return new NodeSelector(
            selector,
            node => node is IElement el ? el.QuerySelector(selector) : null,
            node => node is IElement el ? el.QuerySelectorAll(selector) : null
        );
    }

    /// <summary>
    ///     Creates a new XPath selector.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="selector"/> is null.</exception>
    public static NodeSelector XPath(string selector)
    {
        if (selector is null) throw new ArgumentNullException(nameof(selector));

        return new NodeSelector(
            selector,
            node => node is IElement el ? el.SelectSingleNode(selector) : null,
            node => node is IElement el ? el.SelectNodes(selector) : null
        );
    }

    /// <summary>
    ///     Creates a new XPath selector.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="selector"/> is null.</exception>
    public static NodeSelector XPath(XPathExpression selector)
    {
        if (selector is null) throw new ArgumentNullException(nameof(selector));

        return new NodeSelector(
            selector.Expression,
            node =>
            {
                var nav = new HtmlDocumentNavigator(node.Owner, node, ignoreNamespaces: true);

                // xpath.SetContext(new XmlNamespaceManager(new NameTable()));

                return new NodeIterator(nav.Select(selector)).Current;
            },
            node =>
            {
                var nav = new HtmlDocumentNavigator(node.Owner, node, ignoreNamespaces: true);

                // xpath.SetContext(new XmlNamespaceManager(new NameTable()));

                return new NodeIterator(nav.Select(selector));
            }
        );
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly string? _displayText;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly Func<INode, INode?>? _selectOne;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly Func<INode, IEnumerable<INode?>?>? _selectMany;

    /// <summary>
    ///     Initializes a new instance of the <see cref="NodeSelector"/> class.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="selectOne"/> is null.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="selectMany"/> is null.</exception>
    public NodeSelector(
        Func<INode, INode?> selectOne,
        Func<INode, IEnumerable<INode?>?> selectMany
    )
    {
        if (selectOne is null) throw new ArgumentNullException(nameof(selectOne));
        if (selectMany is null) throw new ArgumentNullException(nameof(selectMany));

        _selectOne = selectOne;
        _selectMany = selectMany;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="NodeSelector"/> class.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="selectOne"/> is null.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="selectMany"/> is null.</exception>
    public NodeSelector(
        string? displayText,
        Func<INode, INode?> selectOne,
        Func<INode, IEnumerable<INode?>?> selectMany
    )
    {
        if (selectOne is null) throw new ArgumentNullException(nameof(selectOne));
        if (selectMany is null) throw new ArgumentNullException(nameof(selectMany));

        _displayText = displayText;
        _selectOne = selectOne;
        _selectMany = selectMany;
    }

    /// <inheritdoc />
    public override INode? Select(INode? node)
    {
        if (_selectOne is null || node is null)
        {
            return null;
        }

        return _selectOne(node);
    }

    /// <inheritdoc />
    public override INode? Select(IEnumerable<INode?>? nodes)
    {
        if (_selectOne is null || nodes is null)
        {
            return null;
        }

        return Select(nodes.OfType<INode>().FirstOrDefault());
    }

    /// <inheritdoc />
    public override IEnumerable<INode> SelectAll(INode? node)
    {
        if (_selectMany is null || node is null)
        {
            yield break;
        }

        var enumerator = _selectMany(node);

        if (enumerator is null)
        {
            yield break;
        }

        foreach (INode? item in enumerator)
        {
            if (item is not null)
            {
                yield return item;
            }
        }
    }

    /// <inheritdoc />
    public override IEnumerable<INode> SelectAll(IEnumerable<INode?>? nodes)
    {
        if (_selectMany is null || nodes is null)
        {
            yield break;
        }

        foreach (INode? node in nodes)
        {
            if (node is not null)
            {
                foreach (INode item in SelectAll(node))
                {
                    yield return item;
                }
            }
        }
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return _displayText ?? DISPLAY_TEXT;
    }
}
