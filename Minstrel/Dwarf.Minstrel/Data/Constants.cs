namespace Dwarf.Minstrel.Data;

// https://learn.microsoft.com/ru-ru/dotnet/maui/data-cloud/database-sqlite?view=net-maui-9.0
public static class Constants
{
	public const string DatabaseFilename = "Minstrel.db3";

	public const SQLite.SQLiteOpenFlags Flags =
		// open the database in read/write mode
		SQLite.SQLiteOpenFlags.ReadWrite |
		// create the database if it doesn't exist
		SQLite.SQLiteOpenFlags.Create |
		// enable multi-threaded database access
		SQLite.SQLiteOpenFlags.SharedCache;

	public static string DatabasePath =>
		Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);
}