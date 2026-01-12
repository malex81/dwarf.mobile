using CommunityToolkit.Maui;
using Dwarf.Toolkit.Maui;
using System.Globalization;

namespace Dwarf.Minstrel.ViewHelpers;

public partial class ImageSourceConverter : BindableObject, IValueConverter
{
	public int GlyphSize { get; set; } = 64;

	[BindableProperty(DefaultValueExpression = "Colors.Black")]
	public partial Color GlyphColor { get; set; }

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