using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Dwarf.Minstrel.Data;
using Dwarf.Minstrel.Helpers;
using Dwarf.Minstrel.MediaEngine;
using Dwarf.Minstrel.Messaging;
using Dwarf.Minstrel.ViewHelpers;

namespace Dwarf.Minstrel.ViewModels;

public partial class ServicesPageModel : ObservableObject
{
	const string ClearDbTitle = "Пересоздание БД";

	private readonly MinstrelDatabase db;
	private readonly MediaBox mediaBox;
	private readonly IAlertService alertService;
	private readonly IMessenger messenger;

	public ServicesPageModel(MinstrelDatabase db, MediaBox mediaBox, IAlertService alertService, IMessenger messenger)
	{
		this.db = db;
		this.mediaBox = mediaBox;
		this.alertService = alertService;
		this.messenger = messenger;
	}

	[RelayCommand]
	async Task ShowAppInfo()
	{
		await Shell.Current.GoToAsync("appInfo");
	}

	[RelayCommand]
	async Task ClearDb()
	{
		if (await alertService.ShowAlert(ClearDbTitle, "Будет выполнена полная очистка всей базы данных. Продолжить?", "Да", "Нет"))
		{
			try
			{
				await db.RecreateDb();
				messenger.Send(RadiocastMessage.Refresh);
				alertService.ShowNotification(ClearDbTitle, "База данных успешно пересоздана", AlertIconKind.Success);
			}
			catch (Exception ex)
			{
				alertService.ShowNotification("Ошибка!", ex.Message, AlertIconKind.Danger);
			}
		}
	}
}