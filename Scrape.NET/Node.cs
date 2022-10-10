namespace Scrape.NET;

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.XPath;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.XPath;

public class NodeException : Exception
{
    /// <summary>Initializes a new instance of the <see cref="NodeException" /> class.</summary>
    public NodeException()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="NodeException" /> class with serialized data.</summary>
    /// <param name="info">The <see cref="SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
    /// <param name="context">The <see cref="StreamingContext" /> that contains contextual information about the source or destination.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="info" /> is <see langword="null" />.</exception>
    /// <exception cref="SerializationException">The class name is <see langword="null" /> or <see cref="System.Exception.HResult" /> is zero (0).</exception>
    protected NodeException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="NodeException" /> class with a specified error message.</summary>
    /// <param name="message">The message that describes the error.</param>
    public NodeException(string? message) : base(message)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="NodeException" /> class with a specified error message and a reference to the inner exception that is the cause of this exception.</summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (<see langword="Nothing" /> in Visual Basic) if no inner exception is specified.</param>
    public NodeException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}

public class AttributeNotFoundException : NodeException
{
    /// <summary>
    ///     The name of the attribute that is missing.
    /// </summary>
    public string? AttributeName { get; }

    /// <summary>Initializes a new instance of the <see cref="AttributeNotFoundException" /> class.</summary>
    public AttributeNotFoundException()
    {

    }

    /// <summary>Initializes a new instance of the <see cref="AttributeNotFoundException" /> class with serialized data.</summary>
    /// <param name="info">The <see cref="SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
    /// <param name="context">The <see cref="StreamingContext" /> that contains contextual information about the source or destination.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="info" /> is <see langword="null" />.</exception>
    /// <exception cref="SerializationException">The class name is <see langword="null" /> or <see cref="System.Exception.HResult" /> is zero (0).</exception>
    protected AttributeNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        AttributeName = info.GetString("AttributeNotFound_AttributeName");
    }

    public AttributeNotFoundException(string? message) : base(message)
    {
    }

    public AttributeNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    public AttributeNotFoundException(string? message, string? attributeName) : base(message)
    {
        AttributeName = attributeName;
    }

    public AttributeNotFoundException(string? message, string? attributeName, Exception? innerException) : base(message, innerException)
    {
        AttributeName = attributeName;
    }

    /// <inheritdoc />
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue("AttributeNotFound_AttributeName", AttributeName, typeof(string));
    }
}

public class NodeNotFoundException : NodeException
{
    public string? NodeSelector { get; }

    /// <summary>Initializes a new instance of the <see cref="AttributeNotFoundException" /> class.</summary>
    public NodeNotFoundException()
    {

    }

    /// <summary>Initializes a new instance of the <see cref="AttributeNotFoundException" /> class with serialized data.</summary>
    /// <param name="info">The <see cref="SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
    /// <param name="context">The <see cref="StreamingContext" /> that contains contextual information about the source or destination.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="info" /> is <see langword="null" />.</exception>
    /// <exception cref="SerializationException">The class name is <see langword="null" /> or <see cref="System.Exception.HResult" /> is zero (0).</exception>
    protected NodeNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        NodeSelector = info.GetString("AttributeNotFound_NodeSelector");
    }

    public NodeNotFoundException(string? message) : base(message)
    {
    }

    public NodeNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    public NodeNotFoundException(string? message, string? nodeSelector) : base(message)
    {
        NodeSelector = nodeSelector;
    }

    public NodeNotFoundException(string? message, string? nodeSelector, Exception? innerException) : base(message, innerException)
    {
        NodeSelector = nodeSelector;
    }

    /// <inheritdoc />
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue("AttributeNotFound_NodeSelector", NodeSelector, typeof(string));
    }
}
public abstract class Selector<TNode, TElement>
{
    public abstract IEnumerable<TElement> SelectAll(TNode? node);
    public abstract IEnumerable<TElement> SelectAll(IEnumerable<TNode?>? nodes);

    public abstract TElement? Select(TNode? node);
    public abstract TElement? Select(IEnumerable<TNode?>? nodes);
}

public class NodeSelector : Selector<INode, INode>
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private const string DISPLAY_TEXT = "Selector()";

    public static NodeSelector Css(string selector)
    {
        if (selector is null) throw new ArgumentNullException(nameof(selector));

        return new NodeSelector(
            selector,
            node => node is IElement el ? el.QuerySelector(selector) : null,
            node => node is IElement el ? el.QuerySelectorAll(selector) : null
        );
    }

    public static NodeSelector XPath(string selector)
    {
        if (selector is null) throw new ArgumentNullException(nameof(selector));

        return new NodeSelector(
            selector,
            node => node is IElement el ? el.SelectSingleNode(selector) : null,
            node => node is IElement el ? el.SelectNodes(selector) : null
        );
    }

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
    private readonly string _displayText;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly Func<INode, INode?>? _selectOne;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly Func<INode, IEnumerable<INode?>?>? _selectMany;

    protected NodeSelector()
    {
        _displayText = DISPLAY_TEXT;
    }

    public NodeSelector(
        Func<INode, INode?> selectOne,
        Func<INode, IEnumerable<INode?>?> selectMany
    )
        : this(DISPLAY_TEXT, selectOne, selectMany)
    {
    }

    public NodeSelector(
        string displayText,
        Func<INode, INode?> selectOne,
        Func<INode, IEnumerable<INode?>?> selectMany
    )
    {
        if (selectOne is null) throw new ArgumentNullException(nameof(selectOne));
        if (selectMany is null) throw new ArgumentNullException(nameof(selectMany));

        _displayText = displayText ?? DISPLAY_TEXT;
        _selectOne = selectOne;
        _selectMany = selectMany;
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
    public override string ToString()
    {
        return _displayText;
    }
}

internal sealed class NodeIterator : IEnumerable<INode>, IEnumerator<INode>
{
    private readonly XPathNodeIterator _nodeIterator;

    public NodeIterator(XPathNodeIterator it)
    {
        _nodeIterator = it;
    }

    public INode Current { get; private set; } = default!;
    object IEnumerator.Current => Current;

    public bool MoveNext()
    {
        while (_nodeIterator.MoveNext())
        {
            if (_nodeIterator.Current is HtmlDocumentNavigator naviagtor)
            {
                Current = naviagtor.CurrentNode;

                return true;
            }
        }

        return false;
    }

    public void Reset() => throw new NotSupportedException();

    public IEnumerator<INode> GetEnumerator() => this;
    IEnumerator IEnumerable.GetEnumerator() => this;

    void IDisposable.Dispose() => ((IDisposable)_nodeIterator).Dispose();
}

internal sealed class NodeList : INodeList
{
    private readonly List<INode> _entries;

    public INode this[int index] => _entries[index];

    public int Length => _entries.Count;

    internal NodeList(IEnumerable<INode> nodes)
    {
        _entries = new List<INode>(nodes);
    }

    public void ToHtml(TextWriter writer, IMarkupFormatter formatter)
    {
        for (var i = 0; i < _entries.Count; i++)
        {
            _entries[i].ToHtml(writer, formatter);
        }
    }

    public IEnumerator<INode> GetEnumerator() => _entries.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _entries.GetEnumerator();
}
