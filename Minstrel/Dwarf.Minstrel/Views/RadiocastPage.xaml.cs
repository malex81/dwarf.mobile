using Dwarf.Minstrel.ViewModels;

namespace Dwarf.Minstrel.Views;

public partial class RadiocastPage : ContentPage
{
	private readonly RadiocastModel radiocastModel;

	public RadiocastPage(RadiocastModel radiocastModel)
	{
		InitializeComponent();
		this.radiocastModel = radiocastModel;
	}
}