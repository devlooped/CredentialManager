using System;
using System.Collections.Generic;
using GitCredentialManager.Interop.Windows;

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

    class CommandContextWrapper(CommandContext context, string? @namespace) : ICommandContext
    {
        readonly ISettings settings = new SettingsWrapper(
            context.Settings is WindowsSettings ?
            new NoGitWindowsSettings(context.Environment, context.Git, context.Trace) :
            new NoGitSettings(context.Environment, context.Git), @namespace);

        public ISettings Settings => settings;

        #region pass-through impl.

        public string ApplicationPath { get => ((ICommandContext)context).ApplicationPath; set => ((ICommandContext)context).ApplicationPath = value; }

        public string InstallationDirectory => ((ICommandContext)context).InstallationDirectory;

        public IStandardStreams Streams => ((ICommandContext)context).Streams;

        public ITerminal Terminal => ((ICommandContext)context).Terminal;

        public ISessionManager SessionManager => ((ICommandContext)context).SessionManager;

        public ITrace Trace => ((ICommandContext)context).Trace;

        public ITrace2 Trace2 => ((ICommandContext)context).Trace2;

        public IFileSystem FileSystem => ((ICommandContext)context).FileSystem;

        public ICredentialStore CredentialStore => ((ICommandContext)context).CredentialStore;

        public IHttpClientFactory HttpClientFactory => ((ICommandContext)context).HttpClientFactory;

        public IGit Git => new NoGit(context.Git);

        public IEnvironment Environment => ((ICommandContext)context).Environment;

        public IProcessManager ProcessManager => ((ICommandContext)context).ProcessManager;

        public void Dispose() => ((IDisposable)context).Dispose();

        #endregion
    }

    class NoGitSettings(IEnvironment environment, IGit git) : Settings(environment, new NoGit(git)) { }

    class NoGitWindowsSettings(IEnvironment environment, IGit git, ITrace trace) : WindowsSettings(environment, new NoGit(git), trace) { }

    class SettingsWrapper(ISettings settings, string? @namespace) : ISettings
    {
        public string CredentialNamespace => @namespace ?? settings.CredentialNamespace;

        #region pass-through impl.

        public string CredentialBackingStore => settings.CredentialBackingStore;

        public bool UseMsAuthDefaultAccount => settings.UseMsAuthDefaultAccount;

        public IEnumerable<string> GetSettingValues(string envarName, string section, string property, bool isPath)
            => settings.GetSettingValues(envarName, section, property, isPath);

        public bool GetTracingEnabled(out string value) => settings.GetTracingEnabled(out value);
        public bool TryGetPathSetting(string envarName, string section, string property, out string value) => settings.TryGetPathSetting(envarName, section, property, out value);
        public bool TryGetSetting(string envarName, string section, string property, out string value) => settings.TryGetSetting(envarName, section, property, out value);

        public Uri RemoteUri { get => settings.RemoteUri; set => settings.RemoteUri = value; }

        public bool IsDebuggingEnabled => settings.IsDebuggingEnabled;

        public bool IsTerminalPromptsEnabled => settings.IsTerminalPromptsEnabled;

        public bool IsGuiPromptsEnabled { get => settings.IsGuiPromptsEnabled; set => settings.IsGuiPromptsEnabled = value; }

        public bool IsInteractionAllowed => settings.IsInteractionAllowed;

        public bool IsSecretTracingEnabled => settings.IsSecretTracingEnabled;

        public bool IsMsalTracingEnabled => settings.IsMsalTracingEnabled;

        public string ProviderOverride => settings.ProviderOverride;

        public string LegacyAuthorityOverride => settings.LegacyAuthorityOverride;

        public bool IsWindowsIntegratedAuthenticationEnabled => settings.IsWindowsIntegratedAuthenticationEnabled;

        public bool IsCertificateVerificationEnabled => settings.IsCertificateVerificationEnabled;

        public bool AutomaticallyUseClientCertificates => settings.AutomaticallyUseClientCertificates;

        public string ParentWindowId => settings.ParentWindowId;

        public string CustomCertificateBundlePath => settings.CustomCertificateBundlePath;

        public string CustomCookieFilePath => settings.CustomCookieFilePath;

        public TlsBackend TlsBackend => settings.TlsBackend;

        public bool UseCustomCertificateBundleWithSchannel => settings.UseCustomCertificateBundleWithSchannel;

        public int AutoDetectProviderTimeout => settings.AutoDetectProviderTimeout;

        public bool UseSoftwareRendering => settings.UseSoftwareRendering;

        public bool AllowUnsafeRemotes => settings.AllowUnsafeRemotes;

        public void Dispose() => settings.Dispose();
        public ProxyConfiguration GetProxyConfiguration() => settings.GetProxyConfiguration();
        public Trace2Settings GetTrace2Settings() => settings.GetTrace2Settings();
        #endregion
    }
}
