using SQLite;

namespace Dwarf.Minstrel.Data.Tables;

[Table("radio")]
public class RadioSource
{
	[PrimaryKey]
	[Column("id")]
	public Guid Id { get; set; }

	[Column("title")]
	[MaxLength(256)]
	public string? Title { get; set; }

	[Column("streamUrl")]
	[MaxLength(2048)]
	public string? StreamUrl { get; set; }
}