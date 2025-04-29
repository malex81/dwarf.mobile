using CommunityToolkit.Maui;
using System.Globalization;

namespace Dwarf.Minstrel.ViewHelpers;

public partial class ImageSourceConverter : BindableObject, IValueConverter
{
	public static readonly BindableProperty GlyphColorProperty = BindableProperty.Create(
			nameof(GlyphColor), typeof(Color), typeof(ImageSourceConverter), Colors.Black);

	public virtual Color GlyphColor
	{
		get => (Color)GetValue(GlyphColorProperty);
		set => SetValue(GlyphColorProperty, value);
	}

	public int GlyphSize { get; set; } = 64;
	//public Color GlyphColor { get; set; } = Colors.Black;

	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => value switch
	{
		FontIconDescriptor fDesc => new FontImageSource()
		{
			Size = GlyphSize,
			Color = GlyphColor
		}.WithGlyph(fDesc.GlyphKind),
		_ => value
	};

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}

public record FontIconDescriptor(object GlyphKind);