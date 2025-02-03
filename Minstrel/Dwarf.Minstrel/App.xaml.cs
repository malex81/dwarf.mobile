namespace Dwarf.Minstrel
{
    public partial class App : Application
    {
		const int WindowWidth = 400;
		const int WindowHeight = 750;

		public App()
        {
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
    }
}
