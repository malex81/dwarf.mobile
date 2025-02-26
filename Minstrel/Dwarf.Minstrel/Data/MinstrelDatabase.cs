using SQLite;

namespace Dwarf.Minstrel.Data;

// https://learn.microsoft.com/ru-ru/dotnet/maui/data-cloud/database-sqlite?view=net-maui-9.0

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

		await Task.Delay(100);

		db = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
		//var result = await db.CreateTableAsync<TodoItem>();
	}
}