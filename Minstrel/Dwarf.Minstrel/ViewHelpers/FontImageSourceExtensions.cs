using Dwarf.Minstrel.Base;

namespace Dwarf.Minstrel.ViewHelpers;

public static class FontImageSourceExtensions
{
	public static FontImageSource WithBigSize(this FontImageSource fi)
	{
		fi.Size = 128;
		return fi;
	}
	public static FontImageSource WithGlyph(this FontImageSource fi, object glyphKind)
	{
		var (font, glyph) = glyphKind switch
		{
			FASolidGlyphs faSolidGlyph => (FontNames.FASolid, (uint)faSolidGlyph),
			FARegularGlyphs faRegularGlyph => (FontNames.FARegular, (uint)faRegularGlyph),
			_ => throw new InvalidOperationException($"Unknown glyph kind {glyphKind}")
		};
		fi.FontFamily = font;
		fi.Glyph = Convert.ToChar(glyph).ToString();
		return fi;
	}
}