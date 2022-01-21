namespace AbbLab.SemanticVersioning
{
    internal enum SemanticErrorCode
    {
        Success = 0,

        MajorNotFound,
        MajorLeadingZeroes,
        MajorTooBig,

        MinorNotFound,
        MinorLeadingZeroes,
        MinorTooBig,

        PatchNotFound,
        PatchLeadingZeroes,
        PatchTooBig,

        PreReleaseNotFound,
        PreReleaseLeadingZeroes,
        PreReleaseTooBig,
        BuildMetadataNotFound,
    }
}