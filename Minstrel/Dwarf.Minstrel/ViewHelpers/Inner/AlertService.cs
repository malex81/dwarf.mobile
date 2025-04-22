namespace Dwarf.Minstrel.ViewHelpers.Inner;

internal class AlertService : IAlertService
{
	Page SomePage => Application.Current?.MainPage ?? throw new InvalidOperationException("Main page not initialized");

	public Task<bool> ShowAlert(string title, string message, string? accept, string cancel) => SomePage.DisplayAlert(title, message, accept, cancel);
}