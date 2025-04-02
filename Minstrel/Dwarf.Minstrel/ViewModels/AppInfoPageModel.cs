using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Dwarf.Minstrel.ViewModels;

public partial class AppInfoPageModel : ObservableObject
{
	[RelayCommand]
	void Back() { }
}