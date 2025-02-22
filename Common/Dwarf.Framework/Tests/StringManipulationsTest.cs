using System;
using System.Collections.Generic;
using System.Text;
using MisCore.Framework.SystemExtension;
using NUnit.Framework;

namespace MisCore.Framework.Tests
{
	[TestFixture]
	public class StringManipulationsTest
	{
		[TestCase]
		public void WildcardCheck()
		{
			var str1 = "Вася.Петя.Коля";
			Assert.That(str1.CompareWildcard("*"));
			Assert.That(str1.CompareWildcard("***"));
			Assert.That(str1.CompareWildcard("Вася*"));
			Assert.That(str1.CompareWildcard("Вася****"));
			Assert.That(str1.CompareWildcard("Вася.Петя.*"));
			Assert.That(str1.CompareWildcard("*.Петя.*"));
			Assert.That(str1.CompareWildcard("***.Петя.**"));
			Assert.That(str1.CompareWildcard("*Петя.Коля"));
			Assert.That(str1.CompareWildcard("Вася.*.Коля"));
			Assert.That(str1.CompareWildcard("Вася.**.Коля"));

			Assert.That(str1.CompareWildcard("*.*."), Is.EqualTo(false));
		}

		[TestCase(100000)]
		public void WildcardPerformance(int num)
		{
			var str1 = "Proposed package version from current time";
			var str2 = "Build started: Project: Alda.Web.Core3, Configuration: Tests Any CPU";
			var str3 = "Новобранец, кидая гранату, помни: ее взрыв должен убить еще кого-то кроме тебя!";
			var res = true;
			for (int i = 0; i < num; i++)
			{
				res &= str1.CompareWildcard("*package*version?from*time");
				res &= str2.CompareWildcard("Build started*Alda.Web*:???sts*");
				res &= str3.CompareWildcard("*ранец??кидая*помни*взрыв должен*кроме тебя?");
			}
			Assert.That(res);
		}
	}
}
