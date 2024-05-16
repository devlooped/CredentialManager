using System.Runtime.InteropServices;
using GitCredentialManager;

namespace Tests;

/// <summary>
/// These tests require the main CredentialManager package to be packed in order for the 
/// restore to work. We test end-to-end across all three OSes in CI. To test locally, 
/// pack the main package and then restore and run the tests.
/// </summary>
public class EndToEnd : IDisposable
{
    public EndToEnd() => Environment.SetEnvironmentVariable("GCM_CREDENTIAL_STORE", null);

    public void Dispose() => Environment.SetEnvironmentVariable("GCM_CREDENTIAL_STORE", null);

    [WindowsFact]
    public void WindowsDPAPIStore()
    {
        Environment.SetEnvironmentVariable("GCM_CREDENTIAL_STORE", "dpapi");
        Run();
    }

    [OSFact(nameof(OSPlatform.Windows), nameof(OSPlatform.OSX))]
    public void DefaultStore() => Run();

    [Fact]
    public void PlainTextStore()
    {
        Environment.SetEnvironmentVariable("GCM_CREDENTIAL_STORE", "plaintext");
        Run();
    }

    [UnixFact]
    public void GitCacheStore()
    {
        Environment.SetEnvironmentVariable("GCM_CREDENTIAL_STORE", "cache");
        Run();
    }

    [LocalFact(nameof(OSPlatform.Linux))]
    public void LinuxSecretService()
    {
        // To test this locally, you need:
        //  sudo apt-get update
        //  sudo apt install libsecret-1-0 libsecret-1-dev
        //  sudo apt install gnome-keyring
        //  dbus-launch --sh-syntax
        //  export $(dbus-launch)
        //  gnome-keyring-daemon -r -d

        // Then run the tests: dotnet test
        // This will require keyring unlocking interactively before tests proceed.

        Environment.SetEnvironmentVariable("GCM_CREDENTIAL_STORE", "secretservice");
        Run();
    }

    void Run()
    {
        var store = CredentialManager.Create(Guid.NewGuid().ToString("N"));

        var usr = Guid.NewGuid().ToString("N");
        var pwd = Guid.NewGuid().ToString("N");

        store.AddOrUpdate("https://test.com", usr, pwd);

        Assert.Equal(pwd, store.Get("https://test.com", usr).Password);
    }
}