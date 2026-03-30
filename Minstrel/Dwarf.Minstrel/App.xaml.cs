using Dwarf.Minstrel.MediaEngine;

namespace Dwarf.Minstrel;

public partial class App : Application
{
	public static Window? MainWindow { get; private set; }

	private readonly IServiceProvider services;

	public App(IServiceProvider services)
	{
		this.services = services;
		InitializeComponent();
#if WINDOWS
		Platforms.Windows.StartupConfig.SetupMapping();
#endif
		//MainPage = new AppShell();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		if(MainWindow != null)
			throw new InvalidOperationException("Main window already created");
		var mediaBox = services.GetService<MediaBox>();
		MainWindow = new(new AppShell());
#if WINDOWS
		Platforms.Windows.StartupConfig.SetupWindow(MainWindow);
#endif
		MainWindow.Destroying += (s, e) => { mediaBox?.Stop(); };
		return MainWindow;
	}
}
