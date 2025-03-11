using CommunityToolkit.Mvvm.ComponentModel;
using Dwarf.Minstrel.Data;

namespace Dwarf.Minstrel.ViewModels;

public partial class RadiocastPageModel : ObservableObject
{
	private readonly MinstrelDatabase db;

	[ObservableProperty]
	public partial RadioItem[]? RadioSet { get; set; }

	public RadiocastPageModel(MinstrelDatabase db)
	{
		this.db = db;
		LoadData();
	}

	async void LoadData()
	{
		var radioList = await db.LoadRadioSources();
		RadioSet = radioList.Select(r => new RadioItem(r)).ToArray();
	}
}