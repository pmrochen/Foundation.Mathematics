using System.Globalization;

namespace Foundation.Mathematics.Tests;

[TestClass]
public sealed class QuaternionTests
{
	private static readonly EulerOrder[] Orders =
		{ EulerOrder.XYZ, EulerOrder.XZY, EulerOrder.YZX, EulerOrder.YXZ, EulerOrder.ZXY, EulerOrder.ZYX };

	[TestMethod]
	public void StaticFields_HaveExpectedValues()
	{
		Assert.AreEqual(new Quaternion(0f, 0f, 0f, 0f), Quaternion.Zero);
		Assert.AreEqual(new Quaternion(0f, 0f, 0f, 1f), Quaternion.Identity);
	}

	[TestMethod]
	public void Constructors_And_Properties()
	{
		Quaternion q = new Quaternion(1f, 2f, 3f, 4f);
		Assert.AreEqual(1f, q.X);
		Assert.AreEqual(2f, q.Y);
		Assert.AreEqual(3f, q.Z);
		Assert.AreEqual(4f, q.W);
		Assert.AreEqual(new Vector3(1f, 2f, 3f), q.Vector);
		Assert.AreEqual(4f, q.Scalar);
		Assert.AreEqual(new Quaternion(0f, 0f, 0f, 5f), new Quaternion(5f));
		Assert.AreEqual(new Quaternion(1f, 2f, 3f, 0f), new Quaternion(new Vector3(1f, 2f, 3f)));
		Assert.AreEqual(q, new Quaternion(new Vector3(1f, 2f, 3f), 4f));
		Assert.AreEqual(q, new Quaternion(new[] { 1f, 2f, 3f, 4f }));
		Quaternion fromScalar = (Quaternion)5f;
		Quaternion fromVector = (Quaternion)new Vector3(1f, 2f, 3f);
		Assert.AreEqual(new Quaternion(5f), fromScalar);
		Assert.AreEqual(new Quaternion(1f, 2f, 3f, 0f), fromVector);

		q.X = -1f;
		q.Y = -2f;
		q.Z = -3f;
		q.W = -4f;
		Assert.AreEqual(new Quaternion(-1f, -2f, -3f, -4f), q);
		q.Vector = new Vector3(7f, 8f, 9f);
		q.Scalar = 10f;
		Assert.AreEqual(new Quaternion(7f, 8f, 9f, 10f), q);
	}

	[TestMethod]
	public void Indexer_GetsSetsAndThrows()
	{
		Quaternion q = new Quaternion(1f, 2f, 3f, 4f);
		for (int i = 0; i < 4; i++)
			Assert.AreEqual(i + 1f, q[i]);
		q[3] = 0f;
		Assert.AreEqual(0f, q.W);
		Assert.ThrowsExactly<IndexOutOfRangeException>(() => _ = q[4]);
		Assert.ThrowsExactly<IndexOutOfRangeException>(() => q[-1] = 0f);
	}

	[TestMethod]
	public void Norms()
	{
		Quaternion q = new Quaternion(1f, 2f, 2f, 4f);
		Assert.AreEqual(5f, q.Magnitude);
		Assert.AreEqual(5f, q.AbsoluteValue);
		Assert.AreEqual(25f, q.MagnitudeSquared);
		Assert.AreEqual(25f, q.Norm);
	}

	[TestMethod]
	public void Equality_ApproxEquals_HashCode()
	{
		Quaternion a = new Quaternion(1f, 2f, 3f, 4f);
		Assert.IsTrue(a == new Quaternion(1f, 2f, 3f, 4f));
		Assert.IsTrue(a != new Quaternion(1f, 2f, 3f, -4f));
		Assert.IsTrue(a.Equals((object)new Quaternion(1f, 2f, 3f, 4f)));
		Assert.IsFalse(a.Equals((object)new Vector4(1f, 2f, 3f, 4f)));
		Assert.AreEqual(a.GetHashCode(), new Quaternion(1f, 2f, 3f, 4f).GetHashCode());
		Assert.IsTrue(a.ApproxEquals(new Quaternion(1f, 2f, 3f, 4f + 1e-7f)));
		Assert.IsFalse(a.ApproxEquals(new Quaternion(1f, 2f, 3.01f, 4f)));
		Assert.IsTrue(a.ApproxEquals(new Quaternion(1f, 2f, 3.01f, 4f), 0.1f));
	}

	[TestMethod]
	public void LinearOperators()
	{
		Quaternion q = new Quaternion(1f, 2f, 3f, 4f);
		Quaternion r = new Quaternion(-1f, 0.5f, 2f, 1f);
		Assert.AreEqual(q, +q);
		Assert.AreEqual(new Quaternion(-1f, -2f, -3f, -4f), -q);
		Assert.AreEqual(new Quaternion(0f, 2.5f, 5f, 5f), q + r);
		Assert.AreEqual(new Quaternion(2f, 1.5f, 1f, 3f), q - r);
		Assert.AreEqual(new Quaternion(2f, 4f, 6f, 8f), q*2f);
		Assert.AreEqual(new Quaternion(2f, 4f, 6f, 8f), 2f*q);
		Assert.AreEqual(new Quaternion(0.5f, 1f, 1.5f, 2f), q/2f);
		AssertEx.AreEqual(2f*Quaternion.Inverse(q), 2f/q);
	}

	[TestMethod]
	public void HamiltonProduct_FollowsUnitRules()
	{
		Quaternion i = new Quaternion(1f, 0f, 0f, 0f);
		Quaternion j = new Quaternion(0f, 1f, 0f, 0f);
		Quaternion k = new Quaternion(0f, 0f, 1f, 0f);
		Quaternion minusOne = new Quaternion(-1f);
		Assert.AreEqual(minusOne, i*i);
		Assert.AreEqual(minusOne, j*j);
		Assert.AreEqual(minusOne, k*k);
		Assert.AreEqual(k, i*j);
		Assert.AreEqual(-k, j*i);
		Assert.AreEqual(i, j*k);
		Assert.AreEqual(j, k*i);
		Assert.AreEqual(minusOne, i*j*k);

		Quaternion q = new Quaternion(1f, 2f, 3f, 4f);
		Assert.AreEqual(q, q*Quaternion.Identity);
		Assert.AreEqual(q, Quaternion.Identity*q);
	}

	[TestMethod]
	public void ProductWithVector_TreatsVectorAsPureQuaternion()
	{
		Quaternion q = new Quaternion(0.5f, -1f, 2f, 3f);
		Vector3 v = new Vector3(1f, 2f, -1f);
		Assert.AreEqual(q*new Quaternion(v), q*v);
		Assert.AreEqual(new Quaternion(v)*q, v*q);
	}

	[TestMethod]
	public void Dot_Normalize_Conjugate_Inverse()
	{
		Quaternion q = new Quaternion(1f, 2f, 2f, 4f);
		Assert.AreEqual(25f, Quaternion.Dot(q, q));
		Assert.AreEqual(4f, Quaternion.Dot(q, Quaternion.Identity));
		AssertEx.AreEqual(new Quaternion(0.2f, 0.4f, 0.4f, 0.8f), Quaternion.Normalize(q));
		Assert.AreEqual(Quaternion.Zero, Quaternion.Normalize(Quaternion.Zero));
		Assert.AreEqual(new Quaternion(-1f, -2f, -2f, 4f), Quaternion.Conjugate(q));
		AssertEx.AreEqual(Quaternion.Identity, q*Quaternion.Inverse(q));
		AssertEx.AreEqual(Quaternion.Identity, Quaternion.Inverse(q)*q);

		Quaternion n = q;
		n.Normalize();
		AssertEx.AreEqual(1f, n.Magnitude);
		Quaternion c = q;
		c.Conjugate();
		Assert.AreEqual(Quaternion.Conjugate(q), c);
		Quaternion inv = q;
		inv.Invert();
		AssertEx.AreEqual(Quaternion.Inverse(q), inv);
	}

	[TestMethod]
	public void FromAxisAngle_AxisAndAngleRoundTrip()
	{
		for (int i = 0; i < Samples.Axes.Length; i++)
		{
			float angle = Math.Abs(Samples.Angles[i]);
			Quaternion q = Quaternion.FromAxisAngle(Samples.Axes[i]*2f, angle);
			AssertEx.AreEqual(1f, q.Magnitude);
			AssertEx.AreEqual(angle, q.Angle, 1e-4f);
			AssertEx.AreEqual(Samples.Axes[i], q.Axis, 1e-4f);
		}
		Assert.AreEqual(Quaternion.Identity, Quaternion.FromAxisAngle(Vector3.Zero, 1f));
		Assert.AreEqual(Quaternion.Identity, Quaternion.FromAxisAngle(Vector3.UnitX, 0f));
		Assert.AreEqual(Vector3.Zero, Quaternion.Identity.Axis);
		Assert.AreEqual(0f, Quaternion.Identity.Angle);
	}

	[TestMethod]
	public void FromAxisAngle_RotatesCounterClockwise()
	{
		Quaternion q = Quaternion.FromAxisAngle(Vector3.UnitZ, SingleConstants.HalfPi);
		AssertEx.AreEqual(Vector3.UnitY, Vector3.Rotate(Vector3.UnitX, q));
	}

	[TestMethod]
	public void FromMatrix_RoundTripsThroughMatrix()
	{
		var quaternions = Samples.UnitQuaternions().ToList();
		quaternions.Add(Quaternion.FromAxisAngle(Vector3.UnitX, SingleConstants.Pi));
		quaternions.Add(Quaternion.FromAxisAngle(Vector3.UnitY, 3f));
		quaternions.Add(Quaternion.FromAxisAngle(Vector3.UnitZ, -3f));
		quaternions.Add(Quaternion.Identity);
		foreach (Quaternion q in quaternions)
		{
			Matrix3 m = Matrix3.FromQuaternion(q);
			AssertEx.AreSameRotation(q, Quaternion.FromMatrix(m), 1e-5f);
			AssertEx.AreSameRotation(q, (Quaternion)m, 1e-5f);
		}
	}

	[TestMethod]
	public void FromEuler_MatchesMatrixRotation()
	{
		Euler angles = new Euler(0.3f, -0.8f, 1.1f, EulerOrder.Unspecified);
		foreach (EulerOrder order in Orders)
		{
			Euler e = new Euler(angles, order);
			AssertEx.AreEqual(Matrix3.Rotation(e), Matrix3.FromQuaternion(Quaternion.FromEuler(e)), 1e-5f, order.ToString());
		}
		Assert.AreEqual(Quaternion.Identity, Quaternion.FromEuler(Euler.ZeroXYZ));
		Assert.AreEqual(Quaternion.Identity, Quaternion.FromEuler(new Euler(1f, 2f, 3f, EulerOrder.Unspecified)));
	}

	[TestMethod]
	public void FromYawPitchRoll_MatchesMatrixRotation()
	{
		YawPitchRoll[] values =
		{
			new YawPitchRoll(0.7f, 0f, 0f), new YawPitchRoll(0f, 0.7f, 0f), new YawPitchRoll(0f, 0f, 0.7f),
			new YawPitchRoll(0.7f, -0.4f, 1.3f), new YawPitchRoll(-2f, 1f, -0.5f),
		};
		foreach (YawPitchRoll ypr in values)
			AssertEx.AreEqual(Matrix3.Rotation(ypr), Matrix3.FromQuaternion(Quaternion.FromYawPitchRoll(ypr)), 1e-5f, ypr.ToString());
		Assert.AreEqual(Quaternion.Identity, Quaternion.FromYawPitchRoll(YawPitchRoll.Zero));
	}

	[TestMethod]
	public void Concat_AppliesFirstArgumentFirst()
	{
		Quaternion q = Quaternion.FromAxisAngle(Vector3.UnitX, 0.5f);
		Quaternion r = Quaternion.FromAxisAngle(Vector3.UnitY, -1.2f);
		Quaternion u = Quaternion.FromAxisAngle(Samples.Axes[3], 0.9f);
		Quaternion qr = Quaternion.Concat(q, r);
		Assert.AreEqual(r*q, qr);
		AssertEx.AreEqual(Matrix3.Concat(Matrix3.FromQuaternion(q), Matrix3.FromQuaternion(r)), Matrix3.FromQuaternion(qr));
		foreach (Vector3 v in Samples.Vectors)
			AssertEx.AreEqual(Vector3.Rotate(Vector3.Rotate(v, q), r), Vector3.Rotate(v, qr), 1e-5f);

		AssertEx.AreEqual(Quaternion.Concat(Quaternion.Concat(q, r), u), Quaternion.Concat(q, r, u));
		AssertEx.AreEqual(Quaternion.Concat(Quaternion.Concat(q, r, u), q), Quaternion.Concat(q, r, u, q));
		AssertEx.AreEqual(Quaternion.Concat(Quaternion.Concat(q, r, u, q), r), Quaternion.Concat(q, r, u, q, r));
		AssertEx.AreEqual(Quaternion.Concat(Quaternion.Concat(q, r, u, q, r), u), Quaternion.Concat(q, r, u, q, r, u));
		AssertEx.AreEqual(Quaternion.Concat(Quaternion.Concat(q, r, u, q, r, u), q), Quaternion.Concat(q, r, u, q, r, u, q));
		AssertEx.AreEqual(Quaternion.Concat(Quaternion.Concat(q, r, u, q, r, u, q), r), Quaternion.Concat(q, r, u, q, r, u, q, r));

		Quaternion c = q;
		c.Concat(r);
		Assert.AreEqual(r*q, c);
		Quaternion p = q;
		p.PreConcat(r);
		Assert.AreEqual(q*r, p);
	}

	[TestMethod]
	public void Rotate_ConjugatesQuaternion()
	{
		Quaternion r = Quaternion.FromAxisAngle(Vector3.UnitZ, SingleConstants.HalfPi);
		Quaternion pure = new Quaternion(Vector3.UnitX);
		AssertEx.AreEqual(new Quaternion(Vector3.UnitY), Quaternion.Rotate(pure, r));
		pure.Rotate(r);
		AssertEx.AreEqual(new Quaternion(Vector3.UnitY), pure);
	}

	[TestMethod]
	public void Lerp_InterpolatesComponentsAlongShortestArc()
	{
		Quaternion q = Quaternion.Identity;
		Quaternion r = Quaternion.FromAxisAngle(Vector3.UnitZ, 1f);
		Assert.AreEqual(q, Quaternion.Lerp(q, r, 0f));
		AssertEx.AreEqual(r, Quaternion.Lerp(q, r, 1f));
		AssertEx.AreEqual(0.5f*(q + r), Quaternion.Lerp(q, r, 0.5f));
		// Opposite hemisphere: -r is used instead of r.
		AssertEx.AreEqual(r, Quaternion.Lerp(q, -r, 1f));
	}

	[TestMethod]
	public void Slerp_InterpolatesRotationAngle()
	{
		Vector3 axis = Samples.Axes[4];
		Quaternion q = Quaternion.Identity;
		Quaternion r = Quaternion.FromAxisAngle(axis, 1.6f);
		AssertEx.AreEqual(q, Quaternion.Slerp(q, r, 0f));
		AssertEx.AreEqual(r, Quaternion.Slerp(q, r, 1f));
		AssertEx.AreEqual(Quaternion.FromAxisAngle(axis, 0.4f), Quaternion.Slerp(q, r, 0.25f));
		AssertEx.AreEqual(Quaternion.FromAxisAngle(axis, 0.8f), Quaternion.Slerp(q, r, 0.5f));
		AssertEx.AreEqual(1f, Quaternion.Slerp(q, r, 0.3f).Magnitude);
		// Shortest arc is taken for opposite-hemisphere inputs.
		AssertEx.AreEqual(Quaternion.FromAxisAngle(axis, 0.8f), Quaternion.Slerp(q, -r, 0.5f));
		// Nearly identical inputs.
		AssertEx.AreEqual(r, Quaternion.Slerp(r, r, 0.5f));
	}

	[TestMethod]
	public void Arc_RotatesFirstVectorOntoSecond()
	{
		Vector3[] pairs =
		{
			Vector3.UnitX, Vector3.UnitY,
			new Vector3(1f, 2f, 3f), new Vector3(-2f, 0.5f, 1f),
			new Vector3(0f, 0f, 5f), new Vector3(0f, 3f, 3f),
		};
		for (int i = 0; i < pairs.Length; i += 2)
		{
			Quaternion q = Quaternion.Arc(pairs[i], pairs[i + 1]);
			AssertEx.AreEqual(1f, q.Magnitude);
			AssertEx.AreEqual(Vector3.Normalize(pairs[i + 1]), Vector3.Rotate(Vector3.Normalize(pairs[i]), q), 1e-5f);
		}
	}

	[TestMethod]
	public void IsFinite_ToArray_ToString_Parse()
	{
		Quaternion q = new Quaternion(0.5f, -1f, 0f, 2.25f);
		Assert.IsTrue(q.IsFinite);
		Assert.IsFalse(new Quaternion(Single.NaN).IsFinite);
		CollectionAssert.AreEqual(new[] { 0.5f, -1f, 0f, 2.25f }, q.ToArray());
		Assert.AreEqual("0.5 -1 0 2.25", q.ToString(CultureInfo.InvariantCulture));
		Assert.AreEqual("0.50 -1.00 0.00 2.25", q.ToString("F2", CultureInfo.InvariantCulture));
		Assert.AreEqual(q, Quaternion.Parse("0.5 -1 0 2.25", CultureInfo.InvariantCulture));
		using (CultureScope.Invariant())
		{
			Assert.AreEqual("0.5 -1 0 2.25", q.ToString());
			Assert.AreEqual("0.50 -1.00 0.00 2.25", q.ToString("F2"));
			Assert.AreEqual(q, Quaternion.Parse("0.5 -1 0 2.25"));
		}
		Assert.ThrowsExactly<ArgumentNullException>(() => Quaternion.Parse(null));
		Assert.ThrowsExactly<FormatException>(() => Quaternion.Parse("1 2 3", CultureInfo.InvariantCulture));
	}
}
