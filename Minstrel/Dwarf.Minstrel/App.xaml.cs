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

		// https://learn.microsoft.com/ru-ru/dotnet/maui/user-interface/handlers/?view=net-maui-9.0
		Microsoft.Maui.Handlers.WindowHandler.Mapper.AppendToMapping(nameof(IWindow), (handler, view) =>
		{
#if WINDOWS
			var mauiWindow = handler.VirtualView;
			var nativeWindow = handler.PlatformView;
			nativeWindow.Activate();
			IntPtr windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(nativeWindow);
			var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(windowHandle);
			var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
			appWindow.Resize(new Windows.Graphics.SizeInt32(WindowWidth, WindowHeight));
#endif
		});

		MainPage = new AppShell();
	}

	/*
	 * https://learn.microsoft.com/en-us/dotnet/maui/fundamentals/app-lifecycle?view=net-maui-9.0
	 */
	protected override Window CreateWindow(IActivationState? activationState)
	{
		var mediaBox = services.GetService<MediaBox>();
		Window window = base.CreateWindow(activationState);

		window.Created += (s, e) => { };
		window.Stopped += (s, e) => { };
		window.Destroying += (s, e) => { mediaBox?.Stop(); };
		return window;
	}
}
