using System;

namespace Greentube.Monitoring
{
    /// <summary>
    /// The Version information of the running service
    /// </summary>
    public sealed class VersionInformation
    {
        /// <summary>
        /// Gets the name of the environment.
        /// </summary>
        /// <value>
        /// The name of the environment.
        /// </value>
        public string EnvironmentName { get; }

        /// <summary>
        /// Gets or sets the startup (in UTC) time of this process.
        /// </summary>
        /// <value>
        /// The startup time.
        /// </value>
        public DateTimeOffset StartupTimeUtc { get; }

        /// <summary>
        /// Gets or sets the runtime framework.
        /// </summary>
        /// <value>
        /// The runtime framework.
        /// </value>
        public string RuntimeFramework { get; }

        /// <summary>
        /// Value of the Assembly FileVersion attribute
        /// </summary>
        /// <remarks>
        /// The assembly version: e.g: 1.0.0.0
        /// </remarks>
        /// <value>
        /// The version.
        /// </value>
        public string AssemblyFileVersion { get; }

        /// <summary>
        /// Value of the Assembly InformationalVersion attribute
        /// </summary>
        /// <remarks>
        /// Often used to add the VCS branch and/or commit hash
        /// </remarks>
        /// <value>
        /// The BranchCommit.
        /// </value>
        public string AssemblyInformationalVersion { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionInformation"/> class.
        /// </summary>
        /// <param name="environmentName">Name of the environment.</param>
        /// <param name="startupTimeUtc">The startup time in UTC.</param>
        /// <param name="runtimeFramework">The runtime framework.</param>
        /// <param name="assemblyFileVersion">The assembly file version.</param>
        /// <param name="assemblyInformationalVersion">The assembly informational version.</param>
        public VersionInformation(
            string environmentName = null,
            DateTimeOffset startupTimeUtc = default(DateTimeOffset),
            string runtimeFramework = null,
            string assemblyFileVersion = null,
            string assemblyInformationalVersion = null)
        {
            EnvironmentName = environmentName;
            StartupTimeUtc = startupTimeUtc;
            RuntimeFramework = runtimeFramework;
            AssemblyFileVersion = assemblyFileVersion;
            AssemblyInformationalVersion = assemblyInformationalVersion;
        }
    }
}