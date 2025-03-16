using CommunityToolkit.Mvvm.ComponentModel;
using Dwarf.Minstrel.Data.Tables;
using Dwarf.Minstrel.Helpers;

namespace Dwarf.Minstrel.ViewModels;

public partial class RadioItem(RadioSource radioSource) : ObservableObject
{
	static readonly byte[] DefaultIcon = ResourceHelper.LoadResource("radio_def_r.png").GetAwaiter().GetResult();

	private readonly RadioSource radioSource = radioSource;

	public int Id => radioSource.Id;
	public string Title => radioSource.Title ?? $"Неизветсное #{Id}";
	public byte[]? Icon => radioSource.Logo ?? DefaultIcon;
}