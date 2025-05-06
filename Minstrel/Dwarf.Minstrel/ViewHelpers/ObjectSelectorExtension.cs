namespace Dwarf.Minstrel.ViewHelpers;

[ContentProperty(nameof(Variants))]
public partial class ObjectSelectorExtension : BindableObject, IMarkupExtension
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

	protected override void OnPropertyChanged(string propertyName)
	{
		base.OnPropertyChanged(propertyName);
	}

	public object? ProvideValue(IServiceProvider serviceProvider)
	{
		var valueProvider = serviceProvider.GetService<IProvideValueTarget>();

		return Variants.FirstOrDefault(v => GetKey(v) == SelectedKey);
	}

	public List<BindableObject> Variants { get; set; } = [];
}