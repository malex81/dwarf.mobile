using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Dwarf.Framework.SystemExtension;
using Dwarf.Minstrel.Data;
using Dwarf.Minstrel.Messaging;

namespace Dwarf.Minstrel.ViewModels;

public partial class RadiocastPageModel : ObservableObject, IRecipient<RadiocastMessage>
{
	private readonly MinstrelDatabase db;
	private readonly IRadioItemFactory itemFactory;
	private readonly IMessenger messenger; // https://learn.microsoft.com/ru-ru/dotnet/communitytoolkit/mvvm/messenger

	[ObservableProperty]
	public partial RadioItem[]? RadioSet { get; set; }
	[ObservableProperty]
	public partial bool IsRefreshing { get; set; }

	public RadiocastPageModel(MinstrelDatabase db, IRadioItemFactory itemFactory, IMessenger messenger)
	{
		this.db = db;
		this.itemFactory = itemFactory;
		this.messenger = messenger;
		messenger.RegisterAll(this);
		_ = LoadData();
	}

	async Task LoadData()
	{
		IsRefreshing = true;
		//await Task.Delay(1000);
		var radioList = await db.LoadRadioSources();
		RadioSet.DisposeAll();
		RadioSet = radioList.Select(itemFactory.Create).ToArray();
		IsRefreshing = false;
	}

	[RelayCommand]
	async Task Refresh()
	{
		await LoadData();
	}

	public void Receive(RadiocastMessage message)
	{
		if (message.Command == RadiocastCommandKind.Refresh)
			_ = Refresh();
	}
}