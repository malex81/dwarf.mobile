using Dwarf.Framework.DIHelpers;

namespace Dwarf.Minstrel.MediaEngine;

public class Services : IServicesBatch
{
	public void Configure(IServiceCollection services)
	{
		services.AddSingleton<MediaBox>();
	}
}