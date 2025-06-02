using CommunityToolkit.Maui.Views;
using Dwarf.Minstrel.ViewBasic;
using Dwarf.Toolkit.Base.SystemExtension;

namespace Dwarf.Minstrel.ViewHelpers.Inner;

internal class AlertService : IAlertService
{
	Page SomePage => Application.Current?.MainPage ?? throw new InvalidOperationException("Main page not initialized");

	public Task<bool> ShowAlert(string title, string message, string? accept, string cancel) => SomePage.DisplayAlert(title, message, accept, cancel);

	public IDisposable ShowNotification(string title, string message, AlertIconKind icon = AlertIconKind.None)
	{
		var popup = new NotificationPopup(new(title, message, icon));
		SomePage.ShowPopup(popup);
		return DisposableHelper.FromAction(() => { popup.Close(); }, false);
	}
}