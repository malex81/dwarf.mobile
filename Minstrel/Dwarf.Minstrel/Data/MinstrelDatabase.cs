using Dwarf.Minstrel.Data.Models;
using Dwarf.Minstrel.Data.Tables;
using SQLite;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Dwarf.Minstrel.Data;

/* https://learn.microsoft.com/ru-ru/dotnet/maui/data-cloud/database-sqlite?view=net-maui-9.0
 * https://github.com/praeclarum/sqlite-net
 * https://github.com/praeclarum/sqlite-net/wiki
 * https://github.com/praeclarum/sqlite-net/wiki/Synchronous-API
 */

public class MinstrelDatabase
{
	static readonly JsonSerializerOptions JsonCaseInsensitive = new() { PropertyNameCaseInsensitive = true };

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
		if (await db.CreateTableAsync<RadioItemState>() == CreateTableResult.Created)
		{
			//await foreach (var radio in InitData.InitialRadioSources())
			//	await db.InsertAsync(radio);
		}
	}

	public async Task<SQLiteAsyncConnection> GetConnection()
	{
		await Init();
		return db;
	}

	async Task Disconnect()
	{
		if (db is null) return;
		await db.CloseAsync();
		db = null;
	}

	public async Task RecreateDb()
	{
		await Disconnect();
		File.Delete(DBConfig.DatabasePath);
	}

	public async Task SaveAsync(DbObjectBase obj)
	{
		await Init();
		if (obj.Id == 0) await db.InsertAsync(obj);
		else await db.UpdateAsync(obj);
	}

	/*	public async Task<RadioSource[]> LoadRadioSources()
		{
			await Init();
			return await db.Table<RadioSource>().ToArrayAsync();
		}

		public async Task RemoveRadioSource(RadioSource radioSource)
		{
			if (db is null) return;
			await db.DeleteAsync(radioSource);
		}
	*/
	public async Task<RadioItemFacade[]> LoadStations()
	{
		await Init();

		var stateList = await db.Table<RadioItemState>().ToArrayAsync();

		using var stream = await FileSystem.OpenAppPackageFileAsync("radio_stations.json");
		using var reader = new StreamReader(stream);

		var jsonContents = await reader.ReadToEndAsync();

		return (JsonSerializer.Deserialize<RadioStation[]>(jsonContents, JsonCaseInsensitive) ?? [])
			.Where(r => !string.IsNullOrWhiteSpace(r.Id) && !string.IsNullOrWhiteSpace(r.StreamUrl))
			.Select(r => new RadioItemFacade(this, r, stateList.FirstOrDefault(s => s.SourceId == r.Id) ?? new() { SourceId = r.Id }))
			.ToArray();
	}
}