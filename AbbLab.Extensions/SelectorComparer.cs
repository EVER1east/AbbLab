using System;
using System.Collections;
using System.Collections.Generic;

namespace AbbLab.Extensions
{
    /// <summary>
    ///   <para>Represents a comparer that uses the result of another comparer, comparing values determined by a selector function.</para>
    /// </summary>
    /// <typeparam name="T">The type of objects to compare.</typeparam>
    /// <typeparam name="TResult">The type of objects, used in the comparison.</typeparam>
    public sealed class SelectorComparer<T, TResult> : IComparer<T>, IComparer
    {
        /// <summary>
        ///   <para>Gets the underlying comparer.</para>
        /// </summary>
        public IComparer<TResult> Comparer { get; }
        /// <summary>
        ///   <para>Gets the selector function that is used to determine an object's value.</para>
        /// </summary>
        public Func<T, TResult> Selector { get; }

        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="SelectorComparer{T,TResult}"/> with the specified <paramref name="selector"/>.</para>
        /// </summary>
        /// <param name="selector">The selector function to determine objects' values.</param>
        public SelectorComparer(Func<T, TResult> selector)
            : this(selector, null) { }
        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="SelectorComparer{T,TResult}"/> with the specified <paramref name="selector"/> and <paramref name="comparer"/>.</para>
        /// </summary>
        /// <param name="selector">The selector function to determine objects' values.</param>
        /// <param name="comparer">The comparer to use.</param>
        public SelectorComparer(Func<T, TResult> selector, IComparer<TResult>? comparer)
        {
            Comparer = comparer ?? Comparer<TResult>.Default;
            Selector = selector;
        }

        /// <inheritdoc/>
        public int Compare(T? a, T? b)
        {
            if (b is null) return a is null ? 0 : 1;
            if (a is null) return -1;
            return Comparer.Compare(Selector(a), Selector(b));
        }
        int IComparer.Compare(object? a, object? b)
        {
            if (b is null) return a is null ? 0 : 1;
            if (a is null) return -1;
            if (a is T tA && b is T tB) return Compare(tA, tB);
            if (a is IComparable comparableA)
                return comparableA.CompareTo(b);
            if (b is IComparable comparableB)
                return -comparableB.CompareTo(a);
            throw new ArgumentException("The objects do not implement the IComparable interface.");
        }

        /// <summary>
        ///   <para>Returns a comparer that uses the result of the specified <paramref name="comparison"/> on values determined by the specified <paramref name="selector"/>.</para>
        /// </summary>
        /// <param name="selector">The selector function to determine objects' values.</param>
        /// <param name="comparison">The comparison to use.</param>
        /// <returns>The new comparer.</returns>
        public static SelectorComparer<T, TResult> Create(Func<T, TResult> selector, Func<TResult, TResult, int> comparison)
            => new SelectorComparer<T, TResult>(selector, Comparer<TResult>.Create(comparison.Invoke));

    }
}
