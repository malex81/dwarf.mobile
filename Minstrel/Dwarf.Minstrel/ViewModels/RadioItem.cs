using CommunityToolkit.Maui.Core.Primitives;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Dwarf.Minstrel.Data;
using Dwarf.Minstrel.Data.Tables;
using Dwarf.Minstrel.Helpers;
using Dwarf.Minstrel.MediaEngine;
using Dwarf.Minstrel.Messaging;
using Dwarf.Minstrel.ViewHelpers;
using Dwarf.Toolkit.Basic.SystemExtension;
using System.ComponentModel;

namespace Dwarf.Minstrel.ViewModels;

public partial class RadioItem : ObservableObject, IDisposable
{
	static readonly byte[] DefaultIcon = ResourceHelper.LoadResource("logo.png").GetAwaiter().GetResult();

	private readonly DisposableList dispSelf = [];
	private readonly RadioSource radioSource;
	private readonly MinstrelDatabase db;
	private readonly MediaBox mediaBox;
	private readonly IAlertService alertService;
	private readonly IMessenger messenger;

	public RadioItem(RadioSource radioSource, MinstrelDatabase db, MediaBox mediaBox, IAlertService alertService, IMessenger messenger)
	{
		this.radioSource = radioSource;
		this.db = db;
		this.mediaBox = mediaBox;
		this.alertService = alertService;
		this.messenger = messenger;

		//UpdateState();
		Dispatcher.GetForCurrentThread()!.DispatchDelayed(TimeSpan.FromMilliseconds(100), UpdateState);
		mediaBox.PropertyChanged += MediaBox_PropertyChanged;
		dispSelf.AddAction(() =>
		{
			mediaBox.PropertyChanged -= MediaBox_PropertyChanged;
		});
	}

	private void MediaBox_PropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		if (e.PropertyName == nameof(mediaBox.State))
			UpdateState();
	}

	void UpdateState()
	{
		if (mediaBox.State.URL != StreamUrl)
		{
			IsPlaying = false;
			ShowLoading = false;
			ErrorMessage = null;
			return;
		}
		var curState = mediaBox.State.CurrentSate;
		IsPlaying = curState == MediaElementState.Playing || curState == MediaElementState.Buffering;
		ShowLoading = curState == MediaElementState.Opening || curState == MediaElementState.Buffering;
		ErrorMessage = mediaBox.State.ErrorMessage;
	}

	public int Id => radioSource.Id;
	public string Title => radioSource.Title ?? $"Неизвестное #{Id}";
	public byte[]? Icon => radioSource.Logo ?? DefaultIcon;
	public string? StreamUrl => radioSource.StreamUrl;

	[ObservableProperty]
	public partial bool InFavorites { get; set; }
	[ObservableProperty]
	public partial bool IsPlaying { get; set; }
	[ObservableProperty]
	public partial bool ShowLoading { get; set; } = true;
	[ObservableProperty]
	public partial string? ErrorMessage { get; set; }

	[RelayCommand]
	void ToggleFavorites()
	{
		InFavorites = !InFavorites;
	}

	[RelayCommand]
	void TogglePlaying()
	{
		if (IsPlaying)
			mediaBox.Stop();
		else if (StreamUrl != null)
			mediaBox.PlayURL(StreamUrl);
	}

	[RelayCommand]
	async Task Delete()
	{
		await db.RemoveRadioSource(radioSource);
		messenger.Send(RadiocastMessage.ShallowRefresh);
	}

	[RelayCommand]
	void Refresh()
	{
		messenger.Send(RadiocastMessage.Refresh);
	}

	public void Dispose()
	{
		if (mediaBox.State.URL == StreamUrl)
			mediaBox.Stop();
		dispSelf.Dispose();
		GC.SuppressFinalize(this);
	}
}

public interface IRadioItemFactory
{
	RadioItem Create(RadioSource radioSource);
}