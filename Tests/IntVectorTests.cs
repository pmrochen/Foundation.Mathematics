using System.Globalization;

namespace Foundation.Mathematics.Tests;

[TestClass]
public sealed class IntVector2Tests
{
	[TestMethod]
	public void Constructors_Properties_Indexer()
	{
		IntVector2 v = new IntVector2(3, -4);
		Assert.AreEqual(3, v.X);
		Assert.AreEqual(-4, v.Y);
		Assert.AreEqual(new IntVector2(5, 5), new IntVector2(5));
		Assert.AreEqual(v, new IntVector2(new[] { 3, -4 }));
		Assert.AreEqual(new IntVector2(0, 0), IntVector2.Zero);
		v.X = 1;
		v[1] = 2;
		Assert.AreEqual(1, v[0]);
		Assert.AreEqual(2, v.Y);
		Assert.ThrowsExactly<IndexOutOfRangeException>(() => _ = v[2]);
		Assert.ThrowsExactly<IndexOutOfRangeException>(() => v[-1] = 0);
	}

	[TestMethod]
	public void Equality_HashCode()
	{
		IntVector2 a = new IntVector2(1, 2);
		Assert.IsTrue(a == new IntVector2(1, 2));
		Assert.IsTrue(a != new IntVector2(2, 1));
		Assert.IsTrue(a.Equals(new IntVector2(1, 2)));
		Assert.IsTrue(a.Equals((object)new IntVector2(1, 2)));
		Assert.IsFalse(a.Equals((object)new Vector2(1f, 2f)));
		Assert.AreEqual(a.GetHashCode(), new IntVector2(1, 2).GetHashCode());
	}

	[TestMethod]
	public void Arithmetic_IsComponentWise()
	{
		IntVector2 u = new IntVector2(6, -8);
		IntVector2 v = new IntVector2(2, 4);
		Assert.AreEqual(u, +u);
		Assert.AreEqual(new IntVector2(-6, 8), -u);
		Assert.AreEqual(new IntVector2(8, -4), u + v);
		Assert.AreEqual(new IntVector2(4, -12), u - v);
		Assert.AreEqual(new IntVector2(12, -32), u*v);
		Assert.AreEqual(new IntVector2(3, -2), u/v);
		Assert.AreEqual(new IntVector2(12, -16), u*2);
		Assert.AreEqual(new IntVector2(12, -16), 2*u);
		Assert.AreEqual(new IntVector2(3, -4), u/2);
		Assert.AreEqual(new IntVector2(4, 2), 8/v);
		Assert.ThrowsExactly<DivideByZeroException>(() => u/IntVector2.Zero);
	}

	[TestMethod]
	public void MinMaxClampAbsSum()
	{
		IntVector2 u = new IntVector2(1, -5);
		IntVector2 v = new IntVector2(2, -6);
		Assert.AreEqual(new IntVector2(1, -6), IntVector2.Min(u, v));
		Assert.AreEqual(new IntVector2(2, -5), IntVector2.Max(u, v));
		Assert.AreEqual(new IntVector2(1, 0), IntVector2.Clamp(new IntVector2(5, -5), IntVector2.Zero, new IntVector2(1)));
		Assert.AreEqual(new IntVector2(1, 5), IntVector2.Abs(u));
		Assert.AreEqual(-4, IntVector2.Sum(u));
		Assert.AreEqual(-5, u.MinComponent);
		Assert.AreEqual(1, u.MaxComponent);
	}

	[TestMethod]
	public void ComparisonPredicates()
	{
		IntVector2 a = new IntVector2(1, 2);
		IntVector2 b = new IntVector2(2, 3);
		IntVector2 c = new IntVector2(1, 3);
		Assert.IsTrue(a.AllLessThan(b));
		Assert.IsFalse(a.AllLessThan(c));
		Assert.IsTrue(a.AllLessThanEqual(c));
		Assert.IsTrue(b.AllGreaterThan(a));
		Assert.IsTrue(c.AllGreaterThanEqual(a));
		Assert.IsTrue(a.AnyLessThan(c));
		Assert.IsTrue(a.AnyLessThanEqual(c));
		Assert.IsFalse(b.AnyLessThanEqual(a));
		Assert.IsTrue(c.AnyGreaterThan(a));
		Assert.IsFalse(a.AnyGreaterThan(c));
		Assert.IsTrue(c.AnyGreaterThanEqual(b));
		Assert.IsTrue(a.AnyGreaterThanEqual(c));
		Assert.IsFalse(a.AnyGreaterThanEqual(b));
	}

	[TestMethod]
	public void FloatConversions_TruncateOrRound()
	{
		Assert.AreEqual(new IntVector2(1, -1), (IntVector2)new Vector2(1.7f, -1.7f));
		Assert.AreEqual(new IntVector2(2, -2), IntVector2.Round(new Vector2(1.7f, -1.7f)));
		Assert.AreEqual(new IntVector2(1, -1), IntVector2.Round(new Vector2(1.2f, -1.2f)));
	}

	[TestMethod]
	public void ToArray_ToString_Parse()
	{
		IntVector2 v = new IntVector2(12, -3);
		CollectionAssert.AreEqual(new[] { 12, -3 }, v.ToArray());
		Assert.AreEqual("12 -3", v.ToString(CultureInfo.InvariantCulture));
		Assert.AreEqual("012 -003", v.ToString("000", CultureInfo.InvariantCulture));
		Assert.AreEqual(v, IntVector2.Parse(" 12  -3 ", CultureInfo.InvariantCulture));
		using (CultureScope.Invariant())
		{
			Assert.AreEqual("12 -3", v.ToString());
			Assert.AreEqual("012 -003", v.ToString("000"));
			Assert.AreEqual(v, IntVector2.Parse("12 -3"));
		}
		Assert.ThrowsExactly<ArgumentNullException>(() => IntVector2.Parse(null));
		Assert.ThrowsExactly<FormatException>(() => IntVector2.Parse("1", CultureInfo.InvariantCulture));
		Assert.ThrowsExactly<FormatException>(() => IntVector2.Parse("1 2.5", CultureInfo.InvariantCulture));
	}
}

[TestClass]
public sealed class IntVector3Tests
{
	[TestMethod]
	public void Constructors_Properties_Indexer()
	{
		IntVector3 v = new IntVector3(1, 2, 3);
		Assert.AreEqual(1, v.X);
		Assert.AreEqual(2, v.Y);
		Assert.AreEqual(3, v.Z);
		Assert.AreEqual(new IntVector3(4, 4, 4), new IntVector3(4));
		Assert.AreEqual(v, new IntVector3(new IntVector2(1, 2), 3));
		Assert.AreEqual(v, new IntVector3(new[] { 1, 2, 3 }));
		Assert.AreEqual(new IntVector3(0, 0, 0), IntVector3.Zero);
		Assert.AreEqual(new IntVector2(1, 2), v.XY);
		Assert.AreEqual(new IntVector2(1, 3), v.XZ);
		Assert.AreEqual(new IntVector2(3, 2), v.ZY);
		v.XY = new IntVector2(7, 8);
		Assert.AreEqual(new IntVector3(7, 8, 3), v);
		v[2] = 9;
		Assert.AreEqual(9, v.Z);
		Assert.ThrowsExactly<IndexOutOfRangeException>(() => _ = v[3]);
	}

	[TestMethod]
	public void Equality_Arithmetic()
	{
		IntVector3 u = new IntVector3(6, -8, 10);
		IntVector3 v = new IntVector3(2, 4, 5);
		Assert.IsTrue(u == new IntVector3(6, -8, 10));
		Assert.IsTrue(u != v);
		Assert.IsTrue(u.Equals((object)new IntVector3(6, -8, 10)));
		Assert.AreEqual(u.GetHashCode(), new IntVector3(6, -8, 10).GetHashCode());
		Assert.AreEqual(new IntVector3(-6, 8, -10), -u);
		Assert.AreEqual(u, +u);
		Assert.AreEqual(new IntVector3(8, -4, 15), u + v);
		Assert.AreEqual(new IntVector3(4, -12, 5), u - v);
		Assert.AreEqual(new IntVector3(12, -32, 50), u*v);
		Assert.AreEqual(new IntVector3(3, -2, 2), u/v);
		Assert.AreEqual(new IntVector3(12, -16, 20), 2*u);
		Assert.AreEqual(new IntVector3(12, -16, 20), u*2);
		Assert.AreEqual(new IntVector3(3, -4, 5), u/2);
		Assert.AreEqual(new IntVector3(10, 5, 4), 20/v);
	}

	[TestMethod]
	public void MinMaxClampAbsSum()
	{
		IntVector3 u = new IntVector3(1, -5, 3);
		IntVector3 v = new IntVector3(2, -6, 0);
		Assert.AreEqual(new IntVector3(1, -6, 0), IntVector3.Min(u, v));
		Assert.AreEqual(new IntVector3(2, -5, 3), IntVector3.Max(u, v));
		Assert.AreEqual(new IntVector3(1, 0, 1), IntVector3.Clamp(u, IntVector3.Zero, new IntVector3(1)));
		Assert.AreEqual(new IntVector3(1, 5, 3), IntVector3.Abs(u));
		Assert.AreEqual(-1, IntVector3.Sum(u));
		Assert.AreEqual(-5, u.MinComponent);
		Assert.AreEqual(3, u.MaxComponent);
	}

	[TestMethod]
	public void ComparisonPredicates()
	{
		IntVector3 a = new IntVector3(1, 2, 3);
		IntVector3 b = new IntVector3(2, 3, 4);
		IntVector3 c = new IntVector3(1, 3, 2);
		Assert.IsTrue(a.AllLessThan(b));
		Assert.IsFalse(a.AllLessThan(c));
		Assert.IsTrue(a.AllLessThanEqual(a));
		Assert.IsTrue(b.AllGreaterThan(a));
		Assert.IsFalse(a.AllGreaterThanEqual(c));
		Assert.IsTrue(a.AnyLessThan(c));
		Assert.IsTrue(a.AnyLessThanEqual(c));
		Assert.IsTrue(a.AnyGreaterThan(c));
		Assert.IsTrue(a.AnyGreaterThanEqual(c));
		Assert.IsFalse(a.AnyGreaterThanEqual(b));
	}

	[TestMethod]
	public void Conversions_ToArray_ToString_Parse()
	{
		Assert.AreEqual(new IntVector3(1, -1, 0), (IntVector3)new Vector3(1.7f, -1.7f, 0.4f));
		Assert.AreEqual(new IntVector3(2, -2, 0), IntVector3.Round(new Vector3(1.7f, -1.7f, 0.4f)));
		IntVector3 v = new IntVector3(1, -2, 3);
		CollectionAssert.AreEqual(new[] { 1, -2, 3 }, v.ToArray());
		Assert.AreEqual("1 -2 3", v.ToString(CultureInfo.InvariantCulture));
		Assert.AreEqual("01 -02 03", v.ToString("00", CultureInfo.InvariantCulture));
		Assert.AreEqual(v, IntVector3.Parse("1 -2 3", CultureInfo.InvariantCulture));
		using (CultureScope.Invariant())
		{
			Assert.AreEqual("1 -2 3", v.ToString());
			Assert.AreEqual(v, IntVector3.Parse("1 -2 3"));
		}
		Assert.ThrowsExactly<ArgumentNullException>(() => IntVector3.Parse(null));
		Assert.ThrowsExactly<FormatException>(() => IntVector3.Parse("1 2", CultureInfo.InvariantCulture));
	}
}

[TestClass]
public sealed class IntVector4Tests
{
	[TestMethod]
	public void Constructors_Properties_Indexer()
	{
		IntVector4 v = new IntVector4(1, 2, 3, 4);
		Assert.AreEqual(1, v.X);
		Assert.AreEqual(2, v.Y);
		Assert.AreEqual(3, v.Z);
		Assert.AreEqual(4, v.W);
		Assert.AreEqual(new IntVector4(9, 9, 9, 9), new IntVector4(9));
		Assert.AreEqual(v, new IntVector4(new IntVector2(1, 2), 3, 4));
		Assert.AreEqual(v, new IntVector4(new IntVector2(1, 2), new IntVector2(3, 4)));
		Assert.AreEqual(v, new IntVector4(new IntVector3(1, 2, 3), 4));
		Assert.AreEqual(v, new IntVector4(new[] { 1, 2, 3, 4 }));
		Assert.AreEqual(new IntVector4(0, 0, 0, 0), IntVector4.Zero);
		Assert.AreEqual(new IntVector2(1, 2), v.XY);
		Assert.AreEqual(new IntVector3(1, 2, 3), v.XYZ);
		v.XYZ = new IntVector3(-1, -2, -3);
		Assert.AreEqual(new IntVector4(-1, -2, -3, 4), v);
		v.XY = new IntVector2(5, 6);
		Assert.AreEqual(new IntVector4(5, 6, -3, 4), v);
		v[3] = 0;
		Assert.AreEqual(0, v.W);
		Assert.ThrowsExactly<IndexOutOfRangeException>(() => _ = v[4]);
	}

	[TestMethod]
	public void Equality_Arithmetic()
	{
		IntVector4 u = new IntVector4(6, -8, 10, 12);
		IntVector4 v = new IntVector4(2, 4, 5, 3);
		Assert.IsTrue(u == new IntVector4(6, -8, 10, 12));
		Assert.IsTrue(u != v);
		Assert.IsTrue(u.Equals((object)new IntVector4(6, -8, 10, 12)));
		Assert.AreEqual(u.GetHashCode(), new IntVector4(6, -8, 10, 12).GetHashCode());
		Assert.AreEqual(u, +u);
		Assert.AreEqual(new IntVector4(-6, 8, -10, -12), -u);
		Assert.AreEqual(new IntVector4(8, -4, 15, 15), u + v);
		Assert.AreEqual(new IntVector4(4, -12, 5, 9), u - v);
		Assert.AreEqual(new IntVector4(12, -32, 50, 36), u*v);
		Assert.AreEqual(new IntVector4(3, -2, 2, 4), u/v);
		Assert.AreEqual(new IntVector4(12, -16, 20, 24), u*2);
		Assert.AreEqual(new IntVector4(12, -16, 20, 24), 2*u);
		Assert.AreEqual(new IntVector4(3, -4, 5, 6), u/2);
		Assert.AreEqual(new IntVector4(30, 15, 12, 20), 60/v);
	}

	[TestMethod]
	public void MinMaxClampAbsSum()
	{
		IntVector4 u = new IntVector4(1, -5, 3, 0);
		IntVector4 v = new IntVector4(2, -6, 0, 1);
		Assert.AreEqual(new IntVector4(1, -6, 0, 0), IntVector4.Min(u, v));
		Assert.AreEqual(new IntVector4(2, -5, 3, 1), IntVector4.Max(u, v));
		Assert.AreEqual(new IntVector4(1, 0, 1, 0), IntVector4.Clamp(u, IntVector4.Zero, new IntVector4(1)));
		Assert.AreEqual(new IntVector4(1, 5, 3, 0), IntVector4.Abs(u));
		Assert.AreEqual(-1, IntVector4.Sum(u));
		Assert.AreEqual(-5, u.MinComponent);
		Assert.AreEqual(3, u.MaxComponent);
	}

	[TestMethod]
	public void ComparisonPredicates()
	{
		IntVector4 a = new IntVector4(1, 2, 3, 4);
		IntVector4 b = new IntVector4(2, 3, 4, 5);
		IntVector4 c = new IntVector4(1, 3, 2, 4);
		Assert.IsTrue(a.AllLessThan(b));
		Assert.IsFalse(a.AllLessThan(c));
		Assert.IsTrue(a.AllLessThanEqual(a));
		Assert.IsTrue(b.AllGreaterThan(a));
		Assert.IsFalse(a.AllGreaterThanEqual(c));
		Assert.IsTrue(a.AnyLessThan(c));
		Assert.IsTrue(a.AnyLessThanEqual(c));
		Assert.IsTrue(a.AnyGreaterThan(c));
		Assert.IsTrue(a.AnyGreaterThanEqual(c));
		Assert.IsFalse(a.AnyGreaterThanEqual(b));
	}

	[TestMethod]
	public void Conversions_ToArray_ToString_Parse()
	{
		Assert.AreEqual(new IntVector4(1, -1, 0, 2), (IntVector4)new Vector4(1.7f, -1.7f, 0.4f, 2f));
		Assert.AreEqual(new IntVector4(2, -2, 0, 2), IntVector4.Round(new Vector4(1.7f, -1.7f, 0.4f, 2f)));
		IntVector4 v = new IntVector4(1, -2, 3, 4);
		CollectionAssert.AreEqual(new[] { 1, -2, 3, 4 }, v.ToArray());
		Assert.AreEqual("1 -2 3 4", v.ToString(CultureInfo.InvariantCulture));
		Assert.AreEqual("01 -02 03 04", v.ToString("00", CultureInfo.InvariantCulture));
		Assert.AreEqual(v, IntVector4.Parse("1 -2 3 4", CultureInfo.InvariantCulture));
		using (CultureScope.Invariant())
		{
			Assert.AreEqual("1 -2 3 4", v.ToString());
			Assert.AreEqual(v, IntVector4.Parse("1 -2 3 4"));
		}
		Assert.ThrowsExactly<ArgumentNullException>(() => IntVector4.Parse(null));
		Assert.ThrowsExactly<FormatException>(() => IntVector4.Parse("1 2 3", CultureInfo.InvariantCulture));
	}
}

/// <summary>
/// Checks for overloads whose parameter type does not match the declaring type.
/// </summary>
[TestClass]
public sealed class ApiSignatureTests
{
	private static void AssertParameterType(Type declaringType, string methodName, Type expectedParameterType)
	{
		var overloads = declaringType.GetMethods().Where(m => (m.Name == methodName) && !m.IsStatic).ToArray();
		Assert.IsTrue(overloads.Length > 0, $"{declaringType.Name}.{methodName} not found.");
		foreach (var method in overloads)
		{
			var parameterType = method.GetParameters()[0].ParameterType;
			if (parameterType.IsByRef)
				parameterType = parameterType.GetElementType();
			Assert.AreEqual(expectedParameterType, parameterType,
				$"{declaringType.Name}.{methodName} takes {parameterType.Name} instead of {expectedParameterType.Name}.");
		}
	}

	[TestMethod]
	public void IntVector2_AnyGreaterThanEqual_TakesIntVector2()
	{
		AssertParameterType(typeof(IntVector2), nameof(IntVector2.AnyGreaterThanEqual), typeof(IntVector2));
	}

	[TestMethod]
	public void Vector2_AnyGreaterThanEqual_TakesVector2()
	{
		AssertParameterType(typeof(Vector2), nameof(Vector2.AnyGreaterThanEqual), typeof(Vector2));
	}

	[TestMethod]
	public void AxisAlignedBox_ApproxEquals_TakesAxisAlignedBox()
	{
		AssertParameterType(typeof(AxisAlignedBox), nameof(AxisAlignedBox.ApproxEquals), typeof(AxisAlignedBox));
	}

	[TestMethod]
	public void ComparisonPredicates_TakeDeclaringType()
	{
		string[] names = { "AllLessThan", "AllLessThanEqual", "AllGreaterThan", "AllGreaterThanEqual",
			"AnyLessThan", "AnyLessThanEqual", "AnyGreaterThan", "AnyGreaterThanEqual" };
		foreach (Type type in new[] { typeof(Vector3), typeof(Vector4), typeof(IntVector3), typeof(IntVector4) })
			foreach (string name in names)
				AssertParameterType(type, name, type);
	}
}
