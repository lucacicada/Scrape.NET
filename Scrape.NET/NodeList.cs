namespace Scrape.NET;

using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using AngleSharp;
using AngleSharp.Dom;

[DebuggerTypeProxy(typeof(DebugView))]
internal sealed class NodeList : INodeList, IReadOnlyList<INode>, IReadOnlyCollection<INode>
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly ImmutableArray<INode> _entries;

    public INode this[int index] => _entries[index];

    public int Length => _entries.Length;
    public int Count => _entries.Length;

    public NodeList(IEnumerable<INode> nodes)
    {
        _entries = nodes.ToImmutableArray();
    }

    void IMarkupFormattable.ToHtml(TextWriter writer, IMarkupFormatter formatter)
    {
        foreach (INode node in _entries)
        {
            node.ToHtml(writer, formatter);
        }
    }

    IEnumerator<INode> IEnumerable<INode>.GetEnumerator() => ((IEnumerable<INode>)_entries).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_entries).GetEnumerator();

    private sealed class DebugView
    {
        public INode[] NodeList { get; }

        public DebugView(NodeList nodeList)
        {
            NodeList = nodeList._entries.ToArray();
        }
    }
}
