using System;
using System.Collections;
using System.Collections.Generic;

namespace AbbLab.Extensions
{
    public sealed class SelectorComparer<T, TResult> : IComparer<T>, IComparer
    {
        public IComparer<TResult> Comparer { get; }
        public Func<T, TResult> Selector { get; }

        public SelectorComparer(Func<T, TResult> selector)
            : this(selector, null) { }
        public SelectorComparer(Func<T, TResult> selector, IComparer<TResult>? comparer)
        {
            Comparer = comparer ?? Comparer<TResult>.Default;
            Selector = selector;
        }

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

        public static SelectorComparer<T, TResult> Create(Func<T, TResult> selector, Func<TResult, TResult, int> comparison)
            => new SelectorComparer<T, TResult>(selector, Comparer<TResult>.Create(comparison.Invoke));

    }
}
