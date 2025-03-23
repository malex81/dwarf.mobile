using CommunityToolkit.Maui.Views;

namespace Dwarf.Minstrel.MediaEngine;

public class MediaBox
{
	private readonly MediaElement mediaPlayer;

	public MediaBox(IApplication app)
	{
		var wnd = app.Windows[0];
		mediaPlayer = new()
		{
			IsVisible = false,
			ShouldAutoPlay = true
		};
		wnd.Children.Add(mediaPlayer);
	}
}