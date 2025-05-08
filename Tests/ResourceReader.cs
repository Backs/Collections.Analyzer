using System.IO;
using System.Reflection;

namespace Tests;

internal static class ResourceReader
{
    public static string ReadFromFile(string resourceName)
    {
        var assembly = Assembly.GetExecutingAssembly();

        using var stream = assembly.GetManifestResourceStream("Tests.Resources." + resourceName);
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}