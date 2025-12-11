using Dwarf.Toolkit.Base.DiHelpers;

namespace Dwarf.Minstrel.ViewHelpers;

internal class Services : IServicesBatch
{
	public void Configure(IServiceCollection services)
	{
		services.AddSingleton<IAlertService, Inner.AlertService>();
	}
}