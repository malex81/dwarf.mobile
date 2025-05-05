
using Dwarf.Minstrel.ViewBasic;

namespace Dwarf.Minstrel.ViewHelpers;

public enum AlertIconKind { None, Success, Error }

public interface IAlertService
{
	Task<bool> ShowAlert(string title, string message, string? accept, string cancel);
	IDisposable ShowNotification(string title, string message, AlertIconKind icon = AlertIconKind.None);
}

public static class AlertServiceExtension
{
	public static Task ShowAlert(this IAlertService alertService, string title, string message, string cancel) => alertService.ShowAlert(title, message, null, cancel);
}