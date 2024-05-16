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

    void Run()
    {
        var store = CredentialManager.Create(Guid.NewGuid().ToString("N"));

        var usr = Guid.NewGuid().ToString("N");
        var pwd = Guid.NewGuid().ToString("N");

        store.AddOrUpdate("https://test.com", usr, pwd);

        Assert.Equal(pwd, store.Get("https://test.com", usr).Password);
    }
}