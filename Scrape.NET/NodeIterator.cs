namespace Scrape.NET;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.XPath;
using AngleSharp.Dom;
using AngleSharp.XPath;

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
