using Dwarf.Framework.DIHelpers;

namespace Dwarf.Minstrel.ViewModels;

class Services : IServicesBatch
{
	public void Configure(IServiceCollection services)
	{
		services.AddSingleton<RadiocastPageModel>();
		services.AddSingleton<ServicesPageModel>();

		services.RegisterFactory<IRadioItemFactory>();
	}
}