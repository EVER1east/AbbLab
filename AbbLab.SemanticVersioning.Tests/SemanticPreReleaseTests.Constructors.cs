using System;
using System.Globalization;
using Xunit;

namespace AbbLab.SemanticVersioning.Tests
{
    public partial class SemanticPreReleaseTests
    {
        [Fact]
        public void ConstructorTest()
        {

        }

        [Theory]
        [MemberData(nameof(ParseFixtures))]
        public void ConstructorTests(ParseFixture test)
        {
            if (test.IsValid)
            {
                Util.AssertPreRelease(test.Input, test.Value);
                Util.AssertPreRelease(new SemanticPreRelease(test.Input), test.Value);
                Util.AssertPreRelease(new SemanticPreRelease(test.Input, SemanticOptions.Strict), test.Value);

                Util.AssertPreRelease(test.Input.AsSpan(), test.Value, false);
                Util.AssertPreRelease(new SemanticPreRelease(test.Input.AsSpan()), test.Value, false);
                Util.AssertPreRelease(new SemanticPreRelease(test.Input.AsSpan(), SemanticOptions.Strict), test.Value, false);
            }
            else
            {
                Assert.Throws<ArgumentException>(() => (SemanticPreRelease)test.Input);
                Assert.Throws<ArgumentException>(() => new SemanticPreRelease(test.Input));
                Assert.Throws<ArgumentException>(() => new SemanticPreRelease(test.Input, SemanticOptions.Strict));

                Assert.Throws<ArgumentException>(() => (SemanticPreRelease)test.Input.AsSpan());
                Assert.Throws<ArgumentException>(() => new SemanticPreRelease(test.Input.AsSpan()));
                Assert.Throws<ArgumentException>(() => new SemanticPreRelease(test.Input.AsSpan(), SemanticOptions.Strict));
            }
            if (test.IsValidLoose)
            {
                Util.AssertPreRelease(new SemanticPreRelease(test.Input, SemanticOptions.Loose), test.Value);
                Util.AssertPreRelease(new SemanticPreRelease(test.Input.AsSpan(), SemanticOptions.Loose), test.Value, false);
            }
            else
            {
                Assert.Throws<ArgumentException>(() => new SemanticPreRelease(test.Input, SemanticOptions.Loose));
                Assert.Throws<ArgumentException>(() => new SemanticPreRelease(test.Input.AsSpan(), SemanticOptions.Loose));
            }

        }

    }
    public static class Ex
    {
        public static TResult ThrowsIf<TResult, TException>(bool condition, Func<TResult> function) where TException : Exception
        {
            if (condition)
            {
                TResult res = default!;
                Assert.Throws<TException>(() => res = function());
                return res;
            }
            else return function();
        }
    }
}
