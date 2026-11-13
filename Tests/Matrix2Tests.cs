using System.Globalization;

namespace Foundation.Mathematics.Tests;

[TestClass]
public sealed class Matrix2Tests
{
	private static readonly Matrix2 M = new Matrix2(1f, 2f, 3f, 4f);
	private static readonly Matrix2 N = new Matrix2(-2f, 0.5f, 1f, 3f);

	[TestMethod]
	public void StaticFields_HaveExpectedValues()
	{
		Assert.AreEqual(new Matrix2(0f, 0f, 0f, 0f), Matrix2.Zero);
		Assert.AreEqual(new Matrix2(1f, 0f, 0f, 1f), Matrix2.Identity);
	}

	[TestMethod]
	public void Constructors_And_ElementAccess()
	{
		Assert.AreEqual(1f, M.M00);
		Assert.AreEqual(2f, M.M01);
		Assert.AreEqual(3f, M.M10);
		Assert.AreEqual(4f, M.M11);
		Assert.AreEqual(M, new Matrix2(new Vector2(1f, 2f), new Vector2(3f, 4f)));
		Assert.AreEqual(M, new Matrix2(new[] { 1f, 2f, 3f, 4f }));
		Assert.AreEqual(new Vector2(1f, 2f), M.Row0);
		Assert.AreEqual(new Vector2(3f, 4f), M.Row1);
		Assert.AreEqual(new Vector2(1f, 3f), M.Column0);
		Assert.AreEqual(new Vector2(2f, 4f), M.Column1);
		Assert.AreEqual(new Vector2(3f, 4f), M[1]);
		Assert.AreEqual(2f, M[0, 1]);
		Assert.AreEqual(3f, M[1, 0]);
	}

	[TestMethod]
	public void Setters_ModifyElements()
	{
		Matrix2 m = Matrix2.Zero;
		m.M00 = 1f;
		m.M01 = 2f;
		m.M10 = 3f;
		m.M11 = 4f;
		Assert.AreEqual(M, m);
		m.Row0 = new Vector2(5f, 6f);
		m[1] = new Vector2(7f, 8f);
		Assert.AreEqual(new Matrix2(5f, 6f, 7f, 8f), m);
		m[0, 1] = -1f;
		m[1, 0] = -2f;
		Assert.AreEqual(new Matrix2(5f, -1f, -2f, 8f), m);
		m.Row1 = Vector2.Zero;
		Assert.AreEqual(new Matrix2(5f, -1f, 0f, 0f), m);
	}

	[TestMethod]
	public void Indexers_OutOfRange_Throw()
	{
		Matrix2 m = M;
		Assert.ThrowsExactly<IndexOutOfRangeException>(() => _ = m[2]);
		Assert.ThrowsExactly<IndexOutOfRangeException>(() => _ = m[0, 2]);
		Assert.ThrowsExactly<IndexOutOfRangeException>(() => _ = m[2, 0]);
		Assert.ThrowsExactly<IndexOutOfRangeException>(() => m[-1] = Vector2.Zero);
		Assert.ThrowsExactly<IndexOutOfRangeException>(() => m[0, -1] = 0f);
	}

	[TestMethod]
	public void Equality_ApproxEquals_HashCode()
	{
		Assert.IsTrue(M == new Matrix2(1f, 2f, 3f, 4f));
		Assert.IsTrue(M != N);
		Assert.IsTrue(M.Equals((object)new Matrix2(1f, 2f, 3f, 4f)));
		Assert.IsFalse(M.Equals((object)Vector4.Zero));
		Assert.AreEqual(M.GetHashCode(), new Matrix2(1f, 2f, 3f, 4f).GetHashCode());
		Assert.IsTrue(M.ApproxEquals(new Matrix2(1f, 2f, 3f, 4f + 1e-7f)));
		Assert.IsFalse(M.ApproxEquals(new Matrix2(1f, 2f, 3f, 4.01f)));
		Assert.IsTrue(M.ApproxEquals(new Matrix2(1f, 2f, 3f, 4.01f), 0.1f));
	}

	[TestMethod]
	public void ArithmeticOperators()
	{
		Assert.AreEqual(M, +M);
		Assert.AreEqual(new Matrix2(-1f, -2f, -3f, -4f), -M);
		Assert.AreEqual(new Matrix2(-1f, 2.5f, 4f, 7f), M + N);
		Assert.AreEqual(new Matrix2(3f, 1.5f, 2f, 1f), M - N);
		Assert.AreEqual(new Matrix2(2f, 4f, 6f, 8f), M*2f);
		Assert.AreEqual(new Matrix2(2f, 4f, 6f, 8f), 2f*M);
	}

	[TestMethod]
	public void MatrixProduct_And_Concat()
	{
		// [1 2; 3 4] * [-2 0.5; 1 3] = [0 6.5; -2 13.5]
		Matrix2 expected = new Matrix2(0f, 6.5f, -2f, 13.5f);
		Assert.AreEqual(expected, M*N);
		Assert.AreEqual(expected, Matrix2.Concat(M, N));
		Matrix2 c = M;
		c.Concat(N);
		Assert.AreEqual(expected, c);
		Matrix2 p = N;
		p.PreConcat(M);
		Assert.AreEqual(expected, p);
		Assert.AreEqual(M, Matrix2.Concat(M, Matrix2.Identity));
		Assert.AreEqual(M, Matrix2.Concat(Matrix2.Identity, M));

		// Vector transformation is consistent with concatenation: v*(A*B) == (v*A)*B
		Vector2 v = new Vector2(0.5f, -2f);
		AssertEx.AreEqual((v*M)*N, v*(M*N));
	}

	[TestMethod]
	public void Concat_ManyArguments_IsLeftToRightProduct()
	{
		Matrix2 a = Matrix2.Rotation(0.1f);
		Matrix2 b = Matrix2.Scaling(new Vector2(2f, 3f));
		Matrix2 c = Matrix2.Shearing(0.5f, -0.25f);
		Matrix2 d = Matrix2.Rotation(-0.4f);
		Assert.AreEqual(a*b*c, Matrix2.Concat(a, b, c));
		Assert.AreEqual(a*b*c*d, Matrix2.Concat(a, b, c, d));
		Assert.AreEqual(a*b*c*d*a, Matrix2.Concat(a, b, c, d, a));
		Assert.AreEqual(a*b*c*d*a*b, Matrix2.Concat(a, b, c, d, a, b));
		Assert.AreEqual(a*b*c*d*a*b*c, Matrix2.Concat(a, b, c, d, a, b, c));
		Assert.AreEqual(a*b*c*d*a*b*c*d, Matrix2.Concat(a, b, c, d, a, b, c, d));
	}

	[TestMethod]
	public void ScaleAndPreScale_MultiplyByDiagonalMatrix()
	{
		Vector2 s = new Vector2(2f, -3f);
		Matrix2 scaled = M;
		scaled.Scale(s);
		Assert.AreEqual(M*Matrix2.Scaling(s), scaled);
		Matrix2 prescaled = M;
		prescaled.PreScale(s);
		Assert.AreEqual(Matrix2.Scaling(s)*M, prescaled);
	}

	[TestMethod]
	public void Transpose_SwapsRowsAndColumns()
	{
		Assert.AreEqual(new Matrix2(1f, 3f, 2f, 4f), Matrix2.Transpose(M));
		Matrix2 m = M;
		m.Transpose();
		Assert.AreEqual(new Matrix2(1f, 3f, 2f, 4f), m);
		Assert.AreEqual(M, Matrix2.Transpose(Matrix2.Transpose(M)));
	}

	[TestMethod]
	public void Rotation_IsCounterClockwiseAndOrthonormal()
	{
		Matrix2 r = Matrix2.Rotation(SingleConstants.HalfPi);
		AssertEx.AreEqual(Vector2.UnitY, Vector2.UnitX*r);
		Assert.AreEqual(Matrix2.Identity, Matrix2.Rotation(0f));
		foreach (float angle in Samples.Angles)
		{
			Matrix2 m = Matrix2.Rotation(angle);
			AssertEx.AreEqual(1f, m.Determinant);
			AssertEx.AreEqual(Matrix2.Identity, m*Matrix2.Transpose(m));
		}
	}

	[TestMethod]
	public void Scaling_And_Shearing()
	{
		Assert.AreEqual(new Matrix2(2f, 0f, 0f, 3f), Matrix2.Scaling(new Vector2(2f, 3f)));
		Assert.AreEqual(new Matrix2(5f, 0f, 0f, 5f), Matrix2.Scaling(5f));
		Assert.AreEqual(new Matrix2(1f, 0.5f, -0.25f, 1f), Matrix2.Shearing(0.5f, -0.25f));
		Vector2 s = new Vector2(2f, 3f);
		AssertEx.AreEqual(Matrix2.Scaling(s)*Matrix2.Rotation(0.7f), Matrix2.ScalingRotation(s, 0.7f));
	}

	[TestMethod]
	public void Determinant_Trace_IsFinite_ScaleComponent()
	{
		Assert.AreEqual(-2f, M.Determinant);
		Assert.AreEqual(5f, M.Trace);
		Assert.IsTrue(M.IsFinite);
		Assert.IsFalse(new Matrix2(1f, Single.NaN, 0f, 1f).IsFinite);
		AssertEx.AreEqual(new Vector2(2f, 3f), Matrix2.ScalingRotation(new Vector2(2f, 3f), 0.3f).ScaleComponent);
	}

	[TestMethod]
	public void Inverse_And_Adjoint()
	{
		Matrix2 inv = Matrix2.Inverse(M);
		AssertEx.AreEqual(new Matrix2(-2f, 1f, 1.5f, -0.5f), inv);
		AssertEx.AreEqual(Matrix2.Identity, M*inv);
		AssertEx.AreEqual(Matrix2.Identity, inv*M);
		AssertEx.AreEqual(M.Determinant*inv, Matrix2.Adjoint(M));
		Matrix2 m = M;
		m.Invert();
		AssertEx.AreEqual(inv, m);
	}

	[TestMethod]
	public void InverseTranspose_IsTransposeOfInverse()
	{
		AssertEx.AreEqual(Matrix2.Transpose(Matrix2.Inverse(M)), Matrix2.InverseTranspose(M));
		AssertEx.AreEqual(Matrix2.Transpose(Matrix2.Inverse(N)), Matrix2.InverseTranspose(N));
	}

	[TestMethod]
	public void Orthonormalize_ProducesRotationPreservingFirstRow()
	{
		Matrix2 m = new Matrix2(3f, 4f, 1f, 2f);
		Matrix2 o = Matrix2.Orthonormalize(m);
		AssertEx.AreEqual(new Vector2(0.6f, 0.8f), o.Row0);
		AssertEx.AreEqual(Matrix2.Identity, o*Matrix2.Transpose(o));
		Assert.IsTrue(Vector2.Dot(o.Row1, m.Row1) > 0f);
		m.Orthonormalize();
		AssertEx.AreEqual(o, m);
	}

	[TestMethod]
	public void ToArray_ToString_Parse()
	{
		CollectionAssert.AreEqual(new[] { 1f, 2f, 3f, 4f }, M.ToArray());
		Matrix2 m = new Matrix2(1.5f, -2f, 0f, 4f);
		Assert.AreEqual("1.5 -2 0 4", m.ToString(CultureInfo.InvariantCulture));
		Assert.AreEqual("1.50 -2.00 0.00 4.00", m.ToString("F2", CultureInfo.InvariantCulture));
		Assert.AreEqual(m, Matrix2.Parse("1.5 -2 0 4", CultureInfo.InvariantCulture));
		using (CultureScope.Invariant())
		{
			Assert.AreEqual("1.5 -2 0 4", m.ToString());
			Assert.AreEqual("1.50 -2.00 0.00 4.00", m.ToString("F2"));
			Assert.AreEqual(m, Matrix2.Parse("1.5 -2 0 4"));
		}
		Assert.ThrowsExactly<ArgumentNullException>(() => Matrix2.Parse(null));
		Assert.ThrowsExactly<FormatException>(() => Matrix2.Parse("1 2 3", CultureInfo.InvariantCulture));
	}
}
