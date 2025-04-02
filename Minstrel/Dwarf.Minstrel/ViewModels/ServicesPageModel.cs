using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Dwarf.Minstrel.ViewModels;

public partial class ServicesPageModel : ObservableObject
{
	[ObservableProperty]
	public partial bool EnableInfo { get; set; }

	[RelayCommand]
	async Task ShowAppInfo()
	{
		await Task.Delay(10);
		await Shell.Current.GoToAsync("appInfo");
	}

	[RelayCommand]
	async Task Test()
	{
		await Task.Delay(200);
		EnableInfo = !EnableInfo;
	}
}