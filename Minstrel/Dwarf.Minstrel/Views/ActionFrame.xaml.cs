using Maui.BindableProperty.Generator.Core;

namespace Dwarf.Minstrel.Views;

public partial class ActionFrame : ContentView
{
#pragma warning disable CS0169
	//https://github.com/rrmanzano/maui-bindableproperty-generator
	[AutoBindable]
	private readonly string? _title;
	[AutoBindable]
	private readonly string? _description;
	[AutoBindable]
	private readonly ImageSource? _icon;
	[AutoBindable]
	private readonly ImageSource? _secondaryIcon;
#pragma warning restore CS0169

	public ActionFrame()
	{
		InitializeComponent();
	}

	partial void OnIconChanged(ImageSource? value)
	{
		if (value is FontImageSource fis)
			fis.Size = 64;
	}
}