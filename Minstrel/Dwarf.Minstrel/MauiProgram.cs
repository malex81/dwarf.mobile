using CommunityToolkit.Maui;
using Dwarf.Framework.DIHelpers;
using Microsoft.Extensions.Logging;

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
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				fonts.AddFont("Font Awesome 6 Free-Regular-400.otf", "FARegular");
				fonts.AddFont("Font Awesome 6 Brands-Regular-400.otf", "FABrands");
				fonts.AddFont("Font Awesome 6 Free-Solid-900.otf", "FASolid");
			}).ConfigureMauiHandlers(handlers =>
			{
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		builder.Services.AddSingleton<MainPage>();

		builder.Services.AddSingleton<ViewModels.RadiocastPageModel>();
		builder.Services.AddSingleton<ViewModels.ServicesPageModel>();

		builder.Services.AddSingleton<Views.RadiocastPage>();
		builder.Services.AddSingleton<Views.ServicesPage>();

		builder.Services.AddBatch<Data.Services>();
		return builder.Build();
	}
}
