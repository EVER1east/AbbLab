using System;
using JetBrains.Annotations;

namespace AbbLab.Parsing
{
    /// <summary>
    ///   <para>Represents a simple, relatively fast string parser.</para>
    /// </summary>
    public ref struct StringParser
    {
        /// <summary>
        ///   <para>The source string to parse.</para>
        /// </summary>
        public readonly ReadOnlySpan<char> Source;
        /// <summary>
        ///   <para>The length of the source string.</para>
        /// </summary>
        public readonly int Length;
        /// <summary>
        ///   <para>The current position of the parser.</para>
        /// </summary>
        public int Position;

        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="StringParser"/> structure with the specified <paramref name="source"/> string.</para>
        /// </summary>
        /// <param name="source">The string to parse.</param>
        public StringParser(string source)
            : this(source.AsSpan()) { }
        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="StringParser"/> structure with the specified <paramref name="source"/> read-only span of characters.</para>
        /// </summary>
        /// <param name="source">The read-only span of characters to parse.</param>
        public StringParser(ReadOnlySpan<char> source)
        {
            Source = source;
            Length = source.Length;
            Position = 0;
        }

        [Pure] public readonly bool CanRead() => Position < Length;

        public readonly int Current => Position < Length ? Source[Position] : -1;
        [Pure] public readonly int Peek() => Position < Length ? Source[Position] : -1;
        [Pure] public readonly int PeekBack() => Position > 0 ? Source[Position - 1] : -1;

        public int Read() => Position < Length ? Source[Position++] : -1;
        public bool TryRead(out char read)
        {
            if (Position < Length)
            {
                read = Source[Position++];
                return true;
            }
            read = default;
            return false;
        }
        public void Skip()
        {
            if (Position < Length) Position++;
        }

        public bool Skip(int character)
        {
            if (Position >= Length || Source[Position] != character) return false;
            Position++;
            return true;
        }
        public bool SkipAny(int a, int b)
        {
            if (Position >= Length) return false;
            int cur = Source[Position];
            bool res = cur == a || cur == b;
            if (res) Position++;
            return res;
        }
        public bool SkipAny(int a, int b, int c)
        {
            if (Position >= Length) return false;
            int cur = Source[Position];
            bool res = cur == a || cur == b || cur == c;
            if (res) Position++;
            return res;
        }
        public bool SkipAny(int a, int b, int c, int d)
        {
            if (Position >= Length) return false;
            int cur = Source[Position];
            bool res = cur == a || cur == b || cur == c || cur == d;
            if (res) Position++;
            return res;
        }
        public bool SkipAny(int a, int b, int c, int d, int e)
        {
            if (Position >= Length) return false;
            int cur = Source[Position];
            bool res = cur == a || cur == b || cur == c || cur == d || cur == e;
            if (res) Position++;
            return res;
        }
        public bool SkipAny(int a, int b, int c, int d, int e, int f)
        {
            if (Position >= Length) return false;
            int cur = Source[Position];
            bool res = cur == a || cur == b || cur == c || cur == d || cur == e || cur == f;
            if (res) Position++;
            return res;
        }

        public bool SkipAny(int a, int b, out int skipped)
        {
            if (Position >= Length)
            {
                skipped = default;
                return false;
            }
            skipped = Source[Position];
            bool res = skipped == a || skipped == b;
            if (res) Position++;
            else skipped = default;
            return res;
        }
        public bool SkipAny(int a, int b, int c, out int skipped)
        {
            if (Position >= Length)
            {
                skipped = default;
                return false;
            }
            skipped = Source[Position];
            bool res = skipped == a || skipped == b || skipped == c;
            if (res) Position++;
            else skipped = default;
            return res;
        }
        public bool SkipAny(int a, int b, int c, int d, out int skipped)
        {
            if (Position >= Length)
            {
                skipped = default;
                return false;
            }
            skipped = Source[Position];
            bool res = skipped == a || skipped == b || skipped == c || skipped == d;
            if (res) Position++;
            else skipped = default;
            return res;
        }
        public bool SkipAny(int a, int b, int c, int d, int e, out int skipped)
        {
            if (Position >= Length)
            {
                skipped = default;
                return false;
            }
            skipped = Source[Position];
            bool res = skipped == a || skipped == b || skipped == c || skipped == d || skipped == e;
            if (res) Position++;
            else skipped = default;
            return res;
        }
        public bool SkipAny(int a, int b, int c, int d, int e, int f, out int skipped)
        {
            if (Position >= Length)
            {
                skipped = default;
                return false;
            }
            skipped = Source[Position];
            bool res = skipped == a || skipped == b || skipped == c || skipped == d || skipped == e || skipped == f;
            if (res) Position++;
            else skipped = default;
            return res;
        }

        public void SkipAll(int character)
        {
            while (Position < Length && Source[Position] == character) Position++;
        }
        public int SkipCountAll(int character)
        {
            int start = Position;
            while (Position < Length && Source[Position] == character) Position++;
            return Position - start;
        }

        public bool SkipConsecutive(int a, int b)
        {
            int posB = Position + 1;
            if (posB < Length && Source[Position] == a && Source[posB] == b)
            {
                Position = posB + 1;
                return true;
            }
            return false;
        }
        public bool SkipConsecutive(int a, int b, int c)
        {
            int next = Position;
            if (Position + 2 < Length && Source[next] == a && Source[++next] == b && Source[++next] == c)
            {
                Position = next + 1;
                return true;
            }
            return false;
        }
        public bool SkipConsecutive(int a, int b, int c, int d)
        {
            int next = Position;
            if (Position + 3 < Length && Source[next] == a && Source[++next] == b && Source[++next] == c
                && Source[++next] == d)
            {
                Position = next + 1;
                return true;
            }
            return false;
        }
        public bool SkipConsecutive(int a, int b, int c, int d, int e)
        {
            int next = Position;
            if (Position + 4 < Length && Source[next] == a && Source[++next] == b && Source[++next] == c
                && Source[++next] == d && Source[++next] == e)
            {
                Position = next + 1;
                return true;
            }
            return false;
        }
        public bool SkipConsecutive(int a, int b, int c, int d, int e, int f)
        {
            int next = Position;
            if (Position + 5 < Length && Source[next] == a && Source[++next] == b && Source[++next] == c
                && Source[++next] == d && Source[++next] == e && Source[++next] == f)
            {
                Position = next + 1;
                return true;
            }
            return false;
        }

        public unsafe void SkipWhile(delegate*<int, bool> predicate)
        {
            while (Position < Length && predicate(Source[Position])) Position++;
        }
        public unsafe int SkipCountWhile(delegate*<int, bool> predicate)
        {
            int start = Position;
            while (Position < Length && predicate(Source[Position])) Position++;
            return Position - start;
        }
        public unsafe void SkipUntil(delegate*<int, bool> predicate)
        {
            while (Position < Length && !predicate(Source[Position])) Position++;
        }
        public unsafe int SkipCountUntil(delegate*<int, bool> predicate)
        {
            int start = Position;
            while (Position < Length && !predicate(Source[Position])) Position++;
            return Position - start;
        }

        public void SkipWhitespaces()
        {
            while (Position < Length && char.IsWhiteSpace(Source[Position])) Position++;
        }

#pragma warning disable IDE0057 // Use range operator
        // Using Range introduces extra local variables in CIL, increasing the method size;
        // That probably increases execution time, although I haven't tested that yet.
        // TODO: benchmark these methods with and without Range operators.

        public ReadOnlySpan<char> Read(int maxLength)
        {
            int remainingLength = Length - Position;
            if (maxLength > remainingLength) maxLength = remainingLength;
            ReadOnlySpan<char> span = Source.Slice(Position, maxLength);
            Position += maxLength;
            return span;
        }
        public string ReadString(int maxLength)
        {
            int remainingLength = Length - Position;
            if (maxLength > remainingLength) maxLength = remainingLength;
            string span = new string(Source.Slice(Position, maxLength));
            Position += maxLength;
            return span;
        }

        public unsafe ReadOnlySpan<char> ReadWhile(delegate*<int, bool> predicate)
        {
            int start = Position;
            while (Position < Length && predicate(Source[Position])) Position++;
            if (Position == start) return ReadOnlySpan<char>.Empty;
            return Source.Slice(start, Position - start);
        }
        public unsafe string ReadStringWhile(delegate*<int, bool> predicate)
        {
            int start = Position;
            while (Position < Length && predicate(Source[Position])) Position++;
            return new string(Source.Slice(start, Position - start));
        }
        public unsafe ReadOnlySpan<char> ReadUntil(delegate*<int, bool> predicate)
        {
            int start = Position;
            while (Position < Length && !predicate(Source[Position])) Position++;
            return Source.Slice(start, Position - start);
        }
        public unsafe string ReadStringUntil(delegate*<int, bool> predicate)
        {
            int start = Position;
            while (Position < Length && !predicate(Source[Position])) Position++;
            return new string(Source.Slice(start, Position - start));
        }

        public ReadOnlySpan<char> ReadRemaining()
        {
            ReadOnlySpan<char> span = Source.Slice(Position, Length - Position);
            Position = Length;
            return span;
        }
        public string ReadRemainingString()
        {
            string span = new string(Source.Slice(Position, Length - Position));
            Position = Length;
            return span;
        }

    }
}
