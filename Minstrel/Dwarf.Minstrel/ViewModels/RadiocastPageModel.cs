using CommunityToolkit.Mvvm.ComponentModel;
using Dwarf.Minstrel.Data;

namespace Dwarf.Minstrel.ViewModels;

public partial class RadiocastPageModel : ObservableObject
{
	private readonly MinstrelDatabase db;
	private readonly IRadioItemFactory itemFactory;

	[ObservableProperty]
	public partial RadioItem[]? RadioSet { get; set; }

	public RadiocastPageModel(MinstrelDatabase db, IRadioItemFactory itemFactory)
	{
		this.db = db;
		this.itemFactory = itemFactory;
		LoadData();
	}

	async void LoadData()
	{
		var radioList = await db.LoadRadioSources();
		RadioSet = radioList.Select(r => itemFactory.Create(r)).ToArray();
	}
}