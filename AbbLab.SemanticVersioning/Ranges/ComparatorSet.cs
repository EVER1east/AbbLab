using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AbbLab.Extensions;
using JetBrains.Annotations;

namespace AbbLab.SemanticVersioning
{
    public sealed class ComparatorSet : IList<Comparator>, IList, IReadOnlyList<Comparator>
    {
        private readonly Comparator[] _comparators;
        private ReadOnlyCollection<Comparator>? _comparatorsReadonly;
        public ReadOnlyCollection<Comparator> Comparators
        {
            get
            {
                if (_comparatorsReadonly is not null) return _comparatorsReadonly;
                if (_comparators.Length is 0)
                    return _comparatorsReadonly = ReadOnlyCollection.Empty<Comparator>();
                return _comparatorsReadonly = new ReadOnlyCollection<Comparator>(_comparators);
            }
        }

        public ComparatorSet([InstantHandle] IEnumerable<Comparator> comparators)
            => _comparators = comparators.ToArray();

        [Pure] public bool IsSatisfiedBy(SemanticVersion version)
            => IsSatisfiedBy(version, false);
        [Pure] public bool IsSatisfiedBy(SemanticVersion version, bool includePreReleases)
        {
            // TODO:
            // > If a version has a pre-release tag then it will only be allowed to satisfy comparator sets
            // > if at least one comparator with the same [major, minor, patch] tuple also has a pre-release tag.
            // > (https://github.com/npm/node-semver#prerelease-tags)
            for (int i = 0, length = _comparators.Length; i < length; i++)
                if (!_comparators[i].IsSatisfiedBy(version, includePreReleases))
                    return false;
            return true;
        }

        public int Count => _comparators.Length;
        public Comparator this[int index] => _comparators[index];

        [MustUseReturnValue] private static Exception GetException()
            => new NotSupportedException($"The {nameof(ComparatorSet)} class is not mutable.");

        int IList.Add(object? obj) => throw GetException();
        void IList.Insert(int index, object? obj) => throw GetException();
        void IList.Remove(object? obj) => throw GetException();
        void IList.RemoveAt(int index) => throw GetException();
        bool IList.Contains(object? obj)
            => obj is Comparator comparator && Array.IndexOf(_comparators, comparator) > -1;
        int IList.IndexOf(object? obj)
            => obj is Comparator comparator ? Array.IndexOf(_comparators, comparator) : -1;
        void IList.Clear() => throw GetException();
        bool IList.IsFixedSize => true;
        bool IList.IsReadOnly => true;
        object? IList.this[int index]
        {
            get => _comparators[index];
            set => throw GetException();
        }

        void IList<Comparator>.Insert(int index, Comparator item) => throw GetException();
        void IList<Comparator>.RemoveAt(int index) => throw GetException();
        int IList<Comparator>.IndexOf(Comparator item)
            => Array.IndexOf(_comparators, item);
        Comparator IList<Comparator>.this[int index]
        {
            get => _comparators[index];
            set => throw GetException();
        }

        void ICollection.CopyTo(Array array, int startIndex)
            => _comparators.CopyTo(array, startIndex);
        int ICollection.Count => _comparators.Length;
        bool ICollection.IsSynchronized => true;
        object ICollection.SyncRoot => this;

        void ICollection<Comparator>.Add(Comparator item) => throw GetException();
        bool ICollection<Comparator>.Remove(Comparator item) => throw GetException();
        void ICollection<Comparator>.Clear() => throw GetException();
        bool ICollection<Comparator>.Contains(Comparator item)
            => Array.IndexOf(_comparators, item) > -1;
        void ICollection<Comparator>.CopyTo(Comparator[] array, int startIndex)
            => _comparators.CopyTo(array, startIndex);
        int ICollection<Comparator>.Count => _comparators.Length;
        bool ICollection<Comparator>.IsReadOnly => true;

        IEnumerator IEnumerable.GetEnumerator() => _comparators.GetEnumerator();
        IEnumerator<Comparator> IEnumerable<Comparator>.GetEnumerator() => _comparators.AsEnumerable().GetEnumerator();

    }
}
