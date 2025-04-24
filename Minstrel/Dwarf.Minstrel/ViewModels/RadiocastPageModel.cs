using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Dwarf.Minstrel.Data;

namespace Dwarf.Minstrel.ViewModels;

public partial class RadiocastPageModel : ObservableObject
{
	private readonly MinstrelDatabase db;
	private readonly IRadioItemFactory itemFactory;
	private readonly IMessenger messenger; // https://learn.microsoft.com/ru-ru/dotnet/communitytoolkit/mvvm/messenger

	[ObservableProperty]
	public partial RadioItem[]? RadioSet { get; set; }

	public RadiocastPageModel(MinstrelDatabase db, IRadioItemFactory itemFactory, IMessenger messenger)
	{
		this.db = db;
		this.itemFactory = itemFactory;
		this.messenger = messenger;
		messenger.RegisterAll(this);
		LoadData();
	}

	async void LoadData()
	{
		var radioList = await db.LoadRadioSources();
		RadioSet = radioList.Select(r => itemFactory.Create(r)).ToArray();
	}
}