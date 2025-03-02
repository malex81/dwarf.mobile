using Dwarf.Minstrel.Data.Tables;
using SQLite;

namespace Dwarf.Minstrel.Data;

/* https://learn.microsoft.com/ru-ru/dotnet/maui/data-cloud/database-sqlite?view=net-maui-9.0
 * https://github.com/praeclarum/sqlite-net
 * https://github.com/praeclarum/sqlite-net/wiki
 * https://github.com/praeclarum/sqlite-net/wiki/Synchronous-API
 */

internal class MinstrelDatabase
{
	SQLiteAsyncConnection? db;

	public MinstrelDatabase()
	{		
	}

	async Task Init()
	{
		if (db is not null)
			return;

		db = new SQLiteAsyncConnection(DBConfig.DatabasePath, DBConfig.Flags);
		var _res = await db.CreateTableAsync<RadioSource>();
	}
}