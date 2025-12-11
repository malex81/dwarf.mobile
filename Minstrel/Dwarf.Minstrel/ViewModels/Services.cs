using Dwarf.Toolkit.Base.DiHelpers;

namespace Dwarf.Minstrel.ViewModels;

class Services : IServicesBatch
{
	public void Configure(IServiceCollection services)
	{
		services.AddSingleton<RadiocastPageModel>();
		services.AddSingleton<ServicesPageModel>();
		services.AddSingleton<AppInfoPageModel>();

		services.RegisterFactory<IRadioItemFactory>();
	}
}