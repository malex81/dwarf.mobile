using Dwarf.Toolkit.Maui;

namespace Dwarf.Maui.Base.Controls;

public partial class SearchBox : ContentView
{
	[BindableProperty]
	public partial string Placeholder { get; set; }

	[BindableProperty(DefaultBindingMode = BindingModeDef.TwoWay)]
	public partial string Text { get; set; }

	public SearchBox()
	{
		InitializeComponent();
	}

	partial void OnTextChanged(string oldValue, string newValue)
	{
		clearButton.IsVisible = !string.IsNullOrEmpty(Text);
	}

	private void OnClearClicked(object sender, EventArgs e)
	{
		Text = string.Empty;
	}
}