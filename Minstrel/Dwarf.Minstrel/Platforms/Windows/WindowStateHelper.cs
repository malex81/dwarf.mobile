using Dwarf.Minstrel.Base;
using Dwarf.Minstrel.Helpers;
using Microsoft.UI.Windowing;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Windows.Forms;
using WinRT.Interop;

namespace Dwarf.Minstrel.Platforms.Windows;

internal static partial class WindowStateHelper
{
	const int GWL_STYLE = -16;
	const uint WS_MAXIMIZEBOX = 0x00010000;

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

	public static void KeepBounds(this Window window, IPreferences preferences, Size defSize)
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
			if (window.Handler?.PlatformView is Microsoft.UI.Xaml.Window platformWindow && platformWindow.AppWindow.IsVisible)
			{
				var currentBounds = new Rect(window.X, window.Y, window.Width, window.Height);
				string json = JsonSerializer.Serialize(currentBounds);
				preferences.Set(PreferenceNames.WindowBounds, json);
			}
		};
	}

/*	public static void ForbidMaximized(this Microsoft.UI.Xaml.Window platformWindow)
	{
		IntPtr hWnd = WindowNative.GetWindowHandle(platformWindow);
		nint currentStyle = GetWindowLongPtr(hWnd, GWL_STYLE);
		SetWindowLongPtr(hWnd, GWL_STYLE, currentStyle & ~(nint)WS_MAXIMIZEBOX);
	}

	[LibraryImport("user32.dll", EntryPoint = "GetWindowLongPtrW")]
	private static partial nint GetWindowLongPtr(IntPtr hWnd, int nIndex);
	[LibraryImport("user32.dll", EntryPoint = "SetWindowLongPtrW")]
	private static partial nint SetWindowLongPtr(IntPtr hWnd, int nIndex, nint dwNewLong);
*/
	#region PerformOnPlatform
	class WindowHandlerHook(Window window)
	{
		readonly List<Action<Microsoft.UI.Xaml.Window>> actions = [];
		bool subscribed = false;
		public void PerformOnPlatform(Action<Microsoft.UI.Xaml.Window> action)
		{
			if (window.Handler?.PlatformView is Microsoft.UI.Xaml.Window platformWindow)
			{
				action(platformWindow);
				return;
			}
			actions.Add(action);
			if (!subscribed)
			{
				subscribed = true;
				window.HandlerChanged += Window_HandlerChanged;
			}
		}

		private void Window_HandlerChanged(object? sender, EventArgs e)
		{
			if (window.Handler?.PlatformView is not Microsoft.UI.Xaml.Window platformWindow)
				return;
			window.HandlerChanged -= Window_HandlerChanged;
			foreach (var action in actions)
				action(platformWindow);
			actions.Clear();
		}
	}

	static readonly ConcurrentDictionary<Window, WindowHandlerHook> handlersHookDict = [];
	public static void PerformOnPlatform(this Window window, Action<Microsoft.UI.Xaml.Window> action)
	{
		var winHook = handlersHookDict.GetOrAdd(window, w => new(w));
		winHook.PerformOnPlatform(action);
	}
	#endregion
}