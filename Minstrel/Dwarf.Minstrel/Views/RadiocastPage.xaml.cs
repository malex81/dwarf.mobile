using Dwarf.Minstrel.ViewModels;

namespace Dwarf.Minstrel.Views;

public partial class RadiocastPage : ContentPage, IRadiocastView
{
	//private readonly RadiocastPageModel radiocastModel;

	public RadiocastPage(RadiocastPageModel radiocastModel)
	{
		InitializeComponent();
		//this.radiocastModel = radiocastModel;
		BindingContext = radiocastModel;
		radiocastModel.View = this;
	}

	public void ScrollTo(RadioItemModel itemModel)
	{
		radioListView.ScrollTo(itemModel, animate: false);
	}
}