using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Input;

namespace Dwarf.Minstrel.ViewModels;

public partial class ServiceActionModel : ObservableObject
{
	[ObservableProperty]
	public partial string Title { get; set; }
	[ObservableProperty]
	public partial string Description { get; set; }
	[ObservableProperty]
	public partial ICommand Command { get; set; }
	[ObservableProperty]
	public partial object Icon { get; set; }
	[ObservableProperty]
	public partial object SecondaryIcon { get; set; }

}