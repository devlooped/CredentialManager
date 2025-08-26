using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using DotNetConfig;

namespace GitCredentialManager;

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
            homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        }
        else
        {
            // On Linux/Mac it's $HOME
            homeDir = Environment.GetEnvironmentVariable("HOME")
                      ?? Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
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
        => Environment.GetEnvironmentVariable("GCM_CREDENTIAL_STORE") is "cache" ? git.InvokeHelperAsync(args, standardInput) :
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
            value = GetAll(level, type, name).FirstOrDefault();
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
