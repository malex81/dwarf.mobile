using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Dwarf.Minstrel.Data;
using Dwarf.Minstrel.Data.Models;
using Dwarf.Minstrel.MediaEngine;
using Dwarf.Minstrel.Messaging;
using Dwarf.Toolkit.Basic.AsyncHelpers;
using Dwarf.Toolkit.Basic.SystemExtension;
using System.ComponentModel;

namespace Dwarf.Minstrel.ViewModels;

public sealed partial class RadiocastPageModel : ObservableObject, IRecipient<RadiocastMessage>
{
	private readonly MinstrelDatabase db;
	private readonly IRadioItemFactory itemFactory;
	private readonly IMessenger messenger; // https://learn.microsoft.com/ru-ru/dotnet/communitytoolkit/mvvm/messenger
	private readonly InvalidatorBase vlInvalidator;

	[ObservableProperty]
	//[NotifyPropertyChangedFor(nameof(ViewRadioSet))]
	public partial RadioItemModel[]? RadioSet { get; set; }
	[ObservableProperty]
	public partial bool IsRefreshing { get; set; }
	[ObservableProperty]
	//[NotifyPropertyChangedFor(nameof(ViewRadioSet))]
	public partial string? FilterText { get; set; }
	[ObservableProperty]
	public partial bool FilterRemoved { get; set; }
	[ObservableProperty]
	public partial RadioItemModel[] ViewRadioList { get; set; } = [];

	public VolumeModel VolumeModel { get; }

	public RadiocastPageModel(MinstrelDatabase db, VolumeModel volumeModel, IRadioItemFactory itemFactory, IMessenger messenger)
	{
		this.db = db;
		VolumeModel = volumeModel;
		this.itemFactory = itemFactory;
		this.messenger = messenger;
		messenger.RegisterAll(this);
		_ = LoadData();
		vlInvalidator = ActionFlow.CreateInvalidator(() => ViewRadioList = GetRadioViewSet(), TimeSpan.FromMilliseconds(300));
	}

	readonly string[] filterProps = [nameof(RadioSet), nameof(FilterText), nameof(FilterRemoved)];
	protected override void OnPropertyChanged(PropertyChangedEventArgs e)
	{
		base.OnPropertyChanged(e);
		if (filterProps.Contains(e.PropertyName))
			vlInvalidator.Invalidate();
	}

	RadioItemModel[] GetRadioViewSet()
	{
		if (RadioSet == null) return [];
		var source = RadioSet.Where(r => r.Removed == FilterRemoved);
		if (!string.IsNullOrWhiteSpace(FilterText))
			source = source.Where(r => r.Title.Contains(FilterText, StringComparison.InvariantCultureIgnoreCase));
		return source.OrderByDescending(r => r.InFavorites).ToArray();
	}

	async Task<RadioItemFacade[]> FetchData() => await db.LoadStations();

	async Task LoadData()
	{
		var radioList = await FetchData();
		RadioSet.DisposeAll();
		RadioSet = radioList.Select(item => itemFactory.Create(this, item)).ToArray();
		ForceUpdateViewList();
	}

	[RelayCommand]
	void ForceUpdateViewList() => vlInvalidator.ForceCall();

	[RelayCommand]
	async Task Refresh()
	{
		try { await LoadData(); }
		finally { IsRefreshing = false; }
	}

	async Task ShallowRefresh()
	{
		var radioList = await FetchData();
		List<RadioItemModel> backItems = [.. RadioSet ?? []];
		List<RadioItemModel> items = [];
		foreach (var rs in radioList)
		{
			var rItem = backItems.Find(ri => ri.Id == rs.Id);
			if (rItem != null) backItems.Remove(rItem);
			else rItem = itemFactory.Create(this, rs);
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