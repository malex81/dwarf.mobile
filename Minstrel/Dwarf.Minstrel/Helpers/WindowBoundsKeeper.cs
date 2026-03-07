#if WINDOWS
using Dwarf.Minstrel.Base;
using Microsoft.UI.Windowing;
using System.Text.Json;

namespace Dwarf.Minstrel.Helpers;

internal static class WindowBoundsKeeper
{
	static bool IsBoundsVisible(Rect bounds)
	{
		var displayAreas = DisplayArea.FindAll();
		for (int i = 0; i < displayAreas.Count; i++)
		{
			var area = displayAreas[i];
			var intersectArea = area.OuterBounds.ToRect().Intersect(bounds);
			if (intersectArea.Width > bounds.Width / 5 && intersectArea.Height > bounds.Height / 5)
				return true;
		}
		return false;
	}

	public static void KeepeBounds(this Window window, IPreferences preferences, Size defSize)
	{
		var savedBoundsJson = preferences.Get(PreferenceNames.WindowBounds, string.Empty);
		Rect windowBounds = string.IsNullOrEmpty(savedBoundsJson)
			? new Rect(new Point(100, 100), defSize)
			: JsonSerializer.Deserialize<Rect>(savedBoundsJson);

		if (IsBoundsVisible(windowBounds))
		{
			window.X = windowBounds.X;
			window.Y = windowBounds.Y;
		}
		window.Width = windowBounds.Width;
		window.Height = windowBounds.Height;

		window.Destroying += (s, e) =>
		{
			var currentBounds = new Rect(window.X, window.Y, window.Width, window.Height);
			string json = JsonSerializer.Serialize(currentBounds);
			preferences.Set(PreferenceNames.WindowBounds, json);
		};
	}
}
#endif