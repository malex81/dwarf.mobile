using System;

namespace Dwarf.Framework.DIHelpers;

[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public sealed class FactoryMethodAttribute(Type? resultType = null) : Attribute
{
	public Type? ResultType => resultType;
}
