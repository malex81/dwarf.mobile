using SQLite;

namespace Dwarf.Minstrel.Data.Tables;

[Table("radio")]
public class RadioSource
{
	[PrimaryKey, AutoIncrement]
	[Column("id")]
	public int Id { get; set; }

	[Column("title")]
	[MaxLength(256)]
	public string? Title { get; set; }

	[Column("logo")]
	public byte[]? Logo { get; set; }

	[Column("streamUrl")]
	[MaxLength(2048)]
	public string? StreamUrl { get; set; }
}