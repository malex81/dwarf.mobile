using Dwarf.Minstrel.MediaEngine;
using System.Runtime.InteropServices;

namespace Dwarf.Minstrel;

public partial class App : Application
{
	private readonly IServiceProvider services;

	public App(IServiceProvider services)
	{
		this.services = services;
		InitializeComponent();


#if WINDOWS
		Microsoft.Maui.Handlers.ElementHandler.ElementMapper.AppendToMapping("FixCursor", (handler, view) =>
		{
			if (view is Border && handler.PlatformView is Microsoft.UI.Xaml.UIElement element)
			{
				element.PointerPressed += (s, e) =>
				{
					if (e.GetCurrentPoint(element).Properties.IsRightButtonPressed)
					{
						var cursor = Platforms.Windows.NativeMethods.LoadCursor(IntPtr.Zero, Platforms.Windows.NativeMethods.IDC_ARROW);
						Platforms.Windows.NativeMethods.SetCursor(cursor);
					}
				};
			}
		});
#endif

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
