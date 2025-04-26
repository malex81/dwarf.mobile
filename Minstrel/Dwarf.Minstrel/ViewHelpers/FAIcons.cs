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
	Star = 0xf005,
	Heart = 0xf004,
	Bookmark = 0xf02e,
	TowerCell = 0xe585,
	TowerBroadcast = 0xf519,
	Signal = 0xf012,
	Info = 0xf129,
	Question = 0x3f,
	CircleQuestion = 0xf059,
	XMark = 0xf00d,
	CircleXMark = 0xf057,
	ArrowRight = 0xf061,
	ArrowLeft = 0xf060,
	ArrowUp = 0xf062,
	ArrowDown = 0xf063,
	Trash = 0xf1f8,
	TrashCan = 0xf2ed,
	Rotate = 0xf2f1,
	ArrowRotate = 0xf021,
	RotateRight = 0xf2f9,
	ArrowRotateRight = 0xf01e,
	Repeat = 0xf363,
}

public enum FARegularGlyphs
{
	None = 0,
	CirclePlay = 0xf144,
	CircleStop = 0xf28d,
	CirclePause = 0xf28b,
	Star = 0xf005,
	Heart = 0xf004,
	Bookmark = 0xf02e,
	CircleQuestion = 0xf059,
	CircleXMark = 0xf057,
	Lightbulb = 0xf0eb,
	TrashCan = 0xf2ed,
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

	public static readonly BindableProperty GlyphColorProperty =
	   BindableProperty.CreateAttached("GlyphColor", typeof(Color), typeof(FAIcons), default(Color), propertyChanged: OnGlyphColorChanged);

	public static Color GetGlyphColor(BindableObject view) => (Color)view.GetValue(GlyphColorProperty);
	public static void SetGlyphColor(BindableObject view, Color value) => view.SetValue(GlyphColorProperty, value);

	static void OnGlyphColorChanged(BindableObject view, object oldValue, object newValue)
	{
		var fis = view switch
		{
			Image img => img.Source as FontImageSource,
			MenuItem tabItem => tabItem.IconImageSource as FontImageSource,
			_ => null
		};
		if (fis != null)
			fis.Color = (Color)newValue;
	}

	static void SetGlyph(BindableObject view, string font, uint glyph)
	{
		var glyphText = Convert.ToChar(glyph).ToString();
		FontImageSource createFontImageSource() => new()
		{
			FontFamily = font,
			Glyph = glyphText,
			Color = GetGlyphColor(view)
		};
		if (view is FontImageSource fis)
		{
			fis.FontFamily = font;
			fis.Glyph = glyphText;
		}
		else if (view is ShellContent shellContent)
			shellContent.Icon = createFontImageSource();
		else if (view is Button btn)
		{
			btn.FontFamily = font;
			btn.Text = glyphText;
		}
		else if (view is Image img)
			img.Source = createFontImageSource();
		else if (view is MenuItem tabItem)
			tabItem.IconImageSource = createFontImageSource();
	}
}