using Dwarf.Toolkit.Maui;
using System.Globalization;

namespace Dwarf.Minstrel.ViewHelpers;

[AcceptEmptyServiceProvider]
[ContentProperty(nameof(Variants))]
public partial class ObjectSelectorExtension : BindableObject, IMarkupExtension<BindingBase>
{
	[AttachedProperty]
	public static partial object? GetKey(BindableObject view);

	public ObjectSelectorExtension()
	{
	}

	public string KeyPath { get; set; } = Binding.SelfPath;
	public List<BindableObject> Variants { get; set; } = [];

	public BindingBase ProvideValue(IServiceProvider serviceProvider)
	{
		//var valueProvider = serviceProvider.GetService<IProvideValueTarget>();		
		return new Binding(KeyPath, BindingMode.OneWay, new VariantPickerConverter(this));
	}

	object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
	{
		return ProvideValue(serviceProvider);
	}

	class VariantPickerConverter : IValueConverter
	{
		private readonly ObjectSelectorExtension parent;

		public VariantPickerConverter(ObjectSelectorExtension parent)
		{
			this.parent = parent;
		}

		public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			return parent.Variants.FirstOrDefault(v => Equals(GetKey(v), value));
		}

		public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}