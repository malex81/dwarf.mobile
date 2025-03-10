using CommunityToolkit.Mvvm.ComponentModel;
using Dwarf.Minstrel.Data;

namespace Dwarf.Minstrel.ViewModels;

public partial class RadiocastPageModel : ObservableObject, IDisposable
{
	private readonly MinstrelDatabase db;

	public RadiocastPageModel(MinstrelDatabase db)
	{
		this.db = db;
		LoadData();
	}

	async void LoadData()
	{
		var radioList = await db.LoadRadioSources();
	}

	public void Dispose()
	{
		GC.SuppressFinalize(this);
	}
}