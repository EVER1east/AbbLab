namespace AbbLab.SemanticVersioning
{
    /// <summary>
    ///   <para>Specifies the semantic version increment type.</para>
    /// </summary>
    public enum IncrementType
    {
        /// <summary>
        ///   <para>Bumps the version to the next major version.</para>
        /// </summary>
        Major,
        /// <summary>
        ///   <para>Bumps the version to the next minor version.</para>
        /// </summary>
        Minor,
        /// <summary>
        ///   <para>Bumps the version to the next patch version.</para>
        /// </summary>
        Patch,
        /// <summary>
        ///   <para>Bumps the version to a pre-release of the next major version.</para>
        /// </summary>
        PreMajor,
        /// <summary>
        ///   <para>Bumps the version to a pre-release of the next minor version.</para>
        /// </summary>
        PreMinor,
        /// <summary>
        ///   <para>Bumps the version to a pre-release of the next patch version.</para>
        /// </summary>
        PrePatch,
        /// <summary>
        ///   <para>Bumps the version to the next pre-release version.</para>
        /// </summary>
        PreRelease,
    }
}
