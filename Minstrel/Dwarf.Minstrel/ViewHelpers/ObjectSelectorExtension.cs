using System.Globalization;

namespace Dwarf.Minstrel.ViewHelpers;

[ContentProperty(nameof(Variants))]
public partial class ObjectSelectorExtension : BindableObject, IMarkupExtension<BindingBase>
{
	public static readonly BindableProperty KeyProperty
		= BindableProperty.CreateAttached("Key", typeof(object), typeof(ObjectSelectorExtension), null);

	public static object GetKey(BindableObject view) => view.GetValue(KeyProperty);
	public static void SetKey(BindableObject view, object value) => view.SetValue(KeyProperty, value);

	public static readonly BindableProperty SelectedKeyProperty
		= BindableProperty.Create(nameof(SelectedKey), typeof(object), typeof(ObjectSelectorExtension), null);
	public virtual object? SelectedKey
	{
		get => GetValue(SelectedKeyProperty);
		set => SetValue(SelectedKeyProperty, value);
	}

	public ObjectSelectorExtension()
	{
	}

	public string KeyPath { get; set; } = Binding.SelfPath;
	public List<BindableObject> Variants { get; set; } = [];

	protected override void OnPropertyChanged(string propertyName)
	{
		base.OnPropertyChanged(propertyName);
	}

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