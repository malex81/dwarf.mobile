using Dwarf.Minstrel.Data.Tables;

namespace Dwarf.Minstrel.Data;

internal static class InitData
{
	public static IEnumerable<RadioSource> InitialRadioSources()
	{
		yield return new()
		{
			Title = "Маруся",
			StreamUrl = "https://radio-holding.ru:9433/marusya_default"
		};
		yield return new()
		{
			Title = "Маруся",
			StreamUrl = "https://radio-holding.ru:9433/marusya_default"
		};
	}
}