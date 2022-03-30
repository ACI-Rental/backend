using System.Reflection;

namespace ACI.Shared;

public static class AppUtils
{
    /// <summary>
    /// Fetches value from the version field set in .csproj file.
    /// </summary>
    /// <returns>InformationalVersionAttribute value.</returns>
    /// <exception cref="InvalidOperationException">Throws when EntryAssembly or version are null.</exception>
    public static string GetVersion()
    {
        return (Assembly.GetEntryAssembly() ?? throw new InvalidOperationException("EntryAssembly was null"))
               .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
               ?? throw new InvalidOperationException("InformationalVersionAttribute is missing! Make sure the <Version> field is set in your .csproj");
    }
}