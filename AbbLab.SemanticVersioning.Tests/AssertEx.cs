using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AbbLab.SemanticVersioning.Tests
{
    public static class AssertEx
    {
        public static void Version(SemanticVersion version, int major, int minor, int patch,
                                   IEnumerable<SemanticPreRelease>? preReleases = null,
                                   IEnumerable<string>? buildMetadata = null)
        {
            Assert.Equal(major, version.Major);
            Assert.Equal(minor, version.Minor);
            Assert.Equal(patch, version.Patch);
            if (preReleases is null) Assert.Empty(version.PreReleases);
            else Assert.Equal(preReleases, version.PreReleases);
            if (buildMetadata is null) Assert.Empty(version.BuildMetadata);
            else Assert.Equal(buildMetadata, version.BuildMetadata);
        }
        public static void Builder(SemanticVersionBuilder builder, int major, int minor, int patch,
                                   IEnumerable<SemanticPreRelease>? preReleases = null,
                                   IEnumerable<string>? buildMetadata = null)
        {
            SemanticPreRelease[]? preReleasesArray = preReleases?.ToArray();
            string[]? buildMetadataArray = buildMetadata?.ToArray();

            Version(builder.ToVersion(), major, minor, patch, preReleasesArray, buildMetadataArray);

            Assert.Equal(major, builder.Major);
            Assert.Equal(minor, builder.Minor);
            Assert.Equal(patch, builder.Patch);
            Assert.Equal(preReleasesArray ?? Array.Empty<SemanticPreRelease>(), builder.PreReleases);
            Assert.Equal(buildMetadataArray ?? Array.Empty<string>(), builder.BuildMetadata);
        }

        public static void PreRelease(SemanticPreRelease preRelease, int number)
        {
            Assert.True(preRelease.IsNumeric);
            Assert.Equal(number, preRelease.Number);
            Assert.Equal(number, (int)preRelease);
            Assert.Throws<InvalidOperationException>(() => preRelease.Text);
            Assert.Equal(number.ToString(), (string)preRelease);
        }
        public static void PreRelease(SemanticPreRelease preRelease, string source, bool spanParameter = false)
        {
            Assert.False(preRelease.IsNumeric);
            if (spanParameter)
            {
                // has to allocate a new string, because ReadOnlySpan<char> was passed as a parameter
                Assert.Equal(source, preRelease.Text);
                Assert.Equal(source, (string)preRelease);
                Assert.NotSame(source, preRelease.Text);
                Assert.NotSame(source, (string)preRelease);
            }
            else
            {
                // no need to allocate a new string, because string was passed as a parameter
                Assert.Same(source, preRelease.Text);
                Assert.Same(source, (string)preRelease);
            }
            Assert.Throws<InvalidOperationException>(() => preRelease.Number);
            Assert.Throws<InvalidOperationException>(() => (int)preRelease);
        }
        public static void PreRelease(SemanticPreRelease preRelease, object? value, bool spanParameter = false)
        {
            if (value is string str) PreRelease(preRelease, str, spanParameter);
            else PreRelease(preRelease, (int)value!);
        }

        public static void Identical<TResult>(bool expected, Parser<TResult>[] functions, Action<TResult> resultAction)
        {
            bool success = functions[0](out TResult? result);
            Assert.Equal(expected, success);
            if (success) resultAction(result!);
            for (int i = 1, length = functions.Length; i < length; i++)
            {
                bool nextSuccess = functions[i](out TResult? nextResult);
                Assert.Equal(expected, nextSuccess);
                if (nextSuccess) resultAction(nextResult!);
            }
        }
        public static void Identical<TResult, TException>(bool expected, Func<TResult>[] functions, Action<TResult> resultAction)
            where TException : Exception
        {
            TException? exception;
            TResult? result;
            try
            {
                result = functions[0]();
                exception = null;
                Assert.True(expected, "Identical(…) was expecting an exception.");
            }
            catch (Exception caught)
            {
                result = default;
                exception = Assert.IsType<TException>(caught);
                Assert.False(expected, "Identical(…) was expecting a valid result.");
            }

            if (exception is null)
            {
                resultAction(result!);
                for (int i = 1, length = functions.Length; i < length; i++)
                    resultAction(functions[i]());
            }
            else
            {
                for (int i = 1, length = functions.Length; i < length; i++)
                    Assert.Throws<TException>(() => functions[i]());
            }
        }

    }
    public delegate bool Parser<TResult>(out TResult? result);
}
