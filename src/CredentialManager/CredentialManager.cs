using System.Collections.Generic;

namespace GitCredentialManager;

public interface ICredentialManager : ICredentialStore
{
    ICommandContext Context { get; }
}

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
    public static ICredentialManager Create(string? @namespace = default)
    {
        // The context already does the check for the platform and configured store to initialize.
        // By overriding the settings with our wrapper, we ensure just the namespace is overriden.
        var context = new CommandContextWrapper(new CommandContext(), @namespace);
        return new CredentialManagerStore(new CredentialStore(context), context);
    }

    class CredentialManagerStore(ICredentialStore store, ICommandContext context) : ICredentialManager
    {
        public ICommandContext Context => context;
        public void AddOrUpdate(string service, string account, string secret) => store.AddOrUpdate(service, account, secret);
        public ICredential Get(string service, string account) => store.Get(service, account);
        public IList<string> GetAccounts(string service) => store.GetAccounts(service);
        public bool Remove(string service, string account) => store.Remove(service, account);
    }
}
