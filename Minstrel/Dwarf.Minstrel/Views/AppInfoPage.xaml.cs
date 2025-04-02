using Dwarf.Minstrel.ViewModels;

namespace Dwarf.Minstrel.Views;

public partial class AppInfoPage : ContentPage
{
	public AppInfoPage(AppInfoPageModel model)
	{
		InitializeComponent();
		BindingContext = model;
	}
}