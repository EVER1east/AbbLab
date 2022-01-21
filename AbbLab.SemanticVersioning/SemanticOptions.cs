using System;

namespace AbbLab.SemanticVersioning
{
    [Flags]
    public enum SemanticOptions
    {
        Strict = 0,

        AllowVersionPrefix          = 1 << 0,
        AllowEqualsPrefix           = 1 << 1,
        AllowLeadingWhite           = 1 << 2,
        AllowTrailingWhite          = 1 << 3,
        AllowInnerWhite             = 1 << 4,
        AllowLeadingZeroes          = 1 << 5,
        OptionalMinor               = 1 << 6,
        OptionalPatch               = 1 << 7,
        OptionalPreReleaseSeparator = 1 << 8,
        AllowLeftovers              = 1 << 9,

        Loose = ~0,
    }
}