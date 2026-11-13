using System.Globalization;

namespace Foundation.Mathematics.Tests;

[TestClass]
public sealed class EulerTests
{
	private static readonly EulerOrder[] Orders =
		{ EulerOrder.XYZ, EulerOrder.XZY, EulerOrder.YZX, EulerOrder.YXZ, EulerOrder.ZXY, EulerOrder.ZYX };

	[TestMethod]
	public void StaticFields_HaveZeroAnglesAndMatchingOrder()
	{
		Assert.AreEqual(new Euler(0f, 0f, 0f, EulerOrder.Unspecified), Euler.Zero);
		Assert.AreEqual(EulerOrder.XYZ, Euler.ZeroXYZ.Order);
		Assert.AreEqual(EulerOrder.XZY, Euler.ZeroXZY.Order);
		Assert.AreEqual(EulerOrder.YZX, Euler.ZeroYZX.Order);
		Assert.AreEqual(EulerOrder.YXZ, Euler.ZeroYXZ.Order);
		Assert.AreEqual(EulerOrder.ZXY, Euler.ZeroZXY.Order);
		Assert.AreEqual(EulerOrder.ZYX, Euler.ZeroZYX.Order);
		foreach (Euler e in new[] { Euler.ZeroXYZ, Euler.ZeroXZY, Euler.ZeroYZX, Euler.ZeroYXZ, Euler.ZeroZXY, Euler.ZeroZYX })
			CollectionAssert.AreEqual(new[] { 0f, 0f, 0f }, e.ToArray());
	}

	[TestMethod]
	public void Constructors_Properties_Indexer()
	{
		Euler e = new Euler(1f, 2f, 3f, EulerOrder.YXZ);
		Assert.AreEqual(1f, e.X);
		Assert.AreEqual(2f, e.Y);
		Assert.AreEqual(3f, e.Z);
		Assert.AreEqual(EulerOrder.YXZ, e.Order);
		Assert.AreEqual(new Euler(0f, 0f, 0f, EulerOrder.ZYX), new Euler(EulerOrder.ZYX));
		Assert.AreEqual(new Euler(1f, 2f, 3f, EulerOrder.XYZ), new Euler(e, EulerOrder.XYZ));
		Assert.AreEqual(e, new Euler(new[] { 1f, 2f, 3f }, EulerOrder.YXZ));
		Assert.AreEqual(2f, e[1]);
		e[0] = 4f;
		e.Y = 5f;
		e.Z = 6f;
		e.Order = EulerOrder.ZXY;
		Assert.AreEqual(new Euler(4f, 5f, 6f, EulerOrder.ZXY), e);
		Assert.ThrowsExactly<IndexOutOfRangeException>(() => _ = e[3]);
		Assert.ThrowsExactly<IndexOutOfRangeException>(() => e[-1] = 0f);
	}

	[TestMethod]
	public void Equality_IncludesOrder()
	{
		Euler a = new Euler(1f, 2f, 3f, EulerOrder.XYZ);
		Assert.IsTrue(a == new Euler(1f, 2f, 3f, EulerOrder.XYZ));
		Assert.IsTrue(a != new Euler(1f, 2f, 3f, EulerOrder.ZYX));
		Assert.IsTrue(a != new Euler(1f, 2f, 4f, EulerOrder.XYZ));
		Assert.IsTrue(a.Equals((object)new Euler(1f, 2f, 3f, EulerOrder.XYZ)));
		Assert.IsFalse(a.Equals((object)new Euler(1f, 2f, 3f, EulerOrder.YXZ)));
		Assert.AreEqual(a.GetHashCode(), new Euler(1f, 2f, 3f, EulerOrder.XYZ).GetHashCode());
		Assert.IsTrue(a.ApproxEquals(new Euler(1f, 2f, 3f + 1e-7f, EulerOrder.XYZ)));
		Assert.IsFalse(a.ApproxEquals(new Euler(1f, 2f, 3f, EulerOrder.YXZ)));
		Assert.IsTrue(a.ApproxEquals(new Euler(1.05f, 2f, 3f, EulerOrder.XYZ), 0.1f));
		Assert.IsFalse(a.ApproxEquals(new Euler(1.05f, 2f, 3f, EulerOrder.XYZ)));
	}

	[TestMethod]
	public void ArithmeticOperators_PreserveOrder()
	{
		Euler a = new Euler(1f, 2f, 4f, EulerOrder.YZX);
		Euler b = new Euler(0.5f, -1f, 2f, EulerOrder.YZX);
		Assert.AreEqual(a, +a);
		Assert.AreEqual(new Euler(-1f, -2f, -4f, EulerOrder.YZX), -a);
		Assert.AreEqual(new Euler(1.5f, 1f, 6f, EulerOrder.YZX), a + b);
		Assert.AreEqual(new Euler(0.5f, 3f, 2f, EulerOrder.YZX), a - b);
		Assert.AreEqual(new Euler(2f, 4f, 8f, EulerOrder.YZX), a*2f);
		Assert.AreEqual(new Euler(2f, 4f, 8f, EulerOrder.YZX), 2f*a);
		Assert.AreEqual(new Euler(0.5f, 1f, 2f, EulerOrder.YZX), a/2f);
		Assert.AreEqual(new Euler(4f, 2f, 1f, EulerOrder.YZX), 4f/a);
	}

	[TestMethod]
	public void AddSubtract_WithDifferentOrders_Throws()
	{
		Euler a = new Euler(1f, 2f, 3f, EulerOrder.XYZ);
		Euler b = new Euler(1f, 2f, 3f, EulerOrder.ZYX);
		Assert.ThrowsExactly<InvalidOperationException>(() => _ = a + b);
		Assert.ThrowsExactly<InvalidOperationException>(() => _ = a - b);
	}

	[TestMethod]
	public void IsFinite_ToArray()
	{
		Assert.IsTrue(new Euler(1f, 2f, 3f, EulerOrder.XYZ).IsFinite);
		Assert.IsFalse(new Euler(1f, Single.NaN, 3f, EulerOrder.XYZ).IsFinite);
		CollectionAssert.AreEqual(new[] { 1f, 2f, 3f }, new Euler(1f, 2f, 3f, EulerOrder.XYZ).ToArray());
	}

	[TestMethod]
	public void FromYawPitchRoll_MapsToZXYOrder()
	{
		YawPitchRoll ypr = new YawPitchRoll(0.1f, 0.2f, 0.3f);
		Euler e = Euler.FromYawPitchRoll(ypr);
		Assert.AreEqual(new Euler(0.2f, 0.1f, 0.3f, EulerOrder.ZXY), e);
		Euler implicitE = ypr;
		Assert.AreEqual(e, implicitE);
		AssertEx.AreEqual(Matrix3.Rotation(ypr), Matrix3.Rotation(e));
	}

	[TestMethod]
	public void FromMatrix_RoundTripsForEveryOrder()
	{
		Euler angles = new Euler(0.3f, -0.8f, 1.1f, EulerOrder.Unspecified);
		foreach (EulerOrder order in Orders)
		{
			Euler e = new Euler(angles, order);
			Euler back = Euler.FromMatrix(Matrix3.Rotation(e), order);
			Assert.AreEqual(order, back.Order);
			Assert.IsTrue(e.ApproxEquals(back, 1e-4f), $"{order}: expected {e}, got {back}");
		}
	}

	[TestMethod]
	public void FromMatrix_ConvertsBetweenOrders()
	{
		Matrix3 rotation = Matrix3.Rotation(Samples.Axes[4], 1.1f);
		foreach (EulerOrder order in Orders)
			AssertEx.AreEqual(rotation, Matrix3.Rotation(Euler.FromMatrix(rotation, order)), 1e-5f, order.ToString());
	}

	[TestMethod]
	public void FromMatrix_GimbalLock_StillReproducesRotation()
	{
		Euler e = new Euler(0.4f, SingleConstants.HalfPi, 0.7f, EulerOrder.XYZ);
		Matrix3 m = Matrix3.Rotation(e);
		AssertEx.AreEqual(m, Matrix3.Rotation(Euler.FromMatrix(m, EulerOrder.XYZ)), 1e-4f);
	}

	[TestMethod]
	public void FromMatrix_UnspecifiedOrder_ReturnsZero()
	{
		Assert.AreEqual(Euler.Zero, Euler.FromMatrix(Matrix3.Rotation(Axis.X, 1f), EulerOrder.Unspecified));
	}

	[TestMethod]
	public void FromQuaternion_RoundTrips()
	{
		foreach (EulerOrder order in Orders)
		{
			Euler e = new Euler(0.5f, 0.2f, -0.9f, order);
			Euler back = Euler.FromQuaternion(Quaternion.FromEuler(e), order);
			Assert.IsTrue(e.ApproxEquals(back, 1e-4f), $"{order}: expected {e}, got {back}");
		}
		Assert.AreEqual(Euler.ZeroYZX, Euler.FromQuaternion(Quaternion.Identity, EulerOrder.YZX));
	}

	[TestMethod]
	public void ToString_And_Parse()
	{
		Euler e = new Euler(0.5f, -1f, 2.25f, EulerOrder.YXZ);
		Assert.AreEqual("0.5 -1 2.25 YXZ", e.ToString(CultureInfo.InvariantCulture));
		Assert.AreEqual("0.50 -1.00 2.25 YXZ", e.ToString("F2", CultureInfo.InvariantCulture));
		Assert.AreEqual(e, Euler.Parse("0.5 -1 2.25 YXZ", CultureInfo.InvariantCulture));
		Assert.AreEqual(e, Euler.Parse("0.5 -1 2.25 yxz", CultureInfo.InvariantCulture));

		Euler degrees = Euler.Parse("90 -45 180 ZYX", CultureInfo.InvariantCulture, true);
		Assert.IsTrue(new Euler(SingleConstants.HalfPi, -SingleConstants.Pi/4f, SingleConstants.Pi, EulerOrder.ZYX).ApproxEquals(degrees));
		Assert.AreEqual(new Euler(90f, -45f, 180f, EulerOrder.ZYX), Euler.Parse("90 -45 180 ZYX", CultureInfo.InvariantCulture, false));

		using (CultureScope.Invariant())
		{
			Assert.AreEqual("0.5 -1 2.25 YXZ", e.ToString());
			Assert.AreEqual("0.5 -1 2.25 YXZ", e.ToString(false));
			Assert.AreEqual("0.50 -1.00 2.25 YXZ", e.ToString("F2"));
			Euler d = new Euler(SingleConstants.HalfPi, 0f, -SingleConstants.Pi, EulerOrder.XYZ);
			string[] parts = d.ToString(true).Split(' ');
			AssertEx.AreEqual(90f, Single.Parse(parts[0]), 1e-3f);
			AssertEx.AreEqual(0f, Single.Parse(parts[1]), 1e-3f);
			AssertEx.AreEqual(-180f, Single.Parse(parts[2]), 1e-3f);
			Assert.AreEqual("XYZ", parts[3]);
			Assert.AreEqual(e, Euler.Parse("0.5 -1 2.25 YXZ"));
			Assert.IsTrue(d.ApproxEquals(Euler.Parse("90 0 -180 XYZ", true)));
			Assert.AreEqual(new Euler(1f, 2f, 3f, EulerOrder.XYZ), Euler.Parse("1 2 3 XYZ", false));
		}

		Assert.ThrowsExactly<ArgumentNullException>(() => Euler.Parse(null));
		Assert.ThrowsExactly<ArgumentNullException>(() => Euler.Parse(null, true));
		Assert.ThrowsExactly<FormatException>(() => Euler.Parse("1 2 3", CultureInfo.InvariantCulture));
		Assert.ThrowsExactly<ArgumentException>(() => Euler.Parse("1 2 3 ABC", CultureInfo.InvariantCulture));
	}
}

[TestClass]
public sealed class YawPitchRollTests
{
	[TestMethod]
	public void Constructor_Properties_Zero()
	{
		YawPitchRoll ypr = new YawPitchRoll(0.1f, 0.2f, 0.3f);
		Assert.AreEqual(0.1f, ypr.Yaw);
		Assert.AreEqual(0.2f, ypr.Pitch);
		Assert.AreEqual(0.3f, ypr.Roll);
		ypr.Yaw = 1f;
		ypr.Pitch = 2f;
		ypr.Roll = 3f;
		Assert.AreEqual(new YawPitchRoll(1f, 2f, 3f), ypr);
		Assert.AreEqual(new YawPitchRoll(0f, 0f, 0f), YawPitchRoll.Zero);
	}

	[TestMethod]
	public void Equality_ApproxEquals_HashCode()
	{
		YawPitchRoll a = new YawPitchRoll(1f, 2f, 3f);
		Assert.IsTrue(a == new YawPitchRoll(1f, 2f, 3f));
		Assert.IsTrue(a != new YawPitchRoll(1f, 2f, 4f));
		Assert.IsTrue(a.Equals((object)new YawPitchRoll(1f, 2f, 3f)));
		Assert.IsFalse(a.Equals((object)new Vector3(1f, 2f, 3f)));
		Assert.AreEqual(a.GetHashCode(), new YawPitchRoll(1f, 2f, 3f).GetHashCode());
		Assert.IsTrue(a.ApproxEquals(new YawPitchRoll(1f, 2f, 3f + 1e-7f)));
		Assert.IsFalse(a.ApproxEquals(new YawPitchRoll(1f, 2.01f, 3f)));
		Assert.IsTrue(a.ApproxEquals(new YawPitchRoll(1f, 2.01f, 3f), 0.1f));
		Assert.IsTrue(a.IsFinite);
		Assert.IsFalse(new YawPitchRoll(Single.NaN, 0f, 0f).IsFinite);
	}

	[TestMethod]
	public void ArithmeticOperators()
	{
		YawPitchRoll a = new YawPitchRoll(1f, 2f, 4f);
		YawPitchRoll b = new YawPitchRoll(0.5f, -1f, 2f);
		Assert.AreEqual(a, +a);
		Assert.AreEqual(new YawPitchRoll(-1f, -2f, -4f), -a);
		Assert.AreEqual(new YawPitchRoll(1.5f, 1f, 6f), a + b);
		Assert.AreEqual(new YawPitchRoll(0.5f, 3f, 2f), a - b);
		Assert.AreEqual(new YawPitchRoll(2f, 4f, 8f), a*2f);
		Assert.AreEqual(new YawPitchRoll(2f, 4f, 8f), 2f*a);
		Assert.AreEqual(new YawPitchRoll(0.5f, 1f, 2f), a/2f);
		Assert.AreEqual(new YawPitchRoll(4f, 2f, 1f), 4f/a);
	}

	[TestMethod]
	public void FromEuler_ZXYIsDirectMapping()
	{
		Euler e = new Euler(0.2f, 0.1f, 0.3f, EulerOrder.ZXY);
		Assert.AreEqual(new YawPitchRoll(0.1f, 0.2f, 0.3f), YawPitchRoll.FromEuler(e));
		Assert.AreEqual(new YawPitchRoll(0.1f, 0.2f, 0.3f), (YawPitchRoll)e);
		Assert.AreEqual(YawPitchRoll.Zero, YawPitchRoll.FromEuler(new Euler(1f, 2f, 3f, EulerOrder.Unspecified)));
	}

	[TestMethod]
	public void FromEuler_OtherOrdersPreserveRotation()
	{
		Euler e = new Euler(0.4f, -0.3f, 0.9f, EulerOrder.XYZ);
		AssertEx.AreEqual(Matrix3.Rotation(e), Matrix3.Rotation(YawPitchRoll.FromEuler(e)), 1e-5f);
	}

	[TestMethod]
	public void FromMatrix_And_FromQuaternion_RoundTrip()
	{
		YawPitchRoll ypr = new YawPitchRoll(0.7f, -0.4f, 1.3f);
		Assert.IsTrue(ypr.ApproxEquals(YawPitchRoll.FromMatrix(Matrix3.Rotation(ypr)), 1e-4f));
		Assert.IsTrue(ypr.ApproxEquals(YawPitchRoll.FromQuaternion(Quaternion.FromYawPitchRoll(ypr)), 1e-4f));
		Assert.AreEqual(YawPitchRoll.Zero, YawPitchRoll.FromQuaternion(Quaternion.Identity));
	}

	[TestMethod]
	public void ToForward_PointsAlongRotatedZAxis()
	{
		AssertEx.AreEqual(Vector3.UnitZ, YawPitchRoll.Zero.ToForward());
		AssertEx.AreEqual(Vector3.UnitX, new YawPitchRoll(SingleConstants.HalfPi, 0f, 0f).ToForward());
		AssertEx.AreEqual(-Vector3.UnitY, new YawPitchRoll(0f, SingleConstants.HalfPi, 0f).ToForward());
		YawPitchRoll ypr = new YawPitchRoll(0.7f, -0.4f, 1.3f);
		AssertEx.AreEqual(Vector3.UnitZ*Matrix3.Rotation(ypr), ypr.ToForward());
	}

	[TestMethod]
	public void FromForward_InvertsToForward()
	{
		YawPitchRoll[] values =
		{
			new YawPitchRoll(0.7f, -0.4f, 0f), new YawPitchRoll(-2.5f, 0.3f, 0f), new YawPitchRoll(0f, 1f, 0f),
			new YawPitchRoll(3f, -1.2f, 0f),
		};
		foreach (YawPitchRoll ypr in values)
		{
			Vector3 forward = ypr.ToForward()*3f;
			Assert.IsTrue(ypr.ApproxEquals(YawPitchRoll.FromForward(forward), 1e-3f), ypr.ToString());
			YawPitchRoll withRoll = YawPitchRoll.FromForward(forward, 0.25f);
			Assert.IsTrue(new YawPitchRoll(ypr.Yaw, ypr.Pitch, 0.25f).ApproxEquals(withRoll, 1e-3f));
		}
		Assert.AreEqual(YawPitchRoll.Zero, YawPitchRoll.FromForward(Vector3.Zero));
		YawPitchRoll up = YawPitchRoll.FromForward(Vector3.UnitY);
		AssertEx.AreEqual(-SingleConstants.HalfPi, up.Pitch);
		Assert.AreEqual(0f, up.Yaw);
		AssertEx.AreEqual(SingleConstants.HalfPi, YawPitchRoll.FromForward(-Vector3.UnitY, 0.5f).Pitch);
	}

	[TestMethod]
	public void ToString_And_Parse()
	{
		YawPitchRoll ypr = new YawPitchRoll(0.5f, -1f, 2.25f);
		Assert.AreEqual("0.5 -1 2.25", ypr.ToString(CultureInfo.InvariantCulture));
		Assert.AreEqual("0.50 -1.00 2.25", ypr.ToString("F2", CultureInfo.InvariantCulture));
		Assert.AreEqual(ypr, YawPitchRoll.Parse("0.5 -1 2.25", CultureInfo.InvariantCulture));
		Assert.AreEqual(new YawPitchRoll(90f, 45f, 0f), YawPitchRoll.Parse("90 45 0", CultureInfo.InvariantCulture, false));
		Assert.IsTrue(new YawPitchRoll(SingleConstants.HalfPi, SingleConstants.Pi/4f, 0f)
			.ApproxEquals(YawPitchRoll.Parse("90 45 0", CultureInfo.InvariantCulture, true)));
		using (CultureScope.Invariant())
		{
			Assert.AreEqual("0.5 -1 2.25", ypr.ToString());
			Assert.AreEqual("0.5 -1 2.25", ypr.ToString(false));
			Assert.AreEqual("0.50 -1.00 2.25", ypr.ToString("F2"));
			string[] parts = new YawPitchRoll(SingleConstants.Pi, 0f, -SingleConstants.HalfPi).ToString(true).Split(' ');
			AssertEx.AreEqual(180f, Single.Parse(parts[0]), 1e-3f);
			AssertEx.AreEqual(-90f, Single.Parse(parts[2]), 1e-3f);
			Assert.AreEqual(ypr, YawPitchRoll.Parse("0.5 -1 2.25"));
			Assert.AreEqual(new YawPitchRoll(1f, 2f, 3f), YawPitchRoll.Parse("1 2 3", false));
			Assert.IsTrue(new YawPitchRoll(SingleConstants.Pi, 0f, 0f).ApproxEquals(YawPitchRoll.Parse("180 0 0", true)));
		}
		Assert.ThrowsExactly<ArgumentNullException>(() => YawPitchRoll.Parse(null));
		Assert.ThrowsExactly<FormatException>(() => YawPitchRoll.Parse("1 2", CultureInfo.InvariantCulture));
	}
}
