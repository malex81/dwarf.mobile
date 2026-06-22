using Serilog;

namespace Dwarf.Minstrel.Helpers;

public static class LogService
{
	public static void Initialize()
	{
		// Определяем путь к файлу лога в папке данных приложения
		string logPath = Path.Combine(FileSystem.AppDataDirectory, "logs", "log.txt");

		Log.Logger = new LoggerConfiguration()
			.MinimumLevel.Debug()
			.WriteTo.Debug() // Вывод в окно отладки IDE
			.WriteTo.File(logPath,
				rollingInterval: RollingInterval.Day,
				retainedFileCountLimit: 7,
				outputTemplate: "{Timestamp:HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
			.CreateLogger();

		Log.Information("Logging initialized. Log file path: {LogPath}", logPath);
	}

	public static void Shutdown()
	{
		Log.CloseAndFlush();
	}
}