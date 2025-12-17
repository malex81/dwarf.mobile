using Dwarf.Toolkit.Basic.DiHelpers;

namespace Dwarf.Minstrel.Data;

internal class Services : IServicesBatch
{
	public void Configure(IServiceCollection services)
	{
		services.AddBatch<Grabbers.Services>();

		services.AddSingleton<MinstrelDatabase>();
	}
}