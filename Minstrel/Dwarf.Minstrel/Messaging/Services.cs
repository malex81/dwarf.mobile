using CommunityToolkit.Mvvm.Messaging;
using Dwarf.Toolkit.Basic.DiHelpers;

namespace Dwarf.Minstrel.Messaging;

class Services : IServicesBatch
{
	public void Configure(IServiceCollection services)
	{
		services.AddSingleton<IMessenger, WeakReferenceMessenger>();
	}
}