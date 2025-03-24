using CommunityToolkit.Maui.Core.Primitives;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using Dwarf.Framework.SystemExtension;

namespace Dwarf.Minstrel.MediaEngine;

public record MediaBaxState(string URL, MediaElementState CurrentSate, string? ErrorMessage = null);

public partial class MediaBox : ObservableObject
{
	private readonly IApplication app;
	private readonly DisposableList mediaHandlers = [];
	private MediaElement? mediaPlayer;

	[ObservableProperty]
	public partial MediaBaxState State { get; set; } = new("", MediaElementState.None);

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
				mediaPlayer.MediaOpened += MediaPlayer_MediaOpened;
				mediaPlayer.MediaFailed += MediaPlayer_MediaFailed;
				mediaHandlers.AddAction(() =>
				{
					mediaPlayer.MediaOpened -= MediaPlayer_MediaOpened;
					mediaPlayer.MediaFailed -= MediaPlayer_MediaFailed;
				});
			}
		}
		return mediaPlayer;
	}

	private void MediaPlayer_MediaOpened(object? sender, EventArgs e)
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
		mp.Source = MediaSource.FromUri(trackURL);
		mp.Play();
	}
}