using Dwarf.Minstrel.MediaEngine;
using Microsoft.Extensions.Logging;

namespace Dwarf.Minstrel;

public partial class App : Application
{
	private readonly IServiceProvider services;
	private readonly ILogger<App> _logger;

	public App(IServiceProvider services, ILogger<App> logger)
	{
		_logger = logger;
		_logger.LogInformation("App started ...");
		this.services = services;
		InitializeComponent();
#if WINDOWS
		Platforms.Windows.StartupConfig.SetupMapping();
#endif
		MainPage = new AppShell();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		var mediaBox = services.GetService<MediaBox>();
		Window window = base.CreateWindow(activationState);
#if WINDOWS
		Platforms.Windows.StartupConfig.SetupWindow(window);
#endif
		window.Destroying += (s, e) => {
			mediaBox?.Stop();
			Helpers.LogService.Shutdown();
		};
		return window;
	}
}
