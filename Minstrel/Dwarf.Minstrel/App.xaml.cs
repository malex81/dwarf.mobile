using Dwarf.Minstrel.MediaEngine;

namespace Dwarf.Minstrel;

public partial class App : Application
{
	private readonly IServiceProvider services;

	public App(IServiceProvider services)
	{
		this.services = services;
		InitializeComponent();

		MainPage = new AppShell();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		var mediaBox = services.GetService<MediaBox>();
		Window window = base.CreateWindow(activationState);

#if WINDOWS
		Dwarf.Minstrel.Platforms.Windows.StartupConfig.Setup(window);
#endif

		window.Destroying += (s, e) => { mediaBox?.Stop(); };
		return window;
	}
}
