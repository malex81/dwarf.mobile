using System.Runtime.InteropServices;

namespace Dwarf.Minstrel.Platforms.Windows;

static partial class NativeMethods
{
	public static readonly IntPtr IDC_ARROW = new(32512);

	[LibraryImport("user32.dll", EntryPoint = "SetCursor")]
	public static partial IntPtr SetCursor(IntPtr hCursor);

	[LibraryImport("user32.dll", EntryPoint = "LoadCursorW", StringMarshalling = StringMarshalling.Utf16)]
	public static partial IntPtr LoadCursor(IntPtr hInstance, IntPtr lpCursorName);
}