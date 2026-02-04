using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Dwarf.Minstrel.Data;
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
	public partial RadioItem[]? RadioSet { get; set; }
	[ObservableProperty]
	public partial bool IsRefreshing { get; set; }
	[ObservableProperty]
	//[NotifyPropertyChangedFor(nameof(ViewRadioSet))]
	public partial string? FilterText { get; set; }
	[ObservableProperty]
	public partial RadioItem[] ViewRadioList { get; set; } = [];

	public VolumeModel VolumeModel { get; }

	public RadiocastPageModel(MinstrelDatabase db, VolumeModel volumeModel, IRadioItemFactory itemFactory, IMessenger messenger)
	{
		this.db = db;
		VolumeModel = volumeModel;
		this.itemFactory = itemFactory;
		this.messenger = messenger;
		messenger.RegisterAll(this);
		_ = LoadData();
		vlInvalidator = ActionFlow.CreateInvalidator(() => ViewRadioList = GetRadioViewSet(), TimeSpan.FromMilliseconds(1000));
	}

	protected override void OnPropertyChanged(PropertyChangedEventArgs e)
	{
		base.OnPropertyChanged(e);
		if (e.PropertyName == nameof(RadioSet) || e.PropertyName == nameof(FilterText))
			vlInvalidator.Invalidate();
	}

	RadioItem[] GetRadioViewSet()
	{
		if (RadioSet == null) return [];
		var source = string.IsNullOrWhiteSpace(FilterText) ? RadioSet
			: RadioSet.Where(r => r.Title.Contains(FilterText, StringComparison.InvariantCultureIgnoreCase));
		return source.OrderByDescending(r => r.InFavorites).ToArray();
	}

	async Task LoadData()
	{
		var radioList = await db.LoadStations();
		RadioSet.DisposeAll();
		RadioSet = radioList.Select(itemFactory.Create).ToArray();
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
		/*		var radioList = await db.LoadRadioSources();
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
		*/
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