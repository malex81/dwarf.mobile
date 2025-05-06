using CommunityToolkit.Maui.Views;
using Dwarf.Minstrel.ViewHelpers;

namespace Dwarf.Minstrel.ViewBasic;

public partial class NotificationPopup : Popup
{
	public NotificationPopup(NotificationPopupModel popupModel)
	{
		BindingContext = popupModel;
		InitializeComponent();

		notifyBox.Opacity = 0;
		notifyBox.TranslationY = 200;
		notifyBox.FadeTo(1);
		notifyBox.TranslateTo(0, 0, easing: Easing.SpringOut);
	}

	protected override async Task OnClosed(object? result, bool wasDismissedByTappingOutsideOfPopup, CancellationToken token = default)
	{
		await base.OnClosed(result, wasDismissedByTappingOutsideOfPopup, token);
	}
}

public record NotificationPopupModel(string Title, string Message, AlertIconKind Icon);