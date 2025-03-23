using CommunityToolkit.Maui.Core.Primitives;
using CommunityToolkit.Maui.Views;
using Dwarf.Framework.SystemExtension;

namespace Dwarf.Minstrel.MediaEngine;

public record MediaState(string URL, MediaElementState CurrentSate, string ErrorMessage);

public class MediaBox
{
	private readonly IApplication app;
	private readonly DisposableList mediaHandlers = [];
	private MediaElement? mediaPlayer;

	public MediaBox(IApplication app)
	{
		//mediaPlayer = new()
		//{
		//	IsVisible = false,
		//	ShouldAutoPlay = true
		//};
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
	}

	private void MediaPlayer_MediaFailed(object? sender, MediaFailedEventArgs e)
	{
	}

	public void PlayURL(string trackURL)
	{
		var mp = GetPlayer();
		if (mp == null) return;
		mp.Source = MediaSource.FromUri(trackURL);
		mp.Play();
	}
}