using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using MisCore.Framework.SystemExtension;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace MisCore.Framework.Tests
{
	[TestFixture]
	public class ObjectManipulationsTest
	{
		#region Test objects

		class TargetObject
		{
			public static string SS { get; set; } = "static prop on obj 1";
			public string a;
			private string PS { get; set; } = "private prop on obj 1";
			public int A { get; set; } = 10;
			public int A2 { get; set; } = 20;
			public string AS { get; set; } = "25";

			public string B { get; set; } = "property B on obj 1";
			public string C { get; } = "read only prop";

			public string InnerConcat { get { return SS + "|" + PS; } }
		}

		class SourceObject
		{
			public static string SS { get; set; } = "static prop on obj 2";
			public string a;
			private string PS { get; set; } = "private prop on obj 2";
			public double A { get; set; } = 4.5;
			public string A2 { get; set; } = "22";
			public int AS { get; set; } = 16;

			public string B { get; } = "property B on obj 2";
			public string C { get; set; } = "prop C on obj 2";
		}

		public class ComplexSubObject : IEquatable<ComplexSubObject>
		{
			public int SubNum { get; set; }
			public string Comment { get; set; }

			public bool Equals([AllowNull] ComplexSubObject other)
			{
				return other != null && other.SubNum == SubNum && other.Comment == Comment;
			}
		}

		public class ComplexItem : IEquatable<ComplexItem>
		{
			public double Num { get; set; }
			public string Comment { get; set; }

			public bool Equals([AllowNull] ComplexItem other)
			{
				return other != null && other.Num == Num && other.Comment == Comment;
			}
		}

		public class ComplexObject : IEquatable<ComplexObject>
		{
			public double Num { get; set; }
			public string Text { get; set; }

			public ComplexSubObject SubObject { get; set; }
			public ComplexSubObject SubObjectReadOnly { get; } = new ComplexSubObject();

			public ComplexItem[] Array { get; set; }

			public bool Equals([AllowNull] ComplexObject other)
			{
				return other != null && other.Num == Num && other.Text == Text
					&& (other.SubObject == SubObject || SubObject != null && SubObject.Equals(other.SubObject));
			}
		}

		#endregion

		#region ShallowCopy tests

		[TestCase]
		public void ShallowCopyObjects()
		{
			var target = new TargetObject();
			var source = new SourceObject();
			target.ShallowCopyFrom(source);

			ClassicAssert.AreEqual(10, target.A);
			ClassicAssert.AreEqual("25", target.AS);
			ClassicAssert.AreEqual(20, target.A2);
			ClassicAssert.AreEqual("property B on obj 2", target.B);
			ClassicAssert.AreEqual("read only prop", target.C);
			ClassicAssert.AreEqual("static prop on obj 1|private prop on obj 1", target.InnerConcat);
		}
		[TestCase]
		public void ShallowCopyWithConvert()
		{
			var target = new TargetObject();
			var source = new SourceObject();
			target.ShallowCopyFrom(source, new ShallowCopyOptions { TryConvert = true });

			ClassicAssert.AreEqual(4, target.A);
			ClassicAssert.AreEqual("16", target.AS);
			ClassicAssert.AreEqual(20, target.A2);
			ClassicAssert.AreEqual("property B on obj 2", target.B);
			ClassicAssert.AreEqual("read only prop", target.C);
			ClassicAssert.AreEqual("static prop on obj 1|private prop on obj 1", target.InnerConcat);
		}
		[TestCase]
		public void ShallowCopyWithMapping()
		{
			var target = new TargetObject();
			var source = new SourceObject();
			target.ShallowCopyFrom(source, new ShallowCopyOptions
			{
				TryConvert = true,
				NameMapping = new Dictionary<string, string>
				{
					{ nameof(source.AS), nameof(target.A) },
					{ nameof(SourceObject.A), nameof(TargetObject.AS) }
				}
			});

			ClassicAssert.AreEqual(16, target.A);
			ClassicAssert.AreEqual(source.A.ToString(), target.AS);
			ClassicAssert.AreEqual(20, target.A2);
			ClassicAssert.AreEqual("property B on obj 2", target.B);
			ClassicAssert.AreEqual("read only prop", target.C);
		}

		#endregion

		#region DeepCopy tests

		[TestCase]
		public void DeepCopySimple()
		{
			var source = new ComplexObject
			{
				Num = 123.1234567,
				Text = "Вася - дурачок",
				SubObject = new ComplexSubObject { SubNum = 8, Comment = "Дуся всем нравится" },
				Array = new[] { new ComplexItem { Num = 11.111, Comment = "qwerty" }, new ComplexItem { Num = 22.222, Comment = "aaaaa" }, new ComplexItem { Num = 333.33, Comment = "rfvedc" } }
			};
			source.SubObjectReadOnly.Comment = "Коля нравится Дусе";

			var res = source.DeepClone();
			ClassicAssert.AreEqual(source.Num, res.Num);
			ClassicAssert.AreEqual(source.Text, res.Text);
			ClassicAssert.AreEqual(source.SubObject.Comment, res.SubObject.Comment);
			ClassicAssert.AreEqual(res.SubObjectReadOnly.Comment, null); // property has getter accessor only
			ClassicAssert.IsTrue(source.Array.SequenceEqual(res.Array));
		}

		[TestCase(1000)]
		public void DeepCopyPerformance(int num)
		{
			var sourceList = new List<ComplexObject>();
			for (int i = 0; i < num; i++)
			{
				sourceList.Add(new ComplexObject
				{
					Num = 123.1234567 + i / 18.0,
					Text = "Вася - дурачок" + i,
					SubObject = new ComplexSubObject { SubNum = i, Comment = "Дуся всем нравится" + 2 * i }
				});
			}
			var sw = Stopwatch.StartNew();
			var resList = sourceList.Select(obj => obj.DeepClone()).ToArray();
			sw.Stop();
			Console.WriteLine(@$"{(sw.Elapsed/num).TotalMilliseconds} ms");
			ClassicAssert.IsTrue(sourceList.SequenceEqual(resList));
		}

		#endregion
	}
}
