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
		notifyBox.TranslationY = 300;
		notifyBox.FadeTo(1, 400);
		notifyBox.TranslateTo(0, 0, length: 600, easing: Easing.SpringOut);
	}

	private async void OnOuterFrameTapped(object sender, TappedEventArgs e)
	{
		_ = notifyBox.TranslateTo(0, -200);
		await notifyBox.FadeTo(0);
		Close();
	}
}

public record NotificationPopupModel(string Title, string Message, AlertIconKind Icon);