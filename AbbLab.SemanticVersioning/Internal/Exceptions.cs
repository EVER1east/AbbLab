namespace AbbLab.SemanticVersioning
{
    internal static class Exceptions
    {
        public const string MajorNotFound = "Expected a major version component.";
        public const string MajorLeadingZeroes = "The major version component cannot contain leading zeroes.";
        public const string MajorNegative = "The major version component cannot be less than 0.";
        public const string MajorTooBig = "The major version component cannot be greater than 2147483647.";

        public const string MinorNotFound = "Expected a minor version component.";
        public const string MinorLeadingZeroes = "The minor version component cannot contain leading zeroes.";
        public const string MinorNegative = "The minor version component cannot be less than 0.";
        public const string MinorTooBig = "The minor version component cannot be greater than 2147483647.";

        public const string PatchNotFound = "Expected a patch version component.";
        public const string PatchLeadingZeroes = "The patch version component cannot contain leading zeroes.";
        public const string PatchNegative = "The patch version component cannot be less than 0.";
        public const string PatchTooBig = "The patch version component cannot be greater than 2147483647.";

        public const string PreReleaseNotFound = "Expected a pre-release identifier.";
        public const string PreReleaseEmpty = "The pre-release identifier cannot be empty.";
        public const string PreReleaseInvalid = "The pre-release identifier must only contain [0-9A-Za-z-] characters.";
        public const string PreReleaseLeadingZeroes = "The pre-release numeric identifier cannot contain leading zeroes.";
        public const string PreReleaseNegative = "The pre-release numeric identifier cannot be less than 0.";
        public const string PreReleaseTooBig = "The pre-release numeric identifier cannot be greater than 2147483647.";

        public const string BuildMetadataNotFound = "Expected a build metadata identifier.";
        public const string BuildMetadataEmpty = "The build metadata identifier cannot be empty.";
        public const string BuildMetadataInvalid = "The build metadata identifier must only contain [0-9A-Za-z-] characters.";

        public const string EncounteredInvalidCharacter = "Encountered an unexpected character at the end of the string.";

    }
}