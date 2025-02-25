using Microsoft.Extensions.DependencyInjection;

namespace Dwarf.Framework.DIHelpers;

public static partial class ServiceCollectionExtensions
{
	#region Batch services
	public static IServiceCollection AddBatch(this IServiceCollection services, IServicesBatch batch)
	{
		batch.Configure(services);
		return services;
	}

	public static IServiceCollection AddBatch<TB>(this IServiceCollection services) where TB : IServicesBatch, new()
	{
		return services.AddBatch(new TB());
	}

	public static IServiceCollection AddBatch<TP>(this IServiceCollection services, IServicesBatch<TP> batch, TP prm)
	{
		batch.Configure(services, prm);
		return services;
	}

	public static IServiceCollection AddBatch<TB, TP>(this IServiceCollection services, TP prm) where TB : IServicesBatch<TP>, new()
	{
		return services.AddBatch(new TB(), prm);
	}
	#endregion

	class ImplHolder<T>(IServiceProvider sp)
	{
		public T Impl { get; } = ActivatorUtilities.CreateInstance<T>(sp);
	}
}
