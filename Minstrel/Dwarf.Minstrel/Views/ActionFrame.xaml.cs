using Maui.BindableProperty.Generator.Core;

namespace Dwarf.Minstrel.Views;

public partial class ActionFrame : ContentView
{
	//https://github.com/rrmanzano/maui-bindableproperty-generator
	[AutoBindable]
	private readonly string _title;

	public ActionFrame()
	{
		InitializeComponent();
	}
}