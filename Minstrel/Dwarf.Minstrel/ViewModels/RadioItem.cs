using CommunityToolkit.Maui.Core.Primitives;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dwarf.Framework.SystemExtension;
using Dwarf.Minstrel.Data.Tables;
using Dwarf.Minstrel.Helpers;
using Dwarf.Minstrel.MediaEngine;
using System.ComponentModel;

namespace Dwarf.Minstrel.ViewModels;

public partial class RadioItem : ObservableObject, IDisposable
{
	static readonly byte[] DefaultIcon = ResourceHelper.LoadResource("radio_def_r.png").GetAwaiter().GetResult();

	private readonly DisposableList dispSelf = [];
	private readonly RadioSource radioSource;
	private readonly MediaBox mediaBox;

	public RadioItem(RadioSource radioSource, MediaBox mediaBox)
	{
		this.radioSource = radioSource;
		this.mediaBox = mediaBox;

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

	public void Dispose()
	{
		dispSelf.Dispose();
		GC.SuppressFinalize(this);
	}
}

public interface IRadioItemFactory
{
	RadioItem Create(RadioSource radioSource);
}