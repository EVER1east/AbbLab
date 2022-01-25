using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace AbbLab.Extensions
{
    public static class ArrayExtensions
    {
        [Pure] public static int IndexOf<T>(this T[] array, T value)
            => Array.IndexOf(array, value);
        [Pure] public static int IndexOf<T>(this T[] array, T value, int startIndex)
            => Array.IndexOf(array, value, startIndex);
        [Pure] public static int IndexOf<T>(this T[] array, T value, int startIndex, int count)
            => Array.IndexOf(array, value, startIndex, count);

        [Pure] public static int LastIndexOf<T>(this T[] array, T value)
            => Array.LastIndexOf(array, value);
        [Pure] public static int LastIndexOf<T>(this T[] array, T value, int startIndex)
            => Array.LastIndexOf(array, value, startIndex);
        [Pure] public static int LastIndexOf<T>(this T[] array, T value, int startIndex, int count)
            => Array.LastIndexOf(array, value, startIndex, count);

        [Pure] public static bool Contains<T>(this T[] array, T value)
            => Array.IndexOf(array, value) is not -1;

        [Pure] public static bool Exists<T>(this T[] array, [InstantHandle] Func<T, bool> predicate)
            => Array.Exists(array, predicate.Invoke);
        [Pure] public static bool Exists<T>(this T[] array, [InstantHandle] Func<T, int, bool> predicate)
        {
            for (int i = 0, length = array.Length; i < length; i++)
                if (predicate(array[i], i))
                    return true;
            return false;
        }

        [Pure] public static T? Find<T>(this T[] array, [InstantHandle] Func<T, bool> predicate)
            => Array.Find(array, predicate.Invoke);
        [Pure] public static T? Find<T>(this T[] array, [InstantHandle] Func<T, int, bool> predicate)
        {
            for (int i = 0, length = array.Length; i < length; i++)
                if (predicate(array[i], i))
                    return array[i];
            return default;
        }

        [Pure] public static T? FindLast<T>(this T[] array, [InstantHandle] Func<T, bool> predicate)
            => Array.FindLast(array, predicate.Invoke);
        [Pure] public static T? FindLast<T>(this T[] array, [InstantHandle] Func<T, int, bool> predicate)
        {
            for (int i = array.Length - 1; i >= 0; i--)
                if (predicate(array[i], i))
                    return array[i];
            return default;
        }

        [Pure] public static int FindIndex<T>(this T[] array, [InstantHandle] Func<T, bool> predicate)
            => Array.FindIndex(array, 0, array.Length, predicate.Invoke);
        [Pure] public static int FindIndex<T>(this T[] array, int startIndex, [InstantHandle] Func<T, bool> predicate)
            => Array.FindIndex(array, startIndex, array.Length - startIndex, predicate.Invoke);
        [Pure] public static int FindIndex<T>(this T[] array, int startIndex, int count, [InstantHandle] Func<T, bool> predicate)
            => Array.FindIndex(array, startIndex, count, predicate.Invoke);

        [Pure] public static int FindLastIndex<T>(this T[] array, [InstantHandle] Func<T, bool> predicate)
            => Array.FindLastIndex(array, 0, array.Length, predicate.Invoke);
        [Pure] public static int FindLastIndex<T>(this T[] array, int startIndex, [InstantHandle] Func<T, bool> predicate)
            => Array.FindLastIndex(array, startIndex, array.Length - startIndex, predicate.Invoke);
        [Pure] public static int FindLastIndex<T>(this T[] array, int startIndex, int count, [InstantHandle] Func<T, bool> predicate)
            => Array.FindLastIndex(array, startIndex, count, predicate.Invoke);

        public static void ForEach<T>(this T[] array, [InstantHandle] Action<T> action)
            => Array.ForEach(array, action);
        public static void ForEach<T>(this T[] array, [InstantHandle] Action<T, int> action)
        {
            for (int i = 0, length = array.Length; i < length; i++)
                action(array[i], i);
        }

        [Pure] public static T[] FindAll<T>(this T[] array, [InstantHandle] Func<T, bool> predicate)
            => Array.FindAll(array, predicate.Invoke);
        [Pure] public static T[] FindAll<T>(this T[] array, [InstantHandle] Func<T, int, bool> predicate)
        {
            List<T> list = new List<T>();
            for (int i = 0, length = array.Length; i < length; i++)
                if (predicate(array[i], i))
                    list.Add(array[i]);
            return list.ToArray();
        }

        [Pure] public static TResult[] ConvertAll<T, TResult>(this T[] array, [InstantHandle] Func<T, TResult> converter)
            => Array.ConvertAll(array, converter.Invoke);
        [Pure] public static TResult[] ConvertAll<T, TResult>(this T[] array, [InstantHandle] Func<T, int, TResult> converter)
        {
            int length = array.Length;
            TResult[] results = new TResult[length];
            for (int i = 0; i < length; i++)
                results[i] = converter(array[i], i);
            return results;
        }

        public static void Reverse<T>(this T[] array)
            => Array.Reverse(array);
        public static void Reverse<T>(this T[] array, int startIndex, int count)
            => Array.Reverse(array, startIndex, count);

        public static void Sort<T>(this T[] array)
            => Array.Sort(array);
        public static void Sort<T>(this T[] array, IComparer<T>? comparer)
            => Array.Sort(array, comparer);
        public static void Sort<T>(this T[] array, [InstantHandle] Func<T, T, int> comparison)
            => Array.Sort(array, Comparer<T>.Create(comparison.Invoke));
        public static void Sort<T>(this T[] array, int startIndex, int count)
            => Array.Sort(array, startIndex, count);
        public static void Sort<T>(this T[] array, int startIndex, int count, IComparer<T>? comparer)
            => Array.Sort(array, startIndex, count, comparer);
        public static void Sort<T>(this T[] array, int startIndex, int count, [InstantHandle] Func<T, T, int> comparison)
            => Array.Sort(array, startIndex, count, Comparer<T>.Create(comparison.Invoke));

        public static TResult[] Cast<TResult>(this Array array)
        {
            int length = array.Length;
            TResult[] result = new TResult[length];
            for (int i = 0; i < length; i++)
                result[i] = (TResult)array.GetValue(i)!;
            return result;
        }

    }
}
