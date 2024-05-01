using System;
using GitCredentialManager.Interop.MacOS;
using GitCredentialManager.Interop.Windows;

namespace GitCredentialManager;

/// <summary>
/// Provides the factory method <see cref="Create"/> to instantiate a 
/// <see cref="ICredentialStore"/> appropriate for the current platform.
/// </summary>
public static class CredentialManager
{
    /// <summary>
    /// Creates the right implementation of <see cref="ICredentialStore"/> 
    /// appropriate for the current platform.
    /// </summary>
    /// <param name="namespace">Optional namespace to scope credential operations.</param>
    /// <returns>The <see cref="ICredentialStore"/>.</returns>
    public static ICredentialStore Create(string? @namespace = default)
    {
        if (PlatformUtils.IsWindows())
            return new WindowsCredentialManager(@namespace);
        else if (PlatformUtils.IsMacOS())
            return new MacOSKeychain(@namespace);
        else if (PlatformUtils.IsLinux())
            return new CommandContext().CredentialStore;
        else
            throw new PlatformNotSupportedException();
    }
}
