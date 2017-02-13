using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.PlatformAbstractions;

namespace Greentube.Monitoring
{
    /// <summary>
    /// Version Service
    /// </summary>
    public sealed class VersionService : IVersionService
    {
        private readonly VersionInformation _versionInformation;

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionService"/> class.
        /// </summary>
        /// <param name="hostingEnvironment">The hosting environment.</param>
        /// <param name="assembly">The assembly to inspect for AssemblyInformationalVersionAttribute.</param>
        public VersionService(IHostingEnvironment hostingEnvironment, Assembly assembly)
        {
            if (hostingEnvironment == null) throw new ArgumentNullException(nameof(hostingEnvironment));
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));

            var informationalVersion = assembly.GetCustomAttributes().ToList()
                .OfType<AssemblyInformationalVersionAttribute>()
                .FirstOrDefault()?.InformationalVersion;

            _versionInformation = new VersionInformation(
                assemblyFileVersion: PlatformServices.Default.Application.ApplicationVersion,
                assemblyInformationalVersion: informationalVersion,
                environmentName: hostingEnvironment.EnvironmentName,
                runtimeFramework: PlatformServices.Default.Application.RuntimeFramework.ToString(),
                startupTimeUtc: Process.GetCurrentProcess().StartTime.ToUniversalTime()
            );
        }

        /// <summary>
        /// Gets the version information of the running process.
        /// </summary>
        /// <returns></returns>
        public VersionInformation GetVersionInformation()
        {
            return _versionInformation;
        }
    }
}
