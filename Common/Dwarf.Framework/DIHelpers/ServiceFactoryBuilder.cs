using Castle.DynamicProxy;
using Dwarf.Framework.Caches;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Dwarf.Framework.DIHelpers;

public static class ServiceFactoryBuilder
{
	private static readonly ProxyGenerator generator = new();
	private static readonly ThreadContextCache<Stack<Type>> resolveStackCache = new(
		() => new Stack<Type>(),
		new ThreadContextCache<Stack<Type>>.GarbageCondition
		{
			AllowRecreate = s => s.Count == 0,
			PreferLiveCount = 10,
			MinLifetime = TimeSpan.FromMinutes(5)
		});

	// https://kozmic.net/dynamic-proxy-tutorial/
	class FactoryInterceptor(IServiceProvider provider, Func<MethodInfo, IServiceProvider, Type>? typeSelector = null) : IInterceptor
	{
		private readonly IServiceProvider provider = provider;
		private readonly Func<MethodInfo, IServiceProvider, Type>? typeSelector = typeSelector;

		public void Intercept(IInvocation invocation)
		{
			var fmAttr = invocation.Method.GetCustomAttribute<FactoryMethodAttribute>();
			Type returnType = (typeSelector != null ? typeSelector(invocation.Method, provider) : fmAttr?.ResultType) ?? invocation.Method.ReturnType;
			Stack<Type> resolveStack = resolveStackCache.Current;
			if (resolveStack.Contains(returnType))
				throw new InvalidOperationException($"Circular dependency detected. Involved types: {string.Join("; ", resolveStack.Select(t => t.FullName))}");
			resolveStack.Push(returnType);
			try
			{
				var args = invocation.Arguments.Where(a => a != null).ToArray();
				invocation.ReturnValue = ActivatorUtilities.CreateInstance(provider, returnType, args);
			}
			finally { resolveStack.Pop(); }
		}
	}

	public static Func<IServiceProvider, IF> CreateFactory<IF>(Func<MethodInfo, IServiceProvider, Type>? typeSelector = null) where IF : class
		=> sp => generator.CreateInterfaceProxyWithoutTarget<IF>(new FactoryInterceptor(sp, typeSelector));

	public static IServiceCollection RegisterFactory<IF>(this IServiceCollection services, Func<MethodInfo, IServiceProvider, Type>? typeSelector = null) where IF : class
	{
		services.AddSingleton(sp => generator.CreateInterfaceProxyWithoutTarget<IF>(new FactoryInterceptor(sp, typeSelector)));
		return services;
	}
}
