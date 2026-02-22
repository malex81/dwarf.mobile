using Dwarf.Minstrel.MediaEngine;

namespace Dwarf.Minstrel;

public partial class App : Application
{
#if WINDOWS
	const int WindowWidth = 480;
	const int WindowHeight = 800;
#endif
	private readonly IServiceProvider services;

	public App(IServiceProvider services)
	{
		this.services = services;
		InitializeComponent();
	}

	/*
	 * https://learn.microsoft.com/en-us/dotnet/maui/fundamentals/app-lifecycle?view=net-maui-9.0
	 */
	protected override Window CreateWindow(IActivationState? activationState)
	{
		var mediaBox = services.GetService<MediaBox>();
		Window window = new(new AppShell());

#if WINDOWS
		window.Width = WindowWidth;
		window.Height = WindowHeight;
		window.MinimumWidth = WindowWidth;
		window.MinimumHeight = WindowHeight;
#endif

		window.Created += (s, e) => { };
		window.Stopped += (s, e) => { };
		window.Destroying += (s, e) => { mediaBox?.Stop(); };
		return window;
	}
}
