namespace Dwarf.Minstrel.Helpers;

internal static class MetricHelpers
{
#if WINDOWS
	public static Rect ToRect(this Windows.Graphics.RectInt32 rectInt32) => new(rectInt32.X, rectInt32.Y, rectInt32.Width, rectInt32.Height);
#endif
}