using Dwarf.Minstrel.ViewModels;

namespace Dwarf.Minstrel.Views;

public partial class ServicesPage : ContentPage
{
	public ServicesPage(ServicesPageModel model)
	{
		InitializeComponent();
		BindingContext = model;
	}
}