using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AbbLab.Extensions;
using JetBrains.Annotations;

namespace AbbLab.SemanticVersioning
{
    public sealed class VersionRange : IList<ComparatorSet>, IList, IReadOnlyList<ComparatorSet>
    {
        private readonly ComparatorSet[] _comparatorSets;
        private ReadOnlyCollection<ComparatorSet>? _comparatorSetsReadonly;
        public ReadOnlyCollection<ComparatorSet> ComparatorSets
        {
            get
            {
                if (_comparatorSetsReadonly is not null) return _comparatorSetsReadonly;
                if (_comparatorSets.Length is 0)
                    return _comparatorSetsReadonly = ReadOnlyCollection.Empty<ComparatorSet>();
                return _comparatorSetsReadonly = new ReadOnlyCollection<ComparatorSet>(_comparatorSets);
            }
        }

        public VersionRange([InstantHandle] IEnumerable<ComparatorSet> comparatorSets)
            => _comparatorSets = comparatorSets.ToArray();

        [Pure] public bool IsSatisfiedBy(SemanticVersion version)
            => IsSatisfiedBy(version, false);
        [Pure] public bool IsSatisfiedBy(SemanticVersion version, bool includePreReleases)
        {
            for (int i = 0, length = _comparatorSets.Length; i < length; i++)
                if (_comparatorSets[i].IsSatisfiedBy(version, includePreReleases))
                    return true;
            return false;
        }

        public int Count => _comparatorSets.Length;
        public ComparatorSet this[int index] => _comparatorSets[index];

        [MustUseReturnValue] private static Exception GetException()
            => new NotSupportedException($"The {nameof(VersionRange)} class is not mutable.");

        int IList.Add(object? obj) => throw GetException();
        void IList.Insert(int index, object? obj) => throw GetException();
        void IList.Remove(object? obj) => throw GetException();
        void IList.RemoveAt(int index) => throw GetException();
        bool IList.Contains(object? obj)
            => obj is ComparatorSet comparator && Array.IndexOf(_comparatorSets, comparator) > -1;
        int IList.IndexOf(object? obj)
            => obj is ComparatorSet comparator ? Array.IndexOf(_comparatorSets, comparator) : -1;
        void IList.Clear() => throw GetException();
        bool IList.IsFixedSize => true;
        bool IList.IsReadOnly => true;
        object? IList.this[int index]
        {
            get => _comparatorSets[index];
            set => throw GetException();
        }

        void IList<ComparatorSet>.Insert(int index, ComparatorSet item) => throw GetException();
        void IList<ComparatorSet>.RemoveAt(int index) => throw GetException();
        int IList<ComparatorSet>.IndexOf(ComparatorSet item)
            => Array.IndexOf(_comparatorSets, item);
        ComparatorSet IList<ComparatorSet>.this[int index]
        {
            get => _comparatorSets[index];
            set => throw GetException();
        }

        void ICollection.CopyTo(Array array, int startIndex)
            => _comparatorSets.CopyTo(array, startIndex);
        int ICollection.Count => _comparatorSets.Length;
        bool ICollection.IsSynchronized => true;
        object ICollection.SyncRoot => this;

        void ICollection<ComparatorSet>.Add(ComparatorSet item) => throw GetException();
        bool ICollection<ComparatorSet>.Remove(ComparatorSet item) => throw GetException();
        void ICollection<ComparatorSet>.Clear() => throw GetException();
        bool ICollection<ComparatorSet>.Contains(ComparatorSet item)
            => Array.IndexOf(_comparatorSets, item) > -1;
        void ICollection<ComparatorSet>.CopyTo(ComparatorSet[] array, int startIndex)
            => _comparatorSets.CopyTo(array, startIndex);
        int ICollection<ComparatorSet>.Count => _comparatorSets.Length;
        bool ICollection<ComparatorSet>.IsReadOnly => true;

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        [Pure] public IEnumerator<ComparatorSet> GetEnumerator() => ((IEnumerable<ComparatorSet>)_comparatorSets).GetEnumerator();

    }
}