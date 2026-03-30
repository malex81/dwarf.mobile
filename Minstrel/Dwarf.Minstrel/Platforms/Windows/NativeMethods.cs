using System.Runtime.InteropServices;

namespace Dwarf.Minstrel.Platforms.Windows;

static partial class NativeMethods
{
	public static readonly IntPtr IDC_ARROW = new(32512);

	[LibraryImport("user32.dll", EntryPoint = "SetCursor")]
	public static partial IntPtr SetCursor(IntPtr hCursor);

	[LibraryImport("user32.dll", EntryPoint = "LoadCursorW", StringMarshalling = StringMarshalling.Utf16)]
	public static partial IntPtr LoadCursor(IntPtr hInstance, IntPtr lpCursorName);

	/*	
		const int GWL_STYLE = -16;
		const uint WS_MAXIMIZEBOX = 0x00010000;

		public static void ForbidMaximized(this Microsoft.UI.Xaml.Window platformWindow)
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
}