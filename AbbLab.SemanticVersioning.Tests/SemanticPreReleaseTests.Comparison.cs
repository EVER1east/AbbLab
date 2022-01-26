using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AbbLab.SemanticVersioning.Tests
{
    public partial class SemanticPreReleaseTests
    {
        [Fact]
        public void ComparisonTest()
        {
            List<SemanticPreRelease> preReleases = SortFixture.Select(SemanticPreRelease.Parse).ToList();
            List<SemanticPreRelease> sorted = preReleases.ToList();
            sorted.Sort();
            Assert.Equal(preReleases, sorted); // in the same order

            int count = preReleases.Count;
            for (int i = 0; i < count; i++)
            {
                SemanticPreRelease me = preReleases[i];
                for (int j = 0; j < i; j++)
                {
                    // greater than any of the previous identifiers
                    SemanticPreRelease other = preReleases[j];

                    Assert.True(me.CompareTo(other) > 0);
                    Assert.True(((IComparable)me).CompareTo(other) > 0);
                    Assert.False(me.Equals(other));
                    Assert.False(me.Equals((object?)other));

                    Assert.True(me > other);
                    Assert.True(me >= other);
                    Assert.False(me < other);
                    Assert.False(me <= other);
                    Assert.True(me != other);
                    Assert.False(me == other);
                }
                for (int j = i + 1; j < count; j++)
                {
                    // less than any of the subsequent identifiers
                    SemanticPreRelease other = preReleases[j];

                    Assert.True(me.CompareTo(other) < 0);
                    Assert.True(((IComparable)me).CompareTo(other) < 0);
                    Assert.False(me.Equals(other));
                    Assert.False(me.Equals((object?)other));

                    Assert.False(me > other);
                    Assert.False(me >= other);
                    Assert.True(me < other);
                    Assert.True(me <= other);
                    Assert.True(me != other);
                    Assert.False(me == other);
                }
                // equal to self
                Assert.True(me.CompareTo(me) == 0);
                Assert.True(((IComparable)me).CompareTo(me) == 0);
                Assert.True(me.Equals(me));
                Assert.True(me.Equals((object)me));
                Assert.Equal(me.GetHashCode(), me.GetHashCode());

                Assert.Throws<ArgumentException>(() => ((IComparable)me).CompareTo(0.0));

#pragma warning disable CS1718 // Comparison made to same variable
                // ReSharper disable EqualExpressionComparison
                Assert.False(me > me);
                Assert.True(me >= me);
                Assert.False(me < me);
                Assert.True(me <= me);
                Assert.False(me != me);
                Assert.True(me == me);
#pragma warning restore CS1718 // Comparison made to same variable
                // ReSharper restore EqualExpressionComparison

            }
        }

        public static readonly string[] SortFixture =
        {
            "0",
            "1",
            "2",
            "10",
            "100",
            "2147483647",
            "a",
            "alpha",
            "b",
            "beta",
            "dev",
            "nightly",
            "rc",
            "zzz",
        };
    }
}
