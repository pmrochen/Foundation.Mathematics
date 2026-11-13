using System.Globalization;

namespace Foundation.Mathematics.Tests;

[TestClass]
public sealed class PlaneTests
{
	// Plane z = 2 (normal +Z).
	private static readonly Plane P = new Plane(Vector3.UnitZ, new Vector3(5f, -3f, 2f));

	[TestMethod]
	public void Constructors_And_Properties()
	{
		Assert.AreEqual(new Plane(0f, 0f, 1f, -2f), P);
		Assert.AreEqual(new Plane(Vector3.UnitZ, -2f), P);
		Assert.AreEqual(0f, P.A);
		Assert.AreEqual(0f, P.B);
		Assert.AreEqual(1f, P.C);
		Assert.AreEqual(-2f, P.D);
		Assert.AreEqual(Vector3.UnitZ, P.Normal);
		Assert.AreEqual(-2f, P.Constant);
		Assert.IsFalse(P.IsEmpty);
		Assert.IsTrue(Plane.Empty.IsEmpty);

		Plane p = Plane.Empty;
		p.A = 1f;
		p.B = 2f;
		p.C = 3f;
		p.D = 4f;
		Assert.AreEqual(new Plane(1f, 2f, 3f, 4f), p);
		p.Normal = Vector3.UnitY;
		p.Constant = -1f;
		Assert.AreEqual(new Plane(0f, 1f, 0f, -1f), p);
	}

	[TestMethod]
	public void ConversionsFromHalfSpace()
	{
		HalfSpace hs = new HalfSpace(1f, 2f, 3f, 4f);
		Plane implicitPlane = hs;
		Assert.AreEqual(new Plane(1f, 2f, 3f, 4f), implicitPlane);
		Assert.AreEqual(new Plane(1f, 2f, 3f, 4f), Plane.FromHalfSpace(hs));
	}

	[TestMethod]
	public void Equality_ApproxEquals_HashCode()
	{
		Plane same = new Plane(0f, 0f, 1f, -2f);
		Assert.IsTrue(P == same);
		Assert.IsFalse(P != same);
		Assert.IsTrue(P != Plane.Flip(P));
		Assert.IsTrue(P.Equals((object)same));
		Assert.IsFalse(P.Equals((object)new HalfSpace(0f, 0f, 1f, -2f)));
		Assert.AreEqual(P.GetHashCode(), same.GetHashCode());
		Assert.IsTrue(P.ApproxEquals(new Plane(0f, 0f, 1f, -2f + 1e-7f)));
		Assert.IsFalse(P.ApproxEquals(new Plane(0f, 0f, 1f, -2.01f)));
		Assert.IsTrue(P.ApproxEquals(new Plane(0f, 0f, 1f, -2.01f), 0.1f));
	}

	[TestMethod]
	public void Normalize_And_Flip()
	{
		Plane p = new Plane(0f, 3f, 4f, -10f);
		Plane n = Plane.Normalize(p);
		AssertEx.AreEqual(new Vector3(0f, 0.6f, 0.8f), n.Normal);
		AssertEx.AreEqual(-2f, n.D);
		p.Normalize();
		Assert.IsTrue(n.ApproxEquals(p));
		Assert.AreEqual(Plane.Empty, Plane.Normalize(Plane.Empty));

		Assert.AreEqual(new Plane(0f, 0f, -1f, 2f), Plane.Flip(P));
		Plane f = P;
		f.Flip();
		Assert.AreEqual(Plane.Flip(P), f);
	}

	[TestMethod]
	public void FromTriangle_UsesCounterClockwiseWinding()
	{
		Vector3 v0 = new Vector3(0f, 0f, 1f);
		Vector3 v1 = new Vector3(1f, 0f, 1f);
		Vector3 v2 = new Vector3(0f, 1f, 1f);
		Plane p = Plane.FromTriangle(v0, v1, v2);
		AssertEx.AreEqual(Vector3.UnitZ, p.Normal);
		AssertEx.AreEqual(-1f, p.D);
		Assert.AreEqual(p, Plane.FromTriangle(new Triangle3(v0, v1, v2)));
		foreach (Vector3 v in new[] { v0, v1, v2 })
			Assert.IsTrue(p.Contains(v));
	}

	[TestMethod]
	public void Distances_And_Contains()
	{
		Assert.AreEqual(3f, P.GetDistanceTo(new Vector3(1f, 1f, 5f)));
		Assert.AreEqual(3f, P.GetDistanceTo(new Vector3(1f, 1f, -1f)));
		Assert.AreEqual(3f, P.GetSignedDistanceTo(new Vector3(1f, 1f, 5f)));
		Assert.AreEqual(-3f, P.GetSignedDistanceTo(new Vector3(1f, 1f, -1f)));
		Assert.IsTrue(P.Contains(new Vector3(7f, 8f, 2f)));
		Assert.IsFalse(P.Contains(new Vector3(7f, 8f, 2.01f)));

		// Unnormalized plane: 2z - 4 = 0.
		Plane u = new Plane(0f, 0f, 2f, -4f);
		AssertEx.AreEqual(3f, u.GetDistanceTo(new Vector3(0f, 0f, 5f), false));
		AssertEx.AreEqual(-3f, u.GetSignedDistanceTo(new Vector3(0f, 0f, -1f), false));
		AssertEx.AreEqual(3f, P.GetDistanceTo(new Vector3(0f, 0f, 5f), true));
		AssertEx.AreEqual(-3f, P.GetSignedDistanceTo(new Vector3(0f, 0f, -1f), true));
	}

	[TestMethod]
	public void Reflect_MirrorsPoint()
	{
		AssertEx.AreEqual(new Vector3(1f, 2f, -1f), P.Reflect(new Vector3(1f, 2f, 5f)));
		AssertEx.AreEqual(new Vector3(1f, 2f, 2f), P.Reflect(new Vector3(1f, 2f, 2f)));
	}

	[TestMethod]
	public void Translate_MovesPlane()
	{
		Plane moved = Plane.Translate(P, new Vector3(10f, -4f, 3f));
		AssertEx.AreEqual(Vector3.UnitZ, moved.Normal);
		AssertEx.AreEqual(-5f, moved.D);
		Plane p = P;
		p.Translate(new Vector3(0f, 0f, -2f));
		AssertEx.AreEqual(0f, p.D);
	}

	[TestMethod]
	public void Transform_MapsPointsOnPlaneToPointsOnTransformedPlane()
	{
		Plane plane = Plane.Normalize(new Plane(1f, -2f, 0.5f, 3f));
		Vector3 u = Vector3.Normalize(Vector3.Perpendicular(plane.Normal));
		Vector3 w = Vector3.Cross(plane.Normal, u);
		Vector3 origin = -plane.D*plane.Normal;
		Vector3[] pointsOnPlane = { origin, origin + 2f*u, origin - 3f*w, origin + u + w };
		foreach (Vector3 p in pointsOnPlane)
			AssertEx.AreEqual(0f, plane.GetSignedDistanceTo(p), 1e-5f);

		Matrix3 general = Samples.GeneralMatrix3;
		Plane byMatrix = Plane.Transform(plane, general);
		AffineTransform at = new AffineTransform(general, new Vector3(1f, -2f, 4f));
		Plane byAffine = Plane.Transform(plane, at);
		foreach (Vector3 p in pointsOnPlane)
		{
			AssertEx.AreEqual(0f, byMatrix.GetSignedDistanceTo(p*general), 1e-4f);
			AssertEx.AreEqual(0f, byAffine.GetSignedDistanceTo(Vector3.Transform(p, at)), 1e-4f);
		}
		AssertEx.AreEqual(1f, byMatrix.Normal.Magnitude);
		AssertEx.AreEqual(1f, byAffine.Normal.Magnitude);

		// Points on the positive side stay on the positive side.
		Vector3 front = origin + plane.Normal;
		Assert.IsTrue(byAffine.GetSignedDistanceTo(Vector3.Transform(front, at)) > 0f);

		Plane inPlace = plane;
		inPlace.Transform(at);
		Assert.IsTrue(byAffine.ApproxEquals(inPlace));
		inPlace = plane;
		inPlace.Transform(general);
		Assert.IsTrue(byMatrix.ApproxEquals(inPlace));
	}

	[TestMethod]
	public void Transform_Orthogonal_MatchesGeneralPathForRotations()
	{
		Plane plane = Plane.Normalize(new Plane(1f, -2f, 0.5f, 3f));
		Matrix3 rotation = Matrix3.Rotation(Samples.Axes[4], 0.9f);
		AffineTransform rigid = new AffineTransform(rotation, new Vector3(1f, 2f, 3f));
		Assert.IsTrue(Plane.Transform(plane, rotation, false).ApproxEquals(Plane.Transform(plane, rotation, true), 1e-5f));
		Assert.IsTrue(Plane.Transform(plane, rigid, false).ApproxEquals(Plane.Transform(plane, rigid, true), 1e-5f));
	}

	[TestMethod]
	public void Intersects_Primitives()
	{
		Assert.IsTrue(P.Intersects(new Triangle3(new Vector3(0f, 0f, 1f), new Vector3(1f, 0f, 3f), new Vector3(0f, 1f, 3f))));
		Assert.IsFalse(P.Intersects(new Triangle3(new Vector3(0f, 0f, 3f), new Vector3(1f, 0f, 3f), new Vector3(0f, 1f, 3f))));
		Assert.IsTrue(P.Intersects(new AxisAlignedBox(new Vector3(0f, 0f, 1f), new Vector3(1f, 1f, 2f))));
		Assert.IsFalse(P.Intersects(new AxisAlignedBox(new Vector3(0f, 0f, 2.1f), new Vector3(1f, 1f, 3f))));
		Assert.IsTrue(P.Intersects(new OrientedBox(new Vector3(0f, 0f, 2.5f), Matrix3.Rotation(Axis.X, SingleConstants.Pi/4f), new Vector3(1f))));
		Assert.IsFalse(P.Intersects(new OrientedBox(new Vector3(0f, 0f, 4f), Matrix3.Rotation(Axis.X, SingleConstants.Pi/4f), new Vector3(1f))));
		Assert.IsTrue(P.Intersects(new Sphere(new Vector3(0f, 0f, 3f), 1f)));
		Assert.IsFalse(P.Intersects(new Sphere(new Vector3(0f, 0f, 3.1f), 1f)));
		Assert.IsTrue(P.Intersects(new Ellipsoid(new Vector3(0f, 0f, 4f), Matrix3.Identity, new Vector3(1f, 1f, 2.5f))));
		Assert.IsFalse(P.Intersects(new Ellipsoid(new Vector3(0f, 0f, 4f), Matrix3.Identity, new Vector3(3f, 3f, 1.5f))));
	}

	[TestMethod]
	public void ToString_And_Parse()
	{
		Plane p = new Plane(0.5f, -1f, 2f, 3.25f);
		Assert.AreEqual("0.5 -1 2 3.25", p.ToString(CultureInfo.InvariantCulture));
		Assert.AreEqual("0.50 -1.00 2.00 3.25", p.ToString("F2", CultureInfo.InvariantCulture));
		Assert.AreEqual(p, Plane.Parse("0.5 -1 2 3.25", CultureInfo.InvariantCulture));
		using (CultureScope.Invariant())
		{
			Assert.AreEqual("0.5 -1 2 3.25", p.ToString());
			Assert.AreEqual("0.50 -1.00 2.00 3.25", p.ToString("F2"));
			Assert.AreEqual(p, Plane.Parse("0.5 -1 2 3.25"));
		}
		Assert.ThrowsExactly<ArgumentNullException>(() => Plane.Parse(null));
		Assert.ThrowsExactly<FormatException>(() => Plane.Parse("1 2 3", CultureInfo.InvariantCulture));
	}
}

[TestClass]
public sealed class HalfSpaceTests
{
	// Points with z <= 2 are inside.
	private static readonly HalfSpace H = new HalfSpace(Vector3.UnitZ, new Vector3(5f, -3f, 2f));

	[TestMethod]
	public void Constructors_And_Properties()
	{
		Assert.AreEqual(new HalfSpace(0f, 0f, 1f, -2f), H);
		Assert.AreEqual(new HalfSpace(Vector3.UnitZ, -2f), H);
		Assert.AreEqual(Vector3.UnitZ, H.Normal);
		Assert.AreEqual(-2f, H.D);
		Assert.AreEqual(-2f, H.Constant);
		Assert.AreEqual(0f, H.A);
		Assert.AreEqual(0f, H.B);
		Assert.AreEqual(1f, H.C);
		Assert.IsTrue(HalfSpace.Empty.IsEmpty);
		Assert.IsFalse(H.IsEmpty);
		HalfSpace fromPlane = new Plane(1f, 2f, 3f, 4f);
		Assert.AreEqual(new HalfSpace(1f, 2f, 3f, 4f), fromPlane);
		Assert.AreEqual(new HalfSpace(1f, 2f, 3f, 4f), HalfSpace.FromPlane(new Plane(1f, 2f, 3f, 4f)));

		HalfSpace h = HalfSpace.Empty;
		h.Normal = Vector3.UnitX;
		h.Constant = 3f;
		Assert.AreEqual(new HalfSpace(1f, 0f, 0f, 3f), h);
		h.A = 0f;
		h.B = 1f;
		h.D = -1f;
		Assert.AreEqual(new HalfSpace(0f, 1f, 0f, -1f), h);
	}

	[TestMethod]
	public void Equality_ApproxEquals_HashCode()
	{
		HalfSpace same = new HalfSpace(0f, 0f, 1f, -2f);
		Assert.IsTrue(H == same);
		Assert.IsFalse(H != same);
		Assert.IsTrue(H.Equals((object)same));
		Assert.AreEqual(H.GetHashCode(), same.GetHashCode());
		Assert.IsTrue(H.ApproxEquals(new HalfSpace(0f, 0f, 1f, -2f + 1e-7f)));
		Assert.IsFalse(H.ApproxEquals(new HalfSpace(0f, 0f, 1f, -2.1f)));
		Assert.IsTrue(H.ApproxEquals(new HalfSpace(0f, 0f, 1f, -2.1f), 0.2f));
	}

	[TestMethod]
	public void Normalize_And_Flip()
	{
		HalfSpace h = new HalfSpace(0f, 3f, 4f, -10f);
		HalfSpace n = HalfSpace.Normalize(h);
		AssertEx.AreEqual(new Vector3(0f, 0.6f, 0.8f), n.Normal);
		AssertEx.AreEqual(-2f, n.D);
		h.Normalize();
		Assert.IsTrue(n.ApproxEquals(h));
		Assert.AreEqual(new HalfSpace(0f, 0f, -1f, 2f), HalfSpace.Flip(H));
		HalfSpace f = H;
		f.Flip();
		Assert.AreEqual(HalfSpace.Flip(H), f);
	}

	[TestMethod]
	public void FromTriangle_InsideIsBehindCounterClockwiseFace()
	{
		Vector3 v0 = Vector3.Zero, v1 = Vector3.UnitX, v2 = Vector3.UnitY;
		HalfSpace h = HalfSpace.FromTriangle(v0, v1, v2);
		AssertEx.AreEqual(Vector3.UnitZ, h.Normal);
		Assert.AreEqual(h, HalfSpace.FromTriangle(new Triangle3(v0, v1, v2)));
		Assert.IsTrue(h.Contains(new Vector3(0f, 0f, -1f)));
		Assert.IsFalse(h.Contains(new Vector3(0f, 0f, 1f)));
	}

	[TestMethod]
	public void Contains_And_Distances()
	{
		Assert.IsTrue(H.Contains(new Vector3(0f, 0f, 2f)));
		Assert.IsTrue(H.Contains(new Vector3(0f, 0f, -100f)));
		Assert.IsFalse(H.Contains(new Vector3(0f, 0f, 2.01f)));
		Assert.AreEqual(0f, H.GetDistanceTo(new Vector3(0f, 0f, -5f)));
		Assert.AreEqual(3f, H.GetDistanceTo(new Vector3(0f, 0f, 5f)));
		Assert.AreEqual(3f, H.GetSignedDistanceTo(new Vector3(0f, 0f, 5f)));
		Assert.AreEqual(-7f, H.GetSignedDistanceTo(new Vector3(0f, 0f, -5f)));

		HalfSpace u = new HalfSpace(0f, 0f, 2f, -4f);
		AssertEx.AreEqual(3f, u.GetDistanceTo(new Vector3(0f, 0f, 5f), false));
		AssertEx.AreEqual(0f, u.GetDistanceTo(new Vector3(0f, 0f, 1f), false));
		AssertEx.AreEqual(-1f, u.GetSignedDistanceTo(new Vector3(0f, 0f, 1f), false));
		AssertEx.AreEqual(3f, H.GetDistanceTo(new Vector3(0f, 0f, 5f), true));
		AssertEx.AreEqual(3f, H.GetSignedDistanceTo(new Vector3(0f, 0f, 5f), true));
	}

	[TestMethod]
	public void Translate_And_Transform()
	{
		HalfSpace moved = HalfSpace.Translate(H, new Vector3(1f, 1f, 1f));
		AssertEx.AreEqual(-3f, moved.D);
		HalfSpace h = H;
		h.Translate(new Vector3(0f, 0f, -2f));
		AssertEx.AreEqual(0f, h.D);

		AffineTransform at = AffineTransform.RotationTranslation(Axis.X, SingleConstants.HalfPi, new Vector3(0f, 0f, 10f));
		HalfSpace t = HalfSpace.Transform(H, at);
		Vector3 inside = new Vector3(1f, 2f, 1f);
		Vector3 outside = new Vector3(1f, 2f, 3f);
		Assert.IsTrue(t.Contains(Vector3.Transform(inside, at)));
		Assert.IsFalse(t.Contains(Vector3.Transform(outside, at)));
		Assert.IsTrue(HalfSpace.Transform(H, at, true).ApproxEquals(t, 1e-5f));
		h = H;
		h.Transform(at);
		Assert.IsTrue(t.ApproxEquals(h));

		Matrix3 m = Samples.GeneralMatrix3;
		Plane byMatrix = HalfSpace.Transform(H, m);
		HalfSpace hm = byMatrix;
		Assert.IsTrue(hm.Contains(inside*m));
		Assert.IsFalse(hm.Contains(outside*m));
		h = H;
		h.Transform(m);
		Assert.IsTrue(h.ApproxEquals(hm));
	}

	[TestMethod]
	public void Intersects_Primitives()
	{
		Assert.IsTrue(H.Intersects(new Triangle3(new Vector3(0f, 0f, 5f), new Vector3(1f, 0f, 5f), new Vector3(0f, 1f, 1f))));
		Assert.IsFalse(H.Intersects(new Triangle3(new Vector3(0f, 0f, 5f), new Vector3(1f, 0f, 5f), new Vector3(0f, 1f, 3f))));
		Assert.IsTrue(H.Intersects(new AxisAlignedBox(new Vector3(0f, 0f, -5f), new Vector3(1f, 1f, -4f))));
		Assert.IsTrue(H.Intersects(new AxisAlignedBox(new Vector3(0f, 0f, 1f), new Vector3(1f, 1f, 4f))));
		Assert.IsFalse(H.Intersects(new AxisAlignedBox(new Vector3(0f, 0f, 2.5f), new Vector3(1f, 1f, 4f))));
		Assert.IsTrue(H.Intersects(new OrientedBox(new Vector3(0f, 0f, 3f), Matrix3.Rotation(Axis.X, SingleConstants.Pi/4f), new Vector3(1f))));
		Assert.IsFalse(H.Intersects(new OrientedBox(new Vector3(0f, 0f, 4f), Matrix3.Rotation(Axis.X, SingleConstants.Pi/4f), new Vector3(1f))));
		Assert.IsTrue(H.Intersects(new Sphere(new Vector3(0f, 0f, -50f), 1f)));
		Assert.IsTrue(H.Intersects(new Sphere(new Vector3(0f, 0f, 3f), 1f)));
		Assert.IsFalse(H.Intersects(new Sphere(new Vector3(0f, 0f, 3.1f), 1f)));
	}

	[TestMethod]
	public void ToString_And_Parse()
	{
		HalfSpace h = new HalfSpace(0.5f, -1f, 2f, 3.25f);
		Assert.AreEqual("0.5 -1 2 3.25", h.ToString(CultureInfo.InvariantCulture));
		Assert.AreEqual("0.50 -1.00 2.00 3.25", h.ToString("F2", CultureInfo.InvariantCulture));
		Assert.AreEqual(h, HalfSpace.Parse("0.5 -1 2 3.25", CultureInfo.InvariantCulture));
		using (CultureScope.Invariant())
		{
			Assert.AreEqual("0.5 -1 2 3.25", h.ToString());
			Assert.AreEqual("0.50 -1.00 2.00 3.25", h.ToString("F2"));
			Assert.AreEqual(h, HalfSpace.Parse("0.5 -1 2 3.25"));
		}
		Assert.ThrowsExactly<ArgumentNullException>(() => HalfSpace.Parse(null));
		Assert.ThrowsExactly<FormatException>(() => HalfSpace.Parse("1 2 3", CultureInfo.InvariantCulture));
	}
}
