using CommunityToolkit.Maui.Core.Primitives;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using Dwarf.Minstrel.Helpers;

namespace Dwarf.Minstrel;

public partial class MainPage : ContentPage
{
	private readonly MediaElement mediaPlayer;

	public MainPage()
	{
		InitializeComponent();
		BindingContext = new MainPageModel();
		mediaPlayer = new()
		{
			IsVisible = false,
			ShouldAutoPlay = true
		};
		MainPane.Children.Add(mediaPlayer);
	}


	private void OnCounterClicked(object sender, EventArgs e)
	{
		if (mediaPlayer.CurrentState != MediaElementState.Playing)
		{
			string trackURL = "https://radio-holding.ru:9433/marusya_default";
			//"https://prod-1.storage.jamendo.com/?trackid=1890762&format=mp31&from=b5bSbOTAT1kXawaT8EV9IA%3D%3D%7CGcDX%2BeejT3P%2F0CfPwtSyYA%3D%3D";
			mediaPlayer.Source = MediaSource.FromUri(trackURL);
			mediaPlayer.Play();
		}
		else
		{
			mediaPlayer.Pause();
		}
	}
}

public partial class MainPageModel : ObservableObject
{

	[ObservableProperty]
	public partial byte[]? ImageSample { get; set; }

	public MainPageModel()
	{
		LoadImageTest();
	}

	async void LoadImageTest()
	{
		ImageSample = await ResourceHelper.LoadResource("marusy_fm.webp");
	}

}
