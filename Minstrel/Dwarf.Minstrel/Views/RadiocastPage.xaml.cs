using Dwarf.Minstrel.ViewModels;

namespace Dwarf.Minstrel.Views;

public partial class RadiocastPage : ContentPage
{
	//private readonly RadiocastPageModel radiocastModel;

	public RadiocastPage(RadiocastPageModel radiocastModel)
	{
		InitializeComponent();
		//this.radiocastModel = radiocastModel;
		BindingContext = radiocastModel;
	}
}