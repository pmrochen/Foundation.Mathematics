using System.Globalization;

namespace Foundation.Mathematics.Tests;

[TestClass]
public sealed class Vector3Tests
{
	[TestMethod]
	public void StaticFields_HaveExpectedValues()
	{
		Assert.AreEqual(new Vector3(0f, 0f, 0f), Vector3.Zero);
		Assert.AreEqual(new Vector3(1f, 0f, 0f), Vector3.UnitX);
		Assert.AreEqual(new Vector3(0f, 1f, 0f), Vector3.UnitY);
		Assert.AreEqual(new Vector3(0f, 0f, 1f), Vector3.UnitZ);
		Assert.AreEqual(new Vector3(Single.PositiveInfinity), Vector3.PositiveInfinity);
		Assert.AreEqual(new Vector3(Single.NegativeInfinity), Vector3.NegativeInfinity);
	}

	[TestMethod]
	public void Constructors_InitializeComponents()
	{
		Vector3 a = new Vector3(2f);
		Assert.AreEqual(2f, a.X);
		Assert.AreEqual(2f, a.Y);
		Assert.AreEqual(2f, a.Z);

		Vector3 b = new Vector3(1f, 2f, 3f);
		Assert.AreEqual(1f, b.X);
		Assert.AreEqual(2f, b.Y);
		Assert.AreEqual(3f, b.Z);

		Assert.AreEqual(b, new Vector3(new Vector2(1f, 2f), 3f));
		Assert.AreEqual(b, new Vector3(new[] { 1f, 2f, 3f }));
	}

	[TestMethod]
	public void ImplicitConversionFromVector2_SetsZToZero()
	{
		Vector3 v = new Vector2(4f, 5f);
		Assert.AreEqual(new Vector3(4f, 5f, 0f), v);
	}

	[TestMethod]
	public void ExplicitConversionFromIntVector3_ConvertsComponents()
	{
		Assert.AreEqual(new Vector3(1f, -2f, 3f), (Vector3)new IntVector3(1, -2, 3));
	}

	[TestMethod]
	public void Properties_GetAndSetComponents()
	{
		Vector3 v = Vector3.Zero;
		v.X = 1f;
		v.Y = 2f;
		v.Z = 3f;
		Assert.AreEqual(new Vector3(1f, 2f, 3f), v);
		Assert.AreEqual(new Vector2(1f, 2f), v.XY);
		Assert.AreEqual(new Vector2(1f, 3f), v.XZ);
		Assert.AreEqual(new Vector2(3f, 2f), v.ZY);
		Assert.AreEqual(new Vector3(2f, 3f, 1f), v.YZX);
		Assert.AreEqual(new Vector3(3f, 1f, 2f), v.ZXY);

		v.XY = new Vector2(7f, 8f);
		Assert.AreEqual(new Vector3(7f, 8f, 3f), v);
	}

	[TestMethod]
	public void Indexer_GetsAndSetsComponents()
	{
		Vector3 v = new Vector3(1f, 2f, 3f);
		Assert.AreEqual(1f, v[0]);
		Assert.AreEqual(2f, v[1]);
		Assert.AreEqual(3f, v[2]);
		v[0] = 4f;
		v[1] = 5f;
		v[2] = 6f;
		Assert.AreEqual(new Vector3(4f, 5f, 6f), v);
	}

	[TestMethod]
	public void Indexer_OutOfRange_Throws()
	{
		Vector3 v = new Vector3(1f, 2f, 3f);
		Assert.ThrowsExactly<IndexOutOfRangeException>(() => _ = v[3]);
		Assert.ThrowsExactly<IndexOutOfRangeException>(() => _ = v[-1]);
		Assert.ThrowsExactly<IndexOutOfRangeException>(() => v[3] = 0f);
	}

	[TestMethod]
	public void MagnitudeAndLength_ReturnEuclideanNorm()
	{
		Vector3 v = new Vector3(2f, 3f, 6f);
		Assert.AreEqual(7f, v.Magnitude);
		Assert.AreEqual(49f, v.MagnitudeSquared);
		Assert.AreEqual(7f, v.Length);
		Assert.AreEqual(49f, v.LengthSquared);
	}

	[TestMethod]
	public void MagnitudeSetter_RescalesVector()
	{
		Vector3 v = new Vector3(2f, 3f, 6f);
		v.Magnitude = 14f;
		AssertEx.AreEqual(new Vector3(4f, 6f, 12f), v);
		v.Length = 7f;
		AssertEx.AreEqual(new Vector3(2f, 3f, 6f), v);

		Vector3 zero = Vector3.Zero;
		zero.Magnitude = 5f;
		Assert.AreEqual(Vector3.Zero, zero);
	}

	[TestMethod]
	public void Equality_ComparesAllComponents()
	{
		Vector3 a = new Vector3(1f, 2f, 3f);
		Vector3 b = new Vector3(1f, 2f, 3f);
		Vector3 c = new Vector3(1f, 2f, 4f);
		Assert.IsTrue(a == b);
		Assert.IsFalse(a != b);
		Assert.IsTrue(a != c);
		Assert.IsTrue(a.Equals(b));
		Assert.IsTrue(a.Equals((object)b));
		Assert.IsFalse(a.Equals((object)c));
		Assert.IsFalse(a.Equals("not a vector"));
		Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
	}

	[TestMethod]
	public void ApproxEquals_UsesTolerance()
	{
		Vector3 a = new Vector3(1f, 2f, 3f);
		Assert.IsTrue(a.ApproxEquals(new Vector3(1f, 2f, 3f + 5e-7f)));
		Assert.IsFalse(a.ApproxEquals(new Vector3(1f, 2f, 3.001f)));
		Assert.IsTrue(a.ApproxEquals(new Vector3(1.05f, 2f, 3f), 0.1f));
	}

	[TestMethod]
	public void ArithmeticOperators_AreComponentWise()
	{
		Vector3 u = new Vector3(1f, 2f, 3f);
		Vector3 v = new Vector3(4f, -5f, 6f);
		Assert.AreEqual(u, +u);
		Assert.AreEqual(new Vector3(-1f, -2f, -3f), -u);
		Assert.AreEqual(new Vector3(5f, -3f, 9f), u + v);
		Assert.AreEqual(new Vector3(-3f, 7f, -3f), u - v);
		Assert.AreEqual(new Vector3(4f, -10f, 18f), u*v);
		Assert.AreEqual(new Vector3(2f, 4f, 6f), u*2f);
		Assert.AreEqual(new Vector3(2f, 4f, 6f), 2f*u);
		Assert.AreEqual(new Vector3(0.25f, -0.4f, 0.5f), u/v);
		Assert.AreEqual(new Vector3(0.5f, 1f, 1.5f), u/2f);
		Assert.AreEqual(new Vector3(6f, 3f, 2f), 6f/u);
	}

	[TestMethod]
	public void Dot_And_Cross_FollowRightHandRule()
	{
		Vector3 u = new Vector3(1f, 2f, 3f);
		Vector3 v = new Vector3(4f, 5f, 6f);
		Assert.AreEqual(32f, Vector3.Dot(u, v));
		Assert.AreEqual(new Vector3(-3f, 6f, -3f), Vector3.Cross(u, v));
		Assert.AreEqual(Vector3.UnitZ, Vector3.Cross(Vector3.UnitX, Vector3.UnitY));
		Assert.AreEqual(Vector3.UnitX, Vector3.Cross(Vector3.UnitY, Vector3.UnitZ));
		Assert.AreEqual(Vector3.UnitY, Vector3.Cross(Vector3.UnitZ, Vector3.UnitX));
		Assert.AreEqual(0f, Vector3.Dot(Vector3.Cross(u, v), u));
	}

	[TestMethod]
	public void Tensor_ReturnsOuterProduct()
	{
		Matrix3 m = Vector3.Tensor(new Vector3(1f, 2f, 3f), new Vector3(4f, 5f, 6f));
		Assert.AreEqual(new Matrix3(4f, 5f, 6f, 8f, 10f, 12f, 12f, 15f, 18f), m);
	}

	[TestMethod]
	public void DistanceFunctions_ReturnEuclideanDistance()
	{
		Vector3 u = new Vector3(1f, 2f, 3f);
		Vector3 v = new Vector3(3f, 5f, 9f);
		Assert.AreEqual(7f, Vector3.Distance(u, v));
		Assert.AreEqual(49f, Vector3.DistanceSquared(u, v));
	}

	[TestMethod]
	public void MinMaxClampAbs_AreComponentWise()
	{
		Vector3 u = new Vector3(1f, -5f, 3f);
		Vector3 v = new Vector3(2f, -6f, 0f);
		Assert.AreEqual(new Vector3(1f, -6f, 0f), Vector3.Min(u, v));
		Assert.AreEqual(new Vector3(2f, -5f, 3f), Vector3.Max(u, v));
		Assert.AreEqual(new Vector3(1f, 0f, 2f), Vector3.Clamp(new Vector3(-1f, 0f, 5f), new Vector3(1f, -1f, 0f), new Vector3(2f, 1f, 2f)));
		Assert.AreEqual(new Vector3(1f, 5f, 3f), Vector3.Abs(u));
		Assert.AreEqual(-5f, u.MinComponent);
		Assert.AreEqual(3f, u.MaxComponent);
		Assert.AreEqual(-1f, Vector3.Sum(u));
	}

	[TestMethod]
	public void Lerp_InterpolatesLinearly()
	{
		Vector3 u = new Vector3(0f, 2f, 4f);
		Vector3 v = new Vector3(2f, 4f, 8f);
		Assert.AreEqual(u, Vector3.Lerp(u, v, 0f));
		Assert.AreEqual(v, Vector3.Lerp(u, v, 1f));
		Assert.AreEqual(new Vector3(1f, 3f, 6f), Vector3.Lerp(u, v, 0.5f));
	}

	[TestMethod]
	public void Normalize_ProducesUnitVector_AndLeavesZeroUnchanged()
	{
		Vector3 v = new Vector3(2f, 3f, 6f);
		AssertEx.AreEqual(new Vector3(2f/7f, 3f/7f, 6f/7f), Vector3.Normalize(v));
		v.Normalize();
		AssertEx.AreEqual(1f, v.Magnitude);
		Assert.AreEqual(Vector3.Zero, Vector3.Normalize(Vector3.Zero));
		Vector3 zero = Vector3.Zero;
		zero.Normalize();
		Assert.AreEqual(Vector3.Zero, zero);
	}

	[TestMethod]
	public void ComparisonPredicates_AllAndAny()
	{
		Vector3 a = new Vector3(1f, 2f, 3f);
		Vector3 b = new Vector3(2f, 3f, 4f);
		Vector3 c = new Vector3(1f, 3f, 2f);
		Assert.IsTrue(a.AllLessThan(b));
		Assert.IsFalse(a.AllLessThan(c));
		Assert.IsTrue(a.AllLessThanEqual(new Vector3(1f, 2f, 3f)));
		Assert.IsTrue(b.AllGreaterThan(a));
		Assert.IsTrue(a.AllGreaterThanEqual(a));
		Assert.IsFalse(a.AllGreaterThanEqual(c));
		Assert.IsTrue(a.AnyLessThan(c));
		Assert.IsFalse(a.AnyLessThan(a));
		Assert.IsTrue(a.AnyLessThanEqual(a));
		Assert.IsTrue(a.AnyGreaterThan(c));
		Assert.IsFalse(a.AnyGreaterThan(b));
		Assert.IsTrue(a.AnyGreaterThanEqual(c));
		Assert.IsFalse(a.AnyGreaterThanEqual(b));
	}

	[TestMethod]
	public void IsFinite_DetectsNonFiniteComponents()
	{
		Assert.IsTrue(new Vector3(1f, 2f, 3f).IsFinite);
		Assert.IsFalse(new Vector3(1f, Single.NaN, 3f).IsFinite);
		Assert.IsFalse(new Vector3(1f, 2f, Single.PositiveInfinity).IsFinite);
	}

	[TestMethod]
	public void MajorAxis_ReturnsAxisWithLargestAbsoluteComponent()
	{
		Assert.AreEqual(Axis.X, new Vector3(-5f, 2f, 3f).MajorAxis);
		Assert.AreEqual(Axis.Y, new Vector3(1f, -4f, 3f).MajorAxis);
		Assert.AreEqual(Axis.Z, new Vector3(1f, 2f, -3f).MajorAxis);
		Assert.AreEqual(Axis.Z, new Vector3(5f, 1f, -6f).MajorAxis);
	}

	[TestMethod]
	public void FromAxis_ReturnsUnitVector()
	{
		Assert.AreEqual(Vector3.UnitX, Vector3.FromAxis(Axis.X));
		Assert.AreEqual(Vector3.UnitY, Vector3.FromAxis(Axis.Y));
		Assert.AreEqual(Vector3.UnitZ, Vector3.FromAxis(Axis.Z));
		Assert.AreEqual(Vector3.Zero, Vector3.FromAxis(Axis.W));
	}

	[TestMethod]
	public void ToArray_ReturnsComponents()
	{
		CollectionAssert.AreEqual(new[] { 1f, 2f, 3f }, new Vector3(1f, 2f, 3f).ToArray());
	}

	[TestMethod]
	public void ToString_And_Parse_RoundTrip()
	{
		Vector3 v = new Vector3(1.5f, -2.25f, 3f);
		string s = v.ToString(CultureInfo.InvariantCulture);
		Assert.AreEqual("1.5 -2.25 3", s);
		Assert.AreEqual(v, Vector3.Parse(s, CultureInfo.InvariantCulture));
		Assert.AreEqual("1.50 -2.25 3.00", v.ToString("F2", CultureInfo.InvariantCulture));
		Assert.AreEqual(v, Vector3.Parse("  1.5   -2.25\t3 ", CultureInfo.InvariantCulture));

		using (CultureScope.Invariant())
		{
			Assert.AreEqual("1.5 -2.25 3", v.ToString());
			Assert.AreEqual("1.50 -2.25 3.00", v.ToString("F2"));
			Assert.AreEqual(v, Vector3.Parse("1.5 -2.25 3"));
		}

		Assert.AreEqual("1,5 -2,25 3", v.ToString(TestCultures.CommaDecimal));
		Assert.AreEqual(v, Vector3.Parse("1,5 -2,25 3", TestCultures.CommaDecimal));
	}

	[TestMethod]
	public void Parse_InvalidInput_Throws()
	{
		Assert.ThrowsExactly<ArgumentNullException>(() => Vector3.Parse(null));
		Assert.ThrowsExactly<ArgumentNullException>(() => Vector3.Parse(null, CultureInfo.InvariantCulture));
		Assert.ThrowsExactly<FormatException>(() => Vector3.Parse("1 2", CultureInfo.InvariantCulture));
		Assert.ThrowsExactly<FormatException>(() => Vector3.Parse("1 2 3 4", CultureInfo.InvariantCulture));
		Assert.ThrowsExactly<FormatException>(() => Vector3.Parse("1 a 3", CultureInfo.InvariantCulture));
	}

	[TestMethod]
	public void Angle_ReturnsUnsignedAngleBetweenVectors()
	{
		AssertEx.AreEqual(SingleConstants.HalfPi, Vector3.Angle(Vector3.UnitX, Vector3.UnitY));
		AssertEx.AreEqual(SingleConstants.Pi, Vector3.Angle(Vector3.UnitX, -Vector3.UnitX));
		AssertEx.AreEqual(SingleConstants.Pi/4f, Vector3.Angle(new Vector3(1f, 0f, 0f), new Vector3(3f, 3f, 0f)));
		Assert.AreEqual(0f, Vector3.Angle(Vector3.UnitZ, 2f*Vector3.UnitZ));
		Assert.AreEqual(0f, Vector3.Angle(Vector3.Zero, Vector3.UnitZ));
	}

	[TestMethod]
	public void Slerp_InterpolatesAlongGreatCircle()
	{
		Vector3 u = Vector3.UnitX;
		Vector3 v = Vector3.UnitY;
		AssertEx.AreEqual(u, Vector3.Slerp(u, v, 0f));
		AssertEx.AreEqual(v, Vector3.Slerp(u, v, 1f));
		float h = MathF.Sqrt(0.5f);
		AssertEx.AreEqual(new Vector3(h, h, 0f), Vector3.Slerp(u, v, 0.5f));
		Vector3 third = Vector3.Slerp(u, v, 1f/3f);
		AssertEx.AreEqual(1f, third.Magnitude);
		AssertEx.AreEqual(SingleConstants.Pi/6f, Vector3.Angle(u, third));
		// Nearly identical inputs fall back to normalized lerp.
		AssertEx.AreEqual(u, Vector3.Slerp(u, u, 0.5f));
	}

	[TestMethod]
	public void TruncateLength_ClampsLongVectorsOnly()
	{
		Vector3 v = new Vector3(2f, 3f, 6f);
		AssertEx.AreEqual(new Vector3(2f, 3f, 6f)*(3.5f/7f), Vector3.TruncateLength(v, 3.5f));
		Assert.AreEqual(v, Vector3.TruncateLength(v, 10f));
		v.TruncateLength(1f);
		AssertEx.AreEqual(1f, v.Magnitude);
		Vector3 w = new Vector3(0.1f, 0f, 0f);
		w.TruncateLength(1f);
		Assert.AreEqual(new Vector3(0.1f, 0f, 0f), w);
	}

	[TestMethod]
	public void RotateAboutAxis_IsCounterClockwise()
	{
		float a = SingleConstants.HalfPi;
		AssertEx.AreEqual(Vector3.UnitZ, Vector3.Rotate(Vector3.UnitY, Axis.X, a));
		AssertEx.AreEqual(Vector3.UnitX, Vector3.Rotate(Vector3.UnitZ, Axis.Y, a));
		AssertEx.AreEqual(Vector3.UnitY, Vector3.Rotate(Vector3.UnitX, Axis.Z, a));
		Vector3 v = new Vector3(1f, 2f, 3f);
		Assert.AreEqual(v, Vector3.Rotate(v, Axis.X, 0f));
		Assert.AreEqual(v, Vector3.Rotate(v, Axis.W, 1f));

		Vector3 w = Vector3.UnitX;
		w.Rotate(Axis.Z, a);
		AssertEx.AreEqual(Vector3.UnitY, w);
	}

	[TestMethod]
	public void RotateAboutAxis_MatchesRotationMatrix()
	{
		foreach (Axis axis in new[] { Axis.X, Axis.Y, Axis.Z })
			foreach (float angle in Samples.Angles)
				foreach (Vector3 v in Samples.Vectors)
					AssertEx.AreEqual(v*Matrix3.Rotation(axis, angle), Vector3.Rotate(v, axis, angle), 1e-5f, $"{axis} {angle}");
	}

	[TestMethod]
	public void RotateByQuaternion_MatchesRotationMatrix()
	{
		foreach (Quaternion q in Samples.UnitQuaternions())
		{
			foreach (Vector3 v in Samples.Vectors)
			{
				Vector3 expected = v*Matrix3.FromQuaternion(q);
				AssertEx.AreEqual(expected, Vector3.Rotate(v, q), 1e-5f);
				Vector3 w = v;
				w.Rotate(q);
				AssertEx.AreEqual(expected, w, 1e-5f);
			}
		}
	}

	[TestMethod]
	public void Reflect_MirrorsAboutNormal()
	{
		AssertEx.AreEqual(new Vector3(1f, 1f, 0f), Vector3.Reflect(new Vector3(1f, -1f, 0f), Vector3.UnitY));
		Vector3 i = new Vector3(3f, -4f, 2f);
		AssertEx.AreEqual(i.Magnitude, Vector3.Reflect(i, Vector3.Normalize(new Vector3(1f, 1f, 1f))).Magnitude);
	}

	[TestMethod]
	public void Refract_FollowsSnellsLaw()
	{
		// Normal incidence passes straight through.
		AssertEx.AreEqual(-Vector3.UnitY, Vector3.Refract(-Vector3.UnitY, Vector3.UnitY, 1f/1.5f));

		// Equal indices leave direction unchanged.
		Vector3 i = Vector3.Normalize(new Vector3(1f, -1f, 0f));
		AssertEx.AreEqual(i, Vector3.Refract(i, Vector3.UnitY, 1f));

		// sin(theta_t) = eta*sin(theta_i)
		float eta = 1f/1.33f;
		Vector3 t = Vector3.Refract(i, Vector3.UnitY, eta);
		AssertEx.AreEqual(1f, t.Magnitude);
		AssertEx.AreEqual(eta*MathF.Sqrt(0.5f), t.X);
		Assert.IsTrue(t.Y < 0f);

		// Total internal reflection.
		Vector3 grazing = Vector3.Normalize(new Vector3(1f, -0.2f, 0f));
		Assert.AreEqual(Vector3.Zero, Vector3.Refract(grazing, Vector3.UnitY, 1.5f));
	}

	[TestMethod]
	public void Project_ProjectsOntoVector()
	{
		AssertEx.AreEqual(new Vector3(3f, 0f, 0f), Vector3.Project(new Vector3(3f, 4f, 5f), new Vector3(2f, 0f, 0f)));
		Assert.AreEqual(Vector3.Zero, Vector3.Project(new Vector3(3f, 4f, 5f), Vector3.Zero));
	}

	[TestMethod]
	public void Perpendicular_IsOrthogonalWithSameLength()
	{
		foreach (Vector3 v in Samples.Vectors.Concat(new[] { Vector3.UnitX, Vector3.UnitY, Vector3.UnitZ, new Vector3(1f, 1f, 1f) }))
		{
			Vector3 p = Vector3.Perpendicular(v);
			AssertEx.AreEqual(0f, Vector3.Dot(v, p), 1e-5f, v.ToString());
			AssertEx.AreEqual(v.Magnitude, p.Magnitude, 1e-5f, v.ToString());
		}
	}

	[TestMethod]
	public void ParallelAndPerpendicularComponents_SumToOriginal()
	{
		Vector3 basis = Vector3.Normalize(new Vector3(1f, 2f, 2f));
		foreach (Vector3 v in Samples.Vectors)
		{
			Vector3 par = Vector3.ParallelComponent(v, basis);
			Vector3 perp = Vector3.PerpendicularComponent(v, basis);
			AssertEx.AreEqual(v, par + perp);
			AssertEx.AreEqual(0f, Vector3.Dot(perp, basis));
			AssertEx.AreEqual(0f, Vector3.Cross(par, basis).Magnitude);
		}
	}

	[TestMethod]
	public void TransformByMatrix_UsesRowVectorConvention()
	{
		Matrix3 m = new Matrix3(1f, 2f, 3f, 4f, 5f, 6f, 7f, 8f, 9f);
		Vector3 v = new Vector3(1f, 0f, -1f);
		Vector3 expected = new Vector3(-6f, -6f, -6f); // v*M
		Assert.AreEqual(expected, v*m);
		Assert.AreEqual(expected, Vector3.Transform(v, m));
		Assert.AreEqual(new Vector3(-2f, -2f, -2f), m*v); // M*v (column vector)
		Vector3 w = v;
		w.Transform(m);
		Assert.AreEqual(expected, w);
	}

	[TestMethod]
	public void TransformByAffineTransform_AppliesBasisThenTranslation()
	{
		AffineTransform t = new AffineTransform(Matrix3.Rotation(Axis.Z, SingleConstants.HalfPi), new Vector3(10f, 20f, 30f));
		AssertEx.AreEqual(new Vector3(10f, 21f, 30f), Vector3.Transform(Vector3.UnitX, t));
		Vector3 v = Vector3.UnitX;
		v.Transform(t);
		AssertEx.AreEqual(new Vector3(10f, 21f, 30f), v);
	}
}
