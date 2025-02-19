namespace Dwarf.Framework.DIHelpers;

public interface IServicesBatch
{
	void Configure(IServiceCollection services);
}

public interface IServicesBatch<T>
{
	void Configure(IServiceCollection services, T prm);
}
