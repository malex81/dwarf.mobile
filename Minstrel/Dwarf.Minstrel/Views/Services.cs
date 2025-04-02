﻿using Dwarf.Framework.DIHelpers;

namespace Dwarf.Minstrel.Views;

class Services : IServicesBatch
{
	public void Configure(IServiceCollection services)
	{
		services.AddSingleton<RadiocastPage>();
		services.AddSingleton<ServicesPage>();
		services.AddSingleton<AppInfoPage>();
	}
}