using Dwarf.Minstrel.Data.Tables;

namespace Dwarf.Minstrel.Data.Models;

public sealed class RadioStation
{
	public string Id { get; set; } = string.Empty;
	public string? Title { get; set; }
	public string? StreamUrl { get; set; }
	public string? Image { get; set; }
}