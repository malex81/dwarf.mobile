using Dwarf.Minstrel.MediaEngine;

namespace Dwarf.Minstrel;

public partial class App : Application
{
	private readonly IServiceProvider services;

	public App(IServiceProvider services)
	{
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
		window.Destroying += (s, e) => { mediaBox?.Stop(); };
		return window;
	}
}
