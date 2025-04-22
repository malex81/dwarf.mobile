
namespace Dwarf.Minstrel.ViewHelpers;

public interface IAlertService
{
	Task<bool> ShowAlert(string title, string message, string? accept, string cancel);
}

public static class AlertServiceExtension
{
	public static Task ShowAlert(this IAlertService alertService, string title, string message, string cancel) => alertService.ShowAlert(title, message, null, cancel);
}