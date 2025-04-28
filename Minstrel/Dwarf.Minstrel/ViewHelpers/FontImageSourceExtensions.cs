namespace Dwarf.Minstrel.ViewHelpers;

public static class FontImageSourceExtensions
{
	public static FontImageSource WithBigSize(this FontImageSource fi)
	{
		fi.Size = 128;
		return fi;
	}
}