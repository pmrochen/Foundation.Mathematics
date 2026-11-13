using System.Globalization;

namespace Foundation.Mathematics.Tests;

[TestClass]
public sealed class Vector2Tests
{
	[TestMethod]
	public void StaticFields_HaveExpectedValues()
	{
		Assert.AreEqual(new Vector2(0f, 0f), Vector2.Zero);
		Assert.AreEqual(new Vector2(1f, 0f), Vector2.UnitX);
		Assert.AreEqual(new Vector2(0f, 1f), Vector2.UnitY);
		Assert.AreEqual(new Vector2(Single.PositiveInfinity), Vector2.PositiveInfinity);
		Assert.AreEqual(new Vector2(Single.NegativeInfinity), Vector2.NegativeInfinity);
	}

	[TestMethod]
	public void Constructors_And_Properties()
	{
		Vector2 v = new Vector2(3f, 4f);
		Assert.AreEqual(3f, v.X);
		Assert.AreEqual(4f, v.Y);
		Assert.AreEqual(new Vector2(2f, 2f), new Vector2(2f));
		Assert.AreEqual(v, new Vector2(new[] { 3f, 4f }));
		Assert.AreEqual(new Vector2(4f, 3f), v.YX);
		v.X = -1f;
		v.Y = -2f;
		Assert.AreEqual(new Vector2(-1f, -2f), v);
		Assert.AreEqual(new Vector2(1f, -2f), (Vector2)new IntVector2(1, -2));
	}

	[TestMethod]
	public void Indexer_GetsSetsAndThrows()
	{
		Vector2 v = new Vector2(1f, 2f);
		Assert.AreEqual(1f, v[0]);
		Assert.AreEqual(2f, v[1]);
		v[0] = 5f;
		v[1] = 6f;
		Assert.AreEqual(new Vector2(5f, 6f), v);
		Assert.ThrowsExactly<IndexOutOfRangeException>(() => _ = v[2]);
		Assert.ThrowsExactly<IndexOutOfRangeException>(() => v[-1] = 0f);
	}

	[TestMethod]
	public void Magnitude_GetAndSet()
	{
		Vector2 v = new Vector2(3f, 4f);
		Assert.AreEqual(5f, v.Magnitude);
		Assert.AreEqual(25f, v.MagnitudeSquared);
		Assert.AreEqual(5f, v.Length);
		Assert.AreEqual(25f, v.LengthSquared);
		v.Magnitude = 10f;
		AssertEx.AreEqual(new Vector2(6f, 8f), v);
		v.Length = 5f;
		AssertEx.AreEqual(new Vector2(3f, 4f), v);
	}

	[TestMethod]
	public void Equality_And_HashCode()
	{
		Vector2 a = new Vector2(1f, 2f);
		Assert.IsTrue(a == new Vector2(1f, 2f));
		Assert.IsTrue(a != new Vector2(2f, 1f));
		Assert.IsTrue(a.Equals((object)new Vector2(1f, 2f)));
		Assert.IsFalse(a.Equals(null));
		Assert.AreEqual(a.GetHashCode(), new Vector2(1f, 2f).GetHashCode());
		Assert.IsTrue(a.ApproxEquals(new Vector2(1f + 1e-7f, 2f)));
		Assert.IsFalse(a.ApproxEquals(new Vector2(1.1f, 2f)));
		Assert.IsTrue(a.ApproxEquals(new Vector2(1.1f, 2f), 0.2f));
	}

	[TestMethod]
	public void ArithmeticOperators_AreComponentWise()
	{
		Vector2 u = new Vector2(1f, 2f);
		Vector2 v = new Vector2(4f, -8f);
		Assert.AreEqual(u, +u);
		Assert.AreEqual(new Vector2(-1f, -2f), -u);
		Assert.AreEqual(new Vector2(5f, -6f), u + v);
		Assert.AreEqual(new Vector2(-3f, 10f), u - v);
		Assert.AreEqual(new Vector2(4f, -16f), u*v);
		Assert.AreEqual(new Vector2(3f, 6f), u*3f);
		Assert.AreEqual(new Vector2(3f, 6f), 3f*u);
		Assert.AreEqual(new Vector2(0.25f, -0.25f), u/v);
		Assert.AreEqual(new Vector2(0.5f, 1f), u/2f);
		Assert.AreEqual(new Vector2(4f, 2f), 4f/u);
	}

	[TestMethod]
	public void MatrixMultiplication_UsesRowVectorConvention()
	{
		Matrix2 m = new Matrix2(1f, 2f, 3f, 4f);
		Vector2 v = new Vector2(1f, 1f);
		Assert.AreEqual(new Vector2(4f, 6f), v*m);
		Assert.AreEqual(new Vector2(3f, 7f), m*v);
		Assert.AreEqual(new Vector2(4f, 6f), Vector2.Transform(v, m));
		v.Transform(m);
		Assert.AreEqual(new Vector2(4f, 6f), v);
	}

	[TestMethod]
	public void DotCrossDistance()
	{
		Vector2 u = new Vector2(1f, 2f);
		Vector2 v = new Vector2(4f, 6f);
		Assert.AreEqual(16f, Vector2.Dot(u, v));
		Assert.AreEqual(-2f, Vector2.Cross(u, v));
		Assert.AreEqual(1f, Vector2.Cross(Vector2.UnitX, Vector2.UnitY));
		Assert.AreEqual(5f, Vector2.Distance(u, v));
		Assert.AreEqual(25f, Vector2.DistanceSquared(u, v));
	}

	[TestMethod]
	public void MinMaxClampAbsSumLerp()
	{
		Vector2 u = new Vector2(1f, -5f);
		Vector2 v = new Vector2(2f, -6f);
		Assert.AreEqual(new Vector2(1f, -6f), Vector2.Min(u, v));
		Assert.AreEqual(new Vector2(2f, -5f), Vector2.Max(u, v));
		Assert.AreEqual(new Vector2(0f, 1f), Vector2.Clamp(new Vector2(-1f, 5f), Vector2.Zero, new Vector2(1f)));
		Assert.AreEqual(new Vector2(1f, 5f), Vector2.Abs(u));
		Assert.AreEqual(-4f, Vector2.Sum(u));
		Assert.AreEqual(-5f, u.MinComponent);
		Assert.AreEqual(1f, u.MaxComponent);
		Assert.AreEqual(new Vector2(1.5f, -5.5f), Vector2.Lerp(u, v, 0.5f));
	}

	[TestMethod]
	public void Normalize_ProducesUnitVector()
	{
		AssertEx.AreEqual(new Vector2(0.6f, 0.8f), Vector2.Normalize(new Vector2(3f, 4f)));
		Assert.AreEqual(Vector2.Zero, Vector2.Normalize(Vector2.Zero));
		Vector2 v = new Vector2(-3f, 4f);
		v.Normalize();
		AssertEx.AreEqual(new Vector2(-0.6f, 0.8f), v);
	}

	[TestMethod]
	public void ComparisonPredicates_AllAndAny()
	{
		Vector2 a = new Vector2(1f, 2f);
		Vector2 b = new Vector2(2f, 3f);
		Vector2 c = new Vector2(1f, 3f);
		Assert.IsTrue(a.AllLessThan(b));
		Assert.IsFalse(a.AllLessThan(c));
		Assert.IsTrue(a.AllLessThanEqual(c));
		Assert.IsTrue(b.AllGreaterThan(a));
		Assert.IsTrue(c.AllGreaterThanEqual(a));
		Assert.IsTrue(c.AnyLessThan(b));
		Assert.IsFalse(b.AnyLessThan(a));
		Assert.IsTrue(a.AnyLessThanEqual(c));
		Assert.IsTrue(c.AnyGreaterThan(a));
		Assert.IsFalse(a.AnyGreaterThan(c));
		Assert.IsTrue(c.AnyGreaterThanEqual(b));
		Assert.IsFalse(a.AnyGreaterThanEqual(b));
	}

	[TestMethod]
	public void IsFinite_MajorAxis_FromAxis()
	{
		Assert.IsTrue(new Vector2(1f, 2f).IsFinite);
		Assert.IsFalse(new Vector2(Single.NaN, 2f).IsFinite);
		Assert.AreEqual(Axis.X, new Vector2(-3f, 2f).MajorAxis);
		Assert.AreEqual(Axis.Y, new Vector2(1f, -2f).MajorAxis);
		Assert.AreEqual(Vector2.UnitX, Vector2.FromAxis(Axis.X));
		Assert.AreEqual(Vector2.UnitY, Vector2.FromAxis(Axis.Y));
		Assert.AreEqual(Vector2.Zero, Vector2.FromAxis(Axis.Z));
	}

	[TestMethod]
	public void ToArray_ToString_Parse()
	{
		Vector2 v = new Vector2(1.5f, -2f);
		CollectionAssert.AreEqual(new[] { 1.5f, -2f }, v.ToArray());
		Assert.AreEqual("1.5 -2", v.ToString(CultureInfo.InvariantCulture));
		Assert.AreEqual("1.50 -2.00", v.ToString("F2", CultureInfo.InvariantCulture));
		Assert.AreEqual(v, Vector2.Parse("1.5 -2", CultureInfo.InvariantCulture));
		using (CultureScope.Invariant())
		{
			Assert.AreEqual("1.5 -2", v.ToString());
			Assert.AreEqual("1.5 -2.0", v.ToString("0.0"));
			Assert.AreEqual(v, Vector2.Parse("1.5 -2"));
		}
		Assert.ThrowsExactly<ArgumentNullException>(() => Vector2.Parse(null));
		Assert.ThrowsExactly<FormatException>(() => Vector2.Parse("1 2 3", CultureInfo.InvariantCulture));
	}

	[TestMethod]
	public void Angle_ReturnsUnsignedAngle()
	{
		AssertEx.AreEqual(SingleConstants.HalfPi, Vector2.Angle(Vector2.UnitX, Vector2.UnitY));
		AssertEx.AreEqual(SingleConstants.HalfPi, Vector2.Angle(Vector2.UnitX, -Vector2.UnitY));
		AssertEx.AreEqual(SingleConstants.Pi, Vector2.Angle(Vector2.UnitX, -Vector2.UnitX));
		Assert.AreEqual(0f, Vector2.Angle(Vector2.Zero, Vector2.UnitX));
	}

	[TestMethod]
	public void Slerp_InterpolatesOnUnitCircle()
	{
		float h = MathF.Sqrt(0.5f);
		AssertEx.AreEqual(new Vector2(h, h), Vector2.Slerp(Vector2.UnitX, Vector2.UnitY, 0.5f));
		AssertEx.AreEqual(Vector2.UnitX, Vector2.Slerp(Vector2.UnitX, Vector2.UnitY, 0f));
		AssertEx.AreEqual(Vector2.UnitY, Vector2.Slerp(Vector2.UnitX, Vector2.UnitY, 1f));
		AssertEx.AreEqual(Vector2.UnitX, Vector2.Slerp(Vector2.UnitX, Vector2.UnitX, 0.3f));
	}

	[TestMethod]
	public void TruncateLength_ClampsLongVectorsOnly()
	{
		AssertEx.AreEqual(new Vector2(0.6f, 0.8f), Vector2.TruncateLength(new Vector2(3f, 4f), 1f));
		Assert.AreEqual(new Vector2(3f, 4f), Vector2.TruncateLength(new Vector2(3f, 4f), 6f));
		Vector2 v = new Vector2(30f, 40f);
		v.TruncateLength(5f);
		AssertEx.AreEqual(new Vector2(3f, 4f), v);
	}

	[TestMethod]
	public void Rotate_IsCounterClockwise_AndMatchesRotationMatrix()
	{
		AssertEx.AreEqual(Vector2.UnitY, Vector2.Rotate(Vector2.UnitX, SingleConstants.HalfPi));
		AssertEx.AreEqual(-Vector2.UnitX, Vector2.Rotate(Vector2.UnitY, SingleConstants.HalfPi));
		Vector2 v = new Vector2(2f, -1f);
		Assert.AreEqual(v, Vector2.Rotate(v, 0f));
		foreach (float angle in Samples.Angles)
			AssertEx.AreEqual(v*Matrix2.Rotation(angle), Vector2.Rotate(v, angle));
		v.Rotate(SingleConstants.Pi);
		AssertEx.AreEqual(new Vector2(-2f, 1f), v);
	}

	[TestMethod]
	public void Project_Perpendicular_Components()
	{
		AssertEx.AreEqual(new Vector2(3f, 0f), Vector2.Project(new Vector2(3f, 4f), new Vector2(5f, 0f)));
		Assert.AreEqual(Vector2.Zero, Vector2.Project(new Vector2(3f, 4f), Vector2.Zero));
		Assert.AreEqual(new Vector2(-4f, 3f), Vector2.Perpendicular(new Vector2(3f, 4f)));
		Vector2 basis = new Vector2(0.6f, 0.8f);
		Vector2 v = new Vector2(2f, -1f);
		Vector2 par = Vector2.ParallelComponent(v, basis);
		Vector2 perp = Vector2.PerpendicularComponent(v, basis);
		AssertEx.AreEqual(v, par + perp);
		AssertEx.AreEqual(0f, Vector2.Dot(perp, basis));
		AssertEx.AreEqual(0f, Vector2.Cross(par, basis));
	}
}

[TestClass]
public sealed class Vector4Tests
{
	[TestMethod]
	public void StaticFields_HaveExpectedValues()
	{
		Assert.AreEqual(new Vector4(0f, 0f, 0f, 0f), Vector4.Zero);
		Assert.AreEqual(new Vector4(1f, 0f, 0f, 0f), Vector4.UnitX);
		Assert.AreEqual(new Vector4(0f, 1f, 0f, 0f), Vector4.UnitY);
		Assert.AreEqual(new Vector4(0f, 0f, 1f, 0f), Vector4.UnitZ);
		Assert.AreEqual(new Vector4(0f, 0f, 0f, 1f), Vector4.UnitW);
		Assert.AreEqual(new Vector4(Single.PositiveInfinity), Vector4.PositiveInfinity);
		Assert.AreEqual(new Vector4(Single.NegativeInfinity), Vector4.NegativeInfinity);
	}

	[TestMethod]
	public void Constructors_And_Properties()
	{
		Vector4 v = new Vector4(1f, 2f, 3f, 4f);
		Assert.AreEqual(1f, v.X);
		Assert.AreEqual(2f, v.Y);
		Assert.AreEqual(3f, v.Z);
		Assert.AreEqual(4f, v.W);
		Assert.AreEqual(v, new Vector4(new Vector2(1f, 2f), 3f, 4f));
		Assert.AreEqual(v, new Vector4(new Vector2(1f, 2f), new Vector2(3f, 4f)));
		Assert.AreEqual(v, new Vector4(new Vector3(1f, 2f, 3f), 4f));
		Assert.AreEqual(v, new Vector4(new[] { 1f, 2f, 3f, 4f }));
		Assert.AreEqual(new Vector4(7f, 7f, 7f, 7f), new Vector4(7f));
		Assert.AreEqual(new Vector2(1f, 2f), v.XY);
		Assert.AreEqual(new Vector3(1f, 2f, 3f), v.XYZ);

		v.XY = new Vector2(-1f, -2f);
		Assert.AreEqual(new Vector4(-1f, -2f, 3f, 4f), v);
		v.XYZ = new Vector3(5f, 6f, 7f);
		Assert.AreEqual(new Vector4(5f, 6f, 7f, 4f), v);
		v.W = 8f;
		Assert.AreEqual(8f, v.W);
	}

	[TestMethod]
	public void Conversions()
	{
		// Implicit conversions produce homogeneous points (w = 1).
		Vector4 a = new Vector2(1f, 2f);
		Assert.AreEqual(new Vector4(1f, 2f, 0f, 1f), a);
		Vector4 b = new Vector3(1f, 2f, 3f);
		Assert.AreEqual(new Vector4(1f, 2f, 3f, 1f), b);
		Assert.AreEqual(new Vector4(1f, 2f, 3f, 4f), (Vector4)new IntVector4(1, 2, 3, 4));
	}

	[TestMethod]
	public void Indexer_GetsSetsAndThrows()
	{
		Vector4 v = new Vector4(1f, 2f, 3f, 4f);
		for (int i = 0; i < 4; i++)
			Assert.AreEqual(i + 1f, v[i]);
		v[3] = 10f;
		Assert.AreEqual(10f, v.W);
		Assert.ThrowsExactly<IndexOutOfRangeException>(() => _ = v[4]);
		Assert.ThrowsExactly<IndexOutOfRangeException>(() => v[-1] = 0f);
	}

	[TestMethod]
	public void Magnitude_GetAndSet()
	{
		Vector4 v = new Vector4(1f, 2f, 2f, 4f);
		Assert.AreEqual(5f, v.Magnitude);
		Assert.AreEqual(25f, v.MagnitudeSquared);
		Assert.AreEqual(5f, v.Length);
		Assert.AreEqual(25f, v.LengthSquared);
		v.Magnitude = 10f;
		AssertEx.AreEqual(new Vector4(2f, 4f, 4f, 8f), v);
	}

	[TestMethod]
	public void Equality_And_ApproxEquals()
	{
		Vector4 a = new Vector4(1f, 2f, 3f, 4f);
		Assert.IsTrue(a == new Vector4(1f, 2f, 3f, 4f));
		Assert.IsTrue(a != new Vector4(1f, 2f, 3f, 5f));
		Assert.IsTrue(a.Equals((object)new Vector4(1f, 2f, 3f, 4f)));
		Assert.IsFalse(a.Equals((object)new Vector3(1f, 2f, 3f)));
		Assert.AreEqual(a.GetHashCode(), new Vector4(1f, 2f, 3f, 4f).GetHashCode());
		Assert.IsTrue(a.ApproxEquals(new Vector4(1f, 2f, 3f, 4f + 1e-7f)));
		Assert.IsFalse(a.ApproxEquals(new Vector4(1f, 2f, 3f, 4.01f)));
		Assert.IsTrue(a.ApproxEquals(new Vector4(1f, 2f, 3f, 4.01f), 0.1f));
	}

	[TestMethod]
	public void ArithmeticOperators_AreComponentWise()
	{
		Vector4 u = new Vector4(1f, 2f, 3f, 4f);
		Vector4 v = new Vector4(2f, 4f, 6f, 8f);
		Assert.AreEqual(u, +u);
		Assert.AreEqual(new Vector4(-1f, -2f, -3f, -4f), -u);
		Assert.AreEqual(new Vector4(3f, 6f, 9f, 12f), u + v);
		Assert.AreEqual(new Vector4(-1f, -2f, -3f, -4f), u - v);
		Assert.AreEqual(new Vector4(2f, 8f, 18f, 32f), u*v);
		Assert.AreEqual(v, u*2f);
		Assert.AreEqual(v, 2f*u);
		Assert.AreEqual(new Vector4(0.5f), u/v);
		Assert.AreEqual(u, v/2f);
		Assert.AreEqual(new Vector4(12f, 6f, 4f, 3f), 12f/u);
	}

	[TestMethod]
	public void MatrixMultiplication_UsesRowVectorConvention()
	{
		Matrix4 m = Matrix4.Translation(new Vector3(10f, 20f, 30f));
		Vector4 point = new Vector4(1f, 2f, 3f, 1f);
		Assert.AreEqual(new Vector4(11f, 22f, 33f, 1f), point*m);
		Assert.AreEqual(new Vector4(11f, 22f, 33f, 1f), Vector4.Transform(point, m));
		Vector4 direction = new Vector4(1f, 2f, 3f, 0f);
		Assert.AreEqual(direction, direction*m);
		Assert.AreEqual(new Vector4(1f, 2f, 3f, 141f), m*point);
		point.Transform(m);
		Assert.AreEqual(new Vector4(11f, 22f, 33f, 1f), point);
	}

	[TestMethod]
	public void DotTensorDistance()
	{
		Vector4 u = new Vector4(1f, 2f, 3f, 4f);
		Vector4 v = new Vector4(2f, 0f, -1f, 1f);
		Assert.AreEqual(3f, Vector4.Dot(u, v));
		Matrix4 t = Vector4.Tensor(u, v);
		for (int r = 0; r < 4; r++)
			for (int c = 0; c < 4; c++)
				Assert.AreEqual(u[r]*v[c], t[r, c]);
		Assert.AreEqual(MathF.Sqrt(1f + 4f + 16f + 9f), Vector4.Distance(u, v));
		Assert.AreEqual(30f, Vector4.DistanceSquared(u, v));
	}

	[TestMethod]
	public void MinMaxClampAbsSumLerp()
	{
		Vector4 u = new Vector4(1f, -5f, 3f, 0f);
		Vector4 v = new Vector4(2f, -6f, 0f, 1f);
		Assert.AreEqual(new Vector4(1f, -6f, 0f, 0f), Vector4.Min(u, v));
		Assert.AreEqual(new Vector4(2f, -5f, 3f, 1f), Vector4.Max(u, v));
		Assert.AreEqual(new Vector4(1f, 0f, 1f, 0f), Vector4.Clamp(u, Vector4.Zero, new Vector4(1f)));
		Assert.AreEqual(new Vector4(1f, 5f, 3f, 0f), Vector4.Abs(u));
		Assert.AreEqual(-1f, Vector4.Sum(u));
		Assert.AreEqual(-5f, u.MinComponent);
		Assert.AreEqual(3f, u.MaxComponent);
		Assert.AreEqual(new Vector4(1.5f, -5.5f, 1.5f, 0.5f), Vector4.Lerp(u, v, 0.5f));
	}

	[TestMethod]
	public void Normalize_ProducesUnitVector()
	{
		AssertEx.AreEqual(new Vector4(0.2f, 0.4f, 0.4f, 0.8f), Vector4.Normalize(new Vector4(1f, 2f, 2f, 4f)));
		Assert.AreEqual(Vector4.Zero, Vector4.Normalize(Vector4.Zero));
		Vector4 v = new Vector4(0f, 0f, 0f, -3f);
		v.Normalize();
		Assert.AreEqual(-Vector4.UnitW, v);
	}

	[TestMethod]
	public void ComparisonPredicates_AllAndAny()
	{
		Vector4 a = new Vector4(1f, 2f, 3f, 4f);
		Vector4 b = new Vector4(2f, 3f, 4f, 5f);
		Vector4 c = new Vector4(1f, 3f, 2f, 4f);
		Assert.IsTrue(a.AllLessThan(b));
		Assert.IsFalse(a.AllLessThan(c));
		Assert.IsTrue(a.AllLessThanEqual(a));
		Assert.IsTrue(b.AllGreaterThan(a));
		Assert.IsFalse(a.AllGreaterThanEqual(c));
		Assert.IsTrue(a.AnyLessThan(c));
		Assert.IsTrue(a.AnyLessThanEqual(c));
		Assert.IsFalse(b.AnyLessThanEqual(a));
		Assert.IsTrue(a.AnyGreaterThan(c));
		Assert.IsTrue(a.AnyGreaterThanEqual(c));
		Assert.IsFalse(a.AnyGreaterThanEqual(b));
	}

	[TestMethod]
	public void IsFinite_FromAxis_ToArray()
	{
		Assert.IsTrue(new Vector4(1f).IsFinite);
		Assert.IsFalse(new Vector4(1f, 1f, 1f, Single.NaN).IsFinite);
		Assert.AreEqual(Vector4.UnitW, Vector4.FromAxis(Axis.W));
		Assert.AreEqual(Vector4.UnitZ, Vector4.FromAxis(Axis.Z));
		CollectionAssert.AreEqual(new[] { 1f, 2f, 3f, 4f }, new Vector4(1f, 2f, 3f, 4f).ToArray());
	}

	[TestMethod]
	public void ToString_And_Parse_RoundTrip()
	{
		Vector4 v = new Vector4(1.5f, -2f, 0f, 4.25f);
		Assert.AreEqual("1.5 -2 0 4.25", v.ToString(CultureInfo.InvariantCulture));
		Assert.AreEqual("1.50 -2.00 0.00 4.25", v.ToString("0.00", CultureInfo.InvariantCulture));
		Assert.AreEqual(v, Vector4.Parse("1.5 -2 0 4.25", CultureInfo.InvariantCulture));
		using (CultureScope.Invariant())
		{
			Assert.AreEqual("1.5 -2 0 4.25", v.ToString());
			Assert.AreEqual(v, Vector4.Parse(v.ToString()));
			Assert.AreEqual("1.50 -2.00 0.00 4.25", v.ToString("F2"));
		}
		Assert.ThrowsExactly<ArgumentNullException>(() => Vector4.Parse(null));
		Assert.ThrowsExactly<FormatException>(() => Vector4.Parse("1 2 3", CultureInfo.InvariantCulture));
	}

	[TestMethod]
	public void Cross_ReturnsVectorOrthogonalToAllInputs()
	{
		Vector4 u = new Vector4(1f, 2f, 0f, -1f);
		Vector4 v = new Vector4(0f, 1f, 3f, 2f);
		Vector4 w = new Vector4(2f, -1f, 1f, 0f);
		Vector4 c = Vector4.Cross(u, v, w);
		AssertEx.AreEqual(0f, Vector4.Dot(c, u));
		AssertEx.AreEqual(0f, Vector4.Dot(c, v));
		AssertEx.AreEqual(0f, Vector4.Dot(c, w));
		Assert.AreNotEqual(Vector4.Zero, c);
		// Generalized cross product of three basis vectors yields (up to sign) the fourth.
		Vector4 e = Vector4.Cross(Vector4.UnitX, Vector4.UnitY, Vector4.UnitZ);
		AssertEx.AreEqual(1f, Math.Abs(e.W));
		AssertEx.AreEqual(0f, e.XYZ.Magnitude);
	}

	[TestMethod]
	public void Slerp_TruncateLength_Project()
	{
		float h = MathF.Sqrt(0.5f);
		AssertEx.AreEqual(new Vector4(h, 0f, 0f, h), Vector4.Slerp(Vector4.UnitX, Vector4.UnitW, 0.5f));
		AssertEx.AreEqual(Vector4.UnitX, Vector4.Slerp(Vector4.UnitX, Vector4.UnitX, 0.5f));
		AssertEx.AreEqual(new Vector4(0.2f, 0.4f, 0.4f, 0.8f), Vector4.TruncateLength(new Vector4(1f, 2f, 2f, 4f), 1f));
		Assert.AreEqual(new Vector4(1f), Vector4.TruncateLength(new Vector4(1f), 3f));
		Vector4 t = new Vector4(1f, 2f, 2f, 4f);
		t.TruncateLength(2.5f);
		AssertEx.AreEqual(2.5f, t.Magnitude);
		AssertEx.AreEqual(new Vector4(0f, 2f, 0f, 0f), Vector4.Project(new Vector4(1f, 2f, 3f, 4f), new Vector4(0f, 5f, 0f, 0f)));
		Assert.AreEqual(Vector4.Zero, Vector4.Project(new Vector4(1f), Vector4.Zero));
	}
}
