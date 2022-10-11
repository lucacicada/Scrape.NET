namespace Scrape.NET;

using System.Collections.Generic;

/// <summary>
///     Provides a way to select a <typeparamref name="TElement"/> from a <typeparamref name="TNode"/>.
/// </summary>
public abstract class Selector<TNode, TElement>
{
    /// <summary>
    ///     Select the first element from the specified node.
    /// </summary>
    public abstract TElement? Select(TNode? node);

    /// <summary>
    ///     Select the first element from the specified node.
    /// </summary>
    public abstract TElement? Select(IEnumerable<TNode?>? nodes);

    /// <summary>
    ///     Select all the element from the specified node.
    /// </summary>
    public abstract IEnumerable<TElement> SelectAll(TNode? node);

    /// <summary>
    ///     Select all the element from the specified node.
    /// </summary>
    public abstract IEnumerable<TElement> SelectAll(IEnumerable<TNode?>? nodes);
}
