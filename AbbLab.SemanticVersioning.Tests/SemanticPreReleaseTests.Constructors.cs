using System;
using System.Globalization;
using Xunit;

namespace AbbLab.SemanticVersioning.Tests
{
    public partial class SemanticPreReleaseTests
    {
        [Fact]
        public void ConstructorsTest()
        {
            int[] numbers = { 0, 1, 23, 4056, 2147483647 };
            string[] numberStrings = Array.ConvertAll(numbers, static n => n.ToString(CultureInfo.InvariantCulture));

            for (int i = 0, length = numbers.Length; i < length; i++)
            {
                AssertPreRelease(new SemanticPreRelease(numbers[i]), numbers[i]);
                AssertPreRelease(numbers[i], numbers[i]); // implicit

                AssertPreRelease(new SemanticPreRelease(numberStrings[i]), numbers[i]);
                AssertPreRelease(new SemanticPreRelease(numberStrings[i], SemanticOptions.Strict), numbers[i]);
                AssertPreRelease(new SemanticPreRelease(numberStrings[i], SemanticOptions.Loose), numbers[i]);
                AssertPreRelease(numberStrings[i], numbers[i]); // implicit
            }

            AssertPreRelease(new SemanticPreRelease("003", SemanticOptions.Loose), 3);
            Assert.Throws<ArgumentException>(static () => new SemanticPreRelease("003"));
            Assert.Throws<ArgumentException>(static () => new SemanticPreRelease("003", SemanticOptions.Strict));
            Assert.Throws<ArgumentException>(static () => (SemanticPreRelease)"003");

            Assert.Throws<ArgumentException>(static () => new SemanticPreRelease("2147483648"));
            Assert.Throws<ArgumentException>(static () => new SemanticPreRelease("2147483648", SemanticOptions.Strict));
            Assert.Throws<ArgumentException>(static () => new SemanticPreRelease("2147483648", SemanticOptions.Loose));
            Assert.Throws<ArgumentException>(static () => (SemanticPreRelease)"2147483648");

            Assert.Throws<ArgumentOutOfRangeException>(static () => new SemanticPreRelease(-1));
            Assert.Throws<ArgumentOutOfRangeException>(static () => (SemanticPreRelease)(-1));

            string[] identifiers = { "-1", "alpha", "00alpha", "-00" };
            for (int i = 0, length = identifiers.Length; i < length; i++)
            {
                string str = identifiers[i];
                AssertPreReleaseString(new SemanticPreRelease(str), str);
                AssertPreReleaseString(new SemanticPreRelease(str, SemanticOptions.Strict), str);
                AssertPreReleaseString(new SemanticPreRelease(str, SemanticOptions.Loose), str);
                AssertPreReleaseString(str, str); // implicit
                ReadOnlySpan<char> span = str.AsSpan();
                AssertPreReleaseSpan(new SemanticPreRelease(span), str);
                AssertPreReleaseSpan(new SemanticPreRelease(span, SemanticOptions.Strict), str);
                AssertPreReleaseSpan(new SemanticPreRelease(span, SemanticOptions.Loose), str);
                AssertPreReleaseSpan(span, str); // implicit
            }

            Assert.Throws<ArgumentException>(static () => new SemanticPreRelease(""));
            Assert.Throws<ArgumentException>(static () => new SemanticPreRelease("", SemanticOptions.Strict));
            Assert.Throws<ArgumentException>(static () => new SemanticPreRelease("", SemanticOptions.Loose));
            Assert.Throws<ArgumentException>(static () => (SemanticPreRelease)"");
            Assert.Throws<ArgumentException>(static () => new SemanticPreRelease("invalid$$"));
            Assert.Throws<ArgumentException>(static () => new SemanticPreRelease("invalid$$", SemanticOptions.Strict));
            Assert.Throws<ArgumentException>(static () => new SemanticPreRelease("invalid$$", SemanticOptions.Loose));
            Assert.Throws<ArgumentException>(static () => (SemanticPreRelease)"invalid$$");

        }

        private static void AssertPreRelease(SemanticPreRelease preRelease, int value)
        {
            Assert.True(preRelease.IsNumeric);
            Assert.Equal(value, preRelease.Number);
            Assert.Equal(value, (int)preRelease);
            Assert.Throws<InvalidOperationException>(() => preRelease.Text);
        }
        private static void AssertPreReleaseString(SemanticPreRelease preRelease, string value)
        {
            Assert.False(preRelease.IsNumeric);
            Assert.Equal(value, preRelease.Text);
            Assert.Equal(value, (string)preRelease);
            Assert.Same(value, preRelease.Text); // no need to allocate a new string
            Assert.Same(value, (string)preRelease);
            Assert.Throws<InvalidOperationException>(() => preRelease.Number);
        }
        private static void AssertPreReleaseSpan(SemanticPreRelease preRelease, string value)
        {
            Assert.False(preRelease.IsNumeric);
            Assert.Equal(value, preRelease.Text);
            Assert.Equal(value, (string)preRelease);
            Assert.NotSame(value, preRelease.Text); // has to allocate a new string
            Assert.NotSame(value, (string)preRelease);
            Assert.Throws<InvalidOperationException>(() => preRelease.Number);
        }

    }
}
