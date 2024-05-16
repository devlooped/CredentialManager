using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Tests;

public class OSFactAttribute : FactAttribute
{
    public OSFactAttribute(params string[] onPlatforms)
        : this(onPlatforms.Select(OSPlatform.Create).ToArray()) { }

    protected OSFactAttribute(params OSPlatform[] onPlatforms)
    {
        var shouldSkip = onPlatforms.All(platform => !RuntimeInformation.IsOSPlatform(platform));

        if (shouldSkip)
            Skip = $"Test runs only on platforms '{string.Join(", ", onPlatforms.Select(x => x.ToString()))}'";
    }
}

public class WindowsFactAttribute() : OSFactAttribute(OSPlatform.Windows);

public class LinuxFactAttribute() : OSFactAttribute(OSPlatform.Linux);

public class macOSFactAttribute() : OSFactAttribute(OSPlatform.OSX);

public class UnixFactAttribute : OSFactAttribute
{
    public UnixFactAttribute() : base(OSPlatform.Linux, OSPlatform.OSX)
    {
    }
}

public class OSTheoryAttribute : TheoryAttribute
{
    public OSTheoryAttribute(params string[] onPlatforms)
        : this(onPlatforms.Select(OSPlatform.Create).ToArray()) { }

    protected OSTheoryAttribute(params OSPlatform[] onPlatforms)
    {
        var shouldSkip = onPlatforms.All(platform => !RuntimeInformation.IsOSPlatform(platform));

        if (shouldSkip)
            Skip = $"Test runs only on platforms '{string.Join(", ", onPlatforms.Select(x => x.ToString()))}'";
    }
}

public class WindowsTheoryAttribute() : OSTheoryAttribute(OSPlatform.Windows);

public class LinuxTheoryAttribute() : OSTheoryAttribute(OSPlatform.Linux);

public class macOSTheoryAttribute() : OSTheoryAttribute(OSPlatform.OSX);

public class UnixTheoryAttribute() : OSTheoryAttribute(OSPlatform.Linux, OSPlatform.OSX);
