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
    public EndToEnd()
    {
        Environment.SetEnvironmentVariable("GCM_CREDENTIAL_STORE", null);

        // Sets fake git path to ensure we're not inadvertently requiring a full git installation
        var file = "git";
        if (OperatingSystem.IsWindows())
            file += ".exe";

        var dir = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? Path.GetTempPath()
            : Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        var path = Path.Combine(dir, file);

        if (!File.Exists(path))
            File.WriteAllBytes(path, []);

        // Sets fake git path to ensure we're not inadvertently requiring a full git installation
        Environment.SetEnvironmentVariable("GIT_EXEC_PATH", dir);
    }

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
        // Git cache store does depend on git being present, so we set remove the fake path.
        Environment.SetEnvironmentVariable("GIT_EXEC_PATH", null);
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

    [WindowsFact]
    public void SavedOneNamespaceCannotRetrieveAnother()
    {
        var ns1 = Guid.NewGuid().ToString("N");
        var ns2 = Guid.NewGuid().ToString("N");

        var store1 = CredentialManager.Create(ns1);
        var store2 = CredentialManager.Create(ns2);

        var usr = Guid.NewGuid().ToString("N");
        var pwd = Guid.NewGuid().ToString("N");

        store1.AddOrUpdate("https://test.com", usr, pwd);

        Assert.Null(store2.Get("https://test.com", usr));
        Assert.Empty(store2.GetAccounts("https://test.com"));
    }

    void Run()
    {
        var store = CredentialManager.Create(Guid.NewGuid().ToString("N"));

        var usr = Guid.NewGuid().ToString("N");
        var pwd = Guid.NewGuid().ToString("N");

        store.AddOrUpdate("https://test.com", usr, pwd);

        Assert.Equal(pwd, store.Get("https://test.com", usr).Password);

        store.Remove("https://test.com", usr);

        Assert.Null(store.Get("https://test.com", usr));
    }
}