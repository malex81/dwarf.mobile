using System.Reflection;

namespace Dwarf.Minstrel.Helpers;

/// <summary>
/// Helper methods for loading embedded resources.
/// </summary>
public static class ResourceHelper
{
    private static readonly Assembly Assembly = Assembly.GetExecutingAssembly();
    private static readonly string[] ResourceNames = Assembly.GetManifestResourceNames();

    /// <summary>
    /// Loads an embedded resource as a byte array.
    /// </summary>
    /// <param name="name">Partial name of the resource to load. Approximate matching (Contains) is used.</param>
    /// <returns>Byte array containing the resource data.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is null or empty.</exception>
    /// <exception cref="ResourceNotFoundException">Thrown when the resource cannot be found or loaded.</exception>
    public static async Task<byte[]> LoadResourceAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Resource name must be provided.", nameof(name));

        var resourceName = FindResource(name) ??
            throw new ResourceNotFoundException($"Resource '{name}' not found.");

        await using var stream = Assembly.GetManifestResourceStream(resourceName)
            ?? throw new ResourceNotFoundException($"Failed to open stream for resource '{resourceName}'.");

        // Asynchronous copy to a MemoryStream avoids allocating a buffer based on Length.
        await using var ms = new MemoryStream();
        await stream.CopyToAsync(ms);
        return ms.ToArray();
    }

    private static string? FindResource(string name)
    {
        // Approximate (Contains) search as per original implementation.
        return ResourceNames.FirstOrDefault(rn => rn.Contains(name, StringComparison.OrdinalIgnoreCase));
    }
}

/// <summary>
/// Exception thrown when an embedded resource cannot be located or loaded.
/// </summary>
public sealed class ResourceNotFoundException : Exception
{
    public ResourceNotFoundException(string message) : base(message) { }
}