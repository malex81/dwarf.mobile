using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;
using System.Diagnostics;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Dwarf.Minstrel.WinUI;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : MauiWinUIApplication
{
	static readonly string AppKey = "6FA9D677-3582-404C-BDFF-4D265561DDE8";
	/// <summary>
	/// Initializes the singleton application object.  This is the first line of authored code
	/// executed, and as such is the logical equivalent of main() or WinMain().
	/// </summary>
	public App()
	{
		var singleInstance = AppInstance.FindOrRegisterForKey(AppKey);

		// 2. Если мы НЕ являемся текущим главным процессом
		if (!singleInstance.IsCurrent)
		{
			// Получаем аргументы запуска этой копии
			var currentInstance = AppInstance.GetCurrent();
			var args = currentInstance.GetActivatedEventArgs();

			// Перенаправляем активацию на первую (основную) копию приложения
			singleInstance.RedirectActivationToAsync(args).GetAwaiter().GetResult();

			// Убиваем эту вторую копию, чтобы она не открывала окно
			Process.GetCurrentProcess().Kill();
			return;
		}
		singleInstance.Activated += SingleInstance_Activated;
		this.InitializeComponent();
	}

	private void SingleInstance_Activated(object? sender, AppActivationArguments e)
	{
	}

	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
