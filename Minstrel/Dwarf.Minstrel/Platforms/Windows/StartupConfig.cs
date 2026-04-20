using Microsoft.UI.Windowing;
using SysDrow = System.Drawing;
using WinForms = System.Windows.Forms;

namespace Dwarf.Minstrel.Platforms.Windows;

internal static class StartupConfig
{
	const int WindowWidth = 480;
	const int WindowHeight = 800;

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
						var cursor = NativeMethods.LoadCursor(IntPtr.Zero, NativeMethods.IDC_ARROW);
						NativeMethods.SetCursor(cursor);
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

		trayIcon.DoubleClick += (s, e) =>
		{
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
		// 3. Выводим на передний план
		platformWindow.Activate();
	}
}