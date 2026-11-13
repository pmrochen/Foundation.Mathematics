using System.Globalization;

namespace Foundation.Mathematics.Tests;

[TestClass]
public sealed class Matrix3Tests
{
	private static readonly Matrix3 M = new Matrix3(1f, 2f, 3f, 4f, 5f, 6f, 7f, 8f, 10f);

	private static readonly EulerOrder[] Orders =
		{ EulerOrder.XYZ, EulerOrder.XZY, EulerOrder.YZX, EulerOrder.YXZ, EulerOrder.ZXY, EulerOrder.ZYX };

	private static Matrix3 AxisRotation(char axis, Euler e)
	{
		switch (axis)
		{
			case 'X': return Matrix3.Rotation(Axis.X, e.X);
			case 'Y': return Matrix3.Rotation(Axis.Y, e.Y);
			default: return Matrix3.Rotation(Axis.Z, e.Z);
		}
	}

	[TestMethod]
	public void StaticFields_HaveExpectedValues()
	{
		Assert.AreEqual(new Matrix3(0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f), Matrix3.Zero);
		Assert.AreEqual(new Matrix3(1f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 1f), Matrix3.Identity);
	}

	[TestMethod]
	public void Constructors_And_ElementAccess()
	{
		Assert.AreEqual(M, new Matrix3(new Vector3(1f, 2f, 3f), new Vector3(4f, 5f, 6f), new Vector3(7f, 8f, 10f)));
		Assert.AreEqual(M, new Matrix3(new[] { 1f, 2f, 3f, 4f, 5f, 6f, 7f, 8f, 10f }));
		float[] expected = { 1f, 2f, 3f, 4f, 5f, 6f, 7f, 8f, 10f };
		float[] actual = { M.M00, M.M01, M.M02, M.M10, M.M11, M.M12, M.M20, M.M21, M.M22 };
		CollectionAssert.AreEqual(expected, actual);
		for (int r = 0; r < 3; r++)
			for (int c = 0; c < 3; c++)
				Assert.AreEqual(expected[3*r + c], M[r, c]);
		Assert.AreEqual(new Vector3(1f, 2f, 3f), M.Row0);
		Assert.AreEqual(new Vector3(4f, 5f, 6f), M.Row1);
		Assert.AreEqual(new Vector3(7f, 8f, 10f), M.Row2);
		Assert.AreEqual(new Vector3(4f, 5f, 6f), M[1]);
		Assert.AreEqual(new Vector3(1f, 4f, 7f), M.Column0);
		Assert.AreEqual(new Vector3(2f, 5f, 8f), M.Column1);
		Assert.AreEqual(new Vector3(3f, 6f, 10f), M.Column2);
		Assert.AreEqual(M.Row0, M.Right);
		Assert.AreEqual(M.Row1, M.Up);
		Assert.AreEqual(M.Row2, M.Forward);
	}

	[TestMethod]
	public void Setters_ModifyElements()
	{
		Matrix3 m = Matrix3.Zero;
		m.M00 = 1f; m.M01 = 2f; m.M02 = 3f;
		m.M10 = 4f; m.M11 = 5f; m.M12 = 6f;
		m.M20 = 7f; m.M21 = 8f; m.M22 = 10f;
		Assert.AreEqual(M, m);

		m.Right = Vector3.UnitX;
		m.Up = Vector3.UnitY;
		m.Forward = Vector3.UnitZ;
		Assert.AreEqual(Matrix3.Identity, m);

		m.Row0 = new Vector3(1f, 1f, 1f);
		m.Row1 = new Vector3(2f, 2f, 2f);
		m.Row2 = new Vector3(3f, 3f, 3f);
		m[1] = new Vector3(4f, 4f, 4f);
		m[2, 0] = -1f;
		Assert.AreEqual(new Matrix3(1f, 1f, 1f, 4f, 4f, 4f, -1f, 3f, 3f), m);
	}

	[TestMethod]
	public void Indexers_OutOfRange_Throw()
	{
		Matrix3 m = M;
		Assert.ThrowsExactly<IndexOutOfRangeException>(() => _ = m[3]);
		Assert.ThrowsExactly<IndexOutOfRangeException>(() => _ = m[0, 3]);
		Assert.ThrowsExactly<IndexOutOfRangeException>(() => _ = m[3, 0]);
		Assert.ThrowsExactly<IndexOutOfRangeException>(() => m[3] = Vector3.Zero);
		Assert.ThrowsExactly<IndexOutOfRangeException>(() => m[-1, 0] = 0f);
	}

	[TestMethod]
	public void ImplicitConversionFromMatrix2_EmbedsInUpperLeft()
	{
		Matrix3 m = new Matrix2(1f, 2f, 3f, 4f);
		Assert.AreEqual(new Matrix3(1f, 2f, 0f, 3f, 4f, 0f, 0f, 0f, 1f), m);
	}

	[TestMethod]
	public void Equality_ApproxEquals_HashCode()
	{
		Matrix3 same = new Matrix3(1f, 2f, 3f, 4f, 5f, 6f, 7f, 8f, 10f);
		Assert.IsTrue(M == same);
		Assert.IsFalse(M != same);
		Assert.IsTrue(M != Matrix3.Identity);
		Assert.IsTrue(M.Equals((object)same));
		Assert.IsFalse(M.Equals("matrix"));
		Assert.AreEqual(M.GetHashCode(), same.GetHashCode());
		Assert.IsTrue(M.ApproxEquals(M + 1e-7f*Matrix3.Identity));
		Assert.IsFalse(M.ApproxEquals(M + 1e-3f*Matrix3.Identity));
		Assert.IsTrue(M.ApproxEquals(M + 1e-3f*Matrix3.Identity, 1e-2f));
	}

	[TestMethod]
	public void ArithmeticOperators()
	{
		Assert.AreEqual(M, +M);
		Assert.AreEqual(new Matrix3(-1f, -2f, -3f, -4f, -5f, -6f, -7f, -8f, -10f), -M);
		Assert.AreEqual(new Matrix3(2f, 2f, 3f, 4f, 6f, 6f, 7f, 8f, 11f), M + Matrix3.Identity);
		Assert.AreEqual(new Matrix3(0f, 2f, 3f, 4f, 4f, 6f, 7f, 8f, 9f), M - Matrix3.Identity);
		Assert.AreEqual(new Matrix3(2f, 4f, 6f, 8f, 10f, 12f, 14f, 16f, 20f), M*2f);
		Assert.AreEqual(M*2f, 2f*M);
	}

	[TestMethod]
	public void MatrixProduct_And_Concat()
	{
		Matrix3 n = Samples.GeneralMatrix3;
		Matrix3 product = M*n;
		for (int r = 0; r < 3; r++)
			for (int c = 0; c < 3; c++)
				AssertEx.AreEqual(Vector3.Dot(M[r], new Vector3(n[0, c], n[1, c], n[2, c])), product[r, c]);
		Assert.AreEqual(product, Matrix3.Concat(M, n));
		Matrix3 c1 = M;
		c1.Concat(n);
		Assert.AreEqual(product, c1);
		Matrix3 c2 = n;
		c2.PreConcat(M);
		Assert.AreEqual(product, c2);
		Assert.AreEqual(M, M*Matrix3.Identity);
		foreach (Vector3 v in Samples.Vectors)
			AssertEx.AreEqual((v*M)*n, v*(M*n), 1e-4f);
	}

	[TestMethod]
	public void Concat_ManyArguments_IsLeftToRightProduct()
	{
		Matrix3 a = Matrix3.Rotation(Axis.X, 0.3f);
		Matrix3 b = Matrix3.Scaling(new Vector3(1f, 2f, 0.5f));
		Matrix3 c = Matrix3.Rotation(Axis.Y, -0.7f);
		Matrix3 d = Matrix3.Shearing(0.1f, 0.2f, 0f, -0.1f, 0.3f, 0f);
		AssertEx.AreEqual(a*b*c, Matrix3.Concat(a, b, c));
		AssertEx.AreEqual(a*b*c*d, Matrix3.Concat(a, b, c, d));
		AssertEx.AreEqual(a*b*c*d*a, Matrix3.Concat(a, b, c, d, a));
		AssertEx.AreEqual(a*b*c*d*a*b, Matrix3.Concat(a, b, c, d, a, b));
		AssertEx.AreEqual(a*b*c*d*a*b*c, Matrix3.Concat(a, b, c, d, a, b, c));
		AssertEx.AreEqual(a*b*c*d*a*b*c*d, Matrix3.Concat(a, b, c, d, a, b, c, d));
	}

	[TestMethod]
	public void ScaleAndPreScale_MultiplyByDiagonalMatrix()
	{
		Vector3 s = new Vector3(2f, -3f, 0.5f);
		Matrix3 scaled = M;
		scaled.Scale(s);
		Assert.AreEqual(M*Matrix3.Scaling(s), scaled);
		Matrix3 prescaled = M;
		prescaled.PreScale(s);
		Assert.AreEqual(Matrix3.Scaling(s)*M, prescaled);
	}

	[TestMethod]
	public void Transpose_SwapsRowsAndColumns()
	{
		Matrix3 t = new Matrix3(1f, 4f, 7f, 2f, 5f, 8f, 3f, 6f, 10f);
		Assert.AreEqual(t, Matrix3.Transpose(M));
		Matrix3 m = M;
		m.Transpose();
		Assert.AreEqual(t, m);
	}

	[TestMethod]
	public void Determinant_Trace_IsFinite()
	{
		Assert.AreEqual(-3f, M.Determinant);
		Assert.AreEqual(16f, M.Trace);
		Assert.AreEqual(1f, Matrix3.Identity.Determinant);
		Assert.IsTrue(M.IsFinite);
		Assert.IsFalse(new Matrix3(1f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, Single.PositiveInfinity).IsFinite);
	}

	[TestMethod]
	public void Inverse_Adjoint_InverseTranspose()
	{
		foreach (Matrix3 m in new[] { M, Samples.GeneralMatrix3 })
		{
			Matrix3 inv = Matrix3.Inverse(m);
			AssertEx.AreEqual(Matrix3.Identity, m*inv, 1e-5f);
			AssertEx.AreEqual(Matrix3.Identity, inv*m, 1e-5f);
			AssertEx.AreEqual(m.Determinant*inv, Matrix3.Adjoint(m), 1e-4f);
			AssertEx.AreEqual(Matrix3.Transpose(inv), Matrix3.InverseTranspose(m), 1e-5f);
			Matrix3 inverted = m;
			inverted.Invert();
			AssertEx.AreEqual(inv, inverted);
		}
		AssertEx.AreEqual(new Matrix3(-2f/3f, -4f/3f, 1f, -2f/3f, 11f/3f, -2f, 1f, -2f, 1f), Matrix3.Inverse(M));
	}

	[TestMethod]
	public void Orthonormalize_ProducesOrthonormalBasis()
	{
		Matrix3 m = Samples.GeneralMatrix3;
		Matrix3 o = Matrix3.Orthonormalize(m);
		AssertEx.IsOrthonormal(o);
		AssertEx.AreEqual(Vector3.Normalize(m.Row0), o.Row0);
		AssertEx.AreEqual(0f, Vector3.Dot(Vector3.Cross(m.Row0, m.Row1), o.Row1)); // Row1 stays in span(Row0, Row1)
		m.Orthonormalize();
		AssertEx.AreEqual(o, m);
	}

	[TestMethod]
	public void ScaleComponent_ReturnsRowLengths()
	{
		Matrix3 m = Matrix3.ScalingRotation(new Vector3(2f, 3f, 4f), Samples.Axes[4], 1.2f);
		AssertEx.AreEqual(new Vector3(2f, 3f, 4f), m.ScaleComponent);
	}

	[TestMethod]
	public void Rotation_AboutPrincipalAxes_IsCounterClockwise()
	{
		float a = SingleConstants.HalfPi;
		AssertEx.AreEqual(Vector3.UnitZ, Vector3.UnitY*Matrix3.Rotation(Axis.X, a));
		AssertEx.AreEqual(Vector3.UnitX, Vector3.UnitZ*Matrix3.Rotation(Axis.Y, a));
		AssertEx.AreEqual(Vector3.UnitY, Vector3.UnitX*Matrix3.Rotation(Axis.Z, a));
		Assert.AreEqual(Matrix3.Identity, Matrix3.Rotation(Axis.X, 0f));
		Assert.AreEqual(Matrix3.Zero, Matrix3.Rotation(Axis.W, 1f));
	}

	[TestMethod]
	public void Rotation_AboutArbitraryAxis_MatchesPrincipalAxesAndPreservesAxis()
	{
		AssertEx.AreEqual(Matrix3.Rotation(Axis.X, 0.4f), Matrix3.Rotation(Vector3.UnitX, 0.4f));
		AssertEx.AreEqual(Matrix3.Rotation(Axis.Y, 0.4f), Matrix3.Rotation(Vector3.UnitY*3f, 0.4f));
		AssertEx.AreEqual(Matrix3.Rotation(Axis.Z, -1.4f), Matrix3.Rotation(Vector3.UnitZ, -1.4f));
		Assert.AreEqual(Matrix3.Identity, Matrix3.Rotation(Vector3.Zero, 1f));
		Assert.AreEqual(Matrix3.Identity, Matrix3.Rotation(Vector3.UnitX, 0f));
		for (int i = 0; i < Samples.Axes.Length; i++)
		{
			Matrix3 r = Matrix3.Rotation(Samples.Axes[i], Samples.Angles[i]);
			AssertEx.IsOrthonormal(r);
			AssertEx.AreEqual(1f, r.Determinant);
			AssertEx.AreEqual(Samples.Axes[i], Samples.Axes[i]*r);
		}
	}

	[TestMethod]
	public void Rotation_FromQuaternion_MatchesAxisAngle()
	{
		for (int i = 0; i < Samples.Axes.Length; i++)
		{
			Quaternion q = Quaternion.FromAxisAngle(Samples.Axes[i], Samples.Angles[i]);
			Matrix3 expected = Matrix3.Rotation(Samples.Axes[i], Samples.Angles[i]);
			AssertEx.AreEqual(expected, Matrix3.FromQuaternion(q));
			AssertEx.AreEqual(expected, Matrix3.Rotation(q));
			AssertEx.AreEqual(expected, (Matrix3)q);
			// Rotation(Quaternion) normalizes its input; FromQuaternion assumes a unit quaternion.
			AssertEx.AreEqual(expected, Matrix3.Rotation(q*3f));
		}
		Assert.AreEqual(Matrix3.Identity, Matrix3.FromQuaternion(Quaternion.Identity));
		Assert.AreEqual(Matrix3.Identity, Matrix3.Rotation(Quaternion.Identity));
	}

	[TestMethod]
	public void Rotation_FromEuler_AppliesAxesInNamedOrder()
	{
		Euler angles = new Euler(0.3f, -0.8f, 1.1f, EulerOrder.Unspecified);
		foreach (EulerOrder order in Orders)
		{
			Euler e = new Euler(angles, order);
			string name = order.ToString();
			Matrix3 expected = Matrix3.Concat(AxisRotation(name[0], e), AxisRotation(name[1], e), AxisRotation(name[2], e));
			AssertEx.AreEqual(expected, Matrix3.Rotation(e), 1e-5f, name);
			AssertEx.AreEqual(expected, Matrix3.FromEuler(e), 1e-5f, name);
		}
	}

	[TestMethod]
	public void Rotation_FromEuler_SpecialCases()
	{
		Assert.AreEqual(Matrix3.Identity, Matrix3.Rotation(Euler.ZeroXYZ));
		Assert.AreEqual(Matrix3.Identity, Matrix3.Rotation(new Euler(1f, 2f, 3f, EulerOrder.Unspecified)));
	}

	[TestMethod]
	public void Rotation_FromYawPitchRoll_IsRollPitchYaw()
	{
		YawPitchRoll ypr = new YawPitchRoll(0.7f, -0.4f, 1.3f);
		Matrix3 expected = Matrix3.Concat(Matrix3.Rotation(Axis.Z, ypr.Roll), Matrix3.Rotation(Axis.X, ypr.Pitch),
			Matrix3.Rotation(Axis.Y, ypr.Yaw));
		AssertEx.AreEqual(expected, Matrix3.Rotation(ypr));
		AssertEx.AreEqual(expected, Matrix3.FromYawPitchRoll(ypr));
		AssertEx.AreEqual(ypr.ToForward(), Matrix3.Rotation(ypr).Forward);
		Assert.AreEqual(Matrix3.Identity, Matrix3.Rotation(YawPitchRoll.Zero));
	}

	[TestMethod]
	public void Scaling_ScalingRotation_Shearing()
	{
		Assert.AreEqual(new Matrix3(2f, 0f, 0f, 0f, 3f, 0f, 0f, 0f, 4f), Matrix3.Scaling(new Vector3(2f, 3f, 4f)));
		Assert.AreEqual(new Matrix3(5f, 0f, 0f, 0f, 5f, 0f, 0f, 0f, 5f), Matrix3.Scaling(5f));
		Assert.AreEqual(new Matrix3(1f, 1f, 2f, 3f, 1f, 4f, 5f, 6f, 1f), Matrix3.Shearing(1f, 2f, 3f, 4f, 5f, 6f));

		Vector3 s = new Vector3(2f, 3f, 4f);
		Matrix3 scale = Matrix3.Scaling(s);
		YawPitchRoll ypr = new YawPitchRoll(0.1f, 0.2f, 0.3f);
		Euler e = new Euler(0.1f, 0.2f, 0.3f, EulerOrder.YXZ);
		Quaternion q = Quaternion.FromAxisAngle(Samples.Axes[3], 0.9f);
		AssertEx.AreEqual(scale*Matrix3.Rotation(Axis.Y, 0.5f), Matrix3.ScalingRotation(s, Axis.Y, 0.5f));
		AssertEx.AreEqual(scale*Matrix3.Rotation(Samples.Axes[4], 0.5f), Matrix3.ScalingRotation(s, Samples.Axes[4], 0.5f));
		AssertEx.AreEqual(scale*Matrix3.Rotation(ypr), Matrix3.ScalingRotation(s, ypr));
		AssertEx.AreEqual(scale*Matrix3.Rotation(e), Matrix3.ScalingRotation(s, e));
		AssertEx.AreEqual(scale*Matrix3.Rotation(q), Matrix3.ScalingRotation(s, q));
	}

	[TestMethod]
	public void FromForward_BuildsRightHandedBasisWithForwardRow()
	{
		Vector3[] forwards =
		{
			Vector3.UnitZ, -Vector3.UnitZ, Vector3.UnitX, new Vector3(1f, 2f, 3f), new Vector3(-4f, -1f, 0.5f),
			Vector3.UnitY, -Vector3.UnitY, new Vector3(0f, 5f, 0f),
		};
		foreach (Vector3 f in forwards)
		{
			Matrix3 m = Matrix3.FromForward(f);
			AssertEx.IsOrthonormal(m);
			AssertEx.AreEqual(1f, m.Determinant);
			AssertEx.AreEqual(Vector3.Normalize(f), m.Forward);
		}
		Assert.AreEqual(Matrix3.Identity, Matrix3.FromForward(Vector3.UnitZ));
		Assert.AreEqual(Matrix3.Identity, Matrix3.FromForward(Vector3.Zero));
		// For non-vertical forward vectors the right axis stays horizontal.
		AssertEx.AreEqual(0f, Matrix3.FromForward(new Vector3(1f, 2f, 3f)).Right.Y);
	}

	[TestMethod]
	public void FromUp_BuildsRightHandedBasisWithUpRow()
	{
		Vector3[] ups = { Vector3.UnitY, -Vector3.UnitY, Vector3.UnitX, new Vector3(1f, 2f, 3f), new Vector3(0.2f, -3f, 1f) };
		foreach (Vector3 u in ups)
		{
			Matrix3 m = Matrix3.FromUp(u);
			AssertEx.IsOrthonormal(m);
			AssertEx.AreEqual(1f, m.Determinant);
			AssertEx.AreEqual(Vector3.Normalize(u), m.Up);
		}
		Assert.AreEqual(Matrix3.Identity, Matrix3.FromUp(Vector3.UnitY));
		Assert.AreEqual(Matrix3.Identity, Matrix3.FromUp(Vector3.Zero));
	}

	[TestMethod]
	public void FromForwardAndUp_KeepsForwardAndUpInSamePlane()
	{
		Vector3 f = new Vector3(1f, 0.5f, 2f);
		Vector3 u = new Vector3(0.1f, 1f, -0.2f);
		Matrix3 m = Matrix3.FromForwardAndUp(f, u);
		AssertEx.IsOrthonormal(m);
		AssertEx.AreEqual(1f, m.Determinant);
		AssertEx.AreEqual(Vector3.Normalize(f), m.Forward);
		AssertEx.AreEqual(0f, Vector3.Dot(m.Up, Vector3.Cross(f, u)));
		Assert.IsTrue(Vector3.Dot(m.Up, u) > 0f);
		AssertEx.AreEqual(Matrix3.Identity, Matrix3.FromForwardAndUp(Vector3.UnitZ, Vector3.UnitY));
		Assert.AreEqual(Matrix3.Identity, Matrix3.FromForwardAndUp(Vector3.Zero, Vector3.UnitY));
		Assert.AreEqual(Matrix3.Identity, Matrix3.FromForwardAndUp(Vector3.UnitZ, Vector3.Zero));
	}

	[TestMethod]
	public void FromRightAndUp_KeepsRightAndUpInSamePlane()
	{
		Vector3 r = new Vector3(2f, 0.1f, -0.3f);
		Vector3 u = new Vector3(0.2f, 1f, 0.4f);
		Matrix3 m = Matrix3.FromRightAndUp(r, u);
		AssertEx.IsOrthonormal(m);
		AssertEx.AreEqual(1f, m.Determinant);
		AssertEx.AreEqual(Vector3.Normalize(r), m.Right);
		AssertEx.AreEqual(0f, Vector3.Dot(m.Up, Vector3.Cross(r, u)));
		Assert.IsTrue(Vector3.Dot(m.Up, u) > 0f);
		AssertEx.AreEqual(Matrix3.Identity, Matrix3.FromRightAndUp(Vector3.UnitX, Vector3.UnitY));
		Assert.AreEqual(Matrix3.Identity, Matrix3.FromRightAndUp(Vector3.Zero, Vector3.UnitY));
	}

	[TestMethod]
	public void ToArray_ToString_Parse()
	{
		CollectionAssert.AreEqual(new[] { 1f, 2f, 3f, 4f, 5f, 6f, 7f, 8f, 10f }, M.ToArray());
		Matrix3 m = new Matrix3(1.5f, -2f, 0f, 4f, 5f, 6f, 7f, 8f, 9.25f);
		const string text = "1.5 -2 0 4 5 6 7 8 9.25";
		Assert.AreEqual(text, m.ToString(CultureInfo.InvariantCulture));
		Assert.AreEqual("1.50 -2.00 0.00 4.00 5.00 6.00 7.00 8.00 9.25", m.ToString("F2", CultureInfo.InvariantCulture));
		Assert.AreEqual(m, Matrix3.Parse(text, CultureInfo.InvariantCulture));
		using (CultureScope.Invariant())
		{
			Assert.AreEqual(text, m.ToString());
			Assert.AreEqual("1.50 -2.00 0.00 4.00 5.00 6.00 7.00 8.00 9.25", m.ToString("F2"));
			Assert.AreEqual(m, Matrix3.Parse(text));
		}
		Assert.ThrowsExactly<ArgumentNullException>(() => Matrix3.Parse(null));
		Assert.ThrowsExactly<FormatException>(() => Matrix3.Parse("1 2 3 4 5 6 7 8", CultureInfo.InvariantCulture));
	}
}
