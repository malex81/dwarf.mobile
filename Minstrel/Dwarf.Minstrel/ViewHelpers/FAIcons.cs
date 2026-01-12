using Dwarf.Minstrel.Base;
using Dwarf.Toolkit.Maui;

namespace Dwarf.Minstrel.ViewHelpers;
/*
 * https://fontawesome.com/v6/search?ic=free-collection
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
	Database = 0xf1c0,
	ChevronUp = 0xf077,
	ChevronDown = 0xf078,
	ChevronRight = 0xf054,
	ChevronLeft = 0xf053,
	CircleChevronUp = 0xf139,
	CircleChevronDown = 0xf13a,
	CircleChevronRight = 0xf138,
	CircleChevronLeft = 0xf137,
	CircleNotch = 0xf1ce,
	Check = 0xf00c,
	CheckDouble = 0xf560,
	SquareCheck = 0xf14a,
	CircleCheck = 0xf058,
	ListCheck = 0xf0ae,
	Cloud = 0xf0c2,
	CloudArrowUp = 0xf0ee,
	CloudArrowDown = 0xf0ed,
	Upload = 0xf093,
	Download = 0xf019,
	VolumnOff = 0xf026,
	VolumnLow = 0xf027,
	VolumnHigh = 0xf028,
	VolumnXMark = 0xf6a9
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
	SquareCheck = 0xf14a,
	CircleCheck = 0xf058,
}

public record FontIconDescriptor(object GlyphKind);

public static partial class FAIcons
{
	[AttachedProperty(DefaultValue = FASolidGlyphs.None)]
	public static partial FASolidGlyphs GetSolidGlyph(BindableObject view);
	[AttachedProperty(DefaultValue = FARegularGlyphs.None)]
	public static partial FARegularGlyphs GetRegularGlyph(BindableObject view);
	[AttachedProperty()]
	public static partial FontIconDescriptor GetGlyphDescriptor(BindableObject view);
	[AttachedProperty]
	public static partial Color? GetGlyphColor(BindableObject view);
	[AttachedProperty(DefaultValue = 30d)]
	public static partial double GetGlyphSize(BindableObject view);

	static void OnSolidGlyphChanged(BindableObject view, FASolidGlyphs glyph)
	{
		if (glyph == FASolidGlyphs.None) return;
		SetGlyph(view, FontNames.FASolid, (uint)glyph);
	}

	static void OnRegularGlyphChanged(BindableObject view, FARegularGlyphs glyph)
	{
		if (glyph == FARegularGlyphs.None) return;
		SetGlyph(view, FontNames.FARegular, (uint)glyph);
	}

	static void OnGlyphDescriptorChanged(BindableObject view, FontIconDescriptor desc)
	{
		if (desc == null) return;
		var (font, glyph) = desc.GlyphKind switch
		{
			FASolidGlyphs faSolidGlyph => (FontNames.FASolid, (uint)faSolidGlyph),
			FARegularGlyphs faRegularGlyph => (FontNames.FARegular, (uint)faRegularGlyph),
			_ => throw new InvalidOperationException($"Unknown glyph kind {desc.GlyphKind}")
		};
		SetGlyph(view, font, glyph);
	}

	static void OnGlyphColorChanged(BindableObject view, Color? color) => view.GetImageSource()?.Color = color;
	static void OnGlyphSizeChanged(BindableObject view, double size) => view.GetImageSource()?.Size = size;

	static FontImageSource? GetImageSource(this BindableObject view) => view switch
	{
		Image img => img.Source as FontImageSource,
		MenuItem tabItem => tabItem.IconImageSource as FontImageSource,
		_ => null
	};

	static void SetGlyph(BindableObject view, string font, uint glyph)
	{
		var glyphText = Convert.ToChar(glyph).ToString();
		FontImageSource createFontImageSource() => new()
		{
			FontFamily = font,
			Glyph = glyphText,
			Color = GetGlyphColor(view),
			Size = GetGlyphSize(view)
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