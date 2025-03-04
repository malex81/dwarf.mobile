using System.Reflection;

namespace Dwarf.Minstrel.Helpers;

public static class ResourceHelper
{
	public static async Task<byte[]> LoadResource(string name)
	{
		var assem = Assembly.GetExecutingAssembly();
		var resName = assem.GetManifestResourceNames().Where(rn => rn.Contains(name)).FirstOrDefault() ?? throw new InvalidDataException($"Resource {name} not found");
		using var stream = assem.GetManifestResourceStream(resName) ?? throw new InvalidDataException($"Resource {resName} load failed");
		byte[] buffer = new byte[stream.Length];
		await stream.ReadExactlyAsync(buffer.AsMemory(0, buffer.Length));
		return buffer;
	}
}