using System.Globalization;

namespace Dwarf.Minstrel.ViewHelpers;

[AcceptEmptyServiceProvider]
public partial class SwitchExtension<T> : IMarkupExtension
{
	class SwitchConverter(T onTrue, T onFalse) : IValueConverter
	{
		public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			if (value is bool v)
				return v ? onTrue : onFalse;
			return default(T);
		}

		public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
	}

	/*public static readonly Microsoft.Maui.Controls.BindableProperty ValueProperty = BindableProperty.Create(
		nameof(Value),
		typeof(bool),
		typeof(SwitchExtension<T>),
		propertyChanged: (s, vo, vn) =>
		{
		});

	public bool Value
	{
		get => (bool)GetValue(ValueProperty);
		set => SetValue(ValueProperty, value);
	}*/

	public BindingBase? Value { get; set; }

	//[BindableProperty]
	//public partial bool Value { get; set; }
	public T OnTrue { get; set; } = default!;
	public T OnFalse { get; set; } = default!;

	object? IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
	{
		if (Value is Binding binding)
		{
			binding.Converter = new SwitchConverter(OnTrue, OnFalse);
			return binding;
		}
		return OnFalse;
	}
}

public partial class TextSwitchExtension : SwitchExtension<string>;