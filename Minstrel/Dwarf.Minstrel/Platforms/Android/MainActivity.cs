using Android.App;
using Android.Content.PM;
using Android.OS;

namespace Dwarf.Minstrel;

/*
 * https://learn.microsoft.com/ru-ru/dotnet/communitytoolkit/maui/views/mediaelement?tabs=android
 */

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ResizeableActivity = true, LaunchMode = LaunchMode.SingleTask, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density, ScreenOrientation = ScreenOrientation.Portrait)]
public class MainActivity : MauiAppCompatActivity
{
	protected override void OnCreate(Bundle? savedInstanceState)
	{
		base.OnCreate(savedInstanceState);
		// Указываем системе, что кнопки громкости должны управлять мультимедиа
		VolumeControlStream = Android.Media.Stream.Music;
	}
}
