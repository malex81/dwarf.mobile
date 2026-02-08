using SQLite;

namespace Dwarf.Minstrel.Data.Tables;

public abstract class DbObjectBase
{
	[PrimaryKey, AutoIncrement]
	[Column("id")]
	public int Id { get; set; }
}