
namespace Dwarf.Minstrel.ViewHelpers;

[AcceptEmptyServiceProvider]
public partial class PrimitiveExtension : IMarkupExtension
{
	private object? value;

	public bool Bool
	{
		get => value is bool v && v;
		set => this.value = value;
	}

	public int Int
	{
		get => value is int v ? v : default;
		set => this.value = value;
	}

	public object? ProvideValue(IServiceProvider serviceProvider) => value;
}