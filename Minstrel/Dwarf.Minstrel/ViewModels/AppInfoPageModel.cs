using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Reflection;

namespace Dwarf.Minstrel.ViewModels;

public record InfoItem(string Name, string Value);

public partial class AppInfoPageModel : ObservableObject
{
	public InfoItem[] InfoList { get; }

	public AppInfoPageModel()
	{
		InfoList = BuildInfoList().ToArray();
	}

	static IEnumerable<InfoItem> BuildInfoList()
	{
		//var assm = Assembly.GetExecutingAssembly();
		yield return new(AppInfo.Current.Name, "@ Dwarf holl");
		yield return new("Версия", AppInfo.Current.VersionString);
	}
}