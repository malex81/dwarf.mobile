using CommunityToolkit.Maui.Core.Primitives;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using Dwarf.Toolkit.Basic.SystemExtension;
using System.ComponentModel;

namespace Dwarf.Minstrel.MediaEngine;

public record MediaBaxState(string URL, MediaElementState CurrentSate, string? ErrorMessage = null);
/*
 * https://learn.microsoft.com/ru-ru/dotnet/communitytoolkit/maui/views/mediaelement
 */
public partial class MediaBox : ObservableObject, IDisposable
{
	private readonly IApplication app;
	private readonly DisposableList mediaHandlers = [];
	private MediaElement? mediaPlayer;

	[ObservableProperty]
	public partial MediaBaxState State { get; set; } = new("", MediaElementState.None);
	[ObservableProperty]
	public partial double Volume { get; set; }

	public MediaBox(IApplication app)
	{
		this.app = app;
	}

	MediaElement? GetPlayer()
	{
		if (mediaPlayer == null)
		{
			var page = ((App)app).MainPage;
			mediaPlayer = page!.GetVisualTreeDescendants().OfType<MediaElement>().FirstOrDefault();
			if (mediaPlayer != null)
			{
				mediaPlayer.MediaFailed += MediaPlayer_MediaFailed;
				mediaPlayer.StateChanged += MediaPlayer_StateChanged;
				mediaPlayer.PropertyChanged += MediaPlayer_PropertyChanged;
				mediaHandlers.AddAction(() =>
				{
					mediaPlayer.MediaFailed -= MediaPlayer_MediaFailed;
					mediaPlayer.StateChanged -= MediaPlayer_StateChanged;
					mediaPlayer.PropertyChanged -= MediaPlayer_PropertyChanged;
				});
				mediaPlayer.Volume = Volume;
			}
		}
		return mediaPlayer;
	}

	private void MediaPlayer_PropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		if (mediaPlayer == null)
			return;
		if (e.PropertyName == "Volume")
			Volume = mediaPlayer.Volume;
	}

	partial void OnVolumeChanged(double value)
	{
		mediaPlayer?.Volume = value;
	}

	private void MediaPlayer_StateChanged(object? sender, MediaStateChangedEventArgs e)
	{
		State = State with { CurrentSate = mediaPlayer!.CurrentState };
	}

	private void MediaPlayer_MediaFailed(object? sender, MediaFailedEventArgs e)
	{
		State = State with { CurrentSate = mediaPlayer!.CurrentState, ErrorMessage = e.ErrorMessage };
	}

	public void PlayURL(string trackURL)
	{
		State = new(trackURL, MediaElementState.None);
		var mp = GetPlayer();
		if (mp == null) return;
		try
		{
			mp.Source = MediaSource.FromUri(trackURL);
			mp.Play();
			State = new(trackURL, mp.CurrentState);
		}
		catch
		{
			mp.Stop();
			State = State with { CurrentSate = MediaElementState.None, ErrorMessage = "Во время запуска произошла ошибка" };
		}
	}

	public void Stop()
	{
		var mp = GetPlayer();
		if (mp == null) return;
		mp.Stop();
	}

	public void Dispose()
	{
		GC.SuppressFinalize(this);
	}
}