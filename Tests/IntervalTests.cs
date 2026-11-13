using System.Globalization;

namespace Foundation.Mathematics.Tests;

[TestClass]
public sealed class IntervalTests
{
	[TestMethod]
	public void StaticFields_And_Constructors()
	{
		Assert.IsTrue(Interval.Empty.IsEmpty);
		Assert.AreEqual(Single.PositiveInfinity, Interval.Empty.Minimum);
		Assert.AreEqual(Single.NegativeInfinity, Interval.Empty.Maximum);
		Assert.AreEqual(new Interval(0f, 0f), Interval.Zero);
		Interval point = new Interval(3f);
		Assert.AreEqual(3f, point.Minimum);
		Assert.AreEqual(3f, point.Maximum);
		Assert.IsTrue(point.IsPoint);
		Assert.IsFalse(new Interval(1f, 2f).IsPoint);
	}

	[TestMethod]
	public void Properties_GetAndSet()
	{
		Interval i = new Interval(1f, 5f);
		Assert.AreEqual(4f, i.Length);
		Assert.AreEqual(3f, i.Center);
		Assert.IsFalse(i.IsEmpty);
		Assert.IsTrue(i.IsFinite);
		Assert.IsFalse(Interval.Empty.IsFinite);

		i.Minimum = 0f;
		i.Maximum = 10f;
		Assert.AreEqual(new Interval(0f, 10f), i);
		i.Length = 4f;
		Assert.AreEqual(new Interval(3f, 7f), i);
		i.Center = 0f;
		Assert.AreEqual(new Interval(-2f, 2f), i);
	}

	[TestMethod]
	public void Equality_ApproxEquals_HashCode()
	{
		Interval a = new Interval(1f, 2f);
		Assert.IsTrue(a == new Interval(1f, 2f));
		Assert.IsTrue(a != new Interval(1f, 3f));
		Assert.IsTrue(a.Equals((object)new Interval(1f, 2f)));
		Assert.IsFalse(a.Equals((object)new Vector2(1f, 2f)));
		Assert.AreEqual(a.GetHashCode(), new Interval(1f, 2f).GetHashCode());
		Assert.IsTrue(a.ApproxEquals(new Interval(1f, 2f + 1e-7f)));
		Assert.IsFalse(a.ApproxEquals(new Interval(1f, 2.01f)));
		Assert.IsTrue(a.ApproxEquals(new Interval(1f, 2.01f), 0.1f));
	}

	[TestMethod]
	public void UnionAndIntersection()
	{
		Interval a = new Interval(1f, 4f);
		Interval b = new Interval(3f, 6f);
		Assert.AreEqual(new Interval(1f, 6f), Interval.Union(a, b));
		Assert.AreEqual(new Interval(3f, 4f), Interval.Intersection(a, b));
		Assert.IsTrue(Interval.Intersection(a, new Interval(5f, 6f)).IsEmpty);
		Assert.AreEqual(a, Interval.Union(a, Interval.Empty));
	}

	[TestMethod]
	public void ExtendBy_GrowsToInclude()
	{
		Interval i = Interval.Empty;
		i.ExtendBy(2f);
		Assert.AreEqual(new Interval(2f), i);
		i.ExtendBy(-1f);
		i.ExtendBy(0.5f);
		Assert.AreEqual(new Interval(-1f, 2f), i);
		i.ExtendBy(new Interval(1f, 5f));
		Assert.AreEqual(new Interval(-1f, 5f), i);
	}

	[TestMethod]
	public void Translate_Scale_ScaleAroundCenter()
	{
		Interval i = new Interval(1f, 3f);
		Assert.AreEqual(new Interval(3f, 5f), Interval.Translate(i, 2f));
		Assert.AreEqual(new Interval(2f, 6f), Interval.Scale(i, 2f));
		Assert.AreEqual(new Interval(0f, 4f), Interval.ScaleAroundCenter(i, 2f));
		Assert.AreEqual(new Interval(1f, 3f), i);
		i.Translate(-1f);
		Assert.AreEqual(new Interval(0f, 2f), i);
		i.Scale(3f);
		Assert.AreEqual(new Interval(0f, 6f), i);
		i.ScaleAroundCenter(0.5f);
		Assert.AreEqual(new Interval(1.5f, 4.5f), i);
	}

	[TestMethod]
	public void Contains_And_Intersects()
	{
		Interval i = new Interval(1f, 3f);
		Assert.IsTrue(i.Contains(1f));
		Assert.IsTrue(i.Contains(3f));
		Assert.IsTrue(i.Contains(2f));
		Assert.IsFalse(i.Contains(3.1f));
		Assert.IsTrue(i.Contains(new Interval(1.5f, 3f)));
		Assert.IsFalse(i.Contains(new Interval(0f, 2f)));
		Assert.IsTrue(i.Intersects(new Interval(3f, 4f)));
		Assert.IsTrue(i.Intersects(new Interval(0f, 5f)));
		Assert.IsFalse(i.Intersects(new Interval(3.5f, 4f)));
		Assert.IsFalse(i.Intersects(new Interval(-2f, 0.5f)));
	}

	[TestMethod]
	public void FindIntersection_ReturnsOverlapOrNull()
	{
		Interval i = new Interval(1f, 3f);
		Assert.AreEqual(new Interval(2f, 3f), i.FindIntersection(new Interval(2f, 5f)));
		Assert.AreEqual(new Interval(1f, 2f), i.FindIntersection(new Interval(0f, 2f)));
		Assert.AreEqual(new Interval(1.5f, 2.5f), i.FindIntersection(new Interval(1.5f, 2.5f)));
		Assert.AreEqual(i, i.FindIntersection(new Interval(-5f, 5f)));
		Assert.AreEqual(new Interval(3f), i.FindIntersection(new Interval(3f, 4f)));
		Assert.AreEqual(new Interval(1f), i.FindIntersection(new Interval(0f, 1f)));
		Assert.IsNull(i.FindIntersection(new Interval(3.5f, 4f)));
		Assert.IsNull(i.FindIntersection(new Interval(-1f, 0.5f)));
	}

	[TestMethod]
	public void ToString_And_Parse()
	{
		Interval i = new Interval(-1.5f, 2.25f);
		Assert.AreEqual("-1.5 2.25", i.ToString(CultureInfo.InvariantCulture));
		Assert.AreEqual("-1.50 2.25", i.ToString("F2", CultureInfo.InvariantCulture));
		Assert.AreEqual(i, Interval.Parse("-1.5 2.25", CultureInfo.InvariantCulture));
		Assert.AreEqual(Interval.Empty, Interval.Parse(Interval.Empty.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture));
		using (CultureScope.Invariant())
		{
			Assert.AreEqual("-1.5 2.25", i.ToString());
			Assert.AreEqual("-1.50 2.25", i.ToString("F2"));
			Assert.AreEqual(i, Interval.Parse("-1.5 2.25"));
			Assert.AreEqual(Interval.Empty, Interval.Parse(Interval.Empty.ToString()));
		}
		Assert.ThrowsExactly<ArgumentNullException>(() => Interval.Parse(null));
		Assert.ThrowsExactly<FormatException>(() => Interval.Parse("1", CultureInfo.InvariantCulture));
	}
}
