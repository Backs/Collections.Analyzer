using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Tests;

internal static class ResourceReader
{
    public static string ReadFromFile(string resourceName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var fullResourceName = assembly.GetManifestResourceNames()
            .FirstOrDefault(x => x.EndsWith("Resources." + resourceName) || x.Contains(".Resources.") && x.EndsWith("." + resourceName))
            ?? throw new InvalidOperationException($"Resource {resourceName} not found");

        using var stream = assembly.GetManifestResourceStream(fullResourceName) ?? throw new InvalidOperationException();
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}