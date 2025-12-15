using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Dwarf.Minstrel.Data;
using Dwarf.Minstrel.Messaging;
using Dwarf.Toolkit.Base.SystemExtension;

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
		var radioList = await db.LoadRadioSources();
		RadioSet.DisposeAll();
		RadioSet = radioList.Select(itemFactory.Create).ToArray();
	}

	[RelayCommand]
	async Task Refresh()
	{
		try { await LoadData(); }
		finally { IsRefreshing = false; }
	}

	async Task ShallowRefresh()
	{
		var radioList = await db.LoadRadioSources();
		List<RadioItem> backItems = [.. RadioSet ?? []];
		List<RadioItem> items = [];
		foreach (var rs in radioList)
		{
			var rItem = backItems.Find(ri => ri.Id == rs.Id);
			if (rItem != null) backItems.Remove(rItem);
			else rItem = itemFactory.Create(rs);
			items.Add(rItem);
		}
		backItems.DisposeAll();
		RadioSet = items.ToArray();
	}

	public void Receive(RadiocastMessage message)
	{
		_ = message.Command switch
		{
			RadiocastCommandKind.Refresh => Refresh(),
			RadiocastCommandKind.ShallowRefresh => ShallowRefresh(),
			_ => null
		};
	}
}