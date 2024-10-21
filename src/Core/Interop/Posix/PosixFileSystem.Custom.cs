namespace GitCredentialManager.Interop.Posix;

// NOTE: delete the existing TryResolve* in the sync'ed file, make the class partial, 
// and this implementation will fix the build when upgrading. Note that links will 
// actually not be supported.
partial class PosixFileSystem
{
#if NETSTANDARD2_0
    private static bool TryResolveFileLink(string path, out string target)
    {
        target = null;
        return false;
    }

    private static bool TryResolveDirectoryLink(string path, out string target)
    {
        target = null;
        return false;
    }
#elif !NETFRAMEWORK
        private static bool TryResolveFileLink(string path, out string target)
        {
            FileSystemInfo fsi = Polyfills.FileResolveLinkTarget(path, true);
            target = fsi?.FullName;
            return fsi != null;
        }

        private static bool TryResolveDirectoryLink(string path, out string target)
        {
            FileSystemInfo fsi = Polyfills.DirectoryResolveLinkTarget(path, true);
            target = fsi?.FullName;
            return fsi != null;
        }
#endif
}
