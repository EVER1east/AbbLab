using System;
using System.Collections;
using System.Collections.Generic;

namespace AbbLab.Extensions
{
    /// <summary>
    ///   <para>Represents a comparer that uses the negated result of another comparer.</para>
    /// </summary>
    /// <typeparam name="T">The type of objects to compare.</typeparam>
    public sealed class ReverseComparer<T> : IComparer<T>, IComparer
    {
        /// <summary>
        ///   <para>Gets the underlying comparer.</para>
        /// </summary>
        public IComparer<T> Comparer { get; }

        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="ReverseComparer{T}"/> class with the specified <paramref name="comparer"/>.</para>
        /// </summary>
        /// <param name="comparer">The comparer to use.</param>
        public ReverseComparer(IComparer<T> comparer) => Comparer = comparer;

        /// <inheritdoc/>
        public int Compare(T? a, T? b)
        {
            if (b is null) return a is null ? 0 : -1;
            if (a is null) return 1;
            return -Comparer.Compare(a, b);
        }
        int IComparer.Compare(object? a, object? b)
        {
            if (b is null) return a is null ? 0 : -1;
            if (a is null) return 1;
            if (a is T tA && b is T tB) return Compare(tA, tB);
            if (a is IComparable comparableA)
                return -comparableA.CompareTo(b);
            if (b is IComparable comparableB)
                return comparableB.CompareTo(a);
            throw new ArgumentException("The objects do not implement the IComparable interface.");
        }

        /// <summary>
        ///   <para>Gets a comparer that uses the negated result of <see cref="Comparer{T}.Default"/>.</para>
        /// </summary>
        public static ReverseComparer<T> Default { get; } = new ReverseComparer<T>(Comparer<T>.Default);

        /// <summary>
        ///   <para>Returns a comparer that uses the negated result of the specified <paramref name="comparison"/>.</para>
        /// </summary>
        /// <param name="comparison">The comparison to use.</param>
        /// <returns>The new comparer.</returns>
        public static ReverseComparer<T> Create(Func<T, T, int> comparison)
            => new ReverseComparer<T>(Comparer<T>.Create(comparison.Invoke));

    }
}
