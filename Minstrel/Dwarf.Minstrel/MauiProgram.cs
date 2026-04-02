using CommunityToolkit.Maui;
using Dwarf.Minstrel.Base;
using Dwarf.Minstrel.Views;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;

namespace Dwarf.Minstrel;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.UseMauiCommunityToolkitMediaElement()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", FontNames.OpenSansRegular);
				fonts.AddFont("OpenSans-Semibold.ttf", FontNames.OpenSansSemibold);
				fonts.AddFont("Font Awesome 6 Free-Solid-900.otf", FontNames.FASolid);
				fonts.AddFont("Font Awesome 6 Free-Regular-400.otf", FontNames.FARegular);
				fonts.AddFont("Font Awesome 6 Brands-Regular-400.otf", FontNames.FABrands);
			}).ConfigureMauiHandlers(handlers =>
			{
			}).ConfigureLifecycleEvents(ev =>
			{
#if WINDOWS
				ev.AddWindows(windows => windows
					.OnWindowCreated(window =>
					{
						window.Closed += (s, e) =>
						{
							System.Diagnostics.Process.GetCurrentProcess().Kill();
							//System.Environment.Exit(0);
						};
					}));
#endif
			});
#if DEBUG
		builder.Logging.AddDebug();
#endif

		RegisterRouting();

		builder.Services.AddBatch<Data.Services>()
			.AddBatch<MediaEngine.Services>()
			.AddBatch<ViewHelpers.Services>()
			.AddBatch<Views.Services>()
			.AddBatch<ViewModels.Services>()
			.AddBatch<Messaging.Services>();

		return builder.Build();
	}

	static void RegisterRouting()
	{
		Routing.RegisterRoute("appInfo", typeof(AppInfoPage));
	}
}
