namespace Dwarf.Minstrel.ViewModels;

public partial class RadiocastModel : IDisposable
{
	public RadiocastModel()
	{
	}

	public void Dispose()
	{
		GC.SuppressFinalize(this);
	}
}