using MisCore.Framework.SystemExtension;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MisCore.Framework.Tests
{
	[TestFixture]
	internal class WeakReferenceTest
	{
		record SimpleData(int Num);

		const int StartCount = 100000;
		private readonly WeakReferencePool<SimpleData> subject = new();

		[SetUp]
		public void FillData()
		{
			for (int i = 0; i < StartCount; i++)
			{
				subject.Push(new(i));
			}
		}

		[Test]
		public void CollectObjects()
		{
			Assert.That(subject.Count(), Is.LessThanOrEqualTo(StartCount));
			GC.Collect();
			Assert.That(subject.Count(), Is.EqualTo(0));
		}

		[Test]
		public void PartialCollectTest()
		{
			Console.WriteLine($"{subject.Count()} items at the beginning");
			var part = subject.Where(s => s.Num % 10 == 4).ToArray();
			GC.Collect();
			Console.WriteLine($"{part.Length} items found");
			Console.WriteLine($"{subject.Count()} items left");
		}

		[TestCase(10)]
		[TestCase(100)]
		[TestCase(1000)]
		//[TestCase(5000)]
		public void QueryObjects(int iterNum)
		{
			Console.WriteLine($"{subject.Count()} items at the beginning");
			var count = 0;
			var sw = Stopwatch.StartNew();
			for (int i = 0; i < iterNum; i++)
			{
				var obj = subject.FirstOrDefault(s => s.Num > StartCount / 2 + i % 10);
				if (obj != null) count++;
			}
			sw.Stop();
			Thread.Sleep(1);
			Assert.That(count, Is.GreaterThan(0).And.LessThanOrEqualTo(iterNum));
			Console.WriteLine($"Iteration num = {iterNum}; found {count} items; mean time pre item: {sw.Elapsed.TotalMilliseconds / iterNum:g4}ms");
			Console.WriteLine($"{subject.Count()} items left");
		}
	}
}
