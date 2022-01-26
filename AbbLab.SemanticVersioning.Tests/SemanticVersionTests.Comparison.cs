using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AbbLab.SemanticVersioning.Tests
{
    public partial class SemanticVersionTests
    {
        [Fact]
        public void ComparisonTest()
        {
            List<SemanticVersion?> versions = SortFixture.Select(static v => v is null ? null : SemanticVersion.Parse(v)).ToList();
            List<SemanticVersion?> sorted = versions.ToList();
            sorted.Sort();
            Assert.Equal(versions, sorted); // in the same order

            int count = versions.Count;
            for (int i = 0; i < count; i++)
            {
                SemanticVersion? me = versions[i];
                for (int j = 0; j < i; j++)
                {
                    // greater than any of the previous versions
                    SemanticVersion? other = versions[j];
                    if (me is not null)
                    {
                        Assert.True(me.CompareTo(other) > 0);
                        Assert.True(((IComparable)me).CompareTo(other) > 0);
                        Assert.False(me.Equals(other));
                        Assert.False(me.Equals((object?)other));
                    }
                    Assert.True(me > other);
                    Assert.True(me >= other);
                    Assert.False(me < other);
                    Assert.False(me <= other);
                    Assert.True(me != other);
                    Assert.False(me == other);
                }
                for (int j = i + 1; j < count; j++)
                {
                    // less than any of the subsequent versions
                    SemanticVersion? other = versions[j];
                    if (me is not null)
                    {
                        Assert.True(me.CompareTo(other) < 0);
                        Assert.True(((IComparable)me).CompareTo(other) < 0);
                        Assert.False(me.Equals(other));
                        Assert.False(me.Equals((object?)other));
                    }
                    Assert.False(me > other);
                    Assert.False(me >= other);
                    Assert.True(me < other);
                    Assert.True(me <= other);
                    Assert.True(me != other);
                    Assert.False(me == other);
                }
                // equal to self
                if (me is not null)
                {
                    Assert.True(me.CompareTo(me) == 0);
                    Assert.True(((IComparable)me).CompareTo(me) == 0);
                    Assert.True(me.Equals(me));
                    Assert.True(me.Equals((object)me));
                    Assert.Equal(me.GetHashCode(), me.GetHashCode());

                    SemanticVersion clone = SemanticVersion.Parse(SortFixture[i]!);
                    Assert.True(me.CompareTo(clone) == 0);
                    Assert.True(((IComparable)me).CompareTo(clone) == 0);
                    Assert.True(me.Equals(clone));
                    Assert.True(me.Equals((object)clone));
                    Assert.Equal(me.GetHashCode(), clone.GetHashCode());

                    Assert.Throws<ArgumentException>(() => ((IComparable)me).CompareTo(0.0));
                }
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

        public static readonly string?[] SortFixture =
        {
            null,

            "0.0.0-0",
            "0.0.0-1",
            "0.0.0-45",
            "0.0.0-alpha",
            "0.0.0-alpha.0",
            "0.0.0-alpha.1",
            "0.0.0-alpha.beta",
            "0.0.0-rc.0",
            "0.0.0-rc.0.0",
            "0.0.0-rc.0.1",
            "0.0.0-rc.1.0",
            "0.0.0-rc.1.1",
            "0.0.0",
            "0.0.1-alpha",
            "0.0.1",
            "0.0.2-5",
            "0.0.2",
            "0.1.0-dev",
            "0.1.0",

            "1.0.0-beta.6",
            "1.0.0",
            "1.0.1-beta",
            "1.0.1-rc.1",
            "1.0.1",
            "1.1.0-alpha",
            "1.1.0-alpha.beta",
            "1.1.0-alpha.beta.7",

            "2.0.0-0",
            "2.0.0-0.dev",
            "2.0.0-0.dev.0",
            "2.0.0-0.dev.5",
            "2.0.0-1",
            "2.0.0-1.rc",
            "2.0.0-rc",
            "2.0.0",

        };

    }
}
