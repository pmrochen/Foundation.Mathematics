using System.Globalization;

namespace Foundation.Mathematics.Tests;

[TestClass]
public sealed class Matrix4Tests
{
	private static readonly Matrix4 M = Samples.GeneralMatrix4;

	private static Vector3 TransformPoint(Vector3 p, in Matrix4 m)
	{
		Vector4 r = new Vector4(p, 1f)*m;
		return r.XYZ/r.W;
	}

	[TestMethod]
	public void StaticFields_HaveExpectedValues()
	{
		for (int r = 0; r < 4; r++)
		{
			for (int c = 0; c < 4; c++)
			{
				Assert.AreEqual(0f, Matrix4.Zero[r, c]);
				Assert.AreEqual((r == c) ? 1f : 0f, Matrix4.Identity[r, c]);
			}
		}
	}

	[TestMethod]
	public void Constructors_And_ElementAccess()
	{
		float[] values = Enumerable.Range(1, 16).Select(i => (float)i).ToArray();
		Matrix4 m = new Matrix4(values);
		Assert.AreEqual(m, new Matrix4(1f, 2f, 3f, 4f, 5f, 6f, 7f, 8f, 9f, 10f, 11f, 12f, 13f, 14f, 15f, 16f));
		Assert.AreEqual(m, new Matrix4(new Vector4(1f, 2f, 3f, 4f), new Vector4(5f, 6f, 7f, 8f),
			new Vector4(9f, 10f, 11f, 12f), new Vector4(13f, 14f, 15f, 16f)));
		float[] actual =
		{
			m.M00, m.M01, m.M02, m.M03, m.M10, m.M11, m.M12, m.M13,
			m.M20, m.M21, m.M22, m.M23, m.M30, m.M31, m.M32, m.M33,
		};
		CollectionAssert.AreEqual(values, actual);
		CollectionAssert.AreEqual(values, m.ToArray());
		for (int r = 0; r < 4; r++)
			for (int c = 0; c < 4; c++)
				Assert.AreEqual(values[4*r + c], m[r, c]);
		Assert.AreEqual(new Vector4(1f, 2f, 3f, 4f), m.Row0);
		Assert.AreEqual(new Vector4(5f, 6f, 7f, 8f), m.Row1);
		Assert.AreEqual(new Vector4(9f, 10f, 11f, 12f), m.Row2);
		Assert.AreEqual(new Vector4(13f, 14f, 15f, 16f), m.Row3);
		Assert.AreEqual(m.Row2, m[2]);
		Assert.AreEqual(new Vector4(1f, 5f, 9f, 13f), m.Column0);
		Assert.AreEqual(new Vector4(2f, 6f, 10f, 14f), m.Column1);
		Assert.AreEqual(new Vector4(3f, 7f, 11f, 15f), m.Column2);
		Assert.AreEqual(new Vector4(4f, 8f, 12f, 16f), m.Column3);
	}

	[TestMethod]
	public void Setters_ModifyElements()
	{
		Matrix4 m = Matrix4.Zero;
		m.M00 = 1f; m.M11 = 1f; m.M22 = 1f; m.M33 = 1f;
		Assert.AreEqual(Matrix4.Identity, m);
		m.M01 = 2f; m.M02 = 3f; m.M03 = 4f;
		m.M10 = 5f; m.M12 = 6f; m.M13 = 7f;
		m.M20 = 8f; m.M21 = 9f; m.M23 = 10f;
		m.M30 = 11f; m.M31 = 12f; m.M32 = 13f;
		Assert.AreEqual(new Matrix4(1f, 2f, 3f, 4f, 5f, 1f, 6f, 7f, 8f, 9f, 1f, 10f, 11f, 12f, 13f, 1f), m);
		m.Row0 = new Vector4(-1f);
		m.Row1 = new Vector4(-2f);
		m.Row2 = new Vector4(-3f);
		m.Row3 = new Vector4(-4f);
		m[3] = new Vector4(0f, 0f, 0f, 1f);
		m[0, 0] = 9f;
		Assert.AreEqual(new Matrix4(9f, -1f, -1f, -1f, -2f, -2f, -2f, -2f, -3f, -3f, -3f, -3f, 0f, 0f, 0f, 1f), m);
	}

	[TestMethod]
	public void Indexers_OutOfRange_Throw()
	{
		Matrix4 m = M;
		Assert.ThrowsExactly<IndexOutOfRangeException>(() => _ = m[4]);
		Assert.ThrowsExactly<IndexOutOfRangeException>(() => _ = m[0, 4]);
		Assert.ThrowsExactly<IndexOutOfRangeException>(() => _ = m[4, 0]);
		Assert.ThrowsExactly<IndexOutOfRangeException>(() => m[-1] = Vector4.Zero);
		Assert.ThrowsExactly<IndexOutOfRangeException>(() => m[0, -1] = 0f);
	}

	[TestMethod]
	public void ImplicitConversions_EmbedSmallerMatrices()
	{
		Matrix4 a = new Matrix2(1f, 2f, 3f, 4f);
		Assert.AreEqual(new Matrix4(1f, 2f, 0f, 0f, 3f, 4f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f), a);
		Matrix4 b = new Matrix3(1f, 2f, 3f, 4f, 5f, 6f, 7f, 8f, 9f);
		Assert.AreEqual(new Matrix4(1f, 2f, 3f, 0f, 4f, 5f, 6f, 0f, 7f, 8f, 9f, 0f, 0f, 0f, 0f, 1f), b);
		AffineTransform t = new AffineTransform(new Matrix3(1f, 2f, 3f, 4f, 5f, 6f, 7f, 8f, 9f), new Vector3(10f, 11f, 12f));
		Matrix4 expected = new Matrix4(1f, 2f, 3f, 0f, 4f, 5f, 6f, 0f, 7f, 8f, 9f, 0f, 10f, 11f, 12f, 1f);
		Matrix4 c = t;
		Assert.AreEqual(expected, c);
		Assert.AreEqual(expected, Matrix4.FromAffineTransform(t));
	}

	[TestMethod]
	public void Equality_ApproxEquals_HashCode()
	{
		Matrix4 same = new Matrix4(M.ToArray());
		Assert.IsTrue(M == same);
		Assert.IsFalse(M != same);
		Assert.IsTrue(M != Matrix4.Identity);
		Assert.IsTrue(M.Equals((object)same));
		Assert.IsFalse(M.Equals(null));
		Assert.AreEqual(M.GetHashCode(), same.GetHashCode());
		Matrix4 nudged = M;
		nudged[3, 3] += 1e-3f;
		Assert.IsFalse(M.ApproxEquals(nudged));
		Assert.IsTrue(M.ApproxEquals(nudged, 1e-2f));
		Assert.IsTrue(M.ApproxEquals(same));
	}

	[TestMethod]
	public void ArithmeticOperators()
	{
		Assert.AreEqual(M, +M);
		Matrix4 neg = -M;
		Matrix4 sum = M + Matrix4.Identity;
		Matrix4 diff = M - Matrix4.Identity;
		Matrix4 scaled = M*2f;
		Matrix4 scaled2 = 2f*M;
		for (int r = 0; r < 4; r++)
		{
			for (int c = 0; c < 4; c++)
			{
				float id = (r == c) ? 1f : 0f;
				Assert.AreEqual(-M[r, c], neg[r, c]);
				Assert.AreEqual(M[r, c] + id, sum[r, c]);
				Assert.AreEqual(M[r, c] - id, diff[r, c]);
				Assert.AreEqual(2f*M[r, c], scaled[r, c]);
				Assert.AreEqual(2f*M[r, c], scaled2[r, c]);
			}
		}
	}

	[TestMethod]
	public void MatrixProduct_And_Concat()
	{
		Matrix4 n = Matrix4.RotationTranslation(Samples.Axes[3], 0.6f, new Vector3(1f, -2f, 3f));
		Matrix4 product = M*n;
		for (int r = 0; r < 4; r++)
			for (int c = 0; c < 4; c++)
				AssertEx.AreEqual(Vector4.Dot(M[r], new Vector4(n[0, c], n[1, c], n[2, c], n[3, c])), product[r, c]);
		Assert.AreEqual(product, Matrix4.Concat(M, n));
		Matrix4 c1 = M;
		c1.Concat(n);
		Assert.AreEqual(product, c1);
		Matrix4 c2 = n;
		c2.PreConcat(M);
		Assert.AreEqual(product, c2);
		Assert.AreEqual(M, M*Matrix4.Identity);
		Vector4 v = new Vector4(1f, 2f, 3f, 1f);
		AssertEx.AreEqual((v*M)*n, v*(M*n), 1e-4f);
	}

	[TestMethod]
	public void Concat_ManyArguments_IsLeftToRightProduct()
	{
		Matrix4 a = Matrix4.Rotation(Axis.X, 0.3f);
		Matrix4 b = Matrix4.Translation(new Vector3(1f, 2f, 3f));
		Matrix4 c = Matrix4.Scaling(new Vector3(1f, 2f, 0.5f));
		Matrix4 d = Matrix4.Rotation(Axis.Z, -0.2f);
		AssertEx.AreEqual(a*b*c, Matrix4.Concat(a, b, c));
		AssertEx.AreEqual(a*b*c*d, Matrix4.Concat(a, b, c, d));
		AssertEx.AreEqual(a*b*c*d*a, Matrix4.Concat(a, b, c, d, a));
		AssertEx.AreEqual(a*b*c*d*a*b, Matrix4.Concat(a, b, c, d, a, b), 1e-4f);
		AssertEx.AreEqual(a*b*c*d*a*b*c, Matrix4.Concat(a, b, c, d, a, b, c), 1e-4f);
		AssertEx.AreEqual(a*b*c*d*a*b*c*d, Matrix4.Concat(a, b, c, d, a, b, c, d), 1e-4f);
	}

	[TestMethod]
	public void Scale_PreScale_Translate()
	{
		Vector3 s = new Vector3(2f, -3f, 0.5f);
		Matrix4 scaled = M;
		scaled.Scale(s);
		AssertEx.AreEqual(M*Matrix4.Scaling(s), scaled);
		Matrix4 prescaled = M;
		prescaled.PreScale(s);
		AssertEx.AreEqual(Matrix4.Scaling(s)*M, prescaled);
		Vector3 t = new Vector3(1f, 2f, -3f);
		Matrix4 translated = M;
		translated.Translate(t);
		AssertEx.AreEqual(M*Matrix4.Translation(t), translated);
	}

	[TestMethod]
	public void Translation_MovesPointsButNotDirections()
	{
		Matrix4 t = Matrix4.Translation(new Vector3(1f, 2f, 3f));
		Assert.AreEqual(new Matrix4(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, 1f, 2f, 3f, 1f), t);
		Assert.AreEqual(new Vector3(2f, 2f, 3f), TransformPoint(Vector3.UnitX, t));
		Assert.AreEqual(new Vector4(1f, 0f, 0f, 0f), new Vector4(1f, 0f, 0f, 0f)*t);
	}

	[TestMethod]
	public void Transpose_SwapsRowsAndColumns()
	{
		Matrix4 t = Matrix4.Transpose(M);
		for (int r = 0; r < 4; r++)
			for (int c = 0; c < 4; c++)
				Assert.AreEqual(M[c, r], t[r, c]);
		Matrix4 m = M;
		m.Transpose();
		Assert.AreEqual(t, m);
	}

	[TestMethod]
	public void Rotations_EmbedMatrix3Rotations()
	{
		YawPitchRoll ypr = new YawPitchRoll(0.3f, 0.2f, -0.6f);
		Euler e = new Euler(0.4f, -0.1f, 0.9f, EulerOrder.ZYX);
		Quaternion q = Quaternion.FromAxisAngle(Samples.Axes[4], 1.3f);
		Assert.AreEqual((Matrix4)Matrix3.Rotation(Axis.Y, 0.5f), Matrix4.Rotation(Axis.Y, 0.5f));
		Assert.AreEqual((Matrix4)Matrix3.Rotation(Samples.Axes[3], 0.5f), Matrix4.Rotation(Samples.Axes[3], 0.5f));
		Assert.AreEqual((Matrix4)Matrix3.Rotation(ypr), Matrix4.Rotation(ypr));
		Assert.AreEqual((Matrix4)Matrix3.Rotation(e), Matrix4.Rotation(e));
		Assert.AreEqual((Matrix4)Matrix3.Rotation(q), Matrix4.Rotation(q));
	}

	[TestMethod]
	public void Scaling_ProducesDiagonalMatrix()
	{
		Assert.AreEqual(new Matrix4(2f, 0f, 0f, 0f, 0f, 3f, 0f, 0f, 0f, 0f, 4f, 0f, 0f, 0f, 0f, 1f), Matrix4.Scaling(new Vector3(2f, 3f, 4f)));
		Assert.AreEqual(new Matrix4(5f, 0f, 0f, 0f, 0f, 5f, 0f, 0f, 0f, 0f, 5f, 0f, 0f, 0f, 0f, 1f), Matrix4.Scaling(5f));
		Assert.AreEqual(new Matrix4(1f, 1f, 2f, 0f, 3f, 1f, 4f, 0f, 5f, 6f, 1f, 0f, 0f, 0f, 0f, 1f), Matrix4.Shearing(1f, 2f, 3f, 4f, 5f, 6f));
	}

	[TestMethod]
	public void CompositeFactories_MatchConcatenationOfParts()
	{
		Vector3 s = new Vector3(2f, 3f, 0.5f);
		Vector3 t = new Vector3(-1f, 4f, 2f);
		Vector3 axis = Samples.Axes[4];
		YawPitchRoll ypr = new YawPitchRoll(0.3f, 0.2f, -0.6f);
		Euler e = new Euler(0.4f, -0.1f, 0.9f, EulerOrder.XZY);
		Quaternion q = Quaternion.FromAxisAngle(Samples.Axes[3], 0.8f);
		Matrix4 S = Matrix4.Scaling(s);
		Matrix4 T = Matrix4.Translation(t);

		AssertEx.AreEqual(Matrix4.Rotation(Axis.Z, 0.7f)*T, Matrix4.RotationTranslation(Axis.Z, 0.7f, t));
		AssertEx.AreEqual(Matrix4.Rotation(axis, 0.7f)*T, Matrix4.RotationTranslation(axis, 0.7f, t));
		AssertEx.AreEqual(Matrix4.Rotation(ypr)*T, Matrix4.RotationTranslation(ypr, t));
		AssertEx.AreEqual(Matrix4.Rotation(e)*T, Matrix4.RotationTranslation(e, t));
		AssertEx.AreEqual(Matrix4.Rotation(q)*T, Matrix4.RotationTranslation(q, t));

		AssertEx.AreEqual(S*Matrix4.Rotation(Axis.Z, 0.7f), Matrix4.ScalingRotation(s, Axis.Z, 0.7f));
		AssertEx.AreEqual(S*Matrix4.Rotation(axis, 0.7f), Matrix4.ScalingRotation(s, axis, 0.7f));
		AssertEx.AreEqual(S*Matrix4.Rotation(ypr), Matrix4.ScalingRotation(s, ypr));
		AssertEx.AreEqual(S*Matrix4.Rotation(e), Matrix4.ScalingRotation(s, e));
		AssertEx.AreEqual(S*Matrix4.Rotation(q), Matrix4.ScalingRotation(s, q));

		AssertEx.AreEqual(S*T, Matrix4.ScalingTranslation(s, t));
		AssertEx.AreEqual(S*Matrix4.Rotation(Axis.Z, 0.7f)*T, Matrix4.ScalingRotationTranslation(s, Axis.Z, 0.7f, t));
		AssertEx.AreEqual(S*Matrix4.Rotation(axis, 0.7f)*T, Matrix4.ScalingRotationTranslation(s, axis, 0.7f, t));
		AssertEx.AreEqual(S*Matrix4.Rotation(ypr)*T, Matrix4.ScalingRotationTranslation(s, ypr, t));
		AssertEx.AreEqual(S*Matrix4.Rotation(e)*T, Matrix4.ScalingRotationTranslation(s, e, t));
		AssertEx.AreEqual(S*Matrix4.Rotation(q)*T, Matrix4.ScalingRotationTranslation(s, q, t));
	}

	[TestMethod]
	public void Reflection_MirrorsPointsAcrossPlane()
	{
		// Plane y = 2 with an unnormalized normal.
		Plane plane = new Plane(new Vector3(0f, 3f, 0f), new Vector3(0f, 2f, 0f));
		Matrix4 r = Matrix4.Reflection(plane);
		AssertEx.AreEqual(new Vector3(1f, 3f, -2f), TransformPoint(new Vector3(1f, 1f, -2f), r));
		AssertEx.AreEqual(new Vector3(5f, 2f, 5f), TransformPoint(new Vector3(5f, 2f, 5f), r));
		AssertEx.AreEqual(-1f, r.Determinant);
		AssertEx.AreEqual(Matrix4.Identity, r*r);

		Plane oblique = Plane.Normalize(new Plane(1f, 1f, 1f, -3f));
		Matrix4 ro = Matrix4.Reflection(oblique);
		foreach (Vector3 p in Samples.Vectors)
		{
			Vector3 q = TransformPoint(p, ro);
			AssertEx.AreEqual(-oblique.GetSignedDistanceTo(p), oblique.GetSignedDistanceTo(q), 1e-4f);
			AssertEx.AreEqual(oblique.Reflect(p), q, 1e-4f);
		}
	}

	[TestMethod]
	public void Determinant_Trace_IsFinite()
	{
		Assert.AreEqual(1f, Matrix4.Identity.Determinant);
		Assert.AreEqual(24f, Matrix4.Scaling(new Vector3(2f, 3f, 4f)).Determinant);
		Matrix4 lower = new Matrix4(2f, 0f, 0f, 0f, 1f, 3f, 0f, 0f, 4f, 5f, 6f, 0f, 7f, 8f, 9f, 10f);
		AssertEx.AreEqual(360f, lower.Determinant);
		Matrix4 singular = new Matrix4(1f, 2f, 3f, 4f, 5f, 6f, 7f, 8f, 9f, 10f, 11f, 12f, 13f, 14f, 15f, 16f);
		AssertEx.AreEqual(0f, singular.Determinant, 1e-3f);
		AssertEx.AreEqual(M.Determinant, Matrix4.Transpose(M).Determinant, 1e-4f);
		AssertEx.AreEqual(7.7f, M.Trace);
		Assert.IsTrue(M.IsFinite);
		Matrix4 bad = M;
		bad[2, 3] = Single.NaN;
		Assert.IsFalse(bad.IsFinite);
	}

	[TestMethod]
	public void Inverse_Adjoint_InverseTranspose()
	{
		Matrix4 inv = Matrix4.Inverse(M);
		AssertEx.AreEqual(Matrix4.Identity, M*inv, 1e-5f);
		AssertEx.AreEqual(Matrix4.Identity, inv*M, 1e-5f);
		AssertEx.AreEqual(M.Determinant*inv, Matrix4.Adjoint(M), 1e-4f);
		AssertEx.AreEqual(Matrix4.Transpose(inv), Matrix4.InverseTranspose(M));
		Matrix4 m = M;
		m.Invert();
		AssertEx.AreEqual(inv, m);

		Matrix4 srt = Matrix4.ScalingRotationTranslation(new Vector3(2f, 3f, 4f), Samples.Axes[3], 0.5f, new Vector3(1f, 2f, 3f));
		Vector3 p = new Vector3(-1f, 0.5f, 2f);
		AssertEx.AreEqual(p, TransformPoint(TransformPoint(p, srt), Matrix4.Inverse(srt)), 1e-5f);
	}

	[TestMethod]
	public void ToString_Parse()
	{
		Matrix4 m = new Matrix4(1.5f, -2f, 0f, 4f, 5f, 6f, 7f, 8f, 9f, 10f, 11f, 12f, 13f, 14f, 15f, 16.25f);
		const string text = "1.5 -2 0 4 5 6 7 8 9 10 11 12 13 14 15 16.25";
		Assert.AreEqual(text, m.ToString(CultureInfo.InvariantCulture));
		StringAssert.StartsWith(m.ToString("F1", CultureInfo.InvariantCulture), "1.5 -2.0 0.0 4.0 ");
		Assert.AreEqual(m, Matrix4.Parse(text, CultureInfo.InvariantCulture));
		using (CultureScope.Invariant())
		{
			Assert.AreEqual(text, m.ToString());
			StringAssert.StartsWith(m.ToString("F1"), "1.5 -2.0 0.0 4.0 ");
			Assert.AreEqual(m, Matrix4.Parse(text));
		}
		Assert.ThrowsExactly<ArgumentNullException>(() => Matrix4.Parse(null));
		Assert.ThrowsExactly<FormatException>(() => Matrix4.Parse("1 2 3", CultureInfo.InvariantCulture));
	}
}
