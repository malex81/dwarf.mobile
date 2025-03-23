using CommunityToolkit.Maui.Views;

namespace Dwarf.Minstrel.MediaEngine;

public class MediaBox
{
	private readonly MediaElement mediaPlayer;

	public MediaBox(IApplication app)
	{
		var page = ((App)app).MainPage;
		mediaPlayer = new()
		{
			IsVisible = false,
			ShouldAutoPlay = true
		};
		//mediaPlayer.Parent = page;
		page!.AddLogicalChild(mediaPlayer);
		//wnd.Children.Add(mediaPlayer);
	}

	public void PlayURL(string trackURL)
	{
		mediaPlayer.Source = MediaSource.FromUri(trackURL);
		mediaPlayer.Play();
	}
}