using SQLite;

namespace Dwarf.Minstrel.Data.Tables;

[Table("radioState")]
public class RadioItemState : DbObjectBase
{
	[Column("sourceId")]
	[MaxLength(32)]
	public string SourceId { get; init; } = string.Empty;

	[Column("removed")]
	public bool Removed { get; set; }

	[Column("favorite")]
	public bool InFavorites { get; set; }
}