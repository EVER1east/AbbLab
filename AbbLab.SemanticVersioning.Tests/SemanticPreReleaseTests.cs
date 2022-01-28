using System;
using Xunit;
using Xunit.Abstractions;

namespace AbbLab.SemanticVersioning.Tests
{
    public partial class SemanticPreReleaseTests
    {
        private readonly ITestOutputHelper Output;
        public SemanticPreReleaseTests(ITestOutputHelper output) => Output = output;

        [Fact]
        public void ConstructorTest()
        {
            Assert.Throws<ArgumentOutOfRangeException>(static () => new SemanticPreRelease(-1));
            Assert.Throws<ArgumentOutOfRangeException>(static () => new SemanticPreRelease(-123));
            Assert.Throws<ArgumentOutOfRangeException>(static () => new SemanticPreRelease(-2147483648));

            // Strings "-1", "-123" and "-2147483648" are valid though, because they're considered alphanumeric.
            // (see ParseTests)

            AssertEx.PreRelease(default, 0);
            AssertEx.PreRelease(new SemanticPreRelease(), 0);

        }

    }
}
