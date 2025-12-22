using CommunityToolkit.Maui.Views;
using Dwarf.Minstrel.ViewBasic;
using Dwarf.Toolkit.Basic.SystemExtension;

namespace Dwarf.Minstrel.ViewHelpers.Inner;

internal class AlertService : IAlertService
{
	Page SomePage => Application.Current?.MainPage ?? throw new InvalidOperationException("Main page not initialized");

	/// <summary>
	/// Show an alert dialog to the application user with a single cancel button.
	/// </summary>
	/// <param name="title">The title of the alert dialog. Can be <see langword="null"/> to hide the title.</param>
	/// <param name="message">The body text of the alert dialog.</param>
	/// <param name="accept">Text to be displayed on the 'Accept' button. Can be <see langword="null"/> to hide this button.</param>
	/// <param name="cancel">Text to be displayed on the 'Cancel' button.</param>
	/// <returns>A <see cref="Task"/> that contains the user's choice as a <see cref="bool"/> value. <see langword="true"/> indicates that the user accepted the alert. <see langword="false"/> indicates that the user cancelled the alert.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="cancel"/> is <see langword="null"/> or empty.</exception>
	public Task<bool> ShowAlert(string title, string message, string? accept, string cancel) => SomePage.DisplayAlert(title, message, accept, cancel);

	public IDisposable ShowNotification(string title, string message, AlertIconKind icon = AlertIconKind.None)
	{
		var popup = new NotificationPopup(new(title, message, icon));
		SomePage.ShowPopup(popup);
		return DisposableHelper.FromAction(() => { popup.Close(); }, false);
	}
}