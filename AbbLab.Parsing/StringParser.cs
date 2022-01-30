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

        /// <summary>
        ///   <para>Determines whether the parser can still read characters from the source string.</para>
        /// </summary>
        /// <returns><see langword="true"/>, if <see cref="Position"/> is less than <see cref="Length"/>; otherwise, <see langword="false"/>.</returns>
        [Pure] public readonly bool CanRead() => Position < Length;

        /// <summary>
        ///   <para>Returns the character the parser is currently on, or -1, if it reached the end of the string.</para>
        /// </summary>
        public readonly int Current => Position < Length ? Source[Position] : -1;
        /// <summary>
        ///   <para>Returns the character the parser is currently on, or -1, if it reached the end of the string.</para>
        /// </summary>
        /// <returns>The character the parser is currently on, or -1, if it reached the end of the string.</returns>
        [Pure] public readonly int Peek() => Position < Length ? Source[Position] : -1;
        /// <summary>
        ///   <para>Returns the character behind the one the parser is currently on, or -1, if the parser is on the first character.</para>
        /// </summary>
        /// <returns>The character behind the one the parser is currently on, or -1, if the parser is on the first character.</returns>
        [Pure] public readonly int PeekBack() => Position > 0 ? Source[Position - 1] : -1;

        /// <summary>
        ///   <para>Returns the character the parser is currently on and moves forward, or -1, if it reached the end of the string.</para>
        /// </summary>
        /// <returns>The character the parser was on, or -1, if it was at the end of the string.</returns>
        public int Read() => Position < Length ? Source[Position++] : -1;
        /// <summary>
        ///   <para>Tries to read the next character and returns the value indicating whether the character was successfully read.</para>
        /// </summary>
        /// <param name="read">When this method returns, contains the character read by the parser, if read successfully, or <see langword="default"/>, if no character was read.</param>
        /// <returns><see langword="true"/>, if the character was successfully read; otherwise, <see langword="false"/>.</returns>
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
        /// <summary>
        ///   <para>Moves the parser forward, if it isn't at the end of the string already.</para>
        /// </summary>
        public void Skip()
        {
            if (Position < Length) Position++;
        }

        /// <summary>
        ///   <para>Tries to read the following <paramref name="character"/> and move the parser forward, and returns a value indicating whether the operation was successful.</para>
        /// </summary>
        /// <param name="character">The character to expect the parser to be on.</param>
        /// <returns><see langword="true"/>, if the parser was on the specified <paramref name="character"/> and it moved forward; otherwise, <see langword="false"/>.</returns>
        public bool Skip(int character)
        {
            if (Position >= Length || Source[Position] != character) return false;
            Position++;
            return true;
        }
        /// <inheritdoc cref="SkipAny(int,int,int,int,int,int)"/>
        public bool SkipAny(int a, int b)
        {
            if (Position >= Length) return false;
            int cur = Source[Position];
            bool res = cur == a || cur == b;
            if (res) Position++;
            return res;
        }
        /// <inheritdoc cref="SkipAny(int,int,int,int,int,int)"/>
        public bool SkipAny(int a, int b, int c)
        {
            if (Position >= Length) return false;
            int cur = Source[Position];
            bool res = cur == a || cur == b || cur == c;
            if (res) Position++;
            return res;
        }
        /// <inheritdoc cref="SkipAny(int,int,int,int,int,int)"/>
        public bool SkipAny(int a, int b, int c, int d)
        {
            if (Position >= Length) return false;
            int cur = Source[Position];
            bool res = cur == a || cur == b || cur == c || cur == d;
            if (res) Position++;
            return res;
        }
        /// <inheritdoc cref="SkipAny(int,int,int,int,int,int)"/>
        public bool SkipAny(int a, int b, int c, int d, int e)
        {
            if (Position >= Length) return false;
            int cur = Source[Position];
            bool res = cur == a || cur == b || cur == c || cur == d || cur == e;
            if (res) Position++;
            return res;
        }
        /// <summary>
        ///   <para>Tries to read one of the specified characters and move the parser forward, and returns a value indicating whether the operation was successful.</para>
        /// </summary>
        /// <param name="a">The first of the characters to expect the parser to be on.</param>
        /// <param name="b">The second of the characters to expect the parser to be on.</param>
        /// <param name="c">The third of the characters to expect the parser to be on.</param>
        /// <param name="d">The fourth of the characters to expect the parser to be on.</param>
        /// <param name="e">The fifth of the characters to expect the parser to be on.</param>
        /// <param name="f">The sixth of the characters to expect the parser to be on.</param>
        /// <returns><see langword="true"/>, if the parser was on one of the specified characters and it moved forward; otherwise, <see langword="false"/>.</returns>
        public bool SkipAny(int a, int b, int c, int d, int e, int f)
        {
            if (Position >= Length) return false;
            int cur = Source[Position];
            bool res = cur == a || cur == b || cur == c || cur == d || cur == e || cur == f;
            if (res) Position++;
            return res;
        }

        /// <inheritdoc cref="SkipAny(int,int,int,int,int,int,out int)"/>
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
        /// <inheritdoc cref="SkipAny(int,int,int,int,int,int,out int)"/>
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
        /// <inheritdoc cref="SkipAny(int,int,int,int,int,int,out int)"/>
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
        /// <inheritdoc cref="SkipAny(int,int,int,int,int,int,out int)"/>
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
        /// <summary>
        ///   <para>Tries to read one of the specified characters and move the parser forward, and returns a value indicating whether the operation was successful.</para>
        /// </summary>
        /// <param name="a">The first of the characters to expect the parser to be on.</param>
        /// <param name="b">The second of the characters to expect the parser to be on.</param>
        /// <param name="c">The third of the characters to expect the parser to be on.</param>
        /// <param name="d">The fourth of the characters to expect the parser to be on.</param>
        /// <param name="e">The fifth of the characters to expect the parser to be on.</param>
        /// <param name="f">The sixth of the characters to expect the parser to be on.</param>
        /// <param name="skipped">When this method returns, contains the character that was read, if the operation was successful, or <see langword="default"/>, if the operation was unsuccessful.</param>
        /// <returns><see langword="true"/>, if the parser was on one of the specified characters and it moved forward; otherwise, <see langword="false"/>.</returns>
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

        /// <summary>
        ///   <para>Moves the parser forward while the current character is equal to the specified <paramref name="character"/>.</para>
        /// </summary>
        /// <param name="character">The character to expect the parser to be on.</param>
        public void SkipAll(int character)
        {
            while (Position < Length && Source[Position] == character) Position++;
        }
        /// <summary>
        ///   <para>Moves the parser forward while the current character is equal to the specified <paramref name="character"/> and returns the amount of characters that the parser skipped.</para>
        /// </summary>
        /// <param name="character">The character to expect the parser to be on.</param>
        /// <returns>The amount of characters that the parser skipped.</returns>
        public int SkipCountAll(int character)
        {
            int start = Position;
            while (Position < Length && Source[Position] == character) Position++;
            return Position - start;
        }

        /// <inheritdoc cref="SkipConsecutive(int,int,int,int,int,int)"/>
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
        /// <inheritdoc cref="SkipConsecutive(int,int,int,int,int,int)"/>
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
        /// <inheritdoc cref="SkipConsecutive(int,int,int,int,int,int)"/>
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
        /// <inheritdoc cref="SkipConsecutive(int,int,int,int,int,int)"/>
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
        /// <summary>
        ///   <para>Tries to read the specified sequence of characters consecutively and moves the parser after the end of the sequence, and returns a value indicating whether the operation was successful.</para>
        /// </summary>
        /// <param name="a">The first character of the sequence to expect.</param>
        /// <param name="b">The second character of the sequence to expect.</param>
        /// <param name="c">The third character of the sequence to expect.</param>
        /// <param name="d">The fourth character of the sequence to expect.</param>
        /// <param name="e">The fifth character of the sequence to expect.</param>
        /// <param name="f">The sixth character of the sequence to expect.</param>
        /// <returns><see langword="true"/>, if the specified sequence of characters was successfully read and the parser moved forward; otherwise, <see langword="false"/>.</returns>
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

        /// <summary>
        ///   <para>Moves the parser forward while the specified <paramref name="predicate"/> returns <see langword="true"/> for the current character.</para>
        /// </summary>
        /// <param name="predicate">A pointer to a function defining the conditions of the characters to skip.</param>
        public unsafe void SkipWhile(delegate*<int, bool> predicate)
        {
            while (Position < Length && predicate(Source[Position])) Position++;
        }
        /// <summary>
        ///   <para>Moves the parser forward while the specified <paramref name="predicate"/> returns <see langword="true"/> for the current character, and returns the amount of characters that the parser skipped.</para>
        /// </summary>
        /// <param name="predicate">A pointer to a function defining the conditions of the characters to skip.</param>
        /// <returns>The amount of characters that the parser skipped.</returns>
        public unsafe int SkipCountWhile(delegate*<int, bool> predicate)
        {
            int start = Position;
            while (Position < Length && predicate(Source[Position])) Position++;
            return Position - start;
        }
        /// <summary>
        ///   <para>Moves the parser forward while the specified <paramref name="predicate"/> returns <see langword="false"/> for the current character.</para>
        /// </summary>
        /// <param name="predicate">A pointer to a function defining the conditions of the characters to stop at.</param>
        public unsafe void SkipUntil(delegate*<int, bool> predicate)
        {
            while (Position < Length && !predicate(Source[Position])) Position++;
        }
        /// <summary>
        ///   <para>Moves the parser forward while the specified <paramref name="predicate"/> returns <see langword="false"/> for the current character.</para>
        /// </summary>
        /// <param name="predicate">A pointer to a function defining the conditions of the characters to stop at.</param>
        /// <returns>The amount of characters that the parser skipped.</returns>
        public unsafe int SkipCountUntil(delegate*<int, bool> predicate)
        {
            int start = Position;
            while (Position < Length && !predicate(Source[Position])) Position++;
            return Position - start;
        }

        /// <summary>
        ///   <para>Moves the parser forward while the current character is a whitespace.</para>
        /// </summary>
        public void SkipWhitespaces()
        {
            while (Position < Length && char.IsWhiteSpace(Source[Position])) Position++;
        }

#pragma warning disable IDE0057 // Use range operator
        // Using Range introduces extra local variables in CIL, increasing the method size;
        // That probably increases execution time, although I haven't tested that yet.
        // TODO: benchmark these methods with and without Range operators.

        /// <summary>
        ///   <para>Reads a span of characters of the specified maximum length from the source string.</para>
        /// </summary>
        /// <param name="maxLength">The maximum length of the span of characters to read.</param>
        /// <returns>The read-only span of characters read by the parser.</returns>
        public ReadOnlySpan<char> Read(int maxLength)
        {
            int remainingLength = Length - Position;
            if (maxLength > remainingLength) maxLength = remainingLength;
            ReadOnlySpan<char> span = Source.Slice(Position, maxLength);
            Position += maxLength;
            return span;
        }
        /// <summary>
        ///   <para>Reads a string of the specified maximum length from the source string.</para>
        /// </summary>
        /// <param name="maxLength">The maximum length of the string to read.</param>
        /// <returns>The string read by the parser.</returns>
        public string ReadString(int maxLength)
        {
            int remainingLength = Length - Position;
            if (maxLength > remainingLength) maxLength = remainingLength;
            string span = new string(Source.Slice(Position, maxLength));
            Position += maxLength;
            return span;
        }

        /// <summary>
        ///   <para>Reads characters while the specified <paramref name="predicate"/> returns <see langword="true"/> for the current character, and returns the read span of characters.</para>
        /// </summary>
        /// <param name="predicate">A pointer to a function defining the conditions of the characters to read.</param>
        /// <returns>The read-only span of characters read by the parser.</returns>
        public unsafe ReadOnlySpan<char> ReadWhile(delegate*<int, bool> predicate)
        {
            int start = Position;
            while (Position < Length && predicate(Source[Position])) Position++;
            if (Position == start) return ReadOnlySpan<char>.Empty;
            return Source.Slice(start, Position - start);
        }
        /// <summary>
        ///   <para>Reads characters while the specified <paramref name="predicate"/> returns <see langword="true"/> for the current character, and returns the read string.</para>
        /// </summary>
        /// <param name="predicate">A pointer to a function defining the conditions of the characters to read.</param>
        /// <returns>The string read by the parser.</returns>
        public unsafe string ReadStringWhile(delegate*<int, bool> predicate)
        {
            int start = Position;
            while (Position < Length && predicate(Source[Position])) Position++;
            return new string(Source.Slice(start, Position - start));
        }
        /// <summary>
        ///   <para>Reads characters while the specified <paramref name="predicate"/> returns <see langword="false"/> for the current character, and return the read span of characters.</para>
        /// </summary>
        /// <param name="predicate">A pointer to a function defining the conditions of the characters to stop reading at.</param>
        /// <returns>The read-only span of characters read by the parser.</returns>
        public unsafe ReadOnlySpan<char> ReadUntil(delegate*<int, bool> predicate)
        {
            int start = Position;
            while (Position < Length && !predicate(Source[Position])) Position++;
            return Source.Slice(start, Position - start);
        }
        /// <summary>
        ///   <para>Reads characters while the specified <paramref name="predicate"/> returns <see langword="false"/> for the current character, and return the read string.</para>
        /// </summary>
        /// <param name="predicate">A pointer to a function defining the conditions of the characters to stop reading at.</param>
        /// <returns>The string read by the parser.</returns>
        public unsafe string ReadStringUntil(delegate*<int, bool> predicate)
        {
            int start = Position;
            while (Position < Length && !predicate(Source[Position])) Position++;
            return new string(Source.Slice(start, Position - start));
        }

        /// <summary>
        ///   <para>Reads all of the remaining characters.</para>
        /// </summary>
        /// <returns>The read-only span of characters read by the parser.</returns>
        public ReadOnlySpan<char> ReadRemaining()
        {
            ReadOnlySpan<char> span = Source.Slice(Position, Length - Position);
            Position = Length;
            return span;
        }
        /// <summary>
        ///   <para>Reads all of the remaining characters.</para>
        /// </summary>
        /// <returns>The string read by the parser.</returns>
        public string ReadRemainingString()
        {
            string span = new string(Source.Slice(Position, Length - Position));
            Position = Length;
            return span;
        }

    }
}
