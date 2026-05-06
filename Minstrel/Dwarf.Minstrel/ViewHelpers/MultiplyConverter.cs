using System.Globalization;

namespace Dwarf.Minstrel.ViewHelpers;

internal sealed class MultiplyConverter : IValueConverter
{
	public double Factor { get; set; } = 1.0;

	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => value switch
	{
		double v => v * Factor,
		float v => (float)(v * Factor),
		int v => (int)(v * Factor),
		_ => value
	};

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => value switch
	{
		double v => v / Factor,
		float v => (float)(v / Factor),
		int v => (int)(v / Factor),
		_ => value
	};
}