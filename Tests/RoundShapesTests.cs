namespace Foundation.Mathematics.Tests;

[TestClass]
public sealed class Circle2Tests
{
	private static readonly Circle2 C = new Circle2(new Vector2(1f, 2f), 3f);

	[TestMethod]
	public void Constructor_And_Properties()
	{
		Assert.AreEqual(new Vector2(1f, 2f), C.Center);
		Assert.AreEqual(3f, C.Radius);
		Assert.AreEqual(6f, C.Diameter);
		AssertEx.AreEqual(6f*MathF.PI, C.Circumference);
		AssertEx.AreEqual(9f*MathF.PI, C.Area);
		Assert.IsTrue(C.IsFinite);
		Assert.IsFalse(new Circle2(Vector2.Zero, Single.NaN).IsFinite);

		Circle2 c = C;
		c.Center = Vector2.Zero;
		c.Radius = 1f;
		Assert.AreEqual(new Circle2(Vector2.Zero, 1f), c);
		c.Diameter = 5f;
		Assert.AreEqual(2.5f, c.Radius);
	}

	[TestMethod]
	public void Equality_ApproxEquals_HashCode()
	{
		Circle2 same = new Circle2(new Vector2(1f, 2f), 3f);
		Assert.IsTrue(C == same);
		Assert.IsFalse(C != same);
		Assert.IsTrue(C.Equals((object)same));
		Assert.IsFalse(C.Equals((object)new Circle2(new Vector2(1f, 2f), 3.5f)));
		Assert.AreEqual(C.GetHashCode(), same.GetHashCode());
		Assert.IsTrue(C.ApproxEquals(new Circle2(new Vector2(1f, 2f), 3f + 1e-7f)));
		Assert.IsFalse(C.ApproxEquals(new Circle2(new Vector2(1f, 2f), 3.01f)));
		Assert.IsTrue(C.ApproxEquals(new Circle2(new Vector2(1f, 2f), 3.01f), 0.1f));
	}

	[TestMethod]
	public void CircumscribedRectangle_And_Translate()
	{
		Assert.AreEqual(new AxisAlignedRectangle(new Vector2(-2f, -1f), new Vector2(4f, 5f)), C.GetCircumscribedRectangle());
		Assert.AreEqual(new Circle2(new Vector2(2f, 0f), 3f), Circle2.Translate(C, new Vector2(1f, -2f)));
		Circle2 c = C;
		c.Translate(new Vector2(1f, -2f));
		Assert.AreEqual(new Vector2(2f, 0f), c.Center);
	}

	[TestMethod]
	public void Distances_And_Contains()
	{
		Assert.AreEqual(2f, C.GetDistanceTo(new Vector2(1f, 7f)));
		Assert.AreEqual(0f, C.GetDistanceTo(new Vector2(1f, 3f)));
		Assert.AreEqual(2f, C.GetSignedDistanceTo(new Vector2(1f, 7f)));
		Assert.AreEqual(-2f, C.GetSignedDistanceTo(new Vector2(1f, 3f)));
		Assert.IsTrue(C.Contains(new Vector2(4f, 2f)));
		Assert.IsTrue(C.Contains(C.Center));
		Assert.IsFalse(C.Contains(new Vector2(4f, 2.1f)));
	}

	[TestMethod]
	public void Intersects()
	{
		Assert.IsTrue(C.Intersects(new Circle2(new Vector2(7f, 2f), 3f)));
		Assert.IsFalse(C.Intersects(new Circle2(new Vector2(7.1f, 2f), 3f)));
		Assert.IsTrue(C.Intersects(new Circle2(C.Center, 0.1f)));
		Assert.IsTrue(C.Intersects(new AxisAlignedRectangle(new Vector2(4f, -10f), new Vector2(5f, 10f))));
		Assert.IsFalse(C.Intersects(new AxisAlignedRectangle(new Vector2(4.1f, -10f), new Vector2(5f, 10f))));
		// Near a corner: the rectangle's corner (4, 5) is at distance sqrt(18) > 3 from the center.
		Assert.IsFalse(C.Intersects(new AxisAlignedRectangle(new Vector2(4f, 5f), new Vector2(6f, 6f))));
		Assert.IsTrue(C.Intersects(new AxisAlignedRectangle(new Vector2(3f, 4f), new Vector2(6f, 6f))));
		Assert.IsTrue(C.Intersects(new AxisAlignedRectangle(new Vector2(-100f), new Vector2(100f))));
	}
}

[TestClass]
public sealed class SphereTests
{
	private static readonly Sphere S = new Sphere(new Vector3(1f, 2f, 3f), 2f);

	[TestMethod]
	public void Constructor_And_Properties()
	{
		Assert.AreEqual(new Vector3(1f, 2f, 3f), S.Center);
		Assert.AreEqual(2f, S.Radius);
		Assert.AreEqual(4f, S.Diameter);
		AssertEx.AreEqual(16f*MathF.PI, S.SurfaceArea);
		AssertEx.AreEqual(32f*MathF.PI/3f, S.Volume);
		Assert.IsTrue(S.IsFinite);
		Assert.IsFalse(new Sphere(new Vector3(Single.NaN), 1f).IsFinite);
		Sphere s = S;
		s.Center = Vector3.Zero;
		s.Radius = 1f;
		Assert.AreEqual(new Sphere(Vector3.Zero, 1f), s);
		s.Diameter = 3f;
		Assert.AreEqual(1.5f, s.Radius);
	}

	[TestMethod]
	public void Equality_ApproxEquals_HashCode()
	{
		Sphere same = new Sphere(new Vector3(1f, 2f, 3f), 2f);
		Assert.IsTrue(S == same);
		Assert.IsFalse(S != same);
		Assert.IsTrue(S.Equals((object)same));
		Assert.IsFalse(S.Equals((object)new Sphere(S.Center, 1f)));
		Assert.AreEqual(S.GetHashCode(), same.GetHashCode());
		Assert.IsTrue(S.ApproxEquals(new Sphere(S.Center, 2f + 1e-7f)));
		Assert.IsFalse(S.ApproxEquals(new Sphere(S.Center, 2.01f)));
		Assert.IsTrue(S.ApproxEquals(new Sphere(S.Center, 2.01f), 0.1f));
	}

	[TestMethod]
	public void CircumscribedBox_FromEllipsoid_Translate()
	{
		Assert.AreEqual(new AxisAlignedBox(new Vector3(-1f, 0f, 1f), new Vector3(3f, 4f, 5f)), S.GetCircumscribedBox());
		Ellipsoid e = new Ellipsoid(new Vector3(1f, 1f, 1f), Matrix3.Rotation(Axis.X, 0.3f), new Vector3(1f, 4f, 2f));
		Assert.AreEqual(new Sphere(new Vector3(1f, 1f, 1f), 4f), Sphere.FromEllipsoid(e));
		Assert.AreEqual(new Sphere(new Vector3(2f, 2f, 2f), 2f), Sphere.Translate(S, new Vector3(1f, 0f, -1f)));
		Sphere s = S;
		s.Translate(new Vector3(1f, 0f, -1f));
		Assert.AreEqual(new Vector3(2f, 2f, 2f), s.Center);
	}

	[TestMethod]
	public void Transform_Orthogonal_MovesCenterOnly()
	{
		Matrix3 r = Matrix3.Rotation(Axis.Z, SingleConstants.HalfPi);
		Sphere rotated = Sphere.Transform(S, r, true);
		AssertEx.AreEqual(new Vector3(-2f, 1f, 3f), rotated.Center);
		Assert.AreEqual(2f, rotated.Radius);
		AffineTransform at = new AffineTransform(r, new Vector3(10f, 0f, 0f));
		Sphere moved = Sphere.Transform(S, at, true);
		AssertEx.AreEqual(new Vector3(8f, 1f, 3f), moved.Center);
		Assert.AreEqual(2f, moved.Radius);

		Sphere s = S;
		s.Transform(r, true);
		Assert.IsTrue(rotated.ApproxEquals(s));
		s = S;
		s.Transform(at, true);
		Assert.IsTrue(moved.ApproxEquals(s));
	}

	[TestMethod]
	public void Transform_General_UsesLargestScale()
	{
		Matrix3 m = Matrix3.ScalingRotation(new Vector3(1f, 3f, 2f), Axis.Z, 0.4f);
		Sphere transformed = Sphere.Transform(S, m);
		AssertEx.AreEqual(S.Center*m, transformed.Center);
		AssertEx.AreEqual(6f, transformed.Radius);

		AffineTransform at = new AffineTransform(m, new Vector3(1f, 1f, 1f));
		Sphere moved = Sphere.Transform(S, at);
		AssertEx.AreEqual(Vector3.Transform(S.Center, at), moved.Center);
		AssertEx.AreEqual(6f, moved.Radius);

		Sphere s = S;
		s.Transform(m);
		Assert.IsTrue(transformed.ApproxEquals(s, 1e-5f));
		s = S;
		s.Transform(at);
		Assert.IsTrue(moved.ApproxEquals(s, 1e-5f));
	}

	[TestMethod]
	public void Distances_And_Contains()
	{
		Assert.AreEqual(3f, S.GetDistanceTo(new Vector3(1f, 2f, 8f)));
		Assert.AreEqual(0f, S.GetDistanceTo(S.Center));
		Assert.AreEqual(3f, S.GetSignedDistanceTo(new Vector3(1f, 2f, 8f)));
		Assert.AreEqual(-2f, S.GetSignedDistanceTo(S.Center));
		Assert.IsTrue(S.Contains(new Vector3(1f, 4f, 3f)));
		Assert.IsFalse(S.Contains(new Vector3(1f, 4.01f, 3f)));
	}

	[TestMethod]
	public void Intersects_PlanarPrimitives()
	{
		Assert.IsTrue(S.Intersects(new Plane(Vector3.UnitZ, new Vector3(0f, 0f, 5f))));
		Assert.IsFalse(S.Intersects(new Plane(Vector3.UnitZ, new Vector3(0f, 0f, 5.1f))));
		Assert.IsFalse(S.Intersects(new Plane(Vector3.UnitZ, new Vector3(0f, 0f, 0.9f))));
		Assert.IsTrue(S.Intersects(new HalfSpace(Vector3.UnitZ, new Vector3(0f, 0f, 1f))));
		Assert.IsTrue(S.Intersects(new HalfSpace(Vector3.UnitZ, new Vector3(0f, 0f, 100f))));
		Assert.IsFalse(S.Intersects(new HalfSpace(Vector3.UnitZ, new Vector3(0f, 0f, 0.9f))));
		Assert.IsTrue(S.Intersects(new Triangle3(new Vector3(-5f, -5f, 4f), new Vector3(5f, -5f, 4f), new Vector3(0f, 5f, 4f))));
		Assert.IsFalse(S.Intersects(new Triangle3(new Vector3(-5f, -5f, 6f), new Vector3(5f, -5f, 6f), new Vector3(0f, 5f, 6f))));
		// Triangle plane crosses the sphere, but the triangle itself is far away.
		Assert.IsFalse(S.Intersects(new Triangle3(new Vector3(10f, 10f, 3f), new Vector3(11f, 10f, 3f), new Vector3(10f, 11f, 3f))));
	}

	[TestMethod]
	public void Intersects_Volumes()
	{
		Assert.IsTrue(S.Intersects(new Sphere(new Vector3(1f, 2f, 7f), 2f)));
		Assert.IsFalse(S.Intersects(new Sphere(new Vector3(1f, 2f, 7.1f), 2f)));
		Assert.IsTrue(S.Intersects(new AxisAlignedBox(new Vector3(2f, 3f, 4f), new Vector3(5f))));
		Assert.IsFalse(S.Intersects(new AxisAlignedBox(new Vector3(3f, 4f, 5f), new Vector3(6f))));
		// The rotated box's nearest vertex lies sqrt(2) closer than its center.
		Assert.IsTrue(S.Intersects(new OrientedBox(new Vector3(4.4f, 2f, 3f), Matrix3.Rotation(Axis.Z, SingleConstants.Pi/4f), new Vector3(1f))));
		Assert.IsFalse(S.Intersects(new OrientedBox(new Vector3(4.5f, 2f, 3f), Matrix3.Rotation(Axis.Z, SingleConstants.Pi/4f), new Vector3(1f))));
		Assert.IsFalse(S.Intersects(new OrientedBox(new Vector3(4.5f, 2f, 3f), Matrix3.Identity, new Vector3(0.4f, 1f, 1f))));
		Cone cone = new Cone(new Vector3(1f, 2f, 4.5f), Vector3.UnitZ, 3f, 1f); // apex inside the sphere
		Assert.IsTrue(S.Intersects(cone));
		Assert.IsFalse(Sphere.Translate(S, new Vector3(0f, 0f, -1f)).Intersects(cone));
	}
}

[TestClass]
public sealed class EllipsoidTests
{
	private static readonly Ellipsoid E = new Ellipsoid(new Vector3(1f, 2f, 3f), Matrix3.Identity, new Vector3(1f, 2f, 3f));

	[TestMethod]
	public void Constructor_And_Properties()
	{
		Assert.AreEqual(new Vector3(1f, 2f, 3f), E.Center);
		Assert.AreEqual(Matrix3.Identity, E.Basis);
		Assert.AreEqual(new Vector3(1f, 2f, 3f), E.Radii);
		Assert.AreEqual(new Vector3(2f, 4f, 6f), E.Diameters);
		AssertEx.AreEqual(8f*MathF.PI, E.Volume);
		Assert.IsTrue(E.IsFinite);
		Assert.IsFalse(new Ellipsoid(Vector3.Zero, Matrix3.Identity, new Vector3(Single.NaN)).IsFinite);

		Ellipsoid e = E;
		e.Center = Vector3.Zero;
		e.Basis = Matrix3.Rotation(Axis.X, 1f);
		e.Radii = new Vector3(1f);
		Assert.AreEqual(new Ellipsoid(Vector3.Zero, Matrix3.Rotation(Axis.X, 1f), new Vector3(1f)), e);
		e.Diameters = new Vector3(4f);
		Assert.AreEqual(new Vector3(2f), e.Radii);
	}

	[TestMethod]
	public void Equality_ApproxEquals_HashCode()
	{
		Ellipsoid same = new Ellipsoid(new Vector3(1f, 2f, 3f), Matrix3.Identity, new Vector3(1f, 2f, 3f));
		Assert.IsTrue(E == same);
		Assert.IsFalse(E != same);
		Assert.IsTrue(E.Equals((object)same));
		Assert.AreEqual(E.GetHashCode(), same.GetHashCode());
		Ellipsoid moved = Ellipsoid.Translate(E, new Vector3(0.01f, 0f, 0f));
		Assert.IsTrue(E != moved);
		Assert.IsFalse(E.ApproxEquals(moved, 1e-3f));
		Assert.IsTrue(E.ApproxEquals(moved, 0.1f));
	}

	[TestMethod]
	public void FromSphere()
	{
		Sphere s = new Sphere(new Vector3(1f, 2f, 3f), 2f);
		Assert.AreEqual(new Ellipsoid(s.Center, Matrix3.Identity, new Vector3(2f)), Ellipsoid.FromSphere(s));

		Matrix3 m = Matrix3.ScalingRotation(new Vector3(1f, 2f, 3f), Axis.Y, 0.5f);
		Ellipsoid scaled = Ellipsoid.FromSphere(s, m);
		AssertEx.AreEqual(s.Center*m, scaled.Center);
		AssertEx.AreEqual(new Vector3(2f, 4f, 6f), scaled.Radii);
		AssertEx.IsOrthonormal(scaled.Basis);
		AssertEx.AreEqual(Matrix3.Rotation(Axis.Y, 0.5f), scaled.Basis);

		AffineTransform at = new AffineTransform(m, new Vector3(5f, 0f, 0f));
		Ellipsoid moved = Ellipsoid.FromSphere(s, at);
		AssertEx.AreEqual(Vector3.Transform(s.Center, at), moved.Center);
		AssertEx.AreEqual(new Vector3(2f, 4f, 6f), moved.Radii);

		Matrix3 r = Matrix3.Rotation(Axis.Z, 0.3f);
		Ellipsoid rotated = Ellipsoid.FromSphere(s, r, true);
		Assert.AreEqual(r, rotated.Basis);
		Assert.AreEqual(new Vector3(2f), rotated.Radii);
	}

	[TestMethod]
	public void CircumscribedSphere_UsesLargestRadius()
	{
		Assert.AreEqual(new Sphere(E.Center, 3f), E.GetCircumscribedSphere());
	}

	[TestMethod]
	public void Orthonormalize_MovesScaleIntoRadii()
	{
		Ellipsoid e = new Ellipsoid(Vector3.Zero, Matrix3.Scaling(new Vector3(2f, 3f, 4f)), new Vector3(1f, 1f, 0.5f));
		Ellipsoid o = Ellipsoid.Orthonormalize(e);
		AssertEx.AreEqual(Matrix3.Identity, o.Basis);
		AssertEx.AreEqual(new Vector3(2f, 3f, 2f), o.Radii);
		e.Orthonormalize();
		Assert.IsTrue(o.ApproxEquals(e, 1e-6f));
	}

	[TestMethod]
	public void Translate_And_Transform_PreserveSurfacePoints()
	{
		Ellipsoid t = Ellipsoid.Translate(E, new Vector3(1f, 1f, 1f));
		Assert.AreEqual(new Vector3(2f, 3f, 4f), t.Center);
		Ellipsoid e = E;
		e.Translate(new Vector3(1f, 1f, 1f));
		Assert.AreEqual(t, e);

		Vector3[] surface = { new Vector3(2f, 2f, 3f), new Vector3(1f, 4f, 3f), new Vector3(1f, 2f, 0f) };
		Matrix3 r = Matrix3.Rotation(Samples.Axes[4], 0.8f);
		AffineTransform at = new AffineTransform(r, new Vector3(-3f, 1f, 2f));
		Ellipsoid byMatrix = Ellipsoid.Transform(E, r);
		Ellipsoid byAffine = Ellipsoid.Transform(E, at);
		foreach (Vector3 p in surface)
		{
			AssertEx.AreEqual(0f, E.Evaluate(p));
			AssertEx.AreEqual(0f, byMatrix.Evaluate(p*r), 1e-5f);
			AssertEx.AreEqual(0f, byAffine.Evaluate(Vector3.Transform(p, at)), 1e-5f);
		}
		e = E;
		e.Transform(r);
		Assert.IsTrue(byMatrix.ApproxEquals(e, 1e-6f));
		e = E;
		e.Transform(at, true);
		Assert.IsTrue(byAffine.ApproxEquals(e, 1e-5f));

		// Uniform scaling grows the radii.
		Ellipsoid scaled = Ellipsoid.Transform(E, Matrix3.Scaling(2f));
		AssertEx.AreEqual(new Vector3(2f, 4f, 6f), scaled.Radii);
		AssertEx.AreEqual(2f*E.Center, scaled.Center);
	}

	[TestMethod]
	public void Evaluate_And_Contains()
	{
		Assert.AreEqual(-1f, E.Evaluate(E.Center));
		AssertEx.AreEqual(0f, E.Evaluate(new Vector3(1f, 2f, 6f)));
		AssertEx.AreEqual(3f, E.Evaluate(new Vector3(3f, 2f, 3f)));
		Assert.IsTrue(E.Contains(E.Center));
		Assert.IsTrue(E.Contains(new Vector3(1f, 3.9f, 3f)));
		Assert.IsFalse(E.Contains(new Vector3(1f, 4.1f, 3f)));
		Assert.IsFalse(E.Contains(new Vector3(1.9f, 3.9f, 3f)));

		Ellipsoid rotated = new Ellipsoid(Vector3.Zero, Matrix3.Rotation(Axis.Z, SingleConstants.HalfPi), new Vector3(3f, 1f, 1f));
		Assert.IsTrue(rotated.Contains(new Vector3(0f, 2.9f, 0f)));
		Assert.IsFalse(rotated.Contains(new Vector3(2.9f, 0f, 0f)));
	}

	[TestMethod]
	public void Intersects_Plane()
	{
		Assert.IsTrue(E.Intersects(new Plane(Vector3.UnitZ, new Vector3(0f, 0f, 5.9f))));
		Assert.IsFalse(E.Intersects(new Plane(Vector3.UnitZ, new Vector3(0f, 0f, 6.1f))));
		Assert.IsTrue(E.Intersects(new Plane(Vector3.UnitX, new Vector3(1.9f, 0f, 0f))));
		Assert.IsFalse(E.Intersects(new Plane(Vector3.UnitX, new Vector3(2.1f, 0f, 0f))));
		Ellipsoid rotated = new Ellipsoid(Vector3.Zero, Matrix3.Rotation(Axis.Z, SingleConstants.HalfPi), new Vector3(3f, 1f, 1f));
		Assert.IsTrue(rotated.Intersects(new Plane(Vector3.UnitY, new Vector3(0f, 2.9f, 0f))));
		Assert.IsFalse(rotated.Intersects(new Plane(Vector3.UnitX, new Vector3(1.1f, 0f, 0f))));
	}
}
