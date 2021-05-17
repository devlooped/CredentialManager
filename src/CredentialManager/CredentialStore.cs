using System;
using System.IO;
using System.Reflection;

namespace Microsoft.Git.CredentialManager
{
    /// <summary>
    /// Provides the factory method <see cref="Create"/> to instantiate the 
    /// right implementation of <see cref="ICredentialStore"/> appropriate for 
    /// the current platform.
    /// </summary>
    public static class CredentialStore
    {
        /// <summary>
        /// Creates the right implementation of <see cref="ICredentialStore"/> 
        /// appropriate for the current platform.
        /// </summary>
        /// <param name="appPath">Optional application path.</param>
        /// <returns>The <see cref="ICredentialStore"/>.</returns>
        public static ICredentialStore Create(string? appPath = default)
            => new CommandContext(appPath ?? GetApplicationPath()).CredentialStore;

        // See GCM's Program.cs
        static string GetApplicationPath()
        {
            // Assembly::Location always returns an empty string if the application was published as a single file
#pragma warning disable IL3000
            bool isSingleFile = string.IsNullOrEmpty(Assembly.GetEntryAssembly()?.Location);
#pragma warning restore IL3000

            // Use "argv[0]" to get the full path to the entry executable - this is consistent across
            // .NET Framework and .NET >= 5 when published as a single file.
            string[] args = Environment.GetCommandLineArgs();
            string candidatePath = args[0];

            // If we have not been published as a single file on .NET 5 then we must strip the ".dll" file extension
            // to get the default AppHost/SuperHost name.
            if (!isSingleFile && Path.HasExtension(candidatePath))
            {
                return Path.ChangeExtension(candidatePath, null);
            }

            return candidatePath;
        }
    }
}
