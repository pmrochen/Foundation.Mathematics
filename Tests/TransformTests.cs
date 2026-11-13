using System.Globalization;

namespace Foundation.Mathematics.Tests;

[TestClass]
public sealed class AffineTransformTests
{
	private static readonly AffineTransform T = new AffineTransform(Samples.GeneralMatrix3, new Vector3(1f, -2f, 3f));

	private static Vector3 ViaMatrix4(Vector3 p, in Matrix4 m) => (new Vector4(p, 1f)*m).XYZ;

	[TestMethod]
	public void StaticFields_HaveExpectedValues()
	{
		Assert.AreEqual(new AffineTransform(Matrix3.Zero, Vector3.Zero), AffineTransform.Zero);
		Assert.AreEqual(new AffineTransform(Matrix3.Identity, Vector3.Zero), AffineTransform.Identity);
	}

	[TestMethod]
	public void Constructors_And_Properties()
	{
		AffineTransform a = new AffineTransform(1f, 2f, 3f, 4f, 5f, 6f, 7f, 8f, 9f, 10f, 11f, 12f);
		Matrix3 basis = new Matrix3(1f, 2f, 3f, 4f, 5f, 6f, 7f, 8f, 9f);
		Vector3 origin = new Vector3(10f, 11f, 12f);
		Assert.AreEqual(basis, a.Basis);
		Assert.AreEqual(origin, a.Origin);
		Assert.AreEqual(a, new AffineTransform(basis, origin));
		Assert.AreEqual(a, new AffineTransform(basis.Row0, basis.Row1, basis.Row2, origin));
		Assert.AreEqual(basis.Row0, a.Right);
		Assert.AreEqual(basis.Row1, a.Up);
		Assert.AreEqual(basis.Row2, a.Forward);

		a.Right = Vector3.UnitX;
		a.Up = Vector3.UnitY;
		a.Forward = Vector3.UnitZ;
		a.Origin = Vector3.Zero;
		Assert.AreEqual(AffineTransform.Identity, a);
		a.Basis = basis;
		Assert.AreEqual(basis, a.Basis);
	}

	[TestMethod]
	public void ImplicitConversions_FromMatrices()
	{
		AffineTransform a = new Matrix2(1f, 2f, 3f, 4f);
		Assert.AreEqual(new AffineTransform((Matrix3)new Matrix2(1f, 2f, 3f, 4f), Vector3.Zero), a);
		AffineTransform b = Samples.GeneralMatrix3;
		Assert.AreEqual(new AffineTransform(Samples.GeneralMatrix3, Vector3.Zero), b);
	}

	[TestMethod]
	public void Equality_ApproxEquals_HashCode()
	{
		AffineTransform same = new AffineTransform(Samples.GeneralMatrix3, new Vector3(1f, -2f, 3f));
		Assert.IsTrue(T == same);
		Assert.IsFalse(T != same);
		Assert.IsTrue(T != AffineTransform.Identity);
		Assert.IsTrue(T.Equals((object)same));
		Assert.IsFalse(T.Equals((object)Samples.GeneralMatrix3));
		Assert.AreEqual(T.GetHashCode(), same.GetHashCode());
		AffineTransform moved = T;
		moved.Translate(new Vector3(0f, 0f, 1e-3f));
		Assert.IsFalse(T.ApproxEquals(moved));
		Assert.IsTrue(T.ApproxEquals(moved, 1e-2f));
		Assert.IsTrue(T.ApproxEquals(same));
	}

	[TestMethod]
	public void Concat_MatchesMatrix4Product()
	{
		AffineTransform a = AffineTransform.RotationTranslation(Samples.Axes[3], 0.7f, new Vector3(1f, 2f, 3f));
		AffineTransform b = AffineTransform.ScalingTranslation(new Vector3(2f, 1f, 0.5f), new Vector3(-1f, 0f, 4f));
		AffineTransform ab = AffineTransform.Concat(a, b);
		AssertEx.AreEqual((Matrix4)a*(Matrix4)b, (Matrix4)ab);
		foreach (Vector3 p in Samples.Vectors)
			AssertEx.AreEqual(Vector3.Transform(Vector3.Transform(p, a), b), Vector3.Transform(p, ab), 1e-5f);

		AffineTransform c = a;
		c.Concat(b);
		AssertEx.AreEqual(ab, c);
		AffineTransform d = b;
		d.PreConcat(a);
		AssertEx.AreEqual(ab, d);

		AffineTransform e = T;
		AssertEx.AreEqual(AffineTransform.Concat(ab, e), AffineTransform.Concat(a, b, e), 1e-5f);
		AssertEx.AreEqual(AffineTransform.Concat(ab, e, a), AffineTransform.Concat(a, b, e, a), 1e-4f);
		AssertEx.AreEqual(AffineTransform.Concat(ab, e, a, b), AffineTransform.Concat(a, b, e, a, b), 1e-4f);
		AssertEx.AreEqual(AffineTransform.Concat(ab, e, a, b, e), AffineTransform.Concat(a, b, e, a, b, e), 1e-3f);
		AssertEx.AreEqual(AffineTransform.Concat(ab, e, a, b, e, a), AffineTransform.Concat(a, b, e, a, b, e, a), 1e-3f);
		AssertEx.AreEqual(AffineTransform.Concat(ab, e, a, b, e, a, b), AffineTransform.Concat(a, b, e, a, b, e, a, b), 1e-3f);
	}

	[TestMethod]
	public void TransformsPoints_LikeEquivalentMatrix4()
	{
		Matrix4 m = T;
		foreach (Vector3 p in Samples.Vectors)
			AssertEx.AreEqual(ViaMatrix4(p, m), Vector3.Transform(p, T), 1e-5f);
	}

	[TestMethod]
	public void Factories_Rotation()
	{
		YawPitchRoll ypr = new YawPitchRoll(0.3f, -0.2f, 0.5f);
		Euler e = new Euler(0.3f, -0.2f, 0.5f, EulerOrder.ZYX);
		Quaternion q = Quaternion.FromAxisAngle(Samples.Axes[4], 1.2f);
		Assert.AreEqual(new AffineTransform(Matrix3.Rotation(Axis.X, 0.4f), Vector3.Zero), AffineTransform.Rotation(Axis.X, 0.4f));
		Assert.AreEqual(new AffineTransform(Matrix3.Rotation(Samples.Axes[3], 0.4f), Vector3.Zero), AffineTransform.Rotation(Samples.Axes[3], 0.4f));
		Assert.AreEqual(new AffineTransform(Matrix3.Rotation(ypr), Vector3.Zero), AffineTransform.Rotation(ypr));
		Assert.AreEqual(new AffineTransform(Matrix3.Rotation(e), Vector3.Zero), AffineTransform.Rotation(e));
		Assert.AreEqual(new AffineTransform(Matrix3.Rotation(q), Vector3.Zero), AffineTransform.Rotation(q));
	}

	[TestMethod]
	public void Factories_ScalingTranslationShearing()
	{
		Vector3 s = new Vector3(2f, 3f, 4f);
		Vector3 t = new Vector3(-1f, 5f, 0.5f);
		Assert.AreEqual(new AffineTransform(Matrix3.Scaling(s), Vector3.Zero), AffineTransform.Scaling(s));
		Assert.AreEqual(new AffineTransform(Matrix3.Scaling(7f), Vector3.Zero), AffineTransform.Scaling(7f));
		Assert.AreEqual(new AffineTransform(Matrix3.Identity, t), AffineTransform.Translation(t));
		Assert.AreEqual(new AffineTransform(Matrix3.Scaling(s), t), AffineTransform.ScalingTranslation(s, t));
		Assert.AreEqual(new AffineTransform(Matrix3.Shearing(1f, 2f, 3f, 4f, 5f, 6f), Vector3.Zero), AffineTransform.Shearing(1f, 2f, 3f, 4f, 5f, 6f));
	}

	[TestMethod]
	public void Factories_CompositeMatchMatrix4()
	{
		Vector3 s = new Vector3(2f, 3f, 0.5f);
		Vector3 t = new Vector3(-1f, 4f, 2f);
		Vector3 axis = Samples.Axes[4];
		YawPitchRoll ypr = new YawPitchRoll(0.3f, 0.2f, -0.6f);
		Euler e = new Euler(0.4f, -0.1f, 0.9f, EulerOrder.XZY);
		Quaternion q = Quaternion.FromAxisAngle(Samples.Axes[3], 0.8f);

		AssertEx.AreEqual(Matrix4.RotationTranslation(Axis.Y, 0.7f, t), (Matrix4)AffineTransform.RotationTranslation(Axis.Y, 0.7f, t));
		AssertEx.AreEqual(Matrix4.RotationTranslation(axis, 0.7f, t), (Matrix4)AffineTransform.RotationTranslation(axis, 0.7f, t));
		AssertEx.AreEqual(Matrix4.RotationTranslation(ypr, t), (Matrix4)AffineTransform.RotationTranslation(ypr, t));
		AssertEx.AreEqual(Matrix4.RotationTranslation(e, t), (Matrix4)AffineTransform.RotationTranslation(e, t));
		AssertEx.AreEqual(Matrix4.RotationTranslation(q, t), (Matrix4)AffineTransform.RotationTranslation(q, t));
		AssertEx.AreEqual(Matrix4.ScalingRotation(s, Axis.Y, 0.7f), (Matrix4)AffineTransform.ScalingRotation(s, Axis.Y, 0.7f));
		AssertEx.AreEqual(Matrix4.ScalingRotation(s, axis, 0.7f), (Matrix4)AffineTransform.ScalingRotation(s, axis, 0.7f));
		AssertEx.AreEqual(Matrix4.ScalingRotation(s, ypr), (Matrix4)AffineTransform.ScalingRotation(s, ypr));
		AssertEx.AreEqual(Matrix4.ScalingRotation(s, e), (Matrix4)AffineTransform.ScalingRotation(s, e));
		AssertEx.AreEqual(Matrix4.ScalingRotation(s, q), (Matrix4)AffineTransform.ScalingRotation(s, q));
		AssertEx.AreEqual(Matrix4.ScalingRotationTranslation(s, Axis.Y, 0.7f, t), (Matrix4)AffineTransform.ScalingRotationTranslation(s, Axis.Y, 0.7f, t));
		AssertEx.AreEqual(Matrix4.ScalingRotationTranslation(s, axis, 0.7f, t), (Matrix4)AffineTransform.ScalingRotationTranslation(s, axis, 0.7f, t));
		AssertEx.AreEqual(Matrix4.ScalingRotationTranslation(s, ypr, t), (Matrix4)AffineTransform.ScalingRotationTranslation(s, ypr, t));
		AssertEx.AreEqual(Matrix4.ScalingRotationTranslation(s, e, t), (Matrix4)AffineTransform.ScalingRotationTranslation(s, e, t));
		AssertEx.AreEqual(Matrix4.ScalingRotationTranslation(s, q, t), (Matrix4)AffineTransform.ScalingRotationTranslation(s, q, t));
	}

	[TestMethod]
	public void Factories_FromDirections()
	{
		Vector3 origin = new Vector3(1f, 2f, 3f);
		Vector3 target = new Vector3(4f, -2f, 3f);
		Vector3 forward = new Vector3(0.5f, 1f, -2f);
		Vector3 up = new Vector3(0.1f, 1f, 0.2f);
		Vector3 right = new Vector3(1f, 0.1f, 0.3f);
		Assert.AreEqual(new AffineTransform(Matrix3.FromForward(forward), Vector3.Zero), AffineTransform.FromForward(forward));
		Assert.AreEqual(new AffineTransform(Matrix3.FromUp(up), Vector3.Zero), AffineTransform.FromUp(up));
		Assert.AreEqual(new AffineTransform(Matrix3.FromForwardAndUp(forward, up), Vector3.Zero), AffineTransform.FromForwardAndUp(forward, up));
		Assert.AreEqual(new AffineTransform(Matrix3.FromRightAndUp(right, up), Vector3.Zero), AffineTransform.FromRightAndUp(right, up));
		Assert.AreEqual(new AffineTransform(Matrix3.FromForward(target - origin), origin), AffineTransform.FromOriginAndTarget(origin, target));
		Assert.AreEqual(new AffineTransform(Matrix3.FromForwardAndUp(target - origin, up), origin), AffineTransform.FromOriginTargetAndUp(origin, target, up));
		Assert.AreEqual(new AffineTransform(Matrix3.FromForward(forward), origin), AffineTransform.FromOriginAndForward(origin, forward));
		Assert.AreEqual(new AffineTransform(Matrix3.FromForwardAndUp(forward, up), origin), AffineTransform.FromOriginForwardAndUp(origin, forward, up));
		Assert.AreEqual(new AffineTransform(Matrix3.FromRightAndUp(right, up), origin), AffineTransform.FromOriginRightAndUp(origin, right, up));

		// The local +Z axis of a look-at transform points at the target.
		AffineTransform lookAt = AffineTransform.FromOriginAndTarget(origin, target);
		AssertEx.AreEqual(target, Vector3.Transform(Vector3.UnitZ*Vector3.Distance(origin, target), lookAt), 1e-5f);
	}

	[TestMethod]
	public void FromPackedTransform_AppliesScaleRotationTranslation()
	{
		Quaternion q = Quaternion.FromAxisAngle(Samples.Axes[3], 0.9f);
		PackedTransform packed = new PackedTransform(new Vector3(1f, 2f, 3f), q, 2.5f);
		AffineTransform expected = AffineTransform.ScalingRotationTranslation(new Vector3(2.5f), q, new Vector3(1f, 2f, 3f));
		AssertEx.AreEqual(expected, AffineTransform.FromPackedTransform(packed));
		AffineTransform implicitT = packed;
		AssertEx.AreEqual(expected, implicitT);
	}

	[TestMethod]
	public void Scale_PreScale_Translate()
	{
		Vector3 s = new Vector3(2f, -1f, 0.5f);
		Vector3 t = new Vector3(3f, 0f, -1f);
		AffineTransform scaled = T;
		scaled.Scale(s);
		AssertEx.AreEqual(AffineTransform.Concat(T, AffineTransform.Scaling(s)), scaled);
		AffineTransform prescaled = T;
		prescaled.PreScale(s);
		AssertEx.AreEqual(AffineTransform.Concat(AffineTransform.Scaling(s), T), prescaled);
		AffineTransform translated = T;
		translated.Translate(t);
		AssertEx.AreEqual(AffineTransform.Concat(T, AffineTransform.Translation(t)), translated);
	}

	[TestMethod]
	public void Reflection_MatchesMatrix4Reflection()
	{
		Plane plane = new Plane(new Vector3(1f, 2f, -1f), new Vector3(0f, 1f, 2f));
		AssertEx.AreEqual(Matrix4.Reflection(plane), (Matrix4)AffineTransform.Reflection(plane), 1e-5f);
		AffineTransform r = AffineTransform.Reflection(plane);
		foreach (Vector3 p in Samples.Vectors)
			AssertEx.AreEqual(plane.GetSignedDistanceTo(p, false)*-1f, plane.GetSignedDistanceTo(Vector3.Transform(p, r), false), 1e-4f);
	}

	[TestMethod]
	public void Inverse_UndoesTransform()
	{
		AffineTransform inv = AffineTransform.Inverse(T);
		AssertEx.AreEqual(AffineTransform.Identity, AffineTransform.Concat(T, inv), 1e-5f);
		AssertEx.AreEqual(AffineTransform.Identity, AffineTransform.Concat(inv, T), 1e-5f);
		AssertEx.AreEqual(Matrix4.Inverse(T), (Matrix4)inv, 1e-5f);
		AffineTransform t = T;
		t.Invert();
		AssertEx.AreEqual(inv, t, 1e-6f);
	}

	[TestMethod]
	public void InverseOrthogonal_UndoesRigidTransform()
	{
		AffineTransform rigid = AffineTransform.RotationTranslation(Samples.Axes[4], 1.1f, new Vector3(5f, -3f, 2f));
		AffineTransform inv = AffineTransform.InverseOrthogonal(rigid);
		AssertEx.AreEqual(AffineTransform.Inverse(rigid), inv, 1e-5f);
		AssertEx.AreEqual(AffineTransform.Identity, AffineTransform.Concat(rigid, inv), 1e-5f);
		AffineTransform t = rigid;
		t.InvertOrthogonal();
		AssertEx.AreEqual(inv, t, 1e-6f);
	}

	[TestMethod]
	public void Orthonormalize_KeepsOriginAndOrthonormalizesBasis()
	{
		AffineTransform o = AffineTransform.Orthonormalize(T);
		AssertEx.IsOrthonormal(o.Basis);
		Assert.AreEqual(T.Origin, o.Origin);
		AffineTransform t = T;
		t.Orthonormalize();
		AssertEx.AreEqual(o, t);
	}

	[TestMethod]
	public void IsFinite_ScaleComponent()
	{
		Assert.IsTrue(T.IsFinite);
		AffineTransform bad = T;
		bad.Origin = new Vector3(Single.NaN);
		Assert.IsFalse(bad.IsFinite);
		AssertEx.AreEqual(new Vector3(2f, 3f, 4f), AffineTransform.ScalingRotationTranslation(new Vector3(2f, 3f, 4f), Axis.Z, 0.5f, Vector3.UnitX).ScaleComponent);
	}

	[TestMethod]
	public void ToString_Parse()
	{
		AffineTransform a = new AffineTransform(1f, 2f, 3f, 4f, 5f, 6f, 7f, 8f, 9f, 10.5f, -11f, 12f);
		const string text = "1 2 3 4 5 6 7 8 9 10.5 -11 12";
		Assert.AreEqual(text, a.ToString(CultureInfo.InvariantCulture));
		Assert.AreEqual("1.0 2.0 3.0 4.0 5.0 6.0 7.0 8.0 9.0 10.5 -11.0 12.0", a.ToString("F1", CultureInfo.InvariantCulture));
		Assert.AreEqual(a, AffineTransform.Parse(text, CultureInfo.InvariantCulture));
		using (CultureScope.Invariant())
		{
			Assert.AreEqual(text, a.ToString());
			Assert.AreEqual("1.0 2.0 3.0 4.0 5.0 6.0 7.0 8.0 9.0 10.5 -11.0 12.0", a.ToString("F1"));
			Assert.AreEqual(a, AffineTransform.Parse(text));
		}
		Assert.ThrowsExactly<ArgumentNullException>(() => AffineTransform.Parse(null));
		Assert.ThrowsExactly<FormatException>(() => AffineTransform.Parse("1 2 3 4 5 6 7 8 9", CultureInfo.InvariantCulture));
	}
}

[TestClass]
public sealed class PackedTransformTests
{
	[TestMethod]
	public void StaticFields_HaveExpectedValues()
	{
		Assert.AreEqual(new PackedTransform(Vector4.Zero, Quaternion.Zero), PackedTransform.Zero);
		Assert.AreEqual(Vector3.Zero, PackedTransform.Identity.Translation);
		Assert.AreEqual(1f, PackedTransform.Identity.Scale);
		Assert.AreEqual(Quaternion.Identity, PackedTransform.Identity.Rotation);
		Assert.AreEqual(AffineTransform.Identity, AffineTransform.FromPackedTransform(PackedTransform.Identity));
	}

	[TestMethod]
	public void Constructors_And_Properties()
	{
		Quaternion q = Quaternion.FromAxisAngle(Vector3.UnitY, 0.5f);
		PackedTransform t = new PackedTransform(new Vector3(1f, 2f, 3f), q, 4f);
		Assert.AreEqual(new Vector3(1f, 2f, 3f), t.Translation);
		Assert.AreEqual(q, t.Rotation);
		Assert.AreEqual(4f, t.Scale);
		Assert.AreEqual(new Vector4(1f, 2f, 3f, 4f), t.TranslationScale);
		Assert.AreEqual(t, new PackedTransform(new Vector4(1f, 2f, 3f, 4f), q));

		t.Translation = new Vector3(-1f, -2f, -3f);
		t.Scale = 0.5f;
		t.Rotation = Quaternion.Identity;
		Assert.AreEqual(new PackedTransform(new Vector4(-1f, -2f, -3f, 0.5f), Quaternion.Identity), t);
		t.TranslationScale = new Vector4(9f);
		Assert.AreEqual(new Vector3(9f), t.Translation);
		Assert.AreEqual(9f, t.Scale);
	}

	[TestMethod]
	public void Equality_ApproxEquals_IsFinite()
	{
		PackedTransform a = new PackedTransform(new Vector3(1f, 2f, 3f), Quaternion.Identity, 2f);
		PackedTransform b = new PackedTransform(new Vector3(1f, 2f, 3f), Quaternion.Identity, 2f);
		PackedTransform c = new PackedTransform(new Vector3(1f, 2f, 3.01f), Quaternion.Identity, 2f);
		Assert.IsTrue(a == b);
		Assert.IsFalse(a != b);
		Assert.IsTrue(a != c);
		Assert.IsTrue(a.Equals((object)b));
		Assert.IsFalse(a.Equals((object)c));
		Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
		Assert.IsFalse(a.ApproxEquals(c));
		Assert.IsTrue(a.ApproxEquals(c, 0.1f));
		Assert.IsTrue(a.IsFinite);
		Assert.IsFalse(new PackedTransform(new Vector3(Single.NaN), Quaternion.Identity, 1f).IsFinite);
		Assert.IsFalse(new PackedTransform(Vector3.Zero, new Quaternion(Single.PositiveInfinity), 1f).IsFinite);
	}

	[TestMethod]
	public void ToString_Parse()
	{
		PackedTransform t = new PackedTransform(new Vector4(1f, 2f, 3f, 0.5f), new Quaternion(0f, 0f, 0.5f, -1.25f));
		const string text = "1 2 3 0.5 0 0 0.5 -1.25";
		Assert.AreEqual(text, t.ToString(CultureInfo.InvariantCulture));
		Assert.AreEqual("1.00 2.00 3.00 0.50 0.00 0.00 0.50 -1.25", t.ToString("F2", CultureInfo.InvariantCulture));
		Assert.AreEqual(t, PackedTransform.Parse(text, CultureInfo.InvariantCulture));
		using (CultureScope.Invariant())
		{
			Assert.AreEqual(text, t.ToString());
			Assert.AreEqual("1.00 2.00 3.00 0.50 0.00 0.00 0.50 -1.25", t.ToString("F2"));
			Assert.AreEqual(t, PackedTransform.Parse(text));
		}
		Assert.ThrowsExactly<ArgumentNullException>(() => PackedTransform.Parse(null));
		Assert.ThrowsExactly<FormatException>(() => PackedTransform.Parse("1 2 3", CultureInfo.InvariantCulture));
	}
}
