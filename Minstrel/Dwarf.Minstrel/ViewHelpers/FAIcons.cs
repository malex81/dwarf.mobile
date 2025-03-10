namespace Dwarf.Minstrel.ViewHelpers;
/*
 * https://fontawesome.com/search?ic=free
 */
public enum FASolidGlyphs
{
	None = 0,
	Music = 0xf001,
	Radio = 0xf8d7,
	Gear = 0xf013,
}

public enum FARegularGlyphs
{
	None = 0,
	CirclePlay = 0xf144,
	CircleStop = 0xf28d,
	CirclePause = 0xf28b,
}

public static class FAIcons
{
	public static readonly BindableProperty SolidGlyphProperty =
	   BindableProperty.CreateAttached("SolidGlyph", typeof(FASolidGlyphs), typeof(FAIcons), FASolidGlyphs.None, propertyChanged: OnSolidGlyphChanged);

	public static FASolidGlyphs GetSolidGlyph(BindableObject view) => (FASolidGlyphs)view.GetValue(SolidGlyphProperty);
	public static void SetSolidGlyph(BindableObject view, FASolidGlyphs value) => view.SetValue(SolidGlyphProperty, value);

	static void OnSolidGlyphChanged(BindableObject view, object oldValue, object newValue)
	{
		if (newValue is not FASolidGlyphs glyph || glyph == FASolidGlyphs.None) return;
		SetGlyph(view, "FASolid", (uint)glyph);
	}

	public static readonly BindableProperty RegularGlyphProperty =
	   BindableProperty.CreateAttached("RegularGlyph", typeof(FARegularGlyphs), typeof(FAIcons), FARegularGlyphs.None, propertyChanged: OnRegularGlyphChanged);

	public static FARegularGlyphs GetRegularGlyph(BindableObject view) => (FARegularGlyphs)view.GetValue(RegularGlyphProperty);
	public static void SetRegularGlyph(BindableObject view, FARegularGlyphs value) => view.SetValue(RegularGlyphProperty, value);

	static void OnRegularGlyphChanged(BindableObject view, object oldValue, object newValue)
	{
		if (newValue is not FARegularGlyphs glyph || glyph == FARegularGlyphs.None) return;
		SetGlyph(view, "FARegular", (uint)glyph);
	}

	static void SetGlyph(BindableObject view, string font, uint glyph)
	{
		if (view is FontImageSource fis)
		{
			fis.FontFamily = font;
			fis.Glyph = Convert.ToChar(glyph).ToString();
			return;
		}
		if (view is ShellContent shellContent)
		{
			shellContent.Icon = new FontImageSource
			{
				FontFamily = font,
				Glyph = Convert.ToChar(glyph).ToString()
			};
			return;
		}
	}
}