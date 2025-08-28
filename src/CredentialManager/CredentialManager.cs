namespace GitCredentialManager;

/// <summary>
/// Provides the factory method <see cref="Create"/> to instantiate a 
/// <see cref="ICredentialStore"/> appropriate for the current platform and 
/// optionally configured store implementation.
/// </summary>
/// <remarks>
/// See https://github.com/git-ecosystem/git-credential-manager/blob/main/docs/credstores.md 
/// for more information.
/// </remarks>
public static class CredentialManager
{
    /// <summary>
    /// Creates the right implementation of <see cref="ICredentialStore"/> 
    /// appropriate for the current platform.
    /// </summary>
    /// <param name="namespace">Optional namespace to scope credential operations.</param>
    /// <returns>The <see cref="ICredentialStore"/>.</returns>
    public static ICredentialStore Create(string? @namespace = default)
        => CreateContext(@namespace).CredentialStore;

    /// <summary>
    /// Creates a new <see cref="ICommandContext"/> that can be used for GCM operations 
    /// without depending on a git installation.
    /// </summary>
    public static ICommandContext CreateContext(string? @namespace = default)
        => new CommandContextAdapter(new CommandContext(), @namespace);
}
