using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using DotNetConfig;
using GitCredentialManager.Interop.Windows;

namespace GitCredentialManager;

/// <summary>
/// A wrapper for <see cref="ICommandContext"/> that overrides the namespace for credentials and also 
/// allows git-less usage except for the git cache store.
/// </summary>
partial class CommandContextAdapter : ICommandContext
{
    readonly CommandContext context;
    readonly ISettings settings;
    readonly IHttpClientFactory clientFactory;
    ICredentialStore store;

    public CommandContextAdapter(CommandContext context, string? @namespace = default)
    {
        this.context = context;

        store = new CredentialStore(this);

        settings = new SettingsAdapter(
            context.Settings is WindowsSettings ?
            new NoGitWindowsSettings(context.Environment, context.Git, context.Trace) :
            new NoGitSettings(context.Environment, context.Git), @namespace);

        clientFactory = new HttpClientFactory(
            context.FileSystem, context.Trace, context.Trace2, settings, context.Streams);
    }

    public ISettings Settings => settings;

    public ICredentialStore CredentialStore
    {
        get => store;
        set => store = value;
    }

    public IHttpClientFactory HttpClientFactory => clientFactory;

    public IGit Git => new NoGit(context.Git);

    #region pass-through impl.

    public string ApplicationPath { get => ((ICommandContext)context).ApplicationPath; set => ((ICommandContext)context).ApplicationPath = value; }

    public string InstallationDirectory => ((ICommandContext)context).InstallationDirectory;

    public IStandardStreams Streams => ((ICommandContext)context).Streams;

    public ITerminal Terminal => ((ICommandContext)context).Terminal;

    public ISessionManager SessionManager => ((ICommandContext)context).SessionManager;

    public ITrace Trace => ((ICommandContext)context).Trace;

    public ITrace2 Trace2 => ((ICommandContext)context).Trace2;

    public IFileSystem FileSystem => ((ICommandContext)context).FileSystem;

    public IEnvironment Environment => ((ICommandContext)context).Environment;

    public IProcessManager ProcessManager => ((ICommandContext)context).ProcessManager;

    public void Dispose() => ((IDisposable)context).Dispose();

    #endregion

    /// <summary>
    /// Implements the <see cref="IGit"/> interface by directly reading settings 
    /// from the global config using DotNetConfig instead of depending on a git installation.
    /// </summary>
    class NoGit(IGit git) : IGit
    {
        static readonly Config gitconfig;

        static NoGit()
        {
            string homeDir;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                homeDir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile);
            }
            else
            {
                // On Linux/Mac it's $HOME
                homeDir = System.Environment.GetEnvironmentVariable("HOME")
                          ?? System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile);
            }

            gitconfig = Config.Build(Path.Combine(homeDir, ".gitconfig"));
        }

        public IGitConfiguration GetConfiguration() => new GitConfig(gitconfig);

        public GitVersion Version => throw new NotSupportedException();
        public ChildProcess CreateProcess(string args) => throw new NotSupportedException();
        public string GetCurrentRepository() => throw new NotSupportedException();
        public IEnumerable<GitRemote> GetRemotes() => throw new NotSupportedException();

        /// <summary>Only allows git interaction for the git cache GCM store.</summary>
        public Task<IDictionary<string, string>> InvokeHelperAsync(string args, IDictionary<string, string> standardInput)
            => System.Environment.GetEnvironmentVariable("GCM_CREDENTIAL_STORE") is "cache" ? git.InvokeHelperAsync(args, standardInput) :
                throw new NotSupportedException();

        public bool IsInsideRepository() => throw new NotSupportedException();

        class GitConfig(Config config) : IGitConfiguration
        {
            public void Enumerate(GitConfigurationLevel level, GitConfigurationEnumerationCallback cb)
            {
                foreach (var entry in config)
                {
                    cb(new GitConfigurationEntry(entry.Key, entry.RawValue));
                }
            }

            public IEnumerable<string> GetAll(GitConfigurationLevel level, GitConfigurationType type, string name)
            {
                foreach (var entry in config)
                {
                    if (string.Equals(entry.Key, name, StringComparison.OrdinalIgnoreCase))
                        yield return entry.RawValue ?? "";
                }
            }

            public IEnumerable<string> GetRegex(GitConfigurationLevel level, GitConfigurationType type, string nameRegex, string valueRegex)
            {
                foreach (var entry in config.GetRegex(nameRegex, valueRegex))
                {
                    yield return entry.RawValue ?? "";
                }
            }

            public bool TryGet(GitConfigurationLevel level, GitConfigurationType type, string name, out string value)
            {
                value = GetAll(level, type, name).FirstOrDefault()!;
                return value is not null;
            }

            #region NotSupported (Write Operations)
            public void Add(GitConfigurationLevel level, string name, string value) => throw new NotSupportedException("Configuration is read-only");
            public void ReplaceAll(GitConfigurationLevel level, string nameRegex, string valueRegex, string value) => throw new NotSupportedException("Configuration is read-only");
            public void Set(GitConfigurationLevel level, string name, string value) => throw new NotSupportedException("Configuration is read-only");
            public void Unset(GitConfigurationLevel level, string name) => throw new NotSupportedException("Configuration is read-only");
            public void UnsetAll(GitConfigurationLevel level, string name, string valueRegex) => throw new NotSupportedException("Configuration is read-only");
            #endregion
        }
    }

    /// <summary>Adapts <see cref="Settings"/> to use <see cref="NoGit"/>.</summary>
    class NoGitSettings(IEnvironment environment, IGit git) : Settings(environment, new NoGit(git)) { }

    /// <summary>Adapts <see cref="WindowsSettings"/> to use <see cref="NoGit"/>.</summary>
    class NoGitWindowsSettings(IEnvironment environment, IGit git, ITrace trace) : WindowsSettings(environment, new NoGit(git), trace) { }

    /// <summary>Allows overriding the credential namespace.</summary>
    class SettingsAdapter(ISettings settings, string? @namespace) : ISettings
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
