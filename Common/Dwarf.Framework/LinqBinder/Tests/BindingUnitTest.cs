using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using MisCore.Framework.Tests;
using NUnit.Framework;
using Assert = NUnit.Framework.Legacy.ClassicAssert;

namespace Dwarf.Framework.LinqBinder.Tests
{
	[TestFixture]
	public class BindingUnitTest
	{
		[TestCase]
		public void Test1()
		{
			var a = new DataA { PropA = 11, B = new DataB { PropB = 8 } };
			var b = new DataB { PropB = 4 };

			var bind = new BindingNode<DataA, int>(a, _a => _a.B.PropB);
			var splice1 = bind.To(b, _b => _b.PropB).ReadTarget();

			Assert.AreEqual(4, a.B.PropB);
			Assert.AreEqual(4, b.PropB);
		}

		[TestCase]
		public void Test2()
		{
			var a = new DataA { PropA = 11, B = new DataB { PropB = 8 } };
			var b = new DataB { PropB = 4, PropC = 3 };

			var bind = new BindingNode<DataA, int>(a, _a => _a.B.PropB);
			var splice1 = bind.To(b, _b => _b.PropC).ReadTarget();

			b.PropC = 11;

			Assert.AreEqual(a.B.PropB, b.PropC);
			Assert.AreEqual(a.B.PropB, 11);
		}

		[TestCase]
		public void Test3()
		{
			var a1 = new DataA { PropA = 11, B = new DataB { PropB = 21 } };
			var a2 = new DataA { PropA = 12, B = new DataB { PropB = 22 } };
			var b = new DataB { PropB = 4, PropC = 3 };

			var bind = new BindingNode<DataA, int>(a1, _a => _a.PropA);
			var splice1 = bind.To(b, _b => _b.PropC).ReadTarget();
			var splice2 = bind.To(a2, _a => _a.PropA).ReadTarget();

			Assert.AreEqual(a1.PropA, 12);
			Assert.AreEqual(a1.PropA, a2.PropA);
			Assert.AreEqual(a1.PropA, b.PropC);

			b.PropC = 18;

			Assert.AreEqual(a1.PropA, 18);
			Assert.AreEqual(a1.PropA, a2.PropA);
			Assert.AreEqual(a1.PropA, b.PropC);
		}

		[TestCase]
		public void TestWithText()
		{
			var a = new DataA { PropA = 11, B = new DataB { PropB = 21 } };
			var t = new TextData { Text = "13" };

			var bind = new BindingNode<DataA, int>(a, _a => _a.PropA);
			var splice = bind.To(t, _t => _t.Text).ReadTarget();

			Assert.AreEqual(13, a.PropA);

			a.PropA = 4;

			Assert.AreEqual("4", t.Text);

			t.Text = "18";

			Assert.AreEqual(18, a.PropA);
		}

		[TestCase]
		public void TestContent()
		{
			var a = new DataA { PropA = 11, B = new DataB { PropB = 21 } };
			var b = new DataB { PropB = 4, PropC = 3 };

			var binder = new BindingContent<DataA>(a);
			binder.Bind(_a => _a.PropA).To(b, _b => _b.PropC).Direction(BindingWay.Manual).ReadSource();

			a.PropA = 16;

			Assert.AreEqual(11, b.PropC);
		}

		[TestCase]
		public void TestContent2()
		{
			var a = new DataA { PropA = 11, B = new DataB { PropB = 21 } };
			var b1 = new DataB { PropB = 4, PropC = 3 };
			var b2 = new DataB { PropB = 8, PropC = 6 };

			var binder = new BindingContent<DataA>(a);
			binder.Bind(_a => _a.PropA).To(b1, _b => _b.PropC).ReadSource();
			binder.Bind(_a => _a.PropA).To(b2, _b => _b.PropC).ReadTarget();

			AssertHelper.AllEqual(6, b1.PropC, b2.PropC, a.PropA);

			b1.PropC = 8;

			AssertHelper.AllEqual(8, b1.PropC, b2.PropC, a.PropA);

			binder.Clear();

			b1.PropC = 11;
			b2.PropC = 12;

			Assert.AreEqual(11, b1.PropC);
			Assert.AreEqual(12, b2.PropC);
			Assert.AreEqual(8, a.PropA);
		}

		[TestCase]
		public void TestContent3()
		{
			var a = new DataA { PropA = 11, B = new DataB { PropB = 21 } };
			var b1 = new DataB { PropB = 4, PropC = 3 };
			var b2 = new DataB { PropB = 8, PropC = 6 };

			var binder = new BindingContent<DataA>(a);
			binder.Bind(_a => _a.PropA).To(b1, _b => _b.PropC);
			binder.Bind(_a => _a.B.PropB).To(b2, _b => _b.PropC);

			binder.ReadSource();

			AssertHelper.AllEqual(11, b1.PropC, a.PropA);
			AssertHelper.AllEqual(21, b2.PropC, a.B.PropB);

			b1.PropC = 8;

			AssertHelper.AllEqual(8, b1.PropC, a.PropA);
			AssertHelper.AllEqual(21, b2.PropC, a.B.PropB);

			binder.Clear();

			b1.PropC = 11;
			b2.PropC = 12;

			Assert.AreEqual(11, b1.PropC);
			Assert.AreEqual(12, b2.PropC);
			Assert.AreEqual(8, a.PropA);
		}

		[TestCase]
		public void TestConverting()
		{
			var a = new DataA { PropA = 11, B = new DataB { PropB = 21 }, Custom = new CustomData(10) };
			var b = new DataB { PropB = 4, PropC = 3 };

			var binder = new BindingContent<DataA>(a);
			binder.Bind(_a => _a.Custom).To(b, _b => _b.PropC)
				.WithConverting(s1 => s1.PropD, s2 => new CustomData(s2)).ReadSource();

			Assert.AreEqual(10, b.PropC);

			b.PropC = 16;

			Assert.AreEqual(16, a.Custom.PropD);
		}

		[TestCase]
		public void TestMultiPath()
		{
			var obj = new DataA { PropA = 11, B = new DataB { PropB = 21, PropC = 12 } };

			var binder = new BindingContent<DataA>(obj);
			binder.Bind(_a => _a.PropA).To(obj, _a => _a.B.PropC).Direction(BindingWay.TwoWay).ReadTarget();

			Assert.AreEqual(12, obj.PropA);
			obj.B.PropC = 16;
			Assert.AreEqual(16, obj.PropA);
		}

		[TestCase]
		public void TestNullableProps()
		{
			var obj = new DataA { PropA = 11 };

			var binder = new BindingContent<DataA>(obj);
			binder.Bind(_a => _a.PropA).To(obj, _a => _a.PropB.PropC).Direction(BindingWay.TwoWay).ReadTarget();

			Assert.AreEqual(0, obj.PropA);
			obj.PropB = new DataB { PropC = 16 };
			Assert.AreEqual(16, obj.PropA);
			obj.PropB.PropC = 18;
			Assert.AreEqual(18, obj.PropA);
			obj.PropB = null;
			Assert.AreEqual(0, obj.PropA);
			obj.PropA = 10;
			Assert.AreEqual(obj.PropB, null);
		}

		[TestCase]
		public void TestNullableProps2()
		{
			var obj = new DataA { PropA = 11 };

			var binder = new BindingContent<DataA>(obj);
			binder.Bind(_a => _a.PropA).To(obj, _a => _a.PropB.PropC).Direction(BindingWay.TwoWay).ReadSource();

			Assert.AreEqual(11, obj.PropA);
			obj.PropB = new DataB { PropC = 16 };
			Assert.AreEqual(16, obj.PropA);
			obj.PropB.PropC = 18;
			Assert.AreEqual(18, obj.PropA);
			obj.PropB = null;
			Assert.AreEqual(0, obj.PropA);
			obj.PropA = 10;
			Assert.AreEqual(obj.PropB, null);
		}

		class DataA
		{
			int mA = 6;
			public int PropA
			{
				get { return mA; }
				set
				{
					mA = value;
					OnPropAChanged();
				}
			}

			DataB propB;
			public event EventHandler PropBChanged;

			public DataB PropB
			{
				get => propB;
				set
				{
					propB = value;
					PropBChanged?.Invoke(this, EventArgs.Empty);
				}
			}

			public DataB B { get; set; }
			public CustomData Custom { get; set; }

			public event EventHandler PropAChanged;

			protected virtual void OnPropAChanged()
			{
				if (PropAChanged != null)
					PropAChanged(this, EventArgs.Empty);
			}
		}

		class DataB
		{
			int c;

			public int PropB { get; set; }

			public int PropC
			{
				get { return c; }
				set
				{
					c = value;
					OnPropCChanged();
				}
			}

			public event EventHandler PropCChanged;

			protected virtual void OnPropCChanged()
			{
				if (PropCChanged != null)
					PropCChanged(this, EventArgs.Empty);
			}
		}

		class TextData : INotifyPropertyChanged
        {
			string text;

			public string Text
			{
				get { return text; }
				set
				{
					text = value;
					OnPropertyChanged("Text");
				}
			}

			#region INotifyPropertyChanged Members

			public event PropertyChangedEventHandler PropertyChanged;

			void OnPropertyChanged(string propName)
			{
				if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propName));
			}

			#endregion
		}

		class CustomData
		{
			public CustomData(int d)
			{
				PropD = d;
			}

			public int PropD { get; set; }
		}
	}
}
