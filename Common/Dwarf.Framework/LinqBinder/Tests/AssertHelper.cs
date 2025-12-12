using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dwarf.Framework.LinqBinder.Tests;

static class AssertHelper
{
	public static void AllEqual(object expected, params object[] actual)
	{
		foreach (var a in actual)
		{
			ClassicAssert.AreEqual(expected, a);
		}
	}
}
