using Microsoft.UI.Windowing;
using WinRT.Interop;
using SysDrow = System.Drawing;
using WinForms = System.Windows.Forms;
using static Dwarf.Minstrel.Platforms.Windows.NativeMethods;

namespace Dwarf.Minstrel.Platforms.Windows;

internal static class StartupConfig
{
	const int WindowWidth = 480;
	const int WindowHeight = 800;

	static Window? mainWindow;

	public static void SetupMapping()
	{
		FixCursor();
	}

	static void FixCursor()
	{
		Microsoft.Maui.Handlers.ElementHandler.ElementMapper.AppendToMapping("FixCursor", (handler, view) =>
		{
			if (view is Border && handler.PlatformView is Microsoft.UI.Xaml.UIElement element)
			{
				element.PointerPressed += (s, e) =>
				{
					if (e.GetCurrentPoint(element).Properties.IsRightButtonPressed)
					{
						var cursor = LoadCursor(IntPtr.Zero, IDC_ARROW);
						SetCursor(cursor);
					}
				};
			}
		});
	}

	public static void SetupWindow(Window window)
	{
		window.MinimumWidth = WindowWidth;
		window.MinimumHeight = WindowHeight;
		window.KeepBounds(Preferences.Default, new(WindowWidth, WindowHeight));
		window.ConfigureWindowPresentation();
		window.PerformOnPlatform(plWin =>
		{
			if (plWin.AppWindow.Presenter is Microsoft.UI.Windowing.OverlappedPresenter presenter)
			{
				presenter.IsMaximizable = false;
			}
		});
		mainWindow = window;
	}

	static WinForms.NotifyIcon? trayIcon;

	static void ConfigureWindowPresentation(this Window window)
	{
		if (trayIcon != null)
			throw new InvalidOperationException("Tray icon already initialized");

		trayIcon = new WinForms.NotifyIcon
		{
			Icon = new SysDrow.Icon(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logo_win.ico")),
			Text = "Minstrel",
			Visible = true
		};

		window.Destroying += (s, e) =>
		{
			trayIcon.Visible = false;
			trayIcon.Dispose();
		};

		trayIcon.Click += (s, e) =>
		{
			if (e is WinForms.MouseEventArgs ma && ma.Button == WinForms.MouseButtons.Left)
				window.TryActivate();
		};

		// Добавление контекстного меню
		trayIcon.ContextMenuStrip = new WinForms.ContextMenuStrip();
		trayIcon.ContextMenuStrip.Items.Add("Выйти", null, (s, e) => Application.Current?.Quit());
		trayIcon.ContextMenuStrip.Items.Add("Показать", null, (s, e) => window.TryActivate());

		window.PerformOnPlatform(platformWindow =>
		{
			var appWindow = platformWindow.AppWindow;
			appWindow.Changed += (sender, args) =>
			{
				if (args.DidPresenterChange && sender.Presenter is OverlappedPresenter presenter)
				{

					if (presenter.State == OverlappedPresenterState.Minimized || presenter.State == OverlappedPresenterState.Maximized)
					{
						appWindow.Hide();
					}
					//if (presenter.State == OverlappedPresenterState.Maximized)
					//{
					//	presenter.Restore();
					//}
				}
			};
		});
	}

	static void TryActivate(this Window window)
	{
		if (window.Handler?.PlatformView is not Microsoft.UI.Xaml.Window platformWindow)
			return;
		// 1. Показываем в панели задач
		platformWindow.AppWindow.Show();
		// 2. Устанавливаем обычный размер (если оно было свернуто)
		if (platformWindow.AppWindow.Presenter is OverlappedPresenter presenter)
		{
			presenter.Restore();
		}

		IntPtr hwnd = WindowNative.GetWindowHandle(platformWindow);
		ShowWindow(hwnd, SW_RESTORE);
		SetForegroundWindow(hwnd);

		// 3. Выводим на передний план
		platformWindow.Activate();
	}

	public static void TryActivate()
	{
		mainWindow?.Dispatcher.Dispatch(() =>
		{
			mainWindow.TryActivate();
		});
	}
}