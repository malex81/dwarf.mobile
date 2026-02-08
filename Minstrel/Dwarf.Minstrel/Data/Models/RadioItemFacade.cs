using Dwarf.Minstrel.Data.Tables;

namespace Dwarf.Minstrel.Data.Models;

public sealed class RadioItemFacade
{
	private readonly MinstrelDatabase db;
	private readonly RadioStation radioStation;

	public RadioItemFacade(MinstrelDatabase db, RadioStation radioStation, RadioItemState state)
	{
		this.db = db;
		this.radioStation = radioStation;
		State = state;
	}

	public string Id => radioStation.Id;
	public string Title => radioStation.Title ?? $"Неизвестное #{Id}";
	public string StreamUrl => radioStation.StreamUrl ?? throw new InvalidOperationException("Invalid RadioStation.StreamUrl");
	public ImageSource? Image => radioStation.Image;

	public RadioItemState State { get; }

	public async Task SaveState()
	{
		//var dbc = await db.GetConnection();
		await db.SaveAsync(State);
	}
}