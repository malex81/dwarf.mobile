using SQLite;

namespace Dwarf.Minstrel.Data;

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