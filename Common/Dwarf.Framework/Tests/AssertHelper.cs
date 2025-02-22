using MisCore;
using MisCore.Framework;
using MisCore.Framework.Tests;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MisCore.Framework.Tests
{
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
}
