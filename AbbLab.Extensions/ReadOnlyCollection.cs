using System;
using System.Collections.ObjectModel;

namespace AbbLab.Extensions
{
    /// <summary>
    ///   <para>Provides a static method for getting an empty instance of the <see cref="ReadOnlyCollection{T}"/> class.</para>
    /// </summary>
    public static class ReadOnlyCollection
    {
        private static class EmptyCollection<T>
        {
            public static readonly ReadOnlyCollection<T> Empty = new ReadOnlyCollection<T>(Array.Empty<T>());
        }
        /// <summary>
        ///   <para>Returns an empty instance of the <see cref="ReadOnlyCollection{T}"/> class.</para>
        /// </summary>
        /// <typeparam name="T">The type of elements of the collection.</typeparam>
        /// <returns>An empty instance of the <see cref="ReadOnlyCollection{T}"/> class.</returns>
        public static ReadOnlyCollection<T> Empty<T>() => EmptyCollection<T>.Empty;
    }
}
