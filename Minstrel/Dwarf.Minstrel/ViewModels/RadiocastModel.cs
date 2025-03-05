using Dwarf.Minstrel.Data;

namespace Dwarf.Minstrel.ViewModels;

public partial class RadiocastModel : IDisposable
{
	private readonly MinstrelDatabase db;

	public RadiocastModel(MinstrelDatabase db)
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