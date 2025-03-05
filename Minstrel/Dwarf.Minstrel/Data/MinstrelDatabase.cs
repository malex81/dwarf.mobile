using Dwarf.Minstrel.Data.Tables;
using SQLite;
using System.Diagnostics.CodeAnalysis;

namespace Dwarf.Minstrel.Data;

/* https://learn.microsoft.com/ru-ru/dotnet/maui/data-cloud/database-sqlite?view=net-maui-9.0
 * https://github.com/praeclarum/sqlite-net
 * https://github.com/praeclarum/sqlite-net/wiki
 * https://github.com/praeclarum/sqlite-net/wiki/Synchronous-API
 */

public class MinstrelDatabase
{
	SQLiteAsyncConnection? db;

	public MinstrelDatabase()
	{
	}

	[MemberNotNull(nameof(db))]
	async Task Init()
	{
		if (db is not null)
			return;

		db = new SQLiteAsyncConnection(DBConfig.DatabasePath, DBConfig.Flags);
		if (await db.CreateTableAsync<RadioSource>() == CreateTableResult.Created)
		{
			await foreach (var radio in InitData.InitialRadioSources())
				await db.InsertAsync(radio);
		}
	}

	public async Task<RadioSource[]> LoadRadioSources()
	{
		await Init();
		return await db.Table<RadioSource>().ToArrayAsync();
	}
}