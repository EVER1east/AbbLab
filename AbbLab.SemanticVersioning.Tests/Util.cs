using System.Collections.Generic;
using System.Linq;

namespace AbbLab.SemanticVersioning.Tests
{
    internal static class Util
    {
        // ReSharper disable once IdentifierTypo
        public static IEnumerable<object[]> Arrayify<T>(IEnumerable<T> versions)
            => versions.Select(static v => new object[1] { v! }).ToArray();
    }
}
